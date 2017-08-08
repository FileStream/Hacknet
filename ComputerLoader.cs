// Decompiled with JetBrains decompiler
// Type: Hacknet.ComputerLoader
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Extensions;
using Hacknet.Mission;
using Hacknet.PlatformAPI.Storage;
using Hacknet.Security;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Hacknet
{
  public static class ComputerLoader
  {
    public static Action MissionPreLoadComplete;
    public static Action postAllLoadedActions;
    private static OS os;

    public static void init(object opsys)
    {
      ComputerLoader.os = (OS) opsys;
    }

    public static object loadComputer(string filename, bool preventAddingToNetmap = false, bool preventInitDaemons = false)
    {
      filename = LocalizedFileLoader.GetLocalizedFilepath(filename);
      Computer c = (Computer) null;
      Stream input1;
      if (filename.EndsWith("ExampleComputer.xml"))
      {
        string s = File.ReadAllText(filename);
        int length = s.IndexOf("<!--START_LABYRINTHS_ONLY_CONTENT-->");
        string str = "<!--END_LABYRINTHS_ONLY_CONTENT-->";
        int num = s.IndexOf(str);
        if (length >= 0 && num >= 0)
          s = s.Substring(0, length) + s.Substring(num + str.Length);
        input1 = Utils.GenerateStreamFromString(s);
      }
      else
        input1 = (Stream) File.OpenRead(filename);
      XmlReader rdr = XmlReader.Create(input1);
      string str1 = "UNKNOWN";
      string str2 = "UNKNOWN";
      string str3 = (string) null;
      int seclevel = 0;
      byte compType = 1;
      bool flag1 = true;
      string compIP = NetworkMap.generateRandomIP();
      while (rdr.Name != "Computer")
      {
        rdr.Read();
        if (rdr.EOF)
          return (object) null;
      }
      if (rdr.MoveToAttribute("id"))
        str1 = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("name"))
        str2 = ComputerLoader.filter(rdr.ReadContentAsString());
      if (rdr.MoveToAttribute("security"))
        seclevel = rdr.ReadContentAsInt();
      if (rdr.MoveToAttribute("type"))
      {
        string str4 = rdr.ReadContentAsString();
        if (str4.ToLowerInvariant() == "empty")
          str4 = string.Concat((object) (byte) 4);
        compType = Convert.ToByte(str4);
      }
      if (rdr.MoveToAttribute("ip"))
        compIP = ComputerLoader.filter(rdr.ReadContentAsString());
      if (rdr.MoveToAttribute("icon"))
        str3 = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("allowsDefaultBootModule"))
        flag1 = rdr.ReadContentAsBoolean();
      c = new Computer(str2, compIP, ComputerLoader.os.netMap.getRandomPosition(), seclevel, compType, ComputerLoader.os);
      c.idName = str1;
      c.AllowsDefaultBootModule = flag1;
      c.icon = str3;
      if ((int) c.type == 4)
      {
        Folder folder = c.files.root.searchForFolder("home");
        if (folder != null)
        {
          folder.files.Clear();
          folder.folders.Clear();
        }
      }
      while (rdr.Name != "Computer")
      {
        if (rdr.Name.ToLower().Equals("file"))
        {
          bool flag2 = true;
          bool flag3 = false;
          string path = !rdr.MoveToAttribute("path") ? "home" : rdr.ReadContentAsString();
          string s1 = !rdr.MoveToAttribute("name") ? "Data" : rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("EduSafe"))
            flag2 = rdr.ReadContentAsString().ToLower() == "true";
          if (rdr.MoveToAttribute("EduSafeOnly"))
            flag3 = rdr.ReadContentAsString().ToLower() == "true";
          string str4 = ComputerLoader.filter(s1);
          int content = (int) rdr.MoveToContent();
          string s2 = rdr.ReadElementContentAsString();
          if (s2.Equals(""))
            s2 = Computer.generateBinaryString(500);
          string dataEntry = ComputerLoader.filter(s2);
          Folder folderFromPath = c.getFolderFromPath(path, true);
          if ((flag2 || !Settings.EducationSafeBuild) && (Settings.EducationSafeBuild || !flag3))
          {
            if (folderFromPath.searchForFile(str4) != null)
              folderFromPath.searchForFile(str4).data = dataEntry;
            else
              folderFromPath.files.Add(new FileEntry(dataEntry, str4));
          }
        }
        if (rdr.Name.Equals("encryptedFile"))
        {
          bool flag2 = false;
          string path = !rdr.MoveToAttribute("path") ? "home" : rdr.ReadContentAsString();
          string s1 = !rdr.MoveToAttribute("name") ? "Data" : rdr.ReadContentAsString();
          string header = !rdr.MoveToAttribute("header") ? "ERROR" : rdr.ReadContentAsString();
          string ipLink = !rdr.MoveToAttribute("ip") ? "ERROR" : rdr.ReadContentAsString();
          string pass = !rdr.MoveToAttribute("pass") ? "" : rdr.ReadContentAsString();
          string fileExtension = !rdr.MoveToAttribute("extension") ? (string) null : rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("double"))
            flag2 = rdr.ReadContentAsBoolean();
          string str4 = ComputerLoader.filter(s1);
          int content = (int) rdr.MoveToContent();
          string s2 = rdr.ReadElementContentAsString();
          if (s2.Equals(""))
            s2 = Computer.generateBinaryString(500);
          string data = ComputerLoader.filter(s2);
          if (flag2)
            data = FileEncrypter.EncryptString(data, header, ipLink, pass, fileExtension);
          string dataEntry = FileEncrypter.EncryptString(data, header, ipLink, pass, flag2 ? "_LAYER2.dec" : fileExtension);
          Folder folderFromPath = c.getFolderFromPath(path, true);
          if (folderFromPath.searchForFile(str4) != null)
            folderFromPath.searchForFile(str4).data = dataEntry;
          else
            folderFromPath.files.Add(new FileEntry(dataEntry, str4));
        }
        if (rdr.Name.Equals("memoryDumpFile"))
        {
          string path = !rdr.MoveToAttribute("path") ? "home" : rdr.ReadContentAsString();
          string str4 = !rdr.MoveToAttribute("name") ? "Data" : rdr.ReadContentAsString();
          int content = (int) rdr.MoveToContent();
          MemoryContents memoryContents = MemoryContents.Deserialize(rdr);
          Folder folderFromPath = c.getFolderFromPath(path, true);
          if (folderFromPath.searchForFile(str4) != null)
            folderFromPath.searchForFile(str4).data = memoryContents.GetEncodedFileString();
          else
            folderFromPath.files.Add(new FileEntry(memoryContents.GetEncodedFileString(), str4));
          while (!(rdr.Name == "memoryDumpFile") || rdr.IsStartElement() || rdr.EOF)
            rdr.Read();
          if (rdr.EOF)
            throw new FormatException("Unexpected end of file looking for memoryDumpFile close tag");
          rdr.Read();
        }
        Color color1;
        if (rdr.Name.ToLower().Equals("customthemefile"))
        {
          string path = !rdr.MoveToAttribute("path") ? "home" : rdr.ReadContentAsString();
          string str4 = ComputerLoader.filter(!rdr.MoveToAttribute("name") ? "Data.txt" : rdr.ReadContentAsString());
          string s = !rdr.MoveToAttribute("themePath") ? (string) null : ThemeManager.getThemeDataStringForCustomTheme(rdr.ReadContentAsString());
          if (string.IsNullOrWhiteSpace(s))
            s = "DEFINITION ERROR - Theme generated incorrectly. No Custom theme found at definition path";
          string dataEntry = ComputerLoader.filter(s);
          Folder folderFromPath = c.getFolderFromPath(path, true);
          if (folderFromPath.searchForFile(str4) != null)
            folderFromPath.searchForFile(str4).data = dataEntry;
          else
            folderFromPath.files.Add(new FileEntry(dataEntry, str4));
        }
        else if (rdr.Name.Equals("ports"))
        {
          int content = (int) rdr.MoveToContent();
          ComputerLoader.loadPortsIntoComputer(rdr.ReadElementContentAsString(), (object) c);
        }
        else if (rdr.Name.Equals("positionNear"))
        {
          string targetNearNodeName = "";
          if (rdr.MoveToAttribute("target"))
            targetNearNodeName = rdr.ReadContentAsString();
          int position = 0;
          int total = 3;
          if (rdr.MoveToAttribute("position"))
            position = rdr.ReadContentAsInt();
          ++position;
          if (rdr.MoveToAttribute("total"))
            total = rdr.ReadContentAsInt();
          bool forceUse = false;
          if (rdr.MoveToAttribute("force"))
            forceUse = rdr.ReadContentAsString().ToLower() == "true";
          float extraDistance = 0.0f;
          if (rdr.MoveToAttribute("extraDistance"))
          {
            extraDistance = rdr.ReadContentAsFloat();
            extraDistance = Math.Max(-1f, Math.Min(1f, extraDistance));
          }
          ComputerLoader.postAllLoadedActions += (Action) (() =>
          {
            Computer computer = Programs.getComputer(ComputerLoader.os, targetNearNodeName);
            if (computer == null)
              return;
            c.location = computer.location + Corporation.getNearbyNodeOffset(computer.location, position, total, ComputerLoader.os.netMap, extraDistance, forceUse);
          });
        }
        else if (rdr.Name.Equals("proxy"))
        {
          float num = 1f;
          if (rdr.MoveToAttribute("time"))
            num = rdr.ReadContentAsFloat();
          if ((double) num > 0.0)
          {
            c.addProxy(Computer.BASE_PROXY_TICKS * num);
          }
          else
          {
            c.hasProxy = false;
            c.proxyActive = false;
          }
        }
        else if (rdr.Name.Equals("portsForCrack"))
        {
          int num = -1;
          if (rdr.MoveToAttribute("val"))
            num = rdr.ReadContentAsInt();
          if (num != -1)
            c.portsNeededForCrack = num - 1;
        }
        else if (rdr.Name.Equals("firewall"))
        {
          int level = 1;
          if (rdr.MoveToAttribute("level"))
            level = rdr.ReadContentAsInt();
          if (level > 0)
          {
            string solution = (string) null;
            float additionalTime = 0.0f;
            if (rdr.MoveToAttribute("solution"))
              solution = rdr.ReadContentAsString();
            if (rdr.MoveToAttribute("additionalTime"))
              additionalTime = rdr.ReadContentAsFloat();
            if (solution != null)
              c.addFirewall(level, solution, additionalTime);
            else
              c.addFirewall(level);
          }
          else
            c.firewall = (Firewall) null;
        }
        else if (rdr.Name.Equals("link"))
        {
          string ip_Or_ID_or_Name = "";
          if (rdr.MoveToAttribute("target"))
            ip_Or_ID_or_Name = rdr.ReadContentAsString();
          Computer computer = Programs.getComputer(ComputerLoader.os, ip_Or_ID_or_Name);
          if (computer != null)
            c.links.Add(ComputerLoader.os.netMap.nodes.IndexOf(computer));
        }
        else if (rdr.Name.Equals("dlink"))
        {
          string comp = "";
          if (rdr.MoveToAttribute("target"))
            comp = rdr.ReadContentAsString();
          Computer local = c;
          ComputerLoader.postAllLoadedActions += (Action) (() =>
          {
            Computer computer = Programs.getComputer(ComputerLoader.os, comp);
            if (computer == null)
              return;
            local.links.Add(ComputerLoader.os.netMap.nodes.IndexOf(computer));
          });
        }
        else if (rdr.Name.Equals("trace"))
        {
          float num = 1f;
          if (rdr.MoveToAttribute("time"))
            num = rdr.ReadContentAsFloat();
          c.traceTime = num;
        }
        else if (rdr.Name.Equals("adminPass"))
        {
          string newPass = (string) null;
          if (rdr.MoveToAttribute("pass"))
            newPass = ComputerLoader.filter(rdr.ReadContentAsString());
          if (newPass == null)
            newPass = PortExploits.getRandomPassword();
          c.setAdminPassword(newPass);
        }
        else if (rdr.Name.Equals("admin"))
        {
          string str4 = "basic";
          bool flag2 = true;
          bool flag3 = false;
          if (rdr.MoveToAttribute("type"))
            str4 = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("resetPassword"))
            flag2 = rdr.ReadContentAsBoolean();
          if (rdr.MoveToAttribute("isSuper"))
            flag3 = rdr.ReadContentAsBoolean();
          switch (str4)
          {
            case "fast":
              c.admin = (Administrator) new FastBasicAdministrator();
              break;
            case "progress":
              c.admin = (Administrator) new FastProgressOnlyAdministrator();
              break;
            case "none":
              c.admin = (Administrator) null;
              break;
            default:
              c.admin = (Administrator) new BasicAdministrator();
              break;
          }
          if (c.admin != null)
          {
            c.admin.ResetsPassword = flag2;
            c.admin.IsSuper = flag3;
          }
        }
        else if (rdr.Name.Equals("portRemap"))
        {
          int content = (int) rdr.MoveToContent();
          string input2 = rdr.ReadElementContentAsString();
          try
          {
            c.PortRemapping = PortRemappingSerializer.Deserialize(input2);
          }
          catch (FormatException ex)
          {
            throw new FormatException("Error in portRemap tag. Check that your list is properly comma separated!\nBroken Data: " + input2, (Exception) ex);
          }
        }
        else if (rdr.Name.Equals("ExternalCounterpart"))
        {
          string serverName = "";
          string idName = "";
          if (rdr.MoveToAttribute("id"))
            serverName = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("name"))
            idName = rdr.ReadContentAsString();
          ExternalCounterpart externalCounterpart = new ExternalCounterpart(idName, ExternalCounterpart.getIPForServerName(serverName));
          c.externalCounterpart = externalCounterpart;
        }
        else if (rdr.Name.Equals("account"))
        {
          byte accountType = 0;
          string s1;
          string s2 = s1 = "ERROR";
          if (rdr.MoveToAttribute("username"))
            s2 = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("password"))
            s1 = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("type"))
          {
            string str4 = rdr.ReadContentAsString();
            accountType = !(str4.ToLower() == "admin") ? (!(str4.ToLower() == "all") ? (!(str4.ToLower() == "mail") ? (!(str4.ToLower() == "missionlist") ? (byte) Convert.ToInt32(str4) : (byte) 3) : (byte) 2) : (byte) 1) : (byte) 0;
          }
          string user = ComputerLoader.filter(s2);
          string password = ComputerLoader.filter(s1);
          bool flag2 = false;
          UserDetail userDetail;
          for (int index = 0; index < c.users.Count; ++index)
          {
            userDetail = c.users[index];
            if (userDetail.name.Equals(user))
            {
              userDetail.pass = password;
              userDetail.type = accountType;
              c.users[index] = userDetail;
              if (user.Equals("admin"))
                c.adminPass = password;
              flag2 = true;
            }
          }
          if (!flag2)
          {
            userDetail = new UserDetail(user, password, accountType);
            c.users.Add(userDetail);
          }
        }
        else if (rdr.Name.ToLower().Equals("tracker"))
          c.HasTracker = true;
        else if (rdr.Name.Equals("missionListingServer"))
        {
          bool flag2 = false;
          bool _isPublic = false;
          string serviceName;
          string group = serviceName = "ERROR";
          if (rdr.MoveToAttribute("name"))
            serviceName = ComputerLoader.filter(rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("group"))
            group = ComputerLoader.filter(rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("assigner"))
            flag2 = rdr.ReadContentAsBoolean();
          if (rdr.MoveToAttribute("public"))
            _isPublic = rdr.ReadContentAsBoolean();
          c.daemons.Add((Daemon) new MissionListingServer(c, serviceName, group, ComputerLoader.os, _isPublic, false)
          {
            missionAssigner = flag2
          });
        }
        else if (rdr.Name.Equals("variableMissionListingServer"))
        {
          bool _isAssigner = false;
          bool _isPublic = false;
          // ISSUE: variable of the null type
          __Null local;
          string str4 = (string) (local = null);
          string articleFolderPath = (string) local;
          string serviceName = (string) local;
          string iconPath = (string) local;
          Color themeColor = Color.IndianRed;
          if (rdr.MoveToAttribute("name"))
            serviceName = ComputerLoader.filter(rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("iconPath"))
            iconPath = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("articleFolderPath"))
            articleFolderPath = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("public"))
            _isPublic = rdr.ReadContentAsBoolean();
          if (rdr.MoveToAttribute("assigner"))
            _isAssigner = rdr.ReadContentAsBoolean();
          if (rdr.MoveToAttribute("title"))
            str4 = ComputerLoader.filter(rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("color"))
            themeColor = Utils.convertStringToColor(rdr.ReadContentAsString());
          MissionListingServer missionListingServer = new MissionListingServer(c, serviceName, iconPath, articleFolderPath, themeColor, ComputerLoader.os, _isPublic, _isAssigner);
          missionListingServer.missionAssigner = _isAssigner;
          if (str4 != null)
            missionListingServer.listingTitle = str4;
          c.daemons.Add((Daemon) missionListingServer);
        }
        else if (rdr.Name.Equals("missionHubServer"))
        {
          Color paleTurquoise;
          Color color2 = paleTurquoise = Color.PaleTurquoise;
          Color color3 = paleTurquoise;
          color1 = paleTurquoise;
          // ISSUE: variable of the null type
          __Null local;
          string serviceName = (string) (local = null);
          string str4 = (string) local;
          string group = (string) local;
          bool flag2 = true;
          if (rdr.MoveToAttribute("groupName"))
            ComputerLoader.filter(group = rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("serviceName"))
            serviceName = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("missionFolderPath"))
            str4 = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("themeColor"))
            color1 = Utils.convertStringToColor(rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("lineColor"))
            color3 = Utils.convertStringToColor(rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("backgroundColor"))
            color2 = Utils.convertStringToColor(rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("allowAbandon"))
            flag2 = rdr.ReadContentAsString().ToLower() == "true";
          string str5 = "Content/Missions/";
          if (Settings.IsInExtensionMode)
            str5 = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
          string str6 = str4.Replace('\\', '/');
          if (!str6.EndsWith("/"))
            str6 += "/";
          c.daemons.Add((Daemon) new MissionHubServer(c, serviceName, group, ComputerLoader.os)
          {
            MissionSourceFolderPath = (str5 + str6),
            themeColor = color1,
            themeColorBackground = color2,
            themeColorLine = color3,
            allowAbandon = flag2
          });
        }
        else if (rdr.Name.Equals("mailServer"))
        {
          string name = "Mail Server";
          if (rdr.MoveToAttribute("name"))
            name = rdr.ReadContentAsString();
          bool flag2 = true;
          if (rdr.MoveToAttribute("generateJunk"))
            flag2 = rdr.ReadContentAsString().ToLower() == "true";
          MailServer ms = new MailServer(c, name, ComputerLoader.os);
          ms.shouldGenerateJunkEmails = flag2;
          if (rdr.MoveToAttribute("color"))
            ms.setThemeColor(Utils.convertStringToColor(rdr.ReadContentAsString()));
          while (!(rdr.Name == "mailServer") || rdr.IsStartElement())
          {
            if (rdr.Name == "email")
            {
              string sender = "UNKNOWN";
              string str4 = (string) null;
              string subject = "UNKNOWN";
              if (rdr.MoveToAttribute("sender"))
                sender = ComputerLoader.filter(rdr.ReadContentAsString());
              if (rdr.MoveToAttribute("recipient"))
                str4 = ComputerLoader.filter(rdr.ReadContentAsString());
              if (rdr.MoveToAttribute("subject"))
                subject = ComputerLoader.filter(rdr.ReadContentAsString());
              int content = (int) rdr.MoveToContent();
              string body = ComputerLoader.filter(rdr.ReadElementContentAsString());
              if (str4 != null)
              {
                string email = MailServer.generateEmail(subject, body, sender);
                string recp = str4;
                ms.setupComplete += (Action) (() => ms.addMail(email, recp));
              }
            }
            rdr.Read();
          }
          c.daemons.Add((Daemon) ms);
        }
        else if (rdr.Name.Equals("addEmailDaemon"))
        {
          AddEmailDaemon addEmailDaemon = new AddEmailDaemon(c, "Final Task", ComputerLoader.os);
          c.daemons.Add((Daemon) addEmailDaemon);
        }
        else if (rdr.Name.Equals("deathRowDatabase"))
        {
          DeathRowDatabaseDaemon rowDatabaseDaemon = new DeathRowDatabaseDaemon(c, "Death Row Database", ComputerLoader.os);
          c.daemons.Add((Daemon) rowDatabaseDaemon);
        }
        else if (rdr.Name.Equals("academicDatabase"))
        {
          AcademicDatabaseDaemon academicDatabaseDaemon = new AcademicDatabaseDaemon(c, "International Academic Database", ComputerLoader.os);
          c.daemons.Add((Daemon) academicDatabaseDaemon);
        }
        else if (rdr.Name.Equals("ispSystem"))
        {
          ISPDaemon ispDaemon = new ISPDaemon(c, ComputerLoader.os);
          c.daemons.Add((Daemon) ispDaemon);
        }
        else if (rdr.Name.Equals("messageBoard"))
        {
          MessageBoardDaemon messageBoardDaemon = new MessageBoardDaemon(c, ComputerLoader.os);
          string str4 = "Anonymous";
          if (rdr.MoveToAttribute("name"))
            str4 = rdr.ReadContentAsString();
          messageBoardDaemon.name = str4;
          messageBoardDaemon.BoardName = str4;
          while (!(rdr.Name == "messageBoard") || rdr.IsStartElement())
          {
            if (rdr.Name == "thread")
            {
              int content = (int) rdr.MoveToContent();
              string str5 = rdr.ReadElementContentAsString();
              string str6 = "Content/Missions/";
              if (str5.StartsWith(str6))
                str5 = str5.Substring(str6.Length);
              if (Settings.IsInExtensionMode)
                str6 = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
              if (str5 != null)
                messageBoardDaemon.AddThread(Utils.readEntireFile(str6 + str5));
            }
            rdr.Read();
          }
          c.daemons.Add((Daemon) messageBoardDaemon);
        }
        else if (rdr.Name.Equals("addAvconDemoEndDaemon"))
        {
          AvconDemoEndDaemon avconDemoEndDaemon = new AvconDemoEndDaemon(c, "Demo End", ComputerLoader.os);
          c.daemons.Add((Daemon) avconDemoEndDaemon);
        }
        else if (rdr.Name.Equals("addWebServer"))
        {
          string serviceName = "Web Server";
          string pageFileLocation = (string) null;
          if (rdr.MoveToAttribute("name"))
            serviceName = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("url"))
            pageFileLocation = rdr.ReadContentAsString();
          WebServerDaemon webServerDaemon = new WebServerDaemon(c, serviceName, ComputerLoader.os, pageFileLocation);
          webServerDaemon.registerAsDefaultBootDaemon();
          c.daemons.Add((Daemon) webServerDaemon);
        }
        else if (rdr.Name.Equals("addOnlineWebServer"))
        {
          string serviceName = "Web Server";
          string url = (string) null;
          if (rdr.MoveToAttribute("name"))
            serviceName = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("url"))
            url = rdr.ReadContentAsString();
          OnlineWebServerDaemon onlineWebServerDaemon = new OnlineWebServerDaemon(c, serviceName, ComputerLoader.os);
          if (url != null)
            onlineWebServerDaemon.setURL(url);
          onlineWebServerDaemon.registerAsDefaultBootDaemon();
          c.daemons.Add((Daemon) onlineWebServerDaemon);
        }
        else if (rdr.Name.Equals("uploadServerDaemon"))
        {
          string serviceName = "File Upload Server";
          string foldername = (string) null;
          string input2 = "0,94,38";
          bool needsAuthentication = false;
          bool flag2 = false;
          if (rdr.MoveToAttribute("name"))
            serviceName = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("folder"))
            foldername = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("color"))
            input2 = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("needsAuth"))
            needsAuthentication = rdr.ReadContentAsString().ToLower() == "true";
          if (rdr.MoveToAttribute("hasReturnViewButton"))
            flag2 = rdr.ReadContentAsString().ToLower() == "true";
          Color color2 = Utils.convertStringToColor(input2);
          UploadServerDaemon uploadServerDaemon = new UploadServerDaemon(c, serviceName, color2, ComputerLoader.os, foldername, needsAuthentication);
          uploadServerDaemon.hasReturnViewButton = flag2;
          uploadServerDaemon.registerAsDefaultBootDaemon();
          c.daemons.Add((Daemon) uploadServerDaemon);
        }
        else if (rdr.Name.Equals("MedicalDatabase"))
        {
          MedicalDatabaseDaemon medicalDatabaseDaemon = new MedicalDatabaseDaemon(c, ComputerLoader.os);
          c.daemons.Add((Daemon) medicalDatabaseDaemon);
        }
        else if (rdr.Name.Equals("HeartMonitor"))
        {
          string str4 = "UNKNOWN";
          if (rdr.MoveToAttribute("patient"))
            str4 = rdr.ReadContentAsString();
          c.daemons.Add((Daemon) new HeartMonitorDaemon(c, ComputerLoader.os)
          {
            PatientID = str4
          });
        }
        else if (rdr.Name.Equals("PointClicker"))
        {
          PointClickerDaemon pointClickerDaemon = new PointClickerDaemon(c, "Point Clicker!", ComputerLoader.os);
          c.daemons.Add((Daemon) pointClickerDaemon);
        }
        else if (rdr.Name.Equals("PorthackHeart"))
        {
          PorthackHeartDaemon porthackHeartDaemon = new PorthackHeartDaemon(c, ComputerLoader.os);
          c.daemons.Add((Daemon) porthackHeartDaemon);
        }
        else if (rdr.Name.Equals("SongChangerDaemon"))
        {
          SongChangerDaemon songChangerDaemon = new SongChangerDaemon(c, ComputerLoader.os);
          c.daemons.Add((Daemon) songChangerDaemon);
        }
        else if (rdr.Name.Equals("DHSDaemon"))
        {
          ComputerLoader.DLCCheck(rdr.Name);
          string group = "UNKNOWN";
          bool flag2 = true;
          bool flag3 = true;
          bool flag4 = false;
          color1 = new Color(38, 201, 155);
          if (rdr.MoveToAttribute("groupName"))
            group = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("addsFactionPointOnMissionComplete"))
            flag2 = rdr.ReadContentAsString().ToLower() == "true";
          if (rdr.MoveToAttribute("autoClearMissionsOnPlayerComplete"))
            flag3 = rdr.ReadContentAsString().ToLower() == "true";
          if (rdr.MoveToAttribute("themeColor"))
            color1 = Utils.convertStringToColor(rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("allowContractAbbandon"))
            flag4 = rdr.ReadContentAsString().ToLower() == "true";
          DLCHubServer dlcHubServer = new DLCHubServer(c, "DHS", group, ComputerLoader.os);
          dlcHubServer.AddsFactionPointForMissionCompleteion = flag2;
          dlcHubServer.AutoClearMissionsOnSingleComplete = flag3;
          dlcHubServer.AllowContractAbbandon = flag4;
          dlcHubServer.themeColor = color1;
          while (!(rdr.Name == "DHSDaemon") || rdr.IsStartElement())
          {
            if (rdr.Name.ToLower() == "user" || rdr.Name.ToLower() == "agent")
            {
              string s = (string) null;
              if (rdr.MoveToAttribute("name"))
                s = ComputerLoader.filter(rdr.ReadContentAsString());
              string agentPassword = "password";
              if (rdr.MoveToAttribute("pass"))
                agentPassword = ComputerLoader.filter(rdr.ReadContentAsString());
              Color color2 = Color.LightGreen;
              if (rdr.MoveToAttribute("color"))
                color2 = Utils.convertStringToColor(rdr.ReadContentAsString());
              if (!string.IsNullOrWhiteSpace(s))
                dlcHubServer.AddAgent(ComputerLoader.filter(s), agentPassword, color2);
            }
            rdr.Read();
          }
          c.daemons.Add((Daemon) dlcHubServer);
        }
        else if (rdr.Name.Equals("CustomConnectDisplayDaemon"))
        {
          CustomConnectDisplayDaemon connectDisplayDaemon = new CustomConnectDisplayDaemon(c, ComputerLoader.os);
          c.daemons.Add((Daemon) connectDisplayDaemon);
        }
        else if (rdr.Name.Equals("DatabaseDaemon"))
        {
          ComputerLoader.DLCCheck(rdr.Name);
          string str4 = (string) null;
          string Foldername = (string) null;
          Color? ThemeColor = new Color?();
          if (rdr.MoveToAttribute("DataType"))
            str4 = rdr.ReadContentAsString();
          DatabaseDaemon.DatabasePermissions permissions = DatabaseDaemon.DatabasePermissions.Public;
          if (rdr.MoveToAttribute("Permissions"))
            permissions = DatabaseDaemon.GetDatabasePermissionsFromString(rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("Foldername"))
            Foldername = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("Color"))
            ThemeColor = new Color?(Utils.convertStringToColor(rdr.ReadContentAsString()));
          string str5 = (string) null;
          string str6 = (string) null;
          if (rdr.MoveToAttribute("AdminEmailAccount"))
            str5 = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("AdminEmailHostID"))
            str6 = rdr.ReadContentAsString();
          string name = "Database";
          if (rdr.MoveToAttribute("Name"))
            name = rdr.ReadContentAsString();
          List<object> objectList = (List<object>) null;
          rdr.MoveToElement();
          if (!rdr.IsEmptyElement)
          {
            Type typeForName = ObjectSerializer.GetTypeForName(str4);
            if (typeForName != (Type) null)
            {
              objectList = new List<object>();
              while (!(rdr.Name == "DatabaseDaemon") || rdr.IsStartElement())
              {
                if (rdr.Name != null && rdr.Name == ObjectSerializer.GetTagNameForType(typeForName))
                {
                  object obj = ObjectSerializer.DeserializeObject(rdr, typeForName);
                  objectList.Add(obj);
                }
                rdr.Read();
              }
            }
          }
          DatabaseDaemon databaseDaemon = new DatabaseDaemon(c, ComputerLoader.os, name, permissions, str4, Foldername, ThemeColor);
          if (!string.IsNullOrWhiteSpace(str5))
          {
            databaseDaemon.adminResetEmailHostID = str6;
            databaseDaemon.adminResetPassEmailAccount = str5;
          }
          databaseDaemon.Dataset = objectList;
          c.daemons.Add((Daemon) databaseDaemon);
        }
        else if (rdr.Name.Equals("WhitelistAuthenticatorDaemon"))
        {
          string str4 = (string) null;
          if (rdr.MoveToAttribute("Remote"))
            str4 = rdr.ReadContentAsString();
          bool flag2 = true;
          if (rdr.MoveToAttribute("SelfAuthenticating"))
            flag2 = rdr.ReadContentAsString().ToLower() == "true";
          WhitelistConnectionDaemon connectionDaemon = new WhitelistConnectionDaemon(c, ComputerLoader.os) { RemoteSourceIP = str4, AuthenticatesItself = flag2 };
          c.daemons.Add((Daemon) connectionDaemon);
        }
        else if (rdr.Name.Equals("MarkovTextDaemon"))
        {
          string corpusLoadPath;
          string name = corpusLoadPath = (string) null;
          if (rdr.MoveToAttribute("Name"))
            name = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("SourceFilesContentFolder"))
            corpusLoadPath = rdr.ReadContentAsString();
          MarkovTextDaemon markovTextDaemon = new MarkovTextDaemon(c, ComputerLoader.os, name, corpusLoadPath);
          c.daemons.Add((Daemon) markovTextDaemon);
        }
        else if (rdr.Name.Equals("IRCDaemon"))
        {
          color1 = new Color(184, 2, 141);
          if (rdr.MoveToAttribute("themeColor"))
            color1 = Utils.convertStringToColor(rdr.ReadContentAsString());
          string name = "IRC Server";
          if (rdr.MoveToAttribute("name"))
            name = rdr.ReadContentAsString();
          bool flag2 = false;
          if (rdr.MoveToAttribute("needsLogin"))
            flag2 = rdr.ReadContentAsString().ToLower() == "true";
          IRCDaemon ircDaemon = new IRCDaemon(c, ComputerLoader.os, name);
          ircDaemon.ThemeColor = color1;
          ircDaemon.RequiresLogin = flag2;
          while (!(rdr.Name == "IRCDaemon") || rdr.IsStartElement())
          {
            if (rdr.Name.ToLower() == "user" || rdr.Name.ToLower() == "agent")
            {
              string s = (string) null;
              if (rdr.MoveToAttribute("name"))
                s = ComputerLoader.filter(rdr.ReadContentAsString());
              Color color2 = Color.LightGreen;
              if (rdr.MoveToAttribute("color"))
                color2 = Utils.convertStringToColor(rdr.ReadContentAsString());
              if (!string.IsNullOrWhiteSpace(s))
                ircDaemon.UserColors.Add(ComputerLoader.filter(s), color2);
            }
            if (rdr.Name == "post" && rdr.IsStartElement())
            {
              string key = (string) null;
              if (rdr.MoveToAttribute("user"))
                key = ComputerLoader.filter(rdr.ReadContentAsString());
              rdr.MoveToElement();
              string s = rdr.ReadElementContentAsString();
              if (!string.IsNullOrWhiteSpace(s) && !string.IsNullOrWhiteSpace(key))
                ircDaemon.StartingMessages.Add(new KeyValuePair<string, string>(key, ComputerLoader.filter(s)));
            }
            rdr.Read();
          }
          c.daemons.Add((Daemon) ircDaemon);
        }
        else if (rdr.Name.Equals("AircraftDaemon"))
        {
          ComputerLoader.DLCCheck(rdr.Name);
          Vector2 zero = Vector2.Zero;
          Vector2 mapDest = Vector2.One * 0.5f;
          float progress = 0.5f;
          string name = (string) null;
          if (rdr.MoveToAttribute("Name"))
            name = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("OriginX"))
            zero.X = rdr.ReadContentAsFloat();
          if (rdr.MoveToAttribute("OriginY"))
            zero.Y = rdr.ReadContentAsFloat();
          if (rdr.MoveToAttribute("DestX"))
            mapDest.X = rdr.ReadContentAsFloat();
          if (rdr.MoveToAttribute("DestY"))
            mapDest.Y = rdr.ReadContentAsFloat();
          if (rdr.MoveToAttribute("Progress"))
            progress = rdr.ReadContentAsFloat();
          AircraftDaemon aircraftDaemon = new AircraftDaemon(c, ComputerLoader.os, name, zero, mapDest, progress);
          c.daemons.Add((Daemon) aircraftDaemon);
        }
        else if (rdr.Name.Equals("LogoCustomConnectDisplayDaemon"))
        {
          string logoImageName = (string) null;
          string titleImageName = (string) null;
          string buttonAlignment = (string) null;
          bool logoShouldClipoverdraw = false;
          if (rdr.MoveToAttribute("logo"))
            logoImageName = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("title"))
            titleImageName = ComputerLoader.filter(rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("overdrawLogo"))
            logoShouldClipoverdraw = rdr.ReadContentAsString().ToLower() == "true";
          if (rdr.MoveToAttribute("buttonAlignment"))
            buttonAlignment = rdr.ReadContentAsString();
          LogoCustomConnectDisplayDaemon connectDisplayDaemon = new LogoCustomConnectDisplayDaemon(c, ComputerLoader.os, logoImageName, titleImageName, logoShouldClipoverdraw, buttonAlignment);
          c.daemons.Add((Daemon) connectDisplayDaemon);
        }
        else if (rdr.Name.Equals("LogoDaemon"))
        {
          string str4;
          string LogoImagePath = str4 = (string) null;
          bool showsTitle = true;
          Color color2 = Color.White;
          if (rdr.MoveToAttribute("LogoImagePath"))
            LogoImagePath = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("TextColor"))
            color2 = Utils.convertStringToColor(rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("Name"))
            str4 = ComputerLoader.filter(rdr.ReadContentAsString());
          if (rdr.MoveToAttribute("ShowsTitle"))
            showsTitle = rdr.ReadContentAsString().ToLower() == "true";
          string str5 = (string) null;
          if (rdr.IsStartElement())
            str5 = rdr.ReadElementContentAsString();
          c.daemons.Add((Daemon) new LogoDaemon(c, ComputerLoader.os, str2, showsTitle, LogoImagePath)
          {
            TextColor = color2,
            BodyText = str5
          });
        }
        else if (rdr.Name.Equals("DLCCredits") || rdr.Name.Equals("CreditsDaemon"))
        {
          string overrideButtonText = (string) null;
          if (rdr.MoveToAttribute("ButtonText"))
            overrideButtonText = ComputerLoader.filter(rdr.ReadContentAsString());
          string overrideTitle = (string) null;
          if (rdr.MoveToAttribute("Title"))
            overrideTitle = ComputerLoader.filter(rdr.ReadContentAsString());
          DLCCreditsDaemon dlcCreditsDaemon = overrideButtonText == null && overrideTitle == null ? new DLCCreditsDaemon(c, ComputerLoader.os) : new DLCCreditsDaemon(c, ComputerLoader.os, overrideTitle, overrideButtonText);
          if (rdr.MoveToAttribute("ConditionalActionSetToRunOnButtonPressPath"))
            dlcCreditsDaemon.ConditionalActionsToLoadOnButtonPress = rdr.ReadContentAsString();
          c.daemons.Add((Daemon) dlcCreditsDaemon);
        }
        else if (rdr.Name.Equals("FastActionHost"))
        {
          FastActionHost fastActionHost = new FastActionHost(c, ComputerLoader.os, str2);
          c.daemons.Add((Daemon) fastActionHost);
        }
        else if (rdr.Name.Equals("eosDevice"))
          EOSComp.AddEOSComp(rdr, c, (object) ComputerLoader.os);
        else if (rdr.Name.Equals("Memory"))
        {
          MemoryContents memoryContents = MemoryContents.Deserialize(rdr);
          c.Memory = memoryContents;
        }
        rdr.Read();
      }
      rdr.Close();
      if (!preventInitDaemons)
        c.initDaemons();
      if (!preventAddingToNetmap)
        ComputerLoader.os.netMap.nodes.Add(c);
      return (object) c;
    }

    private static void DLCCheck(string name)
    {
      if (!DLC1SessionUpgrader.HasDLC1Installed)
        throw new NotSupportedException("LABYRINTHS DLC REQUIRED.\nThe tag " + name + " requires Hacknet Labyrinths to be installed.");
    }

    public static void loadPortsIntoComputer(string portsList, object computer_obj)
    {
      Computer computer = (Computer) computer_obj;
      char[] separator = new char[2]{ ' ', ',' };
      string[] strArray = portsList.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      computer.ports.Clear();
      computer.portsOpen.Clear();
      for (int index = 0; index < strArray.Length; ++index)
      {
        try
        {
          int int32 = Convert.ToInt32(strArray[index]);
          if (PortExploits.portNums.Contains(int32))
          {
            computer.ports.Add(int32);
            computer.portsOpen.Add((byte) 0);
            continue;
          }
        }
        catch (OverflowException ex)
        {
        }
        catch (FormatException ex)
        {
        }
        int num = -1;
        foreach (KeyValuePair<int, string> crack in PortExploits.cracks)
        {
          if (crack.Value.ToLower().Equals(strArray[index].ToLower()))
            num = crack.Key;
        }
        if (num != -1)
        {
          computer.ports.Add(num);
          computer.portsOpen.Add((byte) 0);
        }
      }
    }

    public static object readMission(string filename)
    {
      filename = LocalizedFileLoader.GetLocalizedFilepath(filename);
      XmlReader xmlReader = XmlReader.Create((Stream) File.OpenRead(filename));
      List<MisisonGoal> _goals = new List<MisisonGoal>();
      int val1 = 0;
      string name1 = "";
      string name2 = "";
      int val2 = 0;
      bool flag1 = false;
      bool flag2 = false;
      while (xmlReader.Name != "mission")
      {
        xmlReader.Read();
        if (xmlReader.EOF)
          return (object) null;
      }
      if (xmlReader.MoveToAttribute("activeCheck"))
        flag1 = xmlReader.ReadContentAsBoolean();
      if (xmlReader.MoveToAttribute("shouldIgnoreSenderVerification"))
        flag2 = xmlReader.ReadContentAsBoolean();
      while (xmlReader.Name != "goals" && xmlReader.Name != "generationKeys")
      {
        xmlReader.Read();
        if (xmlReader.EOF)
          throw new FormatException("<goals> tag was not found where it was expected. It's either missing or out of order.");
      }
      if (xmlReader.Name == "generationKeys")
      {
        Dictionary<string, string> keys = new Dictionary<string, string>();
        while (xmlReader.MoveToNextAttribute())
        {
          string name3 = xmlReader.Name;
          string str = xmlReader.Value;
          keys.Add(name3, str);
        }
        int content = (int) xmlReader.MoveToContent();
        string str1 = xmlReader.ReadElementContentAsString();
        if (str1 != null & str1.Length >= 1)
          keys.Add("Data", str1);
        MissionGenerator.setMissionGenerationKeys(keys);
        while (xmlReader.Name != "goals")
          xmlReader.Read();
      }
      xmlReader.Read();
      if (ComputerLoader.MissionPreLoadComplete != null)
        ComputerLoader.MissionPreLoadComplete();
      while (xmlReader.Name != "goals")
      {
        if (xmlReader.Name.Equals("goal"))
        {
          string str1 = "UNKNOWN";
          if (xmlReader.MoveToAttribute("type"))
            str1 = xmlReader.ReadContentAsString();
          try
          {
            if (str1.ToLower().Equals("filedeletion"))
            {
              string str2;
              string path = str2 = "";
              string filename1 = str2;
              string target = str2;
              if (xmlReader.MoveToAttribute("target"))
                target = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("file"))
                filename1 = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("path"))
                path = ComputerLoader.filter(xmlReader.ReadContentAsString());
              Computer comp = ComputerLoader.findComp(target);
              FileDeletionMission fileDeletionMission = new FileDeletionMission(path, filename1, comp != null ? comp.ip : target, ComputerLoader.os);
              _goals.Add((MisisonGoal) fileDeletionMission);
            }
            if (str1.ToLower().Equals("clearfolder"))
            {
              string path;
              string target = path = "";
              if (xmlReader.MoveToAttribute("target"))
                target = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("path"))
                path = ComputerLoader.filter(xmlReader.ReadContentAsString());
              Computer comp = ComputerLoader.findComp(target);
              FileDeleteAllMission deleteAllMission = new FileDeleteAllMission(path, comp.ip, ComputerLoader.os);
              _goals.Add((MisisonGoal) deleteAllMission);
            }
            else if (str1.ToLower().Equals("filedownload"))
            {
              string str2;
              string path = str2 = "";
              string filename1 = str2;
              string computerIP = str2;
              if (xmlReader.MoveToAttribute("target"))
                computerIP = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("file"))
                filename1 = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("path"))
                path = ComputerLoader.filter(xmlReader.ReadContentAsString());
              FileDownloadMission fileDownloadMission = new FileDownloadMission(path, filename1, computerIP, ComputerLoader.os);
              _goals.Add((MisisonGoal) fileDownloadMission);
            }
            else if (str1.ToLower().Equals("filechange"))
            {
              string str2;
              string targetKeyword = str2 = "";
              string path = str2;
              string filename1 = str2;
              string target = str2;
              if (xmlReader.MoveToAttribute("target"))
                target = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("file"))
                filename1 = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("path"))
                path = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("keyword"))
                targetKeyword = ComputerLoader.filter(xmlReader.ReadContentAsString());
              bool isRemoval = false;
              if (xmlReader.MoveToAttribute("removal"))
                isRemoval = xmlReader.ReadContentAsBoolean();
              bool flag3 = false;
              if (xmlReader.MoveToAttribute("caseSensitive"))
                flag3 = xmlReader.ReadContentAsString().ToLower() == "true";
              Computer comp = ComputerLoader.findComp(target);
              if (comp != null)
                _goals.Add((MisisonGoal) new FileChangeMission(path, filename1, comp.ip, targetKeyword, ComputerLoader.os, isRemoval)
                {
                  caseSensitive = flag3
                });
            }
            else if (str1.ToLower().Equals("getadmin"))
            {
              string target = "";
              if (xmlReader.MoveToAttribute("target"))
                target = ComputerLoader.filter(xmlReader.ReadContentAsString());
              GetAdminMission getAdminMission = new GetAdminMission(ComputerLoader.findComp(target).ip, ComputerLoader.os);
              _goals.Add((MisisonGoal) getAdminMission);
            }
            else if (str1.ToLower().Equals("getstring"))
            {
              string targetData = "";
              if (xmlReader.MoveToAttribute("target"))
                targetData = ComputerLoader.filter(xmlReader.ReadContentAsString());
              GetStringMission getStringMission = new GetStringMission(targetData);
              _goals.Add((MisisonGoal) getStringMission);
            }
            else if (str1.ToLower().Equals("delay"))
            {
              float time = 1f;
              if (xmlReader.MoveToAttribute("time"))
                time = xmlReader.ReadContentAsFloat();
              DelayMission delayMission = new DelayMission(time);
              _goals.Add((MisisonGoal) delayMission);
            }
            else if (str1.ToLower().Equals("hasflag"))
            {
              string targetFlagName = "";
              if (xmlReader.MoveToAttribute("target"))
                targetFlagName = ComputerLoader.filter(xmlReader.ReadContentAsString());
              CheckFlagSetMission checkFlagSetMission = new CheckFlagSetMission(targetFlagName, ComputerLoader.os);
              _goals.Add((MisisonGoal) checkFlagSetMission);
            }
            if (str1.ToLower().Equals("fileupload"))
            {
              bool needsDecrypt = false;
              string str2;
              string decryptPass = str2 = "";
              string destToUploadToPath = str2;
              string computerToUploadToIP = str2;
              string path = str2;
              string filename1 = str2;
              string computerWithFileIP = str2;
              if (xmlReader.MoveToAttribute("target"))
                computerWithFileIP = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("file"))
                filename1 = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("path"))
                path = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("destTarget"))
                computerToUploadToIP = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("destPath"))
                destToUploadToPath = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("decryptPass"))
                decryptPass = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("decrypt"))
                needsDecrypt = xmlReader.ReadContentAsBoolean();
              FileUploadMission fileUploadMission = new FileUploadMission(path, filename1, computerWithFileIP, computerToUploadToIP, destToUploadToPath, ComputerLoader.os, needsDecrypt, decryptPass);
              _goals.Add((MisisonGoal) fileUploadMission);
            }
            else if (str1.ToLower().Equals("adddegree"))
            {
              string str2;
              string degreeName = str2 = "";
              string uniName = str2;
              string targetName = str2;
              float desiredGPA = -1f;
              if (xmlReader.MoveToAttribute("owner"))
                targetName = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("degree"))
                degreeName = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("uni"))
                uniName = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("gpa"))
                desiredGPA = xmlReader.ReadContentAsFloat();
              AddDegreeMission addDegreeMission = new AddDegreeMission(targetName, degreeName, uniName, desiredGPA, ComputerLoader.os);
              _goals.Add((MisisonGoal) addDegreeMission);
            }
            else if (str1.ToLower().Equals("wipedegrees"))
            {
              string targetName = "";
              if (xmlReader.MoveToAttribute("owner"))
                targetName = ComputerLoader.filter(xmlReader.ReadContentAsString());
              WipeDegreesMission wipeDegreesMission = new WipeDegreesMission(targetName, ComputerLoader.os);
              _goals.Add((MisisonGoal) wipeDegreesMission);
            }
            else if (str1.ToLower().Equals("removeDeathRowRecord".ToLower()))
            {
              string firstName = "UNKNOWN";
              string lastName = "UNKNOWN";
              if (xmlReader.MoveToAttribute("name"))
              {
                string[] strArray = ComputerLoader.filter(xmlReader.ReadContentAsString()).Split(' ');
                firstName = strArray[0];
                lastName = strArray[1];
              }
              if (xmlReader.MoveToAttribute("fname"))
                firstName = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("lname"))
                lastName = ComputerLoader.filter(xmlReader.ReadContentAsString());
              DeathRowRecordRemovalMission recordRemovalMission = new DeathRowRecordRemovalMission(firstName, lastName, ComputerLoader.os);
              _goals.Add((MisisonGoal) recordRemovalMission);
            }
            else if (str1.ToLower().Equals("modifyDeathRowRecord".ToLower()))
            {
              string firstName = "UNKNOWN";
              string lastName = "UNKNOWN";
              if (xmlReader.MoveToAttribute("name"))
              {
                string[] strArray = ComputerLoader.filter(xmlReader.ReadContentAsString()).Split(' ');
                firstName = strArray[0];
                lastName = strArray[1];
              }
              if (xmlReader.MoveToAttribute("fname"))
                firstName = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("lname"))
                lastName = ComputerLoader.filter(xmlReader.ReadContentAsString());
              int content = (int) xmlReader.MoveToContent();
              string lastWords = xmlReader.ReadElementContentAsString();
              DeathRowRecordModifyMission recordModifyMission = new DeathRowRecordModifyMission(firstName, lastName, lastWords, ComputerLoader.os);
              _goals.Add((MisisonGoal) recordModifyMission);
            }
            else if (str1.ToLower().Equals("sendemail"))
            {
              string proposedEmailSubject;
              string mailRecipient = proposedEmailSubject = "";
              string mailServerID = "jmail";
              if (xmlReader.MoveToAttribute("mailServer"))
                mailServerID = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("recipient"))
                mailRecipient = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("subject"))
                proposedEmailSubject = ComputerLoader.filter(xmlReader.ReadContentAsString());
              SendEmailMission sendEmailMission = new SendEmailMission(mailServerID, mailRecipient, proposedEmailSubject, ComputerLoader.os);
              _goals.Add((MisisonGoal) sendEmailMission);
            }
            else if (str1.ToLower().Equals("databaseentrychange"))
            {
              // ISSUE: variable of the null type
              __Null local;
              string recordName = (string) (local = null);
              string targetValue = (string) local;
              string FieldName = (string) local;
              string operation = (string) local;
              string computerIP = (string) local;
              if (xmlReader.MoveToAttribute("comp"))
                computerIP = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("operation"))
                operation = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("fieldName"))
                FieldName = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("targetValue"))
                targetValue = ComputerLoader.filter(xmlReader.ReadContentAsString());
              if (xmlReader.MoveToAttribute("recordName"))
                recordName = ComputerLoader.filter(xmlReader.ReadContentAsString());
              DatabaseEntryChangeMission entryChangeMission = new DatabaseEntryChangeMission(computerIP, ComputerLoader.os, operation, FieldName, targetValue, recordName);
              _goals.Add((MisisonGoal) entryChangeMission);
            }
            else if (str1.ToLower().Equals("getadminpasswordstring"))
            {
              string target = "";
              if (xmlReader.MoveToAttribute("target"))
                target = ComputerLoader.filter(xmlReader.ReadContentAsString());
              GetAdminPasswordStringMission passwordStringMission = new GetAdminPasswordStringMission(ComputerLoader.findComp(target).ip, ComputerLoader.os);
              _goals.Add((MisisonGoal) passwordStringMission);
            }
          }
          catch (Exception ex)
          {
            if (ex is NullReferenceException)
              throw new FormatException("Error loading mission Goal \"" + str1 + "\"\r\nNullReferenceException - this means something referenced by an ID (probably a computer) or a filename (missions/scripts etc) was not found.");
            throw new FormatException("Error loading mission Goal \"" + str1 + "\"", ex);
          }
        }
        xmlReader.Read();
      }
      while (xmlReader.Name != "nextMission" && xmlReader.Name != "missionEnd" && xmlReader.Name != "missionStart" && !xmlReader.EOF)
        xmlReader.Read();
      if (xmlReader.EOF)
        throw new FormatException("Unexpected end of file looking for nextMission tag in computer.\nYour tags might be out of order!\n");
      if (xmlReader.Name.Equals("missionStart"))
      {
        int num = 1;
        bool flag3 = Settings.IsInExtensionMode;
        if (xmlReader.MoveToAttribute("val"))
          num = xmlReader.ReadContentAsInt();
        if (xmlReader.MoveToAttribute("suppress"))
          flag3 = xmlReader.ReadContentAsBoolean();
        int content = (int) xmlReader.MoveToContent();
        string name3 = xmlReader.ReadElementContentAsString();
        if (flag3)
        {
          name2 = name3;
          val2 = num;
        }
        else
        {
          try
          {
            MissionFunctions.runCommand(num, name3);
          }
          catch (Exception ex)
          {
            Utils.AppendToErrorFile("Mission Start function exception!\r\n" + Utils.GenerateReportFromException(ex));
          }
        }
        xmlReader.Read();
      }
      while (xmlReader.NodeType != XmlNodeType.Element)
        xmlReader.Read();
      if (xmlReader.Name.Equals("missionEnd"))
      {
        int num = 1;
        if (xmlReader.MoveToAttribute("val"))
          num = xmlReader.ReadContentAsInt();
        int content = (int) xmlReader.MoveToContent();
        name1 = xmlReader.ReadElementContentAsString();
        val1 = num;
      }
      bool flag4 = true;
      while (!xmlReader.Name.Equals("nextMission"))
      {
        if (xmlReader.EOF)
          throw new FormatException("Could not find \"nextMission\" tag in mission file! This tag needs to exist!");
        xmlReader.Read();
      }
      if (xmlReader.MoveToAttribute("IsSilent"))
        flag4 = xmlReader.ReadContentAsString().ToLower().Equals("false");
      int content1 = (int) xmlReader.MoveToContent();
      string next = xmlReader.ReadElementContentAsString();
      if (ComputerLoader.os.branchMissions != null)
        ComputerLoader.os.branchMissions.Clear();
      while (xmlReader.Name != "posting" && xmlReader.Name != "email")
      {
        if (xmlReader.Name.Equals("branchMissions"))
        {
          xmlReader.Read();
          List<ActiveMission> activeMissionList = new List<ActiveMission>();
          while (!xmlReader.Name.Equals("branchMissions") || xmlReader.IsStartElement())
          {
            if (xmlReader.Name == "branch")
            {
              int content2 = (int) xmlReader.MoveToContent();
              string str1 = xmlReader.ReadElementContentAsString();
              string str2 = "Content/Missions/";
              if (Settings.IsInExtensionMode)
                str2 = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
              activeMissionList.Add((ActiveMission) ComputerLoader.readMission(str2 + str1));
            }
            xmlReader.Read();
          }
          ComputerLoader.os.branchMissions = activeMissionList;
        }
        xmlReader.Read();
        if (xmlReader.EOF)
          throw new FormatException("email tag not found where it was expected! You may have tags out of order.");
      }
      int num1;
      int num2 = num1 = 0;
      string str3;
      string str4 = str3 = "UNKNOWN";
      string str5 = str3;
      string s1 = str3;
      string str6 = str3;
      string str7 = (string) null;
      while (xmlReader.Name != "posting" && xmlReader.Name != "email")
      {
        if (xmlReader.EOF)
          throw new FormatException("email tag not found where it was expected! You may have tags out of order.");
        xmlReader.Read();
      }
      if (xmlReader.Name.Equals("posting"))
      {
        if (xmlReader.MoveToAttribute("title"))
          s1 = xmlReader.ReadContentAsString();
        if (xmlReader.MoveToAttribute("reqs"))
          str7 = xmlReader.ReadContentAsString();
        if (xmlReader.MoveToAttribute("requiredRank"))
          num2 = xmlReader.ReadContentAsInt();
        if (xmlReader.MoveToAttribute("difficulty"))
          num1 = xmlReader.ReadContentAsInt();
        if (xmlReader.MoveToAttribute("client"))
          str5 = xmlReader.ReadContentAsString();
        if (xmlReader.MoveToAttribute("target"))
          str4 = xmlReader.ReadContentAsString();
        int content2 = (int) xmlReader.MoveToContent();
        string s2 = xmlReader.ReadElementContentAsString();
        s1 = ComputerLoader.filter(s1);
        str6 = ComputerLoader.filter(s2);
      }
      while (xmlReader.Name != "email")
      {
        if (xmlReader.EOF)
          throw new FormatException("email tag was not found!");
        xmlReader.Read();
      }
      while (xmlReader.Name != "sender")
      {
        if (xmlReader.EOF)
          throw new FormatException("sender tag was not found!");
        xmlReader.Read();
      }
      string s3 = xmlReader.ReadElementContentAsString();
      while (xmlReader.Name != "subject")
      {
        if (xmlReader.EOF)
          throw new FormatException("subject tag was not found!");
        xmlReader.Read();
      }
      string s4 = xmlReader.ReadElementContentAsString();
      while (xmlReader.Name != "body")
      {
        if (xmlReader.EOF)
          throw new FormatException("body tag was not found!");
        xmlReader.Read();
      }
      string s5 = xmlReader.ReadElementContentAsString();
      s5.Trim();
      string bod = ComputerLoader.filter(s5);
      string subj = ComputerLoader.filter(s4);
      string sendr = ComputerLoader.filter(s3);
      while (xmlReader.Name != "attachments")
      {
        if (xmlReader.EOF)
          throw new FormatException("attachments tag was not found! A mission must have an attachments tag even if it contains nothing.");
        xmlReader.Read();
      }
      xmlReader.Read();
      List<string> attachments = new List<string>();
      while (xmlReader.Name != "attachments")
      {
        if (xmlReader.Name.Equals("link"))
        {
          string compname = "";
          if (xmlReader.MoveToAttribute("comp"))
            compname = ComputerLoader.filter(xmlReader.ReadContentAsString());
          Computer c = (Computer) null;
          for (int index = 0; index < ComputerLoader.os.netMap.nodes.Count; ++index)
          {
            if (ComputerLoader.os.netMap.nodes[index].idName.Equals(compname))
              c = ComputerLoader.os.netMap.nodes[index];
          }
          if (c != null)
            attachments.Add("link#%#" + c.name + "#%#" + c.ip);
          else
            ComputerLoader.postAllLoadedActions += (Action) (() =>
            {
              for (int index = 0; index < ComputerLoader.os.netMap.nodes.Count; ++index)
              {
                if (ComputerLoader.os.netMap.nodes[index].idName.Equals(compname))
                  c = ComputerLoader.os.netMap.nodes[index];
              }
              if (c == null)
                return;
              attachments.Add("link#%#" + c.name + "#%#" + c.ip);
            });
        }
        if (xmlReader.Name.Equals("account"))
        {
          string str1 = "";
          if (xmlReader.MoveToAttribute("comp"))
            str1 = ComputerLoader.filter(xmlReader.ReadContentAsString());
          Computer computer = (Computer) null;
          for (int index = 0; index < ComputerLoader.os.netMap.nodes.Count; ++index)
          {
            if (ComputerLoader.os.netMap.nodes[index].idName.Equals(str1))
              computer = ComputerLoader.os.netMap.nodes[index];
          }
          string s2;
          string s6 = s2 = "UNKNOWN";
          if (xmlReader.MoveToAttribute("user"))
            s6 = xmlReader.ReadContentAsString();
          if (xmlReader.MoveToAttribute("pass"))
            s2 = xmlReader.ReadContentAsString();
          string str2 = ComputerLoader.filter(s6);
          string str8 = ComputerLoader.filter(s2);
          if (computer != null)
            attachments.Add("account#%#" + computer.name + "#%#" + computer.ip + "#%#" + str2 + "#%#" + str8);
        }
        if (xmlReader.Name.Equals("note"))
        {
          string str1 = "Data";
          if (xmlReader.MoveToAttribute("title"))
            str1 = ComputerLoader.filter(xmlReader.ReadContentAsString());
          int content2 = (int) xmlReader.MoveToContent();
          string str2 = ComputerLoader.filter(xmlReader.ReadElementContentAsString());
          attachments.Add("note#%#" + str1 + "#%#" + str2);
        }
        xmlReader.Read();
        if (xmlReader.EOF)
          throw new FormatException("attachments tag not found where it was expected! You may have tags out of order.");
      }
      MailServer.EMailData _email = new MailServer.EMailData(sendr, bod, subj, attachments);
      ActiveMission activeMission = new ActiveMission(_goals, next, _email);
      activeMission.activeCheck = flag1;
      activeMission.ShouldIgnoreSenderVerification = flag2;
      activeMission.postingBody = str6;
      activeMission.postingTitle = s1;
      activeMission.requiredRank = num2;
      activeMission.difficulty = num1;
      activeMission.client = str5;
      activeMission.target = str4;
      activeMission.reloadGoalsSourceFile = filename;
      if (str7 != null)
        activeMission.postingAcceptFlagRequirements = str7.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
      activeMission.willSendEmail = flag4;
      if (!name1.Equals(""))
        activeMission.addEndFunction(val1, name1);
      if (!name2.Equals(""))
        activeMission.addStartFunction(val2, name2);
      xmlReader.Close();
      return (object) activeMission;
    }

    public static void loadMission(string filename, bool PreventEmail = false)
    {
      ActiveMission activeMission = (ActiveMission) ComputerLoader.readMission(filename);
      ComputerLoader.os.currentMission = activeMission;
      activeMission.sendEmail(ComputerLoader.os);
    }

    public static string filter(string s)
    {
      string str = LocalizedFileLoader.FilterStringForLocalization(MissionGenerationParser.parse(s.Replace("#BINARY#", Computer.generateBinaryString(2000)).Replace("#BINARYSMALL#", Computer.generateBinaryString(800)).Replace("#PLAYERNAME#", ComputerLoader.os.defaultUser.name).Replace("#PLAYER_IP#", ComputerLoader.os.thisComputer.ip).Replace("#PLAYER_ACCOUNT_PASSWORD#", SaveFileManager.LastLoggedInUser.Password).Replace("#RANDOM_IP#", NetworkMap.generateRandomIP()).Replace("#SSH_CRACK#", PortExploits.crackExeData[22]).Replace("#FTP_CRACK#", PortExploits.crackExeData[21]).Replace("#WEB_CRACK#", PortExploits.crackExeData[80]).Replace("#DECYPHER_PROGRAM#", PortExploits.crackExeData[9]).Replace("#DECHEAD_PROGRAM#", PortExploits.crackExeData[10]).Replace("#CLOCK_PROGRAM#", PortExploits.crackExeData[11]).Replace("#MEDICAL_PROGRAM#", PortExploits.crackExeData[104]).Replace("#SMTP_CRACK#", PortExploits.crackExeData[25]).Replace("#SQL_CRACK#", PortExploits.crackExeData[1433]).Replace("#SECURITYTRACER_PROGRAM#", PortExploits.crackExeData[4]).Replace("#HACKNET_EXE#", PortExploits.crackExeData[15]).Replace("#HEXCLOCK_EXE#", PortExploits.crackExeData[16]).Replace("#SEQUENCER_EXE#", PortExploits.crackExeData[17]).Replace("#THEMECHANGER_EXE#", PortExploits.crackExeData[14]).Replace("#EOS_SCANNER_EXE#", PortExploits.crackExeData[13]).Replace("#TRACEKILL_EXE#", PortExploits.crackExeData[12]).Replace("#GREEN_THEME#", ThemeManager.getThemeDataString(OSTheme.HackerGreen)).Replace("#WHITE_THEME#", ThemeManager.getThemeDataString(OSTheme.HacknetWhite)).Replace("#YELLOW_THEME#", ThemeManager.getThemeDataString(OSTheme.HacknetYellow)).Replace("#TEAL_THEME#", ThemeManager.getThemeDataString(OSTheme.HacknetTeal)).Replace("#BASE_THEME#", ThemeManager.getThemeDataString(OSTheme.HacknetBlue)).Replace("#PURPLE_THEME#", ThemeManager.getThemeDataString(OSTheme.HacknetPurple)).Replace("#MINT_THEME#", ThemeManager.getThemeDataString(OSTheme.HacknetMint)).Replace("#PACEMAKER_FW_WORKING#", PortExploits.ValidPacemakerFirmware).Replace("#PACEMAKER_FW_DANGER#", PortExploits.DangerousPacemakerFirmware).Replace("#RTSP_EXE#", PortExploits.crackExeData[554]).Replace("#EXT_SEQUENCER_EXE#", PortExploits.crackExeData[40]).Replace("#SHELL_OPENER_EXE#", PortExploits.crackExeData[41]).Replace("#FTP_FAST_EXE#", PortExploits.crackExeData[211]).Replace("#EXTENSION_FOLDER_PATH#", ExtensionLoader.ActiveExtensionInfo != null ? ExtensionLoader.ActiveExtensionInfo.GetFullFolderPath().Replace("/Content", "/ Content") : "ERROR GETTING PATH").Replace("#PLAYERLOCATION#", "UNKNOWN").Replace("\t", "    ")));
      if (DLC1SessionUpgrader.HasDLC1Installed)
        str = str.Replace("#TORRENT_EXE#", PortExploits.crackExeData[6881]).Replace("#SSL_EXE#", PortExploits.crackExeData[443]).Replace("#KAGUYA_EXE#", PortExploits.crackExeData[31]).Replace("#SIGNAL_SCRAMBLER_EXE#", PortExploits.crackExeData[32]).Replace("#MEM_FORENSICS_EXE#", PortExploits.crackExeData[33]).Replace("#MEM_DUMP_GENERATOR#", PortExploits.crackExeData[34]).Replace("#PACIFIC_EXE#", PortExploits.crackExeData[192]).Replace("#NETMAP_ORGANIZER_EXE#", PortExploits.crackExeData[35]).Replace("#SHELL_CONTROLLER_EXE#", PortExploits.crackExeData[36]).Replace("#NOTES_DUMPER_EXE#", PortExploits.crackExeData[37]).Replace("#CLOCK_V2_EXE#", PortExploits.crackExeData[38]).Replace("#DLC_MUSIC_EXE#", PortExploits.crackExeData[39]).Replace("#GIBSON_IP#", ComputerLoader.os.GibsonIP);
      return str;
    }

    private static Computer findComp(string target)
    {
      for (int index = 0; index < ComputerLoader.os.netMap.nodes.Count; ++index)
      {
        if (ComputerLoader.os.netMap.nodes[index].idName.Equals(target))
          return ComputerLoader.os.netMap.nodes[index];
      }
      return (Computer) null;
    }

    public static object findComputer(string target)
    {
      return (object) ComputerLoader.findComp(target);
    }
  }
}
