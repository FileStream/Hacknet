// Decompiled with JetBrains decompiler
// Type: Hacknet.OS
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Extensions;
using Hacknet.Factions;
using Hacknet.Gui;
using Hacknet.Localization;
using Hacknet.Misc;
using Hacknet.Mission;
using Hacknet.Modules.Overlays;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace Hacknet
{
  internal class OS : GameScreen
  {
    public static bool DEBUG_COMMANDS = Settings.debugCommandsEnabled;
    public static float EXE_MODULE_HEIGHT = 250f;
    public static float TCP_STAYALIVE_TIMER = 10f;
    public static float WARNING_FLASH_TIME = 2f;
    public static int TOP_BAR_HEIGHT = 21;
    public static bool WillLoadSave = false;
    public static bool TestingPassOnly = false;
    public static double currentElapsedTime = 0.0;
    public static float operationProgress = 0.0f;
    public static object displayObjectCache = (object) null;
    public bool FirstTimeStartup = Settings.slowOSStartup;
    public bool initShowsTutorial = Settings.initShowsTutorial;
    public bool inputEnabled = false;
    public bool isLoaded = false;
    public float PorthackCompleteFlashTime = 0.0f;
    public float MissionCompleteFlashTime = 0.0f;
    private string locationString = "";
    public string username = "";
    public int totalRam = 800 - (OS.TOP_BAR_HEIGHT + 2) - RamModule.contentStartOffset;
    public int ramAvaliable = 800 - (OS.TOP_BAR_HEIGHT + 2);
    public int currentPID = 0;
    internal bool bootingUp = false;
    public List<ActiveMission> branchMissions = new List<ActiveMission>();
    private AudioVisualizer audioVisualizer = new AudioVisualizer();
    public string connectedIP = "";
    public Computer thisComputer = (Computer) null;
    public Computer connectedComp = (Computer) null;
    public Computer opponentComputer = (Computer) null;
    public float warningFlashTimer = 0.0f;
    public List<int> navigationPath = new List<int>();
    public float gameSavedTextAlpha = -1f;
    public string SaveGameUserName = "";
    public string SaveUserAccountName = (string) null;
    public string SaveUserPassword = "password";
    private bool SaveInProgress = false;
    private bool SaveInQueue = false;
    public bool multiplayer = false;
    public bool isServer = false;
    private char[] trimChars = new char[3]{ ' ', '\n', char.MinValue };
    private bool DestroyThreads = false;
    public bool canRunContent = true;
    public float stayAliveTimer = OS.TCP_STAYALIVE_TIMER;
    public string opponentLocation = "";
    public string displayCache = "";
    public string getStringCache = "";
    public bool commandInvalid = false;
    public string connectedIPLastFrame = "";
    public string homeNodeID = "entropy00";
    public string homeAssetServerID = "entropy01";
    public bool DisableTopBarButtons = false;
    public bool DisableEmailIcon = false;
    private string LanguageCreatedIn = "en-us";
    public bool HasExitedAndEnded = false;
    private MessageBoxScreen ExitToMenuMessageBox = (MessageBoxScreen) null;
    public ProgressionFlags Flags = new ProgressionFlags();
    public List<KeyValuePair<string, string>> ActiveHackers = new List<KeyValuePair<string, string>>();
    private int updateErrorCount = 0;
    private int drawErrorCount = 0;
    public bool terminalOnlyMode = false;
    public bool HasLoadedDLCContent = false;
    public bool IsInDLCMode = false;
    public string PreDLCFaction = "entropy";
    public string PreDLCVisibleNodesCache = "";
    public bool IsDLCSave = false;
    public bool IsDLCConventionDemo = false;
    public RunnableConditionalActions ConditionalActions = new RunnableConditionalActions();
    public ActiveEffectsUpdater EffectsUpdater = new ActiveEffectsUpdater();
    public bool ShowDLCAlertsIcon = false;
    public List<OS.TrackerDetail> TrackersInProgress = new List<OS.TrackerDetail>();
    internal Stream ForceLoadOverrideStream = (Stream) null;
    public Color defaultHighlightColor = new Color(0, 139, 199, (int) byte.MaxValue);
    public Color defaultTopBarColor = new Color(130, 65, 27);
    public Color warningColor = Color.Red;
    public Color highlightColor = new Color(0, 139, 199, (int) byte.MaxValue);
    public Color subtleTextColor = new Color(90, 90, 90);
    public Color darkBackgroundColor = new Color(8, 8, 8);
    public Color indentBackgroundColor = new Color(12, 12, 12);
    public Color outlineColor = new Color(68, 68, 68);
    public Color lockedColor = new Color(65, 16, 16, 200);
    public Color brightLockedColor = new Color(160, 0, 0);
    public Color brightUnlockedColor = new Color(0, 160, 0);
    public Color unlockedColor = new Color(39, 65, 36);
    public Color lightGray = new Color(180, 180, 180);
    public Color shellColor = new Color(222, 201, 24);
    public Color shellButtonColor = new Color(105, 167, 188);
    public Color moduleColorSolid = new Color(50, 59, 90, (int) byte.MaxValue);
    public Color moduleColorSolidDefault = new Color(50, 59, 90, (int) byte.MaxValue);
    public Color moduleColorStrong = new Color(14, 28, 40, 80);
    public Color moduleColorBacking = new Color(5, 6, 7, 10);
    public Color topBarColor = new Color(0, 139, 199, (int) byte.MaxValue);
    public Color semiTransText = new Color(120, 120, 120, 0);
    public Color terminalTextColor = new Color(213, 245, (int) byte.MaxValue);
    public Color topBarTextColor = new Color(126, 126, 126, 100);
    public Color superLightWhite = new Color(2, 2, 2, 30);
    public Color connectedNodeHighlight = new Color(222, 0, 0, 195);
    public Color exeModuleTopBar = new Color(130, 65, 27, 80);
    public Color exeModuleTitleText = new Color(155, 85, 37, 0);
    public Color netmapToolTipColor = new Color(213, 245, (int) byte.MaxValue, 0);
    public Color netmapToolTipBackground = new Color(0, 0, 0, 150);
    public Color displayModuleExtraLayerBackingColor = new Color(0, 0, 0, 0);
    public Color topBarIconsColor = Color.White;
    public Color BackgroundImageFillColor = Color.Black;
    public bool UseAspectPreserveBackgroundScaling = false;
    public Color AFX_KeyboardMiddle = new Color(77, 145, (int) byte.MaxValue);
    public Color AFX_KeyboardOuter = new Color(105, 138, (int) byte.MaxValue);
    public Color AFX_WordLogo = new Color(105, 138, (int) byte.MaxValue);
    public Color AFX_Other = new Color(0, 178, (int) byte.MaxValue);
    public Color thisComputerNode = new Color(95, 220, 83);
    public Color scanlinesColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, 15);
    public static OS currentInstance;
    private Texture2D scanLines;
    private Texture2D cross;
    private Texture2D cog;
    private Texture2D saveIcon;
    public List<ShellExe> shells;
    public List<string> shellIPs;
    public CrashModule crashModule;
    public TraceDangerSequence TraceDangerSequence;
    private IntroTextModule introTextModule;
    public GameTime lastGameTime;
    public UserDetail defaultUser;
    public Terminal terminal;
    public NetworkMap netMap;
    public DisplayModule display;
    public RamModule ram;
    private List<Module> modules;
    public IncomingConnectionOverlay IncConnectionOverlay;
    public AircraftInfoOverlay AircraftInfoOverlay;
    public ActiveMission currentMission;
    public TraceTracker traceTracker;
    public List<ExeModule> exes;
    private Rectangle topBar;
    public EndingSequenceModule endingSequence;
    public MailIcon mailicon;
    public Faction currentFaction;
    public AllFactions allFactions;
    public ContentManager content;
    private TcpClient client;
    private byte[] inBuffer;
    private byte[] outBuffer;
    private ASCIIEncoding encoder;
    private Thread listenerThread;
    private NetworkStream netStream;
    private bool multiplayerMissionLoaded;
    public Rectangle fullscreen;
    public bool validCommand;
    public ActionDelayer delayer;
    public float timer;
    public SoundEffect beepSound;
    public HubServerAlertsIcon hubServerAlertsIcon;
    public BootCrashAssistanceModule BootAssitanceModule;
    internal string GibsonIP;
    public Action postFXDrawActions;
    public Action<float> UpdateSubscriptions;
    public Action traceCompleteOverrideAction;

    public OS()
    {
      this.multiplayer = false;
      OS.currentInstance = this;
    }

    public OS(TcpClient socket, NetworkStream stream, bool actingServer, ScreenManager sman)
    {
      this.ScreenManager = sman;
      this.multiplayer = true;
      this.client = socket;
      this.isServer = actingServer;
      this.netStream = stream;
      this.inBuffer = new byte[4096];
      this.outBuffer = new byte[4096];
      this.encoder = new ASCIIEncoding();
      this.canRunContent = false;
      TextBox.cursorPosition = 0;
      OS.currentInstance = this;
    }

    public override void LoadContent()
    {
      if (this.canRunContent)
      {
        this.delayer = new ActionDelayer();
        ComputerLoader.init((object) this);
        this.content = this.ScreenManager.Game.Content;
        this.username = this.SaveUserAccountName == null ? (Settings.isConventionDemo ? Settings.ConventionLoginName : Environment.UserName) : this.SaveUserAccountName;
        this.username = FileSanitiser.purifyStringForDisplay(this.username);
        Vector2 compLocation = new Vector2(0.1f, 0.5f);
        if (this.multiplayer && !this.isServer)
          compLocation = new Vector2(0.8f, 0.8f);
        this.ramAvaliable = this.totalRam;
        string str = !this.multiplayer || !this.isServer ? NetworkMap.generateRandomIP() : NetworkMap.generateRandomIP();
        this.thisComputer = new Computer(this.username + " PC", NetworkMap.generateRandomIP(), compLocation, 5, (byte) 4, this);
        this.thisComputer.adminIP = this.thisComputer.ip;
        this.thisComputer.idName = "playerComp";
        this.thisComputer.Memory = new MemoryContents();
        Folder folder1 = this.thisComputer.files.root.searchForFolder("home");
        folder1.folders.Add(new Folder("stash"));
        folder1.folders.Add(new Folder("misc"));
        this.GibsonIP = NetworkMap.generateRandomIP();
        UserDetail user = this.thisComputer.users[0];
        user.known = true;
        this.thisComputer.users[0] = user;
        this.defaultUser = new UserDetail(this.username, "password", (byte) 1);
        this.defaultUser.known = true;
        OSTheme theme = OSTheme.HacknetBlue;
        if (Settings.isConventionDemo && !this.IsDLCConventionDemo && Settings.ShuffleThemeOnDemoStart)
        {
          double num = Utils.random.NextDouble();
          if (num < 0.25)
            theme = OSTheme.HacknetMint;
          else if (num < 0.5)
            theme = OSTheme.HackerGreen;
          else if (num < 0.75)
            theme = OSTheme.HacknetPurple;
        }
        ThemeManager.setThemeOnComputer((object) this.thisComputer, theme);
        if (this.multiplayer)
        {
          this.thisComputer.addMultiplayerTargetFile();
          this.sendMessage("newComp #" + this.thisComputer.ip + "#" + (object) (int) compLocation.X + "#" + (object) (int) compLocation.Y + "#" + (object) 5 + "#" + this.thisComputer.name);
          this.multiplayerMissionLoaded = false;
        }
        if (!OS.WillLoadSave)
          People.init();
        this.modules = new List<Module>();
        this.exes = new List<ExeModule>();
        this.shells = new List<ShellExe>();
        this.shellIPs = new List<string>();
        Viewport viewport1 = this.ScreenManager.GraphicsDevice.Viewport;
        int moduleWidth = RamModule.MODULE_WIDTH;
        int height1 = 205;
        int width1 = (int) ((double) (viewport1.Width - moduleWidth - 6) * 0.4442);
        int num1 = (int) ((double) (viewport1.Width - moduleWidth - 6) * 0.5558);
        int height2 = viewport1.Height - height1 - OS.TOP_BAR_HEIGHT - 6;
        this.terminal = new Terminal(new Rectangle(viewport1.Width - 2 - width1, OS.TOP_BAR_HEIGHT, width1, viewport1.Height - OS.TOP_BAR_HEIGHT - 2), this);
        this.terminal.name = "TERMINAL";
        this.modules.Add((Module) this.terminal);
        this.netMap = new NetworkMap(new Rectangle(moduleWidth + 4, viewport1.Height - height1 - 2, num1 - 1, height1), this);
        this.netMap.name = "netMap v1.7";
        this.modules.Add((Module) this.netMap);
        this.display = new DisplayModule(new Rectangle(moduleWidth + 4, OS.TOP_BAR_HEIGHT, num1 - 2, height2), this);
        this.display.name = "DISPLAY";
        this.modules.Add((Module) this.display);
        this.ram = new RamModule(new Rectangle(2, OS.TOP_BAR_HEIGHT, moduleWidth, this.ramAvaliable + RamModule.contentStartOffset), this);
        this.ram.name = "RAM";
        this.modules.Add((Module) this.ram);
        for (int index = 0; index < this.modules.Count; ++index)
          this.modules[index].LoadContent();
        if (!Settings.IsInExtensionMode)
        {
          for (int index = 0; index < 2; ++index)
          {
            if (this.isServer || !this.multiplayer)
              this.thisComputer.links.Add(index);
            else
              this.thisComputer.links.Add(this.netMap.nodes.Count - 1 - index);
          }
        }
        if (this.allFactions == null)
        {
          this.allFactions = new AllFactions();
          this.allFactions.init();
        }
        if (!Settings.IsInExtensionMode)
          this.currentFaction = this.allFactions.factions[this.allFactions.currentFaction];
        bool needsNodeInjection = false;
        if (!OS.WillLoadSave)
        {
          this.netMap.nodes.Insert(0, this.thisComputer);
          this.netMap.visibleNodes.Add(0);
          if (!Settings.IsInExtensionMode)
            MusicManager.loadAsCurrentSong(this.IsDLCConventionDemo ? "Music\\out_run_the_wolves" : "Music\\Revolve");
          this.LanguageCreatedIn = Settings.ActiveLocale;
          if (Settings.IsInExtensionMode)
            ExtensionLoader.LoadNewExtensionSession(ExtensionLoader.ActiveExtensionInfo, (object) this);
        }
        else
        {
          this.loadSaveFile();
          needsNodeInjection = true;
          Settings.initShowsTutorial = false;
          SaveFixHacks.FixSavesWithTerribleHacks((object) this);
        }
        if (!this.multiplayer && !needsNodeInjection && !Settings.IsInExtensionMode)
        {
          MailServer.shouldGenerateJunk = false;
          this.netMap.mailServer.addNewUser(this.thisComputer.ip, this.defaultUser);
        }
        this.topBar = new Rectangle(0, 0, viewport1.Width, OS.TOP_BAR_HEIGHT - 1);
        int x1 = 0;
        int y1 = 0;
        int width2 = this.ScreenManager.GraphicsDevice.Viewport.Width;
        Viewport viewport2 = this.ScreenManager.GraphicsDevice.Viewport;
        int height3 = viewport2.Height;
        this.crashModule = new CrashModule(new Rectangle(x1, y1, width2, height3), this);
        this.crashModule.LoadContent();
        int x2 = 0;
        int y2 = 0;
        viewport2 = this.ScreenManager.GraphicsDevice.Viewport;
        int width3 = viewport2.Width;
        viewport2 = this.ScreenManager.GraphicsDevice.Viewport;
        int height4 = viewport2.Height;
        this.introTextModule = new IntroTextModule(new Rectangle(x2, y2, width3, height4), this);
        this.introTextModule.LoadContent();
        this.traceTracker = new TraceTracker(this);
        this.IncConnectionOverlay = new IncomingConnectionOverlay((object) this);
        this.scanLines = this.content.Load<Texture2D>("ScanLines");
        this.cross = this.content.Load<Texture2D>("Cross");
        this.cog = this.content.Load<Texture2D>("Cog");
        this.saveIcon = this.content.Load<Texture2D>("SaveIcon");
        this.beepSound = this.content.Load<SoundEffect>("SFX/beep");
        if (!Settings.IsInExtensionMode)
        {
          if (DLC1SessionUpgrader.HasDLC1Installed && this.username.ToLower() == "colamaeleon")
          {
            ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/CoelTheme.xml");
            ThemeManager.switchTheme((object) this, "DLC/Themes/CoelTheme.xml");
          }
          else if (DLC1SessionUpgrader.HasDLC1Installed && this.username.ToLower() == "rain_shatter")
          {
            ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/RainTheme.xml");
            ThemeManager.switchTheme((object) this, "DLC/Themes/RainTheme.xml");
          }
          else if (DLC1SessionUpgrader.HasDLC1Installed && this.username.ToLower() == "orann")
          {
            ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/RiptideThemeStandard.xml");
            ThemeManager.switchTheme((object) this, "DLC/Themes/RiptideThemeStandard.xml");
          }
          else if (DLC1SessionUpgrader.HasDLC1Installed && this.username.ToLower() == "hypernexus")
          {
            ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/MiamiThemeLightBlue.xml");
            ThemeManager.switchTheme((object) this, "DLC/Themes/MiamiThemeLightBlue.xml");
          }
        }
        if (!this.multiplayer && !needsNodeInjection && !Settings.IsInExtensionMode)
          this.loadMissionNodes();
        if (!this.HasLoadedDLCContent && Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed && !Settings.IsInExtensionMode)
        {
          DLC1SessionUpgrader.UpgradeSession((object) this, needsNodeInjection);
          this.HasLoadedDLCContent = true;
        }
        this.mailicon = new MailIcon(this, new Vector2(0.0f, 0.0f));
        this.mailicon.pos.X = (float) (viewport1.Width - this.mailicon.getWidth() - 2);
        if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed)
        {
          this.hubServerAlertsIcon = new HubServerAlertsIcon(this.content, "dhs", new string[3]
          {
            "@channel",
            "@Channel",
            "@" + this.defaultUser.name
          });
          this.hubServerAlertsIcon.Init((object) this);
          if (this.HasLoadedDLCContent)
            this.AircraftInfoOverlay = new AircraftInfoOverlay((object) this);
        }
        SAChangeAlertIcon.UpdateAlertIcon((object) this);
        if (!needsNodeInjection)
          MusicManager.playSong();
        if (needsNodeInjection || !Settings.slowOSStartup)
        {
          this.initShowsTutorial = false;
          this.introTextModule.complete = true;
        }
        this.inputEnabled = true;
        this.isLoaded = true;
        int x3 = 0;
        int y3 = 0;
        viewport2 = this.ScreenManager.GraphicsDevice.Viewport;
        int width4 = viewport2.Width;
        viewport2 = this.ScreenManager.GraphicsDevice.Viewport;
        int height5 = viewport2.Height;
        this.fullscreen = new Rectangle(x3, y3, width4, height5);
        this.TraceDangerSequence = new TraceDangerSequence(this.content, this.ScreenManager.SpriteBatch, this.fullscreen, this);
        this.endingSequence = new EndingSequenceModule(this.fullscreen, this);
        if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed)
          this.BootAssitanceModule = new BootCrashAssistanceModule(this.fullscreen, this);
        bool flag1 = Settings.slowOSStartup && !needsNodeInjection;
        if (Settings.IsInExtensionMode && !ExtensionLoader.ActiveExtensionInfo.HasIntroStartup)
        {
          flag1 = false;
          this.introTextModule.complete = true;
        }
        bool flag2 = Settings.osStartsWithTutorial && (!needsNodeInjection || !this.Flags.HasFlag("TutorialComplete"));
        if (Settings.IsInExtensionMode && !ExtensionLoader.ActiveExtensionInfo.StartsWithTutorial)
        {
          flag2 = false;
          this.initShowsTutorial = false;
        }
        if (flag1)
        {
          this.rebootThisComputer();
          if (Settings.initShowsTutorial)
          {
            this.display.visible = false;
            this.ram.visible = false;
            this.netMap.visible = false;
            this.terminal.visible = true;
          }
        }
        else if (flag2)
        {
          this.display.visible = false;
          this.ram.visible = false;
          this.netMap.visible = false;
          this.terminal.visible = true;
          this.terminal.reset();
          Settings.initShowsTutorial = true;
          this.initShowsTutorial = true;
          if (!OS.TestingPassOnly)
            this.execute("FirstTimeInitdswhupwnemfdsiuoewnmdsmffdjsklanfeebfjkalnbmsdakj Init");
        }
        else if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed && HostileHackerBreakinSequence.IsInBlockingHostileFileState((object) this))
        {
          this.rebootThisComputer();
          this.BootAssitanceModule.ShouldSkipDialogueTypeout = true;
        }
        else
        {
          if (Settings.EnableDLC && HostileHackerBreakinSequence.IsFirstSuccessfulBootAfterBlockingState((object) this))
          {
            HostileHackerBreakinSequence.ReactToFirstSuccesfulBoot((object) this);
            this.rebootThisComputer();
          }
          if (!OS.TestingPassOnly)
            this.runCommand("connect " + this.thisComputer.ip);
          if (this.thisComputer.files.root.searchForFolder("sys").searchForFile("Notes_Reopener.bat") != null)
            this.runCommand("notes");
        }
        if (!Settings.EnableDLC || !DLC1SessionUpgrader.HasDLC1Installed || !this.HasLoadedDLCContent)
          return;
        bool flag3 = false;
        if (!this.Flags.HasFlag("AircraftInfoOverlayDeactivated"))
        {
          if (this.Flags.HasFlag("AircraftInfoOverlayActivated"))
            flag3 = true;
          if (!flag3)
          {
            Computer computer = Programs.getComputer(this, "dair_crash");
            Folder folder2 = computer.files.root.searchForFolder("FlightSystems");
            bool flag4 = false;
            for (int index = 0; index < folder2.files.Count; ++index)
            {
              if (folder2.files[index].name == "747FlightOps.dll")
                flag4 = true;
            }
            AircraftDaemon daemon = (AircraftDaemon) computer.getDaemon(typeof (AircraftDaemon));
            if (!flag4 && !this.Flags.HasFlag("DLC_PlaneResult"))
              flag3 = true;
          }
        }
        if (flag3)
        {
          AircraftDaemon daemon = (AircraftDaemon) Programs.getComputer(this, "dair_crash").getDaemon(typeof (AircraftDaemon));
          daemon.StartReloadFirmware();
          daemon.StartUpdating();
          this.AircraftInfoOverlay.Activate();
          this.AircraftInfoOverlay.IsMonitoringDLCEndingCases = true;
          MissionFunctions.runCommand(0, "playAirlineCrashSongSequence");
        }
      }
      else
      {
        if (!this.multiplayer)
          return;
        this.initializeNetwork();
      }
    }

    public void loadMultiplayerMission()
    {
      this.currentMission = (ActiveMission) ComputerLoader.readMission("Content/Missions/MultiplayerMission.xml");
    }

    public void loadMissionNodes()
    {
      if (this.multiplayer)
        return;
      List<string> list = BootLoadList.getList();
      for (int index = 0; index < list.Count; ++index)
      {
        try
        {
          Computer.loadFromFile(list[index]);
        }
        catch (Exception ex)
        {
          Console.WriteLine((object) ex);
          Console.WriteLine(ex.StackTrace);
          throw ex;
        }
      }
      if (ComputerLoader.postAllLoadedActions != null)
        ComputerLoader.postAllLoadedActions();
      if (Settings.isDemoMode)
      {
        List<string> demoList = BootLoadList.getDemoList();
        for (int index = 0; index < demoList.Count; ++index)
          Computer.loadFromFile(demoList[index]);
      }
      if (!this.initShowsTutorial && !Settings.IsInExtensionMode && !this.IsDLCConventionDemo)
      {
        if (Settings.isSpecialTestBuild)
          ComputerLoader.loadMission("Content/Missions/Misc/TesterCSECIntroMission.xml", false);
        else
          ComputerLoader.loadMission("Content/Missions/BitMissionIntro.xml", false);
      }
      else if (!Settings.IsInExtensionMode)
        this.currentMission = (ActiveMission) ComputerLoader.readMission("Content/Missions/BitMissionIntro.xml");
    }

    public override void UnloadContent()
    {
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      try
      {
        this.lastGameTime = gameTime;
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        TimeSpan elapsedGameTime = gameTime.ElapsedGameTime;
        float totalSeconds1 = (float) elapsedGameTime.TotalSeconds;
        SFX.Update(totalSeconds1);
        this.timer += totalSeconds1;
        if (this.isLoaded && this.bootingUp)
        {
          this.canRunContent = false;
          this.thisComputer.bootupTick(totalSeconds1);
          this.crashModule.Update(totalSeconds1);
          if (!this.thisComputer.disabled)
          {
            if (this.FirstTimeStartup && !this.introTextModule.complete)
              this.introTextModule.Update(totalSeconds1);
            if (this.introTextModule.complete)
            {
              this.FirstTimeStartup = false;
              this.bootingUp = false;
              this.canRunContent = true;
              this.crashModule.completeReboot();
            }
          }
        }
        this.delayer.Pump();
        if (this.canRunContent && this.isLoaded)
        {
          try
          {
            if (this.TraceDangerSequence.IsActive)
              this.TraceDangerSequence.Update(totalSeconds1);
            if (this.multiplayer)
            {
              if (!this.multiplayerMissionLoaded && this.opponentComputer != null)
              {
                this.loadMultiplayerMission();
                this.multiplayerMissionLoaded = true;
              }
              this.stayAliveTimer -= totalSeconds1;
              if ((double) this.stayAliveTimer <= 0.0)
              {
                this.stayAliveTimer = OS.TCP_STAYALIVE_TIMER;
                this.sendMessage("stayAlive " + (object) (int) OS.currentElapsedTime);
              }
            }
            else if (this.IsInDLCMode || this.ShowDLCAlertsIcon)
              this.hubServerAlertsIcon.Update(totalSeconds1);
            else
              this.mailicon.Update(totalSeconds1);
            for (int index = 0; index < this.modules.Count; ++index)
              this.modules[index].Update(totalSeconds1);
            if (this.UpdateSubscriptions != null)
              this.UpdateSubscriptions(totalSeconds1);
            if (this.connectedComp == null)
            {
              if (this.connectedIPLastFrame != null)
                this.handleDisconnection();
            }
            else if (this.connectedIPLastFrame != this.connectedComp.ip)
              this.handleDisconnection();
            for (int index = 0; index < this.TrackersInProgress.Count; ++index)
            {
              if (this.connectedComp != null && this.connectedComp == this.TrackersInProgress[index].comp)
              {
                this.TrackersInProgress.RemoveAt(index);
                --index;
              }
              else
              {
                OS.TrackerDetail trackerDetail = this.TrackersInProgress[index];
                trackerDetail.timeLeft -= totalSeconds1;
                if ((double) trackerDetail.timeLeft <= 0.0)
                {
                  TrackerCompleteSequence.TrackComplete((object) this, trackerDetail.comp);
                  break;
                }
                this.TrackersInProgress[index] = trackerDetail;
              }
            }
            this.ramAvaliable = this.totalRam;
            if (this.exes.Count > 0)
              this.exes[0].bounds.Y = this.ram.bounds.Y + RamModule.contentStartOffset;
            for (int index = 0; index < this.exes.Count; ++index)
            {
              this.exes[index].bounds.X = this.ram.bounds.X;
              if (index > 0 && index < this.exes.Count)
                this.exes[index].bounds.Y = this.exes[index - 1].bounds.Y + this.exes[index - 1].bounds.Height;
              if (this.exes[index].needsRemoval)
              {
                this.exes.RemoveAt(index);
                --index;
              }
              else
                this.ramAvaliable -= this.exes[index].ramCost;
            }
            for (int index = 0; index < this.exes.Count; ++index)
              this.exes[index].Update(totalSeconds1);
            if (this.currentMission != null)
              this.currentMission.Update(totalSeconds1);
            for (int index = 0; index < this.branchMissions.Count; ++index)
              this.branchMissions[index].Update(totalSeconds1);
            this.traceTracker.Update(totalSeconds1);
            if ((double) this.gameSavedTextAlpha > 0.0)
              this.gameSavedTextAlpha -= totalSeconds1;
            if ((double) this.PorthackCompleteFlashTime > 0.0)
              this.PorthackCompleteFlashTime -= totalSeconds1;
            if ((double) this.MissionCompleteFlashTime > 0.0)
              this.MissionCompleteFlashTime -= totalSeconds1;
            if ((double) this.warningFlashTimer > 0.0)
            {
              this.warningFlashTimer -= totalSeconds1;
              if ((double) this.warningFlashTimer <= 0.0)
              {
                this.highlightColor = this.defaultHighlightColor;
              }
              else
              {
                this.highlightColor = Color.Lerp(this.defaultHighlightColor, this.warningColor, this.warningFlashTimer / OS.WARNING_FLASH_TIME);
                this.moduleColorSolid = Color.Lerp(this.moduleColorSolidDefault, this.warningColor, this.warningFlashTimer / OS.WARNING_FLASH_TIME);
              }
            }
            this.EffectsUpdater.Update(totalSeconds1, (object) this);
            elapsedGameTime = gameTime.ElapsedGameTime;
            float totalSeconds2 = (float) elapsedGameTime.TotalSeconds;
            this.IncConnectionOverlay.Update(totalSeconds2);
            if (this.AircraftInfoOverlay != null && this.AircraftInfoOverlay.IsActive)
              this.AircraftInfoOverlay.Update(totalSeconds2);
            double currentElapsedTime = OS.currentElapsedTime;
            elapsedGameTime = gameTime.ElapsedGameTime;
            double totalSeconds3 = elapsedGameTime.TotalSeconds;
            OS.currentElapsedTime = currentElapsedTime + totalSeconds3;
          }
          catch (Exception ex)
          {
            Console.WriteLine((object) ex);
            ++this.updateErrorCount;
            if (this.updateErrorCount < 5)
              Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
          }
        }
        else
        {
          this.endingSequence.Update(totalSeconds1);
          if (this.IsInDLCMode)
            this.BootAssitanceModule.Update(totalSeconds1);
        }
        this.ConditionalActions.Update(totalSeconds1, (object) this);
        this.connectedIPLastFrame = this.connectedComp != null ? this.connectedComp.ip : (string) null;
      }
      catch (Exception ex)
      {
        ++this.updateErrorCount;
        if (this.updateErrorCount >= 3)
          this.handleUpdateError();
        else
          Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
      }
    }

    private void handleDisconnection()
    {
      Computer computer = Programs.getComputer(this, this.connectedIPLastFrame);
      if (computer == null)
        return;
      Administrator admin = computer.admin;
      if (admin != null)
        admin.disconnectionDetected(computer, this);
      if (computer.HasTracker && TrackerCompleteSequence.CompShouldStartTrackerFromLogs((object) this, computer, (string) null))
      {
        float num = TrackerCompleteSequence.MinTrackTime + Utils.randm(TrackerCompleteSequence.MaxTrackTime - TrackerCompleteSequence.MinTrackTime);
        this.TrackersInProgress.Add(new OS.TrackerDetail()
        {
          comp = computer,
          timeLeft = num
        });
      }
    }

    public override void HandleInput(InputState input)
    {
      GuiData.doInput(input);
      if (!Utils.keyPressed(input, Keys.NumLock, new PlayerIndex?(this.ScreenManager.controllingPlayer)))
        return;
      PostProcessor.bloomEnabled = !PostProcessor.bloomEnabled;
    }

    public void drawBackground()
    {
      ThemeManager.drawBackgroundImage(GuiData.spriteBatch, this.fullscreen);
    }

    public void RequestRemovalOfAllPopups()
    {
      if (this.ExitToMenuMessageBox == null)
        return;
      this.ExitToMenuMessageBox.ExitScreen();
      this.ExitToMenuMessageBox = (MessageBoxScreen) null;
    }

    public void drawScanlines()
    {
      if (!PostProcessor.scanlinesEnabled)
        return;
      Vector2 position = new Vector2(0.0f, 0.0f);
      while (true)
      {
        double x = (double) position.X;
        Viewport viewport = this.ScreenManager.GraphicsDevice.Viewport;
        double width = (double) viewport.Width;
        if (x < width)
        {
          while (true)
          {
            double y = (double) position.Y;
            viewport = this.ScreenManager.GraphicsDevice.Viewport;
            double height = (double) viewport.Height;
            if (y < height)
            {
              GuiData.spriteBatch.Draw(this.scanLines, position, this.scanlinesColor);
              position.Y += (float) this.scanLines.Height;
            }
            else
              break;
          }
          position.Y = 0.0f;
          position.X += (float) this.scanLines.Width;
        }
        else
          break;
      }
    }

    public void drawModules(GameTime gameTime)
    {
      Vector2 zero = Vector2.Zero;
      GuiData.spriteBatch.Draw(Utils.white, this.topBar, this.topBarColor);
      float totalSeconds = (float) gameTime.ElapsedGameTime.TotalSeconds;
      try
      {
        if (this.connectedComp != null)
          this.locationString = LocaleTerms.Loc("Location") + ": " + this.connectedComp.name + "@" + this.connectedIP + " ";
        else
          this.locationString = LocaleTerms.Loc("Location: Not Connected") + " ";
        Vector2 vector2 = GuiData.UITinyfont.MeasureString(this.locationString);
        zero.X = (float) this.topBar.Width - vector2.X - (float) this.mailicon.getWidth();
        zero.Y -= 3f;
        GuiData.spriteBatch.DrawString(GuiData.UITinyfont, this.locationString, zero, this.topBarTextColor);
        if ((double) GuiData.ActiveFontConfig.tinyFontCharHeight * 2.0 <= (double) this.topBar.Height)
        {
          string text = LocaleTerms.Loc("Home IP:") + " " + this.thisComputer.ip + " ";
          zero.Y += (float) (this.topBar.Height / 2);
          vector2 = GuiData.UITinyfont.MeasureString(text);
          zero.X = (float) this.topBar.Width - vector2.X - (float) this.mailicon.getWidth();
          GuiData.spriteBatch.DrawString(GuiData.UITinyfont, text, zero, this.topBarTextColor);
        }
        zero.Y = 0.0f;
      }
      catch (Exception ex)
      {
      }
      zero.X = 110f;
      if (!Settings.isLockedDemoMode && !this.DisableTopBarButtons)
      {
        if (Button.doButton(3827178, 3, 0, 20, this.topBar.Height - 1, "", new Color?(this.topBarIconsColor), this.cross))
        {
          this.ExitToMenuMessageBox = new MessageBoxScreen(LocaleTerms.Loc("Quit HackNetOS\nCurrent Session?") + "\n", false, true);
          this.ExitToMenuMessageBox.OverrideAcceptedText = LocaleTerms.Loc("Exit to Menu");
          this.ExitToMenuMessageBox.Accepted += new EventHandler<PlayerIndexEventArgs>(this.quitGame);
          this.ScreenManager.AddScreen((GameScreen) this.ExitToMenuMessageBox);
        }
        if (!this.TraceDangerSequence.IsActive && Button.doButton(3827179, 26, 0, 20, this.topBar.Height - 1, "", new Color?(this.topBarIconsColor), this.cog))
        {
          this.saveGame();
          this.ScreenManager.AddScreen((GameScreen) new OptionsMenu(true), new PlayerIndex?(this.ScreenManager.controllingPlayer));
        }
        if (((this.initShowsTutorial ? (false ? 1 : 0) : (!this.TraceDangerSequence.IsActive ? 1 : 0)) & (!Settings.IsInExtensionMode ? 1 : (ExtensionLoader.ActiveExtensionInfo.AllowSave ? 1 : 0))) != 0 && Button.doButton(3827180, 49, 0, 20, this.topBar.Height - 1, "", new Color?(this.topBarIconsColor), this.saveIcon))
          this.saveGame();
        if (!this.initShowsTutorial && Settings.debugCommandsEnabled)
        {
          if (Button.doButton(3827190, 72, 0, 20, this.topBar.Height - 1, "", new Color?(this.topBarIconsColor), this.cog))
          {
            if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 0)
              ThemeManager.switchTheme((object) this, OSTheme.HacknetBlue);
            else if (ThemeManager.currentTheme == OSTheme.HacknetBlue)
            {
              ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/RiptideClassicTheme.xml");
              ThemeManager.switchTheme((object) this, "DLC/Themes/RiptideClassicTheme.xml");
              this.drawErrorCount = 2;
            }
            else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 2)
            {
              ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/MiamiTheme.xml");
              ThemeManager.switchTheme((object) this, "DLC/Themes/MiamiTheme.xml");
              this.drawErrorCount = 3;
            }
            else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 3)
            {
              ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/RainTheme.xml");
              ThemeManager.switchTheme((object) this, "DLC/Themes/RainTheme.xml");
              this.drawErrorCount = 4;
            }
            else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 4)
            {
              ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/RiptideTheme.xml");
              ThemeManager.switchTheme((object) this, "DLC/Themes/RiptideTheme.xml");
              this.drawErrorCount = 5;
            }
            else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 5)
            {
              ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/CautionTheme.xml");
              ThemeManager.switchTheme((object) this, "DLC/Themes/CautionTheme.xml");
              this.drawErrorCount = 6;
            }
            else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 6)
            {
              ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/HoraTheme.xml");
              ThemeManager.switchTheme((object) this, "DLC/Themes/HoraTheme.xml");
              this.drawErrorCount = 7;
            }
            else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 7)
            {
              ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/Floatvoid.xml");
              ThemeManager.switchTheme((object) this, "DLC/Themes/FloatVoidTheme.xml");
              this.drawErrorCount = 8;
            }
            else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 8)
            {
              ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/MiamiThemeLightBlue.xml");
              ThemeManager.switchTheme((object) this, "DLC/Themes/MiamiThemeLightBlue.xml");
              this.drawErrorCount = 9;
            }
            else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 9)
            {
              ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/StarfieldClassicTheme.xml");
              ThemeManager.switchTheme((object) this, "DLC/Themes/StarfieldClassicTheme.xml");
              this.drawErrorCount = 10;
            }
            else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 10)
            {
              ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/CoelTheme.xml");
              ThemeManager.switchTheme((object) this, "DLC/Themes/CoelTheme.xml");
              this.drawErrorCount = 0;
            }
            else if (ThemeManager.currentTheme == OSTheme.HacknetPurple)
              ThemeManager.switchTheme((object) this, OSTheme.HacknetMint);
            else if (ThemeManager.currentTheme == OSTheme.HacknetMint)
            {
              ThemeManager.switchTheme((object) this, OSTheme.HackerGreen);
            }
            else
            {
              ThemeManager.setThemeOnComputer((object) this.thisComputer, "DLC/Themes/CautionTheme.xml");
              ThemeManager.switchTheme((object) this, "DLC/Themes/CautionTheme.xml");
            }
          }
        }
        else
          zero.X = 80f;
      }
      else
        zero.X = 2f;
      zero.Y = 1f;
      string text1 = string.Concat((object) (int) (1.0 / gameTime.ElapsedGameTime.TotalSeconds + 0.5));
      GuiData.spriteBatch.DrawString(GuiData.UITinyfont, text1, zero, this.topBarTextColor);
      zero.Y = 0.0f;
      if (!this.multiplayer && !this.DisableTopBarButtons && !this.DisableEmailIcon)
      {
        if (this.IsInDLCMode || this.ShowDLCAlertsIcon)
          this.hubServerAlertsIcon.Draw(new Rectangle((int) this.mailicon.pos.X, (int) this.mailicon.pos.Y, this.mailicon.getWidth(), this.topBar.Height - 2), GuiData.spriteBatch);
        else
          this.mailicon.Draw();
      }
      int num1 = this.ram.bounds.Height + this.topBar.Height + 16;
      if (num1 < this.fullscreen.Height && this.ram.visible)
        this.audioVisualizer.Draw(new Rectangle(this.ram.bounds.X, num1 + 1, this.ram.bounds.Width - 2, this.fullscreen.Height - num1 - 4), GuiData.spriteBatch);
      for (int index = 0; index < this.modules.Count; ++index)
      {
        if (this.modules[index].visible)
        {
          this.modules[index].PreDrawStep();
          this.modules[index].Draw(totalSeconds);
          this.modules[index].PostDrawStep();
        }
      }
      if (this.ram.visible)
      {
        for (int index = 0; index < this.exes.Count; ++index)
          this.exes[index].Draw(totalSeconds);
      }
      this.IncConnectionOverlay.Draw(this.fullscreen, GuiData.spriteBatch);
      if (this.AircraftInfoOverlay != null && this.AircraftInfoOverlay.IsActive)
        this.AircraftInfoOverlay.Draw(new Rectangle(this.fullscreen.X, OS.TOP_BAR_HEIGHT + 1 + Module.PANEL_HEIGHT, this.fullscreen.Width, this.fullscreen.Height - (OS.TOP_BAR_HEIGHT + 2 + Module.PANEL_HEIGHT)), GuiData.spriteBatch);
      this.traceTracker.Draw(GuiData.spriteBatch);
      bool drawShadow = TextItem.DrawShadow;
      TextItem.DrawShadow = false;
      float num2 = 1f - this.gameSavedTextAlpha;
      int num3 = 45;
      TextItem.doFontLabel(new Vector2(0.0f, (float) (this.ScreenManager.GraphicsDevice.Viewport.Height - num3)) - num2 * new Vector2(0.0f, 200f), "SESSION SAVED", GuiData.titlefont, new Color?(this.thisComputerNode * this.gameSavedTextAlpha), float.MaxValue, (float) num3, false);
      TextItem.DrawShadow = drawShadow;
    }

    public void quitGame(object sender, PlayerIndexEventArgs e)
    {
      this.HasExitedAndEnded = true;
      this.ExitScreen();
      MainMenu.resetOS();
      this.ScreenManager.AddScreen((GameScreen) new MainMenu());
      SaveFileManager.Init(false);
    }

    public override void Draw(GameTime gameTime)
    {
      try
      {
        float totalSeconds = (float) gameTime.ElapsedGameTime.TotalSeconds;
        if (this.lastGameTime == null)
          this.lastGameTime = gameTime;
        if (this.canRunContent && this.isLoaded)
        {
          PostProcessor.begin();
          GuiData.startDraw();
          try
          {
            if (!this.TraceDangerSequence.PreventOSRendering)
            {
              this.drawBackground();
              if (this.terminalOnlyMode)
                this.terminal.Draw(totalSeconds);
              else
                this.drawModules(gameTime);
              SFX.Draw(GuiData.spriteBatch);
            }
            if (this.TraceDangerSequence.IsActive)
              this.TraceDangerSequence.Draw();
          }
          catch (Exception ex)
          {
            ++this.drawErrorCount;
            if (this.drawErrorCount < 5)
              Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex) + "\r\n\r\n");
          }
          GuiData.endDraw();
          PostProcessor.end();
          GuiData.startDraw();
          if (this.postFXDrawActions != null)
          {
            this.postFXDrawActions();
            this.postFXDrawActions = (Action) null;
          }
          this.drawScanlines();
          GuiData.endDraw();
        }
        else if (this.endingSequence.IsActive)
        {
          PostProcessor.begin();
          this.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
          this.endingSequence.Draw(totalSeconds);
          this.drawScanlines();
          this.ScreenManager.SpriteBatch.End();
          PostProcessor.end();
        }
        else if (this.BootAssitanceModule != null && this.BootAssitanceModule.IsActive)
        {
          PostProcessor.begin();
          this.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
          this.BootAssitanceModule.Draw(totalSeconds);
          this.drawScanlines();
          this.ScreenManager.SpriteBatch.End();
          PostProcessor.end();
        }
        else if (this.bootingUp)
        {
          PostProcessor.begin();
          this.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
          if (this.thisComputer.disabled)
          {
            this.RequestRemovalOfAllPopups();
            if (this.TraceDangerSequence.IsActive)
              this.TraceDangerSequence.CancelTraceDangerSequence();
            this.crashModule.Draw(totalSeconds);
          }
          else
            this.introTextModule.Draw(totalSeconds);
          this.ScreenManager.SpriteBatch.End();
          PostProcessor.end();
        }
        else
        {
          GuiData.startDraw();
          TextItem.doSmallLabel(new Vector2(0.0f, 700f), LocaleTerms.Loc("Loading..."), new Color?());
          GuiData.endDraw();
        }
      }
      catch (Exception ex)
      {
        ++this.drawErrorCount;
        if (this.drawErrorCount >= 3)
          this.handleDrawError();
        else
          Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
      }
    }

    private void handleUpdateError()
    {
      DebugLog.data.Add("----------------------Handling Update error");
      this.connectedComp = (Computer) null;
    }

    private void handleDrawError()
    {
      this.connectedComp = (Computer) null;
      this.ScreenManager.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
    }

    public void endMultiplayerMatch(bool won)
    {
      if (won)
        this.sendMessage("mpOpponentWin " + this.timer.ToString());
      this.ScreenManager.AddScreen((GameScreen) new MultiplayerGameOverScreen(won));
    }

    private void initializeNetwork()
    {
      if (this.isServer)
      {
        string message = "init " + (object) Utils.random.Next();
        this.sendMessage(message);
        Multiplayer.parseInputMessage(message, this);
      }
      else
        this.sendMessage("clientConnect Hallo");
      this.listenerThread = new Thread(new ThreadStart(this.listenThread));
      this.listenerThread.Name = "OS Listener Thread";
      this.listenerThread.Start();
      Console.WriteLine("Created Network Listener Thread");
    }

    public void sendMessage(string message)
    {
      if (!this.netStream.CanWrite)
        return;
      this.outBuffer = this.encoder.GetBytes(message + "#&#");
      this.netStream.Write(this.outBuffer, 0, this.outBuffer.Length);
      this.netStream.Flush();
      for (int index = 0; index < message.Length; ++index)
        this.outBuffer[index] = (byte) 0;
    }

    private void listenThread()
    {
      while (!this.DestroyThreads)
      {
        try
        {
          if (this.netStream.Read(this.inBuffer, 0, this.inBuffer.Length) == 0)
          {
            this.clientDisconnected();
          }
          else
          {
            string str = this.encoder.GetString(this.inBuffer).Trim(this.trimChars);
            string[] separator = new string[1]{ "#&#" };
            foreach (string message in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
              Multiplayer.parseInputMessage(message, this);
            for (int index = 0; index < this.inBuffer.Length; ++index)
              this.inBuffer[index] = (byte) 0;
          }
        }
        catch (IOException ex)
        {
          if (ex.Message.Contains("Unable to read"))
          {
            this.write("Opponent Disconnected - Victory");
            this.ScreenManager.AddScreen((GameScreen) new MultiplayerGameOverScreen(true));
            this.netStream.Close();
            return;
          }
          this.write("NetError: " + DisplayModule.splitForWidth(ex.ToString(), this.terminal.bounds.Width - 20));
        }
        catch (Exception ex)
        {
          this.write("NetError: " + DisplayModule.splitForWidth(ex.ToString(), this.terminal.bounds.Width - 20));
          break;
        }
        if (Game1.threadsExiting)
          return;
      }
      Console.WriteLine("Listener Thread Exiting");
    }

    public void clientDisconnected()
    {
      this.write("DISCONNECTION detected");
      this.DestroyThreads = true;
      this.netStream.Close();
      this.client.Close();
    }

    public void timerExpired()
    {
      if (this.traceCompleteOverrideAction != null)
      {
        this.traceCompleteOverrideAction();
      }
      else
      {
        if (this.connectedComp != null && this.connectedComp.admin != null)
          this.connectedComp.admin.traceEjectionDetected(this.connectedComp, this);
        if (this.Flags.HasFlag("CSEC_Member"))
          this.TraceDangerSequence.BeginTraceDangerSequence();
        else
          this.thisComputer.crash(this.thisComputer.ip);
      }
    }

    public void loadSaveFile()
    {
      Stream input = this.ForceLoadOverrideStream == null || !OS.TestingPassOnly ? SaveFileManager.GetSaveReadStream(this.SaveGameUserName) : this.ForceLoadOverrideStream;
      if (input != null)
      {
        XmlReader xmlReader = XmlReader.Create(input);
        this.loadTitleSaveData(xmlReader);
        if (Settings.ActiveLocale != this.LanguageCreatedIn)
        {
          LocaleActivator.ActivateLocale(this.LanguageCreatedIn, this.content);
          Settings.ActiveLocale = this.LanguageCreatedIn;
        }
        this.LoadExtraTitleSaveData(xmlReader);
        if (this.IsExiting)
          return;
        this.Flags.Load(xmlReader);
        this.netMap.load(xmlReader);
        this.currentMission = (ActiveMission) ActiveMission.load(xmlReader);
        if (this.currentMission != null && !(this.currentMission.reloadGoalsSourceFile == "Missions/BitMissionIntro.xml"))
        {
          ActiveMission activeMission = (ActiveMission) ComputerLoader.readMission(this.currentMission.reloadGoalsSourceFile);
        }
        Console.WriteLine(this.branchMissions.Count);
        this.allFactions = AllFactions.loadFromSave(xmlReader);
        if (!string.IsNullOrWhiteSpace(this.allFactions.currentFaction))
          this.allFactions.setCurrentFaction(this.allFactions.currentFaction, this);
        this.loadOtherSaveData(xmlReader);
        OS.WillLoadSave = false;
        input.Flush();
        input.Close();
      }
      this.FirstTimeStartup = false;
    }

    public void saveGame()
    {
      if (this.initShowsTutorial || Settings.IsInExtensionMode && !ExtensionLoader.ActiveExtensionInfo.AllowSave)
        return;
      this.execute("save!(SJN!*SNL8vAewew57WewJdwl89(*4;;;&!)@&(ak'^&#@J3KH@!*");
    }

    public void threadedSaveExecute(bool preventSaveText = false)
    {
      if (this.SaveInProgress)
      {
        if (this.SaveInQueue)
          return;
        this.SaveInQueue = true;
      }
      lock (SaveFileManager.CurrentlySaving)
      {
        this.SaveInProgress = true;
        if (this.SaveInQueue)
          this.SaveInQueue = false;
        if (!preventSaveText)
          this.gameSavedTextAlpha = 1f;
        this.writeSaveGame(this.SaveUserAccountName);
        StatsManager.SaveStatProgress();
        Console.WriteLine("Session Saved");
        this.SaveInProgress = false;
      }
    }

    private void writeSaveGame(string filename)
    {
      string str1 = "<?xml version =\"1.0\" encoding =\"UTF-8\" ?>\n" + "<HacknetSave generatedMissionCount=\"" + (object) MissionGenerator.generationCount + "\" Username=\"" + this.username + "\" Language=\"" + this.LanguageCreatedIn + "\" DLCMode=\"" + (object) this.IsInDLCMode + "\" DisableMailIcon=\"" + (object) this.DisableEmailIcon + "\">\n" + this.GetDLCSaveString() + this.Flags.GetSaveString() + this.netMap.getSaveString();
      string str2 = (this.currentMission == null ? str1 + "<mission next=\"NULL_MISSION\" goals=\"none\" activeCheck=\"none\">\n</mission>" : str1 + this.currentMission.getSaveString()) + "<branchMissions>\n";
      for (int index = 0; index < this.branchMissions.Count; ++index)
        str2 += this.branchMissions[index].getSaveString();
      SaveFileManager.WriteSaveData(str2 + "</branchMissions>" + this.allFactions.getSaveString() + "<other music=\"" + MusicManager.currentSongName + "\" homeNode=\"" + this.homeNodeID + "\" homeAssetsNode=\"" + this.homeAssetServerID + "\" />" + "</HacknetSave>", filename);
    }

    public void LoadExtraTitleSaveData(XmlReader rdr)
    {
      this.IsDLCSave = false;
      while (!(rdr.Name == "Flags") || !rdr.IsStartElement())
      {
        if (rdr.Name == "DLC")
          this.ReadDLCSaveData(rdr);
        rdr.Read();
      }
    }

    private void ReadDLCSaveData(XmlReader rdr)
    {
      this.IsDLCSave = true;
      while (!(rdr.Name == "DLC") || rdr.IsStartElement())
      {
        if (rdr.Name == "DLC")
        {
          if (rdr.MoveToAttribute("Active"))
            this.IsInDLCMode = rdr.ReadContentAsString().ToLower() == "true";
          if (rdr.MoveToAttribute("LoadedContent"))
            this.HasLoadedDLCContent = rdr.ReadContentAsString().ToLower() == "true";
        }
        if (rdr.Name == "Flags" && rdr.MoveToAttribute("OriginalFaction"))
          this.PreDLCFaction = rdr.ReadContentAsString();
        if (rdr.Name == "OriginalVisibleNodes")
          this.PreDLCVisibleNodesCache = rdr.ReadElementContentAsString();
        if (rdr.Name == "ConditionalActions")
          this.ConditionalActions = RunnableConditionalActions.Deserialize(rdr);
        rdr.Read();
      }
      if (!this.HasLoadedDLCContent || DLC1SessionUpgrader.HasDLC1Installed)
        return;
      MainMenu.AccumErrors = "LOAD ERROR: Save " + this.SaveGameUserName + " is configured for Labyrinths DLC, but it is not installed on this computer.\n\n\n";
      this.ExitScreen();
      this.IsExiting = true;
    }

    private string GetDLCSaveString()
    {
      return "<DLC Active=\"" + (object) this.IsInDLCMode + "\" LoadedContent=\"" + (object) this.HasLoadedDLCContent + "\">\n" + "<Flags OriginalFaction=\"" + this.PreDLCFaction + "\"/>\n" + "<OriginalVisibleNodes>" + this.PreDLCVisibleNodesCache + "</OriginalVisibleNodes>\n" + this.ConditionalActions.GetSaveString() + "\n" + "</DLC>";
    }

    public void loadTitleSaveData(XmlReader reader)
    {
      while (reader.Name != "HacknetSave")
      {
        if (reader.EOF)
          return;
        reader.Read();
      }
      MissionGenerator.generationCount = !reader.MoveToAttribute("generatedMissionCount") ? 100 : reader.ReadContentAsInt();
      if (reader.MoveToAttribute("Username"))
      {
        this.username = reader.ReadContentAsString();
        this.defaultUser.name = this.username;
      }
      this.LanguageCreatedIn = !reader.MoveToAttribute("Language") ? "en-us" : reader.ReadContentAsString();
      if (reader.MoveToAttribute("DLCMode"))
        this.IsInDLCMode = reader.ReadContentAsString().ToLower() == "true" && Settings.EnableDLC;
      if (!reader.MoveToAttribute("DisableMailIcon"))
        return;
      this.DisableEmailIcon = reader.ReadContentAsString().ToLower() == "true" && Settings.EnableDLC;
    }

    public void setMouseVisiblity(bool mouseIsVisible)
    {
      this.delayer.Post(ActionDelayer.NextTick(), (Action) (() => Game1.getSingleton().IsMouseVisible = mouseIsVisible));
    }

    public void loadBranchMissionsSaveData(XmlReader reader)
    {
      while (reader.Name != "branchMissions")
      {
        if (reader.EOF)
          return;
        reader.Read();
        if (reader.Name == "other")
          return;
      }
      if (!reader.IsStartElement())
        return;
      this.branchMissions.Clear();
      reader.Read();
      while (true)
      {
        while ((!reader.IsStartElement() || !(reader.Name == "mission")) && (reader.IsStartElement() || !(reader.Name == "branchMissions")))
          reader.Read();
        if (!(reader.Name == "branchMissions"))
          this.branchMissions.Add((ActiveMission) ActiveMission.load(reader));
        else
          break;
      }
    }

    public void loadOtherSaveData(XmlReader reader)
    {
      while (reader.Name != "other")
      {
        if (reader.EOF)
          return;
        reader.Read();
      }
      reader.MoveToAttribute("music");
      MusicManager.playSongImmediatley(reader.ReadContentAsString());
      if (reader.MoveToAttribute("homeNode"))
        this.homeNodeID = reader.ReadContentAsString();
      if (!reader.MoveToAttribute("homeAssetsNode"))
        return;
      this.homeAssetServerID = reader.ReadContentAsString();
    }

    public override void inputMethodChanged(bool usingGamePad)
    {
    }

    public void write(string text)
    {
      if (this.terminal == null || text.Length <= 0)
        return;
      string text1 = DisplayModule.cleanSplitForWidth(text, this.terminal.bounds.Width - 40);
      if ((int) text1[text1.Length - 1] == 10)
        text1 = text1.Substring(0, text1.Length - 1);
      this.terminal.writeLine(text1);
    }

    public void writeSingle(string text)
    {
      this.terminal.write(text);
    }

    public void runCommand(string text)
    {
      if (this.terminal.preventingExecution)
        return;
      this.write("\n" + this.terminal.prompt + text);
      this.terminal.lastRunCommand = text;
      this.execute(text);
    }

    public void execute(string text)
    {
      string[] strArray = text.Split(' ');
      Thread thread = new Thread(new ParameterizedThreadStart(this.threadExecute));
      thread.Name = "exe" + thread.Name;
      if (!text.StartsWith("save"))
        thread.IsBackground = true;
      thread.CurrentCulture = Game1.culture;
      thread.CurrentUICulture = Game1.culture;
      Console.WriteLine("Spawning thread for command " + text);
      thread.Start((object) strArray);
    }

    public void connectedComputerCrashed(Computer c)
    {
      if (this.connectedComp != null)
        this.connectedComp.disconnecting(this.thisComputer.ip, true);
      this.connectedComp = (Computer) null;
      this.display.command = "crash";
      this.terminal.prompt = "> ";
    }

    public void thisComputerCrashed()
    {
      this.display.command = "";
      this.connectedComp = (Computer) null;
      this.bootingUp = true;
      this.exes.Clear();
      this.shellIPs.Clear();
      this.crashModule.reset();
      this.setMouseVisiblity(false);
    }

    public void thisComputerIPReset()
    {
      if (this.traceTracker.active)
        this.traceTracker.active = false;
      if (this.TraceDangerSequence.IsActive)
        this.TraceDangerSequence.CompleteIPResetSucsesfully();
      this.thisComputer.adminIP = this.thisComputer.ip;
    }

    public void rebootThisComputer()
    {
      this.setMouseVisiblity(false);
      this.crashModule.reset();
      this.inputEnabled = false;
      this.bootingUp = true;
      this.thisComputer.disabled = true;
      this.canRunContent = false;
      this.thisComputer.silent = true;
      this.thisComputer.crash(this.thisComputer.ip);
      this.thisComputer.silent = false;
      this.thisComputer.bootupTick(CrashModule.BLUESCREEN_TIME + 0.5f);
      this.crashModule.Update(CrashModule.BLUESCREEN_TIME + 0.5f);
    }

    public void RefreshTheme()
    {
      this.topBarColor = this.defaultTopBarColor;
      this.highlightColor = this.defaultHighlightColor;
      this.moduleColorSolid = this.moduleColorSolidDefault;
    }

    private void threadExecute(object threadText)
    {
      try
      {
        this.validCommand = ProgramRunner.ExecuteProgram((object) this, (string[]) threadText);
      }
      catch (Exception ex)
      {
        int num = 0 + 1;
      }
    }

    public bool hasConnectionPermission(bool admin)
    {
      if (admin)
        return this.connectedComp == null || this.connectedComp.adminIP == this.thisComputer.ip || (int) this.connectedComp.currentUser.type == 0 && this.connectedComp.currentUser.name != null;
      bool flag = !admin;
      if (flag && this.connectedComp != null && !this.connectedComp.userLoggedIn)
        flag = false;
      return this.connectedComp == null || this.connectedComp.adminIP == this.thisComputer.ip || flag;
    }

    public void takeAdmin()
    {
      if (this.connectedComp == null)
        return;
      this.connectedComp.giveAdmin(this.thisComputer.ip);
      this.runCommand("connect " + this.connectedComp.ip);
    }

    public void takeAdmin(string ip)
    {
      Computer computer = Programs.getComputer(this, ip);
      if (computer == null)
        return;
      computer.giveAdmin(this.thisComputer.ip);
      this.runCommand("connect " + computer.ip);
    }

    public void warningFlash()
    {
      this.warningFlashTimer = OS.WARNING_FLASH_TIME;
    }

    public Rectangle getExeBounds()
    {
      int y = this.ram.bounds.Y + RamModule.contentStartOffset;
      for (int index = 0; index < this.exes.Count; ++index)
        y += this.exes[index].bounds.Height;
      return new Rectangle(this.ram.bounds.X, y, 252, (int) OS.EXE_MODULE_HEIGHT);
    }

    public void launchExecutable(string exeName, string exeFileData, int targetPort, string[] allParams = null, string originalName = null)
    {
      int y = this.ram.bounds.Y + RamModule.contentStartOffset;
      for (int index = 0; index < this.exes.Count; ++index)
        y += this.exes[index].bounds.Height;
      Rectangle location = new Rectangle(this.ram.bounds.X, y, RamModule.MODULE_WIDTH, (int) OS.EXE_MODULE_HEIGHT);
      exeName = exeName.ToLower();
      if (exeName.Equals("porthack"))
      {
        bool flag1 = false;
        bool flag2 = false;
        if (this.connectedComp != null)
        {
          int num = 0;
          for (int index = 0; index < this.connectedComp.portsOpen.Count; ++index)
            num += (int) this.connectedComp.portsOpen[index];
          if (num > this.connectedComp.portsNeededForCrack)
            flag1 = true;
          if (this.connectedComp.firewall != null && !this.connectedComp.firewall.solved)
          {
            if (flag1)
              flag2 = true;
            flag1 = false;
          }
        }
        if (flag1)
          this.addExe((ExeModule) new PortHackExe(location, this));
        else if (flag2)
          this.write(LocaleTerms.Loc("Target Machine Rejecting Syndicated UDP Traffic") + " -\n" + LocaleTerms.Loc("Bypass Firewall to allow unrestricted traffic"));
        else
          this.write(LocaleTerms.Loc("Too Few Open Ports to Run") + " - \n" + LocaleTerms.Loc("Open Additional Ports on Target Machine") + "\n");
      }
      else if (exeName.Equals("forkbomb"))
      {
        if (this.hasConnectionPermission(true))
        {
          if (this.connectedComp == null || this.connectedComp.ip.Equals(this.thisComputer.ip))
            this.addExe((ExeModule) new ForkBombExe(location, this, this.thisComputer.ip));
          else if (this.multiplayer && this.connectedComp.ip.Equals(this.opponentComputer.ip))
            this.sendMessage("eForkBomb " + this.connectedComp.ip);
          else
            this.connectedComp.crash(this.thisComputer.ip);
        }
        else
          this.write(LocaleTerms.Loc("Requires Administrator Access to Run"));
      }
      else if (exeName.Equals("shell"))
      {
        if (this.hasConnectionPermission(true))
        {
          bool flag = false;
          string str = this.connectedComp == null ? this.thisComputer.ip : this.connectedComp.ip;
          for (int index = 0; index < this.exes.Count; ++index)
          {
            if (this.exes[index] is ShellExe && this.exes[index].targetIP.Equals(str))
              flag = true;
          }
          if (!flag)
            this.addExe((ExeModule) new ShellExe(location, this));
          else
            this.write(LocaleTerms.Loc("This computer is already running a shell."));
        }
        else
          this.write(LocaleTerms.Loc("Requires Administrator Access to Run"));
      }
      else
      {
        string exeNameForData = PortExploits.GetExeNameForData(originalName == null ? exeName : originalName, exeFileData);
        if (exeNameForData != null)
        {
          switch (exeNameForData)
          {
            case "SSHcrack.exe":
              this.addExe((ExeModule) new SSHCrackExe(location, this));
              break;
            case "FTPBounce.exe":
              this.addExe((ExeModule) new FTPBounceExe(location, this));
              break;
            case "SMTPoverflow.exe":
              this.addExe((ExeModule) new SMTPoverflowExe(location, this));
              break;
            case "WebServerWorm.exe":
              this.addExe((ExeModule) new HTTPExploitExe(location, this));
              break;
            case "Tutorial.exe":
              this.addExe((ExeModule) new AdvancedTutorial(location, this));
              break;
            case "Notes.exe":
              this.addExe((ExeModule) new NotesExe(location, this));
              break;
            case "SecurityTracer.exe":
              this.addExe((ExeModule) new SecurityTraceExe(location, this));
              break;
            case "SQL_MemCorrupt.exe":
              this.addExe((ExeModule) new SQLExploitExe(location, this));
              break;
            case "Decypher.exe":
              this.addExe((ExeModule) new DecypherExe(location, this, allParams));
              break;
            case "DECHead.exe":
              this.addExe((ExeModule) new DecypherTrackExe(location, this, allParams));
              break;
            case "Clock.exe":
              this.addExe((ExeModule) new ClockExe(location, this, allParams));
              break;
            case "KBT_PortTest.exe":
              this.addExe((ExeModule) new MedicalPortExe(location, this, allParams));
              break;
            case "TraceKill.exe":
              this.addExe((ExeModule) new TraceKillExe(location, this, allParams));
              break;
            case "eosDeviceScan.exe":
              this.addExe((ExeModule) new EOSDeviceScannerExe(location, this, allParams));
              break;
            case "themechanger.exe":
              this.addExe((ExeModule) new ThemeChangerExe(location, this, allParams));
              break;
            case "HexClock.exe":
              this.addExe((ExeModule) new HexClockExe(location, this, allParams));
              break;
            case "Sequencer.exe":
              this.addExe((ExeModule) new SequencerExe(location, this, allParams));
              break;
            case "hacknet.exe":
              this.write(" ");
              this.write(" ----- Error ----- ");
              this.write(" ");
              this.write("Program \"hacknet.exe\" is already running!");
              this.write(" ");
              this.write(" ----------------- ");
              this.write(" ");
              break;
            case "TorrentStreamInjector.exe":
              this.addExe((ExeModule) new TorrentPortExe(location, this, allParams));
              break;
            case "SSLTrojan.exe":
              SSLPortExe nullFromArguments1 = SSLPortExe.GenerateInstanceOrNullFromArguments(allParams, location, (object) this, this.connectedComp == null ? this.thisComputer : this.connectedComp);
              if (nullFromArguments1 != null)
              {
                this.addExe((ExeModule) nullFromArguments1);
                break;
              }
              break;
            case "KaguyaTrial.exe":
              bool flag = false;
              for (int index = 0; index < this.exes.Count; ++index)
              {
                if (this.exes[index] is DLCIntroExe)
                  flag = true;
              }
              if (!flag)
              {
                this.addExe((ExeModule) new DLCIntroExe(location, this, allParams));
                break;
              }
              this.write(LocaleTerms.Loc("Kaguya Trials Already Running!"));
              break;
            case "FTPSprint.exe":
              this.addExe((ExeModule) new FTPFastExe(location, this, allParams));
              break;
            case "SignalScramble.exe":
              DLCTraceSlower nullFromArguments2 = DLCTraceSlower.GenerateInstanceOrNullFromArguments(allParams, location, (object) this, this.connectedComp == null ? this.thisComputer : this.connectedComp);
              if (nullFromArguments2 != null)
              {
                this.addExe((ExeModule) nullFromArguments2);
                break;
              }
              break;
            case "MemForensics.exe":
              MemoryForensicsExe nullFromArguments3 = MemoryForensicsExe.GenerateInstanceOrNullFromArguments(location, this, allParams);
              if (nullFromArguments3 != null)
              {
                this.addExe((ExeModule) nullFromArguments3);
                break;
              }
              break;
            case "MemDumpGenerator.exe":
              MemoryDumpDownloader nullFromArguments4 = MemoryDumpDownloader.GenerateInstanceOrNullFromArguments(allParams, location, (object) this, this.connectedComp == null ? this.thisComputer : this.connectedComp);
              if (nullFromArguments4 != null)
              {
                this.addExe((ExeModule) nullFromArguments4);
                break;
              }
              break;
            case "PacificPortcrusher.exe":
              this.addExe((ExeModule) new PacificPortExe(location, this, allParams));
              break;
            case "NetmapOrganizer.exe":
              this.addExe((ExeModule) new NetmapOrganizerExe(location, this, allParams));
              break;
            case "ComShell.exe":
              ShellOverloaderExe.RunShellOverloaderExe(allParams, (object) this, this.connectedComp == null ? this.thisComputer : this.connectedComp);
              break;
            case "DNotes.exe":
              NotesDumperExe.RunNotesDumperExe(allParams, (object) this, this.connectedComp == null ? this.thisComputer : this.connectedComp);
              break;
            case "ClockV2.exe":
              this.addExe((ExeModule) new Clock2Exe(location, this, allParams));
              break;
            case "Tuneswap.exe":
              this.addExe((ExeModule) new TuneswapExe(location, this, allParams));
              break;
            case "RTSPCrack.exe":
              this.addExe((ExeModule) new RTSPPortExe(location, this, allParams));
              break;
            case "ESequencer.exe":
              this.addExe((ExeModule) new ExtensionSequencerExe(location, this, allParams));
              break;
            case "OpShell.exe":
              ShellReopenerExe.RunShellReopenerExe(allParams, (object) this, this.connectedComp == null ? this.thisComputer : this.connectedComp);
              break;
          }
        }
        else
          this.write(LocaleTerms.Loc("Program not Found"));
      }
    }

    public void addExe(ExeModule exe)
    {
      Computer computer = this.connectedComp == null ? this.thisComputer : this.connectedComp;
      if (exe.needsProxyAccess && computer.proxyActive)
        this.write(LocaleTerms.Loc("Proxy Active -- Cannot Execute"));
      else if (this.ramAvaliable >= exe.ramCost)
      {
        exe.LoadContent();
        this.exes.Add(exe);
      }
      else
      {
        this.ram.FlashMemoryWarning();
        this.write(LocaleTerms.Loc("Insufficient Memory"));
      }
    }

    public void failBoot()
    {
      this.graphicsFailBoot();
    }

    public void graphicsFailBoot()
    {
      ThemeManager.switchTheme((object) this, OSTheme.TerminalOnlyBlack);
      this.topBar.Y = -100000;
      this.terminalOnlyMode = true;
    }

    public void sucsesfulBoot()
    {
      this.topBar.Y = 0;
      this.terminalOnlyMode = false;
      this.connectedComp = (Computer) null;
    }

    public struct TrackerDetail
    {
      public Computer comp;
      public float timeLeft;
    }
  }
}
