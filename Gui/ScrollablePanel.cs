// Decompiled with JetBrains decompiler
// Type: Hacknet.Gui.ScrollablePanel
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet.Gui
{
  public static class ScrollablePanel
  {
    private static Color scrollBarColor = new Color(120, 120, 120, 80);
    private static Stack<RenderTarget2D> targets;
    private static Stack<SpriteBatch> batches;
    private static List<RenderTarget2D> targetPool;
    private static List<SpriteBatch> batchPool;
    private static Stack<Vector2> offsetStack;

    public static void beginPanel(int id, Rectangle drawbounds, Vector2 scroll)
    {
      if (ScrollablePanel.targets == null)
      {
        ScrollablePanel.targets = new Stack<RenderTarget2D>();
        ScrollablePanel.batches = new Stack<SpriteBatch>();
        ScrollablePanel.targets.Push((RenderTarget2D) GuiData.spriteBatch.GraphicsDevice.GetRenderTargets()[0].RenderTarget);
        ScrollablePanel.targetPool = new List<RenderTarget2D>();
        ScrollablePanel.targetPool.Add(new RenderTarget2D(GuiData.spriteBatch.GraphicsDevice, drawbounds.Width, drawbounds.Height));
        ScrollablePanel.batchPool = new List<SpriteBatch>();
        ScrollablePanel.batchPool.Add(new SpriteBatch(GuiData.spriteBatch.GraphicsDevice));
        ScrollablePanel.batches.Push(GuiData.spriteBatch);
        ScrollablePanel.offsetStack = new Stack<Vector2>();
      }
      if (ScrollablePanel.batchPool.Count <= 0)
        ScrollablePanel.batchPool.Add(new SpriteBatch(GuiData.spriteBatch.GraphicsDevice));
      SpriteBatch spriteBatch = ScrollablePanel.batchPool[ScrollablePanel.batchPool.Count - 1];
      ScrollablePanel.batchPool.RemoveAt(ScrollablePanel.batchPool.Count - 1);
      ScrollablePanel.batches.Push(spriteBatch);
      bool flag = false;
      for (int index = ScrollablePanel.targetPool.Count - 1; index >= 0; --index)
      {
        RenderTarget2D renderTarget2D = ScrollablePanel.targetPool[index];
        if (renderTarget2D.Width == drawbounds.Width && renderTarget2D.Height == drawbounds.Height)
        {
          ScrollablePanel.targets.Push(renderTarget2D);
          ScrollablePanel.targetPool.RemoveAt(index);
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        RenderTarget2D renderTarget2D = new RenderTarget2D(GuiData.spriteBatch.GraphicsDevice, drawbounds.Width, drawbounds.Height);
        ScrollablePanel.targets.Push(renderTarget2D);
        Console.WriteLine("Creating RenderTarget");
      }
      ScrollablePanel.offsetStack.Push(GuiData.scrollOffset);
      GuiData.scrollOffset = new Vector2((float) drawbounds.X - scroll.X, (float) drawbounds.Y - scroll.Y);
      GuiData.spriteBatch.GraphicsDevice.SetRenderTarget(ScrollablePanel.targets.Peek());
      GuiData.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
      ScrollablePanel.batches.Peek().Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
      GuiData.spriteBatch = ScrollablePanel.batches.Peek();
    }

    public static Vector2 endPanel(int id, Vector2 scroll, Rectangle bounds, float maxScroll, bool onlyScrollWithMouseOver = false)
    {
      ScrollablePanel.batches.Peek().End();
      RenderTarget2D renderTarget2D = ScrollablePanel.targets.Pop();
      GuiData.spriteBatch.GraphicsDevice.SetRenderTarget(ScrollablePanel.targets.Peek());
      ScrollablePanel.targetPool.Add(renderTarget2D);
      ScrollablePanel.batchPool.Add(ScrollablePanel.batches.Pop());
      GuiData.spriteBatch = ScrollablePanel.batches.Peek();
      GuiData.scrollOffset = ScrollablePanel.offsetStack.Pop();
      Rectangle tmpRect1 = GuiData.tmpRect;
      tmpRect1.X = (int) scroll.X;
      tmpRect1.Y = (int) scroll.Y;
      tmpRect1.Width = bounds.Width;
      tmpRect1.Height = bounds.Height;
      try
      {
        GuiData.spriteBatch.Draw((Texture2D) renderTarget2D, bounds, new Rectangle?(tmpRect1), Color.White);
      }
      catch (InvalidOperationException ex)
      {
        return scroll;
      }
      if (!onlyScrollWithMouseOver || bounds.Contains(GuiData.getMousePoint()))
        scroll.Y += GuiData.getMouseWheelScroll() * 20f;
      scroll.Y = Math.Max(Math.Min(scroll.Y, maxScroll), 0.0f);
      Rectangle tmpRect2 = GuiData.tmpRect;
      float num1 = 5f;
      float num2 = (float) bounds.Height / maxScroll * (float) bounds.Height;
      float num3 = (float) bounds.Height - 4f;
      float num4 = scroll.Y / maxScroll * ((float) bounds.Height - num2);
      tmpRect2.Y = (int) ((double) num4 - (double) num2 / 2.0 + (double) num2 / 2.0 + (double) bounds.Y);
      tmpRect2.X = (int) ((double) (bounds.X + bounds.Width) - 1.5 * (double) num1 - 2.0);
      tmpRect2.Height = (int) num2;
      tmpRect2.Width = (int) num1;
      scroll.Y = ScrollBar.doVerticalScrollBar(id, tmpRect2.X, bounds.Y, tmpRect2.Width, bounds.Height, renderTarget2D.Height, scroll.Y);
      scroll.Y = Math.Max(Math.Min(scroll.Y, maxScroll), 0.0f);
      return scroll;
    }

    public static void ClearCache()
    {
      ScrollablePanel.targets = (Stack<RenderTarget2D>) null;
    }
  }
}
