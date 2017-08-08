// Decompiled with JetBrains decompiler
// Type: Hacknet.UIUtils.AudioUtils
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.IO;

namespace Hacknet.UIUtils
{
  internal class AudioUtils
  {
    public static void openWav(string filename, out double[] left, out double[] right)
    {
      byte[] numArray = File.ReadAllBytes(filename);
      int num1 = (int) numArray[22];
      int index1;
      int index2;
      int num2;
      for (index1 = 12; (int) numArray[index1] != 100 || (int) numArray[index1 + 1] != 97 || (int) numArray[index1 + 2] != 116 || (int) numArray[index1 + 3] != 97; index1 = index2 + (4 + num2))
      {
        index2 = index1 + 4;
        num2 = (int) numArray[index2] + (int) numArray[index2 + 1] * 256 + (int) numArray[index2 + 2] * 65536 + (int) numArray[index2 + 3] * 16777216;
      }
      int index3 = index1 + 8;
      int length = (numArray.Length - index3) / 2;
      if (num1 == 2)
        length /= 2;
      left = new double[length];
      right = num1 != 2 ? (double[]) null : new double[length];
      int index4 = 0;
      while (index3 < numArray.Length)
      {
        left[index4] = AudioUtils.bytesToDouble(numArray[index3], numArray[index3 + 1]);
        index3 += 2;
        if (num1 == 2)
        {
          right[index4] = AudioUtils.bytesToDouble(numArray[index3], numArray[index3 + 1]);
          index3 += 2;
        }
        ++index4;
      }
    }

    private static double bytesToDouble(byte firstByte, byte secondByte)
    {
      return (double) (short) ((int) secondByte << 8 | (int) firstByte) / 32768.0;
    }
  }
}
