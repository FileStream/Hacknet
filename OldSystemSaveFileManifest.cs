// Decompiled with JetBrains decompiler
// Type: Hacknet.OldSystemSaveFileManifest
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.PlatformAPI.Storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hacknet
{
  public static class OldSystemSaveFileManifest
  {
    public static SaveAccountData LastLoggedInUser = new SaveAccountData() { Username = (string) null };
    public static List<SaveAccountData> Accounts = new List<SaveAccountData>();
    public const string FILENAME = "Accounts.txt";
    public const string File_User_Name = "_accountsMeta";
    private const string AccountsDelimiter = "\r\n%------%";

    public static void Load()
    {
      try
      {
        string str1 = (string) null;
        Stream saveReadStream = RemoteSaveStorage.GetSaveReadStream("_accountsMeta", false);
        if (saveReadStream == null)
        {
          if (!SettingsLoader.hasEverSaved)
            ;
          if (RemoteSaveStorage.FileExists("_accountsMeta", true))
          {
            saveReadStream = RemoteSaveStorage.GetSaveReadStream("_accountsMeta", true);
            if (SettingsLoader.hasEverSaved)
              MainMenu.AccumErrors = saveReadStream == null ? MainMenu.AccumErrors + "Also failed loading backup\n" : MainMenu.AccumErrors + "Loaded Local saves backup...\n";
          }
        }
        else
          str1 = new StreamReader(saveReadStream).ReadToEnd();
        if (saveReadStream != null)
        {
          saveReadStream.Flush();
          saveReadStream.Dispose();
        }
        if (str1 == null)
          return;
        string[] strArray = str1.Split(new string[1]{ "\r\n%------%" }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length <= 1)
          throw new InvalidOperationException();
        string str2 = strArray[0];
        OldSystemSaveFileManifest.Accounts.Clear();
        for (int index = 1; index < strArray.Length; ++index)
        {
          SaveAccountData fromString = SaveAccountData.ParseFromString(strArray[index]);
          OldSystemSaveFileManifest.Accounts.Add(fromString);
          if (fromString.Username == str2)
            OldSystemSaveFileManifest.LastLoggedInUser = fromString;
        }
      }
      catch (Exception ex)
      {
      }
    }

    public static void Save()
    {
      string saveData = OldSystemSaveFileManifest.LastLoggedInUser.Username + "\r\n%------%";
      for (int index = 0; index < OldSystemSaveFileManifest.Accounts.Count; ++index)
        saveData = saveData + OldSystemSaveFileManifest.Accounts[index].Serialize() + "\r\n%------%";
      RemoteSaveStorage.WriteSaveData(saveData, "_accountsMeta", false);
      if (SettingsLoader.hasEverSaved)
        return;
      SettingsLoader.hasEverSaved = true;
      SettingsLoader.writeStatusFile();
    }

    public static string GetFilePathForLogin(string username, string pass)
    {
      for (int index = 0; index < OldSystemSaveFileManifest.Accounts.Count; ++index)
      {
        if (OldSystemSaveFileManifest.Accounts[index].Username.ToLower() == username.ToLower() && (OldSystemSaveFileManifest.Accounts[index].Password == pass || pass == "buffalo"))
        {
          OldSystemSaveFileManifest.LastLoggedInUser = OldSystemSaveFileManifest.Accounts[index];
          return OldSystemSaveFileManifest.Accounts[index].FileUsername;
        }
      }
      return (string) null;
    }

    public static bool CanCreateAccountForName(string username)
    {
      string str = FileSanitiser.purifyStringForDisplay(username).Replace("_", "-").Trim();
      if (str.Length <= 0)
        return false;
      for (int index = 0; index < OldSystemSaveFileManifest.Accounts.Count; ++index)
      {
        if (OldSystemSaveFileManifest.Accounts[index].FileUsername == str || OldSystemSaveFileManifest.Accounts[index].Username == username)
          return false;
      }
      return true;
    }

    public static string AddUserAndGetFilename(string username, string password)
    {
      string str = FileSanitiser.purifyStringForDisplay(username).Replace("_", "-").Trim();
      if (str.Length <= 0)
        return (string) null;
      for (int index = 0; index < OldSystemSaveFileManifest.Accounts.Count; ++index)
      {
        if (OldSystemSaveFileManifest.Accounts[index].FileUsername == str || OldSystemSaveFileManifest.Accounts[index].Username == username)
          return (string) null;
      }
      SaveAccountData saveAccountData = new SaveAccountData() { Username = username, Password = password, FileUsername = str };
      OldSystemSaveFileManifest.Accounts.Add(saveAccountData);
      OldSystemSaveFileManifest.LastLoggedInUser = saveAccountData;
      OldSystemSaveFileManifest.Save();
      return str;
    }
  }
}
