// Decompiled with JetBrains decompiler
// Type: Hacknet.BasicAdministrator
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet
{
  internal class BasicAdministrator : Administrator
  {
    public override void disconnectionDetected(Computer c, OS os)
    {
      base.disconnectionDetected(c, os);
      double time = 20.0 * Utils.random.NextDouble();
      os.delayer.Post(ActionDelayer.Wait(time), (Action) (() =>
      {
        if (os.connectedComp != null && !(os.connectedComp.ip != c.ip))
          return;
        for (int index = 0; index < c.ports.Count; ++index)
          c.closePort(c.ports[index], "LOCAL_ADMIN");
        if (this.ResetsPassword)
          c.setAdminPassword(PortExploits.getRandomPassword());
        c.adminIP = c.ip;
        if (c.firewall != null)
        {
          c.firewall.solved = false;
          c.firewall.resetSolutionProgress();
        }
      }));
    }
  }
}
