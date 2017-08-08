// Decompiled with JetBrains decompiler
// Type: Hacknet.Gui.RenderedRectangle
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;

namespace Hacknet.Gui
{
  public static class RenderedRectangle
  {
    public static void doRectangle(int x, int y, int width, int height, Color? color)
    {
      if (!color.HasValue)
        color = new Color?(GuiData.Default_Backing_Color);
      if (width < 0)
      {
        x += width;
        width = Math.Abs(width);
      }
      if (height < 0)
      {
        y += height;
        height = Math.Abs(height);
      }
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = x;
      tmpRect.Width = width;
      tmpRect.Y = y;
      tmpRect.Height = height;
      GuiData.spriteBatch.Draw(Utils.white, tmpRect, color.Value);
    }

    public static void doRectangle(int x, int y, int width, int height, Color? color, bool blocking)
    {
      RenderedRectangle.doRectangle(x, y, width, height, color);
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = x;
      tmpRect.Y = y;
      tmpRect.Width = width;
      tmpRect.Height = height;
      if (!blocking || !tmpRect.Contains(GuiData.getMousePoint()))
        return;
      GuiData.blockingInput = true;
    }

    public static void doRectangleOutline(int x, int y, int width, int height, int thickness, Color? color)
    {
      if (!color.HasValue)
        color = new Color?(GuiData.Default_Backing_Color);
      if (width < 0)
      {
        x += width;
        width = Math.Abs(width);
      }
      if (height < 0)
      {
        y += height;
        height = Math.Abs(height);
      }
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = x + thickness;
      tmpRect.Width = width - 2 * thickness;
      tmpRect.Y = y;
      tmpRect.Height = thickness;
      GuiData.spriteBatch.Draw(Utils.white, tmpRect, color.Value);
      tmpRect.X = x + thickness;
      tmpRect.Width = width - 2 * thickness;
      tmpRect.Y = y + height - thickness;
      tmpRect.Height = thickness;
      GuiData.spriteBatch.Draw(Utils.white, tmpRect, color.Value);
      tmpRect.X = x;
      tmpRect.Width = thickness;
      tmpRect.Y = y;
      tmpRect.Height = height;
      GuiData.spriteBatch.Draw(Utils.white, tmpRect, color.Value);
      tmpRect.X = x + width - thickness;
      tmpRect.Width = thickness;
      tmpRect.Y = y;
      tmpRect.Height = height;
      GuiData.spriteBatch.Draw(Utils.white, tmpRect, color.Value);
    }
  }
}
