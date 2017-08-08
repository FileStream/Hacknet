// Decompiled with JetBrains decompiler
// Type: Hacknet.ISPDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class ISPDaemon : Daemon
  {
    private List<ISPDaemon.ExpandingRectangleData> outlineEffectEntries = new List<ISPDaemon.ExpandingRectangleData>();
    private float lastTimer = 0.0f;
    private float timeEnteredLoadingScreen = 0.0f;
    private ISPDaemon.ISPDaemonState state = ISPDaemon.ISPDaemonState.Welcome;
    private string ipSearch = (string) null;
    private Computer scannedComputer = (Computer) null;
    private bool inspectionFlagged = false;
    private const float MAX_WIDTH = 7f;
    private const float MAX_RATE = 16f;
    private const float EFFECT_TIMER = 30f;
    private const float SEARCH_TIME = 2f;
    private const string ABOUT_MESSAGE_FILE = "ISP_About_Message.txt";

    public ISPDaemon(Computer c, OS os)
      : base(c, LocaleTerms.Loc("ISP Management System"), os)
    {
    }

    public override void initFiles()
    {
      base.initFiles();
      Folder folder = this.comp.files.root.searchForFolder("home");
      if (folder == null)
        return;
      FileEntry fileEntry = new FileEntry(Utils.readEntireFile("Content/LocPost/ISPAbout.txt"), "ISP_About_Message.txt");
      folder.files.Add(fileEntry);
    }

    public override void navigatedTo()
    {
      this.state = ISPDaemon.ISPDaemonState.Welcome;
      base.navigatedTo();
    }

    public override string getSaveString()
    {
      return "<ispSystem />";
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      if (this.state == ISPDaemon.ISPDaemonState.AdminOnlyError || this.state == ISPDaemon.ISPDaemonState.NotFoundError)
        PatternDrawer.draw(bounds, 0.1f, Color.Transparent, this.os.lockedColor, sb, PatternDrawer.errorTile);
      Rectangle bounds1 = new Rectangle(bounds.X + 40, bounds.Y + 40, bounds.Width - 80, bounds.Height - 80);
      switch (this.state)
      {
        case ISPDaemon.ISPDaemonState.Welcome:
          this.DrawWelcomeScreen(bounds1, sb, bounds);
          break;
        case ISPDaemon.ISPDaemonState.About:
          this.DrawAboutScreen(bounds1, sb);
          break;
        case ISPDaemon.ISPDaemonState.Loading:
          this.DrawLoadingScreen(bounds1, sb);
          break;
        case ISPDaemon.ISPDaemonState.IPEntry:
          this.DrawIPEntryScreen(bounds1, sb);
          break;
        case ISPDaemon.ISPDaemonState.EnterIP:
          this.DrawEnterIPScreen(bounds1, sb);
          break;
        case ISPDaemon.ISPDaemonState.AdminOnlyError:
          this.DrawAdminOnlyError(bounds1, sb);
          break;
        case ISPDaemon.ISPDaemonState.NotFoundError:
          this.DrawNotFoundError(bounds1, sb);
          break;
      }
      this.drawOutlineEffect(bounds, sb);
    }

    private void DrawIPEntryScreen(Rectangle bounds, SpriteBatch sb)
    {
      TextItem.doFontLabel(new Vector2((float) (bounds.X + 30), (float) (bounds.Y + 20)), LocaleTerms.Loc("IP Entry") + " :: " + this.scannedComputer.ip, GuiData.font, new Color?(Color.White), (float) bounds.Width, float.MaxValue, false);
      TextItem.doFontLabel(new Vector2((float) (bounds.X + 30), (float) (bounds.Y + 70)), LocaleTerms.Loc("Identified as") + " :\"" + this.scannedComputer.name + "\"", GuiData.smallfont, new Color?(Color.White), (float) bounds.Width, float.MaxValue, false);
      sb.Draw(this.os.display.GetComputerImage(this.scannedComputer), new Vector2((float) (bounds.X + 30), (float) (bounds.Y + 100)), Color.White);
      int x = bounds.X + 30 + 130;
      int y1 = bounds.Y + 100;
      if (Button.doButton(3388301, x, y1, bounds.Width - 180, 30, LocaleTerms.Loc("Assign New IP"), new Color?(this.os.brightUnlockedColor)))
      {
        bool flag;
        do
        {
          this.scannedComputer.ip = NetworkMap.generateRandomIP();
          flag = false;
          for (int index = 0; index < this.os.netMap.nodes.Count; ++index)
          {
            if (this.os.netMap.nodes[index].ip == this.scannedComputer.ip && this.os.netMap.nodes[index].idName != this.scannedComputer.idName)
              flag = true;
          }
        }
        while (flag);
        if (this.os.thisComputer.idName == this.scannedComputer.idName)
          this.os.thisComputerIPReset();
      }
      int y2 = y1 + 34;
      if (Button.doButton(3388304, x, y2, bounds.Width - 180, 30, LocaleTerms.Loc("Flag for Inspection") + (this.inspectionFlagged ? " : " + LocaleTerms.Loc("ACTIVE") : ""), new Color?(this.inspectionFlagged ? Color.Gray : this.os.highlightColor)))
        this.inspectionFlagged = true;
      int y3 = y2 + 34;
      if (!Button.doButton(3388308, x, y3, bounds.Width - 180, 30, LocaleTerms.Loc("Prioritize Routing"), new Color?(this.os.highlightColor)))
        ;
      this.drawBackButton(bounds, ISPDaemon.ISPDaemonState.Welcome);
    }

    private void DrawLoadingScreen(Rectangle bounds, SpriteBatch sb)
    {
      float num1 = this.os.timer - this.timeEnteredLoadingScreen;
      if ((double) num1 >= 2.0)
      {
        this.scannedComputer = (Computer) null;
        for (int index = 0; index < this.os.netMap.nodes.Count; ++index)
        {
          if (this.ipSearch == this.os.netMap.nodes[index].ip)
          {
            this.scannedComputer = this.os.netMap.nodes[index];
            break;
          }
        }
        if (this.scannedComputer != null)
        {
          this.state = ISPDaemon.ISPDaemonState.IPEntry;
          this.inspectionFlagged = false;
        }
        else
          this.state = ISPDaemon.ISPDaemonState.NotFoundError;
      }
      TextItem.doFontLabel(new Vector2((float) (bounds.X + 30), (float) (bounds.Y + bounds.Height / 2 - 35)), LocaleTerms.Loc("Scanning for") + " " + this.ipSearch + " ...", GuiData.font, new Color?(Color.White), (float) bounds.Width, float.MaxValue, false);
      Rectangle destinationRectangle = new Rectangle(bounds.X + 10, bounds.Y + bounds.Height / 2, bounds.Width - 20, 16);
      sb.Draw(Utils.white, destinationRectangle, Color.Gray);
      ++destinationRectangle.X;
      ++destinationRectangle.Y;
      destinationRectangle.Width -= 2;
      destinationRectangle.Height -= 2;
      sb.Draw(Utils.white, destinationRectangle, Color.Black);
      float num2 = Utils.QuadraticOutCurve(num1 / 2f);
      destinationRectangle.Width = (int) ((double) destinationRectangle.Width * (double) num2);
      sb.Draw(Utils.white, destinationRectangle, this.os.highlightColor);
    }

    private void DrawEnterIPScreen(Rectangle bounds, SpriteBatch sb)
    {
      string[] strArray = this.os.getStringCache.Split(new string[1]{ "#$#$#$$#$&$#$#$#$#" }, StringSplitOptions.None);
      string str = (string) null;
      int x = bounds.X + 10;
      int num1 = bounds.Y + 10;
      Vector2 vector2_1 = TextItem.doMeasuredSmallLabel(new Vector2((float) x, (float) num1), LocaleTerms.Loc("Enter IP Address to Scan for") + " :", new Color?());
      int y1 = num1 + ((int) vector2_1.Y + 5);
      if (strArray.Length > 1)
      {
        str = strArray[1];
        if (str.Equals(""))
          str = this.os.terminal.currentLine;
      }
      Rectangle destinationRectangle = new Rectangle(x, y1, bounds.Width - 20, 200);
      sb.Draw(Utils.white, destinationRectangle, this.os.darkBackgroundColor);
      int num2 = y1 + 80;
      Vector2 vector2_2 = TextItem.doMeasuredSmallLabel(new Vector2((float) x, (float) num2), LocaleTerms.Loc("IP Address") + ": " + str, new Color?());
      destinationRectangle.X = x + (int) vector2_2.X + 2;
      destinationRectangle.Y = num2;
      destinationRectangle.Width = 7;
      destinationRectangle.Height = 20;
      if ((double) this.os.timer % 1.0 < 0.300000011920929)
        sb.Draw(Utils.white, destinationRectangle, this.os.outlineColor);
      int y2 = num2 + 122;
      if (strArray.Length > 2 || Button.doButton(30, x, y2, 300, 22, LocaleTerms.Loc("Scan"), new Color?(this.os.highlightColor)))
      {
        if (strArray.Length <= 2)
          this.os.terminal.executeLine();
        this.ipSearch = str;
        this.state = ISPDaemon.ISPDaemonState.Loading;
        this.timeEnteredLoadingScreen = this.os.timer;
        this.os.getStringCache = "";
      }
      int y3 = y2 + 26;
      if (!Button.doButton(35, x, y3, 300, 22, LocaleTerms.Loc("Cancel"), new Color?(this.os.lockedColor)))
        return;
      this.os.terminal.executeLine();
      this.state = ISPDaemon.ISPDaemonState.Welcome;
      this.os.getStringCache = "";
    }

    private void drawBackButton(Rectangle bounds, ISPDaemon.ISPDaemonState stateTo = ISPDaemon.ISPDaemonState.Welcome)
    {
      if (!Button.doButton(92271094, bounds.X + 30, bounds.Y + bounds.Height - 50, 200, 25, LocaleTerms.Loc("Back"), new Color?(this.os.lockedColor)))
        return;
      this.state = stateTo;
    }

    private void DrawAdminOnlyError(Rectangle bounds, SpriteBatch sb)
    {
      TextItem.doFontLabel(new Vector2((float) (bounds.X + 30), (float) (bounds.Y + 20)), LocaleTerms.Loc("Insufficient Permissions"), GuiData.font, new Color?(Color.White), (float) bounds.Width, float.MaxValue, false);
      TextItem.doFontLabel(new Vector2((float) (bounds.X + 30), (float) (bounds.Y + 70)), LocaleTerms.Loc("IP Search is limited to administrators") + "\n" + LocaleTerms.Loc("of this machine only") + ".\n" + LocaleTerms.Loc("If you require access, contact customer support") + ".", GuiData.smallfont, new Color?(Color.White), (float) bounds.Width, float.MaxValue, false);
      this.drawBackButton(bounds, ISPDaemon.ISPDaemonState.Welcome);
    }

    private void DrawNotFoundError(Rectangle bounds, SpriteBatch sb)
    {
      TextItem.doFontLabel(new Vector2((float) (bounds.X + 30), (float) (bounds.Y + 20)), LocaleTerms.Loc("IP Not Found"), GuiData.font, new Color?(Color.White), (float) bounds.Width, float.MaxValue, false);
      TextItem.doFontLabel(new Vector2((float) (bounds.X + 30), (float) (bounds.Y + 70)), LocaleTerms.Loc("IP Address is not registered with") + "\n" + LocaleTerms.Loc("our servers") + ".\n" + LocaleTerms.Loc("Check address and try again") + ".", GuiData.smallfont, new Color?(Color.White), (float) bounds.Width, float.MaxValue, false);
      this.drawBackButton(bounds, ISPDaemon.ISPDaemonState.Welcome);
    }

    private void DrawAboutScreen(Rectangle bounds, SpriteBatch sb)
    {
      TextItem.doFontLabel(new Vector2((float) (bounds.X + 30), (float) (bounds.Y + 20)), LocaleTerms.Loc("About this server"), GuiData.font, new Color?(Color.White), (float) bounds.Width, float.MaxValue, false);
      Folder folder = this.comp.files.root.searchForFolder("home");
      if (folder != null)
      {
        FileEntry fileEntry = folder.searchForFile("ISP_About_Message.txt");
        if (fileEntry != null)
        {
          TextItem.DrawShadow = false;
          TextItem.doFontLabel(new Vector2((float) (bounds.X + 30), (float) (bounds.Y + 70)), fileEntry.data, GuiData.smallfont, new Color?(Color.White), (float) bounds.Width, float.MaxValue, false);
        }
      }
      this.drawBackButton(bounds, ISPDaemon.ISPDaemonState.Welcome);
    }

    private void DrawWelcomeScreen(Rectangle bounds, SpriteBatch sb, Rectangle fullBounds)
    {
      TextItem.doFontLabel(new Vector2((float) (bounds.X + 20), (float) (bounds.Y + 20)), LocaleTerms.Loc("ISP Management System"), GuiData.font, new Color?(Color.White), (float) bounds.Width, float.MaxValue, false);
      int height = 30;
      int width = (int) ((double) bounds.Width * 0.800000011920929);
      int num = bounds.Y + 90;
      Rectangle destinationRectangle = fullBounds;
      destinationRectangle.Y = num - 20;
      destinationRectangle.Height = 22;
      bool flag = this.comp.adminIP == this.os.thisComputer.ip;
      sb.Draw(Utils.white, destinationRectangle, flag ? this.os.unlockedColor : this.os.lockedColor);
      string text = LocaleTerms.Loc("Valid Administrator Account Detected");
      if (!flag)
        text = LocaleTerms.Loc("Non-Admin Account Active");
      Vector2 vector2 = GuiData.smallfont.MeasureString(text);
      Vector2 position = new Vector2((float) (destinationRectangle.X + destinationRectangle.Width / 2) - vector2.X / 2f, (float) destinationRectangle.Y);
      sb.DrawString(GuiData.smallfont, text, position, Color.White);
      int y1 = num + 30;
      if (Button.doButton(95371001, bounds.X + 20, y1, width, height, LocaleTerms.Loc("About"), new Color?(this.os.highlightColor)))
        this.state = ISPDaemon.ISPDaemonState.About;
      int y2 = y1 + (height + 10);
      if (Button.doButton(95371004, bounds.X + 20, y2, width, height, LocaleTerms.Loc("Search for IP"), new Color?(this.os.highlightColor)))
      {
        if (this.comp.adminIP == this.os.thisComputer.ip)
        {
          this.state = ISPDaemon.ISPDaemonState.EnterIP;
          this.os.execute("getString IP_Address");
          this.ipSearch = (string) null;
        }
        else
          this.state = ISPDaemon.ISPDaemonState.AdminOnlyError;
      }
      int y3 = y2 + (height + 10);
      if (!Button.doButton(95371008, bounds.X + 20, y3, width, height, LocaleTerms.Loc("Exit"), new Color?(this.os.highlightColor)))
        return;
      this.os.display.command = "connect";
    }

    private void drawOutlineEffect(Rectangle bounds, SpriteBatch sb)
    {
      if ((double) this.lastTimer == 0.0)
        this.lastTimer = this.os.timer;
      if ((double) this.os.timer % 0.5 > 0.100000001490116 && (double) this.lastTimer % 0.5 < 0.100000001490116)
        this.addNewOutlineEffect();
      for (int index = 0; index < this.outlineEffectEntries.Count; ++index)
      {
        ISPDaemon.ExpandingRectangleData outlineEffectEntry = this.outlineEffectEntries[index];
        outlineEffectEntry.scaleIn += (float) (((double) this.os.timer - (double) this.lastTimer) * (double) outlineEffectEntry.rate * 1.0);
        if ((double) outlineEffectEntry.scaleIn > 30.0)
        {
          this.outlineEffectEntries.RemoveAt(index);
          --index;
        }
        else
        {
          this.drawRect(new Rectangle((int) ((double) bounds.X + (double) outlineEffectEntry.scaleIn), (int) ((double) bounds.Y + (double) outlineEffectEntry.scaleIn), (int) ((double) bounds.Width - 2.0 * (double) outlineEffectEntry.scaleIn), (int) ((double) bounds.Height - 2.0 * (double) outlineEffectEntry.scaleIn)), sb, (int) outlineEffectEntry.thickness, (float) (1.0 - (double) outlineEffectEntry.scaleIn / 30.0), outlineEffectEntry.blackLerp);
          this.outlineEffectEntries[index] = outlineEffectEntry;
        }
      }
      this.drawRect(bounds, sb, 4, 1f, 0.0f);
      this.lastTimer = this.os.timer;
    }

    private void drawRect(Rectangle rect, SpriteBatch sb, int thickness, float opacity, float blackLerp)
    {
      Rectangle destinationRectangle = rect;
      destinationRectangle.Width = thickness;
      Color color = Color.Lerp(this.os.highlightColor, Color.Black, blackLerp) * opacity;
      sb.Draw(Utils.white, destinationRectangle, color);
      destinationRectangle.X += rect.Width - thickness;
      sb.Draw(Utils.white, destinationRectangle, color);
      destinationRectangle.X = rect.X + thickness;
      destinationRectangle.Width = rect.Width - 2 * thickness;
      destinationRectangle.Height = thickness;
      sb.Draw(Utils.white, destinationRectangle, color);
      destinationRectangle.Y += rect.Height - thickness;
      sb.Draw(Utils.white, destinationRectangle, color);
    }

    private void addNewOutlineEffect()
    {
      this.outlineEffectEntries.Add(new ISPDaemon.ExpandingRectangleData()
      {
        rate = (float) ((double) Utils.randm(1f) * 15.0 + 1.0),
        scaleIn = 1f,
        thickness = (float) (1.0 + (double) Utils.randm(1f) * (double) Utils.randm(1f) * 6.0),
        blackLerp = Utils.randm(0.8f)
      });
    }

    private struct ExpandingRectangleData
    {
      public float scaleIn;
      public float rate;
      public float thickness;
      public float blackLerp;
    }

    private enum ISPDaemonState
    {
      Welcome,
      About,
      Loading,
      IPEntry,
      EnterIP,
      AdminOnlyError,
      NotFoundError,
    }
  }
}
