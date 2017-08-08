// Decompiled with JetBrains decompiler
// Type: Hacknet.SAKillExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SAKillExe : SerializableAction
  {
    public string ExeName;
    public string DelayHost;
    public float Delay;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      if ((double) this.Delay <= 0.0)
      {
        if (this.ExeName == "*")
          this.ExeName = "";
        for (int index = 0; index < os.exes.Count; ++index)
        {
          if (os.exes[index].name.ToLower().Contains(this.ExeName.ToLower()))
            os.exes[index].isExiting = true;
        }
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
      SAKillExe saKillExe = new SAKillExe();
      if (rdr.MoveToAttribute("Delay"))
        saKillExe.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        saKillExe.DelayHost = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("ExeName"))
        saKillExe.ExeName = rdr.ReadContentAsString();
      return (SerializableAction) saKillExe;
    }
  }
}
