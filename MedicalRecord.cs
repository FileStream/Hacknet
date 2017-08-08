// Decompiled with JetBrains decompiler
// Type: Hacknet.MedicalRecord
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Hacknet
{
  public class MedicalRecord
  {
    private static List<string> Allergants = (List<string>) null;
    public List<string> Visits = new List<string>();
    public List<string> Perscriptions = new List<string>();
    public List<string> Allergies = new List<string>();
    public int Height = 172;
    public string Notes = "N/A";
    public string BloodType = "AB";
    public DateTime DateofBirth;

    public MedicalRecord()
    {
    }

    public MedicalRecord(WorldLocation location, DateTime dob)
    {
      this.DateofBirth = dob;
      int days = (DateTime.Now - dob).Days;
      int num1 = 155;
      int num2 = 220;
      this.Height = num1 + (int) (Utils.random.NextDouble() * Utils.random.NextDouble() * (double) (num2 - num1));
      this.AddRandomVists(days, location);
      this.BloodType = (Utils.flipCoin() ? (int) 'A' : (Utils.flipCoin() ? (int) 'B' : (int) 'O')).ToString() + (object) (char) (Utils.flipCoin() ? (int) 'A' : (Utils.flipCoin() ? (int) 'B' : (int) 'O'));
      if (MedicalRecord.Allergants == null)
        this.LoadStatics();
      this.AddAllergies();
    }

    private void LoadStatics()
    {
      MedicalRecord.Allergants = new List<string>();
      MedicalRecord.Allergants.AddRange((IEnumerable<string>) Utils.readEntireFile("Content/PersonData/Allergies.txt").Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries));
    }

    private void AddAllergies()
    {
      int num1 = 4;
      int num2 = Math.Max(0, Utils.random.Next(0, num1 + 2) - 2);
      for (int index = 0; index < num2; ++index)
      {
        string allergant;
        do
        {
          allergant = MedicalRecord.Allergants[Utils.random.Next(0, MedicalRecord.Allergants.Count - 1)];
        }
        while (this.Allergies.Contains(allergant));
        this.Allergies.Add(allergant);
      }
    }

    private void AddRandomVists(int daysOld, WorldLocation loc)
    {
      int num1 = (int) (Utils.random.NextDouble() * Utils.random.NextDouble() * 10.0);
      int maxValue = daysOld / 2;
      int num2 = daysOld;
      int num3 = 0;
      for (int index = 0; index < num1; ++index)
      {
        int num4 = Utils.random.Next(1, maxValue);
        this.Visits.Add((this.DateofBirth + TimeSpan.FromDays((double) (num3 + num4) - Utils.random.NextDouble())).ToString() + " - " + (Utils.random.NextDouble() < 0.7 ? loc.name : loc.country) + (Utils.flipCoin() ? " Public" : " Private") + " Hospital");
        num2 -= num4;
        num3 += num4;
        maxValue = (int) ((double) num2 * 0.7);
      }
    }

    public override string ToString()
    {
      CultureInfo cultureInfo = new CultureInfo("en-au");
      string str1 = "Medical Record\n" + "Date of Birth :: " + Utils.SafeWriteDateTime(this.DateofBirth);
      TimeSpan timeSpan = DateTime.Now - this.DateofBirth;
      string str2 = str1 + "\nBlood Type :: " + this.BloodType;
      double num = 25.0 / 762.0 * (double) this.Height;
      string str3 = str2 + "\nHeight :: " + (object) this.Height + "cm (" + (object) (int) num + "\"" + (object) (int) (num % 1.0 * 12.0) + "')" + "\nAllergies :: " + this.GetCSVFromList(this.Allergies) + "\nActive Prescriptions :: ";
      string str4;
      if (this.Perscriptions.Count == 0)
      {
        str4 = str3 + "NONE";
      }
      else
      {
        str4 = str3 + "x" + (object) this.Perscriptions.Count + " Active";
        for (int index = 0; index < this.Perscriptions.Count; ++index)
          str4 = str4 + "\n" + this.Perscriptions[index];
      }
      string str5 = str4 + "\nRecorded Visits ::";
      if (this.Visits.Count == 0)
      {
        str5 += "NONE RECORDED\n";
      }
      else
      {
        for (int index = 0; index < this.Visits.Count; ++index)
          str5 = str5 + this.Visits[index] + "\n";
      }
      return str5 + "Notes :: " + this.Notes + "\n";
    }

    public static MedicalRecord Load(XmlReader rdr, WorldLocation location, DateTime dob)
    {
      MedicalRecord medicalRecord = new MedicalRecord(location, dob);
      while (rdr.Name != "Medical")
        rdr.Read();
      rdr.Read();
      while (!(rdr.Name == "Medical") || rdr.IsStartElement())
      {
        if (rdr.IsStartElement())
        {
          switch (rdr.Name)
          {
            case "Blood":
              medicalRecord.BloodType = rdr.ReadElementContentAsString();
              break;
            case "Height":
              medicalRecord.Height = rdr.ReadElementContentAsInt();
              break;
            case "Allergies":
              medicalRecord.Allergies.Clear();
              medicalRecord.Allergies.AddRange((IEnumerable<string>) rdr.ReadElementContentAsString().Split(new char[1]
              {
                ','
              }, StringSplitOptions.RemoveEmptyEntries));
              break;
            case "Perscription":
              medicalRecord.Perscriptions.Add(rdr.ReadElementContentAsString());
              break;
            case "Notes":
              medicalRecord.Notes = rdr.ReadElementContentAsString();
              break;
          }
        }
        rdr.Read();
      }
      return medicalRecord;
    }

    private string GetCSVFromList(List<string> list)
    {
      if (list.Count <= 0)
        return "NONE";
      string str = "";
      for (int index = 0; index < list.Count; ++index)
        str = str + list[index] + ",";
      return str.Substring(0, str.Length - 1);
    }
  }
}
