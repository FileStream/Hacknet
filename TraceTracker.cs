// Decompiled with JetBrains decompiler
// Type: Hacknet.TraceTracker
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet
{
  internal class TraceTracker
  {
    public float timeSinceFreezeRequest = 0.0f;
    public float trackSpeedFactor = 1f;
    private OS os;
    private float timer;
    private float lastFrameTime;
    private float startingTimer;
    public bool active;
    private static SpriteFont font;
    private static SoundEffect beep;
    private Computer target;
    private string drawtext;
    private Color timerColor;

    public TraceTracker(OS _os)
    {
      this.os = _os;
      this.timerColor = new Color(170, 0, 0);
      this.timer = 0.0f;
      this.active = false;
      if (TraceTracker.font == null)
      {
        TraceTracker.font = this.os.content.Load<SpriteFont>("Kremlin");
        TraceTracker.font.Spacing = 11f;
        TraceTracker.beep = this.os.content.Load<SoundEffect>("SFX/beep");
      }
      this.drawtext = "TRACE :";
    }

    public void Update(float t)
    {
      bool flag = false;
      this.timeSinceFreezeRequest += t;
      if ((double) this.timeSinceFreezeRequest < 0.0)
        this.timeSinceFreezeRequest = 1f;
      if ((double) this.timeSinceFreezeRequest < 0.200000002980232)
        flag = true;
      if (!this.active)
        return;
      if (this.os.connectedComp == null || !this.os.connectedComp.ip.Equals(this.target.ip))
      {
        this.active = false;
        if ((double) this.timer < 0.5)
          AchievementsManager.Unlock("trace_close", false);
        if (this.os.connectedComp == null)
          Console.WriteLine("Trace Ended - connection null");
        else
          Console.WriteLine("Trace Ended - connection changed - was " + this.target.ip + " now is " + this.os.connectedComp.ip);
      }
      else if (!flag)
      {
        float num = Settings.AllTraceTimeSlowed ? 0.055f : 1f;
        this.timer -= t * this.trackSpeedFactor * num;
        if ((double) this.timer <= 0.0)
        {
          this.timer = 0.0f;
          this.active = false;
          this.os.timerExpired();
        }
      }
      float num1 = (float) ((double) this.timer / (double) this.startingTimer * 100.0);
      float num2 = (double) num1 < 45.0 ? ((double) num1 < 15.0 ? 1f : 5f) : 10f;
      if ((double) num1 % (double) num2 > (double) this.lastFrameTime % (double) num2)
      {
        TraceTracker.beep.Play(0.5f, 0.0f, 0.0f);
        this.os.warningFlash();
      }
      this.lastFrameTime = num1;
    }

    public void start(float t)
    {
      if (this.active)
        return;
      this.trackSpeedFactor = 1f;
      this.startingTimer = t;
      this.timer = t;
      this.active = true;
      this.os.warningFlash();
      this.target = this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp;
      Console.WriteLine("Warning flash");
    }

    public void stop()
    {
      this.active = false;
      this.trackSpeedFactor = 1f;
    }

    public void Draw(SpriteBatch sb)
    {
      if (!this.active)
        return;
      string text = ((float) ((double) this.timer / (double) this.startingTimer * 100.0)).ToString("00.00");
      Vector2 vector2 = TraceTracker.font.MeasureString(text);
      Vector2 position = new Vector2(10f, (float) sb.GraphicsDevice.Viewport.Height - vector2.Y);
      sb.DrawString(TraceTracker.font, text, position, this.timerColor);
      position.Y -= 25f;
      sb.DrawString(TraceTracker.font, this.drawtext, position, this.timerColor, 0.0f, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0.5f);
    }
  }
}
