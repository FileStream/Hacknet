// Decompiled with JetBrains decompiler
// Type: Hacknet.Game1
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Extensions;
using Hacknet.Localization;
using Hacknet.PlatformAPI;
using Hacknet.PlatformAPI.Storage;
using Hacknet.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SDL2;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Hacknet
{
  public class Game1 : Game
  {
    public static string AutoLoadExtensionPath = (string) null;
    private bool resolutionSet = true;
    private bool IsDrawing = false;
    private bool CanLoadContent = false;
    private bool HasLoadedContent = false;
    private bool NeedsSettingsLocaleActivation = false;
    private static Game1 singleton;
    public static bool threadsExiting;
    public static CultureInfo culture;
    public static CultureInfo OriginalCultureInfo;
    public GraphicsDeviceManager graphics;
    public GraphicsDeviceInformation graphicsInfo;
    private SpriteBatch spriteBatch;
    public ScreenManager sman;
    private EventHandler<PreparingDeviceSettingsEventArgs> graphicsPreparedHandler;

    public Game1()
    {
      Game1.OriginalCultureInfo = CultureInfo.CurrentCulture;
      Game1.culture = new CultureInfo("en-US");
      Thread.CurrentThread.CurrentCulture = Game1.culture;
      Thread.CurrentThread.CurrentUICulture = Game1.culture;
      Game1.threadsExiting = false;
      this.graphics = new GraphicsDeviceManager((Game) this);
      this.Content.RootDirectory = "Content";
      this.graphics.DeviceDisposing += new EventHandler<EventArgs>(this.graphics_DeviceDisposing);
      this.graphics.DeviceResetting += new EventHandler<EventArgs>(this.graphics_DeviceResetting);
      this.graphics.DeviceReset += new EventHandler<EventArgs>(this.graphics_DeviceReset);
      PlatformAPISettings.InitPlatformAPI();
      SettingsLoader.checkStatus();
      if (SettingsLoader.didLoad)
      {
        this.CanLoadContent = true;
        this.graphics.PreferredBackBufferWidth = Math.Min(SettingsLoader.resWidth, 4096);
        this.graphics.PreferredBackBufferHeight = Math.Min(SettingsLoader.resHeight, 4096);
        this.graphics.IsFullScreen = SettingsLoader.isFullscreen;
        this.graphics.PreferMultiSampling = SettingsLoader.ShouldMultisample;
        if (Settings.StartOnAltMonitor & !SettingsLoader.isFullscreen)
          this.graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(this.graphics_PreparingDeviceSettingsForAltMonitor);
        Console.WriteLine("Loaded Settings - Language Code: " + Settings.ActiveLocale);
      }
      else
      {
        this.graphics.PreferMultiSampling = true;
        if (Settings.windowed)
        {
          this.graphics.PreferredBackBufferWidth = 1280;
          this.graphics.PreferredBackBufferHeight = 800;
          this.CanLoadContent = true;
        }
        else
        {
          this.graphicsPreparedHandler = new EventHandler<PreparingDeviceSettingsEventArgs>(this.graphics_PreparingDeviceSettings);
          this.graphics.PreparingDeviceSettings += this.graphicsPreparedHandler;
          this.graphics.PreferredBackBufferWidth = 1280;
          this.graphics.PreferredBackBufferHeight = 800;
          this.graphics.IsFullScreen = true;
        }
        this.NeedsSettingsLocaleActivation = true;
        Console.WriteLine("Settings file not found, setting defaults.");
      }
      this.CheckAndFixWindowPosition();
      this.IsMouseVisible = true;
      this.IsFixedTimeStep = false;
      Game1.singleton = this;
      this.Exiting += new EventHandler<EventArgs>(this.handleExit);
      StatsManager.InitStats();
    }

    private void GraphicsDevice_DeviceLost(object sender, EventArgs e)
    {
      Console.WriteLine("Device Lost...");
    }

    private void graphics_DeviceReset(object sender, EventArgs e)
    {
      Program.GraphicsDeviceResetLog = Program.GraphicsDeviceResetLog + "Reset at " + DateTime.Now.ToShortTimeString();
      Console.WriteLine("Graphics Device Reset Started");
      if (Utils.white != null)
      {
        Utils.white.Dispose();
        Utils.white = (Texture2D) null;
      }
      this.LoadRegenSafeContent();
      this.HasLoadedContent = true;
      Console.WriteLine("Graphics Device Reset Complete");
    }

    private void graphics_DeviceResetting(object sender, EventArgs e)
    {
      this.HasLoadedContent = false;
      Console.WriteLine("Graphics Device Resetting");
    }

    private void graphics_DeviceDisposing(object sender, EventArgs e)
    {
      this.HasLoadedContent = false;
      Console.WriteLine("Graphics Device Disposing");
    }

    private void graphics_PreparingDeviceSettingsForAltMonitor(object sender, PreparingDeviceSettingsEventArgs e)
    {
      foreach (GraphicsAdapter adapter in GraphicsAdapter.Adapters)
      {
        if (!adapter.IsDefaultAdapter)
        {
          e.GraphicsDeviceInformation.Adapter = adapter;
          break;
        }
      }
    }

    private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
    {
      try
      {
        DisplayMode currentDisplayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
        int width = currentDisplayMode.Width;
        int height = currentDisplayMode.Height;
        this.graphics.PreferredBackBufferWidth = Math.Min(width, 4096);
        this.graphics.PreferredBackBufferHeight = Math.Min(height, 4096);
        this.graphics.PreferMultiSampling = true;
        this.resolutionSet = false;
        this.graphics.PreparingDeviceSettings -= this.graphicsPreparedHandler;
        this.CanLoadContent = true;
      }
      catch (Exception ex)
      {
        string reportFromException = Utils.GenerateReportFromException(ex);
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/My Games/Hacknet/Reports/";
        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);
        Utils.writeToFile(reportFromException, path + "Graphics_CrashReport_" + Guid.NewGuid().ToString().Replace(" ", "_") + ".txt");
        this.graphics.PreferredBackBufferWidth = 1280;
        this.graphics.PreferredBackBufferHeight = 800;
        this.graphics.PreferMultiSampling = true;
        this.graphics.IsFullScreen = false;
      }
    }

    public void setNewGraphics()
    {
      this.graphics.ApplyChanges();
      GameScreen[] screens = this.sman.GetScreens();
      bool flag = false;
      string str1 = (string) null;
      string str2 = (string) null;
      for (int index = 0; index < screens.Length; ++index)
      {
        OS os = screens[index] as OS;
        if (os != null)
        {
          os.threadedSaveExecute(false);
          flag = true;
          str1 = os.SaveGameUserName;
          str2 = os.SaveUserAccountName;
          break;
        }
      }
      this.Components.Remove((IGameComponent) this.sman);
      this.sman = new ScreenManager((Game) this);
      this.Components.Add((IGameComponent) this.sman);
      this.LoadGraphicsContent();
      if (flag)
      {
        OS.WillLoadSave = true;
        OS os = new OS();
        os.SaveGameUserName = str1;
        os.SaveUserAccountName = str2;
        bool isInExtensionMode = Settings.IsInExtensionMode;
        MainMenu.resetOS();
        Settings.IsInExtensionMode = isInExtensionMode;
        this.sman.AddScreen((GameScreen) os, new PlayerIndex?(this.sman.controllingPlayer));
      }
      GuiData.spriteBatch = this.sman.SpriteBatch;
      if (this.sman.GetScreens().Length != 0)
        return;
      this.LoadInitialScreens();
    }

    private void CheckAndFixWindowPosition()
    {
      int x;
      int y;
      SDL.SDL_GetWindowPosition(this.Window.Handle, out x, out y);
      if (this.graphics.IsFullScreen)
        return;
      SDL.SDL_SetWindowPosition(this.Window.Handle, Math.Max(x, 10), Math.Max(y, 10));
    }

    public void setWindowPosition(Vector2 pos)
    {
      if (SettingsLoader.isFullscreen)
        return;
      this.Window.IsBorderlessEXT = true;
      SDL.SDL_SetWindowPosition(this.Window.Handle, (int) pos.X, (int) pos.Y);
    }

    protected override void Initialize()
    {
      this.sman = new ScreenManager((Game) this);
      this.Components.Add((IGameComponent) this.sman);
      this.graphics.PreferMultiSampling = true;
      NameGenerator.init();
      PatternDrawer.init(this.Content);
      ProgramList.init();
      Cube3D.Initilize(this.graphics.GraphicsDevice);
      AlienwareFXManager.Init();
      this.graphics.GraphicsDevice.DeviceLost += new EventHandler<EventArgs>(this.GraphicsDevice_DeviceLost);
      base.Initialize();
    }

    private void handleExit(object sender, EventArgs e)
    {
      Game1.threadsExiting = true;
      MusicManager.stop();
      AlienwareFXManager.ReleaseHandle();
    }

    private void LoadRegenSafeContent()
    {
      Utils.white = new Texture2D(this.graphics.GraphicsDevice, 1, 1);
      Color[] data = new Color[1]{ Color.White };
      Utils.white.SetData<Color>(data);
      ContentManager content = this.Content;
      Utils.gradient = content.Load<Texture2D>("Gradient");
      Utils.gradientLeftRight = content.Load<Texture2D>("GradientHorizontal");
    }

    protected override void LoadContent()
    {
      if (!this.CanLoadContent)
        return;
      PortExploits.populate();
      this.sman.controllingPlayer = PlayerIndex.One;
      if (Settings.isConventionDemo)
        this.setWindowPosition(new Vector2(200f, 200f));
      this.LoadGraphicsContent();
      this.LoadRegenSafeContent();
      ContentManager content = this.Content;
      GuiData.font = content.Load<SpriteFont>("Font23");
      GuiData.titlefont = content.Load<SpriteFont>("Kremlin");
      GuiData.smallfont = this.Content.Load<SpriteFont>("Font12");
      GuiData.UISmallfont = GuiData.smallfont;
      GuiData.tinyfont = this.Content.Load<SpriteFont>("Font10");
      GuiData.UITinyfont = this.Content.Load<SpriteFont>("Font10");
      GuiData.detailfont = this.Content.Load<SpriteFont>("Font7");
      GuiData.spriteBatch = this.sman.SpriteBatch;
      GuiData.InitFontOptions(this.Content);
      GuiData.init(this.Window);
      DLC1SessionUpgrader.CheckForDLCFiles();
      VehicleInfo.init();
      WorldLocationLoader.init();
      ThemeManager.init(content);
      MissionGenerationParser.init();
      MissionGenerator.init(content);
      UsernameGenerator.init();
      MusicManager.init(content);
      SFX.init(content);
      OldSystemSaveFileManifest.Load();
      try
      {
        SaveFileManager.Init(true);
      }
      catch (UnauthorizedAccessException ex)
      {
        MainMenu.AccumErrors += " ---- WARNING ---\nHacknet cannot access the Save File Folder (Path Below) to read/write save files.\nNO PROGRESS WILL BE SAVED.\n";
        MainMenu.AccumErrors += "Check folder permissions, run Hacknet.exe as Administrator, and try again.\n";
        MainMenu.AccumErrors += "If Errors Persist, search for \"Hacknet Workaround\" for a steam forums thread with more options.\n";
        MainMenu.AccumErrors = MainMenu.AccumErrors + ":: Error Details ::\n" + Utils.GenerateReportFromException((Exception) ex);
      }
      if (this.NeedsSettingsLocaleActivation)
      {
        if (!Settings.ForceEnglish)
        {
          string forActiveLanguage = PlatformAPISettings.GetCodeForActiveLanguage(LocaleActivator.SupportedLanguages);
          LocaleActivator.ActivateLocale(forActiveLanguage, this.Content);
          Settings.ActiveLocale = forActiveLanguage;
        }
      }
      else if (SettingsLoader.didLoad && Settings.ActiveLocale != "en-us")
        LocaleActivator.ActivateLocale(Settings.ActiveLocale, this.Content);
      Helpfile.init();
      FileEntry.init(this.Content);
      this.HasLoadedContent = true;
      this.LoadInitialScreens();
      if (!WebRenderer.Enabled)
        return;
      XNAWebRenderer.XNAWR_Initialize("file:///nope.html", WebRenderer.textureUpdated, 512, 512);
      WebRenderer.setSize(512, 512);
    }

    protected void LoadGraphicsContent()
    {
      this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
      if (this.NeedsSettingsLocaleActivation)
        SettingsLoader.ShouldMultisample = this.GraphicsDevice.Adapter.IsProfileSupported(GraphicsProfile.HiDef);
      PostProcessor.init(this.graphics.GraphicsDevice, this.spriteBatch, this.Content);
      WebRenderer.init(this.graphics.GraphicsDevice);
    }

    protected void LoadInitialScreens()
    {
      if (Settings.ForceEnglish)
        LocaleActivator.ActivateLocale(Settings.ActiveLocale, this.Content);
      if (Game1.AutoLoadExtensionPath != null)
      {
        ExtensionLoader.ActiveExtensionInfo = ExtensionInfo.ReadExtensionInfo(Game1.AutoLoadExtensionPath);
        SaveFileManager.Init(true);
        SaveFileManager.DeleteUser("test");
        MainMenu.CreateNewAccountForExtensionAndStart("test", "test", this.sman, (GameScreen) null, (ExtensionsMenuScreen) null);
      }
      else if (Settings.MenuStartup)
        this.sman.AddScreen((GameScreen) new MainMenu(), new PlayerIndex?(this.sman.controllingPlayer));
      else
        this.sman.AddScreen((GameScreen) new OS(), new PlayerIndex?(this.sman.controllingPlayer));
    }

    protected override void UnloadContent()
    {
      if (!WebRenderer.Enabled)
        return;
      XNAWebRenderer.XNAWR_Shutdown();
    }

    protected override void Update(GameTime gameTime)
    {
      if (!this.HasLoadedContent)
        this.LoadContent();
      if (WebRenderer.Enabled)
        XNAWebRenderer.XNAWR_Update();
      if (!this.resolutionSet)
      {
        this.setNewGraphics();
        this.resolutionSet = true;
      }
      TimeSpan elapsedGameTime = gameTime.ElapsedGameTime;
      PatternDrawer.update((float) elapsedGameTime.TotalSeconds);
      elapsedGameTime = gameTime.ElapsedGameTime;
      GuiData.setTimeStep((float) elapsedGameTime.TotalSeconds);
      elapsedGameTime = gameTime.ElapsedGameTime;
      MusicManager.Update((float) elapsedGameTime.TotalSeconds);
      elapsedGameTime = gameTime.ElapsedGameTime;
      ThemeManager.Update((float) elapsedGameTime.TotalSeconds);
      AlienwareFXManager.UpdateForOS((object) OS.currentInstance);
      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      if (!this.HasLoadedContent)
        return;
      this.IsDrawing = true;
      base.Draw(gameTime);
      this.IsDrawing = false;
    }

    public static Game1 getSingleton()
    {
      return Game1.singleton;
    }
  }
}
