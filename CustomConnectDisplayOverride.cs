// Decompiled with JetBrains decompiler
// Type: Hacknet.CustomConnectDisplayOverride
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  internal abstract class CustomConnectDisplayOverride : Daemon
  {
    public CustomConnectDisplayOverride(Computer c, string name, OS os)
      : base(c, name, os)
    {
      this.isListed = false;
    }
  }
}
