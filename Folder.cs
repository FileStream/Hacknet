// Decompiled with JetBrains decompiler
// Type: Hacknet.Folder
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;
using System.Xml;

namespace Hacknet
{
  public class Folder : FileType
  {
    private static string ampersandReplacer = "|##AMP##|";
    private static string backslashReplacer = "|##BS##|";
    private static string rightSBReplacer = "|##RSB##|";
    private static string leftSBReplacer = "|##LSB##|";
    private static string rightABReplacer = "|##RAB##|";
    private static string leftABReplacer = "|##LAB##|";
    private static string quoteReplacer = "|##QOT##|";
    private static string singlequoteReplacer = "|##SIQ##|";
    public List<FileEntry> files = new List<FileEntry>();
    public List<Folder> folders = new List<Folder>();
    public string name;

    public Folder(string foldername)
    {
      this.name = foldername;
    }

    public string getName()
    {
      return this.name;
    }

    public bool containsFile(string name, string data)
    {
      for (int index = 0; index < this.files.Count; ++index)
      {
        if (this.files[index].name.Equals(name) && this.files[index].data.Equals(data))
          return true;
      }
      return false;
    }

    public bool containsFileWithData(string data)
    {
      for (int index = 0; index < this.files.Count; ++index)
      {
        if (this.files[index].data.Equals(data))
          return true;
      }
      return false;
    }

    public bool containsFile(string name)
    {
      for (int index = 0; index < this.files.Count; ++index)
      {
        if (this.files[index].name.Equals(name))
          return true;
      }
      return false;
    }

    public Folder searchForFolder(string folderName)
    {
      for (int index = 0; index < this.folders.Count; ++index)
      {
        if (this.folders[index].name == folderName)
          return this.folders[index];
      }
      return (Folder) null;
    }

    public FileEntry searchForFile(string fileName)
    {
      for (int index = 0; index < this.files.Count; ++index)
      {
        if (this.files[index].name == fileName)
          return this.files[index];
      }
      return (FileEntry) null;
    }

    public string getSaveString()
    {
      string str = "<folder name=\"" + Folder.Filter(this.name) + "\">\n";
      for (int index = 0; index < this.folders.Count; ++index)
        str += this.folders[index].getSaveString();
      for (int index = 0; index < this.files.Count; ++index)
        str = str + "<file name=\"" + Folder.Filter(this.files[index].name) + "\">" + Folder.Filter(this.files[index].data) + "</file>\n";
      return str + "</folder>\n";
    }

    public static string Filter(string s)
    {
      return s.Replace("&", Folder.ampersandReplacer).Replace("\\", Folder.backslashReplacer).Replace("[", Folder.leftSBReplacer).Replace("]", Folder.rightSBReplacer).Replace(">", Folder.rightABReplacer).Replace("<", Folder.leftABReplacer).Replace("\"", Folder.quoteReplacer).Replace("'", Folder.singlequoteReplacer);
    }

    public static string deFilter(string s)
    {
      return s.Replace(Folder.ampersandReplacer, "&").Replace(Folder.backslashReplacer, "\\").Replace(Folder.rightSBReplacer, "]").Replace(Folder.leftSBReplacer, "[").Replace(Folder.rightABReplacer, ">").Replace(Folder.leftABReplacer, "<").Replace(Folder.quoteReplacer, "\"").Replace(Folder.singlequoteReplacer, "'");
    }

    public static Folder load(XmlReader reader)
    {
      while (reader.Name != "folder" || reader.NodeType == XmlNodeType.EndElement)
      {
        reader.Read();
        if (reader.EOF)
          return (Folder) null;
      }
      reader.MoveToAttribute("name");
      Folder folder = new Folder(Folder.deFilter(reader.ReadContentAsString()));
      reader.Read();
      while (reader.Name != "folder" && reader.Name != "file")
      {
        reader.Read();
        if (reader.EOF || reader.Name == "folder" && reader.NodeType == XmlNodeType.EndElement)
          return folder;
      }
      while (reader.Name == "folder")
      {
        if (reader.NodeType == XmlNodeType.EndElement)
          return folder;
        folder.folders.Add(Folder.load(reader));
        reader.Read();
        while (reader.Name != "folder" && reader.Name != "file")
        {
          reader.Read();
          if (reader.EOF || reader.Name == "computer")
            return folder;
        }
      }
      while (reader.Name != "folder" && reader.Name != "file")
        reader.Read();
      while (reader.Name == "file" && reader.NodeType != XmlNodeType.EndElement)
      {
        reader.MoveToAttribute("name");
        string s = reader.ReadContentAsString();
        bool flag = true;
        if (reader.MoveToAttribute("EduSafe"))
          flag = reader.ReadContentAsBoolean();
        string nameEntry = Folder.deFilter(s);
        reader.MoveToElement();
        string dataEntry = Folder.deFilter(reader.ReadElementContentAsString());
        if (flag || !Settings.EducationSafeBuild)
          folder.files.Add(new FileEntry(dataEntry, nameEntry));
        reader.Read();
        while (reader.Name != "folder" && reader.Name != "file")
          reader.Read();
      }
      reader.Read();
      return folder;
    }

    public void load(string data)
    {
    }

    public string TestEqualsFolder(Folder f)
    {
      string str1 = (string) null;
      if (!Utils.CheckStringIsRenderable(this.name) || !Utils.CheckStringIsRenderable(f.name))
        str1 = str1 + "Folder name includes Invalid Chars! " + f.name;
      if (this.name != f.name)
        str1 = str1 + "Name Mismatch : Expected \"" + this.name + "\" But got \"" + f.name + "\"\r\n";
      if (f.folders.Count != this.folders.Count)
        str1 = str1 + "Folder Count Mismatch : Expected \"" + (object) this.folders.Count + "\" But got \"" + (object) f.folders.Count + "\"\r\n";
      if (f.files.Count != this.files.Count)
      {
        str1 = str1 + "File Count Mismatch In folder \"" + f.name + "\" : Expected (loaded) \"" + (object) this.files.Count + "\" But got (original creation) \"" + (object) f.files.Count + "\"\r\nFound Files:\r\n";
        for (int index = 0; index < f.files.Count; ++index)
          str1 = str1 + f.files[index].name + "\r\n--------\r\n" + f.files[index].data + "\r\n#######END FILE#############\r\n\r\n";
      }
      if (str1 != null)
        return str1 + "Previous errors are blocking. Abandoning examination.\r\n";
      for (int index = 0; index < this.folders.Count; ++index)
      {
        string str2 = this.folders[index].TestEqualsFolder(f.folders[index]);
        if (str2 != null)
          str1 = str1 + "\r\n" + str2;
      }
      for (int index = 0; index < this.files.Count; ++index)
      {
        if ((!Utils.CheckStringIsRenderable(this.files[index].name) || !Utils.CheckStringIsRenderable(f.files[index].name)) && Settings.ActiveLocale == "en-us")
          str1 = str1 + "File name includes Invalid Chars! " + f.files[index].name;
        if (this.files[index].name != f.files[index].name)
          str1 = str1 + "Filename Mismatch (" + (object) index + ") expected \"" + this.files[index].name + "\" but got \"" + f.files[index].name + "\"\r\n";
        if (this.files[index].data != f.files[index].data && this.files[index].data.Replace("\r\n", "\n") != f.files[index].data.Replace("\r\n", "\n"))
          str1 = str1 + "Data Mismatch (" + (object) index + ") expected ------\r\n" + this.files[index].data + "\r\n----- but got ------\r\n" + f.files[index].data + "\r\n-----\r\n";
        else if (!(Settings.ActiveLocale == "en-us") || Utils.CheckStringIsRenderable(this.files[index].data))
          ;
      }
      return str1;
    }
  }
}
