// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.RaindropsEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet.Effects
{
  public class RaindropsEffect
  {
    private List<Vector3> Drops = new List<Vector3>();
    private List<Vector3> Circles = new List<Vector3>();
    private List<Vector3> FadeoutLines = new List<Vector3>();
    public float FallRate = 1f;
    public float CircleExpandRate = 0.4f;
    public float LineFadeoutRate = 2f;
    public float MaxVerticalLandingVariane = 0.025f;
    private Texture2D Circle;
    private Texture2D Gradient;
    private Texture2D FlashImage;

    public void Init(ContentManager content)
    {
      this.Circle = content.Load<Texture2D>("CircleOutlineLarge");
      this.Gradient = content.Load<Texture2D>("Gradient");
      this.FlashImage = content.Load<Texture2D>("EffectFiles/ScanlinesTextBackground");
    }

    public void ForceSpawnDrop(Vector3 dropData)
    {
      this.Drops.Add(dropData);
    }

    public void Update(float dt, float dropsAddedPerSecond)
    {
      if ((double) Utils.randm(1f) < (double) dropsAddedPerSecond * (double) dt)
        this.Drops.Add(new Vector3(Utils.randm(1f), 0.0f, Utils.rand(this.MaxVerticalLandingVariane)));
      for (int index = 0; index < this.Drops.Count; ++index)
      {
        float num = (float) ((double) dt * (double) this.FallRate * (1.0 + (double) this.Drops[index].Z / 2.0));
        this.Drops[index] = new Vector3(this.Drops[index].X, this.Drops[index].Y + num, this.Drops[index].Z);
        if ((double) this.Drops[index].Y >= 1.0 + (double) this.Drops[index].Z)
        {
          float x = this.Drops[index].X;
          float z = this.Drops[index].Z;
          this.Drops.RemoveAt(index);
          this.Circles.Add(new Vector3(x, 0.0f, z));
          this.FadeoutLines.Add(new Vector3(x, 0.0f, z));
          --index;
        }
      }
      for (int index = 0; index < this.Circles.Count; ++index)
      {
        this.Circles[index] = new Vector3(this.Circles[index].X, this.Circles[index].Y + dt * this.CircleExpandRate, this.Circles[index].Z);
        if ((double) this.Circles[index].Y >= 1.0)
        {
          this.Circles.RemoveAt(index);
          --index;
        }
      }
      for (int index = 0; index < this.FadeoutLines.Count; ++index)
      {
        this.FadeoutLines[index] = new Vector3(this.FadeoutLines[index].X, this.FadeoutLines[index].Y + dt * this.LineFadeoutRate, this.FadeoutLines[index].Z);
        if ((double) this.FadeoutLines[index].Y >= 1.0)
        {
          this.FadeoutLines.RemoveAt(index);
          --index;
        }
      }
    }

    public void Render(Rectangle dest, SpriteBatch sb, Color DropColor, float maxCircleRadius, float maxFlashWidth)
    {
      int width = 3;
      int height = 4;
      float num1 = 0.3f;
      float num2 = 0.2f;
      Color color1 = Utils.makeColorAddative(DropColor);
      float num3 = (float) dest.Height * 0.84f;
      for (int index = 0; index < this.FadeoutLines.Count; ++index)
      {
        Vector3 fadeoutLine = this.FadeoutLines[index];
        Rectangle destinationRectangle = new Rectangle((int) ((double) dest.X + (double) fadeoutLine.X * (double) dest.Width - (double) width / 2.0), dest.Y, width, (int) ((double) num3 + (double) fadeoutLine.Z * (double) dest.Height));
        sb.Draw(Utils.white, destinationRectangle, DropColor * num2 * (1f - fadeoutLine.Y));
      }
      for (int index = 0; index < this.Drops.Count; ++index)
      {
        Vector3 drop = this.Drops[index];
        Rectangle destinationRectangle1 = new Rectangle((int) ((double) dest.X + (double) drop.X * (double) dest.Width - (double) width / 2.0), dest.Y, width, (int) ((double) drop.Y * ((double) num3 + (double) drop.Z * (double) dest.Height)));
        sb.Draw(Utils.white, destinationRectangle1, DropColor * num1);
        Rectangle destinationRectangle2 = new Rectangle(destinationRectangle1.X, destinationRectangle1.Y + destinationRectangle1.Height - height, destinationRectangle1.Width, height);
        sb.Draw(Utils.gradient, destinationRectangle2, color1);
        sb.Draw(Utils.gradient, destinationRectangle2, Utils.AddativeWhite * 0.5f);
      }
      Vector2 vector2_1 = new Vector2(4f, 1f);
      for (int index = 0; index < this.Circles.Count; ++index)
      {
        Vector3 circle = this.Circles[index];
        Vector2 origin = new Vector2((float) (this.Circle.Width / 2), (float) (this.Circle.Height / 2));
        Vector2 scale1 = vector2_1 * circle.Y * (maxCircleRadius / (float) this.Circle.Width);
        Vector2 vector2_2 = new Vector2((float) dest.X + circle.X * (float) dest.Width, (float) ((double) num3 + (double) dest.Y + (double) circle.Z * (double) dest.Height));
        Rectangle? sourceRectangle = new Rectangle?(Utils.getClipRectForSpritePos(dest, this.Circle, vector2_2, scale1, origin));
        Vector2 vector2_3 = new Vector2((float) sourceRectangle.Value.X * scale1.X, (float) sourceRectangle.Value.Y * scale1.Y);
        Color color2 = Color.Lerp(Utils.AddativeWhite, Color.Transparent, (float) Math.Min(1.0, (double) circle.Y * 2.0));
        float num4 = 1f;
        float num5 = 0.6f;
        if ((double) circle.Y >= (double) num5)
        {
          float num6 = (float) (1.0 / (1.0 - (double) num5));
          num4 = (float) (1.0 - ((double) circle.Y - (double) num5) * (double) num6);
        }
        float num7 = num4 * 0.2f;
        Color color3 = DropColor * num7;
        float num8 = 0.55f;
        if ((double) circle.Y < (double) num8)
        {
          float num6 = Utils.QuadraticOutCurve(circle.Y / num8);
          Vector2 vector2_4 = new Vector2(1f, 3f);
          Vector2 scale2 = new Vector2((float) this.FlashImage.Width * vector2_4.X, (float) this.FlashImage.Height * vector2_4.Y);
          scale2 = new Vector2(maxFlashWidth / scale2.X, maxFlashWidth / scale2.Y);
          scale2 *= (float) (0.300000011920929 + (double) num6 / 3.0 * 2.0);
          Vector2 centreOrigin = this.FlashImage.GetCentreOrigin();
          Rectangle rectForSpritePos = Utils.getClipRectForSpritePos(dest, this.FlashImage, vector2_2, scale2, centreOrigin);
          Vector2 vector2_5 = new Vector2((float) rectForSpritePos.X * scale2.X, (float) rectForSpritePos.Y * scale2.Y);
          sb.Draw(this.FlashImage, vector2_2 + vector2_5, new Rectangle?(rectForSpritePos), color1 * (1f - num6), 0.0f, centreOrigin, scale2, SpriteEffects.None, 0.5f);
          if ((double) num6 < 0.5)
            sb.Draw(this.FlashImage, vector2_2, new Rectangle?(), Utils.AddativeWhite * (float) (1.0 - (double) num6 * 2.0), 0.0f, centreOrigin, scale2 * 0.6f, SpriteEffects.None, 0.5f);
        }
        sb.Draw(this.Circle, vector2_2 + vector2_3, sourceRectangle, color3, 0.0f, origin, scale1, SpriteEffects.None, 0.5f);
        sb.Draw(this.Circle, vector2_2 + vector2_3, sourceRectangle, color2, 0.0f, origin, scale1, SpriteEffects.None, 0.5f);
      }
    }
  }
}
