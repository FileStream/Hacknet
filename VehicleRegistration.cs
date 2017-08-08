// Decompiled with JetBrains decompiler
// Type: Hacknet.VehicleRegistration
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public class VehicleRegistration
  {
    public VehicleType vehicle;
    public string licencePlate;
    public string licenceNumber;

    public VehicleRegistration()
    {
    }

    public VehicleRegistration(VehicleType vehicleType, string plate, string regNumber)
    {
      this.vehicle = vehicleType;
      this.licencePlate = plate;
      this.licenceNumber = regNumber;
    }

    public override string ToString()
    {
      return this.vehicle.maker + " " + this.vehicle.model + " | Plate: " + this.licencePlate + " Licence: " + this.licenceNumber;
    }
  }
}
