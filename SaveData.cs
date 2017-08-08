// Decompiled with JetBrains decompiler
// Type: Hacknet.SaveData
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet
{
  [Serializable]
  public class SaveData
  {
    private List<Computer> nodes;
    private ActiveMission mission;

    public void addNodes(object nodesToAdd)
    {
      this.nodes = (List<Computer>) nodesToAdd;
    }

    public void setMission(object missionToAdd)
    {
      this.mission = (ActiveMission) missionToAdd;
    }

    public object getNodes()
    {
      return (object) this.nodes;
    }

    public object getMission()
    {
      return (object) this.mission;
    }
  }
}
