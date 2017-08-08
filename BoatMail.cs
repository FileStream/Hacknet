// Decompiled with JetBrains decompiler
// Type: Hacknet.BoatMail
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet
{
  internal class BoatMail : MailServer
  {
    public static string JunkEmail = "HOr$e Exp@nding R0Lexxxx corp\n\nHello Sir/Madam,\nI am but a humble nigerian prince who is crippled by the instability in my country, and require a transfer of 5000 united states dollar in order to rid my country of the scourge of the musclebeasts which roam our plains and ravage our villages\nPlease send these funds to real_nigerian_prince_the_third@boatmail.com\nYours in jegus,\nNigerian Prince";
    public Texture2D logo;

    public BoatMail(Computer c, string name, OS os)
      : base(c, name, os)
    {
      this.oddLine = Color.White;
      this.evenLine = Color.White;
      this.setThemeColor(new Color(155, 155, 230));
      this.seperatorLineColor = new Color(22, 22, 22, 150);
      this.panel = TextureBank.load("BoatmailHeader", os.content);
      this.logo = TextureBank.load("BoatmailLogo", os.content);
      this.textColor = Color.Black;
    }

    public override void drawBackingGradient(Rectangle boundsTo, SpriteBatch sb)
    {
    }

    public override void doInboxHeader(Rectangle bounds, SpriteBatch sb)
    {
    }

    public override void drawTopBar(Rectangle bounds, SpriteBatch sb)
    {
      this.os.postFXDrawActions += (Action) (() =>
      {
        Rectangle destinationRectangle = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        sb.Draw(Utils.white, destinationRectangle, Color.White);
        destinationRectangle.Height = this.panel.Height;
        sb.Draw(this.panel, destinationRectangle, Color.White);
        destinationRectangle.Width = destinationRectangle.Height = 36;
        destinationRectangle.X += 30;
        destinationRectangle.Y += 10;
        sb.Draw(this.logo, destinationRectangle, Color.White);
      });
    }
  }
}
