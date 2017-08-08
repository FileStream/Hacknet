// Decompiled with JetBrains decompiler
// Type: Hacknet.MainDisplayOverrideEXE
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
  public interface MainDisplayOverrideEXE
  {
    bool DisplayOverrideIsActive { get; set; }

    void RenderMainDisplay(Rectangle dest, SpriteBatch sb);
  }
}
