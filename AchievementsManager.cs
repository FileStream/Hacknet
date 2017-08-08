// Decompiled with JetBrains decompiler
// Type: Hacknet.AchievementsManager
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Steamworks;

namespace Hacknet
{
  public static class AchievementsManager
  {
    public static bool Unlock(string name, bool recordAndCheckFlag = false)
    {
      try
      {
        string flag = name + "_Unlocked";
        if (recordAndCheckFlag && OS.currentInstance.Flags.HasFlag(flag))
          return false;
        SteamUserStats.SetAchievement(name);
        if (!SteamUserStats.StoreStats())
          return false;
        OS.currentInstance.Flags.AddFlag(flag);
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}
