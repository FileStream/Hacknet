// Decompiled with JetBrains decompiler
// Type: Hacknet.BootCrashAssistanceModule
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class BootCrashAssistanceModule : Module
  {
    public bool IsActive = false;
    private float elapsedTime = 0.0f;
    private List<string> SequenceBlocks = new List<string>();
    private int blocksComplete = 0;
    private bool AwaitingInput = false;
    internal bool ShouldSkipDialogueTypeout = false;
    private const float TimePerBlock = 9.2f;
    private const float TimeSubForEarlyLines = -8.3f;
    private const int NumberOfFastBlocks = 11;

    public BootCrashAssistanceModule(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.bounds = location;
      this.SequenceBlocks.Add("");
      this.SequenceBlocks.Add("");
      this.SequenceBlocks.Add("");
      this.SequenceBlocks.Add("");
      this.SequenceBlocks.Add("");
      this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/Untranslated/BootFailSequence0.txt"));
      this.SequenceBlocks.Add("");
      this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/Untranslated/BootFailSequence1.txt"));
      this.SequenceBlocks.Add("");
      this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/Untranslated/BootFailSequence2.txt"));
      this.SequenceBlocks.Add("");
      this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/Untranslated/BootFailSequence3.txt"));
      this.SequenceBlocks.Add(">");
      this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/BootFailSequenceCoel1.txt"));
      this.SequenceBlocks.Add(">");
      this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/BootFailSequenceCoel2.txt"));
      this.SequenceBlocks.Add(">");
      this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/BootFailSequenceCoel3.txt"));
    }

    public override void Update(float t)
    {
      base.Update(t);
      if (!this.IsActive)
        return;
      this.elapsedTime += t;
      if (!this.AwaitingInput && (double) this.elapsedTime > (this.blocksComplete < 11 ? -8.30000019073486 : 0.0) + 9.19999980926514)
      {
        this.elapsedTime = 0.0f;
        ++this.blocksComplete;
        if (this.blocksComplete >= this.SequenceBlocks.Count || this.ShouldSkipDialogueTypeout && this.blocksComplete == 12)
        {
          this.AwaitingInput = true;
          Game1.getSingleton().IsMouseVisible = true;
          this.blocksComplete = this.SequenceBlocks.Count - 1;
        }
      }
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.spriteBatch.Draw(Utils.white, this.bounds, this.os.darkBackgroundColor);
      Vector2 pos = new Vector2((float) this.bounds.X + 10f, (float) (this.bounds.Y + this.bounds.Height) - (GuiData.ActiveFontConfig.tinyFontCharHeight + 16f));
      Rectangle destinationRectangle = new Rectangle((int) pos.X, (int) pos.Y, 10, 14);
      this.spriteBatch.Draw(Utils.white, destinationRectangle, (double) this.os.timer % 0.300000011920929 < 0.150000005960464 ? Color.White : Color.White * 0.05f);
      List<string> stringList = new List<string>();
      Color c = Color.Lerp(Utils.AddativeWhite, Utils.makeColor((byte) 204, byte.MaxValue, (byte) 249, (byte) 0), 0.5f);
      float num1 = this.blocksComplete < 11 ? -8.3f : 0.0f;
      pos.Y -= 6f;
      if (this.AwaitingInput)
      {
        pos.Y -= 46f;
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
          if (Button.doButton(464191001, (int) pos.X, (int) pos.Y - 4, 220, 30, LocaleTerms.Loc("Proceed"), new Color?(Color.White)))
          {
            HostileHackerBreakinSequence.CopyHelpFile();
            HostileHackerBreakinSequence.OpenWindowsHelpDocument();
            HostileHackerBreakinSequence.OpenTerminal();
            HostileHackerBreakinSequence.CrashProgram();
          }
        }
        else
        {
          if (Button.doButton(464191002, (int) pos.X, (int) pos.Y - 4, 220, 30, LocaleTerms.Loc("README"), new Color?(Color.White)))
          {
            this.SequenceBlocks.Add("");
            this.SequenceBlocks.Add("");
            this.SequenceBlocks.Add("");
            this.SequenceBlocks.Add(HostileHackerBreakinSequence.GetHelpText());
            this.SequenceBlocks.Add("");
            this.blocksComplete = this.SequenceBlocks.Count - 1;
          }
          if (Button.doButton(464191003, (int) pos.X + 230, (int) pos.Y - 4, 220, 30, LocaleTerms.Loc("Terminal"), new Color?(Color.White)))
          {
            this.SequenceBlocks.Add("---------------------------------------");
            this.SequenceBlocks.Add("TERMINAL SHOULD BE OPEN ON YOUR SYSTEM.");
            this.SequenceBlocks.Add("");
            this.SequenceBlocks.Add("IF NOT, OPEN A TERMINAL TO");
            this.SequenceBlocks.Add(HostileHackerBreakinSequence.OpenTerminal());
            this.SequenceBlocks.Add("---------------------------------------");
            this.SequenceBlocks.Add("");
            this.blocksComplete = this.SequenceBlocks.Count - 1;
          }
          if (Button.doButton(464191001, (int) pos.X + 460, (int) pos.Y - 4, 220, 30, LocaleTerms.Loc("Crash VM"), new Color?(Color.White)))
          {
            HostileHackerBreakinSequence.CopyHelpFile();
            HostileHackerBreakinSequence.CrashProgram();
          }
        }
      }
      if (this.blocksComplete + 1 < this.SequenceBlocks.Count)
      {
        string[] strArray = this.SequenceBlocks[this.blocksComplete + 1].Split(Utils.robustNewlineDelim, StringSplitOptions.None);
        int num2 = (int) (Math.Min(1f, this.elapsedTime / (num1 + 9.2f)) * (float) (strArray.Length - 1));
        for (int index = num2; index >= 0; --index)
        {
          string str = strArray[index];
          if (index == num2 && this.blocksComplete + 1 >= 11)
          {
            float val1 = (num1 + 9.2f) / (float) Math.Max(1, strArray.Length - 1);
            float num3 = Math.Min(val1, this.elapsedTime - (float) num2 * val1);
            int length = (int) ((double) str.Length * ((double) num3 / (double) val1));
            str = str.Substring(0, length);
          }
          stringList.Add(str);
        }
      }
      for (int blocksComplete = this.blocksComplete; blocksComplete >= 0; --blocksComplete)
      {
        string[] strArray = this.SequenceBlocks[blocksComplete].Split(Utils.robustNewlineDelim, StringSplitOptions.None);
        for (int index = strArray.Length - 1; index >= 0; --index)
          stringList.Add(strArray[index]);
      }
      for (int index = 0; index < stringList.Count; ++index)
      {
        pos.Y -= GuiData.ActiveFontConfig.tinyFontCharHeight + 6f;
        this.DrawMonospace(stringList[index], GuiData.smallfont, pos, c, LocaleActivator.ActiveLocaleIsCJK() ? 13f : 10f);
      }
    }

    private void DrawMonospace(string text, SpriteFont font, Vector2 pos, Color c, float charWidth)
    {
      for (int index = 0; index < text.Length; ++index)
      {
        this.spriteBatch.DrawString(font, string.Concat((object) text[index]), Utils.ClipVec2ForTextRendering(pos), c);
        pos.X += charWidth;
      }
    }
  }
}
