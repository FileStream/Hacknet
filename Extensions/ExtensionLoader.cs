// Decompiled with JetBrains decompiler
// Type: Hacknet.Extensions.ExtensionLoader
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Localization;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Xml;

namespace Hacknet.Extensions
{
  public static class ExtensionLoader
  {
    public static ExtensionInfo ActiveExtensionInfo = (ExtensionInfo) null;

    public static void LoadNewExtensionSession(ExtensionInfo info, object os_obj)
    {
      LocaleActivator.ActivateLocale(info.Language, Game1.getSingleton().Content);
      OS os = (OS) os_obj;
      People.ReInitPeopleForExtension();
      if (Directory.Exists(info.FolderPath + "/Nodes"))
        Utils.ActOnAllFilesRevursivley(info.FolderPath + "/Nodes", (Action<string>) (filename =>
        {
          if (!filename.EndsWith(".xml"))
            return;
          if (OS.TestingPassOnly)
          {
            try
            {
              Computer c = Computer.loadFromFile(filename);
              if (c != null)
                ExtensionLoader.CheckAndAssignCoreServer(c, os);
            }
            catch (Exception ex)
            {
              string format = "COMPUTER LOAD ERROR:\nError loading computer \"{0}\"";
              Exception exception = ex;
              string message = string.Format(format, (object) filename);
              for (; exception != null; exception = exception.InnerException)
              {
                string str = string.Format("\r\nError: {0} - {1}", (object) exception.GetType().Name, (object) exception.Message);
                message += str;
              }
              throw new FormatException(message, ex);
            }
          }
          else
          {
            Computer c = Computer.loadFromFile(filename);
            if (c != null)
              ExtensionLoader.CheckAndAssignCoreServer(c, os);
          }
        }));
      if (ComputerLoader.postAllLoadedActions != null)
        ComputerLoader.postAllLoadedActions();
      if (Programs.getComputer(os, "jmail") == null)
      {
        Computer c = new Computer("JMail Email Server", NetworkMap.generateRandomIP(), new Vector2(0.8f, 0.2f), 6, (byte) 1, os);
        c.idName = "jmail";
        c.daemons.Add((Daemon) new MailServer(c, "JMail", os));
        MailServer.shouldGenerateJunk = false;
        c.users.Add(new UserDetail(os.defaultUser.name, "mailpassword", (byte) 2));
        c.initDaemons();
        os.netMap.mailServer = c;
        os.netMap.nodes.Add(c);
      }
      for (int index = 0; index < info.StartingVisibleNodes.Length; ++index)
      {
        Computer computer = Programs.getComputer(os, info.StartingVisibleNodes[index]);
        if (computer != null)
          os.netMap.discoverNode(computer);
      }
      for (int index = 0; index < info.FactionDescriptorPaths.Count; ++index)
      {
        string path = info.FolderPath + "/" + info.FactionDescriptorPaths[index];
        using (FileStream fileStream = File.OpenRead(path))
        {
          try
          {
            Faction faction = Faction.loadFromSave(XmlReader.Create((Stream) fileStream));
            os.allFactions.factions.Add(faction.idName, faction);
          }
          catch (Exception ex)
          {
            throw new FormatException("Error loading Faction: " + path, ex);
          }
        }
      }
      OSTheme theme = OSTheme.Custom;
      bool flag = false;
      foreach (object obj in Enum.GetValues(typeof (OSTheme)))
      {
        if (obj.ToString().ToLower() == info.Theme)
        {
          theme = (OSTheme) obj;
          flag = true;
        }
      }
      if (!flag)
      {
        if (File.Exists(info.FolderPath + "/" + info.Theme))
        {
          ThemeManager.setThemeOnComputer((object) os.thisComputer, info.Theme);
          ThemeManager.switchTheme((object) os, info.Theme);
        }
      }
      else
      {
        ThemeManager.setThemeOnComputer((object) os.thisComputer, theme);
        ThemeManager.switchTheme((object) os, theme);
      }
      ExtensionLoader.LoadExtensionStartTrackAsCurrentSong(info);
      if (info.StartingActionsPath != null)
        RunnableConditionalActions.LoadIntoOS(info.StartingActionsPath, (object) os);
      if (info.StartingMissionPath == null || info.StartsWithTutorial || info.HasIntroStartup)
        return;
      ExtensionLoader.SendStartingEmailForActiveExtensionNextFrame((object) os);
    }

    public static void LoadExtensionStartTrackAsCurrentSong(ExtensionInfo info)
    {
      if (info.IntroStartupSong == null)
        return;
      string introStartupSong = info.IntroStartupSong;
      string str1 = introStartupSong;
      if (!introStartupSong.EndsWith(".ogg"))
        str1 = introStartupSong + ".ogg";
      string path = info.FolderPath + "/" + str1;
      MusicManager.stop();
      if (File.Exists(path))
      {
        MusicManager.loadAsCurrentSong(path.StartsWith("Extensions") ? "../" + path : path);
      }
      else
      {
        string str2 = "Music/" + str1;
        if (File.Exists("Content/" + str2))
          MusicManager.loadAsCurrentSong(str2.Replace(".ogg", ""));
        else if (File.Exists("Content/DLC/" + str2))
          MusicManager.loadAsCurrentSong("DLC/" + str2.Replace(".ogg", ""));
      }
      MusicManager.stop();
    }

    public static void SendStartingEmailForActiveExtensionNextFrame(object os_obj)
    {
      OS os = (OS) os_obj;
      if (!string.IsNullOrWhiteSpace(ExtensionLoader.ActiveExtensionInfo.StartingMissionPath))
        os.delayer.Post(ActionDelayer.NextTick(), (Action) (() =>
        {
          ActiveMission activeMission = (ActiveMission) ComputerLoader.readMission(ExtensionLoader.ActiveExtensionInfo.FolderPath + "/" + ExtensionLoader.ActiveExtensionInfo.StartingMissionPath);
          os.currentMission = activeMission;
          activeMission.sendEmail(os);
          activeMission.ActivateSuppressedStartFunctionIfPresent();
          os.saveGame();
        }));
      if (os.Flags.HasFlag("ExtensionFirstBootComplete"))
        return;
      os.Flags.AddFlag("ExtensionFirstBootComplete");
    }

    internal static void CheckAndAssignCoreServer(Computer c, OS os)
    {
      if (c.idName.ToLower() == "academic" && c.getDaemon(typeof (AcademicDatabaseDaemon)) != null)
        os.netMap.academicDatabase = c;
      if (c.idName.ToLower() == "jmail" && c.getDaemon(typeof (MailServer)) != null)
        os.netMap.mailServer = c;
      if (c.idName.ToLower() == "ispComp")
      {
        if (c.getDaemon(typeof (ISPDaemon)) == null)
          throw new InvalidOperationException("ispComp Does not have an ISP Daemon on it!");
        for (int index = 0; index < os.netMap.nodes.Count; ++index)
        {
          if (os.netMap.nodes[index].idName == "ispComp" && os.netMap.nodes[index] != c)
          {
            os.netMap.nodes[index].idName = "ispOriginalComp";
            os.netMap.nodes[index].ip = NetworkMap.generateRandomIP();
            break;
          }
        }
      }
      if (!(c.idName.ToLower() == "playercomp"))
        return;
      os.netMap.nodes.Remove(os.thisComputer);
      os.thisComputer = c;
      c.adminIP = c.ip;
      os.netMap.nodes.Remove(c);
      os.netMap.nodes.Insert(0, c);
      if (!os.netMap.visibleNodes.Contains(0))
        os.netMap.visibleNodes.Add(0);
    }

    public static void ReloadExtensionNodes(object osobj)
    {
      OS os = (OS) osobj;
      ExtensionInfo activeExtensionInfo = ExtensionLoader.ActiveExtensionInfo;
      if (!Directory.Exists(activeExtensionInfo.FolderPath + "/Nodes"))
        return;
      Utils.ActOnAllFilesRevursivley(activeExtensionInfo.FolderPath + "/Nodes", (Action<string>) (filename =>
      {
        if (!filename.EndsWith(".xml"))
          return;
        if (OS.TestingPassOnly)
        {
          try
          {
            Computer c = Computer.loadFromFile(filename);
            if (c != null)
              ExtensionLoader.CheckAndAssignCoreServer(c, os);
          }
          catch (Exception ex)
          {
            throw new FormatException(string.Format("COMPUTER LOAD ERROR:\nError loading computer \"{0}\"\nError: {1} - {2}", (object) filename, (object) ex.GetType().Name, (object) ex.Message), ex);
          }
        }
        else
        {
          Computer c = (Computer) ComputerLoader.loadComputer(filename, true, false);
          for (int index = 0; index < os.netMap.nodes.Count; ++index)
          {
            Computer node = os.netMap.nodes[index];
            if (node.idName == c.idName)
            {
              c.location = node.location;
              c.adminIP = node.adminIP;
              c.ip = node.ip;
              c.highlightFlashTime = 1f;
              os.netMap.nodes[index] = c;
              break;
            }
          }
          if (c != null)
            ExtensionLoader.CheckAndAssignCoreServer(c, os);
        }
      }));
    }
  }
}
