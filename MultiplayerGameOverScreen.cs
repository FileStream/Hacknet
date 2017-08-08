// Decompiled with JetBrains decompiler
// Type: Hacknet.MultiplayerGameOverScreen
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet
{
  internal class MultiplayerGameOverScreen : GameScreen
  {
    private Rectangle contentRect;
    private int screenWidth;
    private int screenHeight;
    private bool isWinner;
    private Color winBacking;
    private Color winPattern;
    private Color lossBacking;
    private Color lossPattern;
    private SpriteFont font;

    public MultiplayerGameOverScreen(bool winner)
    {
      this.isWinner = winner;
      this.TransitionOnTime = TimeSpan.FromSeconds(3.5);
      this.TransitionOffTime = TimeSpan.FromSeconds(0.200000002980232);
      this.IsPopup = true;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.winBacking = new Color(5, 30, 8);
      this.lossBacking = new Color(34, 8, 5);
      this.winPattern = new Color(11, 66, 23);
      this.lossPattern = new Color(86, 11, 11);
      this.screenWidth = this.ScreenManager.GraphicsDevice.Viewport.Width;
      this.screenHeight = this.ScreenManager.GraphicsDevice.Viewport.Height;
      this.contentRect = new Rectangle(0, this.screenHeight / 6, this.screenWidth, this.screenHeight - this.screenHeight / 3);
      this.font = this.ScreenManager.Game.Content.Load<SpriteFont>("Kremlin");
    }

    public override void HandleInput(InputState input)
    {
      base.HandleInput(input);
      GuiData.doInput(input);
    }

    public override void Draw(GameTime gameTime)
    {
      base.Draw(gameTime);
      this.ScreenManager.FadeBackBufferToBlack((int) Math.Min(this.TransitionAlpha, (byte) 150));
      GuiData.startDraw();
      float num = 1f - this.TransitionPosition;
      PatternDrawer.draw(this.contentRect, 1f, (this.isWinner ? this.winBacking : this.lossBacking) * num, (this.isWinner ? this.winPattern : this.lossPattern) * num, GuiData.spriteBatch);
      string text = this.isWinner ? "VICTORY" : "DEFEAT";
      Vector2 pos = this.font.MeasureString(text);
      pos.X = (float) (this.contentRect.X + this.contentRect.Width / 2) - pos.X / 2f;
      pos.Y = (float) (this.contentRect.Y + this.contentRect.Height / 2) - pos.Y / 2f;
      TextItem.DrawShadow = false;
      TextItem.doFontLabel(pos, text, this.font, new Color?(Color.White * num), float.MaxValue, float.MaxValue, false);
      if (Button.doButton(1008, this.contentRect.X + 10, this.contentRect.Y + this.contentRect.Height - 60, 230, 55, LocaleTerms.Loc("Exit"), new Color?(Color.Black)))
      {
        if (OS.currentInstance != null)
          OS.currentInstance.ExitScreen();
        this.ExitScreen();
        this.ScreenManager.AddScreen((GameScreen) new MainMenu());
      }
      GuiData.endDraw();
    }
  }
}
