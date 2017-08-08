// Decompiled with JetBrains decompiler
// Type: Hacknet.SCOnDisconnect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Xml;

namespace Hacknet
{
  public class SCOnDisconnect : SerializableCondition
  {
    private bool hasHadFrameWhereThisWasFalse = false;
    public string target;

    public override bool Check(object os_obj)
    {
      if (!this.hasHadFrameWhereThisWasFalse)
      {
        this.hasHadFrameWhereThisWasFalse = true;
        return false;
      }
      bool flag = ((OS) os_obj).connectedComp == null || ((OS) os_obj).connectedComp == ((OS) os_obj).thisComputer;
      if (string.IsNullOrWhiteSpace(this.target) || this.target.ToLower() == "none")
        return flag;
      OS os = (OS) os_obj;
      Computer computer = Programs.getComputer(os, this.target);
      if (computer == null)
        return flag;
      return os.connectedIPLastFrame == computer.ip && (flag || os.connectedComp != null && os.connectedComp.ip != computer.ip);
    }

    public static SerializableCondition DeserializeFromReader(XmlReader rdr)
    {
      SCOnDisconnect scOnDisconnect = new SCOnDisconnect();
      if (rdr.MoveToAttribute("target"))
        scOnDisconnect.target = rdr.ReadContentAsString();
      return (SerializableCondition) scOnDisconnect;
    }
  }
}
