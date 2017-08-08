// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.TraceDangerSequence
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.Effects
{
  internal class TraceDangerSequence
  {
    private static Color DarkRed = new Color(105, 0, 0, 200);
    private static Color BackgroundRed = new Color(120, 0, 0);
    public bool IsActive = false;
    public bool PreventOSRendering = false;
    private float onBeatFlashTimer = 0.0f;
    private float timeThisState = 0.0f;
    private float percentComplete = 0.0f;
    private TraceDangerSequence.TraceDangerState state = TraceDangerSequence.TraceDangerState.WarningScrenIntro;
    private string oldSong = (string) null;
    private bool warningScreenIsActivating = false;
    private const float WARNING_INTRO_TIME = 1f;
    private const float WARNING_EXIT_TIME = 13.9f;
    private const float DISCONNECT_REBOOT_TIME = 10f;
    private const float COUNTDOWN_TIME = 130f;
    private const float FLASH_FREQUENCY = 1.937667f;
    private SpriteFont titleFont;
    private SpriteFont bodyFont;
    private Rectangle fullscreen;
    private SpriteBatch spriteBatch;
    private SpriteBatch scaleupSpriteBatch;
    private OS os;
    private SoundEffect spinDownSound;
    private SoundEffect spinUpSound;
    private SoundEffect impactSound;

    public TraceDangerSequence(ContentManager content, SpriteBatch sb, Rectangle fullscreenRect, OS os)
    {
      this.titleFont = GuiData.titlefont;
      this.bodyFont = GuiData.font;
      this.fullscreen = fullscreenRect;
      this.spriteBatch = sb;
      this.scaleupSpriteBatch = new SpriteBatch(sb.GraphicsDevice);
      this.os = os;
      this.spinDownSound = os.content.Load<SoundEffect>("Music/Ambient/spiral_gauge_down");
      this.spinUpSound = os.content.Load<SoundEffect>("Music/Ambient/spiral_gauge_up");
      this.impactSound = os.content.Load<SoundEffect>("SFX/MeltImpact");
    }

    public void BeginTraceDangerSequence()
    {
      this.timeThisState = 0.0f;
      this.state = TraceDangerSequence.TraceDangerState.WarningScrenIntro;
      this.IsActive = true;
      this.PreventOSRendering = true;
      this.oldSong = MusicManager.currentSongName;
      this.os.execute("dc");
      this.os.display.command = "";
      this.os.terminal.inputLocked = true;
      MusicManager.playSongImmediatley("Music/Ambient/dark_drone_008");
      this.spinDownSound.Play();
    }

    public void CompleteIPResetSucsesfully()
    {
      this.timeThisState = 0.0f;
      this.state = TraceDangerSequence.TraceDangerState.DisconnectedReboot;
      this.IsActive = true;
      this.PreventOSRendering = true;
      PostProcessor.dangerModeEnabled = false;
      PostProcessor.dangerModePercentComplete = 0.0f;
      MusicManager.stop();
      this.spinDownSound.Play(1f, -0.6f, 0.0f);
      this.impactSound.Play();
    }

    public void CancelTraceDangerSequence()
    {
      this.timeThisState = 0.0f;
      this.state = TraceDangerSequence.TraceDangerState.WarningScrenIntro;
      this.IsActive = false;
      this.PreventOSRendering = false;
      PostProcessor.dangerModeEnabled = false;
      MusicManager.stop();
    }

    public void Update(float t)
    {
      this.timeThisState += t;
      float num = float.MaxValue;
      PostProcessor.dangerModeEnabled = false;
      switch (this.state)
      {
        case TraceDangerSequence.TraceDangerState.WarningScrenIntro:
          num = 1f;
          if ((double) this.timeThisState > (double) num)
          {
            this.timeThisState = 0.0f;
            this.state = TraceDangerSequence.TraceDangerState.WarningScreen;
            this.warningScreenIsActivating = false;
            break;
          }
          break;
        case TraceDangerSequence.TraceDangerState.WarningScreenExiting:
          num = 13.9f;
          if ((double) this.timeThisState > (double) num)
          {
            this.timeThisState = 0.0f;
            this.state = TraceDangerSequence.TraceDangerState.Countdown;
            this.os.display.visible = true;
            this.os.netMap.visible = true;
            this.os.terminal.visible = true;
            this.os.ram.visible = true;
            break;
          }
          break;
        case TraceDangerSequence.TraceDangerState.Countdown:
          num = 130f;
          if ((double) this.timeThisState > (double) num)
          {
            this.timeThisState = 0.0f;
            this.state = TraceDangerSequence.TraceDangerState.Gameover;
            this.CancelTraceDangerSequence();
            Game1.getSingleton().Exit();
          }
          if (((double) this.os.timer - (double) this.onBeatFlashTimer) % 1.93766665458679 < 0.0500000007450581)
            this.os.warningFlash();
          PostProcessor.dangerModePercentComplete = Math.Min(this.timeThisState / (num * 0.85f), 1f);
          PostProcessor.dangerModeEnabled = true;
          this.PreventOSRendering = false;
          break;
        case TraceDangerSequence.TraceDangerState.DisconnectedReboot:
          num = 10f;
          if ((double) this.timeThisState > (double) num)
          {
            this.CancelTraceDangerSequence();
            MusicManager.loadAsCurrentSong(this.oldSong);
            this.os.rebootThisComputer();
            break;
          }
          break;
      }
      this.percentComplete = this.timeThisState / num;
    }

    public void Draw()
    {
      bool drawShadow = TextItem.DrawShadow;
      TextItem.DrawShadow = false;
      switch (this.state)
      {
        case TraceDangerSequence.TraceDangerState.WarningScrenIntro:
          this.DrawFlashingRedBackground();
          Rectangle destinationRectangle1 = new Rectangle(10, this.fullscreen.Height / 2 - 2, this.fullscreen.Width - 20, 4);
          this.spriteBatch.Draw(Utils.white, destinationRectangle1, Color.Black);
          destinationRectangle1.Width = (int) ((double) destinationRectangle1.Width * (1.0 - (double) this.percentComplete));
          this.spriteBatch.Draw(Utils.white, destinationRectangle1, Color.Red);
          break;
        case TraceDangerSequence.TraceDangerState.WarningScreen:
          this.DrawWarningScreen();
          break;
        case TraceDangerSequence.TraceDangerState.WarningScreenExiting:
          this.DrawFlashingRedBackground();
          Rectangle destinationRectangle2 = new Rectangle(10, this.fullscreen.Height / 2 - 2, this.fullscreen.Width - 20, 4);
          if ((double) this.percentComplete > 0.5)
          {
            int num = (int) ((double) ((float) this.os.fullscreen.Height * 0.7f) * (double) Utils.QuadraticOutCurve((float) (((double) this.percentComplete - 0.5) * 2.0)));
            destinationRectangle2.Y = this.fullscreen.Height / 2 - 2 - num / 2;
            destinationRectangle2.Height = num;
          }
          this.spriteBatch.Draw(Utils.white, destinationRectangle2, Color.Black);
          destinationRectangle2.Width = (int) ((double) destinationRectangle2.Width * (double) Math.Min(1f, Utils.QuadraticOutCurve(this.percentComplete * 2f)));
          this.spriteBatch.Draw(Utils.white, destinationRectangle2, Color.DarkRed);
          float num1 = Utils.QuadraticOutCurve((float) (((double) this.percentComplete - 0.5) * 2.0));
          if ((double) this.percentComplete > 0.5)
            new ThinBarcode(destinationRectangle2.Width, destinationRectangle2.Height).Draw(this.spriteBatch, destinationRectangle2.X, destinationRectangle2.Y, (double) Utils.randm(1f) > (double) num1 ? Color.Black : ((double) Utils.randm(1f) <= (double) num1 || (double) Utils.randm(1f) <= 0.800000011920929 ? Utils.VeryDarkGray : Utils.AddativeWhite));
          TextItem.doFontLabel(new Vector2((float) (this.fullscreen.Width / 2 - 250), (float) (destinationRectangle2.Y - 70)), "INITIALIZING FAILSAFE", GuiData.titlefont, new Color?(Color.White), 500f, 70f, false);
          break;
        case TraceDangerSequence.TraceDangerState.Countdown:
          this.PreventOSRendering = false;
          float num2 = this.timeThisState * 0.5f;
          if ((double) num2 < 1.0)
          {
            this.os.display.visible = (double) num2 > (double) Utils.randm(1f);
            this.os.netMap.visible = (double) num2 > (double) Utils.randm(1f);
            this.os.terminal.visible = (double) num2 > (double) Utils.randm(1f);
            this.os.ram.visible = (double) num2 > (double) Utils.randm(1f);
          }
          else
          {
            this.os.display.visible = true;
            this.os.netMap.visible = true;
            this.os.terminal.visible = true;
            this.os.ram.visible = true;
          }
          TraceDangerSequence.DrawCountdownOverlay(this.titleFont, this.bodyFont, (object) this.os, (string) null, (string) null, (string) null, (string) null);
          break;
        case TraceDangerSequence.TraceDangerState.DisconnectedReboot:
          this.DrawDisconnectedScreen();
          break;
      }
      TextItem.DrawShadow = drawShadow;
    }

    private void DrawDisconnectedScreen()
    {
      this.spriteBatch.Draw(Utils.white, this.fullscreen, Color.Black);
      Rectangle destinationRectangle = new Rectangle();
      destinationRectangle.X = this.fullscreen.X + 2;
      destinationRectangle.Width = this.fullscreen.Width - 4;
      destinationRectangle.Y = this.fullscreen.Y + this.fullscreen.Height / 6 * 2;
      destinationRectangle.Height = this.fullscreen.Height / 3;
      this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.indentBackgroundColor);
      Vector2 vector2 = GuiData.titlefont.MeasureString("DISCONNECTED");
      Vector2 position = new Vector2((float) (destinationRectangle.X + this.fullscreen.Width / 2) - vector2.X / 2f, (float) (this.fullscreen.Y + this.fullscreen.Height / 2 - 50));
      this.spriteBatch.DrawString(GuiData.titlefont, "DISCONNECTED", position, this.os.subtleTextColor);
      this.DrawFlashInString(LocaleTerms.Loc("Rebooting"), this.DrawFlashInString(LocaleTerms.Loc("Preparing for system reboot"), this.DrawFlashInString(LocaleTerms.Loc("Foreign trace averted"), this.DrawFlashInString(LocaleTerms.Loc("IP Address successfully reset"), new Vector2(200f, (float) (destinationRectangle.Y + destinationRectangle.Height + 20)), 4f, 0.2f, true, 0.2f), 5f, 0.2f, true, 0.2f), 6f, 0.2f, true, 0.8f), 9f, 0.2f, true, 0.2f);
    }

    private void DrawWarningScreen()
    {
      if (this.warningScreenIsActivating)
        this.spriteBatch.Draw(Utils.white, this.fullscreen, Color.White);
      else
        this.DrawFlashingRedBackground();
      string text = "WARNING";
      Vector2 vector2_1 = this.titleFont.MeasureString(text);
      float widthTo = (float) this.fullscreen.Width * 0.65f;
      float scale = widthTo / vector2_1.X;
      Vector2 vector2_2 = new Vector2(20f, -10f);
      this.spriteBatch.DrawString(this.titleFont, text, vector2_2, Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
      vector2_2.Y += (float) ((double) vector2_1.Y * (double) scale - 55.0);
      TextItem.doFontLabel(vector2_2, LocaleTerms.Loc("COMPLETED TRACE DETECTED : EMERGENCY RECOVERY MODE ACTIVE"), Settings.ActiveLocale.StartsWith("en") ? this.titleFont : GuiData.font, new Color?(Color.Black), widthTo, float.MaxValue, false);
      vector2_2.Y += 40f;
      vector2_2 = this.DrawFlashInString(LocaleTerms.Loc("Unsyndicated foreign connection detected during active trace"), vector2_2, 0.0f, 0.2f, false, 0.2f);
      vector2_2 = this.DrawFlashInString(" :: " + LocaleTerms.Loc("Emergency recovery mode activated"), vector2_2, 0.1f, 0.2f, false, 0.2f);
      vector2_2 = this.DrawFlashInString("-----------------------------------------------------------------------", vector2_2, 0.2f, 0.2f, false, 0.2f);
      vector2_2 = this.DrawFlashInString(" ", vector2_2, 0.5f, 0.2f, false, 0.2f);
      vector2_2 = this.DrawFlashInString(LocaleTerms.Loc("Automated screening procedures will divert incoming connections temporarily"), vector2_2, 0.5f, 0.2f, false, 0.2f);
      vector2_2 = this.DrawFlashInString(LocaleTerms.Loc("This window is a final opportunity to regain anonymity."), vector2_2, 0.6f, 0.2f, false, 0.2f);
      vector2_2 = this.DrawFlashInString(LocaleTerms.Loc("As your current IP Address is known, it must be changed") + " -", vector2_2, 0.7f, 0.2f, false, 0.2f);
      vector2_2 = this.DrawFlashInString(LocaleTerms.Loc("This can only be done on your currently active ISP's routing server"), vector2_2, 0.8f, 0.2f, false, 0.2f);
      Computer computer = Programs.getComputer(this.os, "ispComp");
      vector2_2 = this.DrawFlashInString(string.Format(LocaleTerms.Loc("Reverse tracerouting has located this ISP server's IP address as {0}"), computer != null ? (object) computer.ip : (object) "68.144.93.18"), vector2_2, 0.9f, 0.2f, false, 0.2f);
      vector2_2 = this.DrawFlashInString(string.Format(LocaleTerms.Loc("Your local ip : {0}  must be tracked here and changed."), (object) this.os.thisComputer.ip), vector2_2, 1f, 0.2f, false, 0.2f);
      vector2_2 = this.DrawFlashInString(" ", vector2_2, 1.1f, 0.2f, false, 0.2f);
      vector2_2 = this.DrawFlashInString(LocaleTerms.Loc("Failure to complete this while active diversion holds will result in complete"), vector2_2, 1.1f, 0.2f, false, 0.2f);
      vector2_2 = this.DrawFlashInString(LocaleTerms.Loc("and permanent loss of all account data - THIS IS NOT REPEATABLE AND CANNOT BE DELAYED"), vector2_2, 1.2f, 0.2f, false, 0.2f);
      if (this.warningScreenIsActivating || ((double) this.timeThisState < 1.20000004768372 || !Button.doButton(789798001, 20, (int) ((double) vector2_2.Y + 10.0), 400, 40, LocaleTerms.Loc("BEGIN"), new Color?(Color.Black))))
        return;
      this.timeThisState = 0.0f;
      this.state = TraceDangerSequence.TraceDangerState.WarningScreenExiting;
      this.PreventOSRendering = true;
      this.onBeatFlashTimer = this.os.timer;
      this.warningScreenIsActivating = true;
      this.spinUpSound.Play(1f, 0.0f, 0.0f);
      this.os.terminal.inputLocked = false;
      this.os.delayer.Post(ActionDelayer.Wait(0.1), (Action) (() => this.spinUpSound.Play(1f, 0.0f, 0.0f)));
      this.os.delayer.Post(ActionDelayer.Wait(0.4), (Action) (() => this.spinUpSound.Play(0.4f, 0.0f, 0.0f)));
      this.os.delayer.Post(ActionDelayer.Wait(0.8), (Action) (() => this.spinUpSound.Play(0.2f, 0.1f, 0.0f)));
      this.os.delayer.Post(ActionDelayer.Wait(1.3), (Action) (() => this.spinUpSound.Play(0.1f, 0.2f, 0.0f)));
      this.os.delayer.Post(ActionDelayer.Wait(0.01), (Action) (() => MusicManager.playSongImmediatley("Music/Traced")));
    }

    private Vector2 DrawFlashInString(string text, Vector2 pos, float offset, float transitionInTime = 0.2f, bool hasDots = false, float dotsDelayer = 0.2f)
    {
      Vector2 vector2 = new Vector2(40f, 0.0f);
      if ((double) this.timeThisState >= (double) offset)
      {
        float point = Math.Min((this.timeThisState - offset) / transitionInTime, 1f);
        Vector2 position = pos + vector2 * (1f - Utils.QuadraticOutCurve(point));
        string str = "";
        if (hasDots)
        {
          for (float num = this.timeThisState - offset; (double) num > 0.0 && str.Length < 5; str += ".")
            num -= dotsDelayer;
        }
        float scale = 0.5f;
        float num1 = 17f;
        if (LocaleActivator.ActiveLocaleIsCJK())
        {
          scale = 0.7f;
          num1 = 22f;
        }
        this.spriteBatch.DrawString(this.bodyFont, text + str, position, Color.White * point, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.4f);
        pos.Y += num1;
      }
      return pos;
    }

    public static void DrawCountdownOverlay(SpriteFont titleFont, SpriteFont bodyFont, object osobj, string title = null, string l1 = null, string l2 = null, string l3 = null)
    {
      OS os = (OS) osobj;
      if (title == null)
        title = "EMERGENCY TRACE AVERSION SEQUENCE";
      if (l1 == null)
        l1 = LocaleTerms.Loc("Reset Assigned Ip Address on ISP Mainframe");
      Computer computer = Programs.getComputer(os, "ispComp");
      if (l2 == null)
        l2 = string.Format(LocaleTerms.Loc("ISP Mainframe IP: {0}"), computer != null ? (object) computer.ip : (object) "68.144.93.18");
      if (l3 == null)
        l3 = string.Format(LocaleTerms.Loc("YOUR Assigned IP: {0}"), (object) os.thisComputer.ip);
      Rectangle fullscreen = Utils.GetFullscreen();
      int height = 110;
      Rectangle destinationRectangle = new Rectangle(0, fullscreen.Height - height - 20, 520, height);
      GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Lerp(TraceDangerSequence.DarkRed, Color.Transparent, Utils.randm(0.2f)));
      Vector2 pos = new Vector2((float) (destinationRectangle.X + 6), (float) (destinationRectangle.Y + 4));
      TextItem.doFontLabel(pos, title, titleFont, new Color?(Color.White), (float) (destinationRectangle.Width - 12), 35f, false);
      pos.Y += 32f;
      TextItem.doFontLabel(pos, l1, bodyFont, new Color?(Color.White), (float) (destinationRectangle.Width - 10), 20f, false);
      pos.Y += 16f;
      TextItem.doFontLabel(pos, l2, bodyFont, new Color?(Color.White), (float) (destinationRectangle.Width - 10), 20f, false);
      pos.Y += 16f;
      TextItem.doFontLabel(pos, l3, bodyFont, new Color?(Color.White), (float) (destinationRectangle.Width - 10), 20f, false);
    }

    private void DrawFlashingRedBackground()
    {
      this.spriteBatch.Draw(Utils.white, this.fullscreen, Color.Lerp(TraceDangerSequence.BackgroundRed, Color.Black, Utils.randm(0.22f)));
    }

    private enum TraceDangerState
    {
      WarningScrenIntro,
      WarningScreen,
      WarningScreenExiting,
      Countdown,
      Gameover,
      DisconnectedReboot,
    }
  }
}
