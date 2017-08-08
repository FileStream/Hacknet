// Decompiled with JetBrains decompiler
// Type: Hacknet.Settings
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet
{
  public static class Settings
  {
    public static bool MenuStartup = true;
    public static bool slowOSStartup = true;
    public static bool osStartsWithTutorial = Settings.slowOSStartup;
    public static bool isAlphaDemoMode = false;
    public static bool soundDisabled = false;
    public static bool debugCommandsEnabled = false;
    public static bool testingMenuItemsEnabled = false;
    public static bool debugDrawEnabled = false;
    public static bool forceCompleteEnabled = false;
    public static bool emergencyForceCompleteEnabled = true;
    public static bool emergencyDebugCommandsEnabled = true;
    public static bool AllTraceTimeSlowed = false;
    public static bool FastBootText = false;
    public static bool AllowExtensionMode = true;
    public static bool AllowExtensionPublish = false;
    public static bool EducationSafeBuild = false;
    public static string ActiveLocale = "en-us";
    public static bool EnableDLC = true;
    public static bool isPirateBuild = false;
    public static bool sendsDLC1PromoEmailAtEnd = true;
    public static bool initShowsTutorial = Settings.osStartsWithTutorial;
    public static bool windowed = false;
    public static bool IsInExtensionMode = false;
    public static bool DrawHexBackground = true;
    public static bool StartOnAltMonitor = false;
    public static bool isDemoMode = false;
    public static bool isPressBuildDemo = false;
    public static bool isConventionDemo = false;
    public static bool isLockedDemoMode = false;
    public static bool isSpecialTestBuild = false;
    public static bool lighterColorHexBackground = false;
    public static string ConventionLoginName = "Agent";
    public static bool MultiLingualDemo = false;
    public static bool DLCEnabledDemo = true;
    public static bool ShuffleThemeOnDemoStart = true;
    public static bool HasLabyrinthsDemoStartMainMenuButton = false;
    public static bool ForceEnglish = false;
    public static bool IsExpireLocked = false;
    public static DateTime ExpireTime = Utils.SafeParseDateTime("10/06/2017 23:59:01");
    public static bool isServerMode = false;
    public static bool recoverFromErrorsSilently = true;
  }
}
