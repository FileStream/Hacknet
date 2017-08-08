// Decompiled with JetBrains decompiler
// Type: Hacknet.FastActionHost
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
  internal class FastActionHost : Daemon
  {
    public bool RequiresLogin = false;
    public FastDelayableActionSystem DelayedActions;
    private Folder folder;

    public FastActionHost(Computer c, OS os, string name)
      : base(c, name, os)
    {
      this.isListed = true;
    }

    public override void initFiles()
    {
      base.initFiles();
      this.folder = this.comp.files.root.searchForFolder("runtime");
      if (this.folder == null)
      {
        this.folder = new Folder("runtime");
        this.comp.files.root.folders.Add(this.folder);
      }
      this.DelayedActions = new FastDelayableActionSystem(this.folder, (object) this.os);
    }

    public override void loadInit()
    {
      base.loadInit();
      this.folder = this.comp.files.root.searchForFolder("runtime");
      this.DelayedActions = new FastDelayableActionSystem(this.folder, (object) this.os);
      this.DelayedActions.DeserializeActions(this.folder.files);
    }

    public string GetName()
    {
      return this.name;
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      Vector2 pos = new Vector2((float) bounds.X + 20f, (float) bounds.Y + 20f);
      TextItem.doFontLabel(pos, "Active Actions : " + (object) this.DelayedActions.Actions.Count, GuiData.smallfont, new Color?(Color.White), float.MaxValue, float.MaxValue, false);
      pos.Y += 30f;
      if (!Button.doButton(38391101, (int) pos.X, (int) pos.Y, 300, 25, "Back", new Color?()))
        return;
      this.os.display.command = "connect";
    }

    public override string getSaveString()
    {
      this.folder.files = this.DelayedActions.GetAllFilesForActions();
      return "<FastActionHost />";
    }
  }
}
