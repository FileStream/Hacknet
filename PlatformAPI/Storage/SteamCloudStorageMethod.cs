// Decompiled with JetBrains decompiler
// Type: Hacknet.PlatformAPI.Storage.SteamCloudStorageMethod
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Extensions;
using Steamworks;
using System;
using System.IO;
using System.Text;

namespace Hacknet.PlatformAPI.Storage
{
  public class SteamCloudStorageMethod : BasicStorageMethod
  {
    private bool deserialized = false;

    private string PathPrefix
    {
      get
      {
        if (Settings.IsInExtensionMode)
          return ExtensionLoader.ActiveExtensionInfo.GetFoldersafeName() + "/";
        return "";
      }
    }

    public override bool ShouldWrite
    {
      get
      {
        return true;
      }
    }

    public override bool DidDeserialize
    {
      get
      {
        return this.deserialized;
      }
    }

    public override void Load()
    {
      if (!PlatformAPISettings.Running)
        return;
      try
      {
        this.manifest = SaveFileManifest.Deserialize((IStorageMethod) this);
      }
      catch (NullReferenceException ex)
      {
      }
      catch (FormatException ex)
      {
      }
      if (this.manifest == null)
      {
        this.manifest = new SaveFileManifest();
        this.manifest.Save((IStorageMethod) this, false);
      }
      else
        this.deserialized = true;
    }

    public override bool FileExists(string filename)
    {
      if (!PlatformAPISettings.Running)
        return false;
      return SteamRemoteStorage.FileExists(this.PathPrefix + filename);
    }

    public override Stream GetFileReadStream(string filename)
    {
      if (!PlatformAPISettings.Running)
        return (Stream) null;
      int cubDataToRead = 2000000;
      byte[] numArray = new byte[cubDataToRead];
      if (!SteamRemoteStorage.FileExists(this.PathPrefix + filename))
        return (Stream) null;
      int count = SteamRemoteStorage.FileRead(this.PathPrefix + filename, numArray, cubDataToRead);
      Encoding.UTF8.GetString(numArray);
      return (Stream) new MemoryStream(numArray, 0, count);
    }

    public override void WriteFileData(string filename, byte[] data)
    {
      if (!PlatformAPISettings.Running || SteamRemoteStorage.FileWrite(this.PathPrefix + filename, data, data.Length))
        return;
      Console.WriteLine("Failed to write to steam");
    }

    public override void WriteFileData(string filename, string data)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(data);
      this.WriteFileData(filename, bytes);
    }
  }
}
