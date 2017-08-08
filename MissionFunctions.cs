// Decompiled with JetBrains decompiler
// Type: Hacknet.MissionFunctions
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Factions;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  public static class MissionFunctions
  {
    private static OS os;
    public static Action<string> ReportErrorInCommand;

    private static void assertOS()
    {
      MissionFunctions.os = OS.currentInstance;
    }

    public static void runCommand(int value, string name)
    {
      MissionFunctions.assertOS();
      if (name.ToLower().Trim() == "none")
        return;
      if (name.Equals("addRank"))
      {
        if (!OS.TestingPassOnly || MissionFunctions.os.currentFaction != null)
        {
          MissionFunctions.os.currentFaction.addValue(value, (object) MissionFunctions.os);
          string email = MailServer.generateEmail(LocaleTerms.Loc("Contract Successful"), string.Format(Utils.readEntireFile("Content/LocPost/MissionCompleteEmail.txt"), (object) MissionFunctions.os.currentFaction.getRank(), (object) MissionFunctions.os.currentFaction.getMaxRank(), (object) MissionFunctions.os.currentFaction.name), MissionFunctions.os.currentFaction.name + " ReplyBot");
          ((MailServer) MissionFunctions.os.netMap.mailServer.getDaemon(typeof (MailServer))).addMail(email, MissionFunctions.os.defaultUser.name);
        }
        else if (OS.DEBUG_COMMANDS && MissionFunctions.os.currentFaction == null)
        {
          MissionFunctions.os.write("----------");
          MissionFunctions.os.write("----------");
          MissionFunctions.os.write("ERROR IN FUNCTION 'addRank'");
          MissionFunctions.os.write("Player is not assigned to a faction, so rank cannot be added!");
          MissionFunctions.os.write("Make sure you have assigned a player a faction with the 'SetFaction' function before using this!");
          MissionFunctions.os.write("----------");
          MissionFunctions.os.write("----------");
        }
      }
      else if (name.Equals("addRankSilent"))
      {
        if (OS.TestingPassOnly && MissionFunctions.os.currentFaction == null)
          return;
        MissionFunctions.os.currentFaction.addValue(value, (object) MissionFunctions.os);
      }
      else if (name.StartsWith("addFlags:"))
      {
        foreach (string flag in name.Substring("addFlags:".Length).Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries))
          MissionFunctions.os.Flags.AddFlag(flag);
        CustomFaction currentFaction = MissionFunctions.os.currentFaction as CustomFaction;
        if (currentFaction == null)
          return;
        currentFaction.CheckForAllCustomActionsToRun((object) MissionFunctions.os);
      }
      else if (name.StartsWith("removeFlags:"))
      {
        string[] strArray = name.Substring("removeFlags:".Length).Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
        for (int index = 0; index < strArray.Length; ++index)
        {
          if (MissionFunctions.os.Flags.HasFlag(strArray[index]))
            MissionFunctions.os.Flags.RemoveFlag(strArray[index]);
        }
        CustomFaction currentFaction = MissionFunctions.os.currentFaction as CustomFaction;
        if (currentFaction == null)
          return;
        currentFaction.CheckForAllCustomActionsToRun((object) MissionFunctions.os);
      }
      else if (name.StartsWith("setFaction:"))
      {
        string newFaction = name.Substring("setFaction:".Length);
        bool flag = false;
        foreach (KeyValuePair<string, Faction> faction in MissionFunctions.os.allFactions.factions)
        {
          if (faction.Value.idName.ToLower() == newFaction.ToLower())
          {
            MissionFunctions.os.allFactions.setCurrentFaction(newFaction, MissionFunctions.os);
            flag = true;
            break;
          }
        }
        if (!flag && OS.TestingPassOnly)
          throw new NullReferenceException("Faction " + newFaction + "not found for setFaction action!");
      }
      else if (name.StartsWith("loadConditionalActions:"))
        RunnableConditionalActions.LoadIntoOS(name.Substring("loadConditionalActions:".Length), (object) MissionFunctions.os);
      else if (name.Equals("triggerThemeHackRevenge"))
        MissionFunctions.os.delayer.Post(ActionDelayer.Wait(5.0), (Action) (() =>
        {
          string email = MailServer.generateEmail(LocaleTerms.Loc("Are you Kidding me?"), Utils.readEntireFile("Content/LocPost/NaixEmail.txt"), "naix@jmail.com");
          ((MailServer) MissionFunctions.os.netMap.mailServer.getDaemon(typeof (MailServer))).addMail(email, MissionFunctions.os.defaultUser.name);
          MissionFunctions.os.delayer.Post(ActionDelayer.Wait(24.0), (Action) (() =>
          {
            try
            {
              HackerScriptExecuter.runScript("HackerScripts/ThemeHack.txt", (object) MissionFunctions.os, (string) null, (string) null);
            }
            catch (Exception ex)
            {
              if (!Settings.recoverFromErrorsSilently)
                throw ex;
              MissionFunctions.os.write("CAUTION: UNSYNDICATED OUTSIDE CONNECTION ATTEMPT");
              MissionFunctions.os.write("RECOVERED FROM CONNECTION SUBTERFUGE SUCCESSFULLY");
              Console.WriteLine("Critical error loading hacker script - aborting");
            }
          }));
        }));
      else if (name.Equals("changeSong"))
      {
        switch (value)
        {
          case 2:
            MusicManager.transitionToSong("Music\\The_Quickening");
            break;
          case 3:
            MusicManager.transitionToSong("Music\\TheAlgorithm");
            break;
          case 4:
            MusicManager.transitionToSong("Music\\Ryan3");
            break;
          case 5:
            MusicManager.transitionToSong("Music\\Bit(Ending)");
            break;
          case 6:
            MusicManager.transitionToSong("Music\\Rico_Puestel-Roja_Drifts_By");
            break;
          case 7:
            MusicManager.transitionToSong("Music\\out_run_the_wolves");
            break;
          case 8:
            MusicManager.transitionToSong("Music\\Irritations");
            break;
          case 9:
            MusicManager.transitionToSong("Music\\Broken_Boy");
            break;
          case 10:
            MusicManager.transitionToSong("Music\\Ryan10");
            break;
          case 11:
            MusicManager.transitionToSong("Music\\tetrameth");
            break;
          default:
            MusicManager.transitionToSong("Music\\Revolve");
            break;
        }
      }
      else if (name.Equals("entropyEndMissionSetup"))
      {
        MissionFunctions.runCommand(3, "changeSong");
        Computer comp1 = MissionFunctions.findComp("corp0#IS");
        Computer comp2 = MissionFunctions.findComp("corp0#MF");
        Computer comp3 = MissionFunctions.findComp("corp0#BU");
        FileEntry fileEntry1 = new FileEntry(Computer.generateBinaryString(5000), "HacknetOS.rar");
        FileEntry fileEntry2 = new FileEntry(Computer.generateBinaryString(4000), "HacknetOS_Data.xnb");
        FileEntry fileEntry3 = new FileEntry(Computer.generateBinaryString(4000), "HacknetOS_Content.xnb");
        Folder folder1 = comp1.files.root.folders[2];
        folder1.files.Add(fileEntry1);
        folder1.files.Add(fileEntry2);
        folder1.files.Add(fileEntry3);
        Folder folder2 = comp2.files.root.folders[2];
        folder2.files.Add(fileEntry1);
        folder2.files.Add(fileEntry2);
        folder2.files.Add(fileEntry3);
        FileEntry fileEntry4 = new FileEntry(fileEntry1.data, fileEntry1.name + "_backup");
        FileEntry fileEntry5 = new FileEntry(fileEntry2.data, fileEntry2.name + "_backup");
        FileEntry fileEntry6 = new FileEntry(fileEntry3.data, fileEntry3.name + "_backup");
        Folder folder3 = comp3.files.root.folders[2];
        folder3.files.Add(fileEntry4);
        folder3.files.Add(fileEntry5);
        folder3.files.Add(fileEntry6);
        comp1.traceTime = Computer.BASE_TRACE_TIME * 7.5f;
        comp3.traceTime = Computer.BASE_TRACE_TIME * 7.5f;
        comp2.traceTime = Computer.BASE_TRACE_TIME * 7.5f;
        comp2.portsNeededForCrack = 3;
        comp1.portsNeededForCrack = 2;
        comp3.portsNeededForCrack = 2;
        Folder folder4 = MissionFunctions.findComp("entropy01").files.root.folders[2];
        folder4.files.Add(new FileEntry(PortExploits.crackExeData[25], "SMTPoverflow.exe"));
        folder4.files.Add(new FileEntry(PortExploits.crackExeData[80], "WebServerWorm.exe"));
      }
      else if (name.Equals("entropyAddSMTPCrack"))
      {
        Folder folder = MissionFunctions.findComp("entropy01").files.root.folders[2];
        bool flag = false;
        for (int index = 0; index < folder.files.Count; ++index)
        {
          if (folder.files[index].data == PortExploits.crackExeData[25] || folder.files[index].name == "SMTPoverflow.exe")
            flag = true;
        }
        if (!flag)
          folder.files.Add(new FileEntry(PortExploits.crackExeData[25], Utils.GetNonRepeatingFilename("SMTPoverflow", ".exe", folder)));
        MissionFunctions.os.Flags.AddFlag("ThemeHackTransitionAssetsAdded");
      }
      else if (name.Equals("transitionToBitMissions"))
      {
        if (Settings.isDemoMode)
        {
          MissionFunctions.runCommand(6, "changeSong");
          if (Settings.isPressBuildDemo)
            ComputerLoader.loadMission("Content/Missions/Demo/PressBuild/DemoMission01.xml", false);
          else
            ComputerLoader.loadMission("Content/Missions/Demo/AvconDemo.xml", false);
        }
        else
          ComputerLoader.loadMission("Content/Missions/BitMission0.xml", false);
      }
      else if (name.Equals("entropySendCSECInvite"))
        MissionFunctions.os.delayer.Post(ActionDelayer.Wait(6.0), (Action) (() => ComputerLoader.loadMission("Content/Missions/MainHub/Intro/Intro01.xml", false)));
      else if (name.Equals("hubBitSetComplete01"))
      {
        MissionFunctions.os.delayer.Post(ActionDelayer.Wait(4.0), (Action) (() => MissionFunctions.runCommand(1, "addRank")));
        MissionFunctions.runCommand(3, "changeSong");
        MissionFunctions.os.Flags.AddFlag("csecBitSet01Complete");
      }
      else if (name.Equals("enTechEnableOfflineBackup"))
      {
        Programs.getComputer(MissionFunctions.os, "EnTechOfflineBackup");
        MissionFunctions.os.Flags.AddFlag("VaporSequencerEnabled");
        Folder folder1 = MissionFunctions.findComp("mainHubAssets").files.root.searchForFolder("bin");
        Folder folder2 = folder1.searchForFolder("Sequencer");
        if (folder2 == null)
        {
          folder2 = new Folder("Sequencer");
          folder1.folders.Add(folder2);
        }
        if (folder2.searchForFile("Sequencer.exe") != null)
          return;
        folder2.files.Add(new FileEntry(PortExploits.crackExeData[17], "Sequencer.exe"));
      }
      else if (name.Equals("rudeNaixResponse"))
        AchievementsManager.Unlock("rude_response", false);
      else if (name.Equals("assignPlayerToHubServerFaction"))
      {
        MissionFunctions.os.allFactions.setCurrentFaction("hub", MissionFunctions.os);
        Computer computer = Programs.getComputer(MissionFunctions.os, "mainHub");
        MissionHubServer daemon = (MissionHubServer) computer.getDaemon(typeof (MissionHubServer));
        UserDetail userDetail = new UserDetail(MissionFunctions.os.defaultUser.name, "reptile", (byte) 3);
        computer.addNewUser(computer.ip, userDetail);
        daemon.addUser(userDetail);
        MissionFunctions.os.homeNodeID = "mainHub";
        MissionFunctions.os.homeAssetServerID = "mainHubAssets";
        MissionFunctions.runCommand(3, "changeSong");
        MissionFunctions.os.Flags.AddFlag("CSEC_Member");
        AchievementsManager.Unlock("progress_csec", false);
        if (!MissionFunctions.os.HasLoadedDLCContent || !Settings.EnableDLC || MissionFunctions.os.Flags.HasFlag("dlc_complete"))
          return;
        daemon.AddMissionToListings("Content/DLC/Missions/BaseGameConnectors/Missions/CSEC_DLCConnectorIntro.xml", 1);
      }
      else if (name.Equals("assignPlayerToEntropyFaction"))
      {
        MissionFunctions.runCommand(6, "changeSong");
        MissionFunctions.os.homeNodeID = "entropy00";
        MissionFunctions.os.homeAssetServerID = "entropy01";
        AchievementsManager.Unlock("progress_entropy", false);
      }
      else if (name.Equals("assignPlayerToLelzSec"))
      {
        MissionFunctions.os.homeNodeID = "lelzSecHub";
        MissionFunctions.os.homeAssetServerID = "lelzSecHub";
        MissionFunctions.os.Flags.AddFlag("LelzSec_Member");
        AchievementsManager.Unlock("progress_lelz", false);
      }
      else if (name.Equals("lelzSecVictory"))
        AchievementsManager.Unlock("secret_path_complete", false);
      else if (name.Equals("demoFinalMissionEnd"))
      {
        MissionFunctions.os.exes.Clear();
        PostProcessor.EndingSequenceFlashOutActive = true;
        PostProcessor.EndingSequenceFlashOutPercentageComplete = 1f;
        MusicManager.stop();
        MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.2), (Action) (() => MissionFunctions.os.content.Load<SoundEffect>("Music/Ambient/spiral_gauge_down").Play()));
        MissionFunctions.os.delayer.Post(ActionDelayer.Wait(3.0), (Action) (() =>
        {
          PostProcessor.dangerModeEnabled = false;
          PostProcessor.dangerModePercentComplete = 0.0f;
          MissionFunctions.os.ExitScreen();
          MissionFunctions.os.ScreenManager.AddScreen((GameScreen) new DemoEndScreen());
        }));
      }
      else if (name.Equals("demoFinalMissionEndDLC"))
      {
        if (!Settings.isDemoMode)
          return;
        MissionFunctions.os.exes.Clear();
        PostProcessor.EndingSequenceFlashOutActive = true;
        PostProcessor.EndingSequenceFlashOutPercentageComplete = 1f;
        MusicManager.stop();
        MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.0), (Action) (() => MissionFunctions.os.content.Load<SoundEffect>("SFX/BrightFlash").Play()));
        MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.4), (Action) (() => MissionFunctions.os.content.Load<SoundEffect>("SFX/TraceKill").Play()));
        MissionFunctions.os.delayer.Post(ActionDelayer.Wait(1.6), (Action) (() =>
        {
          MusicManager.playSongImmediatley("DLC/Music/DreamHead");
          PostProcessor.dangerModeEnabled = false;
          PostProcessor.dangerModePercentComplete = 0.0f;
          MissionFunctions.os.ScreenManager.AddScreen((GameScreen) new DemoEndScreen()
          {
            StopsMusic = false,
            IsDLCDemoScreen = true
          });
          MissionFunctions.os.ExitScreen();
        }));
      }
      else if (name.Equals("demoFinalMissionStart"))
      {
        MissionFunctions.os.Flags.AddFlag("DemoSequencerEnabled");
        MusicManager.transitionToSong("Music/Ambient/dark_drone_008");
      }
      else if (name.Equals("CSECTesterGameWorldSetup"))
      {
        for (int index = 0; index < PortExploits.services.Count && index < 4; ++index)
          MissionFunctions.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[PortExploits.portNums[index]], PortExploits.cracks[PortExploits.portNums[index]]));
        for (int index = 0; index < 4; ++index)
        {
          Computer c = new Computer("DebugShell" + (object) index, NetworkMap.generateRandomIP(), MissionFunctions.os.netMap.getRandomPosition(), 0, (byte) 2, MissionFunctions.os);
          c.adminIP = MissionFunctions.os.thisComputer.adminIP;
          MissionFunctions.os.netMap.nodes.Add(c);
          MissionFunctions.os.netMap.discoverNode(c);
        }
        MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.2), (Action) (() =>
        {
          MissionFunctions.os.allFactions.setCurrentFaction("entropy", MissionFunctions.os);
          MissionFunctions.os.currentMission = (ActiveMission) null;
          MissionFunctions.os.netMap.discoverNode(Programs.getComputer(MissionFunctions.os, "entropy00"));
          MissionFunctions.os.netMap.discoverNode(Programs.getComputer(MissionFunctions.os, "entropy01"));
        }));
      }
      else if (name.Equals("EntropyFastFowardSetup"))
      {
        MissionFunctions.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[22], PortExploits.cracks[22]));
        MissionFunctions.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[21], PortExploits.cracks[21]));
        for (int index = 0; index < 3; ++index)
        {
          Computer c = new Computer("DebugShell" + (object) index, NetworkMap.generateRandomIP(), MissionFunctions.os.netMap.getRandomPosition(), 0, (byte) 2, MissionFunctions.os);
          c.adminIP = MissionFunctions.os.thisComputer.adminIP;
          MissionFunctions.os.netMap.nodes.Add(c);
          MissionFunctions.os.netMap.discoverNode(c);
        }
        MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.2), (Action) (() =>
        {
          MissionFunctions.os.allFactions.setCurrentFaction("entropy", MissionFunctions.os);
          MissionFunctions.os.currentMission = (ActiveMission) null;
          MissionFunctions.os.netMap.discoverNode(Programs.getComputer(MissionFunctions.os, "entropy00"));
          MissionFunctions.os.netMap.discoverNode(Programs.getComputer(MissionFunctions.os, "entropy01"));
          Computer computer = Programs.getComputer(MissionFunctions.os, "entropy01");
          UserDetail user = computer.users[0];
          user.known = true;
          computer.users[0] = user;
          MissionFunctions.os.allFactions.factions[MissionFunctions.os.allFactions.currentFaction].playerValue = 2;
          MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.2), (Action) (() =>
          {
            MissionFunctions.os.Flags.AddFlag("eosPathStarted");
            ComputerLoader.loadMission("Content/Missions/Entropy/StartingSet/eosMissions/eosIntroDelayer.xml", false);
          }));
        }));
      }
      else if (name.Equals("CSECFastFowardSetup"))
      {
        MissionFunctions.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[22], PortExploits.cracks[22]));
        MissionFunctions.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[21], PortExploits.cracks[21]));
        for (int index = 0; index < 3; ++index)
        {
          Computer c = new Computer("DebugShell" + (object) index, NetworkMap.generateRandomIP(), MissionFunctions.os.netMap.getRandomPosition(), 0, (byte) 2, MissionFunctions.os);
          c.adminIP = MissionFunctions.os.thisComputer.adminIP;
          MissionFunctions.os.netMap.nodes.Add(c);
          MissionFunctions.os.netMap.discoverNode(c);
        }
        MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.2), (Action) (() =>
        {
          MissionFunctions.runCommand(0, "assignPlayerToHubServerFaction");
          MissionFunctions.os.currentMission = (ActiveMission) null;
          MissionFunctions.os.netMap.discoverNode(Programs.getComputer(MissionFunctions.os, "mainHub"));
          MissionFunctions.os.netMap.discoverNode(Programs.getComputer(MissionFunctions.os, "mainHubAssets"));
          Computer computer = Programs.getComputer(MissionFunctions.os, "mainHubAssets");
          UserDetail user = computer.users[0];
          user.known = true;
          computer.users[0] = user;
        }));
      }
      else if (name.Equals("csecAddTraceKill"))
      {
        Folder folder = MissionFunctions.findComp("mainHubAssets").files.root.searchForFolder("bin");
        Folder f = folder.searchForFolder("TK");
        if (f == null)
        {
          f = new Folder("TK");
          folder.folders.Add(f);
        }
        f.files.Add(new FileEntry(FileEncrypter.EncryptString(PortExploits.crackExeData[12], "Vapor Trick Enc.", "NULL", "dx122DX", ".exe"), Utils.GetNonRepeatingFilename("TraceKill", ".dec", f)));
        MissionFunctions.os.Flags.AddFlag("bitPathStarted");
        MissionFunctions.runCommand(10, "changeSong");
      }
      else if (name.Equals("junebugComplete"))
      {
        Computer computer = Programs.getComputer(MissionFunctions.os, "pacemaker01");
        if (computer != null)
        {
          HeartMonitorDaemon daemon = (HeartMonitorDaemon) computer.getDaemon(typeof (HeartMonitorDaemon));
          if (daemon != null)
            daemon.ForceStopBeepSustainSound();
        }
        MissionFunctions.runCommand(1, "addRank");
      }
      else if (name.Equals("eosIntroMissionSetup"))
      {
        MissionFunctions.findComp("entropy01").files.root.searchForFolder("bin").files.Add(new FileEntry(PortExploits.crackExeData[13], "eosDeviceScan.exe"));
        MissionFunctions.os.delayer.Post(ActionDelayer.Wait(8.0), (Action) (() =>
        {
          string email = MailServer.generateEmail("Fwd: eOS Stuff", Utils.readEntireFile("Content/Post/eosScannerMail.txt"), "vtfx", new List<string>((IEnumerable<string>) new string[1]{ "note#%#" + LocaleTerms.Loc("eOS Security Basics") + "#%#" + Utils.readEntireFile("Content/LocPost/eOSNote.txt") }));
          ((MailServer) MissionFunctions.os.netMap.mailServer.getDaemon(typeof (MailServer))).addMail(email, MissionFunctions.os.defaultUser.name);
          MissionFunctions.os.saveGame();
        }));
        MissionFunctions.runCommand(4, "changeSong");
        MissionFunctions.os.saveGame();
      }
      else if (name.Equals("eosIntroEndFunc"))
      {
        MissionFunctions.runCommand(1, "addRank");
        MissionListingServer daemon = (MissionListingServer) MissionFunctions.findComp("entropy00").getDaemon(typeof (MissionListingServer));
        List<ActiveMission> branchMissions = MissionFunctions.os.branchMissions;
        ActiveMission m = (ActiveMission) ComputerLoader.readMission("Content/Missions/Entropy/StartingSet/eosMissions/eosAddedMission.xml");
        daemon.addMisison(m, false);
        MissionFunctions.os.branchMissions = branchMissions;
      }
      else if (name.Equals("changeSongDLC"))
      {
        switch (value)
        {
          case 2:
            MusicManager.transitionToSong("DLC\\Music\\snidelyWhiplash");
            break;
          case 3:
            MusicManager.transitionToSong("DLC\\Music\\Slow_Motion");
            break;
          case 4:
            MusicManager.transitionToSong("DLC\\Music\\World_Chase");
            break;
          case 5:
            MusicManager.transitionToSong("DLC\\Music\\HOME_Resonance");
            break;
          case 6:
            MusicManager.transitionToSong("DLC\\Music\\Remi_Finale");
            break;
          case 7:
            MusicManager.transitionToSong("DLC\\Music\\RemiDrone");
            break;
          case 8:
            MusicManager.transitionToSong("DLC\\Music\\DreamHead");
            break;
          case 9:
            MusicManager.transitionToSong("DLC\\Music\\Userspacelike");
            break;
          case 10:
            MusicManager.transitionToSong("DLC\\Music\\CrashTrack");
            break;
          default:
            MusicManager.transitionToSong("DLC\\Music\\Remi2");
            break;
        }
      }
      else if (name.Equals("scanAndStartDLCVenganceHack"))
      {
        Computer comp = MissionFunctions.findComp("dAttackTarget");
        if (comp == null)
          return;
        Folder folder = comp.files.root.searchForFolder("log");
        bool flag = false;
        for (int index = 0; index < folder.files.Count; ++index)
        {
          if (folder.files[index].data.Contains(MissionFunctions.os.thisComputer.ip))
          {
            SARunFunction saRunFunction = new SARunFunction() { DelayHost = "dhs", FunctionName = "triggerDLCHackRevenge", FunctionValue = 1 };
            ((DLCHubServer) Programs.getComputer(MissionFunctions.os, "dhs").getDaemon(typeof (DLCHubServer))).DelayedActions.AddAction((SerializableAction) saRunFunction, 16f);
            break;
          }
        }
        if (!flag)
          MissionFunctions.runCommand(4, "changeSongDLC");
      }
      else if (name.Equals("triggerDLCHackRevenge"))
      {
        try
        {
          HackerScriptExecuter.runScript("DLC/ActionScripts/Hackers/SystemHack.txt", (object) MissionFunctions.os, (string) null, (string) null);
        }
        catch (Exception ex)
        {
          if (!Settings.recoverFromErrorsSilently)
            throw ex;
          MissionFunctions.os.write("CAUTION: UNSYNDICATED OUTSIDE CONNECTION ATTEMPT");
          MissionFunctions.os.write("RECOVERED FROM CONNECTION SUBTERFUGE SUCCESSFULLY");
          Console.WriteLine("Critical error loading hacker script - aborting\r\n" + Utils.GenerateReportFromException(ex));
        }
      }
      else if (name.Equals("activateAircraftStatusOverlay"))
      {
        MissionFunctions.os.AircraftInfoOverlay.Activate();
        MissionFunctions.os.AircraftInfoOverlay.IsMonitoringDLCEndingCases = true;
      }
      else if (name.Equals("activateAircraftStatusOverlayLabyrinthsMonitoring"))
        MissionFunctions.os.AircraftInfoOverlay.IsMonitoringDLCEndingCases = true;
      else if (name.Equals("deActivateAircraftStatusOverlay"))
      {
        MissionFunctions.os.AircraftInfoOverlay.IsActive = false;
        MissionFunctions.os.AircraftInfoOverlay.IsMonitoringDLCEndingCases = false;
        MissionFunctions.os.Flags.AddFlag("AircraftInfoOverlayDeactivated");
      }
      else if (name.Equals("defAttackAircraft"))
      {
        Computer computer = Programs.getComputer(MissionFunctions.os, "dair_crash");
        Folder folder = computer.files.root.searchForFolder("FlightSystems");
        for (int index = 0; index < folder.files.Count; ++index)
        {
          if (folder.files[index].name == "747FlightOps.dll")
          {
            folder.files.RemoveAt(index);
            break;
          }
        }
        ((AircraftDaemon) computer.getDaemon(typeof (AircraftDaemon))).StartReloadFirmware();
        if (MissionFunctions.os.AircraftInfoOverlay.IsActive)
          return;
        MissionFunctions.os.AircraftInfoOverlay.Activate();
        MissionFunctions.os.AircraftInfoOverlay.IsMonitoringDLCEndingCases = true;
      }
      else if (name.Equals("playAirlineCrashSongSequence"))
      {
        MusicManager.playSongImmediatley("DLC\\Music\\Remi_Finale");
        MediaPlayer.IsRepeating = false;
      }
      else if (name.Equals("flashUI"))
        MissionFunctions.os.warningFlash();
      else if (name.Equals("addRankSilent"))
        MissionFunctions.os.currentFaction.addValue(value, (object) MissionFunctions.os);
      else if (name.StartsWith("addRankFaction:"))
      {
        string str = name.Substring("addRankFaction:".Length);
        foreach (KeyValuePair<string, Faction> faction in MissionFunctions.os.allFactions.factions)
        {
          if (faction.Value.idName.ToLower() == str.ToLower())
          {
            faction.Value.addValue(value, (object) MissionFunctions.os);
            break;
          }
        }
      }
      else if (name.StartsWith("setHubServer:"))
      {
        string str = name.Substring("setHubServer:".Length);
        MissionFunctions.os.homeNodeID = str;
      }
      else if (name.StartsWith("setAssetServer:"))
      {
        string str = name.Substring("setAssetServer:".Length);
        MissionFunctions.os.homeAssetServerID = str;
      }
      else if (name.StartsWith("playCustomSong:"))
      {
        string songName = Utils.GetFileLoadPrefix() + name.Substring("playCustomSong:".Length);
        if (songName.EndsWith(".ogg"))
          songName = songName.Substring(0, songName.Length - ".ogg".Length);
        if (songName.StartsWith("Content"))
          songName = songName.Substring("Content/".Length);
        else if (songName.StartsWith("Extensions"))
          songName = "../" + songName;
        MusicManager.transitionToSong(songName);
      }
      else if (name.StartsWith("playCustomSongImmediatley:"))
      {
        string songname = Utils.GetFileLoadPrefix() + name.Substring("playCustomSongImmediatley:".Length);
        if (songname.EndsWith(".ogg"))
          songname = songname.Substring(0, songname.Length - ".ogg".Length);
        if (songname.StartsWith("Content"))
          songname = songname.Substring("Content/".Length);
        else if (songname.StartsWith("Extensions"))
          songname = "../" + songname;
        MusicManager.playSongImmediatley(songname);
      }
      else
      {
        if (OS.TestingPassOnly && !string.IsNullOrWhiteSpace(name))
          throw new FormatException("No Command Function " + name);
        if (MissionFunctions.ReportErrorInCommand != null)
          MissionFunctions.ReportErrorInCommand("No command found for \"" + name + "\" with value \"" + (object) value + "\"");
      }
    }

    private static void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
    {
      throw new NotImplementedException();
    }

    private static Computer findComp(string target)
    {
      for (int index = 0; index < MissionFunctions.os.netMap.nodes.Count; ++index)
      {
        if (MissionFunctions.os.netMap.nodes[index].idName.Equals(target))
          return MissionFunctions.os.netMap.nodes[index];
      }
      return (Computer) null;
    }
  }
}
