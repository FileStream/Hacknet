// Decompiled with JetBrains decompiler
// Type: Hacknet.Factions.EntropyFaction
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet.Factions
{
  internal class EntropyFaction : Faction
  {
    public EntropyFaction(string _name, int _neededValue)
      : base(_name, _neededValue)
    {
    }

    public override void addValue(int value, object os)
    {
      int playerValue = this.playerValue;
      base.addValue(value, os);
      if (this.valuePassedPoint(playerValue, 3))
      {
        ((OS) os).Flags.AddFlag("eosPathStarted");
        ComputerLoader.loadMission("Content/Missions/Entropy/StartingSet/eosMissions/eosIntroDelayer.xml", false);
      }
      if (!this.valuePassedPoint(playerValue, 4))
        return;
      if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed && ((OS) os).HasLoadedDLCContent)
      {
        try
        {
          Computer computer = Programs.getComputer((OS) os, "entropy00");
          if (computer != null)
            ;
          ((MissionListingServer) computer.getDaemon(typeof (MissionListingServer))).addMisison((ActiveMission) ComputerLoader.readMission("Content/DLC/Missions/BaseGameConnectors/Missions/EntropyDLCConnectorIntro.xml"), true);
          Console.WriteLine("Injected Labyrinths transition mission to Entropy");
        }
        catch (Exception ex)
        {
          Utils.AppendToErrorFile("Could not add in Labyrinths upgrade mission to entropy!\r\n\r\n" + Utils.GenerateReportFromException(ex));
        }
      }
    }

    public override void playerPassedValue(object os)
    {
      base.playerPassedValue(os);
      if (Settings.isAlphaDemoMode)
        ComputerLoader.loadMission("Content/Missions/Entropy/EntropyMission3.xml", false);
      else
        ((OS) os).delayer.Post(ActionDelayer.Wait(1.7), (Action) (() =>
        {
          ComputerLoader.loadMission("Content/Missions/Entropy/ThemeHackTransitionMission.xml", false);
          ((OS) os).saveGame();
        }));
    }
  }
}
