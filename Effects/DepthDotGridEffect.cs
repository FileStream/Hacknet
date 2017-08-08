// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.DepthDotGridEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.Effects
{
  public class DepthDotGridEffect
  {
    private Texture2D tex;

    public DepthDotGridEffect(ContentManager content)
    {
      this.tex = content.Load<Texture2D>("Circle");
    }

    public void DrawGrid(Rectangle fullAreaDest, Vector2 xyOffset, SpriteBatch sb, float pixelsInOnRecurse, int recursionSteps, Color dotColor, float dotSeperation, float dotSize, float MaxDepthEffectDistance, float timer, float chaosPercent)
    {
      if (recursionSteps > 0 && (double) dotSeperation > (double) dotSize && (double) dotSize > 0.100000001490116)
        this.DrawGrid(fullAreaDest, xyOffset, sb, pixelsInOnRecurse, recursionSteps - 1, dotColor * 0.8f, dotSeperation, dotSize * 0.6f, MaxDepthEffectDistance + pixelsInOnRecurse, timer, chaosPercent);
      Vector2 vector2_1 = new Vector2((float) fullAreaDest.X * xyOffset.X, (float) fullAreaDest.Y + xyOffset.Y);
      Vector2 vector2_2 = vector2_1;
      float num1 = timer + (float) recursionSteps * 0.12f;
      float num2 = 2.5f * (float) recursionSteps;
      Vector2 vector2_3 = new Vector2((float) Math.Sin((double) num1) * num2, 0.0f);
      Vector2 vector2_4 = vector2_3;
      float num3 = num1 % 6f;
      if ((double) num3 <= 2.0)
      {
        vector2_3.X = Utils.QuadraticOutCurve(num3 / 2f) * dotSeperation;
        vector2_3.Y = 0.0f;
      }
      else if ((double) num3 <= 5.0 && (double) num3 >= 3.0)
      {
        vector2_3.Y = Utils.QuadraticOutCurve((float) (((double) num3 - 3.0) / 2.0)) * dotSeperation;
        vector2_3.X = 0.0f;
      }
      else
      {
        vector2_3.X = 0.0f;
        vector2_3.Y = 0.0f;
      }
      vector2_3 += chaosPercent * vector2_4;
      vector2_1 -= new Vector2(dotSeperation * 1f);
      int num4 = 0;
      int num5 = 0;
      while ((double) vector2_1.Y < (double) (fullAreaDest.Y + fullAreaDest.Height))
      {
        while ((double) vector2_1.X - (double) dotSize - (double) MaxDepthEffectDistance < (double) (fullAreaDest.X + fullAreaDest.Width))
        {
          Vector2 vector2_5 = new Vector2(0.0f, (num5 % 2 == 0 ? 1f : -1f) * vector2_3.Y);
          Vector2 vector2_6 = vector2_1 + vector2_5;
          float num6 = (float) fullAreaDest.Width / 2f;
          float num7 = (vector2_6.X - (float) fullAreaDest.X - num6) / num6;
          float num8 = (float) fullAreaDest.Height / 2f;
          float num9 = (vector2_6.Y - (float) fullAreaDest.Y - num8) / num8;
          num7 *= 0.5f;
          num9 *= 0.5f;
          Vector2 vector2_7 = new Vector2(-1f * num7 * MaxDepthEffectDistance, -1f * num9 * MaxDepthEffectDistance);
          Vector2 position = vector2_6 + vector2_7;
          Utils.RenderSpriteIntoClippedRectDest(fullAreaDest, new Rectangle((int) ((double) position.X - (double) dotSize / 2.0), (int) ((double) position.Y - (double) dotSize / 2.0), (int) dotSize, (int) dotSize), this.tex, dotColor, sb);
          if ((Utils.random.NextDouble() < 1E-05 || Utils.random.NextDouble() < 3E-05) && fullAreaDest.Contains((int) position.X, (int) position.Y))
            sb.DrawString(GuiData.UITinyfont, num7.ToString(".0") + "/" + num9.ToString(".0"), position, Color.Red);
          vector2_1.X += dotSeperation;
          ++num5;
        }
        vector2_1.Y += dotSeperation;
        ++num4;
        vector2_1.X = vector2_2.X + (num4 % 2 == 0 ? 1f : -1f) * vector2_3.X;
        num5 = 0;
      }
    }
  }
}
