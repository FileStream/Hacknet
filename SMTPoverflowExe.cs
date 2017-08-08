// Decompiled with JetBrains decompiler
// Type: Hacknet.SMTPoverflowExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class SMTPoverflowExe : ExeModule
  {
    public static float DURATION = 12f;
    public static float BAR_MOVEMENT = 30f;
    public static float BAR_HEIGHT = 2f;
    private float sucsessTimer = 0.5f;
    private float timeAccum = 0.0f;
    private float barSize = 0.0f;
    private int completedIndex = 0;
    private Color activeBarColor = new Color(34, 82, 64, (int) byte.MaxValue);
    private Color activeBarHighlightColor = new Color(0, 186, 99, 0);
    public float progress;
    public bool hasCompleted;
    private List<Vector2> leftBars;
    private List<Vector2> rightBars;

    public SMTPoverflowExe(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.needsProxyAccess = true;
      this.ramCost = 356;
      this.IdentifierName = "SMTP Overflow";
      this.activeBarColor = this.os.unlockedColor;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.barSize = SMTPoverflowExe.BAR_HEIGHT + 1.4f;
      int capacity = (int) ((double) this.bounds.Height / (double) this.barSize);
      int maxValue = this.bounds.Width / 2 - 1;
      this.leftBars = new List<Vector2>(capacity);
      this.rightBars = new List<Vector2>(capacity);
      for (int index = 0; index < capacity; ++index)
      {
        this.leftBars.Add(new Vector2((float) Utils.random.Next(0, maxValue), (float) Utils.random.Next(0, maxValue)));
        this.rightBars.Add(new Vector2((float) Utils.random.Next(0, maxValue), (float) Utils.random.Next(0, maxValue)));
      }
      this.barSize = SMTPoverflowExe.BAR_HEIGHT;
      Programs.getComputer(this.os, this.targetIP).hostileActionTaken();
    }

    public override void Update(float t)
    {
      base.Update(t);
      this.timeAccum += t * 5f;
      this.progress += t / SMTPoverflowExe.DURATION;
      if ((double) this.progress >= 1.0)
      {
        this.progress = 1f;
        if (!this.hasCompleted)
        {
          this.Completed();
          this.hasCompleted = true;
        }
        this.sucsessTimer -= t;
        if ((double) this.sucsessTimer <= 0.0)
          this.isExiting = true;
      }
      Vector2 zero = Vector2.Zero;
      for (int index = 0; index < this.leftBars.Count; ++index)
      {
        if (index > this.completedIndex)
        {
          Vector2 vector2 = this.leftBars[index];
          vector2.X += (float) Math.Sin((double) this.timeAccum + (double) (index * index)) * t * SMTPoverflowExe.BAR_MOVEMENT;
          vector2.Y += (float) Math.Sin((double) this.timeAccum + (double) index) * t * SMTPoverflowExe.BAR_MOVEMENT;
          this.leftBars[index] = vector2;
          vector2 = this.rightBars[index];
          vector2.X += (float) Math.Sin((double) this.timeAccum + (double) (index * index)) * t * SMTPoverflowExe.BAR_MOVEMENT;
          vector2.Y += (float) Math.Sin((double) this.timeAccum + (double) index) * t * SMTPoverflowExe.BAR_MOVEMENT;
          this.rightBars[index] = vector2;
        }
      }
      this.completedIndex = (int) ((double) this.progress * (double) (this.leftBars.Count - 1));
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      this.spriteBatch.DrawString(GuiData.UISmallfont, "SMTP Mail Server Overflow", new Vector2((float) (this.bounds.X + 5), (float) (this.bounds.Y + 12)), this.os.subtleTextColor);
      Vector2 vector2_1 = new Vector2((float) (this.bounds.X + 2), (float) (this.bounds.Y + 57));
      int num = (this.bounds.Width - 4) / 2;
      Rectangle rectangle1 = new Rectangle(this.bounds.X + 2, this.bounds.Y + 38, (this.bounds.Width - 4) / 2, (int) this.barSize);
      Rectangle rectangle2 = new Rectangle(this.bounds.X + 2, this.bounds.Y + 38, (this.bounds.Width - 4) / 2, (int) this.barSize);
      bool flag1 = false;
      Vector2 vector2_2;
      for (int index = 0; index < this.leftBars.Count; ++index)
      {
        flag1 = index <= this.completedIndex;
        vector2_2 = this.leftBars[index];
        rectangle1.Width = (int) vector2_2.Y;
        this.spriteBatch.Draw(Utils.white, rectangle1, (flag1 ? Color.Lerp(this.os.outlineColor, Utils.VeryDarkGray, Utils.randm(0.2f) * Utils.randm(1f)) : this.os.subtleTextColor) * this.fade);
        rectangle1.Y += (int) this.barSize + 1;
        vector2_2 = this.rightBars[index];
        rectangle2.Width = (int) vector2_2.Y;
        rectangle2.X = this.bounds.X + this.bounds.Width - 4 - (int) vector2_2.Y;
        this.spriteBatch.Draw(Utils.white, rectangle2, (flag1 ? Color.Lerp(this.os.outlineColor, Utils.VeryDarkGray, Utils.randm(0.2f) * Utils.randm(1f)) : this.os.subtleTextColor) * this.fade);
        rectangle2.Y += (int) this.barSize + 1;
        if (rectangle2.Y + rectangle2.Height >= this.bounds.Y + this.bounds.Height)
          break;
      }
      rectangle1 = new Rectangle(this.bounds.X + 2, this.bounds.Y + 38, (this.bounds.Width - 4) / 2, (int) this.barSize);
      rectangle2 = new Rectangle(this.bounds.X + 2, this.bounds.Y + 38, (this.bounds.Width - 4) / 2, (int) this.barSize);
      bool flag2 = flag1;
      for (int index = 0; index < this.leftBars.Count; ++index)
      {
        bool barActive = index <= this.completedIndex || flag2;
        vector2_2 = this.leftBars[index];
        rectangle1.Width = (int) vector2_2.X;
        this.DrawBar(rectangle1, barActive, true);
        rectangle1.Y += (int) this.barSize + 1;
        vector2_2 = this.rightBars[index];
        rectangle2.Width = (int) vector2_2.X;
        rectangle2.X = this.bounds.X + this.bounds.Width - 4 - (int) vector2_2.X;
        this.DrawBar(rectangle2, barActive, false);
        rectangle2.Y += (int) this.barSize + 1;
        if (rectangle2.Y + rectangle2.Height >= this.bounds.Y + this.bounds.Height)
          break;
      }
    }

    private void DrawBar(Rectangle dest, bool barActive, bool isLeft)
    {
      float num = Math.Min(1f, (float) dest.Width / ((float) this.bounds.Width / 2f));
      this.spriteBatch.Draw(Utils.white, dest, (this.hasCompleted ? Color.Lerp(this.os.highlightColor, Utils.AddativeWhite, Math.Max(0.0f, this.sucsessTimer)) : (barActive ? this.activeBarColor : Color.White)) * this.fade);
      if (!barActive)
        return;
      int height = dest.Height;
      dest.Height = 1;
      this.spriteBatch.Draw(Utils.gradientLeftRight, dest, new Rectangle?(), this.activeBarHighlightColor * 0.6f * this.fade * num, 0.0f, Vector2.Zero, isLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.3f);
      ++dest.Y;
      this.spriteBatch.Draw(Utils.gradientLeftRight, dest, new Rectangle?(), this.activeBarHighlightColor * 0.2f * this.fade * num, 0.0f, Vector2.Zero, isLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.3f);
      --dest.Y;
      dest.Height = height;
    }

    public override void Completed()
    {
      base.Completed();
      Computer computer = Programs.getComputer(this.os, this.targetIP);
      if (computer == null)
        return;
      computer.openPort(25, this.os.thisComputer.ip);
    }
  }
}
