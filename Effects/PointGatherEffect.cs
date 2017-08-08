// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.PointGatherEffect
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
  public class PointGatherEffect
  {
    public List<PointGatherEffect.StarPoint> Points = new List<PointGatherEffect.StarPoint>();
    public float timeRemainingWithoutAttract = 0.0f;
    public float MaxSpeed = 1.6f;
    public float Friction = 1f / 1000f;
    public float NoAttractPhaseFriction = 1.7f;
    public float GravityConstant = 1f / 1000f;
    public float absorbDistance = 0.005f;
    public float attractionToCentreMass = 1f;
    public float GlowScaleMod = 1f;
    public float LineLengthPercentage = 1f;
    public bool AllowDoubleLines = false;
    private float starSize = -1f;
    public Color NodeColor = Utils.AddativeWhite * 0.65f;
    public Texture2D CircleTex;
    public Texture2D ScanlinesBackground;
    public Texture2D Star;
    public Texture2D LineTexture;

    public void Init(ContentManager content)
    {
      this.CircleTex = content.Load<Texture2D>("Circle");
      this.ScanlinesBackground = content.Load<Texture2D>("EffectFiles/ScanlinesTextBackground");
      this.Star = content.Load<Texture2D>("EffectFiles/PointClicker/Star");
      this.LineTexture = Utils.white;
    }

    public void Explode(int entities)
    {
      for (int index = 0; index < entities; ++index)
      {
        float num = (double) Utils.randm(1f) < 0.300000011920929 ? 2f : 1f;
        this.Points.Add(new PointGatherEffect.StarPoint()
        {
          Pos = new Vector2(0.5f, 0.5f),
          Velocity = new Vector2((float) (-1.0 * (double) this.MaxSpeed + Utils.random.NextDouble() * (double) this.MaxSpeed * 2.0), (float) (-1.0 * (double) this.MaxSpeed + Utils.random.NextDouble() * (double) this.MaxSpeed * 2.0)),
          size = num,
          drawnSize = num,
          AttractedToOthers = (double) Utils.randm(1f) < 0.699999988079071
        });
      }
      this.timeRemainingWithoutAttract = 1f;
    }

    private PointGatherEffect.StarPoint ProcessAttractionBetweenPoints(PointGatherEffect.StarPoint pA, PointGatherEffect.StarPoint pB, float dt, bool constantForce = false)
    {
      Vector2 vector2_1 = constantForce ? pA.Pos + pA.Velocity * 0.4f : pA.Pos;
      float num1 = (float) Math.Pow((double) Vector2.Distance(pA.Pos, pB.Pos), 2.0);
      if (constantForce)
        num1 = 0.2f;
      float num2 = this.GravityConstant * (pA.size * pB.size) / num1 * (pA.size / pB.size);
      Vector2 vector2_2 = vector2_1 - pB.Pos;
      vector2_2.Normalize();
      Vector2 vector2_3 = vector2_2 * num2;
      pB.Velocity += vector2_3 * dt;
      return pB;
    }

    public void FlashComplete()
    {
      this.starSize = 6f;
    }

    public void Update(float dt)
    {
      float num = this.NoAttractPhaseFriction;
      this.timeRemainingWithoutAttract -= dt;
      if ((double) this.timeRemainingWithoutAttract <= 0.0)
      {
        if (this.Points.Count > 1)
          num = this.Friction;
        for (int index1 = 0; index1 < this.Points.Count; ++index1)
        {
          for (int index2 = 0; index2 < this.Points.Count; ++index2)
          {
            if (index1 != index2)
            {
              PointGatherEffect.StarPoint point1 = this.Points[index1];
              PointGatherEffect.StarPoint point2 = this.Points[index2];
              if (point2.AttractedToOthers)
                this.Points[index2] = this.ProcessAttractionBetweenPoints(point1, point2, dt, false);
            }
          }
        }
      }
      PointGatherEffect.StarPoint pA = new PointGatherEffect.StarPoint() { Pos = new Vector2(0.5f), size = this.attractionToCentreMass, Velocity = Vector2.Zero };
      for (int index = 0; index < this.Points.Count; ++index)
      {
        PointGatherEffect.StarPoint point = this.Points[index];
        if ((double) point.drawnSize < (double) point.size)
          point.drawnSize = Math.Min(point.size, point.drawnSize + dt * 15f);
        point.Pos += point.Velocity * dt;
        if ((double) point.Pos.X > 1.0 || (double) point.Pos.Y > 1.0 || (double) point.Pos.X < 0.0 || (double) point.Pos.Y < 0.0)
        {
          point.Pos.X = Math.Max(0.0f, Math.Min(point.Pos.X, 1f));
          point.Pos.Y = Math.Max(0.0f, Math.Min(point.Pos.Y, 1f));
          point.Velocity *= -0.5f;
        }
        point.Velocity -= point.Velocity * (num * dt);
        this.Points[index] = point;
        if (this.Points.Count > 1)
          this.Points[index] = this.ProcessAttractionBetweenPoints(pA, point, dt, true);
      }
      this.ResolveMergesThisFrame();
      if ((double) this.starSize <= 0.0)
        return;
      this.starSize -= dt * 3f;
    }

    private int ResolveMergesThisFrame()
    {
      if ((double) this.timeRemainingWithoutAttract <= 0.0)
      {
        for (int index1 = 0; index1 < this.Points.Count; ++index1)
        {
          for (int index2 = 0; index2 < this.Points.Count; ++index2)
          {
            if (index1 != index2)
            {
              PointGatherEffect.StarPoint point1 = this.Points[index1];
              PointGatherEffect.StarPoint point2 = this.Points[index2];
              if ((double) Vector2.Distance(point1.Pos, point2.Pos) < (double) this.absorbDistance * (1.0 + 0.200000002980232 * ((double) point1.size + (double) point2.size)))
              {
                float num1 = point1.size / point2.size;
                float num2 = (double) point1.drawnSize > (double) point2.drawnSize ? point1.drawnSize : point2.drawnSize;
                PointGatherEffect.StarPoint starPoint = new PointGatherEffect.StarPoint() { Pos = (double) point1.size > (double) point2.size ? point1.Pos : point2.Pos, Velocity = Vector2.Lerp(Vector2.Zero, point1.Velocity + point2.Velocity, 0.3f), size = point1.size + point2.size, drawnSize = num2 };
                this.Points.RemoveAt(index1);
                if (index2 > index1)
                  this.Points.RemoveAt(index2 - 1);
                else
                  this.Points.RemoveAt(index2);
                this.Points.Add(starPoint);
                return 1 + this.ResolveMergesThisFrame();
              }
            }
          }
        }
      }
      return 0;
    }

    public void Render(Rectangle dest, SpriteBatch sb)
    {
      Vector2 vector2_1 = new Vector2((float) dest.Width, (float) dest.Height);
      Vector2 vector2_2 = new Vector2((float) dest.X, (float) dest.Y);
      for (int index1 = 0; index1 < this.Points.Count; ++index1)
      {
        Vector2 vector2_3 = this.Points[index1].Pos * vector2_1 + vector2_2;
        float drawnSize = this.Points[index1].drawnSize;
        int num1 = this.AllowDoubleLines ? this.Points.Count : this.Points.Count / 2;
        for (int index2 = 0; index2 < num1; ++index2)
        {
          if (index2 != index1)
          {
            Vector2 vector2_4 = this.Points[index2].Pos * vector2_1 + vector2_2;
            if ((double) this.Points[index2].drawnSize + (double) drawnSize > 20.0)
              Utils.drawLineAlt(sb, vector2_3, vector2_4, Vector2.Zero, OS.currentInstance.highlightColor * 0.6f, 0.7f, this.LineLengthPercentage, this.LineTexture);
          }
        }
        float num2 = drawnSize / (float) this.CircleTex.Width;
        float num3 = Utils.SmoothStep(0.0f, 20f, this.Points[index1].drawnSize);
        float num4 = 0.5f - num3;
        float num5 = num3 * 0.5f;
        float num6 = num3 * this.GlowScaleMod;
        Vector2 scale = new Vector2(num5 * 1f * num6, num5 * 3f * num6);
        Vector2 origin1 = new Vector2((float) this.ScanlinesBackground.Width / 2f, (float) this.ScanlinesBackground.Height / 2f);
        Vector2 origin2 = new Vector2((float) this.CircleTex.Width / 2f);
        Rectangle rectForSpritePos = Utils.getClipRectForSpritePos(dest, this.ScanlinesBackground, vector2_3 - origin1 * scale, scale);
        sb.Draw(this.ScanlinesBackground, vector2_3 + new Vector2((float) rectForSpritePos.X * scale.X, (float) rectForSpritePos.Y * scale.Y), new Rectangle?(rectForSpritePos), Utils.AddativeWhite * 0.05f, 0.0f, origin1, scale, SpriteEffects.None, 0.7f);
        if ((double) ((float) this.CircleTex.Width * num2) > (double) dest.Height)
          num2 = (float) dest.Height / (float) this.CircleTex.Width;
        sb.Draw(this.CircleTex, vector2_3, new Rectangle?(), this.NodeColor, 0.0f, origin2, new Vector2(num2), SpriteEffects.None, 0.7f);
        if ((double) this.starSize > 0.0 && (double) this.Points[index1].drawnSize > 30.0)
        {
          Color color1 = Color.Lerp(Utils.makeColorAddative(OS.currentInstance.highlightColor), OS.currentInstance.highlightColor, 0.5f);
          sb.Draw(this.Star, vector2_3, new Rectangle?(), color1 * (this.starSize / 4f), OS.currentInstance.timer * 0.0f, new Vector2((float) (this.Star.Width / 2), (float) (this.Star.Height / 2)), new Vector2(this.starSize * 3.2f, this.starSize * 1.2f), SpriteEffects.None, 0.8f);
          Color color2 = Utils.makeColorAddative(OS.currentInstance.highlightColor);
          sb.Draw(this.Star, vector2_3, new Rectangle?(), color2 * (this.starSize / 4f), OS.currentInstance.timer * 0.0f, new Vector2((float) (this.Star.Width / 2), (float) (this.Star.Height / 2)), new Vector2(this.starSize * 2.7f, this.starSize * 1f), SpriteEffects.None, 0.8f);
        }
      }
    }

    public struct StarPoint
    {
      public Vector2 Pos;
      public Vector2 Velocity;
      public float size;
      public bool AttractedToOthers;
      public float drawnSize;
    }
  }
}
