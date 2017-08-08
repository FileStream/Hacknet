// Decompiled with JetBrains decompiler
// Type: Hacknet.Misc.HSLColor
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;

namespace Hacknet.Misc
{
  public class HSLColor
  {
    public float Hue;
    public float Saturation;
    public float Luminosity;

    public HSLColor(float H, float S, float L)
    {
      this.Hue = H;
      this.Saturation = S;
      this.Luminosity = L;
    }

    public static HSLColor FromRGB(Color Clr)
    {
      return HSLColor.FromRGB(Clr.R, Clr.G, Clr.B);
    }

    public static HSLColor FromRGB(byte R, byte G, byte B)
    {
      float val1 = (float) R / (float) byte.MaxValue;
      float val2_1 = (float) G / (float) byte.MaxValue;
      float val2_2 = (float) B / (float) byte.MaxValue;
      float num1 = Math.Min(Math.Min(val1, val2_1), val2_2);
      float num2 = Math.Max(Math.Max(val1, val2_1), val2_2);
      float num3 = num2 - num1;
      float H = 0.0f;
      float S = 0.0f;
      float L = (float) (((double) num2 + (double) num1) / 2.0);
      if ((double) num3 != 0.0)
      {
        S = (double) L >= 0.5 ? num3 / (2f - num2 - num1) : num3 / (num2 + num1);
        if ((double) val1 == (double) num2)
          H = (val2_1 - val2_2) / num3;
        else if ((double) val2_1 == (double) num2)
          H = (float) (2.0 + ((double) val2_2 - (double) val1) / (double) num3);
        else if ((double) val2_2 == (double) num2)
          H = (float) (4.0 + ((double) val1 - (double) val2_1) / (double) num3);
      }
      return new HSLColor(H, S, L);
    }

    private float Hue_2_RGB(float v1, float v2, float vH)
    {
      if ((double) vH < 0.0)
        ++vH;
      if ((double) vH > 1.0)
        --vH;
      if (6.0 * (double) vH < 1.0)
        return v1 + (float) (((double) v2 - (double) v1) * 6.0) * vH;
      if (2.0 * (double) vH < 1.0)
        return v2;
      if (3.0 * (double) vH < 2.0)
        return v1 + (float) (((double) v2 - (double) v1) * (0.0 - (double) vH) * 6.0);
      return v1;
    }

    public Color ToRGB()
    {
      byte num1;
      byte num2;
      byte num3;
      if ((double) this.Saturation == 0.0)
      {
        num1 = (byte) Math.Round((double) this.Luminosity * (double) byte.MaxValue);
        num2 = (byte) Math.Round((double) this.Luminosity * (double) byte.MaxValue);
        num3 = (byte) Math.Round((double) this.Luminosity * (double) byte.MaxValue);
      }
      else
      {
        double num4 = (double) this.Hue / 6.0;
        double t2 = (double) this.Luminosity >= 0.5 ? (double) this.Luminosity + (double) this.Saturation - (double) this.Luminosity * (double) this.Saturation : (double) this.Luminosity * (1.0 + (double) this.Saturation);
        double t1 = 2.0 * (double) this.Luminosity - t2;
        double c1 = num4 + 1.0 / 3.0;
        double c2 = num4;
        double c3 = num4 - 1.0 / 3.0;
        double num5 = HSLColor.ColorCalc(c1, t1, t2);
        double num6 = HSLColor.ColorCalc(c2, t1, t2);
        double num7 = HSLColor.ColorCalc(c3, t1, t2);
        num1 = (byte) Math.Round(num5 * (double) byte.MaxValue);
        num2 = (byte) Math.Round(num6 * (double) byte.MaxValue);
        num3 = (byte) Math.Round(num7 * (double) byte.MaxValue);
      }
      return new Color() { R = num1, G = num2, B = num3, A = byte.MaxValue };
    }

    private static double ColorCalc(double c, double t1, double t2)
    {
      if (c < 0.0)
        ++c;
      if (c > 1.0)
        --c;
      if (6.0 * c < 1.0)
        return t1 + (t2 - t1) * 6.0 * c;
      if (2.0 * c < 1.0)
        return t2;
      if (3.0 * c < 2.0)
        return t1 + (t2 - t1) * (2.0 / 3.0 - c) * 6.0;
      return t1;
    }
  }
}
