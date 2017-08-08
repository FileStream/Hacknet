// Decompiled with JetBrains decompiler
// Type: Hacknet.ProgressionFlags
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Hacknet
{
  public class ProgressionFlags
  {
    private List<string> Flags = new List<string>();

    public bool HasFlag(string flag)
    {
      return this.Flags.Contains(flag);
    }

    public void AddFlag(string flag)
    {
      if (this.HasFlag(flag))
        return;
      this.Flags.Add(flag);
    }

    public void RemoveFlag(string flag)
    {
      this.Flags.Remove(flag);
    }

    public string GetFlagStartingWith(string start)
    {
      for (int index = 0; index < this.Flags.Count; ++index)
      {
        if (this.Flags[index].StartsWith(start))
          return this.Flags[index];
      }
      return (string) null;
    }

    public void Load(XmlReader rdr)
    {
      this.Flags.Clear();
      while (!(rdr.Name == "Flags") || !rdr.IsStartElement())
      {
        rdr.Read();
        if (rdr.EOF)
          throw new InvalidOperationException("XML reached End of file too fast!");
      }
      int content = (int) rdr.MoveToContent();
      string str1 = rdr.ReadElementContentAsString();
      char[] separator = new char[1]{ ',' };
      int num = 1;
      foreach (string str2 in str1.Split(separator, (StringSplitOptions) num))
      {
        string str3 = str2.Replace("[%%COMMAREPLACED%%]", ",");
        if (str3 == "décrypté")
          str3 = "decypher";
        this.Flags.Add(str3);
      }
    }

    public string GetSaveString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < this.Flags.Count; ++index)
      {
        stringBuilder.Append(this.Flags[index].Replace(",", "[%%COMMAREPLACED%%]"));
        stringBuilder.Append(",");
      }
      if (stringBuilder.Length > 0)
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
      return "<Flags>" + stringBuilder.ToString() + "</Flags>\r\n";
    }
  }
}
