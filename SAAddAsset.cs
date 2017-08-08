// Decompiled with JetBrains decompiler
// Type: Hacknet.SAAddAsset
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  public class SAAddAsset : SerializableAction
  {
    public int FunctionValue = 0;
    public string FileName;
    public string FileContents;
    public string TargetComp;
    public string TargetFolderpath;

    public override void Trigger(object os_obj)
    {
      OS os = (OS) os_obj;
      Computer computer = Programs.getComputer(os, this.TargetComp);
      if (computer == null)
        throw new NullReferenceException("Computer " + this.TargetComp + " could not be found for AddAssetFunction, adding file: " + this.FileName);
      Folder folderAtPath = Programs.getFolderAtPath(this.TargetFolderpath, os, computer.files.root, true);
      if (folderAtPath == null)
        throw new NullReferenceException("Folder " + this.TargetFolderpath + " could not be found for AddAssetFunction, adding file: " + this.FileName);
      FileEntry fileEntry = new FileEntry(ComputerLoader.filter(this.FileContents), this.FileName);
      folderAtPath.files.Add(fileEntry);
    }

    public static SerializableAction DeserializeFromReader(XmlReader rdr)
    {
      SAAddAsset saAddAsset = new SAAddAsset();
      if (rdr.MoveToAttribute("FileName"))
        saAddAsset.FileName = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("FileContents"))
        saAddAsset.FileContents = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("TargetComp"))
        saAddAsset.TargetComp = rdr.ReadContentAsString();
      if (rdr.MoveToAttribute("TargetFolderpath"))
        saAddAsset.TargetFolderpath = rdr.ReadContentAsString();
      return (SerializableAction) saAddAsset;
    }
  }
}
