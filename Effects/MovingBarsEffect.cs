// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.MovingBarsEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Hacknet.Effects
{
  public class MovingBarsEffect
  {
    private List<MovingBarsEffect.BarLine> Lines = new List<MovingBarsEffect.BarLine>();
    public float MinLineChangeTime = 0.2f;
    public float MaxLineChangeTime = 2f;
    public bool IsInverted = false;

    public void Update(float dt)
    {
      for (int index = 0; index < this.Lines.Count; ++index)
      {
        MovingBarsEffect.BarLine line = this.Lines[index];
        line.TimeRemaining -= dt;
        if ((double) line.TimeRemaining <= 0.0)
        {
          line.Current = line.Next;
          line.Next = Utils.randm(1f);
          line.TotalTimeThisStep = this.MinLineChangeTime + Utils.randm(this.MaxLineChangeTime - this.MinLineChangeTime);
          line.TimeRemaining = line.TotalTimeThisStep;
        }
        this.Lines[index] = line;
      }
    }

    public void Draw(SpriteBatch sb, Rectangle bounds, float minHeight, float lineWidth, float lineSeperation, Color drawColor)
    {
      int num1 = 0;
      float num2 = 0.0f;
      while ((double) num2 + (double) lineWidth < (double) bounds.Width)
      {
        ++num1;
        num2 += lineWidth + lineSeperation;
      }
      bool flag = false;
      while (this.Lines.Count - 1 < num1)
      {
        this.Lines.Add(new MovingBarsEffect.BarLine()
        {
          TimeRemaining = -1f
        });
        flag = true;
      }
      if (flag)
        this.Update(0.0f);
      float x = (float) bounds.X;
      for (int index = 0; index < num1; ++index)
      {
        MovingBarsEffect.BarLine line = this.Lines[index];
        float num3 = Utils.QuadraticOutCurve((float) (1.0 - (double) line.TimeRemaining / (double) line.TotalTimeThisStep));
        float num4 = line.Current + (line.Next - line.Current) * num3;
        float num5 = (float) bounds.Height - minHeight;
        int height = (int) ((double) minHeight + (double) num5 * (double) num4);
        Rectangle destinationRectangle = new Rectangle((int) x, bounds.Y + bounds.Height - height, (int) lineWidth, height);
        if (this.IsInverted)
          destinationRectangle.Y = bounds.Y;
        sb.Draw(Utils.white, destinationRectangle, drawColor);
        x += lineWidth + lineSeperation;
      }
    }

    private struct BarLine
    {
      public float Current;
      public float Next;
      public float TimeRemaining;
      public float TotalTimeThisStep;
    }
  }
}
