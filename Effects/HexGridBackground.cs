// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.HexGridBackground
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.Effects
{
  public class HexGridBackground
  {
    private float timer = 0.0f;
    private LCG lcg = new LCG(false);
    public bool HasRedFlashyOnes = false;
    public float HexScale = 0.1f;
    private Texture2D Hex;
    private Texture2D Outline;

    public HexGridBackground(ContentManager content)
    {
      this.Hex = content.Load<Texture2D>("Sprites/Misc/Hexagon");
      this.Outline = content.Load<Texture2D>("Sprites/Misc/HexOutline");
    }

    public void Update(float dt)
    {
      this.timer += dt;
    }

    public void Draw(Rectangle dest, SpriteBatch sb, Color first, Color second, HexGridBackground.ColoringAlgorithm algorithm = HexGridBackground.ColoringAlgorithm.CorrectedSinWash, float angle = 0.0f)
    {
      float num1 = 2f;
      float x = (float) dest.X;
      float y = (float) dest.Y;
      float num2 = this.HexScale * (float) this.Hex.Width;
      float num3 = this.HexScale * (float) this.Hex.Height;
      int seed = 10;
      bool flag1 = true;
      while ((double) x + (double) num2 < (double) (dest.X + dest.Width))
      {
        while ((double) y + (double) num3 < (double) (dest.Y + dest.Height))
        {
          this.lcg.reSeed(seed);
          float num4 = this.lcg.NextFloat();
          bool flag2 = false;
          Color color = second;
          if (this.HasRedFlashyOnes && (double) num4 <= 1.0 / 1000.0)
          {
            color = Utils.AddativeRed * 0.5f;
            flag2 = true;
          }
          float amount;
          switch (algorithm)
          {
            case HexGridBackground.ColoringAlgorithm.NegaitiveSinWash:
              amount = (float) Math.Sin((double) num4 + (double) this.timer * (double) this.lcg.NextFloat());
              break;
            default:
              amount = Math.Abs((float) Math.Sin((double) num4 + (double) this.timer * (double) Math.Abs(this.lcg.NextFloat() * (flag2 ? 1f : 0.3f))));
              break;
          }
          sb.Draw(this.Hex, Utils.RotatePoint(new Vector2(x, y), angle), new Rectangle?(), Color.Lerp(first, color, amount), angle, Vector2.Zero, new Vector2(this.HexScale), SpriteEffects.None, 0.4f);
          if (algorithm == HexGridBackground.ColoringAlgorithm.OutlinedSinWash)
          {
            if (flag2)
              color = Utils.AddativeRed;
            sb.Draw(this.Outline, Utils.RotatePoint(new Vector2(x, y), angle), new Rectangle?(), Color.Lerp(first, color, amount), angle, Vector2.Zero, new Vector2(this.HexScale), SpriteEffects.None, 0.4f);
          }
          ++seed;
          y += num3 + num1;
        }
        x += num2 - 60f * this.HexScale + num1;
        y = (float) dest.Y;
        ++seed;
        if (flag1)
          y -= num3 / 2f;
        flag1 = !flag1;
      }
    }

    public enum ColoringAlgorithm
    {
      NegaitiveSinWash,
      CorrectedSinWash,
      OutlinedSinWash,
    }
  }
}
