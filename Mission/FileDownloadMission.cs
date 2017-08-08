// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.FileDownloadMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class FileDownloadMission : FileDeletionMission
  {
    public FileDownloadMission(string path, string filename, string computerIP, OS os)
      : base(path, filename, computerIP, os)
    {
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      Folder root = this.os.thisComputer.files.root;
      for (int index = 0; index < root.folders.Count; ++index)
      {
        if (root.folders[index].containsFileWithData(this.targetData))
          return true;
      }
      return false;
    }

    public override string TestCompletable()
    {
      string str = "";
      if (this.container.searchForFile(this.target) == null)
        str = str + "File to download (" + this.container.name + "/" + this.target + ") does not exist!";
      return str;
    }
  }
}
