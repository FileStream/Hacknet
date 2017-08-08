// Decompiled with JetBrains decompiler
// Type: Hacknet.VehicleInfo
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;
using System.Text;

namespace Hacknet
{
  public static class VehicleInfo
  {
    public static List<VehicleType> vehicleTypes;

    public static void init()
    {
      string[] strArray1 = Utils.readEntireFile("Content/files/VehicleTypes.txt").Split(Utils.newlineDelim);
      char[] chArray = new char[1]{ '#' };
      VehicleInfo.vehicleTypes = new List<VehicleType>(strArray1.Length);
      for (int index = 0; index < strArray1.Length; ++index)
      {
        string[] strArray2 = strArray1[index].Split(chArray);
        VehicleInfo.vehicleTypes.Add(new VehicleType(strArray2[0], strArray2[1]));
      }
    }

    public static VehicleRegistration getRandomRegistration()
    {
      int index1 = Utils.random.Next(VehicleInfo.vehicleTypes.Count);
      VehicleType vehicleType = VehicleInfo.vehicleTypes[index1];
      string plate = ((int) Utils.getRandomLetter() + (int) Utils.getRandomLetter() + (int) Utils.getRandomLetter()).ToString() + "-" + (object) Utils.getRandomLetter() + (object) Utils.getRandomLetter() + (object) Utils.getRandomLetter();
      StringBuilder stringBuilder = new StringBuilder();
      int num1 = 12;
      int num2 = 4;
      for (int index2 = 0; index2 < num1; ++index2)
      {
        if (index2 % num2 == 0 && index2 > 0)
          stringBuilder.Append('-');
        else
          stringBuilder.Append(Utils.getRandomChar());
      }
      string regNumber = stringBuilder.ToString();
      return new VehicleRegistration(vehicleType, plate, regNumber);
    }
  }
}
