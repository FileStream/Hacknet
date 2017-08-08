// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.TunnelingCircleEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.Effects
{
  public class TunnelingCircleEffect
  {
    private static Texture2D CircleTex;

    public static void Draw(SpriteBatch sb, Vector2 circleCentre, float diamater, float nodeDiamater, int subdivisions, float timer, Color nodeColor, Color lineColor, Color ZeroNodeColor, Rectangle clipBounds)
    {
      if (TunnelingCircleEffect.CircleTex == null)
        TunnelingCircleEffect.CircleTex = Game1.getSingleton().Content.Load<Texture2D>("Circle");
      float magnitude = diamater / 2f;
      for (int index = 0; index < subdivisions; ++index)
      {
        float num1 = 3.141593f / (float) subdivisions;
        float angle = (float) index * num1;
        Vector2 left = circleCentre - Utils.PolarToCartesian(angle, magnitude);
        Vector2 right = circleCentre + Utils.PolarToCartesian(angle, magnitude);
        Vector2 leftOut;
        Vector2 rightOut;
        Utils.ClipLineSegmentsForRect(clipBounds, left, right, out leftOut, out rightOut);
        if ((double) Vector2.Distance(leftOut, rightOut) > 1.0 / 1000.0)
          Utils.drawLine(sb, leftOut, rightOut, Vector2.Zero, lineColor, 0.2f);
        float num2 = (float) Math.Sin((double) timer + (double) angle);
        Vector2 vector2 = circleCentre + Utils.PolarToCartesian(angle, magnitude * num2) - new Vector2(nodeDiamater / 2f);
        Vector2 scale = new Vector2(nodeDiamater / (float) TunnelingCircleEffect.CircleTex.Width);
        Vector2 origin = new Vector2(nodeDiamater / 2f * scale.X);
        Rectangle rectForSpritePos = Utils.getClipRectForSpritePos(clipBounds, TunnelingCircleEffect.CircleTex, vector2, scale.X);
        if (rectForSpritePos.Height > 0 && rectForSpritePos.Width > 0)
          sb.Draw(TunnelingCircleEffect.CircleTex, vector2, new Rectangle?(rectForSpritePos), index == 0 ? ZeroNodeColor : nodeColor, 0.0f, origin, scale, SpriteEffects.None, 0.25f);
      }
    }
  }
}
