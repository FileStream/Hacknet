// Decompiled with JetBrains decompiler
// Type: Hacknet.MemoryContents
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Hacknet
{
  public class MemoryContents
  {
    public List<string> DataBlocks = new List<string>();
    public List<string> CommandsRun = new List<string>();
    public List<KeyValuePair<string, string>> FileFragments = new List<KeyValuePair<string, string>>();
    public List<string> Images = new List<string>();
    private const string EncryptionPass = "19474-217316293";
    private const string FileHeader = "MEMORY_DUMP : FORMAT v1.22 ----------\n\n";

    public string GetSaveString()
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.Append("<Memory>\r\n");
      if (this.DataBlocks != null && this.DataBlocks.Count > 0)
      {
        stringBuilder1.Append("<Data>");
        for (int index = 0; index < this.DataBlocks.Count; ++index)
          stringBuilder1.Append("<Block>" + Folder.Filter(this.DataBlocks[index]) + "</Block>\r\n");
        stringBuilder1.Append("</Data>\r\n");
      }
      if (this.CommandsRun != null && this.CommandsRun.Count > 0)
      {
        stringBuilder1.Append("<Commands>");
        for (int index = 0; index < this.CommandsRun.Count; ++index)
          stringBuilder1.Append("<Command>" + Folder.Filter(this.CommandsRun[index]) + "</Command>\r\n");
        stringBuilder1.Append("</Commands>\r\n");
      }
      if (this.FileFragments != null && this.FileFragments.Count > 0)
      {
        stringBuilder1.Append("<FileFragments>");
        for (int index1 = 0; index1 < this.CommandsRun.Count; ++index1)
        {
          StringBuilder stringBuilder2 = stringBuilder1;
          string[] strArray1 = new string[5]
          {
            "<File name=\"",
            null,
            null,
            null,
            null
          };
          string[] strArray2 = strArray1;
          int index2 = 1;
          KeyValuePair<string, string> fileFragment = this.FileFragments[index1];
          string str1 = Folder.Filter(fileFragment.Key);
          strArray2[index2] = str1;
          strArray1[2] = "\">";
          string[] strArray3 = strArray1;
          int index3 = 3;
          fileFragment = this.FileFragments[index1];
          string str2 = Folder.Filter(fileFragment.Value);
          strArray3[index3] = str2;
          strArray1[4] = "</Command>\r\n";
          string str3 = string.Concat(strArray1);
          stringBuilder2.Append(str3);
        }
        stringBuilder1.Append("</FileFragments>\r\n");
      }
      if (this.Images != null && this.Images.Count > 0)
      {
        stringBuilder1.Append("<Images>");
        for (int index = 0; index < this.Images.Count; ++index)
          stringBuilder1.Append("<Image>" + Folder.Filter(this.Images[index]) + "</Image>\r\n");
        stringBuilder1.Append("</Images>\r\n");
      }
      stringBuilder1.Append("</Memory>");
      return stringBuilder1.ToString();
    }

    public static MemoryContents Deserialize(XmlReader rdr)
    {
      MemoryContents ret = new MemoryContents();
      while (rdr.Name != "Memory")
      {
        rdr.Read();
        if (rdr.EOF)
          throw new FormatException("Unexpected end of file looking for start of Memory tag");
      }
      do
      {
        rdr.Read();
        if (rdr.Name == "Memory" && !rdr.IsStartElement())
          return ret;
        Utils.ProcessXmlElementInParent(rdr, "Commands", "Command", (Action) (() =>
        {
          int content = (int) rdr.MoveToContent();
          string s = rdr.ReadElementContentAsString();
          if (s.Contains("\n"))
          {
            string[] strArray = s.Split(Utils.robustNewlineDelim, StringSplitOptions.None);
            for (int index = 0; index < strArray.Length; ++index)
            {
              if (string.IsNullOrEmpty(strArray[index]))
                strArray[index] = " ";
              ret.CommandsRun.Add(ComputerLoader.filter(Folder.deFilter(strArray[index])));
            }
          }
          else
            ret.CommandsRun.Add(ComputerLoader.filter(Folder.deFilter(s)));
        }));
        Utils.ProcessXmlElementInParent(rdr, "Data", "Block", (Action) (() =>
        {
          int content = (int) rdr.MoveToContent();
          ret.DataBlocks.Add(ComputerLoader.filter(Folder.deFilter(rdr.ReadElementContentAsString())));
        }));
        Utils.ProcessXmlElementInParent(rdr, "FileFragments", "File", (Action) (() =>
        {
          string s1 = "UNKNOWN";
          if (rdr.MoveToAttribute("name"))
            s1 = rdr.ReadContentAsString();
          int content = (int) rdr.MoveToContent();
          string s2 = rdr.ReadElementContentAsString();
          ret.FileFragments.Add(new KeyValuePair<string, string>(Folder.deFilter(s1), Folder.deFilter(s2)));
        }));
        Utils.ProcessXmlElementInParent(rdr, "Images", "Image", (Action) (() =>
        {
          int content = (int) rdr.MoveToContent();
          ret.Images.Add(Folder.deFilter(rdr.ReadElementContentAsString()));
        }));
      }
      while (!rdr.EOF);
      throw new FormatException("Unexpected end of file trying to deserialize memory contents!");
    }

    private string GetCompactSaveString()
    {
      return this.GetSaveString().Replace("Commands>", "CM>").Replace("Command>", "c>").Replace("Data>", "D>").Replace("Block>", "b>").Replace("FileFragments>", "FF>").Replace("File>", "f>").Replace("Memory>", "M>").Replace("Images>", "Is>").Replace("Image>", "i>");
    }

    private static string ReExpandSaveString(string save)
    {
      save = save.Replace("CM>", "Commands>\r\n").Replace("</c>", "</Command>\r\n").Replace("c>", "Command>").Replace("D>", "Data>\r\n").Replace("</b>", "</Block>\r\n").Replace("b>", "Block>").Replace("FF>", "FileFragments>\r\n").Replace("f>", "File>\r\n").Replace("M>", "Memory>\r\n").Replace("Is>", "Images>").Replace("i>", "Image>");
      return save;
    }

    public string GetEncodedFileString()
    {
      return "MEMORY_DUMP : FORMAT v1.22 ----------\n\n" + Computer.generateBinaryString(512).Substring(0, 400) + "\n\n" + FileEncrypter.EncryptString(this.GetCompactSaveString(), "MEMORY DUMP", "------", "19474-217316293", (string) null);
    }

    public static MemoryContents GetMemoryFromEncodedFileString(string data)
    {
      using (Stream streamFromString = Utils.GenerateStreamFromString(MemoryContents.ReExpandSaveString(FileEncrypter.DecryptString(data.Substring("MEMORY_DUMP : FORMAT v1.22 ----------\n\n".Length + 400 + 2), "19474-217316293")[2])))
        return MemoryContents.Deserialize(XmlReader.Create(streamFromString));
    }

    public string TestEqualsWithErrorReport(MemoryContents other)
    {
      string str1 = "";
      if (other == null)
        return "Other memory object is null!";
      if (other.DataBlocks.Count == this.DataBlocks.Count)
      {
        for (int index = 0; index < this.DataBlocks.Count; ++index)
        {
          if (other.DataBlocks[index] != this.DataBlocks[index])
            str1 = str1 + "Data block difference for item " + (object) index + " - mismatch";
        }
      }
      else
        str1 = str1 + "Datablock count difference - found " + (object) other.DataBlocks.Count + " - expected " + (object) this.DataBlocks.Count;
      if (other.CommandsRun.Count == this.CommandsRun.Count)
      {
        for (int index = 0; index < this.CommandsRun.Count; ++index)
        {
          if (other.CommandsRun[index] != this.CommandsRun[index])
            str1 = str1 + "\n\nCommandsRun difference for item " + (object) index + " - mismatch.\nFound " + other.CommandsRun[index] + "  :vs:  " + this.CommandsRun[index] + "\n";
        }
      }
      else
        str1 = str1 + "CommandsRun count difference - found " + (object) other.CommandsRun.Count + " - expected " + (object) this.CommandsRun.Count;
      if (other.FileFragments.Count == this.FileFragments.Count)
      {
        for (int index = 0; index < this.FileFragments.Count; ++index)
        {
          KeyValuePair<string, string> fileFragment = other.FileFragments[index];
          string key1 = fileFragment.Key;
          fileFragment = this.FileFragments[index];
          string key2 = fileFragment.Key;
          if (key1 != key2)
            str1 = str1 + "FileFragments difference for item " + (object) index + " - key mismatch";
          fileFragment = other.FileFragments[index];
          string str2 = fileFragment.Value;
          fileFragment = this.FileFragments[index];
          string str3 = fileFragment.Value;
          if (str2 != str3)
            str1 = str1 + "FileFragments difference for item " + (object) index + " - Value mismatch";
        }
      }
      else
        str1 = str1 + "FileFragments count difference - found " + (object) other.FileFragments.Count + " - expected " + (object) this.FileFragments.Count;
      if (other.Images.Count == this.Images.Count)
      {
        for (int index = 0; index < this.Images.Count; ++index)
        {
          if (other.Images[index] != this.Images[index])
            str1 = str1 + "\n\nImages difference for item " + (object) index + " - mismatch.\nFound " + other.Images[index] + "  :vs:  " + this.Images[index] + "\n";
        }
      }
      else
        str1 = str1 + "Images count difference - found " + (object) other.Images.Count + " - expected " + (object) this.Images.Count;
      return str1;
    }
  }
}
