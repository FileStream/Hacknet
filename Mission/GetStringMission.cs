// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.GetStringMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class GetStringMission : MisisonGoal
  {
    public string target;

    public GetStringMission(string targetData)
    {
      this.target = targetData;
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      if (additionalDetails == null)
        return false;
      for (int index = 0; index < additionalDetails.Count; ++index)
      {
        if (additionalDetails[index].ToLower().Contains(this.target.ToLower()))
          return true;
      }
      return false;
    }
  }
}
