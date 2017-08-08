// Decompiled with JetBrains decompiler
// Type: Hacknet.Program
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using SDL2;
using System;
using System.IO;

namespace Hacknet
{
  internal static class Program
  {
    public static string GraphicsDeviceResetLog = "";

    [STAThread]
    private static void Main(string[] args)
    {
      for (int index = 0; index < args.Length; ++index)
      {
        string str = args[index];
        if (str.Equals("-disableweb"))
          WebRenderer.Enabled = false;
        if (str.Equals("-disablebackground"))
          Settings.DrawHexBackground = false;
        if (str.Equals("-altmonitor"))
          Settings.StartOnAltMonitor = true;
        if (str.Equals("-enablefc") && Settings.emergencyForceCompleteEnabled)
          Settings.forceCompleteEnabled = true;
        if (str.Equals("-enabledebug") && Settings.emergencyDebugCommandsEnabled)
        {
          Settings.debugCommandsEnabled = true;
          OS.DEBUG_COMMANDS = true;
        }
        if (str.ToLower().Equals("-extstart") && Settings.AllowExtensionMode && index < args.Length - 1)
        {
          string path2 = args[index + 1];
          ++index;
          Game1.AutoLoadExtensionPath = Path.Combine("Extensions", path2);
        }
        if (str.ToLower().Equals("-allowextpublish"))
          Settings.AllowExtensionPublish = true;
      }
      using (Game1 game1 = new Game1())
      {
        try
        {
          game1.Run();
        }
        catch (Exception ex1)
        {
          SDL.SDL_ShowSimpleMessageBox(SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR, "SAVE THIS MESSAGE!", ex1.ToString(), IntPtr.Zero);
          string str1 = Utils.GenerateReportFromException(ex1);
          try
          {
            if (OS.currentInstance != null)
            {
              str1 = str1 + "\r\n\r\n" + OS.currentInstance.Flags.GetSaveString() + "\r\n\r\n";
              str1 = str1 + "Timer : " + (object) (float) ((double) OS.currentInstance.timer / 60.0) + "mins\r\n";
              str1 = str1 + "Display cache : " + OS.currentInstance.displayCache + "\r\n";
              str1 = str1 + "String Cache : " + OS.currentInstance.getStringCache + "\r\n";
              str1 = str1 + "Terminal--------------\r\n" + OS.currentInstance.terminal.GetRecentTerminalHistoryString() + "-------------\r\n";
            }
            else
              str1 += "\r\n\r\nOS INSTANCE NULL\r\n\r\n";
            str1 = str1 + "\r\nMenuErrorCache: " + MainMenu.AccumErrors + "\r\n";
          }
          catch (Exception ex2)
          {
          }
          string str2 = str1 + "\r\n\r\nPlatform API: " + PlatformAPISettings.Report + "\r\n\r\nGraphics Device Reset Log: " + Program.GraphicsDeviceResetLog + "\r\n\r\nCurrent Time: " + DateTime.Now.ToShortTimeString() + "\r\n" + "\r\n\r\nVersion: " + MainMenu.OSVersion + "\r\n" + "\r\n\r\nMode: FNA\r\n";
          string path = "";
          if (SDL.SDL_GetPlatform().Equals("Windows"))
          {
            path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/My Games/Hacknet/Reports/";
            if (!Directory.Exists(path))
              Directory.CreateDirectory(path);
          }
          Utils.writeToFile(str2, path + "CrashReport_" + Guid.NewGuid().ToString().Replace(" ", "_") + ".txt");
          Utils.SendRealWorldEmail("Hackent " + MainMenu.OSVersion + " Crash " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(), "hacknetbugs+Hacknet@gmail.com", str2);
          throw ex1;
        }
      }
    }
  }
}
