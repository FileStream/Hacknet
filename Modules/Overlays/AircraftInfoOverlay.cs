// Decompiled with JetBrains decompiler
// Type: Hacknet.Modules.Overlays.AircraftInfoOverlay
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Daemons.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;

namespace Hacknet.Modules.Overlays
{
  public class AircraftInfoOverlay
  {
    public bool IsActive = false;
    private float timeElapsed = 0.0f;
    private float flashInTimeLeft = 1f;
    public bool IsMonitoringDLCEndingCases = false;
    private bool TargetHasStartedCrashing = false;
    private bool IsInPostSaveState = false;
    private AircraftDaemon CrashingAircraft;
    private AircraftDaemon SecondaryAircraft;
    private OS os;
    private SoundEffect AircraftSaveSound;

    public AircraftInfoOverlay(object OSobj)
    {
      this.os = (OS) OSobj;
      this.CrashingAircraft = (AircraftDaemon) Programs.getComputer(this.os, "dair_crash").getDaemon(typeof (AircraftDaemon));
      this.CrashingAircraft.CrashAction += (Action) (() => this.CrashingAircraft = (AircraftDaemon) null);
      this.SecondaryAircraft = (AircraftDaemon) Programs.getComputer(this.os, "dair_secondary").getDaemon(typeof (AircraftDaemon));
      this.SecondaryAircraft.CrashAction += (Action) (() => this.SecondaryAircraft = (AircraftDaemon) null);
      this.AircraftSaveSound = this.os.content.Load<SoundEffect>("SFX/TraceKill");
    }

    public void Activate()
    {
      this.IsActive = true;
      this.timeElapsed = 0.0f;
      this.flashInTimeLeft = 1f;
      this.CrashingAircraft.StartUpdating();
      this.SecondaryAircraft.StartUpdating();
      this.os.Flags.AddFlag("AircraftInfoOverlayActivated");
    }

    public void Update(float dt)
    {
      this.flashInTimeLeft = Math.Max(0.0f, this.flashInTimeLeft - dt);
      this.timeElapsed += dt;
      if (!this.IsMonitoringDLCEndingCases)
        return;
      if (this.CrashingAircraft != null)
      {
        if (this.CrashingAircraft.IsInCriticalFirmwareFailure)
        {
          this.TargetHasStartedCrashing = true;
          this.IsInPostSaveState = false;
          double totalSeconds = MediaPlayer.PlayPosition.TotalSeconds;
          double num1 = 1.0 / (131.0 / 60.0);
          double num2 = num1 * 4.0;
          double num3 = num1 * 2.0;
          double num4 = totalSeconds < 58.0 ? 999.0 : (totalSeconds < 117.0 ? num2 : num3);
          if ((totalSeconds + num1 / 2.0) % num4 < num1 / 4.0)
            this.os.warningFlash();
        }
        else if (this.TargetHasStartedCrashing)
        {
          if (!this.os.Flags.HasFlag("DLC_PlaneResult"))
          {
            RunnableConditionalActions.LoadIntoOS("DLC/ActionScripts/FinaleSaveActions.xml", (object) this.os);
            this.os.Flags.AddFlag("DLC_PlaneSaveResponseTriggered");
            this.os.Flags.AddFlag("DLC_PlaneResult");
          }
          if (!this.CrashingAircraft.IsInCriticalDescent() && !MediaPlayer.IsRepeating)
          {
            MusicManager.FADE_TIME = 6f;
            MusicManager.transitionToSong("DLC/Music/RemiDrone");
            MediaPlayer.IsRepeating = true;
            this.os.delayer.Post(ActionDelayer.Wait(2.0), (Action) (() => this.AircraftSaveSound.Play()));
            this.IsInPostSaveState = true;
          }
        }
      }
      else if (this.TargetHasStartedCrashing)
      {
        if (this.SecondaryAircraft == null || this.SecondaryAircraft.IsInCriticalFirmwareFailure)
        {
          if (!this.os.Flags.HasFlag("DLC_PlaneResult"))
          {
            RunnableConditionalActions.LoadIntoOS("DLC/ActionScripts/FinaleDoubleCrashActions.xml", (object) this.os);
            this.os.Flags.AddFlag("DLC_DoubleCrashResponseTriggered");
            this.os.Flags.AddFlag("DLC_PlaneResult");
          }
        }
        else if (!this.os.Flags.HasFlag("DLC_PlaneResult"))
        {
          RunnableConditionalActions.LoadIntoOS("DLC/ActionScripts/FinaleCrashActions.xml", (object) this.os);
          this.os.Flags.AddFlag("DLC_PlaneCrashedResponseTriggered");
          this.os.Flags.AddFlag("DLC_PlaneResult");
        }
        if (MusicManager.currentSongName != "DLC\\Music\\CrashTrack")
        {
          MusicManager.playSongImmediatley("DLC\\Music\\CrashTrack");
          MediaPlayer.IsRepeating = false;
        }
      }
      if (!MediaPlayer.IsRepeating && MediaPlayer.State != MediaState.Playing && !this.IsInPostSaveState)
      {
        MusicManager.FADE_TIME = 6f;
        MissionFunctions.runCommand(7, "changeSongDLC");
        MediaPlayer.IsRepeating = true;
      }
    }

    public void Draw(Rectangle dest, SpriteBatch sb)
    {
      if (!this.IsActive || (double) Utils.randm(1f) < (double) this.flashInTimeLeft)
        return;
      if (this.CrashingAircraft != null)
        AircraftAltitudeIndicator.RenderAltitudeIndicator(dest, sb, (int) this.CrashingAircraft.CurrentAltitude, this.CrashingAircraft.IsInCriticalDescent(), AircraftAltitudeIndicator.GetFlashRateFromTimer(this.os.timer), 50000, 40000, 30000, 14000, 3000);
      else
        AircraftAltitudeIndicator.RenderAltitudeIndicator(dest, sb, 0, true, AircraftAltitudeIndicator.GetFlashRateFromTimer(this.os.timer), 50000, 40000, 30000, 14000, 3000);
    }
  }
}
