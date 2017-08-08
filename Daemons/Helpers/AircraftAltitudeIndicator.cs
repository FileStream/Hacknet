// Decompiled with JetBrains decompiler
// Type: Hacknet.Daemons.Helpers.AircraftAltitudeIndicator
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.Daemons.Helpers
{
  public static class AircraftAltitudeIndicator
  {
    private static Texture2D WarningIcon;
    private static Texture2D PlaneIcon;

    public static void Init(ContentManager content)
    {
      AircraftAltitudeIndicator.WarningIcon = content.Load<Texture2D>("Sprites/Icons/CautionIcon");
      AircraftAltitudeIndicator.PlaneIcon = content.Load<Texture2D>("DLC/Sprites/Airplane");
    }

    public static void RenderAltitudeIndicator(Rectangle dest, SpriteBatch sb, int currentAltitude, bool IsInCriticalDescenet, bool IconFlashIsVisible, int maxAltitude = 50000, int upperReccomended = 40000, int lowerReccomended = 30000, int warningArea = 14000, int criticalFailureArea = 3000)
    {
      if (AircraftAltitudeIndicator.WarningIcon == null)
        AircraftAltitudeIndicator.Init(OS.currentInstance.content);
      bool flag = currentAltitude <= 0;
      if (flag)
        currentAltitude = maxAltitude;
      int width1 = Math.Min(dest.Width, 100);
      Rectangle destinationRectangle1 = new Rectangle(dest.X + dest.Width - width1, dest.Y, width1, dest.Height);
      int width2 = 200;
      Rectangle dest1 = new Rectangle(dest.X + dest.Width - width2, destinationRectangle1.Y, width2, 21);
      Color color = IsInCriticalDescenet ? Utils.AddativeRed : OS.currentInstance.highlightColor;
      Rectangle rectangle = destinationRectangle1;
      rectangle.Width = width1 / 2;
      rectangle.X = dest.X + dest.Width - rectangle.Width;
      sb.Draw(Utils.gradientLeftRight, rectangle, color * 0.2f);
      int heightForAltitude = AircraftAltitudeIndicator.GetHeightForAltitude(currentAltitude, maxAltitude, rectangle);
      destinationRectangle1.Y += heightForAltitude;
      destinationRectangle1.Height -= heightForAltitude;
      sb.Draw(Utils.gradientLeftRight, destinationRectangle1, color);
      AircraftAltitudeIndicator.DrawIndicatorForAltitude(dest1, maxAltitude, LocaleTerms.Loc("Maximum Altitude"), maxAltitude, rectangle, sb, color, true, true);
      AircraftAltitudeIndicator.DrawIndicatorForAltitude(dest1, upperReccomended, LocaleTerms.Loc("Maximum Cruising Altitude"), maxAltitude, rectangle, sb, color, false, false);
      AircraftAltitudeIndicator.DrawIndicatorForAltitude(dest1, lowerReccomended, LocaleTerms.Loc("Minimum Cruising Altitude"), maxAltitude, rectangle, sb, color, false, false);
      AircraftAltitudeIndicator.DrawIndicatorForAltitude(dest1, warningArea, LocaleTerms.Loc("Unsafe Altitude Margin"), maxAltitude, rectangle, sb, color, false, false);
      dest1.Height *= 2;
      AircraftAltitudeIndicator.DrawIndicatorForAltitude(dest1, criticalFailureArea, LocaleTerms.Loc("Critical Failure Region") + "\n- " + LocaleTerms.Loc("POINT OF NO RETURN") + " -", maxAltitude, rectangle, sb, Utils.makeColorAddative(color), true, false);
      dest1 = new Rectangle(dest1.X - 20, dest1.Y, dest1.Width + 20, dest1.Height + 10);
      AircraftAltitudeIndicator.DrawIndicatorForAltitude(dest1, currentAltitude, flag ? LocaleTerms.Loc("CRITICAL ERROR") + "\n" + LocaleTerms.Loc("SIGNAL LOST") : LocaleTerms.Loc("Current Altitude") + "\n" + string.Format("{0}ft", (object) currentAltitude), maxAltitude, rectangle, sb, color, true, false);
      int num = dest1.Height - 4;
      Rectangle rect = new Rectangle(dest1.X - num - 4, dest1.Y + AircraftAltitudeIndicator.GetHeightForAltitude(currentAltitude, maxAltitude, rectangle), num, num);
      Rectangle destinationRectangle2 = new Rectangle(dest1.X - num - 4, rect.Y, num + 4, dest1.Height);
      if (currentAltitude < lowerReccomended)
        PatternDrawer.draw(new Rectangle(destinationRectangle2.X, destinationRectangle2.Y, destinationRectangle2.Width + dest1.Width, destinationRectangle2.Height), 0.2f, Color.Transparent, Color.Red * 0.2f, sb);
      sb.Draw(Utils.white, destinationRectangle2, Color.Black * 0.4f);
      destinationRectangle2.Height = 1;
      sb.Draw(Utils.white, destinationRectangle2, color);
      rect.Y += 2;
      rect.X += 2;
      Rectangle destinationRectangle3 = Utils.InsetRectangle(rect, 4);
      if (IsInCriticalDescenet)
        sb.Draw(AircraftAltitudeIndicator.WarningIcon, destinationRectangle3, Color.Red * (IconFlashIsVisible ? 1f : 0.3f));
      else
        sb.Draw(AircraftAltitudeIndicator.PlaneIcon, destinationRectangle3, color);
    }

    private static int GetHeightForAltitude(int altitude, int maxAltitude, Rectangle glowBar)
    {
      float num = (float) altitude / (float) maxAltitude;
      return (int) ((double) glowBar.Height * (1.0 - (double) num));
    }

    private static void DrawIndicatorForAltitude(Rectangle dest, int altitude, string ElementTitle, int totalAltitude, Rectangle totalBar, SpriteBatch sb, Color c, bool LineAtTop = false, bool useGradientBacking = false)
    {
      dest.Y = totalBar.Y + AircraftAltitudeIndicator.GetHeightForAltitude(altitude, totalAltitude, totalBar);
      if (LineAtTop)
      {
        ++dest.Y;
        --dest.Height;
      }
      sb.Draw(useGradientBacking ? Utils.gradientLeftRight : Utils.white, dest, new Rectangle?(), Color.Black * (useGradientBacking ? 1f : 0.5f), 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.4f);
      TextItem.doFontLabelToSize(dest, ElementTitle, GuiData.font, c, true, true);
      if (LineAtTop)
      {
        --dest.Y;
        dest.Height = 1;
      }
      else
      {
        dest.Y += dest.Height - 2;
        dest.Height = 1;
      }
      sb.Draw(Utils.white, dest, c);
    }

    public static bool GetFlashRateFromTimer(float timer)
    {
      return (double) timer % 0.3 < 0.150000005960464;
    }
  }
}
