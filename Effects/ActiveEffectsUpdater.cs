// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.ActiveEffectsUpdater
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet.Effects
{
  public class ActiveEffectsUpdater
  {
    private bool ScreenBleedActive = false;
    private float ScreenBleedTimeLeft = 0.0f;
    private float ScreenBleedStartTime = 0.0f;
    private string screenBleedTitle = "UNKNOWN";
    private string screenBleedCompleteAction = (string) null;
    private string screenBleedL1;
    private string screenBleedL2;
    private string screenBleedL3;
    private OSTheme oldTheme;
    private string oldThemePath;
    private CustomTheme oldCustomTheme;
    private OSTheme newTheme;
    private string newThemePath;
    private CustomTheme newCustomTheme;
    private float themeSwapTimeRemaining;
    private float originalThemeSwapTime;

    public void Update(float dt, object osobj)
    {
      OS os = (OS) osobj;
      if ((double) this.themeSwapTimeRemaining > 0.0)
      {
        this.themeSwapTimeRemaining -= dt;
        if ((double) this.themeSwapTimeRemaining <= 0.0)
        {
          this.CompleteThemeSwap((object) os);
          return;
        }
        bool flag = (double) Utils.randm(1f) > (double) this.themeSwapTimeRemaining / (double) this.originalThemeSwapTime;
        OSTheme theme = flag ? this.newTheme : this.oldTheme;
        string customThemePath = flag ? this.newThemePath : this.oldThemePath;
        ThemeManager.LastLoadedCustomTheme = flag ? this.newCustomTheme : this.oldCustomTheme;
        if (customThemePath != null)
          ThemeManager.switchTheme((object) os, customThemePath);
        else
          ThemeManager.switchTheme((object) os, theme);
      }
      if (this.ScreenBleedActive)
      {
        this.ScreenBleedTimeLeft -= dt;
        PostProcessor.dangerModePercentComplete = Math.Max(0.0f, Math.Min((float) (1.0 - (double) this.ScreenBleedTimeLeft / (double) this.ScreenBleedStartTime), 1f));
        PostProcessor.dangerModeEnabled = true;
        if ((double) this.ScreenBleedTimeLeft <= 0.0)
        {
          this.ScreenBleedActive = false;
          PostProcessor.dangerModePercentComplete = 0.0f;
          PostProcessor.dangerModeEnabled = false;
          if (!string.IsNullOrWhiteSpace(this.screenBleedCompleteAction))
            RunnableConditionalActions.LoadIntoOS(this.screenBleedCompleteAction, (object) os);
        }
        else
          os.postFXDrawActions += (Action) (() => TraceDangerSequence.DrawCountdownOverlay(Utils.CheckStringIsTitleRenderable(this.screenBleedTitle) ? GuiData.titlefont : GuiData.font, GuiData.smallfont, (object) os, this.screenBleedTitle, this.screenBleedL1, this.screenBleedL2, this.screenBleedL3));
      }
    }

    public void StartThemeSwitch(float time, OSTheme newTheme, object osobj, string customThemePath = null)
    {
      this.oldTheme = ThemeManager.currentTheme;
      this.oldThemePath = this.oldTheme == OSTheme.Custom ? ThemeManager.LastLoadedCustomThemePath : (string) null;
      this.oldCustomTheme = this.oldTheme == OSTheme.Custom ? ThemeManager.LastLoadedCustomTheme : (CustomTheme) null;
      if ((double) this.themeSwapTimeRemaining > 0.0)
        this.CompleteThemeSwap(osobj);
      this.originalThemeSwapTime = this.themeSwapTimeRemaining = time;
      this.newTheme = newTheme;
      this.newThemePath = customThemePath;
      try
      {
        if ((double) time <= 0.0)
          this.CompleteThemeSwap(osobj);
        else if (this.newThemePath != null)
        {
          ThemeManager.switchTheme(osobj, this.newThemePath);
          this.newCustomTheme = ThemeManager.LastLoadedCustomTheme;
        }
        else
          this.newCustomTheme = (CustomTheme) null;
      }
      catch (Exception ex)
      {
        time = this.themeSwapTimeRemaining = 0.0f;
        throw ex;
      }
    }

    public void CompleteThemeSwap(object osobj)
    {
      OS os = (OS) osobj;
      if (this.newThemePath != null)
      {
        ThemeManager.setThemeOnComputer((object) os.thisComputer, this.newThemePath);
        ThemeManager.switchTheme((object) os, this.newThemePath);
      }
      else
      {
        ThemeManager.setThemeOnComputer((object) os.thisComputer, this.newTheme);
        ThemeManager.switchTheme((object) os, this.newTheme);
      }
    }

    public void StartScreenBleed(float time, string title, string line1, string line2, string line3, string completeAction)
    {
      this.ScreenBleedStartTime = this.ScreenBleedTimeLeft = time;
      this.screenBleedTitle = title;
      this.screenBleedL1 = line1;
      this.screenBleedL2 = line2;
      this.screenBleedL3 = line3;
      this.screenBleedCompleteAction = completeAction;
      this.ScreenBleedActive = true;
    }

    public void CancelScreenBleedEffect()
    {
      this.ScreenBleedActive = false;
      PostProcessor.dangerModePercentComplete = 0.0f;
      PostProcessor.dangerModeEnabled = false;
    }
  }
}
