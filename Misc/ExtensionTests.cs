// Decompiled with JetBrains decompiler
// Type: Hacknet.Misc.ExtensionTests
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Extensions;
using Hacknet.Factions;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hacknet.Misc
{
  public static class ExtensionTests
  {
    private static bool OS_wasTestingPass = false;
    internal static string RuntimeLoadAdditionalErrors = "";

    public static string TestExtensions(ScreenManager screenMan, out int errorsAdded)
    {
      string str1 = "";
      int errorsAdded1 = 0;
      string str2 = str1 + ExtensionTests.TestBlankExtension(screenMan, out errorsAdded1) + "\r\nComplete - " + (object) errorsAdded1 + " errors found";
      errorsAdded = errorsAdded1;
      return str2;
    }

    public static string TestExtensionCustomStartSong(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str1 = "";
      ExtensionLoader.ActiveExtensionInfo = ExtensionInfo.ReadExtensionInfo("Content/Tests/TestExtension");
      ExtensionInfo activeExtensionInfo = ExtensionLoader.ActiveExtensionInfo;
      if (activeExtensionInfo.IntroStartupSong != null)
      {
        string str2 = "Content/Music/" + activeExtensionInfo.IntroStartupSong;
        if (File.Exists(str2))
          MusicManager.loadAsCurrentSong(str2);
      }
      errorsAdded = num;
      return str1;
    }

    public static string TestExtensionForRuntime(ScreenManager screenMan, string path, out int errorsAdded)
    {
      int num1 = 0;
      string str1 = "";
      OS os = (OS) null;
      ExtensionTests.RuntimeLoadAdditionalErrors = "";
      int num2;
      try
      {
        os = (OS) ExtensionTests.SetupOSForTests(path, screenMan);
        string str2 = TestSuite.TestMission(ExtensionLoader.ActiveExtensionInfo.FolderPath + "/" + ExtensionLoader.ActiveExtensionInfo.StartingMissionPath, (object) os);
        if (!string.IsNullOrWhiteSpace(str2))
        {
          ++num1;
          str1 = str1 + "\n\nSTART MISSION READ ERRORS:\r\n" + str2;
        }
        int errorsAdded1 = 0;
        string str3 = ExtensionTests.TestAllExtensionMissions(os, out errorsAdded1);
        str1 += str3;
        num1 += errorsAdded1;
        int errorsAdded2 = 0;
        string str4 = ExtensionTests.TestAllExtensionNodesRuntime(os, out errorsAdded2);
        str1 += str4;
        num2 = num1 + errorsAdded2;
      }
      catch (Exception ex)
      {
        str1 = str1 + "\nLoad Error:\n" + Utils.GenerateReportFromException(ex).Trim();
        num2 = num1 + 1;
      }
      if (os != null)
        screenMan.RemoveScreen((GameScreen) os);
      ExtensionTests.CompleteExtensiontesting();
      if (!string.IsNullOrWhiteSpace(ExtensionTests.RuntimeLoadAdditionalErrors))
      {
        ++num2;
        str1 = str1 + "\r\n" + ExtensionTests.RuntimeLoadAdditionalErrors;
      }
      errorsAdded = num2;
      return str1;
    }

    private static string TestAllExtensionMissions(OS os, out int errorsAdded)
    {
      int errors = 0;
      string ret = "";
      Utils.ActOnAllFilesRevursivley(ExtensionLoader.ActiveExtensionInfo.FolderPath + "/Missions", (Action<string>) (filename =>
      {
        if (!filename.EndsWith(".xml"))
          return;
        if (OS.TestingPassOnly)
        {
          try
          {
            ActiveMission activeMission = (ActiveMission) ComputerLoader.readMission(filename);
            if (activeMission != null)
              ExtensionTests.RuntimeLoadAdditionalErrors += ExtensionTests.TestExtensionMission((object) activeMission, filename, (object) os);
          }
          catch (Exception ex)
          {
            ++errors;
            // ISSUE: variable of a compiler-generated type
            ExtensionTests.\u003C\u003Ec__DisplayClass1 cDisplayClass1_1 = this;
            // ISSUE: reference to a compiler-generated field
            string str1 = cDisplayClass1_1.ret + "Error Loading Mission: " + filename;
            // ISSUE: reference to a compiler-generated field
            cDisplayClass1_1.ret = str1;
            // ISSUE: variable of a compiler-generated type
            ExtensionTests.\u003C\u003Ec__DisplayClass1 cDisplayClass1_2 = this;
            // ISSUE: reference to a compiler-generated field
            string str2 = cDisplayClass1_2.ret + "\r\n\r\n" + Utils.GenerateReportFromExceptionCompact(ex) + "\r\n\r\n";
            // ISSUE: reference to a compiler-generated field
            cDisplayClass1_2.ret = str2;
          }
        }
      }));
      errorsAdded = errors;
      return ret;
    }

    private static string TestAllExtensionNodesRuntime(OS os, out int errorsAdded)
    {
      int errors = 0;
      string ret = "";
      Utils.ActOnAllFilesRevursivley(ExtensionLoader.ActiveExtensionInfo.FolderPath + "/Nodes", (Action<string>) (filename =>
      {
        if (!filename.EndsWith(".xml"))
          return;
        try
        {
          Computer c = Computer.loadFromFile(filename);
          if (c != null)
            ExtensionLoader.CheckAndAssignCoreServer(c, os);
        }
        catch (Exception ex)
        {
          string str1 = string.Format("COMPUTER LOAD ERROR:\nError loading computer \"{0}\"\nError: {1} - {2}", (object) filename, (object) ex.GetType().Name, (object) ex.Message);
          // ISSUE: variable of a compiler-generated type
          ExtensionTests.\u003C\u003Ec__DisplayClass4 cDisplayClass4 = this;
          // ISSUE: reference to a compiler-generated field
          string str2 = cDisplayClass4.ret + str1 + "\r\n";
          // ISSUE: reference to a compiler-generated field
          cDisplayClass4.ret = str2;
          ++errors;
        }
      }));
      errorsAdded = errors;
      return ret;
    }

    private static string TestTagisClosed(string tagname, string allData)
    {
      string str = "";
      bool flag = allData.Contains("</" + tagname + ">") || allData.Contains("</" + tagname + " >");
      if (allData.Contains("<" + tagname + ">") && !flag)
        str = str + "Tag " + tagname + " is not properly closed! Check your Slashes." + "\r\n\r\n";
      return str;
    }

    public static string TestExtensionMission(object mission, string filepath, object os)
    {
      string str1 = "";
      ActiveMission activeMission = (ActiveMission) mission;
      string str2 = File.ReadAllText(filepath);
      string str3 = str1 + ExtensionTests.TestTagisClosed("missionEnd", str2) + ExtensionTests.TestTagisClosed("missionStart", str2) + ExtensionTests.TestTagisClosed("nextMission", str2) + ExtensionTests.TestTagisClosed("goals", str2) + ExtensionTests.TestTagisClosed("email", str2) + ExtensionTests.TestTagisClosed("sender", str2) + ExtensionTests.TestTagisClosed("subject", str2) + ExtensionTests.TestTagisClosed("body", str2) + ExtensionTests.TestTagisClosed("attachments", str2);
      if (!str2.Contains("</attachments>"))
        str3 = str3 + "File does not contain attachments tag at the end of the email! It needs to be there!" + "\r\n";
      int count = Regex.Matches(str2, "<goal ").Count;
      if (activeMission.goals.Count != count && activeMission.goals.Count < count)
      {
        string str4 = str3 + "File defines some goals that are not being correctly parsed in! (" + (object) activeMission.goals.Count + " loaded vs " + (object) count + " in file)" + "\r\nCheck your syntax and tags! Valid Goals:\r\n";
        for (int index = 0; index < activeMission.goals.Count; ++index)
          str4 = str4 + "\r\n" + activeMission.goals[index].ToString().Replace("Hacknet.Mission.", "");
        str3 = str4 + "\r\n";
      }
      if (string.IsNullOrWhiteSpace(activeMission.startFunctionName) && str2.Contains("<missionStart"))
        str3 = str3 + "File contains missionStart, but it's not being correctly parsed in. It might be out of order in the file." + "\r\n";
      if (activeMission.startFunctionName != null)
      {
        try
        {
          if (!activeMission.startFunctionName.Contains("addRank"))
            MissionFunctions.runCommand(activeMission.startFunctionValue, activeMission.startFunctionName);
        }
        catch (Exception ex)
        {
          str3 = str3 + "Error running start function " + activeMission.startFunctionName + "\r\n" + Utils.GenerateReportFromException(ex);
        }
      }
      if (string.IsNullOrWhiteSpace(activeMission.endFunctionName) && str2.Contains("<missionEnd"))
        str3 = str3 + "File contains missionEnd, but it's not being correctly parsed in. It might be out of order in the file." + "\r\n";
      if (activeMission.endFunctionName != null)
      {
        try
        {
          if (!activeMission.endFunctionName.Contains("addRank"))
            MissionFunctions.runCommand(activeMission.endFunctionValue, activeMission.endFunctionName);
        }
        catch (Exception ex)
        {
          str3 = str3 + "Error running end function " + activeMission.endFunctionName + "\r\n" + Utils.GenerateReportFromException(ex);
        }
      }
      string str5 = TestSuite.TestMission(ExtensionLoader.ActiveExtensionInfo.FolderPath + "/" + ExtensionLoader.ActiveExtensionInfo.StartingMissionPath, os);
      if (!string.IsNullOrWhiteSpace(str5))
        str3 += str5;
      if (str3.Length > 1)
        return "Mission Errors for " + filepath.Replace("\\", "/") + ":\r\n" + str3;
      return "";
    }

    public static string TestExtensionCustomThemeSerialization(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str1 = "";
      CustomTheme customTheme = new CustomTheme();
      Color color = new Color(22, 22, 22, 19);
      customTheme.defaultTopBarColor = color;
      string str2 = "customTheme1.xml";
      Utils.writeToFile(customTheme.GetSaveString(), str2);
      if (CustomTheme.Deserialize(str2).defaultTopBarColor != color)
      {
        ++num;
        str1 += "\nSave/Load broken for themes!";
      }
      errorsAdded = num;
      return str1;
    }

    public static string TestExtensionCustomThemeFile(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      OS os1 = (OS) ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
      if (ThemeManager.getThemeForDataString(Programs.getComputer(os1, "advExamplePC").files.root.searchForFolder("sys").searchForFile("Custom_x-server.sys").data) != OSTheme.Custom || ThemeManager.LastLoadedCustomTheme == null)
      {
        ++num;
        str += "Custom theme did not read in from file correctly!";
      }
      os1.threadedSaveExecute(false);
      screenMan.RemoveScreen((GameScreen) os1);
      OS.WillLoadSave = true;
      OS os2 = (OS) ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
      if (ThemeManager.getThemeForDataString(Programs.getComputer(os2, "advExamplePC").files.root.searchForFolder("sys").searchForFile("Custom_x-server.sys").data) != OSTheme.Custom || ThemeManager.LastLoadedCustomTheme == null)
      {
        ++num;
        str += "Custom theme did not read in from file correctly after save/load!";
      }
      screenMan.RemoveScreen((GameScreen) os2);
      ExtensionTests.CompleteExtensiontesting();
      errorsAdded = num;
      return str;
    }

    public static string TestExtensionCustomFactions(ScreenManager screenMan, out int errorsAdded)
    {
      int errorsAdded1 = 0;
      string str1 = "";
      OS os1 = (OS) ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
      string str2 = str1 + ExtensionTests.TestExtensionsFactions(os1.allFactions, out errorsAdded1);
      os1.threadedSaveExecute(false);
      screenMan.RemoveScreen((GameScreen) os1);
      OS.WillLoadSave = true;
      OS os2 = (OS) ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
      string str3 = str2 + ExtensionTests.TestExtensionsFactions(os2.allFactions, out errorsAdded1);
      screenMan.RemoveScreen((GameScreen) os2);
      ExtensionTests.CompleteExtensiontesting();
      errorsAdded = errorsAdded1;
      return str3;
    }

    public static string TestExtensionCustomFactionsActions(ScreenManager screenMan, out int errorsAdded)
    {
      int errorsAdded1 = 0;
      string str1 = "";
      OS os1 = (OS) ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
      string str2 = str1 + ExtensionTests.TestExtensionsFactions(os1.allFactions, out errorsAdded1);
      Folder folder = Programs.getComputer(os1, "linkNode1").files.root.searchForFolder("bin");
      if (folder.searchForFile("FTPBounce.exe") != null)
      {
        ++errorsAdded1;
        str2 += "\nFile somehow already on target system for faction test";
      }
      if (os1.currentFaction != null)
      {
        ++errorsAdded1;
        str2 += "\nFaction does not start as null";
      }
      ComputerLoader.loadMission(Utils.GetFileLoadPrefix() + "Missions/FactionTestMission0.xml", false);
      if (os1.currentFaction.idName != "autoTestFaction")
      {
        ++errorsAdded1;
        str2 += "\nLoading mission with start function to set player faction does not load faction correctly";
      }
      if (os1.currentFaction.playerValue != 0)
      {
        ++errorsAdded1;
        str2 = str2 + "\nPlayer faction not expected before mission completion. Expected 0, got " + (object) os1.currentFaction.playerValue;
      }
      os1.currentMission.finish();
      if (os1.currentFaction.playerValue != 5)
      {
        ++errorsAdded1;
        str2 = str2 + "\nPlayer faction not expected after mission completion. Expected 5, got " + (object) os1.currentFaction.playerValue;
      }
      FileEntry fileEntry = folder.searchForFile("FTPBounce.exe");
      if (fileEntry == null)
      {
        ++errorsAdded1;
        str2 += "\nFile not added correctly in response to faction progression";
      }
      else if (fileEntry.data != PortExploits.crackExeData[21])
      {
        ++errorsAdded1;
        str2 += "\nFile added through faction system data not correctly filtered.";
      }
      os1.threadedSaveExecute(false);
      screenMan.RemoveScreen((GameScreen) os1);
      OS.WillLoadSave = true;
      OS os2 = (OS) ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
      string str3 = str2 + ExtensionTests.TestExtensionsFactions(os2.allFactions, out errorsAdded1);
      if (os2.currentFaction.idName != "autoTestFaction")
      {
        ++errorsAdded1;
        str3 += "\nFaction not set correctly after load";
      }
      CustomFaction currentFaction = os2.currentFaction as CustomFaction;
      if (currentFaction == null)
      {
        ++errorsAdded1;
        str3 += "\nFaction is not set to the correct type after load";
      }
      if (currentFaction.CustomActions.Count != 1)
      {
        ++errorsAdded1;
        str3 += "\nFaction has incorrect number of remaining custom actions after load";
      }
      if (os2.currentFaction.playerValue != 5)
      {
        ++errorsAdded1;
        str3 = str3 + "\nPlayer faction value not set correctly after load. Expected 5, got " + (object) os2.currentFaction.playerValue;
      }
      os2.currentMission.finish();
      if (Programs.getComputer(os2, "linkNode1").files.root.searchForFolder("bin").searchForFile("SecondTestFile.txt") == null)
      {
        ++errorsAdded1;
        str3 += "\nSecond File not added correctly in response to faction progression";
      }
      screenMan.RemoveScreen((GameScreen) os2);
      ExtensionTests.CompleteExtensiontesting();
      errorsAdded = errorsAdded1;
      return str3;
    }

    private static string TestExtensionsFactions(AllFactions allFactions, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      if (allFactions.factions.Count != 2)
      {
        ++num;
        str = str + "\nExpected 2 factions, got " + (object) allFactions.factions.Count;
      }
      CustomFaction faction = allFactions.factions["tfac"] as CustomFaction;
      if (faction == null)
      {
        ++num;
        str = str + "\nFaction 1 not listed as custom faction! Reports self as " + (object) faction;
      }
      if (faction.name != "Test Faction")
      {
        ++num;
        str += "\nFaction name incorrect!";
      }
      if (faction.playerValue != 0)
      {
        ++num;
        str += "\nFaction starting player value incorrect!";
      }
      if (faction.CustomActions.Count != 5)
      {
        ++num;
        str = str + "\nIncorrect number of custom faction actions! Expected 5, got " + (object) faction.CustomActions.Count;
      }
      errorsAdded = num;
      return str;
    }

    public static string TestExtensionMissionLoading(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str1 = "";
      OS os = (OS) ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
      string str2 = TestSuite.TestMission(ExtensionLoader.ActiveExtensionInfo.FolderPath + "/" + ExtensionLoader.ActiveExtensionInfo.StartingMissionPath, (object) os);
      if (!string.IsNullOrWhiteSpace(str2))
      {
        ++num;
        str1 = str1 + "\n\nMISSION READ ERRORS:\n" + str2;
      }
      screenMan.RemoveScreen((GameScreen) os);
      ExtensionTests.CompleteExtensiontesting();
      errorsAdded = num;
      return str1;
    }

    public static string TestExtensionStartNodeVisibility(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      OS os = (OS) ExtensionTests.SetupOSForTests("Content/Tests/IntroExtension", screenMan);
      for (int index = 0; index < os.netMap.nodes.Count; ++index)
      {
        string idName = os.netMap.nodes[index].idName;
        if (os.netMap.visibleNodes.Contains(index) && (!(idName == "playerComp") && !((IEnumerable<string>) ExtensionLoader.ActiveExtensionInfo.StartingVisibleNodes).Contains<string>(idName) && !((IEnumerable<string>) ExtensionLoader.ActiveExtensionInfo.StartingVisibleNodes).Contains<string>(os.netMap.nodes[index].ip)))
        {
          ++num;
          str = str + "\nNode is discovered but it should not be! Node: " + idName;
        }
      }
      for (int index = 0; index < ExtensionLoader.ActiveExtensionInfo.StartingVisibleNodes.Length; ++index)
      {
        string startingVisibleNode = ExtensionLoader.ActiveExtensionInfo.StartingVisibleNodes[index];
        Computer computer = Programs.getComputer(os, startingVisibleNode);
        if (computer != null && !os.netMap.visibleNodes.Contains(os.netMap.nodes.IndexOf(computer)))
        {
          ++num;
          str = str + "\nNode " + startingVisibleNode + " should be discovered, but it is not! It's on the starting visible list";
        }
      }
      screenMan.RemoveScreen((GameScreen) os);
      ExtensionTests.CompleteExtensiontesting();
      errorsAdded = num;
      return str;
    }

    public static string TestPopulatedExtension(ScreenManager screenMan, out int errorsAdded)
    {
      int currentErrorCount = 0;
      string str1 = "";
      OS os1 = (OS) ExtensionTests.SetupOSForTests("Content/Tests/IntroExtension", screenMan);
      List<Computer> nodes = os1.netMap.nodes;
      string str2 = str1 + ExtensionTests.TestPopulatedExtensionRead((object) os1, out currentErrorCount);
      os1.threadedSaveExecute(false);
      screenMan.RemoveScreen((GameScreen) os1);
      OS.WillLoadSave = true;
      OS os2 = (OS) ExtensionTests.SetupOSForTests("Content/Tests/IntroExtension", screenMan);
      string str3 = str2 + ExtensionTests.TestPopulatedExtensionRead((object) os2, out currentErrorCount) + TestSuite.getTestingReportForLoadComparison((object) os2, nodes, currentErrorCount, out currentErrorCount);
      screenMan.RemoveScreen((GameScreen) os2);
      ExtensionTests.CompleteExtensiontesting();
      errorsAdded = currentErrorCount;
      return str3;
    }

    private static string TestPopulatedExtensionRead(object os_obj, out int errorsAdded)
    {
      string str = "";
      errorsAdded = 0;
      OS os = (OS) os_obj;
      Computer computer1 = Programs.getComputer(os, "linkNode1");
      Computer computer2 = Programs.getComputer(os, "linkNode2");
      Computer computer3 = Programs.getComputer(os, "linkNode3");
      if (!computer1.links.Contains(os.netMap.nodes.IndexOf(computer2)) || !computer2.links.Contains(os.netMap.nodes.IndexOf(computer3)) || !computer3.links.Contains(os.netMap.nodes.IndexOf(computer1)))
      {
        ++errorsAdded;
        str += "\nLinks Error! Link node 1,2 or 3 does not have specified links\n";
      }
      Computer computer4 = Programs.getComputer(os, "secTestNode");
      if (computer4.adminPass != "sectestpass")
      {
        str += "\nAdmin Password for SecNode does not match!";
        ++errorsAdded;
      }
      if (computer4.portsNeededForCrack != 1)
      {
        str = str + "\nSecNode ports needed for crack mismatch. Expected 2, got " + (object) computer4.portsNeededForCrack;
        ++errorsAdded;
      }
      if (!Utils.FloatEquals(computer4.traceTime, 300f))
      {
        str = str + "\nSecNode trace time mismatch. Expected 300, got " + (object) computer4.traceTime;
        ++errorsAdded;
      }
      if (!Utils.FloatEquals(computer4.proxyOverloadTicks, 2f * Computer.BASE_PROXY_TICKS))
      {
        str = str + "\nSecNode proxy time mismatch. Expected " + (object) (float) (2.0 * (double) Computer.BASE_PROXY_TICKS) + " Got " + (object) computer4.proxyOverloadTicks;
        ++errorsAdded;
      }
      if (computer4.admin.ResetsPassword || computer4.admin.IsSuper || !(computer4.admin is FastBasicAdministrator))
      {
        str = str + "\nSecNode administrator error! Expected non resetting Fast admin, got " + computer4.admin.ToString();
        ++errorsAdded;
      }
      Firewall firewall = new Firewall(6, "Scypio", 1f);
      if (firewall.getSaveString() != computer4.firewall.getSaveString())
      {
        str = str + "\nFireall difference! Expected \n" + firewall.getSaveString() + "\nbut got\n" + computer4.firewall.getSaveString();
        ++errorsAdded;
      }
      if (!computer4.ports.Contains(21) || !computer4.ports.Contains(22) || (!computer4.ports.Contains(80) || !computer4.ports.Contains(25)) || !computer4.ports.Contains(1433))
      {
        ++errorsAdded;
        str += "\nSecNodem ports assignment error\n";
      }
      bool flag1 = false;
      bool flag2 = false;
      for (int index = 0; index < computer4.users.Count; ++index)
      {
        if (computer4.users[index].name == "SecTest")
        {
          flag1 = true;
          if (computer4.users[index].pass != "userpas")
          {
            ++errorsAdded;
            str = str + "\nUser account password for user SecTest do not match. Found Pass :" + computer4.users[index].pass + "\n";
          }
        }
        if (computer4.users[index].name == "mailGuy")
        {
          flag2 = true;
          if (computer4.users[index].pass != "mailPass")
          {
            ++errorsAdded;
            str = str + "\nUser account password for user MailGuy do not match. Found Pass :" + computer4.users[index].pass + "\n";
          }
        }
      }
      if (!flag2 || !flag1)
      {
        ++errorsAdded;
        str = str + "\nAccounts missing from SecTest node - found " + (object) computer4.users.Count + " users.";
      }
      return str;
    }

    public static string TestBlankExtension(ScreenManager screenMan, out int errorsAdded)
    {
      int errors = 0;
      string ret = "";
      OS os;
      try
      {
        os = (OS) ExtensionTests.SetupOSForTests("Content/Tests/TestBlankExtension", screenMan);
      }
      catch (Exception ex)
      {
        errorsAdded = 1;
        return "\nError generating blank extension:\n" + Utils.GenerateReportFromException(ex);
      }
      ExtensionTests.TestOSForBlankSession((object) os, out ret, out errors);
      screenMan.RemoveScreen((GameScreen) os);
      ExtensionTests.CompleteExtensiontesting();
      errorsAdded = errors;
      return ret;
    }

    public static string TestAcademicDatabase(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      OS os1 = (OS) ExtensionTests.SetupOSForTests("Content/Tests/IntroExtension", screenMan);
      if (os1.netMap.academicDatabase == null)
      {
        ++num;
        str += "\nNo Academic Database Detected where there should be one!";
      }
      os1.threadedSaveExecute(false);
      screenMan.RemoveScreen((GameScreen) os1);
      OS os2 = (OS) null;
      OS.WillLoadSave = true;
      OS os3 = (OS) ExtensionTests.SetupOSForTests("Content/Tests/IntroExtension", screenMan);
      if (os3.netMap.academicDatabase == null)
      {
        ++num;
        str += "\nNo Academic Database Detected after save/load!";
      }
      screenMan.RemoveScreen((GameScreen) os3);
      os2 = (OS) null;
      OS.WillLoadSave = false;
      OS os4 = (OS) ExtensionTests.SetupOSForTests("Content/Tests/TestBlankExtension", screenMan);
      if (os4.netMap.academicDatabase != null)
      {
        ++num;
        str += "\nAcademic database was created in empty session!!";
      }
      screenMan.RemoveScreen((GameScreen) os4);
      ExtensionTests.CompleteExtensiontesting();
      errorsAdded = num;
      return str;
    }

    private static void TestOSForBlankSession(object osobj, out string ret, out int errors)
    {
      OS os = (OS) osobj;
      ret = "";
      errors = 0;
      if (os.netMap.nodes.Count != 2)
      {
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        string& local = @ret;
        // ISSUE: explicit reference operation
        string str = ^local + "\nError: Expected exactly 2 nodes but found " + (object) os.netMap.nodes.Count + "\n";
        // ISSUE: explicit reference operation
        ^local = str;
        ++errors;
      }
      bool flag1 = false;
      bool flag2 = false;
      for (int index1 = 0; index1 < os.netMap.nodes.Count; ++index1)
      {
        if (os.netMap.nodes[index1].idName == "playerComp")
          flag1 = true;
        if (os.netMap.nodes[index1].idName == "jmail")
        {
          if (os.netMap.nodes[index1].getDaemon(typeof (MailServer)) != null)
          {
            flag2 = true;
            if (os.netMap.mailServer != os.netMap.nodes[index1])
            {
              ret += "\nMail server is not correctly registered For NetMap.MailServer!\n";
              ++errors;
            }
            bool flag3 = false;
            for (int index2 = 0; index2 < os.netMap.nodes[index1].users.Count; ++index2)
            {
              if (os.netMap.nodes[index1].users[index1].name == os.defaultUser.name)
                flag3 = true;
            }
            if (!flag3)
            {
              // ISSUE: explicit reference operation
              // ISSUE: variable of a reference type
              string& local = @ret;
              // ISSUE: explicit reference operation
              string str = ^local + "M\nail server does not contain a User for the player! Has " + (object) os.netMap.nodes[index1].users.Count + " users.";
              // ISSUE: explicit reference operation
              ^local = str;
              ++errors;
            }
          }
          else
          {
            ret += "\nMail Server ID found but it contains no MailServer Daemon!";
            ++errors;
          }
        }
      }
      if (!flag2 || !flag1)
      {
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        string& local = @ret;
        // ISSUE: explicit reference operation
        string str = ^local + "\nBlank server should have a player node and a JMail server with an email daemon. Has Player Server = " + (object) flag1 + ", Has Mail Server = " + (object) flag2 + "\n";
        // ISSUE: explicit reference operation
        ^local = str;
        ++errors;
      }
      if (os.allFactions.factions.Count == 0)
        return;
      ++errors;
      // ISSUE: explicit reference operation
      // ISSUE: variable of a reference type
      string& local1 = @ret;
      // ISSUE: explicit reference operation
      string str1 = ^local1 + "\nBlank session has factions! : " + (object) os.allFactions.factions.Count + " found.";
      // ISSUE: explicit reference operation
      ^local1 = str1;
    }

    public static string TestBlankExtensionSaveLoad(ScreenManager screenMan, out int errorsAdded)
    {
      string ret = "";
      int errors = 0;
      OS os1 = (OS) ExtensionTests.SetupOSForTests("Content/Tests/TestBlankExtension", screenMan);
      List<Computer> nodes = os1.netMap.nodes;
      os1.threadedSaveExecute(false);
      screenMan.RemoveScreen((GameScreen) os1);
      OS.WillLoadSave = true;
      OS os2 = (OS) ExtensionTests.SetupOSForTests("Content/Tests/TestBlankExtension", screenMan);
      ExtensionTests.TestOSForBlankSession((object) os2, out ret, out errors);
      screenMan.RemoveScreen((GameScreen) os2);
      ExtensionTests.CompleteExtensiontesting();
      errorsAdded = errors;
      return ret;
    }

    private static object SetupOSForTests(string ActiveExtensionFoldername, ScreenManager screenMan)
    {
      string username = "__ExtensionTest";
      string fileNameForUsername = SaveFileManager.GetSaveFileNameForUsername(username);
      SaveFileManager.AddUser(username, "testpass");
      ExtensionTests.OS_wasTestingPass = OS.TestingPassOnly;
      OS.TestingPassOnly = true;
      Settings.IsInExtensionMode = true;
      ExtensionLoader.ActiveExtensionInfo = ExtensionInfo.ReadExtensionInfo(ActiveExtensionFoldername);
      OS os = new OS();
      os.SaveUserAccountName = username;
      os.SaveGameUserName = fileNameForUsername;
      screenMan.AddScreen((GameScreen) os);
      return (object) os;
    }

    private static void CompleteExtensiontesting()
    {
      OS.TestingPassOnly = ExtensionTests.OS_wasTestingPass;
      Settings.IsInExtensionMode = false;
      ExtensionLoader.ActiveExtensionInfo = (ExtensionInfo) null;
    }
  }
}
