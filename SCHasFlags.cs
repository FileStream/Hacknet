// Decompiled with JetBrains decompiler
// Type: Hacknet.SCHasFlags
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SCHasFlags : SerializableCondition
  {
    public string requiredFlags;

    public override bool Check(object os_obj)
    {
      OS os = (OS) os_obj;
      if (!string.IsNullOrWhiteSpace(this.requiredFlags))
      {
        foreach (string flag in this.requiredFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries))
        {
          if (!os.Flags.HasFlag(flag))
            return false;
        }
      }
      return true;
    }

    public static SerializableCondition DeserializeFromReader(XmlReader rdr)
    {
      SCHasFlags scHasFlags = new SCHasFlags();
      if (rdr.MoveToAttribute("requiredFlags"))
        scHasFlags.requiredFlags = rdr.ReadContentAsString();
      return (SerializableCondition) scHasFlags;
    }
  }
}
