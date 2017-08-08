// Decompiled with JetBrains decompiler
// Type: Hacknet.MarkovTextDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.DLC.MarkovTextGenerator;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class MarkovTextDaemon : Daemon
  {
    private bool CorpusBeingLoaded = false;
    private bool SentenceBeingGenerated = false;
    private bool LastSentenceWasError = false;
    private float loadProgress = 0.0f;
    private int DisplayWidth = 300;
    private List<string> GeneratedSentences = new List<string>();
    private Corpus corpus;
    public string corpusFolderPath;
    private ScrollableTextRegion TextDisplay;

    public MarkovTextDaemon(Computer c, OS os, string name, string corpusLoadPath)
      : base(c, name, os)
    {
      this.corpusFolderPath = corpusLoadPath;
      this.TextDisplay = new ScrollableTextRegion(GuiData.spriteBatch.GraphicsDevice);
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      this.GeneratedSentences.Clear();
      this.TextDisplay.UpdateScroll(0.0f);
      if (string.IsNullOrWhiteSpace(this.corpusFolderPath))
        throw new NullReferenceException("No corpus folder path found");
      this.CorpusBeingLoaded = true;
      this.loadProgress = 0.0f;
      CorpusGenerator.GenerateCorpusFromFolderThreaded(Utils.GetFileLoadPrefix() + this.corpusFolderPath, new Action<Corpus>(this.CorpusLoaded), new Action<float, string>(this.LoadProgressUpdated));
    }

    private void CorpusLoaded(Corpus c)
    {
      this.CorpusBeingLoaded = false;
      if (c == null)
        return;
      this.loadProgress = 1f;
      this.corpus = c;
    }

    private void LoadProgressUpdated(float progress, string progressMessage)
    {
      this.loadProgress = progress;
    }

    private void AddNewSentence(string sentence)
    {
      if (!string.IsNullOrWhiteSpace(sentence))
      {
        this.GeneratedSentences.AddRange((IEnumerable<string>) Utils.SuperSmartTwimForWidth(sentence, this.DisplayWidth, GuiData.smallfont).Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries));
        this.SentenceBeingGenerated = false;
        this.LastSentenceWasError = false;
      }
      else
        this.LastSentenceWasError = true;
    }

    private void PrepateToClearCorpusOnNavigateAway()
    {
      this.os.delayer.Post(ActionDelayer.Wait(this.os.lastGameTime.ElapsedGameTime.TotalSeconds * 1.999), (Action) (() =>
      {
        if (!(this.os.display.command != this.name))
          return;
        this.corpus = (Corpus) null;
      }));
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      this.PrepateToClearCorpusOnNavigateAway();
      Rectangle dest = Utils.InsetRectangle(bounds, 2);
      this.DisplayWidth = dest.Width;
      if (this.corpus == null && this.CorpusBeingLoaded)
      {
        this.DrawLoading(dest, sb);
      }
      else
      {
        if (Button.doButton(832192223, bounds.X + 20, bounds.Y + 20, bounds.Width / 3, 20, "Generate", new Color?()) && !this.SentenceBeingGenerated)
        {
          this.SentenceBeingGenerated = true;
          this.corpus.GenerateSentenceThreaded(new Action<string>(this.AddNewSentence));
        }
        dest.Y += 40;
        dest.Height -= 40;
        this.TextDisplay.Draw(dest, this.GeneratedSentences, sb, Color.White);
      }
    }

    private void DrawLoading(Rectangle dest, SpriteBatch sb)
    {
      TextItem.doCenteredFontLabel(dest, "Loading...", GuiData.font, Color.White, false);
      Rectangle rectangle = new Rectangle(dest.X, dest.Y + 30, dest.Width, dest.Height);
      TextItem.doCenteredFontLabel(dest, this.loadProgress.ToString("00.00") + "%", GuiData.font, Color.White, false);
    }

    public override string getSaveString()
    {
      return "<MarkovTextDaemon Name=\"" + this.name + "\" SourceFilesContentFolder=\"" + this.corpusFolderPath + "\" />";
    }
  }
}
