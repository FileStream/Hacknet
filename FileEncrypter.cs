// Decompiled with JetBrains decompiler
// Type: Hacknet.FileEncrypter
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Text;

namespace Hacknet
{
  public static class FileEncrypter
  {
    private static string[] HeaderSplitDelimiters = new string[1]{ "::" };

    public static string EncryptString(string data, string header, string ipLink, string pass = "", string fileExtension = null)
    {
      if (string.IsNullOrWhiteSpace(data))
        data = "";
      ushort passCodeFromString1 = FileEncrypter.GetPassCodeFromString(pass);
      ushort passCodeFromString2 = FileEncrypter.GetPassCodeFromString("");
      StringBuilder stringBuilder = new StringBuilder();
      string str = "#DEC_ENC::" + FileEncrypter.Encrypt(header, passCodeFromString2) + "::" + FileEncrypter.Encrypt(ipLink, passCodeFromString2) + "::" + FileEncrypter.Encrypt("ENCODED", passCodeFromString1);
      if (fileExtension != null)
        str = str + "::" + FileEncrypter.Encrypt(fileExtension, passCodeFromString2);
      stringBuilder.Append(str);
      stringBuilder.Append("\r\n");
      stringBuilder.Append(FileEncrypter.Encrypt(data, passCodeFromString1));
      return stringBuilder.ToString();
    }

    private static ushort GetPassCodeFromString(string code)
    {
      return (ushort) code.GetHashCode();
    }

    private static string Encrypt(string data, ushort passcode)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < data.Length; ++index)
      {
        int num = (int) data[index] * 1822 + (int) short.MaxValue + (int) passcode;
        stringBuilder.Append(num.ToString() + " ");
      }
      return stringBuilder.ToString().Trim();
    }

    private static string Decrypt(string data, ushort passcode)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string str in data.Split(Utils.spaceDelim, StringSplitOptions.RemoveEmptyEntries))
      {
        int num = (Convert.ToInt32(str) - (int) short.MaxValue - (int) passcode) / 1822;
        stringBuilder.Append((char) num);
      }
      return stringBuilder.ToString().Trim();
    }

    public static string[] DecryptString(string data, string pass = "")
    {
      if (string.IsNullOrEmpty(data))
        throw new NullReferenceException("String to decrypt cannot be null or empty");
      string[] strArray1 = new string[6];
      ushort passCodeFromString1 = FileEncrypter.GetPassCodeFromString(pass);
      ushort passCodeFromString2 = FileEncrypter.GetPassCodeFromString("");
      string[] strArray2 = data.Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries);
      if (strArray2.Length < 2)
        throw new FormatException("Tried to decrypt an invalid valid DEC ENC file \"" + data + "\" - not enough elements. Need 2 lines, had " + (object) strArray2.Length);
      string[] strArray3 = strArray2[0].Split(FileEncrypter.HeaderSplitDelimiters, StringSplitOptions.None);
      if (strArray3.Length < 4)
        throw new FormatException("Tried to decrypt an invalid valid DEC ENC file \"" + data + "\" - not enough headers");
      string str1 = FileEncrypter.Decrypt(strArray3[1], passCodeFromString2);
      string str2 = FileEncrypter.Decrypt(strArray3[2], passCodeFromString2);
      string str3 = FileEncrypter.Decrypt(strArray3[3], passCodeFromString1);
      string str4 = (string) null;
      if (strArray3.Length > 4)
        str4 = FileEncrypter.Decrypt(strArray3[4], passCodeFromString2);
      string str5 = (string) null;
      string str6 = "1";
      if (str3 == "ENCODED")
        str5 = FileEncrypter.Decrypt(strArray2[1], passCodeFromString1);
      else
        str6 = "0";
      strArray1[0] = str1;
      strArray1[1] = str2;
      strArray1[2] = str5;
      strArray1[3] = str4;
      strArray1[4] = str6;
      strArray1[5] = str3;
      return strArray1;
    }

    internal static string[] TestingDecryptString(string data, ushort pass)
    {
      if (string.IsNullOrEmpty(data))
        throw new NullReferenceException("String to decrypt cannot be null or empty");
      string[] strArray1 = new string[6];
      ushort passcode = pass;
      ushort passCodeFromString = FileEncrypter.GetPassCodeFromString("");
      string[] strArray2 = data.Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries);
      if (strArray2.Length < 2)
        throw new FormatException("Tried to decrypt an invalid valid DEC ENC file \"" + data + "\" - not enough elements. Need 2 lines, had " + (object) strArray2.Length);
      string[] strArray3 = strArray2[0].Split(FileEncrypter.HeaderSplitDelimiters, StringSplitOptions.None);
      if (strArray3.Length < 4)
        throw new FormatException("Tried to decrypt an invalid valid DEC ENC file \"" + data + "\" - not enough headers");
      string str1 = FileEncrypter.Decrypt(strArray3[1], passCodeFromString);
      string str2 = FileEncrypter.Decrypt(strArray3[2], passCodeFromString);
      string str3 = FileEncrypter.Decrypt(strArray3[3], passcode);
      string str4 = (string) null;
      if (strArray3.Length > 4)
        str4 = FileEncrypter.Decrypt(strArray3[4], passCodeFromString);
      string str5 = (string) null;
      string str6 = "1";
      if (str3 == "ENCODED")
        str5 = FileEncrypter.Decrypt(strArray2[1], passcode);
      else
        str6 = "0";
      strArray1[0] = str1;
      strArray1[1] = str2;
      strArray1[2] = str5;
      strArray1[3] = str4;
      strArray1[4] = str6;
      strArray1[5] = str3;
      return strArray1;
    }

    public static string[] DecryptHeaders(string data, string pass = "")
    {
      string[] strArray1 = new string[3];
      ushort passCodeFromString = FileEncrypter.GetPassCodeFromString(pass);
      string[] strArray2 = data.Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries);
      if (strArray2.Length < 2)
        throw new FormatException("Tried to decrypt an invalid valid DEC ENC file");
      string[] strArray3 = strArray2[0].Split(FileEncrypter.HeaderSplitDelimiters, StringSplitOptions.None);
      if (strArray3.Length < 4)
        throw new FormatException("Tried to decrypt an invalid valid DEC ENC file");
      string str1 = FileEncrypter.Decrypt(strArray3[1], passCodeFromString);
      string str2 = FileEncrypter.Decrypt(strArray3[2], passCodeFromString);
      string str3 = (string) null;
      if (strArray3.Length > 4)
        str3 = FileEncrypter.Decrypt(strArray3[4], passCodeFromString);
      strArray1[0] = str1;
      strArray1[1] = str2;
      strArray1[2] = str3;
      return strArray1;
    }

    public static int FileIsEncrypted(string data, string pass = "")
    {
      if (data.StartsWith("#DEC_ENC::"))
      {
        string[] strArray = FileEncrypter.DecryptString(data, pass);
        if (strArray[5] != "ENCODED")
        {
          if (strArray[4] == "0")
            return 2;
        }
        else
        {
          if (strArray[4] == "0")
            return 2;
          if (strArray[4] == "1")
            return 1;
        }
      }
      return 0;
    }

    public static string MakeReplacementsForDisplay(string input)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < input.Length; ++index)
      {
        if (GuiData.tinyfont.Characters.Contains(input[index]))
          stringBuilder.Append(input[index]);
        else
          stringBuilder.Append("_");
      }
      return stringBuilder.ToString();
    }
  }
}
