// Decompiled with JetBrains decompiler
// Type: Hacknet.PatternDrawer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet
{
  public static class PatternDrawer
  {
    public static float time = 0.0f;
    public static Texture2D warningStripe;
    public static Texture2D errorTile;
    public static Texture2D binaryTile;
    public static Texture2D thinStripe;
    public static Texture2D star;
    public static Texture2D wipTile;

    public static void init(ContentManager content)
    {
      PatternDrawer.warningStripe = TextureBank.load("StripePattern", content);
      PatternDrawer.errorTile = TextureBank.load("ErrorTile", content);
      PatternDrawer.binaryTile = TextureBank.load("BinaryTile", content);
      PatternDrawer.thinStripe = TextureBank.load("ThinStripe", content);
      PatternDrawer.star = TextureBank.load("Sprites/Star", content);
      PatternDrawer.wipTile = TextureBank.load("Sprites/WIPTile", content);
    }

    public static void update(float t)
    {
      PatternDrawer.time += t;
    }

    public static void draw(Rectangle dest, float offset, Color backingColor, Color patternColor, SpriteBatch sb)
    {
      PatternDrawer.draw(dest, offset, backingColor, patternColor, sb, PatternDrawer.warningStripe);
    }

    public static void draw(Rectangle dest, float offset, Color backingColor, Color patternColor, SpriteBatch sb, Texture2D tex)
    {
      int x1 = dest.X;
      int y = dest.Y;
      int height = Math.Min(dest.Height, tex.Height);
      int width = (int) ((double) PatternDrawer.time * (double) offset % 1.0 * (double) tex.Width);
      Rectangle rectangle = Rectangle.Empty;
      sb.Draw(Utils.white, dest, backingColor);
      Rectangle destinationRectangle;
      for (; y - dest.Y + height <= dest.Height; destinationRectangle.Y = y)
      {
        int x2 = dest.X;
        destinationRectangle = new Rectangle(x2, y, width, height);
        rectangle = new Rectangle(0, 0, width, height);
        rectangle.X = tex.Width - rectangle.Width;
        sb.Draw(tex, destinationRectangle, new Rectangle?(rectangle), patternColor);
        int num = x2 + width;
        destinationRectangle.X = num;
        for (destinationRectangle.Width = tex.Width; num <= dest.X + dest.Width - tex.Width; destinationRectangle.X = num)
        {
          Rectangle? sourceRectangle = new Rectangle?();
          if (dest.Height < tex.Height)
            sourceRectangle = new Rectangle?(new Rectangle(0, 0, tex.Width, dest.Height));
          sb.Draw(tex, destinationRectangle, sourceRectangle, patternColor);
          num += tex.Width;
        }
        destinationRectangle.X = num;
        destinationRectangle.Width = dest.X + dest.Width - num;
        rectangle.Width = destinationRectangle.Width;
        rectangle.X = 0;
        sb.Draw(tex, destinationRectangle, new Rectangle?(rectangle), patternColor);
        y += tex.Height;
      }
      destinationRectangle.Height = dest.Height - (y - dest.Y);
      rectangle.Height = destinationRectangle.Height;
      int x3 = dest.X;
      destinationRectangle.X = x3;
      destinationRectangle.Y = y;
      destinationRectangle.Width = width;
      rectangle.Width = width;
      rectangle.X = tex.Width - rectangle.Width;
      sb.Draw(tex, destinationRectangle, new Rectangle?(rectangle), patternColor);
      int num1 = x3 + width;
      destinationRectangle.X = num1;
      destinationRectangle.Width = tex.Width;
      rectangle.Width = tex.Width;
      for (rectangle.X = 0; num1 <= dest.X + dest.Width - tex.Width; destinationRectangle.X = num1)
      {
        sb.Draw(tex, destinationRectangle, new Rectangle?(rectangle), patternColor);
        num1 += tex.Width;
      }
      destinationRectangle.X = num1;
      destinationRectangle.Width = dest.X + dest.Width - num1;
      rectangle.Width = destinationRectangle.Width;
      rectangle.X = 0;
      sb.Draw(tex, destinationRectangle, new Rectangle?(rectangle), patternColor);
      int num2 = y + tex.Height;
      destinationRectangle.Y = num2;
    }
  }
}
