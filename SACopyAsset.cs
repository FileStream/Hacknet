// Decompiled with JetBrains decompiler
// Type: Hacknet.SACopyAsset
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SACopyAsset : SerializableAction
  {
    public int FunctionValue = 0;
    public string DestFileName;
    public string DestFilePath;
    public string DestComp;
    public string SourceComp;
    public string SourceFileName;
    public string SourceFilePath;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      Computer computer1 = Programs.getComputer(os, this.DestComp);
      Computer computer2 = Programs.getComputer(os, this.SourceComp);
      if (computer1 == null)
        throw new NullReferenceException("Destination Computer " + (object) computer1 + " could not be found for SACopyAsset, copying file: " + this.SourceFileName);
      if (computer2 == null)
        throw new NullReferenceException("Source Computer " + (object) computer2 + " could not be found for SACopyAsset, copying file: " + this.SourceFileName);
      Folder folderAtPath = Programs.getFolderAtPath(this.SourceFilePath, os, computer2.files.root, true);
      if (folderAtPath == null)
        throw new NullReferenceException("Source Folder " + this.SourceFilePath + " could not be found for SACopyAsset, adding file: " + this.SourceFileName);
      Folder folderFromPath = computer1.getFolderFromPath(this.DestFilePath, true);
      if (folderFromPath == null)
        return;
      FileEntry fileEntry1 = folderAtPath.searchForFile(this.SourceFileName);
      if (fileEntry1 == null)
        return;
      FileEntry fileEntry2 = new FileEntry(fileEntry1.data, this.DestFileName);
      folderFromPath.files.Add(fileEntry2);
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SACopyAsset saCopyAsset = new SACopyAsset();
      if (rdr.MoveToAttribute("DestFileName"))
        saCopyAsset.DestFileName = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("DestFilePath"))
        saCopyAsset.DestFilePath = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("DestComp"))
        saCopyAsset.DestComp = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("SourceComp"))
        saCopyAsset.SourceComp = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("SourceFileName"))
      {
        saCopyAsset.SourceFileName = rdr.ReadContentAsString();
        if (string.IsNullOrWhiteSpace(saCopyAsset.DestFileName))
          saCopyAsset.DestFileName = saCopyAsset.SourceFileName;
      }
      if (rdr.MoveToAttribute("SourceFilePath"))
        saCopyAsset.SourceFilePath = rdr.ReadContentAsString();
      return (SerializableAction) saCopyAsset;
    }
  }
}
