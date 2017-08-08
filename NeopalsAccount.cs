// Decompiled with JetBrains decompiler
// Type: Hacknet.NeopalsAccount
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet
{
  public class NeopalsAccount
  {
    public string AccountName;
    public long NeoPoints;
    public long BankedPoints;
    public string InventoryID;
    public List<Neopal> Pets;

    public static NeopalsAccount GenerateAccount(string handle, bool isActiveUser = false)
    {
      NeopalsAccount neopalsAccount = new NeopalsAccount() { AccountName = handle, NeoPoints = isActiveUser ? (long) Utils.random.Next(50000) : (long) Utils.random.Next(5000), BankedPoints = isActiveUser ? (long) Utils.random.Next(10000) : (long) Utils.random.Next(2000), InventoryID = Guid.NewGuid().ToString(), Pets = new List<Neopal>() };
      int num = 1 + Utils.random.Next(2);
      if (num == 2 && Utils.flipCoin() && Utils.flipCoin())
        ++num;
      for (int index = 0; index < num; ++index)
        neopalsAccount.Pets.Add(Neopal.GeneratePet(isActiveUser));
      return neopalsAccount;
    }

    public override string ToString()
    {
      return this.AccountName + "_x" + (object) this.Pets.Count;
    }
  }
}
