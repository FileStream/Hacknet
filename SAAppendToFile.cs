// Decompiled with JetBrains decompiler
// Type: Hacknet.SAAppendToFile
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SAAppendToFile : SerializableAction
  {
    [XMLContent]
    public string DataToAdd;
    public string TargetComp;
    public string TargetFolderpath;
    public string TargetFilename;
    public string DelayHost;
    public float Delay;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      if ((double) this.Delay <= 0.0)
      {
        Computer computer = Programs.getComputer(os, this.TargetComp);
        Folder folderAtPath = Programs.getFolderAtPath(this.TargetFolderpath, os, computer.files.root, true);
        if (folderAtPath == null)
          return;
        FileEntry fileEntry = folderAtPath.searchForFile(this.TargetFilename);
        fileEntry.data = fileEntry.data + "\n" + this.DataToAdd;
      }
      else
      {
        Computer computer = Programs.getComputer(os, this.DelayHost);
        if (computer == null)
          throw new NullReferenceException("Computer " + (object) computer + " could not be found as DelayHost for Function");
        float delay = this.Delay;
        this.Delay = -1f;
        DelayableActionSystem.FindDelayableActionSystemOnComputer(computer).AddAction((SerializableAction) this, delay);
      }
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SAAppendToFile saAppendToFile = new SAAppendToFile();
      if (rdr.MoveToAttribute("Delay"))
        saAppendToFile.Delay = rdr.ReadContentAsFloat();
      if (rdr.MoveToAttribute("DelayHost"))
        saAppendToFile.DelayHost = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("TargetComp"))
        saAppendToFile.TargetComp = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("TargetFolderpath"))
        saAppendToFile.TargetFolderpath = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("TargetFilename"))
        saAppendToFile.TargetFilename = rdr.ReadContentAsString();
      int content = (int) rdr.MoveToContent();
      saAppendToFile.DataToAdd = ComputerLoader.filter(rdr.ReadElementContentAsString());
      return (SerializableAction) saAppendToFile;
    }
  }
}
