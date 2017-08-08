// Decompiled with JetBrains decompiler
// Type: Hacknet.SAChangeAlertIcon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SAChangeAlertIcon : SerializableAction
  {
    private const string TypeFlag = "_changeAlertIconType:";
    private const string TargetFlag = "_changeAlertIconTarget:";
    public string Type;
    public string Target;
    public string DelayHost;
    public float Delay;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      if ((double) this.Delay <= 0.0)
      {
        string flagStartingWith1 = os.Flags.GetFlagStartingWith("_changeAlertIconType:");
        string flagStartingWith2 = os.Flags.GetFlagStartingWith("_changeAlertIconTarget:");
        if (flagStartingWith1 != null)
          os.Flags.RemoveFlag(flagStartingWith1);
        if (flagStartingWith2 != null)
          os.Flags.RemoveFlag(flagStartingWith2);
        os.Flags.AddFlag("_changeAlertIconType:" + this.Type);
        os.Flags.AddFlag("_changeAlertIconTarget:" + this.Target);
        if (!os.Flags.HasFlag("_alertIconChanged"))
          os.Flags.AddFlag("_alertIconChanged");
        SAChangeAlertIcon.UpdateAlertIcon((object) os);
      }
      else
      {
        Computer computer = Programs.getComputer(os, this.DelayHost);
        if (computer == null)
          throw new NullReferenceException("Computer " + (object) computer + " could not be found as DelayHost for Function");
        float delay = this.Delay;
        this.Delay = -1f;
        DelayableActionSystem.FindDelayableActionSystemOnComputer(computer).AddAction((SerializableAction) this, delay);
      }
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SAChangeAlertIcon saChangeAlertIcon = new SAChangeAlertIcon();
      if (rdr.MoveToAttribute("Delay"))
        saChangeAlertIcon.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        saChangeAlertIcon.DelayHost = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("Type"))
        saChangeAlertIcon.Type = rdr.ReadContentAsString().ToLower();
      if (rdr.MoveToAttribute("Target"))
        saChangeAlertIcon.Target = rdr.ReadContentAsString();
      if (saChangeAlertIcon.Type == null)
        throw new FormatException("Type tag for ChangeAlertIconAction not found! Make sure you have written it correctly (capital T)!");
      if (saChangeAlertIcon.Type != "mail" && saChangeAlertIcon.Type != "irc" && saChangeAlertIcon.Type != "board" && saChangeAlertIcon.Type != "irchub")
        throw new FormatException("Provided type " + saChangeAlertIcon.Type + " for ChangeAlertIconAction is invalid! Accepted types: mail, irc, board");
      if (saChangeAlertIcon.Type != "mail" && !DLC1SessionUpgrader.HasDLC1Installed)
        throw new NotSupportedException("Changing alert icon to something other than mail requires the Hacknet Labyrinths DLC to be installed.");
      return (SerializableAction) saChangeAlertIcon;
    }

    public static void UpdateAlertIcon(object osobj)
    {
      OS os = (OS) osobj;
      string flagStartingWith1 = os.Flags.GetFlagStartingWith("_changeAlertIconType:");
      string flagStartingWith2 = os.Flags.GetFlagStartingWith("_changeAlertIconTarget:");
      if (flagStartingWith1 == null || flagStartingWith2 == null)
        return;
      string str = flagStartingWith1.Substring("_changeAlertIconType:".Length);
      string ip_Or_ID_or_Name = flagStartingWith2.Substring("_changeAlertIconTarget:".Length);
      Computer computer = Programs.getComputer(os, ip_Or_ID_or_Name);
      switch (str.ToLower())
      {
        case "mail":
          MailServer daemon1 = (MailServer) computer.getDaemon(typeof (MailServer));
          bool flag = false;
          for (int index = 0; index < daemon1.comp.users.Count; ++index)
          {
            if (daemon1.comp.users[index].name == os.defaultUser.name)
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            throw new FormatException("Mail server " + ip_Or_ID_or_Name + " does not have a user account for the player!\nA mail server must have a player account to be used as the alert icon");
          os.mailicon.UpdateTargetServer(daemon1);
          os.ShowDLCAlertsIcon = false;
          break;
        case "irc":
          IRCDaemon daemon2 = (IRCDaemon) computer.getDaemon(typeof (IRCDaemon));
          os.ShowDLCAlertsIcon = true;
          os.hubServerAlertsIcon.UpdateTarget((object) daemon2, (object) daemon2.comp);
          break;
        case "irchub":
          DLCHubServer daemon3 = (DLCHubServer) computer.getDaemon(typeof (DLCHubServer));
          os.ShowDLCAlertsIcon = true;
          os.hubServerAlertsIcon.UpdateTarget((object) daemon3, (object) daemon3.comp);
          break;
        case "board":
          MessageBoardDaemon daemon4 = (MessageBoardDaemon) computer.getDaemon(typeof (MessageBoardDaemon));
          os.ShowDLCAlertsIcon = true;
          os.hubServerAlertsIcon.UpdateTarget((object) daemon4, (object) daemon4.comp);
          break;
      }
    }
  }
}
