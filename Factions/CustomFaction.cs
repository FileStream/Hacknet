// Decompiled with JetBrains decompiler
// Type: Hacknet.Factions.CustomFaction
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Hacknet.Factions
{
  public class CustomFaction : Faction
  {
    internal List<CustomFactionAction> CustomActions = new List<CustomFactionAction>();

    public CustomFaction(string _name, int _neededValue)
      : base(_name, _neededValue)
    {
      this.PlayerLosesValueOnAbandon = false;
    }

    public static CustomFaction ParseFromFile(string filepath)
    {
      CustomFaction customFaction;
      using (FileStream fileStream = File.OpenRead(LocalizedFileLoader.GetLocalizedFilepath(filepath)))
        customFaction = Faction.loadFromSave(XmlReader.Create((Stream) fileStream)) as CustomFaction;
      return customFaction;
    }

    public void CheckForAllCustomActionsToRun(object os_obj)
    {
      OS os = (OS) os_obj;
      for (int index = 0; index < this.CustomActions.Count; ++index)
      {
        if (this.playerValue >= this.CustomActions[index].ValueRequiredForTrigger)
        {
          bool flag1 = true;
          if (!string.IsNullOrWhiteSpace(this.CustomActions[index].FlagsRequiredForTrigger))
          {
            foreach (string flag2 in this.CustomActions[index].FlagsRequiredForTrigger.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries))
            {
              if (!os.Flags.HasFlag(flag2))
                flag1 = false;
            }
          }
          if (flag1)
          {
            CustomFactionAction customAction = this.CustomActions[index];
            this.CustomActions.RemoveAt(index);
            --index;
            customAction.Trigger((object) os);
          }
        }
      }
    }

    public override void addValue(int value, object os_obj)
    {
      int playerValue = this.playerValue;
      base.addValue(value, os_obj);
      this.CheckForAllCustomActionsToRun(os_obj);
    }

    private void SendNotification(object osIn, string body, string subject)
    {
    }

    public override string getSaveString()
    {
      string saveString = base.getSaveString();
      string str = ">\n";
      for (int index = 0; index < this.CustomActions.Count; ++index)
        str = str + this.CustomActions[index].GetSaveString() + "\n";
      string newValue = str + "</CustomFaction>";
      return "\r\n" + saveString.Replace("/>", newValue);
    }

    public static CustomFaction DeserializeFromXmlReader(XmlReader rdr, string name, string id, int playerVal, bool playerHasPassed)
    {
      CustomFaction customFaction = new CustomFaction(name, 100);
      rdr.MoveToElement();
      while (!rdr.EOF)
      {
        if (rdr.Name != "CustomFaction")
        {
          customFaction.CustomActions.Add(CustomFactionAction.Deserialize(rdr));
          rdr.Read();
        }
        else if (!(rdr.Name == "CustomFaction") || rdr.IsStartElement())
          rdr.Read();
        else
          break;
        while (!rdr.EOF && string.IsNullOrWhiteSpace(rdr.Name))
          rdr.Read();
      }
      return customFaction;
    }
  }
}
