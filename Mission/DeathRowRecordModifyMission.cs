// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.DeathRowRecordModifyMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class DeathRowRecordModifyMission : MisisonGoal
  {
    public Folder container;
    public string fname;
    public string lname;
    public Computer deathRowDatabase;
    public OS os;
    public string lastWords;

    public DeathRowRecordModifyMission(string firstName, string lastName, string lastWords, OS _os)
    {
      this.os = _os;
      this.fname = firstName;
      this.lname = lastName;
      this.lastWords = lastWords;
      Computer computer = Programs.getComputer(this.os, "deathRow");
      this.deathRowDatabase = computer;
      this.container = computer.getFolderFromPath("dr_database/Records", false);
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      DeathRowDatabaseDaemon.DeathRowEntry recordForName = ((DeathRowDatabaseDaemon) this.deathRowDatabase.getDaemon(typeof (DeathRowDatabaseDaemon))).GetRecordForName(this.fname, this.lname);
      if (recordForName.RecordNumber == null)
        return false;
      if (this.lastWords != null)
      {
        string str1 = recordForName.Statement.ToLower().Replace("\r", "").Replace(",", "").Replace(".", "");
        string str2 = this.lastWords.ToLower().Replace("\r", "").Replace(",", "").Replace(".", "");
        if (str1.Contains(str2) || str1 == str2 || str1.StartsWith(str2))
          return true;
      }
      return true;
    }
  }
}
