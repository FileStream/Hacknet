// Decompiled with JetBrains decompiler
// Type: Hacknet.Faction
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Factions;
using System;
using System.Xml;

namespace Hacknet
{
  public class Faction
  {
    public string name = "unknown";
    public string idName = "";
    public bool playerHasPassedValue = false;
    public bool PlayerLosesValueOnAbandon = false;
    public int playerValue;
    public int neededValue;

    public Faction(string _name, int _neededValue)
    {
      this.playerValue = 0;
      this.neededValue = _neededValue;
      this.name = _name;
    }

    public virtual void addValue(int value, object os)
    {
      int playerValue = this.playerValue;
      this.playerValue += value;
      if (this.playerValue < this.neededValue || this.playerHasPassedValue)
        return;
      this.playerPassedValue(os);
    }

    public void contractAbbandoned(object osIn)
    {
      OS os = (OS) osIn;
      if (this.PlayerLosesValueOnAbandon)
      {
        this.playerValue -= 10;
        if (this.playerValue < 0)
          this.playerValue = 0;
      }
      string email = MailServer.generateEmail(LocaleTerms.Loc("Contract Abandoned"), string.Format(Utils.readEntireFile("Content/LocPost/ContractAbandonedEmail.txt"), (object) this.getRank(), (object) this.getMaxRank(), (object) this.name), this.name + " ReplyBot");
      ((MailServer) os.netMap.mailServer.getDaemon(typeof (MailServer))).addMail(email, os.defaultUser.name);
    }

    public int getRank()
    {
      return Math.Max(1, Math.Abs((int) ((1.0 - (double) Math.Min((float) this.playerValue, (float) this.neededValue) / (double) this.neededValue) * (double) this.getMaxRank())));
    }

    public virtual string getSaveString()
    {
      string str = "Faction";
      if (this is EntropyFaction)
        str = "EntropyFaction";
      if (this is HubFaction)
        str = "HubFaction";
      if (this is CustomFaction)
        str = "CustomFaction";
      return "<" + str + " name=\"" + this.name + "\" id=\"" + this.idName + "\" neededVal=\"" + (object) this.neededValue + "\" playerVal=\"" + (object) this.playerValue + "\" playerHasPassed=\"" + (object) this.playerHasPassedValue + "\" />";
    }

    public int getMaxRank()
    {
      return 100;
    }

    public virtual void playerPassedValue(object os)
    {
      this.playerHasPassedValue = true;
    }

    public static Faction loadFromSave(XmlReader xmlRdr)
    {
      string str = "UNKNOWN";
      string id = "";
      bool playerHasPassed = false;
      int _neededValue = 100;
      int playerVal = 0;
      while (xmlRdr.Name != "Faction" && xmlRdr.Name != "CustomFaction" && xmlRdr.Name != "EntropyFaction" && xmlRdr.Name != "HubFaction")
        xmlRdr.Read();
      string name = xmlRdr.Name;
      if (xmlRdr.MoveToAttribute("name"))
        str = xmlRdr.ReadContentAsString();
      if (xmlRdr.MoveToAttribute("id"))
        id = xmlRdr.ReadContentAsString();
      if (xmlRdr.MoveToAttribute("neededVal"))
        _neededValue = xmlRdr.ReadContentAsInt();
      if (xmlRdr.MoveToAttribute("playerVal"))
        playerVal = xmlRdr.ReadContentAsInt();
      if (xmlRdr.MoveToAttribute("playerHasPassed"))
        playerHasPassed = xmlRdr.ReadContentAsString().ToLower() == "true";
      Faction faction;
      switch (name)
      {
        case "HubFaction":
          faction = (Faction) new HubFaction(str, _neededValue);
          break;
        case "EntropyFaction":
          faction = (Faction) new EntropyFaction(str, _neededValue);
          break;
        case "CustomFaction":
          faction = (Faction) CustomFaction.DeserializeFromXmlReader(xmlRdr, str, id, playerVal, playerHasPassed);
          break;
        default:
          faction = new Faction(str, _neededValue);
          break;
      }
      faction.playerValue = playerVal;
      faction.idName = id;
      faction.playerHasPassedValue = playerHasPassed;
      return faction;
    }

    public bool valuePassedPoint(int oldValue, int neededValue)
    {
      return this.playerValue >= neededValue && oldValue < neededValue;
    }
  }
}
