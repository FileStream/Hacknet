// Decompiled with JetBrains decompiler
// Type: Hacknet.PointClickerDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class PointClickerDaemon : Daemon
  {
    private float UpgradeCostMultiplier = 13f;
    private List<string> upgradeNames = new List<string>();
    private List<float> upgradeValues = new List<float>();
    private List<float> upgradeCosts = new List<float>();
    private List<string> storyBeats = new List<string>();
    private List<long> storyBeatChangers = new List<long>();
    private PointClickerDaemon.PointClickerScreenState state = PointClickerDaemon.PointClickerScreenState.Welcome;
    private PointClickerDaemon.PointClickerGameState activeState = (PointClickerDaemon.PointClickerGameState) null;
    private float currentRate = 0.0f;
    private float pointOverflow = 0.0f;
    private List<PointClickerDaemon.PointClickerStar> Stars = new List<PointClickerDaemon.PointClickerStar>();
    private Color ThemeColor = new Color(133, 239, (int) byte.MaxValue, 0);
    private Color ThemeColorBacking = new Color(13, 59, 74, 250);
    private Color ThemeColorHighlight = new Color(227, 0, 121, 200);
    private int hoverIndex = 0;
    private string ActiveStory = "";
    private string userFilePath = (string) null;
    private float timeSinceLastSave = 0.0f;
    private List<PointClickerDaemon.UpgradeNotifier> UpgradeNotifiers = new List<PointClickerDaemon.UpgradeNotifier>();
    private Folder savesFolder;
    private Folder rootFolder;
    private Texture2D background1;
    private Texture2D background2;
    private Texture2D logoBase;
    private Texture2D logoOverlay1;
    private Texture2D logoOverlay2;
    private Texture2D logoStar;
    private Texture2D scanlinesTextBackground;
    private RenderTarget2D logoRenderBase;
    private SpriteBatch logoBatch;
    private ScrollableSectionedPanel scrollerPanel;

    public PointClickerDaemon(Computer computer, string serviceName, OS opSystem)
      : base(computer, serviceName, opSystem)
    {
      this.InitGameSettings();
      this.InitLogoSettings();
      this.InitRest();
    }

    private void InitRest()
    {
    }

    private void InitLogoSettings()
    {
      this.background1 = this.os.content.Load<Texture2D>("EffectFiles/PointClicker/Background1");
      this.background2 = this.os.content.Load<Texture2D>("EffectFiles/PointClicker/Background2");
      this.logoBase = this.os.content.Load<Texture2D>("EffectFiles/PointClicker/BaseLogo");
      this.logoOverlay1 = this.os.content.Load<Texture2D>("EffectFiles/PointClicker/LogoOverlay1");
      this.logoOverlay2 = this.os.content.Load<Texture2D>("EffectFiles/PointClicker/LogoOverlay2");
      this.logoStar = this.os.content.Load<Texture2D>("EffectFiles/PointClicker/Star");
      this.scanlinesTextBackground = this.os.content.Load<Texture2D>("EffectFiles/ScanlinesTextBackground");
      this.logoRenderBase = new RenderTarget2D(GuiData.spriteBatch.GraphicsDevice, 768, 384);
      this.logoBatch = new SpriteBatch(GuiData.spriteBatch.GraphicsDevice);
      for (int index = 0; index < 40; ++index)
        this.AddRandomLogoStar(true);
    }

    private void UpdateRate()
    {
      this.currentRate = 0.0f;
      for (int index = 0; index < this.upgradeNames.Count; ++index)
        this.currentRate += this.upgradeValues[index] * (float) this.activeState.upgradeCounts[index];
    }

    private void UpdatePoints()
    {
      if (this.activeState == null)
        return;
      if ((double) this.currentRate > 0.0 || (double) this.currentRate < -1.0)
      {
        double num = (double) this.currentRate * this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
        this.activeState.points += (long) (int) num;
        this.pointOverflow += (float) num - (float) (int) num;
        if ((double) this.pointOverflow > 1.0)
        {
          int pointOverflow = (int) this.pointOverflow;
          this.activeState.points += (long) pointOverflow;
          this.pointOverflow -= (float) pointOverflow;
        }
      }
      this.UpdateStory();
      if (this.ActiveStory == null)
        this.ActiveStory = "";
      if (this.activeState.points <= -1L)
        AchievementsManager.Unlock("pointclicker_expert", true);
      this.timeSinceLastSave += (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
      if ((double) this.timeSinceLastSave > 10.0)
        this.SaveProgress();
    }

    private void UpdateStory()
    {
      if (this.activeState.currentStoryElement < 0)
      {
        this.activeState.currentStoryElement = 0;
        this.ActiveStory = this.storyBeats[this.activeState.currentStoryElement] ?? "";
      }
      else if (this.storyBeatChangers.Count > this.activeState.currentStoryElement + 1 && this.activeState.points >= this.storyBeatChangers[this.activeState.currentStoryElement + 1])
      {
        ++this.activeState.currentStoryElement;
        this.ActiveStory = this.storyBeats[this.activeState.currentStoryElement] ?? "";
      }
    }

    private void PurchaseUpgrade(int index)
    {
      if (this.activeState == null || (double) this.activeState.points < (double) this.upgradeCosts[index])
        return;
      List<int> upgradeCounts;
      int index1;
      (upgradeCounts = this.activeState.upgradeCounts)[index1 = index] = upgradeCounts[index1] + 1;
      this.activeState.points -= (long) (int) this.upgradeCosts[index];
      this.pointOverflow -= this.upgradeCosts[index] - (float) (int) this.upgradeCosts[index];
      this.UpdateRate();
      this.UpgradeNotifiers.Add(new PointClickerDaemon.UpgradeNotifier()
      {
        text = "+" + this.upgradeValues[index].ToString(),
        timer = 1f
      });
      this.SaveProgress();
      if (index >= this.upgradeNames.Count - 1)
        AchievementsManager.Unlock("pointclicker_basic", true);
    }

    private void SaveProgress()
    {
      for (int index = 0; index < this.savesFolder.files.Count; ++index)
      {
        if (this.savesFolder.files[index].name == this.userFilePath)
        {
          this.savesFolder.files[index].data = this.activeState.ToSaveString();
          this.timeSinceLastSave = 0.0f;
          break;
        }
      }
    }

    private void AddRandomLogoStar(bool randomStartLife = false)
    {
      PointClickerDaemon.PointClickerStar pointClickerStar = new PointClickerDaemon.PointClickerStar() { Pos = new Vector2(Utils.randm(1f), Utils.randm(1f)), life = randomStartLife ? Utils.randm(1f) : 1f, rot = Utils.randm(6.48f), scale = 0.2f + Utils.rand(1.3f), timescale = 0.3f + Utils.randm(1.35f) };
      pointClickerStar.color = Utils.AddativeWhite;
      float num = Utils.randm(1f);
      int maxValue = 80;
      if ((double) num < 0.300000011920929)
        pointClickerStar.color.R = (byte) ((int) byte.MaxValue - Utils.random.Next(maxValue));
      else if ((double) num < 0.600000023841858)
        pointClickerStar.color.G = (byte) ((int) byte.MaxValue - Utils.random.Next(maxValue));
      else if ((double) num < 0.899999976158142)
        pointClickerStar.color.B = (byte) ((int) byte.MaxValue - Utils.random.Next(maxValue));
      this.Stars.Add(pointClickerStar);
    }

    private void InitGameSettings()
    {
      this.upgradeNames.Add(LocaleTerms.Loc("Click Me!"));
      this.upgradeNames.Add(LocaleTerms.Loc("Autoclicker v1"));
      this.upgradeNames.Add(LocaleTerms.Loc("Autoclicker v2"));
      this.upgradeNames.Add(LocaleTerms.Loc("Pointereiellion"));
      this.upgradeValues.Add(0.04f);
      this.upgradeValues.Add(1f);
      this.upgradeValues.Add(10f);
      this.upgradeValues.Add(200f);
      this.storyBeats.Add("Your glorious ClickPoints empire begins");
      this.storyBeats.Add("The hard days of manual button clicking labour seem endless, but a better future is in sight.");
      this.storyBeats.Add("The investment is returned - you finally turn a profit.");
      this.storyBeats.Add("Your long days of labour to gather the initial 12 points are a fast-fading memory.");
      this.storyBeats.Add("You reach international acclaim as a prominent and incredibly wealthy point collector.");
      this.storyBeats.Add("Your enormous pile of points is now larger than Everest");
      this.storyBeats.Add("The ClickPoints continent is declared : a landmass made entirely of your insane wealth.");
      this.storyBeatChangers.Add(0L);
      this.storyBeatChangers.Add(5L);
      this.storyBeatChangers.Add(15L);
      this.storyBeatChangers.Add(200L);
      this.storyBeatChangers.Add(100000L);
      this.storyBeatChangers.Add(1000000000000L);
      this.storyBeatChangers.Add(11111000000000000L);
      for (int index = 3; index < 50; ++index)
      {
        this.upgradeNames.Add(LocaleTerms.Loc("Upgrade") + " " + (object) (index + 1));
        this.upgradeValues.Add((float) Math.Max((double) (index * index * index * index * index), 0.01));
      }
      for (int index = 0; index < this.upgradeValues.Count; ++index)
        this.upgradeCosts.Add((float) ((double) this.upgradeValues[index] * (double) (1 + index / 50 * 5) * (double) this.UpgradeCostMultiplier * (1.0 + (double) this.UpgradeCostMultiplier * ((double) ((index + 1) / this.upgradeValues.Count) * 5.0))));
      this.upgradeCosts[0] = 0.0f;
      this.upgradeCosts[1] = 12f;
    }

    public override void initFiles()
    {
      base.initFiles();
      this.rootFolder = new Folder("PointClicker");
      this.savesFolder = new Folder("Saves");
      int num1 = 50;
      int num2 = 0;
      for (int index = 0; index < num1; ++index)
      {
        string name;
        do
        {
          name = People.all[index + num2].handle;
          if (name == null)
            ++num2;
        }
        while (index + num2 < People.all.Count && name == null);
        if (index == 22)
          name = "Mengsk";
        if (index == 28)
          name = "Bit";
        if (name != null)
          this.AddSaveForName(name, index == 22 || index == 28);
      }
      this.rootFolder.folders.Add(this.savesFolder);
      this.comp.files.root.folders.Add(this.rootFolder);
      this.rootFolder.files.Add(new FileEntry(Computer.generateBinaryString(1000), "config.ini"));
      this.rootFolder.files.Add(new FileEntry(LocaleTerms.Loc("IMPORTANT : NEVER DELETE OR RE-NAME") + " \"config.ini\"\n " + LocaleTerms.Loc("IT IS SYSTEM CRITICAL!") + " " + LocaleTerms.Loc("Removing it causes instant crash. DO NOT TEST THIS"), "IMPORTANT_README_DONT_CRASH.txt"));
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      this.state = PointClickerDaemon.PointClickerScreenState.Welcome;
      this.pointOverflow = 0.0f;
    }

    public override void loadInit()
    {
      base.loadInit();
      this.rootFolder = this.comp.files.root.searchForFolder("PointClicker");
      this.savesFolder = this.rootFolder.searchForFolder("Saves");
    }

    private void AddSaveForName(string name, bool isSuperHighScore = false)
    {
      PointClickerDaemon.PointClickerGameState clickerGameState = new PointClickerDaemon.PointClickerGameState(this.upgradeValues.Count);
      for (int index = 0; index < clickerGameState.upgradeCounts.Count; ++index)
      {
        clickerGameState.upgradeCounts[index] = (int) (10.0 * ((double) Utils.randm(1f) * ((double) index / (double) clickerGameState.upgradeCounts.Count)));
        if (isSuperHighScore)
          clickerGameState.upgradeCounts[index] = 900 + (int) ((double) Utils.randm(1f) * 99.9000015258789);
      }
      clickerGameState.points = (long) Utils.random.Next();
      clickerGameState.currentStoryElement = Utils.random.Next(this.storyBeats.Count);
      this.savesFolder.files.Add(new FileEntry(clickerGameState.ToSaveString(), name + ".pcsav"));
    }

    public override string getSaveString()
    {
      return "<PointClicker />";
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      bool drawShadow = TextItem.DrawShadow;
      TextItem.DrawShadow = false;
      switch (this.state)
      {
        case PointClickerDaemon.PointClickerScreenState.Error:
          TextItem.DrawShadow = drawShadow;
          break;
        case PointClickerDaemon.PointClickerScreenState.Main:
          this.UpdatePoints();
          this.DrawMainScreen(bounds, sb);
          goto case PointClickerDaemon.PointClickerScreenState.Error;
        default:
          this.DrawWelcome(bounds, sb);
          goto case PointClickerDaemon.PointClickerScreenState.Error;
      }
    }

    private void DrawLogo(Rectangle dest, SpriteBatch sb)
    {
      float totalSeconds = (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
      for (int index = 0; index < this.Stars.Count; ++index)
      {
        PointClickerDaemon.PointClickerStar star = this.Stars[index];
        star.life -= totalSeconds * 2f * star.timescale;
        if ((double) this.Stars[index].life <= 0.0)
        {
          this.Stars.RemoveAt(index);
          --index;
          this.AddRandomLogoStar(false);
        }
        else
          this.Stars[index] = star;
      }
      RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
      sb.GraphicsDevice.SetRenderTarget(this.logoRenderBase);
      sb.GraphicsDevice.Clear(Color.Transparent);
      this.logoBatch.Begin();
      float num1 = (float) (Math.Sin((double) this.os.timer / 2.20000004768372) + 1.0) / 2f;
      this.logoBatch.Draw(this.background1, Vector2.Zero, Utils.AddativeWhite * num1);
      this.logoBatch.Draw(this.background2, Vector2.Zero, Utils.AddativeWhite * (1f - num1));
      Rectangle dest1 = new Rectangle(0, 0, this.logoBase.Width, this.logoBase.Height);
      this.logoBatch.Draw(this.logoBase, Vector2.Zero, Color.White);
      FlickeringTextEffect.DrawFlickeringSprite(this.logoBatch, dest1, this.logoBase, 4f, 0.25f, (object) this.os, Color.White);
      float num2 = (float) (0.439999997615814 + (Math.Sin((double) this.os.timer * 0.823000013828278) + 1.0) / 2.0);
      this.logoBatch.Draw(this.logoOverlay1, Vector2.Zero, Utils.AddativeWhite * num2);
      this.logoBatch.Draw(this.logoOverlay2, Vector2.Zero, Utils.AddativeWhite * (1f - num2));
      this.logoBatch.End();
      sb.GraphicsDevice.SetRenderTarget(currentRenderTarget);
      for (int index = 0; index < this.Stars.Count; ++index)
        this.DrawStar(dest, sb, this.Stars[index]);
      FlickeringTextEffect.DrawFlickeringSpriteAltWeightings(sb, dest, (Texture2D) this.logoRenderBase, 4f, 0.01f, (object) this.os, Utils.AddativeWhite);
    }

    private void DrawStar(Rectangle logoDest, SpriteBatch sb, PointClickerDaemon.PointClickerStar star)
    {
      Vector2 position = new Vector2((float) ((double) star.Pos.X * (double) logoDest.Width * 0.5) + (float) logoDest.X, (float) ((double) star.Pos.Y * (double) logoDest.Height * 0.5) + (float) logoDest.Y);
      position.X += (float) logoDest.Width * 0.25f;
      position.Y += (float) logoDest.Height * 0.25f;
      float num1 = (double) star.life >= 0.899999976158142 ? (float) (1.0 - ((double) star.life - 0.899999976158142) / 0.100000001490116) : (float) (((double) star.life - 0.100000001490116) / 0.899999976158142);
      float num2 = Vector2.Distance(star.Pos, new Vector2(0.5f));
      float num3 = 0.9f;
      if ((double) num2 > (double) num3)
        num1 = (float) (1.0 - ((double) num2 - (double) num3) / 1.0) * num1;
      sb.Draw(this.logoStar, position, new Rectangle?(), star.color * num1, star.rot * (star.life * 0.5f), new Vector2((float) (this.logoStar.Width / 2), (float) (this.logoStar.Height / 2)), star.scale * num1, SpriteEffects.None, 0.4f);
    }

    private void DrawMainScreen(Rectangle bounds, SpriteBatch sb)
    {
      string points = this.activeState.points.ToString();
      Rectangle rectangle1 = new Rectangle(bounds.X + 1, bounds.Y + 10, bounds.Width - 2, 100);
      this.DrawMonospaceString(rectangle1, sb, points);
      rectangle1.Y -= 4;
      sb.Draw(this.scanlinesTextBackground, rectangle1, this.ThemeColor * 0.2f);
      for (int index = 0; index < this.Stars.Count; ++index)
        this.DrawStar(rectangle1, sb, this.Stars[index]);
      Rectangle bounds1 = new Rectangle(bounds.X + 2, rectangle1.Y + rectangle1.Height + 12, bounds.Width / 2 - 4, bounds.Height - 12 - (rectangle1.Y + rectangle1.Height - bounds.Y));
      this.DrawUpgrades(bounds1, sb);
      float num1 = (float) this.logoRenderBase.Height / (float) this.logoRenderBase.Width;
      int num2 = 45;
      int width = bounds.Width / 2 + num2;
      int height = (int) ((double) width * (double) num1);
      this.DrawHoverTooltip(new Rectangle(bounds.X + bounds1.Width + 4, bounds1.Y, bounds.Width - bounds1.Width - 8, bounds1.Height - height + 30), sb);
      Rectangle rectangle2 = new Rectangle(bounds.X + bounds.Width - width + 42, bounds.Y + bounds.Height - height + 10, width - 50, 50);
      string text = Utils.SuperSmartTwimForWidth(this.ActiveStory, rectangle2.Width, GuiData.smallfont);
      TextItem.doFontLabel(new Vector2((float) rectangle2.X, (float) rectangle2.Y), text, GuiData.smallfont, new Color?(this.ThemeColor), (float) rectangle2.Width, (float) rectangle2.Height, false);
      this.DrawLogo(new Rectangle(bounds.X + bounds.Width - width + 35, bounds.Y + bounds.Height - height + 20, width, height), sb);
      if (!Button.doButton(3032113, bounds.X + bounds.Width - 22, bounds.Y + bounds.Height - 22, 20, 20, "X", new Color?(this.os.lockedColor)))
        return;
      this.state = PointClickerDaemon.PointClickerScreenState.Welcome;
    }

    private void DrawHoverTooltip(Rectangle bounds, SpriteBatch sb)
    {
      float announcerWidth = 80f;
      int num = bounds.Height - 80;
      Rectangle rectangle1;
      if (this.hoverIndex > -1 && this.hoverIndex < this.upgradeNames.Count && this.activeState != null)
      {
        Rectangle bounds1 = bounds;
        bounds1.Height = num;
        bool flag = (double) this.upgradeCosts[this.hoverIndex] <= (double) this.activeState.points;
        float cornerCut = 20f;
        FancyOutlines.DrawCornerCutOutline(bounds1, sb, cornerCut, this.ThemeColor);
        Rectangle dest = new Rectangle((int) ((double) bounds.X + (double) cornerCut + 4.0), bounds.Y + 3, (int) ((double) bounds.Width - ((double) cornerCut + 4.0) * 2.0), 40);
        TextItem.doFontLabelToSize(dest, this.upgradeNames[this.hoverIndex], GuiData.font, this.ThemeColorHighlight, false, false);
        rectangle1 = new Rectangle(bounds.X + 2, bounds.Y + dest.Height + 4, bounds.Width - 4, 20);
        string text = flag ? (Settings.ActiveLocale != "en-us" ? LocaleTerms.Loc("UPGRADE AVALIABLE") : "UPGRADE AVAILABLE") : LocaleTerms.Loc("INSUFFICIENT POINTS");
        TextItem.doFontLabelToSize(rectangle1, text, GuiData.font, flag ? this.ThemeColorHighlight * 0.8f : Color.Gray, false, false);
        rectangle1.Y += rectangle1.Height;
        rectangle1.Height = 50;
        rectangle1.X += 4;
        rectangle1.Width -= 4;
        float f = this.activeState.points == 0L ? 1f : this.upgradeCosts[this.hoverIndex] / (float) this.activeState.points;
        this.DrawStatsTextBlock(LocaleTerms.Loc("COST"), string.Concat((object) this.upgradeCosts[this.hoverIndex]), (!float.IsNaN(f) ? f * 100f : 100f).ToString("00.0") + LocaleTerms.Loc("% of current Points"), rectangle1, sb, announcerWidth);
        rectangle1.Y += rectangle1.Height;
        this.DrawStatsTextBlock("+PPS", string.Concat((object) this.upgradeValues[this.hoverIndex]), ((double) this.currentRate <= 0.0 ? 100f : (float) ((double) this.upgradeValues[this.hoverIndex] / (double) this.currentRate * 100.0)).ToString("00.0") + LocaleTerms.Loc("% of current Points Per Second"), rectangle1, sb, announcerWidth);
        Rectangle rectangle2 = new Rectangle((int) ((double) bounds.X + (double) cornerCut + 4.0), rectangle1.Y + rectangle1.Height + 4, (int) ((double) bounds.Width - ((double) cornerCut + 4.0) * 2.0), 50);
        if (flag)
        {
          sb.Draw(this.scanlinesTextBackground, rectangle2, Utils.makeColorAddative(this.ThemeColorHighlight) * 0.6f);
          FlickeringTextEffect.DrawFlickeringText(rectangle2, "CLICK TO UPGRADE", 3f, 0.1f, GuiData.titlefont, (object) this.os, Utils.AddativeWhite);
        }
        else
        {
          sb.Draw(this.scanlinesTextBackground, rectangle2, Color.Lerp(this.os.brightLockedColor, Utils.makeColorAddative(Color.Red), 0.2f + Utils.randm(0.8f)) * 0.4f);
          FlickeringTextEffect.DrawFlickeringText(rectangle2, "INSUFFICIENT POINTS", 3f, 0.1f, GuiData.titlefont, (object) this.os, Utils.AddativeWhite);
        }
      }
      rectangle1 = new Rectangle(bounds.X + 2, bounds.Y + num + 4, bounds.Width - 4, 50);
      float f1 = (double) this.currentRate <= 0.0 ? 0.0f : (float) this.activeState.points / this.currentRate;
      if (float.IsNaN(f1))
        f1 = float.PositiveInfinity;
      this.DrawStatsTextBlock("PPS", this.currentRate.ToString("000.0") ?? "", f1.ToString("00.0") + " " + LocaleTerms.Loc("seconds to double current points"), rectangle1, sb, announcerWidth);
      float totalSeconds = (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
      for (int index = 0; index < this.UpgradeNotifiers.Count; ++index)
      {
        PointClickerDaemon.UpgradeNotifier upgradeNotifier = this.UpgradeNotifiers[index];
        upgradeNotifier.timer -= totalSeconds * 4f;
        if ((double) upgradeNotifier.timer <= 0.0)
        {
          this.UpgradeNotifiers.RemoveAt(index);
          --index;
        }
        else
        {
          Vector2 vector2 = GuiData.font.MeasureString(upgradeNotifier.text);
          sb.DrawString(GuiData.font, upgradeNotifier.text, new Vector2((float) rectangle1.X + (float) (((double) rectangle1.Width - (double) announcerWidth) / 2.0) + announcerWidth, (float) (rectangle1.Y + 10)), this.ThemeColorHighlight * upgradeNotifier.timer, 0.0f, vector2 / 2f, (float) (0.5 + (1.0 - (double) upgradeNotifier.timer) * 2.20000004768372), SpriteEffects.None, 0.9f);
          this.UpgradeNotifiers[index] = upgradeNotifier;
        }
      }
    }

    private void DrawStatsTextBlock(string anouncer, string main, string secondary, Rectangle bounds, SpriteBatch sb, float announcerWidth)
    {
      Vector2 pos = new Vector2((float) bounds.X, (float) bounds.Y);
      TextItem.doFontLabel(pos, anouncer, GuiData.font, new Color?(Utils.AddativeWhite), announcerWidth - 10f, (float) bounds.Height - 8f, true);
      pos.X += announcerWidth + 2f;
      TextItem.doFontLabel(new Vector2((float) ((double) bounds.X + (double) announcerWidth - 12.0), (float) bounds.Y), ":", GuiData.font, new Color?(Utils.AddativeWhite), 22f, (float) bounds.Height, false);
      TextItem.doFontLabel(pos, main, GuiData.font, new Color?(this.ThemeColorHighlight), (float) bounds.Width - announcerWidth, (float) bounds.Height - 8f, true);
      pos.Y += 29f;
      pos.X = (float) bounds.X;
      TextItem.doFontLabel(pos, "[" + secondary + "]", GuiData.smallfont, new Color?(Color.Gray), (float) bounds.Width, (float) bounds.Height, false);
    }

    private void DrawUpgrades(Rectangle bounds, SpriteBatch sb)
    {
      int panelHeight = 28;
      if (this.scrollerPanel == null)
        this.scrollerPanel = new ScrollableSectionedPanel(panelHeight, sb.GraphicsDevice);
      this.scrollerPanel.NumberOfPanels = this.upgradeNames.Count;
      Button.outlineOnly = true;
      Button.drawingOutline = false;
      int drawnThisCycle = 0;
      bool needsButtonChecks = bounds.Contains(GuiData.getMousePoint());
      this.scrollerPanel.Draw((Action<int, Rectangle, SpriteBatch>) ((index, drawAreaFull, sprBatch) =>
      {
        Rectangle destinationRectangle = new Rectangle(drawAreaFull.X, drawAreaFull.Y, drawAreaFull.Width - 12, drawAreaFull.Height);
        int myID = 115700 + index * 111;
        if (needsButtonChecks && Button.doButton(myID, destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height, "", new Color?(Color.Transparent)))
          this.PurchaseUpgrade(index);
        else if (!needsButtonChecks && GuiData.hot == myID)
          GuiData.hot = -1;
        bool flag1 = (double) this.upgradeCosts[index] <= (double) this.activeState.points;
        bool flag2 = flag1 && GuiData.hot == myID;
        if (GuiData.hot == myID)
          this.hoverIndex = index;
        if (flag2)
        {
          int height = destinationRectangle.Height;
          int num1 = 0;
          int num2 = 0;
          if (drawAreaFull.X == 0 && drawAreaFull.Y == 0)
          {
            if (drawnThisCycle == 0)
            {
              num1 = bounds.X;
              num2 = bounds.Y;
            }
            else
            {
              num1 = bounds.X;
              num2 = bounds.Y + bounds.Height - panelHeight / 2;
            }
          }
          Rectangle rectangle = new Rectangle(num1 + destinationRectangle.X - height, num2 + destinationRectangle.Y - height, destinationRectangle.Width + 2 * height, destinationRectangle.Height + 2 * height);
          for (int index1 = 0; index1 < this.Stars.Count; ++index1)
            this.DrawStar(rectangle, sb, this.Stars[index1]);
          sb.Draw(this.scanlinesTextBackground, rectangle, this.ThemeColor * (GuiData.active == myID ? 0.6f : 0.3f));
        }
        sprBatch.Draw(this.scanlinesTextBackground, destinationRectangle, new Rectangle?(new Rectangle(this.scanlinesTextBackground.Width / 2, this.scanlinesTextBackground.Height / 9 * 4, this.scanlinesTextBackground.Width / 2, this.scanlinesTextBackground.Height / 4)), flag1 ? this.ThemeColor * 0.2f : Utils.AddativeWhite * 0.08f);
        if (GuiData.hot == myID)
          RenderedRectangle.doRectangle(destinationRectangle.X + 1, destinationRectangle.Y + 1, destinationRectangle.Width - 2, destinationRectangle.Height - 2, new Color?(flag2 ? (GuiData.active == myID ? Color.Black : this.ThemeColorBacking) : Color.Black));
        if (index == 0)
          Utils.drawLine(sprBatch, new Vector2((float) (destinationRectangle.X + 1), (float) (destinationRectangle.Y + 1)), new Vector2((float) (destinationRectangle.X + destinationRectangle.Width - 2), (float) (destinationRectangle.Y + 1)), Vector2.Zero, this.ThemeColor, 0.8f);
        Utils.drawLine(sprBatch, new Vector2((float) (destinationRectangle.X + 1), (float) (destinationRectangle.Y + destinationRectangle.Height - 2)), new Vector2((float) (destinationRectangle.X + destinationRectangle.Width - 2), (float) (destinationRectangle.Y + destinationRectangle.Height - 2)), Vector2.Zero, this.ThemeColor, 0.8f);
        if (flag1)
          sprBatch.Draw(Utils.white, new Rectangle(destinationRectangle.X, destinationRectangle.Y + 1, 8, destinationRectangle.Height - 2), this.ThemeColor);
        string text = "[" + this.activeState.upgradeCounts[index].ToString("000") + "] " + this.upgradeNames[index];
        TextItem.doFontLabel(new Vector2((float) (destinationRectangle.X + 4 + (flag1 ? 10 : 0)), (float) (destinationRectangle.Y + 4)), text, GuiData.UISmallfont, new Color?(flag2 ? (GuiData.active == myID ? this.ThemeColor : (flag1 ? Color.White : Color.Gray)) : (flag1 ? Utils.AddativeWhite : Color.Gray)), (float) (destinationRectangle.Width - 6), float.MaxValue, false);
        ++drawnThisCycle;
      }), sb, bounds);
      Button.outlineOnly = false;
      Button.drawingOutline = true;
    }

    private void DrawMonospaceString(Rectangle bounds, SpriteBatch sb, string points)
    {
      points = points.Trim();
      float num1 = 65f;
      float num2 = ((float) points.Length + 1f) * num1;
      float scale = 1f;
      if ((double) num2 > (double) bounds.Width)
        scale = (float) bounds.Width / num2;
      float num3 = num2 * scale;
      float num4 = (float) (((double) bounds.Width - (double) num3) / 2.0);
      Vector2 vector2_1 = new Vector2((float) bounds.X + num4, (float) bounds.Y);
      for (int index = 0; index < points.Length; ++index)
      {
        string text = string.Concat((object) points[index]);
        Vector2 vector2_2 = GuiData.titlefont.MeasureString(text);
        float x = (float) ((double) num1 * (double) scale - (double) vector2_2.X * (double) scale / 2.0);
        sb.DrawString(GuiData.titlefont, text, vector2_1 + new Vector2(x, 0.0f), Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.67f);
        vector2_1.X += num1 * scale;
      }
    }

    private void DrawWelcome(Rectangle bounds, SpriteBatch sb)
    {
      float num1 = (float) this.logoRenderBase.Height / (float) this.logoRenderBase.Width;
      int num2 = 45;
      Rectangle dest = new Rectangle(bounds.X - num2 + 20, bounds.Y, bounds.Width + num2, (int) ((double) (bounds.Width + 2 * num2) * (double) num1));
      this.DrawLogo(dest, sb);
      Rectangle rectangle = new Rectangle(bounds.X, dest.Y + dest.Height, bounds.Width, 60);
      sb.Draw(this.scanlinesTextBackground, rectangle, Utils.AddativeWhite * 0.2f);
      for (int index = 0; index < this.Stars.Count; ++index)
        this.DrawStar(rectangle, sb, this.Stars[index]);
      rectangle.X += 100;
      rectangle.Width = bounds.Width - 200;
      rectangle.Y += 13;
      rectangle.Height = 35;
      if (Button.doButton(98373721, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "GO!", new Color?(Utils.AddativeWhite)))
      {
        this.activeState = (PointClickerDaemon.PointClickerGameState) null;
        string str = this.os.defaultUser.name.Replace(" ", "_");
        for (int index = 0; index < this.savesFolder.files.Count; ++index)
        {
          if (this.savesFolder.files[index].name.StartsWith(str))
          {
            this.userFilePath = this.savesFolder.files[index].name;
            this.activeState = PointClickerDaemon.PointClickerGameState.LoadFromString(this.savesFolder.files[index].data);
            break;
          }
        }
        if (this.activeState == null)
        {
          this.activeState = new PointClickerDaemon.PointClickerGameState(this.upgradeNames.Count);
          FileEntry fileEntry = new FileEntry(this.activeState.ToSaveString(), str + ".pcsav");
          this.savesFolder.files.Add(fileEntry);
          this.userFilePath = fileEntry.name;
        }
        this.state = PointClickerDaemon.PointClickerScreenState.Main;
        this.currentRate = 0.0f;
        this.ActiveStory = "";
        this.UpdateRate();
        this.UpdateStory();
        this.UpdatePoints();
      }
      if (!Button.doButton(98373732, bounds.X + 2, bounds.Y + bounds.Height - 19, 180, 18, LocaleTerms.Loc("Exit") + "  :<", new Color?(this.os.lockedColor)))
        return;
      this.os.display.command = "connect";
    }

    private struct PointClickerStar
    {
      public Vector2 Pos;
      public float scale;
      public float life;
      public float rot;
      public float timescale;
      public Color color;
    }

    private struct UpgradeNotifier
    {
      public string text;
      public float timer;
    }

    private class PointClickerGameState
    {
      public int currentStoryElement;
      public long points;
      public List<int> upgradeCounts;

      public PointClickerGameState(int upgradesTotal)
      {
        this.currentStoryElement = -1;
        this.points = 0L;
        this.upgradeCounts = new List<int>();
        for (int index = 0; index < upgradesTotal; ++index)
          this.upgradeCounts.Add(0);
      }

      public string ToSaveString()
      {
        string str = this.points.ToString() + "\n" + (object) this.currentStoryElement + "\n";
        for (int index = 0; index < this.upgradeCounts.Count; ++index)
          str = str + (object) this.upgradeCounts[index] + ",";
        return str;
      }

      public static PointClickerDaemon.PointClickerGameState LoadFromString(string save)
      {
        PointClickerDaemon.PointClickerGameState clickerGameState = new PointClickerDaemon.PointClickerGameState(0);
        string[] strArray = save.Split(Utils.newlineDelim);
        clickerGameState.points = Convert.ToInt64(strArray[0]);
        clickerGameState.currentStoryElement = Convert.ToInt32(strArray[1]);
        foreach (string str in strArray[2].Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries))
          clickerGameState.upgradeCounts.Add(Convert.ToInt32(str));
        return clickerGameState;
      }
    }

    private enum PointClickerScreenState
    {
      Welcome,
      Error,
      Main,
    }
  }
}
