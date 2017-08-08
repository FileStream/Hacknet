// Decompiled with JetBrains decompiler
// Type: Hacknet.Modules.Helpers.DisplayModuleLSHelper
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet.Modules.Helpers
{
  internal class DisplayModuleLSHelper
  {
    private const int BUTTON_HEIGHT = 20;
    private const int BUTTON_MARGIN = 2;
    private const int INDENT = 30;
    private ScrollableSectionedPanel panel;

    public DisplayModuleLSHelper()
    {
      this.panel = new ScrollableSectionedPanel(24, GuiData.spriteBatch.GraphicsDevice);
    }

    public void DrawUI(Rectangle dest, OS os)
    {
      int ButtonHeight = (int) ((double) GuiData.ActiveFontConfig.tinyFontCharHeight + 10.0);
      if (this.panel.PanelHeight != ButtonHeight + 4)
        this.panel = new ScrollableSectionedPanel(ButtonHeight + 4, GuiData.spriteBatch.GraphicsDevice);
      List<DisplayModuleLSHelper.LSItem> items = this.BuildDirectoryDrawList(os.connectedComp == null ? os.thisComputer.files.root : os.connectedComp.files.root, 0, 0, os);
      this.panel.NumberOfPanels = items.Count;
      int width = dest.Width - 25;
      Action<int, Rectangle, SpriteBatch> DrawSection = (Action<int, Rectangle, SpriteBatch>) ((index, bounds, sb) =>
      {
        DisplayModuleLSHelper.LSItem lsItem = items[index];
        if (lsItem.IsEmtyDisplay)
          TextItem.doFontLabel(new Vector2((float) (bounds.X + 5 + lsItem.indent), (float) (bounds.Y + 2)), "-" + LocaleTerms.Loc("Empty") + "-", GuiData.tinyfont, new Color?(), (float) width, (float) ButtonHeight, false);
        else if (Button.doButton(300000 + index, bounds.X + 5 + lsItem.indent, bounds.Y + 2, width - lsItem.indent, ButtonHeight, lsItem.DisplayName, new Color?()))
          lsItem.Clicked();
      });
      Button.DisableIfAnotherIsActive = true;
      this.panel.Draw(DrawSection, GuiData.spriteBatch, dest);
      Button.DisableIfAnotherIsActive = false;
    }

    private List<DisplayModuleLSHelper.LSItem> BuildDirectoryDrawList(Folder f, int recItteration, int indentOffset, OS os)
    {
      List<DisplayModuleLSHelper.LSItem> lsItemList = new List<DisplayModuleLSHelper.LSItem>();
      double commandSeperationDelay = 0.019;
      for (int index1 = 0; index1 < f.folders.Count; ++index1)
      {
        int myIndex = index1;
        DisplayModuleLSHelper.LSItem lsItem = new DisplayModuleLSHelper.LSItem() { DisplayName = "/" + f.folders[index1].name, Clicked = (Action) (() =>
        {
          int num = 0;
          for (int index = 0; index < os.navigationPath.Count - recItteration; ++index)
          {
            Action action = (Action) (() => os.runCommand("cd .."));
            if (num > 0)
              os.delayer.Post(ActionDelayer.Wait((double) num * commandSeperationDelay), action);
            else
              action();
            ++num;
          }
          Action action1 = (Action) (() => os.runCommand("cd " + f.folders[myIndex].name));
          if (num > 0)
            os.delayer.Post(ActionDelayer.Wait((double) num * commandSeperationDelay), action1);
          else
            action1();
        }), indent = indentOffset };
        lsItemList.Add(lsItem);
        indentOffset += 30;
        if (os.navigationPath.Count - 1 >= recItteration && os.navigationPath[recItteration] == index1)
          lsItemList.AddRange((IEnumerable<DisplayModuleLSHelper.LSItem>) this.BuildDirectoryDrawList(f.folders[index1], recItteration + 1, indentOffset, os));
        indentOffset -= 30;
      }
      for (int index1 = 0; index1 < f.files.Count; ++index1)
      {
        int myIndex = index1;
        DisplayModuleLSHelper.LSItem lsItem = new DisplayModuleLSHelper.LSItem() { DisplayName = f.files[index1].name, Clicked = (Action) (() =>
        {
          int num = 0;
          for (int index = 0; index < os.navigationPath.Count - recItteration; ++index)
          {
            Action action = (Action) (() => os.runCommand("cd .."));
            if (num > 0)
              os.delayer.Post(ActionDelayer.Wait((double) num * commandSeperationDelay), action);
            else
              action();
            ++num;
          }
          Action action1 = (Action) (() => os.runCommand("cat " + f.files[myIndex].name));
          if (num > 0)
            os.delayer.Post(ActionDelayer.Wait((double) num * commandSeperationDelay), action1);
          else
            action1();
        }), indent = indentOffset };
        lsItemList.Add(lsItem);
      }
      if (f.folders.Count == 0 && f.files.Count == 0)
      {
        DisplayModuleLSHelper.LSItem lsItem = new DisplayModuleLSHelper.LSItem() { IsEmtyDisplay = true, indent = indentOffset };
        lsItemList.Add(lsItem);
      }
      return lsItemList;
    }

    private struct LSItem
    {
      public Action Clicked;
      public int indent;
      public string DisplayName;
      public bool IsEmtyDisplay;
    }
  }
}
