// Decompiled with JetBrains decompiler
// Type: Hacknet.ProgramList
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;

namespace Hacknet
{
  internal static class ProgramList
  {
    public static List<string> programs;

    public static void init()
    {
      ProgramList.programs = new List<string>();
      ProgramList.programs.Add("ls");
      ProgramList.programs.Add("cd");
      ProgramList.programs.Add("probe");
      ProgramList.programs.Add("scan");
      ProgramList.programs.Add("ps");
      ProgramList.programs.Add("kill");
      ProgramList.programs.Add("connect");
      ProgramList.programs.Add("dc");
      ProgramList.programs.Add("disconnect");
      ProgramList.programs.Add("help");
      ProgramList.programs.Add("exe");
      ProgramList.programs.Add("cat");
      ProgramList.programs.Add("scp");
      ProgramList.programs.Add("rm");
      ProgramList.programs.Add("openCDTray");
      ProgramList.programs.Add("closeCDTray");
      ProgramList.programs.Add("login");
      ProgramList.programs.Add("reboot");
      ProgramList.programs.Add("mv");
      ProgramList.programs.Add("upload");
      ProgramList.programs.Add("analyze");
      ProgramList.programs.Add("solve");
      ProgramList.programs.Add("addNote");
    }

    public static List<string> getExeList(OS os)
    {
      List<string> stringList = new List<string>();
      stringList.Add("PortHack");
      stringList.Add("ForkBomb");
      stringList.Add("Shell");
      stringList.Add("Tutorial");
      stringList.Add("Notes");
      Folder folder = os.thisComputer.files.root.folders[2];
      for (int index = 0; index < folder.files.Count; ++index)
        stringList.Add(folder.files[index].name.Replace(".exe", ""));
      return stringList;
    }
  }
}
