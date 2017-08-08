// Decompiled with JetBrains decompiler
// Type: Hacknet.MainMenu
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Hacknet.Misc;
using Hacknet.PlatformAPI.Storage;
using Hacknet.Screens;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Threading;
using System.Xml;

namespace Hacknet
{
  internal class MainMenu : GameScreen
  {
    public static string OSVersion = "v5.069";
    public static string AccumErrors = "";
    private bool canLoad = false;
    private bool hasSentErrorEmail = false;
    private int framecount = 0;
    private string testSuiteResult = (string) null;
    private SavefileLoginScreen loginScreen = new SavefileLoginScreen();
    private MainMenu.MainMenuState State = MainMenu.MainMenuState.Normal;
    private AttractModeMenuScreen attractModeScreen = new AttractModeMenuScreen();
    private ExtensionsMenuScreen extensionsScreen = new ExtensionsMenuScreen();
    private bool NextStartedGameShouldBeDLCAccelerated = false;
    private const int CharCountForTestPassedMessage = 950;
    public static Color buttonColor;
    public static Color exitButtonColor;
    private SpriteFont titleFont;
    private Color titleColor;
    private HexGridBackground hexBackground;

    public MainMenu()
    {
      this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
    }

    public override void LoadContent()
    {
      MainMenu.buttonColor = new Color(124, 137, 149);
      MainMenu.exitButtonColor = new Color(105, 82, 82);
      this.titleColor = new Color(190, 190, 190, 0);
      this.titleFont = this.ScreenManager.Game.Content.Load<SpriteFont>("Kremlin");
      this.canLoad = SaveFileManager.StorageMethods[0].GetSaveManifest().LastLoggedInUser.Username != null;
      this.hexBackground = new HexGridBackground(this.ScreenManager.Game.Content);
      this.HookUpCreationEvents();
      MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
      Console.WriteLine("Menu load complete");
    }

    private void HookUpCreationEvents()
    {
      this.loginScreen.RequestGoBack += (Action) (() => this.State = MainMenu.MainMenuState.Normal);
      this.loginScreen.StartNewGameForUsernameAndPass += (Action<string, string>) ((username, pass) =>
      {
        if (SaveFileManager.AddUser(username, pass))
        {
          string filePathForLogin = SaveFileManager.GetFilePathForLogin(username, pass);
          this.ExitScreen();
          MainMenu.resetOS();
          if (!Settings.soundDisabled)
            this.ScreenManager.playAlertSound();
          try
          {
            OS os = new OS();
            os.SaveGameUserName = filePathForLogin;
            os.SaveUserAccountName = username;
            if (this.NextStartedGameShouldBeDLCAccelerated)
            {
              os.IsDLCConventionDemo = true;
              os.Flags.AddFlag("TutorialComplete");
              Settings.EnableDLC = true;
              Settings.initShowsTutorial = false;
              os.initShowsTutorial = false;
            }
            this.ScreenManager.AddScreen((GameScreen) os, new PlayerIndex?(this.ScreenManager.controllingPlayer));
            os.Flags.AddFlag("startVer:" + MainMenu.OSVersion);
            if (!this.NextStartedGameShouldBeDLCAccelerated)
              return;
            SessionAccelerator.AccelerateSessionToDLCStart((object) os);
            os.delayer.Post(ActionDelayer.Wait(0.15), (Action) (() => Game1.getSingleton().IsMouseVisible = true));
            this.NextStartedGameShouldBeDLCAccelerated = false;
          }
          catch (Exception ex)
          {
            this.UpdateUIForSaveCreationFailed(ex);
          }
        }
        else
        {
          this.loginScreen.ResetForNewAccount();
          this.loginScreen.WriteToHistory(" ERROR: Username invalid or already in use.");
        }
      });
      this.loginScreen.LoadGameForUserFileAndUsername += (Action<string, string>) ((userFile, username) =>
      {
        this.ExitScreen();
        MainMenu.resetOS();
        if (SaveFileManager.StorageMethods[0].FileExists(userFile))
        {
          OS.WillLoadSave = true;
          OS os = new OS();
          os.SaveGameUserName = userFile;
          os.SaveUserAccountName = username;
          try
          {
            this.ScreenManager.AddScreen((GameScreen) os, new PlayerIndex?(this.ScreenManager.controllingPlayer));
          }
          catch (XmlException ex)
          {
            this.UpdateUIForSaveCorruption(userFile, (Exception) ex);
          }
          catch (FormatException ex)
          {
            this.UpdateUIForSaveCorruption(userFile, (Exception) ex);
          }
          catch (NullReferenceException ex)
          {
            this.UpdateUIForSaveCorruption(userFile, (Exception) ex);
          }
          catch (FileNotFoundException ex)
          {
            this.UpdateUIForSaveMissing(userFile, (Exception) ex);
          }
          catch (ContentLoadException ex1)
          {
            string str = Utils.ReadEntireContentsOfStream(SaveFileManager.StorageMethods[0].GetFileReadStream(userFile));
            if (str.Contains("DigiPets"))
            {
              string data = str.Replace("DigiPets", "Neopals").Replace("DigiPoints", "Neopoints");
              for (int index = 0; index < 3; ++index)
              {
                try
                {
                  Thread.Sleep(200);
                  SaveFileManager.StorageMethods[0].WriteFileData(userFile, data);
                  break;
                }
                catch (IOException ex2)
                {
                }
                Thread.Sleep(500);
              }
              MainMenu.AccumErrors = "-- Savefile Automatically Upgraded - Try again! --";
            }
            else
              this.UpdateUIForSaveCorruption(userFile, (Exception) ex1);
          }
        }
        else
        {
          OS.WillLoadSave = false;
          this.UpdateUIForSaveMissing(userFile, (Exception) new FileNotFoundException());
        }
      });
      this.attractModeScreen.Start += (Action) (() =>
      {
        try
        {
          this.ExitScreen();
          MainMenu.resetOS();
          if (!Settings.soundDisabled)
            this.ScreenManager.playAlertSound();
          this.ScreenManager.AddScreen((GameScreen) new OS(), new PlayerIndex?(this.ScreenManager.controllingPlayer));
        }
        catch (Exception ex)
        {
          Utils.writeToFile("OS Load Error: " + ex.ToString() + "\n\n" + ex.StackTrace, "crashLog.txt");
        }
      });
      this.attractModeScreen.StartDLC += (Action) (() =>
      {
        try
        {
          this.ExitScreen();
          MainMenu.resetOS();
          Settings.EnableDLC = true;
          Settings.initShowsTutorial = false;
          if (!Settings.soundDisabled)
            this.ScreenManager.playAlertSound();
          OS os = new OS();
          os.IsDLCConventionDemo = true;
          os.Flags.AddFlag("TutorialComplete");
          os.SaveGameUserName = "save_" + Settings.ConventionLoginName + ".xml";
          os.SaveUserAccountName = Settings.ConventionLoginName;
          this.ScreenManager.AddScreen((GameScreen) os, new PlayerIndex?(this.ScreenManager.controllingPlayer));
          os.allFactions.setCurrentFaction("Bibliotheque", os);
          ThemeManager.setThemeOnComputer((object) os.thisComputer, "DLC/Themes/RiptideClassicTheme.xml");
          ThemeManager.switchTheme((object) os, "DLC/Themes/RiptideClassicTheme.xml");
          for (int index1 = 0; index1 < 60; ++index1)
          {
            int index2;
            do
            {
              index2 = Utils.random.Next(os.netMap.nodes.Count);
            }
            while (os.netMap.nodes[index2].idName == "mainHub" || os.netMap.nodes[index2].idName == "entropy00" || os.netMap.nodes[index2].idName == "entropy01");
            os.netMap.discoverNode(os.netMap.nodes[index2]);
          }
          os.delayer.Post(ActionDelayer.Wait(0.15), (Action) (() =>
          {
            Game1.getSingleton().IsMouseVisible = true;
            os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[22], "SSHCrack.exe"));
            os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[21], "FTPBounce.exe"));
            MissionFunctions.runCommand(7, "changeSong");
            MusicManager.stop();
          }));
          os.delayer.Post(ActionDelayer.Wait(38.0), (Action) (() => ComputerLoader.loadMission("Content/DLC/Missions/Demo/DLCDemointroMission1.xml", false)));
        }
        catch (Exception ex)
        {
          Utils.writeToFile("OS Load Error: " + ex.ToString() + "\n\n" + ex.StackTrace, "crashLog.txt");
        }
      });
      this.extensionsScreen.ExitClicked += (Action) (() => this.State = MainMenu.MainMenuState.Normal);
      this.extensionsScreen.CreateNewAccountForExtension_UserAndPass += (Action<string, string>) ((user, pass) => MainMenu.CreateNewAccountForExtensionAndStart(user, pass, this.ScreenManager, (GameScreen) this, this.extensionsScreen));
      this.extensionsScreen.LoadAccountForExtension_FileAndUsername += (Action<string, string>) ((userFile, username) =>
      {
        this.ExitScreen();
        MainMenu.resetOS();
        Settings.IsInExtensionMode = true;
        OS.WillLoadSave = SaveFileManager.StorageMethods[0].FileExists(userFile);
        this.ScreenManager.AddScreen((GameScreen) new OS()
        {
          SaveGameUserName = userFile,
          SaveUserAccountName = username
        }, new PlayerIndex?(this.ScreenManager.controllingPlayer));
      });
    }

    public static void CreateNewAccountForExtensionAndStart(string username, string pass, ScreenManager sman, GameScreen currentScreen = null, ExtensionsMenuScreen extensionsScreen = null)
    {
      if (SaveFileManager.AddUser(username, pass))
      {
        OS os = new OS();
        string filePathForLogin = SaveFileManager.GetFilePathForLogin(username, pass);
        if (currentScreen != null)
          currentScreen.ExitScreen();
        MainMenu.resetOS();
        Settings.IsInExtensionMode = true;
        if (!Settings.soundDisabled)
          sman.playAlertSound();
        os.SaveGameUserName = filePathForLogin;
        os.SaveUserAccountName = username;
        sman.AddScreen((GameScreen) os, new PlayerIndex?(sman.controllingPlayer));
      }
      else if (extensionsScreen != null)
        extensionsScreen.ShowError("Error Creating UserAccount for username :" + username);
      else
        MainMenu.AccumErrors = MainMenu.AccumErrors + "Error auto-loading Extension " + Game1.AutoLoadExtensionPath;
    }

    private void UpdateUIForSaveCorruption(string saveName, Exception ex)
    {
      this.State = MainMenu.MainMenuState.Normal;
      MainMenu.AccumErrors = MainMenu.AccumErrors + string.Format(LocaleTerms.Loc("ACCOUNT FILE CORRUPTION: Account {0} appears to be corrupted, and will not load."), (object) saveName) + " Reported Error:\r\n" + Utils.GenerateReportFromException(ex) + "\r\n";
      if (this.hasSentErrorEmail)
        return;
      new Thread((ThreadStart) (() => Utils.SendErrorEmail(ex, "Save Corruption ", "")))
      {
        IsBackground = true,
        Name = "SaveCorruptErrorReportThread"
      }.Start();
      this.hasSentErrorEmail = true;
    }

    private void UpdateUIForSaveMissing(string saveName, Exception ex)
    {
      this.State = MainMenu.MainMenuState.Normal;
      MainMenu.AccumErrors = MainMenu.AccumErrors + "ACCOUNT FILE NOT FOUND: Account " + saveName + " appears to be missing. It may have been moved or deleted. Reported Error:\r\n" + Utils.GenerateReportFromException(ex) + "\r\n";
      if (this.hasSentErrorEmail)
        return;
      new Thread((ThreadStart) (() => Utils.SendErrorEmail(ex, "Save Missing ", "")))
      {
        IsBackground = true,
        Name = "SaveMissingErrorReportThread"
      }.Start();
      this.hasSentErrorEmail = true;
    }

    private void UpdateUIForSaveCreationFailed(Exception ex)
    {
      this.State = MainMenu.MainMenuState.Normal;
      MainMenu.AccumErrors = MainMenu.AccumErrors + "CRITICAL ERROR CREATING ACCOUNT: Reported Error:\r\n" + Utils.GenerateReportFromException(ex) + "\r\n";
      if (this.hasSentErrorEmail)
        return;
      new Thread((ThreadStart) (() => Utils.SendErrorEmail(ex, "Account Creation Error ", "")))
      {
        IsBackground = true,
        Name = "SaveAccCreationErrorReportThread"
      }.Start();
      this.hasSentErrorEmail = true;
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
      ++this.framecount;
      this.hexBackground.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
    }

    public override void HandleInput(InputState input)
    {
      base.HandleInput(input);
      GuiData.doInput(input);
    }

    public static void resetOS()
    {
      if (Settings.isSpecialTestBuild)
        return;
      TextBox.cursorPosition = 0;
      Settings.initShowsTutorial = Settings.osStartsWithTutorial;
      Settings.IsInExtensionMode = false;
      ScrollablePanel.ClearCache();
      PostProcessor.dangerModeEnabled = false;
      PostProcessor.dangerModePercentComplete = 0.0f;
      PostProcessor.EndingSequenceFlashOutActive = false;
      PostProcessor.EndingSequenceFlashOutPercentageComplete = 0.0f;
    }

    public override void Draw(GameTime gameTime)
    {
      base.Draw(gameTime);
      try
      {
        PostProcessor.begin();
        this.ScreenManager.FadeBackBufferToBlack((int) byte.MaxValue);
        GuiData.startDraw();
        Rectangle dest1 = new Rectangle(0, 0, this.ScreenManager.GraphicsDevice.Viewport.Width, this.ScreenManager.GraphicsDevice.Viewport.Height);
        Rectangle destinationRectangle = new Rectangle(-20, -20, this.ScreenManager.GraphicsDevice.Viewport.Width + 40, this.ScreenManager.GraphicsDevice.Viewport.Height + 40);
        Rectangle dest2 = new Rectangle(dest1.X + dest1.Width / 4, dest1.Height / 4, dest1.Width / 2, dest1.Height / 4);
        GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Black);
        if (Settings.DrawHexBackground)
          this.hexBackground.Draw(dest1, GuiData.spriteBatch, Color.Transparent, Settings.lighterColorHexBackground ? new Color(20, 20, 20) : new Color(15, 15, 15, 0), HexGridBackground.ColoringAlgorithm.NegaitiveSinWash, 0.0f);
        TextItem.DrawShadow = false;
        switch (this.State)
        {
          case MainMenu.MainMenuState.NewUser:
            this.DrawLoginScreen(dest2, true);
            break;
          case MainMenu.MainMenuState.Login:
            this.DrawLoginScreen(dest2, false);
            break;
          case MainMenu.MainMenuState.Extensions:
            this.DrawBackgroundAndTitle();
            this.extensionsScreen.Draw(new Rectangle(180, 150, Math.Min(700, dest1.Width / 2), (int) ((double) dest1.Height * 0.699999988079071)), GuiData.spriteBatch, this.ScreenManager);
            break;
          default:
            if (Settings.isLockedDemoMode)
            {
              this.attractModeScreen.Draw(dest1, GuiData.spriteBatch);
              break;
            }
            bool canRun = this.DrawBackgroundAndTitle();
            if (Settings.isLockedDemoMode)
            {
              this.drawDemoModeButtons(canRun);
            }
            else
            {
              this.drawMainMenuButtons(canRun);
              if (Settings.testingMenuItemsEnabled)
                this.drawTestingMainMenuButtons(canRun);
            }
            break;
        }
        GuiData.endDraw();
        PostProcessor.end();
        this.ScreenManager.FadeBackBufferToBlack((int) byte.MaxValue - (int) this.TransitionAlpha);
      }
      catch (ObjectDisposedException ex)
      {
        if (this.hasSentErrorEmail)
          throw ex;
        string body = Utils.GenerateReportFromException((Exception) ex) + "\r\n Font:" + (object) this.titleFont + "\r\n White:" + (object) Utils.white + "\r\n WhiteDisposed:" + (object) Utils.white.IsDisposed + "\r\n SmallFont:" + (object) GuiData.smallfont + "\r\n TinyFont:" + (object) GuiData.tinyfont + "\r\n LineEffectTarget:" + FlickeringTextEffect.GetReportString() + "\r\n PostProcessort stuff:" + PostProcessor.GetStatusReportString() + "\r\nRESOLUTION:\r\n " + (object) Game1.getSingleton().GraphicsDevice.PresentationParameters.BackBufferWidth + "x" + (object) Game1.getSingleton().GraphicsDevice.PresentationParameters.BackBufferHeight + "\r\nFullscreen: " + (Game1.getSingleton().graphics.IsFullScreen ? "true" : "false") + "\r\n Adapter: " + Game1.getSingleton().GraphicsDevice.Adapter.Description + "\r\n Device Name: " + Game1.getSingleton().GraphicsDevice.Adapter.DeviceName + "\r\n Status: " + (object) Game1.getSingleton().GraphicsDevice.GraphicsDeviceStatus;
        Utils.SendRealWorldEmail("Hacknet " + MainMenu.OSVersion + " Crash " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(), "hacknetbugs+Hacknet@gmail.com", body);
        this.hasSentErrorEmail = true;
        SettingsLoader.writeStatusFile();
      }
    }

    private bool DrawBackgroundAndTitle()
    {
      FlickeringTextEffect.DrawLinedFlickeringText(new Rectangle(180, 120, 340, 100), "HACKNET", 7f, 0.55f, this.titleFont, (object) null, this.titleColor, 2);
      string str = "";
      TextItem.doFontLabel(new Vector2(520f, 178f), "OS" + (DLC1SessionUpgrader.HasDLC1Installed ? "+Labyrinths " : " ") + MainMenu.OSVersion + " " + str, GuiData.smallfont, new Color?(this.titleColor * 0.5f), 600f, 26f, false);
      bool flag = true;
      if (Settings.IsExpireLocked)
      {
        TimeSpan timeSpan = Settings.ExpireTime - DateTime.Now;
        string text;
        if (timeSpan.TotalSeconds < 1.0)
        {
          text = LocaleTerms.Loc("TEST BUILD EXPIRED - EXECUTION DISABLED");
          flag = false;
        }
        else
          text = "Test Build : Expires in " + timeSpan.ToString();
        TextItem.doFontLabel(new Vector2(180f, 105f), text, GuiData.smallfont, new Color?(Color.Red * 0.8f), 600f, 26f, false);
      }
      return flag;
    }

    private void DrawLoginScreen(Rectangle dest, bool needsNewUser = false)
    {
      this.loginScreen.Draw(GuiData.spriteBatch, dest);
    }

    private void drawDemoModeButtons(bool canRun)
    {
      if (Button.doButton(1, 180, 200, 450, 50, LocaleTerms.Loc("New Session"), new Color?(MainMenu.buttonColor)))
      {
        if (canRun)
        {
          try
          {
            this.ExitScreen();
            MainMenu.resetOS();
            this.ScreenManager.playAlertSound();
            this.ScreenManager.AddScreen((GameScreen) new OS(), new PlayerIndex?(this.ScreenManager.controllingPlayer));
          }
          catch (Exception ex)
          {
            Utils.writeToFile(LocaleTerms.Loc("OS Load Error") + ": " + ex.ToString() + "\n\n" + ex.StackTrace, "crashLog.txt");
          }
        }
      }
      if (Button.doButton(11, 180, 265, 450, 50, LocaleTerms.Loc("Load Session"), new Color?(this.canLoad ? MainMenu.buttonColor : Color.Black)))
      {
        if (this.canLoad)
        {
          try
          {
            if (canRun)
            {
              this.ExitScreen();
              MainMenu.resetOS();
              OS.WillLoadSave = true;
              this.ScreenManager.AddScreen((GameScreen) new OS(), new PlayerIndex?(this.ScreenManager.controllingPlayer));
            }
          }
          catch (Exception ex)
          {
            Utils.writeToFile(LocaleTerms.Loc("OS Load Error") + ": " + ex.ToString() + "\n\n" + ex.StackTrace, "crashLog.txt");
          }
        }
      }
      if (!Button.doButton(15, 180, 330, 450, 28, LocaleTerms.Loc("Exit"), new Color?(MainMenu.exitButtonColor)))
        return;
      MusicManager.stop();
      Game1.threadsExiting = true;
      Game1.getSingleton().Exit();
    }

    private void drawTestingMainMenuButtons(bool canRun)
    {
      SpriteFont tinyfont = GuiData.tinyfont;
      string text = "FONT:";
      for (int index = 0; index < tinyfont.Characters.Count; ++index)
      {
        text += (string) (object) tinyfont.Characters[index];
        if (index % 20 == 0)
          text += "\n";
      }
      if (true)
        text = "Labyrinths Testers:\nPress \"Start Full DLC Test\" to begin\n\n" + text;
      GuiData.spriteBatch.DrawString(tinyfont, text, new Vector2(867f, 200f), Color.White);
      if (Button.doButton(8801, 634, 200, 225, 23, "New Test Session", new Color?(MainMenu.buttonColor)) && canRun && canRun)
      {
        this.ExitScreen();
        MainMenu.resetOS();
        if (!Settings.soundDisabled)
          this.ScreenManager.playAlertSound();
        OS os = new OS();
        os.SaveGameUserName = "save_--test.xml";
        os.SaveUserAccountName = "__test";
        this.ScreenManager.AddScreen((GameScreen) os, new PlayerIndex?(this.ScreenManager.controllingPlayer));
        os.Flags.AddFlag("TutorialComplete");
        os.delayer.RunAllDelayedActions();
        os.threadedSaveExecute(false);
        this.ScreenManager.RemoveScreen((GameScreen) os);
        OS.WillLoadSave = true;
        MainMenu.resetOS();
        os = new OS();
        os.SaveGameUserName = "save_--test.xml";
        os.SaveUserAccountName = "__test";
        this.ScreenManager.AddScreen((GameScreen) os, new PlayerIndex?(this.ScreenManager.controllingPlayer));
        os.delayer.Post(ActionDelayer.Wait(0.1), (Action) (() => Game1.getSingleton().IsMouseVisible = true));
        os.delayer.Post(ActionDelayer.Wait(0.4), (Action) (() =>
        {
          os.runCommand("debug");
          ComputerLoader.loadMission("Content/Missions/MainHub/Intro/Intro01.xml", false);
        }));
        if (!Settings.EnableDLC)
          ComputerLoader.loadMission("Content/Missions/BitMission0.xml", false);
      }
      if (Button.doButton(8803, 634, 225, 225, 23, "New DLC Test Session", new Color?(Settings.EnableDLC ? Color.Gray : MainMenu.buttonColor)) && canRun && canRun)
      {
        this.ExitScreen();
        MainMenu.resetOS();
        if (!Settings.soundDisabled)
          this.ScreenManager.playAlertSound();
        OS os1 = new OS();
        os1.SaveGameUserName = "save_--test.xml";
        os1.SaveUserAccountName = "__test";
        this.ScreenManager.AddScreen((GameScreen) os1, new PlayerIndex?(this.ScreenManager.controllingPlayer));
        SessionAccelerator.AccelerateSessionToDLCHA((object) os1);
        os1.threadedSaveExecute(false);
        this.ScreenManager.RemoveScreen((GameScreen) os1);
        OS.WillLoadSave = true;
        MainMenu.resetOS();
        Settings.initShowsTutorial = false;
        OS os2 = new OS();
        os2.SaveGameUserName = "save_--test.xml";
        os2.SaveUserAccountName = "__test";
        this.ScreenManager.AddScreen((GameScreen) os2, new PlayerIndex?(this.ScreenManager.controllingPlayer));
        os2.delayer.Post(ActionDelayer.Wait(0.15), (Action) (() => Game1.getSingleton().IsMouseVisible = true));
      }
      if (Button.doButton(8806, 634, 250, 225, 23, "Run Test Suite", new Color?(MainMenu.buttonColor)))
        this.testSuiteResult = TestSuite.RunTestSuite(this.ScreenManager, false);
      if (Button.doButton(8809, 634, 275, 225, 23, "Run Quick Tests", new Color?(MainMenu.buttonColor)))
      {
        this.testSuiteResult = TestSuite.RunTestSuite(this.ScreenManager, true);
      }
      else
      {
        if (Button.doButton(8812, 634, 300, 225, 23, "Start Full DLC Test", new Color?(MainMenu.buttonColor)) && canRun)
          this.StartFullDLCTest();
        if (this.testSuiteResult == null)
          return;
        TextItem.doFontLabel(new Vector2(635f, 325f), Utils.SuperSmartTwimForWidth(this.testSuiteResult, 600, GuiData.tinyfont), GuiData.tinyfont, new Color?(this.testSuiteResult.Length > 950 ? Utils.AddativeRed : Utils.AddativeWhite), float.MaxValue, float.MaxValue, false);
      }
    }

    private void StartFullDLCTest()
    {
      this.ExitScreen();
      MainMenu.resetOS();
      if (!Settings.soundDisabled)
        this.ScreenManager.playAlertSound();
      OS os = new OS();
      string username = "MediaPreview";
      SaveFileManager.AddUser(username, "test");
      string fileNameForUsername = SaveFileManager.GetSaveFileNameForUsername(username);
      os.IsDLCConventionDemo = true;
      os.Flags.AddFlag("TutorialComplete");
      Settings.EnableDLC = true;
      Settings.initShowsTutorial = false;
      os.SaveGameUserName = fileNameForUsername;
      os.SaveUserAccountName = username;
      os.initShowsTutorial = false;
      this.ScreenManager.AddScreen((GameScreen) os, new PlayerIndex?(this.ScreenManager.controllingPlayer));
      SessionAccelerator.AccelerateSessionToDLCStart((object) os);
      os.delayer.Post(ActionDelayer.Wait(0.15), (Action) (() => Game1.getSingleton().IsMouseVisible = true));
    }

    private void drawMainMenuButtons(bool canRun)
    {
      int num1 = 135;
      int num2;
      if (Button.doButton(1, 180, num2 = num1 + 65, 450, 50, LocaleTerms.Loc("New Session"), new Color?(MainMenu.buttonColor)) && canRun)
      {
        this.NextStartedGameShouldBeDLCAccelerated = false;
        this.State = MainMenu.MainMenuState.NewUser;
        this.loginScreen.ClearTextBox();
        this.loginScreen.ResetForNewAccount();
      }
      bool hasSaves = SaveFileManager.HasSaves;
      string text = LocaleTerms.Loc("No Accounts");
      if (hasSaves)
        text = !this.canLoad ? LocaleTerms.Loc("Invalid Last Account : Login Manually") : string.Format(LocaleTerms.Loc("Continue with account [{0}]"), (object) SaveFileManager.LastLoggedInUser.Username);
      int num3;
      if (Button.doButton(1102, 180, num3 = num2 + 65, 450, 28, text, new Color?(this.canLoad ? MainMenu.buttonColor : Color.Black)) && this.canLoad)
      {
        this.loginScreen.ClearTextBox();
        this.loginScreen.LoadGameForUserFileAndUsername(SaveFileManager.LastLoggedInUser.FileUsername, SaveFileManager.LastLoggedInUser.Username);
      }
      int num4;
      if (Button.doButton(11, 180, num4 = num3 + 39, 450, 50, LocaleTerms.Loc("Login"), new Color?(hasSaves ? MainMenu.buttonColor : Color.Black)))
      {
        if (hasSaves)
        {
          try
          {
            this.State = MainMenu.MainMenuState.Login;
            this.loginScreen.ClearTextBox();
            this.loginScreen.ResetForLogin();
          }
          catch (Exception ex)
          {
            Utils.writeToFile(LocaleTerms.Loc("OS Load Error") + ": " + ex.ToString() + "\n\n" + ex.StackTrace, "crashLog.txt");
          }
        }
      }
      int num5;
      if (Button.doButton(3, 180, num5 = num4 + 65, 450, 50, LocaleTerms.Loc("Settings"), new Color?(MainMenu.buttonColor)))
        this.ScreenManager.AddScreen((GameScreen) new OptionsMenu(), new PlayerIndex?(this.ScreenManager.controllingPlayer));
      int num6;
      int y = num6 = num5 + 65;
      if (Settings.isServerMode)
      {
        if (Button.doButton(4, 180, y, 450, 50, "Start Relay Server", new Color?(MainMenu.buttonColor)))
          this.ScreenManager.AddScreen((GameScreen) new ServerScreen(), new PlayerIndex?(this.ScreenManager.controllingPlayer));
        y += 65;
      }
      if (Settings.AllowExtensionMode)
      {
        if (Button.doButton(5, 180, y, 450, 50, "Extensions", new Color?(MainMenu.buttonColor)))
        {
          this.State = MainMenu.MainMenuState.Extensions;
          this.extensionsScreen.Reset();
        }
        y += 65;
      }
      if (Settings.HasLabyrinthsDemoStartMainMenuButton && DLC1SessionUpgrader.HasDLC1Installed)
      {
        if (Button.doButton(7, 180, y, 450, 28, "New Labyrinths Accelerated Session", new Color?(Color.Lerp(Utils.AddativeWhite, new Color(68, 162, 194), 1f - Utils.rand(0.3f)))) && canRun)
        {
          this.NextStartedGameShouldBeDLCAccelerated = true;
          this.State = MainMenu.MainMenuState.NewUser;
          this.loginScreen.ClearTextBox();
          this.loginScreen.ResetForNewAccount();
        }
        y += 65;
      }
      if (Button.doButton(15, 180, y, 450, 28, LocaleTerms.Loc("Exit"), new Color?(MainMenu.exitButtonColor)))
      {
        MusicManager.stop();
        Game1.threadsExiting = true;
        Game1.getSingleton().Exit();
      }
      int num7 = y + 30;
      if (!PlatformAPISettings.RemoteStorageRunning)
      {
        TextItem.doFontLabel(new Vector2(180f, (float) num7), LocaleTerms.Loc("WARNING: Error connecting to Steam Cloud"), GuiData.smallfont, new Color?(Color.DarkRed), float.MaxValue, float.MaxValue, false);
        num7 += 20;
      }
      if (string.IsNullOrWhiteSpace(MainMenu.AccumErrors))
        return;
      TextItem.doFontLabel(new Vector2(180f, (float) num7), MainMenu.AccumErrors, GuiData.smallfont, new Color?(Color.DarkRed), float.MaxValue, float.MaxValue, false);
      int num8 = num7 + 20;
    }

    private enum MainMenuState
    {
      Normal,
      NewUser,
      Login,
      Extensions,
    }
  }
}
