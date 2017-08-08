// Decompiled with JetBrains decompiler
// Type: Hacknet.SAChangeIP
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SAChangeIP : SerializableAction
  {
    public string TargetComp;
    public string NewIP;
    public string DelayHost;
    public float Delay;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      if ((double) this.Delay <= 0.0)
      {
        Computer computer = Programs.getComputer(os, this.TargetComp);
        if (computer == null)
          return;
        if (string.IsNullOrWhiteSpace(this.NewIP) || this.NewIP.StartsWith("#RANDOM"))
          this.NewIP = NetworkMap.generateRandomIP();
        computer.ip = this.NewIP;
      }
      else
      {
        Computer computer = Programs.getComputer(os, this.DelayHost);
        if (computer == null)
          throw new NullReferenceException("Computer " + (object) computer + " could not be found as DelayHost for Function");
        float delay = this.Delay;
        this.Delay = -1f;
        DelayableActionSystem.FindDelayableActionSystemOnComputer(computer).AddAction((SerializableAction) this, delay);
      }
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SAChangeIP saChangeIp = new SAChangeIP();
      if (rdr.MoveToAttribute("Delay"))
        saChangeIp.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        saChangeIp.DelayHost = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("TargetComp"))
        saChangeIp.TargetComp = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("NewIP"))
        saChangeIp.NewIP = rdr.ReadContentAsString();
      return (SerializableAction) saChangeIp;
    }
  }
}
