// Decompiled with JetBrains decompiler
// Type: Hacknet.SAShowNode
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SAShowNode : SerializableAction
  {
    public string DelayHost;
    public float Delay;
    public string Target;

    public override void Trigger(object os_obj)
    {
      if ((double) this.Delay <= 0.0)
      {
        OS os = (OS) os_obj;
        Computer computer = Programs.getComputer(os, this.Target);
        if (computer != null)
          os.netMap.discoverNode(computer);
        else if (OS.DEBUG_COMMANDS)
          os.write("Error revealing node " + this.Target + " : NODE NOT FOUND");
      }
      else
      {
        Computer computer = Programs.getComputer((OS) os_obj, this.DelayHost);
        if (computer == null)
          throw new NullReferenceException("Computer " + (object) computer + " could not be found as DelayHost for Function");
        float delay = this.Delay;
        this.Delay = -1f;
        DelayableActionSystem.FindDelayableActionSystemOnComputer(computer).AddAction((SerializableAction) this, delay);
      }
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SAShowNode saShowNode = new SAShowNode();
      if (rdr.MoveToAttribute("Delay"))
        saShowNode.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        saShowNode.DelayHost = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("Target"))
        saShowNode.Target = rdr.ReadContentAsString();
      return (SerializableAction) saShowNode;
    }
  }
}
