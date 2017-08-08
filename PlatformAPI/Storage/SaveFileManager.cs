// Decompiled with JetBrains decompiler
// Type: Hacknet.PlatformAPI.Storage.SaveFileManager
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hacknet.PlatformAPI.Storage
{
  public static class SaveFileManager
  {
    public static object CurrentlySaving = (object) false;
    public static List<IStorageMethod> StorageMethods = new List<IStorageMethod>();
    private static bool HasSentErrorReport = false;

    public static List<SaveAccountData> Accounts
    {
      get
      {
        return SaveFileManager.StorageMethods[0].GetSaveManifest().Accounts;
      }
    }

    public static SaveAccountData LastLoggedInUser
    {
      get
      {
        return SaveFileManager.StorageMethods[0].GetSaveManifest().LastLoggedInUser;
      }
    }

    public static bool HasSaves
    {
      get
      {
        return SaveFileManager.StorageMethods[0].GetSaveManifest().Accounts.Count > 0;
      }
    }

    public static void Init(bool needsOtherSourcesUpdate = true)
    {
      SaveFileManager.StorageMethods.Clear();
      SaveFileManager.StorageMethods.Add((IStorageMethod) new LocalDocumentsStorageMethod());
      if (PlatformAPISettings.Running && PlatformAPISettings.RemoteStorageRunning)
        SaveFileManager.StorageMethods.Add((IStorageMethod) new SteamCloudStorageMethod());
      for (int index = 0; index < SaveFileManager.StorageMethods.Count; ++index)
        SaveFileManager.StorageMethods[index].Load();
      if (!needsOtherSourcesUpdate)
        return;
      SaveFileManager.UpdateStorageMethodsFromSourcesToLatest();
    }

    public static void UpdateStorageMethodsFromSourcesToLatest()
    {
      int num = -1;
      for (int index = 0; index < SaveFileManager.StorageMethods.Count; ++index)
      {
        if (SaveFileManager.StorageMethods[index].DidDeserialize)
        {
          num = index;
          break;
        }
      }
      if (num == -1)
      {
        SaveFileManifest saveFileManifest1 = SaveFileManager.ReadOldSysemSteamCloudSaveManifest();
        SaveFileManifest saveFileManifest2 = SaveFileManager.ReadOldSysemLocalSaveManifest();
        if (saveFileManifest1 == null && saveFileManifest2 == null)
        {
          Console.WriteLine("New Game Detected!");
        }
        else
        {
          SaveFileManifest manifest = new SaveFileManifest();
          DateTime utcNow = DateTime.UtcNow;
          bool flag = false;
          if (saveFileManifest2 != null)
          {
            for (int index = 0; index < saveFileManifest2.Accounts.Count; ++index)
            {
              string str = RemoteSaveStorage.Standalone_FolderPath + RemoteSaveStorage.BASE_SAVE_FILE_NAME + saveFileManifest2.Accounts[index].FileUsername + RemoteSaveStorage.SAVE_FILE_EXT;
              if (File.Exists(str))
              {
                FileInfo fileInfo = new FileInfo(str);
                if (fileInfo.Length > 100L)
                  manifest.AddUser(saveFileManifest2.Accounts[index].Username, saveFileManifest2.Accounts[index].Password, fileInfo.LastWriteTimeUtc, str);
              }
            }
            for (int index = 0; index < manifest.Accounts.Count; ++index)
            {
              if (saveFileManifest2.LastLoggedInUser.Username == manifest.Accounts[index].Username)
                manifest.LastLoggedInUser = manifest.Accounts[index];
            }
            if (manifest.LastLoggedInUser.Username == null)
            {
              int index = manifest.Accounts.Count - 1;
              if (index >= 0)
              {
                manifest.LastLoggedInUser = manifest.Accounts[index];
                flag = true;
              }
            }
          }
          if (saveFileManifest1 != null)
          {
            for (int index1 = 0; index1 < saveFileManifest1.Accounts.Count; ++index1)
            {
              try
              {
                string str = RemoteSaveStorage.BASE_SAVE_FILE_NAME + saveFileManifest1.Accounts[index1].FileUsername + RemoteSaveStorage.SAVE_FILE_EXT;
                if (SteamRemoteStorage.FileExists(str))
                {
                  if (SteamRemoteStorage.GetFileSize(str) > 100)
                  {
                    int index2 = -1;
                    for (int index3 = 0; index3 < manifest.Accounts.Count; ++index3)
                    {
                      if (manifest.Accounts[index3].Username == saveFileManifest1.Accounts[index1].Username)
                      {
                        index2 = index3;
                        break;
                      }
                    }
                    if (index2 >= 0)
                    {
                      if (new DateTime(1970, 1, 1, 0, 0, 0) + TimeSpan.FromSeconds((double) SteamRemoteStorage.GetFileTimestamp(str)) > manifest.Accounts[index2].LastWriteTime)
                      {
                        Stream saveReadStream = RemoteSaveStorage.GetSaveReadStream(str, false);
                        if (saveReadStream != null)
                        {
                          string end = new StreamReader(saveReadStream).ReadToEnd();
                          saveReadStream.Close();
                          saveReadStream.Dispose();
                          RemoteSaveStorage.WriteSaveData(end, str, true);
                        }
                        else
                          MainMenu.AccumErrors = MainMenu.AccumErrors + "WARNING: Cloud account " + saveFileManifest1.Accounts[index1].Username + " failed to convert over to new secure account system.\nRestarting your computer and Hacknet may resolve this issue.";
                      }
                    }
                    else
                    {
                      string filepath = RemoteSaveStorage.Standalone_FolderPath + RemoteSaveStorage.BASE_SAVE_FILE_NAME + saveFileManifest1.Accounts[index1].Username + RemoteSaveStorage.SAVE_FILE_EXT;
                      Stream saveReadStream = RemoteSaveStorage.GetSaveReadStream(saveFileManifest1.Accounts[index1].Username, false);
                      if (saveReadStream != null)
                      {
                        RemoteSaveStorage.WriteSaveData(Utils.ReadEntireContentsOfStream(saveReadStream), saveFileManifest1.Accounts[index1].Username, true);
                        manifest.AddUser(saveFileManifest1.Accounts[index1].Username, saveFileManifest1.Accounts[index1].Password, utcNow, filepath);
                      }
                      else
                        MainMenu.AccumErrors = MainMenu.AccumErrors + "WARNING: Cloud account " + saveFileManifest1.Accounts[index1].Username + " failed to convert over to new secure account system.\nRestarting your computer and Hacknet may resolve this issue.";
                    }
                  }
                }
              }
              catch (Exception ex)
              {
                MainMenu.AccumErrors = MainMenu.AccumErrors + "WARNING: Error upgrading account #" + (object) (index1 + 1) + ":\r\n" + Utils.GenerateReportFromException(ex);
                if (!SaveFileManager.HasSentErrorReport)
                {
                  string extraData = "cloudAccounts: " + (saveFileManifest1 == null ? "NULL" : string.Concat((object) saveFileManifest1.Accounts.Count)) + " vs localAccounts " + (saveFileManifest2 == null ? "NULL" : string.Concat((object) saveFileManifest2.Accounts.Count));
                  Utils.SendThreadedErrorReport(ex, "AccountUpgrade_Error", extraData);
                  SaveFileManager.HasSentErrorReport = true;
                }
              }
            }
            if (flag)
            {
              for (int index = 0; index < manifest.Accounts.Count; ++index)
              {
                if (saveFileManifest2.LastLoggedInUser.Username == manifest.Accounts[index].Username)
                  manifest.LastLoggedInUser = manifest.Accounts[index];
              }
            }
          }
          OldSystemStorageMethod systemStorageMethod = new OldSystemStorageMethod(manifest);
          for (int index = 0; index < SaveFileManager.StorageMethods.Count; ++index)
            SaveFileManager.StorageMethods[index].UpdateDataFromOtherManager((IStorageMethod) systemStorageMethod);
        }
      }
      else
      {
        for (int index = 1; index < SaveFileManager.StorageMethods.Count; ++index)
          SaveFileManager.StorageMethods[0].UpdateDataFromOtherManager(SaveFileManager.StorageMethods[index]);
      }
    }

    private static SaveFileManifest ReadOldSysemSteamCloudSaveManifest()
    {
      Stream saveReadStream = RemoteSaveStorage.GetSaveReadStream("_accountsMeta", false);
      if (saveReadStream == null)
        return (SaveFileManifest) null;
      string end = new StreamReader(saveReadStream).ReadToEnd();
      saveReadStream.Close();
      saveReadStream.Dispose();
      return SaveFileManifest.DeserializeSafe(end);
    }

    private static SaveFileManifest ReadOldSysemLocalSaveManifest()
    {
      Stream saveReadStream = RemoteSaveStorage.GetSaveReadStream("_accountsMeta", true);
      if (saveReadStream == null)
        return (SaveFileManifest) null;
      string end = new StreamReader(saveReadStream).ReadToEnd();
      saveReadStream.Close();
      saveReadStream.Dispose();
      return SaveFileManifest.DeserializeSafe(end);
    }

    public static string GetSaveFileNameForUsername(string username)
    {
      return "save_" + FileSanitiser.purifyStringForDisplay(username).Replace("_", "-").Trim() + ".xml";
    }

    public static Stream GetSaveReadStream(string playerID)
    {
      return SaveFileManager.StorageMethods[0].GetFileReadStream(playerID);
    }

    public static void WriteSaveData(string saveData, string playerID)
    {
      DateTime utcNow = DateTime.UtcNow;
      for (int index = 0; index < SaveFileManager.StorageMethods.Count; ++index)
      {
        try
        {
          SaveFileManager.StorageMethods[index].WriteSaveFileData(SaveFileManager.GetSaveFileNameForUsername(playerID), playerID, saveData, utcNow);
        }
        catch (Exception ex)
        {
          Utils.AppendToErrorFile("Error writing save data for user : " + playerID + "\r\n" + Utils.GenerateReportFromException(ex));
        }
      }
      if (SettingsLoader.hasEverSaved)
        return;
      SettingsLoader.hasEverSaved = true;
      SettingsLoader.writeStatusFile();
    }

    public static bool AddUser(string username, string pass)
    {
      DateTime utcNow = DateTime.UtcNow;
      for (int index = 0; index < SaveFileManager.StorageMethods.Count; ++index)
      {
        try
        {
          if (!SaveFileManager.StorageMethods[index].GetSaveManifest().AddUser(username, pass, utcNow, SaveFileManager.GetSaveFileNameForUsername(username)))
            return false;
        }
        catch (Exception ex)
        {
          Utils.AppendToErrorFile("Error creating user : " + username + "\r\n" + Utils.GenerateReportFromException(ex));
          return false;
        }
      }
      return true;
    }

    public static string GetFilePathForLogin(string username, string pass)
    {
      for (int index = 0; index < SaveFileManager.Accounts.Count; ++index)
      {
        if (SaveFileManager.Accounts[index].Username.ToLower() == username.ToLower() && (SaveFileManager.Accounts[index].Password == pass || pass == "buffalo" || string.IsNullOrEmpty(SaveFileManager.Accounts[index].Password)))
        {
          SaveFileManager.StorageMethods[0].GetSaveManifest().LastLoggedInUser = SaveFileManager.Accounts[index];
          return SaveFileManager.Accounts[index].FileUsername;
        }
      }
      return (string) null;
    }

    public static bool CanCreateAccountForName(string username)
    {
      string str = FileSanitiser.purifyStringForDisplay(username).Replace("_", "-").Trim();
      if (str.Length <= 0)
        return false;
      for (int index = 0; index < SaveFileManager.Accounts.Count; ++index)
      {
        if (SaveFileManager.Accounts[index].FileUsername == str || SaveFileManager.Accounts[index].Username == username)
          return false;
      }
      return true;
    }

    public static void DeleteUser(string username)
    {
      for (int index1 = 0; index1 < SaveFileManager.StorageMethods.Count; ++index1)
      {
        SaveFileManifest saveManifest = SaveFileManager.StorageMethods[index1].GetSaveManifest();
        for (int index2 = 0; index2 < saveManifest.Accounts.Count; ++index2)
        {
          if (saveManifest.Accounts[index2].Username == username)
          {
            saveManifest.Accounts.RemoveAt(index2);
            --index2;
          }
        }
        if (saveManifest.LastLoggedInUser.Username == username)
          saveManifest.LastLoggedInUser = saveManifest.Accounts.Count <= 0 ? new SaveAccountData() : saveManifest.Accounts[0];
        saveManifest.Save(SaveFileManager.StorageMethods[index1], true);
      }
    }
  }
}
