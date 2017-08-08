// Decompiled with JetBrains decompiler
// Type: Hacknet.Computer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Security;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Hacknet
{
  [Serializable]
  internal class Computer
  {
    public static float BASE_BOOT_TIME = Settings.isConventionDemo ? 15f : 25.5f;
    public static float BASE_REBOOT_TIME = 10.5f;
    public static float BASE_PROXY_TICKS = 30f;
    public static float BASE_TRACE_TIME = 15f;
    public int portsNeededForCrack = 0;
    public bool silent = false;
    public bool disabled = false;
    internal float bootTimer = 0.0f;
    public bool userLoggedIn = false;
    public Dictionary<int, int> PortRemapping = (Dictionary<int, int>) null;
    public string icon = (string) null;
    public float highlightFlashTime = 0.0f;
    public bool AllowsDefaultBootModule = true;
    public bool hasProxy = false;
    public float proxyOverloadTicks = 0.0f;
    public float startingOverloadTicks = -1f;
    public bool proxyActive = false;
    public ShellExe reportingShell = (ShellExe) null;
    public ExternalCounterpart externalCounterpart = (ExternalCounterpart) null;
    public string attatchedDeviceIDs = (string) null;
    public Firewall firewall = (Firewall) null;
    public bool firewallAnalysisInProgress = false;
    public bool HasTracker = false;
    public Administrator admin = (Administrator) null;
    public const byte CORPORATE = 1;
    public const byte HOME = 2;
    public const byte SERVER = 3;
    public const byte EMPTY = 4;
    public const byte EOS = 5;
    public string name;
    public string idName;
    public string ip;
    public Vector2 location;
    public FileSystem files;
    public int securityLevel;
    public float traceTime;
    public string adminIP;
    public List<UserDetail> users;
    public List<int> links;
    public List<int> ports;
    public List<byte> portsOpen;
    public UserDetail currentUser;
    private float timeLastPinged;
    public byte type;
    public string adminPass;
    public List<Daemon> daemons;
    private OS os;
    public MemoryContents Memory;

    public Computer(string compName, string compIP, Vector2 compLocation, int seclevel, byte compType, OS opSystem)
    {
      this.name = compName;
      this.ip = compIP;
      this.location = compLocation;
      this.type = compType;
      this.files = this.generateRandomFileSystem();
      this.idName = compName.Replace(" ", "_");
      this.os = opSystem;
      this.traceTime = -1f;
      this.securityLevel = seclevel;
      this.adminIP = NetworkMap.generateRandomIP();
      this.users = new List<UserDetail>();
      this.adminPass = PortExploits.getRandomPassword();
      this.users.Add(new UserDetail("admin", this.adminPass, (byte) 1));
      this.ports = new List<int>(seclevel);
      this.portsOpen = new List<byte>(seclevel);
      this.openPortsForSecurityLevel(seclevel);
      this.links = new List<int>();
      this.daemons = new List<Daemon>();
    }

    public void initDaemons()
    {
      for (int index = 0; index < this.daemons.Count; ++index)
      {
        this.daemons[index].initFiles();
        if (this.daemons[index].isListed)
          this.daemons[index].registerAsDefaultBootDaemon();
      }
    }

    public FileSystem generateRandomFileSystem()
    {
      FileSystem fileSystem = new FileSystem();
      if ((int) this.type != 5 && (int) this.type != 4)
      {
        int num1 = Utils.random.Next(6);
        for (int index1 = 0; index1 < num1; ++index1)
        {
          int num2 = 0;
          string str1;
          do
          {
            str1 = (num2 > 10 ? "AA" : "") + this.generateFolderName(Utils.random.Next(100));
            ++num2;
          }
          while (fileSystem.root.folders[0].searchForFolder(str1) != null);
          Folder folder = new Folder(str1);
          int num3 = Utils.random.Next(3);
          for (int index2 = 0; index2 < num3; ++index2)
          {
            if (Utils.random.NextDouble() > 0.8)
            {
              int num4 = 0;
              string str2;
              do
              {
                str2 = this.generateFileName(Utils.random.Next(300));
                ++num4;
                if (num4 > 3)
                  str2 = ((int) Utils.getRandomChar() + (int) Utils.getRandomChar()).ToString() + str2;
              }
              while (folder.searchForFile(str2) != null);
              folder.files.Add(new FileEntry(Utils.flipCoin() ? this.generateFileData(Utils.random.Next(500)) : Computer.generateBinaryString(500), str2));
            }
            else
            {
              FileEntry fileEntry = new FileEntry();
              string name = fileEntry.name;
              while (folder.searchForFile(fileEntry.name) != null)
                fileEntry.name = ((int) Utils.getRandomChar() + (int) Utils.getRandomChar()).ToString() + name;
              folder.files.Add(fileEntry);
            }
          }
          fileSystem.root.folders[0].folders.Add(folder);
        }
      }
      else if ((int) this.type == 5)
        fileSystem.root.folders.Insert(0, EOSComp.GenerateEOSFolder());
      return fileSystem;
    }

    public void openPortsForSecurityLevel(int security)
    {
      this.portsNeededForCrack = security - 1;
      if (security >= 5)
      {
        --this.portsNeededForCrack;
        float time = 0.0f;
        for (int index = 4; index < security; ++index)
          time += Computer.BASE_PROXY_TICKS / (float) (index - 3);
        this.addProxy(time);
      }
      switch (security)
      {
        case 1:
          this.openPorts(PortExploits.portNums.Count - 1);
          break;
        default:
          this.openPorts(PortExploits.portNums.Count);
          break;
      }
      if (security >= 4)
        this.traceTime = (float) Math.Max(10 - security, 3) * Computer.BASE_TRACE_TIME;
      if (security < 5)
        return;
      this.firewall = new Firewall(security - 5);
      this.admin = (Administrator) new BasicAdministrator();
    }

    private void openPorts(int n)
    {
      for (int index = 4 - 1; index >= 0; --index)
      {
        this.ports.Add(PortExploits.portNums[index]);
        this.portsOpen.Add((byte) 0);
      }
    }

    public void addProxy(float time)
    {
      if ((double) time <= 0.0)
        return;
      this.hasProxy = true;
      this.proxyActive = true;
      this.proxyOverloadTicks = time;
      this.startingOverloadTicks = this.proxyOverloadTicks;
    }

    public void addFirewall(int level)
    {
      this.firewall = new Firewall(level);
    }

    public void addFirewall(int level, string solution)
    {
      this.firewall = new Firewall(level, solution);
    }

    public void addFirewall(int level, string solution, float additionalTime)
    {
      this.firewall = new Firewall(level, solution, additionalTime);
    }

    public void addMultiplayerTargetFile()
    {
      this.files.root.folders[0].files.Add(new FileEntry("#CRITICAL SYSTEM FILE - DO NOT MODIFY#\n\n" + Computer.generateBinaryString(2000), "system32.sys"));
    }

    private void sendNetworkMessage(string s)
    {
      if (this.os.multiplayer && !this.silent)
        this.os.sendMessage(s);
      if (this.externalCounterpart == null)
        return;
      this.externalCounterpart.writeMessage(s);
    }

    private void tryExternalCounterpartDisconnect()
    {
      if (this.externalCounterpart == null)
        return;
      this.externalCounterpart.disconnect();
    }

    public void hostileActionTaken()
    {
      if (this.os.connectedComp == null || !this.os.connectedComp.ip.Equals(this.ip))
        return;
      if ((double) this.traceTime > 0.0)
        this.os.traceTracker.start(this.traceTime);
      if ((double) this.os.timer - (double) this.timeLastPinged > 0.349999994039536)
      {
        SFX.addCircle(this.getScreenSpacePosition(), this.os.brightLockedColor, 25f);
        this.timeLastPinged = this.os.timer;
      }
    }

    public void bootupTick(float t)
    {
      this.bootTimer -= t;
      if ((double) this.bootTimer > 0.0)
        return;
      this.disabled = false;
    }

    public void log(string message)
    {
      if (this.disabled)
        return;
      if (this.reportingShell != null)
        this.reportingShell.reportedTo(message);
      message = "@" + (object) (int) OS.currentElapsedTime + " " + message;
      string str1 = message;
      if (str1.Length > 256)
        str1 = str1.Substring(0, 256);
      string str2 = str1.Replace(" ", "_");
      int num = 0;
      Folder folder = this.files.root.searchForFolder("log");
      bool flag;
      do
      {
        flag = false;
        for (int index = 0; index < folder.files.Count; ++index)
        {
          if (folder.files[index] != null && folder.files[index].name == str2)
          {
            flag = true;
            ++num;
            str2 = (str1 + "_" + (object) num).Replace(" ", "_");
          }
        }
      }
      while (flag);
      string nameEntry = str2;
      this.files.root.searchForFolder("log").files.Insert(0, new FileEntry(message, nameEntry));
    }

    public string generateFolderName(int seed)
    {
      return "NewFolder" + (object) seed;
    }

    public string generateFileName(int seed)
    {
      return "Data" + (object) seed;
    }

    public string generateFileData(int seed)
    {
      string str = "";
      for (int index = 0; index < seed; ++index)
        str = str + " " + (object) index;
      return str;
    }

    public bool connect(string ipFrom)
    {
      if (this.disabled)
        return false;
      WhitelistConnectionDaemon daemon = (WhitelistConnectionDaemon) this.getDaemon(typeof (WhitelistConnectionDaemon));
      if (daemon != null && ipFrom == this.os.thisComputer.ip && !daemon.IPCanPassWhitelist(ipFrom, false))
      {
        daemon.DisconnectTarget();
        return false;
      }
      this.log("Connection: from " + ipFrom);
      this.sendNetworkMessage("cConnection " + this.ip + " " + ipFrom);
      if (this.externalCounterpart != null)
        this.externalCounterpart.establishConnection();
      this.userLoggedIn = false;
      return true;
    }

    public void addNewUser(string ipFrom, string name, string pass, byte type)
    {
      this.addNewUser(ipFrom, new UserDetail(name, pass, type));
    }

    public void addNewUser(string ipFrom, UserDetail usr)
    {
      this.users.Add(usr);
      if (!this.silent)
        this.log("User Account Added: from " + ipFrom + " -Name: " + this.name);
      this.sendNetworkMessage("cAddUser #" + this.ip + "#" + ipFrom + "#" + this.name + "#" + usr.pass + "#" + (object) usr.type);
      for (int index = 0; index < this.daemons.Count; ++index)
        this.daemons[index].userAdded(usr.name, usr.pass, usr.type);
    }

    public void crash(string ipFrom)
    {
      if (this.os.connectedComp != null && this.os.connectedComp.Equals((object) this) && !this.os.connectedComp.Equals((object) this.os.thisComputer))
      {
        bool silent = this.os.connectedComp.silent;
        Computer connectedComp = this.os.connectedComp;
        connectedComp.silent = true;
        this.os.connectedComputerCrashed(this);
        connectedComp.silent = silent;
      }
      else if (this.os.thisComputer.Equals((object) this))
        this.os.thisComputerCrashed();
      if (!this.silent)
        this.log("CRASH REPORT: Kernel Panic -- Fatal Trap");
      this.disabled = true;
      this.bootTimer = Computer.BASE_BOOT_TIME;
      this.tryExternalCounterpartDisconnect();
      PostProcessor.dangerModeEnabled = false;
      this.sendNetworkMessage("cCrash " + this.ip + " " + ipFrom);
    }

    public void reboot(string ipFrom)
    {
      if (this.os.connectedComp != null && this.os.connectedComp.Equals((object) this) && !this.os.connectedComp.Equals((object) this.os.thisComputer))
        this.os.connectedComputerCrashed(this);
      else if (this.os.thisComputer.Equals((object) this) || this.os.thisComputer.Equals((object) this.os.connectedComp))
        this.os.rebootThisComputer();
      if (!this.silent)
        this.log("Rebooting system : " + ipFrom);
      this.disabled = true;
      this.bootTimer = Computer.BASE_REBOOT_TIME;
      this.tryExternalCounterpartDisconnect();
      this.sendNetworkMessage("cReboot " + this.ip + " " + ipFrom);
    }

    public bool canReadFile(string ipFrom, FileEntry f, int index)
    {
      bool flag = false;
      if (ipFrom.Equals(this.adminIP))
        flag = true;
      if (ipFrom == this.os.thisComputer.ip && (this.currentUser.name != null && (int) this.currentUser.type == 0))
      {
        flag = true;
      }
      else
      {
        for (int index1 = 0; index1 < this.users.Count; ++index1)
        {
          if (ipFrom.Equals((object) this.users[index1]))
            flag = true;
        }
      }
      if (!flag)
        return false;
      if ((int) f.name[0] != 64)
      {
        this.log("FileRead: by " + ipFrom + " - file:" + f.name);
        this.sendNetworkMessage("cFile " + this.ip + " " + ipFrom + " " + f.name + " " + (object) index);
      }
      return true;
    }

    public bool canCopyFile(string ipFrom, string name)
    {
      if ((int) this.currentUser.type != 0 && (!this.silent && !ipFrom.Equals(this.adminIP)))
        return false;
      this.log("FileCopied: by " + ipFrom + " - file:" + name);
      this.sendNetworkMessage("cCopy " + this.ip + " " + ipFrom + " " + name);
      return true;
    }

    public bool deleteFile(string ipFrom, string name, List<int> folderPath)
    {
      bool flag1 = false;
      if ((int) this.currentUser.type == 1 || (int) this.currentUser.type == 0)
        flag1 = true;
      if (!flag1 && !this.silent && !ipFrom.Equals(this.adminIP) && !ipFrom.Equals(this.ip))
        return false;
      if (name == "*")
      {
        Folder startFolder = this.files.root;
        if (folderPath.Count > 0)
          startFolder = Programs.getFolderFromNavigationPath(folderPath, startFolder, this.os);
        bool flag2 = true;
        List<string> stringList = new List<string>();
        for (int index = 0; index < startFolder.files.Count; ++index)
        {
          if (startFolder.files[index] != null && !string.IsNullOrWhiteSpace(startFolder.files[index].name))
            stringList.Add(startFolder.files[index].name);
        }
        for (int index = 0; index < stringList.Count; ++index)
          flag2 &= this.deleteFile(ipFrom, stringList[index], folderPath);
        return flag2;
      }
      if ((int) name[0] != 64)
        this.log("FileDeleted: by " + ipFrom + " - file:" + name);
      Folder startFolder1 = this.files.root;
      if (folderPath.Count > 0)
        startFolder1 = Programs.getFolderFromNavigationPath(folderPath, startFolder1, this.os);
      string str = name;
      if (this.os.multiplayer && (int) name[0] == 64)
        str = name.Substring(name.IndexOf('_'));
      for (int index = 0; index < startFolder1.files.Count; ++index)
      {
        string name1 = startFolder1.files[index].name;
        if (!this.os.multiplayer || (int) name[0] != 64 ? name1.Equals(name) : name1.Substring(name1.IndexOf('_')).Equals(str))
        {
          startFolder1.files.RemoveAt(index);
          --index;
        }
      }
      string s = "cDelete #" + this.ip + "#" + ipFrom + "#" + name;
      for (int index = 0; index < folderPath.Count; ++index)
        s = s + "#" + (object) folderPath[index];
      this.sendNetworkMessage(s);
      return true;
    }

    public bool moveFile(string ipFrom, string name, string newName, List<int> folderPath, List<int> destFolderPath)
    {
      if ((int) this.currentUser.type != 0 && (!this.silent && !ipFrom.Equals(this.adminIP) && !ipFrom.Equals(this.ip)))
        return false;
      Folder root = this.files.root;
      Folder fromNavigationPath = Programs.getFolderFromNavigationPath(folderPath, this.files.root, this.os);
      Folder folder1 = Programs.getFolderFromNavigationPath(destFolderPath, this.files.root, this.os);
      if (newName.StartsWith("/"))
      {
        if (destFolderPath.Count == 0 || folderPath.Count > 0 && destFolderPath.Count == folderPath.Count && destFolderPath[0] == folderPath[0])
        {
          folder1 = this.files.root;
          newName = newName.Substring(1);
          Folder folder2 = folder1.searchForFolder(newName);
          if (folder2 != null)
          {
            folder1 = folder2;
            newName = name;
          }
        }
        else
          newName = newName.Substring(1);
      }
      FileEntry fileEntry = (FileEntry) null;
      for (int index = 0; index < fromNavigationPath.files.Count; ++index)
      {
        if (fromNavigationPath.files[index].name == name)
        {
          fileEntry = fromNavigationPath.files[index];
          fromNavigationPath.files.RemoveAt(index);
          break;
        }
      }
      if (fileEntry == null)
      {
        this.os.write("File not Found");
        return false;
      }
      if (newName == "" || newName == " ")
        newName = name;
      fileEntry.name = newName;
      string name1 = fileEntry.name;
      int num = 1;
      while (folder1.searchForFile(fileEntry.name) != null)
      {
        fileEntry.name = name1 + "(" + (object) num + ")";
        ++num;
      }
      folder1.files.Add(fileEntry);
      string str = "cMove #" + this.ip + "#" + ipFrom + "#" + name + "#" + newName + "#";
      for (int index = 0; index < folderPath.Count; ++index)
        str = str + "%" + (object) folderPath[index];
      string s = str + "#";
      for (int index = 0; index < destFolderPath.Count; ++index)
        s = s + "%" + (object) destFolderPath[index];
      this.sendNetworkMessage(s);
      this.log("FileMoved: by " + ipFrom + " - file:" + name + " To: " + newName);
      return true;
    }

    public bool makeFile(string ipFrom, string name, string data, List<int> folderPath, bool isUpload = false)
    {
      if (!isUpload && !this.silent && !ipFrom.Equals(this.adminIP) && !ipFrom.Equals(this.ip))
        return false;
      if ((int) name[0] != 64)
        this.log("FileCreated: by " + ipFrom + " - file:" + name);
      Folder folder = this.files.root;
      if (folderPath.Count > 0)
      {
        for (int index = 0; index < folderPath.Count; ++index)
        {
          if (folder.folders.Count > folderPath[index])
            folder = folder.folders[folderPath[index]];
        }
      }
      if (isUpload)
        folder.files.Insert(0, new FileEntry(data, name));
      else
        folder.files.Add(new FileEntry(data, name));
      string s = "cMake #" + this.ip + "#" + ipFrom + "#" + name + "#" + data;
      for (int index = 0; index < folderPath.Count; ++index)
        s = s + "#" + (object) folderPath[index];
      this.sendNetworkMessage(s);
      return true;
    }

    public bool makeFolder(string ipFrom, string name, List<int> folderPath)
    {
      if (!this.silent && !ipFrom.Equals(this.adminIP) && !ipFrom.Equals(this.ip))
        return false;
      if ((int) name[0] != 64)
        this.log("FolderCreated: by " + ipFrom + " - folder:" + name);
      Folder folder = this.files.root;
      if (folderPath.Count > 0)
      {
        for (int index = 0; index < folderPath.Count; ++index)
        {
          if (folder.folders.Count > folderPath[index])
            folder = folder.folders[folderPath[index]];
        }
      }
      folder.folders.Add(new Folder(name));
      string s = "cMkDir #" + this.ip + "#" + ipFrom + "#" + name;
      for (int index = 0; index < folderPath.Count; ++index)
        s = s + "#" + (object) folderPath[index];
      this.sendNetworkMessage(s);
      return true;
    }

    public void disconnecting(string ipFrom, bool externalDisconnectToo = true)
    {
      if (!this.silent)
        this.log(ipFrom + " Disconnected");
      if (this.os.multiplayer && !this.silent)
        this.sendNetworkMessage("cDisconnect " + this.ip + " " + ipFrom);
      if (!externalDisconnectToo)
        return;
      this.tryExternalCounterpartDisconnect();
    }

    public void giveAdmin(string ipFrom)
    {
      this.adminIP = ipFrom;
      this.log(ipFrom + " Became Admin");
      if (this.os.multiplayer && !this.silent)
        this.sendNetworkMessage("cAdmin " + this.ip + " " + ipFrom);
      UserDetail user = this.users[0];
      user.known = true;
      this.users[0] = user;
    }

    public void openPort(int portNum, string ipFrom)
    {
      portNum = this.GetCodePortNumberFromDisplayPort(portNum);
      int index1 = -1;
      for (int index2 = 0; index2 < this.ports.Count; ++index2)
      {
        if (this.ports[index2] == portNum)
        {
          index1 = index2;
          break;
        }
      }
      if (index1 != -1)
        this.portsOpen[index1] = (byte) 1;
      this.log(ipFrom + " Opened Port#" + (object) portNum);
      if (this.silent)
        return;
      this.sendNetworkMessage("cPortOpen " + this.ip + " " + ipFrom + " " + (object) portNum);
    }

    public void closePort(int portNum, string ipFrom)
    {
      portNum = this.GetCodePortNumberFromDisplayPort(portNum);
      int index1 = -1;
      for (int index2 = 0; index2 < this.ports.Count; ++index2)
      {
        if (this.ports[index2] == portNum)
          index1 = index2;
      }
      bool flag = false;
      if (index1 != -1)
      {
        flag = (int) this.portsOpen[index1] != 0;
        this.portsOpen[index1] = (byte) 0;
      }
      if (flag)
        this.log(ipFrom + " Closed Port#" + (object) portNum);
      if (this.silent)
        return;
      this.sendNetworkMessage("cPortClose " + this.ip + " " + ipFrom + " " + (object) portNum);
    }

    public bool isPortOpen(int portNum)
    {
      for (int index = 0; index < this.ports.Count; ++index)
      {
        if (this.ports[index] == portNum)
          return (int) this.portsOpen[index] > 0;
      }
      return false;
    }

    public void openCDTray(string ipFrom)
    {
      if (this.os.thisComputer.ip.Equals(this.ip))
        Programs.cdDrive(true);
      this.sendNetworkMessage("cCDDrive " + this.ip + " open");
    }

    public void closeCDTray(string ipFrom)
    {
      if (this.os.thisComputer.ip.Equals(this.ip))
        Programs.cdDrive(false);
      this.sendNetworkMessage("cCDDrive " + this.ip + " close");
    }

    public void forkBombClients(string ipFrom)
    {
      this.sendNetworkMessage("cFBClients " + this.ip + " " + ipFrom);
      if (this.os.multiplayer)
        return;
      for (int index = 0; index < this.os.ActiveHackers.Count; ++index)
      {
        if (this.os.ActiveHackers[index].Value == this.ip)
          Programs.getComputer(this.os, this.os.ActiveHackers[index].Key).crash(this.ip);
      }
    }

    public virtual int login(string username, string password, byte type = 1)
    {
      if (username.ToLower().Equals("admin") && password.Equals(this.adminPass))
      {
        this.giveAdmin(this.os.thisComputer.ip);
        return 1;
      }
      for (int index = 0; index < this.users.Count; ++index)
      {
        if (this.users[index].name.Equals(username) && this.users[index].pass.Equals(password) && ((int) this.users[index].type == (int) type || (int) type == 1))
        {
          this.currentUser = this.users[index];
          return 2;
        }
      }
      return 0;
    }

    public int GetDisplayPortNumberFromCodePort(int codePort)
    {
      if (this.PortRemapping == null || !this.PortRemapping.ContainsKey(codePort))
        return codePort;
      return this.PortRemapping[codePort];
    }

    public int GetCodePortNumberFromDisplayPort(int displayPort)
    {
      if (this.PortRemapping == null)
        return displayPort;
      foreach (KeyValuePair<int, int> keyValuePair in this.PortRemapping)
      {
        if (keyValuePair.Value == displayPort)
          return keyValuePair.Key;
      }
      return displayPort;
    }

    public void setAdminPassword(string newPass)
    {
      this.adminPass = newPass;
      for (int index = 0; index < this.users.Count; ++index)
      {
        if (this.users[index].name.ToLower().Equals("admin"))
          this.users[index] = new UserDetail("admin", newPass, (byte) 0);
      }
    }

    public string getSaveString()
    {
      string str1 = "none";
      if (this.os.netMap.mailServer.Equals((object) this))
        str1 = "mail";
      if (this.os.thisComputer.Equals((object) this))
        str1 = "player";
      string str2 = this.attatchedDeviceIDs == null ? "" : " devices=\"" + this.attatchedDeviceIDs + "\"";
      string str3 = this.HasTracker ? " tracker=\"true\" " : "";
      string str4 = "<computer name=\"" + this.name + "\" ip=\"" + this.ip + "\" type=\"" + (object) this.type + "\" spec=\"" + str1 + "\" id=\"" + this.idName + "\" " + (this.icon == null ? (object) "" : (object) ("icon=\"" + this.icon + "\"")) + str2 + str3 + " >\n" + "<location x=\"" + this.location.X.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "\" y=\"" + this.location.Y.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "\" />" + "<security level=\"" + (object) this.securityLevel + "\" traceTime=\"" + this.traceTime.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ((double) this.startingOverloadTicks > 0.0 ? (object) ("\" proxyTime=\"" + (this.hasProxy ? this.startingOverloadTicks.ToString((IFormatProvider) CultureInfo.InvariantCulture) : "-1")) : (object) "") + "\" portsToCrack=\"" + (object) this.portsNeededForCrack + "\" adminIP=\"" + this.adminIP + "\" />" + "<admin type=\"" + (this.admin == null ? "none" : (this.admin is FastBasicAdministrator ? "fast" : (this.admin is FastProgressOnlyAdministrator ? "progress" : "basic"))) + "\" resetPass=\"" + (this.admin == null || !this.admin.ResetsPassword ? "false" : "true") + "\"" + " isSuper=\"" + (this.admin == null || !this.admin.IsSuper ? "false" : "true") + "\" />";
      string str5 = "";
      for (int index = 0; index < this.links.Count; ++index)
        str5 = str5 + " " + (object) this.links[index];
      string str6 = str4 + "<links>" + str5 + "</links>\n";
      if (this.firewall != null)
        str6 = str6 + this.firewall.getSaveString() + "\n";
      string str7 = "";
      for (int index = 0; index < this.portsOpen.Count; ++index)
        str7 = str7 + " " + (object) this.ports[index];
      string str8 = str6 + "<portsOpen>" + str7 + "</portsOpen>\n";
      if (this.PortRemapping != null)
        str8 += PortRemappingSerializer.GetSaveString(this.PortRemapping);
      string str9 = str8 + "<users>\n";
      for (int index = 0; index < this.users.Count; ++index)
        str9 = str9 + this.users[index].getSaveString() + "\n";
      string str10 = str9 + "</users>\n";
      if (this.Memory != null)
        str10 += this.Memory.GetSaveString();
      string str11 = str10 + "<daemons>\n";
      for (int index = 0; index < this.daemons.Count; ++index)
        str11 = str11 + this.daemons[index].getSaveString() + "\n";
      return str11 + "</daemons>\n" + this.files.getSaveString() + "</computer>\n";
    }

    public static Computer load(XmlReader reader, OS os)
    {
      while (reader.Name != "computer")
        reader.Read();
      reader.MoveToAttribute("name");
      string str1 = reader.ReadContentAsString();
      reader.MoveToAttribute("ip");
      string compIP = reader.ReadContentAsString();
      reader.MoveToAttribute("type");
      byte compType = (byte) reader.ReadContentAsInt();
      reader.MoveToAttribute("spec");
      string str2 = reader.ReadContentAsString();
      reader.MoveToAttribute("id");
      string str3 = reader.ReadContentAsString();
      string str4 = (string) null;
      if (reader.MoveToAttribute("devices"))
        str4 = reader.ReadContentAsString();
      string str5 = (string) null;
      if (reader.MoveToAttribute("icon"))
        str5 = reader.ReadContentAsString();
      bool flag1 = false;
      if (reader.MoveToAttribute("tracker"))
        flag1 = reader.ReadContentAsString().ToLower() == "true";
      while (reader.Name != "location")
        reader.Read();
      reader.MoveToAttribute("x");
      float x = reader.ReadContentAsFloat();
      reader.MoveToAttribute("y");
      float y = reader.ReadContentAsFloat();
      while (reader.Name != "security")
        reader.Read();
      reader.MoveToAttribute("level");
      int seclevel = reader.ReadContentAsInt();
      reader.MoveToAttribute("traceTime");
      float num1 = reader.ReadContentAsFloat();
      reader.MoveToAttribute("portsToCrack");
      int num2 = reader.ReadContentAsInt();
      reader.MoveToAttribute("adminIP");
      string str6 = reader.ReadContentAsString();
      float time = -1f;
      if (reader.MoveToAttribute("proxyTime"))
        time = reader.ReadContentAsFloat();
      while (reader.Name != "admin")
        reader.Read();
      reader.MoveToAttribute("type");
      string str7 = reader.ReadContentAsString();
      reader.MoveToAttribute("resetPass");
      bool flag2 = reader.ReadContentAsBoolean();
      reader.MoveToAttribute("isSuper");
      bool flag3 = reader.ReadContentAsBoolean();
      Administrator administrator = (Administrator) null;
      switch (str7)
      {
        case "fast":
          administrator = (Administrator) new FastBasicAdministrator();
          break;
        case "basic":
          administrator = (Administrator) new BasicAdministrator();
          break;
        case "progress":
          administrator = (Administrator) new FastProgressOnlyAdministrator();
          break;
      }
      if (administrator != null)
        administrator.ResetsPassword = flag2;
      if (administrator != null)
        administrator.IsSuper = flag3;
      while (reader.Name != "links")
        reader.Read();
      string[] strArray = reader.ReadElementContentAsString().Split();
      Firewall firewall = (Firewall) null;
      while (reader.Name != "portsOpen" && reader.Name != "firewall")
        reader.Read();
      if (reader.Name == "firewall")
        firewall = Firewall.load(reader);
      while (reader.Name != "portsOpen")
        reader.Read();
      string portsList = reader.ReadElementContentAsString();
      Computer computer = new Computer(str1, compIP, new Vector2(x, y), seclevel, compType, os);
      computer.firewall = firewall;
      computer.admin = administrator;
      computer.HasTracker = flag1;
      if ((double) time > 0.0)
      {
        computer.addProxy(time);
      }
      else
      {
        computer.hasProxy = false;
        computer.proxyActive = false;
      }
      while (reader.Name != "users" && reader.Name != "portRemap")
        reader.Read();
      if (reader.Name == "portRemap")
      {
        int content = (int) reader.MoveToContent();
        string input = reader.ReadElementContentAsString();
        computer.PortRemapping = PortRemappingSerializer.Deserialize(input);
      }
      while (reader.Name != "users")
        reader.Read();
      computer.users.Clear();
      bool flag4;
      while (true)
      {
        flag4 = true;
        if (!(reader.Name == "users") || reader.NodeType != XmlNodeType.EndElement)
        {
          if (reader.Name == "user")
          {
            UserDetail userDetail = UserDetail.loadUserDetail(reader);
            if (userDetail.name.ToLower() == "admin")
              computer.adminPass = userDetail.pass;
            computer.users.Add(userDetail);
          }
          reader.Read();
        }
        else
          break;
      }
      while (reader.Name != "Memory" && reader.Name != "daemons")
        reader.Read();
      if (reader.Name == "Memory")
      {
        MemoryContents memoryContents = MemoryContents.Deserialize(reader);
        computer.Memory = memoryContents;
        reader.Read();
      }
      while (reader.Name != "daemons")
        reader.Read();
      reader.Read();
      while (true)
      {
        flag4 = true;
        if (!(reader.Name == "daemons") || reader.NodeType != XmlNodeType.EndElement)
        {
          if (reader.Name == "MailServer")
          {
            reader.MoveToAttribute("name");
            string name = reader.ReadContentAsString();
            MailServer mailServer = new MailServer(computer, name, os);
            computer.daemons.Add((Daemon) mailServer);
            if (reader.MoveToAttribute("color"))
            {
              Color color = Utils.convertStringToColor(reader.ReadContentAsString());
              mailServer.setThemeColor(color);
            }
          }
          else if (reader.Name == "MissionListingServer")
          {
            reader.MoveToAttribute("name");
            string serviceName = reader.ReadContentAsString();
            reader.MoveToAttribute("group");
            string group = reader.ReadContentAsString();
            reader.MoveToAttribute("public");
            bool _isPublic = reader.ReadContentAsString().ToLower().Equals("true");
            reader.MoveToAttribute("assign");
            bool _isAssigner = reader.ReadContentAsString().ToLower().Equals("true");
            string str8 = (string) null;
            if (reader.MoveToAttribute("title"))
              str8 = reader.ReadContentAsString();
            string iconPath = (string) null;
            string input = (string) null;
            string articleFolderPath = (string) null;
            if (reader.MoveToAttribute("icon"))
              iconPath = reader.ReadContentAsString();
            if (reader.MoveToAttribute("articles"))
              articleFolderPath = reader.ReadContentAsString();
            if (reader.MoveToAttribute("color"))
              input = reader.ReadContentAsString();
            MissionListingServer missionListingServer = iconPath == null || input == null ? new MissionListingServer(computer, serviceName, group, os, _isPublic, _isAssigner) : new MissionListingServer(computer, serviceName, iconPath, articleFolderPath, Utils.convertStringToColor(input), os, _isPublic, _isAssigner);
            if (str8 != null)
              missionListingServer.listingTitle = str8;
            computer.daemons.Add((Daemon) missionListingServer);
          }
          else if (reader.Name == "AddEmailServer")
          {
            reader.MoveToAttribute("name");
            string serviceName = reader.ReadContentAsString();
            AddEmailDaemon addEmailDaemon = new AddEmailDaemon(computer, serviceName, os);
            computer.daemons.Add((Daemon) addEmailDaemon);
          }
          else if (reader.Name == "MessageBoard")
          {
            reader.MoveToAttribute("name");
            string str8 = reader.ReadContentAsString();
            MessageBoardDaemon messageBoardDaemon = new MessageBoardDaemon(computer, os);
            if (reader.MoveToAttribute("boardName"))
              messageBoardDaemon.BoardName = reader.ReadContentAsString();
            messageBoardDaemon.name = str8;
            computer.daemons.Add((Daemon) messageBoardDaemon);
          }
          else if (reader.Name == "WebServer")
          {
            reader.MoveToAttribute("name");
            string serviceName = reader.ReadContentAsString();
            reader.MoveToAttribute("url");
            string pageFileLocation = reader.ReadContentAsString();
            WebServerDaemon webServerDaemon = new WebServerDaemon(computer, serviceName, os, pageFileLocation);
            computer.daemons.Add((Daemon) webServerDaemon);
          }
          else if (reader.Name == "OnlineWebServer")
          {
            reader.MoveToAttribute("name");
            string serviceName = reader.ReadContentAsString();
            reader.MoveToAttribute("url");
            string url = reader.ReadContentAsString();
            OnlineWebServerDaemon onlineWebServerDaemon = new OnlineWebServerDaemon(computer, serviceName, os);
            onlineWebServerDaemon.setURL(url);
            computer.daemons.Add((Daemon) onlineWebServerDaemon);
          }
          else if (reader.Name == "AcademicDatabse")
          {
            reader.MoveToAttribute("name");
            string serviceName = reader.ReadContentAsString();
            AcademicDatabaseDaemon academicDatabaseDaemon = new AcademicDatabaseDaemon(computer, serviceName, os);
            computer.daemons.Add((Daemon) academicDatabaseDaemon);
          }
          else if (reader.Name == "MissionHubServer")
          {
            MissionHubServer missionHubServer = new MissionHubServer(computer, "unknown", "unknown", os);
            computer.daemons.Add((Daemon) missionHubServer);
          }
          else if (reader.Name == "DeathRowDatabase")
          {
            DeathRowDatabaseDaemon rowDatabaseDaemon = new DeathRowDatabaseDaemon(computer, "Death Row Database", os);
            computer.daemons.Add((Daemon) rowDatabaseDaemon);
          }
          else if (reader.Name == "MedicalDatabase")
          {
            MedicalDatabaseDaemon medicalDatabaseDaemon = new MedicalDatabaseDaemon(computer, os);
            computer.daemons.Add((Daemon) medicalDatabaseDaemon);
          }
          else if (reader.Name == "HeartMonitor")
          {
            string str8 = "UNKNOWN";
            if (reader.MoveToAttribute("patient"))
              str8 = reader.ReadContentAsString();
            computer.daemons.Add((Daemon) new HeartMonitorDaemon(computer, os)
            {
              PatientID = str8
            });
          }
          else if (reader.Name.Equals("PointClicker"))
          {
            PointClickerDaemon pointClickerDaemon = new PointClickerDaemon(computer, "Point Clicker!", os);
            computer.daemons.Add((Daemon) pointClickerDaemon);
          }
          else if (reader.Name.Equals("ispSystem"))
          {
            ISPDaemon ispDaemon = new ISPDaemon(computer, os);
            computer.daemons.Add((Daemon) ispDaemon);
          }
          else if (reader.Name.Equals("porthackheart"))
          {
            PorthackHeartDaemon porthackHeartDaemon = new PorthackHeartDaemon(computer, os);
            computer.daemons.Add((Daemon) porthackHeartDaemon);
          }
          else if (reader.Name.Equals("SongChangerDaemon"))
          {
            SongChangerDaemon songChangerDaemon = new SongChangerDaemon(computer, os);
            computer.daemons.Add((Daemon) songChangerDaemon);
          }
          else if (reader.Name == "UploadServerDaemon")
          {
            string str8;
            string input = str8 = "";
            string foldername = str8;
            string serviceName = str8;
            if (reader.MoveToAttribute("name"))
              serviceName = reader.ReadContentAsString();
            if (reader.MoveToAttribute("foldername"))
              foldername = reader.ReadContentAsString();
            if (reader.MoveToAttribute("color"))
              input = reader.ReadContentAsString();
            bool needsAuthentication = false;
            bool flag5 = false;
            if (reader.MoveToAttribute("needsAuh"))
              needsAuthentication = reader.ReadContentAsBoolean();
            if (reader.MoveToAttribute("hasReturnViewButton"))
              flag5 = reader.ReadContentAsString().ToLower() == "true";
            Color themeColor = Color.White;
            if (input != "")
              themeColor = Utils.convertStringToColor(input);
            computer.daemons.Add((Daemon) new UploadServerDaemon(computer, serviceName, themeColor, os, foldername, needsAuthentication)
            {
              hasReturnViewButton = flag5
            });
          }
          else if (reader.Name == "DHSDaemon")
          {
            DLCHubServer dlcHubServer = new DLCHubServer(computer, "unknown", "unknown", os);
            computer.daemons.Add((Daemon) dlcHubServer);
          }
          else if (reader.Name == "CustomConnectDisplayDaemon")
          {
            CustomConnectDisplayDaemon connectDisplayDaemon = new CustomConnectDisplayDaemon(computer, os);
            computer.daemons.Add((Daemon) connectDisplayDaemon);
          }
          else if (reader.Name == "DatabaseDaemon")
          {
            // ISSUE: variable of the null type
            __Null local;
            string Foldername = (string) (local = null);
            string permissions = (string) local;
            string DataTypeIdentifier = (string) local;
            string name = (string) local;
            Color? ThemeColor = new Color?();
            string str8 = (string) null;
            string str9 = (string) null;
            if (reader.MoveToAttribute("Name"))
              name = reader.ReadContentAsString();
            if (reader.MoveToAttribute("Permissions"))
              permissions = reader.ReadContentAsString();
            if (reader.MoveToAttribute("DataType"))
              DataTypeIdentifier = reader.ReadContentAsString();
            if (reader.MoveToAttribute("Foldername"))
              Foldername = reader.ReadContentAsString();
            if (reader.MoveToAttribute("Color"))
              ThemeColor = new Color?(Utils.convertStringToColor(reader.ReadContentAsString()));
            if (reader.MoveToAttribute("AdminEmailAccount"))
              str8 = reader.ReadContentAsString();
            if (reader.MoveToAttribute("AdminEmailHostID"))
              str9 = reader.ReadContentAsString();
            DatabaseDaemon databaseDaemon = new DatabaseDaemon(computer, os, name, permissions, DataTypeIdentifier, Foldername, ThemeColor);
            if (!string.IsNullOrWhiteSpace(str8))
            {
              databaseDaemon.adminResetEmailHostID = str9;
              databaseDaemon.adminResetPassEmailAccount = str8;
            }
            computer.daemons.Add((Daemon) databaseDaemon);
          }
          else if (reader.Name == "WhitelistAuthenticatorDaemon")
          {
            bool flag5 = true;
            if (reader.MoveToAttribute("SelfAuthenticating"))
              flag5 = reader.ReadContentAsString().ToLower() == "true";
            WhitelistConnectionDaemon connectionDaemon = new WhitelistConnectionDaemon(computer, os) { AuthenticatesItself = flag5 };
            computer.daemons.Add((Daemon) connectionDaemon);
          }
          else if (reader.Name == "IRCDaemon")
          {
            IRCDaemon ircDaemon = new IRCDaemon(computer, os, "LOAD ERROR");
            computer.daemons.Add((Daemon) ircDaemon);
          }
          else if (reader.Name == "MarkovTextDaemon")
          {
            string corpusLoadPath;
            string name = corpusLoadPath = (string) null;
            if (reader.MoveToAttribute("Name"))
              name = reader.ReadContentAsString();
            if (reader.MoveToAttribute("SourceFilesContentFolder"))
              corpusLoadPath = reader.ReadContentAsString();
            MarkovTextDaemon markovTextDaemon = new MarkovTextDaemon(computer, os, name, corpusLoadPath);
            computer.daemons.Add((Daemon) markovTextDaemon);
          }
          else if (reader.Name.Equals("AircraftDaemon"))
          {
            Vector2 zero = Vector2.Zero;
            Vector2 mapDest = Vector2.One * 0.5f;
            float progress = 0.5f;
            string name = "Pacific Charter Flight";
            if (reader.MoveToAttribute("Name"))
              name = reader.ReadContentAsString();
            if (reader.MoveToAttribute("OriginX"))
              zero.X = Utils.RobustReadAsFloat(reader);
            if (reader.MoveToAttribute("OriginY"))
              zero.Y = Utils.RobustReadAsFloat(reader);
            if (reader.MoveToAttribute("DestX"))
              mapDest.X = Utils.RobustReadAsFloat(reader);
            if (reader.MoveToAttribute("DestY"))
              mapDest.Y = Utils.RobustReadAsFloat(reader);
            if (reader.MoveToAttribute("Progress"))
              progress = Utils.RobustReadAsFloat(reader);
            AircraftDaemon aircraftDaemon = new AircraftDaemon(computer, os, name, zero, mapDest, progress);
            computer.daemons.Add((Daemon) aircraftDaemon);
          }
          else if (reader.Name.Equals("LogoCustomConnectDisplayDaemon"))
          {
            string logoImageName = (string) null;
            string titleImageName = (string) null;
            string buttonAlignment = (string) null;
            bool logoShouldClipoverdraw = false;
            if (reader.MoveToAttribute("logo"))
              logoImageName = reader.ReadContentAsString();
            if (reader.MoveToAttribute("title"))
              titleImageName = reader.ReadContentAsString();
            if (reader.MoveToAttribute("overdrawLogo"))
              logoShouldClipoverdraw = reader.ReadContentAsString().ToLower() == "true";
            if (reader.MoveToAttribute("buttonAlignment"))
              buttonAlignment = reader.ReadContentAsString();
            LogoCustomConnectDisplayDaemon connectDisplayDaemon = new LogoCustomConnectDisplayDaemon(computer, os, logoImageName, titleImageName, logoShouldClipoverdraw, buttonAlignment);
            computer.daemons.Add((Daemon) connectDisplayDaemon);
          }
          else if (reader.Name.Equals("LogoDaemon"))
          {
            string str8;
            string LogoImagePath = str8 = (string) null;
            bool showsTitle = true;
            Color color = Color.White;
            if (reader.MoveToAttribute("LogoImagePath"))
              LogoImagePath = reader.ReadContentAsString();
            if (reader.MoveToAttribute("TextColor"))
              color = Utils.convertStringToColor(reader.ReadContentAsString());
            if (reader.MoveToAttribute("Name"))
              str8 = reader.ReadContentAsString();
            if (reader.MoveToAttribute("ShowsTitle"))
              showsTitle = reader.ReadContentAsString().ToLower() == "true";
            computer.daemons.Add((Daemon) new LogoDaemon(computer, os, str1, showsTitle, LogoImagePath)
            {
              TextColor = color
            });
          }
          else if (reader.Name.Equals("DLCCredits"))
          {
            string overrideButtonText = (string) null;
            if (reader.MoveToAttribute("Button"))
              overrideButtonText = reader.ReadContentAsString();
            string overrideTitle = (string) null;
            if (reader.MoveToAttribute("Title"))
              overrideTitle = reader.ReadContentAsString();
            DLCCreditsDaemon dlcCreditsDaemon = overrideButtonText == null && overrideTitle == null ? new DLCCreditsDaemon(computer, os) : new DLCCreditsDaemon(computer, os, overrideTitle, overrideButtonText);
            if (reader.MoveToAttribute("Action"))
              dlcCreditsDaemon.ConditionalActionsToLoadOnButtonPress = reader.ReadContentAsString();
            computer.daemons.Add((Daemon) dlcCreditsDaemon);
          }
          else if (reader.Name.Equals("FastActionHost"))
          {
            FastActionHost fastActionHost = new FastActionHost(computer, os, str1);
            computer.daemons.Add((Daemon) fastActionHost);
          }
          reader.Read();
        }
        else
          break;
      }
      computer.files = FileSystem.load(reader);
      computer.traceTime = num1;
      computer.portsNeededForCrack = num2;
      computer.adminIP = str6;
      computer.idName = str3;
      computer.icon = str5;
      computer.attatchedDeviceIDs = str4;
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (strArray[index] != "")
          computer.links.Add(Convert.ToInt32(strArray[index]));
      }
      if (portsList.Length > 0)
        ComputerLoader.loadPortsIntoComputer(portsList, (object) computer);
      if (str2 == "mail")
        os.netMap.mailServer = computer;
      else if (str2 == "player")
        os.thisComputer = computer;
      return computer;
    }

    public string getTooltipString()
    {
      return this.name + "\n" + this.ip;
    }

    public Vector2 getScreenSpacePosition()
    {
      Vector2 nodeDrawPos = this.os.netMap.GetNodeDrawPos(this);
      return new Vector2((float) this.os.netMap.bounds.X + nodeDrawPos.X + (float) (NetworkMap.NODE_SIZE / 2), (float) this.os.netMap.bounds.Y + nodeDrawPos.Y + (float) (NetworkMap.NODE_SIZE / 2));
    }

    public Daemon getDaemon(Type t)
    {
      for (int index = 0; index < this.daemons.Count; ++index)
      {
        if (this.daemons[index].GetType().Equals(t))
          return this.daemons[index];
      }
      return (Daemon) null;
    }

    public static string generateBinaryString(int length)
    {
      byte[] buffer = new byte[length / 8];
      Utils.random.NextBytes(buffer);
      string str = "";
      for (int index = 0; index < buffer.Length; ++index)
        str += Convert.ToString(buffer[index], 2);
      return str;
    }

    public static string generateBinaryString(int length, MSRandom rng)
    {
      byte[] buffer = new byte[length / 8];
      rng.NextBytes(buffer);
      string str = "";
      for (int index = 0; index < buffer.Length; ++index)
        str += Convert.ToString(buffer[index], 2);
      return str;
    }

    public static Folder getFolderAtDepth(Computer c, int depth, List<int> path)
    {
      Folder folder = c.files.root;
      if (path.Count > 0)
      {
        for (int index = 0; index < depth; ++index)
        {
          if (folder.folders.Count > path[index])
            folder = folder.folders[path[index]];
        }
      }
      return folder;
    }

    public override string ToString()
    {
      return "Comp : " + this.idName;
    }

    public static Computer loadFromFile(string filename)
    {
      return (Computer) ComputerLoader.loadComputer(filename, false, false);
    }

    public Folder getFolderFromPath(string path, bool createFoldersThatDontExist = false)
    {
      if (string.IsNullOrWhiteSpace(path))
        return this.files.root;
      List<int> folderPath = this.getFolderPath(path, createFoldersThatDontExist);
      return Computer.getFolderAtDepth(this, folderPath.Count, folderPath);
    }

    public List<int> getFolderPath(string path, bool createFoldersThatDontExist = false)
    {
      List<int> intList = new List<int>();
      char[] chArray = new char[2]{ '/', '\\' };
      string[] strArray = path.Split(chArray);
      Folder folder = this.files.root;
      for (int index1 = 0; index1 < strArray.Length; ++index1)
      {
        bool flag = false;
        for (int index2 = 0; index2 < folder.folders.Count; ++index2)
        {
          if (folder.folders[index2].name.Equals(strArray[index1]))
          {
            intList.Add(index2);
            folder = folder.folders[index2];
            flag = true;
            break;
          }
        }
        if (!flag && createFoldersThatDontExist)
        {
          folder.folders.Add(new Folder(strArray[index1]));
          int index2 = folder.folders.Count - 1;
          folder = folder.folders[index2];
          intList.Add(index2);
        }
      }
      return intList;
    }

    public bool PlayerHasAdminPermissions()
    {
      return this.adminIP == this.os.thisComputer.ip || (int) this.currentUser.type == 0 && this.currentUser.name != null;
    }
  }
}
