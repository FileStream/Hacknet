// Decompiled with JetBrains decompiler
// Type: Hacknet.AcademicDatabaseDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Hacknet
{
  internal class AcademicDatabaseDaemon : Daemon
  {
    private AcademicDatabaseDaemon.ADDState state = AcademicDatabaseDaemon.ADDState.Welcome;
    private float searchStartTime = 0.0f;
    private bool needsDeletionConfirmation = false;
    private int editedIndex = 0;
    public const string ROOT_FOLDERNAME = "academic_data";
    public const string ENTRIES_FOLDERNAME = "entry_cache";
    public const string CONFIG_FILENAME = "config.sys";
    public const string INFO_FILENAME = "info.txt";
    private const float SEARCH_TIME = 3.6f;
    private const float MULTI_MATCH_SEARCH_TIME = 0.7f;
    public const string DEGREE_SPLIT_DELIM = "--------------------";
    private Folder root;
    private Folder entries;
    private Color themeColor;
    private Color backThemeColor;
    private Color darkThemeColor;
    private Texture2D loadingCircle;
    private string searchedName;
    private string foundFileName;
    private List<Degree> searchedDegrees;
    private List<string> searchResultsNames;
    private string infoText;
    private AcademicDatabaseDaemon.ADDEditField editedField;
    private List<Vector2> backBars;
    private List<Vector2> topBars;

    public AcademicDatabaseDaemon(Computer c, string serviceName, OS os)
      : base(c, serviceName, os)
    {
      this.themeColor = new Color(53, 96, 156);
      this.backThemeColor = new Color(27, 58, 102);
      this.darkThemeColor = new Color(12, 20, 40, 100);
      this.init();
    }

    private void init()
    {
      this.backBars = new List<Vector2>();
      this.topBars = new List<Vector2>();
      this.searchedDegrees = new List<Degree>();
      this.searchResultsNames = new List<string>();
      for (int index = 0; index < 5; ++index)
      {
        this.backBars.Add(new Vector2(0.5f, Utils.randm(1f)));
        this.topBars.Add(new Vector2(0.5f, Utils.randm(1f)));
      }
      this.loadingCircle = this.os.content.Load<Texture2D>("Sprites/Spinner");
    }

    public override void initFiles()
    {
      this.root = this.comp.files.root.searchForFolder("academic_data");
      if (this.root == null)
      {
        this.root = new Folder("academic_data");
        this.comp.files.root.folders.Add(this.root);
      }
      this.entries = new Folder("entry_cache");
      this.root.folders.Add(this.entries);
      string dataEntry = Utils.readEntireFile("Content/LocPost/AcademicDatabaseInfo.txt");
      this.root.files.Add(new FileEntry(dataEntry, "info.txt"));
      this.infoText = dataEntry;
      this.initFilesFromPeople(People.all);
    }

    public override void loadInit()
    {
      this.root = this.comp.files.root.searchForFolder("academic_data");
      this.entries = this.root.searchForFolder("entry_cache");
      FileEntry fileEntry = this.root.searchForFile("info.txt");
      this.infoText = fileEntry != null ? fileEntry.data : LocaleTerms.Loc("DESCRIPTION FILE NOT FOUND");
    }

    public void initFilesFromPeople(List<Person> people = null)
    {
      if (people == null)
        people = People.all;
      for (int index = 0; index < people.Count; ++index)
      {
        if (people[index].degrees.Count > 0)
          this.addFileForPerson(people[index]);
      }
    }

    private void addFileForPerson(Person p)
    {
      this.entries.files.Add(this.getFileForPerson(p));
    }

    private FileEntry getFileForPerson(Person p)
    {
      FileEntry fileEntry = new FileEntry();
      fileEntry.name = this.convertNameToFileNameStart(p.FullName);
      int num = 0;
      while (this.entries.searchForFile(fileEntry.name) != null)
      {
        if (num == 0)
          fileEntry.name = fileEntry.name.Substring(0, fileEntry.name.Length - 1) + (object) num;
        else
          fileEntry.name += "1";
        ++num;
      }
      string str = p.FullName + "\n--------------------\n";
      for (int index = 0; index < p.degrees.Count; ++index)
        str = str + p.degrees[index].name + "\n" + p.degrees[index].uni + "\n" + (object) p.degrees[index].GPA + "\n--------------------";
      fileEntry.data = str;
      return fileEntry;
    }

    public string convertNameToFileNameStart(string name)
    {
      return name.ToLower().Replace(" ", "_");
    }

    public FileEntry findFileForName(string name)
    {
      FileEntry fileEntry = (FileEntry) null;
      this.searchResultsNames.Clear();
      string fileNameStart = this.convertNameToFileNameStart(name);
      for (int index = 0; index < this.entries.files.Count; ++index)
      {
        if (this.entries.files[index].name.StartsWith(fileNameStart))
        {
          string data = this.entries.files[index].data;
          if (fileEntry == null)
            fileEntry = this.entries.files[index];
          this.searchResultsNames.Add(data.Substring(0, data.IndexOf('\n')));
        }
      }
      if (this.searchResultsNames.Count > 1 || fileEntry == null)
        return (FileEntry) null;
      this.foundFileName = fileEntry.name;
      return fileEntry;
    }

    private void setDegreesFromFileEntryData(string file)
    {
      string[] separator = new string[1]{ "--------------------" };
      string[] strArray1 = file.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      CultureInfo cultureInfo = new CultureInfo("en-au");
      this.searchedName = strArray1[0];
      this.searchedDegrees.Clear();
      for (int index = 1; index < strArray1.Length; ++index)
      {
        string[] strArray2 = strArray1[index].Split(Utils.newlineDelim, StringSplitOptions.RemoveEmptyEntries);
        if (strArray2.Length >= 3)
          this.searchedDegrees.Add(new Degree(strArray2[0], strArray2[1], (float) Convert.ToDouble(strArray2[2], (IFormatProvider) cultureInfo)));
      }
    }

    public bool doesDegreeExist(string owner_name, string degree_name, string uni_name, float gpaMin)
    {
      List<Degree> searchedDegrees = this.searchedDegrees;
      FileEntry fileForName = this.findFileForName(owner_name);
      if (fileForName == null)
        return false;
      this.setDegreesFromFileEntryData(fileForName.data);
      bool flag = false;
      for (int index = 0; index < this.searchedDegrees.Count; ++index)
      {
        if ((double) this.searchedDegrees[index].GPA >= (double) gpaMin && (degree_name == null || this.searchedDegrees[index].name.ToLower().Contains(degree_name.ToLower())) && (uni_name == null || this.searchedDegrees[index].uni.ToLower().Equals(uni_name.ToLower())))
        {
          flag = true;
          break;
        }
      }
      this.searchedDegrees = searchedDegrees;
      return flag;
    }

    public bool hasDegrees(string owner_name)
    {
      FileEntry fileForName = this.findFileForName(owner_name);
      if (fileForName == null)
        return false;
      List<Degree> searchedDegrees = this.searchedDegrees;
      this.setDegreesFromFileEntryData(fileForName.data);
      bool flag = true;
      if (this.searchedDegrees.Count <= 0)
        flag = false;
      this.searchedDegrees = searchedDegrees;
      return flag;
    }

    private void doPreEntryViewSearch()
    {
      FileEntry fileForName = this.findFileForName(this.searchedName);
      if (fileForName != null)
        this.setDegreesFromFileEntryData(fileForName.data);
      else
        this.state = this.searchResultsNames.Count <= 1 ? AcademicDatabaseDaemon.ADDState.EntryNotFound : AcademicDatabaseDaemon.ADDState.MultipleEntriesFound;
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      this.drawSideBar(bounds, sb);
      int num1 = (int) ((double) bounds.Width / 5.0);
      bounds.Width -= num1;
      bounds.X += num1;
      this.drawTitle(bounds, sb);
      bounds.Y += 30;
      bounds.Height -= 30;
      switch (this.state)
      {
        case AcademicDatabaseDaemon.ADDState.Welcome:
          bool flag = this.comp.adminIP == this.os.thisComputer.ip;
          Rectangle destinationRectangle = bounds;
          destinationRectangle.Y = bounds.Y + 60 - 20;
          destinationRectangle.Height = 22;
          sb.Draw(Utils.white, destinationRectangle, flag ? this.themeColor : this.darkThemeColor);
          string text1 = LocaleTerms.Loc("Valid Administrator Account Detected");
          if (!flag)
            text1 = LocaleTerms.Loc("Non-Admin Account Active");
          Vector2 vector2 = GuiData.smallfont.MeasureString(text1);
          Vector2 position1 = new Vector2((float) (destinationRectangle.X + destinationRectangle.Width / 2) - vector2.X / 2f, (float) destinationRectangle.Y);
          sb.DrawString(GuiData.smallfont, text1, position1, Color.Black);
          if (Button.doButton(456011, bounds.X + 30, bounds.Y + bounds.Height / 2 - 15, bounds.Width / 2, 40, LocaleTerms.Loc("About This Server"), new Color?(this.themeColor)))
            this.state = AcademicDatabaseDaemon.ADDState.InfoPanel;
          if (Button.doButton(456001, bounds.X + 30, bounds.Y + bounds.Height / 2 - 15 + 50, bounds.Width / 2, 40, LocaleTerms.Loc("Search Entries"), new Color?(this.themeColor)))
          {
            this.state = AcademicDatabaseDaemon.ADDState.Seach;
            this.os.execute("getString Name");
          }
          if (!Button.doButton(456005, bounds.X + 30, bounds.Y + bounds.Height / 2 - 15 + 100, bounds.Width / 2, 40, LocaleTerms.Loc("Exit"), new Color?(this.themeColor)))
            break;
          this.os.display.command = "connect";
          break;
        case AcademicDatabaseDaemon.ADDState.Seach:
          this.drawSearchState(bounds, sb);
          break;
        case AcademicDatabaseDaemon.ADDState.MultiMatchSearch:
        case AcademicDatabaseDaemon.ADDState.PendingResult:
          if (Button.doButton(456010, bounds.X + 2, bounds.Y + 2, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.darkThemeColor)))
            this.state = AcademicDatabaseDaemon.ADDState.Welcome;
          Vector2 position2 = new Vector2((float) (bounds.X + bounds.Width / 2), (float) (bounds.Y + bounds.Height / 2));
          Vector2 origin = new Vector2((float) (this.loadingCircle.Width / 2), (float) (this.loadingCircle.Height / 2));
          sb.Draw(this.loadingCircle, position2, new Rectangle?(), Color.White, (float) ((double) this.os.timer % Math.PI * 3.0), origin, Vector2.One, SpriteEffects.None, 0.5f);
          float num2 = this.os.timer - this.searchStartTime;
          if ((this.state != AcademicDatabaseDaemon.ADDState.PendingResult || (double) num2 <= 3.59999990463257) && (this.state != AcademicDatabaseDaemon.ADDState.MultiMatchSearch || (double) num2 <= 0.699999988079071))
            break;
          this.state = AcademicDatabaseDaemon.ADDState.Entry;
          this.needsDeletionConfirmation = true;
          this.doPreEntryViewSearch();
          break;
        case AcademicDatabaseDaemon.ADDState.Entry:
        case AcademicDatabaseDaemon.ADDState.EditPerson:
          this.drawEntryState(bounds, sb);
          break;
        case AcademicDatabaseDaemon.ADDState.EntryNotFound:
          if (Button.doButton(456010, bounds.X + 2, bounds.Y + 20, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.darkThemeColor)))
            this.state = AcademicDatabaseDaemon.ADDState.Welcome;
          if (Button.doButton(456015, bounds.X + 2, bounds.Y + 55, 160, 30, LocaleTerms.Loc("Search Again"), new Color?(this.darkThemeColor)))
          {
            this.state = AcademicDatabaseDaemon.ADDState.Seach;
            this.os.execute("getString Name");
          }
          TextItem.doFontLabel(new Vector2((float) (bounds.X + 2), (float) (bounds.Y + 90)), LocaleTerms.Loc("No Entries Found"), GuiData.font, new Color?(), float.MaxValue, float.MaxValue, false);
          break;
        case AcademicDatabaseDaemon.ADDState.MultipleEntriesFound:
          this.drawMultipleEntriesState(bounds, sb);
          break;
        case AcademicDatabaseDaemon.ADDState.InfoPanel:
          if (Button.doButton(456010, bounds.X + 2, bounds.Y + 30, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.darkThemeColor)))
            this.state = AcademicDatabaseDaemon.ADDState.Welcome;
          Vector2 pos = new Vector2((float) (bounds.X + 20), (float) (bounds.Y + 70));
          TextItem.doFontLabel(pos, LocaleTerms.Loc("Information"), GuiData.font, new Color?(), float.MaxValue, float.MaxValue, false);
          pos.Y += 40f;
          FileEntry fileEntry = this.root.searchForFile("info.txt");
          string text2 = "ERROR: Unhandled System.IO.FileNotFoundException\nFile \"info.txt\" was not found";
          if (fileEntry != null)
            text2 = DisplayModule.cleanSplitForWidth(fileEntry.data, bounds.Width - 80);
          TextItem.DrawShadow = false;
          TextItem.doFontLabel(pos, text2, GuiData.smallfont, new Color?(), (float) (bounds.Width - 40), float.MaxValue, false);
          break;
        case AcademicDatabaseDaemon.ADDState.EditEntry:
          this.drawEditDegreeState(bounds, sb);
          break;
      }
    }

    private void drawSearchState(Rectangle bounds, SpriteBatch sb)
    {
      if (Button.doButton(456010, bounds.X + 2, bounds.Y + 2, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.darkThemeColor)))
        this.state = AcademicDatabaseDaemon.ADDState.Welcome;
      string str = "";
      int x = bounds.X + 6;
      int num1 = bounds.Y + 100;
      Vector2 vector2_1 = TextItem.doMeasuredSmallLabel(new Vector2((float) x, (float) num1), LocaleTerms.Loc("Please enter the name you with to search for:"), new Color?());
      int y1 = num1 + ((int) vector2_1.Y + 10);
      string[] strArray = this.os.getStringCache.Split(new string[1]{ "#$#$#$$#$&$#$#$#$#" }, StringSplitOptions.None);
      if (strArray.Length > 1)
      {
        str = strArray[1];
        if (str.Equals(""))
          str = this.os.terminal.currentLine;
      }
      Rectangle destinationRectangle = new Rectangle(x, y1, bounds.Width - 12, 80);
      sb.Draw(Utils.white, destinationRectangle, this.os.darkBackgroundColor);
      int num2 = y1 + 28;
      Vector2 vector2_2 = TextItem.doMeasuredSmallLabel(new Vector2((float) x, (float) num2), LocaleTerms.Loc("Name") + ": " + str, new Color?());
      destinationRectangle.X = x + (int) vector2_2.X + 2;
      destinationRectangle.Y = num2;
      destinationRectangle.Width = 7;
      destinationRectangle.Height = 20;
      if ((double) this.os.timer % 1.0 < 0.300000011920929)
        sb.Draw(Utils.white, destinationRectangle, this.os.outlineColor);
      int y2 = num2 + 122;
      if (strArray.Length <= 2 && !Button.doButton(30, x, y2, 300, 22, LocaleTerms.Loc("Search"), new Color?(this.os.highlightColor)))
        return;
      if (strArray.Length <= 2)
        this.os.terminal.executeLine();
      if (str.Length > 0)
      {
        this.state = AcademicDatabaseDaemon.ADDState.PendingResult;
        this.searchedName = str;
        this.searchStartTime = this.os.timer;
        this.comp.log("ACADEMIC_DATABASE::RecordSearch_:_" + str);
      }
      else
        this.state = AcademicDatabaseDaemon.ADDState.EntryNotFound;
    }

    private void drawMultipleEntriesState(Rectangle bounds, SpriteBatch sb)
    {
      float num1 = 22f;
      float num2 = 2f;
      Vector2 pos = new Vector2((float) bounds.X + 20f, (float) bounds.Y + 10f);
      int num3 = (int) Math.Min((float) this.searchResultsNames.Count, (float) (((double) bounds.Height - 40.0 - 40.0 - 80.0) / ((double) num1 + (double) num2)));
      TextItem.doFontLabel(pos, LocaleTerms.Loc("Multiple Matches"), GuiData.font, new Color?(), float.MaxValue, float.MaxValue, false);
      pos.Y += 30f;
      if (num3 > this.searchResultsNames.Count)
        TextItem.doFontLabel(new Vector2(pos.X, pos.Y - 18f), LocaleTerms.Loc("Some Results Omitted"), GuiData.tinyfont, new Color?(), float.MaxValue, float.MaxValue, false);
      sb.Draw(Utils.white, new Rectangle((int) pos.X, (int) pos.Y, (int) ((double) bounds.Width - (double) pos.X - 5.0), 2), Color.White);
      pos.Y += 12f;
      for (int index = 0; index < num3; ++index)
      {
        if (Button.doButton(1237000 + index, (int) pos.X, (int) pos.Y, (int) ((double) bounds.Width * 0.666), (int) num1, this.searchResultsNames[index], new Color?(this.darkThemeColor)))
        {
          this.searchedName = this.searchResultsNames[index];
          this.state = AcademicDatabaseDaemon.ADDState.MultiMatchSearch;
          this.searchStartTime = this.os.timer;
        }
        pos.Y += num1 + num2;
      }
      pos.Y += 5f;
      sb.Draw(Utils.white, new Rectangle((int) pos.X, (int) pos.Y, (int) ((double) bounds.Width - (double) pos.X - 5.0), 2), Color.White);
      pos.Y += 10f;
      if (Button.doButton(12346080, (int) pos.X, (int) pos.Y, 160, 25, LocaleTerms.Loc("Refine Search"), new Color?(this.themeColor)))
      {
        this.state = AcademicDatabaseDaemon.ADDState.Seach;
        this.os.execute("getString Name");
      }
      if (!Button.doButton(12346085, (int) pos.X + 170, (int) pos.Y, 160, 25, LocaleTerms.Loc("Go Back"), new Color?(this.darkThemeColor)))
        return;
      this.state = AcademicDatabaseDaemon.ADDState.Welcome;
    }

    private void drawEntryState(Rectangle bounds, SpriteBatch sb)
    {
      if (this.state == AcademicDatabaseDaemon.ADDState.Entry && this.os.hasConnectionPermission(true))
        this.state = AcademicDatabaseDaemon.ADDState.EditPerson;
      if (Button.doButton(456010, bounds.X + 2, bounds.Y + 2, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.darkThemeColor)))
        this.state = AcademicDatabaseDaemon.ADDState.Welcome;
      float x = (float) bounds.X + 20f;
      float y1 = (float) bounds.Y + 50f;
      TextItem.doFontLabel(new Vector2(x, y1), this.searchedName, GuiData.font, new Color?(), (float) bounds.Width - (x - (float) bounds.X), 60f, false);
      float y2 = y1 + 30f;
      if (this.searchedDegrees.Count == 0)
        TextItem.doFontLabel(new Vector2(x, y2), " -" + LocaleTerms.Loc("No Degrees Found"), GuiData.smallfont, new Color?(), float.MaxValue, float.MaxValue, false);
      for (int index = 0; index < this.searchedDegrees.Count; ++index)
      {
        string text = LocaleTerms.Loc("Degree") + " :" + this.searchedDegrees[index].name + "\n" + LocaleTerms.Loc("Uni") + "      :" + this.searchedDegrees[index].uni + "\nGPA      :" + (object) this.searchedDegrees[index].GPA;
        TextItem.doFontLabel(new Vector2(x, y2), text, GuiData.smallfont, new Color?(), (float) bounds.Width - ((float) bounds.X - x), 50f, false);
        y2 += 60f;
        if (this.state == AcademicDatabaseDaemon.ADDState.EditPerson)
        {
          float num = y2 - 10f;
          if (Button.doButton(457900 + index, (int) x, (int) num, 100, 20, LocaleTerms.Loc("Edit"), new Color?()))
          {
            this.state = AcademicDatabaseDaemon.ADDState.EditEntry;
            this.editedField = AcademicDatabaseDaemon.ADDEditField.None;
            this.editedIndex = index;
          }
          if (Button.doButton(456900 + index, (int) x + 105, (int) num, 100, 20, this.needsDeletionConfirmation ? LocaleTerms.Loc("Delete") : LocaleTerms.Loc("Confirm?"), new Color?(this.needsDeletionConfirmation ? Color.Gray : Color.Red)))
          {
            if (this.needsDeletionConfirmation)
            {
              this.needsDeletionConfirmation = false;
            }
            else
            {
              this.comp.log("ACADEMIC_DATABASE::RecordDeletion_:_#" + (object) index + "_: " + this.searchedName.Replace(" ", "_"));
              this.searchedDegrees.RemoveAt(index);
              this.saveChangesToEntry();
              --index;
              this.needsDeletionConfirmation = true;
            }
          }
          y2 = num + 35f;
        }
      }
      float num1 = y2 + 10f;
      if (this.state != AcademicDatabaseDaemon.ADDState.EditPerson || !Button.doButton(458009, (int) x, (int) num1, 200, 30, LocaleTerms.Loc("Add Degree"), new Color?(this.themeColor)))
        return;
      Degree degree = new Degree("UNKNOWN", "UNKNOWN", 0.0f);
      this.searchedDegrees.Add(degree);
      this.editedIndex = this.searchedDegrees.IndexOf(degree);
      this.state = AcademicDatabaseDaemon.ADDState.EditEntry;
      this.comp.log("ACADEMIC_DATABASE::RecordAdd_:_#" + (object) this.editedIndex + "_: " + this.searchedName.Replace(" ", "_"));
    }

    private void drawEditDegreeState(Rectangle bounds, SpriteBatch sb)
    {
      if (Button.doButton(456010, bounds.X + 2, bounds.Y + 2, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.darkThemeColor)))
        this.state = AcademicDatabaseDaemon.ADDState.Entry;
      Vector2 pos = new Vector2((float) bounds.X + 10f, (float) bounds.Y + 60f);
      bool flag = this.editedField == AcademicDatabaseDaemon.ADDEditField.None;
      TextItem.doSmallLabel(pos, LocaleTerms.Loc("University") + ":", new Color?());
      pos.X += 110f;
      TextItem.doSmallLabel(pos, this.searchedDegrees[this.editedIndex].uni, new Color?(this.editedField == AcademicDatabaseDaemon.ADDEditField.Uni ? this.themeColor : Color.White));
      Rectangle destinationRectangle;
      if (this.editedField == AcademicDatabaseDaemon.ADDEditField.Uni)
      {
        Vector2 vector2 = GuiData.smallfont.MeasureString(this.searchedDegrees[this.editedIndex].uni);
        destinationRectangle = new Rectangle((int) ((double) pos.X + (double) vector2.X + 4.0), (int) pos.Y, 10, 16);
        if ((double) this.os.timer % 0.600000023841858 < 0.300000011920929)
          sb.Draw(Utils.white, destinationRectangle, Utils.AddativeWhite * 0.8f);
      }
      pos.X -= 110f;
      pos.Y += 20f;
      if (flag && Button.doButton(46700, (int) pos.X, (int) pos.Y, 110, 20, LocaleTerms.Loc("Edit"), new Color?(this.darkThemeColor)))
      {
        this.editedField = AcademicDatabaseDaemon.ADDEditField.Uni;
        this.os.execute("getString University");
      }
      pos.Y += 30f;
      TextItem.doSmallLabel(pos, LocaleTerms.Loc("Degree") + ":", new Color?());
      pos.X += 110f;
      TextItem.doSmallLabel(pos, this.searchedDegrees[this.editedIndex].name, new Color?(this.editedField == AcademicDatabaseDaemon.ADDEditField.Degree ? this.themeColor : Color.White));
      if (this.editedField == AcademicDatabaseDaemon.ADDEditField.Degree)
      {
        Vector2 vector2 = GuiData.smallfont.MeasureString(this.searchedDegrees[this.editedIndex].name);
        destinationRectangle = new Rectangle((int) ((double) pos.X + (double) vector2.X + 4.0), (int) pos.Y, 10, 16);
        if ((double) this.os.timer % 0.600000023841858 < 0.300000011920929)
          sb.Draw(Utils.white, destinationRectangle, Utils.AddativeWhite * 0.8f);
      }
      pos.X -= 110f;
      pos.Y += 20f;
      if (flag && Button.doButton(46705, (int) pos.X, (int) pos.Y, 110, 20, LocaleTerms.Loc("Edit"), new Color?(this.darkThemeColor)))
      {
        this.editedField = AcademicDatabaseDaemon.ADDEditField.Degree;
        this.os.execute("getString Degree");
      }
      pos.Y += 30f;
      TextItem.doSmallLabel(pos, "GPA:", new Color?());
      pos.X += 110f;
      TextItem.doSmallLabel(pos, string.Concat((object) this.searchedDegrees[this.editedIndex].GPA), new Color?(this.editedField == AcademicDatabaseDaemon.ADDEditField.GPA ? this.themeColor : Color.White));
      if (this.editedField == AcademicDatabaseDaemon.ADDEditField.GPA)
      {
        Vector2 vector2 = GuiData.smallfont.MeasureString(string.Concat((object) this.searchedDegrees[this.editedIndex].GPA));
        destinationRectangle = new Rectangle((int) ((double) pos.X + (double) vector2.X + 4.0), (int) pos.Y, 10, 16);
        if ((double) this.os.timer % 0.600000023841858 < 0.300000011920929)
          sb.Draw(Utils.white, destinationRectangle, Utils.AddativeWhite * 0.8f);
      }
      pos.X -= 110f;
      pos.Y += 20f;
      if (flag && Button.doButton(46710, (int) pos.X, (int) pos.Y, 110, 20, LocaleTerms.Loc("Edit"), new Color?(this.darkThemeColor)))
      {
        this.editedField = AcademicDatabaseDaemon.ADDEditField.GPA;
        this.os.execute("getString GPA");
      }
      pos.Y += 30f;
      if (this.editedField != AcademicDatabaseDaemon.ADDEditField.None)
      {
        if (!this.doEditField())
          return;
        this.editedField = AcademicDatabaseDaemon.ADDEditField.None;
        this.os.getStringCache = "";
        this.saveChangesToEntry();
      }
      else if (Button.doButton(486012, bounds.X + 2, bounds.Y + bounds.Height - 40, 230, 30, LocaleTerms.Loc("Save And Return"), new Color?(this.backThemeColor)))
      {
        this.state = AcademicDatabaseDaemon.ADDState.Entry;
        this.comp.log("ACADEMIC_DATABASE::RecordEdit_:_#" + (object) this.editedIndex + "_: " + this.searchedName.Replace(" ", "_"));
      }
    }

    private bool doEditField()
    {
      string str = "";
      string[] strArray = this.os.getStringCache.Split(new string[1]{ "#$#$#$$#$&$#$#$#$#" }, StringSplitOptions.None);
      if (strArray.Length > 1)
      {
        str = strArray[1];
        if (str.Equals(""))
          str = this.os.terminal.currentLine;
        this.setEditedFieldValue(str);
      }
      if (strArray.Length > 2)
      {
        if (strArray.Length <= 2)
          this.os.terminal.executeLine();
        if (str.Length > 0)
        {
          this.setEditedFieldValue(str);
          return true;
        }
      }
      return false;
    }

    private void setEditedFieldValue(string value)
    {
      CultureInfo cultureInfo = new CultureInfo("en-au");
      switch (this.editedField)
      {
        case AcademicDatabaseDaemon.ADDEditField.Uni:
          this.searchedDegrees[this.editedIndex] = new Degree(this.searchedDegrees[this.editedIndex].name, value, this.searchedDegrees[this.editedIndex].GPA);
          break;
        case AcademicDatabaseDaemon.ADDEditField.Degree:
          this.searchedDegrees[this.editedIndex] = new Degree(value, this.searchedDegrees[this.editedIndex].uni, this.searchedDegrees[this.editedIndex].GPA);
          break;
        case AcademicDatabaseDaemon.ADDEditField.GPA:
          float degreeGPA = 2f;
          try
          {
            if (value.Length > 0)
              degreeGPA = (float) Convert.ToDouble(value, (IFormatProvider) cultureInfo);
          }
          catch
          {
          }
          this.searchedDegrees[this.editedIndex] = new Degree(this.searchedDegrees[this.editedIndex].name, this.searchedDegrees[this.editedIndex].uni, degreeGPA);
          break;
      }
    }

    private void drawTitle(Rectangle bounds, SpriteBatch sb)
    {
      TextItem.doFontLabel(new Vector2((float) bounds.X, (float) bounds.Y), LocaleTerms.Loc("International Academic Database"), GuiData.font, new Color?(), (float) (bounds.Width - 6), float.MaxValue, false);
    }

    private void drawSideBar(Rectangle bounds, SpriteBatch sb)
    {
      this.updateSideBar();
      Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 1, bounds.Width / 8, bounds.Height - 2);
      int num = bounds.Width / 15;
      Rectangle destinationRectangle = rectangle;
      destinationRectangle.Width = (int) ((double) rectangle.Width * 1.5);
      sb.Draw(Utils.white, destinationRectangle, this.darkThemeColor);
      for (int index = 0; index < this.backBars.Count; ++index)
      {
        destinationRectangle.X = (int) ((double) rectangle.X + (double) this.backBars[index].X * (double) rectangle.Width);
        destinationRectangle.Width = (int) ((double) num * (double) this.backBars[index].Y);
        sb.Draw(Utils.white, destinationRectangle, this.backThemeColor);
        destinationRectangle.X = (int) ((double) rectangle.X + (double) this.topBars[index].X * (double) rectangle.Width);
        destinationRectangle.Width = (int) ((double) num * (double) this.topBars[index].Y * 0.5);
        sb.Draw(Utils.white, destinationRectangle, this.themeColor);
      }
      destinationRectangle.X = rectangle.X;
      destinationRectangle.Width = (int) ((double) rectangle.Width * 1.5);
      sb.Draw(Utils.gradient, destinationRectangle, Color.Black);
    }

    private void updateSideBar()
    {
      float num = this.os.timer * 0.4f;
      for (int index = 0; index < this.backBars.Count; ++index)
      {
        this.backBars[index] = new Vector2((float) (0.5 + Math.Sin((double) num * (double) index) * 0.5), (float) Math.Abs(Math.Sin((double) num * (double) index)));
        this.topBars[index] = new Vector2((float) (0.5 + Math.Sin(-(double) num * (double) (this.backBars.Count - index)) * 0.5), (float) Math.Abs(Math.Sin(-(double) num / 2.0 * (double) index)));
      }
    }

    private void saveChangesToEntry()
    {
      string[] strArray = this.searchedName.Split(Utils.spaceDelim);
      this.entries.searchForFile(this.foundFileName).data = this.getFileForPerson(new Person(strArray[0], strArray[1], true, false, (string) null)
      {
        degrees = this.searchedDegrees
      }).data;
    }

    public override void navigatedTo()
    {
      this.state = AcademicDatabaseDaemon.ADDState.Welcome;
    }

    public override void userAdded(string name, string pass, byte type)
    {
    }

    public override string getSaveString()
    {
      return "<AcademicDatabse name=\"" + this.name + "\"/>";
    }

    private enum ADDEditField
    {
      None,
      Uni,
      Degree,
      GPA,
    }

    private enum ADDState
    {
      Welcome,
      Seach,
      MultiMatchSearch,
      Entry,
      PendingResult,
      EntryNotFound,
      MultipleEntriesFound,
      InfoPanel,
      EditPerson,
      EditEntry,
    }
  }
}
