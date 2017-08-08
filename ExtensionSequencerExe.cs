// Decompiled with JetBrains decompiler
// Type: Hacknet.ExtensionSequencerExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Extensions;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class ExtensionSequencerExe : ExeModule
  {
    public static int ACTIVATING_RAM_COST = 170;
    public static int BASE_RAM_COST = 60;
    public static float RAM_CHANGE_PS = 100f;
    public static double Song_Length = 186.0;
    private static float TimeBetweenBeats = 1.832061f;
    private MovingBarsEffect bars = new MovingBarsEffect();
    private string oldSongName = (string) null;
    private int targetRamUse = ExtensionSequencerExe.ACTIVATING_RAM_COST;
    private float stateTimer = 0.0f;
    private float beatHits = 0.15f;
    private double beatDropTime = 16.64;
    private List<ConnectedNodeEffect> nodeeffects = new List<ConnectedNodeEffect>();
    private ExtensionSequencerExe.SequencerExeState state = ExtensionSequencerExe.SequencerExeState.Unavaliable;
    private bool HasBeenKilled = false;
    private string targetID;
    private string flagForProgressionName;
    private Computer targetComp;

    public ExtensionSequencerExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.name = "ESequencer";
      this.ramCost = ExtensionSequencerExe.ACTIVATING_RAM_COST;
      this.IdentifierName = "ESequencer";
      this.targetIP = this.os.thisComputer.ip;
      this.bars.MinLineChangeTime = 1f;
      this.bars.MaxLineChangeTime = 3f;
      this.beatDropTime = (double) ExtensionLoader.ActiveExtensionInfo.SequencerSpinUpTime;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
      this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
      this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
      this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
      this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
      this.targetID = ExtensionLoader.ActiveExtensionInfo.SequencerTargetID;
      this.flagForProgressionName = ExtensionLoader.ActiveExtensionInfo.SequencerFlagRequiredForStart;
    }

    public override void Update(float t)
    {
      base.Update(t);
      if (this.HasBeenKilled)
        return;
      this.bars.Update(t);
      this.UpdateRamCost(t);
      this.stateTimer += t;
      if (this.isExiting)
        this.os.netMap.DimNonConnectedNodes = false;
      bool flag = true;
      if (!string.IsNullOrWhiteSpace(this.flagForProgressionName))
      {
        string[] strArray = this.flagForProgressionName.Split(Utils.newlineDelim, StringSplitOptions.RemoveEmptyEntries);
        for (int index = 0; index < strArray.Length; ++index)
          flag = ((flag ? 1 : 0) & (this.os.Flags.HasFlag(this.flagForProgressionName) ? 1 : (Settings.debugCommandsEnabled ? 1 : 0))) != 0;
      }
      switch (this.state)
      {
        case ExtensionSequencerExe.SequencerExeState.Unavaliable:
          if (flag)
          {
            this.state = ExtensionSequencerExe.SequencerExeState.AwaitingActivation;
            break;
          }
          this.bars.MinLineChangeTime = 1f;
          this.bars.MaxLineChangeTime = 3f;
          break;
        case ExtensionSequencerExe.SequencerExeState.AwaitingActivation:
          if (flag)
            break;
          this.state = ExtensionSequencerExe.SequencerExeState.Unavaliable;
          break;
        case ExtensionSequencerExe.SequencerExeState.SpinningUp:
          if (MediaPlayer.State == MediaState.Playing)
          {
            if (MediaPlayer.PlayPosition.TotalSeconds < this.beatDropTime || (double) this.stateTimer <= this.beatDropTime - 0.5)
              break;
            this.MoveToActiveState();
            break;
          }
          if ((double) this.stateTimer > (double) ExtensionLoader.ActiveExtensionInfo.SequencerSpinUpTime)
            this.MoveToActiveState();
          break;
        case ExtensionSequencerExe.SequencerExeState.Active:
          if ((double) this.stateTimer < 2.5 && (MediaPlayer.PlayPosition.TotalSeconds - this.beatDropTime) % (double) this.beatHits < 0.00999999977648258)
            this.os.warningFlash();
          this.ActiveStateUpdate(t);
          break;
      }
    }

    private void ActiveStateUpdate(float t)
    {
      if (((double) this.stateTimer - this.beatDropTime) % (double) ExtensionSequencerExe.TimeBetweenBeats >= (double) t)
        return;
      this.os.warningFlash();
    }

    private void UpdateRamCost(float t)
    {
      if (this.targetRamUse == this.ramCost)
        return;
      if (this.targetRamUse < this.ramCost)
      {
        this.ramCost -= (int) ((double) t * (double) ExtensionSequencerExe.RAM_CHANGE_PS);
        if (this.ramCost < this.targetRamUse)
          this.ramCost = this.targetRamUse;
      }
      else
      {
        int num = (int) ((double) t * (double) ExtensionSequencerExe.RAM_CHANGE_PS);
        if (this.os.ramAvaliable >= num)
        {
          this.ramCost += num;
          if (this.ramCost > this.targetRamUse)
            this.ramCost = this.targetRamUse;
        }
      }
    }

    public override void Killed()
    {
      base.Killed();
      if (this.state == ExtensionSequencerExe.SequencerExeState.Active)
      {
        this.os.delayer.Post(ActionDelayer.NextTick(), (Action) (() =>
        {
          this.os.addExe((ExeModule) new ExtensionSequencerExe(this.bounds, this.os, new string[0])
          {
            state = this.state,
            stateTimer = this.stateTimer,
            bounds = this.bounds,
            oldSongName = this.oldSongName,
            nodeeffects = this.nodeeffects,
            ramCost = this.ramCost
          });
        }));
      }
      else
      {
        this.os.netMap.DimNonConnectedNodes = false;
        this.os.runCommand("disconnect");
        if (this.targetComp != null)
          this.os.netMap.visibleNodes.Remove(this.os.netMap.nodes.IndexOf(this.targetComp));
      }
    }

    private void MoveToActiveState()
    {
      this.state = ExtensionSequencerExe.SequencerExeState.Active;
      this.stateTimer = 0.0f;
      this.targetRamUse = ExtensionSequencerExe.BASE_RAM_COST;
      this.os.warningFlashTimer = OS.WARNING_FLASH_TIME;
      this.os.netMap.DimNonConnectedNodes = true;
      this.os.netMap.discoverNode(this.targetComp);
      this.os.runCommand("connect " + this.targetComp.ip);
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      Rectangle rectangle = Utils.InsetRectangle(this.GetContentAreaDest(), 1);
      float amount = this.os.warningFlashTimer / OS.WARNING_FLASH_TIME;
      float minHeight = 2f;
      if ((double) amount > 0.0)
        minHeight += amount * ((float) rectangle.Height - minHeight);
      Color drawColor = Color.Lerp(Utils.AddativeWhite * 0.5f, Utils.AddativeRed, amount);
      this.bars.Draw(this.spriteBatch, this.GetContentAreaDest(), minHeight, 4f, 1f, drawColor);
      switch (this.state)
      {
        case ExtensionSequencerExe.SequencerExeState.Unavaliable:
          this.spriteBatch.Draw(Utils.white, rectangle, Color.Black * 0.5f);
          Rectangle dest = Utils.InsetRectangle(rectangle, 6);
          if (!this.isExiting)
            TextItem.doFontLabelToSize(dest, "LINK UNAVAILABLE", GuiData.titlefont, Utils.AddativeWhite, false, false);
          Rectangle destinationRectangle1 = dest;
          destinationRectangle1.Y += destinationRectangle1.Height - 20;
          destinationRectangle1.Height = 20;
          if (this.isExiting)
            break;
          GuiData.spriteBatch.Draw(Utils.white, destinationRectangle1, Color.Black * 0.5f);
          if (Button.doButton(32711803, destinationRectangle1.X, destinationRectangle1.Y, destinationRectangle1.Width, destinationRectangle1.Height, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
            this.isExiting = true;
          break;
        case ExtensionSequencerExe.SequencerExeState.AwaitingActivation:
          int height = 30;
          Rectangle destinationRectangle2 = new Rectangle(this.bounds.X + 1, this.bounds.Y + this.bounds.Height / 2 - height, this.bounds.Width - 2, height * 2);
          this.spriteBatch.Draw(Utils.white, destinationRectangle2, Color.Black * 0.92f);
          if (!Button.doButton(8310101, this.bounds.X + 10, this.bounds.Y + this.bounds.Height / 2 - height / 2, this.bounds.Width - 20, height, LocaleTerms.Loc("ACTIVATE"), new Color?(this.os.highlightColor)))
            break;
          if (this.os.TraceDangerSequence.IsActive)
          {
            this.os.write("SEQUENCER ERROR: OS reports critical action already in progress.");
          }
          else
          {
            this.stateTimer = 0.0f;
            this.state = ExtensionSequencerExe.SequencerExeState.SpinningUp;
            this.bars.MinLineChangeTime = 0.1f;
            this.bars.MaxLineChangeTime = 1f;
            MusicManager.FADE_TIME = 0.6f;
            this.oldSongName = MusicManager.currentSongName;
            this.targetComp = Programs.getComputer(this.os, this.targetID);
            WebServerDaemon daemon = (WebServerDaemon) this.targetComp.getDaemon(typeof (WebServerDaemon));
            if (daemon != null)
              daemon.LoadWebPage("index.html");
            RunnableConditionalActions.LoadIntoOS(ExtensionLoader.ActiveExtensionInfo.ActionsToRunOnSequencerStart, (object) this.os);
          }
          break;
        case ExtensionSequencerExe.SequencerExeState.SpinningUp:
          Rectangle bounds = rectangle;
          bounds.Height = (int) ((double) bounds.Height * ((double) this.stateTimer / (double) ExtensionLoader.ActiveExtensionInfo.SequencerSpinUpTime));
          bounds.Y = rectangle.Y + rectangle.Height - bounds.Height + 1;
          bounds.Width += 4;
          this.bars.Draw(this.spriteBatch, bounds, minHeight, 4f, 1f, this.os.brightLockedColor);
          break;
        case ExtensionSequencerExe.SequencerExeState.Active:
          this.spriteBatch.Draw(Utils.white, this.GetContentAreaDest(), Color.Black * 0.5f);
          TextItem.doFontLabelToSize(this.GetContentAreaDest(), " G O   G O   G O ", GuiData.titlefont, Color.Lerp(Utils.AddativeRed, this.os.brightLockedColor, Math.Min(1f, this.stateTimer / 2f)), false, false);
          this.DrawActiveState();
          break;
      }
    }

    private void DrawActiveState()
    {
      float val1 = 5.2f;
      float point = Math.Min(val1, this.stateTimer) / val1;
      float num1 = 30f;
      Vector2 vector2 = new Vector2((float) this.os.netMap.bounds.X, (float) this.os.netMap.bounds.Y) + new Vector2((float) NetworkMap.NODE_SIZE / 2f);
      for (int index = 0; index < this.nodeeffects.Count; ++index)
      {
        float num2 = (float) (index + 1) / (float) (this.nodeeffects.Count + 1);
        float num3 = 3f * num2;
        float num4 = 1f - Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(point)));
        this.nodeeffects[index].ScaleFactor = num2 * (num1 * num4) + num3;
        this.nodeeffects[index].draw(this.spriteBatch, vector2 + this.os.netMap.GetNodeDrawPos(this.targetComp));
      }
    }

    private enum SequencerExeState
    {
      Unavaliable,
      AwaitingActivation,
      SpinningUp,
      Active,
    }
  }
}
