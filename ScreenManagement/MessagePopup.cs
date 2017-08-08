// Decompiled with JetBrains decompiler
// Type: Hacknet.ScreenManagement.MessagePopup
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Hacknet.ScreenManagement
{
  internal class MessagePopup : GameScreen
  {
    private static int BOX_WIDTH = 500;
    private static int BOX_HEIGHT = 350;
    private Texture2D blankTexture = (Texture2D) null;
    private string messageContent = "";

    public MessagePopup(string message)
    {
      this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
      this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
      this.messageContent = message;
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
    }

    public override void HandleInput(InputState input)
    {
      KeyboardState state = Keyboard.GetState();
      if (!state.IsKeyDown(Keys.Space) && !state.IsKeyDown(Keys.Up) && (!state.IsKeyDown(Keys.Z) && !state.IsKeyDown(Keys.Left)) && !state.IsKeyDown(Keys.Right) && !state.IsKeyDown(Keys.X))
        return;
      this.ExitScreen();
    }

    public override void Draw(GameTime gameTime)
    {
      SpriteBatch spriteBatch = this.ScreenManager.SpriteBatch;
      Viewport viewport = this.ScreenManager.GraphicsDevice.Viewport;
      Rectangle rectangle = new Rectangle(0, 0, viewport.Width, viewport.Height);
      Rectangle destinationRectangle = new Rectangle(viewport.Width / 2 - MessagePopup.BOX_WIDTH / 2, viewport.Height / 2 - MessagePopup.BOX_HEIGHT / 2, MessagePopup.BOX_WIDTH, MessagePopup.BOX_HEIGHT);
      byte transitionAlpha = this.TransitionAlpha;
      spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
      spriteBatch.Draw(Utils.white, destinationRectangle, new Color((int) transitionAlpha, (int) transitionAlpha, (int) transitionAlpha));
      SpriteFont font = this.ScreenManager.Font;
      Vector2 origin = new Vector2(0.0f, (float) (font.LineSpacing / 2));
      Vector2 position = new Vector2((float) destinationRectangle.X, (float) destinationRectangle.Y);
      spriteBatch.DrawString(font, this.messageContent, position, Color.White, 0.0f, origin, 1f, SpriteEffects.None, 0.0f);
      spriteBatch.End();
    }

    public override void UnloadContent()
    {
    }
  }
}
