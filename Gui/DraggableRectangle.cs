// Decompiled with JetBrains decompiler
// Type: Hacknet.Gui.DraggableRectangle
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;

namespace Hacknet.Gui
{
  public static class DraggableRectangle
  {
    public static bool isDragging = false;
    public static Vector2 originalClickPos;
    public static Vector2 originalClickOffset;

    public static Vector2 doDraggableRectangle(int myID, float x, float y, int width, int height)
    {
      return DraggableRectangle.doDraggableRectangle(myID, x, y, width, height, -1f, new Color?(), new Color?());
    }

    public static Vector2 doDraggableRectangle(int myID, float x, float y, int width, int height, float selectableBorder, Color? selectedColor, Color? deselectedColor)
    {
      return DraggableRectangle.doDraggableRectangle(myID, x, y, width, height, selectableBorder, selectedColor, deselectedColor, true, true, float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
    }

    public static Vector2 doDraggableRectangle(int myID, float x, float y, int width, int height, float selectableBorder, Color? selectedColor, Color? deselectedColor, bool canMoveY, bool canMoveX, float xMax, float yMax, float xMin, float yMin)
    {
      DraggableRectangle.isDragging = false;
      if (!selectedColor.HasValue)
        selectedColor = new Color?(GuiData.Default_Selected_Color);
      if (!deselectedColor.HasValue)
        deselectedColor = new Color?(GuiData.Default_Unselected_Color);
      Vector2 temp = GuiData.temp;
      temp.X = 0.0f;
      temp.Y = 0.0f;
      Rectangle tmpRect1 = GuiData.tmpRect;
      tmpRect1.X = (int) x;
      tmpRect1.Y = (int) y;
      tmpRect1.Width = width;
      tmpRect1.Height = height;
      Rectangle tmpRect2 = GuiData.tmpRect;
      tmpRect2.X = (int) ((double) x + (double) selectableBorder);
      tmpRect2.Y = (int) ((double) y + (double) selectableBorder);
      tmpRect2.Width = (double) selectableBorder == -1.0 ? 0 : (int) ((double) width - 2.0 * (double) selectableBorder);
      tmpRect2.Height = (double) selectableBorder == -1.0 ? 0 : (int) ((double) height - 2.0 * (double) selectableBorder);
      if (tmpRect1.Contains(GuiData.getMousePoint()) && !tmpRect2.Contains(GuiData.getMousePoint()))
      {
        GuiData.hot = myID;
        if (GuiData.active != myID && GuiData.mouseWasPressed())
        {
          GuiData.active = myID;
          DraggableRectangle.originalClickPos = GuiData.getMousePos();
          DraggableRectangle.originalClickPos.X -= x;
          DraggableRectangle.originalClickPos.Y -= y;
          DraggableRectangle.originalClickOffset = new Vector2(DraggableRectangle.originalClickPos.X - x, DraggableRectangle.originalClickPos.Y - y);
        }
      }
      else if (GuiData.hot == myID)
        GuiData.hot = -1;
      if (GuiData.active == myID)
      {
        if (GuiData.mouseLeftUp())
        {
          GuiData.active = -1;
        }
        else
        {
          if (canMoveX)
          {
            temp.X = (float) GuiData.mouse.X - x - DraggableRectangle.originalClickPos.X;
            temp.X = Math.Min(Math.Max((float) tmpRect1.X + temp.X, xMin), xMax) - (float) tmpRect1.X;
          }
          if (canMoveY)
          {
            temp.Y = (float) GuiData.mouse.Y - y - DraggableRectangle.originalClickPos.Y;
            temp.Y = Math.Min(Math.Max((float) tmpRect1.Y + temp.Y, yMin), yMax) - (float) tmpRect1.Y;
          }
          tmpRect1.X += (int) temp.X;
          tmpRect1.Y += (int) temp.Y;
          DraggableRectangle.isDragging = true;
        }
      }
      if (GuiData.active == myID || GuiData.hot == myID)
        GuiData.blockingInput = true;
      GuiData.spriteBatch.Draw(Utils.white, tmpRect1, GuiData.hot == myID || GuiData.active == myID ? selectedColor.Value : deselectedColor.Value);
      return temp;
    }
  }
}
