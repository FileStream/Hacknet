// Decompiled with JetBrains decompiler
// Type: Hacknet.MusicManager
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Hacknet
{
  public static class MusicManager
  {
    private static float DEFAULT_FADE_TIME = Settings.isConventionDemo ? 0.5f : 2f;
    public static float FADE_TIME = MusicManager.DEFAULT_FADE_TIME;
    private static float fadeTimer = 0.0f;
    private static bool initialized = false;
    public static bool dataLoadedFromOutsideFile = false;
    private static Dictionary<string, Song> loadedSongs = new Dictionary<string, Song>();
    private static bool IsMediaPlayerCrashDisabled = false;
    private const int PLAYING = 0;
    private const int FADING_OUT = 1;
    private const int FADING_IN = 2;
    private const int STOPPED = 3;
    public static Song curentSong;
    private static Song nextSong;
    private static string nextSongName;
    public static bool isPlaying;
    public static bool isMuted;
    public static string currentSongName;
    private static float destinationVolume;
    private static float fadeVolume;
    private static int state;
    private static ContentManager contentManager;

    public static void init(ContentManager content)
    {
      try
      {
        MusicManager.contentManager = content;
        MusicManager.currentSongName = "Music\\Revolve";
        MusicManager.curentSong = content.Load<Song>(MusicManager.currentSongName);
        MusicManager.isPlaying = false;
        if (!MusicManager.dataLoadedFromOutsideFile)
        {
          MusicManager.isMuted = false;
          MediaPlayer.Volume = 0.3f;
        }
        MusicManager.destinationVolume = MediaPlayer.Volume;
        MusicManager.state = 3;
        MusicManager.initialized = true;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Music Error: Could not initialize first song load\r\n" + Utils.GenerateReportFromException(ex));
      }
    }

    public static void playSong()
    {
      try
      {
        if (MusicManager.isPlaying || !MusicManager.initialized)
          return;
        if (!Settings.soundDisabled)
        {
          MediaPlayer.Play(MusicManager.curentSong);
          MediaPlayer.IsRepeating = true;
        }
        MusicManager.isPlaying = true;
        MusicManager.state = 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error playing " + Utils.GenerateReportFromException(ex));
      }
    }

    public static void toggleMute()
    {
      MusicManager.isMuted = !MusicManager.isMuted;
      MediaPlayer.IsMuted = MusicManager.isMuted;
    }

    public static void setIsMuted(bool muted)
    {
      MusicManager.isMuted = muted;
      MediaPlayer.IsMuted = MusicManager.isMuted;
    }

    public static void stop()
    {
      MediaPlayer.Stop();
      MusicManager.isPlaying = false;
      MusicManager.state = 3;
    }

    public static float getVolume()
    {
      return MusicManager.destinationVolume;
    }

    public static void setVolume(float volume)
    {
      MediaPlayer.Volume = volume;
      MusicManager.destinationVolume = volume;
    }

    public static void playSongImmediatley(string songname)
    {
      if (MusicManager.currentSongName != songname)
      {
        try
        {
          MusicManager.curentSong = MusicManager.contentManager.Load<Song>(songname);
        }
        catch (Exception ex)
        {
          Console.WriteLine("Error switching to song: " + songname + "\r\n" + Utils.GenerateReportFromException(ex));
        }
      }
      if (!(MusicManager.curentSong != (Song) null))
        return;
      MusicManager.isPlaying = false;
      MusicManager.currentSongName = songname;
      MusicManager.playSong();
      MusicManager.setVolume(MusicManager.destinationVolume);
    }

    public static void loadAsCurrentSong(string songname)
    {
      try
      {
        MusicManager.curentSong = MusicManager.contentManager.Load<Song>(songname);
        MusicManager.nextSongName = songname;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error switching to song as current: " + songname + "\r\n" + Utils.GenerateReportFromException(ex));
      }
      if (!(MusicManager.curentSong != (Song) null))
        return;
      MusicManager.isPlaying = false;
      MusicManager.currentSongName = songname;
    }

    public static void loadAsCurrentSongUnsafe(string songname)
    {
      MusicManager.curentSong = MusicManager.contentManager.Load<Song>(songname);
      if (!(MusicManager.curentSong != (Song) null))
        return;
      MusicManager.isPlaying = false;
      MusicManager.currentSongName = songname;
    }

    public static void transitionToSong(string songName)
    {
      try
      {
        if (!(MusicManager.currentSongName != songName))
          return;
        Thread thread = new Thread(new ThreadStart(MusicManager.loadSong));
        thread.IsBackground = true;
        MusicManager.nextSongName = songName;
        thread.Start();
        Console.WriteLine("Started song loader thread");
        MusicManager.state = 1;
        MusicManager.fadeTimer = MusicManager.FADE_TIME;
        MusicManager.currentSongName = songName;
      }
      catch
      {
        Console.WriteLine("Error transitioning to Song");
      }
    }

    private static void loadSong()
    {
      try
      {
        MusicManager.nextSong = MusicManager.contentManager.Load<Song>(MusicManager.nextSongName);
        if (MusicManager.loadedSongs.ContainsKey(MusicManager.nextSongName))
          MusicManager.nextSong = MusicManager.loadedSongs[MusicManager.nextSongName];
        else
          MusicManager.loadedSongs.Add(MusicManager.nextSongName, MusicManager.nextSong);
      }
      catch (ArgumentException ex)
      {
      }
      catch (ContentLoadException ex)
      {
        if (OS.TestingPassOnly)
          throw ex;
        if (OS.currentInstance == null)
          return;
        OS.currentInstance.write(ex.ToString());
        OS.currentInstance.write(ex.Message);
      }
    }

    public static void Update(float t)
    {
      switch (MusicManager.state)
      {
        case 0:
          MusicManager.fadeVolume = MusicManager.destinationVolume;
          MusicManager.FADE_TIME = MusicManager.DEFAULT_FADE_TIME;
          break;
        case 1:
          MusicManager.fadeTimer -= t;
          MusicManager.fadeVolume = MusicManager.destinationVolume * (MusicManager.fadeTimer / MusicManager.FADE_TIME);
          if ((double) MusicManager.fadeVolume <= 0.0)
          {
            if (MusicManager.nextSong != (Song) null)
            {
              MusicManager.state = 2;
              MediaPlayer.Volume = 0.0f;
              MusicManager.curentSong = MusicManager.nextSong;
              MusicManager.nextSong = (Song) null;
              MusicManager.fadeTimer = MusicManager.FADE_TIME;
              if (Settings.soundDisabled || MusicManager.IsMediaPlayerCrashDisabled)
                break;
              try
              {
                MediaPlayer.Play(MusicManager.curentSong);
              }
              catch (InvalidOperationException ex)
              {
                MusicManager.IsMediaPlayerCrashDisabled = true;
                if (OS.currentInstance != null && OS.currentInstance.terminal != null)
                {
                  OS.currentInstance.write("-------------------------------");
                  OS.currentInstance.write("-------------WARNING-----------");
                  OS.currentInstance.write("-------------------------------");
                  OS.currentInstance.write("HacknetOS VM Audio hook could not be established.");
                  OS.currentInstance.write("Music Playback Disabled - Media Player (VM Hook:WindowsMediaPlayer)");
                  OS.currentInstance.write("Has been uninstalled or disabled.");
                  OS.currentInstance.write("-------------------------------");
                  OS.currentInstance.write("-------------WARNING-----------");
                  OS.currentInstance.write("-------------------------------");
                }
              }
              break;
            }
            MusicManager.fadeVolume = 0.0f;
            break;
          }
          MediaPlayer.Volume = MusicManager.fadeVolume;
          break;
        case 2:
          MusicManager.fadeTimer -= t;
          MusicManager.fadeVolume = MusicManager.destinationVolume * (float) (1.0 - (double) MusicManager.fadeTimer / (double) MusicManager.FADE_TIME);
          if ((double) MusicManager.fadeVolume >= (double) MusicManager.destinationVolume)
          {
            MediaPlayer.Volume = MusicManager.destinationVolume;
            MusicManager.state = 0;
            break;
          }
          MediaPlayer.Volume = MusicManager.fadeVolume;
          break;
      }
    }
  }
}
