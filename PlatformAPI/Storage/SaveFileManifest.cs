// Decompiled with JetBrains decompiler
// Type: Hacknet.PlatformAPI.Storage.SaveFileManifest
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.IO;

namespace Hacknet.PlatformAPI.Storage
{
  public class SaveFileManifest
  {
    public SaveAccountData LastLoggedInUser = new SaveAccountData() { Username = (string) null };
    public List<SaveAccountData> Accounts = new List<SaveAccountData>();
    public const string FILENAME = "Accounts.txt";
    private const string AccountsDelimiter = "\r\n%------%";

    public static SaveFileManifest Deserialize(IStorageMethod storage)
    {
      if (!storage.FileExists("Accounts.txt"))
        return (SaveFileManifest) null;
      SaveFileManifest saveFileManifest = (SaveFileManifest) null;
      using (Stream fileReadStream = storage.GetFileReadStream("Accounts.txt"))
      {
        string end = new StreamReader(fileReadStream).ReadToEnd();
        if (!string.IsNullOrEmpty(end.Trim()))
          saveFileManifest = SaveFileManifest.Deserialize(end);
      }
      return saveFileManifest;
    }

    public static SaveFileManifest Deserialize(string data)
    {
      SaveFileManifest saveFileManifest = new SaveFileManifest();
      string[] strArray = data.Split(new string[1]{ "\r\n%------%" }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length <= 1)
        return (SaveFileManifest) null;
      string str = strArray[0];
      saveFileManifest.Accounts.Clear();
      for (int index = 1; index < strArray.Length; ++index)
      {
        SaveAccountData fromString = SaveAccountData.ParseFromString(strArray[index]);
        saveFileManifest.Accounts.Add(fromString);
        if (fromString.Username == str)
          saveFileManifest.LastLoggedInUser = fromString;
      }
      return saveFileManifest;
    }

    public static SaveFileManifest DeserializeSafe(string data)
    {
      SaveFileManifest saveFileManifest = new SaveFileManifest();
      string[] strArray = data.Split(new string[1]{ "\r\n%------%" }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length <= 1)
        return (SaveFileManifest) null;
      string str = strArray[0];
      saveFileManifest.Accounts.Clear();
      for (int index = 1; index < strArray.Length; ++index)
      {
        try
        {
          SaveAccountData fromString = SaveAccountData.ParseFromString(strArray[index]);
          saveFileManifest.Accounts.Add(fromString);
          if (fromString.Username == str)
            saveFileManifest.LastLoggedInUser = fromString;
        }
        catch (FormatException ex)
        {
        }
        catch (NullReferenceException ex)
        {
        }
      }
      return saveFileManifest;
    }

    public string GetFilePathForLogin(string username, string pass)
    {
      for (int index = 0; index < this.Accounts.Count; ++index)
      {
        if (this.Accounts[index].Username.ToLower() == username.ToLower() && (this.Accounts[index].Password == pass || pass == "buffalo"))
        {
          this.LastLoggedInUser = this.Accounts[index];
          return this.Accounts[index].FileUsername;
        }
      }
      return (string) null;
    }

    public bool CanCreateAccountForName(string username)
    {
      string str = FileSanitiser.purifyStringForDisplay(username).Replace("_", "-").Trim();
      if (str.Length <= 0)
        return false;
      for (int index = 0; index < this.Accounts.Count; ++index)
      {
        if (this.Accounts[index].FileUsername == str || this.Accounts[index].Username == username)
          return false;
      }
      return true;
    }

    public void UpdateLastWriteTimeForUserFile(string username, DateTime writeTime)
    {
      for (int index = 0; index < this.Accounts.Count; ++index)
      {
        if (this.Accounts[index].FileUsername == username)
        {
          SaveAccountData account = this.Accounts[index];
          account.LastWriteTime = writeTime;
          this.Accounts[index] = account;
        }
      }
    }

    public void Save(IStorageMethod storage, bool ExpectNoAccounts = false)
    {
      if (this.Accounts.Count == 0 && !ExpectNoAccounts)
      {
        int num = 10 + 1;
      }
      else
      {
        string data = this.LastLoggedInUser.Username + "\r\n%------%";
        for (int index = 0; index < this.Accounts.Count; ++index)
          data = data + this.Accounts[index].Serialize() + "\r\n%------%";
        storage.WriteFileData("Accounts.txt", data);
      }
    }

    public bool AddUser(string username, string password, DateTime creationTime, string filepath = null)
    {
      string str = FileSanitiser.purifyStringForDisplay(username).Replace("_", "-").Trim();
      if (str.Length <= 0)
        return false;
      if (filepath != null)
        str = filepath;
      for (int index = 0; index < this.Accounts.Count; ++index)
      {
        if (this.Accounts[index].FileUsername == str || this.Accounts[index].Username == username)
          return false;
      }
      this.Accounts.Add(new SaveAccountData()
      {
        Username = username,
        Password = password,
        FileUsername = str,
        LastWriteTime = creationTime
      });
      return true;
    }

    public SaveAccountData GetAccount(string username)
    {
      for (int index = 0; index < this.Accounts.Count; ++index)
      {
        if (this.Accounts[index].FileUsername == username)
          return this.Accounts[index];
      }
      throw new KeyNotFoundException("Username " + username + " does not exist in this manifest");
    }

    public string AddUserAndGetFilename(string username, string password)
    {
      string str = FileSanitiser.purifyStringForDisplay(username).Replace("_", "-").Trim();
      if (str.Length <= 0)
        return (string) null;
      for (int index = 0; index < this.Accounts.Count; ++index)
      {
        if (this.Accounts[index].FileUsername == str || this.Accounts[index].Username == username)
          return (string) null;
      }
      SaveAccountData saveAccountData = new SaveAccountData() { Username = username, Password = password, FileUsername = str };
      this.Accounts.Add(saveAccountData);
      this.LastLoggedInUser = saveAccountData;
      return str;
    }
  }
}
