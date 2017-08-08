// Decompiled with JetBrains decompiler
// Type: Hacknet.SCOnConnect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Xml;

namespace Hacknet
{
  public class SCOnConnect : SerializableCondition
  {
    public string target;
    public bool needsMissionComplete;
    public string requiredFlags;

    public override bool Check(object os_obj)
    {
      OS os = (OS) os_obj;
      Computer computer = Programs.getComputer(os, this.target);
      if (computer == null)
        return false;
      if (!string.IsNullOrWhiteSpace(this.requiredFlags))
      {
        foreach (string flag in this.requiredFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries))
        {
          if (!os.Flags.HasFlag(flag))
            return false;
        }
      }
      return (!this.needsMissionComplete || os.currentMission != null && os.currentMission.isComplete((List<string>) null)) && (os.connectedComp != null && os.connectedComp.ip == computer.ip);
    }

    public static SerializableCondition DeserializeFromReader(XmlReader rdr)
    {
      SCOnConnect scOnConnect = new SCOnConnect();
      if (rdr.MoveToAttribute("target"))
        scOnConnect.target = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("needsMissionComplete"))
        scOnConnect.needsMissionComplete = rdr.ReadContentAsString().ToLower() == "true";
      if (rdr.MoveToAttribute("requiredFlags"))
        scOnConnect.requiredFlags = rdr.ReadContentAsString();
      if (scOnConnect.target == null)
        throw new FormatException("Target computer not specified in OnConnect condition");
      return (SerializableCondition) scOnConnect;
    }
  }
}
