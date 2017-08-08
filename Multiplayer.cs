// Decompiled with JetBrains decompiler
// Type: Hacknet.Multiplayer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class Multiplayer
  {
    private static int generatedComputerCount = 0;
    public static int PORT = 3020;
    public static char[] delims = new char[2]{ ' ', '\n' };
    public static char[] specSplitDelims = new char[1]{ '#' };

    public static void parseInputMessage(string message, OS os)
    {
      if (message.Equals(""))
        return;
      string[] strArray1 = message.Split(Multiplayer.delims);
      if (strArray1.Length == 0)
        return;
      if (os.thisComputer != null && os.thisComputer.ip.Equals(strArray1[1]))
        os.warningFlash();
      if (strArray1[0].Equals("init"))
      {
        int int32 = Convert.ToInt32(strArray1[1]);
        Utils.random = new Random(int32);
        os.canRunContent = true;
        os.LoadContent();
        os.write("Seed Established :" + (object) int32);
      }
      else if (strArray1[0].Equals("chat"))
      {
        string s = "";
        for (int index = 2; index < strArray1.Length; ++index)
          s = s + strArray1[index] + " ";
        os.write(strArray1[1] + ": " + DisplayModule.splitForWidth(s, 350));
      }
      else if (strArray1[0].Equals("clientConnect"))
        os.write("Connection Established");
      else if (strArray1[0].Equals("cConnection"))
      {
        Computer comp = Multiplayer.getComp(strArray1[1], os);
        if (comp == null)
        {
          os.write("Error in Message : " + message);
        }
        else
        {
          comp.silent = true;
          comp.connect(strArray1[2]);
          comp.silent = false;
          os.opponentLocation = strArray1[1];
        }
      }
      else if (strArray1[0].Equals("cDisconnect"))
      {
        Computer comp = Multiplayer.getComp(strArray1[1], os);
        comp.silent = true;
        comp.disconnecting(strArray1[2], true);
        comp.silent = false;
        os.opponentLocation = "";
      }
      else if (strArray1[0].Equals("cAdmin"))
      {
        Computer comp = Multiplayer.getComp(strArray1[1], os);
        comp.silent = true;
        comp.giveAdmin(strArray1[2]);
        comp.silent = false;
      }
      else if (strArray1[0].Equals("cPortOpen"))
      {
        Computer comp = Multiplayer.getComp(strArray1[1], os);
        comp.silent = true;
        comp.openPort(Convert.ToInt32(strArray1[3]), strArray1[2]);
        comp.silent = false;
      }
      else if (strArray1[0].Equals("cPortClose"))
      {
        Computer comp = Multiplayer.getComp(strArray1[1], os);
        comp.silent = true;
        comp.closePort(Convert.ToInt32(strArray1[3]), strArray1[2]);
        comp.silent = false;
      }
      else if (strArray1[0].Equals("cFile"))
      {
        Computer comp = Multiplayer.getComp(strArray1[1], os);
        comp.silent = true;
        FileEntry f = new FileEntry("", strArray1[3]);
        comp.canReadFile(strArray1[2], f, Convert.ToInt32(strArray1[4]));
        comp.silent = false;
      }
      else if (strArray1[0].Equals("newComp"))
      {
        string[] strArray2 = message.Split(Multiplayer.specSplitDelims);
        Vector2 compLocation = new Vector2((float) Convert.ToInt32(strArray2[2]), (float) Convert.ToInt32(strArray2[3]));
        Computer computer = new Computer(strArray2[5], strArray2[1], compLocation, Convert.ToInt32(strArray2[4]), (byte) 1, os);
        computer.idName = "opponent#" + (object) Multiplayer.generatedComputerCount;
        ++Multiplayer.generatedComputerCount;
        computer.addMultiplayerTargetFile();
        os.netMap.nodes.Add(computer);
        os.opponentComputer = computer;
      }
      else if (strArray1[0].Equals("cDelete"))
      {
        string[] strArray2 = message.Split(Multiplayer.specSplitDelims);
        Computer comp = Multiplayer.getComp(strArray2[1], os);
        List<int> folderPath = new List<int>();
        for (int index = 4; index < strArray2.Length; ++index)
        {
          if (strArray2[index] != "")
            folderPath.Add(Convert.ToInt32(strArray2[index]));
        }
        comp.silent = true;
        comp.deleteFile(strArray2[2], strArray2[3], folderPath);
        comp.silent = false;
      }
      else if (strArray1[0].Equals("cMake"))
      {
        string[] strArray2 = message.Split(Multiplayer.specSplitDelims);
        Computer comp = Multiplayer.getComp(strArray2[1], os);
        List<int> folderPath = new List<int>();
        for (int index = 4; index < strArray2.Length; ++index)
        {
          if (strArray2[index] != "")
            folderPath.Add(Convert.ToInt32(strArray2[index]));
        }
        comp.silent = true;
        comp.makeFile(strArray2[2], strArray2[3], strArray2[4], folderPath, false);
        comp.silent = false;
      }
      else if (strArray1[0].Equals("cMove"))
      {
        string[] strArray2 = message.Split(Multiplayer.specSplitDelims);
        Computer comp = Multiplayer.getComp(strArray2[1], os);
        char[] separator = new char[1]{ '%' };
        List<int> folderPath = new List<int>();
        string[] strArray3 = strArray2[5].Split(separator, 500, StringSplitOptions.RemoveEmptyEntries);
        for (int index = 0; index < strArray3.Length; ++index)
        {
          if (strArray2[index] != "")
            folderPath.Add(Convert.ToInt32(strArray2[index]));
        }
        List<int> destFolderPath = new List<int>();
        string[] strArray4 = strArray2[6].Split(separator, 500, StringSplitOptions.RemoveEmptyEntries);
        for (int index = 0; index < strArray4.Length; ++index)
        {
          if (strArray2[index] != "")
            destFolderPath.Add(Convert.ToInt32(strArray2[index]));
        }
        comp.silent = true;
        comp.moveFile(strArray2[2], strArray2[3], strArray2[4], folderPath, destFolderPath);
        comp.silent = false;
      }
      else if (strArray1[0].Equals("cMkDir"))
      {
        string[] strArray2 = message.Split(Multiplayer.specSplitDelims);
        Computer comp = Multiplayer.getComp(strArray2[1], os);
        List<int> folderPath = new List<int>();
        for (int index = 4; index < strArray2.Length; ++index)
        {
          if (strArray2[index] != "")
            folderPath.Add(Convert.ToInt32(strArray2[index]));
        }
        comp.silent = true;
        comp.makeFolder(strArray2[2], strArray2[3], folderPath);
        comp.silent = false;
      }
      else if (strArray1[0].Equals("cAddUser"))
      {
        string[] strArray2 = message.Split(Multiplayer.specSplitDelims);
        Computer comp = Multiplayer.getComp(strArray2[1], os);
        string name = strArray2[3];
        string pass = strArray2[4];
        byte type = Convert.ToByte(strArray2[5]);
        comp.silent = true;
        comp.addNewUser(strArray2[2], name, pass, type);
        comp.silent = false;
      }
      else if (strArray1[0].Equals("cCopy"))
      {
        Computer comp = Multiplayer.getComp(strArray1[1], os);
        comp.silent = true;
        comp.canCopyFile(strArray1[2], strArray1[3]);
        comp.silent = false;
        FileEntry fileEntry1 = (FileEntry) null;
        for (int index = 0; index < comp.files.root.folders[2].files.Count; ++index)
        {
          if (comp.files.root.folders[2].files[index].name.Equals(strArray1[3]))
            fileEntry1 = comp.files.root.folders[2].files[index];
        }
        FileEntry fileEntry2 = new FileEntry(fileEntry1.data, fileEntry1.name);
        Multiplayer.getComp(strArray1[2], os).files.root.folders[2].files.Add(fileEntry2);
      }
      else if (strArray1[0].Equals("cCDDrive"))
      {
        Computer comp = Multiplayer.getComp(strArray1[1], os);
        comp.silent = true;
        if (strArray1[2].Equals("open"))
          comp.openCDTray(strArray1[1]);
        else
          comp.closeCDTray(strArray1[1]);
        comp.silent = false;
      }
      else if (strArray1[0].Equals("cCrash"))
      {
        Computer comp = Multiplayer.getComp(strArray1[1], os);
        comp.silent = true;
        comp.crash(strArray1[2]);
        comp.silent = false;
      }
      else if (strArray1[0].Equals("cReboot"))
      {
        Computer comp = Multiplayer.getComp(strArray1[1], os);
        comp.silent = true;
        comp.reboot(strArray1[2]);
        comp.silent = false;
      }
      else if (strArray1[0].Equals("cFBClients"))
      {
        Computer comp = Multiplayer.getComp(strArray1[1], os);
        if (os.connectedComp != null && os.connectedComp.ip.Equals(strArray1[1]))
          os.exes.Add((ExeModule) new ForkBombExe(os.getExeBounds(), os));
        comp.silent = true;
        comp.forkBombClients(strArray1[2]);
        comp.silent = false;
      }
      else if (strArray1[0].Equals("eForkBomb"))
      {
        if (!os.thisComputer.ip.Equals(strArray1[1]))
          return;
        ForkBombExe forkBombExe = new ForkBombExe(os.getExeBounds(), os);
        forkBombExe.LoadContent();
        os.exes.Add((ExeModule) forkBombExe);
      }
      else if (strArray1[0].Equals("mpOpponentWin"))
      {
        os.endMultiplayerMatch(false);
      }
      else
      {
        if (strArray1[0].Equals("stayAlive"))
          return;
        os.write("MSG: " + message);
      }
    }

    private static Computer getComp(string ip, OS os)
    {
      for (int index = 0; index < os.netMap.nodes.Count; ++index)
      {
        if (os.netMap.nodes[index].ip.Equals(ip))
          return os.netMap.nodes[index];
      }
      return (Computer) null;
    }
  }
}
