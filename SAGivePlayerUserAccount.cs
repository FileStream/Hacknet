// Decompiled with JetBrains decompiler
// Type: Hacknet.SAGivePlayerUserAccount
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SAGivePlayerUserAccount : SerializableAction
  {
    public string TargetComp;
    public string Username;
    public string DelayHost;
    public float Delay;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      if ((double) this.Delay <= 0.0)
      {
        Computer computer = Programs.getComputer(os, this.TargetComp);
        if (computer == null)
          return;
        for (int index = 0; index < computer.users.Count; ++index)
        {
          if (computer.users[index].name == this.Username)
          {
            UserDetail user = computer.users[index];
            user.known = true;
            computer.users[index] = user;
          }
        }
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
      SAGivePlayerUserAccount playerUserAccount = new SAGivePlayerUserAccount();
      if (rdr.MoveToAttribute("Delay"))
        playerUserAccount.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        playerUserAccount.DelayHost = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("TargetComp"))
        playerUserAccount.TargetComp = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("Username"))
        playerUserAccount.Username = rdr.ReadContentAsString();
      return (SerializableAction) playerUserAccount;
    }
  }
}
