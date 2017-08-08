// Decompiled with JetBrains decompiler
// Type: Hacknet.HostileHackerBreakinSequence
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Extensions;
using SDL2;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Hacknet
{
  public static class HostileHackerBreakinSequence
  {
    private static readonly string BaseDirectory = HostileHackerBreakinSequence.GetBaseDirectory();
    private static readonly string HelpFilePath = Path.Combine(HostileHackerBreakinSequence.BaseDirectory, "VM_Recovery_Guide.txt");
    private static readonly string Win32_BatchFilePath = Path.Combine(HostileHackerBreakinSequence.BaseDirectory, "OpenCMD.bat");
    private static readonly string HostileDirectory = Path.Combine(HostileHackerBreakinSequence.BaseDirectory, "Libs", "Injected");
    private static readonly string HostileFilePath = Path.Combine(HostileHackerBreakinSequence.HostileDirectory, "VMBootloaderTrap.dll");

    private static string GetBaseDirectory()
    {
      string platform = SDL.SDL_GetPlatform();
      string path1;
      if (platform.Equals("Linux"))
      {
        path1 = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
        if (string.IsNullOrEmpty(path1))
        {
          string environmentVariable = Environment.GetEnvironmentVariable("HOME");
          path1 = !string.IsNullOrEmpty(environmentVariable) ? Path.Combine(environmentVariable, ".local/share") : ".";
        }
      }
      else if (platform.Equals("Mac OS X"))
      {
        string environmentVariable = Environment.GetEnvironmentVariable("HOME");
        path1 = !string.IsNullOrEmpty(environmentVariable) ? Path.Combine(environmentVariable, "Library/Application Support") : ".";
      }
      else
      {
        if (!platform.Equals("Windows"))
          throw new NotSupportedException("Unhandled SDL2 platform!");
        path1 = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/My Games/";
      }
      if (Settings.IsInExtensionMode)
        path1 = Path.Combine(path1, ExtensionLoader.ActiveExtensionInfo.GetFoldersafeName());
      string path = Path.Combine(path1, "Hacknet");
      Console.WriteLine("HHBS-Folderpath: " + path);
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      return path;
    }

    internal static void Execute(object osobj, Computer source, Computer target)
    {
      Console.WriteLine("Breakin Started!");
      PostProcessor.EndingSequenceFlashOutActive = true;
      PostProcessor.EndingSequenceFlashOutPercentageComplete = 1f;
      OS os = (OS) osobj;
      DateTime now = DateTime.Now;
      os.Flags.AddFlag("startupBreakinTrapActivated");
      os.threadedSaveExecute(true);
      HostileHackerBreakinSequence.CopyHostileFileToLocalSystem();
      Console.WriteLine("Copied files to local system...");
      double totalSeconds = (DateTime.Now - now).TotalSeconds;
      double time = totalSeconds <= 3.0 ? 4.0 - totalSeconds : 1.5;
      os.delayer.Post(ActionDelayer.Wait(time), (Action) (() =>
      {
        PostProcessor.EndingSequenceFlashOutActive = false;
        PostProcessor.EndingSequenceFlashOutPercentageComplete = 0.0f;
        os.thisComputer.crash(source.ip);
      }));
    }

    public static bool IsFirstSuccessfulBootAfterBlockingState(object osobj)
    {
      OS os = (OS) osobj;
      return os.Flags.HasFlag("startupBreakinTrapActivated") && !os.Flags.HasFlag("startupBreakinTrapPassed") && !File.Exists(HostileHackerBreakinSequence.HostileFilePath);
    }

    public static void ReactToFirstSuccesfulBoot(object osobj)
    {
      ((OS) osobj).Flags.AddFlag("startupBreakinTrapPassed");
      MusicManager.loadAsCurrentSong("DLC\\Music\\World_Chase");
    }

    public static bool IsInBlockingHostileFileState(object osobj)
    {
      Console.WriteLine("Booting up -- checking libs");
      OS os = (OS) osobj;
      if (os.Flags.HasFlag("startupBreakinTrapActivated") && !os.Flags.HasFlag("startupBreakinTrapPassed"))
      {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
          return File.Exists(HostileHackerBreakinSequence.HostileFilePath);
        if (Directory.Exists(HostileHackerBreakinSequence.HostileDirectory))
        {
          if (new DirectoryInfo(HostileHackerBreakinSequence.HostileDirectory).Attributes.HasFlag((Enum) FileAttributes.ReadOnly))
            return true;
          return File.Exists(HostileHackerBreakinSequence.HostileFilePath);
        }
      }
      Console.WriteLine("Libs loaded successfully");
      return false;
    }

    private static void CopyHostileFileToLocalSystem()
    {
      if (!Directory.Exists(HostileHackerBreakinSequence.HostileDirectory))
        Directory.CreateDirectory(HostileHackerBreakinSequence.HostileDirectory);
      try
      {
        if (File.Exists(HostileHackerBreakinSequence.HostileFilePath))
          return;
        File.Copy("Content/DLC/Misc/VMBootloaderTrap.dll", HostileHackerBreakinSequence.HostileFilePath);
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
          HostileHackerBreakinSequence.LockFileToPreventDeletionWin32(HostileHackerBreakinSequence.HostileFilePath);
        else
          HostileHackerBreakinSequence.LockFileToPreventDeletionUnix(HostileHackerBreakinSequence.HostileDirectory, HostileHackerBreakinSequence.HostileFilePath);
      }
      catch (UnauthorizedAccessException ex)
      {
        Utils.AppendToErrorFile("HHBreakinSequence error : Insufficient permissions for folder access.\r\n" + Utils.GenerateReportFromException((Exception) ex));
        Console.WriteLine("HHSequence Error " + Utils.GenerateReportFromException((Exception) ex));
      }
    }

    public static string GetHelpText()
    {
      return File.ReadAllText(LocalizedFileLoader.GetLocalizedFilepath("Content/DLC/Misc/" + (Environment.OSVersion.Platform == PlatformID.Win32NT ? "Win_AllyHelpFile.txt" : "Unix_AllyHelpFile.txt")));
    }

    public static void CopyHelpFile()
    {
      try
      {
        if (File.Exists(HostileHackerBreakinSequence.HelpFilePath))
          return;
        File.Copy(LocalizedFileLoader.GetLocalizedFilepath("Content/DLC/Misc/" + (Environment.OSVersion.Platform == PlatformID.Win32NT ? "Win_AllyHelpFile.txt" : "Unix_AllyHelpFile.txt")), HostileHackerBreakinSequence.HelpFilePath);
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
          Process.Start("chmod", " -x " + HostileHackerBreakinSequence.HelpFilePath);
      }
      catch (UnauthorizedAccessException ex)
      {
        Utils.AppendToErrorFile("HHBreakinSequence error : Insufficient permissions for folder access.\r\n" + Utils.GenerateReportFromException((Exception) ex));
        Console.WriteLine("HHSequence Error " + Utils.GenerateReportFromException((Exception) ex));
      }
      catch (Exception ex)
      {
        Console.WriteLine("HHSequence Error " + Utils.GenerateReportFromException(ex));
      }
    }

    public static void OpenWindowsHelpDocument()
    {
      HostileHackerBreakinSequence.MinimizeAllOpenWindows();
      Process.Start("notepad.exe", HostileHackerBreakinSequence.HelpFilePath);
    }

    public static string OpenTerminal()
    {
      if (Environment.OSVersion.Platform == PlatformID.Win32NT)
      {
        try
        {
          if (!File.Exists(HostileHackerBreakinSequence.Win32_BatchFilePath))
            File.Copy("Content/DLC/Misc/Win_OpenCMD.bat", HostileHackerBreakinSequence.Win32_BatchFilePath);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
        Process.Start(HostileHackerBreakinSequence.Win32_BatchFilePath);
      }
      else
      {
        string platform = SDL.SDL_GetPlatform();
        if (platform.Equals("Linux"))
        {
          try
          {
            if (File.Exists("/usr/bin/gsettings"))
            {
              Process process = Process.Start(new ProcessStartInfo("/usr/bin/gsettings", " get org.gnome.desktop.default-applications.terminal exec")
              {
                RedirectStandardOutput = true,
                UseShellExecute = false
              });
              process.WaitForExit();
              string fileName = process.StandardOutput.ReadLine();
              if (!string.IsNullOrEmpty(fileName))
                fileName = fileName.Replace("'", "");
              if (!string.IsNullOrEmpty(fileName))
                Process.Start(new ProcessStartInfo(fileName)
                {
                  WorkingDirectory = HostileHackerBreakinSequence.BaseDirectory
                });
            }
          }
          catch (Exception ex)
          {
          }
        }
        else if (platform.Equals("Mac OS X"))
          Process.Start("/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal", HostileHackerBreakinSequence.BaseDirectory);
        else
          Console.WriteLine(platform);
      }
      return HostileHackerBreakinSequence.BaseDirectory.Replace("\\", "/");
    }

    public static void CrashProgram()
    {
      MusicManager.stop();
      Game1.threadsExiting = true;
      Game1.getSingleton().Exit();
    }

    private static void LockFileToPreventDeletionWin32(string filepath)
    {
      FileSecurity accessControl = File.GetAccessControl(filepath);
      AuthorizationRuleCollection accessRules = accessControl.GetAccessRules(true, true, typeof (NTAccount));
      accessControl.SetAccessRuleProtection(true, false);
      foreach (FileSystemAccessRule rule in (ReadOnlyCollectionBase) accessRules)
        accessControl.RemoveAccessRule(rule);
      accessControl.AddAccessRule(new FileSystemAccessRule((IdentityReference) new SecurityIdentifier(WellKnownSidType.WorldSid, (SecurityIdentifier) null), FileSystemRights.Delete, AccessControlType.Deny));
      File.SetAccessControl(filepath, accessControl);
    }

    private static void LockFileToPreventDeletionUnix(string dir, string file)
    {
      Process.Start("chmod", " 000 " + file).WaitForExit();
      Process.Start("chmod", " 000 " + dir).WaitForExit();
    }

    private static void MinimizeWindow(IntPtr handle)
    {
    }

    private static void MinimizeAllOpenWindows()
    {
      Process currentProcess = Process.GetCurrentProcess();
      foreach (Process process in Process.GetProcesses())
      {
        if (process != currentProcess)
        {
          IntPtr mainWindowHandle = process.MainWindowHandle;
          if (!(mainWindowHandle == IntPtr.Zero))
            HostileHackerBreakinSequence.MinimizeWindow(mainWindowHandle);
        }
      }
    }
  }
}
