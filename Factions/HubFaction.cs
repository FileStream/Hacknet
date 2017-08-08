// Decompiled with JetBrains decompiler
// Type: Hacknet.Factions.HubFaction
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet.Factions
{
  internal class HubFaction : Faction
  {
    public HubFaction(string _name, int _neededValue)
      : base(_name, _neededValue)
    {
      this.PlayerLosesValueOnAbandon = false;
    }

    public override void addValue(int value, object os)
    {
      int playerValue = this.playerValue;
      base.addValue(value, os);
      if (this.valuePassedPoint(playerValue, 1) && !((OS) os).Flags.HasFlag("themeChangerAdded"))
      {
        Folder folder1 = Programs.getComputer((OS) os, "mainHubAssets").files.root.searchForFolder("bin");
        Folder folder2 = new Folder("ThemeChanger");
        folder2.files.Add(new FileEntry(PortExploits.crackExeData[14], "ThemeChanger.exe"));
        string dataEntry = Utils.readEntireFile("Content/LocPost/ThemeChangerReadme.txt");
        folder2.files.Add(new FileEntry(dataEntry, "info.txt"));
        folder1.folders.Add(folder2);
        ((OS) os).delayer.Post(ActionDelayer.Wait(1.0), (Action) (() =>
        {
          this.SendAssetAddedNotification(os);
          ((OS) os).Flags.AddFlag("themeChangerAdded");
          ((OS) os).saveGame();
        }));
      }
      if (this.valuePassedPoint(playerValue, 4))
        ((OS) os).delayer.Post(ActionDelayer.Wait(2.0), (Action) (() =>
        {
          ((MissionHubServer) Programs.getComputer((OS) os, "mainHub").getDaemon(typeof (MissionHubServer))).AddMissionToListings("Content/Missions/MainHub/BitSet/Missions/BitHubSet01.xml", -1);
          ((OS) os).saveGame();
        }));
      else if (this.playerValue >= 7 && ((OS) os).Flags.HasFlag("decypher") && ((OS) os).Flags.HasFlag("dechead") && !((OS) os).Flags.HasFlag("csecRankingS2Pass"))
      {
        this.SendNotification(os, "Project Junebug");
        ((OS) os).Flags.AddFlag("csecRankingS2Pass");
        ((OS) os).saveGame();
      }
      else if (this.playerValue >= 10 && !((OS) os).Flags.HasFlag("bitPathStarted"))
      {
        MissionHubServer daemon = (MissionHubServer) Programs.getComputer((OS) os, "mainHub").getDaemon(typeof (MissionHubServer));
        if (daemon != null && daemon.GetNumberOfAvaliableMissions() > 0)
          return;
        this.ForceStartBitMissions(os);
      }
      if (this.playerValue >= 2 && ((OS) os).Flags.HasFlag("dlc_complete") && DLC1SessionUpgrader.HasDLC1Installed && !((OS) os).Flags.HasFlag("dlc_post_missionadded"))
      {
        bool flag = false;
        string flagStartingWith = ((OS) os).Flags.GetFlagStartingWith("dlc_csec_end_facval");
        if (flagStartingWith != null)
        {
          try
          {
            if (this.playerValue - Convert.ToInt32(flagStartingWith.Substring(flagStartingWith.IndexOf(":") + 1)) > 1)
              flag = true;
          }
          catch (Exception ex)
          {
            Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
          }
        }
        else
          flag = true;
        if (flag && ((OS) os).Flags.HasFlag("DLC_PlaneSaveResponseTriggered"))
        {
          ((OS) os).Flags.AddFlag("dlc_post_missionadded");
          MissionHubServer daemon1 = (MissionHubServer) Programs.getComputer((OS) os, "mainHub").getDaemon(typeof (MissionHubServer));
          FileEntry fileEntry = Programs.getComputer((OS) os, "dHidden").files.root.searchForFolder("home").searchForFile("PA_0022_Incident.dec");
          Programs.getComputer((OS) os, "mainHubAssets").files.root.searchForFolder("home").files.Add(fileEntry);
          daemon1.AddMissionToListings("Content/DLC/Missions/Injections/Missions/CSEC_Injection_Mission.xml", -1);
          if (Programs.getComputer((OS) os, "dAttackHome").files.root.searchForFolder("home").searchForFolder("uni").files.Count == 0)
          {
            MailServer daemon2 = ((OS) os).netMap.mailServer.getDaemon(typeof (MailServer)) as MailServer;
            string str = Utils.readEntireFile("Content/DLC/Docs/StrikerLateEmail.txt");
            int num = str.IndexOf("\n");
            string subject = str.Substring(0, num).Trim();
            string body = str.Substring(num, str.Length - num).Trim();
            List<string> attachments = new List<string>();
            if (Settings.ActiveLocale == "en-us")
              attachments.Add("note#%#Important Extra Information#%#Fuck you");
            daemon2.addMail(MailServer.generateEmail(subject, body, "StrikeR", attachments), ((OS) os).defaultUser.name);
          }
          ((OS) os).saveGame();
        }
      }
    }

    public void ForceStartBitMissions(object os)
    {
      ((OS) os).Flags.AddFlag("bitPathStarted");
      ((OS) os).delayer.Post(ActionDelayer.Wait(1.6), (Action) (() => ComputerLoader.loadMission("Content/Missions/BitPath/BitAdv_Intro.xml", false)));
      Programs.getComputer((OS) os, "mainHubAssets").files.root.searchForFolder("bin").folders.Add(new Folder("Misc")
      {
        files = {
          new FileEntry(PortExploits.crackExeData[9], "Decypher.exe"),
          new FileEntry(PortExploits.crackExeData[10], "DECHead.exe"),
          new FileEntry(PortExploits.crackExeData[104], "KBT_PortTest.exe"),
          new FileEntry("Kellis BioTech medical port cycler - target 104-103.", "kbt_readme.txt")
        }
      });
      this.SendNotification(os, LocaleTerms.Loc("Agent") + ",\n" + LocaleTerms.Loc("Additional resources have been added to the CSEC members asset pool, for your free use.") + " " + LocaleTerms.Loc("Find them in the misc folder on the asset server.") + "\n\n" + LocaleTerms.Loc("Thankyou") + ",\n -" + this.name, this.name + " " + LocaleTerms.Loc("Admins :: Asset Uploads"));
    }

    private void SendNotification(object osIn, string body, string subject)
    {
      OS os = (OS) osIn;
      string sender = this.name + " ReplyBot";
      string email = MailServer.generateEmail(subject, body, sender);
      ((MailServer) os.netMap.mailServer.getDaemon(typeof (MailServer))).addMail(email, os.defaultUser.name);
    }

    private void SendNotification(object osIn, string contractName)
    {
      OS os = (OS) osIn;
      string email = MailServer.generateEmail(this.name + " " + LocaleTerms.Loc("Admins :: Flagged for Critical Contract"), string.Format(Utils.readEntireFile("Content/LocPost/CSEC_JunebugEmail.txt"), (object) this.name), this.name + " ReplyBot");
      ((MailServer) os.netMap.mailServer.getDaemon(typeof (MailServer))).addMail(email, os.defaultUser.name);
    }

    private void SendAssetAddedNotification(object osIn)
    {
      OS os = (OS) osIn;
      string subject = this.name + " " + LocaleTerms.Loc("Admins :: New asset added");
      string body = string.Format(Utils.readEntireFile("Content/LocPost/CSEC_ThemechangerEmail.txt"), (object) this.name);
      string sender = this.name + " ReplyBot";
      Computer computer = Programs.getComputer(os, "mainHubAssets");
      string str = "link#%#" + computer.name + "#%#" + computer.ip;
      string email = MailServer.generateEmail(subject, body, sender, new List<string>((IEnumerable<string>) new string[1]{ str }));
      ((MailServer) os.netMap.mailServer.getDaemon(typeof (MailServer))).addMail(email, os.defaultUser.name);
    }
  }
}
