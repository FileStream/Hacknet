// Decompiled with JetBrains decompiler
// Type: Hacknet.SALoadMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.IO;
using System.Xml;

namespace Hacknet
{
  public class SALoadMission : SerializableAction
  {
    public string MissionName;

    public override void Trigger(object os_obj)
    {
      ComputerLoader.loadMission(Utils.GetFileLoadPrefix() + this.MissionName, false);
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SALoadMission saLoadMission = new SALoadMission();
      if (rdr.MoveToAttribute("MissionName"))
        saLoadMission.MissionName = rdr.ReadContentAsString();
      if (saLoadMission.MissionName == null || !File.Exists(Utils.GetFileLoadPrefix() + saLoadMission.MissionName))
        throw new FormatException("Invalid Mission file Path :" + saLoadMission.MissionName);
      return (SerializableAction) saLoadMission;
    }
  }
}
