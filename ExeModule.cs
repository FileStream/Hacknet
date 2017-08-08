// Decompiled with JetBrains decompiler
// Type: Hacknet.ExeModule
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  internal class ExeModule : Module
  {
    public static float FADEOUT_RATE = 0.5f;
    public static float MOVE_UP_RATE = 350f;
    public static int DEFAULT_RAM_COST = 246;
    public int PID = 0;
    public float fade = 1f;
    public bool isExiting = false;
    public bool needsRemoval = false;
    public float moveUpBy = 0.0f;
    public int ramCost = ExeModule.DEFAULT_RAM_COST;
    private int baseRamCost = 0;
    public string targetIP = "";
    public bool needsProxyAccess = false;
    public string IdentifierName = "UNKNOWN";

    public ExeModule(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.targetIP = operatingSystem.connectedComp == null ? operatingSystem.thisComputer.ip : operatingSystem.connectedComp.ip;
      bool flag;
      byte randomByte;
      do
      {
        flag = false;
        randomByte = Utils.getRandomByte();
        for (int index = 0; index < this.os.exes.Count; ++index)
        {
          if (this.os.exes[index].PID == (int) randomByte)
          {
            flag = true;
            break;
          }
        }
      }
      while (flag);
      this.os.currentPID = (int) randomByte;
      this.PID = (int) randomByte;
      this.bounds = location;
    }

    public override void LoadContent()
    {
      this.bounds.Height = this.ramCost;
    }

    public override void Update(float t)
    {
      this.bounds.Height = this.ramCost;
      if (this.isExiting)
      {
        if ((double) this.fade >= 1.0)
          this.baseRamCost = this.ramCost;
        this.ramCost = (int) ((double) this.baseRamCost * (double) this.fade);
        this.bounds.Height = this.ramCost;
        this.fade -= t * ExeModule.FADEOUT_RATE;
        if ((double) this.fade <= 0.0)
          this.needsRemoval = true;
      }
      if ((double) this.moveUpBy < 0.0)
        return;
      int num = (int) ((double) ExeModule.MOVE_UP_RATE * (double) t);
      this.bounds.Y -= num;
      this.moveUpBy -= (float) num;
      this.bounds.Y = Math.Max(this.bounds.Y, this.os.ram.bounds.Y + RamModule.contentStartOffset);
    }

    public override void Draw(float t)
    {
    }

    public virtual void Completed()
    {
    }

    public virtual void Killed()
    {
    }

    public virtual void drawOutline()
    {
      Rectangle bounds = this.bounds;
      RenderedRectangle.doRectangleOutline(bounds.X, bounds.Y, bounds.Width, bounds.Height, 1, new Color?(this.os.moduleColorSolid));
      ++bounds.X;
      ++bounds.Y;
      bounds.Width -= 2;
      bounds.Height -= 2;
      this.spriteBatch.Draw(Utils.white, bounds, this.os.moduleColorBacking * this.fade);
    }

    public virtual void drawTarget(string typeName = "app:")
    {
      if (this.bounds.Height <= 14)
        return;
      string text = "IP: " + this.targetIP;
      Rectangle bounds = this.bounds;
      bounds.Height = Math.Min(this.bounds.Height, 14);
      ++bounds.X;
      bounds.Width -= 2;
      this.spriteBatch.Draw(Utils.white, bounds, this.os.exeModuleTopBar);
      RenderedRectangle.doRectangleOutline(bounds.X, bounds.Y, bounds.Width, bounds.Height, 1, new Color?(this.os.topBarColor));
      if (bounds.Height >= 14)
      {
        Vector2 vector2 = GuiData.detailfont.MeasureString(text);
        this.spriteBatch.DrawString(GuiData.detailfont, text, new Vector2((float) (this.bounds.X + this.bounds.Width) - vector2.X, (float) this.bounds.Y), this.os.exeModuleTitleText);
        this.spriteBatch.DrawString(GuiData.detailfont, typeName + this.IdentifierName, new Vector2((float) (this.bounds.X + 2), (float) this.bounds.Y), this.os.exeModuleTitleText);
      }
    }

    public Rectangle GetContentAreaDest()
    {
      return new Rectangle(this.bounds.X + 1, this.bounds.Y + Module.PANEL_HEIGHT, this.bounds.Width - 2, this.bounds.Height - Module.PANEL_HEIGHT - 1);
    }
  }
}
