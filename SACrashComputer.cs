// Decompiled with JetBrains decompiler
// Type: Hacknet.SACrashComputer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SACrashComputer : SerializableAction
  {
    public string TargetComp;
    public string CrashSource;
    public string DelayHost;
    public float Delay;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      Computer computer1 = Programs.getComputer(os, this.TargetComp);
      if (computer1 == null && !OS.DEBUG_COMMANDS)
        return;
      if ((double) this.Delay <= 0.0)
      {
        computer1.crash(this.CrashSource);
      }
      else
      {
        Computer computer2 = Programs.getComputer(os, this.DelayHost);
        if (computer2 == null)
          throw new NullReferenceException("Computer " + (object) computer2 + " could not be found as DelayHost for Function");
        float delay = this.Delay;
        this.Delay = -1f;
        DelayableActionSystem.FindDelayableActionSystemOnComputer(computer2).AddAction((SerializableAction) this, delay);
      }
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SACrashComputer saCrashComputer = new SACrashComputer();
      if (rdr.MoveToAttribute("Delay"))
        saCrashComputer.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("TargetComp"))
        saCrashComputer.TargetComp = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("CrashSource"))
        saCrashComputer.CrashSource = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("DelayHost"))
        saCrashComputer.DelayHost = rdr.ReadContentAsString();
      return (SerializableAction) saCrashComputer;
    }
  }
}
