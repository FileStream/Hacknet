// Decompiled with JetBrains decompiler
// Type: Hacknet.DeathRowDatabaseDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet
{
  internal class DeathRowDatabaseDaemon : Daemon
  {
    private int SelectedIndex = -1;
    private Color themeColor = new Color(207, 44, 19);
    private Vector2 recordScrollPosition = Vector2.Zero;
    public const string ROOT_FOLDERNAME = "dr_database";
    public const string RECORDS_FOLDERNAME = "records";
    public const string SERVER_INFO_FILENAME = "ServerDetails.txt";
    private static Texture2D Logo;
    private static Texture2D Circle;
    private Folder root;
    private Folder records;

    public DeathRowDatabaseDaemon(Computer c, string serviceName, OS os)
      : base(c, serviceName, os)
    {
      if (DeathRowDatabaseDaemon.Logo == null)
        DeathRowDatabaseDaemon.Logo = os.content.Load<Texture2D>("Sprites/DeathRowLogo");
      if (DeathRowDatabaseDaemon.Circle != null)
        return;
      DeathRowDatabaseDaemon.Circle = os.content.Load<Texture2D>("Sprites/ThinCircleOutline");
    }

    public override void initFiles()
    {
      base.initFiles();
      this.root = new Folder("dr_database");
      this.records = new Folder("records");
      this.root.folders.Add(this.records);
      this.comp.files.root.folders.Add(this.root);
      this.root.files.Add(new FileEntry(Utils.readEntireFile("Content/Post/DeathRowServerInfo.txt").Replace('\t', ' '), "ServerDetails.txt"));
      this.LoadRecords((string) null);
    }

    private void LoadRecords(string data = null)
    {
      string str1 = Utils.readEntireFile("Content/Post/DeathRow.txt");
      if (data != null)
        str1 = data;
      string str2 = Utils.readEntireFile("Content/Post/DeathRowSpecials.txt");
      string[] strArray = (str1 + str2).Split(new string[1]{ "\r\n###%%##%%##%%##\r\n" }, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray.Length; ++index)
      {
        DeathRowDatabaseDaemon.DeathRowEntry record = this.ConvertStringToRecord(strArray[index]);
        if (record.FName != null)
        {
          string nameEntry = record.LName + "_" + record.FName + "[" + record.RecordNumber + "]";
          this.records.files.Add(new FileEntry(strArray[index].Replace("#", "#\n"), nameEntry));
        }
      }
      this.records.files.Sort((Comparison<FileEntry>) ((f1, f2) => string.Compare(f1.name, f2.name)));
    }

    private DeathRowDatabaseDaemon.DeathRowEntry ConvertStringToRecord(string data)
    {
      string[] strArray = data.Split(new string[2]{ "#\n", "#" }, StringSplitOptions.None);
      DeathRowDatabaseDaemon.DeathRowEntry deathRowEntry = new DeathRowDatabaseDaemon.DeathRowEntry();
      if (strArray.Length >= 9)
      {
        deathRowEntry.FName = strArray[0];
        deathRowEntry.LName = strArray[1];
        deathRowEntry.RecordNumber = strArray[2];
        deathRowEntry.Age = strArray[3];
        deathRowEntry.Date = strArray[4];
        deathRowEntry.Country = strArray[5];
        deathRowEntry.PriorRecord = strArray[6];
        deathRowEntry.IncidentReport = strArray[7];
        deathRowEntry.Statement = strArray[8];
      }
      return deathRowEntry;
    }

    public override void loadInit()
    {
      base.loadInit();
      this.root = this.comp.files.root.searchForFolder("dr_database");
      this.records = this.root.searchForFolder("records");
    }

    public override string getSaveString()
    {
      return "<DeathRowDatabase />";
    }

    private string ConvertFilesToOutput()
    {
      string str1 = "\r\n###%%##%%##%%##\r\n";
      string str2 = "#";
      string str3 = "";
      this.records.files.Sort((Comparison<FileEntry>) ((f1, f2) => string.Compare(f1.name, f2.name)));
      for (int index = 0; index < this.records.files.Count; ++index)
      {
        DeathRowDatabaseDaemon.DeathRowEntry record = this.ConvertStringToRecord(this.records.files[index].data);
        if (record.RecordNumber != null)
        {
          string str4 = record.FName + str2 + record.LName + str2 + record.RecordNumber + str2 + record.Age + str2 + record.Date + str2 + record.Country + str2 + record.PriorRecord + str2 + record.IncidentReport + str2 + record.Statement + str1;
          str3 += str4;
        }
      }
      return str3;
    }

    private void TestStringConversion()
    {
      string output = this.ConvertFilesToOutput();
      this.records.files.Clear();
      this.LoadRecords(output);
    }

    public bool ContainsRecordForName(string fName, string lName)
    {
      string str = lName + "_" + fName;
      for (int index = 0; index < this.records.files.Count; ++index)
      {
        if (this.records.files[index].name.StartsWith(str))
          return true;
      }
      return false;
    }

    public DeathRowDatabaseDaemon.DeathRowEntry GetRecordForName(string fName, string lName)
    {
      string str = lName + "_" + fName;
      for (int index = 0; index < this.records.files.Count; ++index)
      {
        if (this.records.files[index].name.StartsWith(str))
          return this.ConvertStringToRecord(this.records.files[index].data);
        try
        {
          DeathRowDatabaseDaemon.DeathRowEntry record = this.ConvertStringToRecord(this.records.files[index].data);
          if (record.FName.ToLower() == fName.ToLower() && record.LName.ToLower() == lName.ToLower())
            return record;
        }
        catch (Exception ex)
        {
        }
      }
      return new DeathRowDatabaseDaemon.DeathRowEntry();
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      string[] text = new string[this.records.files.Count];
      for (int index = 0; index < this.records.files.Count; ++index)
      {
        try
        {
          text[index] = this.records.files[index].name.Substring(0, this.records.files[index].name.IndexOf('[')).Replace("_", ", ");
        }
        catch (Exception ex)
        {
          text[index] = "UNKNOWN" + (object) index;
        }
      }
      int width = bounds.Width / 3;
      int selectedIndex = this.SelectedIndex;
      this.SelectedIndex = SelectableTextList.doFancyList(832190831, bounds.X + 1, bounds.Y + 4, width, bounds.Height - 8, text, this.SelectedIndex, new Color?(this.themeColor), true);
      if (this.SelectedIndex != selectedIndex)
        this.recordScrollPosition = Vector2.Zero;
      sb.Draw(Utils.white, new Rectangle(bounds.X + width - 1, bounds.Y + 1, 2, bounds.Height - 2), this.themeColor);
      DeathRowDatabaseDaemon.DeathRowEntry entry = new DeathRowDatabaseDaemon.DeathRowEntry();
      Rectangle bounds1 = bounds;
      bounds1.X += width;
      bounds1.Width -= width + 1;
      if (this.SelectedIndex >= 0 && this.SelectedIndex < this.records.files.Count)
        entry = this.ConvertStringToRecord(this.records.files[this.SelectedIndex].data);
      if (entry.RecordNumber != null)
        this.DrawRecord(bounds1, sb, entry);
      else
        this.DrawTitleScreen(bounds1, sb);
    }

    private void DrawTitleScreen(Rectangle bounds, SpriteBatch sb)
    {
      bool drawShadow = TextItem.DrawShadow;
      TextItem.DrawShadow = false;
      Rectangle destinationRectangle = new Rectangle(bounds.X + 12, bounds.Y + 12, DeathRowDatabaseDaemon.Logo.Width, DeathRowDatabaseDaemon.Logo.Height);
      sb.Draw(DeathRowDatabaseDaemon.Logo, destinationRectangle, Color.White);
      float num1 = 1f;
      float num2 = 1.4f;
      float point = this.os.timer % 3f;
      if ((double) point > 1.0)
        point = 0.0f;
      float num3 = (1f - point) * 0.4f;
      if ((double) point == 0.0)
        num3 = 0.0f;
      float num4 = Utils.QuadraticOutCurve(point) * (num2 - num1);
      if ((double) num4 > 0.0)
        num4 += num1;
      destinationRectangle = new Rectangle((int) ((double) destinationRectangle.X - (double) (destinationRectangle.Width / 2) * (double) num4), (int) ((double) destinationRectangle.Y - (double) (destinationRectangle.Height / 2) * (double) num4), (int) ((double) destinationRectangle.Width * (double) num4), (int) ((double) destinationRectangle.Height * (double) num4));
      destinationRectangle.X += DeathRowDatabaseDaemon.Logo.Width / 2;
      destinationRectangle.Y += DeathRowDatabaseDaemon.Logo.Height / 2;
      sb.Draw(DeathRowDatabaseDaemon.Circle, destinationRectangle, Color.White * num3);
      Vector2 pos = new Vector2((float) (bounds.X + DeathRowDatabaseDaemon.Logo.Width + 22), (float) (bounds.Y + 14));
      TextItem.doFontLabel(pos, "DEATH ROW", GuiData.titlefont, new Color?(this.themeColor), (float) (bounds.Width - DeathRowDatabaseDaemon.Logo.Width) - 26f, 50f, false);
      pos.Y += 45f;
      TextItem.doFontLabel(pos, "EXECUTED OFFENDERS LISTING", GuiData.titlefont, new Color?(Color.White), (float) (bounds.Width - DeathRowDatabaseDaemon.Logo.Width) - 26f, 40f, false);
      pos.Y += (float) (DeathRowDatabaseDaemon.Logo.Height - 40);
      pos.X = (float) (bounds.X + 12);
      FileEntry fileEntry = this.root.searchForFile("ServerDetails.txt");
      if (fileEntry != null)
      {
        string text = Utils.SmartTwimForWidth(fileEntry.data, bounds.Width - 30, GuiData.tinyfont);
        TextItem.doFontLabel(pos, text, GuiData.tinyfont, new Color?(Color.White), (float) (bounds.Width - 20), (float) bounds.Height - 200f, false);
      }
      if (Button.doButton(166261601, bounds.X + 6, bounds.Y + bounds.Height - 26, bounds.Width - 12, 22, LocaleTerms.Loc("Exit"), new Color?(Color.Black)))
        this.os.display.command = "connect";
      TextItem.DrawShadow = drawShadow;
    }

    private void DrawRecord(Rectangle bounds, SpriteBatch sb, DeathRowDatabaseDaemon.DeathRowEntry entry)
    {
      bounds.X += 2;
      bounds.Width -= 2;
      bool drawShadow = TextItem.DrawShadow;
      TextItem.DrawShadow = false;
      int height = 850;
      ScrollablePanel.beginPanel(98302836, new Rectangle(bounds.X, bounds.Y, bounds.Width, height), this.recordScrollPosition);
      int num1 = bounds.Width - 16;
      Vector2 vector2_1 = new Vector2(5f, 5f);
      GuiData.spriteBatch.Draw(DeathRowDatabaseDaemon.Logo, new Rectangle((int) vector2_1.X, (int) vector2_1.Y, 60, 60), Color.White);
      vector2_1.X += 70f;
      TextItem.doFontLabel(vector2_1, "DEATH ROW : EXECUTED OFFENDERS LISTING", GuiData.titlefont, new Color?(this.themeColor), (float) (num1 - 80), 45f, false);
      vector2_1.Y += 22f;
      if (Button.doButton(98102855, (int) vector2_1.X, (int) vector2_1.Y, bounds.Width / 2, 25, "Return", new Color?(Color.Black)))
        this.SelectedIndex = -1;
      vector2_1.X = 5f;
      vector2_1.Y += 55f;
      TextItem.doFontLabel(vector2_1, "RECORD " + entry.RecordNumber, GuiData.titlefont, new Color?(Color.White), (float) (num1 - 4), 60f, false);
      vector2_1.Y += 70f;
      int seperatorHeight = 18;
      int margin = 12;
      vector2_1 = this.DrawCompactLabel(LocaleTerms.Loc("Name") + ":", entry.LName + ", " + entry.FName, vector2_1, margin, seperatorHeight, num1);
      vector2_1 = this.DrawCompactLabel(LocaleTerms.Loc("Age") + ":", entry.Age, vector2_1, margin, seperatorHeight, num1);
      int num2 = 20;
      int num3 = 20;
      Vector2 vector2_2 = Vector2.Zero;
      TextItem.doFontLabel(vector2_1, LocaleTerms.Loc("Incident Report") + ":", GuiData.smallfont, new Color?(this.themeColor), (float) num1, float.MaxValue, false);
      vector2_1.Y += (float) num2;
      TextItem.DrawShadow = false;
      Vector2 vector2_3 = TextItem.doMeasuredSmallLabel(vector2_1, Utils.SmartTwimForWidth(entry.IncidentReport, num1, GuiData.smallfont), new Color?(Color.White));
      vector2_1.Y += Math.Max(vector2_3.Y, (float) num2);
      vector2_1.Y += (float) num3;
      TextItem.doFontLabel(vector2_1, LocaleTerms.Loc("Final Statement") + ":", GuiData.smallfont, new Color?(this.themeColor), (float) num1, float.MaxValue, false);
      vector2_1.Y += (float) num2;
      vector2_2 = TextItem.doMeasuredSmallLabel(vector2_1, Utils.SmartTwimForWidth(entry.Statement, num1, GuiData.smallfont), new Color?(Color.White));
      vector2_1.Y += (float) num3;
      this.recordScrollPosition = ScrollablePanel.endPanel(98302836, this.recordScrollPosition, bounds, (float) (height - bounds.Height), true);
      TextItem.DrawShadow = drawShadow;
    }

    private Vector2 DrawCompactLabel(string label, string value, Vector2 drawPos, int margin, int seperatorHeight, int textWidth)
    {
      TextItem.doFontLabel(drawPos, label, GuiData.smallfont, new Color?(this.themeColor), (float) textWidth, float.MaxValue, false);
      drawPos.Y += (float) seperatorHeight;
      TextItem.doFontLabel(drawPos, value, GuiData.smallfont, new Color?(Color.White), (float) textWidth, float.MaxValue, false);
      drawPos.Y += (float) seperatorHeight;
      drawPos.Y += (float) margin;
      return drawPos;
    }

    public struct DeathRowEntry
    {
      public string FName;
      public string LName;
      public string RecordNumber;
      public string Age;
      public string Date;
      public string Country;
      public string PriorRecord;
      public string IncidentReport;
      public string Statement;
    }
  }
}
