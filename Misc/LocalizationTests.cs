// Decompiled with JetBrains decompiler
// Type: Hacknet.Misc.LocalizationTests
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Localization;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Hacknet.Misc
{
  public static class LocalizationTests
  {
    public static string TestLocalizations(ScreenManager screenMan, out int errorsAdded)
    {
      string str1 = "\r\n";
      int num = 0;
      string activeLocale = Settings.ActiveLocale;
      for (int index = 0; index < LocaleActivator.SupportedLanguages.Count; ++index)
      {
        LocaleActivator.ActivateLocale(LocaleActivator.SupportedLanguages[index].Code, Game1.getSingleton().Content);
        int errorsAdded1 = 0;
        string str2 = LocalizationTests.LoadAndTestOS(screenMan, out errorsAdded1);
        num += errorsAdded1;
        string str3 = str2.Replace(".", "").Trim();
        str1 = str1 + " -- " + LocaleActivator.SupportedLanguages[index].Name + ": " + (string.IsNullOrWhiteSpace(str3) ? "Complete\r\n" : "\r\n" + str3 + "\r\n\r\n") + str3;
      }
      LocaleActivator.ActivateLocale(activeLocale, Game1.getSingleton().Content);
      string str4 = str1 + "Complete - " + (object) num + " load stopping errors found";
      errorsAdded = num;
      return str4;
    }

    private static string LoadAndTestOS(ScreenManager screenMan, out int errorsAdded)
    {
      int errorCount = 0;
      string str1 = "";
      string username = "__hacknettestaccount";
      string pass = "__testingpassword";
      SaveFileManager.AddUser(username, pass);
      string fileNameForUsername = SaveFileManager.GetSaveFileNameForUsername(username);
      OS.TestingPassOnly = true;
      OS os1 = new OS();
      os1.SaveGameUserName = fileNameForUsername;
      os1.SaveUserAccountName = username;
      screenMan.AddScreen((GameScreen) os1, new PlayerIndex?(screenMan.controllingPlayer));
      os1.delayer.RunAllDelayedActions();
      os1.threadedSaveExecute(false);
      List<Computer> nodes = os1.netMap.nodes;
      screenMan.RemoveScreen((GameScreen) os1);
      OS.WillLoadSave = true;
      OS os2 = new OS();
      os2.SaveGameUserName = fileNameForUsername;
      os2.SaveUserAccountName = username;
      screenMan.AddScreen((GameScreen) os2, new PlayerIndex?(screenMan.controllingPlayer));
      os2.delayer.RunAllDelayedActions();
      Game1.getSingleton().IsMouseVisible = true;
      screenMan.RemoveScreen((GameScreen) os2);
      string str2 = str1 + TestSuite.getTestingReportForLoadComparison((object) os2, nodes, errorCount, out errorCount) + "\r\n" + TestSuite.TestMissions((object) os2);
      errorsAdded = errorCount;
      return str2;
    }
  }
}
