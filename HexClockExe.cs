// Decompiled with JetBrains decompiler
// Type: Hacknet.HexClockExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  internal class HexClockExe : ExeModule
  {
    private bool stopUIChange = false;
    private OSTheme theme;

    public HexClockExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.name = "HexClock";
      this.ramCost = 55;
      this.IdentifierName = "HexClock";
      this.targetIP = this.os.thisComputer.ip;
      if (p.Length > 1 && (p[1].ToLower().StartsWith("-s") || p[1].ToLower().StartsWith("-n")))
        this.stopUIChange = true;
      else
        this.os.write("HexClock Running. Use -s or -n for restricted mode\n ");
      this.theme = ThemeManager.currentTheme;
    }

    public override void Killed()
    {
      base.Killed();
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      DateTime now = DateTime.Now;
      string str1 = "#";
      int num = now.Hour;
      string str2 = num.ToString("00");
      num = now.Minute;
      string str3 = num.ToString("00");
      num = now.Second;
      string str4 = num.ToString("00");
      string str5 = str1 + str2 + str3 + str4;
      Color color = Utils.ColorFromHexString(str5);
      if (!this.stopUIChange)
        this.AutoUpdateTheme(color);
      Rectangle contentAreaDest = this.GetContentAreaDest();
      this.spriteBatch.Draw(Utils.white, contentAreaDest, color);
      int width = (int) ((double) this.bounds.Width * 0.800000011920929);
      int height = 30;
      TextItem.doFontLabelToSize(new Rectangle(contentAreaDest.X + (contentAreaDest.Width / 2 - width / 2), contentAreaDest.Y + (contentAreaDest.Height / 2 - height / 2), width, height), str5, GuiData.font, Utils.AddativeWhite, false, false);
    }

    private void AutoUpdateTheme(Color c)
    {
      Color c1 = c;
      double h1;
      double s;
      double l1;
      Utils.RGB2HSL(c, out h1, out s, out l1);
      double sl = Math.Min(0.7, s);
      double num = Math.Max(0.2, l1);
      c = Utils.HSL2RGB(h1, sl, num);
      double l2 = Math.Max(0.35, num);
      double h2 = h1 - 0.5;
      if (h2 < 0.0)
        ++h2;
      Utils.HSL2RGB(h2, sl, l2);
      this.os.defaultHighlightColor = Utils.GetComplimentaryColor(c1);
      this.os.defaultTopBarColor = new Color(0, 0, 0, 60);
      this.os.highlightColor = this.os.defaultHighlightColor;
      this.os.highlightColor = this.os.defaultHighlightColor;
      this.os.moduleColorSolid = Color.Lerp(c, Utils.AddativeWhite, 0.5f);
      this.os.moduleColorSolidDefault = this.os.moduleColorSolid;
      this.os.moduleColorStrong = c;
      this.os.moduleColorStrong.A = (byte) 80;
      this.os.topBarColor = this.os.defaultTopBarColor;
      this.os.exeModuleTopBar = new Color(32, 22, 40, 80);
      this.os.exeModuleTitleText = new Color(91, 132, 207, 0);
      this.os.netmapToolTipColor = new Color(213, 245, (int) byte.MaxValue, 0);
      this.os.netmapToolTipBackground = new Color(0, 0, 0, 70);
      this.os.displayModuleExtraLayerBackingColor = new Color(0, 0, 0, 60);
      this.os.thisComputerNode = new Color(95, 220, 83);
      this.os.scanlinesColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, 15);
    }
  }
}
