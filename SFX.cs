// Decompiled with JetBrains decompiler
// Type: Hacknet.SFX
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  public static class SFX
  {
    private static List<Vector2> circlePos = new List<Vector2>();
    private static List<float> circleRadius = new List<float>();
    private static List<float> circleExpand = new List<float>();
    private static List<Color> circleColor = new List<Color>();
    private static List<SFX.RadialLineData> RadialLines = new List<SFX.RadialLineData>();
    private static Texture2D circleTex;
    private static Vector2 circleOrigin;

    public static void init(ContentManager content)
    {
      SFX.circleTex = content.Load<Texture2D>("CircleOutlineLarge");
      SFX.circleOrigin = new Vector2((float) (SFX.circleTex.Width / 2), (float) (SFX.circleTex.Height / 2));
    }

    public static void AddRadialLine(Vector2 pos, float incomingAngle, float startDistance, float startSpeed, float acceleration, float fadeDistance, float speedSizeMultiplier, Color color, float width = 1f, bool snapMode = false)
    {
      SFX.RadialLines.Add(new SFX.RadialLineData()
      {
        destination = pos,
        angle = incomingAngle,
        color = color,
        distance = startDistance,
        Acceleration = acceleration,
        FadeDistance = fadeDistance,
        SizeForSpeedMultiplier = speedSizeMultiplier,
        SnapMode = snapMode,
        width = width
      });
    }

    public static void addCircle(Vector2 pos, Color color, float radius)
    {
      SFX.circlePos.Add(pos);
      SFX.circleColor.Add(color);
      SFX.circleExpand.Add(0.0f);
      SFX.circleRadius.Add(radius / 256f);
    }

    public static void Update(float t)
    {
      for (int index1 = 0; index1 < SFX.circlePos.Count; ++index1)
      {
        List<float> circleExpand;
        int index2;
        (circleExpand = SFX.circleExpand)[index2 = index1] = circleExpand[index2] + t;
        if ((double) SFX.circleExpand[index1] >= 3.0)
        {
          SFX.circlePos.RemoveAt(index1);
          SFX.circleColor.RemoveAt(index1);
          SFX.circleExpand.RemoveAt(index1);
          SFX.circleRadius.RemoveAt(index1);
          --index1;
        }
      }
      for (int index = 0; index < SFX.RadialLines.Count; ++index)
      {
        SFX.RadialLineData radialLine = SFX.RadialLines[index];
        radialLine.distance -= radialLine.MovementPerSecond * t;
        radialLine.MovementPerSecond += radialLine.Acceleration * t;
        if ((double) radialLine.distance <= 0.0)
        {
          SFX.RadialLines.RemoveAt(index);
          --index;
        }
        else
          SFX.RadialLines[index] = radialLine;
      }
    }

    public static void Draw(SpriteBatch sb)
    {
      try
      {
        for (int index = 0; index < SFX.circlePos.Count; ++index)
        {
          float num1 = SFX.circleExpand[index] / 3f;
          float num2 = (double) num1 >= 0.200000002980232 ? 1f - num1 : num1 / 0.2f;
          sb.Draw(SFX.circleTex, SFX.circlePos[index], new Rectangle?(), SFX.circleColor[index] * num2, 0.0f, SFX.circleOrigin, Vector2.One * SFX.circleRadius[index] * SFX.circleExpand[index], SpriteEffects.None, 0.2f);
        }
        for (int index = 0; index < SFX.RadialLines.Count; ++index)
        {
          SFX.RadialLineData radialLine = SFX.RadialLines[index];
          float val1 = radialLine.MovementPerSecond * radialLine.SizeForSpeedMultiplier;
          Vector2 cartesian = Utils.PolarToCartesian(radialLine.angle, radialLine.distance);
          float magnitude;
          if (radialLine.SnapMode)
          {
            magnitude = Math.Min(val1, cartesian.Length());
          }
          else
          {
            magnitude = Math.Min(val1, radialLine.distance);
            if ((double) magnitude < 0.0)
              magnitude = 0.0f;
          }
          Vector2 vector2 = cartesian + Utils.PolarToCartesian(radialLine.angle, magnitude);
          float num = 1f;
          if ((double) radialLine.distance > (double) radialLine.FadeDistance)
            num = (float) (1.0 - ((double) radialLine.distance - (double) radialLine.FadeDistance) / (double) radialLine.FadeDistance);
          Utils.drawLineAlt(sb, radialLine.destination + cartesian, radialLine.destination + vector2, Vector2.Zero, radialLine.color * num, 0.5f, radialLine.width, Utils.gradientLeftRight);
        }
      }
      catch (IndexOutOfRangeException ex)
      {
      }
      catch (ArgumentOutOfRangeException ex)
      {
      }
    }

    private struct RadialLineData
    {
      public Vector2 destination;
      public float angle;
      public float distance;
      public Color color;
      public float width;
      public float MovementPerSecond;
      public float Acceleration;
      public float FadeDistance;
      public float SizeForSpeedMultiplier;
      public bool SnapMode;
    }
  }
}
