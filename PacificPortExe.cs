// Decompiled with JetBrains decompiler
// Type: Hacknet.PacificPortExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Microsoft.Xna.Framework;

namespace Hacknet
{
  internal class PacificPortExe : ExeModule
  {
    private float elapsedTime = 0.0f;
    private int framesFlashed = 0;
    private const float RUN_TIME = 6f;
    private const float IDLE_TIME = 6.2f;
    private FlyoutEffect effect;

    public PacificPortExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.needsProxyAccess = true;
      this.name = "Pacific_Portcrusher";
      this.ramCost = 190;
      this.IdentifierName = "PacificPortcrusher";
      Programs.getComputer(this.os, this.targetIP).hostileActionTaken();
    }

    public override void Update(float t)
    {
      base.Update(t);
      if ((double) this.elapsedTime < 6.0 && (double) this.elapsedTime + (double) t >= 6.0)
        this.Completed();
      else if ((double) this.elapsedTime < 6.19999980926514 && (double) this.elapsedTime + (double) t >= 6.19999980926514)
        this.isExiting = true;
      this.elapsedTime += t;
    }

    public override void Completed()
    {
      base.Completed();
      Computer computer = Programs.getComputer(this.os, this.targetIP);
      if (computer == null)
        return;
      computer.openPort(192, this.os.thisComputer.ip);
      this.os.write("Pacific Portcrusher  >>" + LocaleTerms.Loc("SUCCESS") + "<<");
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      if (this.effect == null)
        this.effect = new FlyoutEffect(this.spriteBatch.GraphicsDevice, this.os.content, this.bounds.Width - 2, this.bounds.Height - 2);
      float num = 1f;
      if ((double) this.elapsedTime < 6.0)
        num = this.elapsedTime / 6f;
      Rectangle contentAreaDest = this.GetContentAreaDest();
      if (contentAreaDest.Height <= 2)
        return;
      if (this.isExiting)
        ++this.framesFlashed;
      this.effect.Draw((float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds, contentAreaDest, this.spriteBatch, 50f * num, 3, 100f * Utils.QuadraticOutCurve(1f - num), this.os.highlightColor, false, this.isExiting && this.framesFlashed % 30 <= 3);
    }
  }
}
