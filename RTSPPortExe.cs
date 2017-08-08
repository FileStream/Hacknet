// Decompiled with JetBrains decompiler
// Type: Hacknet.RTSPPortExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  internal class RTSPPortExe : ExeModule
  {
    private static float RUN_TIME = 6.3f;
    private static float IDLE_TIME = 30.5f;
    private float elapsedTime = 0.0f;
    private float completionFlashDuration = 3.2f;
    private float completionFlashTimer = 0.0f;
    private bool isComplete = false;
    private int completeRamUse = 220;
    private float preciceRamCost = 0.0f;
    private TrailLoadingSpinnerEffect spinner;

    public RTSPPortExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.name = "RTSPCrack";
      this.ramCost = 360;
      this.IdentifierName = "RTSPCrack";
      (this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp).hostileActionTaken();
      this.spinner = new TrailLoadingSpinnerEffect(operatingSystem);
      this.preciceRamCost = (float) this.ramCost;
    }

    public override void Update(float t)
    {
      base.Update(t);
      if ((double) this.elapsedTime < (double) RTSPPortExe.RUN_TIME && (double) this.elapsedTime + (double) t >= (double) RTSPPortExe.RUN_TIME)
        this.Completed();
      else if ((double) this.elapsedTime < (double) RTSPPortExe.IDLE_TIME && (double) this.elapsedTime + (double) t >= (double) RTSPPortExe.IDLE_TIME)
        this.isExiting = true;
      if (this.isComplete)
      {
        this.completionFlashTimer -= t;
        if ((double) this.completionFlashTimer <= 0.0)
          this.completionFlashTimer = 0.0f;
        if (this.ramCost > this.completeRamUse)
        {
          float num1 = 1f;
          float num2 = 2f;
          if ((double) this.elapsedTime - (double) RTSPPortExe.RUN_TIME < 2.0)
            num1 = (this.elapsedTime - RTSPPortExe.RUN_TIME) / num2;
          this.preciceRamCost -= num1 * 0.5f;
          this.ramCost = (int) this.preciceRamCost;
          if (this.ramCost < this.completeRamUse)
            this.ramCost = this.completeRamUse;
        }
      }
      this.elapsedTime += t;
    }

    public override void Completed()
    {
      base.Completed();
      Computer computer = Programs.getComputer(this.os, this.targetIP);
      if (computer == null)
        return;
      computer.openPort(554, this.os.thisComputer.ip);
      this.os.write("RTSPCrack Complete");
      this.isComplete = true;
      this.completionFlashTimer = this.completionFlashDuration;
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      float num = 1f;
      if ((double) this.elapsedTime < (double) RTSPPortExe.RUN_TIME)
        num = this.elapsedTime / RTSPPortExe.RUN_TIME;
      Rectangle rectangle = new Rectangle(this.bounds.X + 2, this.bounds.Y + this.bounds.Height / 2 - 16, this.bounds.Width - 4, Math.Min(30, Math.Max(0, this.bounds.Height - 50)));
      rectangle.Width = (int) ((double) rectangle.Width * (double) num);
      Color.Lerp(this.os.highlightColor, Utils.AddativeWhite, this.completionFlashTimer / this.completionFlashDuration);
      this.spinner.Draw(this.GetContentAreaDest(), this.spriteBatch, RTSPPortExe.RUN_TIME, (float) Math.Max(0.0, (double) RTSPPortExe.RUN_TIME - (double) this.elapsedTime), Math.Max(0.0f, this.elapsedTime - RTSPPortExe.RUN_TIME), new Color?());
    }
  }
}
