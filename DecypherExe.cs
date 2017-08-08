// Decompiled with JetBrains decompiler
// Type: Hacknet.DecypherExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class DecypherExe : ExeModule
  {
    private string password = "";
    private DecypherExe.DecypherStatus status = DecypherExe.DecypherStatus.Loading;
    private float timeOnThisPhase = 0.0f;
    private float percentComplete = 0.0f;
    private string errorMessage = "Unknown Error";
    private string displayHeader = "Unknown";
    private string displayIP = "Unknown";
    private string writtenFilename = "Unknown";
    private List<int> rowsActive = new List<int>();
    private List<int> columnsActive = new List<int>();
    private float lastLockedPercentage = 0.0f;
    private int rowsDrawn = 10;
    private int columnsDrawn = 10;
    private int lcgSeed = 1;
    private const float LOADING_TIME = 3.5f;
    private const float WORKING_TIME = 10f;
    private const float COMPLETE_TIME = 3f;
    private const float ERROR_TIME = 6f;
    private Computer targetComputer;
    private Folder destFolder;
    private FileEntry targetFile;
    private string targetFilename;
    private string destFilename;

    public DecypherExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.IdentifierName = "Decypher Module";
      this.ramCost = 370;
      if (p.Length < 2)
      {
        this.status = DecypherExe.DecypherStatus.Error;
        this.errorMessage = "No File Provided";
      }
      else
      {
        this.InitializeFiles(p[1]);
        if (p.Length > 2)
          this.password = p[2];
      }
    }

    private void InitializeFiles(string filename)
    {
      this.targetComputer = this.os.thisComputer;
      if (this.os.connectedComp != null)
        this.targetComputer = this.os.connectedComp;
      this.destFolder = Programs.getCurrentFolder(this.os);
      this.targetFilename = filename;
      this.destFilename = filename.Replace(".dec", "[NUMBER][EXT]");
    }

    public override void Update(float t)
    {
      base.Update(t);
      this.timeOnThisPhase += t;
      float num;
      switch (this.status)
      {
        case DecypherExe.DecypherStatus.Error:
          num = 6f;
          if ((double) this.timeOnThisPhase >= 6.0)
          {
            this.isExiting = true;
            break;
          }
          break;
        case DecypherExe.DecypherStatus.Working:
          num = 10f;
          if ((double) this.timeOnThisPhase >= 10.0)
          {
            try
            {
              this.CompleteWorking();
            }
            catch (Exception ex)
            {
              this.status = DecypherExe.DecypherStatus.Error;
              this.timeOnThisPhase = 0.0f;
              this.errorMessage = LocaleTerms.Loc("Fatal error in decryption\nfile may be corrupt");
            }
            this.status = DecypherExe.DecypherStatus.Complete;
            this.timeOnThisPhase = 0.0f;
            break;
          }
          if ((double) this.percentComplete % 0.100000001490116 < 0.00999999977648258)
          {
            this.lastLockedPercentage = this.percentComplete;
            this.lcgSeed = Utils.random.Next();
            this.rowsActive.Clear();
            this.columnsActive.Clear();
            for (int index = 0; index < this.columnsDrawn; ++index)
            {
              if (Utils.random.NextDouble() < 0.2)
                this.rowsActive.Add(index);
            }
            for (int index = 0; index < this.rowsDrawn; ++index)
            {
              if (Utils.random.NextDouble() < 0.2)
                this.columnsActive.Add(index);
            }
          }
          break;
        case DecypherExe.DecypherStatus.Complete:
          num = 3f;
          if ((double) this.timeOnThisPhase >= 3.0)
          {
            this.isExiting = true;
            break;
          }
          break;
        default:
          num = 3.5f;
          if ((double) this.timeOnThisPhase >= 3.5)
          {
            if (this.CompleteLoading())
              this.status = DecypherExe.DecypherStatus.Working;
            this.timeOnThisPhase = 0.0f;
            break;
          }
          break;
      }
      this.percentComplete = this.timeOnThisPhase / num;
    }

    private bool CompleteLoading()
    {
      try
      {
        this.targetFile = this.destFolder.searchForFile(this.targetFilename);
        if (this.targetFile == null)
        {
          this.status = DecypherExe.DecypherStatus.Error;
          this.errorMessage = LocaleTerms.Loc("File not found");
          return false;
        }
        switch (FileEncrypter.FileIsEncrypted(this.targetFile.data, this.password))
        {
          case 0:
            this.status = DecypherExe.DecypherStatus.Error;
            this.errorMessage = LocaleTerms.Loc("File is not\nDEC encrypted");
            return false;
          case 2:
            this.status = DecypherExe.DecypherStatus.Error;
            this.errorMessage = !(this.password == "") ? LocaleTerms.Loc("Provided Password\nIs Incorrect") : LocaleTerms.Loc("This File Requires\n a Password");
            return false;
          default:
            return true;
        }
      }
      catch (Exception ex)
      {
        this.status = DecypherExe.DecypherStatus.Error;
        this.errorMessage = LocaleTerms.Loc("Fatal error in loading");
        return false;
      }
    }

    private void CompleteWorking()
    {
      string[] strArray = FileEncrypter.DecryptString(this.targetFile.data, this.password);
      if (!this.destFilename.Contains("[NUMBER]"))
        this.destFilename += "[NUMBER]";
      string str1 = this.destFilename.Replace("[NUMBER]", "");
      string str2 = strArray[3] == null ? str1.Replace("[EXT]", ".txt") : str1.Replace("[EXT]", strArray[3]);
      if (this.destFolder.containsFile(str2))
      {
        int num = 1;
        do
        {
          str2 = this.destFilename.Replace("[NUMBER]", "(" + (object) num + ")");
          ++num;
        }
        while (this.destFolder.containsFile(str2));
      }
      FileEntry fileEntry = new FileEntry(strArray[2], str2);
      this.writtenFilename = str2;
      this.destFolder.files.Add(fileEntry);
      this.os.write("Decryption complete - file " + this.targetFilename + " decrypted to target file " + this.targetFilename);
      this.os.write("Encryption Header    : \"" + strArray[0] + "\"");
      this.os.write("Encryption Source IP: \"" + strArray[1] + "\"");
      this.displayHeader = strArray[0];
      this.displayIP = strArray[1];
    }

    private Rectangle DrawLoadingMessage(string message, float startPoint, Rectangle dest, bool showLoading = true)
    {
      float num1 = 0.18f;
      float num2 = (this.percentComplete - startPoint) / num1;
      if ((double) this.percentComplete > (double) startPoint)
      {
        dest.Y += 22;
        this.spriteBatch.Draw(Utils.white, dest, Color.Black);
        this.spriteBatch.DrawString(GuiData.tinyfont, message, new Vector2((float) (this.bounds.X + 6), (float) (dest.Y + 2)), Color.White);
        if (showLoading)
        {
          if ((double) this.percentComplete > (double) startPoint + (double) num1)
            this.spriteBatch.DrawString(GuiData.tinyfont, LocaleTerms.Loc("COMPLETE"), new Vector2((float) (this.bounds.X + 172), (float) (dest.Y + 2)), Color.DarkGreen);
          else
            this.spriteBatch.DrawString(GuiData.tinyfont, (num2 * 100f).ToString("00") + "%", new Vector2((float) (this.bounds.X + 195), (float) (dest.Y + 2)), Color.White);
        }
      }
      return dest;
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.status.ToString();
      switch (this.status)
      {
        case DecypherExe.DecypherStatus.Error:
          PatternDrawer.draw(this.bounds, 1.2f, Color.Transparent, this.os.lockedColor, this.spriteBatch, PatternDrawer.errorTile);
          Rectangle destinationRectangle1 = new Rectangle(this.bounds.X + 1, this.bounds.Y + 20, 200, 50);
          this.spriteBatch.Draw(Utils.white, destinationRectangle1, this.os.lockedColor);
          this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("ERROR"), new Vector2((float) (this.bounds.X + 6), (float) (this.bounds.Y + 24)), Color.White);
          destinationRectangle1.Y += 50;
          destinationRectangle1.Height = 80;
          destinationRectangle1.Width = 250;
          this.spriteBatch.Draw(Utils.white, destinationRectangle1, Color.Black);
          this.spriteBatch.DrawString(GuiData.smallfont, this.errorMessage, new Vector2((float) (this.bounds.X + 6), (float) (this.bounds.Y + 74)), Color.White);
          break;
        case DecypherExe.DecypherStatus.Loading:
          PatternDrawer.draw(this.bounds, 1.2f, Color.Transparent, this.os.highlightColor * 0.2f, this.spriteBatch, PatternDrawer.thinStripe);
          Rectangle rectangle1 = new Rectangle(this.bounds.X + 1, this.bounds.Y + 20, 200, 50);
          this.spriteBatch.Draw(Utils.white, rectangle1, Color.Black);
          this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Loading..."), new Vector2((float) (this.bounds.X + 6), (float) (this.bounds.Y + 24)), Color.White);
          rectangle1.Height = 20;
          rectangle1.Width = 240;
          rectangle1.Y += 28;
          this.DrawLoadingMessage("Reading Codes...", 0.72f, this.DrawLoadingMessage("Checking DEC...", 0.54f, this.DrawLoadingMessage("Verifying Content...", 0.36f, this.DrawLoadingMessage("Parsing Headers...", 0.18f, this.DrawLoadingMessage("Reading File...", 0.0f, rectangle1, true), true), true), true), true);
          break;
        case DecypherExe.DecypherStatus.Working:
          Rectangle destinationRectangle2 = new Rectangle(this.bounds.X + 1, this.bounds.Y + 1, 200, 40);
          this.spriteBatch.Draw(Utils.white, destinationRectangle2, Color.Black);
          this.spriteBatch.DrawString(GuiData.font, "Decode:: " + (this.percentComplete * 100f).ToString("00.0") + " %", new Vector2((float) (this.bounds.X + 2), (float) (this.bounds.Y + 2)), Color.White);
          Rectangle rectangle2 = new Rectangle(this.bounds.X, this.bounds.Y + 50, this.bounds.Width, this.bounds.Height - 50);
          int x = rectangle2.X;
          int num1 = 12;
          int y1 = rectangle2.Y;
          int num2 = 13;
          int num3 = 0;
          int num4 = 0;
          int num5 = 0;
          Utils.LCG.reSeed(this.lcgSeed);
          while (x + num1 < rectangle2.X + rectangle2.Width)
          {
            int y2 = rectangle2.Y;
            num5 = 0;
            while (y2 + num2 < rectangle2.Y + rectangle2.Height)
            {
              bool flag = this.rowsActive.Contains(num4) || this.columnsActive.Contains(num5);
              char ch = this.targetFile.data[(int) (((flag ? (double) this.percentComplete : (double) this.lastLockedPercentage) % 10.0 * (double) this.targetFile.data.Length * 3.0 + (double) (num3 * 222)) % (double) this.targetFile.data.Length)];
              this.spriteBatch.DrawString(GuiData.UITinyfont, string.Concat((object) ch), new Vector2((float) x, (float) y2), Color.Lerp(Color.White, this.os.highlightColor, flag ? Utils.randm(1f) : Utils.LCG.NextFloat()));
              y2 += num2;
              ++num3;
              ++num5;
            }
            x += num1 + 2;
            ++num4;
          }
          this.columnsDrawn = num4;
          this.rowsDrawn = num5;
          break;
        case DecypherExe.DecypherStatus.Complete:
          PatternDrawer.draw(this.bounds, 1.2f, Color.Transparent, this.os.highlightColor * 0.2f, this.spriteBatch, PatternDrawer.thinStripe);
          if (this.bounds.Height <= 60)
            break;
          Rectangle rectangle3 = new Rectangle(this.bounds.X + 1, this.bounds.Y + 20, 200, 85);
          this.spriteBatch.Draw(Utils.white, rectangle3, Color.Black);
          this.spriteBatch.DrawString(GuiData.font, "Operation\nComplete", new Vector2((float) (this.bounds.X + 6), (float) (this.bounds.Y + 24)), Color.White);
          rectangle3.Height = 20;
          rectangle3.Width = 240;
          rectangle3.Y += 63;
          if (rectangle3.Y + rectangle3.Height < this.bounds.Y + this.bounds.Height)
            rectangle3 = this.DrawLoadingMessage(LocaleTerms.Loc("Headers:"), 0.0f, rectangle3, false);
          if (rectangle3.Y + rectangle3.Height < this.bounds.Y + this.bounds.Height)
            rectangle3 = this.DrawLoadingMessage("\"" + this.displayHeader + "\"", 0.1f, rectangle3, false);
          if (rectangle3.Y + rectangle3.Height < this.bounds.Y + this.bounds.Height)
            rectangle3 = this.DrawLoadingMessage(LocaleTerms.Loc("Source IP:"), 0.3f, rectangle3, false);
          if (rectangle3.Y + rectangle3.Height < this.bounds.Y + this.bounds.Height)
            rectangle3 = this.DrawLoadingMessage("\"" + this.displayIP + "\"", 0.4f, rectangle3, false);
          if (rectangle3.Y + rectangle3.Height < this.bounds.Y + this.bounds.Height)
            rectangle3 = this.DrawLoadingMessage(LocaleTerms.Loc("Output File:"), 0.6f, rectangle3, false);
          if (rectangle3.Y + rectangle3.Height < this.bounds.Y + this.bounds.Height)
            rectangle3 = this.DrawLoadingMessage("\"" + this.writtenFilename + "\"", 0.7f, rectangle3, false);
          if (rectangle3.Y + rectangle3.Height < this.bounds.Y + this.bounds.Height)
            rectangle3 = this.DrawLoadingMessage(LocaleTerms.Loc("Operation Complete"), 0.9f, rectangle3, false);
          if (rectangle3.Y + rectangle3.Height < this.bounds.Y + this.bounds.Height)
            rectangle3 = this.DrawLoadingMessage(LocaleTerms.Loc("Shutting Down"), 0.95f, rectangle3, false);
          break;
      }
    }

    private enum DecypherStatus
    {
      Error,
      Loading,
      Working,
      Complete,
    }
  }
}
