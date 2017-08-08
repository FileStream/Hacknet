// Decompiled with JetBrains decompiler
// Type: Hacknet.WorldLocation
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public class WorldLocation
  {
    public string country;
    public string name;
    public float educationLevel;
    public float lifeLevel;
    public float employerLevel;
    public float affordabilityLevel;

    public WorldLocation()
    {
    }

    public WorldLocation(string countryName, string locName, float education, float life, float employer, float affordability)
    {
      this.country = countryName;
      this.name = locName;
      this.educationLevel = education;
      this.lifeLevel = life;
      this.employerLevel = employer;
      this.affordabilityLevel = affordability;
    }

    public new string ToString()
    {
      return this.name + ", " + this.country;
    }
  }
}
