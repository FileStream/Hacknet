// Decompiled with JetBrains decompiler
// Type: Hacknet.Localization.LocaleFontLoader
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Hacknet.Localization
{
  public static class LocaleFontLoader
  {
    public static void LoadFontConfigForLocale(string locale, ContentManager content)
    {
      if (!GuiData.LocaleFontConfigs.ContainsKey(locale))
        GuiData.LocaleFontConfigs.Add(locale, LocaleFontLoader.LoadFontConfigSetForLocale(locale, "", content));
      GuiData.ActivateFontConfig(GuiData.ActiveFontConfig.name);
      GuiData.UITinyfont = GuiData.LocaleFontConfigs[locale][0].tinyFont;
      GuiData.UISmallfont = GuiData.LocaleFontConfigs[locale][0].smallFont;
    }

    private static List<GuiData.FontCongifOption> LoadFontConfigSetForLocale(string locale, string fontPrefix, ContentManager content)
    {
      List<GuiData.FontCongifOption> fontCongifOptionList = new List<GuiData.FontCongifOption>();
      string str = "Locales/" + locale + "/Fonts/" + locale + "_" + fontPrefix;
      bool flag = LocaleActivator.ActiveLocaleIsCJK();
      fontCongifOptionList.Add(new GuiData.FontCongifOption()
      {
        name = "default",
        detailFont = content.Load<SpriteFont>(str + "Font7"),
        smallFont = content.Load<SpriteFont>(str + "Font12"),
        tinyFont = content.Load<SpriteFont>(str + "Font10"),
        bigFont = content.Load<SpriteFont>(str + "Font23"),
        tinyFontCharHeight = flag ? 15f : 10f
      });
      if (flag)
      {
        fontCongifOptionList[0].smallFont.LineSpacing += 2;
        fontCongifOptionList[0].detailFont.LineSpacing += 2;
      }
      fontCongifOptionList.Add(new GuiData.FontCongifOption()
      {
        name = "medium",
        detailFont = content.Load<SpriteFont>(str + "Font7"),
        smallFont = content.Load<SpriteFont>(str + "Font14"),
        tinyFont = content.Load<SpriteFont>(str + "Font12"),
        bigFont = content.Load<SpriteFont>(str + "Font23"),
        tinyFontCharHeight = flag ? 17f : 14f
      });
      fontCongifOptionList.Add(new GuiData.FontCongifOption()
      {
        name = "large",
        detailFont = content.Load<SpriteFont>(str + "Font7"),
        smallFont = content.Load<SpriteFont>(str + "Font16"),
        tinyFont = content.Load<SpriteFont>(str + "Font14"),
        bigFont = content.Load<SpriteFont>(str + "Font23"),
        tinyFontCharHeight = flag ? 19f : 16f
      });
      return fontCongifOptionList;
    }
  }
}
