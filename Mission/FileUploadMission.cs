// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.FileUploadMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class FileUploadMission : MisisonGoal
  {
    public Folder container;
    public string target;
    public string targetData;
    public Computer targetComp;
    public Computer uploadTargetComp;
    public OS os;
    public Folder destinationFolder;

    public FileUploadMission(string path, string filename, string computerWithFileIP, string computerToUploadToIP, string destToUploadToPath, OS _os, bool needsDecrypt = false, string decryptPass = "")
    {
      this.target = filename;
      this.os = _os;
      Computer computer1 = Programs.getComputer(this.os, computerWithFileIP);
      if (computer1 == null)
        throw new FormatException("Error parsing File Upload Mission - Source comp " + computerWithFileIP + " not found!");
      this.targetComp = computer1;
      this.container = computer1.getFolderFromPath(path, false);
      for (int index = 0; index < this.container.files.Count; ++index)
      {
        if (this.container.files[index].name.Equals(this.target))
        {
          this.targetData = this.container.files[index].data;
          if (needsDecrypt)
            this.targetData = FileEncrypter.DecryptString(this.targetData, decryptPass)[2];
        }
      }
      Computer computer2 = Programs.getComputer(this.os, computerToUploadToIP);
      if (computer2 == null)
        throw new FormatException("Error parsing File Upload Mission - Dest comp " + computerWithFileIP + " not found!");
      this.uploadTargetComp = computer2;
      this.destinationFolder = computer2.getFolderFromPath(destToUploadToPath, false);
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      for (int index = 0; index < this.destinationFolder.files.Count; ++index)
      {
        if (this.targetData == null)
        {
          if (this.destinationFolder.files[index].name.ToLower().Equals(this.target.ToLower()))
            return true;
        }
        else if (this.destinationFolder.files[index].data.Equals(this.targetData))
          return true;
      }
      return false;
    }

    public override string TestCompletable()
    {
      string str = "";
      if (this.container.searchForFile(this.target) == null)
        str = str + "File to upload (" + this.container.name + "/" + this.target + ") does not exist!";
      return str;
    }
  }
}
