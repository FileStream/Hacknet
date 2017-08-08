// Decompiled with JetBrains decompiler
// Type: Hacknet.EndingSequenceModule
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;

namespace Hacknet
{
  internal class EndingSequenceModule : Module
  {
    public bool IsActive = false;
    private float elapsedTime = 0.0f;
    private float HacknetTitleFreezeTime = 10f;
    private float creditsPixelsScrollPerSecond = 65f;
    private bool IsInCredits = false;
    private float creditsScroll = 0.0f;
    private int SpeechTextIndex = 0;
    private float SpeechTextTimer = 0.0f;
    private const float SpeechTextHashDelay = 1f;
    private const float SpeechTextPercDelay = 0.5f;
    private const float SpeechTextCharDelay = 0.05f;
    private SoundEffect speech;
    private SoundEffectInstance speechinstance;
    private WaveformRenderer waveRender;
    private string[] CreditsData;
    private SoundEffect spinUpEffect;
    private SoundEffect traceDownEffect;
    private string BitSpeechText;

    public EndingSequenceModule(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.bounds = location;
      this.creditsScroll = (float) (this.os.fullscreen.Height / 2);
      this.spinUpEffect = this.os.content.Load<SoundEffect>("Music/Ambient/spiral_gauge_up");
      this.traceDownEffect = this.os.content.Load<SoundEffect>("SFX/TraceKill");
      this.BitSpeechText = Utils.readEntireFile("Content/Post/BitSpeech.txt");
    }

    public override void Update(float t)
    {
      base.Update(t);
      if (!this.IsActive)
        return;
      if (!this.IsInCredits)
      {
        if (this.waveRender == null)
        {
          this.traceDownEffect.Play();
          this.speech = this.os.content.Load<SoundEffect>("SFX/Ending/EndingSpeech");
          this.waveRender = new WaveformRenderer("Content/SFX/Ending/EndingSpeech.wav");
          this.speechinstance = this.speech.CreateInstance();
          this.speechinstance.IsLooped = false;
          this.CreditsData = Utils.readEntireFile("Content/Post/CreditsData.txt").Split(Utils.newlineDelim, StringSplitOptions.None);
          MusicManager.stop();
        }
        if (this.speechinstance.State == SoundState.Playing)
        {
          this.elapsedTime += t;
          if ((double) this.elapsedTime > this.speech.Duration.TotalSeconds)
          {
            this.RollCredits();
          }
          else
          {
            this.SpeechTextTimer += t;
            if (this.SpeechTextIndex < this.BitSpeechText.Length)
            {
              if ((int) this.BitSpeechText[this.SpeechTextIndex] == 35)
              {
                if ((double) this.SpeechTextTimer >= 1.0)
                {
                  --this.SpeechTextTimer;
                  ++this.SpeechTextIndex;
                }
              }
              else if ((int) this.BitSpeechText[this.SpeechTextIndex] == 37)
              {
                if ((double) this.SpeechTextTimer >= 0.5)
                {
                  this.SpeechTextTimer -= 0.5f;
                  ++this.SpeechTextIndex;
                }
              }
              else if ((double) this.SpeechTextTimer >= 0.0500000007450581)
              {
                this.SpeechTextTimer -= 0.05f;
                ++this.SpeechTextIndex;
              }
            }
          }
        }
        else
          this.speechinstance.Play();
      }
      else
      {
        this.elapsedTime += t;
        if ((double) this.elapsedTime > (double) this.HacknetTitleFreezeTime)
        {
          float num = Math.Min(1f, (float) (((double) this.elapsedTime - 10.0) / 8.0));
          this.creditsScroll -= t * this.creditsPixelsScrollPerSecond * num;
        }
      }
    }

    private void RollCredits()
    {
      if (this.os.TraceDangerSequence.IsActive)
        this.os.TraceDangerSequence.CancelTraceDangerSequence();
      this.IsInCredits = true;
      if (this.speechinstance != null)
        this.speechinstance.Stop();
      Settings.soundDisabled = false;
      this.elapsedTime = 0.0f;
      this.os.delayer.Post(ActionDelayer.Wait(1.0), (Action) (() =>
      {
        MusicManager.playSongImmediatley("Music\\Bit(Ending)");
        MediaPlayer.IsRepeating = false;
        AchievementsManager.Unlock("progress_complete", false);
      }));
      PostProcessor.dangerModeEnabled = false;
      PostProcessor.dangerModePercentComplete = 0.0f;
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      Rectangle fullscreen = this.os.fullscreen;
      this.spriteBatch.Draw(Utils.white, fullscreen, Color.Black);
      if (this.waveRender == null)
        return;
      if (!this.IsInCredits)
      {
        int width = this.os.fullscreen.Width;
        int height = this.os.fullscreen.Height;
        this.waveRender.RenderWaveform((double) this.elapsedTime, this.speech.Duration.TotalSeconds, this.spriteBatch, new Rectangle(0, this.os.fullscreen.Height / 2 - height / 2, width, height));
        string[] strArray = this.BitSpeechText.Substring(0, this.SpeechTextIndex).Replace("#", "").Replace("%", "").Split(Utils.newlineDelim, StringSplitOptions.None);
        Vector2 position = new Vector2((float) this.bounds.X + 150f, (float) (this.bounds.Y + this.bounds.Height) - 100f);
        float num = 1f;
        for (int index = strArray.Length - 1; index >= 0 && index > strArray.Length - 5; --index)
        {
          this.spriteBatch.DrawString(GuiData.smallfont, strArray[index], position, Utils.AddativeWhite * num);
          num *= 0.6f;
          position.Y -= GuiData.ActiveFontConfig.tinyFontCharHeight + 8f;
        }
      }
      else
        this.DrawCredits();
    }

    private void DrawCredits()
    {
      Vector2 vector2_1 = new Vector2(0.0f, this.creditsScroll);
      int width = (int) ((double) this.os.fullscreen.Width * 0.5);
      int height = (int) ((double) this.os.fullscreen.Height * 0.400000005960464);
      Rectangle dest = new Rectangle(this.os.fullscreen.Width / 2 - width / 2, (int) ((double) vector2_1.Y - (double) (height / 2)), width, height);
      Rectangle destinationRectangle = new Rectangle(this.os.fullscreen.X, dest.Y + 65, this.os.fullscreen.Width, dest.Height - 135);
      if ((double) this.elapsedTime >= 1.71000003814697)
      {
        this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Lerp(Utils.AddativeRed, Color.Red, 0.2f + Utils.randm(0.05f)) * 0.5f);
        FlickeringTextEffect.DrawLinedFlickeringText(dest, "HACKNET", 16f, 0.4f, GuiData.titlefont, (object) this.os, Color.White, 5);
      }
      vector2_1.Y += (float) this.os.fullscreen.Height / 2f;
      for (int index = 0; index < this.CreditsData.Length; ++index)
      {
        float num = 20f;
        string str1 = this.CreditsData[index];
        if (!string.IsNullOrEmpty(str1))
        {
          string str2 = str1;
          SpriteFont spriteFont = GuiData.font;
          Color color = Color.White * 0.7f;
          if (str1.StartsWith("^"))
          {
            str2 = str1.Substring(1);
            color = Color.Gray * 0.6f;
          }
          else if (str1.StartsWith("%"))
          {
            str2 = str1.Substring(1);
            spriteFont = GuiData.titlefont;
            num = 90f;
          }
          if (str1.StartsWith("$"))
          {
            str2 = str1.Substring(1);
            color = Color.Gray * 0.6f;
            spriteFont = GuiData.smallfont;
          }
          Vector2 vector2_2 = spriteFont.MeasureString(str2);
          Vector2 position = vector2_1 + new Vector2((float) (this.os.fullscreen.Width / 2) - vector2_2.X / 2f, 0.0f);
          string renderable = Utils.CleanStringToRenderable(str2);
          this.spriteBatch.DrawString(spriteFont, renderable, position, color);
          vector2_1.Y += num;
        }
        vector2_1.Y += num;
      }
      if ((double) vector2_1.Y >= -500.0)
        return;
      this.CompleteAndReturnToMenu();
    }

    private void CompleteAndReturnToMenu()
    {
      this.os.Flags.AddFlag("Victory");
      Programs.disconnect(new string[0], this.os);
      Computer computer = Programs.getComputer(this.os, "porthackHeart");
      this.os.netMap.visibleNodes.Remove(this.os.netMap.nodes.IndexOf(computer));
      computer.disabled = true;
      computer.daemons.Clear();
      computer.ip = NetworkMap.generateRandomIP();
      this.os.terminal.inputLocked = false;
      this.os.ram.inputLocked = false;
      this.os.netMap.inputLocked = false;
      this.os.DisableTopBarButtons = false;
      this.os.canRunContent = true;
      this.IsActive = false;
      ComputerLoader.loadMission("Content/Missions/CreditsMission.xml", false);
      this.os.threadedSaveExecute(false);
      MediaPlayer.IsRepeating = true;
      MusicManager.playSongImmediatley("Music\\Bit(Ending)");
      if (Settings.isPirateBuild)
        this.os.delayer.Post(ActionDelayer.Wait(15.0), (Action) (() =>
        {
          try
          {
            ComputerLoader.loadMission("Content/Missions/CreditsMission_p.xml", false);
          }
          catch (Exception ex)
          {
          }
        }));
      if (!Settings.sendsDLC1PromoEmailAtEnd)
        return;
      this.os.delayer.Post(ActionDelayer.Wait(30.0), (Action) (() =>
      {
        try
        {
          string body = Utils.readEntireFile("Content/LocPost/DLCMessage.txt");
          string subject = "Labyrinths";
          string sender = "Matt Trobbiani";
          string email = MailServer.generateEmail(subject, body, sender);
          MailServer daemon = this.os.netMap.mailServer.getDaemon(typeof (MailServer)) as MailServer;
          if (daemon == null)
            return;
          daemon.addMail(email, this.os.defaultUser.name);
        }
        catch (Exception ex)
        {
        }
      }));
    }
  }
}
