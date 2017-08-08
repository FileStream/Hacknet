// Decompiled with JetBrains decompiler
// Type: Hacknet.LocaleTerms
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.IO;

namespace Hacknet
{
  public static class LocaleTerms
  {
    public static Dictionary<string, string> ActiveTerms = new Dictionary<string, string>();
    private const string NewlineReplacer = "[%\\n%]";

    public static void ReadInTerms(string termsFilepath, bool clearPreviouslyLoadedTerms = true)
    {
      char[] separator = new char[1]{ '\t' };
      if (clearPreviouslyLoadedTerms)
        LocaleTerms.ActiveTerms.Clear();
      string[] strArray1 = File.ReadAllLines(termsFilepath);
      for (int index = 1; index < strArray1.Length; ++index)
      {
        if (strArray1[index].StartsWith("\t"))
        {
          string[] strArray2 = strArray1[index].Split(separator, StringSplitOptions.RemoveEmptyEntries);
          if (strArray2.Length > 1 && !LocaleTerms.ActiveTerms.ContainsKey(strArray2[0]) && !LocaleTerms.ActiveTerms.ContainsKey(LocaleTerms.RemoveQuotes(strArray2[0])))
          {
            string input = strArray2[1].Replace("[%\\n%]", "\n");
            LocaleTerms.ActiveTerms.Add(LocaleTerms.RemoveQuotes(strArray2[0]), LocaleTerms.RemoveQuotes(input));
          }
        }
      }
    }

    private static string RemoveQuotes(string input)
    {
      if (input.StartsWith("\""))
        input = input.Substring(1);
      if (input.EndsWith("\""))
        input = input.Substring(0, input.Length - 1);
      return input;
    }

    public static void ClearForEnUS()
    {
      LocaleTerms.ActiveTerms.Clear();
    }

    public static string Loc(string input)
    {
      if (Settings.ActiveLocale == "en-us" || !LocaleTerms.ActiveTerms.ContainsKey(input))
        return input;
      return LocaleTerms.ActiveTerms[input];
    }
  }
}
