// Decompiled with JetBrains decompiler
// Type: Hacknet.AvconDemoEndDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
  internal class AvconDemoEndDaemon : Daemon
  {
    private bool confirmed = false;

    public AvconDemoEndDaemon(Computer c, string name, OS os)
      : base(c, name, os)
    {
      this.name = "Complete Demo";
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      this.confirmed = false;
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      if (Button.doButton((int) byte.MaxValue, bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 4, bounds.Width / 2, 35, LocaleTerms.Loc("Back"), new Color?()))
        this.os.display.command = "connect";
      if (!Button.doButton(258, bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 4 + 40, bounds.Width / 2, 35, !this.confirmed ? "End Session" : "Confirm End", new Color?(this.confirmed ? Color.Red : Color.DarkRed)))
        return;
      if (this.confirmed)
        this.endDemo();
      else
        this.confirmed = true;
    }

    public override string getSaveString()
    {
      return "<addAvconDemoEndDaemon name=\"" + this.name + "\"/>";
    }

    private void endDemo()
    {
      this.os.ScreenManager.AddScreen((GameScreen) new MainMenu());
      this.os.ExitScreen();
    }
  }
}
