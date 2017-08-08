// Decompiled with JetBrains decompiler
// Type: Hacknet.SSLPortExe
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
  internal class SSLPortExe : ExeModule
  {
    private float elapsedTime = 0.0f;
    private SSLPortExe.SSLMode Mode = SSLPortExe.SSLMode.SSH;
    private Vector2 LastCentralRenderOffset = Vector2.Zero;
    private bool IsComplete = false;
    private const float RUN_TIME = 12f;
    private const float IDLE_TIME = 15f;

    private SSLPortExe(Rectangle location, OS operatingSystem, SSLPortExe.SSLMode mode)
      : base(location, operatingSystem)
    {
      this.name = "SSLTrojan";
      this.ramCost = 220;
      this.IdentifierName = "SSLTrojan";
      this.Mode = mode;
      (this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp).hostileActionTaken();
    }

    public static SSLPortExe GenerateInstanceOrNullFromArguments(string[] args, Rectangle location, object osObj, Computer target)
    {
      OS operatingSystem = (OS) osObj;
      if (args.Length < 4)
      {
        operatingSystem.write("--------------------------------------");
        operatingSystem.write("SSLTrojan " + LocaleTerms.Loc("ERROR: Not enough arguments!"));
        operatingSystem.write(LocaleTerms.Loc("Usage:") + " SSLTrojan [" + LocaleTerms.Loc("PortNum") + "] [-" + LocaleTerms.Loc("option") + "] [" + LocaleTerms.Loc("option port num") + "]");
        operatingSystem.write(LocaleTerms.Loc("Valid Options:") + " [-s (SSH)] [-f (FTP)] [-w (HTTP)] [-r (RTSP)]");
        operatingSystem.write("--------------------------------------");
        return (SSLPortExe) null;
      }
      try
      {
        Convert.ToInt32(args[1]);
        int int32_1 = Convert.ToInt32(args[3]);
        string str = args[2].ToLower();
        SSLPortExe.SSLMode mode = SSLPortExe.SSLMode.SSH;
        switch (str)
        {
          case "-s":
            mode = SSLPortExe.SSLMode.SSH;
            break;
          case "-f":
            mode = SSLPortExe.SSLMode.FTP;
            break;
          case "-w":
            mode = SSLPortExe.SSLMode.Web;
            break;
          case "-r":
            mode = SSLPortExe.SSLMode.RTSP;
            break;
          default:
            str = (string) null;
            break;
        }
        if (str == null)
        {
          operatingSystem.write("--------------------------------------");
          operatingSystem.write("SSLTrojan " + string.Format(LocaleTerms.Loc("Error: Mode {0} is invalid."), (object) args[2]));
          operatingSystem.write(LocaleTerms.Loc("Valid Options:") + " [-s (SSH)] [-f (FTP)] [-w (HTTP)] [-r (RTSP)]");
          operatingSystem.write("--------------------------------------");
          return (SSLPortExe) null;
        }
        int num = -1;
        bool flag = false;
        switch (mode)
        {
          case SSLPortExe.SSLMode.SSH:
            flag = target.isPortOpen(22);
            num = target.GetDisplayPortNumberFromCodePort(22);
            break;
          case SSLPortExe.SSLMode.FTP:
            flag = target.isPortOpen(21);
            num = target.GetDisplayPortNumberFromCodePort(21);
            break;
          case SSLPortExe.SSLMode.Web:
            flag = target.isPortOpen(80);
            num = target.GetDisplayPortNumberFromCodePort(80);
            break;
          case SSLPortExe.SSLMode.RTSP:
            flag = target.isPortOpen(554);
            num = target.GetDisplayPortNumberFromCodePort(554);
            break;
        }
        if (!flag)
        {
          operatingSystem.write("--------------------------------------");
          operatingSystem.write("SSLTrojan " + LocaleTerms.Loc("Error: Target bypass port is closed!"));
          return (SSLPortExe) null;
        }
        int int32_2;
        try
        {
          int32_2 = Convert.ToInt32(int32_1);
        }
        catch (FormatException ex)
        {
          operatingSystem.write("--------------------------------------");
          operatingSystem.write("SSLTrojan " + string.Format(LocaleTerms.Loc("Error: Invalid tunnel port number : \"{0}\""), (object) int32_1));
          return (SSLPortExe) null;
        }
        if (int32_2 == num)
          return new SSLPortExe(location, operatingSystem, mode);
        operatingSystem.write("--------------------------------------");
        operatingSystem.write("SSLTrojan " + string.Format(LocaleTerms.Loc("Error: Tunnel port number {0} does not match expected service \"{1}"), (object) int32_2, (object) str));
        return (SSLPortExe) null;
      }
      catch (Exception ex)
      {
        operatingSystem.write("SSLTrojan " + LocaleTerms.Loc("Error:"));
        operatingSystem.write(ex.Message);
        return (SSLPortExe) null;
      }
    }

    public override void Update(float t)
    {
      base.Update(t);
      if ((double) this.elapsedTime < 12.0 && (double) this.elapsedTime + (double) t >= 12.0)
        this.Completed();
      else if ((double) this.elapsedTime < 15.0 && (double) this.elapsedTime + (double) t >= 15.0)
        this.isExiting = true;
      this.elapsedTime += t;
    }

    public override void Completed()
    {
      base.Completed();
      Computer computer = Programs.getComputer(this.os, this.targetIP);
      if (computer == null)
        return;
      computer.openPort(443, this.os.thisComputer.ip);
      this.os.write(":: " + LocaleTerms.Loc("HTTPS SSL Reverse Tunnel Opened"));
      this.IsComplete = true;
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      Rectangle rectangle1 = Utils.InsetRectangle(new Rectangle(this.bounds.X, this.bounds.Y + Module.PANEL_HEIGHT, this.bounds.Width, this.bounds.Height - Module.PANEL_HEIGHT), 2);
      if (this.bounds.Height < Module.PANEL_HEIGHT + 4)
        return;
      float num1 = 1f;
      if ((double) this.elapsedTime < 12.0)
        num1 = this.elapsedTime / 12f;
      float num2 = 0.6f;
      float num3 = Utils.CubicInCurve(Utils.CubicInCurve(Math.Min(1f, num1 * (1f / num2))));
      float num4 = Math.Max(0.0f, Math.Min(1f, (float) (((double) num1 - (double) num2) * (1.0 / (1.0 - (double) num2)))));
      int num5 = 7;
      bool flag = true;
      Vector2 vector2_1 = new Vector2((float) (this.bounds.X + this.bounds.Width / 2), (float) (this.bounds.Y + Module.PANEL_HEIGHT + this.bounds.Height / 2 - 12));
      float num6 = num2 * 12f;
      float num7 = (float) (((double) this.elapsedTime - (double) num6) / (15.0 - (double) num6));
      if ((double) num3 < 0.990000009536743)
        num7 = 0.0f;
      Vector2 left = vector2_1;
      float diamater = (float) this.bounds.Width - (float) (100.0 - (double) num7 * 1500.0);
      float num8 = num4 * 1f;
      Vector2 vector2_2 = new Vector2((float) Math.Sin((double) this.os.timer * 2.0) * (diamater / 4f), (float) Math.Cos((double) this.os.timer * 2.0) * (diamater / 4f));
      Vector2 vector2_3 = vector2_1 - this.LastCentralRenderOffset * num4 * 1f;
      this.LastCentralRenderOffset = Vector2.Zero;
      for (int index = 0; index < num5; ++index)
      {
        int num9 = 13;
        int subdivisions = num9;
        if (index == 0)
          subdivisions = (int) ((double) (num9 - 1) * (double) Math.Min(1f, num3 + 0.0831f)) + 1;
        float nodeDiamater = Math.Max(1f, (float) ((double) num5 - (double) index * 2.0 + 1.0)) + num8;
        float timer = this.os.timer * 2f;
        if (flag)
          timer *= index % 2 == 0 ? 1f : -1f;
        if ((double) index < 1.0 + (double) (int) (6.0 * (double) num4))
        {
          TunnelingCircleEffect.Draw(this.spriteBatch, vector2_3, diamater, nodeDiamater, subdivisions, timer, Color.Lerp(Color.White, Color.Transparent, Utils.QuadraticOutCurve((float) index / (float) num5)), Color.Gray * (float) (0.119999997317791 * (1.0 - (double) num1) + 0.100000001490116), this.os.highlightColor, rectangle1);
          Vector2 leftOut;
          Vector2 rightOut;
          Utils.ClipLineSegmentsForRect(rectangle1, left, vector2_3, out leftOut, out rightOut);
          Utils.drawLineAlt(this.spriteBatch, leftOut, rightOut, Vector2.Zero, this.os.highlightColor * 0.3f, 0.6f, 1f, Utils.gradientLeftRight);
        }
        float num10 = diamater / 4f;
        left = vector2_3;
        Vector2 vector2_4 = new Vector2((float) Math.Sin((double) timer) * num10, (float) Math.Cos((double) timer) * num10);
        this.LastCentralRenderOffset += vector2_4;
        vector2_3 += vector2_4;
        diamater = diamater / 2f - nodeDiamater;
        if (index == num5 - 1 && (double) num3 > 0.990000009536743)
        {
          if ((double) vector2_3.X > (double) rectangle1.X && (double) vector2_3.X < (double) (rectangle1.X + rectangle1.Width) && (double) vector2_3.Y > (double) rectangle1.Y && (double) vector2_3.Y < (double) (rectangle1.Y + rectangle1.Height))
            this.spriteBatch.Draw(Utils.white, vector2_3, new Rectangle?(), (double) num4 > 0.990000009536743 ? this.os.unlockedColor : this.os.highlightColor, 0.0f, Vector2.Zero, Vector2.One * 2f, SpriteEffects.None, 0.7f);
          if (this.IsComplete)
          {
            int num11 = 2;
            Rectangle destinationRectangle = new Rectangle((int) vector2_3.X - num11 / 2, rectangle1.Y, num11, rectangle1.Height);
            this.spriteBatch.Draw(Utils.white, destinationRectangle, Utils.makeColorAddative(this.os.highlightColor));
            Rectangle rectangle2 = new Rectangle(rectangle1.X, (int) vector2_3.Y - num11 / 2, rectangle1.Width, num11);
            if (rectangle2.Y > rectangle1.Y + rectangle1.Height - 30 || rectangle2.Y < rectangle1.Y)
              rectangle2.Y = rectangle1.Y + rectangle1.Height / 2 - num11;
            this.spriteBatch.Draw(Utils.white, rectangle2, Utils.makeColorAddative(this.os.highlightColor));
            rectangle2.Height = Math.Min(24, rectangle1.Height / 2);
            rectangle2.Y = rectangle1.Y + rectangle1.Height / 2 - rectangle2.Height / 2;
            if ((double) Utils.randm(0.25f) + 0.75 < (double) num7)
            {
              this.spriteBatch.Draw(Utils.white, rectangle2, Color.Black * 0.5f);
              TextItem.doFontLabelToSize(rectangle2, "   -  " + LocaleTerms.Loc("SSL TUNNEL COMPLETE") + "  -", GuiData.smallfont, Color.White, true, false);
            }
          }
        }
      }
    }

    private enum SSLMode
    {
      SSH,
      FTP,
      Web,
      RTSP,
    }
  }
}
