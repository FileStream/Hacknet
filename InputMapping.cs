// Decompiled with JetBrains decompiler
// Type: Hacknet.InputMapping
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework.Input;

namespace Hacknet
{
  public class InputMapping
  {
    private static InputStates ret;
    private static InputMap map;
    private static InputStates lastCalculatedState;

    public static InputStates getStatesFromKeys(KeyboardState keys, GamePadState pad, GamePadThumbSticks sticks)
    {
      InputStates ret = InputMapping.ret;
      if (keys.IsKeyDown(Keys.Right) || pad.DPad.Right == ButtonState.Pressed)
      {
        InputMapping.ret.movement = 1f;
      }
      else
      {
        int num = keys.IsKeyDown(Keys.Left) ? 0 : (pad.DPad.Left != ButtonState.Pressed ? 1 : 0);
        InputMapping.ret.movement = num != 0 ? ((double) sticks.Left.X == 0.0 ? 0.0f : sticks.Left.X) : -1f;
      }
      int num1 = pad.Buttons.A == ButtonState.Pressed ? 0 : (!keys.IsKeyDown(Keys.Up) ? 1 : 0);
      InputMapping.ret.jumping = num1 == 0;
      int num2 = pad.Buttons.B == ButtonState.Pressed ? 0 : (!keys.IsKeyDown(Keys.RightShift) ? 1 : 0);
      InputMapping.ret.useItem = num2 == 0;
      if (keys.IsKeyDown(Keys.D) || pad.DPad.Right == ButtonState.Pressed)
      {
        InputMapping.ret.wmovement = 1f;
      }
      else
      {
        int num3 = keys.IsKeyDown(Keys.A) ? 0 : (pad.DPad.Left != ButtonState.Pressed ? 1 : 0);
        InputMapping.ret.wmovement = num3 != 0 ? ((double) sticks.Left.X == 0.0 ? 0.0f : sticks.Left.X) : -1f;
      }
      int num4 = pad.Buttons.A == ButtonState.Pressed || keys.IsKeyDown(Keys.W) || keys.IsKeyDown(Keys.Space) ? 0 : ((double) pad.Triggers.Left <= 0.0 ? 1 : 0);
      InputMapping.ret.wjumping = num4 == 0;
      int num5 = pad.Buttons.B == ButtonState.Pressed ? 0 : (!keys.IsKeyDown(Keys.LeftShift) ? 1 : 0);
      InputMapping.ret.wuseItem = num5 == 0;
      InputMapping.lastCalculatedState = InputMapping.ret;
      return InputMapping.ret;
    }

    public static InputMap getMapFromKeys(KeyboardState keys, GamePadState pad)
    {
      InputMapping.map.last = InputMapping.lastCalculatedState;
      InputMapping.map.now = InputMapping.getStatesFromKeys(keys, pad, pad.ThumbSticks);
      return InputMapping.map;
    }
  }
}
