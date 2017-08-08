// Decompiled with JetBrains decompiler
// Type: Hacknet.Degree
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public class Degree
  {
    public string name;
    public string uni;
    public float GPA;

    public Degree()
    {
    }

    public Degree(string degreeName, string uniName, float degreeGPA)
    {
      this.name = degreeName;
      this.uni = uniName;
      this.GPA = degreeGPA;
    }

    public override string ToString()
    {
      return this.name + " from " + this.uni + ". GPA: " + (object) this.GPA;
    }
  }
}
