// Decompiled with JetBrains decompiler
// Type: Hacknet.ThemeChangerExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class ThemeChangerExe : ExeModule
  {
    private float loadingTimeRemaining = 25.5f;
    private ThemeChangerExe.ThemeChangerState state = ThemeChangerExe.ThemeChangerState.List;
    private int remotesSelected = -1;
    private int localsSelected = -1;
    private int remoteScroll = 0;
    private int localScroll = 0;
    private const float START_LOADING_TIME = 25.5f;
    private RenderTarget2D target;
    private SpriteBatch internalSB;
    private Texture2D circle;
    private BarcodeEffect barcodeEffect;
    private Color themeColor;

    public ThemeChangerExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.name = "ThemeSwitch";
      this.ramCost = 320;
      this.IdentifierName = "Theme Switch";
      this.targetIP = this.os.thisComputer.ip;
      this.circle = TextureBank.load("Circle", this.os.content);
      AchievementsManager.Unlock("themeswitch_run", false);
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      this.themeColor = this.os.highlightColor;
      Rectangle dest = new Rectangle(this.bounds.X + 2, this.bounds.Y + Module.PANEL_HEIGHT, this.bounds.Width - 4, this.bounds.Height - 2 - Module.PANEL_HEIGHT);
      switch (this.state)
      {
        case ThemeChangerExe.ThemeChangerState.Startup:
          this.loadingTimeRemaining -= t;
          this.DrawLoading(this.loadingTimeRemaining, 25.5f, dest, this.spriteBatch);
          break;
        case ThemeChangerExe.ThemeChangerState.LoadItem:
          break;
        case ThemeChangerExe.ThemeChangerState.Activating:
          break;
        default:
          this.DrawListing(dest, this.spriteBatch);
          break;
      }
    }

    public void DrawWithSeperateRT(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      this.themeColor = this.os.highlightColor;
      Rectangle destinationRectangle = new Rectangle(this.bounds.X + 2, this.bounds.Y + Module.PANEL_HEIGHT + 2, this.bounds.Width - 4, this.bounds.Height - 3 - Module.PANEL_HEIGHT);
      bool flag = false;
      if (this.target == null)
      {
        this.target = new RenderTarget2D(this.spriteBatch.GraphicsDevice, destinationRectangle.Width, destinationRectangle.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        this.internalSB = new SpriteBatch(this.spriteBatch.GraphicsDevice);
        flag = true;
      }
      RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
      this.spriteBatch.GraphicsDevice.SetRenderTarget(this.target);
      Rectangle dest = new Rectangle(0, 0, destinationRectangle.Width, destinationRectangle.Height);
      SpriteBatch spriteBatch = GuiData.spriteBatch;
      GuiData.spriteBatch = this.internalSB;
      this.internalSB.Begin();
      if (flag)
        this.internalSB.GraphicsDevice.Clear(Color.Transparent);
      switch (this.state)
      {
        case ThemeChangerExe.ThemeChangerState.Startup:
          this.loadingTimeRemaining -= t;
          this.DrawLoading(this.loadingTimeRemaining, 25.5f, dest, this.internalSB);
          goto case ThemeChangerExe.ThemeChangerState.LoadItem;
        case ThemeChangerExe.ThemeChangerState.LoadItem:
        case ThemeChangerExe.ThemeChangerState.Activating:
          this.internalSB.End();
          GuiData.spriteBatch = spriteBatch;
          this.spriteBatch.GraphicsDevice.SetRenderTarget(currentRenderTarget);
          this.spriteBatch.Draw((Texture2D) this.target, destinationRectangle, Color.White);
          break;
        default:
          this.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
          this.DrawListing(dest, this.internalSB);
          goto case ThemeChangerExe.ThemeChangerState.LoadItem;
      }
    }

    private void DrawListing(Rectangle dest, SpriteBatch sb)
    {
      this.DrawHeaders(dest, sb);
      if (this.isExiting)
        return;
      Vector2 pos = new Vector2((float) (dest.X + 2), (float) dest.Y + 60f);
      TextItem.doFontLabel(pos, LocaleTerms.Loc("Remote"), GuiData.smallfont, new Color?(this.themeColor), (float) (this.bounds.Width - 20), 20f, false);
      pos.Y += 18f;
      sb.Draw(Utils.white, new Rectangle(this.bounds.X + 2, (int) pos.Y, this.bounds.Width - 6, 1), Utils.AddativeWhite);
      List<string> stringList1 = new List<string>();
      Folder currentFolder = Programs.getCurrentFolder(this.os);
      for (int index = 0; index < currentFolder.files.Count; ++index)
      {
        if (ThemeManager.getThemeForDataString(currentFolder.files[index].data) != OSTheme.TerminalOnlyBlack)
          stringList1.Add(currentFolder.files[index].name);
      }
      string str = (string) null;
      string selectedFileData = (string) null;
      Color color = Color.Lerp(this.os.topBarColor, Utils.AddativeWhite, 0.2f);
      int scrollOffset = SelectableTextList.scrollOffset;
      Rectangle rectangle1 = new Rectangle((int) pos.X, (int) pos.Y, this.bounds.Width - 6, 54);
      if (stringList1.Count > 0)
      {
        SelectableTextList.scrollOffset = this.remoteScroll;
        this.remotesSelected = SelectableTextList.doFancyList(8139191 + this.PID, rectangle1.X, rectangle1.Y, rectangle1.Width, rectangle1.Height, stringList1.ToArray(), this.remotesSelected, new Color?(color), true);
        if (SelectableTextList.selectionWasChanged)
          this.localsSelected = -1;
        this.remoteScroll = SelectableTextList.scrollOffset;
        if (this.remotesSelected >= 0)
        {
          if (this.remotesSelected >= stringList1.Count)
          {
            this.remotesSelected = -1;
          }
          else
          {
            str = stringList1[this.remotesSelected];
            selectedFileData = currentFolder.searchForFile(str).data;
          }
        }
      }
      else
      {
        sb.Draw(Utils.white, rectangle1, Utils.VeryDarkGray);
        TextItem.doFontLabelToSize(rectangle1, "    -- " + LocaleTerms.Loc("No Valid Files") + " --    ", GuiData.smallfont, Utils.AddativeWhite, false, false);
      }
      pos.Y += (float) (rectangle1.Height + 6);
      TextItem.doFontLabel(pos, LocaleTerms.Loc("Local Theme Files"), GuiData.smallfont, new Color?(this.themeColor), (float) (this.bounds.Width - 20), 20f, false);
      pos.Y += 18f;
      sb.Draw(Utils.white, new Rectangle(this.bounds.X + 2, (int) pos.Y, this.bounds.Width - 6, 1), Utils.AddativeWhite);
      stringList1.Clear();
      List<string> stringList2 = new List<string>();
      Folder folder1 = this.os.thisComputer.files.root.searchForFolder("sys");
      for (int index = 0; index < folder1.files.Count; ++index)
      {
        if (ThemeManager.getThemeForDataString(folder1.files[index].data) != OSTheme.TerminalOnlyBlack)
        {
          stringList1.Add(folder1.files[index].name);
          stringList2.Add(folder1.files[index].data);
        }
      }
      Folder folder2 = this.os.thisComputer.files.root.searchForFolder("home");
      for (int index = 0; index < folder2.files.Count; ++index)
      {
        if (ThemeManager.getThemeForDataString(folder2.files[index].data) != OSTheme.TerminalOnlyBlack)
        {
          stringList1.Add(folder2.files[index].name);
          stringList2.Add(folder2.files[index].data);
        }
      }
      Rectangle rectangle2 = new Rectangle((int) pos.X, (int) pos.Y, this.bounds.Width - 6, 72);
      if (stringList1.Count > 0)
      {
        SelectableTextList.scrollOffset = this.localScroll;
        this.localsSelected = SelectableTextList.doFancyList(839192 + this.PID, rectangle2.X, rectangle2.Y, rectangle2.Width, rectangle2.Height, stringList1.ToArray(), this.localsSelected, new Color?(color), true);
        if (SelectableTextList.selectionWasChanged)
          this.remotesSelected = -1;
        this.localScroll = SelectableTextList.scrollOffset;
        if (this.localsSelected >= 0)
        {
          str = stringList1[this.localsSelected];
          selectedFileData = stringList2[this.localsSelected];
        }
      }
      else
      {
        sb.Draw(Utils.white, rectangle2, Utils.VeryDarkGray);
        TextItem.doFontLabelToSize(rectangle2, "    -- " + LocaleTerms.Loc("No Valid Files") + " --    ", GuiData.smallfont, Utils.AddativeWhite, false, false);
      }
      SelectableTextList.scrollOffset = scrollOffset;
      pos.Y += (float) (rectangle2.Height + 2);
      Rectangle bounds = new Rectangle(this.bounds.X + 4, (int) pos.Y + 2, this.bounds.Width - 8, (int) ((double) dest.Height - ((double) pos.Y - (double) dest.Y)) - 4);
      this.DrawApplyField(str, selectedFileData, bounds, sb);
    }

    private void DrawApplyField(string selectedFilename, string selectedFileData, Rectangle bounds, SpriteBatch sb)
    {
      sb.Draw(Utils.white, bounds, Utils.VeryDarkGray);
      sb.Draw(Utils.white, new Rectangle(bounds.X, bounds.Y, bounds.Width, 1), Utils.AddativeWhite);
      if (selectedFileData == null || selectedFilename == null)
        return;
      Color representativeColorForTheme = ThemeManager.GetRepresentativeColorForTheme(ThemeManager.getThemeForDataString(selectedFileData));
      Rectangle destinationRectangle = new Rectangle(bounds.X + bounds.Width / 4 * 3, bounds.Y + 2, bounds.Width / 4, bounds.Height - 2);
      sb.Draw(Utils.white, destinationRectangle, representativeColorForTheme);
      destinationRectangle.X += destinationRectangle.Width - 15;
      destinationRectangle.Width = 15;
      sb.Draw(Utils.white, destinationRectangle, Color.Black * 0.6f);
      TextItem.doFontLabel(new Vector2((float) bounds.X, (float) (bounds.Y + 2)), selectedFilename, GuiData.smallfont, new Color?(Utils.AddativeWhite), (float) (bounds.Width / 4 * 3), 25f, false);
      if (Button.doButton(3837791 + this.PID, bounds.X, bounds.Y + 25, bounds.Width / 6 * 5, 30, LocaleTerms.Loc("Activate Theme"), new Color?(representativeColorForTheme)))
        this.ApplyTheme(selectedFilename, selectedFileData);
    }

    private void ApplyTheme(string themeFilename, string fileData)
    {
      string filename = "x-serverBACKUP";
      Folder f = this.os.thisComputer.files.root.searchForFolder("sys");
      FileEntry fileEntry = f.searchForFile("x-server.sys");
      if (fileEntry != null)
      {
        bool flag = true;
        for (int index = 0; index < f.files.Count; ++index)
        {
          if (f.files[index].name.StartsWith(filename) && f.files[index].data == fileEntry.data)
          {
            flag = false;
            break;
          }
        }
        if (flag)
        {
          string repeatingFilename = Utils.GetNonRepeatingFilename(filename, ".sys", f);
          f.files.Add(new FileEntry(fileEntry.data, repeatingFilename));
        }
      }
      fileEntry.data = fileData;
      ThemeManager.switchTheme((object) this.os, ThemeManager.getThemeForDataString(fileData));
    }

    private void DrawHeaders(Rectangle dest, SpriteBatch sb)
    {
      if (this.barcodeEffect == null)
        this.barcodeEffect = new BarcodeEffect(this.bounds.Width - 4, false, false);
      this.barcodeEffect.Update((float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
      this.barcodeEffect.Draw(dest.X, dest.Y, dest.Width - 4, Math.Min(30, this.bounds.Height - Module.PANEL_HEIGHT), sb, new Color?(this.themeColor));
      if (this.bounds.Height <= Module.PANEL_HEIGHT + 60)
        return;
      TextItem.doFontLabel(new Vector2((float) (dest.X + 2), (float) (dest.Y + 32)), "Themechanger", GuiData.font, new Color?(Utils.AddativeWhite * 0.8f), (float) (this.bounds.Width - 80), 30f, false);
      int width = 60;
      if (Button.doButton(this.PID + 228173, this.bounds.X + this.bounds.Width - width - 2, dest.Y + 38, width, 20, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
        this.isExiting = true;
    }

    private void DrawLoading(float timeRemaining, float totalTime, Rectangle dest, SpriteBatch sb)
    {
      float loaderRadius = 20f;
      Vector2 loaderCentre = new Vector2((float) dest.X + (float) dest.Width / 2f, (float) dest.Y + (float) dest.Height / 2f);
      float num1 = totalTime - timeRemaining;
      float num2 = num1 / totalTime;
      sb.Draw(Utils.white, dest, Color.Black * 0.05f);
      this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, loaderRadius, num1 * 0.2f, 1f, sb);
      this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, loaderRadius + 20f * num2, num1 * -0.4f, 0.7f, sb);
      this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, loaderRadius + 35f * num2, num1 * 0.5f, 0.52f, sb);
    }

    private void DrawLoadingCircle(float timeRemaining, float totalTime, Rectangle dest, Vector2 loaderCentre, float loaderRadius, float baseRotationAdd, float rotationRateRPS, SpriteBatch sb)
    {
      float num1 = totalTime - timeRemaining;
      int num2 = 10;
      for (int index = 0; index < num2; ++index)
      {
        float num3 = (float) index / (float) num2;
        float num4 = 2f;
        float num5 = 1f;
        float num6 = 6.283185f;
        float num7 = num6 + num5;
        float num8 = num3 * num4;
        if ((double) num1 > (double) num8)
        {
          float num9 = num1 / num8 * rotationRateRPS % num7;
          if ((double) num9 >= (double) num6)
            num9 = 0.0f;
          float angle = num6 * Utils.QuadraticOutCurve(num9 / num6) + baseRotationAdd;
          Vector2 vector2_1 = loaderCentre + Utils.PolarToCartesian(angle, loaderRadius);
          sb.Draw(this.circle, vector2_1, new Rectangle?(), Utils.AddativeWhite, 0.0f, Vector2.Zero, (float) (0.100000001490116 * ((double) loaderRadius / 120.0)), SpriteEffects.None, 0.3f);
          if (Utils.random.NextDouble() < 0.001)
          {
            Vector2 vector2_2 = loaderCentre + Utils.PolarToCartesian(angle, 20f + Utils.randm(45f));
            sb.Draw(Utils.white, vector2_1, Utils.AddativeWhite);
            Utils.drawLine(sb, vector2_1, vector2_2, Vector2.Zero, Utils.AddativeWhite * 0.4f, 0.1f);
          }
        }
      }
    }

    private enum ThemeChangerState
    {
      Startup,
      List,
      LoadItem,
      Activating,
    }
  }
}
