// Decompiled with JetBrains decompiler
// Type: Hacknet.TuneswapExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class TuneswapExe : ExeModule
  {
    private Color themeColor = Color.Pink;
    private string oldPlayingSong = (string) null;
    private List<string> SongOptions = new List<string>((IEnumerable<string>) new string[8]{ "DLC\\Music\\snidelyWhiplash", "DLC\\Music\\Userspacelike", "DLC\\Music\\Slow_Motion", "DLC\\Music\\World_Chase", "DLC\\Music\\HOME_Resonance", "DLC\\Music\\Remi2", "DLC\\Music\\Remi_Finale", "DLC\\Music\\DreamHead" });
    private List<string> SongNames = new List<string>((IEnumerable<string>) new string[8]{ "Snidely Whiplash", "Payload (AKA Userspacelike)", "Slow Motion", "World Chase", "Resonance", "ClearText", "Sabotage (AKA Altitude_Loss)", "Dream Head" });
    private List<string> ArtistNames = new List<string>((IEnumerable<string>) new string[8]{ "OGRE", "The Algorithm", "Tonspender", "Cinematrik", "HOME", "The Algorithm", "The Algorithm", "HOME" });
    private string mousedOverArtistName = (string) null;
    private RaindropsEffect backdrop;

    public TuneswapExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.needsProxyAccess = false;
      this.name = "Tuneswap";
      this.ramCost = 300;
      this.IdentifierName = "Tuneswap";
      this.backdrop = new RaindropsEffect();
      this.backdrop.Init(this.os.content);
      this.backdrop.MaxVerticalLandingVariane = 0.06f;
      this.backdrop.FallRate = 0.2f;
      this.themeColor = new Color(38, 171, 146, 0) * 0.4f;
    }

    public override void Update(float t)
    {
      base.Update(t);
      this.backdrop.Update(t, 50f);
    }

    public override void Completed()
    {
      base.Completed();
    }

    private bool CanActivateSong(int i)
    {
      return MusicManager.currentSongName != this.SongOptions[i];
    }

    private void ActivateSong(string song)
    {
      if (!this.SongOptions.Contains(MusicManager.currentSongName))
        this.oldPlayingSong = MusicManager.currentSongName;
      try
      {
        MusicManager.playSongImmediatley(song);
      }
      catch (Exception ex)
      {
        this.os.write("Tuneswap.exe :: ERROR PLAYING SONG " + song + "\n -- EXITING\n");
        this.isExiting = true;
      }
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      Rectangle contentAreaDest = this.GetContentAreaDest();
      Rectangle dest = contentAreaDest;
      dest.Height = (int) ((double) dest.Height + 20.0);
      this.backdrop.Render(dest, this.spriteBatch, this.themeColor, 5f, 30f);
      int x = contentAreaDest.X + 10;
      int width = contentAreaDest.Width - 20;
      int y = contentAreaDest.Y + 10;
      if (this.isExiting)
        return;
      this.mousedOverArtistName = (string) null;
      for (int i = 0; i < this.SongOptions.Count; ++i)
      {
        Rectangle destinationRectangle = new Rectangle(x, y, width, 20);
        this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Black * 0.85f);
        int myID = 10777001 + this.PID * 1000 + i;
        if (Button.doButton(myID, x, y, width, 20, this.SongNames[i], new Color?(this.CanActivateSong(i) ? this.themeColor : Color.Gray)))
          this.ActivateSong(this.SongOptions[i]);
        if (GuiData.hot == myID)
          this.mousedOverArtistName = this.ArtistNames[i];
        y += 23;
      }
      if (this.oldPlayingSong != null)
      {
        if (Button.doButton(10797009 + this.PID * 1000, x, y, width, 20, LocaleTerms.Loc("Default") + " Track", new Color?(this.themeColor)))
          this.ActivateSong(this.oldPlayingSong);
        y += 23;
      }
      if (this.mousedOverArtistName != null)
        this.spriteBatch.DrawString(GuiData.smallfont, this.mousedOverArtistName, new Vector2((float) x, (float) y), Color.White);
      Rectangle destinationRectangle1 = new Rectangle(x, contentAreaDest.Y + contentAreaDest.Height - 24, width, 20);
      this.spriteBatch.Draw(Utils.white, destinationRectangle1, Color.Black * 0.6f);
      if (Button.doButton(109271000 + this.PID, x, contentAreaDest.Y + contentAreaDest.Height - 24, width, 20, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
        this.isExiting = true;
    }

    public void RenderMainDisplay(Rectangle dest, SpriteBatch sb)
    {
    }
  }
}
