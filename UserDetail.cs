// Decompiled with JetBrains decompiler
// Type: Hacknet.UserDetail
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Xml;

namespace Hacknet
{
  public struct UserDetail
  {
    public string name;
    public string pass;
    public byte type;
    public bool known;

    public UserDetail(string user, string password, byte accountType)
    {
      this.name = user;
      this.pass = password;
      this.type = accountType;
      this.known = false;
    }

    public UserDetail(string user)
    {
      this.name = user;
      this.pass = PortExploits.getRandomPassword();
      this.type = (byte) 1;
      this.known = false;
    }

    public string getSaveString()
    {
      return "<user name=\"" + this.name + "\" pass=\"" + this.pass + "\" type=\"" + (object) this.type + "\" known=\"" + (object) this.known + "\" />";
    }

    public static UserDetail loadUserDetail(XmlReader reader)
    {
      reader.MoveToAttribute("name");
      string user = reader.ReadContentAsString();
      reader.MoveToAttribute("pass");
      string password = reader.ReadContentAsString();
      reader.MoveToAttribute("type");
      byte accountType = (byte) reader.ReadContentAsInt();
      reader.MoveToAttribute("known");
      bool flag = reader.ReadContentAsString().ToLower().Equals("true");
      return new UserDetail(user, password, accountType) { known = flag };
    }

    public override bool Equals(object obj)
    {
      UserDetail? nullable = obj as UserDetail?;
      if (nullable.HasValue)
        return this.name == nullable.Value.name && this.pass == nullable.Value.pass && (int) this.type == (int) nullable.Value.type && this.known == nullable.Value.known;
      return false;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
