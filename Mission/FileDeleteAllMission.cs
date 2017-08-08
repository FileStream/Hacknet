// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.FileDeleteAllMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class FileDeleteAllMission : MisisonGoal
  {
    public Folder container;
    public Computer targetComp;
    public OS os;

    public FileDeleteAllMission(string path, string computerIP, OS _os)
    {
      this.os = _os;
      Computer computer = Programs.getComputer(this.os, computerIP);
      if (computer == null)
        throw new NullReferenceException("Computer \"" + computerIP + "\" not found for FileDeletion mission goal");
      this.targetComp = computer;
      this.container = computer.getFolderFromPath(path, false);
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      return this.container.files.Count == 0;
    }

    public override string TestCompletable()
    {
      return "";
    }
  }
}
