// Decompiled with JetBrains decompiler
// Type: Hacknet.SASetLock
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SASetLock : SerializableAction
  {
    public bool IsLocked = false;
    public bool IsHidden = false;
    public string DelayHost;
    public float Delay;
    public string Module;

    public override void Trigger(object os_obj)
    {
      if ((double) this.Delay <= 0.0)
      {
        OS os = (OS) os_obj;
        switch (this.Module.ToLower())
        {
          case "terminal":
            os.terminal.inputLocked = this.IsLocked;
            os.terminal.visible = !this.IsHidden;
            break;
          case "netmap":
            os.netMap.inputLocked = this.IsLocked;
            os.netMap.visible = !this.IsHidden;
            break;
          case "ram":
            os.ram.inputLocked = this.IsLocked;
            os.ram.visible = !this.IsHidden;
            break;
          case "display":
            os.display.inputLocked = this.IsLocked;
            os.display.visible = !this.IsHidden;
            break;
        }
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
      SASetLock saSetLock = new SASetLock();
      if (rdr.MoveToAttribute("Delay"))
        saSetLock.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        saSetLock.DelayHost = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("Module"))
        saSetLock.Module = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("IsLocked"))
        saSetLock.IsLocked = rdr.ReadContentAsString().ToLower().StartsWith("t");
      if (rdr.MoveToAttribute("IsHidden"))
        saSetLock.IsHidden = rdr.ReadContentAsString().ToLower().StartsWith("t");
      return (SerializableAction) saSetLock;
    }
  }
}
