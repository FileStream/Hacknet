// Decompiled with JetBrains decompiler
// Type: Hacknet.RamModule
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  internal class RamModule : CoreModule
  {
    public static int contentStartOffset = 16;
    public static int MODULE_WIDTH = 252;
    public static Color USED_RAM_COLOR = new Color(60, 60, 67);
    public static float FLASH_TIME = 3f;
    private string infoString = "";
    private float OutOfMemoryFlashTime = 0.0f;
    private Vector2 infoStringPos;
    private Rectangle infoBar;
    private Rectangle infoBarUsedRam;

    public RamModule(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.infoBar = new Rectangle(this.bounds.X + 1, this.bounds.Y + 1, this.bounds.Width - 2, RamModule.contentStartOffset);
      this.infoBarUsedRam = new Rectangle(this.bounds.X + 1, this.bounds.Y + 1, this.bounds.Width - 2, RamModule.contentStartOffset);
      this.infoStringPos = new Vector2((float) this.infoBar.X, (float) this.infoBar.Y);
    }

    public override void Update(float t)
    {
      base.Update(t);
      this.infoBar = new Rectangle(this.bounds.X + 1, this.bounds.Y + 1, this.bounds.Width - 2, RamModule.contentStartOffset);
      this.infoString = "USED RAM: " + (object) (this.os.totalRam - this.os.ramAvaliable) + "mb / " + (object) this.os.totalRam + "mb";
      this.infoBarUsedRam = new Rectangle(this.bounds.X + 1, this.bounds.Y + 1, this.bounds.Width - 2, RamModule.contentStartOffset);
      if ((double) this.OutOfMemoryFlashTime <= 0.0)
        return;
      this.OutOfMemoryFlashTime -= t;
    }

    public void FlashMemoryWarning()
    {
      this.OutOfMemoryFlashTime = RamModule.FLASH_TIME;
      for (int index = 0; index < this.os.exes.Count; ++index)
      {
        NotesExe ex = this.os.exes[index] as NotesExe;
        if (ex != null)
          ex.DisplayOutOfMemoryWarning();
      }
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.spriteBatch.Draw(Utils.white, this.infoBar, this.os.indentBackgroundColor);
      this.infoBarUsedRam.Width = (int) ((double) this.infoBar.Width * (1.0 - (double) this.os.ramAvaliable / (double) this.os.totalRam));
      this.spriteBatch.Draw(Utils.white, this.infoBarUsedRam, RamModule.USED_RAM_COLOR);
      this.spriteBatch.DrawString(GuiData.detailfont, this.infoString, new Vector2((float) this.infoBar.X, (float) this.infoBar.Y), Color.White);
      this.spriteBatch.DrawString(GuiData.detailfont, string.Concat((object) this.os.exes.Count), new Vector2((float) (this.bounds.X + this.bounds.Width - (this.os.exes.Count >= 10 ? 24 : 12)), (float) this.infoBar.Y), Color.White);
      if ((double) this.OutOfMemoryFlashTime <= 0.0)
        return;
      float num = Math.Min(1f, this.OutOfMemoryFlashTime);
      float amount = Math.Max(0.0f, this.OutOfMemoryFlashTime - (RamModule.FLASH_TIME - 1f));
      PatternDrawer.draw(this.bounds, 0.0f, Color.Transparent, Color.Lerp(this.os.lockedColor, Utils.AddativeRed, amount) * num, this.spriteBatch, PatternDrawer.errorTile);
      int height = 40;
      Rectangle rectangle = new Rectangle(this.bounds.X, this.bounds.Y + this.bounds.Height - height - 1, this.bounds.Width, height);
      this.spriteBatch.Draw(Utils.white, Utils.InsetRectangle(rectangle, 4), Color.Black * 0.75f);
      --rectangle.X;
      string text = " ^ " + LocaleTerms.Loc("INSUFFICIENT MEMORY") + " ^ ";
      TextItem.doFontLabelToSize(rectangle, text, GuiData.font, Color.Black * num, false, false);
      rectangle.X += 2;
      TextItem.doFontLabelToSize(rectangle, text, GuiData.font, Color.Black * num, false, false);
      --rectangle.X;
      TextItem.doFontLabelToSize(rectangle, text, GuiData.font, Color.White * num, false, false);
    }

    public virtual void drawOutline()
    {
      Rectangle bounds = this.bounds;
      this.spriteBatch.Draw(Utils.white, bounds, this.os.outlineColor);
      ++bounds.X;
      ++bounds.Y;
      bounds.Width -= 2;
      bounds.Height -= 2;
      this.spriteBatch.Draw(Utils.white, bounds, this.os.darkBackgroundColor);
    }
  }
}
