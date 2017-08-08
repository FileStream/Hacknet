// Decompiled with JetBrains decompiler
// Type: Hacknet.Extensions.ExtensionInfo
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Hacknet.Extensions
{
  public class ExtensionInfo
  {
    public string Description = "";
    public bool AllowSave = true;
    public bool StartsWithTutorial = false;
    public bool HasIntroStartup = true;
    public string Theme = "HacknetBlue";
    public string IntroStartupSong = (string) null;
    public float IntroStartupSongDelay = 0.0f;
    public string[] StartingVisibleNodes = new string[0];
    public List<string> FactionDescriptorPaths = new List<string>();
    public float SequencerSpinUpTime = 17f;
    public byte WorkshopVisibility = 2;
    public const string INFO_FILENAME = "ExtensionInfo.xml";
    public const string LOGO_FILENAME = "Logo";
    public const string NODES_FOLDER = "/Nodes";
    public const string MISSIONS_FOLDER = "/Missions";
    public string Name;
    public string Language;
    public string FolderPath;
    public string StartingMissionPath;
    public string StartingActionsPath;
    public Texture2D LogoImage;
    public string SequencerTargetID;
    public string SequencerFlagRequiredForStart;
    public string ActionsToRunOnSequencerStart;
    public string WorkshopDescription;
    public string WorkshopLanguage;
    public string WorkshopTags;
    public string WorkshopPreviewImagePath;
    public string WorkshopPublishID;

    public string GetFullFolderPath()
    {
      return Path.Combine(Directory.GetCurrentDirectory(), this.FolderPath).Replace("\\", "/");
    }

    public static ExtensionInfo ReadExtensionInfo(string folderpath)
    {
      if (!ExtensionInfo.ExtensionExists(folderpath))
        throw new FileNotFoundException("No extension info exists for folder " + folderpath);
      ExtensionInfo extensionInfo = new ExtensionInfo();
      extensionInfo.FolderPath = folderpath;
      extensionInfo.Language = "en-us";
      using (FileStream fileStream = File.OpenRead(folderpath + "/ExtensionInfo.xml"))
      {
        XmlReader xmlReader = XmlReader.Create((Stream) fileStream);
        while (!xmlReader.EOF)
        {
          if (xmlReader.Name == "Name")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.Name = Utils.CleanStringToLanguageRenderable(xmlReader.ReadElementContentAsString());
          }
          if (xmlReader.Name == "Language")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.Language = xmlReader.ReadElementContentAsString();
          }
          if (xmlReader.Name == "AllowSaves")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.AllowSave = xmlReader.ReadElementContentAsBoolean();
          }
          if (xmlReader.Name == "StartingVisibleNodes")
          {
            int content = (int) xmlReader.MoveToContent();
            string str = xmlReader.ReadElementContentAsString();
            extensionInfo.StartingVisibleNodes = str.Split(new char[6]
            {
              ',',
              ' ',
              '\t',
              '\n',
              '\r',
              '/'
            }, StringSplitOptions.RemoveEmptyEntries);
          }
          if (xmlReader.Name == "StartingMission")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.StartingMissionPath = xmlReader.ReadElementContentAsString();
            if (extensionInfo.StartingMissionPath == "NONE")
              extensionInfo.StartingMissionPath = (string) null;
          }
          if (xmlReader.Name == "StartingActions")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.StartingActionsPath = xmlReader.ReadElementContentAsString();
            if (extensionInfo.StartingActionsPath == "NONE")
              extensionInfo.StartingActionsPath = (string) null;
          }
          if (xmlReader.Name == "Description")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.Description = Utils.CleanFilterStringToRenderable(xmlReader.ReadElementContentAsString());
          }
          if (xmlReader.Name == "Faction")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.FactionDescriptorPaths.Add(xmlReader.ReadElementContentAsString());
          }
          if (xmlReader.Name == "StartsWithTutorial")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.StartsWithTutorial = xmlReader.ReadElementContentAsString().ToLower() == "true";
          }
          if (xmlReader.Name == "HasIntroStartup")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.HasIntroStartup = xmlReader.ReadElementContentAsString().ToLower() == "true";
          }
          if (xmlReader.Name == "StartingTheme")
          {
            int content = (int) xmlReader.MoveToContent();
            string lower = xmlReader.ReadElementContentAsString().ToLower();
            extensionInfo.Theme = lower;
          }
          if (xmlReader.Name == "IntroStartupSong")
          {
            int content = (int) xmlReader.MoveToContent();
            string str = xmlReader.ReadElementContentAsString();
            extensionInfo.IntroStartupSong = str;
          }
          if (xmlReader.Name == "IntroStartupSongDelay")
          {
            int content = (int) xmlReader.MoveToContent();
            float num = xmlReader.ReadElementContentAsFloat();
            extensionInfo.IntroStartupSongDelay = num;
          }
          if (xmlReader.Name == "SequencerSpinUpTime")
          {
            int content = (int) xmlReader.MoveToContent();
            float num = xmlReader.ReadElementContentAsFloat();
            extensionInfo.SequencerSpinUpTime = num;
          }
          if (xmlReader.Name == "ActionsToRunOnSequencerStart")
          {
            int content = (int) xmlReader.MoveToContent();
            string str = xmlReader.ReadElementContentAsString();
            extensionInfo.ActionsToRunOnSequencerStart = str;
          }
          if (xmlReader.Name == "SequencerFlagRequiredForStart")
          {
            int content = (int) xmlReader.MoveToContent();
            string str = xmlReader.ReadElementContentAsString();
            extensionInfo.SequencerFlagRequiredForStart = str;
          }
          if (xmlReader.Name == "SequencerTargetID")
          {
            int content = (int) xmlReader.MoveToContent();
            string str = xmlReader.ReadElementContentAsString();
            extensionInfo.SequencerTargetID = str;
          }
          if (xmlReader.Name == "WorkshopDescription")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.WorkshopDescription = xmlReader.ReadElementContentAsString();
          }
          if (xmlReader.Name == "WorkshopVisibility")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.WorkshopVisibility = (byte) xmlReader.ReadElementContentAsInt();
          }
          if (xmlReader.Name == "WorkshopTags")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.WorkshopTags = xmlReader.ReadElementContentAsString();
          }
          if (xmlReader.Name == "WorkshopPreviewImagePath")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.WorkshopPreviewImagePath = xmlReader.ReadElementContentAsString();
          }
          if (xmlReader.Name == "WorkshopLanguage")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.WorkshopLanguage = xmlReader.ReadElementContentAsString();
          }
          if (xmlReader.Name == "WorkshopPublishID")
          {
            int content = (int) xmlReader.MoveToContent();
            extensionInfo.WorkshopPublishID = xmlReader.ReadElementContentAsString();
          }
          xmlReader.Read();
        }
      }
      string path = folderpath + "/Logo";
      bool flag = false;
      if (File.Exists(path + ".png"))
      {
        path += ".png";
        flag = true;
      }
      else if (File.Exists(path + ".jpg"))
      {
        path += ".png";
        flag = true;
      }
      if (flag)
      {
        using (FileStream fileStream = File.OpenRead(path))
          extensionInfo.LogoImage = Texture2D.FromStream(Game1.getSingleton().GraphicsDevice, (Stream) fileStream);
      }
      return extensionInfo;
    }

    public static void VerifyExtensionInfo(ExtensionInfo info)
    {
      if (info.Name == null)
        throw new NullReferenceException("Extension must have a Name");
      if (string.IsNullOrWhiteSpace(info.GetFoldersafeName()))
        throw new NullReferenceException("Extension names require at least one letter or number in them");
      if (!Directory.Exists(info.FolderPath))
        throw new DirectoryNotFoundException("Directory folderpath could not be found");
      if (!File.Exists(info.FolderPath + "/" + info.StartingMissionPath))
        throw new FileNotFoundException("Starting Mission File does not exist!");
      if (info.IntroStartupSong == null)
        return;
      string introStartupSong = info.IntroStartupSong;
      string str1 = introStartupSong;
      if (!introStartupSong.EndsWith(".ogg"))
        str1 = introStartupSong + ".ogg";
      if (File.Exists(info.FolderPath + "/" + str1))
      {
        MusicManager.loadAsCurrentSongUnsafe((info.FolderPath.StartsWith("Extensions") ? "../" : "") + info.FolderPath + "/" + introStartupSong);
      }
      else
      {
        string str2 = "Music/" + str1;
        if (File.Exists("Content/" + str2))
          MusicManager.loadAsCurrentSong(str2.Replace(".ogg", ""));
      }
    }

    public static bool ExtensionExists(string folderpath)
    {
      return File.Exists(folderpath + "/ExtensionInfo.xml");
    }

    public string GetFoldersafeName()
    {
      string str = this.Name.Replace(" ", "_");
      foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
        str = str.Replace(string.Concat((object) invalidFileNameChar), "");
      return str;
    }
  }
}
