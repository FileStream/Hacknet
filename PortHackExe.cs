// Decompiled with JetBrains decompiler
// Type: Hacknet.PortHackExe
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
  internal class PortHackExe : ExeModule
  {
    public static float CRACK_TIME = 6f;
    public static float TIME_BETWEEN_TEXT_SWITCH = 0.06f;
    public static float TIME_ALIVE_AFTER_SUCSESS = 5f;
    public static float COMPLETE_LIGHT_FLASH_TIME = 2f;
    private float progress = 0.0f;
    private float textSwitchTimer = PortHackExe.TIME_BETWEEN_TEXT_SWITCH;
    private int textOffsetIndex = 0;
    private float sucsessTimer = PortHackExe.TIME_ALIVE_AFTER_SUCSESS;
    private bool hasCompleted = false;
    private PortHackCubeSequence cubeSeq = new PortHackCubeSequence();
    private bool IsTargetingPorthackHeart = false;
    private bool hasCheckedForheart = false;
    private bool StopProgress = false;
    private int[] textIndex;
    private Computer target;
    private RenderTarget2D renderTarget;

    public PortHackExe(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.IdentifierName = "PortHack";
    }

    public override void LoadContent()
    {
      base.LoadContent();
      int num = PortExploits.passwords.Count / 3;
      this.textIndex = new int[3];
      this.textIndex[0] = 0;
      this.textIndex[1] = num;
      this.textIndex[2] = 2 * num;
      this.target = Programs.getComputer(this.os, this.targetIP);
      this.os.write("Porthack Initialized -- Running...");
    }

    public override void Update(float t)
    {
      base.Update(t);
      if (this.IsTargetingPorthackHeart)
        this.progress = Utils.rand(0.98f);
      else if (!this.StopProgress)
        this.progress += t / PortHackExe.CRACK_TIME;
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
      else
        this.target.hostileActionTaken();
      if ((double) this.progress < 1.0)
      {
        this.textSwitchTimer -= t;
        if ((double) this.textSwitchTimer <= 0.0)
        {
          this.textSwitchTimer = PortHackExe.TIME_BETWEEN_TEXT_SWITCH;
          ++this.textOffsetIndex;
        }
      }
      if (!this.hasCheckedForheart && (double) this.progress > 0.5)
      {
        this.hasCheckedForheart = true;
        PorthackHeartDaemon daemon = this.target.getDaemon(typeof (PorthackHeartDaemon)) as PorthackHeartDaemon;
        if (daemon != null)
        {
          this.IsTargetingPorthackHeart = true;
          if (this.os.connectedComp != null && this.os.connectedComp.ip == this.target.ip)
            daemon.BreakHeart();
          this.cubeSeq.ShouldCentralSpinInfinitley = true;
        }
      }
      if (!((this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp).ip != this.target.ip))
        return;
      this.StopProgress = true;
      this.isExiting = true;
    }

    public override void Completed()
    {
      base.Completed();
      this.os.takeAdmin(this.targetIP);
      this.os.write("--Porthack Complete--");
      this.os.PorthackCompleteFlashTime = PortHackExe.COMPLETE_LIGHT_FLASH_TIME;
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      Rectangle destinationRectangle1 = new Rectangle(this.bounds.X + 1, this.bounds.Y + Module.PANEL_HEIGHT + 1, this.bounds.Width - 2, this.bounds.Height - (Module.PANEL_HEIGHT + 2));
      if (this.renderTarget == null)
        this.renderTarget = new RenderTarget2D(this.spriteBatch.GraphicsDevice, destinationRectangle1.Width, destinationRectangle1.Height);
      RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
      this.spriteBatch.GraphicsDevice.SetRenderTarget(this.renderTarget);
      this.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
      try
      {
        this.cubeSeq.DrawSequence(new Rectangle(0, 0, destinationRectangle1.Width, destinationRectangle1.Height), t, PortHackExe.CRACK_TIME);
      }
      catch (Exception ex)
      {
        Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex) + "\r\n\r\n");
      }
      this.spriteBatch.GraphicsDevice.SetRenderTarget(currentRenderTarget);
      Rectangle bounds = this.bounds;
      ++bounds.X;
      ++bounds.Y;
      bounds.Width -= 2;
      bounds.Height -= 2;
      this.drawOutline();
      this.spriteBatch.Draw((Texture2D) this.renderTarget, destinationRectangle1, Utils.AddativeWhite * ((double) this.progress >= 1.0 ? 0.2f : 0.6f));
      this.drawTarget("app:");
      if ((double) this.progress < 1.0)
      {
        Rectangle destinationRectangle2 = new Rectangle(this.bounds.X, this.bounds.Y + Module.PANEL_HEIGHT + 1, (int) ((double) this.bounds.Width / 2.0), this.bounds.Height - (Module.PANEL_HEIGHT + 2));
        this.spriteBatch.Draw(Utils.gradientLeftRight, destinationRectangle2, new Rectangle?(), Color.Black * 0.9f, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.2f);
        destinationRectangle2.X += this.bounds.Width - destinationRectangle2.Width - 1;
        this.spriteBatch.Draw(Utils.gradientLeftRight, destinationRectangle2, new Rectangle?(), Color.Black * 0.9f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.3f);
      }
      int num1 = (this.bounds.Height - 16 - 16 - 4) / 12;
      Vector2 position = new Vector2((float) (this.bounds.X + 3), (float) (this.bounds.Y + 20));
      Color color = Color.White * this.fade;
      if (this.IsTargetingPorthackHeart)
        color *= 0.5f;
      if ((double) this.progress >= 1.0)
        color = Color.Gray * this.fade;
      for (int index1 = 0; index1 < num1; ++index1)
      {
        int index2 = (this.textIndex[0] + this.textOffsetIndex + index1) % (PortExploits.passwords.Count - 1);
        this.spriteBatch.DrawString(GuiData.UITinyfont, PortExploits.passwords[index2], position, color);
        int index3 = (this.textIndex[1] + this.textOffsetIndex + index1) % (PortExploits.passwords.Count - 1);
        Vector2 vector2 = GuiData.UITinyfont.MeasureString(PortExploits.passwords[index3]);
        position.X = (float) ((double) (this.bounds.X + this.bounds.Width) - (double) vector2.X - 3.0);
        Rectangle destinationRectangle2 = new Rectangle((int) position.X - 1, (int) position.Y, (int) vector2.X, 12);
        if (Settings.ActiveLocale != "en-us")
          destinationRectangle2.Y += 6;
        this.spriteBatch.Draw(Utils.white, destinationRectangle2, Color.Black * 0.8f);
        this.spriteBatch.DrawString(GuiData.UITinyfont, PortExploits.passwords[index3], position, color);
        position.X = (float) (this.bounds.X + 3);
        position.Y += 12f;
      }
      if ((double) this.progress >= 1.0)
      {
        string str1 = "PASSWORD";
        Vector2 vector2_1 = GuiData.font.MeasureString(str1);
        position.X = (float) (this.bounds.X + this.bounds.Width / 2) - vector2_1.X / 2f;
        position.Y = (float) (this.bounds.Y + this.bounds.Height / 2) - vector2_1.Y;
        float num2 = Utils.QuadraticOutCurve(Math.Min(2f, PortHackExe.TIME_ALIVE_AFTER_SUCSESS - this.sucsessTimer) / 2f);
        Rectangle destinationRectangle2 = new Rectangle(0, (int) position.Y - 3, (int) ((double) num2 * (double) this.bounds.Width - 30.0), 2);
        destinationRectangle2.X = this.bounds.X + (this.bounds.Width / 2 - destinationRectangle2.Width / 2);
        if (destinationRectangle2.Y > this.bounds.Y)
          this.spriteBatch.Draw(Utils.white, destinationRectangle2, Utils.AddativeWhite * this.fade);
        this.spriteBatch.DrawString(GuiData.font, Utils.FlipRandomChars(str1, 0.012), position, Color.White * this.fade);
        this.spriteBatch.DrawString(GuiData.font, Utils.FlipRandomChars(str1, 0.11), position, Utils.AddativeWhite * 0.2f * this.fade * this.fade);
        string str2 = "FOUND";
        Vector2 vector2_2 = GuiData.font.MeasureString(str2);
        position.X = (float) (this.bounds.X + this.bounds.Width / 2) - vector2_2.X / 2f;
        position.Y += 35f;
        this.spriteBatch.DrawString(GuiData.font, Utils.FlipRandomChars(str2, 0.012), position, Color.White * this.fade);
        this.spriteBatch.DrawString(GuiData.font, Utils.FlipRandomChars(str2, 0.11), position, Utils.AddativeWhite * 0.2f * this.fade * this.fade);
        destinationRectangle2.Y = (int) ((double) position.Y + 35.0 + 3.0);
        if (destinationRectangle2.Y > this.bounds.Y)
          this.spriteBatch.Draw(Utils.white, destinationRectangle2, Utils.AddativeWhite * this.fade);
      }
      bounds.X += 2;
      bounds.Width -= 4;
      bounds.Y = this.bounds.Y + this.bounds.Height - 2 - 16;
      bounds.Height = 16;
      this.spriteBatch.Draw(Utils.white, bounds, this.os.outlineColor * this.fade);
      ++bounds.X;
      ++bounds.Y;
      bounds.Width -= 2;
      bounds.Height -= 2;
      if (this.IsTargetingPorthackHeart)
      {
        this.spriteBatch.Draw(Utils.white, bounds, Color.DarkRed * (Utils.rand(0.3f) + 0.7f) * this.fade);
        TextItem.doFontLabelToSize(bounds, LocaleTerms.Loc("UNKNOWN ERROR"), GuiData.font, Utils.AddativeWhite, false, false);
      }
      else
      {
        this.spriteBatch.Draw(Utils.white, bounds, this.os.darkBackgroundColor * this.fade);
        bounds.Width = (int) ((double) bounds.Width * (double) this.progress);
        this.spriteBatch.Draw(Utils.white, bounds, this.os.highlightColor * this.fade);
      }
    }
  }
}
