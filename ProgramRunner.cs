// Decompiled with JetBrains decompiler
// Type: Hacknet.ProgramRunner
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Extensions;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  public static class ProgramRunner
  {
    public static bool ExecuteProgram(object os_object, string[] arguments)
    {
      OS os = (OS) os_object;
      string[] strArray = arguments;
      bool flag1 = true;
      if (strArray[0].ToLower().Equals("connect"))
      {
        Programs.connect(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].Equals("disconnect") || strArray[0].Equals("dc"))
        Programs.disconnect(strArray, os);
      else if (strArray[0].Equals("ls") || strArray[0].Equals("dir"))
        Programs.ls(strArray, os);
      else if (strArray[0].Equals("cd"))
        Programs.cd(strArray, os);
      else if (strArray[0].Equals("cd.."))
      {
        strArray = new string[2]{ "cd", ".." };
        Programs.cd(strArray, os);
      }
      else if (strArray[0].Equals("cat") || strArray[0].Equals("more") || strArray[0].Equals("less"))
        Programs.cat(strArray, os);
      else if (strArray[0].Equals("exe"))
      {
        Programs.execute(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("probe") || strArray[0].Equals("nmap"))
        Programs.probe(strArray, os);
      else if (strArray[0].Equals("scp"))
      {
        Programs.scp(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("scan"))
      {
        Programs.scan(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].Equals("rm") || strArray[0].Equals("del"))
      {
        Programs.rm(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].Equals("mv"))
      {
        Programs.mv(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].Equals("ps"))
      {
        Programs.ps(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("kill") || strArray[0].Equals("pkill"))
      {
        Programs.kill(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("reboot"))
      {
        Programs.reboot(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("opencdtray"))
      {
        Programs.opCDTray(strArray, os, true);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("closecdtray"))
      {
        Programs.opCDTray(strArray, os, false);
        flag1 = false;
      }
      else if (strArray[0].Equals("replace"))
      {
        Programs.replace2(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("analyze"))
      {
        Programs.analyze(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("solve"))
      {
        Programs.solve(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("clear"))
      {
        Programs.clear(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("upload") || strArray[0].Equals("up"))
      {
        Programs.upload(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("login"))
      {
        Programs.login(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("addnote"))
      {
        Programs.addNote(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals(":(){:|:&};:"))
        ProgramRunner.ExecuteProgram((object) os, new string[1]
        {
          "forkbomb"
        });
      else if (strArray[0].ToLower().Equals("append"))
      {
        flag1 = false;
        string[] quoteSeperatedArgs = Utils.GetQuoteSeperatedArgs(strArray);
        Folder currentFolder = Programs.getCurrentFolder(os);
        if (quoteSeperatedArgs.Length > 1)
        {
          FileEntry fileEntry1 = currentFolder.searchForFile(quoteSeperatedArgs[1]);
          int num = 2;
          if (fileEntry1 == null)
          {
            fileEntry1 = currentFolder.searchForFile(os.display.commandArgs[1]);
            if (fileEntry1 == null)
            {
              os.write("Usage: append [FILENAME] [LINE TO APPEND]");
              return flag1;
            }
            os.write("No filename provided");
            os.write("Assuming active flag file \"" + fileEntry1.name + "\" For editing");
            if (strArray.Length == 1)
              strArray = new string[2]
              {
                "append",
                fileEntry1.name
              };
            else
              strArray[1] = fileEntry1.name;
            num = 1;
          }
          if (fileEntry1 != null)
          {
            string str1 = "";
            for (int index = num; index < quoteSeperatedArgs.Length; ++index)
              str1 = str1 + quoteSeperatedArgs[index] + " ";
            FileEntry fileEntry2 = fileEntry1;
            string str2 = fileEntry2.data + "\n" + str1;
            fileEntry2.data = str2;
            flag1 = true;
            strArray[0] = "cat";
            strArray[1] = fileEntry1.name;
            for (int index = 2; index < strArray.Length; ++index)
              strArray[index] = "";
            Programs.cat(strArray, os);
          }
        }
        else
        {
          os.write("Usage: append [FILENAME] [LINE TO APPEND]");
          return flag1;
        }
      }
      else if (strArray[0].Equals("remline"))
      {
        FileEntry fileEntry = Programs.getCurrentFolder(os).searchForFile(strArray[1]);
        if (fileEntry != null)
        {
          int length = fileEntry.data.LastIndexOf('\n');
          if (length < 0)
            length = 0;
          fileEntry.data = fileEntry.data.Substring(0, length);
          flag1 = true;
          strArray[0] = "cat";
          for (int index = 2; index < strArray.Length; ++index)
            strArray[index] = "";
          Programs.cat(strArray, os);
        }
      }
      else if (strArray[0].Equals("getString"))
      {
        Programs.getString(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("reloadtheme"))
      {
        FileEntry fileEntry = os.thisComputer.files.root.searchForFolder("sys").searchForFile("x-server.sys");
        if (fileEntry != null)
        {
          OSTheme themeForDataString = ThemeManager.getThemeForDataString(fileEntry.data);
          ThemeManager.switchTheme((object) os, themeForDataString);
        }
        flag1 = false;
      }
      else if (strArray[0].Equals("FirstTimeInitdswhupwnemfdsiuoewnmdsmffdjsklanfeebfjkalnbmsdakj"))
      {
        Programs.firstTimeInit(strArray, os, false);
        flag1 = false;
      }
      else if (strArray[0].Equals("chat"))
      {
        string message = "chat " + os.username + " ";
        for (int index = 1; index < strArray.Length; ++index)
          message = message + strArray[index] + " ";
        if (os.multiplayer)
          os.sendMessage(message);
        flag1 = false;
      }
      else if ((strArray[0].Equals("exitdemo") || strArray[0].Equals("resetdemo")) && Settings.isDemoMode)
      {
        MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
        MainMenu mainMenu = new MainMenu();
        os.ScreenManager.AddScreen((GameScreen) mainMenu);
        MainMenu.resetOS();
        os.ExitScreen();
        OS.currentInstance = (OS) null;
        flag1 = false;
        if (Settings.MultiLingualDemo)
          LocaleActivator.ActivateLocale("zh-cn", Game1.getSingleton().Content);
      }
      else if (strArray[0].Equals("fh") && OS.DEBUG_COMMANDS)
      {
        Programs.fastHack(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].Equals("ra") && OS.DEBUG_COMMANDS)
      {
        Programs.revealAll(strArray, os);
        flag1 = false;
      }
      else if (strArray[0].Equals("deathseq") && OS.DEBUG_COMMANDS)
      {
        os.TraceDangerSequence.BeginTraceDangerSequence();
        flag1 = false;
      }
      else if (strArray[0].Equals("testcredits") && OS.DEBUG_COMMANDS)
      {
        os.endingSequence.IsActive = true;
        flag1 = false;
      }
      else if (strArray[0].Equals("addflag") && OS.DEBUG_COMMANDS)
      {
        if (strArray.Length < 2)
          os.write("\nFlag to add required\n");
        os.Flags.AddFlag(strArray[1]);
        flag1 = false;
      }
      else if (strArray[0].Equals("addTestEmails") && OS.DEBUG_COMMANDS)
      {
        for (int index = 0; index < 4; ++index)
          ((MailServer) os.netMap.mailServer.getDaemon(typeof (MailServer))).addMail(MailServer.generateEmail("testEmail " + (object) index + " " + Utils.getRandomByte().ToString(), "test", "test"), os.defaultUser.name);
        flag1 = false;
      }
      else if (strArray[0].Equals("dscan") && OS.DEBUG_COMMANDS)
      {
        if (strArray.Length < 2)
          os.write("\nNode ID Required\n");
        bool flag2 = false;
        for (int index = 0; index < os.netMap.nodes.Count; ++index)
        {
          if (os.netMap.nodes[index].idName.ToLower().StartsWith(strArray[1].ToLower()))
          {
            os.netMap.discoverNode(os.netMap.nodes[index]);
            os.netMap.nodes[index].highlightFlashTime = 1f;
            flag2 = true;
            break;
          }
        }
        if (!flag2)
          os.write("Node ID Not found");
        flag1 = false;
      }
      else if (strArray[0].Equals("revmany") && OS.DEBUG_COMMANDS)
      {
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
        os.netMap.lastAddedNode = os.thisComputer;
        os.homeAssetServerID = "dhsDrop";
        os.homeNodeID = "dhs";
        os.netMap.discoverNode(Programs.getComputer(os, "dhs"));
        os.netMap.discoverNode(Programs.getComputer(os, "dhsDrop"));
        flag1 = false;
      }
      else if (strArray[0].ToLower().Equals("reloadext") && OS.DEBUG_COMMANDS)
      {
        if (Settings.IsInExtensionMode)
          ExtensionLoader.ReloadExtensionNodes((object) os);
        flag1 = false;
      }
      else if (strArray[0].Equals("testsave") && OS.DEBUG_COMMANDS || strArray[0].Equals("save!(SJN!*SNL8vAewew57WewJdwl89(*4;;;&!)@&(ak'^&#@J3KH@!*"))
      {
        os.threadedSaveExecute(false);
        SettingsLoader.writeStatusFile();
        flag1 = false;
      }
      else if (strArray[0].Equals("testload") && OS.DEBUG_COMMANDS)
        flag1 = false;
      else if (strArray[0].Equals("teststrikerhack") && OS.DEBUG_COMMANDS)
      {
        os.delayer.Post(ActionDelayer.Wait(3.0), (Action) (() => MissionFunctions.runCommand(1, "triggerDLCHackRevenge")));
        flag1 = false;
      }
      else if (strArray[0].Equals("linkToCSECPostDLC") && OS.DEBUG_COMMANDS)
      {
        os.execute("dscan mainhub");
        os.allFactions.setCurrentFaction("hub", os);
        os.currentFaction.playerValue = 2;
        os.Flags.AddFlag("dlc_complete");
        os.Flags.AddFlag("dlc_csec_end_facval:0");
        MissionFunctions.runCommand(1, "addRank");
        flag1 = false;
      }
      else if (strArray[0].Equals("debug") && OS.DEBUG_COMMANDS)
      {
        int num = PortExploits.services.Count;
        if (strArray.Length > 1)
        {
          try
          {
            num = Convert.ToInt32(strArray[1]);
          }
          catch (Exception ex)
          {
          }
        }
        for (int index = 0; index < PortExploits.services.Count && index <= num; ++index)
          os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[PortExploits.portNums[index]], PortExploits.cracks[PortExploits.portNums[index]]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[9], PortExploits.cracks[9]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[10], PortExploits.cracks[10]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[11], PortExploits.cracks[11]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[12], PortExploits.cracks[12]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[13], PortExploits.cracks[13]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[14], PortExploits.cracks[14]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[15], PortExploits.cracks[15]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[16], PortExploits.cracks[16]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[17], PortExploits.cracks[17]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[31], PortExploits.cracks[31]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[33], PortExploits.cracks[33]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[34], PortExploits.cracks[34]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[35], PortExploits.cracks[35]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[36], PortExploits.cracks[36]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[37], PortExploits.cracks[37]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[38], PortExploits.cracks[38]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[39], PortExploits.cracks[39]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[41], PortExploits.cracks[41]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[554], PortExploits.cracks[554]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[40], PortExploits.cracks[40]));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.DangerousPacemakerFirmware, "KBT_TestFirmware.dll"));
        os.Flags.AddFlag("dechead");
        os.Flags.AddFlag("decypher");
        os.Flags.AddFlag("csecBitSet01Complete");
        os.Flags.AddFlag("csecRankingS2Pass");
        os.Flags.AddFlag("CSEC_Member");
        os.Flags.AddFlag("bitPathStarted");
        flag1 = false;
        for (int index = 0; index < 4; ++index)
        {
          Computer c = new Computer("DebugShell" + (object) index, NetworkMap.generateRandomIP(), os.netMap.getRandomPosition(), 0, (byte) 2, os);
          c.adminIP = os.thisComputer.adminIP;
          os.netMap.nodes.Add(c);
          os.netMap.discoverNode(c);
        }
        os.netMap.discoverNode("practiceServer");
        os.netMap.discoverNode("entropy00");
      }
      else if (strArray[0].Equals("flash") && OS.DEBUG_COMMANDS)
      {
        os.traceTracker.start(40f);
        os.warningFlash();
        flag1 = false;
        os.IncConnectionOverlay.Activate();
      }
      else if (strArray[0].Equals("cycletheme") && OS.DEBUG_COMMANDS)
      {
        Action<OSTheme> ctheme = (Action<OSTheme>) (theme => ThemeManager.switchTheme((object) os, theme));
        int next = 1;
        double delay = 1.2;
        Action cthemeAct = (Action) (() =>
        {
          ctheme((OSTheme) next);
          next = (next + 1) % 7;
        });
        cthemeAct += (Action) (() => os.delayer.Post(ActionDelayer.Wait(delay), cthemeAct));
        cthemeAct();
      }
      else if (strArray[0].Equals("testdlc") && OS.DEBUG_COMMANDS)
      {
        MissionFunctions.runCommand(0, "demoFinalMissionEndDLC");
        flag1 = false;
      }
      else if (strArray[0].Equals("testircentries") && OS.DEBUG_COMMANDS)
      {
        DLCHubServer daemon = Programs.getComputer(os, "dhs").getDaemon(typeof (DLCHubServer)) as DLCHubServer;
        for (int index = 0; index < 100; ++index)
          daemon.IRCSystem.AddLog("Test", "Test Message\nMultiline\nMessage", (string) null);
        flag1 = false;
      }
      else if (strArray[0].Equals("testirc") && OS.DEBUG_COMMANDS)
      {
        DLCHubServer daemon = Programs.getComputer(os, "dhs").getDaemon(typeof (DLCHubServer)) as DLCHubServer;
        daemon.IRCSystem.AddLog("Test", "Test Message", (string) null);
        daemon.IRCSystem.AddLog("Channel", "Test Message\nfrom channel", (string) null);
        flag1 = false;
      }
      else if (strArray[0].Equals("flashtest") && OS.DEBUG_COMMANDS)
      {
        if (!PostProcessor.dangerModeEnabled)
        {
          PostProcessor.dangerModeEnabled = true;
          PostProcessor.dangerModePercentComplete = 0.5f;
        }
        else
        {
          PostProcessor.dangerModeEnabled = false;
          PostProcessor.dangerModePercentComplete = 0.0f;
        }
        flag1 = false;
      }
      else if (strArray[0].Equals("dectest") && OS.DEBUG_COMMANDS)
      {
        string str1 = "this is a test message for the encrypter";
        string str2 = FileEncrypter.EncryptString(str1, "header message", "1.2.3.4.5", "loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooongpass", (string) null);
        os.write(str1);
        os.write("  ");
        os.write("  ");
        os.write(str2);
        os.write("  ");
        os.write("  ");
        os.write(FileEncrypter.MakeReplacementsForDisplay(FileEncrypter.DecryptString(str2, "loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooongpass")[2]));
        os.write("  ");
        os.write(FileEncrypter.MakeReplacementsForDisplay(FileEncrypter.DecryptString(str2, "wrongPass")[2] == null ? "NULL" : "CORRECT"));
        os.write("  ");
      }
      else if (strArray[0].Equals("test") && OS.DEBUG_COMMANDS)
        ((DLCHubServer) Programs.getComputer(os, "dhs").getDaemon(typeof (DLCHubServer))).AddMission((ActiveMission) ComputerLoader.readMission("Content/DLC/Missions/Attack/AttackMission.xml"), (string) null, false);
      else if (strArray[0].Equals("testtrace") && OS.DEBUG_COMMANDS)
        MissionFunctions.runCommand(1, "triggerDLCHackRevenge");
      else if (strArray[0].Equals("testboot") && OS.DEBUG_COMMANDS)
      {
        os.BootAssitanceModule.IsActive = true;
        os.bootingUp = false;
        os.canRunContent = false;
        MusicManager.stop();
      }
      else if (strArray[0].Equals("testhhbs") && OS.DEBUG_COMMANDS)
        os.write(HostileHackerBreakinSequence.IsInBlockingHostileFileState((object) os) ? "BLOCKED" : "SAFE");
      else if (strArray[0].Equals("printflags") && OS.DEBUG_COMMANDS)
        os.write(os.Flags.GetSaveString());
      else if (strArray[0].Equals("loseadmin") && OS.DEBUG_COMMANDS)
      {
        os.connectedComp.adminIP = os.connectedComp.ip;
        flag1 = false;
      }
      else if (strArray[0].Equals("runcmd") && OS.DEBUG_COMMANDS)
      {
        if (strArray.Length > 1)
        {
          string name = strArray[1];
          int num = 0;
          if (strArray.Length > 2)
            num = Convert.ToInt32(strArray[1]);
          MissionFunctions.runCommand(num, name);
        }
      }
      else if (strArray[0].ToLower().Equals("runhackscript") && OS.DEBUG_COMMANDS)
      {
        if (strArray.Length > 1)
        {
          string scriptName = strArray[1];
          try
          {
            HackerScriptExecuter.runScript(scriptName, (object) os, os.thisComputer.ip, os.thisComputer.ip);
          }
          catch (Exception ex)
          {
            os.write("Error launching script " + scriptName);
            os.write(Utils.GenerateReportFromExceptionCompact(ex));
          }
        }
      }
      else if (strArray[0].Equals("MotIsTheBest") && OS.DEBUG_COMMANDS)
      {
        os.runCommand("probe");
        os.runCommand("exe WebServerWorm 80");
        os.runCommand("exe SSHcrack 22");
        os.runCommand("exe SMTPoverflow 25");
        os.runCommand("exe FTPBounce 21");
      }
      else if (strArray[0].Equals("help") || strArray[0].Equals("Help") || strArray[0].Equals("?") || strArray[0].Equals("man"))
      {
        int page = 0;
        if (strArray.Length > 1)
        {
          try
          {
            page = Convert.ToInt32(strArray[1]);
            if (page > Helpfile.getNumberOfPages())
            {
              os.write("Invalid Page Number - Displaying First Page");
              page = 0;
            }
          }
          catch (FormatException ex)
          {
            os.write("Invalid Page Number");
          }
          catch (OverflowException ex)
          {
            os.write("Invalid Page Number");
          }
        }
        Helpfile.writeHelp(os, page);
        flag1 = false;
      }
      else
      {
        if (strArray[0] != "")
        {
          int num = ProgramRunner.AttemptExeProgramExecution(os, strArray);
          if (num == 0)
            os.write("Execution failed");
          else if (num < 0)
            os.write("No Command " + strArray[0] + " - Check Syntax\n");
        }
        flag1 = false;
      }
      if (flag1)
      {
        if (!os.commandInvalid)
        {
          os.display.command = strArray[0];
          os.display.commandArgs = strArray;
          os.display.typeChanged();
        }
        else
          os.commandInvalid = false;
      }
      return flag1;
    }

    public static bool ExeProgramExists(string name, object binariesFolder)
    {
      Folder folder = (Folder) binariesFolder;
      name = name.Replace(".exe", "").ToLower();
      switch (name)
      {
        case "porthack":
        case "forkbomb":
        case "shell":
        case "securitytracer":
        case "tutorial":
        case "notes":
          return true;
        default:
          bool flag = false;
          int num = -1;
          for (int index = 0; index < folder.files.Count; ++index)
          {
            if (folder.files[index].name.Equals(name) || folder.files[index].name.Replace(".exe", "").Equals(name) || folder.files[index].name.Replace(".exe", "").ToLower().Equals(name))
            {
              flag = true;
              num = index;
              break;
            }
          }
          return flag;
      }
    }

    private static int GetFileIndexOfExeProgram(string name, object binariesFolder)
    {
      Folder folder = (Folder) binariesFolder;
      name = name.Replace(".exe", "").ToLower();
      switch (name)
      {
        case "porthack":
        case "forkbomb":
        case "shell":
        case "tutorial":
        case "notes":
          return int.MaxValue;
        default:
          int num = -1;
          for (int index = 0; index < folder.files.Count; ++index)
          {
            if (folder.files[index].name.Equals(name) || folder.files[index].name.Replace(".exe", "").Equals(name) || folder.files[index].name.Replace(".exe", "").ToLower().Equals(name))
            {
              num = index;
              break;
            }
          }
          return num;
      }
    }

    private static int AttemptExeProgramExecution(OS os, string[] p)
    {
      Computer target = os.connectedComp != null ? os.connectedComp : os.thisComputer;
      Folder folder = os.thisComputer.files.root.searchForFolder("bin");
      int indexOfExeProgram = ProgramRunner.GetFileIndexOfExeProgram(p[0], (object) folder);
      bool flag = indexOfExeProgram == int.MaxValue;
      int index1 = -1;
      int index2 = -1;
      if (indexOfExeProgram < 0)
        return -1;
      string exeFileData = (string) null;
      string exeName = (string) null;
      if (!flag)
      {
        exeFileData = folder.files[indexOfExeProgram].data;
        for (int index3 = 0; index3 < PortExploits.exeNums.Count; ++index3)
        {
          int exeNum = PortExploits.exeNums[index3];
          if (PortExploits.crackExeData[exeNum].Equals(exeFileData) || PortExploits.crackExeDataLocalRNG[exeNum].Equals(exeFileData))
          {
            exeName = PortExploits.cracks[exeNum].Replace(".exe", "").ToLower();
            index2 = exeNum;
            break;
          }
        }
        if (exeName == "ftpsprint")
        {
          int num;
          index2 = num = 21;
          if (exeFileData == PortExploits.crackExeData[211] || exeFileData == PortExploits.crackExeDataLocalRNG[211])
            index1 = 211;
        }
      }
      else
      {
        exeName = p[0].Replace(".exe", "").ToLower();
        if (exeName == "notes")
          exeFileData = PortExploits.crackExeData[8];
        if (exeName == "tutorial")
          exeFileData = PortExploits.crackExeData[1];
      }
      if (exeName == null)
        return -1;
      int targetPort = -1;
      int codePort = -1;
      if (!flag && PortExploits.needsPort[index2])
      {
        if (p.Length > 1)
        {
          try
          {
            int int32 = Convert.ToInt32(p[1]);
            int num = int32;
            codePort = target.GetCodePortNumberFromDisplayPort(int32);
            if (num == codePort)
            {
              int numberFromCodePort = target.GetDisplayPortNumberFromCodePort(codePort);
              if (codePort != numberFromCodePort)
                codePort = -1;
            }
          }
          catch (FormatException ex)
          {
            codePort = -1;
          }
        }
        else
        {
          if (exeName == "ssltrojan")
          {
            SSLPortExe.GenerateInstanceOrNullFromArguments(p, Rectangle.Empty, (object) os, target);
            return 0;
          }
          os.write(LocaleTerms.Loc("No port number Provided"));
          return 0;
        }
      }
      if (!flag && PortExploits.needsPort[index2])
      {
        try
        {
          for (int index3 = 0; index3 < target.ports.Count; ++index3)
          {
            if (target.ports[index3] == codePort)
            {
              targetPort = target.ports[index3];
              break;
            }
          }
        }
        catch
        {
          os.write(LocaleTerms.Loc("No port number Provided"));
          return 0;
        }
        if (targetPort == -1)
        {
          os.write(LocaleTerms.Loc("Target Port is Closed"));
          return 0;
        }
        if (index1 <= -1)
          index1 = targetPort;
        if (index1 == 211 && targetPort != 21 || exeFileData != PortExploits.crackExeData[index1] && exeFileData != PortExploits.crackExeDataLocalRNG[index1])
        {
          os.write(LocaleTerms.Loc("Target Port running incompatible service for this executable"));
          return 0;
        }
      }
      os.launchExecutable(exeName, exeFileData, targetPort, p, p[0]);
      return 1;
    }
  }
}
