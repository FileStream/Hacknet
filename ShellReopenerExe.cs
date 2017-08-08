// Decompiled with JetBrains decompiler
// Type: Hacknet.ShellReopenerExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Text;

namespace Hacknet
{
  internal static class ShellReopenerExe
  {
    private const string Filename = "ShellSources.txt";

    public static void RunShellReopenerExe(string[] args, object osObj, Computer target)
    {
      OS os = (OS) osObj;
      bool flag1 = false;
      bool flag2 = false;
      if (args.Length > 1)
      {
        if (args[1].ToLower() == "-s")
          flag2 = true;
        else if (args[1].ToLower() == "-o")
          flag1 = true;
      }
      if (!flag1 && !flag2)
      {
        os.write("--------------------------------------");
        os.write("OpShell " + LocaleTerms.Loc("ERROR: Not enough arguments!"));
        os.write(LocaleTerms.Loc("Usage:") + " OpShell [-" + LocaleTerms.Loc("option") + "]");
        os.write(LocaleTerms.Loc("Valid Options:") + " [-s (" + LocaleTerms.Loc("Save state") + ")] [-o (" + LocaleTerms.Loc("Re-open") + ")]");
        os.write("--------------------------------------");
      }
      else
      {
        Folder folder = os.thisComputer.files.root.searchForFolder("sys");
        FileEntry fileEntry = folder.searchForFile("ShellSources.txt");
        List<ShellExe> shellExeList = new List<ShellExe>();
        for (int index = 0; index < os.exes.Count; ++index)
        {
          ShellExe ex = os.exes[index] as ShellExe;
          if (ex != null)
            shellExeList.Add(ex);
        }
        if (flag1)
        {
          if (fileEntry == null)
          {
            os.write("--------------------------------------");
            os.write("OpShell " + LocaleTerms.Loc("ERROR: No shell sources saved. Save a setup first."));
            os.write("--------------------------------------");
          }
          else
          {
            string[] lines = fileEntry.data.Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries);
            double time1 = 0.2;
            os.runCommand("disconnect");
            for (int index1 = 1; index1 < lines.Length; ++index1)
            {
              int index = index1;
              os.delayer.Post(ActionDelayer.Wait(time1), (Action) (() => os.runCommand("connect " + lines[index])));
              double time2 = time1 + 0.2;
              os.delayer.Post(ActionDelayer.Wait(time2), (Action) (() => os.runCommand("shell")));
              time1 = time2 + 0.2;
            }
            os.delayer.Post(ActionDelayer.Wait(time1), (Action) (() =>
            {
              os.runCommand("disconnect");
              os.write("--------------------------------------");
              os.write("OpShell : " + LocaleTerms.Loc("Operation complete - ran shell on " + (object) (lines.Length - 1) + " nodes"));
              os.write("--------------------------------------");
            }));
          }
        }
        else if (flag2)
        {
          if (shellExeList.Count <= 0)
          {
            os.write("--------------------------------------");
            os.write("OpShell " + LocaleTerms.Loc("ERROR: No active shells"));
            os.write("--------------------------------------");
          }
          else
          {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("#OpShell_IP_SourceCache\n");
            for (int index = 0; index < shellExeList.Count; ++index)
              stringBuilder.Append(shellExeList[index].targetIP + "\n");
            if (fileEntry != null)
              fileEntry.data = stringBuilder.ToString();
            else
              folder.files.Add(new FileEntry(stringBuilder.ToString(), "ShellSources.txt"));
            os.write("--------------------------------------");
            os.write("OpShell : " + string.Format(LocaleTerms.Loc("Saved {0} active shell sources successfully"), (object) shellExeList.Count));
            os.write("--------------------------------------");
          }
        }
      }
    }
  }
}
