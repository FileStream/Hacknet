// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.CheckFlagSetMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class CheckFlagSetMission : MisisonGoal
  {
    public string target;
    private OS os;

    public CheckFlagSetMission(string targetFlagName, OS _os)
    {
      this.target = targetFlagName;
      this.os = _os;
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      return this.os.Flags.HasFlag(this.target);
    }
  }
}
