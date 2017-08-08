// Decompiled with JetBrains decompiler
// Type: Hacknet.LoadedTexture
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
  public struct LoadedTexture
  {
    public string path { get; set; }

    public Texture2D tex { get; set; }

    public int retainCount { get; set; }
  }
}
