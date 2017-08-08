// Decompiled with JetBrains decompiler
// Type: Hacknet.SpritePlacementData
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;

namespace Hacknet
{
  public struct SpritePlacementData
  {
    public Vector2 pos;
    public float depth;
    public Vector2 scale;
    public int spriteIndex;

    public SpritePlacementData(Vector2 position, Vector2 scales, float layerDepth)
    {
      this.pos = position;
      this.scale = scales;
      this.depth = layerDepth;
      this.spriteIndex = 0;
    }

    public SpritePlacementData(Vector2 position, Vector2 scales, float layerDepth, int SpriteIndex)
    {
      this.pos = position;
      this.scale = scales;
      this.depth = layerDepth;
      this.spriteIndex = SpriteIndex;
    }
  }
}
