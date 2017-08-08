// Decompiled with JetBrains decompiler
// Type: Hacknet.MissionGenerationParser
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public static class MissionGenerationParser
  {
    public static string Path;
    public static string File;
    public static string Comp;
    public static string Client;
    public static string Target;
    public static string Other;

    public static void init()
    {
      string str;
      MissionGenerationParser.Target = str = "UNKNOWN";
      MissionGenerationParser.Client = str;
      MissionGenerationParser.File = str;
      MissionGenerationParser.Path = str;
      MissionGenerationParser.Comp = "firstGeneratedNode";
    }

    public static string parse(string input)
    {
      return input.Replace("#PATH#", MissionGenerationParser.Path).Replace("#FILE#", MissionGenerationParser.File).Replace("#COMP#", MissionGenerationParser.Comp).Replace("#CLIENT#", MissionGenerationParser.Client).Replace("#TARGET#", MissionGenerationParser.Target).Replace("#OTHER#", MissionGenerationParser.Other).Replace("#LC_CLIENT#", MissionGenerationParser.Client.Replace(' ', '_').ToLower());
    }
  }
}
