// Decompiled with JetBrains decompiler
// Type: Hacknet.RemoteSaveStorage
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Steamworks;
using System;
using System.IO;
using System.Text;

namespace Hacknet
{
  public static class RemoteSaveStorage
  {
    public static string BASE_SAVE_FILE_NAME = "save";
    public static string SAVE_FILE_EXT = ".xml";
    public static string Standalone_FolderPath = "Accounts/";

    public static bool FileExists(string playerID, bool forceLocal = false)
    {
      string pchFile = RemoteSaveStorage.BASE_SAVE_FILE_NAME + playerID + RemoteSaveStorage.SAVE_FILE_EXT;
      if (!forceLocal && PlatformAPISettings.Running)
        return SteamRemoteStorage.FileExists(pchFile);
      return File.Exists(RemoteSaveStorage.Standalone_FolderPath + pchFile);
    }

    public static Stream GetSaveReadStream(string playerID, bool forceLocal = false)
    {
      string pchFile = RemoteSaveStorage.BASE_SAVE_FILE_NAME + playerID + RemoteSaveStorage.SAVE_FILE_EXT;
      if (!forceLocal && PlatformAPISettings.Running)
      {
        int cubDataToRead = 2000000;
        byte[] numArray = new byte[cubDataToRead];
        if (!SteamRemoteStorage.FileExists(pchFile))
          return (Stream) null;
        int count = SteamRemoteStorage.FileRead(pchFile, numArray, cubDataToRead);
        if (count == 0)
          return (Stream) null;
        Encoding.UTF8.GetString(numArray);
        return (Stream) new MemoryStream(numArray, 0, count);
      }
      if (File.Exists(RemoteSaveStorage.Standalone_FolderPath + pchFile))
        return (Stream) File.OpenRead(RemoteSaveStorage.Standalone_FolderPath + pchFile);
      return (Stream) null;
    }

    public static void WriteSaveData(string saveData, string playerID, bool forcelocal = false)
    {
      string pchFile = RemoteSaveStorage.BASE_SAVE_FILE_NAME + playerID + RemoteSaveStorage.SAVE_FILE_EXT;
      if (!forcelocal || !PlatformAPISettings.Running)
      {
        byte[] bytes = Encoding.UTF8.GetBytes(saveData);
        if (!SteamRemoteStorage.FileWrite(pchFile, bytes, bytes.Length))
          Console.WriteLine("Failed to write to steam");
        try
        {
          Utils.SafeWriteToFile(saveData, RemoteSaveStorage.Standalone_FolderPath + pchFile);
        }
        catch (Exception ex)
        {
          Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
        }
      }
      else
        Utils.SafeWriteToFile(saveData, RemoteSaveStorage.Standalone_FolderPath + pchFile);
    }

    public static void Delete(string playerID)
    {
      string pchFile = RemoteSaveStorage.BASE_SAVE_FILE_NAME + playerID + RemoteSaveStorage.SAVE_FILE_EXT;
      if (PlatformAPISettings.Running)
        SteamRemoteStorage.FileDelete(pchFile);
      else if (Directory.Exists(RemoteSaveStorage.Standalone_FolderPath) && File.Exists(RemoteSaveStorage.Standalone_FolderPath + pchFile))
        File.Delete(RemoteSaveStorage.Standalone_FolderPath + pchFile);
    }

    public static bool CanLoad(string playerID)
    {
      string pchFile = RemoteSaveStorage.BASE_SAVE_FILE_NAME + playerID + RemoteSaveStorage.SAVE_FILE_EXT;
      if (PlatformAPISettings.Running)
        return SteamRemoteStorage.FileExists(pchFile);
      return File.Exists(RemoteSaveStorage.Standalone_FolderPath + pchFile);
    }
  }
}
