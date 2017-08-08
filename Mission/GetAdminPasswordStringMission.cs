// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.GetAdminPasswordStringMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class GetAdminPasswordStringMission : MisisonGoal
  {
    public Computer target;
    public OS os;

    public GetAdminPasswordStringMission(string compIP, OS _os)
    {
      this.os = _os;
      this.target = Programs.getComputer(this.os, compIP);
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      if (additionalDetails == null)
        return false;
      for (int index = 0; index < additionalDetails.Count; ++index)
      {
        if (additionalDetails[index].Contains(this.target.adminPass))
          return true;
      }
      return false;
    }
  }
}
