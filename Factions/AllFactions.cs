// Decompiled with JetBrains decompiler
// Type: Hacknet.Factions.AllFactions
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;
using System.Xml;

namespace Hacknet.Factions
{
  internal class AllFactions
  {
    public Dictionary<string, Faction> factions = new Dictionary<string, Faction>();
    public string currentFaction;

    public AllFactions()
    {
      this.currentFaction = (string) null;
    }

    public void init()
    {
      if (Settings.IsInExtensionMode)
        return;
      this.currentFaction = "entropy";
      Dictionary<string, Faction> factions1 = this.factions;
      string key1 = "entropy";
      EntropyFaction entropyFaction1 = new EntropyFaction("Entropy", 5);
      entropyFaction1.idName = "entropy";
      EntropyFaction entropyFaction2 = entropyFaction1;
      factions1.Add(key1, (Faction) entropyFaction2);
      this.factions.Add("lelzSec", new Faction("lelzSec", 1000)
      {
        idName = "lelzSec"
      });
      Dictionary<string, Faction> factions2 = this.factions;
      string key2 = "hub";
      HubFaction hubFaction1 = new HubFaction("CSEC", 10);
      hubFaction1.idName = "hub";
      HubFaction hubFaction2 = hubFaction1;
      factions2.Add(key2, (Faction) hubFaction2);
    }

    public string getSaveString()
    {
      string str = "<AllFactions current=\"" + this.currentFaction + "\">\n";
      foreach (KeyValuePair<string, Faction> faction in this.factions)
        str = str + "\t" + faction.Value.getSaveString();
      return str + "</AllFactions>";
    }

    public void setCurrentFaction(string newFaction, OS os)
    {
      this.currentFaction = newFaction;
      os.currentFaction = this.factions[this.currentFaction];
    }

    public static AllFactions loadFromSave(XmlReader xmlRdr)
    {
      AllFactions allFactions = new AllFactions();
      while (xmlRdr.Name != "AllFactions")
        xmlRdr.Read();
      if (xmlRdr.MoveToAttribute("current"))
        allFactions.currentFaction = xmlRdr.ReadContentAsString();
      do
      {
        xmlRdr.Read();
      }
      while (string.IsNullOrWhiteSpace(xmlRdr.Name));
      while (!(xmlRdr.Name == "AllFactions") || xmlRdr.IsStartElement())
      {
        Faction faction = Faction.loadFromSave(xmlRdr);
        allFactions.factions.Add(faction.idName, faction);
        xmlRdr.Read();
      }
      return allFactions;
    }
  }
}
