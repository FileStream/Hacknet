// Decompiled with JetBrains decompiler
// Type: Hacknet.FileSystem
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Xml;

namespace Hacknet
{
  internal class FileSystem
  {
    public Folder root;

    public FileSystem(bool empty)
    {
    }

    public FileSystem()
    {
      this.root = new Folder("/");
      this.root.folders.Add(new Folder("home"));
      this.root.folders.Add(new Folder("log"));
      this.root.folders.Add(new Folder("bin"));
      this.root.folders.Add(new Folder("sys"));
      this.generateSystemFiles();
    }

    public void generateSystemFiles()
    {
      Folder folder = this.root.searchForFolder("sys");
      folder.files.Add(new FileEntry(ThemeManager.getThemeDataString(OSTheme.HacknetTeal), "x-server.sys"));
      folder.files.Add(new FileEntry(Computer.generateBinaryString(500), "os-config.sys"));
      folder.files.Add(new FileEntry(Computer.generateBinaryString(500), "bootcfg.dll"));
      folder.files.Add(new FileEntry(Computer.generateBinaryString(500), "netcfgx.dll"));
    }

    public string getSaveString()
    {
      return "<filesystem>\n" + this.root.getSaveString() + "</filesystem>\n";
    }

    public static FileSystem load(XmlReader reader)
    {
      FileSystem fileSystem = new FileSystem(true);
      while (reader.Name != "filesystem")
        reader.Read();
      fileSystem.root = Folder.load(reader);
      return fileSystem;
    }

    public string TestEquals(object obj)
    {
      FileSystem fileSystem = obj as FileSystem;
      if (fileSystem == null)
        throw new ArgumentNullException();
      return this.root.TestEqualsFolder(fileSystem.root);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
