﻿// Decompiled with JetBrains decompiler
// Type: Hacknet.Daemons.Helpers.IMedicalMonitor
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Daemons.Helpers
{
  public interface IMedicalMonitor
  {
    void HeartBeat(float beatTime);

    void Update(float dt);

    void Draw(Rectangle bounds, SpriteBatch sb, Color c, float timeRollback);
  }
}
