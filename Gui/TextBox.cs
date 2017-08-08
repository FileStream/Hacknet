// Decompiled with JetBrains decompiler
// Type: Hacknet.Gui.TextBox
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Hacknet.Gui
{
  public static class TextBox
  {
    private static float keyRepeatDelay = 0.44f;
    public static int LINE_HEIGHT = 25;
    public static int cursorPosition = 0;
    public static int textDrawOffsetPosition = 0;
    private static int FramesSelected = 0;
    public static bool MaskingText = false;
    public static bool BoxWasActivated = false;
    public static bool UpWasPresed = false;
    public static bool DownWasPresed = false;
    public static bool TabWasPresed = false;
    private const float DELAY_BEFORE_KEY_REPEAT_START = 0.44f;
    private const float KEY_REPEAT_DELAY = 0.04f;
    private const int OUTLINE_WIDTH = 2;
    private static Keys lastHeldKey;

    public static string doTextBox(int myID, int x, int y, int width, int lines, string str, SpriteFont font)
    {
      string str1 = str;
      if (font == null)
        font = GuiData.smallfont;
      TextBox.BoxWasActivated = false;
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = x;
      tmpRect.Y = y;
      tmpRect.Width = width;
      tmpRect.Height = lines * TextBox.LINE_HEIGHT;
      if (tmpRect.Contains(GuiData.getMousePoint()))
        GuiData.hot = myID;
      else if (GuiData.hot == myID)
        GuiData.hot = -1;
      if (GuiData.mouseWasPressed())
      {
        if (GuiData.hot == myID)
        {
          if (GuiData.active == myID)
          {
            int num = GuiData.mouse.X - x;
            bool flag = false;
            for (int length = 1; length <= str.Length; ++length)
            {
              if ((double) font.MeasureString(str.Substring(0, length)).X > (double) num)
              {
                TextBox.cursorPosition = length - 1;
                break;
              }
              if (!flag)
                TextBox.cursorPosition = str.Length;
            }
          }
          else
          {
            GuiData.active = myID;
            TextBox.cursorPosition = str.Length;
          }
        }
        else if (GuiData.active == myID)
          GuiData.active = -1;
      }
      if (GuiData.active == myID)
      {
        GuiData.willBlockTextInput = true;
        str1 = TextBox.getStringInput(str1, GuiData.getKeyboadState(), GuiData.getLastKeyboadState());
        KeyboardState keyboardState = GuiData.getKeyboadState();
        int num;
        if (keyboardState.IsKeyDown(Keys.Enter))
        {
          keyboardState = GuiData.getLastKeyboadState();
          num = !keyboardState.IsKeyDown(Keys.Enter) ? 1 : 0;
        }
        else
          num = 1;
        if (num == 0)
        {
          TextBox.BoxWasActivated = true;
          GuiData.active = -1;
        }
      }
      ++TextBox.FramesSelected;
      tmpRect.X = x;
      tmpRect.Y = y;
      tmpRect.Width = width;
      tmpRect.Height = lines * TextBox.LINE_HEIGHT;
      GuiData.spriteBatch.Draw(Utils.white, tmpRect, GuiData.active == myID ? GuiData.Default_Lit_Backing_Color : (GuiData.hot == myID ? GuiData.Default_Selected_Color : GuiData.Default_Dark_Background_Color));
      tmpRect.X += 2;
      tmpRect.Y += 2;
      tmpRect.Width -= 4;
      tmpRect.Height -= 4;
      GuiData.spriteBatch.Draw(Utils.white, tmpRect, GuiData.Default_Light_Backing_Color);
      float num1 = (float) (((double) TextBox.LINE_HEIGHT - (double) font.MeasureString(str1).Y) / 2.0);
      GuiData.spriteBatch.DrawString(font, str1, new Vector2((float) (x + 2), (float) y + num1), Color.White);
      if (GuiData.active == myID)
      {
        tmpRect.X = (int) ((double) x + (double) font.MeasureString(str1.Substring(0, TextBox.cursorPosition)).X) + 3;
        tmpRect.Y = y + 2;
        tmpRect.Width = 1;
        tmpRect.Height = TextBox.LINE_HEIGHT - 4;
        GuiData.spriteBatch.Draw(Utils.white, tmpRect, TextBox.FramesSelected % 60 < 40 ? Color.White : Color.Gray);
      }
      return str1;
    }

    public static string doTerminalTextField(int myID, int x, int y, int width, int selectionHeight, int lines, string str, SpriteFont font)
    {
      string s = str;
      if (font == null)
        font = GuiData.smallfont;
      TextBox.BoxWasActivated = false;
      TextBox.UpWasPresed = false;
      TextBox.DownWasPresed = false;
      TextBox.TabWasPresed = false;
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = x;
      tmpRect.Y = y;
      tmpRect.Width = width;
      tmpRect.Height = 0;
      if (tmpRect.Contains(GuiData.getMousePoint()))
        GuiData.hot = myID;
      else if (GuiData.hot == myID)
        GuiData.hot = -1;
      if (GuiData.mouseWasPressed())
      {
        if (GuiData.hot == myID)
        {
          if (GuiData.active == myID)
          {
            int num = GuiData.mouse.X - x;
            bool flag = false;
            for (int length = 1; length <= str.Length; ++length)
            {
              if ((double) font.MeasureString(str.Substring(0, length)).X > (double) num)
              {
                TextBox.cursorPosition = length - 1;
                break;
              }
              if (!flag)
                TextBox.cursorPosition = str.Length;
            }
          }
          else
          {
            GuiData.active = myID;
            TextBox.cursorPosition = str.Length;
          }
        }
        else if (GuiData.active == myID)
          GuiData.active = -1;
      }
      int active1 = GuiData.active;
      bool flag1 = false;
      string filteredStringInput = TextBox.getFilteredStringInput(s, GuiData.getKeyboadState(), GuiData.getLastKeyboadState());
      KeyboardState keyboardState = GuiData.getKeyboadState();
      int num1;
      if (keyboardState.IsKeyDown(Keys.Enter))
      {
        keyboardState = GuiData.getLastKeyboadState();
        num1 = keyboardState.IsKeyDown(Keys.Enter) ? 1 : 0;
      }
      else
        num1 = 1;
      if (num1 == 0)
      {
        TextBox.BoxWasActivated = true;
        TextBox.cursorPosition = 0;
        TextBox.textDrawOffsetPosition = 0;
      }
      tmpRect.Height = lines * TextBox.LINE_HEIGHT;
      ++TextBox.FramesSelected;
      tmpRect.X = x;
      tmpRect.Y = y;
      tmpRect.Width = width;
      tmpRect.Height = 10;
      tmpRect.X += 2;
      tmpRect.Y += 2;
      tmpRect.Width -= 4;
      tmpRect.Height -= 4;
      float num2 = (float) (((double) TextBox.LINE_HEIGHT - (double) font.MeasureString(filteredStringInput).Y) / 2.0);
      string str1 = filteredStringInput;
      int num3 = 0;
      int startIndex = 0;
      int length1;
      for (string text = str1; (double) font.MeasureString(text).X > (double) (width - 5); text = str1.Substring(startIndex, length1))
      {
        ++num3;
        length1 = str1.Length - startIndex - (num3 - startIndex);
        if (length1 < 0)
          break;
      }
      if (TextBox.cursorPosition < TextBox.textDrawOffsetPosition)
        TextBox.textDrawOffsetPosition = Math.Max(0, TextBox.textDrawOffsetPosition - 1);
      while (TextBox.cursorPosition > TextBox.textDrawOffsetPosition + (str1.Length - num3))
        ++TextBox.textDrawOffsetPosition;
      if (str1.Length <= num3 || TextBox.textDrawOffsetPosition < 0)
        TextBox.textDrawOffsetPosition = TextBox.textDrawOffsetPosition > str1.Length - num3 ? 0 : str1.Length - num3;
      else if (TextBox.textDrawOffsetPosition > num3)
        num3 = TextBox.textDrawOffsetPosition;
      if (num3 > str1.Length)
        num3 = str1.Length - 1;
      if (TextBox.textDrawOffsetPosition >= str1.Length)
        TextBox.textDrawOffsetPosition = 0;
      string text1 = str1.Substring(TextBox.textDrawOffsetPosition, str1.Length - num3);
      if (TextBox.MaskingText)
      {
        string str2 = "";
        for (int index = 0; index < filteredStringInput.Length; ++index)
          str2 += "*";
        text1 = str2;
      }
      GuiData.spriteBatch.DrawString(font, text1, Utils.ClipVec2ForTextRendering(new Vector2((float) (x + 2), (float) y + num2)), Color.White);
      int active2 = GuiData.active;
      flag1 = false;
      if (filteredStringInput != "")
      {
        int length2 = Math.Min(TextBox.cursorPosition - TextBox.textDrawOffsetPosition, text1.Length);
        if (length2 <= 0)
          length2 = 1;
        tmpRect.X = text1.Length != 0 ? (int) ((double) x + (double) font.MeasureString(text1.Substring(0, length2)).X) + 3 : x;
      }
      else
        tmpRect.X = x + 3;
      tmpRect.Y = y + 2;
      tmpRect.Width = 1;
      tmpRect.Height = TextBox.LINE_HEIGHT - 4;
      if (LocaleActivator.ActiveLocaleIsCJK())
        tmpRect.Y += 4;
      GuiData.spriteBatch.Draw(Utils.white, tmpRect, TextBox.FramesSelected % 60 < 40 ? Color.White : Color.Gray);
      return filteredStringInput;
    }

    private static string getStringInput(string s, KeyboardState input, KeyboardState lastInput)
    {
      Keys[] pressedKeys = input.GetPressedKeys();
      for (int index = 0; index < pressedKeys.Length; ++index)
      {
        if (!lastInput.IsKeyDown(pressedKeys[index]))
        {
          if (!TextBox.IsSpecialKey(pressedKeys[index]))
          {
            string str = TextBox.ConvertKeyToChar(pressedKeys[index], input.IsKeyDown(Keys.LeftShift) || input.IsKeyDown(Keys.CapsLock));
            s = s.Substring(0, TextBox.cursorPosition) + str + s.Substring(TextBox.cursorPosition);
            ++TextBox.cursorPosition;
          }
          else
          {
            switch (pressedKeys[index])
            {
              case Keys.Delete:
              case Keys.OemClear:
              case Keys.Back:
                if (s.Length > 0 && TextBox.cursorPosition > 0)
                {
                  s = s.Substring(0, TextBox.cursorPosition - 1) + s.Substring(TextBox.cursorPosition);
                  --TextBox.cursorPosition;
                  break;
                }
                break;
              case Keys.Left:
                --TextBox.cursorPosition;
                if (TextBox.cursorPosition < 0)
                {
                  TextBox.cursorPosition = 0;
                  break;
                }
                break;
              case Keys.Right:
                ++TextBox.cursorPosition;
                if (TextBox.cursorPosition > s.Length)
                {
                  TextBox.cursorPosition = s.Length;
                  break;
                }
                break;
            }
          }
        }
      }
      return s;
    }

    private static string getFilteredStringInput(string s, KeyboardState input, KeyboardState lastInput)
    {
      foreach (char filteredKey in GuiData.getFilteredKeys())
      {
        s = s.Substring(0, TextBox.cursorPosition) + (object) filteredKey + s.Substring(TextBox.cursorPosition);
        ++TextBox.cursorPosition;
      }
      Keys[] pressedKeys = input.GetPressedKeys();
      if (pressedKeys.Length == 1 && lastInput.IsKeyDown(pressedKeys[0]))
      {
        if (pressedKeys[0] == TextBox.lastHeldKey && TextBox.IsSpecialKey(pressedKeys[0]))
        {
          TextBox.keyRepeatDelay -= GuiData.lastTimeStep;
          if ((double) TextBox.keyRepeatDelay <= 0.0)
          {
            s = TextBox.forceHandleKeyPress(s, pressedKeys[0], input, lastInput);
            TextBox.keyRepeatDelay = 0.04f;
          }
        }
        else
        {
          TextBox.lastHeldKey = pressedKeys[0];
          TextBox.keyRepeatDelay = 0.44f;
        }
      }
      else
      {
        for (int index = 0; index < pressedKeys.Length; ++index)
        {
          if (!lastInput.IsKeyDown(pressedKeys[index]) && TextBox.IsSpecialKey(pressedKeys[index]))
          {
            switch (pressedKeys[index])
            {
              case Keys.Back:
              case Keys.OemClear:
                if (s.Length > 0 && TextBox.cursorPosition > 0)
                {
                  s = s.Substring(0, TextBox.cursorPosition - 1) + s.Substring(TextBox.cursorPosition);
                  --TextBox.cursorPosition;
                  break;
                }
                break;
              case Keys.Tab:
                TextBox.TabWasPresed = true;
                break;
              case Keys.End:
                TextBox.cursorPosition = TextBox.cursorPosition = s.Length;
                break;
              case Keys.Home:
                TextBox.cursorPosition = 0;
                break;
              case Keys.Left:
                --TextBox.cursorPosition;
                if (TextBox.cursorPosition < 0)
                {
                  TextBox.cursorPosition = 0;
                  break;
                }
                break;
              case Keys.Up:
                TextBox.UpWasPresed = true;
                break;
              case Keys.Right:
                ++TextBox.cursorPosition;
                if (TextBox.cursorPosition > s.Length)
                {
                  TextBox.cursorPosition = s.Length;
                  break;
                }
                break;
              case Keys.Down:
                TextBox.DownWasPresed = true;
                break;
              case Keys.Delete:
                if (s.Length > 0 && TextBox.cursorPosition < s.Length)
                {
                  s = s.Substring(0, TextBox.cursorPosition) + s.Substring(TextBox.cursorPosition + 1);
                  break;
                }
                break;
            }
          }
        }
      }
      return s;
    }

    private static string forceHandleKeyPress(string s, Keys key, KeyboardState input, KeyboardState lastInput)
    {
      if (!TextBox.IsSpecialKey(key))
      {
        string str = TextBox.ConvertKeyToChar(key, input.IsKeyDown(Keys.LeftShift) || input.IsKeyDown(Keys.CapsLock) || input.IsKeyDown(Keys.RightAlt));
        s = s.Substring(0, TextBox.cursorPosition) + str + s.Substring(TextBox.cursorPosition);
        ++TextBox.cursorPosition;
      }
      else
      {
        switch (key)
        {
          case Keys.Delete:
          case Keys.OemClear:
          case Keys.Back:
            if (s.Length > 0 && TextBox.cursorPosition > 0)
            {
              s = s.Substring(0, TextBox.cursorPosition - 1) + s.Substring(TextBox.cursorPosition);
              --TextBox.cursorPosition;
              break;
            }
            break;
          case Keys.Tab:
            TextBox.TabWasPresed = true;
            break;
          case Keys.Left:
            --TextBox.cursorPosition;
            if (TextBox.cursorPosition < 0)
            {
              TextBox.cursorPosition = 0;
              break;
            }
            break;
          case Keys.Up:
            TextBox.UpWasPresed = true;
            break;
          case Keys.Right:
            ++TextBox.cursorPosition;
            if (TextBox.cursorPosition > s.Length)
            {
              TextBox.cursorPosition = s.Length;
              break;
            }
            break;
          case Keys.Down:
            TextBox.DownWasPresed = true;
            break;
        }
      }
      return s;
    }

    public static bool IsSpecialKey(Keys key)
    {
      int num = (int) key;
      return (num < 65 || num > 90) && (num < 48 || num > 57) && (key != Keys.Space && key != Keys.OemPeriod && (key != Keys.OemComma && key != Keys.OemTilde)) && (key != Keys.OemMinus && key != Keys.OemPipe && (key != Keys.OemOpenBrackets && key != Keys.OemCloseBrackets) && (key != Keys.OemQuotes && key != Keys.OemQuestion)) && key != Keys.OemPlus;
    }

    public static string ConvertKeyToChar(Keys key, bool shift)
    {
      switch (key)
      {
        case Keys.Tab:
          return "\t";
        case Keys.Enter:
          return "\n";
        case Keys.Space:
          return " ";
        case Keys.D0:
          return shift ? ")" : "0";
        case Keys.D1:
          return shift ? "!" : "1";
        case Keys.D2:
          return shift ? "@" : "2";
        case Keys.D3:
          return shift ? "#" : "3";
        case Keys.D4:
          return shift ? "$" : "4";
        case Keys.D5:
          return shift ? "%" : "5";
        case Keys.D6:
          return shift ? "^" : "6";
        case Keys.D7:
          return shift ? "&" : "7";
        case Keys.D8:
          return shift ? "*" : "8";
        case Keys.D9:
          return shift ? "(" : "9";
        case Keys.A:
          return shift ? "A" : "a";
        case Keys.B:
          return shift ? "B" : "b";
        case Keys.C:
          return shift ? "C" : "c";
        case Keys.D:
          return shift ? "D" : "d";
        case Keys.E:
          return shift ? "E" : "e";
        case Keys.F:
          return shift ? "F" : "f";
        case Keys.G:
          return shift ? "G" : "g";
        case Keys.H:
          return shift ? "H" : "h";
        case Keys.I:
          return shift ? "I" : "i";
        case Keys.J:
          return shift ? "J" : "j";
        case Keys.K:
          return shift ? "K" : "k";
        case Keys.L:
          return shift ? "L" : "l";
        case Keys.M:
          return shift ? "M" : "m";
        case Keys.N:
          return shift ? "N" : "n";
        case Keys.O:
          return shift ? "O" : "o";
        case Keys.P:
          return shift ? "P" : "p";
        case Keys.Q:
          return shift ? "Q" : "q";
        case Keys.R:
          return shift ? "R" : "r";
        case Keys.S:
          return shift ? "S" : "s";
        case Keys.T:
          return shift ? "T" : "t";
        case Keys.U:
          return shift ? "U" : "u";
        case Keys.V:
          return shift ? "V" : "v";
        case Keys.W:
          return shift ? "W" : "w";
        case Keys.X:
          return shift ? "X" : "x";
        case Keys.Y:
          return shift ? "Y" : "y";
        case Keys.Z:
          return shift ? "Z" : "z";
        case Keys.NumPad0:
          return "0";
        case Keys.NumPad1:
          return "1";
        case Keys.NumPad2:
          return "2";
        case Keys.NumPad3:
          return "3";
        case Keys.NumPad4:
          return "4";
        case Keys.NumPad5:
          return "5";
        case Keys.NumPad6:
          return "6";
        case Keys.NumPad7:
          return "7";
        case Keys.NumPad8:
          return "8";
        case Keys.NumPad9:
          return "9";
        case Keys.Multiply:
          return "*";
        case Keys.Add:
          return "+";
        case Keys.Subtract:
          return "-";
        case Keys.Decimal:
          return ".";
        case Keys.Divide:
          return "/";
        case Keys.OemSemicolon:
          return shift ? ":" : ";";
        case Keys.OemPlus:
          return shift ? "+" : "=";
        case Keys.OemComma:
          return shift ? "<" : ",";
        case Keys.OemMinus:
          return shift ? "_" : "-";
        case Keys.OemPeriod:
          return shift ? ">" : ".";
        case Keys.OemQuestion:
          return shift ? "?" : "/";
        case Keys.OemTilde:
          return shift ? "~" : "`";
        case Keys.OemOpenBrackets:
          return shift ? "{" : "[";
        case Keys.OemPipe:
          return shift ? "|" : "\\";
        case Keys.OemCloseBrackets:
          return shift ? "}" : "]";
        case Keys.OemQuotes:
          return shift ? "\"" : "'";
        default:
          return string.Empty;
      }
    }

    public static void moveCursorToEnd(string targetString)
    {
      TextBox.cursorPosition = targetString.Length;
    }
  }
}
