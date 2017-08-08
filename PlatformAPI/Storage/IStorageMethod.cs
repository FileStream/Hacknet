// Decompiled with JetBrains decompiler
// Type: Hacknet.PlatformAPI.Storage.IStorageMethod
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.IO;

namespace Hacknet.PlatformAPI.Storage
{
  public interface IStorageMethod
  {
    bool ShouldWrite { get; }

    bool DidDeserialize { get; }

    void Load();

    SaveFileManifest GetSaveManifest();

    Stream GetFileReadStream(string filename);

    bool FileExists(string filename);

    void WriteFileData(string filename, string data);

    void WriteFileData(string filename, byte[] data);

    void WriteSaveFileData(string filename, string username, string data, DateTime utcSaveFileTime);

    void UpdateDataFromOtherManager(IStorageMethod otherMethod);
  }
}
