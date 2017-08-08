// Decompiled with JetBrains decompiler
// Type: Hacknet.FileSanitiser
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet
{
  public static class FileSanitiser
  {
    public static string purifyStringForDisplay(string data)
    {
      if (data == null)
        return (string) null;
      data = data.Replace("\t", "    ").Replace("“", "\"").Replace("”", "\"");
      for (int index = 0; index < data.Length; ++index)
      {
        if (((int) data[index] < 32 || (int) data[index] > (int) sbyte.MaxValue) && ((int) data[index] != 10 && (int) data[index] != 11 && (int) data[index] != 12 && (int) data[index] != 10))
          data = data.Replace(data[index], ' ');
        if (GuiData.font != null && (!GuiData.font.Characters.Contains(data[index]) && (int) data[index] != 10))
          data = data.Replace(data[index], '_');
      }
      return data;
    }

    public static void purifyVehicleFile(string path)
    {
      string data = Utils.readEntireFile(path).Replace('\t', '#').Replace("\r", "");
      for (int index = 0; index < data.Length; ++index)
      {
        if (!GuiData.font.Characters.Contains(data[index]) && (int) data[index] != 10)
          data = FileSanitiser.replaceChar(data, index, '_');
      }
      Utils.writeToFile(data, "SanitisedFile.txt");
    }

    public static string replaceChar(string data, int index, char replacer)
    {
      return data.Substring(0, index - 1) + (object) replacer + data.Substring(index + 1, data.Length - index - 2);
    }

    public static void purifyNameFile(string path)
    {
      string[] strArray1 = Utils.readEntireFile(path).Split(Utils.newlineDelim);
      string data = "";
      for (int index = 0; index < strArray1.Length; ++index)
      {
        string[] strArray2 = strArray1[index].Split(Utils.spaceDelim, StringSplitOptions.RemoveEmptyEntries);
        data = data + strArray2[0] + "\n";
      }
      Utils.writeToFile(data, "SanitisedNameFile.txt");
    }

    public static void purifyLocationFile(string path)
    {
      string str = Utils.readEntireFile(path);
      char[] separator = new char[1]{ '\t' };
      string[] strArray1 = str.Split(Utils.newlineDelim);
      string data = "";
      for (int index1 = 1; index1 < strArray1.Length; ++index1)
      {
        string[] strArray2 = strArray1[index1].Split(separator, StringSplitOptions.RemoveEmptyEntries);
        for (int index2 = 1; index2 < strArray2.Length; ++index2)
          data = data + strArray2[index2].Trim() + "#";
        data += "\n";
      }
      Utils.writeToFile(data, "SanitisedLocFile.txt");
    }
  }
}
