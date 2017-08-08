// Decompiled with JetBrains decompiler
// Type: Hacknet.ScreenManager
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  public class ScreenManager : DrawableGameComponent
  {
    private List<GameScreen> screens = new List<GameScreen>();
    private List<GameScreen> screensToUpdate = new List<GameScreen>();
    private InputState input = new InputState();
    public Color screenFillColor = Color.Black;
    public bool usingGamePad = false;
    private SpriteBatch spriteBatch;
    private SpriteFont font;
    private SoundEffect alertSound;
    public SpriteFont hugeFont;
    private Texture2D blankTexture;
    private bool isInitialized;
    private bool traceEnabled;
    public PlayerIndex controllingPlayer;
    public AudioEngine audioEngine;
    public WaveBank waveBank;
    public WaveBank musicBank;
    public SoundBank soundBank;

    public SpriteBatch SpriteBatch
    {
      get
      {
        return this.spriteBatch;
      }
    }

    public SpriteFont Font
    {
      get
      {
        return this.font;
      }
    }

    public bool TraceEnabled
    {
      get
      {
        return this.traceEnabled;
      }
      set
      {
        this.traceEnabled = value;
      }
    }

    public ScreenManager(Game game)
      : base(game)
    {
    }

    public override void Initialize()
    {
      base.Initialize();
      this.isInitialized = true;
      this.traceEnabled = false;
    }

    protected override void LoadContent()
    {
      ContentManager content = this.Game.Content;
      this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
      GuiData.spriteBatch = new SpriteBatch(this.GraphicsDevice);
      this.font = content.Load<SpriteFont>("Font12");
      this.blankTexture = new Texture2D(this.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
      this.blankTexture.SetData<Color>(new Color[1]
      {
        Color.White
      });
      try
      {
        this.alertSound = content.Load<SoundEffect>("SFX/Bip");
      }
      catch (Exception ex)
      {
        Settings.soundDisabled = true;
      }
      foreach (GameScreen screen in this.screens)
        screen.LoadContent();
      this.controllingPlayer = PlayerIndex.One;
    }

    protected override void UnloadContent()
    {
      foreach (GameScreen screen in this.screens)
        screen.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      GameScreen screen1 = (GameScreen) null;
      try
      {
        bool usingGamePad = this.usingGamePad;
        this.usingGamePad = false;
        for (int index = 0; index < this.input.CurrentGamePadStates.Length; ++index)
        {
          if (this.input.CurrentGamePadStates[index].IsConnected)
            this.usingGamePad = true;
        }
        this.input.Update();
        this.screensToUpdate.Clear();
        foreach (GameScreen screen2 in this.screens)
          this.screensToUpdate.Add(screen2);
        if (this.screensToUpdate.Count == 0)
        {
          foreach (GameScreen screen2 in this.screens)
            this.screensToUpdate.Add(screen2);
        }
        bool otherScreenHasFocus = !this.Game.IsActive;
        bool coveredByOtherScreen = false;
        bool flag = false;
        while (this.screensToUpdate.Count > 0)
        {
          screen1 = this.screensToUpdate[this.screensToUpdate.Count - 1];
          this.screensToUpdate.RemoveAt(this.screensToUpdate.Count - 1);
          if (!otherScreenHasFocus && (screen1.ScreenState == ScreenState.TransitionOn || screen1.ScreenState == ScreenState.Active))
          {
            if (usingGamePad != this.usingGamePad)
              screen1.inputMethodChanged(this.usingGamePad);
            screen1.HandleInput(this.input);
            flag = true;
          }
          screen1.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
          if (flag)
            otherScreenHasFocus = true;
          if ((screen1.ScreenState == ScreenState.TransitionOn || screen1.ScreenState == ScreenState.Active) && !screen1.IsPopup)
            coveredByOtherScreen = true;
        }
        if (!this.traceEnabled)
          return;
        this.TraceScreens();
      }
      catch (Exception ex)
      {
        if (screen1 == null)
          return;
        this.RemoveScreen(screen1);
      }
    }

    private void TraceScreens()
    {
      List<string> stringList = new List<string>();
      foreach (GameScreen screen in this.screens)
        stringList.Add(screen.GetType().Name);
    }

    public override void Draw(GameTime gameTime)
    {
      this.GraphicsDevice.Clear(this.screenFillColor);
      for (int index = 0; index < this.screens.Count; ++index)
      {
        if (this.screens[index].ScreenState != ScreenState.Hidden)
          this.screens[index].Draw(gameTime);
      }
    }

    private void handleCriticalErrorBoxAccepted(object sender, PlayerIndexEventArgs e)
    {
      try
      {
        this.spriteBatch.End();
      }
      catch
      {
      }
      this.handleCriticalError();
    }

    public void handleCriticalError()
    {
      if (this.screens.Count > 1)
        return;
      this.AddScreen((GameScreen) new MainMenu(), new PlayerIndex?(this.controllingPlayer));
    }

    public void AddScreen(GameScreen screen)
    {
      this.AddScreen(screen, new PlayerIndex?(this.controllingPlayer));
    }

    public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
    {
      screen.ControllingPlayer = controllingPlayer;
      screen.ScreenManager = this;
      screen.IsExiting = false;
      if (this.isInitialized)
        screen.LoadContent();
      this.screens.Add(screen);
    }

    public void ShowPopup(string message)
    {
      this.AddScreen((GameScreen) new MessageBoxScreen(this.clipStringForMessageBox(message), false), new PlayerIndex?(this.controllingPlayer));
    }

    public void playAlertSound()
    {
      if (Settings.soundDisabled)
        return;
      this.alertSound.Play(0.3f, 0.0f, 0.0f);
    }

    public void RemoveScreen(GameScreen screen)
    {
      if (this.isInitialized)
        screen.UnloadContent();
      this.screens.Remove(screen);
      this.screensToUpdate.Remove(screen);
      if (this.screens.Count > 0)
        return;
      this.handleCriticalError();
    }

    public GameScreen[] GetScreens()
    {
      return this.screens.ToArray();
    }

    public void FadeBackBufferToBlack(int alpha)
    {
      Viewport viewport = this.GraphicsDevice.Viewport;
      this.spriteBatch.Begin();
      this.spriteBatch.Draw(this.blankTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), new Color(0, 0, 0, (int) (byte) alpha));
      this.spriteBatch.End();
    }

    public string clipStringForMessageBox(string s)
    {
      string str = "";
      int num = 0;
      foreach (char ch in s)
      {
        str = (int) ch == 10 ? str + (object) ' ' : str + (object) ch;
        ++num;
        if (num > 50)
        {
          str += (string) (object) '\n';
          num = 0;
        }
      }
      return str;
    }

    private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
    {
    }
  }
}
