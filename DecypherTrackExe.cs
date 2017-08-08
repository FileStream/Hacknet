// Decompiled with JetBrains decompiler
// Type: Hacknet.DecypherTrackExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  internal class DecypherTrackExe : ExeModule
  {
    private static Color LoadingBarColorRed = new Color(196, 29, 60, 80);
    private static Color LoadingBarColorBlue = new Color(29, 113, 196, 80);
    private DecypherTrackExe.DecHeadStatus status = DecypherTrackExe.DecHeadStatus.Loading;
    private float timeOnThisPhase = 0.0f;
    private float percentComplete = 0.0f;
    private string errorMessage = "Unknown Error";
    private string displayHeader = "Unknown";
    private string displayIP = "Unknown";
    private const float LOADING_TIME = 3.5f;
    private const float COMPLETE_TIME = 10f;
    private const float ERROR_TIME = 6f;
    private Computer targetComputer;
    private Folder destFolder;
    private FileEntry targetFile;
    private string targetFilename;
    private string destFilename;

    public DecypherTrackExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.IdentifierName = "DEC File Tracer";
      this.ramCost = 240;
      if (p.Length < 2)
      {
        this.status = DecypherTrackExe.DecHeadStatus.Error;
        this.errorMessage = "No File Provided";
      }
      else
        this.InitializeFiles(p[1]);
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
        case DecypherTrackExe.DecHeadStatus.Error:
          num = 6f;
          if ((double) this.timeOnThisPhase >= 6.0)
          {
            this.isExiting = true;
            break;
          }
          break;
        case DecypherTrackExe.DecHeadStatus.Complete:
          num = 10f;
          if ((double) this.timeOnThisPhase >= 10.0)
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
            {
              this.status = DecypherTrackExe.DecHeadStatus.Complete;
              this.GetHeaders();
            }
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
          this.status = DecypherTrackExe.DecHeadStatus.Error;
          this.errorMessage = "File not found";
          return false;
        }
        if (FileEncrypter.FileIsEncrypted(this.targetFile.data, "") != 0)
          return true;
        this.status = DecypherTrackExe.DecHeadStatus.Error;
        this.errorMessage = "File is not\nDEC encrypted";
        return false;
      }
      catch (Exception ex)
      {
        this.status = DecypherTrackExe.DecHeadStatus.Error;
        this.errorMessage = "Fatal error in loading";
        return false;
      }
    }

    private void GetHeaders()
    {
      try
      {
        string[] strArray = FileEncrypter.DecryptHeaders(this.targetFile.data, "");
        this.displayHeader = strArray[0];
        this.displayIP = strArray[1];
        this.os.write(" \n \n---------------Header Analysis complete---------------\n \nDEC Encrypted File " + this.targetFilename + " headers:");
        this.os.write("Encryption Header    : \"" + this.displayHeader + "\"");
        this.os.write("Encryption Source IP: \"" + this.displayIP + "\"");
        this.os.write(" \n---------------------------------------------------------\n ");
      }
      catch (Exception ex)
      {
        this.os.write(" \n \n--------------- ERROR ---------------\n \n");
        this.os.write("Fatal Error: " + ex.GetType().ToString());
        this.os.write(ex.Message + "\n\n");
      }
    }

    private Rectangle DrawLoadingMessage(string message, float startPoint, Rectangle dest, bool showLoading = true, bool highlight = false)
    {
      float num1 = 0.18f;
      float num2 = (this.percentComplete - startPoint) / num1;
      if ((double) this.percentComplete > (double) startPoint)
      {
        dest.Y += 22;
        this.spriteBatch.Draw(Utils.white, dest, Color.Black);
        float point = (double) this.percentComplete <= (double) startPoint + (double) num1 ? num2 : 1f;
        dest.Width = (int) ((double) dest.Width * (double) Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(point)));
        this.spriteBatch.Draw(Utils.white, dest, this.status == DecypherTrackExe.DecHeadStatus.Complete ? DecypherTrackExe.LoadingBarColorBlue : DecypherTrackExe.LoadingBarColorRed);
        this.spriteBatch.DrawString(GuiData.tinyfont, message, new Vector2((float) (this.bounds.X + 6), (float) (dest.Y + 2)), highlight ? Color.Black : Color.White);
        if (showLoading)
        {
          if ((double) this.percentComplete > (double) startPoint + (double) num1)
            this.spriteBatch.DrawString(GuiData.tinyfont, LocaleTerms.Loc("COMPLETE"), new Vector2((float) (this.bounds.X + 172), (float) (dest.Y + 2)), Color.Black);
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
      Rectangle bounds = this.bounds;
      ++bounds.X;
      ++bounds.Y;
      bounds.Width -= 2;
      bounds.Height -= 2;
      this.status.ToString();
      switch (this.status)
      {
        case DecypherTrackExe.DecHeadStatus.Error:
          PatternDrawer.draw(bounds, 1.2f, Color.Transparent, this.os.lockedColor, this.spriteBatch, PatternDrawer.binaryTile);
          Rectangle destinationRectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + 20, 200, 50);
          if (this.bounds.Height > 120)
          {
            this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.lockedColor);
            this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("ERROR"), new Vector2((float) (this.bounds.X + 6), (float) (this.bounds.Y + 24)), Color.White);
          }
          destinationRectangle.Y += 50;
          destinationRectangle.Height = 80;
          destinationRectangle.Width = 250;
          if (this.bounds.Height <= 160)
            break;
          this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Black);
          this.spriteBatch.DrawString(GuiData.smallfont, this.errorMessage, new Vector2((float) (this.bounds.X + 6), (float) (this.bounds.Y + 74)), Color.White);
          break;
        case DecypherTrackExe.DecHeadStatus.Loading:
          PatternDrawer.draw(bounds, 1.2f, Color.Transparent, this.os.lockedColor, this.spriteBatch, PatternDrawer.binaryTile);
          Rectangle rectangle1 = new Rectangle(this.bounds.X + 1, this.bounds.Y + 20, 200, 50);
          this.spriteBatch.Draw(Utils.white, rectangle1, Color.Black);
          this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Loading") + "...", new Vector2((float) (this.bounds.X + 6), (float) (this.bounds.Y + 24)), Color.White);
          rectangle1.Height = 20;
          rectangle1.Width = 240;
          rectangle1.Y += 28;
          rectangle1 = this.DrawLoadingMessage("Reading File...", 0.0f, rectangle1, true, false);
          rectangle1 = this.DrawLoadingMessage("Parsing Headers...", 0.18f, rectangle1, true, false);
          rectangle1 = this.DrawLoadingMessage("Verifying Content...", 0.36f, rectangle1, true, false);
          rectangle1 = this.DrawLoadingMessage("Checking DEC...", 0.54f, rectangle1, true, false);
          rectangle1 = this.DrawLoadingMessage("Reading Codes...", 0.72f, rectangle1, true, false);
          break;
        case DecypherTrackExe.DecHeadStatus.Complete:
          PatternDrawer.draw(bounds, 1.2f, Color.Transparent, this.os.highlightColor * 0.5f, this.spriteBatch, PatternDrawer.binaryTile);
          if (this.bounds.Height <= 70)
            break;
          Rectangle rectangle2 = new Rectangle(this.bounds.X + 1, this.bounds.Y + 20, 200, 85);
          this.spriteBatch.Draw(Utils.white, rectangle2, Color.Black);
          this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Operation\nComplete"), new Vector2((float) (this.bounds.X + 6), (float) (this.bounds.Y + 24)), Color.White);
          rectangle2.Height = 20;
          rectangle2.Width = 240;
          rectangle2.Y += 63;
          if (rectangle2.Y + rectangle2.Height < this.bounds.Y + this.bounds.Height - 10)
            rectangle2 = this.DrawLoadingMessage("Headers:", 0.0f, rectangle2, false, false);
          if (rectangle2.Y + rectangle2.Height < this.bounds.Y + this.bounds.Height - 10)
            rectangle2 = this.DrawLoadingMessage("\"" + this.displayHeader + "\"", 0.1f, rectangle2, false, true);
          if (rectangle2.Y + rectangle2.Height < this.bounds.Y + this.bounds.Height - 10)
            rectangle2 = this.DrawLoadingMessage("Source IP:", 0.2f, rectangle2, false, false);
          if (rectangle2.Y + rectangle2.Height < this.bounds.Y + this.bounds.Height - 10)
            rectangle2 = this.DrawLoadingMessage("\"" + this.displayIP + "\"", 0.3f, rectangle2, false, true);
          break;
      }
    }

    private enum DecHeadStatus
    {
      Error,
      Loading,
      Complete,
    }
  }
}
