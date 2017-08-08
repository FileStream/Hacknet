// Decompiled with JetBrains decompiler
// Type: Hacknet.Neopal
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet
{
  public class Neopal
  {
    public Neopal.PetType Type;
    public string Name;
    public int DaysSinceFed;
    public byte CombatRating;
    public float Happiness;
    public string Identifier;
    private static string[] PossibleNames;

    private static string GenerateName()
    {
      if (Neopal.PossibleNames == null)
        Neopal.PossibleNames = Utils.readEntireFile("Content/DLC/Docs/Untranslated/NeopalNames.txt").Split(new string[5]
        {
          "\r\n",
          "\n",
          ", ",
          " ,",
          ","
        }, StringSplitOptions.RemoveEmptyEntries);
      return Neopal.PossibleNames[Utils.random.Next(Neopal.PossibleNames.Length)];
    }

    public static Neopal GeneratePet(bool isActiveUser = false)
    {
      isActiveUser = isActiveUser || (double) Utils.randm(1f) < 0.0199999995529652;
      Array values = Enum.GetValues(typeof (Neopal.PetType));
      Neopal.PetType petType = (Neopal.PetType) values.GetValue(Utils.random.Next(values.Length));
      return new Neopal() { CombatRating = (byte) Utils.random.Next((int) byte.MaxValue), DaysSinceFed = isActiveUser ? Utils.random.Next(2) : Utils.random.Next(3650), Happiness = isActiveUser ? 1f - Utils.randm(0.15f) : Utils.randm(0.05f), Name = Neopal.GenerateName(), Type = petType, Identifier = Guid.NewGuid().ToString().Substring(0, 13) };
    }

    public enum PetType
    {
      Blundo,
      Chisha,
      Jubdub,
      Kachici,
      Kyrill,
      Myncl,
      Pageri,
      Psybunny,
      Scorchum,
      Unisam,
    }
  }
}
