// Decompiled with JetBrains decompiler
// Type: Hacknet.Misc.DLCExtendedTests
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Hacknet.Misc
{
  public static class DLCExtendedTests
  {
    public static string TesExtendedFunctionality(ScreenManager screenMan, out int errorsAdded)
    {
      string str1 = "";
      int errorsAdded1 = 0;
      int num1 = 0;
      string str2 = str1 + DLCExtendedTests.TestConditionalActionSets(screenMan, out errorsAdded1);
      int num2 = num1 + errorsAdded1;
      string str3 = str2 + DLCExtendedTests.TestAdvancedConditionalActionSets(screenMan, out errorsAdded1);
      int num3 = num2 + errorsAdded1;
      string str4 = str3 + DLCExtendedTests.TestConditionalActionSetCollections(screenMan, out errorsAdded1);
      int num4 = num3 + errorsAdded1;
      string str5 = str4 + DLCExtendedTests.TestConditionalActionSetCollections2(screenMan, out errorsAdded1);
      int num5 = num4 + errorsAdded1;
      string str6 = str5 + DLCExtendedTests.TestConditionalActionSetCollectionsOnOS(screenMan, out errorsAdded1);
      int num6 = num5 + errorsAdded1;
      string str7 = str6 + DLCExtendedTests.TestObjectSerializer(screenMan, out errorsAdded1);
      int num7 = num6 + errorsAdded1;
      string str8 = str7 + DLCExtendedTests.TestDLCSessionUpgrader(screenMan, out errorsAdded1);
      int num8 = num7 + errorsAdded1;
      string str9 = str8 + (errorsAdded1 > 0 ? (object) "\r\n" : (object) " ") + "Complete - " + (object) errorsAdded1 + " errors found";
      errorsAdded = num8;
      return str9;
    }

    public static string TestConditionalActionSets(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      using (FileStream fileStream1 = File.OpenRead("Content/Tests/DLCTests/TestConditionalActionSet.xml"))
      {
        XmlReader rdr = XmlReader.Create((Stream) fileStream1);
        SerializableConditionalActionSet conditionalActionSet1 = SerializableConditionalActionSet.Deserialize(rdr);
        if (!Directory.Exists("Content/Tests/Output/"))
          Directory.CreateDirectory("Content/Tests/Output");
        File.WriteAllText("Content/Tests/Output/TestConditionalActionSetsOutput.txt", conditionalActionSet1.GetSaveString());
        using (FileStream fileStream2 = File.OpenRead("Content/Tests/Output/TestConditionalActionSetsOutput.txt"))
        {
          SerializableConditionalActionSet conditionalActionSet2 = SerializableConditionalActionSet.Deserialize(XmlReader.Create((Stream) fileStream2));
          if (conditionalActionSet1.Actions.Count != conditionalActionSet2.Actions.Count)
          {
            ++num;
            str = str + "\r\n\r\nConditional Action Sets are Broken! Expected 2 actions, got " + (object) conditionalActionSet1.Actions.Count + " and " + (object) conditionalActionSet2.Actions.Count;
          }
        }
        rdr.Close();
      }
      new GitCommitEntry().EntryNumber = 1;
      errorsAdded = num;
      return str;
    }

    public static string TestAdvancedConditionalActionSets(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      using (FileStream fileStream1 = File.OpenRead("Content/Tests/DLCTests/TestAdvancedConditionalActionSet.xml"))
      {
        XmlReader rdr = XmlReader.Create((Stream) fileStream1);
        SerializableConditionalActionSet conditionalActionSet1 = SerializableConditionalActionSet.Deserialize(rdr);
        if (conditionalActionSet1.Actions.Count != 3)
        {
          ++num;
          str = str + "\r\n\r\nAdvanced Conditional Action Sets are Broken! Expected 3 actions, got " + (object) conditionalActionSet1.Actions.Count;
        }
        if (!Directory.Exists("Content/Tests/Output/"))
          Directory.CreateDirectory("Content/Tests/Output");
        File.WriteAllText("Content/Tests/Output/TestAdvConditionalActionSetsOutput.txt", conditionalActionSet1.GetSaveString());
        using (FileStream fileStream2 = File.OpenRead("Content/Tests/Output/TestAdvConditionalActionSetsOutput.txt"))
        {
          SerializableConditionalActionSet conditionalActionSet2 = SerializableConditionalActionSet.Deserialize(XmlReader.Create((Stream) fileStream2));
          if (conditionalActionSet1.Actions.Count != conditionalActionSet2.Actions.Count)
          {
            ++num;
            str = str + "\r\n\r\nAdvanced Conditional Action Sets are Broken! Expected 2 actions, got " + (object) conditionalActionSet1.Actions.Count + " and " + (object) conditionalActionSet2.Actions.Count;
          }
        }
        rdr.Close();
      }
      using (FileStream fileStream = File.OpenRead("Content/Tests/DLCTests/TestAdvancedConditionalActionSet2.xml"))
      {
        SerializableConditionalActionSet conditionalActionSet = SerializableConditionalActionSet.Deserialize(XmlReader.Create((Stream) fileStream));
        if (conditionalActionSet.Actions.Count != 21)
        {
          ++num;
          str = str + "\r\n\r\nAdvanced Conditional Action Sets are Broken! Expected 21 actions, got " + (object) conditionalActionSet.Actions.Count;
        }
      }
      errorsAdded = num;
      return str;
    }

    public static string TestConditionalActionSetCollections(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      using (FileStream fileStream1 = File.OpenRead("Content/Tests/DLCTests/TestConditionalActionSetCollection.xml"))
      {
        XmlReader rdr = XmlReader.Create((Stream) fileStream1);
        RunnableConditionalActions conditionalActions1 = RunnableConditionalActions.Deserialize(rdr);
        if (!Directory.Exists("Content/Tests/Output/"))
          Directory.CreateDirectory("Content/Tests/Output");
        File.WriteAllText("Content/Tests/Output/TestConditionalActionSetsOutput2.xml", conditionalActions1.GetSaveString());
        using (FileStream fileStream2 = File.OpenRead("Content/Tests/Output/TestConditionalActionSetsOutput2.xml"))
        {
          RunnableConditionalActions conditionalActions2 = RunnableConditionalActions.Deserialize(XmlReader.Create((Stream) fileStream2));
          if (conditionalActions1.Actions.Count != conditionalActions2.Actions.Count || conditionalActions1.Actions.Count != 3)
          {
            ++num;
            str = str + "\r\n\r\nConditional Action Set Collections are Broken! Expected 3 actions, got " + (object) conditionalActions1.Actions.Count + " and " + (object) conditionalActions2.Actions.Count;
          }
          if (conditionalActions1.Actions[0].Actions.Count != 2 || conditionalActions1.Actions[1].Actions.Count != 0)
          {
            ++num;
            str += "\nSave on OS COnditional actions failed! Incorrect action contents on original deserialization. ";
          }
          if (conditionalActions2.Actions[0].Actions.Count != 2 || conditionalActions2.Actions[1].Actions.Count != 0)
          {
            ++num;
            str += "\nSave on OS COnditional actions failed! Incorrect action contents on realo.";
          }
        }
        rdr.Close();
      }
      errorsAdded = num;
      return str;
    }

    public static string TestConditionalActionSetCollections2(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      using (FileStream fileStream1 = File.OpenRead("Content/Tests/DLCTests/TestConditionalActionSetCollection2.xml"))
      {
        XmlReader rdr = XmlReader.Create((Stream) fileStream1);
        RunnableConditionalActions conditionalActions1 = RunnableConditionalActions.Deserialize(rdr);
        if (!Directory.Exists("Content/Tests/Output/"))
          Directory.CreateDirectory("Content/Tests/Output");
        File.WriteAllText("Content/Tests/Output/TestConditionalActionSetsOutput2.xml", conditionalActions1.GetSaveString());
        using (FileStream fileStream2 = File.OpenRead("Content/Tests/Output/TestConditionalActionSetsOutput2.xml"))
        {
          RunnableConditionalActions conditionalActions2 = RunnableConditionalActions.Deserialize(XmlReader.Create((Stream) fileStream2));
          if (conditionalActions1.Actions.Count != conditionalActions2.Actions.Count || conditionalActions1.Actions.Count != 1)
          {
            ++num;
            str = str + "\r\n\r\nConditional Action Set Collections are Broken! Expected 1 actions, got " + (object) conditionalActions1.Actions.Count + " and " + (object) conditionalActions2.Actions.Count;
          }
          if (conditionalActions1.Actions[0].Actions.Count != 1)
          {
            ++num;
            str += "\n\nSave on OS Conditional actions v2 failed! Incorrect action contents on original deserialization. \n\n";
          }
        }
        rdr.Close();
      }
      errorsAdded = num;
      return str;
    }

    public static string TestConditionalActionSetCollectionsOnOS(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      string username = "__hacknettestaccount";
      string pass = "__testingpassword";
      SaveFileManager.AddUser(username, pass);
      string fileNameForUsername = SaveFileManager.GetSaveFileNameForUsername(username);
      OS.TestingPassOnly = true;
      OS os1 = new OS();
      os1.SaveGameUserName = fileNameForUsername;
      os1.SaveUserAccountName = username;
      screenMan.AddScreen((GameScreen) os1, new PlayerIndex?(screenMan.controllingPlayer));
      os1.delayer.RunAllDelayedActions();
      RunnableConditionalActions conditionalActions = os1.ConditionalActions;
      os1.ConditionalActions.Actions.Add(new SerializableConditionalActionSet()
      {
        Condition = (SerializableCondition) new SCOnAdminGained(),
        Actions = new List<SerializableAction>()
      });
      os1.ConditionalActions.Actions.Add(new SerializableConditionalActionSet()
      {
        Condition = (SerializableCondition) new SCOnAdminGained(),
        Actions = new List<SerializableAction>()
      });
      os1.threadedSaveExecute(false);
      List<Computer> nodes = os1.netMap.nodes;
      screenMan.RemoveScreen((GameScreen) os1);
      OS.WillLoadSave = true;
      OS os2 = new OS();
      os2.SaveGameUserName = fileNameForUsername;
      os2.SaveUserAccountName = username;
      screenMan.AddScreen((GameScreen) os2, new PlayerIndex?(screenMan.controllingPlayer));
      os2.delayer.RunAllDelayedActions();
      Game1.getSingleton().IsMouseVisible = true;
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      if (os2.ConditionalActions.Actions.Count != conditionalActions.Actions.Count)
      {
        ++num;
        str = str + "Save on OS COnditional actions failed! Expected 2, got " + (object) os2.ConditionalActions.Actions.Count;
      }
      screenMan.RemoveScreen((GameScreen) os2);
      OS.TestingPassOnly = false;
      errorsAdded = num;
      return str;
    }

    public static string TestObjectSerializer(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = (List<string>) ObjectSerializer.DeserializeObject(Utils.GenerateStreamFromString(ObjectSerializer.SerializeObject((object) stringList1)), stringList1.GetType());
      if (stringList2 == null || stringList2.Count != 0)
      {
        ++num;
        str += "\nError deserializing empty list";
      }
      stringList1.Add("test 1");
      stringList1.Add("12345");
      List<string> stringList3 = (List<string>) ObjectSerializer.DeepCopy((object) stringList1);
      if (stringList3.Count != 2 || stringList3[0] != "test 1" || stringList3[1] != "12345")
      {
        ++num;
        str += "\nError deserializing empty list";
      }
      VehicleRegistration self = new VehicleRegistration() { licenceNumber = "1123-123", licencePlate = "11-11", vehicle = new VehicleType() { maker = "asdf", model = "another asdf" } };
      VehicleRegistration to = (VehicleRegistration) ObjectSerializer.DeepCopy((object) self);
      if (!Utils.PublicInstancePropertiesEqual<VehicleRegistration>(self, to))
      {
        ++num;
        str += "\nError auto deserializing vehicle info\n";
      }
      errorsAdded = num;
      return str;
    }

    public static string TestDLCSessionUpgrader(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      string activeLocale = Settings.ActiveLocale;
      string path = "Content/Tests/DLCTests/save_preDLC.xml";
      OS.TestingPassOnly = true;
      OS os = new OS();
      os.SaveGameUserName = path;
      os.SaveUserAccountName = "preDLCAccountTest";
      OS.WillLoadSave = true;
      try
      {
        using (FileStream fileStream = File.OpenRead(path))
        {
          os.ForceLoadOverrideStream = (Stream) fileStream;
          screenMan.AddScreen((GameScreen) os, new PlayerIndex?(screenMan.controllingPlayer));
        }
      }
      catch (Exception ex)
      {
        ++num;
        str = str + "\r\nUnexpected error loading pre DLC save file for upgrade test!\r\n" + Utils.GenerateReportFromException(ex) + "\r\n";
      }
      if (os.isLoaded)
      {
        Programs.getComputer(os, "polarSnakeDest");
        if (os.thisComputer.Memory == null)
        {
          ++num;
          str += "\r\nPlayer computer memory dump still null after upgrade!\r\n";
        }
        Computer computer = Programs.getComputer(os, "polarSnakeDest");
        if (computer.Memory == null || computer.Memory.DataBlocks.Count != 1)
        {
          ++num;
          str += "\r\nGibson Link memory dump did not get applied correctly!\r\n";
        }
        Folder folder = Programs.getComputer(os, "dPets_MF").files.root.searchForFolder("Database");
        bool flag = false;
        for (int index = 0; index < folder.files.Count; ++index)
        {
          if (folder.files[index].name.Contains("minx"))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          ++num;
          str = str + "\r\nSession upgrade does not add Minx file to DPets Database!\r\n" + "PeopleWereGeneratedWithDLCAdditions=" + (object) People.PeopleWereGeneratedWithDLCAdditions;
        }
      }
      screenMan.RemoveScreen((GameScreen) os);
      OS.TestingPassOnly = false;
      errorsAdded = num;
      return str;
    }
  }
}
