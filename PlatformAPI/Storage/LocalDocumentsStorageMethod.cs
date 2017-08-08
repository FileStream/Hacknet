// Decompiled with JetBrains decompiler
// Type: Hacknet.PlatformAPI.Storage.LocalDocumentsStorageMethod
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Extensions;
using SDL2;
using System;
using System.IO;

namespace Hacknet.PlatformAPI.Storage
{
  public class LocalDocumentsStorageMethod : BasicStorageMethod
  {
    private bool deserialized = false;
    private string FolderPath;

    private string FullFolderPath
    {
      get
      {
        if (Settings.IsInExtensionMode)
          return this.FolderPath + ExtensionLoader.ActiveExtensionInfo.GetFoldersafeName() + "/";
        return this.FolderPath;
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
      string platform = SDL.SDL_GetPlatform();
      if (platform.Equals("Linux"))
      {
        this.FolderPath = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
        if (string.IsNullOrEmpty(this.FolderPath))
        {
          this.FolderPath = Environment.GetEnvironmentVariable("HOME");
          if (string.IsNullOrEmpty(this.FolderPath))
            this.FolderPath = "./";
          else
            this.FolderPath += "/.local/share/Hacknet/Accounts/";
        }
        else
          this.FolderPath += "/Hacknet/Accounts/";
      }
      else if (platform.Equals("Mac OS X"))
      {
        this.FolderPath = Environment.GetEnvironmentVariable("HOME");
        if (string.IsNullOrEmpty(this.FolderPath))
          this.FolderPath = "./";
        else
          this.FolderPath += "/Library/Application Support/Hacknet/Accounts/";
      }
      else
      {
        if (!platform.Equals("Windows"))
          throw new NotSupportedException("Unhandled SDL2 platform!");
        this.FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/My Games/Hacknet/Accounts/";
      }
      try
      {
        if (!Directory.Exists(this.FullFolderPath))
          Directory.CreateDirectory(this.FullFolderPath);
      }
      catch (UnauthorizedAccessException ex)
      {
        Utils.AppendToErrorFile("Local Documents Storage load error : Insufficient permissions for folder access.\r\n" + Utils.GenerateReportFromException((Exception) ex));
        this.FolderPath = "Accounts/";
        MainMenu.AccumErrors = MainMenu.AccumErrors + "\r\nERROR: Local Documents Storage Folder is refusing access. Saving accounts to:\r\n" + Path.GetFullPath(this.FolderPath);
      }
      try
      {
        this.manifest = SaveFileManifest.Deserialize((IStorageMethod) this);
      }
      catch (FormatException ex)
      {
        this.manifest = (SaveFileManifest) null;
        Console.WriteLine("Local Save Manifest Corruption: " + Utils.GenerateReportFromException((Exception) ex));
      }
      catch (NullReferenceException ex)
      {
        this.manifest = (SaveFileManifest) null;
        Console.WriteLine("Local Save Manifest Corruption: " + Utils.GenerateReportFromException((Exception) ex));
      }
      if (this.manifest == null)
      {
        this.manifest = new SaveFileManifest();
        this.manifest.Save((IStorageMethod) this, true);
      }
      else
        this.deserialized = true;
    }

    public override bool FileExists(string filename)
    {
      return File.Exists(this.FullFolderPath + filename);
    }

    public override Stream GetFileReadStream(string filename)
    {
      return (Stream) File.OpenRead(this.FullFolderPath + filename);
    }

    public override void WriteFileData(string filename, byte[] data)
    {
      if (!Directory.Exists(this.FullFolderPath))
        Directory.CreateDirectory(this.FullFolderPath);
      Utils.SafeWriteToFile(data, this.FullFolderPath + filename);
    }

    public override void WriteFileData(string filename, string data)
    {
      if (!Directory.Exists(this.FullFolderPath))
        Directory.CreateDirectory(this.FullFolderPath);
      Utils.SafeWriteToFile(data, this.FullFolderPath + filename);
    }
  }
}
