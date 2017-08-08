// Decompiled with JetBrains decompiler
// Type: Hacknet.UIUtils.FancyOutlines
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.UIUtils
{
  public static class FancyOutlines
  {
    public static void DrawCornerCutOutline(Rectangle bounds, SpriteBatch sb, float cornerCut, Color col)
    {
      Vector2 vector2_1 = new Vector2((float) bounds.X, (float) bounds.Y + cornerCut);
      Vector2 vector2_2 = new Vector2((float) bounds.X + cornerCut, (float) bounds.Y);
      Utils.drawLine(sb, vector2_1, vector2_2, Vector2.Zero, col, 0.6f);
      vector2_1 = new Vector2((float) (bounds.X + bounds.Width) - cornerCut, (float) bounds.Y);
      Utils.drawLine(sb, vector2_2, vector2_1, Vector2.Zero, col, 0.6f);
      vector2_2 = new Vector2((float) (bounds.X + bounds.Width), (float) bounds.Y + cornerCut);
      Utils.drawLine(sb, vector2_2, vector2_1, Vector2.Zero, col, 0.6f);
      vector2_1 = new Vector2((float) (bounds.X + bounds.Width), (float) (bounds.Y + bounds.Height) - cornerCut);
      Utils.drawLine(sb, vector2_2, vector2_1, Vector2.Zero, col, 0.6f);
      vector2_2 = new Vector2((float) (bounds.X + bounds.Width) - cornerCut, (float) (bounds.Y + bounds.Height));
      Utils.drawLine(sb, vector2_2, vector2_1, Vector2.Zero, col, 0.6f);
      vector2_1 = new Vector2((float) bounds.X + cornerCut, (float) (bounds.Y + bounds.Height));
      Utils.drawLine(sb, vector2_2, vector2_1, Vector2.Zero, col, 0.6f);
      vector2_2 = new Vector2((float) bounds.X, (float) (bounds.Y + bounds.Height) - cornerCut);
      Utils.drawLine(sb, vector2_2, vector2_1, Vector2.Zero, col, 0.6f);
      vector2_1 = new Vector2((float) bounds.X, (float) bounds.Y + cornerCut);
      Utils.drawLine(sb, vector2_2, vector2_1, Vector2.Zero, col, 0.6f);
    }
  }
}
