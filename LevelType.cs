// Decompiled with JetBrains decompiler
// Type: Hacknet.LevelType
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public struct LevelType
  {
    public int NumOfPuzzles;
    public int NumOfBackgrounds;
    public string name;

    public LevelType(int puzzles, int bgs, string lvlname)
    {
      this.NumOfPuzzles = puzzles;
      this.NumOfBackgrounds = bgs;
      this.name = lvlname;
    }
  }
}
