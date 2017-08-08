// Decompiled with JetBrains decompiler
// Type: Hacknet.NotesDumperExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet
{
  internal static class NotesDumperExe
  {
    public static void RunNotesDumperExe(string[] args, object osObj, Computer target)
    {
      OS os = (OS) osObj;
      FileEntry fileEntry = os.thisComputer.files.root.searchForFolder("home").searchForFile("Notes.txt");
      if (fileEntry == null)
      {
        os.write(LocaleTerms.Loc("Dump Notes Output:") + "_______________\n");
        os.write(" ");
        os.write(LocaleTerms.Loc("ERROR: No notes found on home system!"));
        os.write("_______________________________");
        os.write(" ");
      }
      else
      {
        string[] strArray = fileEntry.data.Split(new string[1]{ "\n\n----------\n\n" }, StringSplitOptions.RemoveEmptyEntries);
        os.write(" ");
        os.write(LocaleTerms.Loc("Notes") + ":________________________");
        os.write(" ");
        for (int index = 0; index < strArray.Length; ++index)
        {
          os.write(strArray[index]);
          os.write("______________________________");
          os.write(" ");
        }
      }
    }
  }
}
