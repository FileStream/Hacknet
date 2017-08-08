// Decompiled with JetBrains decompiler
// Type: Hacknet.SerializableAction
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Hacknet
{
  public abstract class SerializableAction
  {
    public abstract void Trigger(object os_obj);

    public string GetSaveString()
    {
      Type type = this.GetType();
      string str1 = type.Name;
      if (str1.StartsWith("Hacknet."))
        str1 = str1.Substring("Hacknet.".Length);
      if (str1.StartsWith("SA"))
        str1 = str1.Substring("SA".Length);
      StringBuilder stringBuilder = new StringBuilder("<" + str1 + " ");
      string str2 = (string) null;
      FieldInfo[] fields = type.GetFields();
      for (int index = 0; index < fields.Length; ++index)
      {
        if (Utils.FieldContainsAttributeOfType(fields[index], typeof (XMLContentAttribute)))
        {
          if (str2 != null)
            throw new InvalidOperationException("More than one field in object " + this.ToString() + " is a content serializable type!");
          str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", new object[1]
          {
            fields[index].GetValue((object) this)
          });
        }
        else
        {
          stringBuilder.Append(fields[index].Name + "=\"");
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}", new object[1]
          {
            fields[index].GetValue((object) this)
          });
          stringBuilder.Append("\" ");
        }
      }
      if (str2 == null)
      {
        stringBuilder.Append("/>");
      }
      else
      {
        stringBuilder.Append(">");
        stringBuilder.Append(str2);
        stringBuilder.Append("</" + str1 + ">");
      }
      return stringBuilder.ToString();
    }

    public static SerializableAction Deserialize(XmlReader rdr)
    {
      Dictionary<string, Func<XmlReader, SerializableAction>> dictionary = new Dictionary<string, Func<XmlReader, SerializableAction>>();
      dictionary.Add("LoadMission", new Func<XmlReader, SerializableAction>(SALoadMission.DeserializeFromReader));
      dictionary.Add("RunFunction", new Func<XmlReader, SerializableAction>(SARunFunction.DeserializeFromReader));
      dictionary.Add("AddAsset", new Func<XmlReader, SerializableAction>(SAAddAsset.DeserializeFromReader));
      dictionary.Add("AddMissionToHubServer", new Func<XmlReader, SerializableAction>(SAAddMissionToHubServer.DeserializeFromReader));
      dictionary.Add("RemoveMissionFromHubServer", new Func<XmlReader, SerializableAction>(SARemoveMissionFromHubServer.DeserializeFromReader));
      dictionary.Add("AddThreadToMissionBoard", new Func<XmlReader, SerializableAction>(SAAddThreadToMissionBoard.DeserializeFromReader));
      dictionary.Add("AddIRCMessage", new Func<XmlReader, SerializableAction>(SAAddIRCMessage.DeserializeFromReader));
      dictionary.Add("AddConditionalActions", new Func<XmlReader, SerializableAction>(SAAddConditionalActions.DeserializeFromReader));
      dictionary.Add("CopyAsset", new Func<XmlReader, SerializableAction>(SACopyAsset.DeserializeFromReader));
      dictionary.Add("CrashComputer", new Func<XmlReader, SerializableAction>(SACrashComputer.DeserializeFromReader));
      dictionary.Add("DeleteFile", new Func<XmlReader, SerializableAction>(SADeleteFile.DeserializeFromReader));
      dictionary.Add("LaunchHackScript", new Func<XmlReader, SerializableAction>(SALaunchHackScript.DeserializeFromReader));
      dictionary.Add("SwitchToTheme", new Func<XmlReader, SerializableAction>(SASwitchToTheme.DeserializeFromReader));
      dictionary.Add("StartScreenBleedEffect", new Func<XmlReader, SerializableAction>(SAStartScreenBleedEffect.DeserializeFromReader));
      dictionary.Add("CancelScreenBleedEffect", new Func<XmlReader, SerializableAction>(SACancelScreenBleedEffect.DeserializeFromReader));
      dictionary.Add("AppendToFile", new Func<XmlReader, SerializableAction>(SAAppendToFile.DeserializeFromReader));
      dictionary.Add("KillExe", new Func<XmlReader, SerializableAction>(SAKillExe.DeserializeFromReader));
      dictionary.Add("ChangeAlertIcon", new Func<XmlReader, SerializableAction>(SAChangeAlertIcon.DeserializeFromReader));
      dictionary.Add("HideNode", new Func<XmlReader, SerializableAction>(SAHideNode.DeserializeFromReader));
      dictionary.Add("GivePlayerUserAccount", new Func<XmlReader, SerializableAction>(SAGivePlayerUserAccount.DeserializeFromReader));
      dictionary.Add("ChangeIP", new Func<XmlReader, SerializableAction>(SAChangeIP.DeserializeFromReader));
      dictionary.Add("ChangeNetmapSortMethod", new Func<XmlReader, SerializableAction>(SAChangeNetmapSortMethod.DeserializeFromReader));
      dictionary.Add("SaveGame", new Func<XmlReader, SerializableAction>(SASaveGame.DeserializeFromReader));
      dictionary.Add("HideAllNodes", new Func<XmlReader, SerializableAction>(SAHideAllNodes.DeserializeFromReader));
      dictionary.Add("ShowNode", new Func<XmlReader, SerializableAction>(SAShowNode.DeserializeFromReader));
      dictionary.Add("SetLock", new Func<XmlReader, SerializableAction>(SASetLock.DeserializeFromReader));
      while (!rdr.EOF && (!rdr.IsStartElement() || !dictionary.ContainsKey(rdr.Name)))
        rdr.Read();
      if (rdr.EOF)
        throw new FormatException("Unexpected end of file!");
      return dictionary[rdr.Name](rdr);
    }
  }
}
