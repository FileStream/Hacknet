// Decompiled with JetBrains decompiler
// Type: Hacknet.DLCIntroExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class DLCIntroExe : ExeModule
  {
    private static float SpinUpTime = 13.8f;
    private static float FlickeringTime = 10f;
    private static float MailIconFlickerOutTime = 3.82f;
    private static float AssignMission1Time = 16f;
    private static float AssignMission2Time = 16f;
    private static float WindDownTimeAfterCompletingMission = 0.8f;
    private static float NodeImpactEffectTransOutTime = 3f;
    private static float NodeImpactEffectTransInTime = 2f;
    private Color themeColor = new Color(38, 201, 155, 220);
    private DLCIntroExe.IntroState State = DLCIntroExe.IntroState.NotStarted;
    private float TimeInThisState = 0.0f;
    private float percentageThroughThisState = 0.0f;
    private OSTheme originalTheme = OSTheme.HacknetBlue;
    private List<TraceKillExe.PointImpactEffect> ImpactEffects = new List<TraceKillExe.PointImpactEffect>();
    private float timeBetweenNodeRemovals = 1f;
    private float timeSinceNodeRemoved = 0.0f;
    private string assignment1MissionPath = "Content/DLC/Missions/Intro/KaguyaTrialMission1.xml";
    private string assignment2MissionPath = "Content/DLC/Missions/Intro/KaguyaTrialMission2.xml";
    private string Assignment1Text = "";
    private string Assignment2Text = "";
    private string AssignmentsCompleteText = "";
    private int charsRenderedSoFar = 0;
    private bool IsOnAssignment1 = true;
    private bool AllAssignmentsComplete = false;
    private bool MissionIsComplete = false;
    private string PhaseTitle = "";
    private string PhaseSubtitle = "";
    private bool OSTraceTimerOverrideActive = false;
    private ExplodingUIElementEffect explosion = new ExplodingUIElementEffect();
    private const bool combineNodeExplodeAndMailBurst = false;
    private Texture2D circle;
    private Texture2D circleOutline;
    private Color originalTopBarIconsColor;
    private ActiveMission LoadedMission;
    private HexGridBackground BackgroundEffect;
    private SoundEffect GlowSound;
    private SoundEffect BreakSound;

    public DLCIntroExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.name = "KaguyaTrial";
      this.ramCost = 190;
      this.IdentifierName = "KaguyaTrial";
      this.targetIP = this.os.thisComputer.ip;
      this.circle = this.os.content.Load<Texture2D>("Circle");
      this.circleOutline = this.os.content.Load<Texture2D>("CircleOutlineLarge");
      this.Assignment1Text = Utils.readEntireFile("Content/DLC/Docs/KaguyaTrial1.txt");
      this.Assignment2Text = Utils.readEntireFile("Content/DLC/Docs/KaguyaTrial2.txt");
      this.AssignmentsCompleteText = Utils.readEntireFile("Content/DLC/Docs/KaguyaTrialComplete.txt");
      this.BackgroundEffect = new HexGridBackground(this.os.content);
      this.explosion.Init(this.os.content);
      this.GlowSound = this.os.content.Load<SoundEffect>("SFX/Ending/PorthackSpindown");
      this.BreakSound = this.os.content.Load<SoundEffect>("SFX/DoomShock");
      if (!(MusicManager.currentSongName != "DLC\\Music\\snidelyWhiplash") || this.os.Flags.HasFlag("KaguyaTrialComplete"))
        return;
      MusicManager.transitionToSong("Music/Ambient/dark_drone_008");
    }

    public override void Killed()
    {
      base.Killed();
      if (this.State == DLCIntroExe.IntroState.NotStarted || this.State == DLCIntroExe.IntroState.Exiting)
        return;
      this.os.delayer.Post(ActionDelayer.NextTick(), (Action) (() =>
      {
        DLCIntroExe dlcIntroExe = new DLCIntroExe(this.bounds, this.os, new string[0]) { State = this.State, TimeInThisState = this.TimeInThisState, LoadedMission = this.LoadedMission, IsOnAssignment1 = this.IsOnAssignment1, AllAssignmentsComplete = this.AllAssignmentsComplete, MissionIsComplete = this.MissionIsComplete, PhaseTitle = this.PhaseTitle, PhaseSubtitle = this.PhaseSubtitle };
        if (this.OSTraceTimerOverrideActive)
        {
          this.os.traceCompleteOverrideAction = (Action) null;
          this.os.traceCompleteOverrideAction += new Action(dlcIntroExe.PlayerLostToTraceTimer);
        }
        this.os.addExe((ExeModule) dlcIntroExe);
      }));
    }

    private void UpdateState(float t)
    {
      this.TimeInThisState += t;
      switch (this.State)
      {
        case DLCIntroExe.IntroState.SpinningUp:
          if ((double) this.TimeInThisState >= (double) DLCIntroExe.SpinUpTime)
          {
            this.PrepareForUIBreakdown();
            this.State = DLCIntroExe.IntroState.Flickering;
            this.PhaseTitle = LocaleTerms.Loc("INITIALIZING");
            this.PhaseSubtitle = "---";
            this.TimeInThisState = 0.0f;
            this.os.execute("dc");
            this.os.execute("clear");
            this.os.warningFlash();
          }
          this.percentageThroughThisState = this.TimeInThisState / DLCIntroExe.SpinUpTime;
          break;
        case DLCIntroExe.IntroState.Flickering:
          if ((double) this.TimeInThisState >= (double) DLCIntroExe.FlickeringTime)
          {
            this.State = DLCIntroExe.IntroState.MailIconPhasingOut;
            this.TimeInThisState = 0.0f;
            SFX.addCircle(this.os.mailicon.pos + new Vector2(20f, 6f), Utils.AddativeRed, 100f);
            this.os.execute("clear");
            this.os.warningFlash();
          }
          this.percentageThroughThisState = this.TimeInThisState / DLCIntroExe.FlickeringTime;
          break;
        case DLCIntroExe.IntroState.MailIconPhasingOut:
          if ((double) this.TimeInThisState >= (double) DLCIntroExe.MailIconFlickerOutTime)
          {
            this.CompleteMailPhaseOut();
            this.State = DLCIntroExe.IntroState.AssignMission1;
            this.TimeInThisState = 0.0f;
            this.os.warningFlash();
          }
          this.percentageThroughThisState = this.TimeInThisState / DLCIntroExe.MailIconFlickerOutTime;
          break;
        case DLCIntroExe.IntroState.AssignMission1:
          this.percentageThroughThisState = this.TimeInThisState / DLCIntroExe.AssignMission1Time;
          break;
        case DLCIntroExe.IntroState.OnMission1:
        case DLCIntroExe.IntroState.OnMission2:
          this.CheckProgressOfCurrentAssignment();
          break;
        case DLCIntroExe.IntroState.AssignMission2:
          this.percentageThroughThisState = this.TimeInThisState / DLCIntroExe.AssignMission2Time;
          break;
      }
      this.explosion.Update(t);
    }

    private void PlayerLostToTraceTimer()
    {
      this.os.runCommand("dc");
      this.os.runCommand("clear");
      this.os.delayer.Post(ActionDelayer.NextTick(), (Action) (() =>
      {
        this.os.write("-----------------\n\nKT_OVERRIDE_RECOVERY: " + LocaleTerms.Loc("Critical Error - resetting.") + "\n\n----------------");
        this.State = DLCIntroExe.IntroState.AssignMission1;
        this.TimeInThisState = 0.0f;
      }));
    }

    private void CompleteExecution()
    {
      this.PhaseTitle = LocaleTerms.Loc("COMPLETE");
      this.PhaseSubtitle = "---";
      this.State = DLCIntroExe.IntroState.Exiting;
      this.isExiting = true;
      this.os.traceCompleteOverrideAction = (Action) null;
      if (!this.os.Flags.HasFlag("KaguyaTrialComplete"))
      {
        this.os.Flags.AddFlag("KaguyaTrialComplete");
        this.os.allFactions.setCurrentFaction("Bibliotheque", this.os);
        this.os.homeNodeID = "dhs";
        this.os.homeAssetServerID = "dhsDrop";
        MissionFunctions.runCommand(1, "addRankSilent");
        this.os.currentMission = (ActiveMission) null;
        Computer computer = Programs.getComputer(this.os, "dhs");
        (computer.getDaemon(typeof (DLCHubServer)) as DLCHubServer).AddAgent(this.os.defaultUser.name, "dnkA19ds", new Color(222, 153, 24));
        for (int index = 0; index < computer.users.Count; ++index)
        {
          UserDetail user = computer.users[index];
          if (user.name == this.os.defaultUser.name)
          {
            user.known = true;
            computer.users[index] = user;
          }
        }
      }
      this.os.runCommand("connect 69.58.186.114");
      this.os.IsInDLCMode = true;
      this.os.DisableEmailIcon = false;
      this.isExiting = true;
    }

    private void UpdateMailePhaseOut(float t)
    {
      this.AddRadialMailLine();
      this.AddRadialMailLine();
      if ((double) this.percentageThroughThisState > 0.200000002980232)
      {
        this.AddRadialMailLine();
        this.AddRadialMailLine();
      }
      if ((double) this.TimeInThisState % 0.600000023841858 <= (double) t)
        SFX.addCircle(this.os.mailicon.pos + new Vector2(20f, 6f), Utils.AddativeRed, (float) (100.0 + 200.0 * (double) this.percentageThroughThisState));
      this.os.topBarIconsColor = (double) Utils.randm(1f) < (double) this.percentageThroughThisState ? Color.Red : this.originalTopBarIconsColor;
      Utils.FillEverywhereExcept(Utils.InsetRectangle(this.os.terminal.Bounds, 1), Utils.GetFullscreen(), this.spriteBatch, Color.Black * (0.8f * this.percentageThroughThisState));
    }

    private void CompleteMailPhaseOut()
    {
      this.os.DisableEmailIcon = true;
      SFX.addCircle(this.os.mailicon.pos + new Vector2(20f, 6f), Utils.AddativeRed * 0.8f, 100f);
      for (int index = 0; index < 12; ++index)
        this.os.delayer.Post(ActionDelayer.Wait((double) index / 7.0), (Action) (() => SFX.addCircle(this.os.mailicon.pos + new Vector2(20f, 6f), Utils.AddativeRed * 0.8f, 400f)));
      Vector2 mailIconPos = this.os.mailicon.pos;
      this.explosion.Explode(1500, new Vector2(-0.1f, 3.241593f), mailIconPos, 1f, 8f, 100f, 1600f, 1000f, 1200f, 3f, 7f);
      this.os.delayer.Post(ActionDelayer.Wait(0.1), (Action) (() => this.explosion.Explode(100, new Vector2(-0.1f, 3.241593f), mailIconPos, 1f, 6f, 100f, 1300f, 1000f, 1300f, 3f, 7f)));
      this.BreakSound.Play();
      this.os.topBarIconsColor = this.originalTopBarIconsColor;
      PostProcessor.EndingSequenceFlashOutActive = false;
      PostProcessor.EndingSequenceFlashOutPercentageComplete = 0.0f;
      this.os.terminal.reset();
    }

    private void PrepareForUIBreakdown()
    {
      this.originalTheme = ThemeManager.currentTheme;
      ThemeManager.setThemeOnComputer((object) this.os.thisComputer, "DLC/Themes/MiamiTheme.xml");
      ThemeManager.switchTheme((object) this.os, "DLC/Themes/MiamiTheme.xml");
      this.timeBetweenNodeRemovals = DLCIntroExe.FlickeringTime / 2f / (float) this.os.netMap.visibleNodes.Count;
      this.os.netMap.CleanVisibleListofDuplicates();
      this.originalTopBarIconsColor = this.os.topBarIconsColor;
    }

    private void AddRadialMailLine()
    {
      SFX.AddRadialLine(this.os.mailicon.pos + new Vector2(20f, 10f), 3.141593f + Utils.rand(3.141593f), 600f + Utils.randm(300f), 800f, 500f, 200f + Utils.randm(400f), 0.35f, Color.Lerp(Utils.makeColor((byte) 100, (byte) 0, (byte) 0, byte.MaxValue), Utils.AddativeRed, Utils.randm(1f)), 3f, false);
    }

    private void UpdateUIFlickerIn()
    {
      float num = Math.Min(1f, this.percentageThroughThisState * (float) (1.0 / 0.400000005960464));
      OSTheme theme = OSTheme.Custom;
      if ((double) this.percentageThroughThisState < 0.990000009536743)
      {
        if ((double) Utils.randm(1f) < (double) num)
        {
          ThemeManager.switchTheme((object) this.os, theme);
        }
        else
        {
          ThemeManager.switchThemeColors(this.os, this.originalTheme);
          ThemeManager.loadThemeBackground(this.os, this.originalTheme);
          ThemeManager.currentTheme = this.originalTheme;
        }
      }
      PostProcessor.EndingSequenceFlashOutActive = true;
      PostProcessor.EndingSequenceFlashOutPercentageComplete = 1f - num;
      if ((double) this.percentageThroughThisState <= 0.699999988079071)
        return;
      this.AddRadialMailLine();
    }

    private void UpdateUIBreaking(float t)
    {
      float num = 0.6f;
      Math.Max(0.0f, Math.Min(1f, (float) (((double) this.percentageThroughThisState - (1.0 - (double) num)) * (1.0 / (double) num))));
      if ((double) this.percentageThroughThisState <= 0.0 || this.os.netMap.visibleNodes.Count <= 1)
        return;
      this.timeSinceNodeRemoved += t;
      if ((double) this.timeSinceNodeRemoved > (double) this.timeBetweenNodeRemovals)
      {
        this.timeSinceNodeRemoved -= this.timeBetweenNodeRemovals;
        int index;
        Computer node;
        do
        {
          index = Utils.random.Next(this.os.netMap.visibleNodes.Count);
          node = this.os.netMap.nodes[this.os.netMap.visibleNodes[index]];
        }
        while (node == this.os.thisComputer);
        Vector2 screenSpacePosition = node.getScreenSpacePosition();
        OS os = this.os;
        string str = os.PreDLCVisibleNodesCache + (this.os.PreDLCVisibleNodesCache.Length > 0 ? (object) "," : (object) "") + (object) this.os.netMap.visibleNodes[index];
        os.PreDLCVisibleNodesCache = str;
        this.os.netMap.nodes[this.os.netMap.visibleNodes[index]].adminIP = node.ip;
        this.os.netMap.visibleNodes.RemoveAt(index);
        this.ImpactEffects.Add(new TraceKillExe.PointImpactEffect()
        {
          location = screenSpacePosition,
          scaleModifier = (float) (3.0 + (node.securityLevel > 2 ? 1.0 : 0.0)),
          cne = new ConnectedNodeEffect(this.os, true),
          timeEnabled = 0.0f,
          HasHighlightCircle = true
        });
        if (node.securityLevel > 3)
        {
          for (int val2 = 0; val2 < node.securityLevel && val2 < 6; ++val2)
            this.ImpactEffects.Add(new TraceKillExe.PointImpactEffect()
            {
              location = screenSpacePosition,
              scaleModifier = (float) Math.Min(8, val2),
              cne = new ConnectedNodeEffect(this.os, true),
              timeEnabled = 0.0f,
              HasHighlightCircle = false
            });
        }
      }
    }

    private void DrawAssignmentPhase(float t)
    {
      Utils.FillEverywhereExcept(Utils.InsetRectangle(this.os.terminal.Bounds, 1), Utils.GetFullscreen(), this.spriteBatch, Color.Black * 0.8f);
      float num1 = Utils.CubicInCurve((float) (1.0 - (double) Math.Min(2f, this.TimeInThisState / 2f) / 2.0));
      Rectangle rectangle = Utils.InsetRectangle(this.os.terminal.bounds, (int) (-1.0 * 200.0 * (double) num1));
      float num2 = 1f - num1;
      if ((double) num2 >= 0.800000011920929)
        num2 = (float) ((1.0 - ((double) num2 - 0.800000011920929) * 5.0) * 0.800000011920929);
      RenderedRectangle.doRectangleOutline(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, (int) (60.0 * (0.0599999986588955 + (double) num1)), new Color?(this.os.highlightColor * num2));
      string wholeText = this.AssignmentsCompleteText;
      if (!this.AllAssignmentsComplete)
        wholeText = this.IsOnAssignment1 ? this.Assignment1Text : this.Assignment2Text;
      this.charsRenderedSoFar = TextWriterTimed.WriteTextToTerminal(wholeText, (object) this.os, 0.04f, 1f, 20f, this.TimeInThisState, this.charsRenderedSoFar);
      if (this.charsRenderedSoFar < wholeText.Length)
        return;
      this.StartAssignment();
    }

    private void StartAssignment()
    {
      if (!this.AllAssignmentsComplete)
      {
        if (this.IsOnAssignment1)
        {
          this.PhaseTitle = "74.125.23.121";
          this.PhaseSubtitle = LocaleTerms.Loc("Download Tools") + "\n" + LocaleTerms.Loc("Delete System Files");
        }
        else
        {
          this.PhaseTitle = "216.239.32.181";
          this.PhaseSubtitle = LocaleTerms.Loc("Adapt") + "\n" + LocaleTerms.Loc("Advance");
        }
        this.State = this.IsOnAssignment1 ? DLCIntroExe.IntroState.OnMission1 : DLCIntroExe.IntroState.OnMission2;
        this.TimeInThisState = 0.0f;
        this.LoadedMission = (ActiveMission) ComputerLoader.readMission(this.IsOnAssignment1 ? this.assignment1MissionPath : this.assignment2MissionPath);
      }
      else
        this.CompleteExecution();
    }

    private void CheckProgressOfCurrentAssignment()
    {
      if (this.MissionIsComplete)
      {
        if ((double) this.TimeInThisState < (double) DLCIntroExe.WindDownTimeAfterCompletingMission)
          return;
        this.MissionWasCompleted();
      }
      else if (this.os.connectedComp == null && this.LoadedMission.isComplete((List<string>) null))
      {
        this.TimeInThisState = 0.0f;
        this.MissionIsComplete = true;
      }
    }

    private void MissionWasCompleted()
    {
      if (this.IsOnAssignment1)
      {
        this.State = DLCIntroExe.IntroState.AssignMission2;
      }
      else
      {
        this.State = DLCIntroExe.IntroState.Outro;
        this.AllAssignmentsComplete = true;
        this.GlowSound.Play();
        MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
        for (int index = 0; index < this.os.exes.Count; ++index)
        {
          if (this.os.exes[index] is ShellExe)
          {
            (this.os.exes[index] as ShellExe).Killed();
            (this.os.exes[index] as ShellExe).isExiting = true;
          }
        }
      }
      this.charsRenderedSoFar = 0;
      this.TimeInThisState = 0.0f;
      this.MissionIsComplete = false;
      if (this.IsOnAssignment1)
        this.IsOnAssignment1 = false;
      for (int index = 0; index < this.os.exes.Count; ++index)
      {
        if (this.os.exes[index] != this)
          this.os.exes[index].isExiting = true;
      }
      this.os.execute("clear");
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      this.UpdateState(t);
      Rectangle dest1 = new Rectangle(this.bounds.X + 2, this.bounds.Y + Module.PANEL_HEIGHT + 2, this.bounds.Width - 4, this.bounds.Height - (Module.PANEL_HEIGHT + 4));
      Rectangle dest2 = new Rectangle(this.bounds.X + 2, this.bounds.Y + Module.PANEL_HEIGHT + 10, this.bounds.Width - 4, this.bounds.Height - (Module.PANEL_HEIGHT + 6));
      switch (this.State)
      {
        case DLCIntroExe.IntroState.NotStarted:
          this.BackgroundEffect.Update(t);
          this.BackgroundEffect.Draw(dest2, this.spriteBatch, Color.Black, this.themeColor * 0.2f, HexGridBackground.ColoringAlgorithm.CorrectedSinWash, 0.0f);
          int height1 = 30;
          this.spriteBatch.Draw(Utils.white, new Rectangle(this.bounds.X + 10, this.bounds.Y + this.bounds.Height / 2 - height1 / 2, this.bounds.Width - 20, height1), Color.Black);
          if (!this.os.Flags.HasFlag("KaguyaTrialComplete"))
          {
            if (Button.doButton(8310101 + this.PID, this.bounds.X + 10, this.bounds.Y + this.bounds.Height / 2 - height1 / 2, this.bounds.Width - 20, height1, LocaleTerms.Loc("BEGIN TRIAL"), new Color?(this.os.highlightColor)))
            {
              this.State = DLCIntroExe.IntroState.SpinningUp;
              this.TimeInThisState = 0.0f;
              MusicManager.stop();
              MusicManager.playSongImmediatley("DLC\\Music\\snidelyWhiplash");
              this.os.mailicon.isEnabled = false;
              this.os.thisComputer.links.Clear();
              this.os.traceCompleteOverrideAction += new Action(this.PlayerLostToTraceTimer);
              this.OSTraceTimerOverrideActive = true;
              break;
            }
            break;
          }
          TextItem.doCenteredFontLabel(dest1, LocaleTerms.Loc("Trials Locked"), GuiData.font, Color.White, false);
          if (Button.doButton(8310101 + this.PID, dest1.X + 10, dest1.Y + dest1.Height - 22, dest1.Width - 20, 18, "Exit", new Color?(this.os.lockedColor)))
            this.isExiting = true;
          break;
        case DLCIntroExe.IntroState.SpinningUp:
          Utils.LCG.reSeed(this.PID);
          Rectangle destinationRectangle = new Rectangle(dest1.X, dest1.Y, dest1.Width, 1);
          for (int index = 0; index < dest1.Height; ++index)
          {
            float point = Math.Min(1f, this.TimeInThisState / (Utils.LCG.NextFloatScaled() * DLCIntroExe.SpinUpTime));
            float num1;
            if ((double) Utils.LCG.NextFloatScaled() > 0.5)
            {
              float num2 = 0.8f;
              float num3 = point * (1f - num2);
              if ((double) point > (double) num2)
              {
                float num4 = 1f - num3;
                float num5 = Utils.QuadraticOutCurve((float) (((double) point - (double) num2) / (1.0 - (double) num2)));
                num1 = num3 + num4 * num5;
              }
              else
                num1 = num3;
            }
            else
              num1 = Utils.QuadraticOutCurve(point);
            destinationRectangle.Y = dest1.Y + index;
            destinationRectangle.Width = (int) ((double) num1 * (double) dest1.Width);
            Color color = Color.Lerp(Utils.AddativeWhite * 0.1f, this.themeColor, Utils.LCG.NextFloatScaled());
            this.spriteBatch.Draw(Utils.white, destinationRectangle, color);
          }
          break;
        case DLCIntroExe.IntroState.Flickering:
          this.UpdateUIFlickerIn();
          this.UpdateUIBreaking(t);
          this.DrawPhaseTitle(t, dest2);
          break;
        case DLCIntroExe.IntroState.MailIconPhasingOut:
          this.DrawPhaseTitle(t, dest2);
          this.UpdateMailePhaseOut(t);
          break;
        case DLCIntroExe.IntroState.AssignMission1:
        case DLCIntroExe.IntroState.AssignMission2:
        case DLCIntroExe.IntroState.Outro:
          this.DrawPhaseTitle(t, dest2);
          this.DrawAssignmentPhase(t);
          break;
        case DLCIntroExe.IntroState.OnMission1:
        case DLCIntroExe.IntroState.OnMission2:
          this.DrawPhaseTitle(t, dest2);
          if (Settings.forceCompleteEnabled)
          {
            int height2 = 19;
            if (Button.doButton(8310102, this.bounds.X + 10, this.bounds.Y + height2 + 4, this.bounds.Width - 20, height2, LocaleTerms.Loc("DEBUG: Skip"), new Color?(this.os.highlightColor)))
            {
              this.os.thisComputer.files.root.searchForFolder("bin").files.Add(new FileEntry(PortExploits.crackExeData[6881], PortExploits.cracks[6881]));
              this.CompleteExecution();
            }
            break;
          }
          break;
        case DLCIntroExe.IntroState.Exiting:
          this.DrawPhaseTitle(t, dest2);
          Utils.FillEverywhereExcept(Utils.InsetRectangle(this.os.terminal.Bounds, 1), Utils.GetFullscreen(), this.spriteBatch, Color.Black * 0.8f * (1f - Math.Min(1f, this.TimeInThisState)));
          break;
      }
      this.UpdateImpactEffects(t);
      this.DrawImpactEffects(this.ImpactEffects);
      this.explosion.Render(this.spriteBatch);
    }

    private void DrawPhaseTitle(float t, Rectangle dest)
    {
      this.BackgroundEffect.Update(t);
      this.BackgroundEffect.Draw(dest, this.spriteBatch, Color.Black, this.themeColor * 0.2f, HexGridBackground.ColoringAlgorithm.CorrectedSinWash, 0.0f);
      int height = 40;
      if (dest.Height <= height)
        return;
      Rectangle rectangle1 = new Rectangle(dest.X, dest.Y + (int) ((double) dest.Height / 2.8) - height / 2, dest.Width, height);
      this.spriteBatch.Draw(Utils.white, rectangle1, Color.Black * 0.6f);
      TextItem.doFontLabelToSize(rectangle1, this.PhaseTitle, Utils.GetTitleFontForLocalizedString(this.PhaseTitle), Color.White, true, false);
      if (!this.isExiting)
      {
        string[] strArray = this.PhaseSubtitle.Split(Utils.newlineDelim);
        Rectangle rectangle2 = new Rectangle(rectangle1.X, rectangle1.Y + rectangle1.Height + 2, rectangle1.Width, 22);
        for (int index = 0; index < strArray.Length; ++index)
        {
          this.spriteBatch.Draw(Utils.white, rectangle2, Color.Black * 0.4f);
          TextItem.doFontLabelToSize(rectangle2, strArray[index], GuiData.UISmallfont, Utils.AddativeWhite * 0.9f, true, false);
          rectangle2.Y += rectangle2.Height + 2;
        }
      }
    }

    private void UpdateImpactEffects(float t)
    {
      for (int index = 0; index < this.ImpactEffects.Count; ++index)
      {
        TraceKillExe.PointImpactEffect impactEffect = this.ImpactEffects[index];
        impactEffect.timeEnabled += t;
        if ((double) impactEffect.timeEnabled > (double) DLCIntroExe.NodeImpactEffectTransInTime + (double) DLCIntroExe.NodeImpactEffectTransOutTime)
        {
          this.ImpactEffects.RemoveAt(index);
          --index;
        }
        else
          this.ImpactEffects[index] = impactEffect;
      }
    }

    private void DrawImpactEffects(List<TraceKillExe.PointImpactEffect> Effects)
    {
      Utils.LCG.reSeed(this.PID);
      for (int index = 0; index < Effects.Count; ++index)
      {
        Color color = Color.Lerp(Utils.AddativeWhite, Utils.AddativeRed, (float) (0.600000023841858 + 0.400000005960464 * (double) Utils.LCG.NextFloatScaled())) * (float) (0.600000023841858 + 0.400000005960464 * (double) Utils.LCG.NextFloatScaled());
        TraceKillExe.PointImpactEffect effect = Effects[index];
        Vector2 location = effect.location;
        float num1 = Utils.QuadraticOutCurve(effect.timeEnabled / DLCIntroExe.NodeImpactEffectTransInTime);
        float num2 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(effect.timeEnabled / (DLCIntroExe.NodeImpactEffectTransInTime + DLCIntroExe.NodeImpactEffectTransOutTime)));
        float num3 = Utils.QuadraticOutCurve((effect.timeEnabled - DLCIntroExe.NodeImpactEffectTransInTime) / DLCIntroExe.NodeImpactEffectTransOutTime);
        effect.cne.color = color * num1;
        effect.cne.ScaleFactor = num2 * effect.scaleModifier;
        if ((double) effect.timeEnabled > (double) DLCIntroExe.NodeImpactEffectTransInTime)
          effect.cne.color = color * (1f - num3);
        if ((double) num1 >= 0.0 && effect.HasHighlightCircle)
          this.spriteBatch.Draw(this.circle, location, new Rectangle?(), color * (float) (1.0 - (double) num1 - ((double) num3 >= 0.0 ? 1.0 - (double) num3 : 0.0)), 0.0f, new Vector2((float) (this.circle.Width / 2), (float) (this.circle.Height / 2)), (float) ((double) num1 / (double) this.circle.Width * 60.0), SpriteEffects.None, 0.7f);
        effect.cne.draw(this.spriteBatch, location);
      }
    }

    private enum IntroState
    {
      NotStarted,
      SpinningUp,
      Flickering,
      MailIconPhasingOut,
      AssignMission1,
      OnMission1,
      AssignMission2,
      OnMission2,
      Outro,
      Exiting,
    }
  }
}
