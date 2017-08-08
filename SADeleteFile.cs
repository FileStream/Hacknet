// Decompiled with JetBrains decompiler
// Type: Hacknet.SADeleteFile
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SADeleteFile : SerializableAction
  {
    public string TargetComp;
    public string FilePath;
    public string FileName;
    public string DelayHost;
    public float Delay;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      Computer computer1 = Programs.getComputer(os, this.TargetComp);
      if ((double) this.Delay <= 0.0)
      {
        Folder folderAtPath = Programs.getFolderAtPath(this.FilePath, os, computer1.files.root, true);
        if (folderAtPath == null)
          return;
        FileEntry fileEntry = folderAtPath.searchForFile(this.FileName);
        if (fileEntry != null)
          folderAtPath.files.Remove(fileEntry);
      }
      else
      {
        Computer computer2 = Programs.getComputer(os, this.DelayHost);
        if (computer2 == null)
          throw new NullReferenceException("Computer " + (object) computer2 + " could not be found as DelayHost for Function");
        float delay = this.Delay;
        this.Delay = -1f;
        DelayableActionSystem.FindDelayableActionSystemOnComputer(computer2).AddAction((SerializableAction) this, delay);
      }
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SADeleteFile saDeleteFile = new SADeleteFile();
      if (rdr.MoveToAttribute("Delay"))
        saDeleteFile.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("TargetComp"))
        saDeleteFile.TargetComp = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("FilePath"))
        saDeleteFile.FilePath = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("FileName"))
        saDeleteFile.FileName = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("DelayHost"))
        saDeleteFile.DelayHost = rdr.ReadContentAsString();
      return (SerializableAction) saDeleteFile;
    }
  }
}
