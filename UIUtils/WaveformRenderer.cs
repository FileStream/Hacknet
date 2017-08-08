// Decompiled with JetBrains decompiler
// Type: Hacknet.UIUtils.WaveformRenderer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.UIUtils
{
  internal class WaveformRenderer
  {
    private int SecondBlockSize = 100;
    private double[] left;
    private double[] right;

    public WaveformRenderer(string filename)
    {
      AudioUtils.openWav(filename, out this.left, out this.right);
    }

    public void RenderWaveform(double time, double totalTime, SpriteBatch sb, Rectangle bounds)
    {
      time %= totalTime;
      this.SecondBlockSize = (int) ((double) this.left.Length / totalTime);
      int num1 = this.SecondBlockSize / 100;
      int num2 = (int) (time * (double) this.SecondBlockSize);
      int num3 = Math.Min(this.left.Length - 1, num2 + num1) - num2;
      double num4 = (double) bounds.Width / (double) num3;
      int num5 = 0;
      for (int index = 0; index < num3; ++index)
      {
        double num6 = this.left[num2 + index];
        if (num6 == 0.0)
          ++num5;
        double num7 = num6 * (double) bounds.Height;
        Rectangle destinationRectangle = new Rectangle((int) ((double) bounds.X + (double) index * num4), bounds.Y + bounds.Height / 2 - (int) (num7 / 2.0), (int) num4, (int) num7);
        sb.Draw(Utils.white, destinationRectangle, Color.White * 0.4f);
      }
    }
  }
}
