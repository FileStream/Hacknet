// Decompiled with JetBrains decompiler
// Type: Hacknet.SAStartScreenBleedEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Xml;

namespace Hacknet
{
  public class SAStartScreenBleedEffect : SerializableAction
  {
    public float TotalDurationSeconds = 200f;
    [XMLContent]
    public string ContentLines;
    public string AlertTitle;
    public string CompleteAction;
    public string DelayHost;
    public float Delay;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      if ((double) this.Delay <= 0.0)
      {
        List<string> stringList = new List<string>((IEnumerable<string>) this.ContentLines.Split(Utils.robustNewlineDelim, StringSplitOptions.None));
        while (stringList.Count < 3)
          stringList.Add("");
        os.EffectsUpdater.StartScreenBleed(this.TotalDurationSeconds, this.AlertTitle, stringList[0], stringList[1], stringList[2], this.CompleteAction);
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
      SAStartScreenBleedEffect screenBleedEffect = new SAStartScreenBleedEffect();
      if (rdr.MoveToAttribute("Delay"))
        screenBleedEffect.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        screenBleedEffect.DelayHost = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("AlertTitle"))
        screenBleedEffect.AlertTitle = ComputerLoader.filter(rdr.ReadContentAsString());
      if (rdr.MoveToAttribute("CompleteAction"))
        screenBleedEffect.CompleteAction = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("TotalDurationSeconds"))
        screenBleedEffect.TotalDurationSeconds = (float) rdr.ReadContentAsDouble();
      int content = (int) rdr.MoveToContent();
      screenBleedEffect.ContentLines = ComputerLoader.filter(rdr.ReadElementContentAsString());
      return (SerializableAction) screenBleedEffect;
    }
  }
}
