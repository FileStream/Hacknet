// Decompiled with JetBrains decompiler
// Type: Hacknet.OnlineAccount
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public class OnlineAccount
  {
    public int ID = 0;
    public string Username;
    public string BanStatus;
    public string Notes;

    public override string ToString()
    {
      return this.Username + "#" + (object) this.ID;
    }
  }
}
