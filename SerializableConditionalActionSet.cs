// Decompiled with JetBrains decompiler
// Type: Hacknet.SerializableConditionalActionSet
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Hacknet
{
  public class SerializableConditionalActionSet
  {
    public List<SerializableAction> Actions = new List<SerializableAction>();
    public SerializableCondition Condition;

    public string GetSaveString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < this.Actions.Count; ++index)
      {
        stringBuilder.Append(this.Actions[index].GetSaveString());
        stringBuilder.Append("\r\n");
      }
      return this.Condition.GetSaveString(stringBuilder.ToString());
    }

    public static SerializableConditionalActionSet Deserialize(XmlReader rdr)
    {
      SerializableConditionalActionSet ret = new SerializableConditionalActionSet();
      Action<XmlReader, string> bodyContentReadAction = (Action<XmlReader, string>) ((xmlReader, EndKeyName) =>
      {
        while (!rdr.EOF && (string.IsNullOrWhiteSpace(rdr.Name) || rdr.NodeType == XmlNodeType.Comment || rdr.NodeType == XmlNodeType.Whitespace))
          rdr.Read();
        for (bool flag = !xmlReader.EOF && (!(xmlReader.Name == EndKeyName) || xmlReader.IsStartElement()); flag; flag = !xmlReader.EOF && (!(xmlReader.Name == EndKeyName) || xmlReader.IsStartElement()))
        {
          ret.Actions.Add(SerializableAction.Deserialize(xmlReader));
          do
          {
            xmlReader.Read();
          }
          while (xmlReader.NodeType == XmlNodeType.Whitespace || xmlReader.NodeType == XmlNodeType.Comment);
        }
      });
      ret.Condition = SerializableCondition.Deserialize(rdr, bodyContentReadAction);
      return ret;
    }
  }
}
