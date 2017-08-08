// Decompiled with JetBrains decompiler
// Type: Hacknet.People
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Hacknet
{
  public static class People
  {
    public static bool PeopleWereGeneratedWithDLCAdditions = false;
    private const int NUMBER_OF_PEOPLE = 200;
    private const int NUMBER_OF_HACKERS = 10;
    private const int NUMBER_OF_HUB_AGENTS = 22;
    public static List<Person> all;
    public static List<Person> hackers;
    public static List<Person> hubAgents;
    public static string[] maleNames;
    public static string[] femaleNames;
    public static string[] surnames;

    public static void init()
    {
      People.maleNames = Utils.readEntireFile("Content/PersonData/MaleNames.txt").Replace("\r", "").Split(Utils.newlineDelim);
      People.femaleNames = Utils.readEntireFile("Content/PersonData/FemaleNames.txt").Replace("\r", "").Split(Utils.newlineDelim);
      People.surnames = Utils.readEntireFile("Content/PersonData/Surnames.txt").Replace("\r", "").Split(Utils.newlineDelim);
      People.all = new List<Person>(200);
      int num = 0;
      FileInfo[] files = new DirectoryInfo("Content/People").GetFiles("*.xml");
      List<string> stringList = new List<string>();
      for (int index = 0; index < files.Length; ++index)
        stringList.Add("Content/People/" + Path.GetFileName(files[index].Name));
      if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed)
      {
        foreach (FileSystemInfo file in new DirectoryInfo("Content/DLC/People").GetFiles("*.xml"))
          stringList.Add("Content/DLC/People/" + Path.GetFileName(file.Name));
        People.PeopleWereGeneratedWithDLCAdditions = true;
      }
      for (int index = 0; index < stringList.Count; ++index)
      {
        Person person = People.loadPersonFromFile(LocalizedFileLoader.GetLocalizedFilepath(stringList[index]));
        if (person != null)
        {
          People.all.Add(person);
          ++num;
        }
        else
          Console.WriteLine("Person Load Error: " + stringList[index]);
      }
      for (int index = num; index < 200; ++index)
      {
        bool male = Utils.flipCoin();
        string fName = male ? People.maleNames[Utils.random.Next(People.maleNames.Length)] : People.femaleNames[Utils.random.Next(People.femaleNames.Length)];
        string surname = People.surnames[Utils.random.Next(People.surnames.Length)];
        People.all.Add(new Person(fName, surname, male, false, UsernameGenerator.getName()));
      }
      People.hackers = new List<Person>();
      People.generatePeopleForList(People.hackers, 10, true);
      People.hubAgents = new List<Person>();
      People.generatePeopleForList(People.hubAgents, 22, true);
    }

    public static void LoadInDLCPeople()
    {
      if (!Settings.EnableDLC || !DLC1SessionUpgrader.HasDLC1Installed)
        return;
      foreach (FileSystemInfo file in new DirectoryInfo("Content/DLC/People").GetFiles("*.xml"))
      {
        Person person = People.loadPersonFromFile(LocalizedFileLoader.GetLocalizedFilepath("Content/DLC/People/" + Path.GetFileName(file.Name)));
        if (person != null)
          People.all.Insert(0, person);
      }
    }

    public static void ReInitPeopleForExtension()
    {
      People.all.Clear();
      People.hackers.Clear();
      People.hubAgents.Clear();
      int num = 0;
      string path = Path.Combine(Utils.GetFileLoadPrefix(), "People");
      if (Directory.Exists(path))
      {
        foreach (FileSystemInfo file in new DirectoryInfo(path).GetFiles("*.xml"))
        {
          Person person = People.loadPersonFromFile(LocalizedFileLoader.GetLocalizedFilepath(path + "/" + Path.GetFileName(file.Name)));
          if (person != null)
          {
            People.all.Insert(0, person);
            ++num;
          }
        }
      }
      for (int index = num; index < 200; ++index)
      {
        bool male = Utils.flipCoin();
        string fName = male ? People.maleNames[Utils.random.Next(People.maleNames.Length)] : People.femaleNames[Utils.random.Next(People.femaleNames.Length)];
        string surname = People.surnames[Utils.random.Next(People.surnames.Length)];
        People.all.Add(new Person(fName, surname, male, false, UsernameGenerator.getName()));
      }
      People.hackers = new List<Person>();
      People.generatePeopleForList(People.hackers, 10, true);
      People.hubAgents = new List<Person>();
      People.generatePeopleForList(People.hubAgents, 22, true);
    }

    private static void generatePeopleForList(List<Person> list, int numberToGenerate, bool areHackers = false)
    {
      for (int index = 0; index < numberToGenerate; ++index)
      {
        bool male = Utils.flipCoin();
        if (areHackers)
          male = !male || !Utils.flipCoin();
        Person person = new Person(male ? People.maleNames[Utils.random.Next(People.maleNames.Length)] : People.femaleNames[Utils.random.Next(People.femaleNames.Length)], People.surnames[Utils.random.Next(People.surnames.Length)], male, areHackers, UsernameGenerator.getName());
        list.Add(person);
        People.all.Add(person);
      }
    }

    public static Person loadPersonFromFile(string path)
    {
      try
      {
        using (FileStream fileStream = new FileStream(path, FileMode.Open))
        {
          XmlReader rdr = XmlReader.Create((Stream) fileStream, new XmlReaderSettings());
          while (rdr.Name != "Person")
            rdr.Read();
          string str1;
          string lName = str1 = "unknown";
          string fName = str1;
          string handle = str1;
          string str2 = str1;
          bool male = true;
          bool isHacker = false;
          bool flag = false;
          if (rdr.MoveToAttribute("id"))
            str2 = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("handle"))
            handle = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("firstName"))
            fName = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("lastName"))
            lName = rdr.ReadContentAsString();
          if (rdr.MoveToAttribute("isMale"))
            male = rdr.ReadContentAsBoolean();
          if (rdr.MoveToAttribute("isHacker"))
            isHacker = rdr.ReadContentAsBoolean();
          if (rdr.MoveToAttribute("forceHasNeopals"))
            flag = rdr.ReadContentAsBoolean();
          Person person = new Person(fName, lName, male, isHacker, handle);
          if (person.NeopalsAccount == null && flag && DLC1SessionUpgrader.HasDLC1Installed)
            person.NeopalsAccount = NeopalsAccount.GenerateAccount(person.handle, Utils.flipCoin());
          rdr.Read();
          while (!(rdr.Name == "Person") || rdr.IsStartElement())
          {
            switch (rdr.Name)
            {
              case "Degrees":
                List<Degree> degreeList = new List<Degree>();
                rdr.Read();
                while (!(rdr.Name == "Degrees") || rdr.IsStartElement())
                {
                  if (rdr.Name == "Degree")
                  {
                    string str3;
                    string uniName = str3 = "UNKNOWN";
                    double num = 3.0;
                    if (rdr.MoveToAttribute("uni"))
                      uniName = rdr.ReadContentAsString();
                    if (rdr.MoveToAttribute("gpa"))
                      num = rdr.ReadContentAsDouble();
                    int content = (int) rdr.MoveToContent();
                    Degree degree = new Degree(rdr.ReadElementContentAsString(), uniName, (float) num);
                    degreeList.Add(degree);
                  }
                  rdr.Read();
                }
                if (degreeList.Count > 0)
                {
                  person.degrees = degreeList;
                  break;
                }
                break;
              case "Birthplace":
                string name = (string) null;
                if (rdr.MoveToAttribute("name"))
                  name = rdr.ReadContentAsString();
                if (name == null)
                  name = WorldLocationLoader.getRandomLocation().name;
                person.birthplace = WorldLocationLoader.getClosestOrCreate(name);
                break;
              case "DOB":
                CultureInfo cultureInfo = new CultureInfo("en-au");
                int content1 = (int) rdr.MoveToContent();
                DateTime dateTime = Utils.SafeParseDateTime(rdr.ReadElementContentAsString());
                if (dateTime.Hour == 0 && dateTime.Second == 0)
                {
                  TimeSpan timeSpan = TimeSpan.FromHours(Utils.random.NextDouble() * 23.99);
                  dateTime += timeSpan;
                }
                person.DateOfBirth = dateTime;
                break;
              case "Medical":
                person.medicalRecord = MedicalRecord.Load(rdr, person.birthplace, person.DateOfBirth);
                break;
            }
            rdr.Read();
          }
          if (DLC1SessionUpgrader.HasDLC1Installed)
          {
            if (person.handle == "Minx" && person.NeopalsAccount == null)
              person.NeopalsAccount = NeopalsAccount.GenerateAccount("Minx", false);
            if (person.handle == "Orann" && person.NeopalsAccount == null && DLC1SessionUpgrader.HasDLC1Installed && People.PeopleWereGeneratedWithDLCAdditions)
              person.NeopalsAccount = NeopalsAccount.GenerateAccount("Orann", false);
          }
          return person;
        }
      }
      catch (FileNotFoundException ex)
      {
        return (Person) null;
      }
    }

    public static void printAllPeople()
    {
      for (int index = 0; index < People.all.Count; ++index)
        Console.WriteLine("------------------------------------------\n" + People.all[index].ToString());
    }
  }
}
