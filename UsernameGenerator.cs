// Decompiled with JetBrains decompiler
// Type: Hacknet.UsernameGenerator
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace Hacknet
{
  public static class UsernameGenerator
  {
    private static string[] delims = new string[1]{ "\r\n\r\n" };
    public static string[] names;
    private static int nameIndex;

    public static void init()
    {
      StreamReader streamReader = new StreamReader(TitleContainer.OpenStream("Content/Usernames.txt"));
      string end = streamReader.ReadToEnd();
      streamReader.Close();
      UsernameGenerator.names = end.Split(UsernameGenerator.delims, StringSplitOptions.RemoveEmptyEntries);
      UsernameGenerator.nameIndex = (int) (Utils.random.NextDouble() * (double) (UsernameGenerator.names.Length - 1));
    }

    public static string getName()
    {
      UsernameGenerator.nameIndex = (UsernameGenerator.nameIndex + 1) % (UsernameGenerator.names.Length - 1);
      return UsernameGenerator.names[UsernameGenerator.nameIndex];
    }
  }
}
