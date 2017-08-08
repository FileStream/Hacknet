// Decompiled with JetBrains decompiler
// Type: Hacknet.Daemons.Helpers.IRCSystem
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hacknet.Daemons.Helpers
{
  public class IRCSystem
  {
    private static string[] EntryLineDelimiter = new string[1]{ "\n#" };
    private int DrawnButtonIndex = 0;
    private int messagesAddedSinceLastView = 0;
    private bool isCurrentlyBeingViewed = true;
    public SoundEffect AttachmentPressedSound = (SoundEffect) null;
    private const string ACTIVE_LOG_FILENAME = "active.log";
    public const string ATTACHMENT_FLAG_PREFIX = "!ATTACHMENT:";
    public const string ANNOUNCE_FLAG_PREFIX = "!ANNOUNCEMENT!";
    private const string EntryLineDelimiterMarker = "\n#";
    private Folder StorageFolder;
    private FileEntry ActiveLogFile;
    public Action<string, string> LogAdded;

    public IRCSystem(Folder storageFolder)
    {
      this.StorageFolder = storageFolder;
      this.ActiveLogFile = this.StorageFolder.searchForFile("active.log");
      if (this.ActiveLogFile != null)
        return;
      this.ActiveLogFile = new FileEntry("#", "active.log");
      this.StorageFolder.files.Add(this.ActiveLogFile);
    }

    private List<IRCSystem.IRCLogEntry> GetLogsFromFile()
    {
      string[] strArray = this.ActiveLogFile.data.Substring(1).Split(IRCSystem.EntryLineDelimiter, StringSplitOptions.RemoveEmptyEntries);
      List<IRCSystem.IRCLogEntry> ircLogEntryList = new List<IRCSystem.IRCLogEntry>();
      for (int index = 0; index < strArray.Length; ++index)
        ircLogEntryList.Add(IRCSystem.IRCLogEntry.DeserializeSafe(strArray[index]));
      return ircLogEntryList;
    }

    public void AddLog(string author, string message, double timestampSecondsOffset)
    {
      DateTime dateTime = DateTime.Now - TimeSpan.FromSeconds(timestampSecondsOffset);
      int num = dateTime.Hour;
      string str1 = num.ToString("00");
      string str2 = ":";
      num = dateTime.Minute;
      string str3 = num.ToString("00");
      string timestamp = str1 + str2 + str3;
      this.AddLog(author, message, timestamp);
    }

    public void AddLog(string author, string message, string timestamp = null)
    {
      if (timestamp == null)
      {
        DateTime now = DateTime.Now;
        int num = now.Hour;
        string str1 = num.ToString("00");
        string str2 = ":";
        num = now.Minute;
        string str3 = num.ToString("00");
        timestamp = str1 + str2 + str3;
      }
      string str = new IRCSystem.IRCLogEntry() { Author = author, Message = message, Timestamp = timestamp }.Serialize();
      if (this.ActiveLogFile.data.Length > 1)
        str = "\n#" + str;
      this.ActiveLogFile.data += str;
      ++this.messagesAddedSinceLastView;
      if (this.LogAdded == null)
        return;
      this.LogAdded(author, message);
    }

    public void LeftView()
    {
      this.messagesAddedSinceLastView = 0;
      this.isCurrentlyBeingViewed = false;
    }

    public void Draw(Rectangle dest, SpriteBatch sb, bool CanWrite, string WriteUsername, Dictionary<string, Color> HighlightKeywords)
    {
      this.isCurrentlyBeingViewed = true;
      if (CanWrite)
        return;
      this.DrawLog(dest, sb, HighlightKeywords);
    }

    private void DrawLog(Rectangle dest, SpriteBatch sb, Dictionary<string, Color> HighlightKeywords)
    {
      List<IRCSystem.IRCLogEntry> logsFromFile = this.GetLogsFromFile();
      int lineHeight = (int) ((double) GuiData.ActiveFontConfig.tinyFontCharHeight + 4.0);
      int num1 = 4;
      int linesRemaining = (int) ((double) dest.Height / (double) lineHeight);
      int index = logsFromFile.Count - 1;
      int num2 = 0;
      int y = dest.Y;
      this.DrawnButtonIndex = 1892;
      for (; linesRemaining > 0 && index >= 0 && dest.Height - num2 > lineHeight; --index)
      {
        bool needsNewMessagesLineDraw = this.messagesAddedSinceLastView > 0 && this.messagesAddedSinceLastView < logsFromFile.Count && logsFromFile.Count - index == this.messagesAddedSinceLastView;
        linesRemaining -= this.DrawLogEntry(logsFromFile[index], dest, sb, HighlightKeywords, lineHeight, linesRemaining, y, needsNewMessagesLineDraw, out dest);
        dest.Y -= num1;
        num2 += num1;
      }
      if (index > -1 || linesRemaining <= 1)
        return;
      int height = lineHeight + 8;
      Rectangle rectangle = new Rectangle(dest.X, dest.Y + dest.Height - height, dest.Width, height);
      SpriteFont tinyfont = GuiData.tinyfont;
      string text = "--- " + LocaleTerms.Loc("Log Cleared by Administrator") + " ---";
      Vector2 vector2 = tinyfont.MeasureString(text);
      sb.DrawString(tinyfont, text, Utils.ClipVec2ForTextRendering(new Vector2((float) ((double) rectangle.X + (double) rectangle.Width / 2.0 - (double) vector2.X / 2.0), (float) ((double) rectangle.Y + (double) rectangle.Height / 2.0 - (double) vector2.Y / 2.0))), Color.Gray);
    }

    private int DrawLogEntry(IRCSystem.IRCLogEntry log, Rectangle startingDest, SpriteBatch sb, Dictionary<string, Color> HighlightKeywords, int lineHeight, int linesRemaining, int yNotToPass, bool needsNewMessagesLineDraw, out Rectangle dest)
    {
      dest = startingDest;
      int num1 = 55;
      int val1 = 76;
      int num2 = 4;
      if (Settings.ActiveLocale != "en-us")
        val1 = 78;
      if (GuiData.ActiveFontConfig.name.ToLower() == "medium")
        val1 = 92;
      else if (GuiData.ActiveFontConfig.name.ToLower() == "large")
        val1 = 115;
      string str = "<" + log.Author + ">";
      int num3 = (int) ((double) GuiData.tinyfont.MeasureString(str).X + (double) num2);
      int num4 = Math.Max(val1, (int) ((double) GuiData.tinyfont.MeasureString(str).X + (double) num2));
      int width = dest.Width - (num1 + num2 + num4);
      string message = log.Message;
      string[] strArray = new string[1]{ message };
      if (!log.Message.StartsWith("!ATTACHMENT:"))
        strArray = Utils.SuperSmartTwimForWidth(message, width, GuiData.tinyfont).Split(Utils.newlineDelim, StringSplitOptions.None);
      Rectangle dest1 = new Rectangle(dest.X + num1 + num2 + num4, dest.Y, dest.Width - (num1 + num2 + num4), dest.Height);
      Rectangle dest2 = new Rectangle(dest.X, dest.Y, num1 + num4, dest.Height);
      Color defaultColor1 = Color.LightBlue;
      if (HighlightKeywords.ContainsKey(log.Author))
        defaultColor1 = HighlightKeywords[log.Author];
      Color defaultColor2 = Color.Lerp(Color.White, defaultColor1, 0.22f);
      if (needsNewMessagesLineDraw)
      {
        int length = strArray.Length;
        Rectangle destinationRectangle = new Rectangle(dest.X, dest.Y + dest.Height - lineHeight * length + 1, dest.Width, 1);
        sb.Draw(Utils.white, destinationRectangle, Color.White * 0.5f);
      }
      for (int index = strArray.Length - 1; index >= 0 && linesRemaining > 0; --index)
      {
        if (index == 0)
        {
          this.DrawLine("[" + log.Timestamp + "] ", dest2, sb, Color.White);
          int x = dest2.X;
          dest2.X = dest2.X + dest2.Width - num3;
          this.DrawLine(str, dest2, sb, defaultColor1);
          dest2.X = x;
        }
        this.DrawLine(strArray[index], dest1, sb, defaultColor2);
        dest.Height -= lineHeight;
        dest1.Height = dest.Height;
        dest2.Height = dest.Height;
        --linesRemaining;
        if (dest.Y + dest.Height - 6 <= yNotToPass)
        {
          needsNewMessagesLineDraw = false;
          break;
        }
      }
      Rectangle destinationRectangle1 = dest1;
      destinationRectangle1.Width = 1;
      destinationRectangle1.X -= 5;
      destinationRectangle1.Height = lineHeight * strArray.Length + 4;
      destinationRectangle1.Y = dest1.Y + dest1.Height + 2;
      sb.Draw(Utils.white, destinationRectangle1, Color.White * 0.12f);
      return strArray.Length;
    }

    private void DrawLine(string line, Rectangle dest, SpriteBatch sb, Color defaultColor)
    {
      Vector2 vector2 = Utils.ClipVec2ForTextRendering(new Vector2((float) dest.X, (float) (dest.Y + dest.Height) - (GuiData.ActiveFontConfig.tinyFontCharHeight + 1f)));
      if (line.StartsWith("!ATTACHMENT:"))
      {
        if (!AttachmentRenderer.RenderAttachment(line.Substring("!ATTACHMENT:".Length), (object) OS.currentInstance, vector2, this.DrawnButtonIndex, this.AttachmentPressedSound))
          return;
        ++this.DrawnButtonIndex;
      }
      else if (line.StartsWith("!ANNOUNCEMENT!"))
      {
        Rectangle destinationRectangle = new Rectangle((int) vector2.X - 2, (int) vector2.Y, dest.Width + 2, (int) ((double) GuiData.ActiveFontConfig.tinyFontCharHeight + 6.0));
        sb.Draw(Utils.white, destinationRectangle, Color.Red * 0.22f);
        sb.DrawString(GuiData.tinyfont, line, vector2, defaultColor);
      }
      else
        sb.DrawString(GuiData.tinyfont, line, vector2, defaultColor);
    }

    public struct IRCLogEntry
    {
      private static string[] SplitDelmiter = new string[1]{ "//" };
      private const string Delimiter = "//";
      private const string SerializationDelimiterReplacement = "&dsr";
      public string Message;
      public string Timestamp;
      public string Author;

      public string Serialize()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(this.Timestamp == null ? "" : this.Timestamp);
        stringBuilder.Append("//");
        stringBuilder.Append(this.Author);
        stringBuilder.Append("//");
        stringBuilder.Append(this.Message.Replace("//", "&dsr"));
        return stringBuilder.ToString();
      }

      public static IRCSystem.IRCLogEntry Deserialize(string entry)
      {
        IRCSystem.IRCLogEntry ircLogEntry = new IRCSystem.IRCLogEntry();
        string[] strArray = entry.Split(IRCSystem.IRCLogEntry.SplitDelmiter, StringSplitOptions.None);
        ircLogEntry.Timestamp = strArray[0];
        ircLogEntry.Author = strArray[1];
        ircLogEntry.Message = strArray[2].Replace("&dsr", "//");
        return ircLogEntry;
      }

      public static IRCSystem.IRCLogEntry DeserializeSafe(string entry)
      {
        try
        {
          return IRCSystem.IRCLogEntry.Deserialize(entry);
        }
        catch (Exception ex)
        {
          return new IRCSystem.IRCLogEntry();
        }
      }
    }
  }
}
