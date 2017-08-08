// Decompiled with JetBrains decompiler
// Type: Hacknet.FastProgressOnlyAdministrator
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  internal class FastProgressOnlyAdministrator : Administrator
  {
    public override void traceEjectionDetected(Computer c, OS os)
    {
      base.traceEjectionDetected(c, os);
      this.disconnectionDetected(c, os);
    }

    public override void disconnectionDetected(Computer c, OS os)
    {
      base.disconnectionDetected(c, os);
      if (!(c.adminIP != os.thisComputer.adminIP))
        return;
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
    }
  }
}
