// Decompiled with JetBrains decompiler
// Type: Hacknet.MedicalDatabaseDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Hacknet
{
  internal class MedicalDatabaseDaemon : Daemon
  {
    private MedicalDatabaseDaemon.MedicalDatabaseState state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;
    private float totalTimeThisState = 1f;
    private float elapsedTimeThisState = 0.0f;
    private string searchName = "";
    private string emailRecipientAddress = "";
    private FileEntry currentFile = (FileEntry) null;
    private MedicalDatabaseDaemon.FileMedicalRecord currentRecord = (MedicalDatabaseDaemon.FileMedicalRecord) null;
    private string errorMessage = LocaleTerms.Loc("UNKNOWN ERROR");
    private Color theme_deep = new Color(8, 78, 90);
    private Color theme_strong = new Color(64, 157, 174);
    private Color theme_light = new Color(165, 237, 249);
    private Color theme_back = new Color(20, 20, 20);
    private MedicalDatabaseDaemon.GridSpot[,] themeGrid = new MedicalDatabaseDaemon.GridSpot[20, 100];
    private const string FOLDER_NAME = "Medical";
    private const float SEARCH_TIME = 1.6f;
    private Folder recordsFolder;
    private ScrollableSectionedPanel displayPanel;
    private Texture2D logo;

    public MedicalDatabaseDaemon(Computer c, OS os)
      : base(c, LocaleTerms.Loc("Universal Medical Database"), os)
    {
      this.logo = os.content.Load<Texture2D>("Sprites/MedicalLogo");
    }

    public override void initFiles()
    {
      base.initFiles();
      this.recordsFolder = new Folder("Medical");
      this.comp.files.root.folders.Add(this.recordsFolder);
      for (int index = 0; index < People.all.Count; ++index)
      {
        MedicalDatabaseDaemon.FileMedicalRecord fileMedicalRecord = new MedicalDatabaseDaemon.FileMedicalRecord(People.all[index]);
        this.recordsFolder.files.Add(new FileEntry(fileMedicalRecord.ToString(), fileMedicalRecord.GetFileName()));
      }
      string dataEntry = Utils.readEntireFile("Content/Post/MedicalDatabaseInfo.txt");
      Folder folder = this.comp.files.root.searchForFolder("home");
      if (folder == null)
        return;
      folder.files.Add(new FileEntry(dataEntry, "MedicalDatabaseInfo.txt"));
    }

    public override void loadInit()
    {
      base.loadInit();
      this.recordsFolder = this.comp.files.root.searchForFolder("Medical");
    }

    public override string getSaveString()
    {
      return "<MedicalDatabase />";
    }

    public void ResetThemeGrid()
    {
      for (int y = 0; y < this.themeGrid.GetLength(0); ++y)
      {
        for (int x = 0; x < this.themeGrid.GetLength(1); ++x)
          this.ResetGridPoint(x, y);
      }
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      this.elapsedTimeThisState = 0.0f;
      this.state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;
    }

    public void ResetGridPoint(int x, int y)
    {
      // ISSUE: explicit reference operation
      ^this.themeGrid.Address(y, x) = new MedicalDatabaseDaemon.GridSpot()
      {
        from = this.themeGrid[y, x].to,
        to = Utils.randm(2f),
        time = 0.0f,
        totalTime = Utils.randm(3f) + 1.2f
      };
    }

    private void LookupEntry()
    {
      List<string> stringList = new List<string>();
      stringList.Add(this.searchName.Trim().ToLower().Replace(" ", "_"));
      if (this.searchName.Contains(" "))
      {
        string str1 = (this.searchName.Substring(this.searchName.IndexOf(" ")) + this.searchName.Substring(0, this.searchName.IndexOf(" "))).Trim().ToLower().Replace(" ", "_");
        stringList.Add(str1);
        string str2 = (this.searchName.Substring(this.searchName.IndexOf(" ")) + "_" + this.searchName.Substring(0, this.searchName.IndexOf(" "))).Trim().ToLower().Replace(" ", "_");
        stringList.Add(str2);
      }
      FileEntry fileEntry = (FileEntry) null;
      for (int index1 = 0; index1 < stringList.Count; ++index1)
      {
        for (int index2 = 0; index2 < this.recordsFolder.files.Count; ++index2)
        {
          if (this.recordsFolder.files[index2].name.ToLower().StartsWith(stringList[index1]))
          {
            fileEntry = this.recordsFolder.files[index2];
            break;
          }
        }
        if (fileEntry != null)
          break;
      }
      if (fileEntry == null)
      {
        this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Error;
        this.errorMessage = LocaleTerms.Loc("No entry found for name") + " " + this.searchName + "\n" + LocaleTerms.Loc("Permutations tested") + ":\n";
        for (int index = 0; index < stringList.Count; ++index)
        {
          MedicalDatabaseDaemon medicalDatabaseDaemon = this;
          string str = medicalDatabaseDaemon.errorMessage + stringList[index] + "\n";
          medicalDatabaseDaemon.errorMessage = str;
        }
        this.elapsedTimeThisState = 0.0f;
      }
      else
      {
        this.currentFile = fileEntry;
        MedicalDatabaseDaemon.FileMedicalRecord record = new MedicalDatabaseDaemon.FileMedicalRecord();
        if (MedicalDatabaseDaemon.FileMedicalRecord.RecordFromString(this.currentFile.data, out record))
        {
          this.currentRecord = record;
          this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Entry;
          this.elapsedTimeThisState = 0.0f;
        }
        else
        {
          this.elapsedTimeThisState = 0.0f;
          this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Error;
          this.errorMessage = LocaleTerms.Loc("Corrupt record") + " --\n" + LocaleTerms.Loc("Unable to parse record") + " " + this.currentFile.name;
        }
      }
    }

    private void UpdateStates(float t)
    {
      this.elapsedTimeThisState += t;
      if ((double) this.elapsedTimeThisState < (double) this.totalTimeThisState || this.state != MedicalDatabaseDaemon.MedicalDatabaseState.Searching)
        return;
      this.LookupEntry();
    }

    private void SendReportEmail(MedicalDatabaseDaemon.FileMedicalRecord record, string emailAddress)
    {
      try
      {
        string email = MailServer.generateEmail(LocaleTerms.Loc("MedicalRecord") + " - " + record.Lastname + "_" + record.Firstname, record.ToEmailString(), "records@meddb.org");
        string userTo = emailAddress;
        if (emailAddress.Contains<char>('@'))
          userTo = emailAddress.Substring(0, emailAddress.IndexOf('@'));
        Computer computer = Programs.getComputer(this.os, "jmail");
        if (computer == null)
          return;
        MailServer daemon = (MailServer) computer.getDaemon(typeof (MailServer));
        if (daemon != null)
          daemon.addMail(email, userTo);
      }
      catch (Exception ex)
      {
      }
    }

    private void updateGrid()
    {
      float totalSeconds = (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
      for (int y = 0; y < this.themeGrid.GetLength(0); ++y)
      {
        for (int x = 0; x < this.themeGrid.GetLength(1); ++x)
        {
          MedicalDatabaseDaemon.GridSpot gridSpot = this.themeGrid[y, x];
          gridSpot.time += totalSeconds;
          if ((double) gridSpot.time >= (double) gridSpot.totalTime)
            this.ResetGridPoint(x, y);
          else
            this.themeGrid[y, x] = gridSpot;
        }
      }
    }

    private void drawGrid(Rectangle bounds, SpriteBatch sb, int width)
    {
      int num1 = 12;
      int x = bounds.X + bounds.Width - num1 - 1;
      Rectangle destinationRectangle = new Rectangle(x, bounds.Y + 1, num1, num1);
      int num2;
      int num3 = num2 = 0;
      while (destinationRectangle.Y + 1 < bounds.Y + bounds.Height)
      {
        if (destinationRectangle.Y + num1 + 1 >= bounds.Y + bounds.Height)
          destinationRectangle.Height = bounds.Y + bounds.Height - (destinationRectangle.Y + 2);
        while (destinationRectangle.X - num1 > bounds.X + bounds.Width - width - 1)
        {
          MedicalDatabaseDaemon.GridSpot gridSpot = this.themeGrid[num3 % this.themeGrid.GetLength(0), num2 % this.themeGrid.GetLength(1)];
          float num4 = gridSpot.time / gridSpot.totalTime;
          float amount = gridSpot.from + num4 * (gridSpot.to - gridSpot.from);
          Color color1 = this.theme_deep;
          Color color2 = this.theme_strong;
          if ((double) amount >= 1.0)
          {
            --amount;
            color1 = this.theme_strong;
            color2 = this.theme_light;
          }
          Color color3 = Color.Lerp(color1, color2, amount);
          sb.Draw(Utils.white, destinationRectangle, color3);
          ++num3;
          destinationRectangle.X -= num1 + 1;
        }
        ++num2;
        destinationRectangle.X = x;
        destinationRectangle.Y += num1 + 1;
      }
    }

    public void DrawError(Rectangle bounds, SpriteBatch sb)
    {
      Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 12, (int) ((double) bounds.Width / 1.6) - 4, bounds.Height / 2);
      rectangle.Height = (int) ((double) this.logo.Height / (double) this.logo.Width * (double) rectangle.Height);
      sb.Draw(this.logo, rectangle, this.theme_deep * 0.4f);
      int width = rectangle.Width;
      rectangle.Y += 100;
      rectangle.Height = 35;
      rectangle.Width = (int) ((double) width * (double) Utils.QuadraticOutCurve(Math.Min(this.elapsedTimeThisState * 2f, 1f)));
      this.DrawMessage(LocaleTerms.Loc("Error"), true, sb, rectangle, this.os.lockedColor, Color.White);
      rectangle.Y += rectangle.Height + 2;
      rectangle.Height = 100;
      rectangle.Width = (int) ((double) width * (double) Utils.QuadraticOutCurve(Math.Min(this.elapsedTimeThisState * 1.5f, 1f)));
      this.DrawMessageBot(this.errorMessage, false, sb, rectangle, this.theme_back, this.theme_light);
      rectangle.Width = width;
      rectangle.Y += rectangle.Height + 2;
      if (!Button.doButton(444402002, rectangle.X, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Back to menu"), new Color?(this.theme_strong)))
        return;
      this.elapsedTimeThisState = 0.0f;
      this.state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;
    }

    public void DrawSendReport(Rectangle bounds, SpriteBatch sb)
    {
      Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 12, (int) ((double) bounds.Width / 1.6) - 4, bounds.Height / 2);
      rectangle.Height = (int) ((double) this.logo.Height / (double) this.logo.Width * (double) rectangle.Height);
      sb.Draw(this.logo, rectangle, this.theme_deep * 0.4f);
      int width = rectangle.Width;
      rectangle.Y += 100;
      rectangle.Height = 35;
      rectangle.Width = (int) ((double) width * (double) Utils.QuadraticOutCurve(Math.Min(this.elapsedTimeThisState * 2f, 1f)));
      this.DrawMessage(LocaleTerms.Loc("Send Record Copy"), true, sb, rectangle, this.theme_deep, Color.White);
      rectangle.Y += rectangle.Height + 2;
      rectangle.Height = 22;
      rectangle.Width = (int) ((double) width * (double) Utils.QuadraticOutCurve(Math.Min(this.elapsedTimeThisState * 1.5f, 1f)));
      this.DrawMessageBot(LocaleTerms.Loc("Record for") + " " + this.currentRecord.Firstname + " " + this.currentRecord.Lastname, false, sb, rectangle, this.theme_back, this.theme_light);
      rectangle.Width = width;
      rectangle.Y += rectangle.Height + 12;
      rectangle.Height = 35;
      rectangle.Width = (int) (float) width;
      this.DrawMessage(LocaleTerms.Loc("Recipient Address"), true, sb, rectangle, this.theme_deep, Color.White);
      rectangle.Y += rectangle.Height + 2;
      string upperPrompt = " ---------";
      rectangle.Height = 130;
      Rectangle bounds1 = rectangle;
      if (bounds1.Width < 400)
        bounds1.Width = bounds.Width - bounds.Width / 4 - 2;
      if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReportSearch)
      {
        this.emailRecipientAddress = GetStringUIControl.DrawGetStringControl(LocaleTerms.Loc("Recipient Address (Case Sensitive):") + " ", bounds1, (Action) (() =>
        {
          this.elapsedTimeThisState = 0.0f;
          this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Error;
          this.errorMessage = LocaleTerms.Loc("Error getting recipient email");
        }), (Action) (() => this.state = MedicalDatabaseDaemon.MedicalDatabaseState.SendReport), sb, (object) this.os, this.theme_strong, this.os.lockedColor, upperPrompt, new Color?());
        rectangle.Y += 26;
        if (this.emailRecipientAddress != null)
        {
          this.state = MedicalDatabaseDaemon.MedicalDatabaseState.SendReportSending;
          this.elapsedTimeThisState = 1f;
        }
      }
      else
        GetStringUIControl.DrawGetStringControlInactive(LocaleTerms.Loc("Recipient Address") + ": ", this.emailRecipientAddress == null ? LocaleTerms.Loc("Undefined") : this.emailRecipientAddress, bounds1, sb, (object) this.os, upperPrompt);
      rectangle.Y += rectangle.Height + 2;
      rectangle.Height = 24;
      if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReport)
      {
        if (Button.doButton(444402023, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Specify Address"), new Color?(this.theme_strong)))
        {
          GetStringUIControl.StartGetString("Recipient_Address", (object) this.os);
          this.state = MedicalDatabaseDaemon.MedicalDatabaseState.SendReportSearch;
        }
      }
      else if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReportSending || this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete)
      {
        float point = (float) (((double) this.elapsedTimeThisState - 1.0) / 3.0);
        if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete)
          point = 1f;
        if ((double) point >= 1.0 && this.state != MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete)
        {
          this.state = MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete;
          this.SendReportEmail(this.currentRecord, this.emailRecipientAddress);
        }
        sb.Draw(Utils.white, rectangle, this.theme_back);
        Rectangle destinationRectangle = rectangle;
        destinationRectangle.Width = (int) ((double) destinationRectangle.Width * (double) Utils.QuadraticOutCurve(point));
        sb.Draw(Utils.white, destinationRectangle, this.theme_light);
        sb.DrawString(GuiData.smallfont, this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete ? LocaleTerms.Loc("COMPLETE") : LocaleTerms.Loc("SENDING") + " ...", new Vector2((float) rectangle.X, (float) (rectangle.Y + 2)), Color.Black);
      }
      rectangle.Y += rectangle.Height + 2;
      if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete && Button.doButton(444402001, rectangle.X, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Send to different address"), new Color?(this.theme_light)))
        this.state = MedicalDatabaseDaemon.MedicalDatabaseState.SendReport;
      rectangle.Y += rectangle.Height + 2;
      if (!Button.doButton(444402002, rectangle.X, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Back to menu"), new Color?(this.theme_strong)))
        return;
      this.elapsedTimeThisState = 0.0f;
      this.state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;
    }

    public void DrawAbout(Rectangle bounds, SpriteBatch sb)
    {
      string data = (string) null;
      Folder folder = this.comp.files.root.searchForFolder("home");
      if (folder != null)
      {
        FileEntry fileEntry = folder.searchForFile("MedicalDatabaseInfo.txt");
        if (fileEntry != null)
          data = fileEntry.data;
      }
      if (data == null)
      {
        this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Error;
        this.errorMessage = "DatabaseInfo file not found\n~/home/MedicalDatabaseInfo.txt\nCould not be found or opened";
        this.elapsedTimeThisState = 0.0f;
      }
      else
      {
        Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 12, (int) ((double) bounds.Width / 1.6) - 4, bounds.Height / 2);
        rectangle.Height = (int) ((double) this.logo.Height / (double) this.logo.Width * (double) rectangle.Height);
        sb.Draw(this.logo, rectangle, this.theme_deep * 0.4f);
        int width = rectangle.Width;
        rectangle.Y += 100;
        rectangle.Height = 35;
        rectangle.Width = (int) ((double) width * (double) Utils.QuadraticOutCurve(Math.Min(this.elapsedTimeThisState * 2f, 1f)));
        this.DrawMessage(LocaleTerms.Loc("Info"), true, sb, rectangle, this.theme_deep, Color.White);
        string msg = Utils.SuperSmartTwimForWidth(data, rectangle.Width - 12, GuiData.tinyfont);
        rectangle.Y += rectangle.Height + 2;
        rectangle.Height = Math.Min(bounds.Height - 200, 420);
        rectangle.Width = (int) ((double) width * (double) Utils.QuadraticOutCurve(Math.Min(this.elapsedTimeThisState * 1.5f, 1f)));
        this.DrawMessageBot(msg, false, sb, rectangle, this.theme_back, this.theme_light);
        rectangle.Width = width;
        rectangle.Y += rectangle.Height + 2;
        if (!Button.doButton(444402002, rectangle.X, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Back to menu"), new Color?(this.theme_strong)))
          return;
        this.elapsedTimeThisState = 0.0f;
        this.state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;
      }
    }

    public void DrawEntry(Rectangle bounds, SpriteBatch sb)
    {
      int num1 = 34;
      if (this.displayPanel == null)
        this.displayPanel = new ScrollableSectionedPanel(26, sb.GraphicsDevice);
      List<Action<int, Rectangle, SpriteBatch>> drawCalls = new List<Action<int, Rectangle, SpriteBatch>>();
      Rectangle rectangle1 = new Rectangle(bounds.X + 2, bounds.Y + 12, (int) ((double) bounds.Width / 1.6) - 4, bounds.Height / 2);
      Rectangle allTextBounds = rectangle1;
      allTextBounds.Width += 2;
      allTextBounds.Y += num1;
      allTextBounds.Height = bounds.Height - num1 - 2 - 40 - 28;
      rectangle1.Height = this.logo.Height / this.logo.Width * rectangle1.Width;
      sb.Draw(this.logo, rectangle1, this.theme_deep * 0.4f);
      rectangle1.Height = num1;
      this.DrawMessage(this.currentRecord.Lastname + ", " + this.currentRecord.Firstname, true, sb, rectangle1, this.theme_light, this.theme_back);
      rectangle1.Y += num1;
      int num2 = 22;
      rectangle1.Height = num2;
      string[] lines = this.currentRecord.record.Split(Utils.newlineDelim);
      string[] separator = new string[5]{ " :: ", ":: ", " ::", "::", "\n" };
      bool flag = false;
      for (int index1 = 0; index1 < lines.Length; ++index1)
      {
        string[] sections = Utils.SuperSmartTwimForWidth(lines[index1], rectangle1.Width - 12, GuiData.tinyfont).Split(separator, StringSplitOptions.RemoveEmptyEntries);
        if (sections.Length > 1)
        {
          for (int index2 = 0; index2 < sections.Length; ++index2)
          {
            if (index2 == 0 && !flag)
            {
              if (sections[index2] == "Notes")
                flag = true;
              int secID = index2;
              drawCalls.Add((Action<int, Rectangle, SpriteBatch>) ((index, drawPos, sprBatch) =>
              {
                Rectangle dest = drawPos;
                ++dest.Y;
                dest.Height -= 2;
                this.DrawMessage(sections[secID] + " :", false, sprBatch, dest, this.theme_deep, this.theme_light);
              }));
              rectangle1.Y += num2 + 2;
            }
            else if (sections[index2].Trim().Length > 0)
            {
              int subSecID = index2;
              drawCalls.Add((Action<int, Rectangle, SpriteBatch>) ((index, drawPos, sprBatch) =>
              {
                Rectangle dest = drawPos;
                ++dest.Y;
                dest.Height -= 2;
                this.DrawMessage(sections[subSecID], false, sprBatch, dest);
              }));
              rectangle1.Y += num2 + 2;
            }
          }
        }
        else if (lines[index1].Trim().Length > 0)
        {
          int idx = index1;
          drawCalls.Add((Action<int, Rectangle, SpriteBatch>) ((index, drawPos, sprBatch) =>
          {
            Rectangle rectangle2 = drawPos;
            ++rectangle2.Y;
            rectangle2.Height -= 2;
            this.DrawMessage(lines[idx], false, sprBatch, drawPos);
          }));
          rectangle1.Y += num2 + 2;
        }
      }
      drawCalls.Add((Action<int, Rectangle, SpriteBatch>) ((index, drawPos, sprBatch) =>
      {
        Rectangle dest = drawPos;
        dest.Y += 2;
        dest.Height -= 4;
        this.DrawMessage(" ", false, sprBatch, dest);
      }));
      rectangle1.Y += num2 + 2;
      this.displayPanel.NumberOfPanels = drawCalls.Count;
      this.displayPanel.Draw((Action<int, Rectangle, SpriteBatch>) ((idx, rect, sprBatch) =>
      {
        if ((drawCalls.Count + 1) * this.displayPanel.PanelHeight >= allTextBounds.Height)
          rect.Width -= 10;
        drawCalls[idx](idx, rect, sprBatch);
      }), sb, allTextBounds);
      rectangle1.Y += 2;
      if (Button.doButton(444402033, rectangle1.X, bounds.Y + bounds.Height - 26, rectangle1.Width, 24, LocaleTerms.Loc("Back to menu"), new Color?(this.theme_strong)))
      {
        this.elapsedTimeThisState = 0.0f;
        this.state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;
      }
      if (Button.doButton(444402035, rectangle1.X, bounds.Y + bounds.Height - 26 - 2 - 26, rectangle1.Width, 24, LocaleTerms.Loc("e-mail this record"), new Color?(this.theme_light)))
      {
        this.elapsedTimeThisState = 0.0f;
        this.state = MedicalDatabaseDaemon.MedicalDatabaseState.SendReport;
      }
      Rectangle dest1 = new Rectangle(rectangle1.X + rectangle1.Width + 2, bounds.Y + 34 + 12, (int) ((double) bounds.Width / 6.5) - 2, bounds.Height - 4);
      int num3 = 33;
      int num4 = 22;
      dest1.Height = num3;
      this.DrawMessage(LocaleTerms.Loc("Age"), true, sb, dest1);
      dest1.Y += dest1.Height + 2;
      TimeSpan timeSpan = DateTime.Now - this.currentRecord.DOB;
      int num5 = (int) ((double) timeSpan.Days / 365.0);
      this.DrawMessage(string.Concat((object) (timeSpan.Days / 365)), true, sb, dest1, Color.Transparent, this.theme_light);
      dest1.Y += dest1.Height;
      dest1.Height = num4;
      this.DrawMessage(LocaleTerms.Loc("Years"), false, sb, dest1, Color.Transparent, this.theme_light);
      dest1.Y += dest1.Height + 2;
      dest1.Height = num3;
      this.DrawMessage(string.Concat((object) (timeSpan.Days - num5 * 365)), true, sb, dest1, Color.Transparent, this.theme_light);
      dest1.Y += dest1.Height;
      dest1.Height = num4;
      this.DrawMessage(LocaleTerms.Loc("Days"), false, sb, dest1, Color.Transparent, this.theme_light);
      dest1.Y += dest1.Height + 2;
      dest1.Height = num3;
      this.DrawMessage(string.Concat((object) timeSpan.Hours), true, sb, dest1, Color.Transparent, this.theme_light);
      dest1.Y += dest1.Height;
      dest1.Height = num4;
      this.DrawMessage(LocaleTerms.Loc("Hours"), false, sb, dest1, Color.Transparent, this.theme_light);
      dest1.Y += dest1.Height + 2;
      dest1.Height = num3;
      this.DrawMessage(string.Concat((object) timeSpan.Minutes), true, sb, dest1, Color.Transparent, this.theme_light);
      dest1.Y += dest1.Height;
      dest1.Height = num4;
      this.DrawMessage(LocaleTerms.Loc("Minutes"), false, sb, dest1, Color.Transparent, this.theme_light);
      dest1.Y += dest1.Height + 2;
      dest1.Height = num3;
      this.DrawMessage(string.Concat((object) timeSpan.Seconds), true, sb, dest1, Color.Transparent, this.theme_light);
      dest1.Y += dest1.Height;
      dest1.Height = num4;
      this.DrawMessage(LocaleTerms.Loc("Seconds"), false, sb, dest1, Color.Transparent, this.theme_light);
      dest1.Y += dest1.Height + 2;
      dest1.Height = num3;
    }

    public void DrawMenu(Rectangle bounds, SpriteBatch sb)
    {
      int height = 34;
      Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 12, bounds.Width / 2 - 4, height);
      this.DrawMessage(LocaleTerms.Loc("Universal Medical"), true, sb, rectangle, this.theme_light, Color.Black);
      rectangle.Y += height + 2;
      rectangle.Height = 20;
      this.DrawMessage(LocaleTerms.Loc("Records & Monitoring Services"), false, sb, rectangle);
      Rectangle destinationRectangle1 = new Rectangle(bounds.X + bounds.Width / 2 + 10, bounds.Y + 12, bounds.Width / 4 - 12, (int) ((double) this.logo.Height / (double) this.logo.Width * ((double) bounds.Width / 4.0)));
      sb.Draw(this.logo, destinationRectangle1, this.theme_light);
      Rectangle destinationRectangle2 = new Rectangle(rectangle.X + 10, rectangle.Y + 40, rectangle.Width - 20, 1);
      sb.Draw(Utils.white, destinationRectangle2, Utils.SlightlyDarkGray * 0.5f);
      destinationRectangle2.Y += 4;
      sb.Draw(Utils.white, destinationRectangle2, Utils.SlightlyDarkGray * 0.5f);
      rectangle.Y += 90;
      if (!(this.comp.adminIP == this.os.thisComputer.ip))
      {
        rectangle.Height = bounds.Y + bounds.Height - rectangle.Y;
        this.DrawNoAdminMenuSection(rectangle, sb);
      }
      else
      {
        rectangle.Height = 80;
        this.DrawMessageBot(LocaleTerms.Loc("Information"), true, sb, rectangle, this.theme_light, Color.Black);
        rectangle.Y += rectangle.Height + 2;
        rectangle.Height = 20;
        this.DrawMessage(LocaleTerms.Loc("Details and Administration"), false, sb, rectangle);
        rectangle.Y += rectangle.Height + 2;
        if (Button.doButton(444402000, rectangle.X + 1, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Info"), new Color?(this.theme_strong)))
        {
          this.state = MedicalDatabaseDaemon.MedicalDatabaseState.AboutScreen;
          this.elapsedTimeThisState = 0.0f;
        }
        rectangle.Y += 60;
        rectangle.Height = 80;
        this.DrawMessageBot(LocaleTerms.Loc("Database"), true, sb, rectangle, this.theme_light, Color.Black);
        rectangle.Y += rectangle.Height + 2;
        rectangle.Height = 20;
        this.DrawMessage(LocaleTerms.Loc("Records Lookup"), false, sb, rectangle);
        rectangle.Y += rectangle.Height + 2;
        if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu)
        {
          if (Button.doButton(444402005, rectangle.X + 1, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Search"), new Color?(this.theme_strong)))
          {
            this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Search;
            this.elapsedTimeThisState = 0.0f;
            GetStringUIControl.StartGetString("Patient_Name", (object) this.os);
          }
          rectangle.Y += 26;
          if (Button.doButton(444402007, rectangle.X + 1, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Random Entry"), new Color?(this.theme_strong)))
          {
            this.searchName = this.recordsFolder.files[Utils.random.Next(this.recordsFolder.files.Count)].name;
            this.elapsedTimeThisState = 0.0f;
            this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Searching;
            this.totalTimeThisState = 1.6f;
          }
        }
        else if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.Search)
        {
          float num = Utils.QuadraticOutCurve(Math.Min(1f, this.elapsedTimeThisState * 2f));
          Rectangle bounds1 = new Rectangle(rectangle.X, rectangle.Y - 10, rectangle.Width, (int) ((double) num * 72.0));
          Rectangle destinationRectangle3 = new Rectangle(bounds1.X, rectangle.Y + 2, rectangle.Width, (int) ((double) num * 32.0));
          sb.Draw(Utils.white, destinationRectangle3, this.os.darkBackgroundColor);
          string stringControl = GetStringUIControl.DrawGetStringControl(LocaleTerms.Loc("Enter patient name") + " :", bounds1, (Action) (() =>
          {
            this.elapsedTimeThisState = 0.0f;
            this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Error;
            this.errorMessage = LocaleTerms.Loc("Error in name input");
          }), (Action) (() =>
          {
            this.elapsedTimeThisState = 0.0f;
            this.state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;
            this.os.terminal.executeLine();
          }), sb, (object) this.os, this.theme_strong, this.theme_back, "", new Color?(Color.Transparent));
          if (stringControl != null)
          {
            this.searchName = stringControl;
            this.elapsedTimeThisState = 0.0f;
            this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Searching;
            this.totalTimeThisState = 1.6f;
          }
        }
        else if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.Searching)
        {
          Rectangle destinationRectangle3 = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, 24);
          sb.Draw(Utils.white, destinationRectangle3, this.theme_deep);
          destinationRectangle3.Width = (int) ((double) destinationRectangle3.Width * (double) Utils.QuadraticOutCurve(this.elapsedTimeThisState / this.totalTimeThisState));
          sb.Draw(Utils.white, destinationRectangle3, this.theme_light);
          destinationRectangle3.Y += destinationRectangle3.Height / 2 - 2;
          destinationRectangle3.Height = 4;
          sb.Draw(Utils.white, destinationRectangle3, this.theme_deep);
        }
        if (Button.doButton(444402800, rectangle.X + 1, bounds.Y + bounds.Height - 28, rectangle.Width, 24, LocaleTerms.Loc("Exit Database View"), new Color?(this.os.lockedColor)))
          this.os.display.command = "connect";
      }
    }

    private void DrawNoAdminMenuSection(Rectangle bounds, SpriteBatch sb)
    {
      bounds.Height -= 2;
      ++bounds.X;
      bounds.Width -= 2;
      bounds.Height -= 30;
      PatternDrawer.draw(bounds, 0.2f, Color.Transparent, this.os.brightLockedColor, sb, PatternDrawer.errorTile);
      bounds.Height += 30;
      Rectangle dest = bounds;
      dest.Height = 36;
      dest.Y = bounds.Y + bounds.Height / 2 - dest.Height / 2;
      this.DrawMessage(LocaleTerms.Loc("Admin Access"), true, sb, dest, this.os.brightLockedColor * 0.8f, Color.Black);
      dest.Y += dest.Height + 2;
      dest.Height = 22;
      this.DrawMessage(LocaleTerms.Loc("Required for use"), false, sb, dest, this.theme_back, this.os.brightLockedColor);
      if (!Button.doButton(444402800, bounds.X + 1, bounds.Y + bounds.Height - 28, bounds.Width, 24, LocaleTerms.Loc("Exit Database View"), new Color?(this.os.lockedColor)))
        return;
      this.os.display.command = "connect";
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      this.UpdateStates((float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
      this.updateGrid();
      this.drawGrid(bounds, sb, bounds.Width / 4);
      switch (this.state)
      {
        case MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu:
        case MedicalDatabaseDaemon.MedicalDatabaseState.Search:
        case MedicalDatabaseDaemon.MedicalDatabaseState.Searching:
          this.DrawMenu(bounds, sb);
          break;
        case MedicalDatabaseDaemon.MedicalDatabaseState.Entry:
          this.DrawEntry(bounds, sb);
          break;
        case MedicalDatabaseDaemon.MedicalDatabaseState.Error:
          this.DrawError(bounds, sb);
          break;
        case MedicalDatabaseDaemon.MedicalDatabaseState.AboutScreen:
          this.DrawAbout(bounds, sb);
          break;
        case MedicalDatabaseDaemon.MedicalDatabaseState.SendReport:
        case MedicalDatabaseDaemon.MedicalDatabaseState.SendReportSearch:
        case MedicalDatabaseDaemon.MedicalDatabaseState.SendReportSending:
        case MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete:
          this.DrawSendReport(bounds, sb);
          break;
      }
    }

    private void DrawMessage(string msg, bool big, SpriteBatch sb, Rectangle dest)
    {
      this.DrawMessage(msg, big, sb, dest, this.theme_back, this.theme_light);
    }

    private void DrawMessage(string msg, bool big, SpriteBatch sb, Rectangle dest, Color back, Color front)
    {
      sb.Draw(Utils.white, dest, back);
      SpriteFont spriteFont = big ? GuiData.font : GuiData.tinyfont;
      Vector2 vector2 = spriteFont.MeasureString(msg);
      Vector2 position = new Vector2((float) (dest.X + dest.Width - 4) - vector2.X, (float) ((double) dest.Y + (double) dest.Height / 2.0 - (double) vector2.Y / 2.0));
      if ((double) vector2.X >= (double) (dest.Width - 4) || (double) vector2.Y >= (double) dest.Height)
        TextItem.doFontLabelToSize(dest, msg, spriteFont, front, false, false);
      else
        sb.DrawString(spriteFont, msg, position, front);
    }

    private void DrawMessageBot(string msg, bool big, SpriteBatch sb, Rectangle dest, Color back, Color front)
    {
      sb.Draw(Utils.white, dest, back);
      SpriteFont spriteFont = big ? GuiData.font : GuiData.tinyfont;
      Vector2 vector2 = spriteFont.MeasureString(msg);
      Vector2 position = new Vector2((float) (dest.X + dest.Width - 4) - vector2.X, (float) ((double) (dest.Y + dest.Height) - (double) vector2.Y - 4.0));
      if ((double) vector2.X >= (double) (dest.Width - 4) || (double) vector2.Y >= (double) (dest.Height - 4))
        TextItem.doFontLabelToSize(dest, msg, spriteFont, front, false, false);
      else
        sb.DrawString(spriteFont, msg, position, front);
    }

    private class FileMedicalRecord
    {
      private static string[] SPLIT_DELIM = new string[1]{ "\n-----------------\n" };
      public bool IsMale = true;
      private const string DELIMITER = "\n-----------------\n";
      public string Firstname;
      public string Lastname;
      public string record;
      public DateTime DOB;

      public FileMedicalRecord()
      {
      }

      public FileMedicalRecord(Person p)
      {
        this.Firstname = p.firstName;
        this.Lastname = p.lastName;
        this.IsMale = p.isMale;
        this.record = this.MedicalRecordToReport(p.medicalRecord);
        this.DOB = p.medicalRecord.DateofBirth;
      }

      public string MedicalRecordToReport(MedicalRecord rec)
      {
        return rec.ToString();
      }

      public override string ToString()
      {
        return this.Firstname + "\n-----------------\n" + this.Lastname + "\n-----------------\n" + (this.IsMale ? "male" : "female") + "\n-----------------\n" + Utils.SafeWriteDateTime(this.DOB) + "\n-----------------\n" + this.record;
      }

      public string ToEmailString()
      {
        return this.ToString().Replace("tions ::", "tions::\n").Replace("Visits ::", "Visits::\n").Replace("Notes ::", "\nNotes ::\n");
      }

      public string GetFileName()
      {
        return this.Lastname.ToLower() + "_" + this.Firstname.ToLower() + ".rec";
      }

      public static bool RecordFromString(string rec, out MedicalDatabaseDaemon.FileMedicalRecord record)
      {
        CultureInfo cultureInfo = new CultureInfo("en-au");
        string[] strArray = rec.Split(MedicalDatabaseDaemon.FileMedicalRecord.SPLIT_DELIM, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length >= 5)
        {
          record = new MedicalDatabaseDaemon.FileMedicalRecord();
          record.Firstname = strArray[0];
          record.Lastname = strArray[1];
          record.IsMale = strArray[2] == "male";
          record.DOB = Utils.SafeParseDateTime(strArray[3]);
          record.record = strArray[4];
          return true;
        }
        record = (MedicalDatabaseDaemon.FileMedicalRecord) null;
        return false;
      }
    }

    private enum MedicalDatabaseState
    {
      MainMenu,
      Search,
      Searching,
      Entry,
      Error,
      AboutScreen,
      SendReport,
      SendReportSearch,
      SendReportSending,
      SendReportComplete,
    }

    private struct GridSpot
    {
      public float from;
      public float to;
      public float time;
      public float totalTime;
    }
  }
}
