// Decompiled with JetBrains decompiler
// Type: Hacknet.SettingsLoader
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDL2;
using System;
using System.Globalization;
using System.IO;

namespace Hacknet
{
  public static class SettingsLoader
  {
    public static bool isFullscreen = false;
    public static bool didLoad = false;
    public static bool hasEverSaved = false;
    public static bool ShouldMultisample = true;
    public static bool ShouldDrawMusicVis = true;
    private static readonly string settingsPath = SettingsLoader.GetSettingsPath();
    public static int resWidth;
    public static int resHeight;

    private static string GetSettingsPath()
    {
      string platform = SDL.SDL_GetPlatform();
      if (platform.Equals("Linux"))
      {
        string str = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        if (string.IsNullOrEmpty(str))
        {
          string environmentVariable = Environment.GetEnvironmentVariable("HOME");
          if (string.IsNullOrEmpty(environmentVariable))
            return "Settings.txt";
          str = environmentVariable + "/.config";
        }
        string path = str + "/Hacknet";
        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);
        return path + "/Settings.txt";
      }
      if (platform.Equals("Mac OS X"))
      {
        string environmentVariable = Environment.GetEnvironmentVariable("HOME");
        if (string.IsNullOrEmpty(environmentVariable))
          return "Settings.txt";
        string path = environmentVariable + "/Library/Application Support/Hacknet";
        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);
        return path + "/Settings.txt";
      }
      if (!platform.Equals("Windows"))
        throw new NotSupportedException("Unhandled SDL2 platform!");
      return "Settings.txt";
    }

    public static void checkStatus()
    {
      if (!File.Exists(SettingsLoader.settingsPath))
        return;
      string[] strArray = new StreamReader(TitleContainer.OpenStream(SettingsLoader.settingsPath)).ReadToEnd().Split(new string[2]{ "\r\n", "\n" }, StringSplitOptions.None);
      SettingsLoader.resWidth = Convert.ToInt32(strArray[0]);
      SettingsLoader.resHeight = Convert.ToInt32(strArray[1]);
      SettingsLoader.isFullscreen = strArray[2].ToLower().Equals("true");
      if (strArray.Length > 3)
      {
        PostProcessor.bloomEnabled = strArray[3].Substring(strArray[3].IndexOf(' ') + 1) == "true";
        PostProcessor.scanlinesEnabled = strArray[4].Substring(strArray[4].IndexOf(' ') + 1) == "true";
      }
      if (strArray.Length > 6)
      {
        MusicManager.setIsMuted(strArray[5].Substring(strArray[5].IndexOf(' ') + 1) == "true");
        string str = strArray[6].Substring(strArray[6].IndexOf(' ') + 1).Trim();
        MusicManager.getVolume();
        try
        {
          MusicManager.setVolume((float) Convert.ToDouble(str, (IFormatProvider) CultureInfo.InvariantCulture));
        }
        catch (FormatException ex)
        {
        }
        MusicManager.dataLoadedFromOutsideFile = true;
      }
      if (strArray.Length > 7)
      {
        string str = strArray[7].Substring(strArray[7].IndexOf(' ') + 1);
        GuiData.ActiveFontConfig.name = str;
      }
      if (strArray.Length > 8)
        SettingsLoader.hasEverSaved = strArray[8].Substring(strArray[8].IndexOf(' ') + 1) == "True";
      if (strArray.Length > 9)
        SettingsLoader.ShouldMultisample = strArray[9].Substring(strArray[9].IndexOf(' ') + 1).ToLower() == "true";
      if (strArray.Length > 10 && !string.IsNullOrWhiteSpace(strArray[10]))
      {
        string str = strArray[10].Substring(strArray[10].IndexOf(' ') + 1);
        if (string.IsNullOrWhiteSpace(str))
          str = "en-us";
        Settings.ActiveLocale = str;
      }
      if (strArray.Length > 11 && !string.IsNullOrWhiteSpace(strArray[11]))
        SettingsLoader.ShouldDrawMusicVis = strArray[11].Substring(strArray[11].IndexOf(' ') + 1).ToLower().Trim() == "true";
      SettingsLoader.didLoad = true;
    }

    public static void writeStatusFile()
    {
      GraphicsDevice graphicsDevice = Game1.getSingleton().GraphicsDevice;
      if (graphicsDevice == null)
        return;
      Utils.writeToFile(graphicsDevice.PresentationParameters.BackBufferWidth.ToString() + "\r\n" + (object) graphicsDevice.PresentationParameters.BackBufferHeight + "\r\n" + (Game1.getSingleton().graphics.IsFullScreen ? "true" : "false") + "\r\n" + "bloom: " + (PostProcessor.bloomEnabled ? "true" : "false") + "\r\n" + "scanlines: " + (PostProcessor.scanlinesEnabled ? "true" : "false") + "\r\n" + "muted: " + (MusicManager.isMuted ? "true" : "false") + "\r\n" + "volume: " + MusicManager.getVolume().ToString((IFormatProvider) CultureInfo.InvariantCulture) + "\r\n" + "fontConfig: " + GuiData.ActiveFontConfig.name + "\r\n" + "hasSaved: " + (object) SettingsLoader.hasEverSaved + "\r\n" + "shouldMultisample: " + (object) SettingsLoader.ShouldMultisample + "\r\n" + "defaultLocale: " + Settings.ActiveLocale + "\r\n" + "drawMusicVis: " + (object) SettingsLoader.ShouldDrawMusicVis + "\r\n", SettingsLoader.settingsPath);
    }
  }
}
