// Decompiled with JetBrains decompiler
// Type: Hacknet.Helpfile
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System.Collections.Generic;
using System.Text;

namespace Hacknet
{
  internal class Helpfile
  {
    private static int ITEMS_PER_PAGE = 10;
    public static string prefix = "---------------------------------\n" + LocaleTerms.Loc("Command List - Page [PAGENUM] of [TOTALPAGES]") + ":\n";
    private static string postfix = "help [PAGE NUMBER]\n " + LocaleTerms.Loc("Displays the specified page of commands.") + "\n---------------------------------\n";
    private static string LoadedLanguage = "en-us";
    public static List<string> help;

    public static void init()
    {
      Helpfile.help = new List<string>();
      string newValue = "\n    ";
      Helpfile.help.Add("help [PAGE NUMBER]" + newValue + LocaleTerms.Loc("Displays the specified page of commands.").Replace("\n", newValue));
      Helpfile.help.Add("scp [filename] [OPTIONAL: destination]" + newValue + LocaleTerms.Loc("Copies file named [filename] from remote machine to specified local folder (/bin default)").Replace("\n", newValue));
      Helpfile.help.Add("scan" + newValue + LocaleTerms.Loc("Scans for links on the connected machine and adds them to the Map").Replace("\n", newValue));
      Helpfile.help.Add("rm [filename (or use * for all files in folder)]" + newValue + LocaleTerms.Loc("Deletes specified file(s)").Replace("\n", newValue));
      Helpfile.help.Add("ps" + newValue + LocaleTerms.Loc("Lists currently running processes and their PIDs").Replace("\n", newValue));
      Helpfile.help.Add("kill [PID]" + newValue + LocaleTerms.Loc("Kills Process number [PID]").Replace("\n", newValue));
      Helpfile.help.Add("ls" + newValue + LocaleTerms.Loc("Lists all files in current directory").Replace("\n", newValue));
      Helpfile.help.Add("cd [foldername]" + newValue + LocaleTerms.Loc("Moves current working directory to the specified folder").Replace("\n", newValue));
      Helpfile.help.Add("mv [FILE] [DESTINATION]" + newValue + LocaleTerms.Loc("Moves or renames [FILE] to [DESTINATION]").Replace("\n", newValue) + newValue + "(i.e: mv hi.txt ../bin/hi.txt)");
      Helpfile.help.Add("connect [ip]" + newValue + LocaleTerms.Loc("Connect to an External Computer").Replace("\n", newValue));
      Helpfile.help.Add("probe" + newValue + string.Format(LocaleTerms.Loc("Scans the connected machine for{0}active ports and security level"), (object) newValue).Replace("\n", newValue));
      Helpfile.help.Add("exe" + newValue + LocaleTerms.Loc("Lists all available executables in the local /bin/ folder (Includes hidden and embedded executables)").Replace("\n", newValue));
      Helpfile.help.Add("disconnect" + newValue + LocaleTerms.Loc("Terminate the current open connection.").Replace("\n", newValue) + " ALT: \"dc\"");
      Helpfile.help.Add("cat [filename]" + newValue + LocaleTerms.Loc("Displays contents of file").Replace("\n", newValue));
      Helpfile.help.Add("openCDTray" + newValue + LocaleTerms.Loc("Opens the connected Computer's CD Tray").Replace("\n", newValue));
      Helpfile.help.Add("closeCDTray" + newValue + LocaleTerms.Loc("Closes the connected Computer's CD Tray").Replace("\n", newValue));
      Helpfile.help.Add("reboot [OPTIONAL: -i]" + newValue + LocaleTerms.Loc("Reboots the connected computer. The -i flag reboots instantly").Replace("\n", newValue));
      Helpfile.help.Add("replace [filename] \"target\" \"replacement\"" + newValue + LocaleTerms.Loc("Replaces the target text in the file with the replacement").Replace("\n", newValue));
      Helpfile.help.Add("analyze" + newValue + LocaleTerms.Loc("Performs an analysis pass on the firewall of the target machine").Replace("\n", newValue));
      Helpfile.help.Add("solve [FIREWALL SOLUTION]" + newValue + LocaleTerms.Loc("Attempts to solve the firewall of target machine to allow UDP Traffic").Replace("\n", newValue));
      Helpfile.help.Add("login" + newValue + LocaleTerms.Loc("Requests a username and password to log in to the connected system").Replace("\n", newValue));
      Helpfile.help.Add("upload [LOCAL FILE PATH]" + newValue + LocaleTerms.Loc("Uploads the indicated file on your local machine to the current connected directory").Replace("\n", newValue));
      Helpfile.help.Add("clear" + newValue + LocaleTerms.Loc("Clears the terminal").Replace("\n", newValue));
      Helpfile.help.Add("addNote [NOTE]" + newValue + LocaleTerms.Loc("Add Note").Replace("\n", newValue));
      Helpfile.help.Add("append [FILENAME] [DATA]" + newValue + LocaleTerms.Loc("Appends a line containing [DATA] to [FILENAME]").Replace("\n", newValue));
      Helpfile.help.Add("shell" + newValue + LocaleTerms.Loc("Opens a remote access shell on target machine with Proxy overload\n and IP trap capabilities").Replace("\n", newValue));
      Helpfile.LoadedLanguage = Settings.ActiveLocale;
    }

    public static void writeHelp(OS os, int page = 0)
    {
      if (Helpfile.LoadedLanguage != Settings.ActiveLocale)
        Helpfile.init();
      if (page == 0)
        page = 1;
      int num = (page - 1) * Helpfile.ITEMS_PER_PAGE;
      if (num >= Helpfile.help.Count)
        num = 0;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(Helpfile.prefix.Replace("[PAGENUM]", string.Concat((object) page)).Replace("[TOTALPAGES]", string.Concat((object) Helpfile.getNumberOfPages())) + "\n");
      for (int index = num; index < Helpfile.help.Count && index < num + Helpfile.ITEMS_PER_PAGE; ++index)
        stringBuilder.Append((index == 0 ? " " : "") + Helpfile.help[index] + "\n  \n ");
      os.write(stringBuilder.ToString() + "\n" + Helpfile.postfix);
    }

    public static int getNumberOfPages()
    {
      return Helpfile.help.Count / Helpfile.ITEMS_PER_PAGE + 1;
    }
  }
}
