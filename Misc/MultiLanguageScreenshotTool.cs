// Decompiled with JetBrains decompiler
// Type: Hacknet.Misc.MultiLanguageScreenshotTool
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Hacknet.Misc
{
  public static class MultiLanguageScreenshotTool
  {
    public static void CaptureMultiLanguageScreen(object os_obj)
    {
      OS os = (OS) os_obj;
      GameTime gameTime = new GameTime(os.lastGameTime.TotalGameTime, TimeSpan.Zero, false);
      string str1 = "Screenshots/";
      string activeLocale = Settings.ActiveLocale;
      for (int index = 0; index < LocaleActivator.SupportedLanguages.Count; ++index)
      {
        string code = LocaleActivator.SupportedLanguages[index].Code;
        LocaleActivator.ActivateLocale(code, os.content);
        Texture2D texture2D = (Texture2D) null;
        if (!Directory.Exists(str1 + code))
          Directory.CreateDirectory(str1 + code);
        string str2 = Guid.NewGuid().ToString() + ".png";
        using (FileStream fileStream = File.Create(str1 + code + "/" + str2))
          texture2D.SaveAsPng((Stream) fileStream, texture2D.Width, texture2D.Height);
      }
      LocaleActivator.ActivateLocale(activeLocale, os.content);
    }
  }
}
