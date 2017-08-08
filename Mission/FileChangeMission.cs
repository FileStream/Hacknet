// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.FileChangeMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class FileChangeMission : MisisonGoal
  {
    public bool isRemoval = false;
    public bool caseSensitive = false;
    public Folder container;
    public string target;
    public string targetData;
    public string targetKeyword;
    public Computer targetComp;
    public OS os;

    public FileChangeMission(string path, string filename, string computerIP, string targetKeyword, OS _os, bool isRemoval = false)
    {
      this.target = filename;
      this.targetKeyword = targetKeyword;
      this.isRemoval = isRemoval;
      this.os = _os;
      Computer computer = Programs.getComputer(this.os, computerIP);
      if (computer == null)
        throw new NullReferenceException("Computer \"" + computerIP + "\" not found for FileChange mission goal");
      this.targetComp = computer;
      this.container = computer.getFolderFromPath(path, false);
      for (int index = 0; index < this.container.files.Count; ++index)
      {
        if (this.container.files[index].name.Equals(this.target))
          this.targetData = this.container.files[index].data;
      }
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      for (int index = 0; index < this.container.files.Count; ++index)
      {
        if (this.container.files[index].name.Equals(this.target))
        {
          string str1 = this.container.files[index].data;
          string str2 = this.targetKeyword;
          if (!this.caseSensitive)
          {
            str1 = str1.ToLower();
            str2 = str2.ToLower();
          }
          if (str1.Contains(str2))
            return !this.isRemoval;
          return this.isRemoval;
        }
      }
      if (!this.isRemoval)
        return false;
      Utils.AppendToWarningsFile("FileChangeMissionGoal Error: File " + this.target + " was not found in the container folder \"" + this.container.name + "\" - defaulting to true");
      return true;
    }

    public override string TestCompletable()
    {
      string str = "";
      if (this.container.searchForFile(this.target) == null)
        str = str + "File to change (" + this.container.name + "/" + this.target + ") does not exist!";
      return str;
    }
  }
}
