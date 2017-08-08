// Decompiled with JetBrains decompiler
// Type: Hacknet.FTPFastExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  internal class FTPFastExe : ExeModule
  {
    private float elapsedTime = 0.0f;
    private PointGatherEffect points = new PointGatherEffect();
    private const float RUN_TIME = 7f;
    private const float IDLE_TIME = 7.5f;

    public FTPFastExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.needsProxyAccess = true;
      this.name = "FTP_SprintBreak";
      this.ramCost = 190;
      this.IdentifierName = "FTPSprint";
      this.points.Init(this.os.content);
      this.points.AllowDoubleLines = true;
      this.points.Explode(70);
      Programs.getComputer(this.os, this.targetIP).hostileActionTaken();
    }

    public override void Update(float t)
    {
      base.Update(t);
      this.points.attractionToCentreMass = this.elapsedTime;
      this.points.Update(t);
      this.points.Update(t);
      this.points.Update(t);
      if ((double) this.elapsedTime < 7.0 && (double) this.elapsedTime + (double) t >= 7.0)
        this.Completed();
      else if ((double) this.elapsedTime < 7.5 && (double) this.elapsedTime + (double) t >= 7.5)
      {
        this.isExiting = true;
        this.points.NodeColor = Color.White;
      }
      else
        this.points.LineLengthPercentage = Math.Min(0.5f, (float) ((double) this.elapsedTime / 7.0 * 0.699999988079071));
      this.elapsedTime += t;
    }

    public override void Completed()
    {
      base.Completed();
      Computer computer = Programs.getComputer(this.os, this.targetIP);
      if (computer != null)
      {
        computer.openPort(21, this.os.thisComputer.ip);
        this.os.write(">> " + LocaleTerms.Loc("FTP Sprint Crack Successful"));
      }
      this.points.FlashComplete();
      this.points.Explode(30);
      this.points.timeRemainingWithoutAttract = 10f;
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      float num = 1f;
      if ((double) this.elapsedTime < 7.0)
        num = this.elapsedTime / 7f;
      Rectangle rectangle = new Rectangle(this.bounds.X + 2, this.bounds.Y + this.bounds.Height / 2 - 16, this.bounds.Width - 4, Math.Min(30, Math.Max(0, this.bounds.Height - 50)));
      Rectangle contentAreaDest = this.GetContentAreaDest();
      if (contentAreaDest.Height <= 2)
        return;
      this.points.Render(contentAreaDest, this.spriteBatch);
    }
  }
}
