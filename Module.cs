// Decompiled with JetBrains decompiler
// Type: Hacknet.Module
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
  internal class Module
  {
    public static int PANEL_HEIGHT = 15;
    public string name = "Unknown";
    public bool visible = true;
    public Rectangle bounds;
    public SpriteBatch spriteBatch;
    public OS os;
    private static Rectangle tmpRect;

    public Rectangle Bounds
    {
      get
      {
        return this.bounds;
      }
      set
      {
        this.bounds = value;
        this.bounds.Y += Module.PANEL_HEIGHT;
        this.bounds.Height -= Module.PANEL_HEIGHT;
      }
    }

    public Module(Rectangle location, OS operatingSystem)
    {
      location.Y += Module.PANEL_HEIGHT;
      location.Height -= Module.PANEL_HEIGHT;
      this.bounds = location;
      this.os = operatingSystem;
      this.spriteBatch = this.os.ScreenManager.SpriteBatch;
    }

    public virtual void LoadContent()
    {
    }

    public virtual void Update(float t)
    {
    }

    public virtual void PreDrawStep()
    {
    }

    public virtual void Draw(float t)
    {
      this.drawFrame();
    }

    public virtual void PostDrawStep()
    {
    }

    public void drawFrame()
    {
      Module.tmpRect = this.bounds;
      Module.tmpRect.Y -= Module.PANEL_HEIGHT;
      Module.tmpRect.Height += Module.PANEL_HEIGHT;
      this.spriteBatch.Draw(Utils.white, Module.tmpRect, this.os.moduleColorBacking);
      RenderedRectangle.doRectangleOutline(Module.tmpRect.X, Module.tmpRect.Y, Module.tmpRect.Width, Module.tmpRect.Height, 1, new Color?(this.os.moduleColorSolid));
      Module.tmpRect.Height = Module.PANEL_HEIGHT;
      this.spriteBatch.Draw(Utils.white, Module.tmpRect, this.os.moduleColorStrong);
      this.spriteBatch.DrawString(GuiData.detailfont, this.name, new Vector2((float) (Module.tmpRect.X + 2), (float) (Module.tmpRect.Y + 2)), this.os.semiTransText);
      Module.tmpRect = this.bounds;
      Module.tmpRect.Y -= Module.PANEL_HEIGHT;
      Module.tmpRect.Height += Module.PANEL_HEIGHT;
      RenderedRectangle.doRectangleOutline(Module.tmpRect.X, Module.tmpRect.Y, Module.tmpRect.Width, Module.tmpRect.Height, 1, new Color?(this.os.moduleColorSolid));
    }
  }
}
