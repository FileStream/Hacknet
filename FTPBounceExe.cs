// Decompiled with JetBrains decompiler
// Type: Hacknet.FTPBounceExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  internal class FTPBounceExe : ExeModule
  {
    public static float DURATION = 15f;
    public static float SCROLL_RATE = 0.08f;
    private int binaryChars = 0;
    private float progress = 0.0f;
    private float timeLeft = FTPBounceExe.DURATION;
    private float binaryScrollTimer = FTPBounceExe.SCROLL_RATE;
    private int binaryIndex = 0;
    private bool complete = false;
    private int unlockedChars1 = 0;
    private int unlockedChars2 = 0;
    private string binary;
    private byte[] acceptedBinary1;
    private byte[] acceptedBinary2;
    private int[] unlockOrder;

    public FTPBounceExe(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.needsProxyAccess = true;
      this.ramCost = 210;
      this.IdentifierName = "FTP Bounce";
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.binary = Computer.generateBinaryString(1024);
      int num1 = 12;
      this.binaryChars = (this.bounds.Width - 4) / num1;
      this.acceptedBinary1 = new byte[this.binaryChars];
      this.acceptedBinary2 = new byte[this.binaryChars];
      for (int index = 0; index < num1; ++index)
      {
        this.acceptedBinary1[index] = Utils.random.NextDouble() > 0.5 ? (byte) 0 : (byte) 1;
        this.acceptedBinary2[index] = Utils.random.NextDouble() > 0.5 ? (byte) 0 : (byte) 1;
      }
      this.unlockOrder = new int[this.binaryChars];
      for (int index = 0; index < this.binaryChars; ++index)
        this.unlockOrder[index] = index;
      int num2 = 300;
      for (int index1 = 0; index1 < num2; ++index1)
      {
        int index2 = Utils.random.Next(0, this.unlockOrder.Length - 1);
        int index3 = Utils.random.Next(0, this.unlockOrder.Length - 1);
        int num3 = this.unlockOrder[index2];
        this.unlockOrder[index2] = this.unlockOrder[index3];
        this.unlockOrder[index3] = num3;
      }
      Programs.getComputer(this.os, this.targetIP).hostileActionTaken();
    }

    public override void Update(float t)
    {
      base.Update(t);
      if (!this.complete)
      {
        this.binaryScrollTimer -= t;
        if ((double) this.binaryScrollTimer <= 0.0)
        {
          ++this.binaryIndex;
          this.binaryScrollTimer = FTPBounceExe.SCROLL_RATE;
        }
      }
      this.timeLeft -= t;
      if ((double) this.timeLeft <= 0.0 && !this.complete)
      {
        this.complete = true;
        this.Completed();
      }
      this.progress = Math.Min(Math.Abs((float) (1.0 - (double) this.timeLeft / (double) FTPBounceExe.DURATION)), 1f);
      this.unlockedChars1 = Math.Min((int) ((double) this.binaryChars * ((double) this.progress * 2.0)), this.binaryChars);
      this.unlockedChars2 = Math.Min((int) ((double) this.binaryChars * (double) this.progress), this.binaryChars);
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      Vector2 position = new Vector2((float) (this.bounds.X + 6), (float) (this.bounds.Y + 12));
      for (int index1 = 0; index1 < 6; ++index1)
      {
        for (int index2 = 0; index2 < this.binaryChars; ++index2)
        {
          this.spriteBatch.DrawString(GuiData.UITinyfont, string.Concat((object) this.binary[(this.binaryIndex + index2 + index1 * 20) % (this.binary.Length - 1)]), position, Color.White);
          position.X += 12f;
        }
        position.Y += 12f;
        position.X = (float) (this.bounds.X + 6);
        if ((double) position.Y - (double) this.bounds.Y + 24.0 > (double) this.bounds.Height)
          return;
      }
      position.Y += 16f;
      if ((double) position.Y - (double) this.bounds.Y + 24.0 > (double) this.bounds.Height)
        return;
      this.spriteBatch.DrawString(GuiData.UISmallfont, "Working ::", position, this.os.subtleTextColor);
      position.Y += 20f;
      Rectangle destinationRectangle = new Rectangle((int) position.X, (int) position.Y, this.bounds.Width - 12, 80);
      destinationRectangle.Height = (int) Math.Min((float) destinationRectangle.Height, (float) this.bounds.Height - (position.Y - (float) this.bounds.Y));
      this.spriteBatch.Draw(Utils.white, destinationRectangle, (this.complete ? this.os.unlockedColor : this.os.lockedColor) * this.fade);
      if ((double) position.Y - (double) this.bounds.Y + 80.0 > (double) this.bounds.Height)
        return;
      for (int index1 = 0; index1 < this.unlockedChars1; ++index1)
      {
        int index2 = this.unlockOrder[index1];
        int index3 = this.unlockOrder[this.binaryChars - index1 - 1];
        position.X = (float) (this.bounds.X + 6 + 12 * index2);
        this.spriteBatch.DrawString(GuiData.UISmallfont, string.Concat((object) this.acceptedBinary1[index2]), position, Color.White * this.fade);
        this.spriteBatch.DrawString(GuiData.UISmallfont, string.Concat((object) this.acceptedBinary1[index3]), position + new Vector2(0.0f, 32f), Color.White * this.fade);
      }
      position.Y += 16f;
      for (int index1 = 0; index1 < this.unlockedChars2; ++index1)
      {
        int index2 = this.unlockOrder[this.binaryChars - index1 - 1];
        int index3 = this.unlockOrder[index1];
        position.X = (float) (this.bounds.X + 6 + 12 * index2);
        this.spriteBatch.DrawString(GuiData.UISmallfont, string.Concat((object) this.acceptedBinary2[index2]), position, Color.White * this.fade);
        this.spriteBatch.DrawString(GuiData.UISmallfont, string.Concat((object) this.acceptedBinary2[index3]), position + new Vector2(0.0f, 32f), Color.White * this.fade);
      }
    }

    public override void Completed()
    {
      base.Completed();
      Computer computer = Programs.getComputer(this.os, this.targetIP);
      if (computer != null)
        computer.openPort(21, this.os.thisComputer.ip);
      this.isExiting = true;
    }
  }
}
