// Decompiled with JetBrains decompiler
// Type: Hacknet.FastBasicAdministrator
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet
{
  internal class FastBasicAdministrator : Administrator
  {
    public override void disconnectionDetected(Computer c, OS os)
    {
      base.disconnectionDetected(c, os);
      for (int index = 0; index < c.ports.Count; ++index)
        c.closePort(c.ports[index], "LOCAL_ADMIN");
      if (c.firewall != null)
      {
        c.firewall.resetSolutionProgress();
        c.firewall.solved = false;
      }
      if (c.hasProxy)
      {
        c.proxyActive = true;
        c.proxyOverloadTicks = c.startingOverloadTicks;
      }
      double time = 20.0 * Utils.random.NextDouble();
      Action action = (Action) (() =>
      {
        if (os.connectedComp != null && !(os.connectedComp.ip != c.ip))
          return;
        for (int index = 0; index < c.ports.Count; ++index)
          c.closePort(c.ports[index], "LOCAL_ADMIN");
        if (this.ResetsPassword)
          c.setAdminPassword(PortExploits.getRandomPassword());
        c.adminIP = c.ip;
        if (c.firewall != null)
          c.firewall.resetSolutionProgress();
      });
      if (this.IsSuper)
        action();
      else
        os.delayer.Post(ActionDelayer.Wait(time), action);
    }
  }
}
