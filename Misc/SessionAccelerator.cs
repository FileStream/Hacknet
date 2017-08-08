// Decompiled with JetBrains decompiler
// Type: Hacknet.Misc.SessionAccelerator
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet.Misc
{
  public static class SessionAccelerator
  {
    public static void AccelerateSessionToDLCHA(object osObj)
    {
      OS os = (OS) osObj;
      os.Flags.AddFlag("TutorialComplete");
      os.delayer.RunAllDelayedActions();
      os.allFactions.setCurrentFaction("Bibliotheque", os);
      os.IsInDLCMode = true;
      ThemeManager.setThemeOnComputer((object) os.thisComputer, "DLC/Themes/RiptideThemeStandard.xml");
      ThemeManager.switchTheme((object) os, "DLC/Themes/RiptideThemeStandard.xml");
      os.netMap.discoverNode(Programs.getComputer(os, "dhs"));
      MissionFunctions.runCommand(0, "setFaction:Bibliotheque");
      DLCHubServer daemon = (DLCHubServer) Programs.getComputer(os, "dhs").getDaemon(typeof (DLCHubServer));
      int num = 11;
      for (int index = 0; index < num; ++index)
      {
        MissionFunctions.runCommand(1, "addRankSilent");
        if (index + 1 < num)
        {
          os.delayer.RunAllDelayedActions();
          daemon.DelayedActions.InstantlyResolveAllActions((object) os);
          daemon.ClearAllActiveMissions();
        }
      }
      SessionAccelerator.AddProgramToComputer(os.thisComputer, 6881);
      SessionAccelerator.AddProgramToComputer(os.thisComputer, 211);
      SessionAccelerator.AddProgramToComputer(os.thisComputer, 31);
      SessionAccelerator.AddProgramToComputer(os.thisComputer, 443);
      MissionFunctions.runCommand(9, "changeSongDLC");
    }

    public static void AccelerateSessionToDLCEND(object osObj)
    {
      OS os = (OS) osObj;
      os.Flags.AddFlag("TutorialComplete");
      os.delayer.RunAllDelayedActions();
      os.allFactions.setCurrentFaction("Bibliotheque", os);
      os.IsInDLCMode = true;
      ThemeManager.setThemeOnComputer((object) os.thisComputer, "DLC/Themes/RiptideThemeStandard.xml");
      ThemeManager.switchTheme((object) os, "DLC/Themes/RiptideThemeStandard.xml");
      os.netMap.discoverNode(Programs.getComputer(os, "dhs"));
      MissionFunctions.runCommand(0, "setFaction:Bibliotheque");
      DLCHubServer daemon = (DLCHubServer) Programs.getComputer(os, "dhs").getDaemon(typeof (DLCHubServer));
      int num = 11;
      for (int index = 0; index < num; ++index)
      {
        MissionFunctions.runCommand(1, "addRankSilent");
        if (index + 1 < num)
        {
          os.delayer.RunAllDelayedActions();
          daemon.DelayedActions.InstantlyResolveAllActions((object) os);
          daemon.ClearAllActiveMissions();
        }
      }
      SessionAccelerator.AddProgramToComputer(os.thisComputer, 6881);
      SessionAccelerator.AddProgramToComputer(os.thisComputer, 211);
      SessionAccelerator.AddProgramToComputer(os.thisComputer, 31);
      SessionAccelerator.AddProgramToComputer(os.thisComputer, 443);
    }

    public static void AccelerateSessionToDLCStart(object osObj)
    {
      OS os = (OS) osObj;
      os.Flags.AddFlag("TutorialComplete");
      os.delayer.RunAllDelayedActions();
      ThemeManager.setThemeOnComputer((object) os.thisComputer, "DLC/Themes/RiptideClassicTheme.xml");
      ThemeManager.switchTheme((object) os, "DLC/Themes/RiptideClassicTheme.xml");
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
      os.delayer.Post(ActionDelayer.Wait(0.15), (Action) (() =>
      {
        Game1.getSingleton().IsMouseVisible = true;
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[22], "SSHCrack.exe"));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[21], "FTPBounce.exe"));
        os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[13], "eosDeviceScan.exe"));
        MissionFunctions.runCommand(7, "changeSong");
        MusicManager.stop();
      }));
      os.delayer.Post(ActionDelayer.Wait(56.0), (Action) (() => ComputerLoader.loadMission("Content/DLC/Missions/Demo/DLCDemoIntroMission1.xml", false)));
    }

    internal static void AddProgramToComputer(Computer c, int portnum)
    {
      FileEntry fileEntry = new FileEntry(PortExploits.crackExeData[portnum], PortExploits.cracks[portnum]);
      c.files.root.searchForFolder("bin").files.Add(fileEntry);
    }
  }
}
