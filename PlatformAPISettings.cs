// Decompiled with JetBrains decompiler
// Type: Hacknet.PlatformAPISettings
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Localization;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Hacknet
{
  public static class PlatformAPISettings
  {
    public static string Report = "";
    public static bool Running = false;
    public static bool RemoteStorageRunning = false;

    public static void InitPlatformAPI()
    {
      if (Settings.isConventionDemo)
        return;
      PlatformAPISettings.Running = SteamAPI.Init();
      if (!PlatformAPISettings.Running)
      {
        PlatformAPISettings.Report = "First Init Failed. ";
        Console.WriteLine("Steam Init Failed!");
        PlatformAPISettings.Running = SteamAPI.InitSafe();
        PlatformAPISettings.Report = PlatformAPISettings.Report + " Second init Running = " + (object) PlatformAPISettings.Running;
        Console.WriteLine(PlatformAPISettings.Report);
      }
      else
        PlatformAPISettings.Report = "Steam API Running :" + (object) PlatformAPISettings.Running;
      if (PlatformAPISettings.Running)
        PlatformAPISettings.RemoteStorageRunning = SteamRemoteStorage.IsCloudEnabledForAccount();
    }

    public static string GetCodeForActiveLanguage(List<LocaleActivator.LanguageInfo> supportedLanguages)
    {
      Console.WriteLine("Scanning for language code...");
      if (PlatformAPISettings.Running)
      {
        string currentGameLanguage = SteamApps.GetCurrentGameLanguage();
        for (int index = 0; index < supportedLanguages.Count; ++index)
        {
          if (supportedLanguages[index].SteamCode == currentGameLanguage)
          {
            Console.WriteLine("Matched Steam Language Code : " + currentGameLanguage);
            return supportedLanguages[index].Code;
          }
        }
      }
      CultureInfo originalCultureInfo = Game1.OriginalCultureInfo;
      for (int index = 0; index < supportedLanguages.Count; ++index)
      {
        if (supportedLanguages[index].Code == originalCultureInfo.Name)
        {
          Console.WriteLine("Found exact language match for " + originalCultureInfo.Name);
          return supportedLanguages[index].Code;
        }
      }
      for (int index = 0; index < supportedLanguages.Count; ++index)
      {
        string str = supportedLanguages[index].Code.Substring(0, 2);
        if (originalCultureInfo.Name.StartsWith(str))
        {
          Console.WriteLine("Found close enough language match for " + str);
          return supportedLanguages[index].Code;
        }
      }
      Console.WriteLine("No language match found for " + originalCultureInfo.Name + " - " + originalCultureInfo.DisplayName + ". Reverting to English...");
      return "en-us";
    }
  }
}
