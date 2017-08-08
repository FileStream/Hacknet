// Decompiled with JetBrains decompiler
// Type: Hacknet.UIUtils.ScrollableTextRegion
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet.UIUtils
{
  public class ScrollableTextRegion
  {
    private string activeFontConfigName = (string) null;
    private ScrollableSectionedPanel Panel;

    public ScrollableTextRegion(GraphicsDevice gd)
    {
      this.Panel = new ScrollableSectionedPanel((int) ((double) GuiData.ActiveFontConfig.tinyFontCharHeight + 2.0), gd);
      this.activeFontConfigName = GuiData.ActiveFontConfig.name;
    }

    public void Draw(Rectangle dest, string text, SpriteBatch sb)
    {
      this.Draw(dest, text, sb, Color.White);
    }

    public void Draw(Rectangle dest, string text, SpriteBatch sb, Color TextDrawColor)
    {
      try
      {
        if (GuiData.ActiveFontConfig.name != this.activeFontConfigName)
        {
          this.Panel.ScrollDown = 0.0f;
          this.Panel.PanelHeight = (int) ((double) GuiData.ActiveFontConfig.tinyFontCharHeight + 2.0 + (double) GuiData.ActiveFontConfig.tinyFont.LineSpacing);
          this.activeFontConfigName = GuiData.ActiveFontConfig.name;
        }
        string[] data = text.Split(Utils.robustNewlineDelim, StringSplitOptions.None);
        this.Panel.NumberOfPanels = data.Length;
        this.Panel.Draw((Action<int, Rectangle, SpriteBatch>) ((index, panelDest, spriteBatch) => spriteBatch.DrawString(GuiData.tinyfont, data[index], Utils.ClipVec2ForTextRendering(new Vector2((float) panelDest.X, (float) panelDest.Y)), TextDrawColor)), sb, dest);
      }
      catch (Exception ex)
      {
        TextItem.doFontLabelToSize(dest, text, GuiData.tinyfont, TextDrawColor, false, false);
      }
    }

    public void Draw(Rectangle dest, List<string> textLines, SpriteBatch sb, Color TextDrawColor)
    {
      if (GuiData.ActiveFontConfig.name != this.activeFontConfigName)
      {
        this.Panel.ScrollDown = 0.0f;
        this.Panel.PanelHeight = (int) ((double) GuiData.ActiveFontConfig.tinyFontCharHeight + 2.0);
        this.activeFontConfigName = GuiData.ActiveFontConfig.name;
      }
      this.Panel.NumberOfPanels = textLines.Count;
      this.Panel.Draw((Action<int, Rectangle, SpriteBatch>) ((index, panelDest, spriteBatch) => spriteBatch.DrawString(GuiData.tinyfont, textLines[index], Utils.ClipVec2ForTextRendering(new Vector2((float) panelDest.X, (float) panelDest.Y)), TextDrawColor)), sb, dest);
    }

    public void UpdateScroll(float newScroll)
    {
      this.Panel.ScrollDown = newScroll;
    }

    public float GetScrollDown()
    {
      return this.Panel.ScrollDown;
    }

    public void SetScrollbarUIIndexOffset(int index)
    {
      this.Panel.ScrollbarUIIndexOffset = index;
    }
  }
}
