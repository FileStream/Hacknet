// Decompiled with JetBrains decompiler
// Type: Hacknet.Terminal
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
  internal class Terminal : CoreModule
  {
    public static float PROMPT_OFFSET = 0.0f;
    private List<string> runCommands = new List<string>();
    public bool usingTabExecution = false;
    public bool preventingExecution = false;
    public bool executionPreventionIsInteruptable = false;
    private Color outlineColor = new Color(68, 68, 68);
    private Color backColor = new Color(8, 8, 8);
    private Color historyTextColor = new Color(220, 220, 220);
    private Color currentTextColor = Color.White;
    private List<string> history;
    private int commandHistoryOffset;
    public string currentLine;
    public string lastRunCommand;
    public string prompt;

    public Terminal(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
    }

    public override void LoadContent()
    {
      this.history = new List<string>(512);
      this.runCommands = new List<string>(512);
      this.commandHistoryOffset = 0;
      this.currentLine = "";
      this.lastRunCommand = "";
      this.prompt = "> ";
    }

    public override void Update(float t)
    {
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      float tinyFontCharHeight = GuiData.ActiveFontConfig.tinyFontCharHeight;
      this.spriteBatch.Draw(Utils.white, this.bounds, this.os.displayModuleExtraLayerBackingColor);
      int num = Math.Min((int) ((double) (this.bounds.Height - 12) / ((double) tinyFontCharHeight + 1.0)) - 3, this.history.Count);
      Vector2 input = new Vector2((float) (this.bounds.X + 4), (float) (this.bounds.Y + this.bounds.Height) - tinyFontCharHeight * 5f);
      if (num > 0)
      {
        for (int count = this.history.Count; count > this.history.Count - num; --count)
        {
          try
          {
            this.spriteBatch.DrawString(GuiData.tinyfont, this.history[count - 1], Utils.ClipVec2ForTextRendering(input), this.os.terminalTextColor);
            input.Y -= tinyFontCharHeight + 1f;
          }
          catch (Exception ex)
          {
          }
        }
      }
      this.doGui();
    }

    public void executeLine()
    {
      string str = this.currentLine;
      if (TextBox.MaskingText)
      {
        str = "";
        for (int index = 0; index < this.currentLine.Length; ++index)
          str += "*";
      }
      this.history.Add(this.prompt + str);
      this.lastRunCommand = this.currentLine;
      this.runCommands.Add(this.currentLine);
      if (!this.preventingExecution)
      {
        this.commandHistoryOffset = 0;
        this.os.execute(this.currentLine);
        if (this.currentLine.Length > 0)
          StatsManager.IncrementStat("commands_run", 1);
      }
      this.currentLine = "";
      TextBox.cursorPosition = 0;
      TextBox.textDrawOffsetPosition = 0;
      this.executionPreventionIsInteruptable = false;
    }

    public string GetRecentTerminalHistoryString()
    {
      string str = "";
      for (int index = this.history.Count - 1; index > this.history.Count - 30 && this.history.Count > index; --index)
        str = str + this.history[index] + "\r\n";
      return str;
    }

    public List<string> GetRecentTerminalHistoryList()
    {
      List<string> stringList = new List<string>();
      for (int index = 0; index < 30 && index < this.history.Count; ++index)
        stringList.Add(this.history[index]);
      return stringList;
    }

    public void NonThreadedInstantExecuteLine()
    {
      string str = this.currentLine;
      if (TextBox.MaskingText)
      {
        str = "";
        for (int index = 0; index < this.currentLine.Length; ++index)
          str += "*";
      }
      this.history.Add(this.prompt + str);
      this.lastRunCommand = this.currentLine;
      this.runCommands.Add(this.currentLine);
      if (!this.preventingExecution)
      {
        this.commandHistoryOffset = 0;
        ProgramRunner.ExecuteProgram((object) this.os, this.currentLine.Split(' '));
      }
      this.currentLine = "";
      TextBox.cursorPosition = 0;
      TextBox.textDrawOffsetPosition = 0;
      this.executionPreventionIsInteruptable = false;
    }

    public void doGui()
    {
      SpriteFont tinyfont = GuiData.tinyfont;
      float tinyFontCharHeight = GuiData.ActiveFontConfig.tinyFontCharHeight;
      int num1 = -4;
      int num2 = (int) ((double) (this.bounds.Y + this.bounds.Height - 16) - (double) tinyFontCharHeight - (double) num1);
      int x = (int) tinyfont.MeasureString(this.prompt).X;
      if (this.bounds.Width > 0)
      {
        for (; x >= (int) ((double) this.bounds.Width * 0.7); x = (int) tinyfont.MeasureString(this.prompt).X)
          this.prompt = this.prompt.Substring(1);
      }
      this.spriteBatch.DrawString(tinyfont, this.prompt, new Vector2((float) (this.bounds.X + 3), (float) num2), this.currentTextColor);
      if (LocaleActivator.ActiveLocaleIsCJK())
        num1 -= 4;
      int y = num2 + num1;
      if (!this.os.inputEnabled || this.inputLocked)
        return;
      TextBox.LINE_HEIGHT = (int) ((double) tinyFontCharHeight + 15.0);
      this.currentLine = TextBox.doTerminalTextField(7001, this.bounds.X + 3 + (int) Terminal.PROMPT_OFFSET + (int) tinyfont.MeasureString(this.prompt).X, y, this.bounds.Width - x - 4, this.bounds.Height, 1, this.currentLine, tinyfont);
      if (TextBox.BoxWasActivated)
        this.executeLine();
      if (TextBox.UpWasPresed && this.runCommands.Count > 0)
      {
        ++this.commandHistoryOffset;
        if (this.commandHistoryOffset > this.runCommands.Count)
          this.commandHistoryOffset = this.runCommands.Count;
        this.currentLine = this.runCommands[this.runCommands.Count - this.commandHistoryOffset];
        TextBox.cursorPosition = this.currentLine.Length;
      }
      if (TextBox.DownWasPresed && this.commandHistoryOffset > 0)
      {
        --this.commandHistoryOffset;
        if (this.commandHistoryOffset < 0)
          this.commandHistoryOffset = 0;
        this.currentLine = this.commandHistoryOffset > 0 ? this.runCommands[this.runCommands.Count - this.commandHistoryOffset] : "";
        TextBox.cursorPosition = this.currentLine.Length;
      }
      if (TextBox.TabWasPresed)
      {
        if (this.usingTabExecution)
          this.executeLine();
        else
          this.doTabComplete();
      }
    }

    public void doTabComplete()
    {
      List<string> stringList1 = new List<string>();
      if (this.currentLine.Length == 0)
        return;
      int length1 = this.currentLine.IndexOf(' ');
      if (length1 >= 1)
      {
        string path = this.currentLine.Substring(length1 + 1);
        string str1 = this.currentLine.Substring(0, length1);
        if (str1.Equals("upload") || str1.Equals("up"))
        {
          int length2 = path.LastIndexOf('/');
          if (length2 < 0)
            length2 = 0;
          string str2 = path.Substring(0, length2) + "/";
          if (str2.StartsWith("/"))
            str2 = str2.Substring(1);
          string str3 = path.Substring(length2 + (length2 == 0 ? 0 : 1));
          Folder folder = Programs.getFolderAtPathAsFarAsPossible(path, this.os, this.os.thisComputer.files.root);
          bool flag = false;
          if (folder == this.os.thisComputer.files.root && str2.Length > 1)
            flag = true;
          if (folder == null)
            folder = this.os.thisComputer.files.root;
          if (!flag)
          {
            for (int index = 0; index < folder.folders.Count; ++index)
            {
              if (folder.folders[index].name.ToLower().StartsWith(str3.ToLower(), StringComparison.InvariantCultureIgnoreCase))
                stringList1.Add(str1 + " " + str2 + folder.folders[index].name + "/");
            }
            for (int index = 0; index < folder.files.Count; ++index)
            {
              if (folder.files[index].name.ToLower().StartsWith(str3.ToLower()))
                stringList1.Add(str1 + " " + str2 + folder.files[index].name);
            }
          }
        }
        else
        {
          if (path == null || (path.Equals("") || path.Length < 1) && !str1.Equals("exe"))
            return;
          Folder currentFolder = Programs.getCurrentFolder(this.os);
          for (int index = 0; index < currentFolder.folders.Count; ++index)
          {
            if (currentFolder.folders[index].name.StartsWith(path, StringComparison.InvariantCultureIgnoreCase))
              stringList1.Add(str1 + " " + currentFolder.folders[index].name + "/");
          }
          for (int index = 0; index < currentFolder.files.Count; ++index)
          {
            if (currentFolder.files[index].name.StartsWith(path, StringComparison.InvariantCultureIgnoreCase))
              stringList1.Add(str1 + " " + currentFolder.files[index].name);
          }
          if (stringList1.Count == 0)
          {
            for (int index = 0; index < currentFolder.files.Count; ++index)
            {
              if (currentFolder.files[index].name.StartsWith(path, StringComparison.InvariantCultureIgnoreCase))
                stringList1.Add(str1 + " " + currentFolder.files[index].name);
            }
          }
        }
      }
      else
      {
        List<string> stringList2 = new List<string>();
        stringList2.AddRange((IEnumerable<string>) ProgramList.programs);
        stringList2.AddRange((IEnumerable<string>) ProgramList.getExeList(this.os));
        for (int index = 0; index < stringList2.Count; ++index)
        {
          if (stringList2[index].ToLower().StartsWith(this.currentLine.ToLower()))
            stringList1.Add(stringList2[index]);
        }
      }
      if (stringList1.Count == 1)
      {
        this.currentLine = stringList1[0];
        TextBox.moveCursorToEnd(this.currentLine);
      }
      else
      {
        if (stringList1.Count <= 1)
          return;
        this.os.write(this.prompt + this.currentLine);
        string str = stringList1[0];
        for (int index = 0; index < stringList1.Count; ++index)
        {
          this.os.write(stringList1[index]);
          for (int length2 = 0; length2 < str.Length; ++length2)
          {
            if (stringList1[index].Length <= length2 || (int) string.Concat((object) str[length2]).ToLowerInvariant()[0] != (int) string.Concat((object) stringList1[index][length2]).ToLowerInvariant()[0])
            {
              str = str.Substring(0, length2);
              break;
            }
          }
          this.currentLine = str;
          TextBox.moveCursorToEnd(this.currentLine);
        }
      }
    }

    public void writeLine(string text)
    {
      text = Utils.SuperSmartTwimForWidth(text, this.bounds.Width - 6, GuiData.tinyfont);
      string str1 = text;
      char[] chArray = new char[1]{ '\n' };
      foreach (string str2 in str1.Split(chArray))
        this.history.Add(str2);
    }

    public void write(string text)
    {
      if (this.history.Count <= 0 || (double) GuiData.tinyfont.MeasureString(this.history[this.history.Count - 1] + text).X > (double) (this.bounds.Width - 6))
      {
        this.writeLine(text);
      }
      else
      {
        List<string> history;
        int index;
        (history = this.history)[index = this.history.Count - 1] = history[index] + text;
      }
    }

    public void clearCurrentLine()
    {
      this.currentLine = "";
      TextBox.cursorPosition = 0;
      TextBox.textDrawOffsetPosition = 0;
    }

    public void reset()
    {
      this.history.Clear();
      this.clearCurrentLine();
    }

    public int commandsRun()
    {
      return this.runCommands.Count;
    }

    public string getLastRunCommand()
    {
      return this.lastRunCommand;
    }
  }
}
