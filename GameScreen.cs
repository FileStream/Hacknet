// Decompiled with JetBrains decompiler
// Type: Hacknet.GameScreen
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  public abstract class GameScreen
  {
    private bool isPopup = false;
    private TimeSpan transitionOnTime = TimeSpan.Zero;
    private TimeSpan transitionOffTime = TimeSpan.Zero;
    private float transitionPosition = 1f;
    private ScreenState screenState = ScreenState.TransitionOn;
    private bool isExiting = false;
    private bool otherScreenHasFocus;
    private ScreenManager screenManager;
    private PlayerIndex? controllingPlayer;

    public bool IsPopup
    {
      get
      {
        return this.isPopup;
      }
      protected set
      {
        this.isPopup = value;
      }
    }

    public TimeSpan TransitionOnTime
    {
      get
      {
        return this.transitionOnTime;
      }
      protected set
      {
        this.transitionOnTime = value;
      }
    }

    public TimeSpan TransitionOffTime
    {
      get
      {
        return this.transitionOffTime;
      }
      protected set
      {
        this.transitionOffTime = value;
      }
    }

    public float TransitionPosition
    {
      get
      {
        return this.transitionPosition;
      }
      protected set
      {
        this.transitionPosition = value;
      }
    }

    public byte TransitionAlpha
    {
      get
      {
        return (byte) ((double) byte.MaxValue - (double) this.TransitionPosition * (double) byte.MaxValue);
      }
    }

    public ScreenState ScreenState
    {
      get
      {
        return this.screenState;
      }
      protected set
      {
        this.screenState = value;
      }
    }

    public bool IsExiting
    {
      get
      {
        return this.isExiting;
      }
      protected internal set
      {
        this.isExiting = value;
      }
    }

    public bool IsActive
    {
      get
      {
        return !this.otherScreenHasFocus && (this.screenState == ScreenState.TransitionOn || this.screenState == ScreenState.Active);
      }
    }

    public ScreenManager ScreenManager
    {
      get
      {
        return this.screenManager;
      }
      internal set
      {
        this.screenManager = value;
      }
    }

    public PlayerIndex? ControllingPlayer
    {
      get
      {
        return this.controllingPlayer;
      }
      internal set
      {
        this.controllingPlayer = value;
      }
    }

    public virtual void LoadContent()
    {
    }

    public virtual void UnloadContent()
    {
    }

    public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      this.otherScreenHasFocus = otherScreenHasFocus;
      if (this.isExiting)
      {
        this.screenState = ScreenState.TransitionOff;
        if (this.UpdateTransition(gameTime, this.transitionOffTime, 1))
          return;
        this.ScreenManager.RemoveScreen(this);
      }
      else if (coveredByOtherScreen)
      {
        if (this.UpdateTransition(gameTime, this.transitionOffTime, 1))
          this.screenState = ScreenState.TransitionOff;
        else
          this.screenState = ScreenState.Hidden;
      }
      else
        this.screenState = !this.UpdateTransition(gameTime, this.transitionOnTime, -1) ? ScreenState.Active : ScreenState.TransitionOn;
    }

    private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
    {
      this.transitionPosition += (!(time == TimeSpan.Zero) ? (float) (gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds) : 1f) * (float) direction;
      if ((direction >= 0 || (double) this.transitionPosition > 0.0) && (direction <= 0 || (double) this.transitionPosition < 1.0))
        return true;
      this.transitionPosition = MathHelper.Clamp(this.transitionPosition, 0.0f, 1f);
      return false;
    }

    public virtual void HandleInput(InputState input)
    {
    }

    public virtual void Draw(GameTime gameTime)
    {
    }

    public void ExitScreen()
    {
      if (this.TransitionOffTime == TimeSpan.Zero)
        this.ScreenManager.RemoveScreen(this);
      else
        this.isExiting = true;
    }

    public virtual void inputMethodChanged(bool usingGamePad)
    {
    }
  }
}
