// Decompiled with JetBrains decompiler
// Type: Hacknet.Utils
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Extensions;
using Hacknet.Misc;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using SDL2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace Hacknet
{
  internal static class Utils
  {
    public static float PARRALAX_MULTIPLIER = 1f;
    public static float MIN_DIFF_FOR_PARRALAX = 0.1f;
    public static Random random = new Random();
    public static byte[] byteBuffer = new byte[1];
    public static readonly string LevelStateFilename = "LevelState.lst";
    public static Color VeryDarkGray = new Color(22, 22, 22);
    public static Color SlightlyDarkGray = new Color(100, 100, 100);
    public static Color AddativeWhite = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, 0);
    public static Color AddativeRed = new Color((int) byte.MaxValue, 15, 15, 0);
    private static HSLColor hslColor = new HSLColor(1f, 1f, 1f);
    public static char[] newlineDelim = new char[1]{ '\n' };
    public static string[] robustNewlineDelim = new string[2]{ "\r\n", "\n" };
    public static char[] spaceDelim = new char[1]{ ' ' };
    public static string[] commaDelim = new string[3]{ " ,", ", ", "," };
    public static string[] directorySplitterDelim = new string[2]{ "/", "\\" };
    public static string[] WhitespaceDelim = new string[3]{ "\r\n", "\n", " " };
    public static LCG LCG = new LCG(true);
    public static Texture2D white;
    public static Texture2D gradient;
    public static Texture2D gradientLeftRight;
    public static AudioEmitter emitter;
    public static Vector3 vec3;
    public static StorageDevice device;
    public static Color col;

    public static Vector2 getParallax(Vector2 objectPosition, Vector2 CameraPosition, float objectDepth, float focusDepth)
    {
      if ((double) objectDepth >= (double) focusDepth)
      {
        float num = (double) objectDepth - (double) focusDepth > 0.100000001490116 ? (objectDepth - focusDepth) * Utils.PARRALAX_MULTIPLIER : 0.0f * Utils.PARRALAX_MULTIPLIER;
        return new Vector2((CameraPosition.X - objectPosition.X) * num, 0.0f);
      }
      float num1 = (double) objectDepth - (double) focusDepth < -0.0500000007450581 ? (objectDepth - focusDepth) * Utils.PARRALAX_MULTIPLIER : 0.0f;
      return new Vector2((float) (((double) CameraPosition.X - (double) objectPosition.X) * ((double) num1 == 0.0 ? 0.0 : (double) num1 - 1.0)), 0.0f);
    }

    public static void drawLine(SpriteBatch spriteBatch, Vector2 vector1, Vector2 vector2, Vector2 OffsetPosition, Color Colour, float Depth)
    {
      Texture2D white = Utils.white;
      float x = Vector2.Distance(vector1, vector2);
      float rotation = (float) Math.Atan2((double) vector2.Y - (double) vector1.Y, (double) vector2.X - (double) vector1.X);
      spriteBatch.Draw(white, OffsetPosition + vector1, new Rectangle?(), Colour, rotation, Vector2.Zero, new Vector2(x, 1f), SpriteEffects.None, Depth);
    }

    public static void drawLine(SpriteBatch spriteBatch, Vector2 vector1, Vector2 vector2, Vector2 OffsetPosition, Color Colour, float Depth, Texture2D altTex = null)
    {
      Texture2D texture = altTex == null ? Utils.white : altTex;
      float x = Vector2.Distance(vector1, vector2);
      float rotation = (float) Math.Atan2((double) vector2.Y - (double) vector1.Y, (double) vector2.X - (double) vector1.X);
      spriteBatch.Draw(texture, OffsetPosition + vector1, new Rectangle?(), Colour, rotation, Vector2.Zero, new Vector2(x, 1f), SpriteEffects.None, Depth);
    }

    public static void drawLineAlt(SpriteBatch spriteBatch, Vector2 vector1, Vector2 vector2, Vector2 OffsetPosition, Color Colour, float Depth, float width, Texture2D altTex = null)
    {
      Texture2D texture = altTex == null ? Utils.white : altTex;
      float num = Vector2.Distance(vector1, vector2);
      float rotation = (float) Math.Atan2((double) vector2.Y - (double) vector1.Y, (double) vector2.X - (double) vector1.X);
      float y = 1f;
      if (texture.Height > 1)
        y = 1f / (float) texture.Height;
      spriteBatch.Draw(texture, OffsetPosition + vector1, new Rectangle?(), Colour, rotation, Vector2.Zero, new Vector2(num / (float) texture.Width * width, y), SpriteEffects.FlipHorizontally, Depth);
    }

    public static bool keyPressed(InputState input, Keys key, PlayerIndex? player)
    {
      if (GuiData.blockingTextInput)
        return false;
      int index = 0;
      if (player.HasValue)
        index = (int) player.Value;
      KeyboardState currentKeyboardState = input.CurrentKeyboardStates[index];
      KeyboardState lastKeyboardState = input.LastKeyboardStates[index];
      return currentKeyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key);
    }

    public static bool buttonPressed(InputState input, Buttons button, PlayerIndex? player)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) player.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) player.Value];
      return currentGamePadState.IsButtonDown(button) && lastGamePadState.IsButtonUp(button);
    }

    public static bool arraysAreTheSame(Keys[] a, Keys[] b)
    {
      if (a.Length != b.Length)
        return false;
      for (int index = 0; index < a.Length; ++index)
      {
        if (a[index].CompareTo((object) b[index]) == 0)
          return false;
      }
      return true;
    }

    public static bool arraysAreTheSame(Buttons[] a, Buttons[] b)
    {
      if (a.Length != b.Length)
        return false;
      for (int index = 0; index < a.Length; ++index)
      {
        if (a[index].CompareTo((object) b[index]) == 0)
          return false;
      }
      return true;
    }

    public static float rand(float range)
    {
      return (float) (Utils.random.NextDouble() * (double) range - Utils.random.NextDouble() * (double) range);
    }

    public static float randm(float range)
    {
      return (float) Utils.random.NextDouble() * range;
    }

    public static float rand()
    {
      return (float) Utils.random.NextDouble();
    }

    public static byte getRandomByte()
    {
      Utils.random.NextBytes(Utils.byteBuffer);
      return Utils.byteBuffer[0];
    }

    public static Rectangle GetFullscreen()
    {
      Viewport viewport = GuiData.spriteBatch.GraphicsDevice.Viewport;
      return new Rectangle(0, 0, viewport.Width, viewport.Height);
    }

    public static AudioEmitter emitterAtPosition(float x, float y)
    {
      if (Utils.emitter == null)
        Utils.emitter = new AudioEmitter();
      Utils.vec3.X = x;
      Utils.vec3.Y = y;
      Utils.vec3.Z = 0.0f;
      Utils.emitter.Position = Utils.vec3;
      return Utils.emitter;
    }

    public static AudioEmitter emitterAtPosition(float x, float y, float z)
    {
      if (Utils.emitter == null)
        Utils.emitter = new AudioEmitter();
      Utils.vec3.X = x;
      Utils.vec3.Y = y;
      Utils.vec3.Z = z;
      Utils.emitter.Position = Utils.vec3;
      return Utils.emitter;
    }

    public static Texture2D White(ContentManager content)
    {
      if (Utils.white == null || Utils.white.IsDisposed)
        Utils.white = TextureBank.load("Other\\white", content);
      return Utils.white;
    }

    public static Color makeColor(byte r, byte g, byte b, byte a)
    {
      Utils.col.R = r;
      Utils.col.G = g;
      Utils.col.B = b;
      Utils.col.A = a;
      return Utils.col;
    }

    public static Color makeColorAddative(Color c)
    {
      Utils.col.R = c.R;
      Utils.col.G = c.G;
      Utils.col.B = c.B;
      Utils.col.A = (byte) 0;
      return Utils.col;
    }

    public static bool flipCoin()
    {
      return Utils.random.NextDouble() > 0.5;
    }

    public static byte randomCompType()
    {
      return Utils.flipCoin() ? (byte) 1 : (byte) 2;
    }

    public static void writeToFile(string data, string filename)
    {
      using (StreamWriter streamWriter = new StreamWriter(filename))
      {
        streamWriter.Write(data);
        streamWriter.Flush();
        streamWriter.Close();
      }
    }

    public static void SafeWriteToFile(string data, string filename)
    {
      string str = filename + ".tmp";
      if (!Directory.Exists(str))
        Directory.CreateDirectory(Path.GetDirectoryName(str));
      using (StreamWriter streamWriter = new StreamWriter(str, false))
      {
        streamWriter.Write(data);
        streamWriter.Flush();
        streamWriter.Close();
      }
      if (System.IO.File.Exists(filename))
        System.IO.File.Delete(filename);
      System.IO.File.Move(str, filename);
    }

    public static void SafeWriteToFile(byte[] data, string filename)
    {
      string str = filename + ".tmp";
      if (!Directory.Exists(str))
        Directory.CreateDirectory(Path.GetDirectoryName(str));
      using (StreamWriter streamWriter = new StreamWriter(str, false))
      {
        streamWriter.Write((object) data);
        streamWriter.Flush();
        streamWriter.Close();
      }
      System.IO.File.Delete(filename);
      System.IO.File.Move(str, filename);
    }

    public static void appendToFile(string data, string filename)
    {
      StreamWriter streamWriter = new StreamWriter(filename, true);
      streamWriter.Write(data);
      streamWriter.Close();
    }

    public static string readEntireFile(string filename)
    {
      if (Settings.ActiveLocale != "en-us")
        filename = LocalizedFileLoader.GetLocalizedFilepath(filename);
      StreamReader streamReader = new StreamReader((Stream) System.IO.File.OpenRead(filename));
      string end = streamReader.ReadToEnd();
      streamReader.Close();
      return LocalizedFileLoader.FilterStringForLocalization(end);
    }

    public static char getRandomLetter()
    {
      return Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * Utils.random.NextDouble() + 65.0)));
    }

    public static char getRandomChar()
    {
      if (Utils.random.NextDouble() > 0.7)
        return string.Concat((object) Math.Min((int) Math.Floor((double) Utils.random.Next(0, 10)), 9))[0];
      return Utils.getRandomLetter();
    }

    public static char getRandomNumberChar()
    {
      return string.Concat((object) Math.Min((int) Math.Floor((double) Utils.random.Next(0, 10)), 9))[0];
    }

    public static Color convertStringToColor(string input)
    {
      Color color = Color.White;
      char[] separator = new char[3]{ ',', ' ', '/' };
      string[] strArray = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length < 3)
        return color;
      byte num1 = byte.MaxValue;
      byte num2 = byte.MaxValue;
      byte num3 = byte.MaxValue;
      byte num4 = byte.MaxValue;
      for (int index = 0; index < 4; ++index)
      {
        if (strArray.Length > index)
        {
          try
          {
            byte num5 = Convert.ToByte(strArray[index]);
            switch (index)
            {
              case 0:
                num1 = num5;
                break;
              case 1:
                num2 = num5;
                break;
              case 2:
                num3 = num5;
                break;
              case 3:
                num4 = num5;
                break;
            }
          }
          catch (FormatException ex)
          {
          }
          catch (OverflowException ex)
          {
          }
        }
      }
      color = new Color((int) num1, (int) num2, (int) num3, (int) num4);
      return color;
    }

    public static string convertColorToParseableString(Color c)
    {
      return ((int) c.R).ToString() + "," + (object) c.G + "," + (object) c.B + ((int) c.A != (int) byte.MaxValue ? (object) ("," + (object) c.A) : (object) "");
    }

    public static void ClipLineSegmentsForRect(Rectangle dest, Vector2 left, Vector2 right, out Vector2 leftOut, out Vector2 rightOut)
    {
      leftOut = left;
      rightOut = right;
      if ((double) left.X < (double) dest.X)
        leftOut.X = (float) dest.X;
      if ((double) right.X < (double) dest.X)
        rightOut.X = (float) dest.X;
      if ((double) left.X > (double) (dest.X + dest.Width))
        leftOut.X = (float) (dest.X + dest.Width);
      if ((double) right.X > (double) (dest.X + dest.Width))
        rightOut.X = (float) (dest.X + dest.Width);
      if ((double) left.Y < (double) dest.Y)
        leftOut.Y = (float) dest.Y;
      if ((double) right.Y < (double) dest.Y)
        rightOut.Y = (float) dest.Y;
      if ((double) left.Y > (double) (dest.Y + dest.Height))
        leftOut.Y = (float) (dest.Y + dest.Height);
      if ((double) right.Y <= (double) (dest.Y + dest.Height))
        return;
      rightOut.Y = (float) (dest.Y + dest.Height);
    }

    public static Rectangle getClipRectForSpritePos(Rectangle bounds, Texture2D tex, Vector2 pos, float scale)
    {
      int num1 = (int) ((double) tex.Width * (double) scale);
      int num2 = (int) ((double) tex.Height * (double) scale);
      int y;
      int x = y = 0;
      int width = tex.Width;
      int height = tex.Height;
      if ((double) pos.X < (double) bounds.X)
        x += (int) (((double) bounds.X - (double) pos.X) / (double) scale);
      if ((double) pos.Y < (double) bounds.Y)
        y += (int) (((double) bounds.Y - (double) pos.Y) / (double) scale);
      if ((double) pos.X + (double) num1 > (double) (bounds.X + bounds.Width))
        width -= (int) (((double) pos.X + (double) num1 - (double) (bounds.X + bounds.Width)) * (1.0 / (double) scale));
      if ((double) pos.Y + (double) num2 > (double) (bounds.Y + bounds.Height))
        height -= (int) (((double) pos.Y + (double) num2 - (double) (bounds.Y + bounds.Height)) * (1.0 / (double) scale));
      if (x > tex.Width)
      {
        x = tex.Width;
        width = 0;
      }
      if (y > tex.Height)
      {
        y = tex.Height;
        height = 0;
      }
      return new Rectangle(x, y, width, height);
    }

    public static Rectangle getClipRectForSpritePos(Rectangle bounds, Texture2D tex, Vector2 pos, Vector2 scale)
    {
      int num1 = (int) ((double) tex.Width * (double) scale.X);
      int num2 = (int) ((double) tex.Height * (double) scale.Y);
      int y;
      int x = y = 0;
      int width = tex.Width;
      int height = tex.Height;
      if ((double) pos.X < (double) bounds.X)
      {
        int num3 = (int) (((double) bounds.X - (double) pos.X) / (double) scale.X);
        x += num3;
        width -= num3;
      }
      if ((double) pos.Y < (double) bounds.Y)
      {
        int num3 = (int) (((double) bounds.Y - (double) pos.Y) / (double) scale.Y);
        y += num3;
        height -= num3;
      }
      if ((double) pos.X + (double) num1 > (double) (bounds.X + bounds.Width))
        width -= (int) (((double) pos.X + (double) num1 - (double) (bounds.X + bounds.Width)) * (1.0 / (double) scale.X));
      if ((double) pos.Y + (double) num2 > (double) (bounds.Y + bounds.Height))
        height -= (int) (((double) pos.Y + (double) num2 - (double) (bounds.Y + bounds.Height)) * (1.0 / (double) scale.Y));
      if (x > tex.Width)
      {
        x = tex.Width;
        width = 0;
      }
      if (y > tex.Height)
      {
        y = tex.Height;
        height = 0;
      }
      return new Rectangle(x, y, width, height);
    }

    public static Rectangle getClipRectForSpritePos(Rectangle bounds, Texture2D tex, Vector2 pos, Vector2 scale, Vector2 origin)
    {
      return Utils.getClipRectForSpritePos(bounds, tex, pos - origin * scale, scale);
    }

    public static void RenderSpriteIntoClippedRectDest(Rectangle fullBounds, Rectangle targetBounds, Texture2D tex, Color c, SpriteBatch sb)
    {
      Vector2 scale = new Vector2((float) targetBounds.Width / (float) tex.Width, (float) targetBounds.Height / (float) tex.Height);
      Vector2 pos = new Vector2((float) targetBounds.X, (float) targetBounds.Y);
      Rectangle rectForSpritePos = Utils.getClipRectForSpritePos(fullBounds, tex, pos, scale, Vector2.Zero);
      Vector2 position = pos + new Vector2((float) rectForSpritePos.X * scale.X, (float) rectForSpritePos.Y * scale.Y);
      sb.Draw(tex, position, new Rectangle?(rectForSpritePos), c, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
    }

    public static Vector2 GetCentreOrigin(this Texture2D tex)
    {
      return new Vector2((float) (tex.Width / 2), (float) (tex.Height / 2));
    }

    public static string SmartTwimForWidth(string data, int width, SpriteFont font)
    {
      string[] strArray = data.Split(new string[2]{ "\r\n", "\n" }, StringSplitOptions.None);
      for (int index1 = 0; index1 < strArray.Length; ++index1)
      {
        string text = strArray[index1];
        if ((double) font.MeasureString(text).X > (double) width)
        {
          string str1 = "";
          for (int index2 = 0; index2 < text.Length; ++index2)
          {
            bool flag = false;
            switch (text[index2])
            {
              case '\t':
              case ' ':
                str1 += (string) (object) text[index2];
                break;
              default:
                flag = true;
                break;
            }
            if (flag)
              break;
          }
          List<string> stringList = new List<string>((IEnumerable<string>) text.Substring(str1.Length).Split(new string[1]{ " " }, StringSplitOptions.None));
          string str2 = "";
          string str3 = "";
          while (stringList.Count > 0)
          {
            if ((double) font.MeasureString(str3 + " " + stringList[0]).X > (double) width)
            {
              str2 = str2 + str1 + str3.Trim() + "\r\n";
              str3 = "";
            }
            str3 = str3 + stringList[0] + " ";
            stringList.RemoveAt(0);
          }
          strArray[index1] = str2 + str1 + str3.Trim() + "\r\n";
        }
      }
      string str = "";
      for (int index = 0; index < strArray.Length; ++index)
        str = str + strArray[index] + "\r\n";
      return str.Trim();
    }

    public static Stream GenerateStreamFromString(string s)
    {
      MemoryStream memoryStream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter((Stream) memoryStream);
      streamWriter.Write(s);
      streamWriter.Flush();
      memoryStream.Position = 0L;
      return (Stream) memoryStream;
    }

    public static string SuperSmartTwimForWidth(string data, int width, SpriteFont font)
    {
      if (width <= 0)
        return data;
      string[] strArray = data.Split(new string[2]{ "\r\n", "\n" }, StringSplitOptions.None);
      for (int index1 = 0; index1 < strArray.Length; ++index1)
      {
        string text = strArray[index1];
        if ((double) font.MeasureString(text).X > (double) width)
        {
          string str1 = "";
          for (int index2 = 0; index2 < text.Length; ++index2)
          {
            bool flag = false;
            switch (text[index2])
            {
              case '\t':
              case ' ':
                str1 += (string) (object) text[index2];
                break;
              default:
                flag = true;
                break;
            }
            if (flag)
              break;
          }
          List<string> stringList = new List<string>((IEnumerable<string>) text.Substring(str1.Length).Split(new string[1]{ " " }, StringSplitOptions.None));
          string str2 = "";
          StringBuilder stringBuilder = new StringBuilder();
          while (stringList.Count > 0)
          {
            if ((double) font.MeasureString(stringBuilder.ToString() + " " + stringList[0]).X > (double) width)
            {
              str2 = str2 + str1 + stringBuilder.ToString().Trim() + "\r\n";
              stringBuilder.Clear();
            }
            int length1 = 1;
            if ((double) font.MeasureString(stringList[0]).X > (double) width)
            {
              int num1 = 0;
              int num2 = 1;
              if ((double) font.MeasureString(stringList[0].Substring(0, length1)).X >= (double) width && (double) font.MeasureString(stringList[0].Substring(0, length1 - 1)).X < (double) width)
                num2 = length1;
              int length2;
              for (length2 = num2; length2 < stringList[0].Length && (double) font.MeasureString(stringList[0].Substring(0, length2)).X < (double) width; ++length2)
              {
                int num3 = 40;
                if (length2 + num3 < stringList[0].Length && (double) font.MeasureString(stringList[0].Substring(0, length2 + num3)).X < (double) width)
                {
                  length2 += num3;
                  num1 += num3;
                }
              }
              int num4 = length2 - 1;
              if (num4 == 0)
              {
                stringBuilder.Append(stringList[0]);
                stringBuilder.Append(" ");
                stringList.RemoveAt(0);
              }
              else
              {
                string str3 = stringList[0];
                stringBuilder.Append(stringList[0].Substring(0, num4));
                stringBuilder.Append(" ");
                stringList[0] = str3.Substring(num4);
              }
            }
            else
            {
              stringBuilder.Append(stringList[0]);
              stringBuilder.Append(" ");
              stringList.RemoveAt(0);
            }
          }
          bool flag1 = strArray.Length > index1 + 1 && string.IsNullOrWhiteSpace(strArray[index1 + 1]);
          strArray[index1] = str2 + str1 + stringBuilder.ToString().Trim() + (flag1 ? "" : "\r\n");
        }
      }
      string str = "";
      for (int index = 0; index < strArray.Length; ++index)
        str = str + strArray[index] + "\r\n";
      return str.Trim();
    }

    public static float QuadraticOutCurve(float point)
    {
      return (float) ((-100000000.0 * (double) point * ((double) point - 2.0) - 1.0) / 100000000.0);
    }

    public static float CubicInCurve(float point)
    {
      return (float) (100000000.0 * (double) (point /= 1f) * (double) point * (double) point / 100000000.0);
    }

    public static float CubicOutCurve(float point)
    {
      return (float) (100000000.0 * ((double) (point = (float) ((double) point / 1.0 - 1.0)) * (double) point * (double) point + 1.0) / 100000000.0);
    }

    public static RenderTarget2D GetCurrentRenderTarget()
    {
      RenderTargetBinding[] renderTargets = GuiData.spriteBatch.GraphicsDevice.GetRenderTargets();
      if (renderTargets.Length == 0)
        return (RenderTarget2D) null;
      return (RenderTarget2D) renderTargets[0].RenderTarget;
    }

    public static float SmoothStep(float edge0, float edge1, float x)
    {
      x = (float) Math.Min(Math.Max(((double) x - (double) edge0) / ((double) edge1 - (double) edge0), 0.0), 1.0);
      return (float) ((double) x * (double) x * (double) x * ((double) x * ((double) x * 6.0 - 15.0) + 10.0));
    }

    public static Rectangle InsetRectangle(Rectangle rect, int inset)
    {
      return new Rectangle(rect.X + inset, rect.Y + inset, rect.Width - inset * 2, rect.Height - inset * 2);
    }

    public static Vector2 GetNearestPointOnCircle(Vector2 point, Vector2 CircleCentre, float circleRadius)
    {
      float num1 = point.X - CircleCentre.X;
      float num2 = point.Y - CircleCentre.Y;
      float num3 = (float) Math.Sqrt((double) num1 * (double) num1 + (double) num2 * (double) num2);
      return new Vector2(CircleCentre.X + num1 / num3 * circleRadius, CircleCentre.Y + num2 / num3 * circleRadius);
    }

    public static float Clamp(float val, float min, float max)
    {
      if ((double) val < (double) min)
        val = min;
      if ((double) val > (double) max)
        val = max;
      return val;
    }

    public static Vector2 Clamp(Vector2 val, float min, float max)
    {
      return new Vector2(Utils.Clamp(val.X, min, max), Utils.Clamp(val.Y, min, max));
    }

    public static string RandomFromArray(string[] array)
    {
      return array[Utils.random.Next(array.Length)];
    }

    public static string GetNonRepeatingFilename(string filename, string extension, Folder f)
    {
      string str = filename;
      int num = 0;
      bool flag;
      do
      {
        flag = true;
        for (int index = 0; index < f.files.Count; ++index)
        {
          if (f.files[index].name == str + extension)
          {
            ++num;
            str = filename + "(" + (object) num + ")";
            flag = false;
            break;
          }
        }
      }
      while (!flag);
      return str + extension;
    }

    public static string FlipRandomChars(string original, double chancePerChar)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < original.Length; ++index)
      {
        if (Utils.random.NextDouble() < chancePerChar)
          stringBuilder.Append(Utils.getRandomChar());
        else
          stringBuilder.Append(original[index]);
      }
      return stringBuilder.ToString();
    }

    public static Vector3 ColorToVec3(Color c)
    {
      float num = (float) c.A / (float) byte.MaxValue;
      return new Vector3((float) c.R / (float) byte.MaxValue * num, (float) c.G / (float) byte.MaxValue * num, (float) c.B / (float) byte.MaxValue * num);
    }

    public static Color AdditivizeColor(Color c)
    {
      Utils.col.R = c.R;
      Utils.col.G = c.G;
      Utils.col.B = c.B;
      Utils.col.A = (byte) 0;
      return Utils.col;
    }

    public static Vector2 RotatePoint(Vector2 point, float angle)
    {
      return Utils.PolarToCartesian(angle + Utils.GetPolarAngle(point), point.Length());
    }

    public static bool DebugGoFast()
    {
      return Settings.debugCommandsEnabled && GuiData.getKeyboadState().IsKeyDown(Keys.LeftAlt);
    }

    public static bool FieldContainsAttributeOfType(FieldInfo field, Type attributeType)
    {
      foreach (object customAttribute in field.GetCustomAttributes(true))
      {
        if (customAttribute.GetType() == attributeType)
          return true;
      }
      return false;
    }

    public static void SendRealWorldEmail(string subject, string to, string body)
    {
      try
      {
        MailAddress from = new MailAddress("fractalalligatordev@gmail.com");
        MailAddress to1 = new MailAddress(to);
        using (MailMessage message = new MailMessage(from, to1) { Subject = subject, Body = body })
          new SmtpClient()
          {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = ((ICredentialsByHost) new NetworkCredential(from.Address, "rgaekwivookrsfqg"))
          }.Send(message);
      }
      catch (Exception ex)
      {
      }
    }

    public static string GenerateReportFromException(Exception ex)
    {
      string str = ex.GetType().ToString() + "\r\n\r\n" + ex.Message + "\r\n\r\nSource : " + ex.Source + "\r\n\r\n" + (object) ex + "\r\n\r\n";
      if (ex.InnerException != null)
        str = str + "Inner : ---------------\r\n\r\n" + Utils.GenerateReportFromException(ex.InnerException).Replace("\t", "\0").Replace("\r\n", "\r\n\t").Replace("\0", "\t") + "\r\n\r\n";
      string data = FileSanitiser.purifyStringForDisplay(str + "\r\n\r\n" + ex.StackTrace);
      try
      {
        data = Utils.SuperSmartTwimForWidth(data, 800, GuiData.smallfont);
      }
      catch (Exception ex1)
      {
      }
      return data;
    }

    public static string GenerateReportFromExceptionCompact(Exception ex)
    {
      string data1 = ex.Message + "\r\n\r\nSource : " + ex.Source + "  ::  " + ex.GetType().ToString();
      if (ex.InnerException != null)
        data1 = data1 + "\r\nInner : ---------------\r\n" + Utils.GenerateReportFromExceptionCompact(ex.InnerException).Replace("\t", "\0").Replace("\r\n", "\r\n\t").Replace("\0", "\t") + "\r\n---------------\r\n\r\n";
      string data2 = FileSanitiser.purifyStringForDisplay(data1);
      try
      {
        data2 = Utils.SuperSmartTwimForWidth(data2, 800, GuiData.smallfont);
      }
      catch (Exception ex1)
      {
      }
      return data2;
    }

    public static bool FloatEquals(float a, float b)
    {
      return (double) Math.Abs(a - b) < 9.99999974737875E-05;
    }

    public static Vector2 PolarToCartesian(float angle, float magnitude)
    {
      return new Vector2(magnitude * (float) Math.Cos((double) angle), magnitude * (float) Math.Sin((double) angle));
    }

    public static float GetPolarAngle(Vector2 point)
    {
      return (float) Math.Atan2((double) point.Y, (double) point.X);
    }

    public static Vector3 NormalizeRotationVector(Vector3 rot)
    {
      return new Vector3(rot.X % 6.283185f, rot.Y % 6.283185f, rot.Z % 6.283185f);
    }

    public static void FillEverywhereExcept(Rectangle bounds, Rectangle fullscreen, SpriteBatch sb, Color col)
    {
      Rectangle destinationRectangle1 = new Rectangle(fullscreen.X, fullscreen.Y, bounds.X - fullscreen.X, fullscreen.Height);
      Rectangle destinationRectangle2 = new Rectangle(bounds.X, fullscreen.Y, bounds.Width, bounds.Y - fullscreen.Y);
      Rectangle destinationRectangle3 = new Rectangle(bounds.X, bounds.Y + bounds.Height, bounds.Width, fullscreen.Height - (bounds.Y + bounds.Height));
      Rectangle destinationRectangle4 = new Rectangle(bounds.X + bounds.Width, fullscreen.Y, fullscreen.Width - (bounds.X + bounds.Width), fullscreen.Height);
      sb.Draw(Utils.white, destinationRectangle1, col);
      sb.Draw(Utils.white, destinationRectangle4, col);
      sb.Draw(Utils.white, destinationRectangle2, col);
      sb.Draw(Utils.white, destinationRectangle3, col);
    }

    public static bool CheckStringIsRenderable(string input)
    {
      for (int index = 0; index < input.Length; ++index)
      {
        if ((int) input[index] != 10 && !GuiData.font.Characters.Contains(input[index]))
        {
          char ch = input[index];
          Console.WriteLine("\r\n------------------\r\nInvalid Char : {" + (object) input[index] + "}\r\n----------------------\r\n");
          return false;
        }
      }
      return true;
    }

    public static bool CheckStringIsTitleRenderable(string input)
    {
      string source = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890,./!@#$%^&*()<>\\\":;{}_-+= \r\n?[]`~'|";
      for (int index = 0; index < input.Length; ++index)
      {
        if (!source.Contains<char>(input[index]))
        {
          char ch = input[index];
          Console.WriteLine("\r\n------------------\r\nInvalid Char : {" + (object) input[index] + "}\r\n----------------------\r\n");
          return false;
        }
      }
      return true;
    }

    public static bool StringContainsInvalidFilenameChars(string input)
    {
      char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
      char[] chArray = new char[5]{ '&', '<', '>', '\'', '"' };
      for (int index = 0; index < invalidFileNameChars.Length; ++index)
      {
        if (input.Contains<char>(invalidFileNameChars[index]))
          return true;
      }
      for (int index = 0; index < chArray.Length; ++index)
      {
        if (input.Contains<char>(chArray[index]))
          return true;
      }
      return false;
    }

    public static string CleanStringToRenderable(string input)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string source = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890,./!@#$%^&*()<>\\\":;{}_-+= \r\n?[]`~'|";
      for (int index = 0; index < input.Length; ++index)
      {
        if (!source.Contains<char>(input[index]))
          stringBuilder.Append('?');
        else
          stringBuilder.Append(input[index]);
      }
      return stringBuilder.ToString();
    }

    public static string CleanFilterStringToRenderable(string input)
    {
      return Utils.CleanStringToLanguageRenderable(input.Replace("\t", "    ").Replace("…", "..."));
    }

    public static string CleanStringToLanguageRenderable(string input)
    {
      input = input.Replace("\t", "    ");
      StringBuilder stringBuilder = new StringBuilder();
      string source = "\r\n";
      for (int index = 0; index < input.Length; ++index)
      {
        if (source.Contains<char>(input[index]))
          stringBuilder.Append(input[index]);
        else if (!GuiData.font.Characters.Contains(input[index]))
          stringBuilder.Append('?');
        else
          stringBuilder.Append(input[index]);
      }
      return stringBuilder.ToString();
    }

    public static void AppendToErrorFile(string text)
    {
      try
      {
        if (!SDL.SDL_GetPlatform().Equals("Windows"))
          return;
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/My Games/Hacknet/Reports/";
        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);
        string str1 = "RuntimeErrors.txt";
        if (!System.IO.File.Exists(path + str1))
          System.IO.File.WriteAllText(path + str1, "Hacknet v" + MainMenu.OSVersion + " Runtime ErrorLog\r\n\r\n");
        string str2 = "-----\r\n" + MainMenu.OSVersion + " : " + DateTime.Now.ToString();
        using (StreamWriter streamWriter = System.IO.File.AppendText(path + str1))
        {
          streamWriter.WriteLine(str2);
          streamWriter.WriteLine(text);
        }
      }
      catch (Exception ex)
      {
      }
    }

    public static void AppendToWarningsFile(string text)
    {
      try
      {
        string path = Utils.GetFileLoadPrefix() + "warnings.txt";
        string str = "";
        if (System.IO.File.Exists(path))
          str = System.IO.File.ReadAllText(path) + "--------------------------------\r\n\r\n";
        string contents = str + text;
        System.IO.File.WriteAllText(path, contents);
      }
      catch (Exception ex)
      {
      }
    }

    public static string SerializeListToCSV(List<string> list)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < list.Count; ++index)
      {
        if (index != 0)
          stringBuilder.Append(',');
        stringBuilder.Append(list[index]);
      }
      return stringBuilder.ToString();
    }

    public static string[] SplitToTokens(string input)
    {
      return Regex.Matches(input, "[\\\"].+?[\\\"]|[^ ]+").Cast<Match>().Select<Match, string>((Func<Match, string>) (m => m.Value)).ToList<string>().ToArray();
    }

    public static string[] SplitToTokens(string[] input)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < input.Length; ++index)
      {
        stringBuilder.Append(input[index]);
        stringBuilder.Append(" ");
      }
      return Utils.SplitToTokens(stringBuilder.ToString());
    }

    public static string ExtractBracketedSection(string input, out string bracketedBit)
    {
      string str1 = input;
      bracketedBit = "";
      int num1 = input.IndexOf('(');
      if (num1 == -1)
        return str1;
      int num2 = input.Substring(num1).IndexOf(')');
      if (num2 == -1)
        return str1;
      int num3 = num2 + num1;
      string str2 = input.Substring(0, num1) + input.Substring(num3 + 1, input.Length - (num3 + 1));
      bracketedBit = input.Substring(num1, num3 - num1 + 1);
      return str2;
    }

    public static Color ColorFromHexString(string hexString)
    {
      if (hexString.StartsWith("#"))
        hexString = hexString.Substring(1);
      uint num = uint.Parse(hexString, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
      Color white = Color.White;
      if (hexString.Length == 8)
      {
        white.A = (byte) (num >> 24);
        white.R = (byte) (num >> 16);
        white.G = (byte) (num >> 8);
        white.B = (byte) num;
      }
      else
      {
        if (hexString.Length != 6)
          throw new InvalidOperationException("Invald hex representation of an ARGB or RGB color value.");
        white.R = (byte) (num >> 16);
        white.G = (byte) (num >> 8);
        white.B = (byte) num;
      }
      return white;
    }

    public static string ReadEntireContentsOfStream(Stream input)
    {
      string end = new StreamReader(input).ReadToEnd();
      input.Flush();
      input.Close();
      input.Dispose();
      return end;
    }

    public static void SendErrorEmail(Exception ex, string postfix = "", string extraData = "")
    {
      string body = Utils.GenerateReportFromException(ex) + "\r\n White:" + (object) Utils.white + "\r\n WhiteDisposed:" + (object) Utils.white.IsDisposed + "\r\n SmallFont:" + (object) GuiData.smallfont + "\r\n TinyFont:" + (object) GuiData.tinyfont + "\r\n LineEffectTarget:" + FlickeringTextEffect.GetReportString() + "\r\n PostProcessort stuff:" + PostProcessor.GetStatusReportString() + "\r\nRESOLUTION:\r\n " + (object) Game1.getSingleton().GraphicsDevice.PresentationParameters.BackBufferWidth + "x" + (object) Game1.getSingleton().GraphicsDevice.PresentationParameters.BackBufferHeight + "\r\nFullscreen: " + (Game1.getSingleton().graphics.IsFullScreen ? "true" : "false") + "\r\n Adapter: " + Game1.getSingleton().GraphicsDevice.Adapter.Description + "\r\n Device Name: " + Game1.getSingleton().GraphicsDevice.Adapter.DeviceName + "\r\n Status: " + (object) Game1.getSingleton().GraphicsDevice.GraphicsDeviceStatus + "\r\n Extra:\r\n" + extraData + "\r\n";
      string[] strArray1 = new string[7]{ "Hacknet ", postfix, MainMenu.OSVersion, " Crash ", null, null, null };
      string[] strArray2 = strArray1;
      int index1 = 4;
      DateTime now = DateTime.Now;
      string shortDateString = now.ToShortDateString();
      strArray2[index1] = shortDateString;
      strArray1[5] = " ";
      string[] strArray3 = strArray1;
      int index2 = 6;
      now = DateTime.Now;
      string shortTimeString = now.ToShortTimeString();
      strArray3[index2] = shortTimeString;
      Utils.SendRealWorldEmail(string.Concat(strArray1), "hacknetbugs+Hacknet@gmail.com", body);
    }

    public static void SendThreadedErrorReport(Exception ex, string postfix = "", string extraData = "")
    {
      new Thread((ThreadStart) (() => Utils.SendErrorEmail(ex, postfix, extraData)))
      {
        IsBackground = true,
        Name = (postfix + "_Errorthread")
      }.Start();
    }

    public static DateTime SafeParseDateTime(string input)
    {
      DateTime result = new DateTime();
      if (DateTime.TryParseExact(input, "dd/MM/yyyy HH:mm:ss", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result) || DateTime.TryParse(input, (IFormatProvider) new CultureInfo("en-au"), DateTimeStyles.None, out result) || DateTime.TryParse(input, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        return result;
      Console.WriteLine("Error Parsing DateTime : " + input);
      return DateTime.Now;
    }

    public static string SafeWriteDateTime(DateTime input)
    {
      return input.ToString("dd/MM/yyyy HH:mm:ss", (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static Color GetComplimentaryColor(Color c)
    {
      Utils.hslColor = HSLColor.FromRGB(c);
      Utils.hslColor.Luminosity = Math.Max(0.4f, Utils.hslColor.Luminosity);
      Utils.hslColor.Saturation = Math.Max(0.4f, Utils.hslColor.Saturation);
      Utils.hslColor.Saturation = Math.Min(0.55f, Utils.hslColor.Saturation);
      Utils.hslColor.Hue -= 3.141593f;
      if ((double) Utils.hslColor.Hue < 0.0)
        Utils.hslColor.Hue += 6.283185f;
      return Utils.hslColor.ToRGB();
    }

    public static void MeasureTimedSpeechSection()
    {
      string[] strArray = Utils.readEntireFile("Content/Post/BitSpeech.txt").Split(Utils.newlineDelim);
      float num1 = 0.0f;
      for (int index1 = 0; index1 < strArray.Length; ++index1)
      {
        string str = strArray[index1];
        float num2 = 0.0f;
        for (int index2 = 0; index2 < str.Length; ++index2)
        {
          if ((int) str[index2] == 35)
            ++num2;
          else if ((int) str[index2] == 37)
            num2 += 0.5f;
          else
            num2 += 0.05f;
        }
        Console.WriteLine("LINE " + (object) index1 + ": " + (object) num1 + "  --  " + (object) num2 + "   : " + strArray[index1]);
        num1 += num2;
      }
    }

    public static Color HSL2RGB(double h, double sl, double l)
    {
      double num1 = l;
      double num2 = l;
      double num3 = l;
      double num4 = l <= 0.5 ? l * (1.0 + sl) : l + sl - l * sl;
      if (num4 > 0.0)
      {
        double num5 = l + l - num4;
        double num6 = (num4 - num5) / num4;
        h *= 6.0;
        int num7 = (int) h;
        double num8 = h - (double) num7;
        double num9 = num4 * num6 * num8;
        double num10 = num5 + num9;
        double num11 = num4 - num9;
        switch (num7)
        {
          case 0:
            num1 = num4;
            num2 = num10;
            num3 = num5;
            break;
          case 1:
            num1 = num11;
            num2 = num4;
            num3 = num5;
            break;
          case 2:
            num1 = num5;
            num2 = num4;
            num3 = num10;
            break;
          case 3:
            num1 = num5;
            num2 = num11;
            num3 = num4;
            break;
          case 4:
            num1 = num10;
            num2 = num5;
            num3 = num4;
            break;
          case 5:
            num1 = num4;
            num2 = num5;
            num3 = num11;
            break;
        }
      }
      return new Color() { R = Convert.ToByte(num1 * (double) byte.MaxValue), G = Convert.ToByte(num2 * (double) byte.MaxValue), B = Convert.ToByte(num3 * (double) byte.MaxValue), A = byte.MaxValue };
    }

    public static void RGB2HSL(Color rgb, out double h, out double s, out double l)
    {
      double val1 = (double) rgb.R / (double) byte.MaxValue;
      double val2_1 = (double) rgb.G / (double) byte.MaxValue;
      double val2_2 = (double) rgb.B / (double) byte.MaxValue;
      h = 0.0;
      s = 0.0;
      l = 0.0;
      double num1 = Math.Max(Math.Max(val1, val2_1), val2_2);
      double num2 = Math.Min(Math.Min(val1, val2_1), val2_2);
      l = (num2 + num1) / 2.0;
      if (l <= 0.0)
        return;
      double num3 = num1 - num2;
      s = num3;
      if (s <= 0.0)
        return;
      s /= l <= 0.5 ? num1 + num2 : 2.0 - num1 - num2;
      double num4 = (num1 - val1) / num3;
      double num5 = (num1 - val2_1) / num3;
      double num6 = (num1 - val2_2) / num3;
      h = val1 != num1 ? (val2_1 != num1 ? (val1 == num2 ? 3.0 + num5 : 5.0 - num4) : (val2_2 == num2 ? 1.0 + num4 : 3.0 - num6)) : (val2_1 == num2 ? 5.0 + num6 : 1.0 - num5);
      h /= 6.0;
    }

    public static void ActOnAllFilesRevursivley(string foldername, Action<string> FileAction)
    {
      foreach (string file in Directory.GetFiles(foldername))
        FileAction(file);
      foreach (string directory in Directory.GetDirectories(foldername))
        Utils.ActOnAllFilesRevursivley(directory, FileAction);
    }

    public static string GetFileLoadPrefix()
    {
      if (Settings.IsInExtensionMode)
        return ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
      return "Content/";
    }

    public static Vector2 ClipVec2ForTextRendering(Vector2 input)
    {
      input.X = (float) (int) ((double) input.X + 0.5);
      input.Y = (float) (int) ((double) input.Y + 0.5);
      return input;
    }

    public static string SerializeObject(object o)
    {
      Type type = o.GetType();
      string str = type.Name;
      if (str.StartsWith("Hacknet."))
        str = str.Substring("Hacknet.".Length);
      StringBuilder stringBuilder = new StringBuilder("<" + str + ">");
      FieldInfo[] fields = type.GetFields();
      for (int index = 0; index < fields.Length; ++index)
      {
        string name = fields[index].Name;
        object obj = fields[index].GetValue(o);
        if (obj != null)
        {
          string parseableString = obj.ToString();
          if (fields[index].FieldType == typeof (Color))
            parseableString = Utils.convertColorToParseableString((Color) fields[index].GetValue(o));
          stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\n\t<{0}>{1}</{0}>", new object[2]
          {
            (object) name,
            (object) parseableString
          }));
        }
      }
      stringBuilder.Append("\n</" + str + ">");
      return stringBuilder.ToString();
    }

    public static object DeserializeObject(Stream s, Type t)
    {
      using (XmlReader xmlReader = XmlReader.Create(s))
      {
        object instance = Activator.CreateInstance(t);
        FieldInfo[] fields = t.GetFields();
        XmlNamespaceManager namespaceManager = new XmlNamespaceManager((XmlNameTable) new NameTable());
        while (!xmlReader.EOF)
        {
          if (!string.IsNullOrWhiteSpace(xmlReader.Name))
          {
            for (int index = 0; index < fields.Length; ++index)
            {
              if (fields[index].Name == xmlReader.Name)
              {
                int content = (int) xmlReader.MoveToContent();
                object obj = (object) null;
                if (fields[index].FieldType == typeof (Color))
                  obj = (object) Utils.convertStringToColor(xmlReader.ReadElementContentAsString());
                if (obj == null)
                  obj = xmlReader.ReadElementContentAs(fields[index].FieldType, (IXmlNamespaceResolver) namespaceManager);
                fields[index].SetValue(instance, obj);
              }
            }
          }
          xmlReader.Read();
        }
        return instance;
      }
    }

    public static void ProcessXmlElementInParent(XmlReader rdr, string ParentTag, string ElementTag, Action ProcessElement)
    {
      int num = 0;
      if (!(rdr.Name == ParentTag))
        return;
      do
      {
        if (rdr.Name == ElementTag && rdr.IsStartElement() && ProcessElement != null)
          ProcessElement();
        do
        {
          rdr.Read();
          ++num;
        }
        while (rdr.IsEmptyElement && !rdr.EOF);
        if (rdr.Name == ParentTag && num > 10)
          ++num;
        if (rdr.EOF)
          throw new FormatException("Unexpected End of File looking for " + ElementTag + " in " + ParentTag + " tag. Made " + (object) num + " reads.");
      }
      while (!(rdr.Name == ParentTag) || rdr.IsStartElement());
    }

    public static bool PublicInstancePropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
    {
      if ((object) self == null || (object) to == null)
        return (object) self == (object) to;
      Type type = self.GetType();
      List<string> stringList = new List<string>((IEnumerable<string>) ignore);
      foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
      {
        if (!stringList.Contains(property.Name))
        {
          object self1 = type.GetProperty(property.Name).GetValue((object) self, (object[]) null);
          object to1 = type.GetProperty(property.Name).GetValue((object) to, (object[]) null);
          if (property.PropertyType.IsClass && !property.PropertyType.Module.ScopeName.Equals("CommonLanguageRuntimeLibrary"))
          {
            if (!Utils.PublicInstancePropertiesEqual<object>(self1, to1, ignore))
              return false;
          }
          else if (self1 != to1 && (self1 == null || !self1.Equals(to1)))
            return false;
        }
      }
      return true;
    }

    public static Rectangle DrawSpriteAspectCorrect(Rectangle dest, SpriteBatch sb, Texture2D sprite, Color color, bool ForceToBottom = false)
    {
      Rectangle destinationRectangle = dest;
      float a = (float) dest.Width / (float) dest.Height;
      float b = (float) sprite.Width / (float) sprite.Height;
      if (!Utils.FloatEquals(a, b))
      {
        if ((double) a > (double) b)
        {
          int height = dest.Height;
          int width = (int) ((double) dest.Height * (double) b);
          int num = (int) ((double) (dest.Width - width) / 2.0);
          destinationRectangle = new Rectangle(dest.X + num, dest.Y, width, height);
        }
        else
        {
          int width = dest.Width;
          int height = (int) ((double) dest.Width / (double) b);
          int num = (int) ((double) (dest.Height - height) / (ForceToBottom ? 1.0 : 2.0));
          destinationRectangle = new Rectangle(dest.X, dest.Y + num, width, height);
        }
      }
      sb.Draw(sprite, destinationRectangle, color);
      return destinationRectangle;
    }

    public static int GetXForAlignment(AlignmentX align, int width, int margin, int objectWidth)
    {
      int num;
      switch (align)
      {
        case AlignmentX.Left:
          num = margin;
          break;
        case AlignmentX.Right:
          num = width - (objectWidth + margin);
          break;
        default:
          num = width / 2 - objectWidth / 2;
          break;
      }
      return num;
    }

    public static void LoadImageFromContentOrExtension(string imagePath, ContentManager content, Action<Texture2D> LoadComplete)
    {
      Texture2D texture2D = (Texture2D) null;
      if (imagePath == null)
        return;
      string path = Utils.GetFileLoadPrefix() + imagePath;
      if (path.EndsWith(".jpg") || path.EndsWith(".png"))
      {
        if (System.IO.File.Exists(path))
        {
          using (FileStream fileStream = System.IO.File.OpenRead(path))
            texture2D = Texture2D.FromStream(Game1.getSingleton().GraphicsDevice, (Stream) fileStream);
        }
      }
      else
        texture2D = !System.IO.File.Exists(path + ".xnb") ? content.Load<Texture2D>(imagePath) : content.Load<Texture2D>("../" + path);
      if (LoadComplete == null)
        return;
      LoadComplete(texture2D);
    }

    public static float RobustReadAsFloat(XmlReader rdr)
    {
      return (float) Convert.ToDouble(rdr.ReadContentAsString().Replace(",", "."), (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static string[] GetQuoteSeperatedArgs(string[] args)
    {
      List<string> stringList = new List<string>();
      StringBuilder stringBuilder1 = new StringBuilder();
      for (int index = 0; index < args.Length; ++index)
      {
        stringBuilder1.Append(args[index]);
        stringBuilder1.Append(" ");
      }
      string str = stringBuilder1.ToString().Trim();
      bool flag = false;
      StringBuilder stringBuilder2 = (StringBuilder) null;
      int index1 = -1;
      while (index1 < str.Length - 1)
      {
        ++index1;
        char ch = str[index1];
        if (stringBuilder2 == null)
        {
          if (flag && (int) ch == 34)
          {
            flag = false;
          }
          else
          {
            if (!flag)
            {
              if ((int) ch != 32)
              {
                if ((int) ch == 34)
                {
                  flag = true;
                  continue;
                }
              }
              else
                continue;
            }
            stringBuilder2 = new StringBuilder();
            stringBuilder2.Append(ch);
          }
        }
        else if (flag && (int) ch == 34)
        {
          stringList.Add(stringBuilder2.ToString());
          flag = false;
          stringBuilder2 = (StringBuilder) null;
        }
        else if (!flag && (int) ch == 32)
        {
          stringList.Add(stringBuilder2.ToString());
          flag = false;
          stringBuilder2 = (StringBuilder) null;
        }
        else
          stringBuilder2.Append(ch);
      }
      if (stringBuilder2 != null)
        stringList.Add(stringBuilder2.ToString());
      return stringList.ToArray();
    }

    public static SpriteFont GetTitleFontForLocalizedString(string data)
    {
      for (int index = 0; index < data.Length; ++index)
      {
        if (!GuiData.titlefont.Characters.Contains(data[index]))
          return GuiData.font;
      }
      return GuiData.titlefont;
    }

    public static void DrawStringMonospace(SpriteBatch spriteBatch, string text, SpriteFont font, Vector2 pos, Color c, float charWidth)
    {
      for (int index = 0; index < text.Length; ++index)
      {
        spriteBatch.DrawString(font, string.Concat((object) text[index]), Utils.ClipVec2ForTextRendering(pos), c);
        pos.X += charWidth;
      }
    }
  }
}
