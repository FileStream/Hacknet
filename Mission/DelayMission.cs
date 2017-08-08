// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.DelayMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class DelayMission : MisisonGoal
  {
    private DateTime? firstRequest = new DateTime?();
    public float time;

    public DelayMission(float time)
    {
      this.time = time;
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      if (this.firstRequest.HasValue)
        return (DateTime.Now - this.firstRequest.Value).TotalSeconds >= (double) this.time;
      this.firstRequest = new DateTime?(DateTime.Now);
      return false;
    }
  }
}
