// Decompiled with JetBrains decompiler
// Type: Hacknet.Misc.DLCLocalizationTests
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Localization;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Hacknet.Misc
{
  public static class DLCLocalizationTests
  {
    public static string TestDLCLocalizations(ScreenManager screenMan, out int errorsAdded)
    {
      string str1 = "\r\n[";
      int num = 0;
      string activeLocale = Settings.ActiveLocale;
      for (int index = 0; index < LocaleActivator.SupportedLanguages.Count; ++index)
      {
        LocaleActivator.ActivateLocale(LocaleActivator.SupportedLanguages[index].Code, Game1.getSingleton().Content);
        int errorsAdded1 = 0;
        string str2 = DLCLocalizationTests.LoadAndTestOS(screenMan, out errorsAdded1);
        num += errorsAdded1;
        string str3 = str2.Replace(".", "").Trim();
        if (string.IsNullOrWhiteSpace(str3))
          str1 += "==";
        else
          str1 = str1 + "\r\n -- " + LocaleActivator.SupportedLanguages[index].Name + ": " + (string.IsNullOrWhiteSpace(str3) ? "Complete\r\n\r\n" : "\r\n" + str3 + "\r\n\r\n");
      }
      LocaleActivator.ActivateLocale(activeLocale, Game1.getSingleton().Content);
      string str4 = str1 + "]\r\nComplete - " + (object) num + " load stopping errors found";
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
      SessionAccelerator.AccelerateSessionToDLCEND((object) os1);
      os1.PreDLCVisibleNodesCache = "123,456,789";
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
      if (os2.PreDLCVisibleNodesCache != "123,456,789")
      {
        ++errorCount;
        str1 += "PreDLC Visible Node Cache not saving correctly";
      }
      screenMan.RemoveScreen((GameScreen) os2);
      string str2 = str1 + TestSuite.getTestingReportForLoadComparison((object) os2, nodes, errorCount, out errorCount) + "\r\n" + TestSuite.TestMissions((object) os2);
      string str3 = DLCTests.TestDLCFunctionality(screenMan, out errorsAdded);
      string str4 = str2 + (str3.Length > 30 ? "\r\n" + str3 : "");
      errorsAdded = errorCount;
      return str4;
    }
  }
}
