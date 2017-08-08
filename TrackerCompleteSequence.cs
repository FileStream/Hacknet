// Decompiled with JetBrains decompiler
// Type: Hacknet.TrackerCompleteSequence
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public static class TrackerCompleteSequence
  {
    public static float MinTrackTime = 10f;
    public static float MaxTrackTime = 20f;
    internal static string ForkbombCompleteTraceIP = (string) null;
    public static bool NextCompleteForkbombShouldTrace;

    internal static void TrackComplete(object osobj, Computer source)
    {
      OS os = (OS) osobj;
      os.TrackersInProgress.Clear();
      Folder folder = source.files.root.searchForFolder("log");
      for (int index = 0; index < folder.files.Count; ++index)
      {
        string data = folder.files[index].data;
        if (data.Contains(os.thisComputer.ip) && (data.Contains("FileCopied") || data.Contains("FileDeleted") || data.Contains("FileMoved")))
        {
          folder.files.RemoveAt(index);
          --index;
        }
      }
      HackerScriptExecuter.runScript("HackerScripts/TrackSequence.txt", (object) os, source.ip, (string) null);
    }

    internal static bool CompShouldStartTrackerFromLogs(object osobj, Computer c, string targetIP = null)
    {
      OS os = (OS) osobj;
      Folder folder = c.files.root.searchForFolder("log");
      if (targetIP == null)
        targetIP = os.thisComputer.ip;
      for (int index = 0; index < folder.files.Count; ++index)
      {
        string data = folder.files[index].data;
        if (data.Contains(targetIP) && (data.Contains("FileCopied") || data.Contains("FileDeleted") || data.Contains("FileMoved")))
          return true;
      }
      return false;
    }

    internal static void TriggerETAS(object osobj)
    {
      ((OS) osobj).timerExpired();
    }

    internal static void FlagNextForkbombCompletionToTrace(string source)
    {
      TrackerCompleteSequence.NextCompleteForkbombShouldTrace = true;
      TrackerCompleteSequence.ForkbombCompleteTraceIP = source;
    }
  }
}
