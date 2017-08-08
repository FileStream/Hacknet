// Decompiled with JetBrains decompiler
// Type: Hacknet.Person
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet
{
  public class Person
  {
    public DateTime DateOfBirth = DateTime.Now;
    public bool isMale = true;
    public bool isHacker = false;
    public string firstName;
    public string lastName;
    public string handle;
    public List<Degree> degrees;
    public List<VehicleRegistration> vehicles;
    public WorldLocation birthplace;
    public MedicalRecord medicalRecord;
    public NeopalsAccount NeopalsAccount;

    public string FullName
    {
      get
      {
        return this.firstName + " " + this.lastName;
      }
    }

    public Person()
    {
    }

    public Person(string fName, string lName, bool male, bool isHacker = false, string handle = null)
    {
      this.firstName = fName;
      this.lastName = lName;
      this.isMale = male;
      if (handle == null)
        handle = UsernameGenerator.getName();
      if (handle == null)
        throw new InvalidOperationException();
      this.handle = handle;
      this.isHacker = isHacker;
      this.birthplace = WorldLocationLoader.getRandomLocation();
      this.vehicles = new List<VehicleRegistration>();
      this.degrees = new List<Degree>();
      this.addRandomDegrees();
      this.addRandomVehicles();
      int num1 = 18;
      int num2 = 72;
      if (isHacker)
        num2 = 45;
      this.DateOfBirth = DateTime.Now - TimeSpan.FromDays((double) (num1 * 365 + (int) (Utils.random.NextDouble() * (double) (num2 - num1) * 365.0)));
      this.medicalRecord = new MedicalRecord(this.birthplace, this.DateOfBirth);
      if (!Settings.EnableDLC || !DLC1SessionUpgrader.HasDLC1Installed || !isHacker && (double) Utils.randm(1f) >= 0.800000011920929)
        return;
      this.NeopalsAccount = NeopalsAccount.GenerateAccount(handle, Utils.flipCoin() && Utils.flipCoin() && isHacker && handle.ToLower() != "bit");
    }

    public void addRandomDegrees()
    {
      double num = 0.6;
      if (this.isHacker)
        num = 0.9;
      while (Utils.random.NextDouble() < num)
      {
        if (this.isHacker)
          this.degrees.Add(PeopleAssets.getRandomHackerDegree(this.birthplace));
        else
          this.degrees.Add(PeopleAssets.getRandomDegree(this.birthplace));
        num *= num;
        if (this.isHacker)
          num *= 0.36;
      }
    }

    public void addRandomVehicles()
    {
      double num = 0.7;
      while (Utils.random.NextDouble() < num)
      {
        this.vehicles.Add(VehicleInfo.getRandomRegistration());
        num *= num;
        if (this.isHacker)
          num *= num;
      }
    }

    public override string ToString()
    {
      return this.firstName + " " + this.lastName + "\n Gender: " + (this.isMale ? "Male  " : "Female  ") + "Born: " + this.birthplace.ToString() + "\n" + this.getDegreeString() + " " + this.getVehicleRegString() + "\n" + this.medicalRecord.ToString();
    }

    private string getDegreeString()
    {
      string str = "Degrees:\n";
      for (int index = 0; index < this.degrees.Count; ++index)
        str = str + " -" + this.degrees[index].ToString() + "\n";
      return str;
    }

    private string getVehicleRegString()
    {
      string str = "Vehicle Registrations:\n";
      for (int index = 0; index < this.vehicles.Count; ++index)
        str = str + " -" + this.vehicles[index].ToString() + "\n";
      return str;
    }
  }
}
