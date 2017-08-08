// Decompiled with JetBrains decompiler
// Type: Hacknet.TraceKillExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class TraceKillExe : ExeModule
  {
    private static Color BoatBarColor = new Color(0, 0, 0, 200);
    private Vector2 EffectFocus = new Vector2(0.5f);
    private float timer = 0.0f;
    private Vector2 focusPointLocation = Vector2.Zero;
    private float timeTillNextFocusPoint = 1f;
    private bool isOnFocusPoint = false;
    private float focusPointTransitionTime = 0.0f;
    private float timeOnFocusPoint = 0.0f;
    private float focusPointIdleTime = 0.0f;
    private bool hasDoneBurstForThisFocusPoint = false;
    private List<TraceKillExe.PointImpactEffect> ImpactEffects = new List<TraceKillExe.PointImpactEffect>();
    private float traceActivityTimer = 0.0f;
    private const float TIME_BETWEEN_FOCUS_POINTS = 4f;
    private const float FOCUS_POINT_TRANSITION_TIME = 1.2f;
    private const float MAX_FOCUS_POINT_IDLE_TIME = 0.1f;
    private RenderTarget2D effectTarget;
    private SpriteBatch effectSB;
    private SoundEffect traceKillSound;
    private Texture2D circle;
    private BarcodeEffect BotBarcode;

    public TraceKillExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.name = "TraceKill";
      this.ramCost = 600;
      this.IdentifierName = "TraceKill";
      this.targetIP = this.os.thisComputer.ip;
      this.effectSB = new SpriteBatch(GuiData.spriteBatch.GraphicsDevice);
      this.circle = this.os.content.Load<Texture2D>("Circle");
      this.traceKillSound = this.os.content.Load<SoundEffect>("SFX/TraceKill");
    }

    public override void Update(float t)
    {
      base.Update(t);
      this.UpdateEffect(t);
      if (!this.isExiting && (this.os.connectedComp == null || !(this.os.connectedComp.idName == "dGibson")))
      {
        if ((double) this.os.traceTracker.timeSinceFreezeRequest > 0.25)
          this.traceKillSound.Play();
        this.os.traceTracker.timeSinceFreezeRequest = 0.0f;
      }
      if (this.BotBarcode != null)
        this.BotBarcode.Update(t * (this.os.traceTracker.active ? 3f : 2f));
      if (this.os.traceTracker.active)
        this.traceActivityTimer += t;
      else
        this.traceActivityTimer = 0.0f;
    }

    private void UpdateEffect(float t)
    {
      float num = 1f;
      if (this.os.traceTracker.active)
        num = 1.6f;
      this.timer += t * num;
      Vector2 vector2 = new Vector2((float) Math.Sin((double) this.timer * 2.0), (float) Math.Sin((double) this.timer)) * 0.4f + new Vector2(0.5f);
      if (this.isOnFocusPoint && (double) this.focusPointTransitionTime >= 1.20000004768372)
      {
        this.EffectFocus = this.focusPointLocation;
        this.timeOnFocusPoint += t;
        if (!this.hasDoneBurstForThisFocusPoint)
        {
          this.ImpactEffects.Add(new TraceKillExe.PointImpactEffect(this.focusPointLocation, this.os));
          this.hasDoneBurstForThisFocusPoint = true;
        }
        if ((double) this.timeOnFocusPoint >= (double) this.focusPointIdleTime)
        {
          this.isOnFocusPoint = false;
          this.focusPointTransitionTime = 1.2f;
          this.timeTillNextFocusPoint = Utils.randm(4f);
        }
      }
      else if (this.isOnFocusPoint && (double) this.focusPointTransitionTime < 1.20000004768372 || !this.isOnFocusPoint && (double) this.focusPointTransitionTime > 0.0)
      {
        this.focusPointTransitionTime += t * (this.isOnFocusPoint ? 1f : -1f);
        float point = this.focusPointTransitionTime / 1.2f;
        this.EffectFocus = Vector2.Lerp(vector2, this.focusPointLocation, Utils.QuadraticOutCurve(point));
      }
      else
      {
        this.EffectFocus = vector2;
        this.timeTillNextFocusPoint -= t;
        if ((double) this.timeTillNextFocusPoint <= 0.0)
        {
          this.isOnFocusPoint = true;
          this.focusPointTransitionTime = 0.0f;
          this.timeOnFocusPoint = 0.0f;
          this.hasDoneBurstForThisFocusPoint = false;
          this.focusPointIdleTime = Utils.randm(0.1f);
          this.focusPointLocation = new Vector2(Utils.randm(1f), Utils.randm(1f));
        }
      }
      for (int index = 0; index < this.ImpactEffects.Count; ++index)
      {
        TraceKillExe.PointImpactEffect impactEffect = this.ImpactEffects[index];
        impactEffect.timeEnabled += t;
        if ((double) impactEffect.timeEnabled > 3.0)
        {
          this.ImpactEffects.RemoveAt(index);
          --index;
        }
        else
          this.ImpactEffects[index] = impactEffect;
      }
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      this.DrawEffect(t);
      if (this.bounds.Height > 100)
      {
        Rectangle dest = new Rectangle(this.bounds.X + 1, this.bounds.Y + 60, (int) ((double) this.bounds.Width * 0.7), 40);
        TextItem.doRightAlignedBackingLabelFill(dest, this.os.traceTracker.active ? "SUPPRESSION ACTIVE" : "        SCANNING...", GuiData.titlefont, TraceKillExe.BoatBarColor, this.os.traceTracker.active ? Color.Lerp(Color.Red, this.os.brightLockedColor, Utils.rand(1f)) : Color.White * 0.8f);
        if (!this.os.traceTracker.active)
        {
          dest.Y += dest.Height + 2;
          dest.Height = 16;
          TextItem.doRightAlignedBackingLabel(dest, "TraceKill v0.8011", GuiData.detailfont, TraceKillExe.BoatBarColor, Color.DarkGray);
        }
      }
      if (this.bounds.Height <= 40)
        return;
      if (this.BotBarcode == null)
        this.BotBarcode = new BarcodeEffect(this.bounds.Width - 2, true, false);
      int maxHeight = 12;
      int height = 22;
      this.BotBarcode.Draw(this.bounds.X + 1, this.bounds.Y + this.bounds.Height - (maxHeight + height), this.bounds.Width - 2, maxHeight, this.spriteBatch, new Color?(TraceKillExe.BoatBarColor));
      Rectangle destinationRectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + this.bounds.Height - height - 1, this.bounds.Width - 2, height);
      this.spriteBatch.Draw(Utils.white, destinationRectangle, TraceKillExe.BoatBarColor);
      int width = 130;
      if (Button.doButton(34004301, destinationRectangle.X + destinationRectangle.Width - width - 2, destinationRectangle.Y + 2, width, destinationRectangle.Height - 4, "EXIT", new Color?(this.os.brightLockedColor)))
        this.isExiting = true;
      Rectangle dest1 = new Rectangle(destinationRectangle.X + 2, destinationRectangle.Y + 5, destinationRectangle.Width - width - 4, destinationRectangle.Height / 2);
      TextItem.doRightAlignedBackingLabel(dest1, this.traceActivityTimer.ToString("0.000000"), GuiData.detailfont, Color.Transparent, Color.White * 0.8f);
      dest1.Y += dest1.Height - 2;
      TextItem.doRightAlignedBackingLabel(dest1, this.os.connectedComp != null ? this.os.connectedComp.ip : "UNKNOWN", GuiData.detailfont, Color.Transparent, Color.White * 0.8f);
    }

    private void DrawEffect(float t)
    {
      int num = 16;
      Rectangle destinationRectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + num, this.bounds.Width - 2, this.bounds.Height - (num + 2));
      if (this.effectTarget == null || this.effectTarget.Width != destinationRectangle.Width || this.effectTarget.Height < destinationRectangle.Height)
      {
        if (this.effectTarget != null)
          this.effectTarget.Dispose();
        this.effectTarget = new RenderTarget2D(this.spriteBatch.GraphicsDevice, destinationRectangle.Width, destinationRectangle.Height, false, SurfaceFormat.Rgba64, DepthFormat.None, 4, RenderTargetUsage.PlatformContents);
      }
      RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
      this.spriteBatch.GraphicsDevice.SetRenderTarget(this.effectTarget);
      this.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
      this.effectSB.Begin();
      Rectangle dest = destinationRectangle;
      dest.X = 0;
      dest.Y = 0;
      this.DrawEffectFill(this.effectSB, dest);
      this.DrawImpactEffects(this.effectSB, dest);
      this.effectSB.End();
      this.spriteBatch.GraphicsDevice.SetRenderTarget(currentRenderTarget);
      Rectangle rectangle = destinationRectangle;
      rectangle.X = rectangle.Y = 0;
      this.spriteBatch.Draw((Texture2D) this.effectTarget, destinationRectangle, new Rectangle?(rectangle), Color.White);
    }

    private void DrawImpactEffects(SpriteBatch sb, Rectangle dest)
    {
      Color color = this.os.traceTracker.active ? Color.Red : Utils.AddativeWhite;
      for (int index = 0; index < this.ImpactEffects.Count; ++index)
      {
        TraceKillExe.PointImpactEffect impactEffect = this.ImpactEffects[index];
        Vector2 vector2 = new Vector2((float) dest.X + impactEffect.location.X * (float) dest.Width, (float) dest.Y + impactEffect.location.Y * (float) dest.Height);
        if ((double) impactEffect.timeEnabled <= 1.0)
        {
          float num = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(impactEffect.timeEnabled / 1f));
          impactEffect.cne.color = color * num;
          impactEffect.cne.ScaleFactor = num;
          sb.Draw(this.circle, vector2, new Rectangle?(), color * (1f - num), 0.0f, new Vector2((float) (this.circle.Width / 2), (float) (this.circle.Height / 2)), (float) ((double) num / (double) this.circle.Width * 30.0), SpriteEffects.None, 0.7f);
        }
        else
        {
          float num = Utils.QuadraticOutCurve((float) (((double) impactEffect.timeEnabled - 1.0) / 2.0));
          impactEffect.cne.color = color * (1f - num);
          impactEffect.cne.ScaleFactor = 1f;
        }
        impactEffect.cne.draw(sb, vector2);
      }
    }

    private void DrawEffectFill(SpriteBatch sb, Rectangle dest)
    {
      sb.Draw(Utils.white, dest, this.os.indentBackgroundColor * 0.8f);
      float num1 = 10f;
      float num2 = 2.5f;
      float lineThickness = 2f;
      float num3 = 0.08f;
      Color baseColor = this.os.traceTracker.active ? this.os.lockedColor : Utils.VeryDarkGray * 0.4f;
      Color color = this.os.traceTracker.active ? this.os.brightLockedColor : Color.Gray;
      Vector2 targetPos = this.EffectFocus * new Vector2((float) dest.Width, (float) dest.Height);
      List<Action> actionList = new List<Action>();
      int num4 = 0;
      Vector2 vector2 = new Vector2((float) dest.X - num1 / 2f, (float) dest.Y - num1);
      while ((double) vector2.Y + (double) lineThickness < (double) (dest.Y + dest.Height) + (double) num1)
      {
        vector2.X = (float) dest.X - num1 / 2f;
        while ((double) vector2.X + (double) lineThickness < (double) (dest.X + dest.Width) + (double) num1)
        {
          Vector2 pos = vector2;
          float amount = 1f - Utils.QuadraticOutCurve(Vector2.Distance(pos, targetPos) / (float) dest.Height);
          Color highlightColor = Color.Lerp(baseColor, color, amount);
          float length = Math.Min(num1 * 1.1f, Vector2.Distance(vector2, targetPos));
          this.DrawTracerLine(sb, pos, lineThickness, targetPos, length, baseColor, highlightColor);
          actionList.Add((Action) (() => this.DrawTracerLineShadow(sb, pos, lineThickness, targetPos, length, Color.Black * 0.6f)));
          vector2.X += num1 + lineThickness / 2f;
        }
        ++num4;
        num1 += num2;
        lineThickness += num3;
        vector2.Y += num1;
        if ((double) num1 <= 0.0)
          break;
      }
      for (int index = 0; index < actionList.Count; ++index)
        actionList[index]();
    }

    private void DrawTracerLine(SpriteBatch sb, Vector2 pos, float thickness, Vector2 target, float length, Color baseColor, Color highlightColor)
    {
      float rotation = (float) Math.Atan2((double) target.Y - (double) pos.Y, (double) target.X - (double) pos.X);
      Rectangle destinationRectangle = new Rectangle((int) pos.X, (int) pos.Y, (int) length, (int) thickness);
      sb.Draw(Utils.white, destinationRectangle, new Rectangle?(), baseColor, rotation, Vector2.Zero, SpriteEffects.None, 0.7f);
      sb.Draw(Utils.gradientLeftRight, destinationRectangle, new Rectangle?(), highlightColor, rotation, Vector2.Zero, SpriteEffects.None, 0.7f);
    }

    private void DrawTracerLineShadow(SpriteBatch sb, Vector2 pos, float thickness, Vector2 target, float length, Color color)
    {
      float rotation = (float) Math.Atan2((double) target.Y - (double) pos.Y, (double) target.X - (double) pos.X);
      sb.Draw(Utils.gradient, pos, new Rectangle?(), color, rotation, Vector2.Zero, new Vector2(length, (float) (int) thickness), SpriteEffects.None, 0.6f);
      Rectangle rectangle = new Rectangle((int) pos.X, (int) pos.Y, (int) length, (int) thickness);
    }

    public struct PointImpactEffect
    {
      public const float TransInTime = 1f;
      public const float TransOutTime = 2f;
      public ConnectedNodeEffect cne;
      public float timeEnabled;
      public Vector2 location;
      public float scaleModifier;
      public bool HasHighlightCircle;

      public PointImpactEffect(Vector2 location, OS os)
      {
        this.cne = new ConnectedNodeEffect(os, true);
        this.timeEnabled = 0.0f;
        this.location = location;
        this.scaleModifier = 1f;
        this.HasHighlightCircle = true;
      }
    }
  }
}
