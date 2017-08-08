// Decompiled with JetBrains decompiler
// Type: Hacknet.UIUtils.ScrollableSectionedPanel
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.UIUtils
{
  public class ScrollableSectionedPanel
  {
    public int PanelHeight = 100;
    public float ScrollDown = 0.0f;
    public int NumberOfPanels = 0;
    public int ScrollbarUIIndexOffset = 0;
    public bool HasScrollBar = true;
    private RenderTarget2D AboveFragment;
    private RenderTarget2D BelowFragment;
    private SpriteBatch fragmentBatch;
    private GraphicsDevice graphics;

    public ScrollableSectionedPanel(int panelHeight, GraphicsDevice graphics)
    {
      this.PanelHeight = panelHeight;
      this.fragmentBatch = new SpriteBatch(graphics);
      this.fragmentBatch.Name = "AltRTBatch";
      this.graphics = graphics;
    }

    private void UpdateInput(Rectangle dest)
    {
      this.ScrollDown += GuiData.getMouseWheelScroll() * 20f;
      this.ScrollDown = Math.Max(Math.Min(this.ScrollDown, this.GetMaxScroll(dest)), 0.0f);
    }

    private float GetMaxScroll(Rectangle dest)
    {
      return (float) (this.NumberOfPanels * this.PanelHeight) - (float) dest.Height;
    }

    public void Draw(Action<int, Rectangle, SpriteBatch> DrawSection, SpriteBatch sb, Rectangle destination)
    {
      this.UpdateInput(destination);
      this.SetRenderTargetsToFrame(destination);
      bool flag = destination.Contains(GuiData.getMousePoint());
      Vector2 scrollOffset = GuiData.scrollOffset;
      Vector2 vector2 = new Vector2((float) destination.X, (float) destination.Y - this.ScrollDown % (float) this.PanelHeight);
      if (flag)
        GuiData.scrollOffset = vector2;
      SpriteBatch spriteBatch = GuiData.spriteBatch;
      int index1 = 0;
      float scrollDown = this.ScrollDown;
      while ((double) scrollDown >= (double) this.PanelHeight)
      {
        scrollDown -= (float) this.PanelHeight;
        ++index1;
      }
      int num1 = 0;
      if ((double) this.ScrollDown % (double) this.PanelHeight != 0.0)
      {
        GuiData.spriteBatch = this.fragmentBatch;
        this.RenderToTarget(DrawSection, this.AboveFragment, index1, new Rectangle(0, 0, destination.Width, this.PanelHeight));
        int height = this.PanelHeight - (int) ((double) this.ScrollDown % (double) this.PanelHeight);
        Rectangle destinationRectangle = new Rectangle(destination.X, destination.Y, destination.Width, height);
        Rectangle rectangle = new Rectangle(0, this.PanelHeight - destinationRectangle.Height, destination.Width, height);
        sb.Draw((Texture2D) this.AboveFragment, destinationRectangle, new Rectangle?(rectangle), Color.White);
        num1 += height;
        ++index1;
      }
      GuiData.spriteBatch = spriteBatch;
      if (flag)
        GuiData.scrollOffset = scrollOffset;
      int index2 = index1;
      Rectangle rectangle1 = new Rectangle(destination.X, destination.Y + num1, destination.Width, this.PanelHeight);
      while (num1 + this.PanelHeight < destination.Height && index2 < this.NumberOfPanels)
      {
        DrawSection(index2, rectangle1, sb);
        ++index2;
        num1 += this.PanelHeight;
        rectangle1.Y = destination.Y + num1;
      }
      vector2 = new Vector2((float) destination.X, (float) rectangle1.Y);
      if (flag)
        GuiData.scrollOffset = vector2;
      if (index2 < this.NumberOfPanels && destination.Height - num1 > 0)
      {
        GuiData.spriteBatch = this.fragmentBatch;
        this.RenderToTarget(DrawSection, this.BelowFragment, index2, new Rectangle(0, 0, destination.Width, this.PanelHeight));
        int height = destination.Height - num1;
        Rectangle destinationRectangle = new Rectangle(destination.X, rectangle1.Y, destination.Width, height);
        Rectangle rectangle2 = new Rectangle(0, 0, destination.Width, height);
        sb.Draw((Texture2D) this.BelowFragment, destinationRectangle, new Rectangle?(rectangle2), Color.White);
        int num2 = num1 + height;
        int num3 = index1 + 1;
      }
      GuiData.spriteBatch = spriteBatch;
      if (flag)
        GuiData.scrollOffset = scrollOffset;
      if (!this.HasScrollBar)
        return;
      int width = 7;
      this.DrawScrollBar(new Rectangle(destination.X + destination.Width - width - 2, destination.Y, width, destination.Height - 1), width);
    }

    private void DrawScrollBar(Rectangle dest, int width)
    {
      this.ScrollDown = ScrollBar.doVerticalScrollBar(184602004 + this.ScrollbarUIIndexOffset, dest.X, dest.Y, dest.Width, dest.Height, this.NumberOfPanels * this.PanelHeight, this.ScrollDown);
    }

    private void RenderToTarget(Action<int, Rectangle, SpriteBatch> DrawSection, RenderTarget2D target, int index, Rectangle destination)
    {
      RenderTarget2D renderTarget = (RenderTarget2D) this.graphics.GetRenderTargets()[0].RenderTarget;
      this.graphics.SetRenderTarget(target);
      this.graphics.Clear(Color.Transparent);
      this.fragmentBatch.Begin();
      DrawSection(index, destination, this.fragmentBatch);
      this.fragmentBatch.End();
      this.graphics.SetRenderTarget(renderTarget);
    }

    private void SetRenderTargetsToFrame(Rectangle frame)
    {
      if (this.AboveFragment != null && this.AboveFragment.Width == frame.Width)
        return;
      this.AboveFragment = new RenderTarget2D(this.graphics, frame.Width, this.PanelHeight);
      this.BelowFragment = new RenderTarget2D(this.graphics, frame.Width, this.PanelHeight);
    }
  }
}
