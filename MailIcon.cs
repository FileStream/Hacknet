// Decompiled with JetBrains decompiler
// Type: Hacknet.MailIcon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet
{
  internal class MailIcon : MailResponder
  {
    private static float ALERT_TIME = 2.4f;
    private static float SCALE = 1f;
    private static Color uncheckedMailPulseColor = new Color(110, 110, 110, 0);
    public bool isEnabled = true;
    private float bigTexScaleMod = 1f;
    private float firstEverAlertMod = 2.7f;
    private MailServer targetServer = (MailServer) null;
    private Texture2D tex;
    private Texture2D texBig;
    public Vector2 pos;
    private SoundEffect newMailSound;
    private float alertTimer;
    private bool mailUnchecked;
    private OS os;

    public MailIcon(OS operatingSystem, Vector2 position)
    {
      this.os = operatingSystem;
      this.pos = position;
      this.alertTimer = 0.0f;
      this.tex = this.os.content.Load<Texture2D>("UnopenedMail");
      this.texBig = this.os.content.Load<Texture2D>("MailIconBig");
      this.bigTexScaleMod = (float) this.tex.Width / (float) this.texBig.Width;
      this.newMailSound = this.os.content.Load<SoundEffect>("SFX/EmailSound");
      if (!this.os.multiplayer)
        ((MailServer) this.os.netMap.mailServer.daemons[0]).addResponder((MailResponder) this);
      this.mailUnchecked = false;
    }

    public void UpdateTargetServer(MailServer server)
    {
      if (this.targetServer != null)
        this.targetServer.removeResponder((MailResponder) this);
      else
        (this.os.netMap.mailServer.daemons[0] as MailServer).removeResponder((MailResponder) this);
      this.targetServer = server;
      this.targetServer.addResponder((MailResponder) this);
    }

    public void Update(float t)
    {
      float alertTimer = this.alertTimer;
      if (!this.os.Flags.HasFlag("FirstAlertComplete"))
        this.alertTimer -= t * 0.65f;
      else
        this.alertTimer -= t;
      if ((double) this.alertTimer <= 0.0)
        this.alertTimer = 0.0f;
      if ((double) alertTimer <= 0.0 || (double) this.alertTimer > 0.0)
        return;
      float radius = 280f;
      if (!this.os.Flags.HasFlag("FirstAlertComplete"))
      {
        this.os.Flags.AddFlag("FirstAlertComplete");
        radius *= this.firstEverAlertMod;
      }
      SFX.addCircle(this.pos + new Vector2((float) ((double) this.tex.Width * (double) MailIcon.SCALE / 2.0), (float) ((double) this.tex.Height * (double) MailIcon.SCALE / 2.0)), MailIcon.uncheckedMailPulseColor, radius);
    }

    public void Draw()
    {
      if (Button.doButton(45687, (int) this.pos.X, (int) this.pos.Y, (int) ((double) this.tex.Width * (double) MailIcon.SCALE), (int) ((double) this.tex.Height * (double) MailIcon.SCALE), "", new Color?(this.os.topBarIconsColor), this.tex) && this.isEnabled)
        this.connectToMail();
      float percent = this.alertTimer / MailIcon.ALERT_TIME;
      percent *= percent;
      percent *= percent;
      percent = (float) Math.Pow((double) percent, 1.0 - (double) percent);
      Vector2 iconPositionOffset = new Vector2(-100f, 20f);
      float scaleMod = this.bigTexScaleMod;
      if (!this.os.Flags.HasFlag("FirstAlertComplete"))
      {
        scaleMod += this.firstEverAlertMod * percent;
        iconPositionOffset *= this.firstEverAlertMod * percent;
      }
      if ((double) this.alertTimer > 0.0)
      {
        this.os.postFXDrawActions += (Action) (() =>
        {
          Vector2 origin = new Vector2((float) ((double) this.texBig.Width * (double) MailIcon.SCALE / 2.0), (float) ((double) this.texBig.Height * (double) MailIcon.SCALE / 2.0));
          GuiData.spriteBatch.Draw(this.texBig, this.pos + origin * scaleMod + iconPositionOffset * percent, new Rectangle?(), Color.White * (1f - percent), 0.0f, origin, (Vector2.One + Vector2.One * percent * 25f) * scaleMod, SpriteEffects.None, 0.5f);
        });
      }
      else
      {
        if (!this.mailUnchecked || (double) this.os.timer % 2.0 >= 0.0322580635547638)
          return;
        SFX.addCircle(this.pos + new Vector2((float) ((double) this.tex.Width * (double) MailIcon.SCALE / 2.0), (float) ((double) this.tex.Height * (double) MailIcon.SCALE / 2.0)), MailIcon.uncheckedMailPulseColor, 40f);
      }
    }

    public int getWidth()
    {
      return (int) ((double) this.tex.Width * (double) MailIcon.SCALE);
    }

    public void connectToMail()
    {
      if (this.os.terminal.preventingExecution)
        return;
      if (!this.os.netMap.visibleNodes.Contains(this.os.netMap.nodes.IndexOf(this.os.netMap.mailServer)))
        this.os.netMap.discoverNode(this.os.netMap.mailServer);
      MailServer mailServer = (MailServer) this.os.netMap.mailServer.daemons[0];
      if (this.targetServer != null)
        mailServer = this.targetServer;
      this.os.connectedComp = mailServer.comp;
      this.os.terminal.prompt = this.os.connectedComp.ip + "@> ";
      mailServer.comp.userLoggedIn = true;
      this.os.display.command = mailServer.name;
      mailServer.viewInbox(this.os.defaultUser);
      this.mailUnchecked = false;
    }

    public void mailSent(string mail, string userTo)
    {
    }

    public void mailReceived(string mail, string userTo)
    {
      if (!userTo.Equals(this.os.defaultUser.name))
        return;
      this.alertTimer = MailIcon.ALERT_TIME;
      if (!Settings.soundDisabled)
        this.newMailSound.Play();
      if (this.os.connectedComp == null || !this.os.connectedComp.Equals((object) this.os.netMap.mailServer))
        this.mailUnchecked = true;
    }
  }
}
