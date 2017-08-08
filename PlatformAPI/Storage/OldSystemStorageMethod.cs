// Decompiled with JetBrains decompiler
// Type: Hacknet.PlatformAPI.Storage.OldSystemStorageMethod
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.IO;

namespace Hacknet.PlatformAPI.Storage
{
  public class OldSystemStorageMethod : IStorageMethod
  {
    private SaveFileManifest manifest;

    public bool ShouldWrite
    {
      get
      {
        return false;
      }
    }

    public bool DidDeserialize
    {
      get
      {
        return false;
      }
    }

    public OldSystemStorageMethod(SaveFileManifest manifest)
    {
      this.manifest = manifest;
    }

    public void Load()
    {
    }

    public SaveFileManifest GetSaveManifest()
    {
      return this.manifest;
    }

    public Stream GetFileReadStream(string filename)
    {
      return (Stream) File.OpenRead(filename);
    }

    public bool FileExists(string filename)
    {
      return File.Exists(filename);
    }

    public void WriteFileData(string filename, byte[] data)
    {
      Utils.SafeWriteToFile(data, filename);
    }

    public void WriteFileData(string filename, string data)
    {
      Utils.SafeWriteToFile(data, filename);
    }

    public void UpdateDataFromOtherManager(IStorageMethod otherMethod)
    {
      throw new NotImplementedException();
    }

    public void WriteSaveFileData(string filename, string username, string data, DateTime utcSaveFileTime)
    {
      throw new NotImplementedException();
    }
  }
}
