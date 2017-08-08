// Decompiled with JetBrains decompiler
// Type: Hacknet.GuiData
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Text;

namespace Hacknet
{
  public static class GuiData
  {
    public static Vector2 temp = new Vector2();
    private static Point tmpPoint = new Point();
    public static Color tmpColor = new Color();
    public static Rectangle tmpRect = new Rectangle();
    public static Color Default_Selected_Color = new Color(0, 166, 235);
    public static Color Default_Unselected_Color = new Color((int) byte.MaxValue, 128, 0);
    public static Color Default_Backing_Color = new Color(30, 30, 50, 100);
    public static Color Default_Light_Backing_Color = new Color(80, 80, 100, (int) byte.MaxValue);
    public static Color Default_Lit_Backing_Color = new Color((int) byte.MaxValue, 199, 41, 100);
    public static Color Default_Dark_Neutral_Color = new Color(10, 10, 15, 200);
    public static Color Default_Dark_Background_Color = new Color(40, 40, 45, 180);
    public static Color Default_Trans_Grey = new Color(30, 30, 30, 100);
    public static Color Default_Trans_Grey_Bright = new Color(60, 60, 60, 100);
    public static Color Default_Trans_Grey_Dark = new Color(20, 20, 20, 200);
    public static Color Default_Trans_Grey_Strong = new Color(80, 80, 80, 100);
    public static Color Default_Trans_Grey_Solid = new Color(100, 100, 100, (int) byte.MaxValue);
    public static int lastMouseWheelPos = -1;
    private static int lastMouseScroll = 0;
    public static int hot = -1;
    public static int active = -1;
    public static int enganged = -1;
    public static bool blockingInput = false;
    public static bool blockingTextInput = false;
    public static bool willBlockTextInput = false;
    public static Vector2 scrollOffset = Vector2.Zero;
    public static float lastTimeStep = 0.016f;
    private static bool initialized = false;
    public static List<GuiData.FontCongifOption> FontConfigs = new List<GuiData.FontCongifOption>();
    public static Dictionary<string, List<GuiData.FontCongifOption>> LocaleFontConfigs = new Dictionary<string, List<GuiData.FontCongifOption>>();
    public static GuiData.FontCongifOption ActiveFontConfig = new GuiData.FontCongifOption();
    public static MouseState lastMouse;
    public static MouseState mouse;
    public static InputState lastInput;
    public static SpriteBatch spriteBatch;
    public static SpriteFont font;
    public static SpriteFont titlefont;
    public static SpriteFont smallfont;
    public static SpriteFont tinyfont;
    public static SpriteFont UITinyfont;
    public static SpriteFont UISmallfont;
    public static SpriteFont detailfont;
    private static TextInputHook TextInputHook;

    public static void InitFontOptions(ContentManager content)
    {
      string name = GuiData.ActiveFontConfig.name;
      GuiData.FontConfigs.Clear();
      GuiData.FontConfigs.Add(new GuiData.FontCongifOption()
      {
        name = "default",
        smallFont = content.Load<SpriteFont>("Font12"),
        tinyFont = content.Load<SpriteFont>("Font10"),
        bigFont = content.Load<SpriteFont>("Font23"),
        tinyFontCharHeight = 10f
      });
      if (string.IsNullOrEmpty(name))
        GuiData.ActiveFontConfig = GuiData.FontConfigs[0];
      GuiData.FontConfigs.Add(new GuiData.FontCongifOption()
      {
        name = "medium",
        smallFont = content.Load<SpriteFont>("Font14"),
        tinyFont = content.Load<SpriteFont>("Font12"),
        bigFont = content.Load<SpriteFont>("Font23"),
        tinyFontCharHeight = 14f
      });
      GuiData.FontConfigs.Add(new GuiData.FontCongifOption()
      {
        name = "large",
        smallFont = content.Load<SpriteFont>("Font16"),
        tinyFont = content.Load<SpriteFont>("Font14"),
        bigFont = content.Load<SpriteFont>("Font23"),
        tinyFontCharHeight = 16f
      });
      bool flag = false;
      for (int index = 0; index < GuiData.FontConfigs.Count; ++index)
      {
        if (GuiData.FontConfigs[index].name == name)
        {
          GuiData.ActivateFontConfig(GuiData.FontConfigs[index]);
          flag = true;
          break;
        }
      }
      if (!flag)
        GuiData.ActivateFontConfig(GuiData.FontConfigs[0]);
      if (GuiData.LocaleFontConfigs.ContainsKey("en-us"))
        return;
      GuiData.LocaleFontConfigs.Add("en-us", GuiData.FontConfigs);
    }

    public static void ActivateFontConfig(string configName)
    {
      List<GuiData.FontCongifOption> fontCongifOptionList = GuiData.FontConfigs;
      if (GuiData.LocaleFontConfigs.ContainsKey(Settings.ActiveLocale))
        fontCongifOptionList = GuiData.LocaleFontConfigs[Settings.ActiveLocale];
      for (int index = 0; index < fontCongifOptionList.Count; ++index)
      {
        if (fontCongifOptionList[index].name == configName)
        {
          GuiData.ActivateFontConfig(fontCongifOptionList[index]);
          break;
        }
      }
    }

    public static void ActivateFontConfig(GuiData.FontCongifOption config)
    {
      if (config.detailFont != null)
        GuiData.detailfont = config.detailFont;
      GuiData.smallfont = config.smallFont;
      GuiData.tinyfont = config.tinyFont;
      GuiData.font = config.bigFont;
      GuiData.ActiveFontConfig = config;
    }

    public static void init(GameWindow window)
    {
      if (GuiData.initialized)
        return;
      GuiData.TextInputHook = new TextInputHook(window.Handle);
      GuiData.initialized = true;
    }

    public static void doInput()
    {
      GuiData.lastMouse = GuiData.mouse;
      GuiData.mouse = Mouse.GetState();
      if (GuiData.lastMouseWheelPos == -1)
        GuiData.lastMouseWheelPos = GuiData.mouse.ScrollWheelValue;
      GuiData.lastMouseScroll = GuiData.lastMouseWheelPos - GuiData.mouse.ScrollWheelValue;
      GuiData.lastMouseWheelPos = GuiData.mouse.ScrollWheelValue;
      GuiData.blockingInput = false;
      GuiData.blockingTextInput = GuiData.willBlockTextInput;
      GuiData.willBlockTextInput = false;
    }

    public static void doInput(InputState input)
    {
      GuiData.doInput();
      GuiData.lastInput = input;
    }

    public static void setTimeStep(float t)
    {
      GuiData.lastTimeStep = t;
    }

    public static KeyboardState getKeyboadState()
    {
      return GuiData.lastInput.CurrentKeyboardStates[0];
    }

    public static KeyboardState getLastKeyboadState()
    {
      return GuiData.lastInput.LastKeyboardStates[0];
    }

    public static Vector2 getMousePos()
    {
      GuiData.temp.X = (float) GuiData.mouse.X - GuiData.scrollOffset.X;
      GuiData.temp.Y = (float) GuiData.mouse.Y - GuiData.scrollOffset.Y;
      return GuiData.temp;
    }

    public static Point getMousePoint()
    {
      GuiData.tmpPoint.X = GuiData.mouse.X - (int) GuiData.scrollOffset.X;
      GuiData.tmpPoint.Y = GuiData.mouse.Y - (int) GuiData.scrollOffset.Y;
      return GuiData.tmpPoint;
    }

    public static float getMouseWheelScroll()
    {
      return (float) (GuiData.lastMouseScroll / 120);
    }

    public static bool isMouseLeftDown()
    {
      return GuiData.mouse.LeftButton == ButtonState.Pressed;
    }

    public static bool mouseLeftUp()
    {
      return GuiData.lastMouse.LeftButton == ButtonState.Pressed && GuiData.mouse.LeftButton == ButtonState.Released;
    }

    public static bool mouseWasPressed()
    {
      return GuiData.lastMouse.LeftButton == ButtonState.Released && GuiData.mouse.LeftButton == ButtonState.Pressed;
    }

    public static void startDraw()
    {
      GuiData.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
    }

    public static void endDraw()
    {
      GuiData.spriteBatch.End();
    }

    public static char[] getFilteredKeys()
    {
      string buffer = GuiData.TextInputHook.Buffer;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < buffer.Length; ++index)
      {
        if ((int) buffer[index] >= 32 && (int) buffer[index] <= 126)
          stringBuilder.Append(buffer[index]);
      }
      GuiData.TextInputHook.clearBuffer();
      string str = stringBuilder.ToString();
      char[] chArray = new char[str.Length];
      for (int index = 0; index < str.Length; ++index)
        chArray[index] = str[index];
      return chArray;
    }

    public struct FontCongifOption
    {
      public SpriteFont smallFont;
      public SpriteFont tinyFont;
      public SpriteFont bigFont;
      public SpriteFont detailFont;
      public string name;
      public float tinyFontCharHeight;
    }
  }
}
