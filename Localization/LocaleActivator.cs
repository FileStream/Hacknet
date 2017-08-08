// Decompiled with JetBrains decompiler
// Type: Hacknet.Localization.LocaleActivator
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hacknet.Localization
{
  public static class LocaleActivator
  {
    private static List<LocaleActivator.LanguageInfo> supportedLanguages = (List<LocaleActivator.LanguageInfo>) null;

    public static List<LocaleActivator.LanguageInfo> SupportedLanguages
    {
      get
      {
        if (LocaleActivator.supportedLanguages == null)
          LocaleActivator.LoadSupportedLocales();
        return LocaleActivator.supportedLanguages;
      }
    }

    private static void LoadSupportedLocales()
    {
      LocaleActivator.supportedLanguages = new List<LocaleActivator.LanguageInfo>();
      foreach (string str in Utils.readEntireFile("Content/Locales/SupportedLanguages.txt").Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries))
      {
        string[] strArray = str.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length > 2)
          LocaleActivator.supportedLanguages.Add(new LocaleActivator.LanguageInfo()
          {
            Name = strArray[0],
            Code = strArray[1],
            SteamCode = strArray[2]
          });
      }
    }

    public static void ActivateLocale(string localeCode, ContentManager content)
    {
      if (localeCode == "en-us")
      {
        LocaleTerms.ClearForEnUS();
      }
      else
      {
        string str1 = "Content/Locales/" + localeCode + "/UI_Terms.txt";
        string path = "Content/Locales/" + localeCode + "/Hacknet_UI_Terms.txt";
        if (!File.Exists(str1))
        {
          if (!File.Exists(path))
            throw new NotSupportedException("Locale " + localeCode + " does not exist or is not supported");
          str1 = path;
        }
        LocaleTerms.ReadInTerms(str1, true);
        if (DLC1SessionUpgrader.HasDLC1Installed)
        {
          string str2 = "Content/Locales/" + localeCode + "/DLC/Hacknet_UI_Terms.txt";
          if (File.Exists(str2))
            LocaleTerms.ReadInTerms(str2, false);
        }
      }
      Settings.ActiveLocale = localeCode;
      LocaleFontLoader.LoadFontConfigForLocale(localeCode, content);
      FileEntry.init(content);
    }

    public static bool ActiveLocaleIsCJK()
    {
      return Settings.ActiveLocale.StartsWith("zh") || Settings.ActiveLocale.StartsWith("ja") || Settings.ActiveLocale.StartsWith("ko");
    }

    public struct LanguageInfo
    {
      public string Name;
      public string Code;
      public string SteamCode;
    }
  }
}
