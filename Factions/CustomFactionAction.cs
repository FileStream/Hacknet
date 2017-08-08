// Decompiled with JetBrains decompiler
// Type: Hacknet.Factions.CustomFactionAction
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Hacknet.Factions
{
  internal class CustomFactionAction
  {
    public int ValueRequiredForTrigger = 10;
    public string FlagsRequiredForTrigger = (string) null;
    public List<SerializableAction> TriggerActions = new List<SerializableAction>();
    public const string XML_ELEMENT_NAME = "Action";

    public static CustomFactionAction Deserialize(XmlReader rdr)
    {
      CustomFactionAction customFactionAction = new CustomFactionAction();
      while (!rdr.EOF && (!(rdr.Name == "Action") || !rdr.IsStartElement()))
        rdr.Read();
      if (rdr.EOF)
        throw new FormatException("Expected Start element <Action> but did not find it in file!");
      if (rdr.MoveToAttribute("ValueRequired"))
        customFactionAction.ValueRequiredForTrigger = rdr.ReadContentAsInt();
      if (rdr.MoveToAttribute("Flags"))
        customFactionAction.FlagsRequiredForTrigger = rdr.ReadContentAsString();
      rdr.Read();
      while (!rdr.EOF && (!(rdr.Name == "Action") || rdr.IsStartElement()))
      {
        if (string.IsNullOrWhiteSpace(rdr.Name))
        {
          rdr.Read();
        }
        else
        {
          SerializableAction serializableAction = SerializableAction.Deserialize(rdr);
          customFactionAction.TriggerActions.Add(serializableAction);
          rdr.Read();
          while ((rdr.NodeType == XmlNodeType.Comment || rdr.NodeType == XmlNodeType.Whitespace || rdr.NodeType == XmlNodeType.SignificantWhitespace) && !rdr.EOF)
            rdr.Read();
        }
      }
      if (rdr.EOF)
        throw new FormatException("Unexpected end of file: No closing tag for </Action> Found!");
      return customFactionAction;
    }

    public string GetSaveString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<Action ValueRequired=\"" + this.ValueRequiredForTrigger.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "\" ");
      if (this.FlagsRequiredForTrigger != null)
        stringBuilder.Append("Flags=\"" + this.FlagsRequiredForTrigger + "\" ");
      stringBuilder.Append(">\n");
      for (int index = 0; index < this.TriggerActions.Count; ++index)
      {
        stringBuilder.Append("\t" + this.TriggerActions[index].GetSaveString());
        stringBuilder.Append("\n");
      }
      stringBuilder.Append("</Action>");
      return stringBuilder.ToString();
    }

    public void Trigger(object os_obj)
    {
      for (int index = 0; index < this.TriggerActions.Count; ++index)
        this.TriggerActions[index].Trigger(os_obj);
    }
  }
}
