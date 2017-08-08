// Decompiled with JetBrains decompiler
// Type: Hacknet.Gui.ScrollBar
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;

namespace Hacknet.Gui
{
  public static class ScrollBar
  {
    public static bool AlwaysDrawUnderBar = false;

    public static float doVerticalScrollBar(int id, int xPos, int yPos, int drawWidth, int drawHeight, int contentHeight, float scroll)
    {
      if (drawHeight > contentHeight)
        contentHeight = drawHeight;
      Rectangle destinationRectangle = new Rectangle(xPos, yPos, drawWidth, drawHeight);
      if (ScrollBar.AlwaysDrawUnderBar || destinationRectangle.Contains(GuiData.getMousePoint()) || GuiData.active == id)
        GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Gray * 0.1f);
      float num1 = scroll;
      float num2 = (float) (contentHeight - drawHeight);
      float num3 = (float) drawHeight / (float) contentHeight * (float) drawHeight;
      float num4 = scroll / num2 * ((float) drawHeight - num3);
      float y = DraggableRectangle.doDraggableRectangle(id, (float) xPos, (float) yPos + num4, drawWidth, (int) num3, (float) drawWidth, new Color?(Color.White), new Color?(Color.Gray), true, false, (float) xPos, (float) (yPos + drawHeight) - num3, (float) xPos, (float) yPos).Y;
      if ((double) Math.Abs(y) > 0.100000001490116)
        num1 = (float) (((double) num4 + (double) y) / ((double) drawHeight - (double) num3)) * num2;
      return num1;
    }
  }
}
