// Decompiled with JetBrains decompiler
// Type: Hacknet.RunnableConditionalActions
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
  public class RunnableConditionalActions
  {
    public List<SerializableConditionalActionSet> Actions = new List<SerializableConditionalActionSet>();
    private bool IsUpdating = false;
    public const string SerializationKey = "ConditionalActions";

    public virtual void Update(float dt, object os)
    {
      this.IsUpdating = true;
      for (int index1 = 0; index1 < this.Actions.Count; ++index1)
      {
        if (this.Actions[index1].Condition.Check(os))
        {
          SerializableConditionalActionSet action = this.Actions[index1];
          this.Actions.RemoveAt(index1);
          --index1;
          for (int index2 = 0; index2 < action.Actions.Count; ++index2)
            action.Actions[index2].Trigger(os);
        }
      }
      this.IsUpdating = false;
    }

    public string GetSaveString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<ConditionalActions>\r\n");
      for (int index = 0; index < this.Actions.Count; ++index)
        stringBuilder.Append(this.Actions[index].GetSaveString() + "\r\n");
      stringBuilder.Append("</ConditionalActions>");
      return stringBuilder.ToString();
    }

    public static RunnableConditionalActions Deserialize(XmlReader rdr)
    {
      while (!rdr.EOF && rdr.Name != "ConditionalActions" && !rdr.IsStartElement())
        rdr.Read();
      if (rdr.EOF)
        throw new FormatException("Unexpected end of file trying to deserialize Runnable Conditional Actions!");
      RunnableConditionalActions conditionalActions = new RunnableConditionalActions();
      rdr.Read();
      bool flag;
      do
      {
        while (!rdr.EOF && (rdr.IsEmptyElement || string.IsNullOrWhiteSpace(rdr.Name)))
          rdr.Read();
        if (rdr.EOF)
          throw new FormatException("Unexpected end of file trying to deserialize Runnable Conditional Actions!");
        flag = !(rdr.Name == "ConditionalActions") || rdr.IsStartElement();
        if (flag)
        {
          SerializableConditionalActionSet conditionalActionSet = SerializableConditionalActionSet.Deserialize(rdr);
          conditionalActions.Actions.Add(conditionalActionSet);
          rdr.Read();
        }
      }
      while (flag);
      return conditionalActions;
    }

    public static void LoadIntoOS(string filepath, object OSobj)
    {
      OS os = (OS) OSobj;
      using (FileStream fileStream = File.OpenRead(LocalizedFileLoader.GetLocalizedFilepath(Utils.GetFileLoadPrefix() + filepath)))
      {
        RunnableConditionalActions conditionalActions = RunnableConditionalActions.Deserialize(XmlReader.Create((Stream) fileStream));
        for (int index = 0; index < conditionalActions.Actions.Count; ++index)
          os.ConditionalActions.Actions.Add(conditionalActions.Actions[index]);
      }
      if (os.ConditionalActions.IsUpdating)
        return;
      os.ConditionalActions.Update(0.0f, (object) os);
    }
  }
}
