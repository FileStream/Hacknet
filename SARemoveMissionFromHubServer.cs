// Decompiled with JetBrains decompiler
// Type: Hacknet.SARemoveMissionFromHubServer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SARemoveMissionFromHubServer : SerializableAction
  {
    public string MissionFilepath;
    public string TargetComp;

    public override void Trigger(object os_obj)
    {
      Computer computer = Programs.getComputer((OS) os_obj, this.TargetComp);
      if (computer == null)
        throw new NullReferenceException("Computer " + this.TargetComp + " could not be found for SARemoveMissionFromHubServer Function, removing mission: " + this.MissionFilepath);
      MissionHubServer daemon1 = computer.getDaemon(typeof (MissionHubServer)) as MissionHubServer;
      if (daemon1 != null)
      {
        daemon1.RemoveMissionFromListings(this.MissionFilepath);
      }
      else
      {
        DLCHubServer daemon2 = computer.getDaemon(typeof (DLCHubServer)) as DLCHubServer;
        if (daemon2 != null)
        {
          daemon2.RemoveMission(this.MissionFilepath);
        }
        else
        {
          MissionListingServer daemon3 = computer.getDaemon(typeof (MissionListingServer)) as MissionListingServer;
          if (daemon3 == null)
            throw new NullReferenceException("Computer " + this.TargetComp + " does not contain a MissionHubServer, MissionListingServer or DLCHubServer daemon for remove mission function adding mission: " + this.MissionFilepath);
          daemon3.removeMission(this.MissionFilepath);
        }
      }
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SARemoveMissionFromHubServer missionFromHubServer = new SARemoveMissionFromHubServer();
      if (rdr.MoveToAttribute("MissionFilepath"))
        missionFromHubServer.MissionFilepath = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("TargetComp"))
        missionFromHubServer.TargetComp = rdr.ReadContentAsString();
      if (string.IsNullOrWhiteSpace(missionFromHubServer.MissionFilepath))
        throw new FormatException("Invalid MissionFilepath");
      if (string.IsNullOrWhiteSpace(missionFromHubServer.TargetComp))
        throw new FormatException("Invalid TargetComp");
      return (SerializableAction) missionFromHubServer;
    }
  }
}
