// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.AddDegreeMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class AddDegreeMission : MisisonGoal
  {
    public AcademicDatabaseDaemon database;
    private string ownerName;
    private string degreeName;
    private string uniName;
    private float desiredGPA;

    public AddDegreeMission(string targetName, string degreeName, string uniName, float desiredGPA, OS _os)
    {
      AddDegreeMission addDegreeMission = this;
      this.ownerName = targetName;
      this.degreeName = degreeName;
      this.uniName = uniName;
      this.desiredGPA = desiredGPA;
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
      return this.database.doesDegreeExist(this.ownerName, this.degreeName, this.uniName, this.desiredGPA);
    }
  }
}
