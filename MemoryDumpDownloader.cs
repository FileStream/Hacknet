// Decompiled with JetBrains decompiler
// Type: Hacknet.MemoryDumpDownloader
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class MemoryDumpDownloader : ExeModule
  {
    private bool DownloadComplete = false;
    private bool DidFail = false;
    private string savedFileName = "Unknown";
    private float elapsedTime = 0.0f;
    private const float DownloadTime = 6f;
    private const float FailTime = 2f;
    private const float ExitTime = 5f;
    private Computer target;

    private MemoryDumpDownloader(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.name = "MemDumpGenerator";
      this.ramCost = 80;
      this.IdentifierName = "MemoryDumpGenerator";
      this.target = this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp;
    }

    public static MemoryDumpDownloader GenerateInstanceOrNullFromArguments(string[] args, Rectangle location, object osObj, Computer target)
    {
      OS operatingSystem = (OS) osObj;
      if (operatingSystem.hasConnectionPermission(true))
        return new MemoryDumpDownloader(location, operatingSystem);
      operatingSystem.write(LocaleTerms.Loc("Admin access required to generate memory dump"));
      return (MemoryDumpDownloader) null;
    }

    public override void Update(float t)
    {
      base.Update(t);
      this.elapsedTime += t;
      if (this.DownloadComplete)
      {
        if ((double) this.elapsedTime <= 5.0)
          return;
        this.isExiting = true;
      }
      else if ((double) this.elapsedTime > 2.0 && this.target.Memory == null)
      {
        this.DownloadComplete = true;
        this.DidFail = true;
        this.elapsedTime = 0.0f;
      }
      else if ((double) this.elapsedTime > 6.0 && this.target.Memory != null)
      {
        this.DownloadComplete = true;
        this.DidFail = false;
        this.elapsedTime = 0.0f;
        this.DownloadMemoryDump();
      }
    }

    private void DownloadMemoryDump()
    {
      string encodedFileString = this.target.Memory.GetEncodedFileString();
      if (this.os.thisComputer == this.target)
      {
        List<string> stringList = new List<string>();
        for (int index = 0; index < this.os.exes.Count; ++index)
        {
          NotesExe ex = this.os.exes[index] as NotesExe;
          if (ex != null)
            stringList.AddRange((IEnumerable<string>) ex.notes);
        }
        encodedFileString = new MemoryContents()
        {
          DataBlocks = new List<string>((IEnumerable<string>) stringList.ToArray()),
          CommandsRun = this.os.terminal.GetRecentTerminalHistoryList()
        }.GetEncodedFileString();
      }
      string reportFilename = this.getReportFilename(this.target.name);
      Folder folder = this.os.thisComputer.files.root.searchForFolder("home");
      Folder f = folder.searchForFolder("MemDumps");
      if (f == null)
      {
        folder.folders.Add(new Folder("MemDumps"));
        f = folder.searchForFolder("MemDumps");
      }
      string repeatingFilename = Utils.GetNonRepeatingFilename(reportFilename, ".mem", f);
      f.files.Add(new FileEntry(encodedFileString, repeatingFilename));
      this.savedFileName = "home/MemDumps/" + repeatingFilename;
    }

    private string getReportFilename(string s)
    {
      return Utils.CleanStringToRenderable(s).Replace(" ", "_").ToLower() + "_dump";
    }

    public override void Completed()
    {
      base.Completed();
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      Rectangle contentAreaDest = this.GetContentAreaDest();
      if (contentAreaDest.Height < 5)
        return;
      PatternDrawer.draw(contentAreaDest, 0.1f, Color.Transparent, this.os.highlightColor * 0.1f, this.spriteBatch, PatternDrawer.wipTile);
      int height = Math.Min(contentAreaDest.Height / 2, 30);
      Rectangle rectangle = new Rectangle(contentAreaDest.X, contentAreaDest.Y + contentAreaDest.Height / 2 - height, contentAreaDest.Width, height);
      this.spriteBatch.Draw(Utils.white, rectangle, Color.Black);
      if (this.DownloadComplete)
      {
        this.spriteBatch.Draw(Utils.white, rectangle, this.DidFail ? Color.Red * 0.5f : this.os.highlightColor * 0.5f);
        TextItem.doFontLabelToSize(Utils.InsetRectangle(rectangle, 4), this.DidFail ? LocaleTerms.Loc("Empty Scan Detected") : LocaleTerms.Loc("Download Complete"), GuiData.smallfont, Utils.AddativeWhite, true, false);
        rectangle.Y += rectangle.Height;
        rectangle.Height -= 4;
        rectangle.X += 4;
        rectangle.Width -= 8;
        if (this.DidFail)
          return;
        TextItem.doFontLabelToSize(rectangle, this.savedFileName, GuiData.font, Utils.AddativeWhite, true, false);
      }
      else
      {
        float num = this.elapsedTime / 6f;
        rectangle.Width = (int) ((double) rectangle.Width * (double) num);
        this.spriteBatch.Draw(Utils.white, rectangle, this.os.highlightColor * 0.5f);
      }
    }
  }
}
