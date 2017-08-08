// Decompiled with JetBrains decompiler
// Type: Hacknet.Gui.TextItem
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.Gui
{
  public static class TextItem
  {
    public static bool DrawShadow = false;

    public static Vector2 doMeasuredLabel(Vector2 pos, string text, Color? color)
    {
      if (!color.HasValue)
        color = new Color?(Color.White);
      Vector2 vector2 = GuiData.font.MeasureString(text);
      GuiData.spriteBatch.DrawString(GuiData.font, text, pos + Vector2.One, Color.Gray);
      GuiData.spriteBatch.DrawString(GuiData.font, text, pos, color.Value);
      return vector2;
    }

    public static void doLabel(Vector2 pos, string text, Color? color)
    {
      if (!color.HasValue)
        color = new Color?(Color.White);
      GuiData.spriteBatch.DrawString(GuiData.font, text, pos, color.Value);
    }

    public static void doLabel(Vector2 pos, string text, Color? color, float MaxWidth)
    {
      if (!color.HasValue)
        color = new Color?(Color.White);
      TextItem.doFontLabel(pos, text, GuiData.font, color, MaxWidth, float.MaxValue, false);
    }

    public static void doFontLabelToSize(Rectangle dest, string text, SpriteFont font, Color color, bool doNotOversize = false, bool offsetToTopLeft = false)
    {
      Vector2 vector2 = font.MeasureString(text);
      Vector2 scale = new Vector2((float) dest.Width / vector2.X, (float) dest.Height / vector2.Y);
      float num = Math.Min(scale.X, scale.Y);
      Vector2 zero = Vector2.Zero;
      if ((double) scale.X > (double) num)
      {
        zero.X = (float) ((double) vector2.X * ((double) scale.X - (double) num) / 2.0);
        scale.X = num;
      }
      if ((double) scale.Y > (double) num)
      {
        zero.Y = (float) ((double) vector2.Y * ((double) scale.Y - (double) num) / 2.0);
        scale.Y = num;
      }
      if (doNotOversize)
      {
        scale.X = Math.Min(scale.X, 1f);
        scale.Y = Math.Min(scale.Y, 1f);
      }
      Vector2 input = new Vector2((float) dest.X, (float) dest.Y);
      if (!offsetToTopLeft)
        input += zero;
      Vector2 position = Utils.ClipVec2ForTextRendering(input);
      GuiData.spriteBatch.DrawString(font, text, position, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.55f);
    }

    public static void doCenteredFontLabel(Rectangle dest, string text, SpriteFont font, Color color, bool LockToLeft = false)
    {
      Vector2 vector2_1 = font.MeasureString(text);
      Vector2 vector2_2 = new Vector2((float) dest.Width / vector2_1.X, (float) dest.Height / vector2_1.Y);
      if ((double) Math.Min(vector2_2.X, vector2_2.Y) < 1.0)
      {
        TextItem.doFontLabelToSize(dest, text, font, color, true, false);
      }
      else
      {
        Vector2 position = Utils.ClipVec2ForTextRendering(new Vector2((float) ((double) dest.X + (double) dest.Width / 2.0 - (double) vector2_1.X / 2.0), (float) ((double) dest.Y + (double) dest.Height / 2.0 - (double) vector2_1.Y / 2.0)));
        if (LockToLeft)
          position.X = (float) dest.X;
        GuiData.spriteBatch.DrawString(font, text, position, color);
      }
    }

    public static void doFontLabel(Vector2 pos, string text, SpriteFont font, Color? color, float widthTo = 3.402823E+38f, float heightTo = 3.402823E+38f, bool centreVertically = false)
    {
      if (!color.HasValue)
        color = new Color?(Color.White);
      Vector2 scale = font.MeasureString(text);
      float y = scale.Y;
      scale.X = (double) scale.X <= (double) widthTo ? 1f : widthTo / scale.X;
      scale.Y = (double) scale.Y <= (double) heightTo ? 1f : heightTo / scale.Y;
      scale.X = scale.Y = Math.Min(scale.X, scale.Y);
      if (TextItem.DrawShadow)
        GuiData.spriteBatch.DrawString(font, text, pos + Vector2.One, Color.Gray, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
      Vector2 vector2 = Vector2.Zero;
      if (centreVertically && (double) heightTo < 3.40282346638529E+38)
        vector2 = new Vector2(0.0f, (float) (((double) heightTo - (double) y * (double) scale.Y) / 2.0));
      Vector2 position = Utils.ClipVec2ForTextRendering(pos + vector2);
      GuiData.spriteBatch.DrawString(font, text, position, color.Value, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
    }

    public static void doRightAlignedBackingLabel(Rectangle dest, string msg, SpriteFont font, Color back, Color front)
    {
      GuiData.spriteBatch.Draw(Utils.white, dest, back);
      Vector2 vector2 = font.MeasureString(msg);
      Vector2 position = new Vector2((float) (dest.X + dest.Width - 4) - vector2.X, (float) (dest.Y + dest.Height - 2) - vector2.Y);
      GuiData.spriteBatch.DrawString(font, msg, position, front);
    }

    public static void doRightAlignedBackingLabelScaled(Rectangle dest, string msg, SpriteFont font, Color back, Color front)
    {
      GuiData.spriteBatch.Draw(Utils.white, dest, back);
      Vector2 vector2_1 = font.MeasureString(msg);
      Vector2[] stringScaleForSize = TextItem.GetStringScaleForSize(font, msg, dest);
      Vector2 scale = stringScaleForSize[0];
      Vector2 vector2_2 = stringScaleForSize[1];
      Vector2 vector2_3 = vector2_1 * scale;
      Vector2 position = new Vector2((float) (dest.X + dest.Width) - vector2_3.X, (float) (dest.Y + dest.Height / 2) - vector2_3.Y / 2f);
      GuiData.spriteBatch.DrawString(font, msg, position, front, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.6f);
    }

    public static void doRightAlignedBackingLabelFill(Rectangle dest, string msg, SpriteFont font, Color back, Color front)
    {
      GuiData.spriteBatch.Draw(Utils.white, dest, back);
      Vector2 vector2_1 = font.MeasureString(msg);
      Rectangle dest1 = dest;
      dest1.Width -= 7;
      dest1.X += 4;
      Vector2[] stringScaleForSize = TextItem.GetStringScaleForSize(font, msg, dest1);
      Vector2 scale = stringScaleForSize[0];
      Vector2 vector2_2 = stringScaleForSize[1];
      vector2_2.Y *= 2f;
      vector2_2.Y -= 3f;
      Vector2 vector2_3 = new Vector2((float) dest1.X, (float) dest.Y);
      GuiData.spriteBatch.DrawString(font, msg, vector2_3 + vector2_2, front, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.6f);
      Rectangle destinationRectangle = new Rectangle(dest.X, (int) ((double) dest.Y + (double) vector2_2.Y - 4.0 + (double) vector2_1.Y * (double) scale.Y), dest.Width, 1);
      GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, front);
    }

    private static Vector2[] GetStringScaleForSize(SpriteFont font, string text, Rectangle dest)
    {
      Vector2 vector2_1 = font.MeasureString(text);
      Vector2 vector2_2 = new Vector2((float) dest.Width / vector2_1.X, (float) dest.Height / vector2_1.Y);
      float num = Math.Min(vector2_2.X, vector2_2.Y);
      Vector2 zero = Vector2.Zero;
      if ((double) vector2_2.X > (double) num)
      {
        zero.X = (float) ((double) vector2_1.X * ((double) vector2_2.X - (double) num) / 2.0);
        vector2_2.X = num;
      }
      if ((double) vector2_2.Y > (double) num)
      {
        zero.Y = (float) ((double) vector2_1.Y * ((double) vector2_2.Y - (double) num) / 2.0);
        vector2_2.Y = num;
      }
      return new Vector2[2]{ vector2_2, zero };
    }

    public static Vector2 doMeasuredFontLabel(Vector2 pos, string text, SpriteFont font, Color? color, float widthTo = 3.402823E+38f, float heightTo = 3.402823E+38f)
    {
      if (!color.HasValue)
        color = new Color?(Color.White);
      Vector2 scale = font.MeasureString(text);
      scale.X = (double) scale.X <= (double) widthTo ? 1f : widthTo / scale.X;
      scale.Y = (double) scale.Y <= (double) heightTo ? 1f : heightTo / scale.Y;
      scale.X = scale.Y = Math.Min(scale.X, scale.Y);
      if (TextItem.DrawShadow)
        GuiData.spriteBatch.DrawString(font, text, pos + Vector2.One, Color.Gray, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
      GuiData.spriteBatch.DrawString(font, text, pos, color.Value, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
      return font.MeasureString(text) * scale;
    }

    public static void doSmallLabel(Vector2 pos, string text, Color? color)
    {
      if (!color.HasValue)
        color = new Color?(Color.White);
      if (TextItem.DrawShadow)
        GuiData.spriteBatch.DrawString(GuiData.smallfont, text, pos + Vector2.One, Color.Gray);
      GuiData.spriteBatch.DrawString(GuiData.smallfont, text, pos, color.Value);
    }

    public static void doTinyLabel(Vector2 pos, string text, Color? color)
    {
      if (!color.HasValue)
        color = new Color?(Color.White);
      GuiData.spriteBatch.DrawString(GuiData.tinyfont, text, pos, color.Value);
    }

    public static void doSmallLabel(Vector2 pos, string text, Color? color, float widthTo, float heightTo)
    {
      if (!color.HasValue)
        color = new Color?(Color.White);
      Vector2 scale = GuiData.font.MeasureString(text);
      scale.X = (double) scale.X <= (double) widthTo ? 1f : widthTo / scale.X;
      scale.Y = (double) scale.Y <= (double) heightTo ? 1f : heightTo / scale.Y;
      if (TextItem.DrawShadow)
        GuiData.spriteBatch.DrawString(GuiData.smallfont, text, pos + Vector2.One, Color.Gray, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
      GuiData.spriteBatch.DrawString(GuiData.smallfont, text, pos, color.Value, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
    }

    public static Vector2 doMeasuredSmallLabel(Vector2 pos, string text, Color? color)
    {
      if (!color.HasValue)
        color = new Color?(Color.White);
      Vector2 vector2 = GuiData.smallfont.MeasureString(text);
      if (TextItem.DrawShadow)
        GuiData.spriteBatch.DrawString(GuiData.smallfont, text, pos + Vector2.One, Color.Gray);
      GuiData.spriteBatch.DrawString(GuiData.smallfont, text, pos, color.Value);
      return vector2;
    }

    public static Vector2 doMeasuredTinyLabel(Vector2 pos, string text, Color? color)
    {
      if (!color.HasValue)
        color = new Color?(Color.White);
      Vector2 vector2 = GuiData.tinyfont.MeasureString(text);
      if (TextItem.DrawShadow)
        GuiData.spriteBatch.DrawString(GuiData.tinyfont, text, pos + Vector2.One, Color.Gray);
      GuiData.spriteBatch.DrawString(GuiData.tinyfont, text, pos, color.Value);
      return vector2;
    }
  }
}
