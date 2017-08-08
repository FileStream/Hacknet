// Decompiled with JetBrains decompiler
// Type: Hacknet.ShellOverloaderExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  internal static class ShellOverloaderExe
  {
    public static void RunShellOverloaderExe(string[] args, object osObj, Computer target)
    {
      OS os = (OS) osObj;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      if (args.Length > 1)
      {
        if (args[1].ToLower() == "-c")
        {
          flag1 = false;
          flag2 = false;
          flag3 = true;
        }
        else if (args[1].ToLower() == "-o")
        {
          flag1 = false;
          flag2 = true;
        }
        else if (args[1].ToLower() == "-e")
        {
          flag1 = true;
          flag2 = false;
          flag3 = false;
        }
      }
      if (!flag2 && !flag1 && !flag3)
      {
        os.write("--------------------------------------");
        os.write("ConShell " + LocaleTerms.Loc("ERROR: Not enough arguments!"));
        os.write(LocaleTerms.Loc("Usage:") + " ConShell [-" + LocaleTerms.Loc("option") + "]");
        os.write(LocaleTerms.Loc("Valid Options:") + " [-e (" + LocaleTerms.Loc("Exit") + ")] [-o (" + LocaleTerms.Loc("Overload") + ")] [-c (" + LocaleTerms.Loc("Cancel Overload") + ")]");
        os.write("--------------------------------------");
      }
      else if (os.exes.Count <= 0)
      {
        os.write("--------------------------------------");
        os.write("ConShell " + LocaleTerms.Loc("ERROR: No active shells"));
        os.write("--------------------------------------");
      }
      else
      {
        for (int index = 0; index < os.exes.Count; ++index)
        {
          ShellExe ex = os.exes[index] as ShellExe;
          if (ex != null)
          {
            if (flag1)
              ex.Completed();
            else if (flag3)
              ex.cancelTarget();
            else if (flag2)
              ex.StartOverload();
          }
        }
      }
    }
  }
}
