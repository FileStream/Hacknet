// Decompiled with JetBrains decompiler
// Type: Hacknet.NotesExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hacknet
{
  internal class NotesExe : ExeModule
  {
    private static int noteBaseCost = 8;
    private static int noteCostPerLine = 14;
    public List<string> notes = new List<string>();
    private int targetRamUse = 100;
    private int baseRamCost = 100;
    private bool gettingNewNote = false;
    public float MemoryWarningFlashTime = 0.0f;
    private const float RAM_CHANGE_PS = 350f;
    private const int BASE_PANEL_HEIGHT = 22;
    private const float NO_MEMORY_WARNING_TIME = 4f;
    internal const string NotesSaveFilename = "Notes.txt";
    public const string NotesReopenOnLoadFile = "Notes_Reopener.bat";
    internal const string NotesSaveFileDelimiter = "\n\n----------\n\n";
    private static Texture2D crossTexture;
    private static Texture2D circleTexture;

    public NotesExe(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.IdentifierName = "Notes";
      this.ramCost = Module.PANEL_HEIGHT + 22;
      this.baseRamCost = this.ramCost;
      this.targetRamUse = this.ramCost;
      this.targetIP = this.os.thisComputer.ip;
      if (NotesExe.crossTexture == null)
        NotesExe.crossTexture = this.os.content.Load<Texture2D>("cross");
      if (NotesExe.circleTexture != null)
        return;
      NotesExe.circleTexture = this.os.content.Load<Texture2D>("CircleOutline");
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.LoadNotesFromDrive();
      Folder folder = this.os.thisComputer.files.root.searchForFolder("sys");
      if (folder.searchForFile("Notes_Reopener.bat") != null)
        return;
      folder.files.Add(new FileEntry("true", "Notes_Reopener.bat"));
    }

    public override void Killed()
    {
      base.Killed();
      this.RemoveReopnener();
    }

    public void DisplayOutOfMemoryWarning()
    {
      this.MemoryWarningFlashTime = 4f;
    }

    private void RemoveReopnener()
    {
      Folder folder = this.os.thisComputer.files.root.searchForFolder("sys");
      for (int index = 0; index < folder.files.Count; ++index)
      {
        if (folder.files[index].name == "Notes_Reopener.bat")
        {
          folder.files.RemoveAt(index);
          break;
        }
      }
    }

    public static bool NoteExists(string note, OS os)
    {
      for (int index = 0; index < os.exes.Count; ++index)
      {
        NotesExe ex = os.exes[index] as NotesExe;
        if (ex != null)
          return ex.HasNote(note);
      }
      return false;
    }

    public static void AddNoteToOS(string note, OS os, bool isRecursiveSelfAdd = false)
    {
      for (int index = 0; index < os.exes.Count; ++index)
      {
        NotesExe ex = os.exes[index] as NotesExe;
        if (ex != null)
        {
          ex.AddNote(note);
          return;
        }
      }
      if (!isRecursiveSelfAdd)
        os.runCommand("notes");
      Action action = (Action) (() => NotesExe.AddNoteToOS(note, os, true));
      os.delayer.Post(ActionDelayer.NextTick(), action);
    }

    public override void Update(float t)
    {
      base.Update(t);
      if (this.targetRamUse != this.ramCost)
      {
        if (this.targetRamUse < this.ramCost)
        {
          this.ramCost -= (int) ((double) t * 350.0);
          if (this.ramCost < this.targetRamUse)
            this.ramCost = this.targetRamUse;
        }
        else
        {
          int num = (int) ((double) t * 350.0);
          if (this.os.ramAvaliable >= num)
          {
            this.ramCost += num;
            if (this.ramCost > this.targetRamUse)
              this.ramCost = this.targetRamUse;
          }
        }
      }
      if (!this.gettingNewNote)
        return;
      string data = (string) null;
      if (Programs.parseStringFromGetStringCommand(this.os, out data))
      {
        this.gettingNewNote = false;
        this.AddNote(data);
      }
    }

    public void AddNote(string note)
    {
      string str = Utils.SuperSmartTwimForWidth(note, this.os.ram.bounds.Width - 8, GuiData.UITinyfont);
      for (int index = 0; index < this.notes.Count; ++index)
      {
        if (this.notes[index] == str)
          return;
      }
      this.notes.Add(str);
      this.recalcualteRamCost();
      this.SaveNotesToDrive();
    }

    public bool HasNote(string note)
    {
      string str = Utils.SuperSmartTwimForWidth(note, this.os.ram.bounds.Width - 8, GuiData.UITinyfont);
      for (int index = 0; index < this.notes.Count; ++index)
      {
        if (this.notes[index] == str)
          return true;
      }
      return false;
    }

    private void LoadNotesFromDrive()
    {
      FileEntry fileEntry = this.os.thisComputer.files.root.searchForFolder("home").searchForFile("Notes.txt");
      if (fileEntry == null)
        return;
      string[] strArray = fileEntry.data.Split(new string[1]{ "\n\n----------\n\n" }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length >= 0)
      {
        this.notes.AddRange((IEnumerable<string>) strArray);
        this.recalcualteRamCost();
      }
    }

    private void SaveNotesToDrive()
    {
      Folder folder = this.os.thisComputer.files.root.searchForFolder("home");
      FileEntry fileEntry = folder.searchForFile("Notes.txt");
      if (fileEntry == null)
      {
        fileEntry = new FileEntry("", "Notes.txt");
        folder.files.Add(fileEntry);
      }
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < this.notes.Count; ++index)
      {
        if (index != 0)
          stringBuilder.Append("\n\n----------\n\n");
        stringBuilder.Append(this.notes[index]);
      }
      fileEntry.data = stringBuilder.ToString();
    }

    private void recalcualteRamCost()
    {
      this.targetRamUse = this.baseRamCost;
      for (int index = 0; index < this.notes.Count; ++index)
        this.targetRamUse += NotesExe.noteBaseCost + this.notes[index].Split(Utils.newlineDelim).Length * NotesExe.noteCostPerLine;
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      if ((double) this.MemoryWarningFlashTime > 0.0)
        this.MemoryWarningFlashTime -= (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
      if (this.ramCost >= Module.PANEL_HEIGHT)
      {
        this.drawOutline();
        this.drawTarget("");
      }
      else
        this.drawFrame();
      this.DrawNotes(new Rectangle(this.bounds.X + 2, this.bounds.Y + 2 + Module.PANEL_HEIGHT, this.bounds.Width - 4, this.ramCost - Module.PANEL_HEIGHT - 22));
      if (this.ramCost < Module.PANEL_HEIGHT + 22)
        return;
      this.DrawBasePanel(new Rectangle(this.bounds.X + 2, this.bounds.Y + this.bounds.Height - 22, this.bounds.Width - 4, 22));
    }

    private void DrawBasePanel(Rectangle dest)
    {
      int width1 = dest.Width - 8;
      int width2 = (int) ((double) width1 * 0.4);
      int width3 = (int) ((double) width1 * 0.6);
      if (!this.gettingNewNote && Button.doButton(631012 + this.os.exes.IndexOf((ExeModule) this), dest.X + dest.Width - width2, dest.Y + 4, width2, dest.Height - 6, LocaleTerms.Loc("Close"), new Color?(this.os.lockedColor * this.fade)))
      {
        if (this.gettingNewNote)
        {
          this.os.terminal.executeLine();
          this.gettingNewNote = false;
        }
        this.Completed();
        this.RemoveReopnener();
        this.isExiting = true;
      }
      if (!this.gettingNewNote && Button.doButton(611014 + this.os.exes.IndexOf((ExeModule) this), dest.X, dest.Y + 4, width3, dest.Height - 6, LocaleTerms.Loc("Add Note"), new Color?(this.os.highlightColor * this.fade)))
      {
        this.os.runCommand("getString Note");
        this.os.getStringCache = "";
        this.gettingNewNote = true;
      }
      else
      {
        if (!this.gettingNewNote)
          return;
        Rectangle dest1 = new Rectangle(dest.X, dest.Y + 4, width1, dest.Height - 6);
        PatternDrawer.draw(dest1, 1f, Color.Transparent, this.os.highlightColor * 0.5f, this.spriteBatch, PatternDrawer.thinStripe);
        TextItem.doFontLabelToSize(dest1, LocaleTerms.Loc("Type In Terminal..."), GuiData.smallfont, Color.White, false, false);
      }
    }

    private void DrawNotes(Rectangle dest)
    {
      int y = dest.Y;
      for (int index1 = 0; index1 < this.notes.Count; ++index1)
      {
        string[] strArray = this.notes[index1].Split(Utils.newlineDelim);
        int num1 = NotesExe.noteBaseCost + strArray.Length * NotesExe.noteCostPerLine;
        if (y - dest.Y + num1 > dest.Height + 1)
          break;
        Rectangle destinationRectangle = new Rectangle(dest.X, y + NotesExe.noteBaseCost / 2 - 2, dest.Width, num1 - NotesExe.noteBaseCost / 2);
        this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.highlightColor * 0.2f);
        int num2 = y + NotesExe.noteBaseCost / 2;
        for (int index2 = 0; index2 < strArray.Length; ++index2)
        {
          this.spriteBatch.DrawString(GuiData.UITinyfont, strArray[index2], new Vector2((float) (destinationRectangle.X + 2), (float) num2), Color.White);
          num2 += NotesExe.noteCostPerLine;
        }
        int num3 = 13;
        if (Button.doButton(539261 + index1 * 100 + this.os.exes.IndexOf((ExeModule) this) * 2000, destinationRectangle.X + destinationRectangle.Width - num3 - 1, y + NotesExe.noteBaseCost / 2 + 1, num3, num3, "", new Color?(Color.White * 0.5f), NotesExe.crossTexture))
        {
          this.notes.RemoveAt(index1);
          this.recalcualteRamCost();
          this.SaveNotesToDrive();
          --index1;
        }
        if ((double) this.MemoryWarningFlashTime > 0.0)
        {
          float num4 = Math.Min(1f, this.MemoryWarningFlashTime);
          Color color = Color.Lerp(this.os.lockedColor, this.os.brightLockedColor, Utils.rand(0.4f)) * num4;
          if (4.0 - (double) this.MemoryWarningFlashTime < 0.300000011920929)
          {
            float num5 = (float) (1.0 - (4.0 - (double) this.MemoryWarningFlashTime) / 0.300000011920929);
            color = Color.Lerp(color, Utils.AddativeWhite, num5 * 0.7f);
          }
          this.spriteBatch.Draw(Utils.white, destinationRectangle, color);
          int height = 60;
          int num6 = (destinationRectangle.Height - height) / 2;
          Rectangle rectangle1 = new Rectangle(destinationRectangle.X, destinationRectangle.Y + num6, destinationRectangle.Width, height);
          this.spriteBatch.Draw(Utils.white, rectangle1, Color.Black * num4 * 0.8f);
          int num7 = height - 10;
          rectangle1.Y += 5;
          rectangle1.X += 10;
          rectangle1.Width -= 20;
          rectangle1.Height = num7 / 2;
          TextItem.doFontLabelToSize(rectangle1, Utils.FlipRandomChars(LocaleTerms.Loc("Insufficient Memory"), 0.00700000021606684), GuiData.font, Color.White * num4, false, false);
          rectangle1.Y += num7 / 2;
          TextItem.doFontLabelToSize(rectangle1, Utils.FlipRandomChars(LocaleTerms.Loc("Close Notes to Free Space"), 0.00700000021606684), GuiData.font, Color.White * num4, false, false);
          Rectangle rectangle2 = new Rectangle(destinationRectangle.X + destinationRectangle.Width - num3 - 1, y + NotesExe.noteBaseCost / 2 + 1, num3, num3);
          this.spriteBatch.Draw(NotesExe.crossTexture, rectangle2, Color.Red * num4);
          double num8 = (1.0 + Math.Sin((double) this.os.timer)) / 2.0;
          rectangle2 = Utils.InsetRectangle(rectangle2, (int) (num8 * -20.0));
          this.spriteBatch.Draw(NotesExe.crossTexture, rectangle2, Utils.AddativeWhite * num4 * (float) (1.0 - num8));
        }
        y += num1;
      }
    }
  }
}
