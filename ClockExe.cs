// Decompiled with JetBrains decompiler
// Type: Hacknet.ClockExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  internal class ClockExe : ExeModule
  {
    public ClockExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.name = "Clock";
      this.ramCost = 60;
      this.IdentifierName = "Clock";
      this.targetIP = this.os.thisComputer.ip;
      AchievementsManager.Unlock("clock_run", false);
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      DateTime now = DateTime.Now;
      TextItem.doFontLabel(new Vector2((float) (this.bounds.X + 2), (float) (this.bounds.Y + 12)), (now.Hour % 12).ToString("00") + " : " + now.Minute.ToString("00") + " : " + now.Second.ToString("00"), GuiData.titlefont, new Color?(RamModule.USED_RAM_COLOR), (float) (this.bounds.Width - 34), (float) (this.bounds.Height - 10), true);
      TextItem.doFontLabel(new Vector2((float) (this.bounds.X + this.bounds.Width - 28), (float) (this.bounds.Y + this.bounds.Height - 38)), now.Hour > 12 ? "PM" : "AM", GuiData.titlefont, new Color?(RamModule.USED_RAM_COLOR), 30f, 26f, false);
      int width = this.bounds.Width - 2;
      Rectangle destinationRectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + this.bounds.Height - 1 - 6, width, 1);
      float num1 = (float) now.Millisecond / 1000f;
      float num2 = 0.0f;
      if ((double) num1 < 0.5)
        num2 = (float) (1.0 - (double) num1 * 2.0);
      this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.moduleColorSolidDefault * 0.2f * num2);
      destinationRectangle.Width = (int) ((double) width * (double) num1);
      this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.moduleColorSolidDefault * 0.2f);
      float num3 = ((float) now.Second + num1) / 60f;
      destinationRectangle.Width = (int) ((double) width * (double) num3);
      destinationRectangle.Y += 2;
      this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.moduleColorStrong);
      float num4 = (float) now.Minute / 60f;
      float num5 = (float) now.Hour / 60f;
      destinationRectangle.Width = (int) ((double) width * (double) num4);
      destinationRectangle.Y += 2;
      this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.moduleColorSolid);
    }
  }
}
