// Decompiled with JetBrains decompiler
// Type: Hacknet.NetmapOrganizerExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Hacknet
{
  internal class NetmapOrganizerExe : ExeModule, MainDisplayOverrideEXE
  {
    private bool AllowChaos = false;

    public bool DisplayOverrideIsActive { get; set; }

    public NetmapOrganizerExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.needsProxyAccess = false;
      this.name = "NetmapOrganizer";
      this.ramCost = 300;
      this.IdentifierName = "NetmapOrganizer";
      this.DisplayOverrideIsActive = false;
      for (int index = 1; index < p.Length; ++index)
      {
        if (p[index].ToLower().StartsWith("-c"))
          this.AllowChaos = true;
      }
    }

    public override void Update(float t)
    {
      base.Update(t);
    }

    public override void Completed()
    {
      base.Completed();
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      Rectangle contentAreaDest = this.GetContentAreaDest();
      ZoomingDotGridEffect.Render(contentAreaDest, this.spriteBatch, this.os.timer, this.os.highlightColor * 0.4f);
      int x = contentAreaDest.X + 10;
      int width = contentAreaDest.Width - 20;
      int y1 = contentAreaDest.Y + 50;
      if (this.isExiting)
        return;
      if (Button.doButton(10777001 + this.PID, x, y1, width, 20, LocaleTerms.Loc("Scatter"), new Color?(this.os.netMap.SortingAlgorithm == NetmapSortingAlgorithm.Scatter ? Color.White : this.os.highlightColor)))
        this.os.netMap.SortingAlgorithm = NetmapSortingAlgorithm.Scatter;
      int y2 = y1 + 25;
      if (Button.doButton(10777003 + this.PID, x, y2, width, 20, LocaleTerms.Loc("Grid"), new Color?(this.os.netMap.SortingAlgorithm == NetmapSortingAlgorithm.Grid ? Color.White : this.os.highlightColor)))
        this.os.netMap.SortingAlgorithm = NetmapSortingAlgorithm.Grid;
      int y3 = y2 + 25;
      if (Button.doButton(10777005 + this.PID, x, y3, width, 20, LocaleTerms.Loc("Scan Sequence Grid"), new Color?(this.os.netMap.SortingAlgorithm == NetmapSortingAlgorithm.LockGrid ? Color.White : this.os.highlightColor)))
      {
        this.os.netMap.SortingAlgorithm = NetmapSortingAlgorithm.LockGrid;
        this.os.netMap.visibleNodes = this.os.netMap.visibleNodes.Distinct<int>().ToList<int>();
      }
      int y4 = y3 + 25;
      if (this.AllowChaos && Button.doButton(10777019 + this.PID, x, y4, width, 20, LocaleTerms.Loc("CHAOS"), new Color?(this.os.netMap.SortingAlgorithm == NetmapSortingAlgorithm.Chaos ? Color.White : this.os.highlightColor)))
        this.os.netMap.SortingAlgorithm = NetmapSortingAlgorithm.Chaos;
      int num = y4 + 25;
      if (Button.doButton(10777088 + this.PID, x, contentAreaDest.Y + contentAreaDest.Height - 24, width, 20, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
        this.isExiting = true;
    }

    public void RenderMainDisplay(Rectangle dest, SpriteBatch sb)
    {
    }
  }
}
