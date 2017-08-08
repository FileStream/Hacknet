// Decompiled with JetBrains decompiler
// Type: Hacknet.ThemeManager
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hacknet
{
  public static class ThemeManager
  {
    private static string CustomThemeIDSeperator = "___";
    private static string customBackgroundImageLoadPath = (string) null;
    private static bool backgroundNeedsDisposal = false;
    public static CustomTheme LastLoadedCustomTheme = (CustomTheme) null;
    public static string LastLoadedCustomThemePath = (string) null;
    private static int framesTillWebUpdate = -1;
    private static bool HasNeverSwappedThemeBefore = true;
    private static Texture2D backgroundImage;
    private static Texture2D lastLoadedCustomBackground;
    public static OSTheme currentTheme;
    private static HexGridBackground hexGrid;
    private static Dictionary<OSTheme, string> fileData;
    private static int webWidth;
    private static int webHeight;

    public static void init(ContentManager content)
    {
      ThemeManager.fileData = new Dictionary<OSTheme, string>();
      UTF8Encoding utF8Encoding = new UTF8Encoding();
      ThemeManager.hexGrid = new HexGridBackground(content);
      ThemeManager.hexGrid.HexScale = 0.12f;
      foreach (OSTheme key in Enum.GetValues(typeof (OSTheme)))
      {
        byte[] bytes = utF8Encoding.GetBytes(key.ToString() + key.GetHashCode().ToString());
        string str = "";
        for (int index = 0; index < bytes.Length; ++index)
          str += (string) (object) bytes[index];
        ThemeManager.fileData.Add(key, str);
      }
    }

    public static void Update(float dt)
    {
      if (ThemeManager.hexGrid != null)
      {
        ThemeManager.hexGrid.Update(dt * 0.4f);
        ThemeManager.hexGrid.HexScale = 0.2f;
      }
      if (ThemeManager.framesTillWebUpdate < 0)
        return;
      --ThemeManager.framesTillWebUpdate;
      if (ThemeManager.framesTillWebUpdate == -1)
      {
        if (OS.currentInstance.connectedComp != null && OS.currentInstance.connectedComp.getDaemon(typeof (WebServerDaemon)) != null)
          WebRenderer.setSize(ThemeManager.webWidth, ThemeManager.webHeight);
        else
          ThemeManager.framesTillWebUpdate = 0;
      }
    }

    public static void switchTheme(object osObject, string customThemePath)
    {
      string str = "Content/";
      if (Settings.IsInExtensionMode)
        str = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
      if (File.Exists(str + customThemePath))
      {
        ThemeManager.LastLoadedCustomTheme = CustomTheme.Deserialize(str + customThemePath);
        ThemeManager.LastLoadedCustomThemePath = customThemePath;
      }
      else if (File.Exists("Content/" + customThemePath))
        ThemeManager.LastLoadedCustomTheme = CustomTheme.Deserialize("Content/" + customThemePath);
      ThemeManager.switchTheme(osObject, OSTheme.Custom);
    }

    public static void switchTheme(object osObject, OSTheme theme)
    {
      OS os = (OS) osObject;
      if (theme == OSTheme.Custom)
      {
        string backgroundImageLoadPath = ThemeManager.customBackgroundImageLoadPath;
        ThemeManager.switchTheme((object) os, OSTheme.HacknetBlue);
        ThemeManager.customBackgroundImageLoadPath = backgroundImageLoadPath;
        ThemeManager.switchThemeLayout(os, ThemeManager.LastLoadedCustomTheme.GetThemeForLayout());
        ThemeManager.loadCustomThemeBackground(os, ThemeManager.LastLoadedCustomTheme.backgroundImagePath);
        ThemeManager.LastLoadedCustomTheme.LoadIntoOS((object) os);
      }
      else
      {
        ThemeManager.switchThemeColors(os, theme);
        ThemeManager.loadThemeBackground(os, theme);
        ThemeManager.switchThemeLayout(os, theme);
      }
      ThemeManager.currentTheme = theme;
      os.RefreshTheme();
    }

    private static void switchThemeLayout(OS os, OSTheme theme)
    {
      int width1 = os.ScreenManager.GraphicsDevice.Viewport.Width;
      int height1 = os.ScreenManager.GraphicsDevice.Viewport.Height;
      int width2 = os.display.bounds.Width;
      int height2 = os.display.bounds.Height;
      switch (theme)
      {
        case OSTheme.TerminalOnlyBlack:
          Rectangle rectangle = new Rectangle(-100000, -100000, 16, 16);
          os.terminal.bounds = new Rectangle(0, 0, width1, height1);
          os.netMap.bounds = rectangle;
          os.display.bounds = rectangle;
          os.ram.bounds = rectangle;
          break;
        case OSTheme.HackerGreen:
          int height3 = 205;
          int width3 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * 0.4442);
          int num1 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * 0.5558);
          int height4 = height1 - height3 - OS.TOP_BAR_HEIGHT - 6;
          os.terminal.Bounds = new Rectangle(width1 - width3 - RamModule.MODULE_WIDTH - 4, OS.TOP_BAR_HEIGHT, width3, height1 - OS.TOP_BAR_HEIGHT - 2);
          os.netMap.Bounds = new Rectangle(2, height1 - height3 - 2, num1 - 1, height3);
          os.display.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, num1 - 2, height4);
          os.ram.Bounds = new Rectangle(width1 - RamModule.MODULE_WIDTH - 2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
          break;
        case OSTheme.HacknetWhite:
          int height5 = 205;
          int width4 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * 0.4442);
          int width5 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * 0.5558);
          int height6 = height1 - height5 - OS.TOP_BAR_HEIGHT - 6;
          os.terminal.Bounds = new Rectangle(width1 - width4 - 2, OS.TOP_BAR_HEIGHT, width4, height6);
          os.netMap.Bounds = new Rectangle(RamModule.MODULE_WIDTH + 4 + width5 + 4, height1 - height5 - 2, os.terminal.bounds.Width - 4, height5);
          os.display.Bounds = new Rectangle(RamModule.MODULE_WIDTH + 4, OS.TOP_BAR_HEIGHT, width5, height1 - OS.TOP_BAR_HEIGHT - 2);
          os.ram.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
          break;
        case OSTheme.HacknetMint:
          int height7 = 205;
          int num2 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * 0.5058);
          int width6 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * 0.4942);
          int height8 = height1 - height7 - OS.TOP_BAR_HEIGHT - 6;
          os.terminal.Bounds = new Rectangle(RamModule.MODULE_WIDTH + 4, OS.TOP_BAR_HEIGHT, width6, height8);
          os.netMap.Bounds = new Rectangle(RamModule.MODULE_WIDTH + 4, height1 - height7 - 2, width6 - 1, height7);
          os.display.Bounds = new Rectangle(width1 - num2 - 2, OS.TOP_BAR_HEIGHT, num2 - 1, height1 - OS.TOP_BAR_HEIGHT - 2);
          os.ram.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
          break;
        case OSTheme.Colamaeleon:
          int height9 = (int) ((double) height1 * 0.33);
          double num3 = 0.4;
          int width7 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * (1.0 - num3)) - 6;
          int width8 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * num3);
          int height10 = height1 - height9 - OS.TOP_BAR_HEIGHT - 6 + 1;
          os.terminal.Bounds = new Rectangle(width1 - width7 - RamModule.MODULE_WIDTH - 4, OS.TOP_BAR_HEIGHT, width7, height1 - OS.TOP_BAR_HEIGHT - 2);
          os.netMap.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, width8, height9);
          os.display.Bounds = new Rectangle(2, os.netMap.bounds.Y + os.netMap.bounds.Height + 3, width8, height10);
          os.ram.Bounds = new Rectangle(width1 - RamModule.MODULE_WIDTH - 2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
          break;
        case OSTheme.GreenCompact:
          int height11 = 205;
          int num4 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * 0.4442);
          int num5 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * 0.5558);
          int height12 = height1 - height11 - OS.TOP_BAR_HEIGHT - 3;
          os.terminal.Bounds = new Rectangle(width1 - num4 - RamModule.MODULE_WIDTH - 4 - 2, OS.TOP_BAR_HEIGHT, num4 + 3, height1 - OS.TOP_BAR_HEIGHT - 2);
          os.netMap.Bounds = new Rectangle(1, height1 - height11 - 2, num5 - 1, height11);
          os.display.Bounds = new Rectangle(1, OS.TOP_BAR_HEIGHT, num5 - 1, height12);
          os.ram.Bounds = new Rectangle(width1 - RamModule.MODULE_WIDTH - 2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
          break;
        case OSTheme.Riptide:
          int height13 = (int) ((double) height1 * 0.33);
          double num6 = 0.51;
          int num7 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * (1.0 - num6)) - 2;
          int width9 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * num6);
          int height14 = height1 - height13 - OS.TOP_BAR_HEIGHT - 6 + 2;
          os.terminal.Bounds = new Rectangle(width1 - num7 - RamModule.MODULE_WIDTH - 6, OS.TOP_BAR_HEIGHT, num7 + 2, height1 - OS.TOP_BAR_HEIGHT - 2);
          os.netMap.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, width9, height13);
          os.display.Bounds = new Rectangle(2, os.netMap.bounds.Y + os.netMap.bounds.Height + 2, width9, height14);
          os.ram.Bounds = new Rectangle(width1 - RamModule.MODULE_WIDTH - 2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
          break;
        case OSTheme.Riptide2:
          int height15 = (int) ((double) height1 * 0.33);
          double num8 = 0.55;
          int width10 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * (1.0 - num8)) - 6;
          int num9 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * num8);
          int num10 = height1 - height15 - OS.TOP_BAR_HEIGHT - 6 + 1;
          int x = width1 - width10 - RamModule.MODULE_WIDTH - 4;
          os.display.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, num9 + 4, height1 - OS.TOP_BAR_HEIGHT - 2);
          os.netMap.Bounds = new Rectangle(x, OS.TOP_BAR_HEIGHT, width10, height15);
          os.terminal.Bounds = new Rectangle(x, os.netMap.bounds.Y + os.netMap.bounds.Height, width10, num10 + 3);
          os.ram.Bounds = new Rectangle(width1 - RamModule.MODULE_WIDTH - 2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
          break;
        default:
          int height16 = 205;
          int width11 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * 0.4442);
          int num11 = (int) ((double) (width1 - RamModule.MODULE_WIDTH - 6) * 0.5558) + 1;
          int height17 = height1 - height16 - OS.TOP_BAR_HEIGHT - 6;
          if (theme == OSTheme.HacknetPurple)
          {
            height17 += 2;
            ++num11;
          }
          os.terminal.Bounds = new Rectangle(width1 - width11 - 2, OS.TOP_BAR_HEIGHT, width11, height1 - OS.TOP_BAR_HEIGHT - 2);
          os.netMap.Bounds = new Rectangle(RamModule.MODULE_WIDTH + 4, height1 - height16 - 2, num11 - 2, height16);
          os.display.Bounds = new Rectangle(RamModule.MODULE_WIDTH + 4, OS.TOP_BAR_HEIGHT, num11 - 2, height17);
          os.ram.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
          break;
      }
      if (!ThemeManager.HasNeverSwappedThemeBefore && (os.display.bounds.Width == width2 && os.display.bounds.Height == height2))
        return;
      ThemeManager.webWidth = os.display.bounds.Width;
      ThemeManager.webHeight = os.display.bounds.Height;
      ThemeManager.framesTillWebUpdate = 600;
      ThemeManager.HasNeverSwappedThemeBefore = false;
    }

    internal static void loadCustomThemeBackground(OS os, string imagePathName)
    {
      if (imagePathName == ThemeManager.customBackgroundImageLoadPath)
      {
        ThemeManager.backgroundImage = ThemeManager.lastLoadedCustomBackground;
      }
      else
      {
        if (ThemeManager.backgroundNeedsDisposal)
        {
          Texture2D oldBackground = ThemeManager.backgroundImage;
          os.delayer.Post(ActionDelayer.NextTick(), (Action) (() => oldBackground.Dispose()));
        }
        if (string.IsNullOrWhiteSpace(imagePathName))
        {
          ThemeManager.backgroundImage = (Texture2D) null;
        }
        else
        {
          string path = Utils.GetFileLoadPrefix() + imagePathName;
          if (!File.Exists(path))
            path = "Content/" + imagePathName;
          if (File.Exists(path))
          {
            try
            {
              using (FileStream fileStream = File.OpenRead(path))
              {
                ThemeManager.backgroundImage = Texture2D.FromStream(GuiData.spriteBatch.GraphicsDevice, (Stream) fileStream);
                ThemeManager.backgroundNeedsDisposal = true;
              }
              ThemeManager.lastLoadedCustomBackground = ThemeManager.backgroundImage;
              ThemeManager.customBackgroundImageLoadPath = imagePathName;
            }
            catch (Exception ex)
            {
              ThemeManager.backgroundImage = (Texture2D) null;
            }
          }
          else
            ThemeManager.backgroundImage = (Texture2D) null;
        }
      }
    }

    internal static void loadThemeBackground(OS os, OSTheme theme)
    {
      switch (theme)
      {
        case OSTheme.TerminalOnlyBlack:
          ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/NoThemeWallpaper");
          break;
        case OSTheme.HacknetTeal:
          ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/AbstractWaves");
          break;
        case OSTheme.HacknetYellow:
          ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/DarkenedAway");
          break;
        case OSTheme.HackerGreen:
          ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/GreenAbstract");
          break;
        case OSTheme.HacknetWhite:
          ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/AwayTooLong");
          break;
        case OSTheme.HacknetPurple:
          ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/BlueCirclesBackground");
          break;
        case OSTheme.HacknetMint:
          ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/WaterWashGreen2");
          break;
        default:
          ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/Razzamataz2");
          break;
      }
      ThemeManager.backgroundNeedsDisposal = false;
    }

    internal static void switchThemeColors(OS os, OSTheme theme)
    {
      os.displayModuleExtraLayerBackingColor = Color.Transparent;
      os.UseAspectPreserveBackgroundScaling = false;
      os.BackgroundImageFillColor = Color.Black;
      switch (theme)
      {
        case OSTheme.TerminalOnlyBlack:
          Color color = new Color(0, 0, 0, 0);
          os.defaultHighlightColor = new Color(0, 0, 0, 200);
          os.defaultTopBarColor = new Color(0, 0, 0, 0);
          os.highlightColor = os.defaultHighlightColor;
          os.shellColor = color;
          os.shellButtonColor = color;
          os.moduleColorSolid = new Color(0, 0, 0, 0);
          os.moduleColorStrong = new Color(0, 0, 0, 0);
          os.moduleColorSolidDefault = new Color(0, 0, 0, 0);
          os.moduleColorBacking = color;
          os.topBarColor = os.defaultTopBarColor;
          os.terminalTextColor = new Color((int) byte.MaxValue, 254, 235);
          os.AFX_KeyboardMiddle = new Color(0, 0, 0);
          os.AFX_KeyboardOuter = new Color(0, 0, 0);
          os.AFX_WordLogo = new Color(0, 0, 0);
          os.AFX_Other = new Color(0, 0, 0);
          os.exeModuleTopBar = color;
          os.exeModuleTitleText = color;
          break;
        case OSTheme.HacknetBlue:
          os.defaultHighlightColor = new Color(0, 139, 199, (int) byte.MaxValue);
          os.defaultTopBarColor = new Color(130, 65, 27);
          os.warningColor = Color.Red;
          os.highlightColor = os.defaultHighlightColor;
          os.subtleTextColor = new Color(90, 90, 90);
          os.darkBackgroundColor = new Color(8, 8, 8);
          os.indentBackgroundColor = new Color(12, 12, 12);
          os.outlineColor = new Color(68, 68, 68);
          os.lockedColor = new Color(65, 16, 16, 200);
          os.brightLockedColor = new Color(160, 0, 0);
          os.brightUnlockedColor = new Color(0, 160, 0);
          os.unlockedColor = new Color(39, 65, 36);
          os.lightGray = new Color(180, 180, 180);
          os.shellColor = new Color(222, 201, 24);
          os.shellButtonColor = new Color(105, 167, 188);
          os.moduleColorSolid = new Color(50, 59, 90, (int) byte.MaxValue);
          os.moduleColorSolidDefault = new Color(50, 59, 90, (int) byte.MaxValue);
          os.moduleColorStrong = new Color(14, 28, 40, 80);
          os.moduleColorBacking = new Color(5, 6, 7, 10);
          os.topBarColor = os.defaultTopBarColor;
          os.semiTransText = new Color(120, 120, 120, 0);
          os.terminalTextColor = new Color(213, 245, (int) byte.MaxValue);
          os.topBarTextColor = new Color(126, 126, 126, 100);
          os.superLightWhite = new Color(2, 2, 2, 30);
          os.connectedNodeHighlight = new Color(222, 0, 0, 195);
          os.exeModuleTopBar = new Color(130, 65, 27, 80);
          os.exeModuleTitleText = new Color(155, 85, 37, 0);
          os.netmapToolTipColor = new Color(213, 245, (int) byte.MaxValue, 0);
          os.netmapToolTipBackground = new Color(0, 0, 0, 70);
          os.topBarIconsColor = Color.White;
          os.AFX_KeyboardMiddle = new Color(0, 120, (int) byte.MaxValue);
          os.AFX_KeyboardOuter = new Color((int) byte.MaxValue, 150, 0);
          os.AFX_WordLogo = new Color(0, 120, (int) byte.MaxValue);
          os.AFX_Other = new Color(0, 100, (int) byte.MaxValue);
          os.thisComputerNode = new Color(95, 220, 83);
          os.scanlinesColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, 15);
          break;
        case OSTheme.HacknetTeal:
          os.defaultHighlightColor = new Color(59, 134, 134, (int) byte.MaxValue);
          os.defaultTopBarColor = new Color(11, 72, 107);
          os.warningColor = Color.Red;
          os.highlightColor = os.defaultHighlightColor;
          os.subtleTextColor = new Color(90, 90, 90);
          os.darkBackgroundColor = new Color(8, 8, 8);
          os.indentBackgroundColor = new Color(12, 12, 12);
          os.outlineColor = new Color(68, 68, 68);
          os.lockedColor = new Color(65, 16, 16, 200);
          os.brightLockedColor = new Color(160, 0, 0);
          os.brightUnlockedColor = new Color(0, 160, 0);
          os.unlockedColor = new Color(39, 65, 36);
          os.lightGray = new Color(180, 180, 180);
          os.shellColor = new Color(121, 189, 154);
          os.shellButtonColor = new Color(207, 240, 158);
          os.moduleColorSolid = new Color(59, 134, 134);
          os.moduleColorSolidDefault = new Color(59, 134, 134);
          os.moduleColorStrong = new Color(14, 28, 40, 80);
          os.moduleColorBacking = new Color(5, 7, 6, 200);
          os.topBarColor = os.defaultTopBarColor;
          os.semiTransText = new Color(120, 120, 120, 0);
          os.terminalTextColor = new Color(213, 245, (int) byte.MaxValue);
          os.topBarTextColor = new Color(126, 126, 126, 100);
          os.superLightWhite = new Color(2, 2, 2, 30);
          os.connectedNodeHighlight = new Color(222, 0, 0, 195);
          os.exeModuleTopBar = new Color(12, 33, 33, 80);
          os.exeModuleTitleText = new Color(11, 72, 107, 0);
          os.netmapToolTipColor = new Color(213, 245, (int) byte.MaxValue, 0);
          os.netmapToolTipBackground = new Color(0, 0, 0, 70);
          os.topBarIconsColor = Color.White;
          os.thisComputerNode = new Color(95, 220, 83);
          os.AFX_KeyboardMiddle = new Color(168, 219, 168);
          os.AFX_KeyboardOuter = new Color(121, 189, 154);
          os.AFX_WordLogo = new Color(59, 134, 134);
          os.AFX_Other = new Color(207, 240, 158);
          os.scanlinesColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, 15);
          break;
        case OSTheme.HacknetYellow:
          os.defaultHighlightColor = new Color(186, 98, 9, (int) byte.MaxValue);
          os.defaultTopBarColor = new Color(89, 48, 6);
          os.highlightColor = os.defaultHighlightColor;
          os.shellColor = new Color(60, 107, 85);
          os.shellButtonColor = new Color(207, 240, 158);
          os.moduleColorSolid = new Color(186, 170, 67, 10);
          os.moduleColorStrong = new Color(201, 154, 10, 10);
          os.moduleColorBacking = new Color(5, 7, 6, 200);
          os.topBarColor = os.defaultTopBarColor;
          os.AFX_KeyboardMiddle = new Color((int) byte.MaxValue, 150, 0);
          os.AFX_KeyboardOuter = new Color((int) byte.MaxValue, 204, 0);
          os.AFX_WordLogo = new Color((int) byte.MaxValue, 179, 0);
          os.AFX_Other = new Color((int) byte.MaxValue, 221, 0);
          os.exeModuleTopBar = new Color(12, 33, 33, 80);
          os.exeModuleTitleText = new Color(11, 72, 107, 0);
          break;
        case OSTheme.HackerGreen:
          os.defaultHighlightColor = new Color(135, 222, 109, 200);
          os.defaultTopBarColor = new Color(6, 40, 16);
          os.highlightColor = os.defaultHighlightColor;
          os.shellColor = new Color(135, 222, 109);
          os.shellButtonColor = new Color(207, 240, 158);
          os.moduleColorSolid = new Color(60, 222, 10, 100);
          os.moduleColorSolidDefault = new Color(60, 222, 10, 100);
          os.moduleColorStrong = new Color(10, 80, 20, 50);
          os.moduleColorBacking = new Color(6, 6, 6, 200);
          os.topBarColor = os.defaultTopBarColor;
          os.terminalTextColor = new Color(95, (int) byte.MaxValue, 70);
          os.AFX_KeyboardMiddle = new Color(60, (int) byte.MaxValue, 10);
          os.AFX_KeyboardOuter = new Color(60, 222, 10);
          os.AFX_WordLogo = new Color(0, (int) byte.MaxValue, 20);
          os.AFX_Other = new Color(0, (int) byte.MaxValue, 0);
          os.exeModuleTopBar = new Color(12, 33, 33, 80);
          os.exeModuleTitleText = new Color(11, 107, 20, 0);
          os.topBarIconsColor = Color.White;
          break;
        case OSTheme.HacknetWhite:
          os.defaultHighlightColor = new Color(185, 219, (int) byte.MaxValue, (int) byte.MaxValue);
          os.defaultTopBarColor = new Color(20, 20, 20);
          os.highlightColor = os.defaultHighlightColor;
          os.shellColor = new Color(156, 185, 190);
          os.shellButtonColor = new Color(159, 220, 231);
          os.moduleColorSolid = new Color(220, 222, 220, 100);
          os.moduleColorSolidDefault = new Color(220, 222, 220, 100);
          os.moduleColorStrong = new Color(71, 71, 71, 50);
          os.moduleColorBacking = new Color(6, 6, 6, 205);
          os.topBarColor = os.defaultTopBarColor;
          os.terminalTextColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
          os.AFX_KeyboardMiddle = new Color(220, 220, 220);
          os.AFX_KeyboardOuter = new Color(180, 180, 180);
          os.AFX_WordLogo = new Color(170, 80, 80);
          os.AFX_Other = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
          os.exeModuleTopBar = new Color(20, 20, 20, 80);
          os.exeModuleTitleText = new Color(200, 200, 200, 0);
          os.displayModuleExtraLayerBackingColor = new Color(0, 0, 0, 140);
          break;
        case OSTheme.HacknetPurple:
          os.defaultHighlightColor = new Color(35, 158, 121);
          os.defaultTopBarColor = new Color(0, 0, 0, 60);
          os.highlightColor = os.defaultHighlightColor;
          os.highlightColor = os.defaultHighlightColor;
          os.moduleColorSolid = new Color(154, 119, 189, (int) byte.MaxValue);
          os.moduleColorSolidDefault = new Color(154, 119, 189, (int) byte.MaxValue);
          os.moduleColorStrong = new Color(27, 14, 40, 80);
          os.moduleColorBacking = new Color(6, 5, 7, 205);
          os.topBarColor = os.defaultTopBarColor;
          os.exeModuleTopBar = new Color(32, 22, 40, 80);
          os.exeModuleTitleText = new Color(91, 132, 207, 0);
          os.netmapToolTipColor = new Color(213, 245, (int) byte.MaxValue, 0);
          os.netmapToolTipBackground = new Color(0, 0, 0, 70);
          os.displayModuleExtraLayerBackingColor = new Color(0, 0, 0, 60);
          os.AFX_KeyboardMiddle = new Color(190, 0, (int) byte.MaxValue);
          os.AFX_KeyboardOuter = new Color(20, 90, (int) byte.MaxValue);
          os.AFX_WordLogo = new Color(0, (int) byte.MaxValue, 60);
          os.AFX_Other = new Color(0, 201, 141);
          os.thisComputerNode = new Color(95, 220, 83);
          os.scanlinesColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, 15);
          break;
        case OSTheme.HacknetMint:
          os.defaultHighlightColor = new Color(35, 158, 121);
          os.defaultTopBarColor = new Color(0, 0, 0, 40);
          os.topBarColor = os.defaultTopBarColor;
          os.highlightColor = os.defaultHighlightColor;
          os.moduleColorSolid = new Color(150, 150, 150, (int) byte.MaxValue);
          os.moduleColorSolidDefault = new Color(150, 150, 150, (int) byte.MaxValue);
          os.moduleColorStrong = new Color(43, 43, 43, 80);
          os.moduleColorBacking = new Color(6, 6, 6, 145);
          os.displayModuleExtraLayerBackingColor = new Color(0, 0, 0, 70);
          os.thisComputerNode = new Color(95, 220, 83);
          os.topBarIconsColor = new Color(49, 224, 172);
          os.AFX_KeyboardMiddle = new Color(35, (int) byte.MaxValue, 173);
          os.AFX_KeyboardOuter = new Color(180, (int) byte.MaxValue, (int) byte.MaxValue);
          os.AFX_WordLogo = new Color(0, (int) byte.MaxValue, 60);
          os.AFX_Other = new Color(220, 220, 220);
          os.exeModuleTopBar = new Color(20, 20, 20, 80);
          os.exeModuleTitleText = new Color(49, 224, 172, 0);
          os.scanlinesColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, 15);
          break;
      }
    }

    public static Color GetRepresentativeColorForTheme(OSTheme theme)
    {
      switch (theme)
      {
        case OSTheme.HacknetBlue:
          return new Color(0, 139, 199, (int) byte.MaxValue);
        case OSTheme.HacknetTeal:
          return new Color(59, 134, 134, (int) byte.MaxValue);
        case OSTheme.HacknetYellow:
          return new Color(186, 98, 9, (int) byte.MaxValue);
        case OSTheme.HackerGreen:
          return new Color(135, 222, 109, 200);
        case OSTheme.HacknetWhite:
          return new Color(185, 219, (int) byte.MaxValue, (int) byte.MaxValue);
        case OSTheme.HacknetPurple:
          return new Color(111, 89, 171, (int) byte.MaxValue);
        case OSTheme.HacknetMint:
          return new Color(35, 158, 121);
        default:
          return Utils.VeryDarkGray;
      }
    }

    public static void drawBackgroundImage(SpriteBatch sb, Rectangle area)
    {
      switch (ThemeManager.currentTheme)
      {
        case OSTheme.HacknetYellow:
          sb.Draw(Utils.white, area, new Color(51, 38, 0, (int) byte.MaxValue));
          Rectangle dest1 = new Rectangle(area.X - 30, area.Y - 20, area.Width + 60, area.Height + 40);
          ThemeManager.hexGrid.Draw(dest1, sb, Color.Transparent, new Color((int) byte.MaxValue, 217, 105) * 0.2f, HexGridBackground.ColoringAlgorithm.CorrectedSinWash, 0.0f);
          break;
        case OSTheme.Custom:
          if (ThemeManager.backgroundImage != null)
          {
            if (OS.currentInstance.UseAspectPreserveBackgroundScaling)
            {
              sb.Draw(Utils.white, area, OS.currentInstance.BackgroundImageFillColor);
              Utils.DrawSpriteAspectCorrect(area, sb, ThemeManager.backgroundImage, Color.White, false);
              break;
            }
            sb.Draw(ThemeManager.backgroundImage, area, Color.White);
            break;
          }
          sb.Draw(Utils.white, area, Color.Lerp(Color.Black, ThemeManager.LastLoadedCustomTheme.moduleColorBacking, 0.33f));
          Rectangle dest2 = new Rectangle(area.X - 30, area.Y - 20, area.Width + 60, area.Height + 40);
          ThemeManager.hexGrid.Draw(dest2, sb, Color.Transparent, Color.Lerp(Color.Transparent, ThemeManager.LastLoadedCustomTheme.moduleColorStrong, 0.2f), HexGridBackground.ColoringAlgorithm.CorrectedSinWash, 0.0f);
          break;
        default:
          sb.Draw(ThemeManager.backgroundImage, area, ThemeManager.currentTheme == OSTheme.HackerGreen ? new Color(100, 150, 100) : Color.White);
          break;
      }
    }

    public static string getThemeDataString(OSTheme theme)
    {
      return ThemeManager.fileData[theme];
    }

    public static string getThemeDataStringForCustomTheme(string customThemePath)
    {
      return ThemeManager.getThemeDataString(OSTheme.Custom) + ThemeManager.CustomThemeIDSeperator + FileEncrypter.EncryptString(customThemePath, "", "", "", (string) null);
    }

    public static OSTheme getThemeForDataString(string data)
    {
      if (!data.Contains(ThemeManager.CustomThemeIDSeperator))
        return ThemeManager.fileData.FirstOrDefault<KeyValuePair<OSTheme, string>>((Func<KeyValuePair<OSTheme, string>, bool>) (x => x.Value == data)).Key;
      string[] seperated = data.Split(new string[1]{ ThemeManager.CustomThemeIDSeperator }, StringSplitOptions.RemoveEmptyEntries);
      OSTheme key = ThemeManager.fileData.FirstOrDefault<KeyValuePair<OSTheme, string>>((Func<KeyValuePair<OSTheme, string>, bool>) (x => x.Value == seperated[0])).Key;
      try
      {
        string str1 = FileEncrypter.DecryptString(seperated[1], "")[2];
        string str2 = "Content/";
        if (Settings.IsInExtensionMode)
          str2 = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
        ThemeManager.LastLoadedCustomTheme = CustomTheme.Deserialize(str2 + str1);
      }
      catch (Exception ex)
      {
        ThemeManager.LastLoadedCustomTheme = (CustomTheme) null;
        return OSTheme.TerminalOnlyBlack;
      }
      return key;
    }

    public static void setThemeOnComputer(object computerObject, OSTheme theme)
    {
      Folder folder = ((Computer) computerObject).files.root.searchForFolder("sys");
      if (folder.containsFile("x-server.sys"))
      {
        folder.searchForFile("x-server.sys").data = ThemeManager.getThemeDataString(theme);
      }
      else
      {
        FileEntry fileEntry = new FileEntry(ThemeManager.getThemeDataString(theme), "x-server.sys");
        folder.files.Add(fileEntry);
      }
    }

    public static void setThemeOnComputer(object computerObject, string customThemePath)
    {
      Folder folder = ((Computer) computerObject).files.root.searchForFolder("sys");
      string stringForCustomTheme = ThemeManager.getThemeDataStringForCustomTheme(customThemePath);
      if (folder.containsFile("x-server.sys"))
      {
        folder.searchForFile("x-server.sys").data = stringForCustomTheme;
      }
      else
      {
        FileEntry fileEntry = new FileEntry(stringForCustomTheme, "x-server.sys");
        folder.files.Add(fileEntry);
      }
    }
  }
}
