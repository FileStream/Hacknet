// Decompiled with JetBrains decompiler
// Type: Hacknet.LogoDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet
{
  internal class LogoDaemon : Daemon
  {
    public Color TextColor = Color.White;
    private bool hasTriedToLoadLogo = false;
    private float timeOnPage = 0.0f;
    private const string FileName = "DisplayText.txt";
    private string LogoImagePath;
    private bool showsTitle;
    public string BodyText;
    private Texture2D LoadedLogo;
    private TrailLoadingSpinnerEffect spinner;

    public LogoDaemon(Computer c, OS os, string name, bool showsTitle, string LogoImagePath)
      : base(c, name, os)
    {
      this.isListed = true;
      this.showsTitle = showsTitle;
      this.LogoImagePath = LogoImagePath;
      this.name = name;
      this.spinner = new TrailLoadingSpinnerEffect(os);
    }

    public override void initFiles()
    {
      base.initFiles();
      if (string.IsNullOrWhiteSpace(this.BodyText))
        return;
      this.comp.files.root.searchForFolder("sys").files.Add(new FileEntry(this.BodyText, "DisplayText.txt"));
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      this.timeOnPage = 0.0f;
      if (this.LoadedLogo == null && !this.hasTriedToLoadLogo && !string.IsNullOrWhiteSpace(this.LogoImagePath))
        Utils.LoadImageFromContentOrExtension(this.LogoImagePath, this.os.content, (Action<Texture2D>) (loadedTex =>
        {
          this.hasTriedToLoadLogo = true;
          this.LoadedLogo = loadedTex;
        }));
      FileEntry fileEntry = this.comp.files.root.searchForFolder("sys").searchForFile("DisplayText.txt");
      if (fileEntry == null)
        return;
      this.BodyText = fileEntry.data;
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      bounds = Utils.InsetRectangle(bounds, 1);
      sb.Draw(Utils.white, bounds, Color.Black * 0.4f);
      Rectangle dest = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 8);
      Rectangle rectangle = new Rectangle(bounds.X, bounds.Y + dest.Height, bounds.Width, bounds.Height / 8 * 7);
      if (!this.showsTitle)
      {
        rectangle.Y -= dest.Height;
        rectangle.Height += dest.Height;
        rectangle.Y += 6;
        rectangle.Height -= 6;
      }
      int num1 = 40;
      rectangle.Height -= num1;
      if (!string.IsNullOrWhiteSpace(this.BodyText))
      {
        string[] strArray = this.BodyText.Trim().Split(Utils.robustNewlineDelim, StringSplitOptions.None);
        int num2 = (int) GuiData.smallfont.MeasureString("W").Y + 2;
        int num3 = 6;
        int num4 = num2 * strArray.Length + num3 * 2;
        rectangle.Height -= num4;
        int num5 = bounds.Y + bounds.Height - num3 - num2 + 1 - num1;
        for (int index = strArray.Length - 1; index >= 0; --index)
        {
          TextItem.doCenteredFontLabel(new Rectangle(bounds.X, num5 + 1, bounds.Width, num2 - 1), strArray[index], GuiData.smallfont, this.TextColor, false);
          num5 -= num2;
        }
      }
      if (this.LoadedLogo == null)
      {
        this.timeOnPage += (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
        this.spinner.Draw(rectangle, sb, 1f, (float) (1.0 - (double) this.timeOnPage * 0.400000005960464), 0.0f, new Color?(this.TextColor));
      }
      else
        Utils.DrawSpriteAspectCorrect(rectangle, sb, this.LoadedLogo, Color.White, false);
      if (this.showsTitle)
        TextItem.doCenteredFontLabel(dest, this.name, GuiData.font, this.TextColor, false);
      int width = bounds.Width / 2;
      int height = 26;
      if (!Button.doButton(22928201, bounds.X + (bounds.Width - width) / 2, bounds.Y + bounds.Height - (num1 - height) / 2 - height, width, height, LocaleTerms.Loc("Proceed"), new Color?(Color.Black)))
        return;
      this.os.display.command = "connect";
    }

    public override string getSaveString()
    {
      return string.Format("<LogoDaemon LogoImagePath=\"{0}\" ShowsTitle=\"{1}\" TextColor=\"{2}\" Name=\"{3}\" />", (object) this.LogoImagePath, this.showsTitle ? (object) "true" : (object) "false", (object) Utils.convertColorToParseableString(this.TextColor), (object) this.name);
    }
  }
}
