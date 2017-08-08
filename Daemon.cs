// Decompiled with JetBrains decompiler
// Type: Hacknet.Daemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
  internal class Daemon
  {
    public string name;
    public bool isListed;
    public Computer comp;
    public OS os;

    public Daemon(Computer computer, string serviceName, OS opSystem)
    {
      this.name = serviceName;
      this.isListed = true;
      this.comp = computer;
      this.os = opSystem;
    }

    public virtual void initFiles()
    {
    }

    public virtual void draw(Rectangle bounds, SpriteBatch sb)
    {
    }

    public virtual void navigatedTo()
    {
    }

    public virtual void userAdded(string name, string pass, byte type)
    {
    }

    public virtual string getSaveString()
    {
      return "";
    }

    public virtual void loadInit()
    {
    }

    public static bool validUser(byte type)
    {
      return (int) type == 1 || (int) type == 0;
    }

    public void registerAsDefaultBootDaemon()
    {
      if (!this.comp.AllowsDefaultBootModule)
        return;
      FileEntry fileEntry = this.comp.files.root.searchForFolder("sys").searchForFile(ComputerTypeInfo.getDefaultBootDaemonFilename((object) this));
      if (fileEntry != null)
      {
        if (fileEntry.data != "[Locked]")
          fileEntry.data = LocaleTerms.Loc(this.name);
      }
      else
        this.comp.files.root.searchForFolder("sys").files.Add(new FileEntry(LocaleTerms.Loc(this.name), ComputerTypeInfo.getDefaultBootDaemonFilename((object) this)));
    }
  }
}
