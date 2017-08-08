// Decompiled with JetBrains decompiler
// Type: Hacknet.CoreModule
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
  internal class CoreModule : Module
  {
    public bool inputLocked = false;
    private bool guiInputLockStatus = false;
    private static Texture2D LockSprite;

    public CoreModule(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
    }

    public override void LoadContent()
    {
      base.LoadContent();
      if (CoreModule.LockSprite != null)
        return;
      CoreModule.LockSprite = this.os.content.Load<Texture2D>("Lock");
    }

    public override void PreDrawStep()
    {
      base.PreDrawStep();
      if (!this.inputLocked)
        return;
      this.guiInputLockStatus = GuiData.blockingInput;
      GuiData.blockingInput = true;
    }

    public override void PostDrawStep()
    {
      base.PostDrawStep();
      if (!this.inputLocked)
        return;
      GuiData.blockingInput = false;
      GuiData.blockingInput = this.guiInputLockStatus;
      Rectangle bounds = this.bounds;
      if (bounds.Contains(GuiData.getMousePoint()))
      {
        GuiData.spriteBatch.Draw(Utils.white, bounds, Color.Gray * 0.5f);
        Vector2 position = new Vector2((float) (bounds.X + bounds.Width / 2 - CoreModule.LockSprite.Width / 2), (float) (bounds.Y + bounds.Height / 2 - CoreModule.LockSprite.Height / 2));
        GuiData.spriteBatch.Draw(CoreModule.LockSprite, position, Color.White);
      }
    }
  }
}
