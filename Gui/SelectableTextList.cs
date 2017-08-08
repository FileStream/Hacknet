// Decompiled with JetBrains decompiler
// Type: Hacknet.Gui.SelectableTextList
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.Gui
{
  public static class SelectableTextList
  {
    public static int scrollOffset = 0;
    public static bool wasActivated = false;
    public static bool selectionWasChanged = false;
    private static Color scrollBarColor = new Color(140, 140, 140, 80);
    public const int BORDER_WIDTH = 2;
    public const float ITEM_HEIGHT = 18f;

    public static int doList(int myID, int x, int y, int width, int height, string[] text, int lastSelectedIndex, Color? selectedColor)
    {
      if (!selectedColor.HasValue)
        selectedColor = new Color?(GuiData.Default_Selected_Color);
      int num1 = -1;
      SelectableTextList.wasActivated = false;
      int num2 = lastSelectedIndex;
      SelectableTextList.selectionWasChanged = false;
      Vector2 mousePos = GuiData.getMousePos();
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = x;
      tmpRect.Y = y;
      tmpRect.Width = width;
      tmpRect.Height = height;
      if (tmpRect.Contains(GuiData.getMousePoint()))
      {
        GuiData.hot = myID;
        SelectableTextList.scrollOffset += (int) GuiData.getMouseWheelScroll();
        SelectableTextList.scrollOffset = Math.Max(0, Math.Min(SelectableTextList.scrollOffset, text.Length - (int) ((double) height / 18.0)));
      }
      else if (GuiData.hot == myID)
        GuiData.hot = -1;
      int num3 = Math.Max(0, Math.Min(SelectableTextList.scrollOffset, text.Length - (int) ((double) height / 18.0)));
      if (GuiData.hot == myID)
      {
        for (int index = 0; index < text.Length; ++index)
        {
          if ((double) mousePos.Y >= (double) y + (double) index * 18.0 && (double) mousePos.Y <= (double) y + (double) (index + 1) * 18.0 && (double) mousePos.Y < (double) (y + height))
          {
            num1 = index + num3;
            SelectableTextList.wasActivated = true;
          }
        }
      }
      if (num1 != -1 && num1 != lastSelectedIndex && GuiData.mouseLeftUp())
        lastSelectedIndex = num1;
      GuiData.spriteBatch.Draw(Utils.white, tmpRect, GuiData.hot == myID ? GuiData.Default_Lit_Backing_Color : GuiData.Default_Backing_Color);
      tmpRect.X += 2;
      tmpRect.Width -= 4;
      tmpRect.Y += 2;
      tmpRect.Height -= 4;
      GuiData.spriteBatch.Draw(Utils.white, tmpRect, GuiData.Default_Dark_Background_Color);
      Vector2 position = new Vector2((float) tmpRect.X, (float) tmpRect.Y);
      tmpRect.Height = 18;
      for (int index = num3; index < text.Length; ++index)
      {
        GuiData.spriteBatch.Draw(Utils.white, tmpRect, lastSelectedIndex == index ? selectedColor.Value : (num1 == index ? GuiData.Default_Unselected_Color : GuiData.Default_Dark_Neutral_Color));
        Vector2 scale = GuiData.UITinyfont.MeasureString(text[index]);
        scale.X = (double) scale.X <= (double) (width - 4) ? 1f : (float) (width - 4) / scale.X;
        scale.Y = (double) scale.Y <= 18.0 ? 1f : 14f / scale.Y;
        GuiData.spriteBatch.DrawString(GuiData.UITinyfont, text[index], position, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
        position.Y += 18f;
        tmpRect.Y += 18;
        if ((double) position.Y > (double) (y + height - 4))
          break;
      }
      if ((double) text.Length * 18.0 > (double) height)
      {
        float num4 = 2f;
        float num5 = (float) height / ((float) text.Length * 18f) * (float) height;
        height -= 4;
        float num6 = (float) -height + (float) (((double) height - (double) num5) * ((double) num3 / (((double) text.Length * 18.0 - (double) height) / 18.0)));
        tmpRect.X = (int) ((double) position.X + (double) width - 3.0 * (double) num4 - 2.0);
        tmpRect.Y = (int) ((double) position.Y + (double) num6 + 2.0);
        tmpRect.Height = (int) num5;
        tmpRect.Width = (int) num4;
        GuiData.spriteBatch.Draw(Utils.white, tmpRect, SelectableTextList.scrollBarColor);
      }
      if (lastSelectedIndex != num2)
        SelectableTextList.selectionWasChanged = true;
      return lastSelectedIndex;
    }

    public static int doFancyList(int myID, int x, int y, int width, int height, string[] text, int lastSelectedIndex, Color? selectedColor, bool HasDraggableScrollbar = false)
    {
      if (!selectedColor.HasValue)
        selectedColor = new Color?(GuiData.Default_Selected_Color);
      int num1 = -1;
      SelectableTextList.wasActivated = false;
      int num2 = lastSelectedIndex;
      SelectableTextList.selectionWasChanged = false;
      Vector2 mousePos = GuiData.getMousePos();
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = x;
      tmpRect.Y = y;
      tmpRect.Width = width;
      tmpRect.Height = height;
      if (tmpRect.Contains(GuiData.getMousePoint()))
      {
        GuiData.hot = myID;
        SelectableTextList.scrollOffset += (int) GuiData.getMouseWheelScroll();
        SelectableTextList.scrollOffset = Math.Max(0, Math.Min(SelectableTextList.scrollOffset, text.Length - (int) ((double) height / 18.0)));
      }
      else if (GuiData.hot == myID)
        GuiData.hot = -1;
      int num3 = Math.Max(0, Math.Min(SelectableTextList.scrollOffset, text.Length - (int) ((double) height / 18.0)));
      float num4 = HasDraggableScrollbar ? 4f : 2f;
      if (GuiData.hot == myID && (!HasDraggableScrollbar || (double) mousePos.X < (double) (x + width) - 2.0 * (double) num4))
      {
        for (int index = 0; index < text.Length; ++index)
        {
          if ((double) mousePos.Y >= (double) y + (double) index * 18.0 && (double) mousePos.Y <= (double) y + (double) (index + 1) * 18.0 && (double) mousePos.Y < (double) (y + height))
            num1 = index + num3;
        }
      }
      if (num1 != -1 && num1 != lastSelectedIndex && GuiData.mouseLeftUp())
      {
        lastSelectedIndex = num1;
        SelectableTextList.wasActivated = true;
      }
      tmpRect.X += 2;
      tmpRect.Width -= 4;
      tmpRect.Y += 2;
      tmpRect.Height -= 4;
      Vector2 input = new Vector2((float) tmpRect.X, (float) tmpRect.Y);
      tmpRect.Height = 18;
      for (int index = num3; index < text.Length; ++index)
      {
        GuiData.spriteBatch.Draw(Utils.white, tmpRect, lastSelectedIndex == index ? selectedColor.Value : (num1 == index ? selectedColor.Value * 0.45f : GuiData.Default_Dark_Neutral_Color));
        Vector2 scale = GuiData.UITinyfont.MeasureString(text[index]);
        scale.X = (double) scale.X <= (double) (width - 4) ? 1f : (float) (width - 4) / scale.X;
        scale.Y = (double) scale.Y <= 18.0 ? 1f : 18f / scale.Y;
        scale.X = Math.Min(scale.X, scale.Y);
        scale.Y = Math.Min(scale.X, scale.Y);
        bool flag = !LocaleActivator.ActiveLocaleIsCJK() && Settings.ActiveLocale != "en-us";
        if (flag)
          input.Y += 3f;
        GuiData.spriteBatch.DrawString(GuiData.UITinyfont, text[index], Utils.ClipVec2ForTextRendering(input), lastSelectedIndex == index ? Color.Black : Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
        if (flag)
          input.Y -= 3f;
        input.Y += 18f;
        tmpRect.Y += 18;
        if ((double) input.Y > (double) (y + height - 4))
          break;
      }
      if ((double) text.Length * 18.0 > (double) height)
      {
        float num5 = num4;
        float num6 = (float) height / ((float) text.Length * 18f) * (float) height;
        height -= 4;
        float num7 = (float) -height + (float) (((double) height - (double) num6) * ((double) num3 / (((double) text.Length * 18.0 - (double) height) / 18.0)));
        tmpRect.X = (int) ((double) input.X + (double) width - (HasDraggableScrollbar ? 2.0 : 3.0) * (double) num5 - 2.0);
        tmpRect.Y = (int) ((double) input.Y + (double) num7 + 2.0);
        tmpRect.Height = (int) num6;
        tmpRect.Width = (int) num5;
        if (!HasDraggableScrollbar)
          GuiData.spriteBatch.Draw(Utils.white, tmpRect, SelectableTextList.scrollBarColor);
        else
          SelectableTextList.scrollOffset = (int) ((double) ScrollBar.doVerticalScrollBar(myID + 101, tmpRect.X, y, tmpRect.Width, height, (int) ((double) text.Length * 18.0), (float) num3 * 18f) / 18.0);
      }
      if (lastSelectedIndex != num2)
        SelectableTextList.selectionWasChanged = true;
      return lastSelectedIndex;
    }
  }
}
