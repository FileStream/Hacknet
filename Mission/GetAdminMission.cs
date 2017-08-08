// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.GetAdminMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class GetAdminMission : MisisonGoal
  {
    public Computer target;
    public OS os;

    public GetAdminMission(string compIP, OS _os)
    {
      this.os = _os;
      this.target = Programs.getComputer(this.os, compIP);
      if (this.target == null)
        throw new NullReferenceException("Computer \"" + compIP + "\" not found for FileDeletion mission goal");
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      return this.target.adminIP.Equals(this.os.thisComputer.ip);
    }

    public override void reset()
    {
    }
  }
}
