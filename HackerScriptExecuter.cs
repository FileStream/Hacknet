// Decompiled with JetBrains decompiler
// Type: Hacknet.HackerScriptExecuter
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Hacknet
{
  public class HackerScriptExecuter
  {
    public const string splitDelimiter = " $#%#$\r\n";
    private static SoundEffect MusicStopSFX;

    public static void runScript(string scriptName, object os, string sourceCompReplacer = null, string targetCompReplacer = null)
    {
      string[] separator = new string[4]{ " $#%#$\r\n", " $#%#$\n", "$#%#$\r\n", "$#%#$\n" };
      string localizedFilepath = LocalizedFileLoader.GetLocalizedFilepath(Utils.GetFileLoadPrefix() + scriptName);
      if (!File.Exists(localizedFilepath))
        localizedFilepath = LocalizedFileLoader.GetLocalizedFilepath("Content/" + scriptName);
      string str = Utils.readEntireFile(localizedFilepath);
      if (sourceCompReplacer != null)
        str = str.Replace("[SOURCE_COMP]", sourceCompReplacer);
      if (targetCompReplacer != null)
        str = str.Replace("[TARGET_COMP]", targetCompReplacer);
      string[] script = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      if (!OS.TestingPassOnly)
      {
        Thread thread = new Thread((ThreadStart) (() => HackerScriptExecuter.executeThreadedScript(script, (OS) os))) { IsBackground = true, CurrentCulture = Game1.culture, CurrentUICulture = Game1.culture };
        thread.IsBackground = true;
        thread.Name = "OpposingHackerThread";
        thread.Start();
      }
      else
        HackerScriptExecuter.executeThreadedScript(script, (OS) os);
    }

    private static void executeThreadedScript(string[] script, OS os)
    {
      KeyValuePair<string, string>? nullable = new KeyValuePair<string, string>?();
      bool flag1 = false;
      Computer target = os.thisComputer;
      Computer source = (Computer) null;
      TimeSpan timeout = TimeSpan.FromSeconds(0.5);
      for (int index1 = 0; index1 < script.Length; ++index1)
      {
        if (source != null && source.disabled)
        {
          Multiplayer.parseInputMessage(HackerScriptExecuter.getBasicNetworkCommand("cDisconnect", target, source), os);
          Console.WriteLine("Early Script Exit on Source Disable");
          return;
        }
        if (!string.IsNullOrWhiteSpace(script[index1]))
        {
          string[] strArray = script[index1].Trim().Split(Utils.spaceDelim, StringSplitOptions.RemoveEmptyEntries);
          CultureInfo cultureInfo = new CultureInfo("en-au");
          bool flag2 = target == os.thisComputer;
          try
          {
            switch (strArray[0])
            {
              case "config":
                target = Programs.getComputer(os, strArray[1]);
                if (target == null)
                {
                  if (!OS.DEBUG_COMMANDS)
                    return;
                  os.write(" ");
                  os.write("Error: ");
                  os.write("Hack Script target " + strArray[1] + " not found! Aborting.");
                  os.write("This error will not show up if debug commands are disabled.");
                  os.write(" ");
                  return;
                }
                source = Programs.getComputer(os, strArray[2]);
                if (source == null)
                {
                  if (!OS.DEBUG_COMMANDS)
                    return;
                  os.write(" ");
                  os.write("Error: ");
                  os.write("Hack Script source " + strArray[2] + " not found! Aborting.");
                  os.write("This error will not show up if debug commands are disabled.");
                  os.write(" ");
                  return;
                }
                timeout = TimeSpan.FromSeconds(Convert.ToDouble(strArray[3], (IFormatProvider) cultureInfo));
                flag2 = false;
                nullable = new KeyValuePair<string, string>?(new KeyValuePair<string, string>(source.ip, target.ip));
                os.ActiveHackers.Add(nullable.Value);
                break;
              case "delay":
                if (!OS.TestingPassOnly)
                  Thread.Sleep(TimeSpan.FromSeconds(Convert.ToDouble(strArray[1], (IFormatProvider) cultureInfo)));
                flag2 = false;
                break;
              case "connect":
                Multiplayer.parseInputMessage(HackerScriptExecuter.getBasicNetworkCommand("cConnection", target, source), os);
                if (!flag1 && target.ip == os.thisComputer.ip)
                {
                  os.IncConnectionOverlay.Activate();
                  flag1 = true;
                  break;
                }
                break;
              case "openPort":
                Multiplayer.parseInputMessage(HackerScriptExecuter.getBasicNetworkCommand("cPortOpen", target, source) + " " + strArray[1], os);
                break;
              case "delete":
                string pathString = HackerScriptExecuter.getPathString(strArray[1], os, target.files.root);
                Multiplayer.parseInputMessage("cDelete #" + target.ip + "#" + source.ip + "#" + strArray[2] + pathString, os);
                break;
              case "reboot":
                if (target == os.thisComputer)
                {
                  if (os.connectedComp == null || os.connectedComp == os.thisComputer)
                  {
                    os.runCommand("reboot");
                    break;
                  }
                  os.rebootThisComputer();
                  break;
                }
                target.reboot(source.ip);
                break;
              case "forkbomb":
                Multiplayer.parseInputMessage(HackerScriptExecuter.getBasicNetworkCommand("eForkBomb", target, source), os);
                break;
              case "disconnect":
                target.disconnecting(source.ip, true);
                break;
              case "systakeover":
                HostileHackerBreakinSequence.Execute((object) os, source, target);
                break;
              case "clearTerminal":
                if (target == os.thisComputer)
                {
                  os.terminal.reset();
                  break;
                }
                break;
              case "write":
                string str1 = "";
                for (int index2 = 1; index2 < strArray.Length; ++index2)
                  str1 = str1 + strArray[index2] + " ";
                string str2 = ComputerLoader.filter(str1.Trim());
                if (target == os.thisComputer)
                {
                  os.terminal.write(" " + str2);
                  os.warningFlash();
                  break;
                }
                break;
              case "write_silent":
                string str3 = "";
                for (int index2 = 1; index2 < strArray.Length; ++index2)
                  str3 = str3 + strArray[index2] + " ";
                string str4 = ComputerLoader.filter(str3.Trim());
                if (target == os.thisComputer)
                  os.terminal.write(" " + str4);
                flag2 = false;
                break;
              case "writel":
                string str5 = "";
                for (int index2 = 1; index2 < strArray.Length; ++index2)
                  str5 = str5 + strArray[index2] + " ";
                string text1 = ComputerLoader.filter(str5.Trim());
                if (string.IsNullOrWhiteSpace(text1))
                  flag2 = false;
                if (target == os.thisComputer)
                {
                  os.terminal.writeLine(text1);
                  os.warningFlash();
                  break;
                }
                break;
              case "writel_silent":
                string str6 = "";
                for (int index2 = 1; index2 < strArray.Length; ++index2)
                  str6 = str6 + strArray[index2] + " ";
                string text2 = ComputerLoader.filter(str6.Trim());
                if (string.IsNullOrWhiteSpace(text2))
                  flag2 = false;
                if (target == os.thisComputer)
                  os.terminal.writeLine(text2);
                flag2 = false;
                break;
              case "hideNetMap":
                if (target == os.thisComputer)
                {
                  os.netMap.visible = false;
                  break;
                }
                break;
              case "hideRam":
                if (target == os.thisComputer)
                {
                  os.ram.visible = false;
                  break;
                }
                break;
              case "hideDisplay":
                if (target == os.thisComputer)
                {
                  os.display.visible = false;
                  break;
                }
                break;
              case "hideTerminal":
                if (target == os.thisComputer)
                {
                  os.terminal.visible = false;
                  break;
                }
                break;
              case "showNetMap":
                if (target == os.thisComputer)
                {
                  os.netMap.visible = true;
                  break;
                }
                break;
              case "showRam":
                if (target == os.thisComputer)
                {
                  os.ram.visible = true;
                  break;
                }
                break;
              case "showTerminal":
                if (target == os.thisComputer)
                {
                  os.terminal.visible = true;
                  break;
                }
                break;
              case "showDisplay":
                if (target == os.thisComputer)
                {
                  os.display.visible = true;
                  break;
                }
                break;
              case "stopMusic":
                flag2 = false;
                if (target == os.thisComputer)
                {
                  if (HackerScriptExecuter.MusicStopSFX == null)
                    HackerScriptExecuter.MusicStopSFX = !DLC1SessionUpgrader.HasDLC1Installed ? os.content.Load<SoundEffect>("SFX/MeltImpact") : os.content.Load<SoundEffect>("DLC/SFX/GlassBreak");
                  MusicManager.stop();
                  if (HackerScriptExecuter.MusicStopSFX != null)
                    HackerScriptExecuter.MusicStopSFX.Play();
                  break;
                }
                break;
              case "startMusic":
                flag2 = false;
                if (!OS.TestingPassOnly)
                {
                  if (target == os.thisComputer)
                    MusicManager.playSong();
                  break;
                }
                break;
              case "trackseq":
                try
                {
                  if (target == os.thisComputer)
                  {
                    TrackerCompleteSequence.FlagNextForkbombCompletionToTrace(source != null ? source.ip : (string) null);
                    break;
                  }
                  break;
                }
                catch (Exception ex)
                {
                  os.write(Utils.GenerateReportFromExceptionCompact(ex));
                  break;
                }
              case "instanttrace":
                if (target == os.thisComputer)
                {
                  TrackerCompleteSequence.TriggerETAS((object) os);
                  break;
                }
                break;
              case "flash":
                if (!OS.TestingPassOnly)
                {
                  if (target == os.thisComputer)
                    os.warningFlash();
                  break;
                }
                break;
              case "openCDTray":
                if (!OS.TestingPassOnly)
                {
                  if (target == os.thisComputer)
                    target.openCDTray(source.ip);
                  break;
                }
                break;
              case "closeCDTray":
                if (!OS.TestingPassOnly)
                {
                  if (target == os.thisComputer)
                    target.closeCDTray(source.ip);
                  break;
                }
                break;
              case "setAdminPass":
                target.setAdminPassword(strArray[1]);
                break;
              case "makeFile":
                string folderName = strArray[1];
                StringBuilder stringBuilder = new StringBuilder();
                for (int index2 = 3; index2 < strArray.Length; ++index2)
                {
                  stringBuilder.Append(strArray[index2]);
                  if (index2 + 1 < strArray.Length)
                    stringBuilder.Append(" ");
                }
                Folder folder = target.files.root.searchForFolder(folderName);
                List<int> folderPath = new List<int>();
                if (folder == null)
                  folderPath.Add(0);
                else
                  folderPath.Add(target.files.root.folders.IndexOf(folder));
                target.makeFile(source.ip, strArray[2], ComputerLoader.filter(stringBuilder.ToString()), folderPath, true);
                break;
            }
          }
          catch (Exception ex)
          {
            if (OS.TestingPassOnly)
              throw new FormatException("Error Parsing command " + strArray[0] + " in HackerScript:", ex);
            if (OS.DEBUG_COMMANDS)
            {
              os.terminal.write(Utils.GenerateReportFromException(ex));
              os.write("HackScript error: " + strArray[0]);
              os.write("Report written to Warnings file");
              Utils.AppendToWarningsFile(Utils.GenerateReportFromException(ex));
            }
          }
          try
          {
            if (flag2 && !os.thisComputer.disabled)
            {
              if (!OS.TestingPassOnly)
                os.beepSound.Play();
            }
          }
          catch (Exception ex)
          {
            os.terminal.write(Utils.GenerateReportFromException(ex));
            Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
            return;
          }
          if (!OS.TestingPassOnly)
            Thread.Sleep(timeout);
        }
      }
      if (!nullable.HasValue)
        return;
      os.ActiveHackers.Remove(nullable.Value);
    }

    private static string getBasicNetworkCommand(string targetCommand, Computer target, Computer source)
    {
      return targetCommand + " " + target.ip + " " + source.ip;
    }

    private static string getPathString(string fPath, OS os, Folder f)
    {
      List<int> navigationPathAtPath = Programs.getNavigationPathAtPath(fPath, os, f);
      string str = "";
      for (int index = 0; index < navigationPathAtPath.Count; ++index)
        str = str + "#" + (object) navigationPathAtPath[index];
      return str;
    }
  }
}
