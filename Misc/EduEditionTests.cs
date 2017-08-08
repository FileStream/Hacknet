// Decompiled with JetBrains decompiler
// Type: Hacknet.Misc.EduEditionTests
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet.Misc
{
  public static class EduEditionTests
  {
    public static string TestEDUFunctionality(ScreenManager screenMan, out int errorsAdded)
    {
      string str1 = "";
      int errorsAdded1 = 0;
      int num1 = 0;
      bool educationSafeBuild = Settings.EducationSafeBuild;
      string str2 = str1 + EduEditionTests.TestEduSafeFileFlags(screenMan, out errorsAdded1);
      int num2 = num1 + errorsAdded1;
      string str3 = str2 + (errorsAdded1 > 0 ? (object) "\r\n" : (object) " ") + "Complete - " + (object) errorsAdded1 + " errors found";
      errorsAdded = num2;
      Settings.EducationSafeBuild = educationSafeBuild;
      return str3;
    }

    public static string TestEduSafeFileFlags(ScreenManager screenMan, out int errorsAdded)
    {
      int num = 0;
      string str = "";
      Settings.EducationSafeBuild = true;
      Computer computer1 = (Computer) ComputerLoader.loadComputer("Content/Tests/TestComputer.xml", false, false);
      Settings.EducationSafeBuild = false;
      Computer computer2 = (Computer) ComputerLoader.loadComputer("Content/Tests/TestComputer.xml", false, false);
      Folder folder1 = computer1.files.root.searchForFolder("testfolder");
      Folder folder2 = computer2.files.root.searchForFolder("testfolder");
      if (folder1.containsFile("eduUnsafeFile.txt") || !folder1.containsFile("eduSafeFile.txt") || !folder1.containsFile("eduSafeExplicit.txt") || !folder1.containsFile("eduSafeOnlyFile.txt"))
      {
        ++num;
        str += "\nError in Education File Flags - EDU Safe version has invalid file set";
      }
      if (!folder2.containsFile("eduUnsafeFile.txt") || !folder2.containsFile("eduSafeFile.txt") || !folder2.containsFile("eduSafeExplicit.txt") || folder2.containsFile("eduSafeOnlyFile.txt"))
      {
        ++num;
        str += "\nError in Education File Flags - EDU Unsafe version has invalid file set";
      }
      errorsAdded = num;
      return str;
    }
  }
}
