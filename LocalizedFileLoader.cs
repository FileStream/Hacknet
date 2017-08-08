// Decompiled with JetBrains decompiler
// Type: Hacknet.LocalizedFileLoader
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.IO;
using System.Linq;
using System.Text;

namespace Hacknet
{
  public static class LocalizedFileLoader
  {
    public static string Read(string filepath)
    {
      return LocalizedFileLoader.FilterStringForLocalization(File.ReadAllText(LocalizedFileLoader.GetLocalizedFilepath(filepath)));
    }

    public static string GetLocalizedFilepath(string filepath)
    {
      filepath = filepath.Replace("\\", "/");
      string str = "Content/Locales/" + Settings.ActiveLocale;
      string path = filepath.Replace("Content/", str + "/");
      if (File.Exists(path))
        return path;
      return filepath;
    }

    public static string SafeFilterString(string data)
    {
      string source = "\r\n\t ";
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < data.Length; ++index)
      {
        if (GuiData.tinyfont.Characters.Contains(data[index]) || source.Contains<char>(data[index]))
          stringBuilder.Append(data[index]);
        else
          stringBuilder.Append("?");
      }
      return stringBuilder.ToString();
    }

    public static string FilterStringForLocalization(string data)
    {
      return data.Replace("&quot;", "'").Replace(" ", "").Replace("[PRÉNOM]#[NOM]#[NUM_DOSSIER]#32#Rural#N/A#N/A#N/A#[DERNIERS_MOTS]", "[FIRST_NAME]#[LAST_NAME]#[RECORD_NUM]#32#Rural#N/A#N/A#N/A#[LAST_WORDS]").Replace("[PRÉNOM]", "[FIRST_NAME]");
    }
  }
}
