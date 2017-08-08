// Decompiled with JetBrains decompiler
// Type: Hacknet.SongChangerDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
  internal class SongChangerDaemon : Daemon
  {
    private MovingBarsEffect topEffect;
    private MovingBarsEffect botEffect;

    public SongChangerDaemon(Computer c, OS os)
      : base(c, LocaleTerms.Loc("Music Changer"), os)
    {
      this.topEffect = new MovingBarsEffect();
      this.botEffect = new MovingBarsEffect()
      {
        IsInverted = true
      };
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      this.topEffect.Update((float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
      this.botEffect.Update((float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
      int height = 30;
      Rectangle rectangle1 = new Rectangle(bounds.X + 30, bounds.Y + bounds.Height / 2 - height / 2, bounds.Width - 60, height);
      Rectangle bounds1 = rectangle1;
      bounds1.Height = 60;
      bounds1.Y -= bounds1.Height;
      this.topEffect.Draw(sb, bounds1, 1f, 3f, 1f, this.os.highlightColor);
      if (Button.doButton(73518921, rectangle1.X, rectangle1.Y, rectangle1.Width, rectangle1.Height, LocaleTerms.Loc("Shuffle Music"), new Color?(this.os.highlightColor)))
      {
        int maxValue = 12;
        MissionFunctions.runCommand(Utils.random.Next(maxValue) + 1, "changeSong");
      }
      bounds1.Y += bounds1.Height + height;
      this.botEffect.Draw(sb, bounds1, 1f, 3f, 1f, this.os.highlightColor);
      Rectangle rectangle2 = new Rectangle(bounds.X + 4, bounds.Y + bounds.Height - 4 - 20, (int) ((double) bounds.Width * 0.5), 20);
      if (!Button.doButton(73518924, rectangle2.X, rectangle2.Y, rectangle2.Width, rectangle2.Height, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
        return;
      this.os.display.command = "connect";
    }

    public override string getSaveString()
    {
      return "<SongChangerDaemon />";
    }
  }
}
