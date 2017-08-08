// Decompiled with JetBrains decompiler
// Type: Hacknet.WorldLocationLoader
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Hacknet
{
  public static class WorldLocationLoader
  {
    public static List<WorldLocation> locations;

    public static void init()
    {
      CultureInfo cultureInfo = new CultureInfo("en-au");
      string[] strArray1 = Utils.readEntireFile("Content/PersonData/LocationData.txt").Split(Utils.newlineDelim);
      char[] chArray = new char[1]{ '#' };
      WorldLocationLoader.locations = new List<WorldLocation>(strArray1.Length);
      for (int index = 0; index < strArray1.Length; ++index)
      {
        string[] strArray2 = strArray1[index].Split(chArray);
        WorldLocationLoader.locations.Add(new WorldLocation(strArray2[1], strArray2[0], (float) Convert.ToDouble(strArray2[2], (IFormatProvider) cultureInfo), (float) Convert.ToDouble(strArray2[3], (IFormatProvider) cultureInfo), (float) Convert.ToDouble(strArray2[4], (IFormatProvider) cultureInfo), (float) Convert.ToDouble(strArray2[5], (IFormatProvider) cultureInfo)));
      }
    }

    public static WorldLocation getRandomLocation()
    {
      return WorldLocationLoader.locations[Utils.random.Next(WorldLocationLoader.locations.Count)];
    }

    public static WorldLocation getClosestOrCreate(string name)
    {
      for (int index = 0; index < WorldLocationLoader.locations.Count; ++index)
      {
        if (WorldLocationLoader.locations[index].name.ToLower().Equals(name.ToLower()))
          return WorldLocationLoader.locations[index];
      }
      WorldLocation worldLocation = new WorldLocation(name, name, (float) Utils.random.NextDouble(), (float) Utils.random.NextDouble(), (float) Utils.random.NextDouble(), (float) Utils.random.NextDouble());
      WorldLocationLoader.locations.Add(worldLocation);
      return worldLocation;
    }
  }
}
