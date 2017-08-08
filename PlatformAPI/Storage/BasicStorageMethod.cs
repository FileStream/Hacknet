// Decompiled with JetBrains decompiler
// Type: Hacknet.PlatformAPI.Storage.BasicStorageMethod
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.IO;

namespace Hacknet.PlatformAPI.Storage
{
  public class BasicStorageMethod : IStorageMethod
  {
    protected SaveFileManifest manifest;

    public virtual bool ShouldWrite
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public virtual bool DidDeserialize
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public virtual void Load()
    {
      throw new NotImplementedException();
    }

    public virtual SaveFileManifest GetSaveManifest()
    {
      return this.manifest;
    }

    public virtual Stream GetFileReadStream(string filename)
    {
      throw new NotImplementedException();
    }

    public virtual bool FileExists(string filename)
    {
      throw new NotImplementedException();
    }

    public virtual void WriteFileData(string filename, string data)
    {
      throw new NotImplementedException();
    }

    public virtual void WriteFileData(string filename, byte[] data)
    {
      throw new NotImplementedException();
    }

    public virtual void WriteSaveFileData(string filename, string username, string data, DateTime utcSaveFileTime)
    {
      this.WriteFileData(filename, data);
      for (int index = 0; index < this.manifest.Accounts.Count; ++index)
      {
        SaveAccountData account = this.manifest.Accounts[index];
        if (account.Username == username)
        {
          account.LastWriteTime = utcSaveFileTime;
          this.manifest.LastLoggedInUser = account;
          break;
        }
      }
      this.manifest.Save((IStorageMethod) this, false);
    }

    public virtual void UpdateDataFromOtherManager(IStorageMethod otherMethod)
    {
      string username = this.manifest.LastLoggedInUser.Username;
      SaveFileManifest saveManifest = otherMethod.GetSaveManifest();
      for (int index1 = 0; index1 < saveManifest.Accounts.Count; ++index1)
      {
        SaveAccountData account1 = saveManifest.Accounts[index1];
        bool flag = false;
        for (int index2 = 0; index2 < this.manifest.Accounts.Count; ++index2)
        {
          SaveAccountData account2 = this.manifest.Accounts[index2];
          if (account2.Username == account1.Username)
          {
            flag = true;
            TimeSpan timeSpan = account1.LastWriteTime - account2.LastWriteTime;
            if (account1.LastWriteTime > account2.LastWriteTime && timeSpan.TotalSeconds > 5.0)
            {
              Stream fileReadStream = otherMethod.GetFileReadStream(account1.FileUsername);
              if (fileReadStream != null)
              {
                string data = Utils.ReadEntireContentsOfStream(fileReadStream);
                if (data.Length > 100)
                  this.WriteFileData(account2.FileUsername, data);
              }
              break;
            }
            break;
          }
        }
        if (!flag)
        {
          Stream fileReadStream = otherMethod.GetFileReadStream(account1.FileUsername);
          if (fileReadStream != null)
          {
            string fileNameForUsername = SaveFileManager.GetSaveFileNameForUsername(account1.Username);
            this.manifest.AddUser(account1.Username, account1.Password, DateTime.UtcNow, fileNameForUsername);
            string data = Utils.ReadEntireContentsOfStream(fileReadStream);
            this.WriteFileData(fileNameForUsername, data);
          }
        }
      }
      for (int index = 0; index < this.manifest.Accounts.Count; ++index)
      {
        if (this.manifest.Accounts[index].Username == username)
          this.manifest.LastLoggedInUser = this.manifest.Accounts[index];
      }
      if (this.manifest.LastLoggedInUser.Username == null && this.manifest.Accounts.Count > 0)
        this.manifest.LastLoggedInUser = this.manifest.Accounts[0];
      this.manifest.Save((IStorageMethod) this, false);
    }
  }
}
