// Decompiled with JetBrains decompiler
// Type: Hacknet.Input.TextInputHook
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework.Input;
using SDL2;
using System;

namespace Hacknet.Input
{
  public class TextInputHook : IDisposable
  {
    private string buffer = "";
    private bool backSpace = false;

    public string Buffer
    {
      get
      {
        return this.buffer;
      }
    }

    public bool BackSpace
    {
      get
      {
        bool backSpace = this.backSpace;
        this.backSpace = false;
        return backSpace;
      }
    }

    public TextInputHook(IntPtr whnd)
    {
      TextInputEXT.TextInput += new Action<char>(this.OnTextInput);
      SDL.SDL_StartTextInput();
    }

    public void clearBuffer()
    {
      this.buffer = "";
    }

    public void Dispose()
    {
      SDL.SDL_StopTextInput();
      TextInputEXT.TextInput -= new Action<char>(this.OnTextInput);
    }

    private void OnTextInput(char c)
    {
      if ((int) c == 8)
        this.backSpace = true;
      else if ((int) c == 22)
        this.buffer += SDL.SDL_GetClipboardText();
      else
        this.buffer += (string) (object) c;
    }
  }
}
