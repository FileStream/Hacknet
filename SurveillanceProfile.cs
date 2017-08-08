// Decompiled with JetBrains decompiler
// Type: Hacknet.SurveillanceProfile
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public class SurveillanceProfile
  {
    public string Name;
    public string Age;
    public string HomeCity;
    public string Notes;
    public string CriminalRecord;

    public override string ToString()
    {
      return this.Name;
    }
  }
}
