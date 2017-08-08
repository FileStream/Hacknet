// Decompiled with JetBrains decompiler
// Type: Hacknet.DebugLog
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;

namespace Hacknet
{
  public static class DebugLog
  {
    public static char[] delimiters = new char[2]{ '\n', '\r' };
    public static List<string> data = new List<string>(64);

    public static void add(string s)
    {
      string[] strArray = s.Split(DebugLog.delimiters);
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (!strArray[index].Equals(""))
          DebugLog.data.Add(strArray[index]);
      }
    }

    public static string GetDump()
    {
      string str = "";
      for (int index = 0; index < DebugLog.data.Count; ++index)
        str = str + DebugLog.data[index] + "\r\n";
      return str;
    }
  }
}
