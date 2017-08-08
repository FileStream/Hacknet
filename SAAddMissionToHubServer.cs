// Decompiled with JetBrains decompiler
// Type: Hacknet.SAAddMissionToHubServer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Xml;

namespace Hacknet
{
  public class SAAddMissionToHubServer : SerializableAction
  {
    public string MissionFilepath;
    public string TargetComp;
    public string AssignmentTag;
    public bool StartsComplete;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      Computer computer = Programs.getComputer(os, this.TargetComp);
      if (computer == null)
        throw new NullReferenceException("Computer " + this.TargetComp + " could not be found for SAAddMissionToHubServer Function, adding mission: " + this.MissionFilepath);
      MissionHubServer daemon1 = computer.getDaemon(typeof (MissionHubServer)) as MissionHubServer;
      if (daemon1 != null)
      {
        daemon1.AddMissionToListings(Utils.GetFileLoadPrefix() + this.MissionFilepath, -1);
      }
      else
      {
        DLCHubServer daemon2 = computer.getDaemon(typeof (DLCHubServer)) as DLCHubServer;
        if (daemon2 != null)
        {
          daemon2.AddMission(Utils.GetFileLoadPrefix() + this.MissionFilepath, this.AssignmentTag, this.StartsComplete);
        }
        else
        {
          MissionListingServer daemon3 = computer.getDaemon(typeof (MissionListingServer)) as MissionListingServer;
          if (daemon3 == null)
            throw new NullReferenceException("Computer " + this.TargetComp + " does not contain a MissionHubServer, MissionListingServer or DLCHubServer daemon for addMission function adding mission: " + this.MissionFilepath);
          List<ActiveMission> branchMissions = os.branchMissions;
          ActiveMission m = (ActiveMission) ComputerLoader.readMission(Utils.GetFileLoadPrefix() + this.MissionFilepath);
          os.branchMissions = branchMissions;
          daemon3.addMisison(m, this.AssignmentTag.ToLower() == "top");
        }
      }
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SAAddMissionToHubServer missionToHubServer = new SAAddMissionToHubServer();
      if (rdr.MoveToAttribute("MissionFilepath"))
        missionToHubServer.MissionFilepath = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("TargetComp"))
        missionToHubServer.TargetComp = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("AssignmentTag"))
        missionToHubServer.AssignmentTag = rdr.ReadContentAsString();
      if (string.IsNullOrWhiteSpace(missionToHubServer.AssignmentTag))
        missionToHubServer.AssignmentTag = (string) null;
      if (rdr.MoveToAttribute("StartsComplete"))
        missionToHubServer.StartsComplete = rdr.ReadContentAsString().ToLower() == "true";
      if (string.IsNullOrWhiteSpace(missionToHubServer.MissionFilepath))
        throw new FormatException("Invalid MissionFilepath");
      if (string.IsNullOrWhiteSpace(missionToHubServer.TargetComp))
        throw new FormatException("Invalid TargetComp");
      return (SerializableAction) missionToHubServer;
    }
  }
}
