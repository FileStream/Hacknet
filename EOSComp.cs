// Decompiled with JetBrains decompiler
// Type: Hacknet.EOSComp
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  internal class EOSComp
  {
    public static void AddEOSComp(XmlReader rdr, Computer compAttatchedTo, object osObj)
    {
      OS os = (OS) osObj;
      string compName = "Unregistered eOS Device";
      string str1 = compAttatchedTo.idName + "_eos";
      bool flag = false;
      if (rdr.MoveToAttribute("name"))
        compName = ComputerLoader.filter(rdr.ReadContentAsString());
      if (rdr.MoveToAttribute("id"))
        str1 = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("empty"))
        flag = rdr.ReadContentAsString().ToLower() == "true";
      string newPass = "alpine";
      if (rdr.MoveToAttribute("passOverride"))
        newPass = rdr.ReadContentAsString();
      Computer device = new Computer(compName, NetworkMap.generateRandomIP(), os.netMap.getRandomPosition(), 0, (byte) 5, os);
      device.idName = str1;
      string str2 = "ePhone";
      if (rdr.MoveToAttribute("icon"))
        str2 = rdr.ReadContentAsString();
      device.icon = str2;
      device.location = compAttatchedTo.location + Corporation.getNearbyNodeOffset(compAttatchedTo.location, Utils.random.Next(12), 12, os.netMap, 0.0f, false);
      device.setAdminPassword(newPass);
      ComputerLoader.loadPortsIntoComputer("22,3659", (object) device);
      device.portsNeededForCrack = 2;
      EOSComp.GenerateEOSFilesystem(device);
      rdr.Read();
      Folder folder1 = device.files.root.searchForFolder("eos");
      Folder folder2 = folder1.searchForFolder("notes");
      Folder folder3 = folder1.searchForFolder("mail");
      while (!(rdr.Name == "eosDevice") || rdr.IsStartElement())
      {
        if (rdr.Name.ToLower() == "note" && rdr.IsStartElement())
        {
          string nameEntry = (string) null;
          if (rdr.MoveToAttribute("filename"))
            nameEntry = ComputerLoader.filter(rdr.ReadContentAsString());
          int content = (int) rdr.MoveToContent();
          string dataEntry = ComputerLoader.filter(rdr.ReadElementContentAsString().TrimStart());
          if (nameEntry == null)
          {
            int length = dataEntry.IndexOf("\n");
            if (length == -1)
              length = dataEntry.IndexOf("\n");
            if (length == -1)
              length = dataEntry.Length;
            string str3 = dataEntry.Substring(0, length);
            if (str3.Length > 50)
              str3 = str3.Substring(0, 47) + "...";
            nameEntry = str3.Replace(" ", "_").Replace(":", "").ToLower().Trim() + ".txt";
          }
          FileEntry fileEntry = new FileEntry(dataEntry, nameEntry);
          folder2.files.Add(fileEntry);
        }
        if (rdr.Name.ToLower() == "mail" && rdr.IsStartElement())
        {
          string str3 = (string) null;
          string str4 = (string) null;
          if (rdr.MoveToAttribute("username"))
            str3 = ComputerLoader.filter(rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("pass"))
            str4 = ComputerLoader.filter(rdr.ReadContentAsString());
          string dataEntry = "MAIL ACCOUNT : " + str3 + "\nAccount   :" + str3 + "\nPassword :" + str4 + "\nLast Sync :" + DateTime.Now.ToString() + "\n\n" + Computer.generateBinaryString(512);
          string nameEntry = str3 + ".act";
          folder3.files.Add(new FileEntry(dataEntry, nameEntry));
        }
        if (rdr.Name.ToLower() == "file" && rdr.IsStartElement())
        {
          string nameEntry = (string) null;
          if (rdr.MoveToAttribute("name"))
            nameEntry = rdr.ReadContentAsString();
          string path = "home";
          if (rdr.MoveToAttribute("path"))
            path = rdr.ReadContentAsString();
          int content = (int) rdr.MoveToContent();
          string dataEntry = ComputerLoader.filter(rdr.ReadElementContentAsString()).TrimStart();
          device.getFolderFromPath(path, true).files.Add(new FileEntry(dataEntry, nameEntry));
        }
        rdr.Read();
        if (rdr.EOF)
          break;
      }
      if (flag)
      {
        Folder folder4 = folder1.searchForFolder("apps");
        if (folder4 != null)
        {
          folder4.files.Clear();
          folder4.folders.Clear();
        }
      }
      os.netMap.nodes.Add(device);
      ComputerLoader.postAllLoadedActions += (Action) (() => device.links.Add(os.netMap.nodes.IndexOf(compAttatchedTo)));
      if (compAttatchedTo.attatchedDeviceIDs != null)
        compAttatchedTo.attatchedDeviceIDs += ",";
      compAttatchedTo.attatchedDeviceIDs += device.idName;
    }

    public static Folder GenerateEOSFolder()
    {
      Folder folder1 = new Folder("eos");
      Folder folder2 = new Folder("apps");
      Folder folder3 = new Folder("system");
      Folder folder4 = new Folder("notes");
      Folder folder5 = new Folder("mail");
      folder1.folders.Add(folder2);
      folder1.folders.Add(folder4);
      folder1.folders.Add(folder5);
      folder1.folders.Add(folder3);
      folder3.files.Add(new FileEntry(Computer.generateBinaryString(1024), "core.sys"));
      folder3.files.Add(new FileEntry(Computer.generateBinaryString(1024), "runtime.bin"));
      int num = 4 + Utils.random.Next(8);
      for (int index = 0; index < num; ++index)
        folder2.folders.Add(EOSAppGenerator.GetAppFolder());
      return folder1;
    }

    public static void GenerateEOSFilesystem(Computer device)
    {
      if (device.files.root.searchForFolder("eos") != null)
        return;
      device.files.root.folders.Insert(0, EOSComp.GenerateEOSFolder());
    }
  }
}
