// Decompiled with JetBrains decompiler
// Type: Hacknet.SCDoesNotHaveFlags
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SCDoesNotHaveFlags : SerializableCondition
  {
    public string Flags;

    public override bool Check(object os_obj)
    {
      OS os = (OS) os_obj;
      if (!string.IsNullOrWhiteSpace(this.Flags))
      {
        foreach (string flag in this.Flags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries))
        {
          if (os.Flags.HasFlag(flag))
            return false;
        }
      }
      return true;
    }

    public static SerializableCondition DeserializeFromReader(XmlReader rdr)
    {
      SCDoesNotHaveFlags doesNotHaveFlags = new SCDoesNotHaveFlags();
      if (rdr.MoveToAttribute("Flags"))
        doesNotHaveFlags.Flags = rdr.ReadContentAsString();
      return (SerializableCondition) doesNotHaveFlags;
    }
  }
}
