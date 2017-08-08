// Decompiled with JetBrains decompiler
// Type: Hacknet.GitCommitEntry
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;

namespace Hacknet
{
  public class GitCommitEntry
  {
    public int EntryNumber = 0;
    public List<string> ChangedFiles = new List<string>();
    public string Message;
    public string UserName;
    public string SourceIP;

    public override string ToString()
    {
      return "Commit#" + this.EntryNumber.ToString("000") + (this.SourceIP.StartsWith("192.168.1.1") ? "" : "*");
    }
  }
}
