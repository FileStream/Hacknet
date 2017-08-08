// Decompiled with JetBrains decompiler
// Type: Hacknet.MessageBoxScreen
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet
{
  internal class MessageBoxScreen : GameScreen
  {
    public static float HEIGHT_BUFFER = 20f;
    private bool hasEscGuide = false;
    public string OverrideAcceptedText = (string) null;
    public string OverrideCancelText = (string) null;
    private string message;
    private Texture2D top;
    private Texture2D mid;
    private Texture2D bottom;
    private Texture2D inputGuide;
    private SpriteFont guideFont;
    private Rectangle contentBounds;
    private Vector2 topLeft;
    public Action AcceptedClicked;
    public Action CancelClicked;

    public event EventHandler<PlayerIndexEventArgs> Accepted;

    public event EventHandler<PlayerIndexEventArgs> Cancelled;

    public MessageBoxScreen(string message)
      : this(message, false)
    {
    }

    public MessageBoxScreen(string message, bool includeUsageText)
    {
      this.message = !includeUsageText ? message : message + "\nA button, Space, Enter = ok\nB button, Esc = cancel";
      this.IsPopup = true;
      this.TransitionOnTime = TimeSpan.FromSeconds(0.2);
      this.TransitionOffTime = TimeSpan.FromSeconds(0.05);
    }

    public MessageBoxScreen(string message, bool includesUsageText, bool hasEscPrompt)
      : this(message, false)
    {
      this.hasEscGuide = hasEscPrompt;
    }

    public override void LoadContent()
    {
      ContentManager content = this.ScreenManager.Game.Content;
      this.top = TextureBank.load("PopupFrame", content);
      this.mid = TextureBank.load("PopupFrame", content);
      this.bottom = !this.ScreenManager.usingGamePad ? TextureBank.load("PopupBase", content) : TextureBank.load("PopupBase", content);
      float num = GuiData.font.MeasureString(this.message).Y + MessageBoxScreen.HEIGHT_BUFFER;
      this.topLeft = new Vector2((float) ((double) this.ScreenManager.GraphicsDevice.Viewport.Width / 2.0 - (double) this.top.Width / 2.0), (float) ((double) this.ScreenManager.GraphicsDevice.Viewport.Height / 2.0 - (double) num / 2.0) - (float) this.top.Height);
      this.contentBounds = new Rectangle((int) this.topLeft.X, (int) this.topLeft.Y + this.top.Height, this.top.Width, (int) num);
      if (!this.hasEscGuide)
        return;
      this.guideFont = GuiData.font;
    }

    public override void HandleInput(InputState input)
    {
      PlayerIndex playerIndex;
      if (input.IsMenuSelect(this.ControllingPlayer, out playerIndex))
      {
        if (this.Accepted != null)
          this.Accepted((object) this, new PlayerIndexEventArgs(playerIndex));
        if (this.AcceptedClicked != null)
          this.AcceptedClicked();
        this.ExitScreen();
      }
      else if (input.IsMenuCancel(this.ControllingPlayer, out playerIndex))
      {
        if (this.Cancelled != null)
          this.Cancelled((object) this, new PlayerIndexEventArgs(playerIndex));
        if (this.CancelClicked != null)
          this.CancelClicked();
        this.ExitScreen();
      }
      GuiData.doInput(input);
    }

    public override void Draw(GameTime gameTime)
    {
      SpriteBatch spriteBatch = this.ScreenManager.SpriteBatch;
      SpriteFont font = GuiData.font;
      this.ScreenManager.FadeBackBufferToBlack((int) Math.Min(this.TransitionAlpha, (byte) 140));
      Viewport viewport = this.ScreenManager.GraphicsDevice.Viewport;
      Vector2 position = new Vector2((float) viewport.Width, (float) viewport.Height) / 2f - font.MeasureString(this.message) / 2f;
      Color color = new Color(22, 22, 22, (int) byte.MaxValue);
      float num = 1f - this.TransitionPosition;
      if (this.ScreenState == ScreenState.TransitionOff)
        return;
      spriteBatch.Begin();
      spriteBatch.Draw(this.top, this.topLeft, color);
      spriteBatch.Draw(this.mid, this.contentBounds, color);
      spriteBatch.Draw(this.top, this.topLeft + new Vector2(0.0f, (float) (this.top.Height + this.contentBounds.Height)), color);
      spriteBatch.DrawString(font, this.message, position, Color.White);
      int width = 150;
      int x1 = this.contentBounds.X + this.contentBounds.Width - (width + 5);
      int y = this.contentBounds.Y + this.contentBounds.Height + this.top.Height - 40;
      string input1 = this.OverrideCancelText == null ? "Resume" : this.OverrideCancelText;
      if (Button.doButton(331, x1, y, width, 27, LocaleTerms.Loc(input1), new Color?()))
      {
        if (this.Cancelled != null)
          this.Cancelled((object) this, new PlayerIndexEventArgs(this.ScreenManager.controllingPlayer));
        if (this.CancelClicked != null)
          this.CancelClicked();
        this.ExitScreen();
      }
      int x2 = this.contentBounds.X + 5;
      string input2 = this.OverrideAcceptedText == null ? "Quit Hacknet" : this.OverrideAcceptedText;
      if (Button.doButton(332, x2, y, width, 27, LocaleTerms.Loc(input2), new Color?()))
      {
        if (this.Accepted != null)
          this.Accepted((object) this, new PlayerIndexEventArgs(this.ScreenManager.controllingPlayer));
        if (this.AcceptedClicked != null)
          this.AcceptedClicked();
        this.ExitScreen();
      }
      spriteBatch.End();
    }

    public override void inputMethodChanged(bool usingGamePad)
    {
    }
  }
}
