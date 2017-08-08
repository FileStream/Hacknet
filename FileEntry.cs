// Decompiled with JetBrains decompiler
// Type: Hacknet.FileEntry
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hacknet
{
  public class FileEntry : FileType
  {
    public static List<string> filenames;
    public static List<string> fileData;
    public string name;
    public string data;
    public int size;
    public int secondCreatedAt;

    public FileEntry()
    {
      int index = Utils.random.Next(0, FileEntry.filenames.Count - 1);
      this.name = FileEntry.filenames[index];
      this.data = FileEntry.fileData[index];
      this.size = this.data.Length * 8;
      this.secondCreatedAt = (int) OS.currentElapsedTime;
    }

    public FileEntry(string dataEntry, string nameEntry)
    {
      nameEntry = nameEntry.Replace(" ", "_");
      this.name = nameEntry;
      this.data = dataEntry;
      this.size = this.data.Length * 8;
      this.secondCreatedAt = (int) OS.currentElapsedTime;
    }

    public string head()
    {
      int index = 0;
      string str = "";
      for (; index < this.data.Length && (int) this.data[index] != 10 && index < 50; ++index)
        str += (string) (object) this.data[index];
      return str;
    }

    public string getName()
    {
      return this.name;
    }

    public static void init(ContentManager content)
    {
      FileEntry.filenames = new List<string>(128);
      FileEntry.fileData = new List<string>(128);
      FileInfo[] files = new DirectoryInfo(Path.Combine(content.RootDirectory, "files")).GetFiles("*.*");
      for (int index = 0; index < files.Length; ++index)
      {
        FileEntry.filenames.Add(Path.GetFileNameWithoutExtension(files[index].Name));
        string filename = "Content/files/" + Path.GetFileName(files[index].Name);
        FileEntry.fileData.Add(Utils.readEntireFile(filename));
      }
      string[] strArray = Utils.readEntireFile(Settings.EducationSafeBuild ? "Content/BashLogs_StudentSafe.txt" : "Content/BashLogs.txt").Split(new string[1]{ "\n#" }, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray.Length; ++index)
      {
        strArray[index].Trim();
        int num = strArray[index].Length - strArray[index].IndexOf("\r\n");
        FileEntry.filenames.Add("IRC_Log:" + strArray[index].Substring(0, strArray[index].IndexOf("\r\n")).Replace("- [X]", "").Replace(" ", ""));
        string data = strArray[index].Substring(strArray[index].IndexOf("\r\n")).Replace("\n ", "\n");
        if (Settings.ActiveLocale == "en-us")
          data = FileSanitiser.purifyStringForDisplay(data);
        FileEntry.fileData.Add(data + "\n\nArchived Via : http://Bash.org");
      }
    }
  }
}
