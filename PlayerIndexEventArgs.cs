// Decompiled with JetBrains decompiler
// Type: Hacknet.PlayerIndexEventArgs
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  internal class PlayerIndexEventArgs : EventArgs
  {
    private PlayerIndex playerIndex;

    public PlayerIndex PlayerIndex
    {
      get
      {
        return this.playerIndex;
      }
    }

    public PlayerIndexEventArgs(PlayerIndex playerIndex)
    {
      this.playerIndex = playerIndex;
    }
  }
}
