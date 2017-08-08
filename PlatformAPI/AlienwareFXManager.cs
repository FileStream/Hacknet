// Decompiled with JetBrains decompiler
// Type: Hacknet.PlatformAPI.AlienwareFXManager
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using AlienFXManagedWrapper;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hacknet.PlatformAPI
{
  public static class AlienwareFXManager
  {
    public static bool IsRunning = false;
    private static bool HasUpdatedPostFlash = false;
    private static List<uint> deviceLightCounts = new List<uint>();
    private static List<List<string>> deviceLightDescriptions = new List<List<string>>();
    private const double MIN_SECONDS_BETWEEN_UPDATES = 0.1;
    private static ILightFXController LightFX;
    private static uint numDevices;
    private static DateTime TimeStarted;
    private static DateTime LastUpdateTime;
    private static Color LastLogoColor;
    private static Color LastMidKeyColor;
    private static Color LastOutKeyColor;
    private static Color LastOtherColor;

    public static void Init()
    {
      AlienwareFXManager.LightFX = (ILightFXController) new LightFXController();
      if (AlienwareFXManager.LightFX.LFX_Initialize() != LFX_Result.LFX_Success)
        return;
      int numDevices = (int) AlienwareFXManager.LightFX.LFX_GetNumDevices(out AlienwareFXManager.numDevices);
      for (uint devIndex = 0; devIndex < AlienwareFXManager.numDevices; ++devIndex)
      {
        uint numLights1;
        int numLights2 = (int) AlienwareFXManager.LightFX.LFX_GetNumLights(devIndex, out numLights1);
        AlienwareFXManager.deviceLightCounts.Add(numLights1);
        List<string> stringList = new List<string>();
        StringBuilder lightDesc = new StringBuilder((int) byte.MaxValue);
        for (uint lightIndex = 0; lightIndex < numLights1; ++lightIndex)
        {
          int lightDescription = (int) AlienwareFXManager.LightFX.LFX_GetLightDescription(devIndex, lightIndex, lightDesc, (int) byte.MaxValue);
          stringList.Add(lightDesc.ToString());
        }
        AlienwareFXManager.deviceLightDescriptions.Add(stringList);
      }
      AlienwareFXManager.TimeStarted = DateTime.Now;
      AlienwareFXManager.IsRunning = true;
    }

    public static void UpdateForOS(object OS_Obj)
    {
      if (!AlienwareFXManager.IsRunning)
        return;
      if (OS_Obj != null)
      {
        OS os = (OS) OS_Obj;
        double warningFlashTimer = (double) os.warningFlashTimer;
        AlienwareFXManager.UpdateLightsForThemeAndFlash(os);
        AlienwareFXManager.HasUpdatedPostFlash = false;
      }
      else
        AlienwareFXManager.CycleAllLights(Color.Red, Color.White, (float) (DateTime.Now - AlienwareFXManager.TimeStarted).TotalSeconds);
    }

    private static void UpdateLightsForThemeAndFlash(OS os)
    {
      if (os.thisComputer.disabled)
      {
        AlienwareFXManager.UpdateLightColors(Color.Black, Color.Black, Color.Black, Color.Black);
      }
      else
      {
        float amount = Math.Max(0.0f, Math.Min(1f, os.warningFlashTimer));
        Color KeyboardOuterColor = Color.Lerp(Color.Lerp(os.AFX_KeyboardOuter, Color.Red, amount), Color.White, os.PorthackCompleteFlashTime / PortHackExe.COMPLETE_LIGHT_FLASH_TIME);
        AlienwareFXManager.UpdateLightColors(Color.Lerp(Color.Lerp(os.AFX_WordLogo, Color.Red, amount), Color.White, os.MissionCompleteFlashTime / 3f), Color.Lerp(os.AFX_KeyboardMiddle, Color.Red, amount), KeyboardOuterColor, Color.Lerp(os.AFX_Other, Color.Red, amount));
      }
    }

    private static void CycleAllLights(Color from, Color to, float time)
    {
      if ((DateTime.Now - AlienwareFXManager.LastUpdateTime).TotalSeconds < 0.1)
        return;
      for (uint devIndex = 0; devIndex < AlienwareFXManager.numDevices; ++devIndex)
      {
        for (uint lightIndex = 0; lightIndex < AlienwareFXManager.deviceLightCounts[(int) devIndex]; ++lightIndex)
        {
          float amount = (float) ((Math.Sin((double) time / ((double) lightIndex / 2.0)) + 1.0) / 2.0);
          LFX_ColorStruct lightCol = AlienwareFXManager.Col2LFXC(Color.Lerp(from, to, amount));
          int num = (int) AlienwareFXManager.LightFX.LFX_SetLightColor(devIndex, lightIndex, ref lightCol);
        }
      }
      int num1 = (int) AlienwareFXManager.LightFX.LFX_Update();
      AlienwareFXManager.LastUpdateTime = DateTime.Now;
    }

    private static void UpdateLightColors(Color LogoColor, Color KeyboardMiddleColor, Color KeyboardOuterColor, Color StatusColor)
    {
      if (LogoColor == AlienwareFXManager.LastLogoColor && KeyboardMiddleColor == AlienwareFXManager.LastMidKeyColor && KeyboardOuterColor == AlienwareFXManager.LastOutKeyColor && StatusColor == AlienwareFXManager.LastOtherColor || (DateTime.Now - AlienwareFXManager.LastUpdateTime).TotalSeconds < 0.1)
        return;
      for (uint devIndex = 0; devIndex < AlienwareFXManager.numDevices; ++devIndex)
      {
        for (uint lightIndex = 0; lightIndex < AlienwareFXManager.deviceLightCounts[(int) devIndex]; ++lightIndex)
        {
          LFX_ColorStruct lightCol = AlienwareFXManager.Col2LFXC(LogoColor);
          string lower = AlienwareFXManager.deviceLightDescriptions[(int) devIndex][(int) lightIndex].ToLower();
          if (lower.Contains("keyboard"))
            lightCol = !lower.Contains("middle") ? AlienwareFXManager.Col2LFXC(KeyboardOuterColor) : AlienwareFXManager.Col2LFXC(KeyboardMiddleColor);
          else if (lower.Contains("status"))
            lightCol = AlienwareFXManager.Col2LFXC(StatusColor);
          int num = (int) AlienwareFXManager.LightFX.LFX_SetLightColor(devIndex, lightIndex, ref lightCol);
        }
      }
      int num1 = (int) AlienwareFXManager.LightFX.LFX_Update();
      AlienwareFXManager.LastLogoColor = LogoColor;
      AlienwareFXManager.LastMidKeyColor = KeyboardMiddleColor;
      AlienwareFXManager.LastOutKeyColor = KeyboardOuterColor;
      AlienwareFXManager.LastOtherColor = StatusColor;
      AlienwareFXManager.LastUpdateTime = DateTime.Now;
    }

    private static LFX_ColorStruct Col2LFXC(Color c)
    {
      float num = (float) ((int) c.R + (int) c.G + (int) c.B) / 765f;
      return new LFX_ColorStruct() { red = c.R, green = c.G, blue = c.B, brightness = byte.MaxValue };
    }

    public static void ReleaseHandle()
    {
      if (!AlienwareFXManager.IsRunning)
        return;
      int num = (int) AlienwareFXManager.LightFX.LFX_Release();
    }
  }
}
