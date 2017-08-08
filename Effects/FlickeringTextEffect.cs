// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.FlickeringTextEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.Effects
{
  public static class FlickeringTextEffect
  {
    private static float internalTimer = 0.0f;
    private static RenderTarget2D LinedItemTarget = (RenderTarget2D) null;
    private static SpriteBatch LinedItemSB;

    public static void DrawFlickeringText(Rectangle dest, string text, float maxOffset, float rarity, SpriteFont font, object os_Obj, Color BaseCol)
    {
      OS os = (OS) null;
      if (os_Obj != null)
        os = (OS) os_Obj;
      else
        FlickeringTextEffect.internalTimer += 0.01666667f;
      Color color1 = new Color((int) BaseCol.R, 0, 0, 0);
      Color color2 = new Color(0, (int) BaseCol.G, 0, 0);
      Color color3 = new Color(0, 0, (int) BaseCol.B, 0);
      TextItem.doFontLabelToSize(FlickeringTextEffect.RectAddX(dest, (int) ((double) maxOffset * (double) FlickeringTextEffect.GetOffsetForSinTime(1.3f, 12.3f, rarity, (object) os))), text, font, color1, false, false);
      TextItem.doFontLabelToSize(FlickeringTextEffect.RectAddX(dest, (int) ((double) maxOffset * (double) FlickeringTextEffect.GetOffsetForSinTime(0.8f, 29f, rarity, (object) os))), text, font, color2, false, false);
      TextItem.doFontLabelToSize(FlickeringTextEffect.RectAddX(dest, (int) ((double) maxOffset * (double) FlickeringTextEffect.GetOffsetForSinTime(0.5f, -939.7f, rarity, (object) os))), text, font, color3, false, false);
    }

    public static void DrawLinedFlickeringText(Rectangle dest, string text, float maxOffset, float rarity, SpriteFont font, object os_Obj, Color BaseCol, int segmentHeight = 2)
    {
      GraphicsDevice graphicsDevice = GuiData.spriteBatch.GraphicsDevice;
      if (FlickeringTextEffect.LinedItemTarget == null)
      {
        FlickeringTextEffect.LinedItemTarget = new RenderTarget2D(graphicsDevice, 1920, 1080, false, SurfaceFormat.Rgba64, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        FlickeringTextEffect.LinedItemSB = new SpriteBatch(graphicsDevice);
      }
      if (dest.Width > FlickeringTextEffect.LinedItemTarget.Width || dest.Height > FlickeringTextEffect.LinedItemTarget.Height)
        throw new InvalidOperationException("Target area is too large for the supported rendertarget size!");
      OS os = (OS) null;
      if (os_Obj != null)
        os = (OS) os_Obj;
      Rectangle dest1 = new Rectangle(0, 0, dest.Width, dest.Height);
      RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
      graphicsDevice.SetRenderTarget(FlickeringTextEffect.LinedItemTarget);
      graphicsDevice.Clear(Color.Transparent);
      SpriteBatch spriteBatch = GuiData.spriteBatch;
      FlickeringTextEffect.LinedItemSB.Begin();
      GuiData.spriteBatch = FlickeringTextEffect.LinedItemSB;
      FlickeringTextEffect.DrawFlickeringText(dest1, text, maxOffset, rarity, font, os_Obj, BaseCol);
      FlickeringTextEffect.LinedItemSB.End();
      GuiData.spriteBatch = spriteBatch;
      graphicsDevice.SetRenderTarget(currentRenderTarget);
      int num1 = (int) ((double) dest.Height / 19.0);
      int num2 = (int) ((double) dest.Height / (double) (num1 + 1));
      float num3 = (float) dest.Height / (float) (num1 + 1);
      for (int index = 0; index < num1 + 1; ++index)
      {
        Rectangle rectangle1 = new Rectangle(0, (int) ((double) index * (double) num2 - (double) segmentHeight), dest.Width, num2 - segmentHeight);
        Rectangle destinationRectangle1 = new Rectangle(dest.X + dest1.X, dest.Y + rectangle1.Y, rectangle1.Width, rectangle1.Height);
        GuiData.spriteBatch.Draw((Texture2D) FlickeringTextEffect.LinedItemTarget, destinationRectangle1, new Rectangle?(rectangle1), Color.White);
        Rectangle rectangle2 = new Rectangle(0, rectangle1.Y + rectangle1.Height, rectangle1.Width, segmentHeight);
        Rectangle destinationRectangle2 = rectangle2;
        destinationRectangle2.X += dest.X;
        destinationRectangle2.Y += dest.Y;
        int num4 = (int) ((double) maxOffset * 1.20000004768372 * (double) FlickeringTextEffect.GetOffsetForSinTime((float) (1.29999995231628 + Math.Sin((double) index * 2.5)), 12.3f * (float) index, rarity, (object) os));
        destinationRectangle2.X += num4;
        GuiData.spriteBatch.Draw((Texture2D) FlickeringTextEffect.LinedItemTarget, destinationRectangle2, new Rectangle?(rectangle2), Color.White);
      }
    }

    public static void DrawFlickeringSprite(SpriteBatch sb, Rectangle dest, Texture2D texture, float maxOffset, float rarity, object os_Obj, Color BaseCol)
    {
      OS os = (OS) null;
      if (os_Obj != null)
        os = (OS) os_Obj;
      else
        FlickeringTextEffect.internalTimer += 0.01666667f;
      Color color1 = new Color((int) BaseCol.R, 0, 0, 0);
      Color color2 = new Color(0, (int) BaseCol.G, 0, 0);
      Color color3 = new Color(0, 0, (int) BaseCol.B, 0);
      sb.Draw(texture, FlickeringTextEffect.RectAddX(dest, (int) ((double) maxOffset * (double) FlickeringTextEffect.GetOffsetForSinTime(1.3f, 12.3f, rarity, (object) os))), color1);
      sb.Draw(texture, FlickeringTextEffect.RectAddX(dest, (int) ((double) maxOffset * (double) FlickeringTextEffect.GetOffsetForSinTime(0.8f, 29f, rarity, (object) os))), color2);
      sb.Draw(texture, FlickeringTextEffect.RectAddX(dest, (int) ((double) maxOffset * (double) FlickeringTextEffect.GetOffsetForSinTime(0.5f, -939.7f, rarity, (object) os))), color3);
    }

    public static void DrawFlickeringSpriteAltWeightings(SpriteBatch sb, Rectangle dest, Texture2D texture, float maxOffset, float rarity, object os_Obj, Color BaseCol)
    {
      OS os = (OS) null;
      if (os_Obj != null)
        os = (OS) os_Obj;
      else
        FlickeringTextEffect.internalTimer += 0.01666667f;
      Color color1 = new Color((int) BaseCol.R, 0, 0, 0);
      Color color2 = new Color(0, (int) BaseCol.G, 0, 0);
      Color color3 = new Color(0, 0, (int) BaseCol.B, 0);
      sb.Draw(texture, FlickeringTextEffect.RectAddX(dest, (int) ((double) maxOffset * (double) FlickeringTextEffect.GetOffsetForSinTime(4f, 0.0f, rarity, (object) os))), color1);
      sb.Draw(texture, FlickeringTextEffect.RectAddX(dest, (int) ((double) maxOffset * (double) FlickeringTextEffect.GetOffsetForSinTime(2f, 2f, rarity, (object) os))), color2);
      sb.Draw(texture, FlickeringTextEffect.RectAddX(dest, (int) ((double) maxOffset * (double) FlickeringTextEffect.GetOffsetForSinTime(0.5f, -4f, rarity, (object) os))), color3);
    }

    public static void DrawFlickeringSpriteFull(SpriteBatch sb, Vector2 pos, float rotation, Vector2 scale, Vector2 origin, Texture2D texture, float timerOffset, float maxOffset, float rarity, object os_Obj, Color BaseCol)
    {
      OS os = (OS) null;
      if (os_Obj != null)
        os = (OS) os_Obj;
      else
        FlickeringTextEffect.internalTimer += 0.01666667f;
      Color color1 = new Color((int) BaseCol.R, 0, 0, 0);
      Color color2 = new Color(0, (int) BaseCol.G, 0, 0);
      Color color3 = new Color(0, 0, (int) BaseCol.B, 0);
      sb.Draw(texture, FlickeringTextEffect.VecAddX(pos, maxOffset * FlickeringTextEffect.GetOffsetForSinTime(1.3f, 12.3f + timerOffset, rarity, (object) os)), new Rectangle?(), color1, rotation, origin, scale, SpriteEffects.None, 0.9f);
      sb.Draw(texture, FlickeringTextEffect.VecAddX(pos, maxOffset * FlickeringTextEffect.GetOffsetForSinTime(0.8f, 29f + timerOffset, rarity, (object) os)), new Rectangle?(), color2, rotation, origin, scale, SpriteEffects.None, 0.9f);
      sb.Draw(texture, FlickeringTextEffect.VecAddX(pos, maxOffset * FlickeringTextEffect.GetOffsetForSinTime(0.5f, timerOffset - 939.7f, rarity, (object) os)), new Rectangle?(), color3, rotation, origin, scale, SpriteEffects.None, 0.9f);
    }

    public static float GetOffsetForSinTime(float frequency, float offset, float rarity, object os_Obj)
    {
      float range = (float) Math.Sin(((os_Obj != null ? (double) ((OS) os_Obj).timer : (double) FlickeringTextEffect.internalTimer) + (double) offset) * (double) frequency) - rarity;
      if ((double) range < 0.0)
        range = 0.0f;
      return (float) (2.0 * ((double) (0.5f + Utils.QuadraticOutCurve(Utils.randm(range))) - 0.5)) - range;
    }

    private static Vector2 VecAddX(Vector2 vec, float x)
    {
      return vec + new Vector2(x, 0.0f);
    }

    private static Rectangle RectAddX(Rectangle rect, int x)
    {
      rect.X += x;
      return rect;
    }

    public static string GetReportString()
    {
      return "Target : " + (object) FlickeringTextEffect.LinedItemTarget + "\r\nTargetDisposed : " + (object) FlickeringTextEffect.LinedItemTarget.IsDisposed;
    }
  }
}
