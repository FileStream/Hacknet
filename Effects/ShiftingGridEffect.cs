// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.ShiftingGridEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
  public class ShiftingGridEffect
  {
    private ShiftingGridEffect.ShiftingGridSpot[,] themeGrid = new ShiftingGridEffect.ShiftingGridSpot[20, 100];

    public ShiftingGridEffect()
    {
    }

    public ShiftingGridEffect(int patternWidth, int patternHeight)
    {
      this.themeGrid = new ShiftingGridEffect.ShiftingGridSpot[patternWidth, patternHeight];
    }

    public void ResetThemeGrid()
    {
      for (int y = 0; y < this.themeGrid.GetLength(0); ++y)
      {
        for (int x = 0; x < this.themeGrid.GetLength(1); ++x)
          this.ResetGridPoint(x, y);
      }
    }

    public void ResetGridPoint(int x, int y)
    {
      // ISSUE: explicit reference operation
      ^this.themeGrid.Address(y, x) = new ShiftingGridEffect.ShiftingGridSpot()
      {
        from = this.themeGrid[y, x].to,
        to = Utils.randm(2f),
        time = 0.0f,
        totalTime = Utils.randm(3f) + 1.2f
      };
    }

    public void Update(float t)
    {
      for (int y = 0; y < this.themeGrid.GetLength(0); ++y)
      {
        for (int x = 0; x < this.themeGrid.GetLength(1); ++x)
        {
          ShiftingGridEffect.ShiftingGridSpot shiftingGridSpot = this.themeGrid[y, x];
          shiftingGridSpot.time += t;
          if ((double) shiftingGridSpot.time >= (double) shiftingGridSpot.totalTime)
            this.ResetGridPoint(x, y);
          else
            this.themeGrid[y, x] = shiftingGridSpot;
        }
      }
    }

    public void RenderGrid(Rectangle bounds, SpriteBatch sb, Color c1, Color c2, Color c3, bool centreEffect = false)
    {
      int num1 = 12;
      if (centreEffect)
        bounds.X -= bounds.Width % num1;
      int x = bounds.X + bounds.Width - num1 - 1;
      Rectangle destinationRectangle = new Rectangle(x, bounds.Y + 1, num1, num1);
      int num2;
      int num3 = num2 = 0;
      while (destinationRectangle.Y + 1 < bounds.Y + bounds.Height)
      {
        if (destinationRectangle.Y + num1 + 1 >= bounds.Y + bounds.Height)
          destinationRectangle.Height = bounds.Y + bounds.Height - (destinationRectangle.Y + 2);
        while (destinationRectangle.X - num1 > bounds.X + bounds.Width - bounds.Width - 1)
        {
          ShiftingGridEffect.ShiftingGridSpot shiftingGridSpot = this.themeGrid[num3 % this.themeGrid.GetLength(0), num2 % this.themeGrid.GetLength(1)];
          float num4 = shiftingGridSpot.time / shiftingGridSpot.totalTime;
          float amount = shiftingGridSpot.from + num4 * (shiftingGridSpot.to - shiftingGridSpot.from);
          Color color1 = c3;
          Color color2 = c2;
          if ((double) amount >= 1.0)
          {
            --amount;
            color1 = c2;
            color2 = c1;
          }
          Color color3 = Color.Lerp(color1, color2, amount);
          sb.Draw(Utils.white, destinationRectangle, color3);
          ++num3;
          destinationRectangle.X -= num1 + 1;
        }
        ++num2;
        destinationRectangle.X = x;
        destinationRectangle.Y += num1 + 1;
      }
    }

    private struct ShiftingGridSpot
    {
      public float from;
      public float to;
      public float time;
      public float totalTime;
    }
  }
}
