// Decompiled with JetBrains decompiler
// Type: Hacknet.Account
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public class Account
  {
    public string ID;
    public string Cash;
    public string Bank;
    public string Apartments;
    public string Vehicles;
    public string PegasusVehicles;
    public string Rank;
    public string RP;
    public string Kills;

    public override string ToString()
    {
      return this.ID;
    }
  }
}
