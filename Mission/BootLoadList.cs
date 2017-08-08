// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.BootLoadList
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
  public static class BootLoadList
  {
    public static List<string> getList()
    {
      string str = "";
      if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed)
        str = str + "\r\n" + Utils.readEntireFile("Content/DLC/DLCBootList.txt");
      return BootLoadList.getListFromData(str + Utils.readEntireFile("Content/Computers/BootLoadList.txt"));
    }

    public static List<string> getDLCList()
    {
      return BootLoadList.getListFromData(Utils.readEntireFile("Content/DLC/DLCBootList.txt"));
    }

    public static List<string> getDemoList()
    {
      return BootLoadList.getListFromData(Utils.readEntireFile("Content/Computers/DemoLoadList.txt"));
    }

    public static List<string> getAdventureList()
    {
      return BootLoadList.getListFromData(Utils.readEntireFile("Content/AdventureNetwork/AdventureLoadList.txt"));
    }

    private static List<string> getListFromData(string data)
    {
      string[] separator = new string[2]{ "\n\r", "\r\n" };
      string[] strArray = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      List<string> stringList = new List<string>();
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (!strArray[index].StartsWith("#") && strArray[index].Length > 1)
          stringList.Add(strArray[index]);
      }
      return stringList;
    }
  }
}
