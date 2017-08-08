// Decompiled with JetBrains decompiler
// Type: Hacknet.SAChangeNetmapSortMethod
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SAChangeNetmapSortMethod : SerializableAction
  {
    public string Method;
    public string DelayHost;
    public float Delay;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      if ((double) this.Delay <= 0.0)
      {
        NetmapSortingAlgorithm sortingAlgorithm = NetmapSortingAlgorithm.Grid;
        switch (this.Method.ToLower())
        {
          case "scatter":
            sortingAlgorithm = NetmapSortingAlgorithm.Scatter;
            break;
          case "grid":
            sortingAlgorithm = NetmapSortingAlgorithm.Grid;
            break;
          case "chaos":
            sortingAlgorithm = NetmapSortingAlgorithm.Chaos;
            break;
          case "scangrid":
          case "seqgrid":
          case "sequencegrid":
          case "sequence grid":
            sortingAlgorithm = NetmapSortingAlgorithm.LockGrid;
            break;
        }
        os.netMap.SortingAlgorithm = sortingAlgorithm;
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
      SAChangeNetmapSortMethod netmapSortMethod = new SAChangeNetmapSortMethod();
      if (rdr.MoveToAttribute("Delay"))
        netmapSortMethod.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        netmapSortMethod.DelayHost = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("Method"))
        netmapSortMethod.Method = rdr.ReadContentAsString();
      return (SerializableAction) netmapSortMethod;
    }
  }
}
