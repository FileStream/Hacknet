// Decompiled with JetBrains decompiler
// Type: Hacknet.HubServerAlertsIcon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  public class HubServerAlertsIcon
  {
    public bool IsEnabled = true;
    private bool PendingAlerts = false;
    private List<float> RadiusCircles = new List<float>();
    private Texture2D icon;
    private Texture2D circle;
    private SoundEffect alertSound;
    private string syncServerName;
    private IMonitorableDaemon HubServer;
    private Computer HubServerComp;
    private string[] TagsToAlertFor;
    private OS os;

    public HubServerAlertsIcon(ContentManager content, string serverToSyncTo, string[] tagsToAlertFor)
    {
      if (DLC1SessionUpgrader.HasDLC1Installed)
      {
        this.icon = content.Load<Texture2D>("DLC/Icons/ChatWideIcon");
        this.alertSound = content.Load<SoundEffect>("DLC/SFX/NotificationSound");
      }
      else
      {
        this.icon = content.Load<Texture2D>("Sprites/Misc/ChatWideIcon");
        this.alertSound = content.Load<SoundEffect>("SFX/EmailSound");
      }
      this.circle = content.Load<Texture2D>("CircleOutlineLarge");
      this.syncServerName = serverToSyncTo;
      this.TagsToAlertFor = tagsToAlertFor;
    }

    public void Init(object OSobj)
    {
      this.os = (OS) OSobj;
      this.HubServerComp = Programs.getComputer(this.os, this.syncServerName);
      if (this.HubServerComp == null && Settings.IsInExtensionMode)
        return;
      this.HubServer = (IMonitorableDaemon) (this.HubServerComp.getDaemon(typeof (DLCHubServer)) as DLCHubServer);
      if (this.HubServer == null)
        throw new NullReferenceException("DLCHubServer not found on AlertsIcon sync destination computer");
      this.HubServer.SubscribeToAlertActionFroNewMessage(new Action<string, string>(this.ProcessNewLog));
    }

    public void UpdateTarget(object monitor, object comp)
    {
      if (this.HubServer != null)
        this.HubServer.UnSubscribeToAlertActionFroNewMessage(new Action<string, string>(this.ProcessNewLog));
      this.HubServer = (IMonitorableDaemon) monitor;
      this.HubServerComp = (Computer) comp;
      this.HubServer.SubscribeToAlertActionFroNewMessage(new Action<string, string>(this.ProcessNewLog));
    }

    private void ProcessNewLog(string Author, string Message)
    {
      for (int index = 0; index < this.TagsToAlertFor.Length; ++index)
      {
        if (Message.Contains(this.TagsToAlertFor[index]))
        {
          if (!this.HubServer.ShouldDisplayNotifications())
            break;
          this.SendAlert();
          break;
        }
      }
    }

    private void SendAlert()
    {
      if (!Settings.soundDisabled)
        this.alertSound.Play();
      this.RadiusCircles.Add(0.0f);
      if (this.os.connectedComp == this.HubServerComp)
        return;
      this.PendingAlerts = true;
    }

    public void Update(float dt)
    {
      for (int index1 = 0; index1 < this.RadiusCircles.Count; ++index1)
      {
        List<float> radiusCircles;
        int index2;
        (radiusCircles = this.RadiusCircles)[index2 = index1] = radiusCircles[index2] + dt * 0.2f;
        if ((double) this.RadiusCircles[index1] > 1.0)
        {
          this.RadiusCircles.RemoveAt(index1);
          --index1;
        }
      }
    }

    private void ConnectToServer()
    {
      if (this.os.terminal.preventingExecution)
        return;
      if (!this.os.netMap.visibleNodes.Contains(this.os.netMap.nodes.IndexOf(this.HubServerComp)))
        this.os.netMap.discoverNode(this.HubServerComp);
      this.os.connectedComp = this.HubServerComp;
      this.HubServerComp.userLoggedIn = true;
      this.HubServer.navigatedTo();
      this.os.display.command = this.HubServer.GetName();
      this.PendingAlerts = false;
    }

    public void Draw(Rectangle dest, SpriteBatch sb)
    {
      if (Button.doButton(4568702, dest.X, dest.Y, dest.Width, dest.Height, "", new Color?(this.os.topBarIconsColor), this.icon) && this.IsEnabled)
        this.ConnectToServer();
      if (this.RadiusCircles.Count > 0)
      {
        for (int index = 0; index < this.RadiusCircles.Count; ++index)
        {
          float renderSize = (float) (320.0 - (double) this.RadiusCircles[index] * 240.0) * Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(this.RadiusCircles[index]));
          int activeIndex = index;
          this.os.postFXDrawActions += (Action) (() => sb.Draw(this.circle, Utils.InsetRectangle(new Rectangle(dest.X + dest.Width / 2, dest.Y + dest.Height / 2, 1, 1), (int) renderSize), this.os.highlightColor * (1f - this.RadiusCircles[activeIndex])));
        }
      }
      else if (this.PendingAlerts && (double) this.os.timer % 2.0 < 0.0322580635547638)
        SFX.addCircle(new Vector2((float) dest.X, (float) dest.Y) + new Vector2((float) (dest.Width / 2), (float) (dest.Height / 2)), this.os.highlightColor, 40f);
    }
  }
}
