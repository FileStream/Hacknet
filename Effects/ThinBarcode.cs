// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.ThinBarcode
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet.Effects
{
  public class ThinBarcode
  {
    private List<int> widths = new List<int>();
    private List<int> gaps = new List<int>();
    private List<int> widthsLast = new List<int>();
    private List<int> gapsLast = new List<int>();
    private int height;
    private int oldW;

    public ThinBarcode(int w, int height)
    {
      this.oldW = w;
      this.height = height;
      this.regenerate();
      this.widthsLast = this.widths;
      this.gapsLast = this.gaps;
    }

    public void regenerate()
    {
      int oldW = this.oldW;
      this.widthsLast = this.widths;
      this.gapsLast = this.gaps;
      this.widths.Clear();
      this.gaps.Clear();
      int num1 = 0;
      while (num1 < oldW)
      {
        int num2 = Math.Min(oldW - num1, Utils.random.Next(1, 20));
        if (num2 > 0)
        {
          int num3 = Utils.random.Next(3, 11);
          this.widths.Add(num2);
          this.gaps.Add(num3);
          num1 += num2 + num3;
        }
      }
    }

    public void Draw(SpriteBatch sb, int posX, int posY, Color c)
    {
      Rectangle destinationRectangle = new Rectangle(posX, posY, 0, this.height);
      for (int index = 0; index < this.widths.Count; ++index)
      {
        destinationRectangle.Width = this.widths[index];
        sb.Draw(Utils.white, destinationRectangle, c);
        destinationRectangle.X += this.widths[index];
        destinationRectangle.X += this.gaps[index];
      }
    }
  }
}
