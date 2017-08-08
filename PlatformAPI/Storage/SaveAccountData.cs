// Decompiled with JetBrains decompiler
// Type: Hacknet.PlatformAPI.Storage.SaveAccountData
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet.PlatformAPI.Storage
{
  public struct SaveAccountData
  {
    private static string[] Delimiter = new string[2]{ "\r\n__", "\n__" };
    public string Username;
    public string Password;
    public string FileUsername;
    public DateTime LastWriteTime;

    public static SaveAccountData ParseFromString(string input)
    {
      string[] strArray = input.Split(SaveAccountData.Delimiter, StringSplitOptions.None);
      string str = "";
      if (!string.IsNullOrEmpty(strArray[1]))
      {
        try
        {
          str = FileEncrypter.DecryptString(strArray[1], "")[2];
        }
        catch (FormatException ex)
        {
          Console.WriteLine("ACCOUNT AUTHENTICATION DETAILS CORRUPT : " + strArray[0]);
          str = "";
        }
        catch (NullReferenceException ex)
        {
          Console.WriteLine("ACCOUNT AUTHENTICATION DETAILS REMOVED OR DELETED : " + strArray[0]);
          str = "";
        }
      }
      if (strArray.Length <= 3)
        return new SaveAccountData() { Username = strArray[0], Password = str, FileUsername = strArray[2] };
      return new SaveAccountData() { Username = strArray[0], Password = str, LastWriteTime = Utils.SafeParseDateTime(strArray[2]), FileUsername = strArray[3] };
    }

    public string Serialize()
    {
      return "" + this.Username + SaveAccountData.Delimiter[0] + FileEncrypter.EncryptString(this.Password, "", "", "", (string) null) + SaveAccountData.Delimiter[0] + Utils.SafeWriteDateTime(this.LastWriteTime) + SaveAccountData.Delimiter[0] + this.FileUsername;
    }
  }
}
