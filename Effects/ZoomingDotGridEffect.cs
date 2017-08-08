// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.ZoomingDotGridEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.Effects
{
  public static class ZoomingDotGridEffect
  {
    private static Texture2D CircleTex;

    public static void Render(Rectangle dest, SpriteBatch sb, float timer, Color themeColor)
    {
      if (ZoomingDotGridEffect.CircleTex == null)
        ZoomingDotGridEffect.CircleTex = OS.currentInstance.content.Load<Texture2D>("Circle");
      float num1 = 12f;
      float num2 = 4f;
      float num3 = 4f;
      float num4 = 3.5f * num3;
      float num5 = (float) Math.Sqrt(Math.Pow((double) num4, 2.0) * 2.0);
      float num6 = timer % num1;
      float num7 = num4 + (float) (((double) num5 - (double) num4) * ((double) num6 / (double) num1));
      float num8 = 0.7853982f;
      float rotation = num6 / num1 * num8;
      Vector2 origin = new Vector2((float) dest.X, (float) dest.Y);
      float num9 = (num6 - (num1 - num2)) / num2;
      if ((double) num1 - (double) num6 > (double) num2)
        num9 = 0.0f;
      Vector2 vector2_1 = num6 / num1 * new Vector2((float) (-1.0 * (double) num7 / 2.5), (float) (-1.0 * (double) num7 / 3.0));
      Vector2 zero = Vector2.Zero;
      Vector2 point = new Vector2((float) dest.X - num7 * 0.0f, (float) dest.Y - num7 * 10f);
      float x1 = point.X;
      Rectangle targetBounds;
      while ((double) point.Y < (double) (dest.Y + dest.Height) + (double) num5 + (double) num3)
      {
        point.X = x1;
        while ((double) point.X < (double) (dest.X + dest.Width * 2) + (double) num5 + (double) num3)
        {
          Vector2 vector2_2 = ZoomingDotGridEffect.rotatePointAroundOrigin(point, origin, rotation) + zero;
          targetBounds = new Rectangle((int) ((double) vector2_2.X - (double) num3 / 2.0), (int) ((double) vector2_2.Y - (double) num3 / 2.0), (int) num3, (int) num3);
          Utils.RenderSpriteIntoClippedRectDest(dest, targetBounds, ZoomingDotGridEffect.CircleTex, themeColor, sb);
          point.X += num7;
        }
        point.Y += num7;
      }
      if ((double) num1 - (double) num6 > (double) num2)
        return;
      point = new Vector2((float) dest.X - num7 / 2f, (float) dest.Y - num7 * 10.5f);
      float x2 = point.X;
      while ((double) point.Y < (double) (dest.Y + dest.Height) + (double) num5 + (double) num3)
      {
        point.X = x2;
        while ((double) point.X < (double) (dest.X + dest.Width * 2) + (double) num5 + (double) num3)
        {
          Vector2 vector2_2 = ZoomingDotGridEffect.rotatePointAroundOrigin(point, origin, rotation) + zero;
          targetBounds = new Rectangle((int) ((double) vector2_2.X - (double) num3 / 2.0), (int) ((double) vector2_2.Y - (double) num3 / 2.0), (int) num3, (int) num3);
          Utils.RenderSpriteIntoClippedRectDest(dest, targetBounds, ZoomingDotGridEffect.CircleTex, themeColor * num9, sb);
          point.X += num7;
        }
        point.Y += num7;
      }
    }

    private static Vector2 rotatePointAroundOrigin(Vector2 point, Vector2 origin, float rotation)
    {
      return Utils.RotatePoint(point - origin, rotation) + origin;
    }
  }
}
