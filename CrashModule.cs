// Decompiled with JetBrains decompiler
// Type: Hacknet.CrashModule
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Hacknet
{
  internal class CrashModule : Module
  {
    public static float BLUESCREEN_TIME = 8f;
    public static float BOOT_TIME = Settings.isConventionDemo ? 5f : (Settings.FastBootText ? 1.2f : 14.5f);
    public static float BLACK_TIME = 2f;
    public static float POST_BLACK_TIME = 1f;
    public string BootLoadErrors = "";
    public float elapsedTime = 0.0f;
    public int state = 0;
    public Color bluescreenBlue = new Color(0, 0, 170);
    public Color bluescreenGrey = new Color(167, 167, 167);
    public Color textColor = new Color(0, 0, (int) byte.MaxValue);
    private string bsodText = "";
    private int bootTextCount = 0;
    private float bootTextDelay = 1f;
    private float bootTextTimer = 0.0f;
    private float bootTextErrorDelay = 0.0f;
    private bool graphicsErrorsDetected = false;
    private bool hasPlayedBeep = false;
    private bool IsInHostileFileCrash = false;
    private int extraErrors = 0;
    private const float BOOT_FAIL_CRASH_TIME = 15f;
    private SpriteFont bsodFont;
    private static SoundEffect beep;
    private string originalBootText;
    private string[] bootText;

    public CrashModule(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.bounds = location;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.bsodFont = this.os.ScreenManager.Game.Content.Load<SpriteFont>("BSODFont");
      CrashModule.beep = this.os.content.Load<SoundEffect>("SFX/beep");
      char[] chArray = new char[1]{ '\n' };
      StreamReader streamReader1 = new StreamReader(TitleContainer.OpenStream("Content/BSOD.txt"));
      this.bsodText = streamReader1.ReadToEnd();
      streamReader1.Close();
      StreamReader streamReader2 = new StreamReader(TitleContainer.OpenStream("Content/OSXBoot.txt"));
      this.originalBootText = streamReader2.ReadToEnd();
      streamReader2.Close();
      this.loadBootText();
      this.bootTextDelay = CrashModule.BOOT_TIME / ((float) (this.bootText.Length - 1) * 2f);
    }

    private void loadBootText()
    {
      this.bootText = this.checkOSBootFiles(this.originalBootText).Split('\n');
    }

    public override void Update(float t)
    {
      base.Update(t);
      this.elapsedTime += t;
      if (this.IsInHostileFileCrash)
      {
        if ((double) this.elapsedTime % 0.5 < 0.0333333350718021)
          ++this.extraErrors;
        if ((double) this.elapsedTime < 15.0)
          return;
        if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed)
          this.os.BootAssitanceModule.IsActive = true;
        this.reset();
        this.os.canRunContent = false;
        this.os.bootingUp = false;
      }
      else if ((double) this.elapsedTime < (double) CrashModule.BLUESCREEN_TIME / 4.0)
        this.state = 0;
      else if ((double) this.elapsedTime < (double) CrashModule.BLUESCREEN_TIME)
        this.state = 1;
      else if ((double) this.elapsedTime < (double) CrashModule.BLUESCREEN_TIME + (double) CrashModule.BLACK_TIME)
        this.state = 2;
      else if ((double) this.elapsedTime < (double) CrashModule.BLUESCREEN_TIME + (double) CrashModule.BLACK_TIME + (double) CrashModule.BOOT_TIME + (double) this.bootTextErrorDelay)
      {
        this.state = 3;
        this.bootTextTimer -= t;
        if ((double) this.bootTextTimer <= 0.0)
        {
          this.bootTextTimer = this.bootTextDelay - (float) Utils.random.NextDouble() * this.bootTextDelay + (float) Utils.random.NextDouble() * this.bootTextDelay;
          ++this.bootTextCount;
          if (this.bootTextCount >= this.bootText.Length - 1)
            this.bootTextCount = this.bootText.Length - 1;
          if (this.bootText[this.bootTextCount].Equals(" "))
            this.bootTextTimer = this.bootTextDelay * 12f;
          if (this.bootText[this.bootTextCount].StartsWith("ERROR:"))
          {
            this.bootTextTimer = this.bootTextDelay * 29f;
            this.os.thisComputer.bootupTick((float) -((double) this.bootTextDelay * 42.0));
            this.bootTextErrorDelay += this.bootTextDelay * 42f;
          }
          if (this.bootTextCount == 50 && HostileHackerBreakinSequence.IsInBlockingHostileFileState((object) this.os))
          {
            this.bootTextTimer = 999999f;
            this.os.thisComputer.bootTimer = 9999f;
            this.IsInHostileFileCrash = true;
            this.elapsedTime = 0.2f;
          }
        }
        if (!this.hasPlayedBeep)
        {
          if (!Settings.soundDisabled)
          {
            CrashModule.beep.Play(0.5f, 0.5f, 0.0f);
            this.os.delayer.Post(ActionDelayer.Wait(0.1), (Action) (() => CrashModule.beep.Play(0.5f, 0.5f, 0.0f)));
          }
          this.hasPlayedBeep = true;
        }
      }
      else
        this.state = 2;
    }

    public override void Draw(float t)
    {
      switch (this.state)
      {
        case 0:
          this.spriteBatch.Draw(Utils.white, this.bounds, this.bluescreenBlue);
          this.drawString((double) this.elapsedTime % 0.800000011920929 > 0.5 ? "" : "_", new Vector2((float) this.bounds.X, (float) (this.bounds.Y + 10)), this.bsodFont);
          break;
        case 1:
          this.spriteBatch.Draw(Utils.white, this.bounds, this.bluescreenBlue);
          this.drawString(this.bsodText, new Vector2((float) this.bounds.X, (float) (this.bounds.Y + 10)), this.bsodFont);
          break;
        case 3:
          float num = GuiData.ActiveFontConfig.tinyFontCharHeight + 1f;
          this.spriteBatch.Draw(Utils.white, this.bounds, this.os.darkBackgroundColor);
          Math.Min((int) (((double) (this.bounds.Height - 10) - (double) num) / (double) num), this.bootTextCount);
          int bootTextCount = this.bootTextCount;
          Vector2 dpos = new Vector2((float) (this.bounds.X + 10), (float) ((double) bootTextCount * (double) num + 10.0));
          float y = dpos.Y;
          if ((double) dpos.Y > (double) (this.bounds.Y + this.bounds.Height - 14))
            dpos.Y = (float) (this.bounds.Y + this.bounds.Height - 14);
          for (; bootTextCount >= 0 && (double) dpos.Y > (double) num; --bootTextCount)
          {
            this.drawString(this.bootText[bootTextCount], dpos, GuiData.tinyfont);
            dpos.Y -= num;
          }
          if (!this.IsInHostileFileCrash)
            break;
          dpos.Y = y + num;
          this.drawString("ERROR: Critical boot error loading \"VMBootloaderTrap.dll\"", dpos, GuiData.tinyfont);
          for (int index = 0; index < this.extraErrors; ++index)
          {
            dpos.Y += num;
            this.drawString("ERROR:", dpos, GuiData.tinyfont);
          }
          break;
        default:
          this.spriteBatch.Draw(Utils.white, this.bounds, this.os.darkBackgroundColor);
          break;
      }
    }

    private Vector2 drawString(string text, Vector2 dpos, SpriteFont font)
    {
      if (this.IsInHostileFileCrash)
      {
        float num = Utils.QuadraticOutCurve(this.elapsedTime / 15f);
        text = Utils.FlipRandomChars(text, (double) num * 0.2 * (double) num * (double) num);
        string str = "";
        for (int index = 0; index < text.Length; ++index)
          str += (string) (object) (char) ((double) Utils.randm(1f) < (double) num * (double) num * (double) num ? 32 : (int) text[index]);
        text = str;
      }
      Vector2 vector2 = font.MeasureString(text);
      bool flag = text.StartsWith("ERROR:");
      this.spriteBatch.DrawString(font, text, dpos, flag ? Color.Red : Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
      return vector2;
    }

    public string checkOSBootFiles(string bootString)
    {
      this.BootLoadErrors = "";
      Folder folder = this.os.thisComputer.files.root.searchForFolder("sys");
      bool flag = true;
      string newValue1 = "ERROR: " + LocaleTerms.Loc("Unable to Load System file os-config.sys") + "\n";
      if (folder.containsFile("os-config.sys"))
      {
        newValue1 = "Loaded os-config.sys : System Config Initialized";
      }
      else
      {
        this.os.failBoot();
        flag = false;
        CrashModule crashModule = this;
        string str = crashModule.BootLoadErrors + newValue1 + " \n";
        crashModule.BootLoadErrors = str;
      }
      bootString = bootString.Replace("[OSBoot1]", newValue1);
      string newValue2 = "ERROR: " + LocaleTerms.Loc("Unable to Load System file bootcfg.dll") + "\n";
      if (folder.containsFile("bootcfg.dll"))
      {
        newValue2 = "Loaded bootcfg.dll : Boot Config Module Loaded";
      }
      else
      {
        this.os.failBoot();
        flag = false;
        CrashModule crashModule = this;
        string str = crashModule.BootLoadErrors + newValue2 + " \n";
        crashModule.BootLoadErrors = str;
      }
      bootString = bootString.Replace("[OSBoot2]", newValue2);
      string newValue3 = "ERROR: " + LocaleTerms.Loc("Unable to Load System file netcfgx.dll") + "\n";
      if (folder.containsFile("netcfgx.dll"))
      {
        newValue3 = "Loaded netcfgx.dll : Network Config Module Loaded";
      }
      else
      {
        this.os.failBoot();
        flag = false;
        CrashModule crashModule = this;
        string str = crashModule.BootLoadErrors + newValue3 + " \n";
        crashModule.BootLoadErrors = str;
      }
      bootString = bootString.Replace("[OSBoot3]", newValue3);
      string newValue4 = "ERROR: " + LocaleTerms.Loc("Unable to Load System file x-server.sys") + "\nERROR: " + LocaleTerms.Loc("Locate and restore a valid x-server file in ~/sys/ folder to restore UX functionality") + "\nERROR: " + LocaleTerms.Loc("Consider examining reports in ~/log/ for problem cause and source") + "\nERROR: " + LocaleTerms.Loc("System UX resources unavailable -- defaulting to terminal mode") + "\n .\n .\n .\n";
      if (folder.containsFile("x-server.sys"))
      {
        newValue4 = "Loaded x-server.sys : UX Graphics Module Loaded";
        ThemeManager.switchTheme((object) this.os, ThemeManager.getThemeForDataString(folder.searchForFile("x-server.sys").data));
        this.graphicsErrorsDetected = false;
      }
      else
      {
        this.os.graphicsFailBoot();
        flag = false;
        this.graphicsErrorsDetected = true;
        CrashModule crashModule = this;
        string str = crashModule.BootLoadErrors + newValue4 + " \n";
        crashModule.BootLoadErrors = str;
      }
      bootString = bootString.Replace("[OSBootTheme]", newValue4);
      if (flag)
      {
        if (this.os.Flags.HasFlag("BootFailure") && !this.os.Flags.HasFlag("BootFailureThemeSongChange") && ThemeManager.currentTheme != OSTheme.HacknetBlue)
        {
          this.os.Flags.AddFlag("BootFailureThemeSongChange");
          if (MusicManager.isPlaying)
            MusicManager.stop();
          MusicManager.loadAsCurrentSong("Music\\The_Quickening");
        }
        this.os.sucsesfulBoot();
      }
      else
        this.os.Flags.AddFlag("BootFailure");
      return bootString;
    }

    public void reset()
    {
      this.elapsedTime = 0.0f;
      this.state = 0;
      this.bootTextCount = 0;
      this.bootTextTimer = 0.0f;
      this.bootTextErrorDelay = 0.0f;
      this.hasPlayedBeep = false;
      this.IsInHostileFileCrash = false;
      this.extraErrors = 0;
      this.loadBootText();
      MusicManager.stop();
    }

    public void completeReboot()
    {
      this.os.terminal.reset();
      this.os.execute("FirstTimeInitdswhupwnemfdsiuoewnmdsmffdjsklanfeebfjkalnbmsdakj Init");
      this.os.inputEnabled = false;
      if (!this.graphicsErrorsDetected)
      {
        this.os.setMouseVisiblity(true);
        MusicManager.playSong();
      }
      this.os.connectedComp = (Computer) null;
      if (this.os.thisComputer.files.root.searchForFolder("sys").searchForFile("Notes_Reopener.bat") != null)
        this.os.runCommand("notes");
      try
      {
        this.os.threadedSaveExecute(false);
        this.os.gameSavedTextAlpha = -1f;
      }
      catch (Exception ex)
      {
        int num = 1 - 1;
      }
    }
  }
}
