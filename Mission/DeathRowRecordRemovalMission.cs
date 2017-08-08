// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.DeathRowRecordRemovalMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class DeathRowRecordRemovalMission : MisisonGoal
  {
    public Folder container;
    public string fname;
    public string lname;
    public Computer deathRowDatabase;
    public OS os;

    public DeathRowRecordRemovalMission(string firstName, string lastName, OS _os)
    {
      this.os = _os;
      this.fname = firstName;
      this.lname = lastName;
      Computer computer = Programs.getComputer(this.os, "deathRow");
      this.deathRowDatabase = computer;
      this.container = computer.getFolderFromPath("dr_database/Records", false);
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      return !((DeathRowDatabaseDaemon) this.deathRowDatabase.getDaemon(typeof (DeathRowDatabaseDaemon))).ContainsRecordForName(this.fname, this.lname);
    }
  }
}
