// Decompiled with JetBrains decompiler
// Type: Hacknet.DelayableActionSystem
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Hacknet
{
  public class DelayableActionSystem
  {
    private static string EncryptionPass = "dasencrypt";
    private Folder folder;

    public DelayableActionSystem(Folder sourceFolder, object osObj)
    {
      DelayableActionSystem delayableActionSystem = this;
      this.folder = sourceFolder;
      ((OS) osObj).UpdateSubscriptions += (Action<float>) (t => delayableActionSystem.Update(t, (object) (OS) osObj));
    }

    internal DelayableActionSystem()
    {
    }

    private void Update(float t, object osObj)
    {
      for (int index = 0; index < this.folder.files.Count; ++index)
      {
        try
        {
          string data1 = this.folder.files[index].data;
          int length = data1.IndexOf('\n');
          string str = data1.Substring(0, length);
          string data2 = data1.Substring(length + 1);
          float num = (float) Convert.ToDouble(str, (IFormatProvider) CultureInfo.InvariantCulture) - t;
          if ((double) num <= 0.0)
          {
            using (Stream streamFromString = Utils.GenerateStreamFromString(FileEncrypter.DecryptString(data2, DelayableActionSystem.EncryptionPass)[2]))
            {
              try
              {
                XmlReader rdr = XmlReader.Create(streamFromString);
                SerializableAction.Deserialize(rdr).Trigger((object) (OS) osObj);
                rdr.Close();
              }
              catch (Exception ex)
              {
                ((OS) osObj).write(Utils.GenerateReportFromException(ex));
                Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
              }
            }
            this.folder.files.RemoveAt(index);
            --index;
          }
          else
          {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(num.ToString("0.0000000000", (IFormatProvider) CultureInfo.InvariantCulture));
            stringBuilder.Append('\n');
            stringBuilder.Append(data2);
            this.folder.files[index].data = stringBuilder.ToString();
          }
        }
        catch (Exception ex)
        {
        }
      }
    }

    public virtual void InstantlyResolveAllActions(object osObj)
    {
      for (int index = 0; index < this.folder.files.Count; index = index - 1 + 1)
      {
        string data = this.folder.files[index].data;
        int num = data.IndexOf('\n');
        using (Stream streamFromString = Utils.GenerateStreamFromString(FileEncrypter.DecryptString(data.Substring(num + 1), DelayableActionSystem.EncryptionPass)[2]))
        {
          XmlReader rdr = XmlReader.Create(streamFromString);
          SerializableAction.Deserialize(rdr).Trigger((object) (OS) osObj);
          rdr.Close();
        }
        this.folder.files.RemoveAt(index);
      }
    }

    public virtual void AddAction(SerializableAction action, float delay)
    {
      string str = FileEncrypter.EncryptString(action.GetSaveString(), "das", "UNKNOWN", DelayableActionSystem.EncryptionPass, (string) null);
      this.folder.files.Add(new FileEntry(delay.ToString("0.0000000000", (IFormatProvider) CultureInfo.InvariantCulture) + "\n" + str, this.GetNextSeqNumber() + ".act"));
    }

    internal string GetNextSeqNumber()
    {
      int num = 0;
      if (this.folder.files.Count > 0)
      {
        string name = this.folder.files[this.folder.files.Count - 1].name;
        string str = name.Substring(0, name.IndexOf('.'));
        try
        {
          num = Convert.ToInt32(str, (IFormatProvider) CultureInfo.InvariantCulture) + 1;
        }
        catch (Exception ex)
        {
        }
      }
      return num.ToString("000", (IFormatProvider) CultureInfo.InvariantCulture);
    }

    internal static DelayableActionSystem FindDelayableActionSystemOnComputer(Computer c)
    {
      IRCDaemon daemon1 = c.getDaemon(typeof (IRCDaemon)) as IRCDaemon;
      if (daemon1 != null)
        return daemon1.DelayedActions;
      DLCHubServer daemon2 = c.getDaemon(typeof (DLCHubServer)) as DLCHubServer;
      if (daemon2 != null)
        return daemon2.DelayedActions;
      FastActionHost daemon3 = c.getDaemon(typeof (FastActionHost)) as FastActionHost;
      if (daemon3 != null)
        return (DelayableActionSystem) daemon3.DelayedActions;
      throw new InvalidOperationException("Target computer " + c.name + " does not contain a Daemon that supports delayable actions");
    }
  }
}
