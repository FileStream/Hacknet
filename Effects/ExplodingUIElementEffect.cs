// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.ExplodingUIElementEffect
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet.Effects
{
  public class ExplodingUIElementEffect
  {
    private List<Texture2D> Textures = new List<Texture2D>();
    private List<ExplodingUIElementEffect.ExplosionParticle> Particles = new List<ExplodingUIElementEffect.ExplosionParticle>();

    public void Init(ContentManager content)
    {
      this.Textures.Add(Utils.white);
      this.Textures.Add(Utils.white);
    }

    public void Explode(int particles, Vector2 AngleRange, Vector2 startPos, float minScale, float maxScale, float minSpeed, float maxSpeed, float minFriction, float maxFriction, float minStartFade, float maxStartFade)
    {
      for (int index = 0; index < particles; ++index)
        this.Particles.Add(new ExplodingUIElementEffect.ExplosionParticle()
        {
          Pos = startPos,
          angle = AngleRange.X + Utils.randm(AngleRange.Y - AngleRange.X),
          fade = minStartFade + Utils.randm(maxStartFade - minStartFade),
          rotation = Utils.randm(6.283185f),
          rotationRate = Utils.randm(20f) - Utils.randm(20f),
          Size = minScale + Utils.randm(maxScale - minScale),
          speed = minSpeed + Utils.randm(maxSpeed - minSpeed),
          frictionSlowdownPerSec = minFriction + Utils.randm(minFriction - maxFriction),
          textureIndex = Utils.random.Next(this.Textures.Count),
          FlickerOffset = Utils.randm(50f)
        });
    }

    public void Update(float dt)
    {
      for (int index = 0; index < this.Particles.Count; ++index)
      {
        ExplodingUIElementEffect.ExplosionParticle particle = this.Particles[index];
        particle.Pos += Utils.PolarToCartesian(particle.angle, particle.speed * dt);
        particle.speed -= particle.frictionSlowdownPerSec * dt;
        if ((double) particle.speed <= 0.0)
          particle.speed = 0.0f;
        particle.rotation += particle.rotationRate * dt;
        particle.rotationRate -= (float) ((double) particle.frictionSlowdownPerSec * (double) dt * 0.100000001490116);
        if ((double) particle.rotationRate <= 0.0)
          particle.rotationRate = 0.0f;
        particle.fade -= dt;
        if ((double) particle.fade <= 0.0)
        {
          this.Particles.RemoveAt(index);
          --index;
        }
        else
          this.Particles[index] = particle;
      }
    }

    public void Render(SpriteBatch sb)
    {
      for (int index = 0; index < this.Particles.Count; ++index)
      {
        ExplodingUIElementEffect.ExplosionParticle particle = this.Particles[index];
        Texture2D texture2D = this.Textures[particle.textureIndex];
        if (texture2D.IsDisposed)
          texture2D = Utils.white;
        Vector2 scale = new Vector2(particle.Size / (float) texture2D.Width, particle.Size / (float) texture2D.Width);
        scale = new Vector2(Math.Min(scale.X, scale.Y), Math.Min(scale.X, scale.Y));
        float num = 1f - Math.Min(1f, Math.Max(0.0f, particle.fade / 2f));
        FlickeringTextEffect.DrawFlickeringSpriteFull(sb, particle.Pos, particle.rotation, scale, texture2D.GetCentreOrigin(), texture2D, particle.FlickerOffset, particle.Size, 0.0f, (object) OS.currentInstance, Utils.AddativeWhite * (particle.fade / 4f));
      }
    }

    private struct ExplosionParticle
    {
      public Vector2 Pos;
      public float angle;
      public float speed;
      public float rotation;
      public float rotationRate;
      public float fade;
      public int textureIndex;
      public float Size;
      public float frictionSlowdownPerSec;
      public float FlickerOffset;
    }
  }
}
