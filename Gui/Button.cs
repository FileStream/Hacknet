// Decompiled with JetBrains decompiler
// Type: Hacknet.Gui.Button
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Hacknet.Gui
{
  public static class Button
  {
    public static bool wasPressedDown = false;
    public static bool wasReleased = false;
    public static bool drawingOutline = true;
    public static bool outlineOnly = false;
    public static bool smallButtonDraw = false;
    public static bool DisableIfAnotherIsActive = false;
    public static bool ForceNoColorTag = false;
    public const int BORDER_WIDTH = 1;

    public static bool doButton(int myID, int x, int y, int width, int height, string text, Color? selectedColor)
    {
      return Button.doButton(myID, x, y, width, height, text, selectedColor, Utils.white);
    }

    public static bool doButton(int myID, int x, int y, int width, int height, string text, Color? selectedColor, Texture2D tex)
    {
      bool flag = false;
      if (GuiData.hot == myID && !GuiData.blockingInput && GuiData.active == myID && (GuiData.mouseLeftUp() || GuiData.mouse.LeftButton == ButtonState.Released))
      {
        flag = true;
        GuiData.active = -1;
      }
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = x;
      tmpRect.Y = y;
      tmpRect.Width = width;
      tmpRect.Height = height;
      if (tmpRect.Contains(GuiData.getMousePoint()) && !GuiData.blockingInput)
      {
        GuiData.hot = myID;
        if (GuiData.isMouseLeftDown() && (!Button.DisableIfAnotherIsActive || GuiData.active == -1))
          GuiData.active = myID;
      }
      else
      {
        if (GuiData.hot == myID)
          GuiData.hot = -1;
        if (GuiData.isMouseLeftDown() && GuiData.active == myID && GuiData.active == myID)
          GuiData.active = -1;
      }
      Button.drawModernButton(myID, x, y, width, height, text, selectedColor, tex);
      return flag;
    }

    private static void drawButton(int myID, int x, int y, int width, int height, string text, Color? selectedColor, Texture2D tex)
    {
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = x;
      tmpRect.Y = y;
      tmpRect.Width = width;
      tmpRect.Height = height;
      --tmpRect.X;
      tmpRect.Width += 2;
      --tmpRect.Y;
      tmpRect.Height += 2;
      if (Button.outlineOnly)
        RenderedRectangle.doRectangleOutline(tmpRect.X, tmpRect.Y, tmpRect.Width, tmpRect.Height, 1, new Color?(GuiData.hot == myID ? (GuiData.active == myID ? GuiData.Default_Selected_Color : GuiData.Default_Lit_Backing_Color) : GuiData.Default_Backing_Color));
      else if (Button.drawingOutline)
        GuiData.spriteBatch.Draw(tex, tmpRect, GuiData.hot == myID ? GuiData.Default_Lit_Backing_Color : GuiData.Default_Backing_Color);
      ++tmpRect.X;
      tmpRect.Width -= 2;
      ++tmpRect.Y;
      tmpRect.Height -= 2;
      if (!Button.outlineOnly)
        GuiData.spriteBatch.Draw(tex, tmpRect, GuiData.active == myID ? GuiData.Default_Unselected_Color : selectedColor.Value);
      Vector2 scale = GuiData.tinyfont.MeasureString(text);
      scale.X = (double) scale.X <= (double) (width - 4) ? 1f : (float) (width - 4) / scale.X;
      scale.Y = (double) scale.Y <= (double) (height - 4) ? 1f : (float) (height - 2) / scale.Y;
      scale.X = Math.Min(scale.X, scale.Y);
      scale.Y = scale.X;
      GuiData.spriteBatch.DrawString(GuiData.tinyfont, text, new Vector2((float) (x + 2 + 1), (float) (y + 1 + 1)), Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
      GuiData.spriteBatch.DrawString(GuiData.tinyfont, text, new Vector2((float) (x + 2), (float) (y + 1)), Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
    }

    private static void drawModernButton(int myID, int x, int y, int width, int height, string text, Color? selectedColor, Texture2D tex)
    {
      int num1 = Button.ForceNoColorTag || width <= 65 ? 0 : 13;
      if (!selectedColor.HasValue)
        selectedColor = new Color?(GuiData.Default_Trans_Grey_Strong);
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = x;
      tmpRect.Y = y;
      tmpRect.Width = width;
      tmpRect.Height = height;
      if (tex.Equals((object) Utils.white))
      {
        if (!Button.outlineOnly)
        {
          GuiData.spriteBatch.Draw(Utils.white, tmpRect, GuiData.hot == myID ? (GuiData.active == myID ? GuiData.Default_Trans_Grey_Dark : GuiData.Default_Trans_Grey_Bright) : GuiData.Default_Trans_Grey);
          tmpRect.Width = num1;
          GuiData.spriteBatch.Draw(Utils.white, tmpRect, selectedColor.Value);
        }
        RenderedRectangle.doRectangleOutline(x, y, width, height, 1, Button.outlineOnly ? selectedColor : new Color?(GuiData.Default_Trans_Grey_Solid));
      }
      else
        GuiData.spriteBatch.Draw(tex, tmpRect, GuiData.hot == myID ? (GuiData.active == myID ? GuiData.Default_Unselected_Color : GuiData.Default_Lit_Backing_Color) : selectedColor.Value);
      SpriteFont spriteFont = Button.smallButtonDraw ? GuiData.detailfont : GuiData.tinyfont;
      Vector2 scale = spriteFont.MeasureString(text);
      float num2 = LocaleActivator.ActiveLocaleIsCJK() ? 4f : 0.0f;
      float y1 = scale.Y;
      scale.X = (double) scale.X <= (double) (width - 4) ? 1f : (float) (width - (4 + num1 + 5)) / scale.X;
      scale.Y = (double) scale.Y <= (double) height + (double) num2 - 0.0 ? 1f : (float) ((double) height + (double) num2 - 0.0) / scale.Y;
      scale.X = Math.Min(scale.X, scale.Y);
      scale.Y = scale.X;
      if (Utils.FloatEquals(1f, scale.Y))
        scale = Vector2.One;
      int num3 = num1 + 4;
      float num4 = y1 * scale.Y;
      float num5 = (float) ((double) y + (double) height / 2.0 - (double) num4 / 2.0 + 1.0 - (double) num2 * (double) scale.Y / 2.0);
      GuiData.spriteBatch.DrawString(spriteFont, text, new Vector2((float) (int) ((double) (x + 2 + num3) + 0.5), (float) (int) ((double) num5 + 0.5)), Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
    }

    public static bool doHoldDownButton(int myID, int x, int y, int width, int height, string text, bool hasOutline, Color? outlineColor, Color? selectedColor)
    {
      Button.wasPressedDown = false;
      Button.wasReleased = false;
      if (!outlineColor.HasValue)
        outlineColor = new Color?(Color.White);
      if (!selectedColor.HasValue)
        selectedColor = new Color?(GuiData.Default_Selected_Color);
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = x;
      tmpRect.Y = y;
      tmpRect.Width = width;
      tmpRect.Height = height;
      bool flag = GuiData.isMouseLeftDown() && GuiData.active == myID;
      if (tmpRect.Contains(GuiData.getMousePoint()))
      {
        GuiData.hot = myID;
        if (GuiData.mouseWasPressed())
          Button.wasPressedDown = true;
        if (GuiData.isMouseLeftDown())
        {
          GuiData.active = myID;
          flag = true;
        }
      }
      else
      {
        if (GuiData.hot == myID)
          GuiData.hot = -1;
        if (!GuiData.isMouseLeftDown() && GuiData.active == myID && GuiData.active == myID)
          GuiData.active = -1;
      }
      if (GuiData.mouseLeftUp())
        Button.wasReleased = true;
      GuiData.spriteBatch.Draw(Utils.white, tmpRect, GuiData.active == myID ? selectedColor.Value : (GuiData.hot == myID ? GuiData.Default_Lit_Backing_Color : GuiData.Default_Light_Backing_Color));
      if (hasOutline)
        RenderedRectangle.doRectangleOutline(x, y, width, height, 2, outlineColor);
      return flag;
    }
  }
}
