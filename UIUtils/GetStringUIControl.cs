// Decompiled with JetBrains decompiler
// Type: Hacknet.UIUtils.GetStringUIControl
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.UIUtils
{
  public static class GetStringUIControl
  {
    public static void StartGetString(string prompt, object os_obj)
    {
      ((OS) os_obj).execute("getString " + prompt);
    }

    public static string DrawGetStringControl(string prompt, Rectangle bounds, Action errorOccurs, Action cancelled, SpriteBatch sb, object os_obj, Color SearchButtonColor, Color CancelButtonColor, string upperPrompt = null, Color? BackingPanelColor = null)
    {
      OS os = (OS) os_obj;
      upperPrompt = upperPrompt == null ? prompt : upperPrompt;
      string str = "";
      int x = bounds.X + 6;
      int y1 = bounds.Y + 2;
      Vector2 zero = Vector2.Zero;
      if (upperPrompt.Length > 0)
      {
        Vector2 vector2 = TextItem.doMeasuredSmallLabel(new Vector2((float) x, (float) y1), upperPrompt, new Color?());
        y1 += (int) vector2.Y + 10;
      }
      string[] separator = new string[1]{ "#$#$#$$#$&$#$#$#$#" };
      string[] strArray = os.getStringCache.Split(separator, StringSplitOptions.None);
      if (strArray.Length > 1)
      {
        str = strArray[1];
        if (str.Equals(""))
          str = os.terminal.currentLine;
      }
      Rectangle destinationRectangle = new Rectangle(x, y1, bounds.Width - 12, bounds.Height - 46);
      sb.Draw(Utils.white, destinationRectangle, BackingPanelColor.HasValue ? BackingPanelColor.Value : os.darkBackgroundColor);
      int num = y1 + 18;
      string bracketedBit = "";
      string bracketedSection = Utils.ExtractBracketedSection(prompt, out bracketedBit);
      Vector2 vector2_1 = TextItem.doMeasuredSmallLabel(new Vector2((float) x, (float) num), bracketedSection + str, new Color?());
      TextItem.doSmallLabel(new Vector2((float) x, (float) ((double) num + (double) vector2_1.Y + 2.0)), bracketedBit, new Color?());
      destinationRectangle.X = x + (int) vector2_1.X + 2;
      destinationRectangle.Y = num;
      destinationRectangle.Width = 7;
      destinationRectangle.Height = 20;
      if ((double) os.timer % 1.0 < 0.300000011920929)
        sb.Draw(Utils.white, destinationRectangle, os.outlineColor);
      int y2 = num + (bounds.Height - 44);
      if (strArray.Length > 2 || Button.doButton(30, x, y2, 300, 22, LocaleTerms.Loc("Search"), new Color?(SearchButtonColor)))
      {
        if (strArray.Length <= 2)
          os.terminal.executeLine();
        if (str.Length > 0)
          return str;
        errorOccurs();
        return (string) null;
      }
      if (Button.doButton(38, x, y2 + 24, 300, 22, LocaleTerms.Loc("Cancel"), new Color?(CancelButtonColor)))
      {
        cancelled();
        os.terminal.clearCurrentLine();
        os.terminal.executeLine();
      }
      return (string) null;
    }

    public static void DrawGetStringControlInactive(string prompt, string valueText, Rectangle bounds, SpriteBatch sb, object os_obj, string upperPrompt = null)
    {
      OS os = (OS) os_obj;
      upperPrompt = upperPrompt == null ? prompt : upperPrompt;
      string str = valueText;
      int x = bounds.X + 6;
      int num1 = bounds.Y + 2;
      Vector2 vector2_1 = TextItem.doMeasuredSmallLabel(new Vector2((float) x, (float) num1), upperPrompt, new Color?());
      int y = num1 + ((int) vector2_1.Y + 10);
      Rectangle destinationRectangle = new Rectangle(x, y, bounds.Width - 12, bounds.Height - 46);
      sb.Draw(Utils.white, destinationRectangle, os.darkBackgroundColor);
      int num2 = y + 28;
      Vector2 vector2_2 = TextItem.doMeasuredSmallLabel(new Vector2((float) x, (float) num2), prompt + str, new Color?());
      destinationRectangle.X = x + (int) vector2_2.X + 2;
      destinationRectangle.Y = num2;
      destinationRectangle.Width = 7;
      destinationRectangle.Height = 20;
      int num3 = num2 + (bounds.Height - 44);
    }
  }
}
