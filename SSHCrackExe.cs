// Decompiled with JetBrains decompiler
// Type: Hacknet.SSHCrackExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  internal class SSHCrackExe : ExeModule
  {
    public static float DURATION = 8f;
    private static float GRID_REVEAL_DELAY = 0.6f;
    private static float ENDING_FLASH = 0.7f;
    private static float SHEEN_FLASH_DELAY = 0.03f;
    private float timeLeft = SSHCrackExe.DURATION;
    private bool complete = false;
    private bool GridShowingSheen = false;
    private int width;
    private int height;
    private SSHCrackExe.SSHCrackGridEntry[,] Grid;
    private int GridEntryWidth;
    private int GridEntryHeight;
    private Color unlockedFlashColor;

    public SSHCrackExe(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.needsProxyAccess = true;
      this.IdentifierName = "SecureShellCrack";
      this.ramCost = 242;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.GridEntryWidth = 31;
      this.GridEntryHeight = 15;
      this.width = (int) (((double) this.bounds.Width - 4.0) / (double) this.GridEntryWidth);
      this.height = (int) (((double) this.bounds.Height - 24.0) / (double) this.GridEntryHeight);
      this.Grid = new SSHCrackExe.SSHCrackGridEntry[this.width, this.height];
      int num1 = 0;
      float num2 = 0.0f;
      float range = SSHCrackExe.DURATION - SSHCrackExe.GRID_REVEAL_DELAY - SSHCrackExe.ENDING_FLASH;
      for (int index1 = 0; index1 < this.height; ++index1)
      {
        for (int index2 = 0; index2 < this.width; ++index2)
        {
          int num3 = Math.Max(Math.Abs(index2 - index1 / 2), Math.Abs(index2 + index1 / 2));
          int num4 = index1 / 2 + num3;
          float num5 = range - num2;
          if (num1 % 2 == 0)
          {
            num5 = Utils.randm(range);
            num2 = num5;
          }
          // ISSUE: explicit reference operation
          ^this.Grid.Address(index2, index1) = new SSHCrackExe.SSHCrackGridEntry()
          {
            TimeSinceActivated = 0.0f,
            CurrentValue = Utils.getRandomByte(),
            TimeTillActive = (float) num4 * SSHCrackExe.SHEEN_FLASH_DELAY,
            TimeTillSolved = num5
          };
          ++num1;
        }
      }
      this.unlockedFlashColor = Color.Lerp(this.os.unlockedColor, this.os.brightUnlockedColor, 0.4f);
      Programs.getComputer(this.os, this.targetIP).hostileActionTaken();
      this.os.write("SecureShellCrack Running...");
    }

    public override void Update(float t)
    {
      base.Update(t);
      this.timeLeft -= t;
      if ((double) this.timeLeft <= 0.0 && !this.complete)
      {
        this.complete = true;
        this.Completed();
        this.isExiting = true;
      }
      if ((double) this.timeLeft < (double) SSHCrackExe.GRID_REVEAL_DELAY && (double) SSHCrackExe.GRID_REVEAL_DELAY - (double) this.timeLeft <= (double) t)
      {
        this.GridShowingSheen = true;
        for (int index1 = 0; index1 < this.height; ++index1)
        {
          for (int index2 = 0; index2 < this.width; ++index2)
          {
            int num1 = Math.Max(Math.Abs(index2 - index1 / 2), Math.Abs(index2 + index1 / 2));
            int num2 = index1 / 2 + num1;
            SSHCrackExe.SSHCrackGridEntry sshCrackGridEntry = this.Grid[index2, index1];
            sshCrackGridEntry.TimeSinceActivated = (float) (-1.0 * ((double) num2 * (double) SSHCrackExe.SHEEN_FLASH_DELAY));
            this.Grid[index2, index1] = sshCrackGridEntry;
          }
        }
      }
      float num = SSHCrackExe.DURATION - this.timeLeft;
      for (int index1 = 0; index1 < this.height; ++index1)
      {
        for (int index2 = 0; index2 < this.width; ++index2)
        {
          SSHCrackExe.SSHCrackGridEntry sshCrackGridEntry = this.Grid[index2, index1];
          sshCrackGridEntry.TimeTillActive -= t;
          if ((double) Utils.randm(0.5f) <= (double) t)
            sshCrackGridEntry.CurrentValue = Utils.getRandomByte();
          if ((double) sshCrackGridEntry.TimeTillActive <= 0.0)
          {
            sshCrackGridEntry.TimeTillActive = 0.0f;
            sshCrackGridEntry.TimeSinceActivated += t;
            if ((double) num > (double) SSHCrackExe.GRID_REVEAL_DELAY)
            {
              sshCrackGridEntry.TimeTillSolved -= t;
              if ((double) sshCrackGridEntry.TimeTillSolved <= 0.0)
                sshCrackGridEntry.CurrentValue = (byte) 0;
            }
          }
          this.Grid[index2, index1] = sshCrackGridEntry;
        }
      }
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      Rectangle empty = Rectangle.Empty;
      this.drawOutline();
      this.drawTarget("");
      TextItem.doFontLabel(new Vector2((float) (this.bounds.X + 2), (float) (this.bounds.Y + 14)), this.complete ? "Operation Complete" : "SSH Crack in operation...", GuiData.UITinyfont, new Color?(Utils.AddativeWhite * 0.8f * this.fade), (float) (this.bounds.Width - 6), float.MaxValue, false);
      int num1 = this.bounds.Y + 30;
      int num2 = this.bounds.X + 2;
      float num3 = 0.4f;
      Rectangle rectangle = new Rectangle(this.bounds.X + 2, this.bounds.Y + 16, this.GridEntryWidth - 2, this.GridEntryHeight - 2);
      for (int index1 = 0; index1 < this.width; ++index1)
      {
        for (int index2 = 0; index2 < this.height; ++index2)
        {
          SSHCrackExe.SSHCrackGridEntry sshCrackGridEntry = this.Grid[index1, index2];
          rectangle.X = num2 + index1 * this.GridEntryWidth;
          rectangle.Y = num1 + index2 * this.GridEntryHeight;
          if (rectangle.Y + 1 <= this.bounds.Y + this.bounds.Height && (double) sshCrackGridEntry.TimeTillActive <= 0.0)
          {
            Color color = this.os.lockedColor;
            if ((double) sshCrackGridEntry.TimeSinceActivated < (double) num3)
              color = Color.Lerp(this.os.brightLockedColor, this.os.lockedColor, sshCrackGridEntry.TimeSinceActivated / num3);
            if ((double) sshCrackGridEntry.TimeTillSolved <= 0.0)
            {
              color = this.os.unlockedColor;
              if ((double) sshCrackGridEntry.TimeTillSolved > -1.0 * (double) num3)
                color = Color.Lerp(this.os.unlockedColor, this.unlockedFlashColor, -1f * sshCrackGridEntry.TimeTillSolved / num3);
            }
            if (this.GridShowingSheen)
            {
              float amount = 0.0f;
              if ((double) sshCrackGridEntry.TimeSinceActivated >= 0.0)
              {
                amount = sshCrackGridEntry.TimeSinceActivated / (num3 / 2f);
                if ((double) sshCrackGridEntry.TimeSinceActivated > (double) num3 / 2.0)
                  amount = 1f - Math.Min(1f, (float) (((double) sshCrackGridEntry.TimeSinceActivated - (double) num3 / 2.0) / ((double) num3 / 2.0)));
              }
              if ((double) amount < 0.25)
                amount = 0.0f;
              else if ((double) amount < 0.75)
                amount = 0.5f;
              color = Color.Lerp(this.os.unlockedColor, this.unlockedFlashColor, amount);
            }
            Rectangle destinationRectangle = rectangle;
            bool flag = true;
            if (rectangle.Y + rectangle.Height > this.bounds.Y + this.bounds.Height)
            {
              destinationRectangle.Height = rectangle.Height - (rectangle.Y + rectangle.Height - (this.bounds.Y + this.bounds.Height));
              flag = false;
            }
            this.spriteBatch.Draw(Utils.white, destinationRectangle, color * this.fade);
            float num4 = (int) sshCrackGridEntry.CurrentValue >= 10 ? ((int) sshCrackGridEntry.CurrentValue >= 100 ? 1f : 5f) : 8f;
            if (flag)
              this.spriteBatch.DrawString(GuiData.UITinyfont, string.Concat((object) sshCrackGridEntry.CurrentValue), new Vector2((float) rectangle.X + num4, (float) rectangle.Y - 1.5f), Color.White * this.fade);
          }
        }
      }
    }

    public override void Completed()
    {
      base.Completed();
      Computer computer = Programs.getComputer(this.os, this.targetIP);
      if (computer == null)
        return;
      computer.openPort(22, this.os.thisComputer.ip);
      this.os.write("-- SecureShellCrack Complete --");
    }

    private struct SSHCrackGridEntry
    {
      public float TimeTillActive;
      public float TimeTillSolved;
      public float TimeSinceActivated;
      public byte CurrentValue;
    }
  }
}
