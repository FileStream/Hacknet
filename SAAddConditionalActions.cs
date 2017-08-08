// Decompiled with JetBrains decompiler
// Type: Hacknet.SAAddConditionalActions
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SAAddConditionalActions : SerializableAction
  {
    public string Filepath;
    public string DelayHost;
    public float Delay;

    public override void Trigger(object os_obj)
    {
      if ((double) this.Delay <= 0.0)
      {
        RunnableConditionalActions.LoadIntoOS(this.Filepath, os_obj);
      }
      else
      {
        Computer computer = Programs.getComputer((OS) os_obj, this.DelayHost);
        if (computer == null)
          throw new NullReferenceException("Computer " + (object) computer + " could not be found as DelayHost for Function");
        float delay = this.Delay;
        this.Delay = -1f;
        DelayableActionSystem.FindDelayableActionSystemOnComputer(computer).AddAction((SerializableAction) this, delay);
      }
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SAAddConditionalActions conditionalActions = new SAAddConditionalActions();
      if (rdr.MoveToAttribute("Filepath"))
        conditionalActions.Filepath = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("Delay"))
        conditionalActions.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        conditionalActions.DelayHost = rdr.ReadContentAsString();
      if (string.IsNullOrWhiteSpace(conditionalActions.Filepath))
        throw new FormatException("Invalid Filepath");
      return (SerializableAction) conditionalActions;
    }
  }
}
