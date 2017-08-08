// Decompiled with JetBrains decompiler
// Type: Hacknet.SCOnAdminGained
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SCOnAdminGained : SerializableCondition
  {
    public string target;

    public override bool Check(object os_obj)
    {
      OS os = (OS) os_obj;
      Computer computer = Programs.getComputer(os, this.target);
      if (computer == null)
        return false;
      return computer.adminIP == os.thisComputer.ip;
    }

    public static SerializableCondition DeserializeFromReader(XmlReader rdr)
    {
      SCOnAdminGained scOnAdminGained = new SCOnAdminGained();
      if (rdr.MoveToAttribute("target"))
        scOnAdminGained.target = rdr.ReadContentAsString();
      if (scOnAdminGained.target == null)
        throw new FormatException("Target computer not specified in OnAdminGained condition");
      return (SerializableCondition) scOnAdminGained;
    }
  }
}
