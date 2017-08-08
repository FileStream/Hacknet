// Decompiled with JetBrains decompiler
// Type: Hacknet.PeopleAssets
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet
{
  internal class PeopleAssets
  {
    private static string[] degreeTitles = new string[3]{ "Bachelor of ", "Masters in ", "PHD in " };
    private static string[] degreeNames = new string[12]{ "Computer Science", "Business", "Electrical Engineering", "Finance", "Marketing", "The Arts", "Computer Graphics", "Design", "Medicine", "Pharmacy", "Information Technology", "Psychology" };
    private static string[] hackerDegreeNames = new string[5]{ "Computer Science", "Digital Security", "Computer Networking", "Information Technology", "Computer Graphics" };

    public static Degree getRandomDegree(WorldLocation origin)
    {
      Degree degree = new Degree();
      degree.name = PeopleAssets.randOf(PeopleAssets.degreeTitles) + PeopleAssets.randOf(PeopleAssets.degreeNames);
      degree.GPA = (float) (3.0 + 3.0 * (Utils.random.NextDouble() - 0.5) * 0.5);
      while ((double) degree.GPA > 4.0)
        degree.GPA -= Utils.randm(0.4f);
      degree.uni = "University of " + origin.name;
      if (Utils.flipCoin())
        degree.uni = origin.name + " University";
      return degree;
    }

    public static Degree getRandomHackerDegree(WorldLocation origin)
    {
      Degree degree = new Degree();
      degree.name = PeopleAssets.randOf(PeopleAssets.degreeTitles) + PeopleAssets.randOf(PeopleAssets.hackerDegreeNames);
      degree.GPA = (float) (3.0 + 5.0 * (Utils.random.NextDouble() - 0.5) * 0.5);
      degree.uni = "University of " + origin.name;
      if (Utils.flipCoin())
        degree.uni = origin.name + " University";
      return degree;
    }

    public static string randOf(string[] array)
    {
      int index = (int) (Math.Max(0.0001, Utils.random.NextDouble()) * (double) array.Length - 1.0);
      return array[index];
    }
  }
}
