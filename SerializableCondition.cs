// Decompiled with JetBrains decompiler
// Type: Hacknet.SerializableCondition
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Hacknet
{
  public abstract class SerializableCondition
  {
    public abstract bool Check(object os_obj);

    public string GetSaveString(string bodyContent)
    {
      Type type = this.GetType();
      string str1 = type.Name;
      if (str1.StartsWith("Hacknet."))
        str1 = str1.Substring("Hacknet.".Length);
      if (str1.StartsWith("SC"))
        str1 = str1.Substring("SC".Length);
      StringBuilder stringBuilder = new StringBuilder("<" + str1 + " ");
      string str2 = bodyContent;
      FieldInfo[] fields = type.GetFields();
      for (int index = 0; index < fields.Length; ++index)
      {
        stringBuilder.Append(fields[index].Name + "=\"");
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}", new object[1]
        {
          fields[index].GetValue((object) this)
        });
        stringBuilder.Append("\" ");
      }
      stringBuilder.Append(">\r\n");
      stringBuilder.Append(str2);
      stringBuilder.Append("\r\n</" + str1 + ">");
      return stringBuilder.ToString();
    }

    public static SerializableCondition Deserialize(XmlReader rdr, Action<XmlReader, string> bodyContentReadAction)
    {
      Dictionary<string, Func<XmlReader, SerializableCondition>> dictionary = new Dictionary<string, Func<XmlReader, SerializableCondition>>();
      dictionary.Add("OnAdminGained", new Func<XmlReader, SerializableCondition>(SCOnAdminGained.DeserializeFromReader));
      dictionary.Add("OnConnect", new Func<XmlReader, SerializableCondition>(SCOnConnect.DeserializeFromReader));
      dictionary.Add("HasFlags", new Func<XmlReader, SerializableCondition>(SCHasFlags.DeserializeFromReader));
      dictionary.Add("Instantly", new Func<XmlReader, SerializableCondition>(SCInstantly.DeserializeFromReader));
      dictionary.Add("OnDisconnect", new Func<XmlReader, SerializableCondition>(SCOnDisconnect.DeserializeFromReader));
      while (!rdr.EOF && (!rdr.IsStartElement() || !dictionary.ContainsKey(rdr.Name)))
        rdr.Read();
      if (rdr.EOF)
        throw new FormatException("Unexpected end of file!");
      string name = rdr.Name;
      SerializableCondition serializableCondition = dictionary[rdr.Name](rdr);
      rdr.Read();
      if (bodyContentReadAction != null)
        bodyContentReadAction(rdr, name);
      rdr.Read();
      return serializableCondition;
    }
  }
}
