// Decompiled with JetBrains decompiler
// Type: Hacknet.UIUtils.AttractModeMenuScreen
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.UIUtils
{
  public class AttractModeMenuScreen
  {
    public Action Start;
    public Action Exit;
    public Action StartDLC;

    public void Draw(Rectangle dest, SpriteBatch sb)
    {
      int width1 = (int) ((double) dest.Width * 0.5);
      int val2 = (int) ((double) dest.Height * 0.400000005960464);
      int val1 = 400;
      bool flag = val1 > val2;
      int height1 = Math.Max(val1, val2);
      Rectangle dest1 = new Rectangle(dest.Width / 2 - width1 / 2, (int) ((double) dest.Y + (double) dest.Height / 2.5 - (double) (height1 / 2)), width1, height1);
      Rectangle destinationRectangle1 = new Rectangle(dest.X, dest1.Y + 115 + (flag ? 7 : 0), dest.Width, dest1.Height - 245);
      sb.Draw(Utils.white, destinationRectangle1, Color.Lerp(new Color(115, 0, 0), new Color(122, 0, 0), 0.0f + Utils.randm((float) (1.0 - (double) Utils.randm(1f) * (double) Utils.randm(1f)))));
      dest1.Y += 12;
      TextItem.doFontLabelToSize(dest1, Utils.FlipRandomChars("HACKNET", 0.028), GuiData.titlefont, Color.White * 0.12f, false, false);
      FlickeringTextEffect.DrawLinedFlickeringText(dest1, Utils.FlipRandomChars("HACKNET", 0.003), 11f, 0.7f, GuiData.titlefont, (object) null, Color.White, 5);
      int y = destinationRectangle1.Y + dest1.Height - 80;
      int width2 = dest.Width / 4;
      int num1 = 20;
      int num2 = (width2 - num1) / 2;
      int height2 = 42;
      Rectangle destinationRectangle2 = new Rectangle(dest.X + dest.Width / 2 - width2 / 2, y, width2, height2);
      if (Settings.MultiLingualDemo)
      {
        sb.Draw(Utils.white, destinationRectangle2, Color.Black * 0.8f);
        if (Button.doButton(18273302, destinationRectangle2.X, destinationRectangle2.Y, destinationRectangle2.Width, destinationRectangle2.Height, LocaleTerms.Loc("New Session"), new Color?(new Color(124, 137, 149))))
        {
          LocaleActivator.ActivateLocale("ko-kr", Game1.getSingleton().Content);
          if (this.Start != null)
            this.Start();
        }
        destinationRectangle2.Y += destinationRectangle2.Height + 12;
        sb.Draw(Utils.white, destinationRectangle2, Color.Black * 0.8f);
        if (Button.doButton(18273304, destinationRectangle2.X, destinationRectangle2.Y, destinationRectangle2.Width, destinationRectangle2.Height, "Hacknet Veterans", new Color?(new Color(124, 137, 149))))
        {
          LocaleActivator.ActivateLocale("en-us", Game1.getSingleton().Content);
          if (this.StartDLC != null)
            this.StartDLC();
        }
        destinationRectangle2.Y += destinationRectangle2.Height + 12;
        sb.Draw(Utils.white, destinationRectangle2, Color.Black * 0.8f);
        if (!Button.doButton(18273306, destinationRectangle2.X, destinationRectangle2.Y, destinationRectangle2.Width, destinationRectangle2.Height, "New Session", new Color?(new Color(124, 137, 149))))
          return;
        LocaleActivator.ActivateLocale("en-us", Game1.getSingleton().Content);
        if (this.Start != null)
          this.Start();
      }
      else
      {
        sb.Draw(Utils.white, destinationRectangle2, Color.Black * 0.8f);
        if (Button.doButton(18273302, destinationRectangle2.X, destinationRectangle2.Y, destinationRectangle2.Width, destinationRectangle2.Height, LocaleTerms.Loc("New Session"), new Color?(new Color(124, 137, 149))) && this.Start != null)
          this.Start();
        destinationRectangle2.Y += destinationRectangle2.Height + 12;
        if (Settings.DLCEnabledDemo)
        {
          sb.Draw(Utils.white, destinationRectangle2, Color.Black * 0.8f);
          if (Button.doButton(18273303, destinationRectangle2.X, destinationRectangle2.Y, destinationRectangle2.Width, destinationRectangle2.Height, LocaleTerms.Loc("Labyrinths (Hacknet Veterans)"), new Color?(new Color(184, 137, 149))) && this.StartDLC != null)
            this.StartDLC();
        }
      }
    }
  }
}
