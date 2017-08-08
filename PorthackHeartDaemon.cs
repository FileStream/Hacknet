// Decompiled with JetBrains decompiler
// Type: Hacknet.PorthackHeartDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet
{
  internal class PorthackHeartDaemon : Daemon
  {
    private float playTimeExpended = 0.0f;
    private bool PlayingHeartbreak = false;
    private PortHackCubeSequence pcs = new PortHackCubeSequence();
    private float FadeoutDelay = 1f;
    private float FadeoutDuration = 10f;
    private bool IsFlashingOut = false;
    private float flashOutTime = 0.0f;
    private RenderTarget2D rendertarget;
    private SpriteBatch rtSpritebatch;
    private SoundEffect SpinDownEffect;
    private SoundEffect glowSoundEffect;

    public PorthackHeartDaemon(Computer c, OS os)
      : base(c, "Porthack.Heart", os)
    {
      this.name = "Porthack.Heart";
      this.SpinDownEffect = os.content.Load<SoundEffect>("SFX/TraceKill");
      this.glowSoundEffect = os.content.Load<SoundEffect>("SFX/Ending/PorthackSpindown");
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
    }

    private void UpdateForTime(Rectangle bounds, SpriteBatch sb)
    {
      if ((double) this.playTimeExpended > (double) this.FadeoutDelay)
      {
        float fade = Math.Min(1f, (this.playTimeExpended - this.FadeoutDelay) / this.FadeoutDuration);
        Rectangle correctedbounds = new Rectangle(bounds.X, bounds.Y - Module.PANEL_HEIGHT, bounds.Width, bounds.Height + Module.PANEL_HEIGHT);
        this.os.postFXDrawActions += (Action) (() => Utils.FillEverywhereExcept(correctedbounds, this.os.fullscreen, sb, Color.Black * fade * 0.8f));
      }
      if (this.pcs.HeartFadeSequenceComplete)
      {
        this.IsFlashingOut = true;
        this.flashOutTime += (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
        float num = 3.8f;
        if ((double) this.flashOutTime > (double) num)
        {
          this.flashOutTime = num;
          this.os.canRunContent = false;
          this.os.endingSequence.IsActive = true;
          PostProcessor.EndingSequenceFlashOutActive = false;
          PostProcessor.EndingSequenceFlashOutPercentageComplete = 0.0f;
          return;
        }
        PostProcessor.EndingSequenceFlashOutPercentageComplete = this.flashOutTime / num;
      }
      else
        this.IsFlashingOut = false;
      PostProcessor.EndingSequenceFlashOutActive = this.IsFlashingOut;
    }

    public void BreakHeart()
    {
      if (this.os.TraceDangerSequence.IsActive)
        this.os.TraceDangerSequence.CancelTraceDangerSequence();
      this.os.RequestRemovalOfAllPopups();
      this.PlayingHeartbreak = true;
      this.os.terminal.inputLocked = true;
      this.os.netMap.inputLocked = true;
      this.os.ram.inputLocked = true;
      this.os.DisableTopBarButtons = true;
      MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
      this.SpinDownEffect.Play();
      this.os.delayer.Post(ActionDelayer.Wait(18.0), (Action) (() => this.glowSoundEffect.Play()));
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      try
      {
        int width = bounds.Width;
        int height = bounds.Height;
        if (this.rendertarget == null || this.rendertarget.Width != width || this.rendertarget.Height != height)
        {
          if (this.rtSpritebatch == null)
            this.rtSpritebatch = new SpriteBatch(sb.GraphicsDevice);
          if (this.rendertarget != null)
            this.rendertarget.Dispose();
          this.rendertarget = new RenderTarget2D(sb.GraphicsDevice, width, height);
        }
        if (!this.PlayingHeartbreak)
        {
          TextItem.DrawShadow = false;
          TextItem.doFontLabel(new Vector2((float) (bounds.X + 6), (float) (bounds.Y + 2)), Utils.FlipRandomChars("PortHack.Heart", 0.003), GuiData.font, new Color?(Utils.AddativeWhite * 0.6f), (float) (bounds.Width - 10), 100f, false);
          TextItem.doFontLabel(new Vector2((float) (bounds.X + 6), (float) (bounds.Y + 2)), Utils.FlipRandomChars("PortHack.Heart", 0.1), GuiData.font, new Color?(Utils.AddativeWhite * 0.2f), (float) (bounds.Width - 10), 100f, false);
        }
        if (this.PlayingHeartbreak)
          this.playTimeExpended += (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
        this.UpdateForTime(bounds, sb);
        RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
        sb.GraphicsDevice.SetRenderTarget(this.rendertarget);
        sb.GraphicsDevice.Clear(Color.Transparent);
        this.rtSpritebatch.Begin();
        Rectangle dest = new Rectangle(0, 0, bounds.Width, bounds.Height);
        Vector3 vector3_1 = new Vector3(MathHelper.ToRadians(35.4f), MathHelper.ToRadians(45f), MathHelper.ToRadians(0.0f));
        Vector3 vector3_2 = new Vector3(1f, 1f, 0.0f) * this.os.timer * 0.2f + new Vector3(this.os.timer * 0.1f, this.os.timer * -0.4f, 0.0f);
        float num = 2.5f;
        if (this.PlayingHeartbreak)
        {
          if ((double) this.playTimeExpended < (double) num)
            Cube3D.RenderWireframe(Vector3.Zero, 2.6f, Vector3.Lerp(Utils.NormalizeRotationVector(vector3_2), vector3_1, Utils.QuadraticOutCurve(this.playTimeExpended / num)), Color.White);
          else
            this.pcs.DrawHeartSequence(dest, (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds, 30f);
        }
        else
          Cube3D.RenderWireframe(new Vector3(0.0f, 0.0f, 0.0f), 2.6f, vector3_2, Color.White);
        this.rtSpritebatch.End();
        sb.GraphicsDevice.SetRenderTarget(currentRenderTarget);
        Rectangle rectangle = new Rectangle(bounds.X + (bounds.Width - width) / 2, bounds.Y + (bounds.Height - height) / 2, width, height);
        float rarity = Math.Min(1f, (float) ((double) this.playTimeExpended / (double) num * 0.800000011920929 + 0.200000002980232));
        FlickeringTextEffect.DrawFlickeringSprite(sb, rectangle, (Texture2D) this.rendertarget, 4f, rarity, (object) this.os, Color.White);
        sb.Draw((Texture2D) this.rendertarget, rectangle, Utils.AddativeWhite * 0.7f);
      }
      catch (Exception ex)
      {
        Console.WriteLine(Utils.GenerateReportFromException(ex));
      }
    }

    public override string getSaveString()
    {
      return "<porthackheart name=\"" + this.name + "\"/>";
    }
  }
}
