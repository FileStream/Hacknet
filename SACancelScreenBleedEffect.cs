// Decompiled with JetBrains decompiler
// Type: Hacknet.SACancelScreenBleedEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SACancelScreenBleedEffect : SerializableAction
  {
    public string DelayHost;
    public float Delay;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      if ((double) this.Delay <= 0.0)
      {
        os.EffectsUpdater.CancelScreenBleedEffect();
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
      SACancelScreenBleedEffect screenBleedEffect = new SACancelScreenBleedEffect();
      if (rdr.MoveToAttribute("Delay"))
        screenBleedEffect.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        screenBleedEffect.DelayHost = rdr.ReadContentAsString();
      return (SerializableAction) screenBleedEffect;
    }
  }
}
