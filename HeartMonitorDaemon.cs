// Decompiled with JetBrains decompiler
// Type: Hacknet.HeartMonitorDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Daemons.Helpers;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class HeartMonitorDaemon : Daemon
  {
    private List<Action<int, int, SpriteBatch>> PostBloomDrawCalls = new List<Action<int, int, SpriteBatch>>();
    private Color bloomColor = new Color(10, 10, 10, 0);
    private Color heartColor = new Color(247, 237, 125);
    private bool HasSecondaryLogin = false;
    public string PatientID = "UNKNOWN";
    private bool PatientInCardiacArrest = false;
    private bool PatientDead = false;
    private float PatientTimeInDanger = 0.0f;
    private float timeDead = 0.0f;
    private float timeTillNextHeartbeat = 0.0f;
    private float timeBetweenHeartbeats = 0.8823529f;
    private float beatTime = 0.18f;
    private float volume = 0.4f;
    private float opTransition = 0.0f;
    private bool opOpening = false;
    private string loginUsername = (string) null;
    private string loginPass = (string) null;
    private float firmwareLoadTime = 0.0f;
    private int selectedFirmwareIndex = 0;
    private bool isConfirmingSelection = false;
    private string selectedFirmwareName = "";
    private string selectedFirmwareData = "";
    private List<IMedicalMonitor> Monitors = new List<IMedicalMonitor>();
    private float projectionFowardsTime = 0.3f;
    private float timeSinceLastHeartBeat = float.MaxValue;
    private int HeartRate = 0;
    private float currentSPO2 = 0.0f;
    private float averageSPO2 = 0.0f;
    private int reportedSP02 = 95;
    private float timeSinceNormalHeartRate = 0.0f;
    private float alarmHeartOKTimer = 0.0f;
    private HeartMonitorDaemon.HeartMonitorState State = HeartMonitorDaemon.HeartMonitorState.Welcome;
    private float timeThisState = 0.0f;
    private const string ActiveFirmwareFilename = "LiveFirmware.dll";
    private const string FolderName = "KBT_Pacemaker";
    private const string LiveFolderName = "Active";
    private const string SubLoginUsername = "EAdmin";
    private const string SubLoginPass = "tens86";
    private const float MinFirmwareLoadTime = 10f;
    private const float TimeToDeathInDanger = 21f;
    private const float DyingDangerTime = 6f;
    private const float DeadBeepSustainFadeOut = 16f;
    private const float DeadBeepSustainFadeoutStartDelay = 5f;
    private Texture2D Heart;
    private Texture2D OxyIcon;
    private Texture2D WarnIcon;
    private Effect blufEffect;
    private RenderTarget2D bloomTarget;
    private RenderTarget2D priorTarget;
    private RenderTarget2D secondaryBloomTarget;
    private SpriteBatch BlurContentSpritebatch;
    private SoundEffect beepSound;
    private SoundEffectInstance beepSustainSound;
    private BasicMedicalMonitor HeartMonitor;
    private BasicMedicalMonitor BPMonitor;
    private BasicMedicalMonitor SPMonitor;

    public HeartMonitorDaemon(Computer c, OS os)
      : base(c, LocaleTerms.Loc("Remote Monitor"), os)
    {
      this.blufEffect = os.content.Load<Effect>("Shaders/DOFBlur");
      this.blufEffect.CurrentTechnique = this.blufEffect.Techniques["SmoothGaussBlur"];
      this.Heart = os.content.Load<Texture2D>("Sprites/Icons/Heart");
      this.OxyIcon = os.content.Load<Texture2D>("Sprites/Icons/O2Img");
      this.WarnIcon = os.content.Load<Texture2D>("Sprites/Icons/CautionIcon");
      this.beepSound = os.content.Load<SoundEffect>("SFX/HeartMonitorBeep");
      this.beepSustainSound = os.content.Load<SoundEffect>("SFX/HeartMonitorSustain").CreateInstance();
      this.beepSustainSound.IsLooped = true;
      this.beepSustainSound.Volume = this.volume;
      this.SetUpMonitors();
    }

    public override string getSaveString()
    {
      return "<HeartMonitor patient=\"" + this.PatientID + "\" />";
    }

    public override void initFiles()
    {
      base.initFiles();
      Folder folder1 = this.comp.files.root.searchForFolder("KBT_Pacemaker");
      if (folder1 == null)
      {
        folder1 = new Folder("KBT_Pacemaker");
        this.comp.files.root.folders.Add(folder1);
      }
      Folder folder2 = folder1.searchForFolder("Active") ?? new Folder("Active");
      folder1.folders.Add(folder2);
      FileEntry fileEntry1 = new FileEntry(PortExploits.ValidPacemakerFirmware, "KBT_Firmware_v1.2.dll");
      folder1.files.Add(fileEntry1);
      FileEntry fileEntry2 = new FileEntry(PortExploits.ValidPacemakerFirmware, "LiveFirmware.dll");
      folder2.files.Add(fileEntry1);
    }

    private void SetUpMonitors()
    {
      this.HeartMonitor = new BasicMedicalMonitor((Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>>) ((lastVal, dt) =>
      {
        List<BasicMedicalMonitor.MonitorRecordKeypoint> monitorRecordKeypointList = new List<BasicMedicalMonitor.MonitorRecordKeypoint>();
        BasicMedicalMonitor.MonitorRecordKeypoint monitorRecordKeypoint = new BasicMedicalMonitor.MonitorRecordKeypoint();
        monitorRecordKeypoint.timeOffset = dt;
        monitorRecordKeypoint.value = lastVal;
        float num = this.PatientDead ? 0.05f : 0.25f;
        if ((double) lastVal > (double) num)
          monitorRecordKeypoint.value -= dt * 0.5f;
        else if ((double) lastVal < -1.0 * (double) num)
          monitorRecordKeypoint.value += (float) ((double) dt * 0.5 * (this.PatientDead ? 0.5 * (double) Math.Max((float) (1.0 - (double) this.timeDead / 16.0), 0.0f) : 1.0));
        else
          monitorRecordKeypoint.value += (float) (((double) Utils.randm(0.2f) - 0.100000001490116) * 0.300000011920929);
        monitorRecordKeypointList.Add(monitorRecordKeypoint);
        return monitorRecordKeypointList;
      }), (Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>>) ((lastVal, dt) =>
      {
        List<BasicMedicalMonitor.MonitorRecordKeypoint> monitorRecordKeypointList = new List<BasicMedicalMonitor.MonitorRecordKeypoint>();
        if (!this.PatientDead)
        {
          monitorRecordKeypointList.Add(new BasicMedicalMonitor.MonitorRecordKeypoint()
          {
            timeOffset = dt / 3f,
            value = (float) ((double) Utils.randm(0.2f) - 0.100000001490116 - 0.800000011920929)
          });
          monitorRecordKeypointList.Add(new BasicMedicalMonitor.MonitorRecordKeypoint()
          {
            timeOffset = dt / 3f,
            value = (float) (0.899999976158142 + ((double) Utils.randm(0.1f) - 0.0500000007450581))
          });
          monitorRecordKeypointList.Add(new BasicMedicalMonitor.MonitorRecordKeypoint()
          {
            timeOffset = dt / 3f,
            value = Utils.randm(0.1f * dt) - 0.05f
          });
        }
        return monitorRecordKeypointList;
      }));
      this.Monitors.Add((IMedicalMonitor) this.HeartMonitor);
      this.BPMonitor = new BasicMedicalMonitor((Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>>) ((lastVal, dt) =>
      {
        List<BasicMedicalMonitor.MonitorRecordKeypoint> monitorRecordKeypointList = new List<BasicMedicalMonitor.MonitorRecordKeypoint>();
        BasicMedicalMonitor.MonitorRecordKeypoint monitorRecordKeypoint = new BasicMedicalMonitor.MonitorRecordKeypoint();
        monitorRecordKeypoint.timeOffset = dt;
        monitorRecordKeypoint.value = lastVal;
        if ((double) lastVal > 0.25)
          monitorRecordKeypoint.value -= dt * 0.5f;
        else if ((double) lastVal < -0.25)
          monitorRecordKeypoint.value += dt * 0.5f;
        else
          monitorRecordKeypoint.value += (float) (((double) Utils.randm(0.2f) - 0.100000001490116) * 0.300000011920929 * (this.PatientDead ? 0.5 * (double) Math.Max((float) (1.0 - (double) this.timeDead / 16.0), 0.0f) : 1.0));
        monitorRecordKeypointList.Add(monitorRecordKeypoint);
        return monitorRecordKeypointList;
      }), (Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>>) ((lastVal, dt) => new List<BasicMedicalMonitor.MonitorRecordKeypoint>()
      {
        new BasicMedicalMonitor.MonitorRecordKeypoint()
        {
          timeOffset = dt / 3f,
          value = (float) ((double) Utils.randm(0.2f) - 0.100000001490116 - 0.800000011920929)
        },
        new BasicMedicalMonitor.MonitorRecordKeypoint()
        {
          timeOffset = dt / 3f,
          value = (float) (0.899999976158142 + ((double) Utils.randm(0.1f) - 0.0500000007450581))
        },
        new BasicMedicalMonitor.MonitorRecordKeypoint()
        {
          timeOffset = dt / 3f,
          value = Utils.randm(0.1f * dt) - 0.05f
        }
      }));
      this.Monitors.Add((IMedicalMonitor) this.BPMonitor);
      this.SPMonitor = new BasicMedicalMonitor((Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>>) ((lastVal, dt) =>
      {
        List<BasicMedicalMonitor.MonitorRecordKeypoint> monitorRecordKeypointList = new List<BasicMedicalMonitor.MonitorRecordKeypoint>();
        BasicMedicalMonitor.MonitorRecordKeypoint monitorRecordKeypoint = new BasicMedicalMonitor.MonitorRecordKeypoint();
        monitorRecordKeypoint.timeOffset = dt;
        monitorRecordKeypoint.value = lastVal;
        if ((double) lastVal > 0.800000011920929)
          monitorRecordKeypoint.value += (float) (((double) Utils.randm(0.2f) - 0.100000001490116) * 0.300000011920929);
        else
          monitorRecordKeypoint.value += (float) (0.150000005960464 * (Utils.random.NextDouble() * Utils.random.NextDouble()) * (this.PatientDead ? 0.5 * (double) Math.Max((float) (1.0 - (double) this.timeDead / 16.0), 0.0f) : 1.0));
        monitorRecordKeypointList.Add(monitorRecordKeypoint);
        return monitorRecordKeypointList;
      }), (Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>>) ((lastVal, dt) =>
      {
        List<BasicMedicalMonitor.MonitorRecordKeypoint> monitorRecordKeypointList = new List<BasicMedicalMonitor.MonitorRecordKeypoint>();
        BasicMedicalMonitor.MonitorRecordKeypoint monitorRecordKeypoint = new BasicMedicalMonitor.MonitorRecordKeypoint();
        monitorRecordKeypoint.timeOffset = dt * 0.7f;
        monitorRecordKeypoint.value = -0.6f - Utils.randm(0.4f);
        monitorRecordKeypointList.Add(monitorRecordKeypoint);
        monitorRecordKeypointList.Add(new BasicMedicalMonitor.MonitorRecordKeypoint()
        {
          timeOffset = dt * 0.3f,
          value = (float) ((double) monitorRecordKeypoint.value - (double) Utils.randm(0.15f) - 0.0500000007450581)
        });
        return monitorRecordKeypointList;
      }));
      this.Monitors.Add((IMedicalMonitor) this.SPMonitor);
    }

    private void UpdateReports(float dt)
    {
      this.timeSinceLastHeartBeat += dt;
      this.currentSPO2 = 1f - this.SPMonitor.GetCurrentValue(this.projectionFowardsTime);
      this.averageSPO2 = (float) ((double) this.averageSPO2 * 0.959999978542328 + (double) this.currentSPO2 * 0.0399999991059303);
      this.reportedSP02 = Math.Min(100, 90 + (int) (5.0 * (double) this.averageSPO2 + 0.5));
      if (this.HeartRate > 120 || this.HeartRate < 50)
      {
        this.timeSinceNormalHeartRate += dt;
      }
      else
      {
        this.alarmHeartOKTimer += dt;
        if ((double) this.alarmHeartOKTimer > 10.0)
        {
          this.timeSinceNormalHeartRate = 0.0f;
          this.alarmHeartOKTimer = 0.0f;
        }
      }
    }

    private void UpdateReportsForHeartbeat()
    {
      this.HeartRate = (int) (60.0 / (double) this.timeSinceLastHeartBeat + 0.5);
      this.timeSinceLastHeartBeat = 0.0f;
    }

    private void ChangeState(HeartMonitorDaemon.HeartMonitorState newState)
    {
      if (this.State == HeartMonitorDaemon.HeartMonitorState.MainDisplay && newState == HeartMonitorDaemon.HeartMonitorState.MainDisplay)
        return;
      if (newState == HeartMonitorDaemon.HeartMonitorState.MainDisplay)
      {
        if (this.State != HeartMonitorDaemon.HeartMonitorState.Welcome)
        {
          this.opOpening = false;
          this.opTransition = 0.0f;
        }
        else
        {
          this.opOpening = false;
          this.opTransition = 2f;
        }
      }
      else if (this.State == HeartMonitorDaemon.HeartMonitorState.MainDisplay)
      {
        this.opOpening = true;
        this.opTransition = 0.0f;
      }
      this.State = newState;
      this.timeThisState = 0.0f;
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      this.ChangeState(HeartMonitorDaemon.HeartMonitorState.Welcome);
      if (!this.os.Flags.HasFlag(this.PatientID + ":DEAD"))
        return;
      this.PatientDead = true;
    }

    private void UpdateStates(float dt)
    {
      this.timeThisState += dt;
      switch (this.State)
      {
        case HeartMonitorDaemon.HeartMonitorState.MainDisplay:
          this.opTransition += (this.opOpening ? 1f : 2f) * dt;
          break;
        case HeartMonitorDaemon.HeartMonitorState.SecondaryLogin:
        case HeartMonitorDaemon.HeartMonitorState.SecondaryLoginRunning:
        case HeartMonitorDaemon.HeartMonitorState.Error:
        case HeartMonitorDaemon.HeartMonitorState.FirmwareScreen:
        case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenConfirm:
          this.opTransition += dt;
          break;
        case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenLoading:
          this.opTransition += dt;
          this.firmwareLoadTime += dt * (float) Utils.random.NextDouble();
          if ((double) this.firmwareLoadTime <= 10.0)
            break;
          this.EnactFirmwareChange();
          this.ChangeState(HeartMonitorDaemon.HeartMonitorState.FirmwareScreenComplete);
          this.firmwareLoadTime = 0.0f;
          break;
        case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenComplete:
          this.opTransition += dt;
          this.firmwareLoadTime += dt;
          if ((double) this.firmwareLoadTime <= 3.33333325386047)
            break;
          this.ChangeState(HeartMonitorDaemon.HeartMonitorState.MainDisplay);
          break;
      }
    }

    public void ForceStopBeepSustainSound()
    {
      if (this.beepSustainSound == null)
        return;
      this.beepSustainSound.Stop();
    }

    private void Update(float dt)
    {
      this.os.delayer.Post(ActionDelayer.Wait(this.os.lastGameTime.ElapsedGameTime.TotalSeconds * 1.999), (Action) (() =>
      {
        if (!(this.os.display.command != this.name) || !this.PatientDead)
          return;
        this.beepSustainSound.Stop(true);
      }));
      if (this.PatientInCardiacArrest)
      {
        this.PatientTimeInDanger += dt;
        if ((double) this.PatientTimeInDanger > 21.0)
        {
          this.PatientDead = true;
          this.HeartRate = 0;
          this.os.Flags.AddFlag(this.PatientID + ":DEAD");
          this.beepSustainSound.Play();
        }
        else
        {
          float num = 3.5f;
          if ((double) this.PatientTimeInDanger - (double) dt < (double) num && (double) this.PatientTimeInDanger >= (double) num && this.os.currentMission.postingTitle == "Project Junebug")
          {
            MusicManager.FADE_TIME = 10f;
            MusicManager.transitionToSong("Music/Ambient/dark_drone_008");
          }
        }
      }
      else
        this.PatientTimeInDanger = 0.0f;
      this.timeTillNextHeartbeat -= dt;
      if ((double) this.timeTillNextHeartbeat <= 0.0 && !this.PatientDead)
      {
        this.timeTillNextHeartbeat = this.timeBetweenHeartbeats + (Utils.randm(0.1f) - 0.05f);
        if (this.PatientInCardiacArrest)
          this.timeTillNextHeartbeat = (double) this.PatientTimeInDanger <= 15.0 ? Utils.randm(this.timeBetweenHeartbeats * 2f) : (float) (0.360000014305115 - 0.25 * ((double) this.PatientTimeInDanger / 21.0));
        this.projectionFowardsTime += this.beatTime + dt;
        for (int index = 0; index < this.Monitors.Count; ++index)
          this.Monitors[index].HeartBeat(this.beatTime);
        this.UpdateReportsForHeartbeat();
        if (this.State != HeartMonitorDaemon.HeartMonitorState.Welcome)
          this.os.delayer.Post(ActionDelayer.Wait((double) this.projectionFowardsTime + (double) this.beatTime / 4.0), (Action) (() => this.beepSound.Play(this.volume, 0.0f, 0.0f)));
      }
      else if ((double) this.projectionFowardsTime > 0.300000011920929)
      {
        this.projectionFowardsTime -= dt;
      }
      else
      {
        for (int index = 0; index < this.Monitors.Count; ++index)
          this.Monitors[index].Update(dt);
      }
      if (this.PatientDead)
      {
        this.timeDead += dt;
        if ((double) this.timeDead > 5.0)
          this.beepSustainSound.Volume = Math.Max(0.0f, (float) (1.0 - ((double) this.timeDead - 5.0) / 16.0) * this.volume);
      }
      this.UpdateStates(dt);
      this.UpdateReports(dt);
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      this.Update((float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
      if (this.bloomTarget == null || this.bloomTarget.Width != bounds.Width || this.bloomTarget.Height != bounds.Height)
      {
        this.bloomTarget = new RenderTarget2D(sb.GraphicsDevice, bounds.Width, bounds.Height);
        this.secondaryBloomTarget = new RenderTarget2D(sb.GraphicsDevice, bounds.Width, bounds.Height);
      }
      if (this.BlurContentSpritebatch == null)
        this.BlurContentSpritebatch = new SpriteBatch(sb.GraphicsDevice);
      bool drawShadow = TextItem.DrawShadow;
      TextItem.DrawShadow = false;
      this.PostBloomDrawCalls.Clear();
      this.StartBloomDraw(this.BlurContentSpritebatch);
      Rectangle rectangle = bounds;
      rectangle.X = rectangle.Y = 0;
      SpriteBatch spriteBatch = GuiData.spriteBatch;
      GuiData.spriteBatch = this.BlurContentSpritebatch;
      this.DrawStates(rectangle, this.BlurContentSpritebatch);
      GuiData.spriteBatch = spriteBatch;
      this.EndBloomDraw(bounds, rectangle, sb, this.BlurContentSpritebatch);
      for (int index = 0; index < this.PostBloomDrawCalls.Count; ++index)
        this.PostBloomDrawCalls[index](bounds.X, bounds.Y, sb);
      TextItem.DrawShadow = drawShadow;
    }

    private void DrawStates(Rectangle bounds, SpriteBatch sb)
    {
      switch (this.State)
      {
        case HeartMonitorDaemon.HeartMonitorState.Welcome:
          this.DrawWelcomeScreen(bounds, sb);
          break;
        default:
          this.DrawSegments(bounds, sb);
          break;
      }
      if (this.State == HeartMonitorDaemon.HeartMonitorState.Welcome)
        return;
      this.DrawOptionsPanel(bounds, sb);
    }

    private void StartBloomDraw(SpriteBatch sb)
    {
      this.priorTarget = (RenderTarget2D) sb.GraphicsDevice.GetRenderTargets()[0].RenderTarget;
      sb.GraphicsDevice.SetRenderTarget(this.bloomTarget);
      sb.GraphicsDevice.Clear(Color.Transparent);
      sb.Begin();
    }

    private void EndBloomDraw(Rectangle bounds, Rectangle zeroedBounds, SpriteBatch mainSB, SpriteBatch bloomContentSpritebatch)
    {
      bloomContentSpritebatch.End();
      mainSB.GraphicsDevice.SetRenderTarget(this.secondaryBloomTarget);
      mainSB.GraphicsDevice.Clear(Color.Transparent);
      bloomContentSpritebatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, this.blufEffect);
      zeroedBounds.X -= 2;
      bloomContentSpritebatch.Draw((Texture2D) this.bloomTarget, zeroedBounds, this.bloomColor);
      zeroedBounds.X += 4;
      bloomContentSpritebatch.Draw((Texture2D) this.bloomTarget, zeroedBounds, this.bloomColor);
      zeroedBounds.X -= 2;
      zeroedBounds.Y -= 2;
      bloomContentSpritebatch.Draw((Texture2D) this.bloomTarget, zeroedBounds, this.bloomColor);
      zeroedBounds.Y += 4;
      bloomContentSpritebatch.Draw((Texture2D) this.bloomTarget, zeroedBounds, this.bloomColor);
      bloomContentSpritebatch.End();
      mainSB.GraphicsDevice.SetRenderTarget(this.priorTarget);
      mainSB.Draw((Texture2D) this.bloomTarget, bounds, Color.White);
      mainSB.Draw((Texture2D) this.secondaryBloomTarget, bounds, Color.White);
    }

    public void DrawSegments(Rectangle bounds, SpriteBatch sb)
    {
      int num = 26;
      Rectangle dest1 = bounds;
      dest1.Height = bounds.Height / 4 - 4;
      this.DrawGraph((IMedicalMonitor) this.HeartMonitor, dest1, this.heartColor, sb, true);
      Rectangle rectangle = dest1;
      rectangle.Y += rectangle.Height + 2;
      Rectangle HRM_DisplayBounds = rectangle;
      HRM_DisplayBounds.Width = rectangle.Width / 3;
      bool showingHeartIcon = (double) this.timeSinceLastHeartBeat > (double) this.beatTime * 1.60000002384186 && (double) this.timeSinceLastHeartBeat < 4.0 * (double) this.beatTime;
      this.PostBloomDrawCalls.Add((Action<int, int, SpriteBatch>) ((x, y, sprBatch) =>
      {
        Rectangle bounds1 = HRM_DisplayBounds;
        bounds1.X += x;
        bounds1.Y += y;
        this.DrawMonitorNumericalDisplay(bounds1, "HR", string.Concat((object) this.HeartRate), sprBatch, this.heartColor, showingHeartIcon ? this.Heart : (Texture2D) null);
      }));
      Rectangle statusMonitorBounds = rectangle;
      statusMonitorBounds.Width -= HRM_DisplayBounds.Width + 8 + num;
      statusMonitorBounds.X += HRM_DisplayBounds.Width + 4 + num;
      this.PostBloomDrawCalls.Add((Action<int, int, SpriteBatch>) ((x, y, sprBatch) =>
      {
        Rectangle bounds1 = statusMonitorBounds;
        bounds1.X += x;
        bounds1.Y += y;
        string str = "OK";
        Color col = Color.Gray;
        Texture2D icon = (Texture2D) null;
        if ((double) this.timeSinceNormalHeartRate > 0.5)
        {
          if ((double) this.timeSinceNormalHeartRate > 4.0)
          {
            str = "DANGER";
            col = Color.Red;
            if ((double) this.os.timer % 0.300000011920929 < 0.100000001490116)
              icon = this.WarnIcon;
          }
          else
          {
            str = "WARN";
            col = Color.Yellow;
            if ((double) this.os.timer % 0.5 < 0.200000002980232)
              icon = this.WarnIcon;
          }
        }
        this.DrawMonitorStatusPanelDisplay(bounds1, "ALARM", str, sprBatch, col, icon);
      }));
      Color BPColor = new Color(148, 231, 243);
      Rectangle BP_DisplayBounds = HRM_DisplayBounds;
      BP_DisplayBounds.Y += BP_DisplayBounds.Height + 4;
      this.PostBloomDrawCalls.Add((Action<int, int, SpriteBatch>) ((x, y, sprBatch) =>
      {
        Rectangle bounds1 = BP_DisplayBounds;
        bounds1.X += x;
        bounds1.Y += y;
        this.DrawMonitorNumericalDisplay(bounds1, "BP", "133 : 97\n\n  (109)", sprBatch, BPColor, (Texture2D) null);
      }));
      Rectangle dest2 = statusMonitorBounds;
      dest2.Y += dest2.Height + 4;
      this.DrawGraph((IMedicalMonitor) this.BPMonitor, dest2, BPColor, sb, true);
      Color SPColor = new Color(165, 241, 138);
      Rectangle SP_DisplayBounds = BP_DisplayBounds;
      SP_DisplayBounds.Y += SP_DisplayBounds.Height + 4;
      this.PostBloomDrawCalls.Add((Action<int, int, SpriteBatch>) ((x, y, sprBatch) =>
      {
        Rectangle bounds1 = SP_DisplayBounds;
        bounds1.X += x;
        bounds1.Y += y;
        this.DrawMonitorNumericalDisplay(bounds1, "Sp02", string.Concat((object) this.reportedSP02), sprBatch, SPColor, this.reportedSP02 >= 91 ? this.OxyIcon : (Texture2D) null);
      }));
      Rectangle dest3 = dest2;
      dest3.Y += dest3.Height + 4;
      this.DrawGraph((IMedicalMonitor) this.SPMonitor, dest3, SPColor, sb, true);
    }

    private void DrawMonitorNumericalDisplay(Rectangle bounds, string display, string value, SpriteBatch sb, Color col, Texture2D icon = null)
    {
      float heightTo = 45f;
      Vector2 vector2 = TextItem.doMeasuredFontLabel(new Vector2((float) bounds.X + 2f, (float) bounds.Y + 2f), display, GuiData.font, new Color?(col), (float) bounds.Width - 12f, heightTo);
      int num = 8;
      Rectangle destinationRectangle1 = new Rectangle((int) ((double) bounds.X + (double) vector2.X + 9.0), bounds.Y + num, 8, (int) heightTo - 2 * num);
      sb.Draw(Utils.white, destinationRectangle1, Color.DarkRed);
      if (icon != null)
        sb.Draw(icon, new Rectangle(bounds.X + 2, (int) ((double) bounds.Y + (double) vector2.Y + 2.0), (int) heightTo, (int) heightTo), col);
      TextItem.doFontLabelToSize(new Rectangle(bounds.X + bounds.Width / 3, bounds.Y + bounds.Height / 3, (int) ((double) bounds.Width * 0.6), (int) ((double) bounds.Height * 0.6)), value, GuiData.titlefont, col, false, false);
      Rectangle destinationRectangle2 = new Rectangle(bounds.X + 4, bounds.Y + bounds.Height + 2, bounds.Width - 8, 1);
      sb.Draw(Utils.white, destinationRectangle2, Color.Gray);
    }

    private void DrawMonitorStatusPanelDisplay(Rectangle bounds, string display, string value, SpriteBatch sb, Color col, Texture2D icon = null)
    {
      float heightTo = 45f;
      Vector2 vector2 = TextItem.doMeasuredFontLabel(new Vector2((float) bounds.X + 2f, (float) bounds.Y + 2f), display, GuiData.font, new Color?(col), (float) bounds.Width - 12f, heightTo);
      int num = 8;
      Rectangle destinationRectangle1 = new Rectangle((int) ((double) bounds.X + (double) vector2.X + 9.0), bounds.Y + num, 8, (int) heightTo - 2 * num);
      sb.Draw(Utils.white, destinationRectangle1, Color.DarkRed);
      if (icon != null)
        sb.Draw(icon, new Rectangle(bounds.X + 2, (int) ((double) bounds.Y + (double) vector2.Y + 2.0), (int) heightTo, (int) heightTo), col);
      TextItem.doFontLabelToSize(new Rectangle(bounds.X + (int) ((double) heightTo * 1.5), bounds.Y + bounds.Height / 3, (int) ((double) bounds.Width * 0.3), (int) ((double) bounds.Height * 0.3)), value, GuiData.titlefont, col, false, false);
      Rectangle destinationRectangle2 = new Rectangle(bounds.X + 4, bounds.Y + bounds.Height + 2, bounds.Width - 8, 1);
      sb.Draw(Utils.white, destinationRectangle2, Color.Gray);
    }

    private void DrawGraph(IMedicalMonitor monitor, Rectangle dest, Color col, SpriteBatch sb, bool drawUnderline = true)
    {
      monitor.Draw(dest, sb, col, this.projectionFowardsTime);
      if (!drawUnderline)
        return;
      this.PostBloomDrawCalls.Add((Action<int, int, SpriteBatch>) ((x, y, sprBatch) =>
      {
        Rectangle destinationRectangle = new Rectangle(dest.X + 4 + x, dest.Y + dest.Height + 2 + y, dest.Width - 8, 1);
        sprBatch.Draw(Utils.white, destinationRectangle, Color.Gray);
      }));
    }

    private void EnactFirmwareChange()
    {
      this.PatientInCardiacArrest = this.selectedFirmwareData == PortExploits.DangerousPacemakerFirmware || this.selectedFirmwareData == PortExploits.DangerousPacemakerFirmwareLRNG;
    }

    public void DrawWelcomeScreen(Rectangle bounds, SpriteBatch sb)
    {
      bool hasAdmin = this.comp.adminIP == this.os.thisComputer.ip;
      Color graphColor = Color.Red;
      graphColor.A = (byte) 100;
      if (hasAdmin)
        graphColor = new Color(20, 200, 20, 100);
      Rectangle heartRateDisplay = new Rectangle(bounds.X + 10, bounds.Y + bounds.Height / 4, bounds.Width - 20, bounds.Height / 4);
      this.DrawGraph((IMedicalMonitor) this.HeartMonitor, heartRateDisplay, graphColor, sb, false);
      heartRateDisplay.Y += heartRateDisplay.Height + 10;
      string adminMsg = hasAdmin ? LocaleTerms.Loc("Admin Access Granted") : LocaleTerms.Loc("Admin Access Required");
      adminMsg = adminMsg.ToUpper();
      heartRateDisplay.Height = 20;
      this.DrawLinedMessage(adminMsg, graphColor * 0.2f, heartRateDisplay, sb);
      Rectangle nextToButonsLineThing = new Rectangle(heartRateDisplay.X + heartRateDisplay.Width / 4 - 18, heartRateDisplay.Y + heartRateDisplay.Height + 10, 14, bounds.Height / 3);
      sb.Draw(Utils.white, nextToButonsLineThing, graphColor * 0.2f);
      this.PostBloomDrawCalls.Add((Action<int, int, SpriteBatch>) ((x, y, sprBatch) =>
      {
        Rectangle dest = heartRateDisplay;
        dest.X += x;
        dest.Y += y;
        this.DrawLinedMessage(adminMsg, graphColor, dest, sprBatch);
        nextToButonsLineThing.X += x;
        nextToButonsLineThing.Y += y;
        sprBatch.Draw(Utils.white, nextToButonsLineThing, graphColor);
        dest.Y += dest.Height + 10;
        dest.Width = dest.Width / 2;
        dest.X += dest.Width / 2;
        dest.Height = 28;
        dest.Height *= 2;
        sprBatch.DrawString(GuiData.font, this.comp.name, new Vector2((float) dest.X, (float) dest.Y), Color.White);
        sprBatch.DrawString(GuiData.detailfont, "Kellis Biotech\nB-Type Pacemaker v2.44", new Vector2((float) (dest.X + 2), (float) (dest.Y + 30)), Color.White, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.5f);
        dest.Y += dest.Height + 6;
        dest.Height /= 2;
        if (Button.doButton(686868001, dest.X, dest.Y, dest.Width, dest.Height + 6, LocaleTerms.Loc("View Monitor"), new Color?(hasAdmin ? this.os.highlightColor : Color.Gray)) && hasAdmin)
          this.ChangeState(HeartMonitorDaemon.HeartMonitorState.MainDisplay);
        dest.Y += dest.Height + 10 + 6;
        if (Button.doButton(686868003, dest.X, dest.Y, dest.Width, dest.Height, LocaleTerms.Loc("Admin Login"), new Color?(!hasAdmin ? this.os.highlightColor : Color.Gray)) && !hasAdmin)
          this.os.runCommand("login");
        dest.Y += dest.Height + 10;
        if (!Button.doButton(686868005, dest.X, dest.Y, dest.Width, dest.Height, LocaleTerms.Loc("Exit"), new Color?(this.os.brightLockedColor)))
          return;
        this.os.display.command = "connect";
      }));
    }

    private void DrawLinedMessage(string message, Color col, Rectangle dest, SpriteBatch sb)
    {
      int num = 16;
      SpriteFont smallfont = GuiData.smallfont;
      Vector2 vector2 = smallfont.MeasureString(message);
      vector2.X += (float) num;
      dest.Height = (int) ((double) vector2.Y + 0.5);
      Rectangle destinationRectangle = new Rectangle(dest.X, dest.Y + dest.Height / 2 - 1, dest.Width / 2 - (int) ((double) vector2.X / 2.0), 2);
      sb.Draw(Utils.white, destinationRectangle, col);
      sb.DrawString(smallfont, message, new Vector2((float) (destinationRectangle.X + destinationRectangle.Width + num / 2), (float) (destinationRectangle.Y - dest.Height / 2)), col);
      destinationRectangle.X = dest.X + dest.Width - destinationRectangle.Width;
      sb.Draw(Utils.white, destinationRectangle, col);
    }

    private void DrawOptionsPanel(Rectangle bounds, SpriteBatch spritebatch)
    {
      int buttonWidth = 120;
      float num1 = (float) (bounds.Width - buttonWidth) * 0.7f;
      float point = Math.Min(1f, this.opTransition);
      if (!this.opOpening)
        point = 1f - point;
      float num2 = Utils.QuadraticOutCurve(point);
      bool ShowingContent = (double) num2 >= 0.980000019073486;
      float contentFade = 0.0f;
      float num3 = 0.5f;
      if (ShowingContent)
        contentFade = Math.Min(1f + num3, this.opTransition) - num3;
      int width = (int) ((double) num2 * (double) num1) + buttonWidth;
      Rectangle panelArea = new Rectangle(bounds.X + bounds.Width - width, bounds.Y + bounds.Height / 10, width, bounds.Height - bounds.Height / 5);
      Rectangle buttonSourceRect = panelArea;
      this.PostBloomDrawCalls.Add((Action<int, int, SpriteBatch>) ((x, y, sprBatch) =>
      {
        Rectangle rectangle = buttonSourceRect;
        rectangle.X += x;
        rectangle.Y += y;
        int num4 = bounds.Height / 4 + 12;
        rectangle.Y += num4 - bounds.Height / 10;
        int height = 30;
        Rectangle destinationRectangle = rectangle;
        destinationRectangle.Width = buttonWidth;
        destinationRectangle.Height = height;
        destinationRectangle.X -= 2;
        sprBatch.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray);
        if (Button.doButton(83838001, destinationRectangle.X, destinationRectangle.Y, buttonWidth, height, LocaleTerms.Loc("Login"), new Color?(this.State != HeartMonitorDaemon.HeartMonitorState.SecondaryLogin ? this.os.highlightColor : Color.Black)))
          this.ChangeState(HeartMonitorDaemon.HeartMonitorState.SecondaryLogin);
        destinationRectangle.Y += height + 2;
        sprBatch.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray);
        if (Button.doButton(83838003, destinationRectangle.X, destinationRectangle.Y, buttonWidth, height, LocaleTerms.Loc("Firmware"), new Color?(this.State != HeartMonitorDaemon.HeartMonitorState.FirmwareScreen ? this.os.highlightColor : Color.Black)))
          this.ChangeState(HeartMonitorDaemon.HeartMonitorState.FirmwareScreen);
        destinationRectangle.Y += height + 2;
        sprBatch.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray);
        if (Button.doButton(83838006, destinationRectangle.X, destinationRectangle.Y, buttonWidth, height, LocaleTerms.Loc("Monitor"), new Color?(this.State != HeartMonitorDaemon.HeartMonitorState.MainDisplay ? this.os.highlightColor : Color.Black)))
          this.ChangeState(HeartMonitorDaemon.HeartMonitorState.MainDisplay);
        destinationRectangle.Y += height + 2;
        sprBatch.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray);
        if (Button.doButton(83838009, destinationRectangle.X, destinationRectangle.Y, buttonWidth, height, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
          this.ChangeState(HeartMonitorDaemon.HeartMonitorState.Welcome);
        destinationRectangle.Y += height + 2;
        panelArea.X += x;
        panelArea.Y += y;
        panelArea.X += buttonWidth - 3;
        panelArea.Width -= buttonWidth;
        if (this.opOpening || (double) this.opTransition < 1.0)
          panelArea.Width += 2;
        sprBatch.Draw(Utils.white, panelArea, this.os.outlineColor);
        int num5 = 3;
        panelArea.X += num5;
        panelArea.Width -= 2 * num5;
        panelArea.Y += num5;
        panelArea.Height -= 2 * num5;
        sprBatch.Draw(Utils.white, panelArea, this.os.indentBackgroundColor);
        if (!ShowingContent)
          return;
        this.DrawOptionsPanelContent(panelArea, sprBatch, contentFade);
      }));
    }

    private Rectangle DrawOptionsPanelHeaders(Rectangle bounds, SpriteBatch sb, float contentFade)
    {
      Rectangle rectangle = bounds;
      rectangle.Height = 30;
      rectangle.X += 2;
      rectangle.Width -= 4;
      rectangle.Y += 2;
      string message = LocaleTerms.Loc("Firmware Config");
      this.DrawLinedMessage(message, this.os.highlightColor * contentFade, rectangle, sb);
      rectangle.X -= 2;
      this.DrawLinedMessage(message, this.os.highlightColor * contentFade * 0.2f, rectangle, sb);
      rectangle.X += 4;
      this.DrawLinedMessage(message, this.os.highlightColor * contentFade * 0.2f, rectangle, sb);
      rectangle.X -= 2;
      rectangle.Y += rectangle.Height - 10;
      rectangle.Height = 54;
      string str = LocaleTerms.Loc("Firmware Administration") + "\n" + LocaleTerms.Loc("Access") + " : ";
      Color color1 = this.os.brightLockedColor;
      string text;
      if (this.HasSecondaryLogin)
      {
        text = str + " " + LocaleTerms.Loc("GRANTED");
        color1 = this.os.brightUnlockedColor;
      }
      else
        text = str + " " + LocaleTerms.Loc("DENIED");
      Color color2 = color1 * contentFade;
      sb.DrawString(GuiData.font, text, new Vector2((float) rectangle.X, (float) rectangle.Y), color2, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.6f);
      rectangle.Y += rectangle.Height;
      sb.DrawString(GuiData.tinyfont, LocaleTerms.Loc("Secondary security layer for firmware read/write access"), new Vector2((float) rectangle.X, (float) rectangle.Y), color2 * 0.7f, 0.0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0.6f);
      rectangle.Y += 15;
      rectangle.Height = 1;
      sb.Draw(Utils.white, rectangle, Color.Gray * contentFade * (float) (Utils.random.NextDouble() * 0.15 + 0.280000001192093));
      return new Rectangle(bounds.X, rectangle.Y, bounds.Width, bounds.Y + bounds.Height - rectangle.Y - rectangle.Height);
    }

    private void DrawOptionsPanelContent(Rectangle bounds, SpriteBatch sb, float contentFade)
    {
      Rectangle bounds1 = this.DrawOptionsPanelHeaders(bounds, sb, contentFade);
      switch (this.State)
      {
        case HeartMonitorDaemon.HeartMonitorState.Error:
          break;
        case HeartMonitorDaemon.HeartMonitorState.FirmwareScreen:
        case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenConfirm:
        case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenLoading:
        case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenComplete:
          this.DrawOptionsPanelFirmwareContent(bounds1, sb, contentFade);
          break;
        default:
          this.DrawOptionsPanelLoginContent(bounds1, sb, contentFade);
          break;
      }
    }

    private void DrawOptionsPanelFirmwareContent(Rectangle bounds, SpriteBatch sb, float contentFade)
    {
      Color heartColor = this.heartColor;
      heartColor.A = (byte) 0;
      if (!this.HasSecondaryLogin)
      {
        TextItem.doFontLabelToSize(new Rectangle(bounds.X + bounds.Width / 4, bounds.Y, bounds.Width / 2, bounds.Height), LocaleTerms.Loc("Firmware Administration") + "\n" + LocaleTerms.Loc("Access Required") + "\n" + LocaleTerms.Loc("Log In First"), GuiData.font, heartColor, false, false);
      }
      else
      {
        switch (this.State)
        {
          case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenConfirm:
            break;
          case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenLoading:
            Rectangle bounds1 = bounds;
            ++bounds1.X;
            bounds1.Width -= 2;
            bounds1.Height = 110;
            this.DrawSelectedFirmwareFileDetails(bounds1, sb, this.selectedFirmwareData, this.selectedFirmwareName);
            bounds1.Y += bounds1.Height + 2;
            Rectangle rectangle1 = bounds;
            int num1 = bounds1.Height + 10;
            rectangle1.Height -= num1;
            rectangle1.Y += num1;
            Rectangle destinationRectangle = rectangle1;
            destinationRectangle.Height = 1;
            float num2 = this.firmwareLoadTime / 10f;
            heartColor.A = (byte) 0;
            int num3 = 0;
            while (num3 < rectangle1.Height)
            {
              float num4 = (float) num3 / (float) rectangle1.Height;
              if ((double) num2 > (double) num4)
              {
                destinationRectangle.Y = rectangle1.Y + num3;
                sb.Draw(Utils.white, destinationRectangle, heartColor);
              }
              num3 += 3;
            }
            break;
          case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenComplete:
            Rectangle rectangle2 = bounds;
            rectangle2.X += 2;
            rectangle2.Width -= 4;
            rectangle2.Y += 2;
            rectangle2.Height -= 4;
            sb.Draw(Utils.white, rectangle2, this.os.brightLockedColor * 0.3f * contentFade);
            rectangle2.X += 40;
            rectangle2.Width -= 80;
            TextItem.doFontLabelToSize(rectangle2, LocaleTerms.Loc("FIRMWARE UPDATE COMPLETE"), GuiData.font, Color.White, false, false);
            break;
          default:
            Folder folder = this.comp.files.root.searchForFolder("KBT_Pacemaker");
            string[] text = new string[folder.files.Count + 1];
            text[0] = LocaleTerms.Loc("Currently Active Firmware");
            if (folder.files.Count > 0)
            {
              for (int index = 0; index < folder.files.Count; ++index)
                text[index + 1] = folder.files[index].name;
            }
            this.selectedFirmwareIndex = SelectableTextList.doFancyList(8937001, bounds.X + 2, bounds.Y + 10, (int) ((double) bounds.Width - 4.0), bounds.Height / 3, text, this.selectedFirmwareIndex, new Color?(this.os.topBarColor), false);
            Rectangle rectangle3 = new Rectangle(bounds.X + 2, bounds.Y + 10 + bounds.Height / 3 + 4, bounds.Width - 4, bounds.Height / 4);
            string data = this.selectedFirmwareIndex != 0 ? folder.files[this.selectedFirmwareIndex - 1].data : (string) null;
            string filename = this.selectedFirmwareIndex != 0 ? folder.files[this.selectedFirmwareIndex - 1].name : LocaleTerms.Loc("Currently Active Firmware");
            bool flag = this.DrawSelectedFirmwareFileDetails(rectangle3, sb, data, filename);
            rectangle3.Y += rectangle3.Height + 6;
            if (!flag || this.selectedFirmwareIndex == 0)
              break;
            rectangle3.Height = 30;
            if (!this.isConfirmingSelection)
            {
              if (Button.doButton(8937004, rectangle3.X, rectangle3.Y, rectangle3.Width, rectangle3.Height, LocaleTerms.Loc("Activate This Firmware"), new Color?(this.os.highlightColor)))
                this.isConfirmingSelection = true;
            }
            else
            {
              this.DrawLinedMessage(LocaleTerms.Loc("Confirm Firmware Activation"), this.os.brightLockedColor, rectangle3, sb);
              rectangle3.Y += rectangle3.Height;
              if (Button.doButton(8937008, rectangle3.X, rectangle3.Y, rectangle3.Width, rectangle3.Height, LocaleTerms.Loc("Confirm Activation"), new Color?(this.os.highlightColor)))
              {
                this.selectedFirmwareName = filename;
                this.selectedFirmwareData = data;
                this.ChangeState(HeartMonitorDaemon.HeartMonitorState.FirmwareScreenLoading);
                this.firmwareLoadTime = 0.0f;
                this.isConfirmingSelection = false;
              }
              rectangle3.Y += rectangle3.Height + 4;
              rectangle3.Height = 20;
              if (Button.doButton(8937009, rectangle3.X, rectangle3.Y, rectangle3.Width, rectangle3.Height, LocaleTerms.Loc("Cancel"), new Color?(this.os.lockedColor)))
              {
                this.ChangeState(HeartMonitorDaemon.HeartMonitorState.FirmwareScreen);
                this.isConfirmingSelection = false;
              }
            }
            break;
        }
      }
    }

    private bool DrawSelectedFirmwareFileDetails(Rectangle bounds, SpriteBatch sb, string data, string filename)
    {
      bool flag = data == null || this.IsValidFirmwareData(data);
      Rectangle dest = bounds;
      dest.Height = 28;
      this.DrawLinedMessage(flag ? LocaleTerms.Loc("Valid Firmware File") : LocaleTerms.Loc("Invalid Firmware File"), flag ? this.heartColor : this.os.brightLockedColor, dest, sb);
      dest.Y += dest.Height - 4;
      Color color = Color.White;
      if (!flag)
        color = Color.Gray;
      TextItem.doFontLabel(new Vector2((float) dest.X + 6f, (float) dest.Y), filename, GuiData.font, new Color?(color), (float) dest.Width, (float) dest.Height, false);
      dest.Y += dest.Height + 2;
      dest.Height = 14;
      string text = LocaleTerms.Loc("Invalid binary package") + "\n" + LocaleTerms.Loc("Valid firmware packages must be") + "\n" + LocaleTerms.Loc("digitally signed by an authorized manufacturer");
      if (flag)
        text = LocaleTerms.Loc("Valid binary package") + "\n" + LocaleTerms.Loc("Signed by") + " : KELLIS BIOTECH\n" + LocaleTerms.Loc("Compiled by") + " : EIDOLON SOFT";
      TextItem.doFontLabel(new Vector2((float) dest.X + 6f, (float) dest.Y), text, GuiData.detailfont, new Color?(color * 0.7f), (float) (dest.Width - 12), float.MaxValue, false);
      Rectangle destinationRectangle = bounds;
      destinationRectangle.Y += destinationRectangle.Height - 1;
      destinationRectangle.Height = 1;
      sb.Draw(Utils.white, destinationRectangle, Color.Gray);
      return flag;
    }

    private bool IsValidFirmwareData(string data)
    {
      return data == PortExploits.DangerousPacemakerFirmware || data == PortExploits.ValidPacemakerFirmware;
    }

    private void DrawOptionsPanelLoginContent(Rectangle bounds, SpriteBatch sb, float contentFade)
    {
      string data = LocaleTerms.Loc("A secondary login is required to review and modify running firmware. Personal login details are provided for each chip. If you have lost your login details, connect your support program to our content server") + " (111.105.22.1)";
      Rectangle rectangle1 = new Rectangle(bounds.X + 2, bounds.Y + 10, bounds.Width - 4, 24);
      string text = Utils.SuperSmartTwimForWidth(data, rectangle1.Width, GuiData.detailfont);
      sb.DrawString(GuiData.detailfont, text, new Vector2((float) rectangle1.X, (float) rectangle1.Y), Color.Gray * contentFade);
      Vector2 vector2 = GuiData.detailfont.MeasureString(text);
      rectangle1.Y += (int) ((double) vector2.Y + 10.0);
      if (this.State == HeartMonitorDaemon.HeartMonitorState.SecondaryLoginRunning || this.HasSecondaryLogin)
      {
        Rectangle rectangle2 = rectangle1;
        rectangle2.Height = bounds.Height / 4;
        int height = rectangle2.Height;
        if (this.loginUsername != null)
        {
          rectangle2.Height = 60;
          GetStringUIControl.DrawGetStringControlInactive(LocaleTerms.Loc("Username") + " : ", this.loginUsername, rectangle2, sb, (object) this.os, "");
          rectangle2.Y += rectangle2.Height + 2;
        }
        if (this.loginPass != null)
        {
          rectangle2.Height = 60;
          GetStringUIControl.DrawGetStringControlInactive(LocaleTerms.Loc("Password") + " : ", this.loginPass, rectangle2, sb, (object) this.os, "");
          rectangle2.Y += rectangle2.Height + 2;
        }
        rectangle2.Height = height;
        if (this.loginPass == null || this.loginUsername == null)
        {
          string stringControl = GetStringUIControl.DrawGetStringControl(this.loginUsername == null ? LocaleTerms.Loc("Username") + " : " : LocaleTerms.Loc("Password") + " : ", rectangle2, (Action) (() => this.loginUsername = this.loginPass = ""), (Action) (() => this.loginUsername = this.loginPass = ""), sb, (object) this.os, this.os.highlightColor, this.os.lockedColor, "", new Color?());
          if (stringControl != null)
          {
            if (this.loginUsername == null)
            {
              this.loginUsername = stringControl;
              GetStringUIControl.StartGetString(LocaleTerms.Loc("Password"), (object) this.os);
            }
            else
              this.loginPass = stringControl;
          }
          rectangle2.Y += rectangle2.Height + 10;
        }
        else
        {
          rectangle2.Y += 20;
          this.HasSecondaryLogin = this.loginUsername == "EAdmin" && this.loginPass == "tens86";
          rectangle2.Height = 20;
          this.DrawLinedMessage(this.HasSecondaryLogin ? LocaleTerms.Loc("Login Complete") : LocaleTerms.Loc("Login Failed"), this.HasSecondaryLogin ? this.os.brightUnlockedColor : this.os.brightLockedColor, rectangle2, sb);
          rectangle2.Y += rectangle2.Height + 20;
          if (!this.HasSecondaryLogin)
          {
            if (Button.doButton(92923008, rectangle2.X + 4, rectangle2.Y, rectangle2.Width - 80, 24, LocaleTerms.Loc("Retry Login"), new Color?()))
              this.ChangeState(HeartMonitorDaemon.HeartMonitorState.SecondaryLogin);
          }
          else if (Button.doButton(92923009, rectangle2.X + 4, rectangle2.Y, rectangle2.Width - 80, 24, LocaleTerms.Loc("Administrate Firmware"), new Color?(this.os.highlightColor)))
            this.ChangeState(HeartMonitorDaemon.HeartMonitorState.FirmwareScreen);
        }
      }
      else if (!this.HasSecondaryLogin && (this.ButtonFlashForContentFade(contentFade) && Button.doButton(92923001, rectangle1.X, rectangle1.Y, (int) ((double) rectangle1.Width * 0.699999988079071), 30, LocaleTerms.Loc("Login"), new Color?(this.os.highlightColor))))
      {
        this.loginUsername = this.loginPass = (string) null;
        GetStringUIControl.StartGetString(LocaleTerms.Loc("Username"), (object) this.os);
        this.ChangeState(HeartMonitorDaemon.HeartMonitorState.SecondaryLoginRunning);
      }
    }

    private bool ButtonFlashForContentFade(float contentFade)
    {
      if ((double) contentFade > 0.949999988079071)
        return true;
      return Utils.random.NextDouble() > (double) contentFade;
    }

    private enum HeartMonitorState
    {
      Welcome,
      MainDisplay,
      SecondaryLogin,
      SecondaryLoginRunning,
      Error,
      FirmwareScreen,
      FirmwareScreenConfirm,
      FirmwareScreenLoading,
      FirmwareScreenComplete,
    }
  }
}
