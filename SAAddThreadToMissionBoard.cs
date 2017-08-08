// Decompiled with JetBrains decompiler
// Type: Hacknet.SAAddThreadToMissionBoard
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SAAddThreadToMissionBoard : SerializableAction
  {
    public string ThreadFilepath;
    public string TargetComp;

    public override void Trigger(object os_obj)
    {
      Computer computer = Programs.getComputer((OS) os_obj, this.TargetComp);
      if (computer == null)
        throw new NullReferenceException("Computer " + this.TargetComp + " could not be found for SAAddThreadToMissionBoard Function, adding thread: " + this.ThreadFilepath);
      MessageBoardDaemon daemon = computer.getDaemon(typeof (MessageBoardDaemon)) as MessageBoardDaemon;
      if (daemon == null)
        throw new NullReferenceException("Computer " + this.TargetComp + " does not contain a MessageBoard daemon for SAAddThreadToMissionBoard function adding thread: " + this.ThreadFilepath);
      string threadData = Utils.readEntireFile(Utils.GetFileLoadPrefix() + this.ThreadFilepath);
      daemon.AddThread(threadData);
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SAAddThreadToMissionBoard threadToMissionBoard = new SAAddThreadToMissionBoard();
      if (rdr.MoveToAttribute("ThreadFilepath"))
        threadToMissionBoard.ThreadFilepath = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("TargetComp"))
        threadToMissionBoard.TargetComp = rdr.ReadContentAsString();
      if (string.IsNullOrWhiteSpace(threadToMissionBoard.ThreadFilepath))
        throw new FormatException("Invalid MissionFilepath");
      if (string.IsNullOrWhiteSpace(threadToMissionBoard.TargetComp))
        throw new FormatException("Invalid TargetComp");
      return (SerializableAction) threadToMissionBoard;
    }
  }
}
