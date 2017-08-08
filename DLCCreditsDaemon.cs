// Decompiled with JetBrains decompiler
// Type: Hacknet.DLCCreditsDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;

namespace Hacknet
{
  internal class DLCCreditsDaemon : Daemon
  {
    private bool showingCredits = false;
    private bool isInResetSequence = false;
    private float timeInReset = 0.0f;
    private float timeInCredits = 0.0f;
    private bool hasCuedBuildup = false;
    private bool hasCuedFinaleSong = false;
    public string OverrideTitle = (string) null;
    public string OverrideButtonText = (string) null;
    public string ConditionalActionsToLoadOnButtonPress = (string) null;
    private const float ResetSequenceTime = 5f;
    private string[] CreditsData;
    private SoundEffect spindown;
    private SoundEffect spindownImpact;
    private SoundEffect buildup;

    public DLCCreditsDaemon(Computer c, OS os)
      : base(c, LocaleTerms.Loc("Labyrinths project"), os)
    {
      this.LoadSounds();
    }

    public DLCCreditsDaemon(Computer c, OS os, string overrideTitle, string overrideButtonText)
      : base(c, overrideTitle, os)
    {
      this.LoadSounds();
      this.OverrideTitle = overrideTitle;
      this.OverrideButtonText = overrideButtonText;
    }

    private void LoadSounds()
    {
      if (DLC1SessionUpgrader.HasDLC1Installed)
      {
        this.spindown = this.os.content.Load<SoundEffect>("DLC/SFX/Spindown");
        this.spindownImpact = this.os.content.Load<SoundEffect>("DLC/SFX/RecoverImpact");
        this.buildup = this.os.content.Load<SoundEffect>("DLC/SFX/Kilmer_Woosh");
      }
      else
      {
        this.spindown = (SoundEffect) null;
        this.spindownImpact = this.os.content.Load<SoundEffect>("SFX/MeltImpact");
        this.buildup = this.os.content.Load<SoundEffect>("SFX/BrightFlash");
      }
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      FileEntry fileEntry = this.comp.files.root.searchForFolder("home").searchForFile("CreditsData.txt");
      if (fileEntry == null)
        this.CreditsData = new string[2]
        {
          "- " + LocaleTerms.Loc("Critical Error") + " -",
          LocaleTerms.Loc("Datafile not found")
        };
      else
        this.CreditsData = fileEntry.data.Split(Utils.robustNewlineDelim, StringSplitOptions.None);
      if (!this.os.Flags.HasFlag("dlc_complete"))
        return;
      this.showingCredits = true;
    }

    private void EndDLC()
    {
      DLC1SessionUpgrader.EndDLCSection((object) this.os);
    }

    private void AddRadialMailLine()
    {
      SFX.AddRadialLine(this.os.mailicon.pos + new Vector2(20f, 10f), 3.141593f + Utils.rand(3.141593f), 600f + Utils.randm(300f), 800f, 500f, 200f + Utils.randm(400f), 0.35f, Utils.AddativeWhite * Utils.randm(1f), 3f, false);
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      bounds = Utils.InsetRectangle(bounds, 1);
      int num1 = 90;
      int height1 = 30;
      TextItem.doCenteredFontLabel(new Rectangle(bounds.X, bounds.Y + height1, bounds.Width, num1 - height1), this.OverrideTitle == null ? LocaleTerms.Loc("Labyrinths Project") : this.OverrideTitle, GuiData.font, Color.White, false);
      Rectangle rectangle = new Rectangle(bounds.X, bounds.Y + num1 + height1, bounds.Width, bounds.Height - (num1 + 2 * height1));
      if (this.showingCredits)
      {
        this.timeInCredits += (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
        float timeInCredits = this.timeInCredits;
        float num2 = ((double) timeInCredits > 5.0 ? timeInCredits - 4f : Utils.CubicInCurve(timeInCredits / 5f)) % 165f;
        Utils.FillEverywhereExcept(rectangle, Utils.GetFullscreen(), sb, Color.Black * 0.5f);
        float num3 = 20f;
        float num4 = (float) (rectangle.Y - height1 + rectangle.Height) - num2 * num3;
        for (int index = 0; index < this.CreditsData.Length; ++index)
        {
          int height2 = 22;
          SpriteFont font = GuiData.smallfont;
          Color color = Color.LightGray * 0.9f;
          string text = this.CreditsData[index];
          if (text.StartsWith("%"))
          {
            text = text.Substring(1);
            height2 = 45;
            font = GuiData.font;
            color = Utils.AddativeWhite * 0.9f;
          }
          else if (text.StartsWith("^"))
          {
            text = text.Substring(1);
            height2 = 30;
            font = GuiData.font;
            color = Color.White;
          }
          if ((double) num4 >= (double) (rectangle.Y - height1))
            TextItem.doCenteredFontLabel(new Rectangle(rectangle.X, (int) num4, rectangle.Width, height2), text, font, color, false);
          num4 += (float) (height2 + 2);
          if ((double) num4 > (double) (rectangle.Y + rectangle.Height))
            break;
        }
        if ((double) this.timeInCredits > 40.0 && Button.doButton(18394902, rectangle.X + rectangle.Width / 4, rectangle.Y + rectangle.Height - 23, rectangle.Width / 2, 20, LocaleTerms.Loc("Proceed"), new Color?((double) this.timeInCredits > 65.0 ? this.os.highlightColor : Color.Black)))
          this.os.display.command = "connect";
      }
      else if (this.isInResetSequence)
      {
        this.timeInReset += (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
        if ((double) this.timeInReset >= 5.0)
        {
          this.showingCredits = true;
          PostProcessor.EndingSequenceFlashOutActive = false;
          PostProcessor.EndingSequenceFlashOutPercentageComplete = 0.0f;
          if (!Settings.IsInExtensionMode)
          {
            this.EndDLC();
            if (!this.hasCuedFinaleSong)
            {
              this.os.delayer.Post(ActionDelayer.Wait(2.0), (Action) (() => MusicManager.playSongImmediatley("DLC\\Music\\DreamHead")));
              this.hasCuedFinaleSong = true;
            }
            MediaPlayer.IsRepeating = true;
            for (int index = 0; index < 9; ++index)
              this.os.delayer.Post(ActionDelayer.Wait((double) index / 7.0), (Action) (() => SFX.addCircle(this.os.mailicon.pos + new Vector2(20f, 6f), Utils.AddativeWhite * 0.8f, 400f)));
          }
        }
        else
        {
          float num2 = Math.Min(this.timeInReset, 1f);
          Utils.FillEverywhereExcept(rectangle, Utils.GetFullscreen(), sb, Color.Black * 0.5f * num2);
          PatternDrawer.draw(rectangle, 0.3f, Color.Transparent, Color.Black * 0.7f, sb, PatternDrawer.binaryTile);
          this.AddRadialMailLine();
          PostProcessor.EndingSequenceFlashOutActive = true;
          PostProcessor.EndingSequenceFlashOutPercentageComplete = num2;
          if (!this.hasCuedBuildup && (double) this.timeInReset > 2.8)
          {
            this.buildup.Play();
            this.hasCuedBuildup = true;
          }
          TextItem.doCenteredFontLabel(rectangle, LocaleTerms.Loc("Disabling..."), GuiData.font, Color.White, false);
        }
      }
      else
      {
        PatternDrawer.draw(rectangle, 0.3f, Color.Transparent, Color.Black * 0.7f, sb, PatternDrawer.binaryTile);
        string text = this.OverrideButtonText == null ? LocaleTerms.Loc("Disable Agent Monitoring") : this.OverrideButtonText;
        bool flag = this.OverrideButtonText != null;
        if (Button.doButton(38101920, rectangle.X + 50, rectangle.Y + rectangle.Height / 2 - 13, rectangle.Width - 100, 26, text, new Color?(this.os.highlightColor)))
        {
          if (!flag)
          {
            this.isInResetSequence = true;
            this.timeInReset = 0.0f;
            if (MusicManager.currentSongName == "DLC/Music/RemiDrone")
              MusicManager.stop();
            this.spindownImpact.Play();
            if (this.spindown != null)
              this.os.delayer.Post(ActionDelayer.Wait(1.1), (Action) (() => this.spindown.Play()));
            DLC1SessionUpgrader.ReDsicoverAllVisibleNodesInOSCache((object) this.os);
          }
          else
          {
            this.isInResetSequence = false;
            this.showingCredits = true;
            if (this.ConditionalActionsToLoadOnButtonPress != null)
              RunnableConditionalActions.LoadIntoOS(this.ConditionalActionsToLoadOnButtonPress, (object) this.os);
          }
        }
      }
      Rectangle destinationRectangle = new Rectangle(bounds.X, bounds.Y + num1, bounds.Width, height1);
      sb.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray);
      destinationRectangle.Y = bounds.Y + num1 + height1 + rectangle.Height;
      sb.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray);
    }

    public override string getSaveString()
    {
      string str = "";
      if (this.OverrideTitle != null)
        str += string.Format("Title=\"{0}\" ", (object) this.OverrideTitle);
      if (this.OverrideButtonText != null)
        str += string.Format("Button=\"{0}\" ", (object) this.OverrideButtonText);
      if (this.ConditionalActionsToLoadOnButtonPress != null)
        str += string.Format("Action=\"{0}\" ", (object) this.ConditionalActionsToLoadOnButtonPress);
      return "<DLCCredits " + str + "/>";
    }
  }
}
