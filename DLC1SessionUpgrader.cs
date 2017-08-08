// Decompiled with JetBrains decompiler
// Type: Hacknet.DLC1SessionUpgrader
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Factions;
using Hacknet.Mission;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hacknet
{
  internal class DLC1SessionUpgrader
  {
    public static bool HasDLC1Installed = false;

    public static void CheckForDLCFiles()
    {
      if (File.Exists("Content/DLC/DLCFaction.xml"))
        DLC1SessionUpgrader.HasDLC1Installed = true;
      else
        DLC1SessionUpgrader.HasDLC1Installed = false;
    }

    public static void UpgradeSession(object osobj, bool needsNodeInjection)
    {
      OS os = (OS) osobj;
      MemoryDumpInjector.InjectMemory(LocalizedFileLoader.GetLocalizedFilepath("Content/DLC/Missions/Injections/MemoryDumps/GibsonLink.xml"), (object) Programs.getComputer(os, "polarSnakeDest"));
      MemoryDumpInjector.InjectMemory(LocalizedFileLoader.GetLocalizedFilepath("Content/DLC/Missions/Injections/MemoryDumps/NaixHome.xml"), (object) Programs.getComputer(os, "naixGateway"));
      MemoryDumpInjector.InjectMemory(LocalizedFileLoader.GetLocalizedFilepath("Content/DLC/Missions/Injections/MemoryDumps/BitDropBox.xml"), (object) Programs.getComputer(os, "BitWorkServer"));
      MemoryDumpInjector.InjectMemory(LocalizedFileLoader.GetLocalizedFilepath("Content/DLC/Missions/Injections/MemoryDumps/ExpandKeysInjection.xml"), (object) Programs.getComputer(os, "portcrack01"));
      if (os.thisComputer.Memory == null)
        os.thisComputer.Memory = new MemoryContents();
      os.allFactions.factions.Add("Bibliotheque", (Faction) CustomFaction.ParseFromFile("Content/DLC/DLCFaction.xml"));
      if (needsNodeInjection)
      {
        if (People.all == null)
          People.init();
        else if (!People.PeopleWereGeneratedWithDLCAdditions)
          People.LoadInDLCPeople();
        List<string> dlcList = BootLoadList.getDLCList();
        for (int index = 0; index < dlcList.Count; ++index)
          Computer.loadFromFile(dlcList[index]);
        ComputerLoader.postAllLoadedActions();
      }
      Computer computer = Programs.getComputer(os, "ispComp");
      if (!computer.ports.Contains(443))
        computer.ports.Add(443);
      if (computer.ports.Contains(6881))
        return;
      computer.ports.Add(6881);
    }

    public static void EndDLCSection(object osobj)
    {
      OS os = (OS) osobj;
      os.IsInDLCMode = false;
      os.mailicon.isEnabled = true;
      os.Flags.AddFlag("dlc_complete");
      if (os.Flags.HasFlag("dlc_start_csec"))
      {
        os.Flags.AddFlag("dlc_complete_FromCSEC");
        ComputerLoader.loadMission("Content/DLC/Missions/BaseGameConnectors/Missions/CSEC_DLC_EndEmail.xml", false);
        os.allFactions.setCurrentFaction("hub", os);
        os.homeNodeID = "mainHub";
        os.homeAssetServerID = "mainHubAssets";
        os.Flags.AddFlag("dlc_csec_end_facval:" + (object) os.currentFaction.playerValue);
      }
      else if (os.Flags.HasFlag("dlc_start_entropy"))
      {
        os.Flags.AddFlag("dlc_complete_FromEntropy");
        ComputerLoader.loadMission("Content/DLC/Missions/BaseGameConnectors/Missions/Entropy_DLC_EndEmail.xml", false);
        os.allFactions.setCurrentFaction("entropy", os);
        os.homeNodeID = "entropy00";
        os.homeAssetServerID = "entropy01";
      }
      else
      {
        os.Flags.AddFlag("dlc_complete_FromUnknown");
        ComputerLoader.loadMission("Content/DLC/Missions/BaseGameConnectors/Missions/Entropy_DLC_EndEmail.xml", false);
        os.allFactions.setCurrentFaction("entropy", os);
        os.homeNodeID = "entropy00";
        os.homeAssetServerID = "entropy01";
      }
      DLC1SessionUpgrader.ReDsicoverAllVisibleNodesInOSCache((object) os);
    }

    public static void ReDsicoverAllVisibleNodesInOSCache(object osobj)
    {
      OS os = (OS) osobj;
      float num1 = 0.0f;
      float num2 = 8f;
      float num3 = num2 / (float) os.netMap.nodes.Count;
      string[] strArray = os.PreDLCVisibleNodesCache.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray.Length; ++index)
      {
        try
        {
          int nIndex = Convert.ToInt32(strArray[index]);
          os.delayer.Post(ActionDelayer.Wait((double) num1), (Action) (() =>
          {
            os.netMap.visibleNodes.Add(nIndex);
            os.netMap.nodes[nIndex].highlightFlashTime = 1f;
            SFX.addCircle(os.netMap.nodes[nIndex].getScreenSpacePosition(), Utils.AddativeWhite * 0.4f, 70f);
          }));
          os.delayer.Post(ActionDelayer.Wait((double) num2), (Action) (() => SFX.addCircle(os.netMap.nodes[nIndex].getScreenSpacePosition(), Utils.AddativeWhite * 0.3f, 30f)));
        }
        catch (FormatException ex)
        {
          Console.WriteLine("Error restoring node " + (object) index);
        }
        num1 += num3;
        os.PreDLCVisibleNodesCache = "";
      }
    }
  }
}
