// Decompiled with JetBrains decompiler
// Type: Hacknet.IntroTextModule
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Extensions;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Linq;

namespace Hacknet
{
  internal class IntroTextModule : Module
  {
    private static float FLASH_TIME = 3.5f;
    private static float STAY_ONSCREEN_TIME = 3f;
    private static float MODULE_FLASH_TIME = 2f;
    private static float CHAR_TIME = 0.048f;
    private static float LINE_TIME = 0.455f;
    private static float DELAY_FROM_START_MUSIC_TIMER = Settings.isConventionDemo ? 6.2f : 15.06f;
    private static float DEMO_DELAY_FROM_START_MUSIC_TIMER = 5.18f;
    private static string[] delims = new string[1]{ "\r\n" };
    private int charIndex = 0;
    private float timer;
    private float charTimer;
    private float lineTimer;
    public bool complete;
    private bool finishedText;
    private string[] text;
    private int textIndex;
    private Rectangle fullscreen;

    public IntroTextModule(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.bounds = location;
      this.timer = 0.0f;
      this.complete = false;
      this.textIndex = 0;
      this.finishedText = false;
      int x = 0;
      int y = 0;
      Viewport viewport = this.spriteBatch.GraphicsDevice.Viewport;
      int width = viewport.Width;
      viewport = this.spriteBatch.GraphicsDevice.Viewport;
      int height = viewport.Height;
      this.fullscreen = new Rectangle(x, y, width, height);
      string str = this.os.multiplayer ? "Content/MultiplayerIntroText.txt" : (this.os.IsDLCConventionDemo ? "Content/DLC/Docs/DemoIntroText.txt" : "Content/BitSpeech.txt");
      if (Settings.IsInExtensionMode)
      {
        str = Path.Combine(ExtensionLoader.ActiveExtensionInfo.FolderPath, "Intro.txt");
        if (!File.Exists(str))
          str = Path.Combine(ExtensionLoader.ActiveExtensionInfo.FolderPath, "intro.txt");
      }
      if (File.Exists(str))
        this.text = Utils.CleanFilterStringToRenderable(LocalizedFileLoader.Read(str).Replace("\t", "    ")).Split(IntroTextModule.delims, StringSplitOptions.RemoveEmptyEntries);
      else
        this.text = new string[1]{ "   " };
    }

    public override void Update(float t)
    {
      base.Update(t);
      double timer = (double) this.timer;
      this.timer += t;
      float num1 = Settings.isDemoMode ? IntroTextModule.DEMO_DELAY_FROM_START_MUSIC_TIMER : IntroTextModule.DELAY_FROM_START_MUSIC_TIMER;
      if (Settings.IsInExtensionMode)
        num1 = ExtensionLoader.ActiveExtensionInfo.IntroStartupSongDelay;
      if (timer < (double) num1 && (double) this.timer >= (double) num1)
        MusicManager.playSong();
      float num2 = 1f;
      if (LocaleActivator.ActiveLocaleIsCJK())
        num2 = 0.6f;
      if (this.finishedText)
      {
        if ((double) this.timer <= (double) IntroTextModule.STAY_ONSCREEN_TIME || (double) this.timer <= (double) IntroTextModule.STAY_ONSCREEN_TIME + (double) IntroTextModule.MODULE_FLASH_TIME)
          return;
        this.complete = true;
      }
      else if ((double) this.timer > (double) IntroTextModule.FLASH_TIME)
      {
        this.charTimer += t * num2;
        if ((double) this.charTimer < (double) IntroTextModule.CHAR_TIME)
          return;
        KeyboardState keyboadState;
        double num3;
        if (!Settings.isConventionDemo)
        {
          num3 = 0.0;
        }
        else
        {
          double charTime = (double) IntroTextModule.CHAR_TIME;
          keyboadState = GuiData.getKeyboadState();
          double num4 = keyboadState.IsKeyDown(Keys.LeftShift) ? 0.990000009536743 : 0.5;
          num3 = charTime * num4;
        }
        this.charTimer = (float) num3;
        ++this.charIndex;
        if (this.charIndex >= this.text[this.textIndex].Length)
        {
          this.charTimer = IntroTextModule.CHAR_TIME;
          this.charIndex = this.text[this.textIndex].Length - 1;
          this.lineTimer += t;
          if ((double) this.lineTimer >= (double) IntroTextModule.LINE_TIME)
          {
            double num4;
            if (!Settings.isConventionDemo)
            {
              num4 = 0.0;
            }
            else
            {
              double lineTime = (double) IntroTextModule.LINE_TIME;
              keyboadState = GuiData.getKeyboadState();
              double num5 = keyboadState.IsKeyDown(Keys.LeftShift) ? 0.990000009536743 : 0.200000002980232;
              num4 = lineTime * num5;
            }
            this.lineTimer = (float) num4;
            ++this.textIndex;
            this.charIndex = 0;
            if (this.textIndex >= this.text.Length)
            {
              if (!MusicManager.isPlaying)
                MusicManager.playSong();
              this.finishedText = true;
              this.timer = 0.0f;
            }
          }
        }
      }
      else if (Settings.isConventionDemo && GuiData.getKeyboadState().IsKeyDown(Keys.LeftShift))
        this.timer += t + t;
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.spriteBatch.Draw(Utils.white, this.bounds, this.os.darkBackgroundColor);
      if (Utils.random.NextDouble() < (double) this.timer / (double) IntroTextModule.FLASH_TIME || (double) this.timer > (double) IntroTextModule.FLASH_TIME || this.finishedText)
        this.os.drawBackground();
      Color color = this.os.terminalTextColor * (this.finishedText ? (float) (1.0 - (double) this.timer / (double) IntroTextModule.STAY_ONSCREEN_TIME) : 1f);
      Vector2 vector2 = new Vector2(120f, 100f);
      if ((double) this.timer > (double) IntroTextModule.FLASH_TIME || this.finishedText)
      {
        for (int index = 0; index < this.textIndex; ++index)
        {
          string splitVersionOfString = this.GetScreensizeSplitVersionOfString(this.text[index], vector2);
          this.spriteBatch.DrawString(GuiData.smallfont, splitVersionOfString, vector2, color);
          vector2.Y += 16f;
          if (splitVersionOfString.Contains<char>('\n'))
            vector2.Y += 16f;
        }
        if (!this.finishedText)
        {
          string splitVersionOfString = this.GetScreensizeSplitVersionOfString(this.text[this.textIndex].Substring(0, this.charIndex + 1), vector2);
          this.spriteBatch.DrawString(GuiData.smallfont, splitVersionOfString, vector2, color);
        }
      }
      if (this.finishedText && (double) this.timer > (double) IntroTextModule.STAY_ONSCREEN_TIME && Utils.random.NextDouble() < ((double) this.timer - (double) IntroTextModule.STAY_ONSCREEN_TIME) / (double) IntroTextModule.MODULE_FLASH_TIME)
        this.os.drawModules(this.os.lastGameTime);
      this.os.drawScanlines();
    }

    private string GetScreensizeSplitVersionOfString(string input, Vector2 dpos)
    {
      int width = this.spriteBatch.GraphicsDevice.Viewport.Width - (int) dpos.X - 10;
      if ((double) GuiData.smallfont.MeasureString(input).X > (double) width)
        return Utils.SuperSmartTwimForWidth(input, width, GuiData.smallfont);
      return input;
    }
  }
}
