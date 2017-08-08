// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.FlyoutEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.Effects
{
  public class FlyoutEffect : IDisposable
  {
    private Vector2 offset = Vector2.Zero;
    private float timeSinceRender = 0.0f;
    private float rotation = 0.0f;
    private float elapsedTime = 0.0f;
    public float TimeBetweenRenderings = 0.1f;
    private static RenderTarget2D PooledTarget;
    private static RenderTarget2D PooledBackTarget;
    private RenderTarget2D DrawTarget;
    private RenderTarget2D BackTarget;
    private SpriteBatch innerBatch;
    private Texture2D UsedSprite;
    private Texture2D FlashSprite;

    public FlyoutEffect(GraphicsDevice gd, ContentManager c, int width, int height)
    {
      if (FlyoutEffect.PooledTarget != null && FlyoutEffect.PooledTarget.Width == width && FlyoutEffect.PooledTarget.Height == height)
      {
        this.DrawTarget = FlyoutEffect.PooledTarget;
        this.BackTarget = FlyoutEffect.PooledBackTarget;
        FlyoutEffect.PooledTarget = (RenderTarget2D) null;
      }
      else
      {
        this.DrawTarget = new RenderTarget2D(gd, width, height);
        this.BackTarget = new RenderTarget2D(gd, width, height);
      }
      this.innerBatch = new SpriteBatch(gd);
      this.UsedSprite = c.Load<Texture2D>("CircleOutlineLarge");
      this.FlashSprite = c.Load<Texture2D>("EffectFiles/PointClicker/Star");
    }

    public void Draw(float dt, Rectangle dest, SpriteBatch sb, float cornerIn, int spiralElements, float spiralRadius, Color color, bool drawFlashFromMiddle, bool flashBackground)
    {
      this.elapsedTime += dt;
      RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
      sb.GraphicsDevice.SetRenderTarget(this.DrawTarget);
      sb.GraphicsDevice.Clear(Color.Transparent);
      Rectangle rectangle = new Rectangle(0, 0, this.DrawTarget.Width, this.DrawTarget.Height);
      Rectangle destinationRectangle = Utils.InsetRectangle(rectangle, 2);
      this.rotation += dt * 3f;
      this.offset = new Vector2((float) (Math.Cos((double) this.elapsedTime * 2.0) * 40.0), -1f * (float) (Math.Cos((double) this.elapsedTime * 8.0) * 40.0));
      Vector2 origin = new Vector2((float) (this.DrawTarget.Width / 2), (float) (this.DrawTarget.Height / 2));
      this.innerBatch.Begin();
      destinationRectangle.X = this.DrawTarget.Width / 2;
      destinationRectangle.Y = this.DrawTarget.Height / 2;
      this.innerBatch.Draw((Texture2D) this.BackTarget, destinationRectangle, new Rectangle?(), new Color(250, 250, 250), 0.0f, origin, SpriteEffects.None, 0.5f);
      if (flashBackground)
        GridEffect.DrawGridBackground(rectangle, this.innerBatch, 4, Utils.AddativeWhite);
      for (int index = 0; index < spiralElements; ++index)
        this.innerBatch.Draw(this.UsedSprite, Utils.PolarToCartesian((float) ((double) index / (double) spiralElements * 6.28318548202515) + this.rotation, spiralRadius) + new Vector2((float) (this.DrawTarget.Width / 2), (float) (this.DrawTarget.Height / 2)), new Rectangle?(), flashBackground ? Utils.AddativeWhite : color, this.rotation, this.UsedSprite.GetCentreOrigin(), Vector2.One * 0.1f, SpriteEffects.None, 0.4f);
      if (drawFlashFromMiddle)
      {
        int num1 = 4;
        float num2 = (float) dest.Width - 50f;
        for (int index = 0; index < num1; ++index)
          this.innerBatch.Draw(this.FlashSprite, origin - new Vector2(3f), new Rectangle?(), Color.Lerp(color, Utils.AddativeWhite, 0.5f) * 0.4f, 0.0f, this.FlashSprite.GetCentreOrigin(), Vector2.One * (2.5f + Utils.randm(3f)), SpriteEffects.None, 0.8f);
      }
      this.innerBatch.End();
      sb.GraphicsDevice.SetRenderTarget(currentRenderTarget);
      sb.Draw((Texture2D) this.DrawTarget, dest, Color.White);
      RenderTarget2D backTarget = this.BackTarget;
      this.BackTarget = this.DrawTarget;
      this.DrawTarget = backTarget;
    }

    public void Draw(float dt, Rectangle dest, SpriteBatch sb, Action<SpriteBatch, Rectangle> render)
    {
      this.elapsedTime += dt;
      RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
      sb.GraphicsDevice.SetRenderTarget(this.DrawTarget);
      sb.GraphicsDevice.Clear(Color.Transparent);
      Rectangle rect = new Rectangle(0, 0, this.DrawTarget.Width, this.DrawTarget.Height);
      Rectangle destinationRectangle = Utils.InsetRectangle(rect, 2);
      this.rotation += dt * 3f;
      this.offset = new Vector2((float) (Math.Cos((double) this.elapsedTime * 2.0) * 40.0), -1f * (float) (Math.Cos((double) this.elapsedTime * 8.0) * 40.0));
      Vector2 origin = new Vector2((float) (this.DrawTarget.Width / 2), (float) (this.DrawTarget.Height / 2));
      this.innerBatch.Begin();
      destinationRectangle.X = this.DrawTarget.Width / 2;
      destinationRectangle.Y = this.DrawTarget.Height / 2;
      this.innerBatch.Draw((Texture2D) this.BackTarget, destinationRectangle, new Rectangle?(), new Color(250, 250, 250), 0.0f, origin, SpriteEffects.None, 0.5f);
      if (render != null)
        render(this.innerBatch, rect);
      this.innerBatch.End();
      sb.GraphicsDevice.SetRenderTarget(currentRenderTarget);
      sb.Draw((Texture2D) this.DrawTarget, dest, Color.White);
      RenderTarget2D backTarget = this.BackTarget;
      this.BackTarget = this.DrawTarget;
      this.DrawTarget = backTarget;
    }

    public void Dispose()
    {
      if (FlyoutEffect.PooledTarget == null)
      {
        FlyoutEffect.PooledTarget = this.DrawTarget;
        FlyoutEffect.PooledBackTarget = this.BackTarget;
      }
      else
      {
        this.DrawTarget.Dispose();
        this.BackTarget.Dispose();
      }
      this.DrawTarget = (RenderTarget2D) null;
    }
  }
}
