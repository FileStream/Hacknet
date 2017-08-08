// Decompiled with JetBrains decompiler
// Type: Hacknet.ConnectedNodeEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class ConnectedNodeEffect
  {
    public bool Intense = false;
    public float ScaleFactor = 1f;
    private const int NUMBER_OF_SEGMENTS = 7;
    private const float MIN_DISTANCE = 18f;
    private const float MAX_DISTANCE = 30f;
    private OS os;
    private static List<Texture2D> textures;
    public Color color;
    private int[] tex;
    private float[] distance;
    private float[] offset;
    private float[] timescale;
    private Vector2 origin;

    public ConnectedNodeEffect(OS os)
    {
      this.os = os;
      this.init(false);
    }

    public ConnectedNodeEffect(OS os, bool intense)
    {
      this.os = os;
      this.Intense = intense;
      this.init(intense);
    }

    private void init(bool intesne = false)
    {
      if (ConnectedNodeEffect.textures == null)
      {
        ConnectedNodeEffect.textures = new List<Texture2D>();
        ConnectedNodeEffect.textures.Add(this.os.content.Load<Texture2D>("rotator"));
        ConnectedNodeEffect.textures.Add(this.os.content.Load<Texture2D>("rotator2"));
        ConnectedNodeEffect.textures.Add(this.os.content.Load<Texture2D>("rotator3"));
        ConnectedNodeEffect.textures.Add(this.os.content.Load<Texture2D>("rotator4"));
        ConnectedNodeEffect.textures.Add(this.os.content.Load<Texture2D>("rotator5"));
      }
      this.origin = new Vector2((float) (ConnectedNodeEffect.textures[0].Width / 2), (float) (ConnectedNodeEffect.textures[0].Height / 2));
      int length = (intesne ? 2 : 1) * 7;
      this.tex = new int[length];
      this.distance = new float[length];
      this.offset = new float[length];
      this.timescale = new float[length];
      this.color = new Color(140, 12, 12, 50);
      this.reset();
    }

    public void reset()
    {
      float num = this.Intense ? 1.5f : 1f;
      for (int index = 0; index < 7; ++index)
      {
        this.tex[index] = Utils.random.Next(ConnectedNodeEffect.textures.Count - 1);
        this.distance[index] = (float) (Utils.random.NextDouble() * (30.0 * (double) num - 18.0 / (double) num) + 18.0 / (double) num);
        this.offset[index] = (float) (Utils.random.NextDouble() * (2.0 * Math.PI));
        this.timescale[index] = (float) Utils.random.NextDouble();
      }
    }

    public void draw(SpriteBatch sb, Vector2 pos)
    {
      for (int index = 0; index < 7; ++index)
      {
        Vector2 origin = this.origin + new Vector2(0.0f, this.distance[index]);
        float rotation = (float) ((double) this.os.timer * ((double) this.timescale[index] * 0.200000002980232) % 1.0 * (2.0 * Math.PI) * (index % 2 == 0 ? 1.0 : -1.0)) + this.offset[index];
        Vector2 scale = new Vector2((float) ((double) this.distance[index] / (30.0 * (this.Intense ? 1.5 : 1.0)) * 0.699999988079071 + 0.300000011920929), 1f);
        scale *= this.ScaleFactor;
        sb.Draw(ConnectedNodeEffect.textures[this.tex[index]], pos, new Rectangle?(), this.color, rotation, origin, scale, SpriteEffects.None, 0.5f);
      }
    }
  }
}
