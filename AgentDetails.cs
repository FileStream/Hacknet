// Decompiled with JetBrains decompiler
// Type: Hacknet.AgentDetails
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public class AgentDetails
  {
    public string Codename;
    public string RealName;
    public string IP;
    public string SpecialNotes;

    public override string ToString()
    {
      return this.Codename;
    }
  }
}
