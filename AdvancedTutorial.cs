// Decompiled with JetBrains decompiler
// Type: Hacknet.AdvancedTutorial
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class AdvancedTutorial : ExeModule
  {
    private static string loadedLocale = "en-us";
    private string[] hintButtonText = (string[]) null;
    private float hintTextFadeTimer = 0.0f;
    private string[] hintButtonDelimiter = new string[1]{ "|" };
    public bool CanActivateFirstStep = true;
    private static List<string> commandSequence;
    private static List<string[]> altCommandequence;
    private static List<string> feedbackSequence;
    private int state;
    private string lastCommand;
    private string[] renderText;
    private float flashTimer;
    private SoundEffect startSound;
    private SoundEffect advanceFlash;
    private List<Action> stepCompletionSequence;
    private NodeBounceEffect bounceEffect;

    public AdvancedTutorial(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.state = 0;
      this.lastCommand = "";
      this.ramCost = 500;
      this.IdentifierName = "Tutorial v16.2";
      this.flashTimer = 1f;
      this.startSound = this.os.content.Load<SoundEffect>("SFX/DoomShock");
      this.advanceFlash = this.os.content.Load<SoundEffect>("SFX/BrightFlash");
      AdvancedTutorial.commandSequence = (List<string>) null;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.bounceEffect = new NodeBounceEffect()
      {
        NodeHitDelay = 0.02f,
        TimeBetweenBounces = 0.2f
      };
      string[] strArray = LocalizedFileLoader.Read("Content/Post/AdvancedTutorialData.txt").Replace("\r\n", "\n").Split(new string[1]{ "\n\n%&%&%&%\n" }, StringSplitOptions.None);
      if (AdvancedTutorial.feedbackSequence != null && AdvancedTutorial.loadedLocale != Settings.ActiveLocale)
        AdvancedTutorial.feedbackSequence = (List<string>) null;
      AdvancedTutorial.loadedLocale = Settings.ActiveLocale ?? "";
      if (AdvancedTutorial.commandSequence == null)
      {
        AdvancedTutorial.altCommandequence = new List<string[]>();
        for (int index = 0; index < 15; ++index)
          AdvancedTutorial.altCommandequence.Add(new string[0]);
        AdvancedTutorial.commandSequence = new List<string>();
        AdvancedTutorial.commandSequence.Add("&#*@(&#@(&#@&)@&#)(@&)@#");
        AdvancedTutorial.commandSequence.Add("connect " + this.os.thisComputer.ip);
        AdvancedTutorial.commandSequence.Add("scan");
        AdvancedTutorial.commandSequence.Add("dc");
        AdvancedTutorial.altCommandequence[3] = new string[1]
        {
          "disconnect"
        };
        AdvancedTutorial.commandSequence.Add("connect");
        AdvancedTutorial.commandSequence.Add("probe");
        AdvancedTutorial.altCommandequence[5] = new string[1]
        {
          "nmap"
        };
        AdvancedTutorial.commandSequence.Add("porthack");
        AdvancedTutorial.commandSequence.Add("scan");
        AdvancedTutorial.commandSequence.Add("ls");
        AdvancedTutorial.altCommandequence[8] = new string[1]
        {
          "dir"
        };
        AdvancedTutorial.commandSequence.Add("cd bin");
        AdvancedTutorial.altCommandequence[9] = new string[3]
        {
          "cd ../bin",
          "cd ../../bin",
          "cd /bin"
        };
        AdvancedTutorial.commandSequence.Add("cat config.txt");
        AdvancedTutorial.altCommandequence[10] = new string[1]
        {
          "less config.txt"
        };
        AdvancedTutorial.commandSequence.Add("cd ..");
        AdvancedTutorial.altCommandequence[11] = new string[2]
        {
          "cd..",
          "cd /"
        };
        AdvancedTutorial.commandSequence.Add("cd log");
        AdvancedTutorial.altCommandequence[12] = new string[3]
        {
          "cd ../log",
          "cd ../../log",
          "cd /log"
        };
        AdvancedTutorial.commandSequence.Add("rm *");
        AdvancedTutorial.commandSequence.Add("dc");
        AdvancedTutorial.altCommandequence[14] = new string[1]
        {
          "disconnect"
        };
      }
      if (AdvancedTutorial.feedbackSequence == null)
      {
        AdvancedTutorial.feedbackSequence = new List<string>();
        AdvancedTutorial.feedbackSequence.Add(strArray[0]);
        AdvancedTutorial.feedbackSequence.Add(strArray[1]);
        AdvancedTutorial.feedbackSequence.Add(strArray[2]);
        AdvancedTutorial.feedbackSequence.Add(strArray[3]);
        AdvancedTutorial.feedbackSequence.Add(strArray[4]);
        AdvancedTutorial.feedbackSequence.Add(strArray[5]);
        AdvancedTutorial.feedbackSequence.Add(strArray[6]);
        AdvancedTutorial.feedbackSequence.Add(strArray[7]);
        AdvancedTutorial.feedbackSequence.Add(strArray[8]);
        AdvancedTutorial.feedbackSequence.Add(strArray[9]);
        AdvancedTutorial.feedbackSequence.Add(strArray[10]);
        AdvancedTutorial.feedbackSequence.Add(strArray[11]);
        AdvancedTutorial.feedbackSequence.Add(strArray[12]);
        AdvancedTutorial.feedbackSequence.Add(strArray[13]);
        AdvancedTutorial.feedbackSequence.Add(strArray[14]);
        AdvancedTutorial.feedbackSequence.Add(strArray[15]);
      }
      this.stepCompletionSequence = new List<Action>();
      for (int index = 0; index < AdvancedTutorial.feedbackSequence.Count; ++index)
        this.stepCompletionSequence.Add((Action) null);
      this.stepCompletionSequence[1] = (Action) (() =>
      {
        this.os.netMap.visible = true;
        this.os.netMap.inputLocked = false;
      });
      this.stepCompletionSequence[2] = (Action) (() =>
      {
        this.os.display.visible = true;
        this.os.display.inputLocked = false;
      });
      this.stepCompletionSequence[3] = (Action) (() => this.os.netMap.inputLocked = true);
      this.stepCompletionSequence[4] = (Action) (() => this.os.netMap.inputLocked = false);
      this.stepCompletionSequence[5] = (Action) (() =>
      {
        this.os.display.inputLocked = true;
        this.os.ram.inputLocked = true;
        this.os.netMap.inputLocked = true;
        this.os.terminal.visible = true;
        this.os.terminal.inputLocked = false;
        this.os.terminal.clearCurrentLine();
      });
      this.stepCompletionSequence[8] = (Action) (() =>
      {
        this.os.display.inputLocked = false;
        this.os.ram.inputLocked = false;
      });
      this.state = 0;
      this.getRenderText();
    }

    public override void Update(float t)
    {
      base.Update(t);
      string lastRunCommand = this.os.terminal.getLastRunCommand();
      if (this.lastCommand != lastRunCommand)
      {
        this.lastCommand = lastRunCommand;
        this.parseCommand();
      }
      else if (GuiData.getKeyboadState().IsKeyDown(Keys.F8))
        this.lastCommand = "";
      this.flashTimer -= t;
      this.flashTimer = Math.Max(this.flashTimer, 0.0f);
      this.bounceEffect.Update(t, (Action<Vector2>) (nodePos =>
      {
        this.bounceEffect.NodeHitDelay = Math.Max(0.0f, Utils.randm(0.5f) - 0.2f);
        this.bounceEffect.TimeBetweenBounces = 0.15f + Utils.randm(0.7f);
      }));
      this.hintTextFadeTimer = Math.Max(0.0f, this.hintTextFadeTimer - t);
    }

    public void parseCommand()
    {
      try
      {
        if (this.state < AdvancedTutorial.commandSequence.Count)
        {
          bool flag = this.lastCommand.ToLower().StartsWith(AdvancedTutorial.commandSequence[this.state]);
          if (!flag)
          {
            for (int index = 0; index < AdvancedTutorial.altCommandequence[this.state].Length; ++index)
              flag |= this.lastCommand.ToLower().StartsWith(AdvancedTutorial.altCommandequence[this.state][index]);
          }
          if (!flag)
            return;
          this.advanceState();
        }
        else
          this.lastCommand = (string) null;
      }
      catch (Exception ex)
      {
        this.lastCommand = "";
      }
    }

    private void advanceState()
    {
      ++this.state;
      this.getRenderText();
      if (this.state > 0)
        this.printCurrentCommandToTerminal();
      this.flashTimer = 1f;
      if (this.stepCompletionSequence[this.state] != null)
        this.stepCompletionSequence[this.state]();
      if (Settings.soundDisabled || MusicManager.isMuted || this.state <= 1)
        return;
      this.advanceFlash.Play(0.6f, 1f, 1f);
    }

    public void printCurrentCommandToTerminal()
    {
      this.os.terminal.writeLine("\n--------------------------------------------------");
      this.os.terminal.writeLine(" ");
      for (int index = 0; index < this.renderText.Length; ++index)
      {
        if (!this.renderText[index].Contains("<#") && !this.renderText[index].Contains("#>"))
          this.os.terminal.writeLine(this.renderText[index]);
      }
      this.os.terminal.writeLine(" ");
      this.os.terminal.writeLine("--------------------------------------------------\n");
    }

    public void getRenderText()
    {
      string data = AdvancedTutorial.feedbackSequence[this.state];
      char[] chArray = new char[1]{ '\n' };
      string[] strArray = data.Split(this.hintButtonDelimiter, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length > 1)
      {
        this.hintButtonText = DisplayModule.cleanSplitForWidth(strArray[1], 178).Split(chArray);
        data = strArray[0];
        this.hintTextFadeTimer = 0.0f;
      }
      else
        this.hintButtonText = (string[]) null;
      this.renderText = Utils.SuperSmartTwimForWidth(LocalizedFileLoader.SafeFilterString(data), this.bounds.Width - 10, GuiData.tinyfont).Split(chArray);
    }

    public override void Killed()
    {
      base.Killed();
      if (!this.os.multiplayer && this.os.initShowsTutorial)
      {
        this.os.currentMission.sendEmail(this.os);
        this.os.Flags.AddFlag("TutorialComplete");
        this.os.initShowsTutorial = false;
      }
      if (this.os.hubServerAlertsIcon != null)
        this.os.hubServerAlertsIcon.IsEnabled = true;
      this.os.mailicon.isEnabled = true;
      this.os.terminal.visible = true;
      this.os.terminal.inputLocked = false;
      this.os.netMap.visible = true;
      this.os.netMap.inputLocked = false;
      this.os.ram.visible = true;
      this.os.ram.inputLocked = false;
      this.os.display.visible = true;
      this.os.display.inputLocked = false;
      if (this.state >= AdvancedTutorial.feedbackSequence.Count - 1)
        return;
      AchievementsManager.Unlock("kill_tutorial", false);
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      Rectangle bounds1 = this.bounds;
      ++bounds1.X;
      ++bounds1.Y;
      bounds1.Width -= 2;
      bounds1.Height -= 2;
      bounds1.Height -= Module.PANEL_HEIGHT;
      bounds1.Y += Module.PANEL_HEIGHT;
      PatternDrawer.draw(bounds1, 1f, this.os.highlightColor * 0.2f, this.os.highlightColor, this.spriteBatch);
      bounds1.X += 2;
      bounds1.Y += 2;
      bounds1.Width -= 4;
      bounds1.Height -= 4;
      this.spriteBatch.Draw(Utils.white, bounds1, this.os.darkBackgroundColor * 0.99f);
      this.drawTarget("app:");
      int height1 = 260;
      Rectangle bounds2 = new Rectangle(this.bounds.X + 1, this.bounds.Y + this.bounds.Height - height1, this.bounds.Width - 2, height1);
      this.bounceEffect.Draw(this.spriteBatch, bounds2, Utils.AdditivizeColor(this.os.highlightColor) * 0.35f, Utils.AddativeWhite * 0.5f);
      int height2 = 210;
      Rectangle destinationRectangle1 = new Rectangle(bounds2.X, bounds2.Y + (height1 - height2), bounds2.Width, height2);
      this.spriteBatch.Draw(Utils.gradient, destinationRectangle1, new Rectangle?(), this.os.darkBackgroundColor, 0.0f, Vector2.Zero, SpriteEffects.FlipVertically, 0.7f);
      Rectangle destinationRectangle2 = destinationRectangle1;
      destinationRectangle2.Y = bounds2.Y;
      destinationRectangle2.Height = height1 - height2;
      this.spriteBatch.Draw(Utils.white, destinationRectangle2, this.os.darkBackgroundColor);
      this.spriteBatch.Draw(Utils.white, bounds1, this.os.highlightColor * this.flashTimer);
      this.spriteBatch.DrawString(GuiData.font, Utils.FlipRandomChars("Tutorial", 0.024), new Vector2((float) (this.bounds.X + 5), (float) (this.bounds.Y + 22)), this.os.subtleTextColor * 0.6f);
      this.spriteBatch.DrawString(GuiData.font, "Tutorial", new Vector2((float) (this.bounds.X + 5), (float) (this.bounds.Y + 22)), this.os.subtleTextColor);
      float charHeight = GuiData.ActiveFontConfig.tinyFontCharHeight + 1f;
      Vector2 dpos = this.RenderText(this.renderText, new Vector2((float) (this.bounds.X + 5), (float) (this.bounds.Y + 67)), charHeight, 1f);
      dpos.Y += charHeight;
      if (this.state == 0 && this.CanActivateFirstStep)
      {
        if (!Button.doButton(2933201, this.bounds.X + 10, (int) ((double) dpos.Y + 20.0), this.bounds.Width - 20, 30, LocaleTerms.Loc("Continue"), new Color?()) && (this.os.terminal.visible || !Utils.keyPressed(GuiData.lastInput, Keys.Enter, new PlayerIndex?())))
          return;
        this.advanceState();
        if (!Settings.soundDisabled)
        {
          this.startSound.Play(1f, 0.0f, 0.0f);
          this.startSound.Play(1f, 0.0f, 0.0f);
        }
      }
      else
      {
        if (this.hintButtonText == null)
          return;
        if (Button.doButton(2933202, this.bounds.X + 10, (int) ((double) dpos.Y + 6.0), this.bounds.Width - 20, 20, LocaleTerms.Loc("Hint"), new Color?()))
          this.hintTextFadeTimer = 9f;
        dpos.Y += 30f;
        dpos.X += 10f;
        float opacityMod = Math.Min(1f, this.hintTextFadeTimer / 3f);
        string str = "";
        for (int index = 0; index < this.hintButtonText.Length; ++index)
          str = str + this.hintButtonText[index] + "\n";
        string[] stringData = Utils.SuperSmartTwimForWidth(str.Trim(), this.bounds.Width, GuiData.tinyfont).Split(Utils.robustNewlineDelim, StringSplitOptions.None);
        Rectangle destinationRectangle3 = new Rectangle((int) dpos.X, (int) dpos.Y, this.bounds.Width, (int) ((double) charHeight * (double) stringData.Length));
        this.spriteBatch.Draw(Utils.white, destinationRectangle3, Color.Black * 0.5f * opacityMod);
        this.RenderText(stringData, dpos, charHeight, opacityMod);
      }
    }

    private Vector2 RenderText(string[] stringData, Vector2 dpos, float charHeight, float opacityMod = 1f)
    {
      bool flag = false;
      for (int index = 0; index < stringData.Length; ++index)
      {
        string text = stringData[index].Replace("\r", "");
        if (text.Length > 0)
        {
          if (text.Trim() == "<#")
            flag = true;
          else if (text.Trim() == "#>")
          {
            flag = false;
          }
          else
          {
            this.spriteBatch.DrawString(GuiData.tinyfont, text, dpos, (flag ? this.os.highlightColor : Color.White) * opacityMod);
            dpos.Y += charHeight;
          }
        }
      }
      return dpos;
    }

    private Vector2 RenderTextOld(string[] stringData, Vector2 dpos, float charHeight, float opacityMod = 1f)
    {
      for (int index = 0; index < stringData.Length; ++index)
      {
        string text = stringData[index];
        if (text.Length > 0)
        {
          bool flag = false;
          if ((int) text[0] == 35)
          {
            text = text.Substring(1, text.Length - 2);
            flag = true;
          }
          this.spriteBatch.DrawString(GuiData.tinyfont, text, dpos, (flag ? this.os.highlightColor : Color.White) * opacityMod);
          dpos.Y += charHeight;
        }
      }
      return dpos;
    }
  }
}
