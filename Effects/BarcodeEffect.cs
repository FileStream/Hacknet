// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.BarcodeEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet.Effects
{
  public class BarcodeEffect
  {
    private float completeTime = 0.0f;
    public bool isInverted = false;
    public bool leftRightBias = false;
    public bool LeftRightMaxSizeFalloff = false;
    private const double MAX_WIDTH = 8.0;
    private const double MAX_OFFSET = 2.0;
    private const double MAX_DELAY = 3.0;
    private const float COMPLETE_TIME = 4f;
    private const float COMPLETE_TIME_DELAY = 0.9f;
    private List<float> widths;
    private List<float> offsets;
    private List<float> delays;
    private List<float> complete;
    public int maxWidth;

    public BarcodeEffect(int width, bool inverted = false, bool leftRight = false)
    {
      this.maxWidth = width;
      this.isInverted = inverted;
      this.leftRightBias = leftRight;
      this.reset();
    }

    public void reset()
    {
      this.widths = new List<float>();
      this.offsets = new List<float>();
      this.delays = new List<float>();
      this.complete = new List<float>();
      float num1 = 0.0f;
      bool flag = false;
      while ((double) num1 < (double) this.maxWidth)
      {
        if ((double) num1 + 8.0 >= (double) this.maxWidth)
        {
          this.widths.Add((float) this.maxWidth - num1);
          this.delays.Add(0.0f);
          this.complete.Add(0.0f);
          num1 = (float) this.maxWidth;
        }
        else
        {
          float num2 = (float) (Utils.random.NextDouble() * (flag ? 2.0 : 8.0));
          if (flag)
          {
            this.offsets.Add(num2);
          }
          else
          {
            this.widths.Add(num2);
            this.complete.Add(0.0f);
            if (this.leftRightBias)
            {
              float num3 = num1 / (float) this.maxWidth;
              this.delays.Add((float) ((Utils.random.NextDouble() / 2.0 + (double) num3 / 2.0) * (3.0 * (double) num3)));
            }
            else
              this.delays.Add((float) (Utils.random.NextDouble() * 3.0));
          }
          num1 += num2;
        }
        flag = !flag;
      }
    }

    public void Update(float t)
    {
      bool flag = true;
      for (int index1 = 0; index1 < this.delays.Count; ++index1)
      {
        if ((double) this.delays[index1] > 0.0)
        {
          List<float> delays;
          int index2;
          (delays = this.delays)[index2 = index1] = delays[index2] - t;
          if ((double) this.delays[index1] < 0.0)
          {
            this.complete[index1] = (float) (-(double) this.delays[index1] / 4.0);
            this.delays[index1] = 0.0f;
          }
          flag = false;
        }
        else
        {
          List<float> complete;
          int index2;
          (complete = this.complete)[index2 = index1] = complete[index2] + t / 4f;
          if ((double) this.complete[index1] > 1.0)
            this.complete[index1] = 1f;
          else
            flag = false;
        }
      }
      if (!flag)
        return;
      this.completeTime += t;
      if ((double) this.completeTime > 0.899999976158142)
      {
        this.reset();
        this.completeTime = 0.0f;
      }
    }

    public void Draw(int x, int y, int maxWidth, int maxHeight, SpriteBatch sb, Color? barColor = null)
    {
      Color color = (barColor.HasValue ? barColor.Value : Color.White) * (float) Math.Pow(1.0 - (double) this.completeTime / 0.899999976158142, 3.0);
      Vector2 position = new Vector2((float) x, (float) y);
      float num1 = 0.0f;
      for (int index = 0; index < this.widths.Count; ++index)
      {
        float num2 = (float) maxHeight;
        if (this.LeftRightMaxSizeFalloff)
        {
          float num3 = 1f - Utils.QuadraticOutCurve((float) Math.Abs(index - this.widths.Count / 2) / ((float) this.widths.Count / 2f));
          num2 = (float) maxHeight * num3;
        }
        position.X = (float) x + num1;
        position.Y = (float) y;
        float width = this.widths[index];
        float y1 = (double) this.delays[index] <= 0.0 ? num2 * (float) Math.Pow((double) this.complete[index], 0.5) : 0.0f;
        if (this.isInverted)
          position.Y = (float) (y + maxHeight) - y1;
        sb.Draw(Utils.white, position, new Rectangle?(), color, 0.0f, Vector2.Zero, new Vector2(width, y1), SpriteEffects.None, 0.5f);
        num1 += this.widths[index];
        if (this.offsets.Count > index)
          num1 += this.offsets[index];
      }
    }
  }
}
