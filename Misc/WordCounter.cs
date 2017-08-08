// Decompiled with JetBrains decompiler
// Type: Hacknet.Misc.WordCounter
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.IO;

namespace Hacknet.Misc
{
  public class WordCounter
  {
    private static string accum;
    private static int charAccum;

    public static void PerformWordCount(string[] folders, string[] fileOnlyFolders)
    {
      int num = 0;
      WordCounter.accum = "";
      WordCounter.charAccum = 0;
      for (int index1 = 0; index1 < folders.Length; ++index1)
      {
        string[] directories = Directory.GetDirectories(folders[index1]);
        num += WordCounter.GetWordCountFromFolder(folders[index1]);
        for (int index2 = 0; index2 < directories.Length; ++index2)
          num += WordCounter.GetWordCountFromFolder(directories[index2]);
      }
      for (int index = 0; index < fileOnlyFolders.Length; ++index)
        num += WordCounter.GetWordCountFromFolder(fileOnlyFolders[index]);
      Console.WriteLine("--------------\n\nWORD COUNT COMPLETE::\n\n");
      Console.WriteLine("Total Words: " + (object) num + "\nTotal Chars: " + (object) WordCounter.charAccum + "\n\n");
      WordCounter.accum = WordCounter.accum + "\r\n---------------\r\nTotal Count: " + (object) num + "\r\nChars: " + (object) WordCounter.charAccum + "\r\n";
      File.WriteAllText("WordCount.txt", WordCounter.accum);
    }

    private static int GetWordCountFromFolder(string folderpath)
    {
      if (folderpath.EndsWith("Untranslated"))
        return 0;
      string[] files = Directory.GetFiles(folderpath);
      int num1 = 0;
      for (int index = 0; index < files.Length; ++index)
      {
        int num2 = 0;
        string path = files[index];
        if (path.EndsWith(".xml"))
        {
          Console.WriteLine("Reading " + path);
          try
          {
            int num3 = num2 + WordCounter.GetTextCountFromXMLFile(path);
            num1 += num3;
            WordCounter.accum += string.Format("COMPLETE ({0}) : {1}\r\n", (object) num3, (object) path);
          }
          catch (Exception ex)
          {
            WordCounter.accum = WordCounter.accum + "ERROR: Could not process " + path + "\r\n";
          }
        }
        else if (path.EndsWith(".txt"))
        {
          Console.WriteLine("Reading " + path);
          string input = File.ReadAllText(path);
          WordCounter.charAccum += input.Length;
          int num3 = num2 + WordCounter.CountString(input);
          num1 += num3;
          Console.Write("...Complete\n");
          WordCounter.accum += string.Format("COMPLETE ({0}) : {1}\r\n", (object) num3, (object) path);
        }
      }
      return num1;
    }

    private static int CountString(string input)
    {
      char[] separator = new char[7]{ ' ', '.', ',', ';', ':', '\n', '\t' };
      return input.Split(separator, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static int GetWordCountFromComputer(Computer c)
    {
      int num = 0 + WordCounter.GetWordCountFromFolder(c.files.root);
      WordCounter.charAccum += c.name.Length;
      return num + WordCounter.CountString(c.name);
    }

    private static int GetWordCountFromFolder(Folder f)
    {
      int num = 0;
      if (f.name == "sys")
        return num;
      for (int index = 0; index < f.folders.Count; ++index)
        num += WordCounter.GetWordCountFromFolder(f.folders[index]);
      for (int index = 0; index < f.files.Count; ++index)
      {
        WordCounter.charAccum += f.files[index].name.Length;
        WordCounter.charAccum += f.files[index].data.Length;
        num = num + WordCounter.CountString(f.files[index].name) + WordCounter.CountString(f.files[index].data);
      }
      return num;
    }

    private static int GetWordCountFromMission(ActiveMission m)
    {
      int num = 0;
      WordCounter.charAccum += m.email.body.Length;
      WordCounter.charAccum += m.email.subject.Length;
      WordCounter.charAccum += m.postingBody.Length;
      WordCounter.charAccum += m.postingTitle.Length;
      return num + WordCounter.CountString(m.email.body) + WordCounter.CountString(m.email.subject) + WordCounter.CountString(m.postingBody) + WordCounter.CountString(m.postingTitle);
    }

    private static int GetTextCountFromXMLFile(string path)
    {
      throw new NotSupportedException("Only Supported in XNA!");
    }
  }
}
