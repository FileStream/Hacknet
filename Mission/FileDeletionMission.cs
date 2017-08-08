// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.FileDeletionMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class FileDeletionMission : MisisonGoal
  {
    public Folder container;
    public string target;
    public string targetData;
    public string targetComp;
    public string targetPath;
    public OS os;

    public FileDeletionMission(string path, string filename, string computerIP, OS _os)
    {
      this.target = filename;
      this.os = _os;
      this.targetComp = computerIP;
      Computer computer = Programs.getComputer(this.os, this.targetComp);
      if (computer == null && Settings.IsInExtensionMode)
        throw new NullReferenceException("Computer \"" + computerIP + "\" was not found for File Deletion mission goal!");
      this.targetPath = path;
      if (computer == null)
        return;
      this.container = computer.getFolderFromPath(path, false);
      if (this.container == null)
        throw new NullReferenceException("Folder " + path + " was not found on computer " + computerIP + " for file deletion mission goal");
      for (int index = 0; index < this.container.files.Count; ++index)
      {
        if (this.container.files[index].name.Equals(this.target))
          this.targetData = this.container.files[index].data;
      }
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      Computer computer = Programs.getComputer(this.os, this.targetComp);
      if (computer == null)
        return true;
      this.container = computer.getFolderFromPath(this.targetPath, false);
      if (this.container == null)
        return true;
      for (int index = 0; index < this.container.files.Count; ++index)
      {
        if (this.container.files[index].name.Equals(this.target) && (this.container.files[index].data.Equals(this.targetData) || this.targetData == null))
          return false;
      }
      return true;
    }

    public override string TestCompletable()
    {
      string str = "";
      if (this.container.searchForFile(this.target) == null)
        str = str + "File to delete (" + this.container.name + "/" + this.target + ") does not exist!";
      return str;
    }
  }
}
