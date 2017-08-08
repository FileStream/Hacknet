// Decompiled with JetBrains decompiler
// Type: Hacknet.CAROData
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public class CAROData
  {
    public string UserID;
    public string Headshots;
    public string Kills;
    public string Rank;
    public string Crowbars;
    public string InventoryID;
    public string BanStatus;

    public override string ToString()
    {
      return this.UserID;
    }
  }
}
