// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.GridEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.Effects
{
  public static class GridEffect
  {
    public static void DrawGridBackground(Rectangle dest, SpriteBatch sb, int desiredNumOfBlocks, Color CrossColor)
    {
      int num1 = dest.Width / (desiredNumOfBlocks + 2);
      int width = 9;
      float num2 = 1f;
      int num3 = dest.X + num1 / 2;
      int num4 = dest.Y + num1 / 2;
      do
      {
        int num5 = 0;
        do
        {
          Rectangle destinationRectangle1 = new Rectangle(Math.Max(dest.X, num3 - width / 2), num4 - (int) ((double) num2 / 2.0 + 0.5), width, (int) num2);
          sb.Draw(Utils.white, destinationRectangle1, CrossColor);
          Rectangle destinationRectangle2 = new Rectangle(num3 - (int) ((double) num2 / 2.0), Math.Max(dest.Y, num4 - (int) ((double) width / 2.0 + 0.5)), (int) num2, width / 2 - (int) ((double) num2 / 2.0));
          sb.Draw(Utils.white, destinationRectangle2, CrossColor);
          Rectangle destinationRectangle3 = new Rectangle(num3 - (int) ((double) num2 / 2.0), num4 + (int) ((double) num2 / 2.0), (int) num2, width / 2 - (int) ((double) num2 / 2.0));
          sb.Draw(Utils.white, destinationRectangle3, CrossColor);
          num3 += num1;
          ++num5;
        }
        while (num3 <= dest.X + dest.Width - num1 / 2);
        num3 = dest.X + num1 / 2;
        num4 += num1;
      }
      while (num4 <= dest.Y + dest.Height - num1 / 2);
    }
  }
}
