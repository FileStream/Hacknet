// Decompiled with JetBrains decompiler
// Type: Hacknet.SequencerExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class SequencerExe : ExeModule
  {
    public static int ACTIVATING_RAM_COST = 170;
    public static int BASE_RAM_COST = 60;
    public static float RAM_CHANGE_PS = 100f;
    public static double Song_Length = 186.0;
    public static float SPIN_UP_TIME = 17f;
    private static float TimeBetweenBeats = 1.832061f;
    private MovingBarsEffect bars = new MovingBarsEffect();
    private string oldSongName = (string) null;
    private int targetRamUse = SequencerExe.ACTIVATING_RAM_COST;
    private float stateTimer = 0.0f;
    private float beatHits = 0.15f;
    private double beatDropTime = 16.64;
    private List<ConnectedNodeEffect> nodeeffects = new List<ConnectedNodeEffect>();
    private SequencerExe.SequencerExeState state = SequencerExe.SequencerExeState.Unavaliable;
    private OSTheme targetTheme = OSTheme.HacknetWhite;
    private bool HasBeenKilled = false;
    private string targetID;
    private string flagForProgressionName;
    private Computer targetComp;
    private OSTheme originalTheme;

    public SequencerExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.name = "Sequencer";
      this.ramCost = SequencerExe.ACTIVATING_RAM_COST;
      this.IdentifierName = "Sequencer";
      this.targetIP = this.os.thisComputer.ip;
      this.bars.MinLineChangeTime = 1f;
      this.bars.MaxLineChangeTime = 3f;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
      this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
      this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
      this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
      this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
      if (Settings.isPressBuildDemo)
      {
        this.targetID = "finalNodeDemo";
        this.flagForProgressionName = "DemoSequencerEnabled";
      }
      else
      {
        this.targetID = "EnTechOfflineBackup";
        this.flagForProgressionName = "VaporSequencerEnabled";
      }
      if (ThemeManager.currentTheme != OSTheme.HacknetWhite)
        return;
      this.targetTheme = OSTheme.HacknetBlue;
    }

    public override void Update(float t)
    {
      base.Update(t);
      if (this.HasBeenKilled)
        return;
      this.bars.Update(t);
      this.UpdateRamCost(t);
      this.stateTimer += t;
      switch (this.state)
      {
        case SequencerExe.SequencerExeState.Unavaliable:
          if (this.os.Flags.HasFlag(this.flagForProgressionName) || Settings.debugCommandsEnabled)
          {
            this.state = SequencerExe.SequencerExeState.AwaitingActivation;
            break;
          }
          this.bars.MinLineChangeTime = 1f;
          this.bars.MaxLineChangeTime = 3f;
          break;
        case SequencerExe.SequencerExeState.SpinningUp:
          if (MediaPlayer.State == MediaState.Playing)
          {
            if (MediaPlayer.PlayPosition.TotalSeconds < this.beatDropTime || (double) this.stateTimer <= 10.0)
              break;
            this.MoveToActiveState();
            break;
          }
          if ((double) this.stateTimer > (double) SequencerExe.SPIN_UP_TIME)
            this.MoveToActiveState();
          break;
        case SequencerExe.SequencerExeState.Active:
          float num = 2.5f;
          if ((double) this.stateTimer < (double) num)
          {
            if ((double) Utils.randm(1f) < 0.300000011920929 + (double) this.stateTimer / (double) num * 0.699999988079071)
            {
              ThemeManager.switchThemeColors(this.os, this.targetTheme);
              ThemeManager.loadThemeBackground(this.os, this.targetTheme);
              ThemeManager.currentTheme = this.targetTheme;
            }
            else
            {
              ThemeManager.switchThemeColors(this.os, this.originalTheme);
              ThemeManager.loadThemeBackground(this.os, this.originalTheme);
              ThemeManager.currentTheme = this.originalTheme;
            }
            if ((MediaPlayer.PlayPosition.TotalSeconds - this.beatDropTime) % (double) this.beatHits < 0.00999999977648258)
              this.os.warningFlash();
          }
          this.ActiveStateUpdate(t);
          break;
      }
    }

    private void ActiveStateUpdate(float t)
    {
      PostProcessor.dangerModeEnabled = true;
      double num = (double) this.stateTimer;
      if (MediaPlayer.State == MediaState.Playing && !Settings.soundDisabled)
        num = MediaPlayer.PlayPosition.TotalSeconds;
      PostProcessor.dangerModePercentComplete = (float) ((num - (double) SequencerExe.SPIN_UP_TIME) / (SequencerExe.Song_Length - (double) SequencerExe.SPIN_UP_TIME));
      if ((double) PostProcessor.dangerModePercentComplete >= 1.0)
      {
        if (Settings.isDemoMode)
        {
          MissionFunctions.runCommand(0, "demoFinalMissionEnd");
        }
        else
        {
          MusicManager.playSongImmediatley("Music/Ambient/AmbientDrone_Clipped");
          this.os.netMap.visibleNodes.Remove(this.os.netMap.nodes.IndexOf(this.targetComp));
          PostProcessor.dangerModeEnabled = false;
          PostProcessor.dangerModePercentComplete = 0.0f;
          this.os.thisComputer.crash(this.os.thisComputer.ip);
        }
      }
      if (this.os.connectedComp == null && (double) this.stateTimer > 1.0 && !Settings.isDemoMode)
      {
        this.isExiting = true;
        if (this.oldSongName != null)
        {
          MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
          MediaPlayer.IsRepeating = true;
        }
        this.os.netMap.visibleNodes.Remove(this.os.netMap.nodes.IndexOf(this.targetComp));
        PostProcessor.dangerModeEnabled = false;
        PostProcessor.dangerModePercentComplete = 0.0f;
      }
      if ((num - this.beatDropTime) % (double) SequencerExe.TimeBetweenBeats >= (double) t)
        return;
      this.os.warningFlash();
    }

    private void UpdateRamCost(float t)
    {
      if (this.targetRamUse == this.ramCost)
        return;
      if (this.targetRamUse < this.ramCost)
      {
        this.ramCost -= (int) ((double) t * (double) SequencerExe.RAM_CHANGE_PS);
        if (this.ramCost < this.targetRamUse)
          this.ramCost = this.targetRamUse;
      }
      else
      {
        int num = (int) ((double) t * (double) SequencerExe.RAM_CHANGE_PS);
        if (this.os.ramAvaliable >= num)
        {
          this.ramCost += num;
          if (this.ramCost > this.targetRamUse)
            this.ramCost = this.targetRamUse;
        }
      }
    }

    public override void Killed()
    {
      base.Killed();
      this.HasBeenKilled = true;
      PostProcessor.dangerModeEnabled = false;
      PostProcessor.dangerModePercentComplete = 0.0f;
      if (this.oldSongName != null)
      {
        MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
        MediaPlayer.IsRepeating = true;
      }
      this.os.netMap.DimNonConnectedNodes = false;
      this.os.runCommand("disconnect");
      if (this.targetComp == null)
        return;
      this.os.netMap.visibleNodes.Remove(this.os.netMap.nodes.IndexOf(this.targetComp));
    }

    private void MoveToActiveState()
    {
      this.state = SequencerExe.SequencerExeState.Active;
      this.stateTimer = 0.0f;
      this.targetRamUse = SequencerExe.BASE_RAM_COST;
      this.os.warningFlashTimer = OS.WARNING_FLASH_TIME;
      this.os.netMap.DimNonConnectedNodes = true;
      this.os.netMap.discoverNode(this.targetComp);
      this.os.runCommand("connect " + this.targetComp.ip);
      this.os.delayer.Post(ActionDelayer.Wait(0.05), (Action) (() => this.os.runCommand("probe")));
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      Rectangle rectangle = Utils.InsetRectangle(this.GetContentAreaDest(), 1);
      float amount = this.os.warningFlashTimer / OS.WARNING_FLASH_TIME;
      float minHeight = 2f;
      if ((double) amount > 0.0)
        minHeight += amount * ((float) rectangle.Height - minHeight);
      Color drawColor = Color.Lerp(Utils.AddativeWhite * 0.5f, Utils.AddativeRed, amount);
      this.bars.Draw(this.spriteBatch, this.GetContentAreaDest(), minHeight, 4f, 1f, drawColor);
      switch (this.state)
      {
        case SequencerExe.SequencerExeState.Unavaliable:
          this.spriteBatch.Draw(Utils.white, rectangle, Color.Black * 0.5f);
          Rectangle dest = Utils.InsetRectangle(rectangle, 6);
          if (!this.isExiting)
            TextItem.doFontLabelToSize(dest, "LINK UNAVAILABLE", GuiData.titlefont, Utils.AddativeWhite, false, false);
          Rectangle destinationRectangle1 = dest;
          destinationRectangle1.Y += destinationRectangle1.Height - 20;
          destinationRectangle1.Height = 20;
          if (this.isExiting)
            break;
          GuiData.spriteBatch.Draw(Utils.white, destinationRectangle1, Color.Black * 0.5f);
          if (Button.doButton(32711803, destinationRectangle1.X, destinationRectangle1.Y, destinationRectangle1.Width, destinationRectangle1.Height, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
            this.isExiting = true;
          break;
        case SequencerExe.SequencerExeState.AwaitingActivation:
          int height = 30;
          Rectangle destinationRectangle2 = new Rectangle(this.bounds.X + 1, this.bounds.Y + this.bounds.Height / 2 - height, this.bounds.Width - 2, height * 2);
          this.spriteBatch.Draw(Utils.white, destinationRectangle2, Color.Black * 0.92f);
          if (!Button.doButton(8310101, this.bounds.X + 10, this.bounds.Y + this.bounds.Height / 2 - height / 2, this.bounds.Width - 20, height, LocaleTerms.Loc("ACTIVATE"), new Color?(this.os.highlightColor)))
            break;
          if (this.os.TraceDangerSequence.IsActive)
          {
            this.os.write("SEQUENCER ERROR: OS reports critical action already in progress.");
          }
          else
          {
            this.stateTimer = 0.0f;
            this.state = SequencerExe.SequencerExeState.SpinningUp;
            this.bars.MinLineChangeTime = 0.1f;
            this.bars.MaxLineChangeTime = 1f;
            this.originalTheme = ThemeManager.currentTheme;
            MusicManager.FADE_TIME = 0.6f;
            this.oldSongName = MusicManager.currentSongName;
            MusicManager.transitionToSong("Music\\Roller_Mobster_Clipped");
            MediaPlayer.IsRepeating = false;
            this.targetComp = Programs.getComputer(this.os, this.targetID);
            WebServerDaemon daemon = (WebServerDaemon) this.targetComp.getDaemon(typeof (WebServerDaemon));
            if (daemon != null)
              daemon.LoadWebPage("index.html");
          }
          break;
        case SequencerExe.SequencerExeState.SpinningUp:
          Rectangle bounds = rectangle;
          bounds.Height = (int) ((double) bounds.Height * ((double) this.stateTimer / (double) SequencerExe.SPIN_UP_TIME));
          bounds.Y = rectangle.Y + rectangle.Height - bounds.Height + 1;
          bounds.Width += 4;
          this.bars.Draw(this.spriteBatch, bounds, minHeight, 4f, 1f, this.os.brightLockedColor);
          break;
        case SequencerExe.SequencerExeState.Active:
          this.spriteBatch.Draw(Utils.white, this.GetContentAreaDest(), Color.Black * 0.5f);
          TextItem.doFontLabelToSize(this.GetContentAreaDest(), " G O   G O   G O ", GuiData.titlefont, Color.Lerp(Utils.AddativeRed, this.os.brightLockedColor, Math.Min(1f, this.stateTimer / 2f)), false, false);
          this.DrawActiveState();
          break;
      }
    }

    private void DrawActiveState()
    {
      float val1 = 5.2f;
      float point = Math.Min(val1, this.stateTimer) / val1;
      float num1 = 30f;
      Vector2 vector2 = new Vector2((float) this.os.netMap.bounds.X, (float) this.os.netMap.bounds.Y) + new Vector2((float) NetworkMap.NODE_SIZE / 2f);
      for (int index = 0; index < this.nodeeffects.Count; ++index)
      {
        float num2 = (float) (index + 1) / (float) (this.nodeeffects.Count + 1);
        float num3 = 3f * num2;
        float num4 = 1f - Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(point)));
        this.nodeeffects[index].ScaleFactor = num2 * (num1 * num4) + num3;
        this.nodeeffects[index].draw(this.spriteBatch, vector2 + this.os.netMap.GetNodeDrawPos(this.targetComp));
      }
      this.DrawCountdownOverlay();
    }

    private void DrawCountdownOverlay()
    {
      int height = 110;
      Rectangle destinationRectangle = new Rectangle(0, this.os.fullscreen.Height - height - 20, 400, height);
      Color color = new Color(100, 0, 0, 200);
      this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Lerp(color, Color.Transparent, Utils.randm(0.2f)));
      Vector2 pos = new Vector2((float) (destinationRectangle.X + 6), (float) (destinationRectangle.Y + 4));
      TextItem.doFontLabel(pos, "ENTECH SEQUENCER ATTACK", GuiData.titlefont, new Color?(Color.White), (float) (destinationRectangle.Width - 12), 35f, false);
      pos.Y += 32f;
      TextItem.doFontLabel(pos, Settings.isDemoMode ? "Analyse security countermeasures with \"Probe\"" : LocaleTerms.Loc("Break active security on target"), GuiData.smallfont, new Color?(Color.White), (float) (destinationRectangle.Width - 10), 20f, false);
      pos.Y += 16f;
      TextItem.doFontLabel(pos, Settings.isDemoMode ? "Break active security and gain access (Programs + Porthack)" : LocaleTerms.Loc("Delete all Hacknet related files"), GuiData.smallfont, new Color?(Color.White), (float) (destinationRectangle.Width - 10), 20f, false);
      pos.Y += 16f;
      TextItem.doFontLabel(pos, Settings.isDemoMode ? "Delete all files in directories /sys/ and /log/" : LocaleTerms.Loc("Disconnect"), GuiData.smallfont, new Color?(Color.White), (float) (destinationRectangle.Width - 10), 20f, false);
    }

    private enum SequencerExeState
    {
      Unavaliable,
      AwaitingActivation,
      SpinningUp,
      Active,
    }
  }
}
