// Decompiled with JetBrains decompiler
// Type: Hacknet.Misc.SaveFixHacks
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Mission;
using System;
using System.Collections.Generic;

namespace Hacknet.Misc
{
  public static class SaveFixHacks
  {
    public static void FixSavesWithTerribleHacks(object osObj)
    {
      OS os = (OS) osObj;
      Computer computer1 = Programs.getComputer(os, "mainHubAssets");
      if (computer1 != null)
      {
        Folder folder1 = computer1.files.root.searchForFolder("bin");
        if (folder1 != null)
        {
          Folder folder2 = folder1.searchForFolder("Sequencer");
          if (folder2 != null)
          {
            FileEntry fileEntry = folder2.searchForFile("Sequencer.exe");
            if (fileEntry == null)
              folder2.files.Add(new FileEntry(PortExploits.crackExeData[17], "Sequencer.exe"));
            else
              fileEntry.data = PortExploits.crackExeData[17];
          }
        }
      }
      Computer computer2 = Programs.getComputer(os, "pacemakerSW_BE");
      if (computer2 != null)
      {
        Console.WriteLine("Searching for pacemaker comp");
        Folder folder1 = computer2.files.root.searchForFolder("projects");
        if (folder1 != null)
        {
          Console.WriteLine("Searching for pacemaker projects");
          Folder folder2 = folder1.searchForFolder("KellisBT");
          if (folder2 != null)
          {
            Folder folder3 = folder2.searchForFolder("Tests");
            if (folder3 != null)
            {
              Console.WriteLine("Searching for pacemaker file");
              FileEntry fileEntry = folder3.searchForFile("PacemakerFirmware_Cycle_Test.dll");
              if (fileEntry == null)
                folder3.files.Add(new FileEntry(PortExploits.DangerousPacemakerFirmware, "PacemakerFirmware_Cycle_Test.dll"));
              else
                fileEntry.data = PortExploits.DangerousPacemakerFirmware;
            }
          }
        }
      }
      if (!os.HasLoadedDLCContent)
        return;
      List<Computer> computerList = new List<Computer>();
      if (Programs.getComputer(os, "dPets_MF").links.Count == 0)
      {
        ComputerLoader.postAllLoadedActions = (Action) null;
        List<string> dlcList = BootLoadList.getDLCList();
        for (int index = 0; index < dlcList.Count; ++index)
        {
          Computer computer3 = (Computer) ComputerLoader.loadComputer(dlcList[index], true, true);
          computerList.Add(computer3);
        }
        ComputerLoader.postAllLoadedActions();
      }
      for (int index = 0; index < computerList.Count; ++index)
        Programs.getComputer(os, computerList[index].idName).links = computerList[index].links;
      Folder folder = Programs.getComputer(os, "dPets_MF").files.root.searchForFolder("Database");
      bool flag = false;
      if (folder.files.Count > 0 && folder.files[0].data.Contains("DigiPet"))
      {
        for (int index = 0; index < folder.files.Count; ++index)
        {
          folder.files[index].data = folder.files[index].data.Replace("DigiPet", "Neopal");
          if (folder.files[index].data.Contains("Minx"))
            flag = true;
        }
        if (flag)
          ;
        DLCHubServer daemon = (DLCHubServer) Programs.getComputer(os, "dhs").getDaemon(typeof (DLCHubServer));
        daemon.navigatedTo();
        if (daemon.ActiveMissions.Count == 0)
          daemon.AddMission(os.currentMission, os.defaultUser.name, false);
      }
      if (os.Flags.HasFlag("KaguyaTrialComplete") && !os.netMap.visibleNodes.Contains(os.netMap.nodes.IndexOf(Programs.getComputer(os, "dhs"))))
        os.Flags.RemoveFlag("KaguyaTrialComplete");
    }

    public static string GetReportOnHashCodeOfEmptyStringForOtherCLRVersion()
    {
      string data1 = Utils.readEntireFile("DebugTest.txt");
      string data2 = FileEncrypter.EncryptString("test content", "headercontent", "1.1.1.1.1", "12345asdf", (string) null);
      Console.WriteLine("password 12345asdf hashes to: " + (object) (ushort) "12345asdf".GetHashCode());
      for (ushort pass = 0; (int) pass < (int) ushort.MaxValue; ++pass)
      {
        if (FileEncrypter.TestingDecryptString(data1, pass)[0].Contains("Firmware"))
        {
          Console.WriteLine("Match Found: " + (object) pass + " is valid match, current CLR produces: " + (object) (ushort) "".GetHashCode());
        }
        else
        {
          if ((int) pass % 1000 == 0)
            Console.Write(".");
          if ((int) pass % 50000 == 0)
            Console.WriteLine("");
        }
        string[] strArray = FileEncrypter.TestingDecryptString(data2, pass);
        if (strArray[2] != null && strArray[2].Contains("test") && (strArray[4] != null && strArray[4] == "1") && strArray[5] != null && strArray[5] == "ENCODED")
          Console.WriteLine("Alt Match Found: " + (object) pass + " is valid match, current CLR produces: " + (object) (ushort) "".GetHashCode());
      }
      Console.WriteLine("Operation Complete");
      return "";
    }
  }
}
