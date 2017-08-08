// Decompiled with JetBrains decompiler
// Type: Hacknet.MemoryForensicsExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Hacknet
{
  internal class MemoryForensicsExe : ExeModule, MainDisplayOverrideEXE
  {
    public MemoryForensicsExe.MemForensicsState State = MemoryForensicsExe.MemForensicsState.ReadingFile;
    private string ErrorMessage = "Unknown Error";
    private float timeInCurrentState = 0.0f;
    private float processingTimeThisBatch = 4f;
    private string filenameLoaded = "UNKNOWN";
    private MemoryContents ActiveMem = (MemoryContents) null;
    private ShiftingGridEffect GridEffect = new ShiftingGridEffect();
    private Color ThemeColorMain = new Color(30, 59, 44, 0);
    private Color ThemeColorLight = new Color(89, 181, 183, 0);
    private Color ThemeColorDark = new Color(19, 51, 35, 0);
    private List<string> OutputData = new List<string>();
    private bool IsDisplayingImages = false;
    private List<Texture2D> OutputTextures = new List<Texture2D>();
    private string AnnouncementData = "Unknown";
    private Vector2 PanelScroll = Vector2.Zero;
    private const float FileReadTime = 2f;
    private const float BaseProcessingTime = 2f;
    private FlyoutEffect flyoutEffect;

    public bool DisplayOverrideIsActive { get; set; }

    public MemoryForensicsExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.needsProxyAccess = false;
      this.name = "MemForensics";
      this.ramCost = 300;
      this.IdentifierName = "MemForensics";
      if (p.Length <= 1)
      {
        this.ErrorMessage = "No file specified.";
        this.State = MemoryForensicsExe.MemForensicsState.Error;
      }
      else
        this.LoadFile(p[1], Programs.getCurrentFolder(this.os));
      this.DisplayOverrideIsActive = true;
    }

    public static MemoryForensicsExe GenerateInstanceOrNullFromArguments(Rectangle location, OS os, string[] p)
    {
      for (int index = 0; index < os.exes.Count; ++index)
      {
        MemoryForensicsExe ex = os.exes[index] as MemoryForensicsExe;
        if (ex != null)
        {
          if (p.Length > 1)
          {
            ex.LoadFile(p[1], Programs.getCurrentFolder(os));
          }
          else
          {
            ex.State = MemoryForensicsExe.MemForensicsState.SelectingFile;
            ex.timeInCurrentState = 0.0f;
          }
          return (MemoryForensicsExe) null;
        }
      }
      return new MemoryForensicsExe(location, os, p);
    }

    internal void LoadFile(string filename, Folder f)
    {
      FileEntry fileEntry = f.searchForFile(filename);
      if (fileEntry == null)
      {
        fileEntry = f.searchForFile(filename + ".md");
        if (fileEntry == null)
        {
          this.State = MemoryForensicsExe.MemForensicsState.Error;
          this.ErrorMessage = string.Format(LocaleTerms.Loc("File {0} not found in current folder!"), (object) filename);
          return;
        }
      }
      try
      {
        this.ActiveMem = MemoryContents.GetMemoryFromEncodedFileString(fileEntry.data);
        this.filenameLoaded = filename;
        this.State = MemoryForensicsExe.MemForensicsState.ReadingFile;
        this.timeInCurrentState = 0.0f;
      }
      catch (Exception ex)
      {
        this.State = MemoryForensicsExe.MemForensicsState.Error;
        this.ErrorMessage = LocaleTerms.Loc("Error deserializing memory dump.") + "\r\n\r\n" + Utils.GenerateReportFromException(ex);
      }
    }

    public override void Update(float t)
    {
      base.Update(t);
      this.timeInCurrentState += t;
      if (this.State == MemoryForensicsExe.MemForensicsState.ReadingFile && (double) this.timeInCurrentState >= 2.0)
      {
        this.timeInCurrentState = 0.0f;
        this.State = MemoryForensicsExe.MemForensicsState.Main;
      }
      if (this.State == MemoryForensicsExe.MemForensicsState.Processing && (double) this.timeInCurrentState >= (double) this.processingTimeThisBatch)
      {
        this.timeInCurrentState = 0.0f;
        this.State = MemoryForensicsExe.MemForensicsState.DisplayingSolution;
      }
      if (this.State == MemoryForensicsExe.MemForensicsState.Main)
        this.OutputData.Clear();
      this.GridEffect.Update(t);
    }

    private void StartLoadingInTexturesForMemory()
    {
      this.OutputTextures.Clear();
      for (int index = 0; index < this.OutputData.Count; ++index)
      {
        string assetName = this.OutputData[index];
        Texture2D texture2D = (Texture2D) null;
        string path = Utils.GetFileLoadPrefix() + assetName;
        if (path.EndsWith(".jpg") || path.EndsWith(".png"))
        {
          using (FileStream fileStream = File.OpenRead(path))
            texture2D = Texture2D.FromStream(Game1.getSingleton().GraphicsDevice, (Stream) fileStream);
        }
        else
          texture2D = this.os.content.Load<Texture2D>(assetName);
        this.OutputTextures.Add(texture2D);
      }
    }

    public override void Completed()
    {
      base.Completed();
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      Rectangle contentAreaDest = this.GetContentAreaDest();
      if (this.flyoutEffect == null)
        this.flyoutEffect = new FlyoutEffect(GuiData.spriteBatch.GraphicsDevice, this.os.content, contentAreaDest.Width, contentAreaDest.Height);
      this.flyoutEffect.Draw((float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds, contentAreaDest, this.spriteBatch, (Action<SpriteBatch, Rectangle>) ((sb, innerDest) => ZoomingDotGridEffect.Render(innerDest, sb, this.os.timer, this.ThemeColorDark)));
      this.spriteBatch.Draw(Utils.white, contentAreaDest, Color.Black * 0.6f);
      if (this.isExiting)
        return;
      Rectangle rectangle = new Rectangle(contentAreaDest.X + 8, contentAreaDest.Y + contentAreaDest.Height - 30 - 8, contentAreaDest.Width - 16, 30);
      if (Button.doButton(383023811 + this.PID, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
        this.isExiting = true;
      rectangle.Y -= rectangle.Height + 8;
      if (Button.doButton(383023822 + this.PID, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, this.DisplayOverrideIsActive ? LocaleTerms.Loc("Close Main Display") : LocaleTerms.Loc("Re-Open Main Display"), new Color?(this.ThemeColorMain)))
        this.DisplayOverrideIsActive = !this.DisplayOverrideIsActive;
    }

    private void MoveToProcessing()
    {
      this.State = MemoryForensicsExe.MemForensicsState.Processing;
      this.timeInCurrentState = 0.0f;
      this.processingTimeThisBatch = (float) (2.0 + ((double) Utils.randm(2f) - 1.0));
      this.PanelScroll = Vector2.Zero;
      this.IsDisplayingImages = false;
    }

    public void RenderMainDisplay(Rectangle dest, SpriteBatch sb)
    {
      dest = this.RenderMainDisplayHeaders(dest, sb);
      switch (this.State)
      {
        case MemoryForensicsExe.MemForensicsState.Error:
          PatternDrawer.draw(dest, 1f, Color.Transparent, Utils.AddativeRed, this.spriteBatch, PatternDrawer.errorTile);
          Rectangle rectangle1 = new Rectangle(dest.X, dest.Y + dest.Height / 3, dest.Width, 60);
          sb.Draw(Utils.white, rectangle1, Color.Black * 0.4f);
          TextItem.doFontLabelToSize(rectangle1, LocaleTerms.Loc("ERROR"), GuiData.font, Color.White, true, true);
          Rectangle rectangle2 = new Rectangle(dest.X, rectangle1.Y + rectangle1.Height + 2, dest.Width, dest.Height / 2);
          sb.Draw(Utils.white, rectangle2, Color.Black * 0.8f);
          string text = Utils.SuperSmartTwimForWidth(this.ErrorMessage.Length > 500 ? this.ErrorMessage.Substring(0, 500) : this.ErrorMessage, rectangle2.Width, GuiData.smallfont);
          TextItem.doFontLabelToSize(rectangle2, text, GuiData.smallfont, Utils.AddativeRed, true, true);
          if (!Button.doButton(381023801, dest.X + 2, rectangle2.Y + rectangle2.Height + 2, this.bounds.Width / 3, 30, LocaleTerms.Loc("Close"), new Color?(this.os.lockedColor)))
            break;
          this.isExiting = true;
          break;
        case MemoryForensicsExe.MemForensicsState.ReadingFile:
          Rectangle bounds = new Rectangle(dest.X, dest.Y, (int) ((double) dest.Width * ((double) this.timeInCurrentState / 2.0)), dest.Height);
          bounds = new Rectangle(dest.X + (int) ((double) dest.Width * (1.0 - (double) this.timeInCurrentState / 2.0)), dest.Y, (int) ((double) dest.Width * ((double) this.timeInCurrentState / 2.0)), dest.Height);
          this.GridEffect.RenderGrid(bounds, sb, this.ThemeColorDark * 0.5f, this.ThemeColorMain, this.ThemeColorDark, false);
          break;
        case MemoryForensicsExe.MemForensicsState.Main:
          this.DrawMainStateBackground(dest, sb);
          this.RenderMenuMainState(dest, sb);
          break;
        case MemoryForensicsExe.MemForensicsState.Processing:
          this.DrawMainStateBackground(dest, sb);
          this.RenderResultsDisplayMainState(dest, sb, true);
          break;
        case MemoryForensicsExe.MemForensicsState.DisplayingSolution:
          this.DrawMainStateBackground(dest, sb);
          this.RenderResultsDisplayMainState(dest, sb, false);
          break;
      }
    }

    private Rectangle RenderMainDisplayHeaders(Rectangle dest, SpriteBatch sb)
    {
      if (Button.doButton(381023001, dest.X + 3, dest.Y + 2, dest.Width / 2, 25, LocaleTerms.Loc("Close Display"), new Color?(Color.Gray)))
        this.DisplayOverrideIsActive = false;
      dest.Y += 30;
      dest.Height -= 30;
      return dest;
    }

    private void DrawMainStateBackground(Rectangle dest, SpriteBatch sb)
    {
      this.GridEffect.RenderGrid(dest, sb, this.ThemeColorLight, this.ThemeColorMain, this.ThemeColorDark, true);
    }

    private void RenderMenuMainState(Rectangle dest, SpriteBatch sb)
    {
      int num1 = 5;
      int height = Math.Min(dest.Height / (num1 + 1), 32);
      int num2 = (int) ((double) (dest.Height - height * (num1 + 1)) / 2.0);
      bool flag = this.State == MemoryForensicsExe.MemForensicsState.Processing;
      Color color = flag ? Color.Gray : Color.White;
      Rectangle dest1 = new Rectangle(dest.X + 20, dest.Y + num2, (int) ((double) dest.Width * 0.899999976158142), height);
      Rectangle destinationRectangle = new Rectangle(dest.X + 2, dest1.Y - 4, dest.Width - 4, dest1.Height * (num1 + 1));
      sb.Draw(Utils.white, destinationRectangle, Color.Black * 0.8f);
      if (height <= 6)
        return;
      TextItem.doFontLabelToSize(dest1, LocaleTerms.Loc("Memory Dump") + " : " + this.filenameLoaded, GuiData.smallfont, Color.LightGray, true, true);
      dest1.Y += dest1.Height + 2;
      if (Button.doButton(381023801, dest1.X, dest1.Y, dest1.Width, dest1.Height, LocaleTerms.Loc("Process Recent Commands Run..."), new Color?(color)) && !flag)
      {
        this.MoveToProcessing();
        this.OutputData.Clear();
        this.OutputData.AddRange((IEnumerable<string>) this.ActiveMem.CommandsRun);
        this.AnnouncementData = LocaleTerms.Loc("Results for recently run commands remaining in cached memory") + "::";
      }
      dest1.Y += dest1.Height + 2;
      if (Button.doButton(381023803, dest1.X, dest1.Y, dest1.Width, dest1.Height, LocaleTerms.Loc("Process Files in Memory..."), new Color?(color)) && !flag)
      {
        this.MoveToProcessing();
        this.OutputData.Clear();
        this.OutputData.AddRange((IEnumerable<string>) this.ActiveMem.DataBlocks);
        this.AnnouncementData = LocaleTerms.Loc("Results for accessed non-binary file fragments in cached memory") + "::";
      }
      dest1.Y += dest1.Height + 2;
      if (Button.doButton(381023807, dest1.X, dest1.Y, dest1.Width, dest1.Height, LocaleTerms.Loc("Process Images in Memory..."), new Color?(color)) && !flag)
      {
        this.MoveToProcessing();
        this.IsDisplayingImages = true;
        this.OutputData.Clear();
        this.OutputData.AddRange((IEnumerable<string>) this.ActiveMem.Images);
        Task.Factory.StartNew((Action) (() => this.StartLoadingInTexturesForMemory()));
        this.AnnouncementData = LocaleTerms.Loc("Results for accessed image-type tagged binary fragments in cached memory") + "::";
      }
      dest1.Y += dest1.Height + 2;
      if (Button.doButton(381023809, dest1.X, dest1.Y, dest1.Width, dest1.Height, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
        this.isExiting = true;
      dest1.Y += dest1.Height + 2;
    }

    private void RenderResultsDisplayMainState(Rectangle dest, SpriteBatch sb, bool isProcessing)
    {
      Color color = new Color(5, 5, 5, 230);
      Rectangle rectangle1 = new Rectangle(dest.X + 22, dest.Y + 30, dest.Width - 44, 40);
      sb.Draw(Utils.white, rectangle1, color);
      rectangle1.X += 10;
      TextItem.doFontLabelToSize(rectangle1, isProcessing ? LocaleTerms.Loc("PROCESSING") : LocaleTerms.Loc("OUTPUT"), GuiData.font, Color.White, true, true);
      rectangle1.X -= 10;
      int width = 200;
      int height = 30;
      if (Button.doButton(381023909, rectangle1.X + rectangle1.Width - (width + 6), rectangle1.Y + (rectangle1.Height / 2 - height / 2), width, height, isProcessing ? LocaleTerms.Loc("Cancel") : LocaleTerms.Loc("Return to Menu"), new Color?(Color.White)))
      {
        this.State = MemoryForensicsExe.MemForensicsState.Main;
        this.timeInCurrentState = 0.0f;
      }
      rectangle1.Y += rectangle1.Height;
      rectangle1.Height = 1;
      sb.Draw(Utils.white, rectangle1, Color.White);
      if (isProcessing)
      {
        Rectangle destinationRectangle = new Rectangle(dest.X + 2, rectangle1.Y + rectangle1.Height + 2, dest.Width - 4, 20);
        sb.Draw(Utils.white, destinationRectangle, Color.Black);
        float num = this.timeInCurrentState / this.processingTimeThisBatch;
        destinationRectangle.Width = (int) ((double) destinationRectangle.Width * (double) num);
        sb.Draw(Utils.white, destinationRectangle, Utils.makeColorAddative(Color.LightBlue));
      }
      else
      {
        Rectangle rectangle2 = new Rectangle(rectangle1.X, rectangle1.Y + rectangle1.Height + 2, rectangle1.Width, 1);
        rectangle2.Height = dest.Y + dest.Height - (rectangle1.Y + rectangle1.Height + 4);
        sb.Draw(Utils.white, rectangle2, color);
        if (this.OutputData.Count <= 0)
        {
          TextItem.doFontLabelToSize(rectangle2, " - " + LocaleTerms.Loc("No Valid Matches Found") + " - ", GuiData.smallfont, Color.White, true, false);
        }
        else
        {
          Rectangle drawbounds = new Rectangle(0, 0, rectangle2.Width, rectangle2.Height);
          float num1 = 10f;
          for (int index = 0; index < this.OutputData.Count; ++index)
          {
            if (this.IsDisplayingImages)
            {
              if (this.OutputTextures.Count >= index)
                num1 += (float) (Math.Min(drawbounds.Width - 16, this.OutputTextures[index].Height) + 30);
              else
                break;
            }
            else
              num1 += 26f + GuiData.smallfont.MeasureString(Utils.SuperSmartTwimForWidth(this.OutputData[index], drawbounds.Width - 16, GuiData.smallfont)).Y;
          }
          drawbounds.Height = (int) num1;
          ScrollablePanel.beginPanel(3371001, drawbounds, this.PanelScroll);
          Vector2 pos = new Vector2((float) (drawbounds.X + 14), (float) drawbounds.Y);
          TextItem.doMeasuredFontLabel(pos, this.AnnouncementData, GuiData.tinyfont, new Color?(Color.White), float.MaxValue, float.MaxValue);
          pos.Y += 20f;
          Rectangle destinationRectangle1 = new Rectangle(drawbounds.X, (int) pos.Y, drawbounds.Width, 1);
          GuiData.spriteBatch.Draw(Utils.white, destinationRectangle1, Color.White);
          pos.Y += 20f;
          for (int index = 0; index < this.OutputData.Count; ++index)
          {
            if (this.IsDisplayingImages)
            {
              if (this.OutputTextures.Count >= index)
              {
                Rectangle dest1 = new Rectangle((int) pos.X + 8, (int) pos.Y, drawbounds.Width - 16, drawbounds.Width - 16);
                dest1.Height = Math.Min(dest1.Height, this.OutputTextures[index].Height);
                Rectangle rectangle3 = Utils.DrawSpriteAspectCorrect(dest1, GuiData.spriteBatch, this.OutputTextures[index], Color.White, false);
                float num2 = (float) (rectangle3.Y - dest1.Y + rectangle3.Height);
                pos.Y += num2 + 12f;
              }
              else
                break;
            }
            else
            {
              string text = Utils.SuperSmartTwimForWidth(this.OutputData[index], drawbounds.Width - 16, GuiData.smallfont);
              Vector2 vector2 = TextItem.doMeasuredFontLabel(pos, text, GuiData.smallfont, new Color?(Color.White * 0.9f), float.MaxValue, float.MaxValue);
              Rectangle destinationRectangle2 = new Rectangle(drawbounds.X + 6, (int) pos.Y + (int) (((double) vector2.Y - 3.0) / 2.0), 3, 3);
              GuiData.spriteBatch.Draw(Utils.white, destinationRectangle2, Color.Gray);
              pos.Y += vector2.Y + 6f;
            }
            destinationRectangle1 = new Rectangle(drawbounds.X, (int) pos.Y, rectangle2.Width, 1);
            GuiData.spriteBatch.Draw(Utils.white, destinationRectangle1, Color.Gray);
            pos.Y += 10f;
          }
          this.PanelScroll = ScrollablePanel.endPanel(3371001, this.PanelScroll, rectangle2, 2000f, false);
          float num3 = num1 - (float) rectangle2.Height;
          if ((double) this.PanelScroll.Y > (double) num3)
            this.PanelScroll.Y = num3;
        }
      }
    }

    public enum MemForensicsState
    {
      Error,
      SelectingFile,
      ReadingFile,
      Main,
      Processing,
      DisplayingSolution,
    }
  }
}
