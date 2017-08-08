// Decompiled with JetBrains decompiler
// Type: Hacknet.DLC.MarkovTextGenerator.PotentialEntry
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet.DLC.MarkovTextGenerator
{
  public class PotentialEntry
  {
    public string word;
    public double weighting;

    public override string ToString()
    {
      return this.word + " %" + this.weighting.ToString("0.00");
    }
  }
}
