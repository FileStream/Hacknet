// Decompiled with JetBrains decompiler
// Type: Hacknet.StatsManager
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Steamworks;

namespace Hacknet
{
  public static class StatsManager
  {
    private static bool HasReceivedUserStats = false;
    private static StatsManager.StatData[] StatDefinitions = new StatsManager.StatData[1]{ new StatsManager.StatData() { Name = "commands_run" } };
    private static Callback<UserStatsReceived_t> UserStatsCallback;

    public static void InitStats()
    {
      StatsManager.UserStatsCallback = Callback<UserStatsReceived_t>.Create(new Callback<UserStatsReceived_t>.DispatchDelegate(StatsManager.OnUserStatsReceived));
    }

    private static void OnUserStatsReceived(UserStatsReceived_t pCallback)
    {
      int num = 0;
      while (num < StatsManager.StatDefinitions.Length)
        ++num;
    }

    public static void IncrementStat(string statName, int valueChange)
    {
      if (!StatsManager.HasReceivedUserStats)
        return;
      SteamUserStats.SetStat(statName, valueChange);
    }

    public static void SaveStatProgress()
    {
      if (!StatsManager.HasReceivedUserStats)
        return;
      SteamUserStats.StoreStats();
    }

    private struct StatData
    {
      public string Name;
      public int IntVal;
      public float FloatVal;
    }
  }
}
