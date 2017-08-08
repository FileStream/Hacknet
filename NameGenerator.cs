// Decompiled with JetBrains decompiler
// Type: Hacknet.NameGenerator
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;

namespace Hacknet
{
  internal static class NameGenerator
  {
    public static List<string> main;
    public static List<string> postfix;

    public static void init()
    {
      NameGenerator.main = new List<string>();
      NameGenerator.main.Add("Holopoint");
      NameGenerator.main.Add("Ascendant");
      NameGenerator.main.Add("Enabled");
      NameGenerator.main.Add("Subversion");
      NameGenerator.main.Add("Introversion");
      NameGenerator.main.Add("Photonic");
      NameGenerator.main.Add("Enlightened");
      NameGenerator.main.Add("Software");
      NameGenerator.main.Add("Facespace");
      NameGenerator.main.Add("Mott");
      NameGenerator.main.Add("Starchip");
      NameGenerator.main.Add("Macrosoft");
      NameGenerator.main.Add("Oppol");
      NameGenerator.main.Add("Octovision");
      NameGenerator.main.Add("tijital");
      NameGenerator.main.Add("Valence");
      NameGenerator.main.Add("20%Cooler");
      NameGenerator.main.Add("Celestia");
      NameGenerator.main.Add("Manic");
      NameGenerator.main.Add("Dengler");
      NameGenerator.main.Add("Beagle");
      NameGenerator.main.Add("Warden");
      NameGenerator.main.Add("Phoenix");
      NameGenerator.main.Add("Banished Stallion");
      NameGenerator.postfix = new List<string>();
      NameGenerator.postfix.Add(" Inc");
      NameGenerator.postfix.Add(" Interactive");
      NameGenerator.postfix.Add(".com");
      NameGenerator.postfix.Add(" Internal");
      NameGenerator.postfix.Add(" Software");
      NameGenerator.postfix.Add(" Technologies");
      NameGenerator.postfix.Add(" Tech");
      NameGenerator.postfix.Add(" Solutions");
      NameGenerator.postfix.Add(" Enterprises");
      NameGenerator.postfix.Add(" Studios");
      NameGenerator.postfix.Add(" Consortium");
      NameGenerator.postfix.Add(" Communications");
    }

    public static string generateName()
    {
      return "" + NameGenerator.main[Utils.random.Next(0, NameGenerator.main.Count)] + NameGenerator.postfix[Utils.random.Next(0, NameGenerator.postfix.Count)];
    }

    public static string[] generateCompanyName()
    {
      return new string[2]{ NameGenerator.main[Utils.random.Next(0, NameGenerator.main.Count)], NameGenerator.postfix[Utils.random.Next(0, NameGenerator.postfix.Count)] };
    }

    public static string getRandomMain()
    {
      return NameGenerator.main[Utils.random.Next(0, NameGenerator.main.Count)];
    }
  }
}
