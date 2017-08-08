// Decompiled with JetBrains decompiler
// Type: Hacknet.TrailLoadingSpinnerEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet
{
  internal class TrailLoadingSpinnerEffect
  {
    private RenderTarget2D target;
    private SpriteBatch internalSB;
    private Texture2D circle;
    private FlyoutEffect flyout;

    public TrailLoadingSpinnerEffect(OS operatingSystem)
    {
      this.circle = TextureBank.load("Circle", operatingSystem.content);
    }

    public void Draw(Rectangle bounds, SpriteBatch spriteBatch, float totalTime, float timeRemaining, float extraTime = 0.0f, Color? color = null)
    {
      Color c = color.HasValue ? color.Value : OS.currentInstance.highlightColor;
      Rectangle destinationRectangle = new Rectangle(bounds.X + 2, bounds.Y + 2, bounds.Width - 4, bounds.Height - 3);
      bool flag = false;
      if (this.target == null)
      {
        this.target = new RenderTarget2D(spriteBatch.GraphicsDevice, destinationRectangle.Width, destinationRectangle.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        this.internalSB = new SpriteBatch(spriteBatch.GraphicsDevice);
        flag = true;
      }
      RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
      spriteBatch.GraphicsDevice.SetRenderTarget(this.target);
      Rectangle dest = new Rectangle(0, 0, destinationRectangle.Width, destinationRectangle.Height);
      this.internalSB.Begin();
      if (flag)
        this.internalSB.GraphicsDevice.Clear(Color.Transparent);
      this.DrawLoading(timeRemaining, totalTime, dest, this.internalSB, c, extraTime);
      this.internalSB.End();
      spriteBatch.GraphicsDevice.SetRenderTarget(currentRenderTarget);
      spriteBatch.Draw((Texture2D) this.target, destinationRectangle, Color.White);
    }

    public void Draw2(Rectangle bounds, SpriteBatch spriteBatch, float totalTime, float timeRemaining, float extraTime = 0.0f, float dt = 0.0f)
    {
      if (this.flyout == null)
        this.flyout = new FlyoutEffect(GuiData.spriteBatch.GraphicsDevice, OS.currentInstance.content, bounds.Width, bounds.Height);
      this.flyout.Draw(dt, bounds, spriteBatch, (Action<SpriteBatch, Rectangle>) ((sb, dest) => this.DrawLoading(timeRemaining, totalTime, dest, sb, OS.currentInstance.highlightColor, extraTime)));
    }

    private void DrawLoading(float timeRemaining, float totalTime, Rectangle dest, SpriteBatch sb, Color c, float timeAdd = 0.0f)
    {
      float loaderRadius = 20f;
      float num1 = (float) ((double) dest.Width / 2.0 - ((double) loaderRadius + 2.0));
      Vector2 loaderCentre = new Vector2((float) dest.X + (float) dest.Width / 2f, (float) dest.Y + (float) dest.Height / 2f);
      float num2 = totalTime - timeRemaining + timeAdd;
      float num3 = Math.Min(1f, (num2 + timeAdd) / totalTime);
      Rectangle destinationRectangle = new Rectangle(0, 0, this.target.Width, this.target.Height);
      sb.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray * 0.05f);
      this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, loaderRadius, num2 * 0.2f, 1f, sb, Utils.AddativeWhite, 10, 1f);
      this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, loaderRadius + num1 / 2f * num3, num2 * -0.4f, 0.7f, sb, Utils.AddativeWhite * (1f - num3), 10, 0.8f);
      this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, loaderRadius + num1 * num3, num2 * 0.5f, 0.52f, sb, Utils.AddativeWhite * (1f - num3), 10, 0.8f);
      int num4 = 30;
      for (int index = 0; index < num4; ++index)
      {
        float num5 = (float) index / (float) num4;
        this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, loaderRadius + num1 * num5 * num3, num2 * -0.4f * num5, 2f * num5, sb, c, 6, (float) (0.200000002980232 + (double) num5 * 0.200000002980232));
      }
    }

    private void DrawLoadingCircle(float timeRemaining, float totalTime, Rectangle dest, Vector2 loaderCentre, float loaderRadius, float baseRotationAdd, float rotationRateRPS, SpriteBatch sb, Color c, int NumberOfCircles = 10, float scaleMod = 1f)
    {
      float num1 = totalTime - timeRemaining;
      for (int index = 0; index < NumberOfCircles; ++index)
      {
        float num2 = (float) index / (float) NumberOfCircles;
        float num3 = 2f;
        float num4 = 1f;
        float num5 = 6.283185f;
        float num6 = num5 + num4;
        float num7 = num2 * num3;
        if ((double) num1 > (double) num7)
        {
          float num8 = num1 / num7 * rotationRateRPS % num6;
          if ((double) num8 >= (double) num5)
            num8 = 0.0f;
          float angle = num5 * Utils.QuadraticOutCurve(num8 / num5) + baseRotationAdd;
          Vector2 vector2_1 = loaderCentre + Utils.PolarToCartesian(angle, loaderRadius);
          sb.Draw(this.circle, vector2_1, new Rectangle?(), c, 0.0f, Vector2.Zero, (float) ((double) scaleMod * 0.100000001490116 * ((double) loaderRadius / 120.0)), SpriteEffects.None, 0.3f);
          if (Utils.random.NextDouble() < 0.001)
          {
            Vector2 vector2_2 = loaderCentre + Utils.PolarToCartesian(angle, 20f + Utils.randm(45f));
            sb.Draw(Utils.white, vector2_1, Utils.AddativeWhite);
            Utils.drawLine(sb, vector2_1, vector2_2, Vector2.Zero, Utils.AddativeWhite * 0.4f, 0.1f);
          }
        }
      }
    }
  }
}
