// Decompiled with JetBrains decompiler
// Type: Hacknet.SAAddIRCMessage
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Daemons.Helpers;
using System;
using System.Xml;

namespace Hacknet
{
  public class SAAddIRCMessage : SerializableAction
  {
    public float Delay = 0.0f;
    [XMLContent]
    public string Message;
    public string Author;
    public string TargetComp;

    public override void Trigger(object os_obj)
    {
      Computer computer = Programs.getComputer((OS) os_obj, this.TargetComp);
      if (computer == null)
        throw new NullReferenceException("Computer " + this.TargetComp + " could not be found for SAAddIRCMessage Function, adding message: " + this.Message);
      IRCDaemon daemon1 = computer.getDaemon(typeof (IRCDaemon)) as IRCDaemon;
      IRCSystem ircSystem;
      if (daemon1 != null)
      {
        ircSystem = daemon1.System;
      }
      else
      {
        DLCHubServer daemon2 = computer.getDaemon(typeof (DLCHubServer)) as DLCHubServer;
        if (daemon2 == null)
          throw new NullReferenceException("Computer " + this.TargetComp + " does not contain an IRC server daemon for SAAddIRCMessage function adding message: " + this.Message);
        ircSystem = daemon2.IRCSystem;
      }
      if ((double) this.Delay <= 0.0)
      {
        if ((double) Math.Abs(this.Delay) < 1.0 / 1000.0)
        {
          ircSystem.AddLog(this.Author, this.Message, (string) null);
        }
        else
        {
          DateTime dateTime = DateTime.Now - TimeSpan.FromSeconds((double) this.Delay);
          string timestamp = dateTime.Hour.ToString("00") + ":" + dateTime.Minute.ToString("00");
          ircSystem.AddLog(this.Author, this.Message, timestamp);
        }
      }
      else
      {
        float delay = this.Delay;
        this.Delay = -1f;
        DelayableActionSystem.FindDelayableActionSystemOnComputer(computer).AddAction((SerializableAction) this, delay);
      }
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SAAddIRCMessage saAddIrcMessage = new SAAddIRCMessage();
      if (rdr.MoveToAttribute("Author"))
        saAddIrcMessage.Author = ComputerLoader.filter(rdr.ReadContentAsString());
      if (rdr.MoveToAttribute("Delay"))
        saAddIrcMessage.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("TargetComp"))
        saAddIrcMessage.TargetComp = rdr.ReadContentAsString();
      int content = (int) rdr.MoveToContent();
      saAddIrcMessage.Message = ComputerLoader.filter(rdr.ReadElementContentAsString());
      if (string.IsNullOrWhiteSpace(saAddIrcMessage.TargetComp))
        throw new FormatException("Invalid Target Comp");
      if (string.IsNullOrWhiteSpace(saAddIrcMessage.Message))
        throw new FormatException("Invalid or Empty Message!");
      return (SerializableAction) saAddIrcMessage;
    }
  }
}
