// Decompiled with JetBrains decompiler
// Type: Hacknet.CustomConnectDisplayDaemon
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
  internal class CustomConnectDisplayDaemon : CustomConnectDisplayOverride
  {
    private bool HasBeenAdminBefore = false;
    internal float timeInThisState = 0.0f;
    private MovingBarsEffect topEffect;
    private MovingBarsEffect botEffect;

    public CustomConnectDisplayDaemon(Computer c, OS os)
      : base(c, LocaleTerms.Loc("Display Override"), os)
    {
      this.topEffect = new MovingBarsEffect();
      this.botEffect = new MovingBarsEffect()
      {
        IsInverted = true
      };
    }

    public CustomConnectDisplayDaemon(Computer c, string name, OS os)
      : base(c, name, os)
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
      this.timeInThisState += (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
      this.topEffect.Update((float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
      this.botEffect.Update((float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
      int num1 = bounds.X + 20;
      int num2 = bounds.Y + 60;
      Computer c = this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp;
      if (!(c.adminIP == this.os.thisComputer.adminIP))
      {
        this.DrawNonAdminDisplay(bounds, sb);
        this.HasBeenAdminBefore = false;
      }
      else
      {
        if (!this.HasBeenAdminBefore)
        {
          this.timeInThisState = 0.0f;
          this.HasBeenAdminBefore = true;
        }
        this.DrawAdminDisplay(bounds, sb, c);
      }
    }

    internal virtual void DrawAdminDisplay(Rectangle bounds, SpriteBatch sb, Computer c)
    {
      int width = bounds.Width;
      float num1 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(this.timeInThisState / 1f));
      if ((double) this.timeInThisState < 1.0)
        width = (int) ((double) bounds.Width * (double) num1);
      float num2 = 120f;
      float num3 = 60f;
      float num4 = 30f;
      float num5 = 10f;
      float num6 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(Math.Min(1f, (float) (((double) this.timeInThisState - 1.0) / 1.0))));
      if ((double) this.timeInThisState < 1.0)
        num6 = 0.0f;
      int height = (int) ((double) num3 + ((double) num2 - (double) num3) * (1.0 - (double) num6));
      if ((double) this.timeInThisState < 1.0)
        height = (int) ((double) height * (0.5 + 0.5 * (double) num1));
      int stripHeight = (int) ((double) num5 + ((double) num4 - (double) num5) * (1.0 - (double) num6));
      int num7 = bounds.Height / 2 - height / 2;
      int y1 = (int) ((double) bounds.Y + (double) num7 * (1.0 - (double) num6));
      this.DrawCautionLinedMessage(new Rectangle(bounds.X + bounds.Width / 2 - width / 2, y1, width, height), stripHeight, this.os.highlightColor, (double) this.timeInThisState > 0.5 + (double) Utils.randm(0.5f) ? "ACCESS GRANTED" : "", sb, PatternDrawer.warningStripe, 0);
      int num8 = bounds.X + 20;
      int y2 = bounds.Y + 80 + 2;
      if ((double) this.timeInThisState < 2.0)
        return;
      this.DrawConnectButtons(bounds, sb, c, 20, y2, AlignmentX.Middle);
    }

    internal virtual void DrawConnectButtons(Rectangle bounds, SpriteBatch sb, Computer c, int margin, int y, AlignmentX ButtonAlignment = AlignmentX.Middle)
    {
      int num1 = bounds.X + margin;
      int num2 = c.daemons.Count + 6;
      int height = 40;
      int num3 = bounds.Height - (y - bounds.Y) - 20 - num2 * 5;
      if ((double) num3 / (double) num2 < (double) height)
        height = (int) ((double) num3 / (double) num2);
      int width = bounds.Width / 2;
      int x;
      switch (ButtonAlignment)
      {
        case AlignmentX.Left:
          x = bounds.X + margin;
          break;
        case AlignmentX.Right:
          x = bounds.X + bounds.Width - (width + margin);
          break;
        default:
          x = bounds.X + bounds.Width / 2 - width / 2;
          break;
      }
      if (c.daemons.Count > 0)
        y += height + 5;
      for (int index = 0; index < c.daemons.Count; ++index)
      {
        if (!(c.daemons[index] is CustomConnectDisplayOverride))
        {
          if (Button.doButton(29000 + index, x, y, width, height, c.daemons[index].name, new Color?(this.os.highlightColor)))
          {
            this.os.display.command = c.daemons[index].name;
            c.daemons[index].navigatedTo();
          }
          y += height + 5;
        }
      }
      if (Button.doButton(300000, x, y, width, height, LocaleTerms.Loc("Login"), new Color?(this.os.subtleTextColor)))
      {
        this.os.runCommand("login");
        this.os.terminal.clearCurrentLine();
      }
      y += height + 5;
      if (Button.doButton(300002, x, y, width, height, LocaleTerms.Loc("Probe System"), new Color?(this.os.highlightColor)))
        this.os.runCommand("probe");
      y += height + 5;
      if (Button.doButton(300003, x, y, width, height, LocaleTerms.Loc("View Filesystem"), new Color?(this.os.hasConnectionPermission(false) ? this.os.highlightColor : this.os.subtleTextColor)))
        this.os.runCommand("ls");
      y += height + 5;
      if (Button.doButton(300006, x, y, width, height, LocaleTerms.Loc("View Logs"), new Color?(this.os.hasConnectionPermission(false) ? this.os.highlightColor : this.os.subtleTextColor)))
        this.os.runCommand("cd log");
      y += height + 5;
      if (Button.doButton(300009, x, y, width, height, LocaleTerms.Loc("Scan Network"), new Color?(this.os.hasConnectionPermission(true) ? this.os.highlightColor : this.os.subtleTextColor)))
        this.os.runCommand("scan");
      y = bounds.Y + bounds.Height - 30;
      if (!Button.doButton(300012, x, y, width, 20, LocaleTerms.Loc("Disconnect"), new Color?(this.os.lockedColor)))
        return;
      this.os.runCommand("dc");
    }

    internal virtual void DrawCautionLinedMessage(Rectangle dest, int stripHeight, Color color, string Message, SpriteBatch sb, Texture2D stripTexture = null, int textOffsetY = 0)
    {
      if (stripTexture == null)
        stripTexture = PatternDrawer.warningStripe;
      Rectangle dest1 = dest;
      dest1.Height = stripHeight;
      PatternDrawer.draw(dest1, 1f, Color.Transparent, color, sb, stripTexture);
      dest1 = new Rectangle(dest1.X, dest.Y + dest.Height - stripHeight, dest1.Width, stripHeight);
      PatternDrawer.draw(dest1, 1f, Color.Transparent, color, sb, stripTexture);
      int height = dest.Height - dest1.Height * 2 + dest.Height / 10;
      Rectangle dest2 = new Rectangle(dest.X, dest.Y + stripHeight + 2 + textOffsetY, dest.Width, height);
      if (Settings.ActiveLocale == "en-us")
        TextItem.doFontLabelToSize(dest2, Message, GuiData.titlefont, color, true, false);
      else
        TextItem.doCenteredFontLabel(dest2, Message, GuiData.font, color, false);
      Rectangle rectangle = dest2;
      rectangle.Y = dest.Y + dest.Height + 2;
      rectangle.Height = 30;
    }

    internal virtual void DrawNonAdminDisplay(Rectangle dest, SpriteBatch sb)
    {
      Rectangle destinationRectangle = new Rectangle(dest.X + 1, dest.Y, dest.Width, 1);
      sb.Draw(Utils.white, destinationRectangle, Utils.AddativeRed);
      int height1 = 120;
      Rectangle rectangle = new Rectangle(dest.X, dest.Y + (dest.Height / 2 - height1), dest.Width, height1);
      sb.Draw(Utils.white, rectangle, Color.Black * 0.8f);
      int stripHeight = 20;
      this.DrawCautionLinedMessage(Utils.InsetRectangle(rectangle, 4), stripHeight, Color.Red, "ACCESS DENIED", sb, (Texture2D) null, 0);
      int height2 = 90;
      Rectangle dest1 = Utils.InsetRectangle(new Rectangle(rectangle.X, rectangle.Y + stripHeight + 2, rectangle.Width, height2), 20);
      dest1.Y = rectangle.Y + rectangle.Height + 2;
      dest1.Height = 30;
      TextItem.doFontLabelToSize(dest1, LocaleTerms.Loc("Non-Admin Account Detected - Login to Proceed or Disconnect Now"), GuiData.font, Color.Black, true, false);
      TextItem.doFontLabelToSize(dest1, LocaleTerms.Loc("Non-Admin Account Detected - Login to Proceed or Disconnect Now"), GuiData.font, Color.Red * 0.7f, true, false);
      int width = 340;
      int x = dest.X + dest.Width / 2 - width / 2;
      int y1 = dest1.Y + dest1.Height + 10;
      int height3 = 40;
      if (Button.doButton(302001, x, y1, width, height3, LocaleTerms.Loc("Login"), new Color?(Color.Gray)))
      {
        this.os.runCommand("login");
        this.os.terminal.clearCurrentLine();
      }
      int y2 = y1 + (height3 + 6);
      if (!Button.doButton(302003, x, y2, width, 40, LocaleTerms.Loc("Disconnect"), new Color?(Color.Red)))
        return;
      this.os.runCommand("dc");
    }

    public override string getSaveString()
    {
      return "<CustomConnectDisplayDaemon />";
    }
  }
}
