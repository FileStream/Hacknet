﻿// Decompiled with JetBrains decompiler
// Type: Hacknet.DelayedInput
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public struct DelayedInput
  {
    public double Delay { get; set; }

    public InputMap inputs { get; set; }

    public float xPos { get; set; }

    public float yPos { get; set; }
  }
}