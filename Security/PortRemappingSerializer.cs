// Decompiled with JetBrains decompiler
// Type: Hacknet.Security.PortRemappingSerializer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet.Security
{
  public static class PortRemappingSerializer
  {
    private static char[] EqualsDelimiter = new char[1]{ '=' };

    public static string GetSaveString(Dictionary<int, int> input)
    {
      if (input == null || input.Count == 0)
        return "";
      string str = "<portRemap>";
      foreach (KeyValuePair<int, int> keyValuePair in input)
        str = str + (object) keyValuePair.Key + "=" + (object) keyValuePair.Value + ",";
      return str.Substring(0, str.Length - 1) + "</portRemap>\n";
    }

    public static Dictionary<int, int> Deserialize(string input)
    {
      if (string.IsNullOrWhiteSpace(input))
        return (Dictionary<int, int>) null;
      Dictionary<int, int> dictionary = new Dictionary<int, int>();
      foreach (string str in input.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries))
      {
        string[] strArray = str.Trim().Split(PortRemappingSerializer.EqualsDelimiter, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length >= 2)
        {
          string lower = strArray[0].ToLower();
          int key;
          switch (lower)
          {
            case "ftp":
              key = 21;
              break;
            case "web":
              key = 80;
              break;
            case "ssh":
              key = 22;
              break;
            case "smtp":
              key = 25;
              break;
            case "sql":
              key = 1433;
              break;
            case "medical":
              key = 104;
              break;
            case "torrent":
              key = 6881;
              break;
            case "ssl":
              key = 443;
              break;
            default:
              key = Convert.ToInt32(lower);
              break;
          }
          dictionary.Add(key, Convert.ToInt32(strArray[1]));
        }
      }
      return dictionary;
    }
  }
}
