// Decompiled with JetBrains decompiler
// Type: Hacknet.Programs
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Extensions;
using Hacknet.Gui;
using Microsoft.Xna.Framework.Input;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Hacknet
{
  internal static class Programs
  {
    private static readonly object ConnectionLockObject = new object();

    public static void doDots(int num, int msDelay, OS os)
    {
      for (int index = 0; index < num; ++index)
      {
        os.writeSingle(".");
        Thread.Sleep(Utils.DebugGoFast() ? 1 : msDelay);
      }
    }

    public static void typeOut(string s, OS os, int delay = 50)
    {
      os.write(string.Concat((object) s[0]));
      Thread.Sleep(delay);
      for (int index = 1; index < s.Length; ++index)
      {
        os.writeSingle(string.Concat((object) s[index]));
        Thread.Sleep(Utils.DebugGoFast() ? 1 : delay);
      }
    }

    public static void firstTimeInit(string[] args, OS os, bool callWasRecursed = false)
    {
      bool flag1 = Settings.initShowsTutorial;
      bool flag2 = false;
      if (Settings.IsInExtensionMode && !ExtensionLoader.ActiveExtensionInfo.StartsWithTutorial)
      {
        flag1 = false;
        if (!OS.WillLoadSave)
          flag2 = true;
      }
      if (flag1)
      {
        os.display.visible = false;
        os.ram.visible = false;
        os.netMap.visible = false;
        os.terminal.visible = true;
        os.mailicon.isEnabled = false;
        if (os.hubServerAlertsIcon != null)
          os.hubServerAlertsIcon.IsEnabled = false;
      }
      int msDelay = Settings.isConventionDemo ? 80 : 200;
      int millisecondsTimeout = Settings.isConventionDemo ? 150 : 300;
      if (Settings.debugCommandsEnabled && GuiData.getKeyboadState().IsKeyDown(Keys.LeftAlt))
        msDelay = millisecondsTimeout = 1;
      Programs.typeOut("Initializing .", os, 50);
      Programs.doDots(7, msDelay + 100, os);
      Programs.typeOut("Loading modules.", os, 50);
      Programs.doDots(5, msDelay, os);
      os.writeSingle("Complete");
      if (!Utils.DebugGoFast())
        Thread.Sleep(millisecondsTimeout);
      Programs.typeOut("Loading nodes.", os, 50);
      Programs.doDots(5, msDelay, os);
      os.writeSingle("Complete");
      if (!Utils.DebugGoFast())
        Thread.Sleep(millisecondsTimeout);
      Programs.typeOut("Reticulating splines.", os, 50);
      Programs.doDots(5, msDelay - 50, os);
      os.writeSingle("Complete");
      if (!Utils.DebugGoFast())
        Thread.Sleep(millisecondsTimeout);
      if (os.crashModule.BootLoadErrors.Length > 0)
      {
        Programs.typeOut("\n------ " + LocaleTerms.Loc("BOOT ERRORS DETECTED") + " ------", os, 50);
        Thread.Sleep(200);
        foreach (string s in os.crashModule.BootLoadErrors.Split(Utils.newlineDelim, StringSplitOptions.RemoveEmptyEntries))
        {
          Programs.typeOut(s, os, 50);
          Thread.Sleep(100);
        }
        Programs.typeOut("---------------------------------\n", os, 50);
        Thread.Sleep(200);
      }
      Programs.typeOut("\n--Initialization Complete--\n", os, 50);
      GuiData.getFilteredKeys();
      os.inputEnabled = true;
      if (!Utils.DebugGoFast())
        Thread.Sleep(millisecondsTimeout + 100);
      if (!flag1)
      {
        Programs.typeOut(LocaleTerms.Loc("For A Command List, type \"help\""), os, 50);
        if (!Utils.DebugGoFast())
          Thread.Sleep(millisecondsTimeout + 100);
      }
      os.write("");
      if (!Utils.DebugGoFast())
        Thread.Sleep(millisecondsTimeout);
      os.write("");
      if (!Utils.DebugGoFast())
        Thread.Sleep(millisecondsTimeout);
      os.write("");
      if (!Utils.DebugGoFast())
        Thread.Sleep(millisecondsTimeout);
      os.write("\n");
      if (flag1)
      {
        os.write(LocaleTerms.Loc("Launching Tutorial..."));
        os.launchExecutable("Tutorial.exe", PortExploits.crackExeData[1], -1, (string[]) null, (string) null);
        Settings.initShowsTutorial = false;
        AdvancedTutorial advancedTutorial = (AdvancedTutorial) null;
        for (int index = 0; index < os.exes.Count; ++index)
        {
          advancedTutorial = os.exes[index] as AdvancedTutorial;
          if (advancedTutorial != null)
            break;
        }
        if (advancedTutorial != null)
          advancedTutorial.CanActivateFirstStep = false;
        int num1 = 100;
        for (int index = 0; index < num1; ++index)
        {
          double num2 = (double) index / (double) num1;
          if (Utils.random.NextDouble() < num2)
          {
            os.ram.visible = true;
            os.netMap.visible = false;
            os.terminal.visible = false;
          }
          else
          {
            os.ram.visible = false;
            os.netMap.visible = false;
            os.terminal.visible = true;
          }
          Thread.Sleep(16);
        }
        os.ram.visible = true;
        os.netMap.visible = false;
        os.terminal.visible = false;
        if (advancedTutorial != null)
          advancedTutorial.CanActivateFirstStep = true;
      }
      else
      {
        os.runCommand("connect " + os.thisComputer.ip);
        if (flag2 && !os.Flags.HasFlag("ExtensionFirstBootComplete"))
        {
          ExtensionLoader.SendStartingEmailForActiveExtensionNextFrame((object) os);
          int num1 = (int) (60.0 * 2.20000004768372);
          for (int index = 0; index < num1; ++index)
          {
            double num2 = (double) index / (double) num1;
            os.ram.visible = Utils.random.NextDouble() < num2;
            os.netMap.visible = Utils.random.NextDouble() < num2;
            os.display.visible = Utils.random.NextDouble() < num2;
            Thread.Sleep(16);
          }
          os.terminal.visible = true;
          os.display.visible = true;
          os.netMap.visible = true;
          os.ram.visible = true;
          os.terminal.visible = true;
          os.display.inputLocked = false;
          os.netMap.inputLocked = false;
          os.ram.inputLocked = false;
          os.Flags.AddFlag("ExtensionFirstBootComplete");
        }
      }
      Thread.Sleep(500);
      if (callWasRecursed)
      {
        os.ram.visible = true;
        os.ram.inputLocked = false;
        os.display.visible = true;
        os.display.inputLocked = false;
        os.netMap.visible = true;
        os.netMap.inputLocked = false;
      }
      else if (flag1)
      {
        os.ram.visible = true;
        os.ram.inputLocked = false;
        os.display.visible = true;
        os.display.inputLocked = false;
        os.netMap.visible = true;
        os.netMap.inputLocked = false;
      }
      else if (!os.ram.visible && !Settings.IsInExtensionMode)
        Programs.firstTimeInit(args, os, true);
    }

    public static void connect(string[] args, OS os)
    {
      lock (Programs.ConnectionLockObject)
      {
        os.navigationPath.Clear();
        if (os.connectedComp != null)
          os.connectedComp.disconnecting(os.thisComputer.ip, false);
        os.terminal.prompt = "> ";
        os.write("Disconnected \n");
        bool lockTaken1 = false;
        ConnectedNodeEffect connectedNodeEffect;
        try
        {
          Monitor.Enter((object) (connectedNodeEffect = os.netMap.nodeEffect), ref lockTaken1);
          os.netMap.nodeEffect.reset();
        }
        finally
        {
          if (lockTaken1)
            Monitor.Exit((object) connectedNodeEffect);
        }
        bool lockTaken2 = false;
        try
        {
          Monitor.Enter((object) (connectedNodeEffect = os.netMap.adminNodeEffect), ref lockTaken2);
          os.netMap.adminNodeEffect.reset();
        }
        finally
        {
          if (lockTaken2)
            Monitor.Exit((object) connectedNodeEffect);
        }
        os.display.command = args[0];
        os.display.commandArgs = args;
        os.display.typeChanged();
        bool flag = false;
        if (args.Length < 2)
        {
          os.write("Usage: connect [WHERE TO CONNECT TO]");
        }
        else
        {
          os.write("Scanning For " + args[1]);
          for (int index1 = 0; index1 < os.netMap.nodes.Count; ++index1)
          {
            if (os.netMap.nodes[index1].ip.Equals(args[1]) || os.netMap.nodes[index1].name.Equals(args[1]))
            {
              if (os.netMap.nodes[index1].connect(os.thisComputer.ip))
              {
                os.connectedComp = os.netMap.nodes[index1];
                os.connectedIP = os.netMap.nodes[index1].ip;
                os.write("Connection Established ::");
                os.write("Connected To " + os.netMap.nodes[index1].name + "@" + os.netMap.nodes[index1].ip);
                os.terminal.prompt = os.netMap.nodes[index1].ip + "@> ";
                if (!os.netMap.visibleNodes.Contains(index1))
                  os.netMap.visibleNodes.Add(index1);
                FileEntry fileEntry = os.netMap.nodes[index1].files.root.searchForFolder("sys").searchForFile(ComputerTypeInfo.getDefaultBootDaemonFilename((object) os.netMap.nodes[index1]));
                if (fileEntry == null)
                  return;
                string data = fileEntry.data;
                for (int index2 = 0; index2 < os.netMap.nodes[index1].daemons.Count; ++index2)
                {
                  if (os.netMap.nodes[index1].daemons[index2].name == data)
                  {
                    os.netMap.nodes[index1].daemons[index2].navigatedTo();
                    os.display.command = os.netMap.nodes[index1].daemons[index2].name;
                  }
                }
                return;
              }
              os.write("External Computer Refused Connection");
              flag = true;
            }
          }
          if (!flag)
          {
            os.write("Failed to Connect:\nCould Not Find Computer at " + args[1]);
            os.display.command = "dc";
          }
          os.connectedComp = (Computer) null;
          os.connectedIP = "";
        }
      }
    }

    public static void disconnect(string[] args, OS os)
    {
      os.navigationPath.Clear();
      if (os.connectedComp != null)
        os.connectedComp.disconnecting(os.thisComputer.ip, true);
      os.connectedComp = (Computer) null;
      os.connectedIP = "";
      os.terminal.prompt = "> ";
      os.write("Disconnected \n");
      bool lockTaken1 = false;
      ConnectedNodeEffect connectedNodeEffect;
      try
      {
        Monitor.Enter((object) (connectedNodeEffect = os.netMap.nodeEffect), ref lockTaken1);
        os.netMap.nodeEffect.reset();
      }
      finally
      {
        if (lockTaken1)
          Monitor.Exit((object) connectedNodeEffect);
      }
      bool lockTaken2 = false;
      try
      {
        Monitor.Enter((object) (connectedNodeEffect = os.netMap.adminNodeEffect), ref lockTaken2);
        os.netMap.adminNodeEffect.reset();
      }
      finally
      {
        if (lockTaken2)
          Monitor.Exit((object) connectedNodeEffect);
      }
    }

    public static void getString(string[] args, OS os)
    {
      string prompt = os.terminal.prompt;
      os.getStringCache = "terminalString#$#$#$$#$&$#$#$#$#";
      os.terminal.preventingExecution = true;
      os.terminal.prompt = args[1] + " :";
      os.terminal.usingTabExecution = true;
      int num = os.terminal.commandsRun();
      while (os.terminal.commandsRun() == num)
        Thread.Sleep(16);
      string lastRunCommand = os.terminal.getLastRunCommand();
      os.getStringCache = "loginData#$#$#$$#$&$#$#$#$#" + lastRunCommand;
      os.terminal.prompt = prompt;
      os.terminal.preventingExecution = false;
      os.terminal.usingTabExecution = false;
      os.getStringCache += "#$#$#$$#$&$#$#$#$#done";
    }

    public static bool parseStringFromGetStringCommand(OS os, out string data)
    {
      string[] separator = new string[1]{ "#$#$#$$#$&$#$#$#$#" };
      string[] strArray = os.getStringCache.Split(separator, StringSplitOptions.None);
      string str = (string) null;
      if (strArray.Length > 1)
      {
        str = strArray[1];
        if (str.Equals(""))
          str = os.terminal.currentLine;
      }
      data = str;
      return strArray.Length > 2;
    }

    public static void login(string[] args, OS os)
    {
      string prompt = os.terminal.prompt;
      os.display.command = "login";
      os.displayCache = "loginData#$#$#$$#$&$#$#$#$#";
      os.terminal.preventingExecution = true;
      os.terminal.prompt = "Username :";
      os.terminal.usingTabExecution = true;
      int num1 = os.terminal.commandsRun();
      while (os.terminal.commandsRun() == num1)
        Thread.Sleep(4);
      string lastRunCommand1 = os.terminal.getLastRunCommand();
      os.displayCache = "loginData#$#$#$$#$&$#$#$#$#" + lastRunCommand1 + "#$#$#$$#$&$#$#$#$#";
      os.terminal.prompt = "Password :";
      TextBox.MaskingText = true;
      int num2 = os.terminal.commandsRun();
      while (os.terminal.commandsRun() == num2)
        Thread.Sleep(16);
      string lastRunCommand2 = os.terminal.getLastRunCommand();
      os.displayCache = os.displayCache + lastRunCommand2;
      int num3 = (os.connectedComp == null ? os.thisComputer : os.connectedComp).login(lastRunCommand1, lastRunCommand2, (byte) 1);
      os.terminal.prompt = prompt;
      os.terminal.preventingExecution = false;
      os.terminal.usingTabExecution = false;
      TextBox.MaskingText = false;
      switch (num3)
      {
        case 1:
          os.write("Admin Login Successful");
          break;
        case 2:
          os.write("User " + lastRunCommand1 + " Login Successful");
          os.connectedComp.userLoggedIn = true;
          break;
        default:
          os.write("Login Failed");
          break;
      }
      os.displayCache = os.displayCache + "#$#$#$$#$&$#$#$#$#" + (object) num3 + "#$#$#$$#$&$#$#$#$#";
    }

    public static void ls(string[] args, OS os)
    {
      if (os.hasConnectionPermission(false))
      {
        Folder folder = Programs.getCurrentFolder(os);
        if (args.Length >= 2)
          folder = Programs.getFolderAtPath(args[1], os, (Folder) null, false);
        int num = 0;
        for (int index = 0; index < folder.folders.Count; ++index)
        {
          os.write(":" + folder.folders[index].name);
          ++num;
        }
        for (int index = 0; index < folder.files.Count; ++index)
        {
          os.write(folder.files[index].name ?? "");
          ++num;
        }
        if (num != 0)
          return;
        os.write("--Folder Empty --");
      }
      else
        os.write("Insufficient Privileges to Perform Operation");
    }

    public static void cat(string[] args, OS os)
    {
      if (os.hasConnectionPermission(true))
      {
        os.displayCache = "";
        Folder currentFolder = Programs.getCurrentFolder(os);
        if (args.Length < 2)
        {
          os.write("Usage: cat [FILENAME]");
        }
        else
        {
          for (int index = 0; index < currentFolder.files.Count; ++index)
          {
            if (currentFolder.files[index].name.Equals(args[1]))
            {
              bool flag = false;
              if (os.connectedComp != null)
              {
                if (os.connectedComp.canReadFile(os.thisComputer.ip, currentFolder.files[index], index))
                  flag = true;
              }
              else
                flag = os.thisComputer.canReadFile(os.thisComputer.ip, currentFolder.files[index], index);
              if (!flag)
                return;
              string str = LocalizedFileLoader.SafeFilterString(currentFolder.files[index].data);
              os.write(currentFolder.files[index].name + " : " + (object) ((double) currentFolder.files[index].size / 1000.0) + "kb\n" + str + "\n");
              os.displayCache = currentFolder.files[index].data;
              os.display.LastDisplayedFileFolder = currentFolder;
              os.display.LastDisplayedFileSourceIP = os.connectedComp != null ? os.connectedComp.ip : os.thisComputer.ip;
              return;
            }
          }
          os.displayCache = "File Not Found";
          os.validCommand = false;
          os.write("File Not Found\n");
        }
      }
      else
      {
        os.displayCache = "Insufficient Privileges";
        os.validCommand = false;
        os.write("Insufficient Privileges to Perform Operation");
      }
    }

    public static void ps(string[] args, OS os)
    {
      if (os.exes.Count > 0)
      {
        os.write("UID   :  PID  :  NAME");
        Thread.Sleep(100);
        for (int index = 0; index < os.exes.Count; ++index)
        {
          os.write("root  :  " + (object) os.exes[index].PID + "     : " + os.exes[index].IdentifierName);
          Thread.Sleep(100);
        }
        os.write("");
      }
      else
        os.write("No Running Processes");
    }

    public static void kill(string[] args, OS os)
    {
      try
      {
        int int32 = Convert.ToInt32(args[1].Replace("[", "").Replace("]", "").Replace("\"", ""));
        int index1 = -1;
        for (int index2 = 0; index2 < os.exes.Count; ++index2)
        {
          if (os.exes[index2].PID == int32)
            index1 = index2;
        }
        if (index1 < 0 || index1 >= os.exes.Count)
        {
          os.write(LocaleTerms.Loc("Invalid PID"));
        }
        else
        {
          os.write("Process " + (object) int32 + "[" + os.exes[index1].IdentifierName + "] Ended");
          os.exes[index1].Killed();
          os.exes.RemoveAt(index1);
        }
      }
      catch
      {
        os.write(LocaleTerms.Loc("Error: Invalid PID or Input Format"));
      }
    }

    public static void scp(string[] args, OS os)
    {
      if (os.connectedComp == null)
        os.write("Must be Connected to a Non-Local Host\n");
      else if (args.Length < 2)
      {
        os.write("Not Enough Arguments");
      }
      else
      {
        string path = "bin";
        string str1 = path;
        bool flag1 = false;
        if (args.Length > 2)
        {
          str1 = args[2];
          if (!args[2].Contains<char>('.') || (args[2].Contains<char>('/') || args[2].Contains<char>('\\')))
          {
            path = args[2];
            flag1 = true;
          }
        }
        Folder currentFolder = Programs.getCurrentFolder(os);
        bool flag2 = false;
        List<KeyValuePair<string, int>> keyValuePairList = new List<KeyValuePair<string, int>>();
        for (int index = 0; index < currentFolder.files.Count; ++index)
        {
          if (args[1] == "*")
          {
            flag2 = true;
            keyValuePairList.Add(new KeyValuePair<string, int>(currentFolder.files[index].name, currentFolder.files[index].size));
          }
          else if (currentFolder.files[index].name.ToLower().Equals(args[1].ToLower()))
          {
            flag2 = true;
            keyValuePairList.Add(new KeyValuePair<string, int>(currentFolder.files[index].name, currentFolder.files[index].size));
            break;
          }
        }
        if (!flag2)
        {
          os.write("File \"" + args[1] + "\" Does Not Exist\n");
        }
        else
        {
          for (int index1 = 0; index1 < keyValuePairList.Count; ++index1)
          {
            if (index1 > 0)
              Thread.Sleep(250);
            List<FileEntry> files = currentFolder.files;
            Folder folder1 = currentFolder;
            KeyValuePair<string, int> keyValuePair = keyValuePairList[index1];
            string key1 = keyValuePair.Key;
            FileEntry fileEntry = folder1.searchForFile(key1);
            int index2 = files.IndexOf(fileEntry);
            keyValuePair = keyValuePairList[index1];
            int num1 = keyValuePair.Value;
            if (!flag1)
            {
              string lower = currentFolder.files[index2].name.ToLower();
              if (lower.EndsWith(".exe"))
                path = "bin";
              else if (lower.EndsWith(".sys"))
                path = "sys";
              else if (lower.StartsWith("@"))
              {
                path = "home/dl_logs";
                Folder folder2 = os.thisComputer.files.root.searchForFolder("home");
                if (folder2.searchForFolder("dl_logs") == null)
                  folder2.folders.Add(new Folder("dl_logs"));
              }
              else
                path = "home";
            }
            string str2 = currentFolder.files[index2].name;
            string[] strArray = str1.Split(Utils.directorySplitterDelim, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length > 0 && strArray[strArray.Length - 1].Contains("."))
            {
              str2 = strArray[strArray.Length - 1];
              StringBuilder stringBuilder = new StringBuilder();
              int num2 = 0;
              for (int index3 = 0; index3 < strArray.Length - 1; ++index3)
              {
                stringBuilder.Append(strArray[index3]);
                stringBuilder.Append('/');
                ++num2;
              }
              if (num2 > 0)
              {
                path = stringBuilder.ToString();
                if (path.Length > 0)
                  path = path.Substring(0, path.Length - 1);
              }
            }
            string likelyFilename = (string) null;
            Folder folder3 = Programs.getFolderAtPathAsFarAsPossible(path, os, os.thisComputer.files.root, out likelyFilename);
            if (likelyFilename != null)
              str2 = likelyFilename;
            if (likelyFilename == path && folder3 == os.thisComputer.files.root)
              folder3 = Programs.getCurrentFolder(os);
            Computer computer = os.connectedComp != null ? os.connectedComp : os.thisComputer;
            string ip = os.thisComputer.ip;
            keyValuePair = keyValuePairList[index1];
            string key2 = keyValuePair.Key;
            if (computer.canCopyFile(ip, key2))
            {
              OS os1 = os;
              string str3 = "Copying Remote File ";
              keyValuePair = keyValuePairList[index1];
              string key3 = keyValuePair.Key;
              string str4 = "\nto Local Folder /";
              string str5 = path;
              string text = str3 + key3 + str4 + str5;
              os1.write(text);
              os.write(".");
              for (int index3 = 0; index3 < Math.Min(num1 / 500, 20); ++index3)
              {
                os.writeSingle(".");
                OS.operationProgress = (float) index3 / (float) (num1 / 1000);
                Thread.Sleep(200);
              }
              string nameEntry = str2;
              int num2 = 0;
              bool flag3;
              do
              {
                flag3 = true;
                for (int index3 = 0; index3 < folder3.files.Count; ++index3)
                {
                  if (folder3.files[index3].name == nameEntry)
                  {
                    ++num2;
                    nameEntry = str2 + "(" + (object) num2 + ")";
                    flag3 = false;
                    break;
                  }
                }
              }
              while (!flag3);
              try
              {
                folder3.files.Add(new FileEntry(currentFolder.files[index2].data, nameEntry));
              }
              catch (ArgumentOutOfRangeException ex)
              {
                os.write("SCP ERROR: File not found.");
                break;
              }
              os.write("Transfer Complete\n");
            }
            else
              os.write("Insufficient Privileges to Perform Operation : This File Requires Admin Access\n");
          }
        }
      }
    }

    public static void upload(string[] args, OS os)
    {
      if (os.connectedComp == null || os.connectedComp == os.thisComputer)
      {
        os.write("Must be Connected to a Non-Local Host\n");
        os.write("Connect to a REMOTE host and run upload with a LOCAL filepath\n");
      }
      else if (args.Length < 2)
        os.write("Not Enough Arguments");
      else if (!os.hasConnectionPermission(false))
      {
        os.write("Insufficient user permissions to upload");
      }
      else
      {
        Folder root = os.thisComputer.files.root;
        int length = args[1].LastIndexOf('/');
        if (length > 0)
        {
          string path = args[1].Substring(0, length);
          Folder folderAtPath = Programs.getFolderAtPath(path, os, os.thisComputer.files.root, false);
          if (folderAtPath == null)
          {
            os.write("Local Folder " + path + " not found.");
          }
          else
          {
            string fileName = args[1].Substring(length + 1);
            FileEntry fileEntry = folderAtPath.searchForFile(fileName);
            if (fileEntry == null)
            {
              os.write("File " + fileName + " not found at specified filepath.");
            }
            else
            {
              Folder currentFolder = Programs.getCurrentFolder(os);
              List<int> folderPath = new List<int>();
              folderPath.AddRange((IEnumerable<int>) os.navigationPath);
              os.write("Uploading Local File " + fileName + "\nto Remote Folder /" + currentFolder.name);
              int num = fileEntry.size;
              if (num > 150)
                num = (int) (((double) num - 150.0) * 0.200000002980232 + 150.0);
              for (int index = 0; index < num / 300; ++index)
              {
                os.writeSingle(".");
                OS.operationProgress = (float) index / (float) (num / 1000);
                Thread.Sleep(200);
              }
              os.connectedComp.makeFile(os.thisComputer.ip, fileEntry.name, fileEntry.data, folderPath, true);
              os.write("Transfer Complete\n");
            }
          }
        }
      }
    }

    public static void replace2(string[] args, OS os)
    {
      Folder currentFolder = Programs.getCurrentFolder(os);
      FileEntry fileEntry = (FileEntry) null;
      int index1 = 2;
      string[] tokens = Utils.SplitToTokens(args);
      if (tokens.Length < 3)
      {
        os.write("Not Enough Arguments\n");
        os.write("Usage: replace targetFile.txt \"Target String\" \"Replacement String\"");
      }
      else
      {
        if (tokens.Length <= 3)
        {
          if (os.display.command.StartsWith("cat"))
          {
            string commandArg = os.display.commandArgs[1];
            fileEntry = currentFolder.searchForFile(commandArg);
            if (fileEntry != null)
            {
              os.write("Assuming active flag file \"" + commandArg + "\" For editing");
              index1 = 1;
            }
          }
          if (fileEntry == null)
            os.write("Not Enough Arguments\n");
        }
        if (fileEntry == null)
        {
          for (int index2 = 0; index2 < currentFolder.files.Count; ++index2)
          {
            if (currentFolder.files[index2].name.Equals(args[1]))
              fileEntry = currentFolder.files[index2];
          }
          if (fileEntry == null)
          {
            os.write("File " + args[1] + " not found.");
            if (!os.display.command.StartsWith("cat"))
              return;
            string commandArg = os.display.commandArgs[1];
            if (currentFolder.searchForFile(commandArg) == null)
              return;
            os.write("Assuming active file \"" + commandArg + "\" for editing");
            return;
          }
        }
        string oldValue = tokens[index1].Replace("\"", "");
        string newValue = tokens[index1 + 1].Replace("\"", "");
        string data = fileEntry.data;
        fileEntry.data = fileEntry.data.Replace(oldValue, newValue);
        if (fileEntry.data.Length > 20000)
        {
          fileEntry.data = data;
          os.write("REPLACE ERROR: Replacement will cause file to be too long.");
        }
        if (!os.display.command.ToLower().Equals("cat"))
          return;
        os.displayCache = fileEntry.data;
      }
    }

    public static void replace(string[] args, OS os)
    {
      Folder currentFolder = Programs.getCurrentFolder(os);
      FileEntry fileEntry = (FileEntry) null;
      int num1 = 2;
      if (args.Length <= 3)
      {
        if (os.display.command.StartsWith("cat"))
        {
          string commandArg = os.display.commandArgs[1];
          fileEntry = currentFolder.searchForFile(commandArg);
          if (fileEntry != null)
          {
            os.write("Assuming active flag file \"" + commandArg + "\" For editing");
            num1 = 1;
          }
        }
        if (fileEntry == null)
          os.write("Not Enough Arguments\n");
      }
      if (fileEntry == null)
      {
        for (int index = 0; index < currentFolder.files.Count; ++index)
        {
          if (currentFolder.files[index].name.Equals(args[1]))
            fileEntry = currentFolder.files[index];
        }
        if (fileEntry == null)
        {
          os.write("File " + args[1] + " not found.");
          if (!os.display.command.StartsWith("cat"))
            return;
          string commandArg = os.display.commandArgs[1];
          if (currentFolder.searchForFile(commandArg) == null)
            return;
          os.write("Assuming active file \"" + commandArg + "\" for editing");
          return;
        }
      }
      string str1 = "";
      for (int index = num1; index < args.Length; ++index)
        str1 = str1 + args[index] + " ";
      string str2 = str1.Trim();
      if ((int) str2[0] != 34)
        return;
      int num2 = str2.IndexOf('"', 1);
      if (num2 >= 1)
      {
        int num3 = num2 - 1;
        int length1 = num3;
        string oldValue = str2.Substring(1, length1);
        int startIndex1 = num3 + 2;
        int num4 = str2.IndexOf(" \"", startIndex1);
        if (num4 > num3)
        {
          int num5 = str2.LastIndexOf('"');
          if (num5 > num4)
          {
            int startIndex2 = num4 + 2;
            int length2 = num5 - startIndex2;
            string newValue = str2.Substring(startIndex2, length2);
            fileEntry.data = fileEntry.data.Replace(oldValue, newValue);
            if (!os.display.command.ToLower().Equals("cat"))
              return;
            os.displayCache = fileEntry.data;
            return;
          }
        }
      }
      os.write("Format Error: Target and Replacement strings not found.");
      os.write("Usage: replace targetFile.txt \"Target String\" \"Replacement String\"");
    }

    public static void rm(string[] args, OS os)
    {
      List<FileEntry> fileEntryList = new List<FileEntry>();
      if (args.Length <= 1)
      {
        os.write("Not Enough Arguments\n");
      }
      else
      {
        Computer computer = os.connectedComp != null ? os.connectedComp : os.thisComputer;
        int length = args[1].LastIndexOf('/');
        string str = args[1];
        string path = (string) null;
        if (length > 0 && length < args[1].Length - 1)
        {
          str = args[1].Substring(length + 1);
          path = args[1].Substring(0, length);
        }
        Folder rootFolder = Programs.getCurrentFolder(os);
        if (path != null)
        {
          rootFolder = Programs.getFolderAtPath(path, os, rootFolder, true);
          if (rootFolder == null)
          {
            os.write("Folder " + path + " Not found!");
            return;
          }
        }
        if (str.Equals("*"))
        {
          for (int index = 0; index < rootFolder.files.Count; ++index)
          {
            if (!fileEntryList.Contains(rootFolder.files[index]))
              fileEntryList.Add(rootFolder.files[index]);
          }
        }
        else
        {
          bool flag = false;
          for (int index = 0; index < rootFolder.files.Count; ++index)
          {
            if (rootFolder.files[index].name.Equals(str))
            {
              fileEntryList.Add(rootFolder.files[index]);
              break;
            }
            if (flag)
              break;
          }
        }
        if (fileEntryList.Count == 0)
        {
          os.write("File " + str + " not found!");
        }
        else
        {
          List<int> folderPath = new List<int>();
          for (int index = 0; index < os.navigationPath.Count; ++index)
            folderPath.Add(os.navigationPath[index]);
          if (path != null)
            folderPath.AddRange((IEnumerable<int>) Programs.getNavigationPathAtPath(path, os, Programs.getCurrentFolder(os)));
          for (int index1 = 0; index1 < fileEntryList.Count; ++index1)
          {
            os.write(LocaleTerms.Loc("Deleting") + " " + fileEntryList[index1].name + ".");
            for (int index2 = 0; index2 < Math.Min(Math.Max(fileEntryList[index1].size / 1000, 3), 26); ++index2)
            {
              Thread.Sleep(200);
              os.writeSingle(".");
            }
            if (!computer.deleteFile(os.thisComputer.ip, fileEntryList[index1].name, folderPath))
              os.writeSingle(LocaleTerms.Loc("Error - Insufficient Privileges"));
            else
              os.writeSingle(LocaleTerms.Loc("Done"));
          }
          os.write("");
        }
      }
    }

    public static void rm2(string[] args, OS os)
    {
      List<FileEntry> fileEntryList = new List<FileEntry>();
      if (args.Length <= 1)
      {
        os.write("Not Enough Arguments\n");
      }
      else
      {
        Computer computer = os.connectedComp != null ? os.connectedComp : os.thisComputer;
        bool flag1 = false;
        string str1 = args[1];
        string str2;
        if (args[1].ToLower() == "-rf" && args.Length >= 3)
        {
          str2 = args[2];
          flag1 = true;
        }
        else
          str2 = args[1];
        int length = str2.LastIndexOf('/');
        string str3 = str2;
        string path = (string) null;
        if (length > 0 && length < str2.Length - 1)
        {
          str3 = str2.Substring(length + 1);
          path = str2.Substring(0, length);
        }
        Folder rootFolder = Programs.getCurrentFolder(os);
        if (path != null)
        {
          rootFolder = Programs.getFolderAtPath(path, os, rootFolder, true);
          if (rootFolder == null)
          {
            os.write("Folder " + path + " Not found!");
            return;
          }
        }
        if (str3.Equals("*") || str3.Equals("*.*"))
        {
          for (int index = 0; index < rootFolder.files.Count; ++index)
          {
            if (!fileEntryList.Contains(rootFolder.files[index]))
              fileEntryList.Add(rootFolder.files[index]);
          }
        }
        else
        {
          bool flag2 = false;
          for (int index = 0; index < rootFolder.files.Count; ++index)
          {
            if (rootFolder.files[index].name.Equals(str3))
            {
              fileEntryList.Add(rootFolder.files[index]);
              break;
            }
            if (flag2)
              break;
          }
        }
        if (fileEntryList.Count == 0)
        {
          os.write("File " + str3 + " not found!");
        }
        else
        {
          List<int> folderPath = new List<int>();
          for (int index = 0; index < os.navigationPath.Count; ++index)
            folderPath.Add(os.navigationPath[index]);
          if (path != null)
            folderPath.AddRange((IEnumerable<int>) Programs.getNavigationPathAtPath(path, os, Programs.getCurrentFolder(os)));
          for (int index1 = 0; index1 < fileEntryList.Count; ++index1)
          {
            os.write(LocaleTerms.Loc("Deleting") + " " + fileEntryList[index1].name + ".");
            for (int index2 = 0; index2 < Math.Min(Math.Max(fileEntryList[index1].size / 1000, 3), 26); ++index2)
            {
              Thread.Sleep(200);
              os.writeSingle(".");
            }
            if (!computer.deleteFile(os.thisComputer.ip, fileEntryList[index1].name, folderPath))
              os.writeSingle(LocaleTerms.Loc("Error - Insufficient Privileges"));
            else
              os.writeSingle(LocaleTerms.Loc("Done"));
            if (os.HasExitedAndEnded)
              return;
          }
          os.write("");
        }
      }
    }

    public static void mv(string[] args, OS os)
    {
      Computer computer = os.connectedComp != null ? os.connectedComp : os.thisComputer;
      Programs.getCurrentFolder(os);
      List<string> stringList = new List<string>((IEnumerable<string>) args);
      for (int index = 0; index < stringList.Count; ++index)
      {
        if (string.IsNullOrWhiteSpace(stringList[index]))
        {
          stringList.RemoveAt(index);
          --index;
        }
      }
      List<int> folderPath = new List<int>();
      for (int index = 0; index < os.navigationPath.Count; ++index)
        folderPath.Add(os.navigationPath[index]);
      if (stringList.Count < 3)
      {
        os.write(LocaleTerms.Loc("Not Enough Arguments. Usage: mv [FILE] [DESTINATION]"));
      }
      else
      {
        char[] chArray = new char[2]{ '\\', '/' };
        string name = stringList[1];
        string str = stringList[2];
        string path = "";
        int num = str.LastIndexOf('/');
        string newName;
        if (num > 0)
        {
          newName = str.Substring(num + 1);
          path = str.Substring(0, num + 1);
          if (newName.Length <= 0)
            newName = name;
        }
        else
          newName = str;
        List<int> navigationPathAtPath = Programs.getNavigationPathAtPath(path, os, (Folder) null);
        navigationPathAtPath.InsertRange(0, (IEnumerable<int>) folderPath);
        computer.moveFile(os.thisComputer.ip, name, newName, folderPath, navigationPathAtPath);
      }
    }

    public static void analyze(string[] args, OS os)
    {
      Computer computer = os.connectedComp != null ? os.connectedComp : os.thisComputer;
      if (computer.firewall != null)
      {
        computer.firewall.writeAnalyzePass((object) os, (object) computer);
        computer.hostileActionTaken();
      }
      else
        os.write("No Firewall Detected");
    }

    public static void solve(string[] args, OS os)
    {
      Computer computer = os.connectedComp != null ? os.connectedComp : os.thisComputer;
      if (computer.firewall != null)
      {
        if (args.Length >= 2)
        {
          string attempt = args[1];
          os.write("");
          Programs.doDots(30, 60, os);
          if (computer.firewall.attemptSolve(attempt, (object) os))
            os.write("SOLVE " + LocaleTerms.Loc("SUCCESSFUL") + " - Syndicated UDP Traffic Enabled");
          else
            os.write("SOLVE " + LocaleTerms.Loc("FAILED") + " - Incorrect bypass sequence");
        }
        else
          os.write(LocaleTerms.Loc("ERROR: No Solution provided"));
      }
      else
        os.write("No Firewall Detected");
    }

    public static void clear(string[] args, OS os)
    {
      os.terminal.reset();
    }

    public static void execute(string[] args, OS os)
    {
      Computer computer = os.connectedComp != null ? os.connectedComp : os.thisComputer;
      Folder folder = os.thisComputer.files.root.folders[2];
      os.write("Available Executables:\n");
      os.write("PortHack");
      os.write("ForkBomb");
      os.write("Shell");
      os.write("Tutorial");
      for (int index1 = 0; index1 < folder.files.Count; ++index1)
      {
        for (int index2 = 0; index2 < PortExploits.exeNums.Count; ++index2)
        {
          if (folder.files[index1].data == PortExploits.crackExeData[PortExploits.exeNums[index2]] || folder.files[index1].data == PortExploits.crackExeDataLocalRNG[PortExploits.exeNums[index2]])
          {
            os.write(folder.files[index1].name.Replace(".exe", ""));
            break;
          }
        }
      }
      os.write(" ");
    }

    public static void scan(string[] args, OS os)
    {
      if (args.Length > 1)
      {
        Computer c = (Computer) null;
        for (int index = 0; index < os.netMap.nodes.Count; ++index)
        {
          if (os.netMap.nodes[index].ip.Equals(args[1]) || os.netMap.nodes[index].name.Equals(args[1]))
          {
            c = os.netMap.nodes[index];
            break;
          }
        }
        if (c == null)
          return;
        os.netMap.discoverNode(c);
        os.write("Found Terminal : " + c.name + "@" + c.ip);
      }
      else
      {
        Computer computer = os.connectedComp != null ? os.connectedComp : os.thisComputer;
        if (os.hasConnectionPermission(true))
        {
          os.write("Scanning...");
          for (int index = 0; index < computer.links.Count; ++index)
          {
            if (!os.netMap.visibleNodes.Contains(computer.links[index]))
              os.netMap.visibleNodes.Add(computer.links[index]);
            os.netMap.nodes[computer.links[index]].highlightFlashTime = 1f;
            os.write("Found Terminal : " + os.netMap.nodes[computer.links[index]].name + "@" + os.netMap.nodes[computer.links[index]].ip);
            os.netMap.lastAddedNode = os.netMap.nodes[computer.links[index]];
            Thread.Sleep(400);
          }
          os.write("Scan Complete\n");
        }
        else
          os.write("Scanning Requires Admin Access\n");
      }
    }

    public static void fastHack(string[] args, OS os)
    {
      if (!OS.DEBUG_COMMANDS || os.connectedComp == null)
        return;
      os.connectedComp.adminIP = os.thisComputer.ip;
    }

    public static void revealAll(string[] args, OS os)
    {
      for (int index = 0; index < os.netMap.nodes.Count; ++index)
        os.netMap.visibleNodes.Add(index);
    }

    public static void cd(string[] args, OS os)
    {
      if (os.hasConnectionPermission(false))
      {
        if (args.Length >= 2)
        {
          Folder folder = Programs.getCurrentFolder(os);
          if (args[1].Equals("/"))
            os.navigationPath.Clear();
          if (args[1].Equals(".."))
          {
            if (os.navigationPath.Count > 0)
            {
              os.navigationPath.RemoveAt(os.navigationPath.Count - 1);
            }
            else
            {
              os.display.command = "connect";
              os.validCommand = false;
            }
          }
          else
          {
            if (args[1].StartsWith("/"))
            {
              folder = (os.connectedComp != null ? os.connectedComp : os.thisComputer).files.root;
              os.navigationPath.Clear();
            }
            List<int> navigationPathAtPath = Programs.getNavigationPathAtPath(args[1], os, (Folder) null);
            for (int index = 0; index < navigationPathAtPath.Count; ++index)
            {
              if (navigationPathAtPath[index] == -1)
                os.navigationPath.RemoveAt(os.navigationPath.Count - 1);
              else
                os.navigationPath.Add(navigationPathAtPath[index]);
            }
          }
        }
        else
          os.write("Usage: cd [WHERE TO GO or .. TO GO BACK]");
        string str1 = "";
        if (os.connectedComp != null)
          str1 = os.connectedComp.ip + "/";
        for (int index = 0; index < os.navigationPath.Count; ++index)
          str1 = str1 + Programs.getFolderAtDepth(os, index + 1).name + "/";
        string str2 = str1 + "> ";
        os.terminal.prompt = str2;
      }
      else
        os.write("Insufficient Privileges to Perform Operation");
    }

    public static void probe(string[] args, OS os)
    {
      Computer computer = os.connectedComp != null ? os.connectedComp : os.thisComputer;
      string ip = computer.ip;
      os.write("Probing " + ip + "...\n");
      for (int index = 0; index < 10; ++index)
      {
        Thread.Sleep(80);
        os.writeSingle(".");
      }
      os.write("\nProbe Complete - Open ports:\n");
      os.write("---------------------------------");
      for (int index = 0; index < computer.ports.Count; ++index)
      {
        os.write("Port#: " + (object) computer.GetDisplayPortNumberFromCodePort(computer.ports[index]) + "  -  " + PortExploits.services[computer.ports[index]] + ((int) computer.portsOpen[index] > 0 ? (object) " : OPEN" : (object) ""));
        Thread.Sleep(120);
      }
      os.write("---------------------------------");
      os.write("Open Ports Required for Crack : " + (object) Math.Max(computer.portsNeededForCrack + 1, 0));
      if (computer.hasProxy)
        os.write("Proxy Detected : " + (computer.proxyActive ? "ACTIVE" : "INACTIVE"));
      if (computer.firewall == null)
        return;
      os.write("Firewall Detected : " + (computer.firewall.solved ? "SOLVED" : "ACTIVE"));
    }

    public static void reboot(string[] args, OS os)
    {
      if (os.hasConnectionPermission(true))
      {
        bool flag1 = os.connectedComp == null || os.connectedComp == os.thisComputer;
        bool flag2 = false;
        if (args.Length > 1 && args[1].ToLower() == "-i")
          flag2 = true;
        Computer computer = os.connectedComp ?? os.thisComputer;
        if (!flag1)
          os.write("Rebooting Connected System : " + computer.name);
        if (!flag2)
        {
          int num = 5;
          while (num > 0)
          {
            os.write("System Reboot in ............" + (object) num);
            --num;
            Thread.Sleep(1000);
            if (os.HasExitedAndEnded)
              return;
          }
        }
        if (os.HasExitedAndEnded)
          return;
        if (computer == null || computer == os.thisComputer)
          os.rebootThisComputer();
        else
          computer.reboot(os.thisComputer.ip);
      }
      else
        os.write("Rebooting requires Admin access");
    }

    public static void addNote(string[] args, OS os)
    {
      string str = "";
      for (int index = 1; index < args.Length; ++index)
        str = str + args[index] + " ";
      NotesExe.AddNoteToOS(str.Trim().Replace("\\n", "\n"), os, false);
    }

    public static void opCDTray(string[] args, OS os, bool isOpen)
    {
      Computer computer = os.connectedComp != null ? os.connectedComp : os.thisComputer;
      if (os.hasConnectionPermission(true) || computer.ip.Equals(os.thisComputer.ip))
      {
        if (isOpen)
          computer.openCDTray(computer.ip);
        else
          computer.closeCDTray(computer.ip);
      }
      else
        os.write("Insufficient Privileges to Perform Operation");
    }

    [DllImport("winmm.dll")]
    private static extern long mciSendString(string a, StringBuilder b, int c, IntPtr d);

    [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void system([MarshalAs(UnmanagedType.LPStr)] string cmd);

    public static void cdDrive(bool open)
    {
      string platform = SDL.SDL_GetPlatform();
      if (platform.Equals("Linux"))
        Programs.system("eject" + (open ? "" : " -t"));
      else if (platform.Equals("Windows"))
      {
        Programs.mciSendString("set cdaudio door " + (open ? "open" : "closed"), (StringBuilder) null, 0, IntPtr.Zero);
      }
      else
      {
        if (!platform.Equals("Mac OS X"))
          throw new NotSupportedException("Unhandled SDL2 platform!");
        Console.WriteLine("CD drive interop unsupported on OSX!");
      }
    }

    public static void sudo(OS os, Action action)
    {
      string adminIp = os.connectedComp.adminIP;
      os.connectedComp.adminIP = os.thisComputer.ip;
      if (action != null)
        action();
      os.connectedComp.adminIP = adminIp;
    }

    public static Folder getCurrentFolder(OS os)
    {
      return Programs.getFolderAtDepth(os, os.navigationPath.Count);
    }

    public static Folder getFolderAtDepth(OS os, int depth)
    {
      Folder folder = os.connectedComp != null ? os.connectedComp.files.root : os.thisComputer.files.root;
      if (os.navigationPath.Count > 0)
      {
        try
        {
          for (int index = 0; index < depth; ++index)
          {
            if (folder.folders.Count > os.navigationPath[index])
              folder = folder.folders[os.navigationPath[index]];
          }
        }
        catch
        {
        }
      }
      return folder;
    }

    public static bool computerExists(OS os, string ip)
    {
      for (int index = 0; index < os.netMap.nodes.Count; ++index)
      {
        if (os.netMap.nodes[index].ip.Equals(ip) || os.netMap.nodes[index].name.Equals(ip))
          return true;
      }
      return false;
    }

    public static Computer getComputer(OS os, string ip_Or_ID_or_Name)
    {
      for (int index = 0; index < os.netMap.nodes.Count; ++index)
      {
        if (os.netMap.nodes[index].ip.Equals(ip_Or_ID_or_Name) || os.netMap.nodes[index].idName.Equals(ip_Or_ID_or_Name) || os.netMap.nodes[index].name.Equals(ip_Or_ID_or_Name))
          return os.netMap.nodes[index];
      }
      return (Computer) null;
    }

    public static Folder getFolderAtPath(string path, OS os, Folder rootFolder = null, bool returnsNullOnNoFind = false)
    {
      Folder folder = rootFolder != null ? rootFolder : Programs.getCurrentFolder(os);
      char[] chArray = new char[2]{ '/', '\\' };
      string[] strArray = path.Split(chArray);
      int num = 0;
      for (int index1 = 0; index1 < strArray.Length; ++index1)
      {
        if (!(strArray[index1] == "") && !(strArray[index1] == " "))
        {
          if (strArray[index1] == "..")
          {
            ++num;
            folder = Programs.getFolderAtDepth(os, os.navigationPath.Count - num);
          }
          else
          {
            bool flag = false;
            for (int index2 = 0; index2 < folder.folders.Count; ++index2)
            {
              if (folder.folders[index2].name == strArray[index1])
              {
                folder = folder.folders[index2];
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              os.write("Invalid Path");
              if (!returnsNullOnNoFind)
                return Programs.getCurrentFolder(os);
              return (Folder) null;
            }
          }
        }
      }
      return folder;
    }

    public static Folder getFolderAtPathAsFarAsPossible(string path, OS os, Folder rootFolder)
    {
      Folder folder = rootFolder != null ? rootFolder : Programs.getCurrentFolder(os);
      char[] chArray = new char[2]{ '/', '\\' };
      string[] strArray = path.Split(chArray);
      int num = 0;
      if (path == "/")
        return (os.connectedComp ?? os.thisComputer).files.root;
      for (int index1 = 0; index1 < strArray.Length; ++index1)
      {
        if (!(strArray[index1] == "") && !(strArray[index1] == " "))
        {
          if (strArray[index1] == "..")
          {
            ++num;
            folder = Programs.getFolderAtDepth(os, os.navigationPath.Count - num);
          }
          else
          {
            for (int index2 = 0; index2 < folder.folders.Count; ++index2)
            {
              if (folder.folders[index2].name == strArray[index1])
              {
                folder = folder.folders[index2];
                break;
              }
            }
          }
        }
      }
      return folder;
    }

    public static Folder getFolderAtPathAsFarAsPossible(string path, OS os, Folder rootFolder, out string likelyFilename)
    {
      Folder folder = rootFolder != null ? rootFolder : Programs.getCurrentFolder(os);
      char[] chArray = new char[2]{ '/', '\\' };
      string[] strArray = path.Split(chArray);
      int num = 0;
      likelyFilename = (string) null;
      if (path == "/")
        return (os.connectedComp ?? os.thisComputer).files.root;
      for (int index1 = 0; index1 < strArray.Length; ++index1)
      {
        if (!(strArray[index1] == "") && !(strArray[index1] == " "))
        {
          if (strArray[index1] == "..")
          {
            ++num;
            folder = Programs.getFolderAtDepth(os, os.navigationPath.Count - num);
          }
          else
          {
            bool flag = false;
            for (int index2 = 0; index2 < folder.folders.Count; ++index2)
            {
              if (folder.folders[index2].name == strArray[index1])
              {
                folder = folder.folders[index2];
                flag = true;
                break;
              }
            }
            if (!flag)
              likelyFilename = strArray[index1];
          }
        }
      }
      return folder;
    }

    public static List<int> getNavigationPathAtPath(string path, OS os, Folder currentFolder = null)
    {
      List<int> intList = new List<int>();
      Folder folder = currentFolder != null ? currentFolder : Programs.getCurrentFolder(os);
      char[] chArray = new char[2]{ '/', '\\' };
      string[] strArray = path.Split(chArray);
      int num = 0;
      for (int index1 = 0; index1 < strArray.Length; ++index1)
      {
        if (!(strArray[index1] == "") && !(strArray[index1] == " "))
        {
          if (strArray[index1] == "..")
          {
            intList.Add(-1);
            ++num;
            folder = Programs.getFolderAtDepth(os, os.navigationPath.Count - num);
          }
          else
          {
            bool flag = false;
            for (int index2 = 0; index2 < folder.folders.Count; ++index2)
            {
              if (folder.folders[index2].name == strArray[index1])
              {
                folder = folder.folders[index2];
                flag = true;
                intList.Add(index2);
                break;
              }
            }
            if (!flag)
            {
              os.write("Invalid Path");
              intList.Clear();
              return intList;
            }
          }
        }
      }
      return intList;
    }

    public static Folder getFolderFromNavigationPath(List<int> path, Folder startFolder, OS os)
    {
      Folder folder1 = startFolder;
      Folder folder2 = startFolder;
      for (int index = 0; index < path.Count; ++index)
      {
        if (path[index] <= -1)
          folder1 = folder2;
        else if (folder1.folders.Count > path[index])
        {
          folder2 = folder1;
          folder1 = folder1.folders[path[index]];
        }
        else
        {
          os.write("Invalid Path");
          return folder1;
        }
      }
      return folder1;
    }
  }
}
