// Decompiled with JetBrains decompiler
// Type: Hacknet.SARunFunction
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SARunFunction : SerializableAction
  {
    public int FunctionValue = 0;
    public float Delay = 0.0f;
    public string FunctionName;
    public string DelayHost;

    public override void Trigger(object os_obj)
    {
      if ((double) this.Delay <= 0.0)
      {
        MissionFunctions.runCommand(this.FunctionValue, this.FunctionName);
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
      SARunFunction saRunFunction = new SARunFunction();
      if (rdr.MoveToAttribute("FunctionName"))
        saRunFunction.FunctionName = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("FunctionValue"))
        saRunFunction.FunctionValue = rdr.ReadContentAsInt();
      if (rdr.MoveToAttribute("Delay"))
        saRunFunction.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        saRunFunction.DelayHost = rdr.ReadContentAsString();
      if (string.IsNullOrWhiteSpace(saRunFunction.FunctionName))
        throw new FormatException("Invalid function name :" + saRunFunction.FunctionName);
      return (SerializableAction) saRunFunction;
    }
  }
}
