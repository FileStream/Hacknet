// Decompiled with JetBrains decompiler
// Type: Hacknet.LogoCustomConnectDisplayDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Hacknet
{
  internal class LogoCustomConnectDisplayDaemon : CustomConnectDisplayDaemon
  {
    private bool LogoShouldClipOverdraw = true;
    private const int ButtonLowerHeight = 80;
    public AlignmentX ButtonsAlignment;
    private string logoImageName;
    private string titleImageName;
    private string buttonAlignmentName;
    private Texture2D LogoImage;
    private Texture2D TitleImage;

    public LogoCustomConnectDisplayDaemon(Computer c, OS os, string logoImageName, string titleImageName, bool logoShouldClipoverdraw, string buttonAlignment)
      : base(c, LocaleTerms.Loc("Logo Display Override"), os)
    {
      this.isListed = false;
      this.logoImageName = logoImageName;
      this.titleImageName = titleImageName;
      this.LogoShouldClipOverdraw = logoShouldClipoverdraw;
      this.buttonAlignmentName = buttonAlignment;
      if (!Enum.TryParse<AlignmentX>(buttonAlignment, true, out this.ButtonsAlignment))
        this.ButtonsAlignment = AlignmentX.Right;
      string path1 = Utils.GetFileLoadPrefix() + logoImageName;
      if (path1.EndsWith(".jpg") || path1.EndsWith(".png"))
      {
        using (FileStream fileStream = File.OpenRead(path1))
          this.LogoImage = Texture2D.FromStream(Game1.getSingleton().GraphicsDevice, (Stream) fileStream);
      }
      else
        this.LogoImage = os.content.Load<Texture2D>(logoImageName);
      string path2 = Utils.GetFileLoadPrefix() + titleImageName;
      if (path2.EndsWith(".jpg") || path2.EndsWith(".png"))
      {
        using (FileStream fileStream = File.OpenRead(path2))
          this.TitleImage = Texture2D.FromStream(Game1.getSingleton().GraphicsDevice, (Stream) fileStream);
      }
      else
        this.TitleImage = os.content.Load<Texture2D>(titleImageName);
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
    }

    internal override void DrawAdminDisplay(Rectangle bounds, SpriteBatch sb, Computer c)
    {
      this.DrawLogoAndTitle(bounds, sb);
      int width = bounds.Width;
      float num1 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(this.timeInThisState / 1f));
      if ((double) this.timeInThisState < 1.0)
        width = (int) ((double) bounds.Width * (double) num1);
      float num2 = 110f;
      float num3 = 40f;
      float num4 = 40f;
      float num5 = 8f;
      float num6 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(Math.Min(1f, (float) (((double) this.timeInThisState - 1.0) / 1.0))));
      if ((double) this.timeInThisState < 1.0)
        num6 = 0.0f;
      int height = (int) ((double) num3 + ((double) num2 - (double) num3) * (1.0 - (double) num6));
      if ((double) this.timeInThisState < 1.0)
        height = (int) ((double) height * (0.5 + 0.5 * (double) num1));
      int stripHeight = (int) ((double) num5 + ((double) num4 - (double) num5) * (1.0 - (double) num6));
      int num7 = bounds.Height / 2 - height / 2;
      int y1 = (int) ((double) bounds.Y + (double) num7 * (1.0 - (double) num6) - 60.0 * (double) num6);
      this.DrawCautionLinedMessage(new Rectangle(bounds.X + bounds.Width / 2 - width / 2, y1, width, height), stripHeight, this.os.highlightColor, (double) this.timeInThisState > 0.5 + (double) Utils.randm(0.5f) ? LocaleTerms.Loc("ACCESS GRANTED") : "", sb, PatternDrawer.warningStripe, -2);
      int num8 = bounds.X + 20;
      int y2 = bounds.Y + 80 + 2;
      if ((double) this.timeInThisState < 2.0)
        return;
      this.DrawConnectButtons(bounds, sb, c, 20, y2, this.ButtonsAlignment);
    }

    internal override void DrawNonAdminDisplay(Rectangle dest, SpriteBatch sb)
    {
      this.DrawLogoAndTitle(dest, sb);
      base.DrawNonAdminDisplay(dest, sb);
    }

    private void DrawLogoAndTitle(Rectangle dest, SpriteBatch sb)
    {
      AlignmentX align1 = AlignmentX.Left;
      AlignmentX align2 = AlignmentX.Right;
      if (this.ButtonsAlignment == AlignmentX.Right)
      {
        align1 = AlignmentX.Right;
        align2 = AlignmentX.Left;
      }
      Color white = Color.White;
      bool flag = this.comp.adminIP == this.os.thisComputer.adminIP;
      if (!flag)
        white *= 0.4f;
      int num1 = this.LogoShouldClipOverdraw ? (int) ((double) dest.Width * 0.620000004768372) : (int) ((double) dest.Width * 0.449999988079071);
      Rectangle targetBounds1 = new Rectangle(dest.X + 1 + Utils.GetXForAlignment(align2, dest.Width, this.LogoShouldClipOverdraw ? -130 : 0, num1), dest.Y + (this.LogoShouldClipOverdraw ? -50 : 20), num1, (int) ((double) num1 * ((double) this.LogoImage.Height / (double) this.LogoImage.Width)));
      Rectangle fullBounds = new Rectangle(dest.X, dest.Y - 20, dest.Width, dest.Height + 10);
      if (!flag)
        fullBounds = dest;
      Utils.RenderSpriteIntoClippedRectDest(fullBounds, targetBounds1, this.LogoImage, white, sb);
      int num2 = dest.Width / 2;
      int height = (int) ((double) num2 * ((double) this.TitleImage.Height / (double) this.TitleImage.Width));
      Rectangle targetBounds2 = new Rectangle(dest.X + Utils.GetXForAlignment(align1, dest.Width, 20, num2), dest.Y + 80 - (height - 40), num2, height);
      Utils.RenderSpriteIntoClippedRectDest(dest, targetBounds2, this.TitleImage, white, sb);
    }

    public override string getSaveString()
    {
      return string.Format("<LogoCustomConnectDisplayDaemon logo=\"{0}\" title=\"{1}\" overdrawLogo=\"{2}\" buttonAlignment=\"{3}\" />", (object) this.logoImageName, (object) this.titleImageName, this.LogoShouldClipOverdraw ? (object) "true" : (object) "false", (object) this.buttonAlignmentName);
    }
  }
}
