// Decompiled with JetBrains decompiler
// Type: Hacknet.Misc.TestSuite
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Extensions;
using Hacknet.Localization;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hacknet.Misc
{
  public class TestSuite
  {
    private static string ActiveObjectID = "";
    private static List<string> TestedMissionNames = new List<string>();

    public static string RunTestSuite(ScreenManager screenMan, bool IsQuickTestMode = false)
    {
      bool soundDisabled = Settings.soundDisabled;
      Settings.soundDisabled = true;
      string activeLocale = Settings.ActiveLocale;
      LocaleActivator.ActivateLocale("en-us", Game1.getSingleton().Content);
      Settings.ActiveLocale = "en-us";
      string str = TestSuite.TestSaveLoadOnFile(screenMan, IsQuickTestMode);
      LocaleActivator.ActivateLocale(activeLocale, Game1.getSingleton().Content);
      Settings.ActiveLocale = activeLocale;
      Settings.soundDisabled = soundDisabled;
      return str;
    }

    public static string TestSaveLoadOnFile(ScreenManager screenMan, bool IsQuicktestMode = false)
    {
      string username = "__hacknettestaccount";
      string pass = "__testingpassword";
      SaveFileManager.AddUser(username, pass);
      string fileNameForUsername = SaveFileManager.GetSaveFileNameForUsername(username);
      OS.TestingPassOnly = true;
      string str1 = "";
      OS os1 = new OS();
      os1.SaveGameUserName = fileNameForUsername;
      os1.SaveUserAccountName = username;
      screenMan.AddScreen((GameScreen) os1, new PlayerIndex?(screenMan.controllingPlayer));
      os1.delayer.RunAllDelayedActions();
      os1.threadedSaveExecute(false);
      List<Computer> nodes1 = os1.netMap.nodes;
      screenMan.RemoveScreen((GameScreen) os1);
      OS.WillLoadSave = true;
      OS os2 = new OS();
      os2.SaveGameUserName = fileNameForUsername;
      os2.SaveUserAccountName = username;
      screenMan.AddScreen((GameScreen) os2, new PlayerIndex?(screenMan.controllingPlayer));
      os2.delayer.RunAllDelayedActions();
      Game1.getSingleton().IsMouseVisible = true;
      string str2 = "Serialization and Integrity Test Report:\r\n";
      Console.WriteLine(str2);
      string str3 = str1 + str2;
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      int errorCount = 0;
      string str4 = str3 + TestSuite.getTestingReportForLoadComparison((object) os2, nodes1, errorCount, out errorCount) + "\r\n" + TestSuite.TestMissions((object) os2);
      int errorsOut = 0;
      string str5 = str4 + TestSuite.TestGameProgression((object) os2, out errorsOut);
      int num = errorCount + errorsOut;
      for (int index = 0; index < os2.netMap.nodes.Count; ++index)
        TestSuite.DeleteAllFilesRecursivley(os2.netMap.nodes[index].files.root);
      os2.SaveGameUserName = fileNameForUsername;
      os2.SaveUserAccountName = username;
      os2.threadedSaveExecute(false);
      List<Computer> nodes2 = os2.netMap.nodes;
      screenMan.RemoveScreen((GameScreen) os2);
      OS.WillLoadSave = true;
      OS os3 = new OS();
      os3.SaveGameUserName = fileNameForUsername;
      os3.SaveUserAccountName = username;
      screenMan.AddScreen((GameScreen) os3, new PlayerIndex?(screenMan.controllingPlayer));
      screenMan.RemoveScreen((GameScreen) os3);
      OS.TestingPassOnly = false;
      SaveFileManager.DeleteUser(username);
      int errorsAdded = 0;
      if (!IsQuicktestMode)
        str5 = str5 + "\r\nLocalization Tests: " + LocalizationTests.TestLocalizations(screenMan, out errorsAdded) + "\r\nDLC Localization Tests: " + DLCLocalizationTests.TestDLCLocalizations(screenMan, out errorsAdded);
      string str6 = str5 + "\r\nDLC Tests: " + DLCTests.TestDLCFunctionality(screenMan, out errorsAdded) + "\r\nDLC Extended Tests: " + DLCExtendedTests.TesExtendedFunctionality(screenMan, out errorsAdded) + "\r\nEDU Edition Tests: " + EduEditionTests.TestEDUFunctionality(screenMan, out errorsAdded) + "\r\nMisc Tests: " + TestSuite.TestMiscAndCLRFeatures(screenMan, out errorsAdded);
      string str7 = "\r\nCore Tests: Complete - " + (object) num + " errors found.\r\nTested " + (object) nodes2.Count + " generated nodes vs " + (object) os3.netMap.nodes.Count + " loaded nodes";
      string str8 = str6 + str7;
      Console.WriteLine(str7);
      MusicManager.stop();
      try
      {
        string str9 = "testreport.txt";
        File.Delete(str9);
        Utils.writeToFile(str8, str9);
      }
      catch (Exception ex)
      {
      }
      return Utils.CleanStringToRenderable(str8);
    }

    public static string TestMiscAndCLRFeatures(ScreenManager screenMan, out int errorsAdded)
    {
      string str1 = "";
      int num = 0;
      if ((int) (ushort) "".GetHashCode() != 5886)
      {
        ++num;
        str1 += "\nBlank string Hashes to an unexpected value! This will break Header file decryption!\n";
      }
      if ((int) (ushort) "12345asdf".GetHashCode() != 25213)
      {
        ++num;
        str1 += "\nTest string Hashes to an unexpected value! This can break file decryption!\n";
      }
      string str2 = str1 + "Complete - " + (object) num + " errors found";
      errorsAdded = num;
      return str2;
    }

    private static void DeleteAllFilesRecursivley(Folder f)
    {
      f.files.Clear();
      for (int index = 0; index < f.folders.Count; ++index)
        TestSuite.DeleteAllFilesRecursivley(f.folders[index]);
    }

    internal static string getTestingReportForLoadComparison(object osobj, List<Computer> oldComps, int currentErrorCount, out int errorCount)
    {
      OS os = (OS) osobj;
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      string str1 = "";
      errorCount = currentErrorCount;
      bool flag = false;
      for (int index1 = 0; index1 < os.netMap.nodes.Count; ++index1)
      {
        for (int index2 = 0; index2 < oldComps.Count; ++index2)
        {
          if (index1 != index2 && os.netMap.nodes[index1].idName == oldComps[index2].idName)
          {
            ++errorCount;
            str1 = str1 + "\nDuplicate Node ID found! " + (object) index1 + ": " + os.netMap.nodes[index1].name + " and " + (object) index2 + ": " + oldComps[index2].name;
            flag = true;
          }
        }
      }
      if (flag)
        return str1 + "\nCritical Error encountered - exiting tests early";
      for (int index1 = 0; index1 < oldComps.Count; ++index1)
      {
        Computer oldComp = oldComps[index1];
        Computer computer = Programs.getComputer(os, oldComp.ip);
        TestSuite.Assert(oldComp.name, computer.name);
        string str2 = computer.files.TestEquals((object) oldComp.files);
        if (stringList1.Contains(computer.idName))
        {
          int index2 = stringList1.IndexOf(computer.idName);
          str2 = str2 + "Duplicate ID Found - \"" + computer.idName + "\" from " + stringList2[index2] + "@" + (object) index2 + " current: " + computer.name + "@" + (object) index1;
        }
        stringList1.Add(computer.idName);
        stringList2.Add(computer.name);
        if (!Utils.FloatEquals(oldComp.startingOverloadTicks, computer.startingOverloadTicks))
          str2 = str2 + "Proxy timer difference - \"" + computer.name + "\" from Base:" + (object) oldComp.startingOverloadTicks + " to Current: " + (object) computer.startingOverloadTicks;
        if ((oldComp.firewall != null || computer.firewall != null) && !oldComp.firewall.Equals((object) computer.firewall))
          str2 = str2 + "Firewall difference - \"" + computer.name + "\" from Base:" + oldComp.firewall.ToString() + "\r\n to Current: " + computer.firewall.ToString() + "\r\n";
        if (oldComp.icon != computer.icon)
          str2 = str2 + "Icon difference - \"" + computer.name + "\" from Base:" + oldComp.icon + " to Current: " + computer.icon;
        if (oldComp.portsNeededForCrack != computer.portsNeededForCrack)
          str2 = str2 + "Port for crack difference - \"" + computer.name + "\" from Base:" + (object) oldComp.portsNeededForCrack + " to Current: " + (object) computer.portsNeededForCrack;
        for (int index2 = 0; index2 < oldComp.links.Count; ++index2)
        {
          if (oldComp.links[index2] != computer.links[index2])
            str2 = str2 + "Link difference - \"" + computer.name + "\" @" + (object) index2 + " " + (object) oldComp.links[index2] + " vs " + (object) computer.links[index2];
        }
        if (!Utils.FloatEquals(oldComp.location.X, computer.location.X) || !Utils.FloatEquals(oldComp.location.Y, computer.location.Y))
          str2 = str2 + "Location difference - \"" + computer.name + "\" from Base:" + (object) oldComp.location + " to Current: " + (object) computer.location;
        if (!Utils.FloatEquals(oldComp.traceTime, computer.traceTime))
          str2 = str2 + "Trace timer difference - \"" + computer.name + "\" from Base:" + (object) oldComp.traceTime + " to Current: " + (object) computer.traceTime;
        if (oldComp.adminPass != computer.adminPass)
          str2 = str2 + "Password Difference: expected \"" + oldComp.adminPass + "\" but got \"" + computer.adminPass + "\"\r\n";
        if (oldComp.adminIP != computer.adminIP)
          str2 = str2 + "Admin IP Difference: expected \"" + oldComp.adminIP + "\" but got \"" + computer.adminIP + "\"\r\n";
        for (int index2 = 0; index2 < oldComp.users.Count; ++index2)
        {
          if (!oldComp.users[index2].Equals((object) computer.users[index2]))
            str2 = str2 + "User difference - \"" + computer.name + "\" @" + (object) index2 + " " + (object) oldComp.users[index2] + " vs " + (object) computer.users[index2];
        }
        for (int index2 = 0; index2 < oldComp.daemons.Count; ++index2)
        {
          if (oldComp.daemons[index2].getSaveString() != computer.daemons[index2].getSaveString())
            str2 = str2 + "Daemon Difference: expected \r\n-----\r\n" + oldComp.daemons[index2].getSaveString() + "\r\n----- but got -----\r\n" + computer.daemons[index2].getSaveString() + "\r\n-----\r\n";
        }
        if (oldComp.hasProxy != computer.hasProxy)
          str2 = str2 + "Proxy Difference: OldProxy:" + (object) oldComp.hasProxy + " vs NewProxy:" + (object) computer.hasProxy;
        if (oldComp.proxyActive != computer.proxyActive)
          str2 = str2 + "Proxy Active Difference: OldProxy:" + (object) oldComp.proxyActive + " vs NewProxy:" + (object) computer.proxyActive;
        if (oldComp.portsNeededForCrack != computer.portsNeededForCrack)
          str2 = str2 + "Ports for crack Difference: Old:" + (object) oldComp.portsNeededForCrack + " vs New:" + (object) computer.portsNeededForCrack;
        if (oldComp.admin != null && computer.admin != null && oldComp.admin.GetType() != computer.admin.GetType())
          str2 = str2 + "SecAdmin Difference: Old:" + (object) oldComp.admin + " vs New:" + (object) computer.admin;
        if (oldComp.admin != null && oldComp.admin.IsSuper != computer.admin.IsSuper)
          str2 = str2 + "SecAdmin Super Difference: Old:" + (object) oldComp.admin.IsSuper + " vs New:" + (object) computer.admin.IsSuper;
        if (oldComp.admin != null && oldComp.admin.ResetsPassword != computer.admin.ResetsPassword)
          str2 = str2 + "SecAdmin PassReset Difference: Old:" + (object) oldComp.admin.ResetsPassword + " vs New:" + (object) computer.admin.ResetsPassword;
        if (str2 != null)
        {
          string str3 = "\r\nErrors in " + oldComp.idName + " - \"" + oldComp.name + "\"\r\n" + str2 + "\r\n";
          str1 = str1 + str3 + "\r\n";
          Console.WriteLine(str3);
          ++errorCount;
        }
        else
        {
          Console.Write(".");
          str1 += ".";
        }
      }
      return str1;
    }

    public static string TestMissions(object os_obj)
    {
      OS os = (OS) os_obj;
      string str1 = "";
      string retAdditions = "";
      MissionFunctions.ReportErrorInCommand += (Action<string>) (report =>
      {
        // ISSUE: variable of a compiler-generated type
        TestSuite.\u003C\u003Ec__DisplayClass1 cDisplayClass1 = this;
        // ISSUE: reference to a compiler-generated field
        string str2 = cDisplayClass1.retAdditions + TestSuite.ActiveObjectID + " : " + report + "\r\n";
        // ISSUE: reference to a compiler-generated field
        cDisplayClass1.retAdditions = str2;
      });
      TestSuite.TestedMissionNames.Clear();
      try
      {
        string str2 = TestSuite.TestMission("Content/Missions/BitMissionIntro.xml", (object) os);
        str1 += str2;
        string str3 = TestSuite.TestMission("Content/Missions/BitMission0.xml", (object) os);
        str1 += str3;
        string path1 = "Content/Missions/Entropy/StartingSet/";
        foreach (FileSystemInfo file in new DirectoryInfo(path1).GetFiles("*.xml"))
        {
          string str4 = TestSuite.TestMission(path1 + file.Name, (object) os);
          str1 += str4;
        }
        string str5 = TestSuite.TestMission("Content/Missions/Entropy/ThemeHackTransitionMission.xml", (object) os);
        str1 += str5;
        string str6 = TestSuite.TestMission("Content/Missions/MainHub/Intro/Intro01.xml", (object) os);
        str1 += str6;
        string path2 = "Content/Missions/MainHub/FirstSet/";
        foreach (FileSystemInfo file in new DirectoryInfo(path2).GetFiles("*.xml"))
        {
          string str4 = TestSuite.TestMission(path2 + file.Name, (object) os);
          str1 += str4;
        }
        string str7 = TestSuite.TestMission("Content/Missions/lelzSec/IntroTestMission.xml", (object) os);
        str1 += str7;
        string str8 = TestSuite.TestMission("Content/Missions/MainHub/BitSet/Missions/BitHubSet01.xml", (object) os);
        str1 += str8;
        string str9 = TestSuite.TestMission("Content/Missions/BitPath/BitAdv_Intro.xml", (object) os);
        str1 += str9;
        string str10 = TestSuite.TestMissionEndFucntion("Content/Missions/MainHub/FirstSet/01HubSet02.xml", "changeSong", (object) os);
        str1 += str10;
        string str11 = TestSuite.TestMissionEndFucntion("Content/Missions/MainHub/FirstSet/02HubSet05.xml", "addFlags:decypher", (object) os);
        str1 += str11;
        string str12 = TestSuite.TestMissionEndFucntion("Content/Missions/MainHub/DecypherSet/DECHeadMission02.xml", "addFlags:dechead", (object) os);
        str1 += str12;
        MissionFunctions.ReportErrorInCommand = (Action<string>) null;
      }
      finally
      {
        MissionFunctions.ReportErrorInCommand = (Action<string>) null;
      }
      return str1 + retAdditions;
    }

    public static string TestMission(string missionName, object os_obj)
    {
      string str1 = "";
      OS os = (OS) os_obj;
      if (TestSuite.TestedMissionNames.Contains(missionName))
        return str1;
      try
      {
        if (!File.Exists(missionName))
          str1 = str1 + "Invalid Mission Path! : " + missionName + "\r\n";
        ActiveMission activeMission = (ActiveMission) ComputerLoader.readMission(missionName);
        TestSuite.ActiveObjectID = missionName;
        string str2 = "";
        for (int index = 0; index < activeMission.goals.Count; ++index)
        {
          string str3 = activeMission.goals[index].TestCompletable();
          if (str3 != null && str3.Length > 0)
            str2 = str2 + missionName + " Goal[" + (object) index + "] " + activeMission.goals[index].ToString() + " :: " + str3 + "\r\n";
        }
        try
        {
          if (!string.IsNullOrWhiteSpace(activeMission.startFunctionName))
          {
            if (!Utils.CheckStringIsRenderable(activeMission.startFunctionName))
              str1 = str1 + "Mission " + missionName + " has unrenderable start function " + Utils.CleanStringToRenderable(activeMission.startFunctionName);
            MissionFunctions.runCommand(activeMission.startFunctionValue, activeMission.startFunctionName);
          }
          if (!string.IsNullOrWhiteSpace(activeMission.endFunctionName))
          {
            if (!Utils.CheckStringIsRenderable(activeMission.endFunctionName))
              str1 = str1 + "Mission " + missionName + " has unrenderable end function " + Utils.CleanStringToRenderable(activeMission.endFunctionName);
            MissionFunctions.runCommand(activeMission.endFunctionValue, activeMission.endFunctionName);
          }
          string str3 = Directory.GetCurrentDirectory() + "/";
          string fileLoadPrefix = Utils.GetFileLoadPrefix();
          if (fileLoadPrefix == "Content/")
            fileLoadPrefix += "Missions/";
          else if (!fileLoadPrefix.StartsWith("Extensions"))
            str3 = "";
          string path = str3 + LocalizedFileLoader.GetLocalizedFilepath(fileLoadPrefix + activeMission.nextMission);
          if (!(activeMission.nextMission == "NONE") && !File.Exists(path))
            str1 = str1 + "\r\nNextMission Tag for mission \"" + missionName + "\" has nonexistent next mission path: " + activeMission.nextMission + "\r\n";
        }
        catch (Exception ex)
        {
          str1 = str1 + "Error running start or end mission function of mission: " + missionName + "\r\nStart Func: " + activeMission.startFunctionName + "\r\nEnd Func: " + activeMission.endFunctionName;
          str1 = str1 + "\r\n" + Utils.GenerateReportFromException(ex) + "\r\n";
        }
        if (str2.Length > 0)
          str1 = str1 + str2 + "--------------\r\n";
        TestSuite.TestedMissionNames.Add(missionName);
        string str4 = "Content/Missions/";
        if (Settings.IsInExtensionMode)
          str4 = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
        List<ActiveMission> activeMissionList = new List<ActiveMission>();
        for (int index = 0; index < os.branchMissions.Count; ++index)
          activeMissionList.Add(os.branchMissions[index]);
        if (activeMission.nextMission != null && activeMission.nextMission.ToLower() != "none")
          str1 += TestSuite.TestMission(str4 + activeMission.nextMission, (object) os);
        for (int index = 0; index < activeMissionList.Count; ++index)
        {
          string localizedFilepath = LocalizedFileLoader.GetLocalizedFilepath(activeMissionList[index].reloadGoalsSourceFile);
          if (!TestSuite.TestedMissionNames.Contains(localizedFilepath))
          {
            Console.WriteLine("testing Branch Mission " + localizedFilepath);
            str1 += TestSuite.TestMission(localizedFilepath, (object) os);
          }
        }
      }
      catch (Exception ex)
      {
        str1 = str1 + "Error Loading " + missionName + "\r\n" + ex.ToString();
      }
      return str1;
    }

    public static string TestMissionEndFucntion(string missionName, string expectedEndFunction, object os_obj)
    {
      string str = "";
      if (!File.Exists(missionName))
        str = str + "Invalid Mission Path! : " + missionName + "\r\n";
      ActiveMission activeMission = (ActiveMission) ComputerLoader.readMission(missionName);
      if (activeMission.endFunctionName != expectedEndFunction)
        str = str + "\r\nUnexpected end function in " + missionName + "\r\nExpected: " + expectedEndFunction + " -- found: " + activeMission.endFunctionName;
      return str;
    }

    public static string TestGameProgression(object os_obj, out int errorsOut)
    {
      string str = "";
      int num = 0;
      OS os = (OS) os_obj;
      os.delayer.RunAllDelayedActions();
      ComputerLoader.loadMission("Content/Missions/BitMissionIntro.xml", false);
      Folder folder = os.thisComputer.files.root.searchForFolder("bin");
      folder.files.Clear();
      if (!os.currentMission.isComplete((List<string>) null))
      {
        ++num;
        str += "\r\nCouldn't finish first mission...\r\n";
      }
      os.currentMission.finish();
      FileEntry file = Programs.getComputer(os, "portcrack01").files.root.searchForFolder("bin").files[0];
      folder.files.Add(new FileEntry(file.data, "ssh.exe"));
      if (!os.currentMission.isComplete((List<string>) null))
      {
        ++num;
        str += "\r\nCouldn't finish sshcrack mission...\r\n";
      }
      os.currentMission.finish();
      if (os.currentMission.isComplete((List<string>) null))
      {
        ++num;
        str += "\r\nMission 3 is completable early!\r\n";
      }
      Programs.getComputer(os, "bitMission00").giveAdmin(os.thisComputer.ip);
      if (!os.currentMission.isComplete((List<string>) null))
      {
        ++num;
        str += "\r\nCouldn't finish BitMission0\r\n";
      }
      os.currentMission.finish();
      errorsOut = num;
      return str;
    }

    private static void Assert(string first, string second)
    {
      if (first != second)
        throw new InvalidProgramException();
    }

    public static void DoWordCount()
    {
      WordCounter.PerformWordCount(new string[4]
      {
        "Content/DLC/Missions",
        "Content/DLC/Docs",
        "Content/DLC/Misc",
        "Content/DLC/ActionScripts"
      }, new string[1]{ "Content/DLC" });
    }
  }
}
