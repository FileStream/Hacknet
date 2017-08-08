// Decompiled with JetBrains decompiler
// Type: Hacknet.DemoEndScreen
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Hacknet
{
  public class DemoEndScreen : GameScreen
  {
    public bool StopsMusic = true;
    public bool IsDLCDemoScreen = false;
    private double timeOnThisScreen = 0.0;
    private PointGatherEffect pointEffect = new PointGatherEffect();
    private Rectangle Fullscreen;
    private HexGridBackground HexBackground;

    public override void LoadContent()
    {
      base.LoadContent();
      int x = 0;
      int y = 0;
      Viewport viewport = this.ScreenManager.GraphicsDevice.Viewport;
      int width = viewport.Width;
      viewport = this.ScreenManager.GraphicsDevice.Viewport;
      int height = viewport.Height;
      this.Fullscreen = new Rectangle(x, y, width, height);
      if (this.StopsMusic)
        MediaPlayer.Stop();
      PostProcessor.EndingSequenceFlashOutActive = false;
      PostProcessor.EndingSequenceFlashOutPercentageComplete = 0.0f;
      PostProcessor.dangerModeEnabled = false;
      if (!this.IsDLCDemoScreen)
        return;
      this.pointEffect.Init(this.ScreenManager.Game.Content);
      this.pointEffect.GravityConstant = 2E-05f;
      this.pointEffect.GlowScaleMod = 5f;
      this.pointEffect.CircleTex = this.ScreenManager.Game.Content.Load<Texture2D>("NodeCircle");
      this.pointEffect.NodeColor = Utils.AddativeRed * 0.5f;
      this.pointEffect.Explode(300);
      this.pointEffect.timeRemainingWithoutAttract = 2f;
      this.pointEffect.LineTexture = Utils.gradientLeftRight;
      this.pointEffect.LineLengthPercentage = 1f;
      OS.currentInstance.highlightColor = Utils.AddativeRed;
      this.HexBackground = new HexGridBackground(this.ScreenManager.Game.Content);
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
      if (this.IsDLCDemoScreen)
      {
        float totalSeconds = (float) gameTime.ElapsedGameTime.TotalSeconds;
        this.pointEffect.Update(totalSeconds);
        this.HexBackground.Update(totalSeconds);
      }
      if (!otherScreenHasFocus && !coveredByOtherScreen)
        this.timeOnThisScreen += gameTime.ElapsedGameTime.TotalSeconds;
      if (this.timeOnThisScreen > 5.0 && Keyboard.GetState().GetPressedKeys().Length > 0)
      {
        if (Settings.isConventionDemo)
        {
          OS.currentInstance.ScreenManager.AddScreen((GameScreen) new MainMenu());
          OS.currentInstance.ExitScreen();
          OS.currentInstance = (OS) null;
        }
        else
          Game1.getSingleton().Exit();
      }
      else if (this.timeOnThisScreen > (this.IsDLCDemoScreen ? 51.5 : 20.0))
      {
        if (Settings.isConventionDemo)
        {
          OS.currentInstance.ScreenManager.AddScreen((GameScreen) new MainMenu());
          OS.currentInstance.ExitScreen();
          OS.currentInstance = (OS) null;
          this.ExitScreen();
          MainMenu.resetOS();
        }
        else
          Game1.getSingleton().Exit();
      }
      PostProcessor.EndingSequenceFlashOutActive = false;
      PostProcessor.EndingSequenceFlashOutPercentageComplete = 0.0f;
      PostProcessor.dangerModeEnabled = false;
    }

    public override void Draw(GameTime gameTime)
    {
      base.Draw(gameTime);
      PostProcessor.begin();
      GuiData.startDraw();
      GuiData.spriteBatch.Draw(Utils.white, this.Fullscreen, Color.Black);
      if (this.IsDLCDemoScreen)
      {
        this.HexBackground.Draw(Utils.GetFullscreen(), GuiData.spriteBatch, Utils.AddativeRed * 0.2f, Color.Black, HexGridBackground.ColoringAlgorithm.OutlinedSinWash, 0.0f);
        this.pointEffect.Render(Utils.GetFullscreen(), GuiData.spriteBatch);
        GuiData.spriteBatch.Draw(Utils.white, this.Fullscreen, Color.Black * 0.5f);
      }
      Rectangle dest = Utils.InsetRectangle(this.Fullscreen, 200);
      dest.Y = dest.Y + dest.Height / 2 - 200;
      dest.Height = 400;
      Rectangle destinationRectangle = new Rectangle(this.Fullscreen.X, dest.Y + 50, this.Fullscreen.Width, dest.Height - 148);
      GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, Utils.AddativeRed * (0.5f + Utils.randm(0.1f)));
      string text = "HACKNET";
      FlickeringTextEffect.DrawLinedFlickeringText(dest, text, this.IsDLCDemoScreen ? 5f : 18f, this.IsDLCDemoScreen ? 0.8f : 0.7f, GuiData.titlefont, (object) null, Color.White, 6);
      dest.Y += 400;
      dest.Height = 120;
      SpriteFont font = GuiData.titlefont;
      if (Settings.ActiveLocale != "en-us")
        font = GuiData.font;
      string input = this.IsDLCDemoScreen ? "EXPANSION COMING DECEMBER" : "MORE SOON";
      FlickeringTextEffect.DrawFlickeringText(dest, Utils.FlipRandomChars(LocaleTerms.Loc(input), this.IsDLCDemoScreen ? 0.0045 : 0.008), -8f, 0.7f, font, (object) null, Color.Gray);
      FlickeringTextEffect.DrawFlickeringText(dest, Utils.FlipRandomChars(LocaleTerms.Loc(input), 0.03), -8f, 0.7f, font, (object) null, Utils.AddativeWhite * 0.15f);
      GuiData.endDraw();
      PostProcessor.end();
    }
  }
}
