// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.WebpageLoadingEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.Effects
{
  public static class WebpageLoadingEffect
  {
    public static void DrawLoadingEffect(Rectangle bounds, SpriteBatch sb, object OS_obj, bool drawLoadingText = true)
    {
      OS os = (OS) OS_obj;
      Rectangle rectangle = new Rectangle(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2, 2, 70);
      float num1 = (float) Math.Abs(Math.Sin((double) os.timer * 0.200000002980232) * 100.0);
      Vector2 origin = new Vector2(2f, 10f);
      float num2 = 6.283185f / num1;
      float rotation = 0.0f;
      int num3 = 0;
      while ((double) rotation < 6.28318548202515)
      {
        Rectangle destinationRectangle = rectangle;
        destinationRectangle.Height = Math.Abs((int) ((double) rectangle.Height * Math.Sin(2.0 * (double) os.timer + (double) num3 * 0.200000002980232))) + 10;
        origin.Y = 1.6f;
        sb.Draw(Utils.white, destinationRectangle, new Rectangle?(), os.highlightColor, rotation, origin, SpriteEffects.FlipVertically, 0.6f);
        ++num3;
        rotation += num2;
      }
      if (!drawLoadingText)
        return;
      TextItem.doFontLabelToSize(new Rectangle(bounds.X, bounds.Y + 20, bounds.Width, 30), LocaleTerms.Loc("Loading..."), GuiData.font, Utils.AddativeWhite, false, false);
    }
  }
}
