﻿// Decompiled with JetBrains decompiler
// Type: Hacknet.MissionSerializer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Text;

namespace Hacknet
{
  public static class MissionSerializer
  {
    public const string MISSION_FILE_DELIMITER = "  #%#\n";

    public static string generateMissionFile(object mission_obj, int contractRegistryNumber = 0, string GroupName = "CSEC", string Tag = null)
    {
      ActiveMission activeMission = (ActiveMission) mission_obj;
      string str = GroupName + " Contract #" + (object) contractRegistryNumber + "\n--------------------------------------------  #%#\n" + "Code = " + MissionSerializer.encodeString(activeMission.reloadGoalsSourceFile) + "\n  #%#\n" + "Client = " + activeMission.client + "  #%#\n" + "Target = " + activeMission.target + "  #%#\n" + "RequiredRank = " + (object) activeMission.requiredRank + "  #%#\n" + "Difficulty = " + (object) activeMission.difficulty + "\n  #%#\n" + "Title = " + activeMission.postingTitle + "  #%#\n" + "Posting = " + activeMission.postingBody + "\n  #%#\n" + "E_TargetTrack = " + MissionSerializer.encodeString(activeMission.genTarget) + "  #%#\n" + "E_TargetTaskData = " + MissionSerializer.encodeString(activeMission.genFile) + "  #%#\n" + "E_TargetTaskTrack = " + MissionSerializer.encodeString(activeMission.genPath) + "  #%#\n" + "E2_TargetTaskTrack_1 = " + MissionSerializer.encodeString(activeMission.genTargetName) + "  #%#\n" + "E3_TargetTaskTrack_2 = " + MissionSerializer.encodeString(activeMission.genOther) + "  #%#\n" + "E3_TargetTaskTrack_3 = " + (activeMission.wasAutoGenerated ? "gen" : "cmd") + "  #%#\n" + "E4_ContractReg = " + (object) contractRegistryNumber + "  #%#\n";
      if (Tag != null)
        str = str + "Tag = " + Tag + "  #%#\n";
      return str;
    }

    public static object restoreMissionFromFile(string data, out int contractRegistryNumber)
    {
      string Tag = (string) null;
      return MissionSerializer.restoreMissionFromFile(data, out contractRegistryNumber, out Tag);
    }

    public static object restoreMissionFromFile(string data, out int contractRegistryNumber, out string Tag)
    {
      string[] separator = new string[1]{ "  #%#\n" };
      string[] strArray = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      string str1;
      string str2 = str1 = "unknown";
      string str3 = str1;
      string str4 = str1;
      string str5 = str1;
      string str6 = str1;
      string str7 = str1;
      string str8 = str1;
      string filename = str1;
      bool flag = false;
      int num1;
      int num2 = num1 = 0;
      contractRegistryNumber = 1;
      Tag = (string) null;
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (!strArray[index].StartsWith("//"))
        {
          string line = strArray[index];
          if (line.StartsWith("Code"))
            filename = MissionSerializer.decodeString(MissionSerializer.getDataFromConfigLine(line, "= "));
          else if (line.StartsWith("Client"))
            str8 = MissionSerializer.getDataFromConfigLine(line, "= ");
          else if (line.StartsWith("Target"))
            str7 = MissionSerializer.getDataFromConfigLine(line, "= ");
          else if (line.StartsWith("E_TargetTrack"))
            str6 = MissionSerializer.decodeString(MissionSerializer.getDataFromConfigLine(line, "= "));
          else if (line.StartsWith("E_TargetTaskData"))
            str5 = MissionSerializer.decodeString(MissionSerializer.getDataFromConfigLine(line, "= "));
          else if (line.StartsWith("E_TargetTaskTrack"))
            str4 = MissionSerializer.decodeString(MissionSerializer.getDataFromConfigLine(line, "= "));
          else if (line.StartsWith("E2_TargetTaskTrack_1"))
            str3 = MissionSerializer.decodeString(MissionSerializer.getDataFromConfigLine(line, "= "));
          else if (line.StartsWith("E3_TargetTaskTrack_2"))
            str2 = MissionSerializer.decodeString(MissionSerializer.getDataFromConfigLine(line, "= "));
          else if (line.StartsWith("E3_TargetTaskTrack_3"))
            flag = MissionSerializer.getDataFromConfigLine(line, "= ") == "gen";
          else if (line.StartsWith("Tag"))
            Tag = MissionSerializer.getDataFromConfigLine(line, "= ");
          else if (line.StartsWith("E4_ContractReg"))
          {
            try
            {
              contractRegistryNumber = Convert.ToInt32(MissionSerializer.getDataFromConfigLine(line, "= "));
            }
            catch (FormatException ex)
            {
              contractRegistryNumber = 0;
            }
            catch (OverflowException ex)
            {
              contractRegistryNumber = 0;
            }
          }
          else if (line.StartsWith("Rank"))
          {
            try
            {
              num2 = Convert.ToInt32(MissionSerializer.getDataFromConfigLine(line, "= "));
            }
            catch (FormatException ex)
            {
              contractRegistryNumber = 0;
            }
            catch (OverflowException ex)
            {
              contractRegistryNumber = 0;
            }
          }
          else if (line.StartsWith("Difficulty"))
          {
            try
            {
              num1 = Convert.ToInt32(MissionSerializer.getDataFromConfigLine(line, "= "));
            }
            catch (FormatException ex)
            {
              contractRegistryNumber = 0;
            }
            catch (OverflowException ex)
            {
              contractRegistryNumber = 0;
            }
          }
        }
      }
      MissionGenerationParser.Client = str8;
      MissionGenerationParser.Comp = str6;
      MissionGenerationParser.File = str5;
      MissionGenerationParser.Path = str4;
      ActiveMission activeMission = (ActiveMission) ComputerLoader.readMission(filename);
      activeMission.genFile = str5;
      activeMission.genPath = str4;
      activeMission.genTarget = str6;
      activeMission.genTargetName = str3;
      activeMission.genOther = str2;
      activeMission.target = str7;
      activeMission.client = str8;
      activeMission.requiredRank = num2;
      activeMission.difficulty = num1;
      activeMission.wasAutoGenerated = flag;
      return (object) activeMission;
    }

    private static string encodeString(string s)
    {
      string str = "";
      if (s != null)
      {
        for (int index = 0; index < s.Length; ++index)
          str = str + (object) (int) s[index] + " ";
      }
      return str.Trim();
    }

    private static string decodeString(string s)
    {
      char[] separator = new char[1]{ ' ' };
      string[] strArray = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < strArray.Length; ++index)
        stringBuilder.Append(Convert.ToChar(Convert.ToInt32(strArray[index])));
      return stringBuilder.ToString();
    }

    private static string getDataFromConfigLine(string line, string sentinel = "= ")
    {
      return line.Substring(line.IndexOf(sentinel) + 2);
    }
  }
}
