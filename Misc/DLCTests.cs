// Decompiled with JetBrains decompiler
// Type: Hacknet.Misc.DLCTests
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Factions;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hacknet.Misc
{
  public static class DLCTests
  {
    public static string TestDLCFunctionality(ScreenManager screenMan, out int errorsAdded)
    {
      string str1 = "";
      int errorsAdded1 = 0;
      bool enableDlc = Settings.EnableDLC;
      Settings.EnableDLC = true;
      string str2 = str1 + DLCTests.TestCustomPortMapping(screenMan, out errorsAdded1) + DLCTests.TestCustomPortMappingOnLoadedComputer(screenMan, out errorsAdded1) + DLCTests.TestMemorySerialization(screenMan, out errorsAdded1) + DLCTests.TestMemoryOnLoadedComputer(screenMan, out errorsAdded1) + DLCTests.TestMemoryInjectionOnLoadedComputer(screenMan, out errorsAdded1) + DLCTests.TestDLCProgression(screenMan, out errorsAdded1) + DLCTests.TestDLCPasswordsCorrect(screenMan, out errorsAdded1) + DLCTests.TestDLCMiscFilesCorrect(screenMan, out errorsAdded1);
      Settings.EnableDLC = enableDlc;
      string str3 = str2 + (str2.Length > 10 ? (object) "\r\n" : (object) " ") + "Complete - " + (object) errorsAdded1 + " errors found";
      errorsAdded = errorsAdded1;
      return str3;
    }

    public static string TestCustomPortMapping(ScreenManager screenMan, out int errorsAdded)
    {
      int errors = 0;
      string ret = "";
      DLCTests.TestComputersForLoad(screenMan, (Action<Computer, Computer>) ((old, current) =>
      {
        if (old.PortRemapping == null && current.PortRemapping != null || old.PortRemapping != null && current.PortRemapping == null)
        {
          ++errors;
          // ISSUE: variable of a compiler-generated type
          DLCTests.\u003C\u003Ec__DisplayClass1 cDisplayClass1 = this;
          // ISSUE: reference to a compiler-generated field
          string str = cDisplayClass1.ret + "\r\nPort Remapping Failed to load on computer " + current.name;
          // ISSUE: reference to a compiler-generated field
          cDisplayClass1.ret = str;
        }
        if (old.PortRemapping == null || current.PortRemapping == null)
          return;
        foreach (KeyValuePair<int, int> keyValuePair in current.PortRemapping)
        {
          if (!old.PortRemapping.ContainsKey(keyValuePair.Key))
          {
            ++errors;
            // ISSUE: variable of a compiler-generated type
            DLCTests.\u003C\u003Ec__DisplayClass1 cDisplayClass1 = this;
            // ISSUE: reference to a compiler-generated field
            string str = cDisplayClass1.ret + "\r\n\r\nPort remapping save error on Computer " + current.name + "\r\n" + (object) keyValuePair.Key + " : " + (object) keyValuePair.Value + " has no corresponding value";
            // ISSUE: reference to a compiler-generated field
            cDisplayClass1.ret = str;
          }
          else if (old.PortRemapping[keyValuePair.Key] != keyValuePair.Value)
          {
            ++errors;
            // ISSUE: variable of a compiler-generated type
            DLCTests.\u003C\u003Ec__DisplayClass1 cDisplayClass1 = this;
            // ISSUE: reference to a compiler-generated field
            string str = cDisplayClass1.ret + "\r\n\r\nPort remapping save error on Computer " + current.name + "\r\nKey " + (object) keyValuePair.Key + " : Current: " + (object) keyValuePair.Value + " Old: " + (object) old.PortRemapping[keyValuePair.Key];
            // ISSUE: reference to a compiler-generated field
            cDisplayClass1.ret = str;
          }
        }
      }));
      errorsAdded = errors;
      return ret;
    }

    public static string TestMemorySerialization(ScreenManager screenMan, out int errorsAdded)
    {
      int errors = 0;
      string ret = "";
      string[] wildcardUpgradeServerIDs = new string[3]{ "polarSnakeDest", "naixGateway", "BitWorkServer" };
      DLCTests.TestComputersForLoad(screenMan, (Action<Computer, Computer>) ((old, current) =>
      {
        if ((old.Memory != null && current.Memory == null || old.Memory == null && current.Memory != null) && !((IEnumerable<string>) wildcardUpgradeServerIDs).Contains<string>(old.idName))
        {
          ++errors;
          // ISSUE: reference to a compiler-generated field
          this.ret += "\r\n\r\nMemory serialization error: old and current null mismatch";
        }
        if (old.Memory == null || current.Memory == null)
          return;
        string str = old.Memory.TestEqualsWithErrorReport(current.Memory);
        if (str.Length > 0)
        {
          ++errors;
          // ISSUE: reference to a compiler-generated field
          this.ret += str;
        }
      }));
      errorsAdded = errors;
      return ret;
    }

    public static string TestCustomPortMappingOnLoadedComputer(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      Computer computer = (Computer) ComputerLoader.loadComputer("Content/Tests/DLCTests/TestComp1.xml", false, false);
      if (computer.PortRemapping == null)
      {
        ++num;
        str += "\r\nLoaded test comp1 Does not correctly load in port remapping.";
      }
      if (computer.PortRemapping.Count != 4)
      {
        ++num;
        str = str + "\r\nLoaded test comp reads in " + (object) computer.PortRemapping.Count + " remaps, instead of the expected 4";
      }
      if (computer.PortRemapping[22] != 1234 || computer.PortRemapping[21] != 99 || computer.PortRemapping[80] != 3 || computer.PortRemapping[1433] != 1432)
      {
        ++num;
        str += "\r\nLoaded test comp reads in incorrect port mapping.";
      }
      errorsAdded = num;
      return str;
    }

    public static string TestMemoryOnLoadedComputer(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str1 = "";
      Computer computer = (Computer) ComputerLoader.loadComputer("Content/Tests/DLCTests/TestComp1.xml", false, false);
      if (computer.Memory == null)
      {
        ++num;
        str1 += "\r\nLoaded test comp1 Does not correctly load in memory.";
      }
      if (computer.Memory.DataBlocks.Count != 3)
      {
        ++num;
        str1 = str1 + "\r\nLoaded test comp reads in " + (object) computer.Memory.DataBlocks.Count + " data blocks in memory, instead of the expected 3";
      }
      if (computer.Memory.CommandsRun.Count != 1)
      {
        ++num;
        str1 = str1 + "\r\nLoaded test comp reads in " + (object) computer.Memory.CommandsRun.Count + " commands run in memory, instead of the expected 1";
      }
      else if (computer.Memory.CommandsRun[0] != "connect 123.123.123.123")
      {
        ++num;
        str1 = str1 + "\r\nLoaded test comp reads in " + computer.Memory.CommandsRun[0] + " as command, instead of the expected value";
      }
      string str2 = MemoryContents.GetMemoryFromEncodedFileString(computer.Memory.GetEncodedFileString()).TestEqualsWithErrorReport(computer.Memory);
      if (str2.Length > 0)
      {
        ++num;
        str1 = str1 + "\r\nErrors in Memory file serialization cycle!\r\n" + str2 + "\n\n";
      }
      errorsAdded = num;
      return str1;
    }

    public static string TestMemoryInjectionOnLoadedComputer(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      Computer computer = (Computer) ComputerLoader.loadComputer("Content/Tests/DLCTests/TestCompNoMemory.xml", false, false);
      if (computer.Memory != null)
      {
        ++num;
        str += "\r\nLoaded test comp for no memory does infact have memory somehow????\r\n";
      }
      else
      {
        MemoryDumpInjector.InjectMemory("Content/Tests/DLCTests/InjectedMemory.xml", (object) computer);
        if (computer.Memory == null)
        {
          ++num;
          str += "\r\nInjecting memory into loaded comp failed!\r\n";
        }
        else if (computer.Memory.DataBlocks.Count != 2)
        {
          ++num;
          str = str + "\r\nLoaded test comp reads in " + (object) computer.Memory.DataBlocks.Count + " data blocks in memory, instead of the expected 2";
        }
      }
      errorsAdded = num;
      return str;
    }

    public static string TestDLCProgression(ScreenManager screenMan, out int errorsAdded)
    {
      int errors = 0;
      string ret = "";
      DLCTests.SetupTestingEnvironment(screenMan, (Action<OS, List<Computer>>) ((os, comps) =>
      {
        Console.WriteLine("Testing DLC Progression in " + Settings.ActiveLocale);
        SessionAccelerator.AccelerateSessionToDLCEND((object) os);
        if (Programs.getComputer(os, "dhsDrop").files.root.searchForFolder("home").searchForFile("p_SQL_399.gz") == null)
        {
          ++errors;
          // ISSUE: reference to a compiler-generated field
          this.ret += "\r\nExpo Grave Mission files not copied over to drop server! Mission Incompletable.";
        }
        if (!Programs.getComputer(os, "ds4_expo").ports.Contains(21))
        {
          ++errors;
          // ISSUE: reference to a compiler-generated field
          this.ret += "\r\nExpo Grave Website does not have port 21 added! Mission Incompletable.";
        }
        Folder folder = Programs.getComputer(os, "ds3_mail").files.root.searchForFolder("mail").searchForFolder("accounts").searchForFolder("cornch1p");
        if (folder == null)
        {
          ++errors;
          // ISSUE: reference to a compiler-generated field
          this.ret += "\r\nMagma Mailbox server for It Follows (set3) does not have account! in file, replace kburnaby with cornch1p to fix.";
        }
        else if (folder.searchForFolder("inbox").files.Count <= 0)
        {
          ++errors;
          // ISSUE: reference to a compiler-generated field
          this.ret += "\r\nMagma Mailbox server for It Follows (set3) does not have emails! in file, replace kburnaby with cornch1p to fix.";
        }
        CustomFaction fromFile = CustomFaction.ParseFromFile("Content/DLC/DLCFaction.xml");
        if (fromFile.idName != "Bibliotheque")
        {
          ++errors;
          // ISSUE: variable of a compiler-generated type
          DLCTests.\u003C\u003Ec__DisplayClass7 cDisplayClass7 = this;
          // ISSUE: reference to a compiler-generated field
          string str = cDisplayClass7.ret + "\r\nHub Faction ID Name is wrong: " + fromFile.idName;
          // ISSUE: reference to a compiler-generated field
          cDisplayClass7.ret = str;
        }
        CustomFactionAction customAction = fromFile.CustomActions[6];
        for (int index = 0; index < customAction.TriggerActions.Count; ++index)
        {
          SAAddConditionalActions triggerAction = customAction.TriggerActions[index] as SAAddConditionalActions;
          if (triggerAction != null && triggerAction.Filepath.Contains("PetsAcceptedActions"))
          {
            ++errors;
            // ISSUE: reference to a compiler-generated field
            this.ret += "\r\nHub Faction (stage 7) contains PetsAcceptedActions.xml loader - it should be only on the mission itself! Remove it!";
          }
        }
        if (Programs.getComputer(os, "dPets_MF").name.ToLower().Contains("digipets"))
        {
          ++errors;
          // ISSUE: reference to a compiler-generated field
          this.ret += "\r\nNeopals server rename from DigiPets is not complete in this version! Fix it! Remember to do foldernames too.\r\n";
        }
        bool flag = false;
        Computer computer = Programs.getComputer(os, "dMF_1_Misc");
        for (int index = 0; index < computer.users.Count; ++index)
        {
          if (computer.users[index].name == "listen" && computer.users[index].pass == "4TL4S")
            flag = true;
        }
        if (!flag)
        {
          ++errors;
          // ISSUE: reference to a compiler-generated field
          this.ret += "\r\nAtlas server (MemForensics/Atlas) needs user listen w. pass 4TL4S\r\n";
        }
        if (((ActiveMission) ComputerLoader.readMission("Content/DLC/Missions/Neopals/PetsMission1.xml")).email.body.Contains("DigiPoints"))
        {
          ++errors;
          // ISSUE: reference to a compiler-generated field
          this.ret += "\r\nNeopals missions references DigiPoints, where it should be NeoPoints! Remember to check the goals field!\r\n";
        }
        if (Programs.getComputer(os, "dhsDrop").admin == null)
          return;
        ++errors;
        // ISSUE: reference to a compiler-generated field
        this.ret += "\r\nDLC Drop server has an admin - remove it with type=none. This stops the player being auto logged out.\r\n";
      }));
      errorsAdded = errors;
      return ret;
    }

    public static string TestDLCPasswordsCorrect(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string ret = "";
      DLCTests.SetupTestingEnvironment(screenMan, (Action<OS, List<Computer>>) ((os, comps) =>
      {
        // ISSUE: reference to a compiler-generated field
        this.ret += DLCTests.TestPassword(os, "ds4_grave", "fma93dK");
        // ISSUE: reference to a compiler-generated field
        this.ret += DLCTests.TestPassword(os, "dpae_psy_1", "catsarebestpet");
        // ISSUE: reference to a compiler-generated field
        this.ret += DLCTests.TestPassword(os, "dpa_psylance", "1185JACK");
      }));
      errorsAdded = num;
      return ret;
    }

    public static string TestDLCMiscFilesCorrect(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      if (!Utils.readEntireFile("Content/DLC/Docs/KaguyaTrial2.txt").Contains("216.239.32.181"))
      {
        ++num;
        str += "\r\nDLC/Docs/KaguyaTrial2.txt Does not have the correct IP! Needs 216.239.32.181";
      }
      errorsAdded = num;
      return str;
    }

    private static string TestPassword(OS os, string compID, string passToFind)
    {
      Computer computer = Programs.getComputer(os, compID);
      if (computer == null)
        return "\r\nComputer " + compID + " Not found when searching for password " + passToFind + "\r\n";
      bool hasPass = false;
      DLCTests.CheckAllFiles(computer.files.root, (Action<FileEntry>) (f =>
      {
        if (!f.data.Contains(passToFind))
          return;
        hasPass = true;
      }));
      if (hasPass)
        return "";
      return "\r\nComputer " + compID + " (" + computer.name + ") Does not contain necessary password: " + passToFind + "\r\n";
    }

    private static void CheckAllFiles(Folder f, Action<FileEntry> act)
    {
      for (int index = 0; index < f.files.Count; ++index)
      {
        if (act != null)
          act(f.files[index]);
      }
      for (int index = 0; index < f.folders.Count; ++index)
        DLCTests.CheckAllFiles(f.folders[index], act);
    }

    internal static void SetupTestingEnvironment(ScreenManager screenMan, Action<OS, List<Computer>> CompareSessions)
    {
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
      CompareSessions(os2, nodes);
      screenMan.RemoveScreen((GameScreen) os2);
      OS.TestingPassOnly = false;
    }

    internal static void TestComputersForLoad(ScreenManager screenMan, Action<Computer, Computer> CompareComputerAfterLoad)
    {
      DLCTests.SetupTestingEnvironment(screenMan, (Action<OS, List<Computer>>) ((os, oldComps) =>
      {
        for (int index = 0; index < oldComps.Count; ++index)
        {
          Computer oldComp = oldComps[index];
          Computer computer = Programs.getComputer(os, oldComp.ip);
          CompareComputerAfterLoad(oldComp, computer);
        }
      }));
    }
  }
}
