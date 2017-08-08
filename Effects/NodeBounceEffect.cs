// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.NodeBounceEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet.Effects
{
  public class NodeBounceEffect
  {
    public float TimeBetweenBounces = 0.07f;
    public float NodeHitDelay = 0.2f;
    private List<Vector2> locations = new List<Vector2>();
    private float timeToNextBounce = 0.0f;
    private float delayTillNextBounce = 0.0f;
    public int maxNodes = 200;

    public NodeBounceEffect()
    {
      this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
      this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
    }

    public void Update(float t, Action<Vector2> nodeHitAction = null)
    {
      this.timeToNextBounce -= t;
      if ((double) this.timeToNextBounce > 0.0)
        return;
      if ((double) this.delayTillNextBounce <= 0.0)
      {
        if (nodeHitAction != null)
          nodeHitAction(this.locations[this.locations.Count - 1]);
        this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
        this.timeToNextBounce = this.TimeBetweenBounces;
        this.delayTillNextBounce = this.NodeHitDelay;
        while (this.locations.Count > this.maxNodes)
          this.locations.RemoveAt(0);
      }
      else
      {
        this.delayTillNextBounce -= t;
        this.timeToNextBounce = 0.0f;
      }
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle bounds, Color lineColor, Color nodeColor)
    {
      Vector2 location = this.locations[0];
      Vector2 vector2_1 = new Vector2((float) bounds.X + 2f, (float) bounds.Y + 26f);
      Vector2 vector2_2 = new Vector2((float) bounds.Width - 4f, (float) bounds.Height - 30f);
      if ((double) vector2_2.X <= 0.0 || (double) vector2_2.Y <= 0.0)
        return;
      for (int index = 1; index < this.locations.Count; ++index)
      {
        Vector2 vector2_3 = this.locations[index];
        if (index == this.locations.Count - 1)
          vector2_3 = Vector2.Lerp(location, vector2_3, (float) (1.0 - (double) this.timeToNextBounce / (double) this.TimeBetweenBounces));
        Utils.drawLine(spriteBatch, vector2_1 + location * vector2_2, vector2_1 + vector2_3 * vector2_2, Vector2.Zero, lineColor * ((float) index / (float) this.locations.Count), 0.4f);
        location = this.locations[index];
      }
      for (int index = 1; index < this.locations.Count; ++index)
        spriteBatch.Draw(Utils.white, this.locations[index] * vector2_2 + vector2_1, nodeColor);
    }
  }
}
