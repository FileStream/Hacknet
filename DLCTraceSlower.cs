// Decompiled with JetBrains decompiler
// Type: Hacknet.DLCTraceSlower
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet
{
  internal class DLCTraceSlower : ExeModule
  {
    private string ActiveConnectedCompIP = (string) null;
    private const int TargetRamUse = 600;
    private const float RAM_CHANGE_PS = 200f;
    private const int RAM_STARTING = 50;
    private DepthDotGridEffect dotEffect;

    private DLCTraceSlower(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.name = "SignalScramble";
      this.ramCost = 50;
      this.IdentifierName = "SignalScramble";
      this.dotEffect = new DepthDotGridEffect(this.os.content);
      this.ActiveConnectedCompIP = this.os.connectedComp == null ? this.os.thisComputer.ip : this.os.connectedComp.ip;
    }

    public static DLCTraceSlower GenerateInstanceOrNullFromArguments(string[] args, Rectangle location, object osObj, Computer target)
    {
      OS operatingSystem = (OS) osObj;
      target.hostileActionTaken();
      return new DLCTraceSlower(location, operatingSystem);
    }

    public override void Update(float t)
    {
      base.Update(t);
      if (this.ActiveConnectedCompIP != (this.os.connectedComp == null ? this.os.thisComputer.ip : this.os.connectedComp.ip))
        this.isExiting = true;
      if (600 != this.ramCost)
      {
        if (600 < this.ramCost)
        {
          this.ramCost -= (int) ((double) t * 200.0);
          if (this.ramCost < 600)
            this.ramCost = 600;
        }
        else
        {
          int num = (int) ((double) t * 200.0);
          if (this.os.ramAvaliable >= num)
          {
            this.ramCost += num;
            if (this.ramCost > 600)
              this.ramCost = 600;
          }
        }
      }
      bool flag = !this.isExiting && (this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp).PlayerHasAdminPermissions();
      float point = (float) (((double) this.ramCost - 50.0) / 550.0);
      if (!flag)
        return;
      this.os.traceTracker.trackSpeedFactor = Math.Min(this.os.traceTracker.trackSpeedFactor, 1f - Utils.QuadraticOutCurve(point));
    }

    public override void Killed()
    {
      base.Killed();
      this.os.traceTracker.trackSpeedFactor = 1f;
    }

    public override void Completed()
    {
      base.Completed();
      this.os.traceTracker.trackSpeedFactor = 1f;
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      Rectangle fullAreaDest = Utils.InsetRectangle(new Rectangle(this.bounds.X, this.bounds.Y + Module.PANEL_HEIGHT, this.bounds.Width, this.bounds.Height - Module.PANEL_HEIGHT), 2);
      fullAreaDest.Height = (int) ((double) fullAreaDest.Height * 1.125);
      bool flag1 = this.os.connectedComp != null && this.ramCost >= 600 && this.os.connectedComp.adminIP == this.os.thisComputer.ip;
      bool flag2 = (this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp).PlayerHasAdminPermissions();
      this.dotEffect.DrawGrid(fullAreaDest, Vector2.Zero, this.spriteBatch, 30f, 10, flag2 ? Color.Gray : new Color(200, 15, 15, 0), 60f, 10f, 0.0f, this.os.timer, !flag1 ? 1f : (float) (1.0 - (double) this.ramCost / 600.0));
      if (this.bounds.Height < Module.PANEL_HEIGHT + 4)
        return;
      string str1 = (string) null;
      string str2;
      if (this.ramCost < 600)
      {
        if (this.isExiting)
        {
          str2 = LocaleTerms.Loc("Spinning down...");
        }
        else
        {
          str2 = LocaleTerms.Loc("Spinning up...");
          float num = (float) ((double) this.ramCost / 600.0 * 100.0);
          string str3 = num.ToString("00");
          string str4 = ".";
          num = Utils.randm(99f);
          string str5 = num.ToString("00");
          string str6 = "%";
          str1 = str3 + str4 + str5 + str6;
        }
      }
      else
      {
        str2 = LocaleTerms.Loc("Suppression Active");
        if (!flag2)
          str2 = LocaleTerms.Loc("Administrator Access Required");
      }
      int num1 = 27;
      if (this.bounds.Height > 40)
      {
        string text = "[ " + str2 + " ]";
        Rectangle destinationRectangle = new Rectangle(fullAreaDest.X, fullAreaDest.Y + fullAreaDest.Height - num1 - 10 - num1, (int) ((double) fullAreaDest.Width * 0.920000016689301), num1 + 2);
        destinationRectangle.Width += 8;
        this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Black * 0.9f);
        destinationRectangle.Width -= 8;
        destinationRectangle.Y += 4;
        destinationRectangle.Height -= 8;
        Vector2 vector2_1 = GuiData.font.MeasureString(text);
        float num2 = str1 != null ? 0.7f : 1f;
        float num3 = Math.Min(1f, Math.Min((float) destinationRectangle.Width * num2 / vector2_1.X, (float) destinationRectangle.Height / vector2_1.Y));
        Vector2 vector2_2 = new Vector2((float) destinationRectangle.Width - vector2_1.X * num3, (float) (destinationRectangle.Height / 2) - (float) ((double) vector2_1.Y * (double) num3 / 2.0));
        Color color = Color.Lerp(Color.White, this.os.highlightColor, 0.4f + Utils.randm(0.1f));
        this.spriteBatch.DrawString(GuiData.font, text, new Vector2((float) destinationRectangle.X, (float) destinationRectangle.Y) + vector2_2, color, 0.0f, Vector2.Zero, new Vector2(num3), SpriteEffects.None, 0.5f);
        if (str1 != null)
        {
          Vector2 vector2_3 = new Vector2((float) destinationRectangle.X + 2f, (float) destinationRectangle.Y + vector2_2.Y);
          for (int index = 0; index < str1.Length; ++index)
          {
            this.spriteBatch.DrawString(GuiData.font, string.Concat((object) str1[index]), vector2_3 + ((int) str1[index] == 46 ? new Vector2(3f, 0.0f) : Vector2.Zero), color, 0.0f, Vector2.Zero, new Vector2(num3), SpriteEffects.None, 0.5f);
            vector2_3.X += (float) ((double) vector2_1.X / (double) text.Length * (double) num3 + 2.0);
          }
        }
      }
      Rectangle destinationRectangle1 = new Rectangle(fullAreaDest.X, fullAreaDest.Y + fullAreaDest.Height - num1, fullAreaDest.Width, 25);
      this.spriteBatch.Draw(Utils.white, destinationRectangle1, Color.Black);
      if (!Button.doButton(19371002 + this.PID, destinationRectangle1.X + 1, destinationRectangle1.Y, destinationRectangle1.Width - 2, destinationRectangle1.Height, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
        return;
      this.isExiting = true;
      this.os.traceTracker.trackSpeedFactor = 1f;
    }
  }
}
