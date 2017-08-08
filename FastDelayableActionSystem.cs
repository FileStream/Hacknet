// Decompiled with JetBrains decompiler
// Type: Hacknet.FastDelayableActionSystem
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
  public class FastDelayableActionSystem : DelayableActionSystem
  {
    private static string EncryptionPass = "dasencrypt";
    internal List<KeyValuePair<float, SerializableAction>> Actions = new List<KeyValuePair<float, SerializableAction>>();
    private int SeqNum = 0;
    private Folder folder;

    public FastDelayableActionSystem(Folder sourceFolder, object osObj)
    {
      FastDelayableActionSystem delayableActionSystem = this;
      this.folder = sourceFolder;
      ((OS) osObj).UpdateSubscriptions += (Action<float>) (t => delayableActionSystem.Update(t, (object) (OS) osObj));
    }

    private void Update(float t, object osObj)
    {
      for (int index = 0; index < this.Actions.Count; ++index)
      {
        try
        {
          float key1 = this.Actions[index].Key;
          SerializableAction serializableAction = this.Actions[index].Value;
          float key2 = key1 - t;
          if ((double) key2 <= 0.0)
          {
            try
            {
              serializableAction.Trigger((object) (OS) osObj);
            }
            catch (Exception ex)
            {
              ((OS) osObj).write(Utils.GenerateReportFromException(ex));
              Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
            }
            this.Actions.RemoveAt(index);
            --index;
          }
          else
            this.Actions[index] = new KeyValuePair<float, SerializableAction>(key2, serializableAction);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public override void InstantlyResolveAllActions(object osObj)
    {
      for (int index = 0; index < this.Actions.Count; ++index)
      {
        try
        {
          this.Actions[index].Value.Trigger(osObj);
          this.Actions.RemoveAt(index);
          --index;
        }
        catch (Exception ex)
        {
          ((OS) osObj).write(Utils.GenerateReportFromException(ex));
          Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
        }
      }
    }

    public override void AddAction(SerializableAction action, float delay)
    {
      this.Actions.Add(new KeyValuePair<float, SerializableAction>(delay, action));
    }

    private FileEntry EncryptAction(SerializableAction action, float delay)
    {
      string str = FileEncrypter.EncryptString(action.GetSaveString(), "das", "UNKNOWN", FastDelayableActionSystem.EncryptionPass, (string) null);
      FileEntry fileEntry = new FileEntry(delay.ToString("0.0000000000", (IFormatProvider) CultureInfo.InvariantCulture) + "\n" + str, this.SeqNum.ToString("000") + ".act");
      ++this.SeqNum;
      return fileEntry;
    }

    public List<FileEntry> GetAllFilesForActions()
    {
      this.SeqNum = 0;
      List<FileEntry> fileEntryList1 = new List<FileEntry>();
      for (int index = 0; index < this.Actions.Count; ++index)
      {
        List<FileEntry> fileEntryList2 = fileEntryList1;
        KeyValuePair<float, SerializableAction> action1 = this.Actions[index];
        SerializableAction action2 = action1.Value;
        action1 = this.Actions[index];
        double key = (double) action1.Key;
        FileEntry fileEntry = this.EncryptAction(action2, (float) key);
        fileEntryList2.Add(fileEntry);
      }
      return fileEntryList1;
    }

    public void DeserializeActions(List<FileEntry> files)
    {
      this.Actions.Clear();
      for (int index = 0; index < files.Count; ++index)
      {
        try
        {
          string data1 = this.folder.files[index].data;
          int length = data1.IndexOf('\n');
          string str = data1.Substring(0, length);
          string data2 = data1.Substring(length + 1);
          float key = (float) Convert.ToDouble(str, (IFormatProvider) CultureInfo.InvariantCulture);
          using (Stream streamFromString = Utils.GenerateStreamFromString(FileEncrypter.DecryptString(data2, FastDelayableActionSystem.EncryptionPass)[2]))
          {
            try
            {
              SerializableAction serializableAction = SerializableAction.Deserialize(XmlReader.Create(streamFromString));
              this.Actions.Add(new KeyValuePair<float, SerializableAction>(key, serializableAction));
            }
            catch (Exception ex)
            {
              if (OS.DEBUG_COMMANDS)
                Utils.AppendToWarningsFile("Error deserializing action in fast action host :\n" + Utils.GenerateReportFromExceptionCompact(ex));
            }
          }
        }
        catch (Exception ex)
        {
          if (OS.DEBUG_COMMANDS)
            Utils.AppendToWarningsFile("Error deserializing action " + (object) index + " in fast action host :\n" + Utils.GenerateReportFromExceptionCompact(ex));
        }
      }
    }
  }
}
