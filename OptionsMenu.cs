// Decompiled with JetBrains decompiler
// Type: Hacknet.OptionsMenu
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class OptionsMenu : GameScreen
  {
    private int currentFontIndex = -1;
    private char[] xArray = new char[1]{ 'x' };
    private bool needsApply = false;
    private bool mouseHasBeenReleasedOnThisScreen = false;
    private string originallyActiveLocale = "en-us";
    private int currentLocaleIndex = 0;
    private bool startedFromGameContext = false;
    private string[] resolutions;
    private int currentResIndex;
    private string[] fontConfigs;
    private bool resolutionChanged;
    private bool windowed;
    private string[] localeNames;

    public OptionsMenu()
    {
      this.resolutionChanged = false;
    }

    public OptionsMenu(bool startedFromGameContext)
    {
      this.resolutionChanged = false;
      this.startedFromGameContext = startedFromGameContext;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.resolutions = new string[20];
      List<string> stringList = new List<string>();
      stringList.Add("1152x768");
      stringList.Add("1280x854");
      stringList.Add("1440x960");
      stringList.Add("2880x1920");
      stringList.Add("1152x864");
      stringList.Add("1280x960");
      stringList.Add("1400x1050");
      stringList.Add("1600x1200");
      stringList.Add("1280x760");
      stringList.Add("1280x1024");
      stringList.Add("1280x720");
      stringList.Add("1365x760");
      stringList.Add("1366x768");
      stringList.Add("1408x792");
      stringList.Add("1600x900");
      stringList.Add("1920x1080");
      stringList.Add("2560x1440");
      stringList.Add("1280x800");
      stringList.Add("1440x900");
      stringList.Add("1680x1050");
      stringList.Add("1920x1200");
      stringList.Add("1792x1008");
      stringList.Add("2560x1600");
      stringList.Add("2560x1080");
      stringList.Add("3440x1440");
      stringList.Add("3440x1440");
      stringList.Add("3840x2160");
      stringList.Add("4096x2160");
      stringList.Sort();
      this.resolutions = stringList.ToArray();
      this.fontConfigs = new string[GuiData.FontConfigs.Count];
      for (int index = 0; index < GuiData.FontConfigs.Count; ++index)
      {
        this.fontConfigs[index] = GuiData.FontConfigs[index].name;
        if (GuiData.ActiveFontConfig.name == this.fontConfigs[index])
          this.currentFontIndex = index;
      }
      for (int index = 0; index < this.resolutions.Length; ++index)
      {
        if (this.resolutions[index].Equals(this.getCurrentResolution()))
        {
          this.currentResIndex = index;
          break;
        }
      }
      this.windowed = this.getIfWindowed();
      this.localeNames = new string[LocaleActivator.SupportedLanguages.Count];
      for (int index = 0; index < LocaleActivator.SupportedLanguages.Count; ++index)
      {
        this.localeNames[index] = LocaleActivator.SupportedLanguages[index].Name;
        if (LocaleActivator.SupportedLanguages[index].Code == Settings.ActiveLocale)
          this.currentLocaleIndex = index;
      }
      this.originallyActiveLocale = Settings.ActiveLocale;
    }

    public string getCurrentResolution()
    {
      return "" + (object) this.ScreenManager.GraphicsDevice.Viewport.Width + "x" + (object) this.ScreenManager.GraphicsDevice.Viewport.Height;
    }

    public bool getIfWindowed()
    {
      return Game1.getSingleton().graphics.IsFullScreen;
    }

    public void apply()
    {
      bool flag = false;
      if (this.windowed != this.getIfWindowed())
      {
        Game1.getSingleton().graphics.ToggleFullScreen();
        Settings.windowed = this.getIfWindowed();
        flag = true;
      }
      if (this.resolutionChanged)
      {
        string[] strArray = this.resolutions[this.currentResIndex].Split(this.xArray);
        int int32_1 = Convert.ToInt32(strArray[0]);
        int int32_2 = Convert.ToInt32(strArray[1]);
        Game1.getSingleton().graphics.PreferredBackBufferWidth = int32_1;
        Game1.getSingleton().graphics.PreferredBackBufferHeight = int32_2;
        Game1.getSingleton().graphics.PreferMultiSampling = SettingsLoader.ShouldMultisample;
        PostProcessor.GenerateMainTarget(Game1.getSingleton().graphics.GraphicsDevice);
      }
      GuiData.ActivateFontConfig(this.fontConfigs[this.currentFontIndex]);
      if (this.resolutionChanged || flag)
      {
        Game1.getSingleton().graphics.ApplyChanges();
        Game1.getSingleton().setNewGraphics();
      }
      else
        this.ExitScreen();
      SettingsLoader.writeStatusFile();
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
      if (this.needsApply)
      {
        this.ScreenManager.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
        this.apply();
        this.needsApply = false;
      }
      if (GuiData.mouse.LeftButton != ButtonState.Released)
        return;
      this.mouseHasBeenReleasedOnThisScreen = true;
    }

    public override void HandleInput(InputState input)
    {
      base.HandleInput(input);
      GuiData.doInput(input);
      if (!Settings.debugCommandsEnabled || !Utils.keyPressed(input, Keys.F8, new PlayerIndex?()))
        return;
      this.ExitScreen();
      Game1.getSingleton().Exit();
    }

    public override void Draw(GameTime gameTime)
    {
      base.Draw(gameTime);
      PostProcessor.begin();
      this.ScreenManager.FadeBackBufferToBlack((int) byte.MaxValue);
      GuiData.startDraw();
      int x = 0;
      int y = 0;
      Viewport viewport = this.ScreenManager.GraphicsDevice.Viewport;
      int width = viewport.Width;
      viewport = this.ScreenManager.GraphicsDevice.Viewport;
      int height = viewport.Height;
      PatternDrawer.draw(new Rectangle(x, y, width, height), 0.5f, Color.Black, new Color(2, 2, 2), GuiData.spriteBatch);
      if (Button.doButton(999, 10, 10, 220, 30, "<- " + LocaleTerms.Loc("Back"), new Color?(Color.Gray)))
      {
        SettingsLoader.writeStatusFile();
        this.ExitScreen();
      }
      if (Button.doButton(9907, 10, 44, 220, 20, LocaleTerms.Loc("Apply Changes"), new Color?(Color.LightBlue)))
        this.needsApply = true;
      int num1 = 100;
      TextItem.doLabel(new Vector2(400f, (float) num1), LocaleTerms.Loc("Resolutions"), new Color?(), 200f);
      int currentResIndex = this.currentResIndex;
      this.currentResIndex = SelectableTextList.doFancyList(10, 400, num1 + 36, 200, 450, this.resolutions, this.currentResIndex, new Color?(), false);
      if (!this.mouseHasBeenReleasedOnThisScreen)
        this.currentResIndex = currentResIndex;
      else if (SelectableTextList.wasActivated)
        this.resolutionChanged = true;
      if (!this.startedFromGameContext)
      {
        TextItem.doLabel(new Vector2(620f, (float) num1), LocaleTerms.Loc("Language"), new Color?(), 200f);
        int currentLocaleIndex = this.currentLocaleIndex;
        this.currentLocaleIndex = SelectableTextList.doFancyList(1013, 620, num1 + 36, 200, 450, this.localeNames, this.currentLocaleIndex, new Color?(), false);
        if (!this.mouseHasBeenReleasedOnThisScreen)
          this.currentLocaleIndex = currentLocaleIndex;
        else if (SelectableTextList.wasActivated)
        {
          LocaleActivator.ActivateLocale(LocaleActivator.SupportedLanguages[this.currentLocaleIndex].Code, Game1.getSingleton().Content);
          Settings.ActiveLocale = LocaleActivator.SupportedLanguages[this.currentLocaleIndex].Code;
        }
      }
      int num2 = 64;
      float MaxWidth = 280f;
      int num3;
      TextItem.doLabel(new Vector2(100f, (float) (num3 = num2 + 36)), LocaleTerms.Loc("Fullscreen"), new Color?(), MaxWidth);
      int num4;
      this.windowed = CheckBox.doCheckBox(20, 100, num4 = num3 + 34, this.windowed, new Color?());
      int num5;
      TextItem.doLabel(new Vector2(100f, (float) (num5 = num4 + 32)), LocaleTerms.Loc("Bloom"), new Color?(), MaxWidth);
      int num6;
      PostProcessor.bloomEnabled = CheckBox.doCheckBox(21, 100, num6 = num5 + 34, PostProcessor.bloomEnabled, new Color?());
      int num7;
      TextItem.doLabel(new Vector2(100f, (float) (num7 = num6 + 32)), LocaleTerms.Loc("Scanlines"), new Color?(), MaxWidth);
      int num8;
      PostProcessor.scanlinesEnabled = CheckBox.doCheckBox(22, 100, num8 = num7 + 34, PostProcessor.scanlinesEnabled, new Color?());
      int num9;
      TextItem.doLabel(new Vector2(100f, (float) (num9 = num8 + 32)), LocaleTerms.Loc("Multisampling"), new Color?(), MaxWidth);
      bool shouldMultisample = SettingsLoader.ShouldMultisample;
      int num10;
      SettingsLoader.ShouldMultisample = CheckBox.doCheckBox(221, 100, num10 = num9 + 34, SettingsLoader.ShouldMultisample, new Color?());
      if (shouldMultisample != SettingsLoader.ShouldMultisample)
        this.resolutionChanged = true;
      int num11;
      TextItem.doLabel(new Vector2(100f, (float) (num11 = num10 + 32)), LocaleTerms.Loc("Audio Visualiser"), new Color?(), MaxWidth);
      int num12;
      SettingsLoader.ShouldDrawMusicVis = CheckBox.doCheckBox(223, 100, num12 = num11 + 34, SettingsLoader.ShouldDrawMusicVis, new Color?());
      int num13;
      TextItem.doLabel(new Vector2(100f, (float) (num13 = num12 + 32)), LocaleTerms.Loc("Sound Enabled"), new Color?(), MaxWidth);
      int num14;
      MusicManager.setIsMuted(!CheckBox.doCheckBox(23, 100, num14 = num13 + 34, !MusicManager.isMuted, new Color?()));
      int num15;
      TextItem.doLabel(new Vector2(100f, (float) (num15 = num14 + 32)), LocaleTerms.Loc("Music Volume"), new Color?(), MaxWidth);
      int num16;
      MusicManager.setVolume(SliderBar.doSliderBar(24, 100, num16 = num15 + 34, 210, 30, 1f, 0.0f, MusicManager.getVolume(), 1f / 1000f));
      int num17;
      TextItem.doLabel(new Vector2(100f, (float) (num17 = num16 + 32)), LocaleTerms.Loc("Text Size"), new Color?(), MaxWidth);
      int currentFontIndex = this.currentFontIndex;
      int num18;
      this.currentFontIndex = SelectableTextList.doFancyList(25, 100, num18 = num17 + 34, 200, 160, this.fontConfigs, this.currentFontIndex, new Color?(), false);
      if (this.currentFontIndex != currentFontIndex && this.startedFromGameContext)
      {
        try
        {
          if (OS.currentInstance != null)
            OS.currentInstance.terminal.reset();
        }
        catch (Exception ex)
        {
          Console.WriteLine((object) ex);
          Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
        }
      }
      if (Button.doButton(990, 10, num18 + 150, 220, 30, LocaleTerms.Loc("Apply Changes"), new Color?(Color.LightBlue)))
        this.needsApply = true;
      GuiData.endDraw();
      PostProcessor.end();
    }
  }
}
