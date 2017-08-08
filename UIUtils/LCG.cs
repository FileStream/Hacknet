// Decompiled with JetBrains decompiler
// Type: Hacknet.UIUtils.LCG
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet.UIUtils
{
  public class LCG
  {
    private int _state;

    public bool Microsoft { get; set; }

    public bool BSD
    {
      get
      {
        return !this.Microsoft;
      }
      set
      {
        this.Microsoft = !value;
      }
    }

    public LCG(bool microsoft = true)
    {
      this._state = (int) DateTime.Now.Ticks;
      this.Microsoft = microsoft;
    }

    public LCG(int n, bool microsoft = true)
    {
      this._state = n;
      this.Microsoft = microsoft;
    }

    public void reSeed(int seed)
    {
      this._state = seed;
    }

    public int Next()
    {
      if (this.BSD)
        return this._state = 1103515245 * this._state + 12345 & int.MaxValue;
      return ((this._state = 214013 * this._state + 2531011) & int.MaxValue) >> 16;
    }

    public float NextFloat()
    {
      return (float) this.Next() / (float) int.MaxValue;
    }

    public float NextFloatScaled()
    {
      return (float) this.Next() / (float) short.MaxValue;
    }

    public bool Flip()
    {
      if (this.BSD)
        return (this._state = 1103515245 * this._state + 12345 & int.MaxValue) > 1073741823;
      return ((this._state = 214013 * this._state + 2531011) & int.MaxValue) >> 16 > 1073741823;
    }

    public IEnumerable<int> Seq()
    {
      while (true)
        yield return this.Next();
    }
  }
}
