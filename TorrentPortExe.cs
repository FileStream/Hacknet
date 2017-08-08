// Decompiled with JetBrains decompiler
// Type: Hacknet.TorrentPortExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  internal class TorrentPortExe : ExeModule
  {
    private float elapsedTime = 0.0f;
    private float completionFlashDuration = 3.2f;
    private float completionFlashTimer = 0.0f;
    private bool isComplete = false;
    private RaindropsEffect RainEffect = new RaindropsEffect();
    private RaindropsEffect BackgroundRainEffect = new RaindropsEffect();
    private const float RUN_TIME = 4.8f;
    private const float IDLE_TIME = 16.5f;

    public TorrentPortExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.name = "TorrentStreamInjector";
      this.ramCost = 360;
      this.IdentifierName = "TorrentInjector";
      this.BackgroundRainEffect.Init(this.os.content);
      this.BackgroundRainEffect.MaxVerticalLandingVariane += 0.04f;
      this.BackgroundRainEffect.FallRate = 0.5f;
      this.RainEffect.Init(this.os.content);
      this.RainEffect.ForceSpawnDrop(new Vector3(0.5f, 0.0f, 0.0f));
      (this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp).hostileActionTaken();
    }

    public override void Update(float t)
    {
      base.Update(t);
      float num = 20f;
      if ((double) this.elapsedTime < 1.0)
        num *= this.elapsedTime * 0.2f;
      this.RainEffect.Update(t, this.isComplete ? 2f : num);
      this.BackgroundRainEffect.Update(t, num * 3f);
      if ((double) this.elapsedTime < 4.80000019073486 && (double) this.elapsedTime + (double) t >= 4.80000019073486)
        this.Completed();
      else if ((double) this.elapsedTime < 16.5 && (double) this.elapsedTime + (double) t >= 16.5)
        this.isExiting = true;
      if (this.isComplete)
      {
        this.completionFlashTimer -= t;
        if ((double) this.completionFlashTimer <= 0.0)
          this.completionFlashTimer = 0.0f;
      }
      this.elapsedTime += t;
    }

    public override void Completed()
    {
      base.Completed();
      Computer computer = Programs.getComputer(this.os, this.targetIP);
      if (computer == null)
        return;
      computer.openPort(6881, this.os.thisComputer.ip);
      this.os.write(" - " + LocaleTerms.Loc("Torrent Stream Injection Complete") + " - ");
      this.isComplete = true;
      this.completionFlashTimer = this.completionFlashDuration;
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      float num = 1f;
      if ((double) this.elapsedTime < 4.80000019073486)
        num = this.elapsedTime / 4.8f;
      Rectangle rectangle = new Rectangle(this.bounds.X + 2, this.bounds.Y + this.bounds.Height / 2 - 16, this.bounds.Width - 4, Math.Min(30, Math.Max(0, this.bounds.Height - 50)));
      rectangle.Width = (int) ((double) rectangle.Width * (double) num);
      if (this.isComplete)
        this.BackgroundRainEffect.Render(this.GetContentAreaDest(), this.spriteBatch, Color.Lerp(this.os.highlightColor * 0.2f, Utils.AddativeWhite, this.completionFlashTimer / this.completionFlashDuration), 20f, 40f);
      Color DropColor = this.os.highlightColor;
      if (this.isComplete)
        DropColor = Color.Lerp(DropColor, Utils.AddativeWhite, this.completionFlashTimer / this.completionFlashDuration);
      this.RainEffect.Render(this.GetContentAreaDest(), this.spriteBatch, DropColor, 50f, 100f);
    }
  }
}
