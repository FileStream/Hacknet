// Decompiled with JetBrains decompiler
// Type: Hacknet.InputState
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hacknet
{
  public class InputState
  {
    public static InputState Empty = new InputState();
    public const int MaxInputs = 4;
    public KeyboardState[] CurrentKeyboardStates;
    public GamePadState[] CurrentGamePadStates;
    public KeyboardState[] LastKeyboardStates;
    public GamePadState[] LastGamePadStates;
    public readonly bool[] GamePadWasConnected;

    public InputState()
    {
      this.CurrentKeyboardStates = new KeyboardState[4];
      this.CurrentGamePadStates = new GamePadState[4];
      this.LastKeyboardStates = new KeyboardState[4];
      this.LastGamePadStates = new GamePadState[4];
      this.GamePadWasConnected = new bool[4];
    }

    public void Update()
    {
      for (int index = 0; index < 4; ++index)
      {
        this.LastKeyboardStates[index] = this.CurrentKeyboardStates[index];
        this.LastGamePadStates[index] = this.CurrentGamePadStates[index];
        this.CurrentKeyboardStates[index] = Keyboard.GetState((PlayerIndex) index);
        this.CurrentGamePadStates[index] = GamePad.GetState((PlayerIndex) index);
        if (this.CurrentGamePadStates[index].IsConnected)
          this.GamePadWasConnected[index] = true;
      }
    }

    public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
      if (!controllingPlayer.HasValue)
        return this.IsNewKeyPress(key, new PlayerIndex?(PlayerIndex.One), out playerIndex) || this.IsNewKeyPress(key, new PlayerIndex?(PlayerIndex.Two), out playerIndex) || this.IsNewKeyPress(key, new PlayerIndex?(PlayerIndex.Three), out playerIndex) || this.IsNewKeyPress(key, new PlayerIndex?(PlayerIndex.Four), out playerIndex);
      playerIndex = controllingPlayer.Value;
      int index = (int) playerIndex;
      return this.CurrentKeyboardStates[index].IsKeyDown(key) && this.LastKeyboardStates[index].IsKeyUp(key);
    }

    public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
      if (!controllingPlayer.HasValue)
        return this.IsNewButtonPress(button, new PlayerIndex?(PlayerIndex.One), out playerIndex) || this.IsNewButtonPress(button, new PlayerIndex?(PlayerIndex.Two), out playerIndex) || this.IsNewButtonPress(button, new PlayerIndex?(PlayerIndex.Three), out playerIndex) || this.IsNewButtonPress(button, new PlayerIndex?(PlayerIndex.Four), out playerIndex);
      playerIndex = controllingPlayer.Value;
      int index = (int) playerIndex;
      return this.CurrentGamePadStates[index].IsButtonDown(button) && this.LastGamePadStates[index].IsButtonUp(button);
    }

    public bool IsMenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
      return this.IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) || this.IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
    }

    public bool IsMenuCancel(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
      return this.IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
    }

    public bool IsMenuUp(PlayerIndex? controllingPlayer)
    {
      PlayerIndex playerIndex;
      return this.IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) || this.IsNewKeyPress(Keys.W, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
    }

    public bool IsMenuDown(PlayerIndex? controllingPlayer)
    {
      PlayerIndex playerIndex;
      return this.IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) || this.IsNewKeyPress(Keys.D, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
    }

    public bool IsPauseGame(PlayerIndex? controllingPlayer)
    {
      PlayerIndex playerIndex;
      return this.IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
    }
  }
}
