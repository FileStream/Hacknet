// Decompiled with JetBrains decompiler
// Type: Hacknet.SCInstantly
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;
using System.Xml;

namespace Hacknet
{
  public class SCInstantly : SerializableCondition
  {
    public bool needsMissionComplete;

    public override bool Check(object os_obj)
    {
      return !this.needsMissionComplete || ((OS) os_obj).currentMission.isComplete((List<string>) null);
    }

    public static SerializableCondition DeserializeFromReader(XmlReader rdr)
    {
      SCInstantly scInstantly = new SCInstantly();
      if (rdr.MoveToAttribute("needsMissionComplete"))
        scInstantly.needsMissionComplete = rdr.ReadContentAsString().ToLower() == "true";
      return (SerializableCondition) scInstantly;
    }
  }
}
