// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.WipeDegreesMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class WipeDegreesMission : MisisonGoal
  {
    public AcademicDatabaseDaemon database;
    private string ownerName;

    public WipeDegreesMission(string targetName, OS _os)
    {
      WipeDegreesMission wipeDegreesMission = this;
      this.ownerName = targetName;
      Action init = (Action) null;
      init = (Action) (() =>
      {
        if (_os.netMap.academicDatabase == null)
          _os.delayer.Post(ActionDelayer.NextTick(), init);
        else
          closure_0.database = (AcademicDatabaseDaemon) _os.netMap.academicDatabase.getDaemon(typeof (AcademicDatabaseDaemon));
      });
      init();
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      return !this.database.hasDegrees(this.ownerName);
    }
  }
}
