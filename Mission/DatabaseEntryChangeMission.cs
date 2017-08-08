// Decompiled with JetBrains decompiler
// Type: Hacknet.Mission.DatabaseEntryChangeMission
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
  internal class DatabaseEntryChangeMission : MisisonGoal
  {
    private Computer c;
    private string Operation;
    private string RecordName;
    private string FieldName;
    private string TargetValue;

    public DatabaseEntryChangeMission(string computerIP, OS os, string operation, string FieldName, string targetValue, string recordName)
    {
      this.c = Programs.getComputer(os, computerIP);
      this.Operation = operation;
      this.FieldName = FieldName;
      this.TargetValue = targetValue;
      this.RecordName = recordName;
    }

    public override bool isComplete(List<string> additionalDetails = null)
    {
      try
      {
        object objectForRecordName = ((DatabaseDaemon) this.c.getDaemon(typeof (DatabaseDaemon))).GetObjectForRecordName(this.RecordName);
        if (objectForRecordName != null)
        {
          object valueFromObject = ObjectSerializer.GetValueFromObject(objectForRecordName, this.FieldName);
          try
          {
            double num1 = Convert.ToDouble(valueFromObject);
            double num2 = Convert.ToDouble(this.TargetValue);
            switch (this.Operation)
            {
              case ">":
              case "greater":
                return num1 > num2;
              case "<":
              case "less":
                return num1 < num2;
              case "=":
              case "equals":
                return Math.Abs(num1 - num2) < 0.0001;
            }
          }
          catch (FormatException ex)
          {
            if (valueFromObject == null)
              return false;
            return valueFromObject.ToString() == this.TargetValue;
          }
        }
        return false;
      }
      catch (Exception ex)
      {
        Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
        return true;
      }
    }

    public override string TestCompletable()
    {
      return "";
    }
  }
}
