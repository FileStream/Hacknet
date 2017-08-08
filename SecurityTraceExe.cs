// Decompiled with JetBrains decompiler
// Type: Hacknet.SecurityTraceExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;

namespace Hacknet
{
  internal class SecurityTraceExe : ExeModule
  {
    public SecurityTraceExe(Rectangle location, OS _os)
      : base(location, _os)
    {
      this.name = "SecurityTrace";
      this.IdentifierName = "Security Tracer";
      this.ramCost = 150;
      if (this.os.connectedComp == null)
        this.os.connectedComp = this.os.thisComputer;
      this.os.traceTracker.start(30f);
      this.os.warningFlash();
    }

    public override void Killed()
    {
      base.Killed();
      if (this.os.connectedComp != null && this.os.connectedComp.idName == "dGibson")
        return;
      this.os.traceTracker.stop();
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      Rectangle contentAreaDest = this.GetContentAreaDest();
      contentAreaDest.X += 2;
      contentAreaDest.Width -= 4;
      contentAreaDest.Height -= Module.PANEL_HEIGHT + 1;
      contentAreaDest.Y += Module.PANEL_HEIGHT;
      PatternDrawer.draw(contentAreaDest, -1f, Color.Transparent, this.os.darkBackgroundColor, this.spriteBatch);
      PatternDrawer.draw(contentAreaDest, 3f, Color.Transparent, this.os.lockedColor, this.spriteBatch, PatternDrawer.errorTile);
    }
  }
}
