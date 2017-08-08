// Decompiled with JetBrains decompiler
// Type: Hacknet.DisplayModule
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Hacknet.Gui;
using Hacknet.Localization;
using Hacknet.Modules.Helpers;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hacknet
{
  internal class DisplayModule : CoreModule
  {
    public string command = "";
    public Folder LastDisplayedFileFolder = (Folder) null;
    public string LastDisplayedFileSourceIP = (string) null;
    private DisplayModuleLSHelper lsModuleHelper = new DisplayModuleLSHelper();
    private int errorCount = 0;
    private bool hasSentErrorEmail = false;
    private string invioableSecurityCacheString = (string) null;
    private float invioabilityCharChangeTimer = 0.0f;
    private string loginDetailsCache = (string) null;
    private bool lockLoginDisplayCache = false;
    private const int MAX_DISPLAY_STRING_LENGTH = 6000;
    public string[] commandArgs;
    private int x;
    private int y;
    private Rectangle tmpRect;
    private List<Texture2D> computers;
    private Dictionary<string, Texture2D> compAltIcons;
    private Texture2D defaultComputer;
    private Texture2D lockSprite;
    private Texture2D openLockSprite;
    private Texture2D fancyCornerSprite;
    private Texture2D fancyPanelSprite;
    private ScrollableTextRegion catTextRegion;
    private Vector2 scroll;
    private Vector2 catScroll;

    public DisplayModule(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.catTextRegion = new ScrollableTextRegion(GuiData.spriteBatch.GraphicsDevice);
    }

    public override void LoadContent()
    {
      this.computers = new List<Texture2D>();
      this.computers.Add(TextureBank.load("Sprites/CompLogos/Sec0Computer", this.os.content));
      this.computers.Add(TextureBank.load("Sprites/CompLogos/Sec1Computer", this.os.content));
      this.computers.Add(TextureBank.load("Sprites/CompLogos/Computer", this.os.content));
      this.computers.Add(TextureBank.load("Sprites/CompLogos/OldServer", this.os.content));
      this.computers.Add(TextureBank.load("Sprites/CompLogos/Sec2Computer", this.os.content));
      this.computers.Add(TextureBank.load("Sprites/CompLogos/Sec2Computer", this.os.content));
      this.compAltIcons = new Dictionary<string, Texture2D>();
      this.compAltIcons.Add("laptop", TextureBank.load("Sprites/CompLogos/Laptop", this.os.content));
      this.compAltIcons.Add("chip", TextureBank.load("Sprites/CompLogos/Chip", this.os.content));
      this.compAltIcons.Add("kellis", TextureBank.load("Sprites/CompLogos/KellisCompIcon", this.os.content));
      this.compAltIcons.Add("tablet", TextureBank.load("Sprites/CompLogos/Tablet", this.os.content));
      this.compAltIcons.Add("ePhone", TextureBank.load("Sprites/CompLogos/Phone1", this.os.content));
      this.compAltIcons.Add("ePhone2", TextureBank.load("Sprites/CompLogos/Phone2", this.os.content));
      if (DLC1SessionUpgrader.HasDLC1Installed)
      {
        this.compAltIcons.Add("Psylance", TextureBank.load("DLC/Sprites/Psylance", this.os.content));
        this.compAltIcons.Add("PacificAir", TextureBank.load("DLC/Sprites/PacificAir", this.os.content));
        this.compAltIcons.Add("Alchemist", TextureBank.load("DLC/Sprites/AlchemistsIcon", this.os.content));
        this.compAltIcons.Add("DLCLaptop", TextureBank.load("DLC/Icons/Laptop", this.os.content));
        this.compAltIcons.Add("DLCPC1", TextureBank.load("DLC/Icons/PC1", this.os.content));
        this.compAltIcons.Add("DLCPC2", TextureBank.load("DLC/Icons/PC2", this.os.content));
        this.compAltIcons.Add("DLCServer", TextureBank.load("DLC/Icons/Server", this.os.content));
      }
      this.defaultComputer = TextureBank.load("Sprites/CompLogos/Computer", this.os.content);
      this.lockSprite = TextureBank.load("Lock", this.os.content);
      this.openLockSprite = TextureBank.load("OpenLock", this.os.content);
      this.fancyCornerSprite = TextureBank.load("Corner", this.os.content);
      this.fancyPanelSprite = TextureBank.load("Panel", this.os.content);
      this.tmpRect = new Rectangle();
      this.scroll = Vector2.Zero;
      this.catScroll = Vector2.Zero;
    }

    public override void Update(float t)
    {
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      try
      {
        this.doCommandModule();
        this.errorCount = 0;
      }
      catch (Exception ex)
      {
        string reportFromException = Utils.GenerateReportFromException(ex);
        Console.WriteLine(reportFromException);
        if (OS.DEBUG_COMMANDS)
        {
          this.os.write("ERROR RENDERING DAEMON: " + this.command);
          this.os.write(Utils.GenerateReportFromExceptionCompact(ex));
        }
        ++this.errorCount;
        if (this.errorCount >= 3)
        {
          if (!this.hasSentErrorEmail)
          {
            Utils.SendThreadedErrorReport(ex, LocaleTerms.Loc("Display module Crash: ") + this.command, DebugLog.GetDump());
            this.hasSentErrorEmail = true;
          }
          this.command = "connect";
          this.commandArgs = new string[1];
          this.commandArgs[0] = "connect";
          this.errorCount = 0;
        }
        else
          Utils.AppendToErrorFile(reportFromException);
      }
    }

    public void typeChanged()
    {
      this.scroll = Vector2.Zero;
      this.catScroll = Vector2.Zero;
      Computer computer = this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp;
    }

    private void doCommandModule()
    {
      this.x = this.bounds.X + 5;
      this.y = this.bounds.Y + 5;
      this.spriteBatch.Draw(Utils.white, this.bounds, this.os.displayModuleExtraLayerBackingColor);
      if (!this.command.Equals("login"))
      {
        this.lockLoginDisplayCache = false;
        this.loginDetailsCache = (string) null;
      }
      for (int index = 0; index < this.os.exes.Count; ++index)
      {
        MainDisplayOverrideEXE ex = this.os.exes[index] as MainDisplayOverrideEXE;
        if (ex != null && ex.DisplayOverrideIsActive && !this.os.exes[index].isExiting)
        {
          ex.RenderMainDisplay(this.bounds, this.spriteBatch);
          return;
        }
      }
      if (this.os.connectedComp != null && this.os.connectedComp.getDaemon(typeof (PorthackHeartDaemon)) is PorthackHeartDaemon)
        this.doDaemonDisplay();
      else if (this.command.Equals("ls") || this.command.Equals("dir") || this.command.Equals("cd"))
        this.doLsDisplay();
      else if (this.command.Equals("connect"))
      {
        this.doConnectDisplay();
        this.invioableSecurityCacheString = (string) null;
      }
      else if (this.command.Equals("cat") || this.command.Equals("less"))
        this.doCatDisplay();
      else if (this.command.ToLower().Equals("probe") || this.command.Equals("nmap"))
        this.doProbeDisplay();
      else if (this.command.Equals("dc") || this.command.Equals("disconnect") || this.command.Equals("crash"))
        this.doDisconnectDisplay();
      else if (this.command.Equals("login"))
        this.doLoginDisplay();
      else if (this.command.Equals("connectiondenied"))
        this.doDisconnectForcedDisplay();
      else
        this.doDaemonDisplay();
    }

    private void doDaemonDisplay()
    {
      Computer computer = this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp;
      for (int index = 0; index < computer.daemons.Count; ++index)
      {
        if (computer.daemons[index].name.Equals(this.command) || computer.daemons[index] is PorthackHeartDaemon)
        {
          computer.daemons[index].draw(this.bounds, this.spriteBatch);
          break;
        }
      }
    }

    private void doLoginDisplay()
    {
      string[] separator = new string[1]{ "#$#$#$$#$&$#$#$#$#" };
      if (!this.lockLoginDisplayCache)
        this.loginDetailsCache = this.os.displayCache;
      string[] strArray = this.loginDetailsCache.Split(separator, StringSplitOptions.None);
      string text1 = "";
      string text2 = "";
      int num1 = -1;
      int num2 = 0;
      if (strArray[0].Equals("loginData"))
      {
        text1 = !(strArray[1] != "") ? this.os.terminal.currentLine : strArray[1];
        if (strArray.Length > 2)
        {
          num2 = 1;
          text2 = strArray[2];
          if (text2.Equals(""))
          {
            for (int index = 0; index < this.os.terminal.currentLine.Length; ++index)
              text2 += "*";
          }
          else
          {
            string str = "";
            for (int index = 0; index < text2.Length; ++index)
              str += "*";
            text2 = str;
          }
        }
        if (strArray.Length > 3)
        {
          num2 = 2;
          num1 = Convert.ToInt32(strArray[3]);
        }
      }
      this.doConnectHeader();
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = this.bounds.X + 2;
      tmpRect.Y = this.y;
      tmpRect.Height = 200;
      tmpRect.Width = this.bounds.Width - 4;
      this.spriteBatch.Draw(Utils.white, tmpRect, num1 == 0 ? this.os.lockedColor : this.os.indentBackgroundColor);
      if (num1 != 0)
        ;
      tmpRect.Height = 22;
      this.y += 30;
      Vector2 vector2 = TextItem.doMeasuredLabel(new Vector2((float) this.x, (float) this.y), LocaleTerms.Loc("Login "), new Color?(Color.White));
      if (num1 == 0)
      {
        this.x += (int) vector2.X;
        TextItem.doLabel(new Vector2((float) this.x, (float) this.y), LocaleTerms.Loc("Failed"), new Color?(this.os.brightLockedColor));
        this.x -= (int) vector2.X;
        this.lockLoginDisplayCache = true;
      }
      else if (num1 != -1)
      {
        this.x += (int) vector2.X;
        TextItem.doLabel(new Vector2((float) this.x, (float) this.y), LocaleTerms.Loc("Successful"), new Color?(this.os.brightUnlockedColor));
        this.x -= (int) vector2.X;
        this.lockLoginDisplayCache = true;
      }
      this.y += 60;
      if (num2 == 0)
      {
        tmpRect.Y = this.y;
        this.spriteBatch.Draw(Utils.white, tmpRect, this.os.subtleTextColor);
      }
      int num3 = 100;
      string text3 = LocaleTerms.Loc("username :");
      string text4 = LocaleTerms.Loc("password :");
      int num4 = (int) Math.Max(Math.Max(GuiData.smallfont.MeasureString(text3).X + 4f, GuiData.smallfont.MeasureString(text4).X + 4f), (float) num3);
      this.spriteBatch.DrawString(GuiData.smallfont, text3, new Vector2((float) this.x, (float) this.y), Color.White);
      this.x += num4;
      this.spriteBatch.DrawString(GuiData.smallfont, text1, new Vector2((float) this.x, (float) this.y), Color.White);
      this.x -= num4;
      this.y += 30;
      if (num2 == 1)
      {
        tmpRect.Y = this.y;
        this.spriteBatch.Draw(Utils.white, tmpRect, this.os.subtleTextColor);
      }
      this.spriteBatch.DrawString(GuiData.smallfont, text4, new Vector2((float) this.x, (float) this.y), Color.White);
      this.x += num4;
      this.spriteBatch.DrawString(GuiData.smallfont, text2, new Vector2((float) this.x, (float) this.y), Color.White);
      this.y += 30;
      this.x -= num4;
      if (num1 != -1)
      {
        int width = Math.Min(this.bounds.Width / 3, 180);
        if (Button.doButton(12345, this.x, this.y, num1 > 0 ? 140 : width, 30, num1 > 0 ? LocaleTerms.Loc("Complete") : LocaleTerms.Loc("Back"), new Color?(this.os.indentBackgroundColor)))
          this.command = "connect";
        if (num1 > 0 || !Button.doButton(123456, this.x + width + 5, this.y, width, 30, LocaleTerms.Loc("Retry"), new Color?(this.os.indentBackgroundColor)))
          return;
        this.lockLoginDisplayCache = false;
        this.loginDetailsCache = (string) null;
        this.os.runCommand("login");
      }
      else
      {
        this.y += 65;
        int x = this.x;
        int y = this.y;
        Computer computer = this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp;
        for (int index = 0; index < computer.users.Count; ++index)
        {
          if (computer.users[index].known && Daemon.validUser(computer.users[index].type))
          {
            x = this.x + 320;
            if (Button.doButton(123457 + index, this.x, this.y, 300, 25, "Login - User: " + computer.users[index].name + " Pass: " + computer.users[index].pass, new Color?(this.os.darkBackgroundColor)))
              this.forceLogin(computer.users[index].name, computer.users[index].pass);
            this.y += 27;
          }
        }
        if (Button.doButton(2111844, x, y, 100, 25, LocaleTerms.Loc("Cancel"), new Color?(this.os.lockedColor)))
        {
          this.forceLogin("", "");
          this.command = "connect";
        }
      }
    }

    public void forceLogin(string username, string pass)
    {
      string prompt = this.os.terminal.prompt;
      this.os.terminal.currentLine = username;
      this.os.terminal.executeLine();
      do
        ;
      while (this.os.terminal.prompt.Equals(prompt));
      this.os.terminal.currentLine = pass;
      this.os.terminal.executeLine();
    }

    public Texture2D GetComputerImage(Computer comp)
    {
      if (comp.icon == null || !this.compAltIcons.ContainsKey(comp.icon))
        return comp.securityLevel >= this.computers.Count ? this.defaultComputer : this.computers[comp.securityLevel];
      return this.compAltIcons[comp.icon];
    }

    private void doConnectHeader()
    {
      Computer comp = this.os.connectedComp != null ? this.os.connectedComp : this.os.thisComputer;
      this.x += 20;
      this.y += 5;
      this.spriteBatch.Draw(this.GetComputerImage(comp), new Vector2((float) this.x, (float) this.y), Color.White);
      this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Connected to") + " ", new Vector2((float) (this.x + 160), (float) this.y), Color.White);
      this.y += 40;
      TextItem.doFontLabel(new Vector2((float) (this.x + 160), (float) this.y), this.os.connectedComp == null ? this.os.thisComputer.name : this.os.connectedComp.name, GuiData.font, new Color?(Color.White), (float) this.bounds.Width - 190f, 60f, false);
      this.y += 33;
      string str = this.os.connectedComp == null ? this.os.thisComputer.ip : this.os.connectedComp.ip;
      float num1 = LocaleActivator.ActiveLocaleIsCJK() ? 4f : 0.0f;
      this.spriteBatch.DrawString(GuiData.smallfont, "@  " + str, new Vector2((float) (this.x + 160), (float) this.y + num1), Color.White);
      this.y += 60;
      if (this.os.hasConnectionPermission(true))
      {
        this.y -= 20;
        Rectangle empty = Rectangle.Empty;
        empty.X = this.bounds.X + 1;
        empty.Y = this.y;
        empty.Width = this.bounds.Width - 2;
        empty.Height = 20;
        this.spriteBatch.Draw(Utils.white, empty, this.os.highlightColor);
        string text = LocaleTerms.Loc("You are the Administrator of this System");
        Vector2 vector2 = GuiData.UISmallfont.MeasureString(text);
        Vector2 pos = new Vector2((float) (empty.X + empty.Width / 2) - vector2.X / 2f, (float) empty.Y);
        if (LocaleActivator.ActiveLocaleIsCJK())
          pos.Y -= 2f;
        this.os.postFXDrawActions += (Action) (() => this.spriteBatch.DrawString(GuiData.UISmallfont, text, pos, Color.Black));
        if (this.bounds.Height > 500)
          this.y += 40;
        else
          this.y += 12;
      }
      if (comp.portsNeededForCrack > 100)
      {
        this.y += 10;
        Rectangle rectangle = new Rectangle(this.bounds.X + 2, this.y, this.bounds.Width - 4, 20);
        string text = "INVIOLABILITY DETECTED";
        Vector2 vector2 = GuiData.titlefont.MeasureString(text);
        float num2 = vector2.X / vector2.Y;
        int num3 = 10;
        int width = (int) ((double) rectangle.Height * (double) num2);
        vector2.Y /= vector2.Y / 20f;
        int num4 = (int) ((double) (rectangle.Width - width) / 2.0);
        Rectangle destinationRectangle = new Rectangle(rectangle.X, rectangle.Y, num4 - num3, rectangle.Height);
        Rectangle dest = new Rectangle(destinationRectangle.X + destinationRectangle.Width + num3, destinationRectangle.Y, width, rectangle.Height);
        destinationRectangle.Y += 4;
        destinationRectangle.Height -= 8;
        this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Red);
        FlickeringTextEffect.DrawLinedFlickeringText(dest, text, 3f, 0.1f, GuiData.titlefont, (object) this.os, Utils.AddativeWhite, 2);
        destinationRectangle.X = rectangle.X + rectangle.Width - (destinationRectangle.Width + num3) + num3;
        this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Red);
        this.y += 40;
      }
      else
      {
        if (!this.os.hasConnectionPermission(true))
          return;
        this.y += 40;
      }
    }

    private void doConnectDisplay()
    {
      this.doConnectHeader();
      Computer computer = this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp;
      for (int index = 0; index < computer.daemons.Count; ++index)
      {
        if (computer.daemons[index] is CustomConnectDisplayOverride)
        {
          Rectangle bounds = new Rectangle(this.bounds.X + 1, this.y, this.bounds.Width - 2, this.bounds.Height - (this.y - this.bounds.Y) - 1);
          computer.daemons[index].draw(bounds, this.spriteBatch);
          return;
        }
      }
      int num1 = computer.daemons.Count + 6;
      int height = 40;
      int num2 = this.bounds.Height - (this.y - this.bounds.Y) - 20 - num1 * 5;
      if ((double) num2 / (double) num1 < (double) height)
        height = (int) ((double) num2 / (double) num1);
      for (int index = 0; index < computer.daemons.Count; ++index)
      {
        if (Button.doButton(29000 + index, this.x, this.y, 300, height, computer.daemons[index].name, new Color?(this.os.highlightColor)))
        {
          this.command = computer.daemons[index].name;
          computer.daemons[index].navigatedTo();
        }
        this.y += height + 5;
      }
      if (Button.doButton(300000, this.x, this.y, 300, height, LocaleTerms.Loc("Login"), new Color?(this.os.hasConnectionPermission(true) ? this.os.subtleTextColor : this.os.highlightColor)))
      {
        this.os.runCommand("login");
        this.os.terminal.clearCurrentLine();
      }
      this.y += height + 5;
      if (Button.doButton(300002, this.x, this.y, 300, height, LocaleTerms.Loc("Probe System"), new Color?(this.os.highlightColor)))
        this.os.runCommand("probe");
      this.y += height + 5;
      if (Button.doButton(300003, this.x, this.y, 300, height, LocaleTerms.Loc("View Filesystem"), new Color?(this.os.hasConnectionPermission(false) ? this.os.highlightColor : this.os.subtleTextColor)))
        this.os.runCommand("ls");
      this.y += height + 5;
      if (Button.doButton(300006, this.x, this.y, 300, height, LocaleTerms.Loc("View Logs"), new Color?(this.os.hasConnectionPermission(false) ? this.os.highlightColor : this.os.subtleTextColor)))
        this.os.runCommand("cd log");
      this.y += height + 5;
      if (Button.doButton(300009, this.x, this.y, 300, height, LocaleTerms.Loc("Scan Network"), new Color?(this.os.hasConnectionPermission(true) ? this.os.highlightColor : this.os.subtleTextColor)))
        this.os.runCommand("scan");
      this.y = this.bounds.Y + this.bounds.Height - 30;
      if (!Button.doButton(300012, this.x, this.y, 300, 20, LocaleTerms.Loc("Disconnect"), new Color?(this.os.lockedColor)))
        return;
      this.os.runCommand("dc");
    }

    private void doDisconnectDisplay()
    {
      this.tmpRect.X = this.bounds.X + 2;
      this.tmpRect.Width = this.bounds.Width - 4;
      this.tmpRect.Y = this.bounds.Y + this.bounds.Height / 6 * 2;
      this.tmpRect.Height = this.bounds.Height / 3;
      this.spriteBatch.Draw(Utils.white, this.tmpRect, this.os.indentBackgroundColor);
      Vector2 position = new Vector2((float) (this.tmpRect.X + this.bounds.Width / 2) - GuiData.font.MeasureString(LocaleTerms.Loc("Disconnected")).X / 2f, (float) (this.bounds.Y + this.bounds.Height / 2 - 10));
      this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Disconnected"), position, this.os.subtleTextColor);
    }

    private void doDisconnectForcedDisplay()
    {
      this.tmpRect.X = this.bounds.X + 2;
      this.tmpRect.Width = this.bounds.Width - 4;
      this.tmpRect.Y = this.bounds.Y + this.bounds.Height / 6 * 2;
      this.tmpRect.Height = this.bounds.Height / 3;
      Rectangle tmpRect = this.tmpRect;
      double num1 = Math.Abs(Math.Sin((double) this.os.timer));
      int num2 = (int) (num1 * 40.0);
      Rectangle destinationRectangle = Utils.InsetRectangle(tmpRect, -1 * num2);
      destinationRectangle.X = this.tmpRect.X;
      destinationRectangle.Width = this.tmpRect.Width;
      this.spriteBatch.Draw(Utils.white, destinationRectangle, Utils.AddativeRed * (float) (1.0 - num1));
      this.spriteBatch.Draw(Utils.white, this.tmpRect, this.os.indentBackgroundColor);
      Vector2 position = new Vector2((float) (this.tmpRect.X + this.bounds.Width / 2) - GuiData.font.MeasureString(LocaleTerms.Loc("Connection Denied by Remote Server")).X / 2f, (float) (this.bounds.Y + this.bounds.Height / 2 - 10));
      this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Connection Denied by Remote Server"), position, this.os.brightLockedColor);
    }

    private void doCatDisplay()
    {
      if (this.os.hasConnectionPermission(false))
      {
        if (Button.doButton(299999, this.bounds.X + (this.bounds.Width - 41), this.bounds.Y + 12, 27, 29, "<-", new Color?()))
          this.os.runCommand("ls");
        Rectangle tmpRect = GuiData.tmpRect;
        tmpRect.Width = this.bounds.Width;
        tmpRect.X = this.bounds.X;
        tmpRect.Y = this.bounds.Y + 1;
        tmpRect.Height = this.bounds.Height - 2;
        if (this.os.connectedComp != null && this.os.connectedComp.ip != this.LastDisplayedFileSourceIP && this.LastDisplayedFileSourceIP != this.os.thisComputer.ip)
        {
          this.command = "dc";
        }
        else
        {
          string data = "";
          for (int index = 1; index < this.commandArgs.Length; ++index)
            data = data + this.commandArgs[index] + " ";
          string text1 = LocalizedFileLoader.SafeFilterString(data);
          if (this.LastDisplayedFileFolder.searchForFile(text1.Trim()) == null)
          {
            this.os.postFXDrawActions += (Action) (() =>
            {
              Rectangle rectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + this.bounds.Height / 2 - 70, this.bounds.Width - 2, 140);
              this.spriteBatch.Draw(Utils.white, rectangle, this.os.lockedColor);
              TextItem.doCenteredFontLabel(rectangle, "File Not Found", GuiData.font, Color.White, false);
            });
            this.catScroll = Vector2.Zero;
          }
          else
          {
            TextItem.doFontLabel(new Vector2((float) this.x, (float) (this.y + 3)), text1, GuiData.font, new Color?(Color.White), (float) (this.bounds.Width - 70), float.MaxValue, false);
            int num = 55;
            Rectangle dest = new Rectangle(tmpRect.X + 4, tmpRect.Y + num, tmpRect.Width - 6, tmpRect.Height - num - 2);
            string displayCache = this.os.displayCache;
            this.y += 70;
            string text2 = Utils.SuperSmartTwimForWidth(LocalizedFileLoader.SafeFilterString(displayCache), this.bounds.Width - 40, GuiData.tinyfont);
            this.catTextRegion.Draw(dest, text2, this.spriteBatch);
          }
        }
      }
      else
        this.command = "connect";
    }

    private void doProbeDisplay()
    {
      Rectangle rectangle = Rectangle.Empty;
      Computer computer = this.os.connectedComp == null ? this.os.thisComputer : this.os.connectedComp;
      if (computer.proxyActive)
      {
        rectangle = this.bounds;
        ++rectangle.X;
        ++rectangle.Y;
        rectangle.Width -= 2;
        rectangle.Height -= 2;
        PatternDrawer.draw(rectangle, 0.8f, Color.Transparent, this.os.superLightWhite, this.os.ScreenManager.SpriteBatch);
      }
      if (Button.doButton(299999, this.bounds.X + (this.bounds.Width - 50), this.bounds.Y + this.y, 27, 27, "<-", new Color?()))
        this.command = "connect";
      this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Open Ports"), new Vector2((float) this.x, (float) this.y), Color.White);
      this.y += 40;
      this.spriteBatch.DrawString(GuiData.smallfont, computer.name + " @" + computer.ip, new Vector2((float) this.x, (float) this.y), Color.White);
      this.y += 30;
      int num = Math.Max(computer.portsNeededForCrack + 1, 0);
      string str = string.Concat((object) num);
      bool flag1 = num > 100;
      if (flag1)
      {
        if (this.invioableSecurityCacheString == null)
        {
          StringBuilder stringBuilder = new StringBuilder();
          for (int index = 0; index < str.Length; ++index)
            stringBuilder.Append(Utils.getRandomChar());
          this.invioableSecurityCacheString = stringBuilder.ToString();
        }
        else
        {
          this.invioabilityCharChangeTimer -= (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
          if ((double) this.invioabilityCharChangeTimer <= 0.0)
          {
            StringBuilder stringBuilder = new StringBuilder(this.invioableSecurityCacheString);
            stringBuilder[Utils.random.Next(stringBuilder.Length)] = Utils.random.NextDouble() > 0.3 ? Utils.getRandomNumberChar() : Utils.getRandomChar();
            this.invioableSecurityCacheString = stringBuilder.ToString();
            this.invioabilityCharChangeTimer = 0.025f;
          }
        }
        str = this.invioableSecurityCacheString;
      }
      this.spriteBatch.DrawString(GuiData.smallfont, LocalizedFileLoader.SafeFilterString(LocaleTerms.Loc("Open Ports Required for Crack:")) + " " + str, new Vector2((float) this.x, (float) this.y), flag1 ? Color.Lerp(Color.Red, this.os.brightLockedColor, Utils.randm(0.5f) + 0.5f) : this.os.highlightColor);
      this.y += 40;
      if (flag1)
      {
        rectangle.X = this.bounds.X + 2;
        rectangle.Y = this.y;
        rectangle.Width = this.bounds.Width - 4;
        rectangle.Height = 110;
        this.DrawInvioabilityEffect(rectangle);
        this.y += rectangle.Height + 10;
      }
      if (computer.hasProxy)
      {
        rectangle.X = this.x;
        rectangle.Y = this.y;
        rectangle.Width = this.bounds.Width - 10;
        rectangle.Height = 40;
        PatternDrawer.draw(rectangle, 1f, computer.proxyActive ? this.os.topBarColor : Color.Lerp(this.os.unlockedColor, Color.Black, 0.2f), computer.proxyActive ? this.os.shellColor * 0.3f : this.os.unlockedColor, this.os.ScreenManager.SpriteBatch);
        if (computer.proxyActive)
        {
          rectangle.Width = (int) ((double) rectangle.Width * (1.0 - (double) computer.proxyOverloadTicks / (double) computer.startingOverloadTicks));
          this.spriteBatch.Draw(Utils.white, rectangle, Color.Black * 0.5f);
        }
        this.spriteBatch.DrawString(GuiData.smallfont, computer.proxyActive ? LocaleTerms.Loc("Proxy Detected") : LocaleTerms.Loc("Proxy Bypassed"), new Vector2((float) (this.x + 4), (float) (this.y + 2)), Color.Black);
        this.spriteBatch.DrawString(GuiData.smallfont, computer.proxyActive ? LocaleTerms.Loc("Proxy Detected") : LocaleTerms.Loc("Proxy Bypassed"), new Vector2((float) (this.x + 3), (float) (this.y + 1)), computer.proxyActive ? Color.White : this.os.highlightColor);
        this.y += 60;
      }
      if (computer.firewall != null)
      {
        rectangle.X = this.x;
        rectangle.Y = this.y;
        rectangle.Width = this.bounds.Width - 10;
        rectangle.Height = 40;
        bool flag2 = !computer.firewall.solved;
        PatternDrawer.draw(rectangle, 1f, flag2 ? this.os.topBarColor : Color.Lerp(this.os.unlockedColor, Color.Black, 0.2f), flag2 ? this.os.shellColor * 0.3f : this.os.unlockedColor, this.os.ScreenManager.SpriteBatch);
        this.spriteBatch.DrawString(GuiData.smallfont, flag2 ? LocaleTerms.Loc("Firewall Detected") : LocaleTerms.Loc("Firewall Solved"), new Vector2((float) (this.x + 4), (float) (this.y + 2)), Color.Black);
        this.spriteBatch.DrawString(GuiData.smallfont, flag2 ? LocaleTerms.Loc("Firewall Detected") : LocaleTerms.Loc("Firewall Solved"), new Vector2((float) (this.x + 3), (float) (this.y + 1)), flag2 ? Color.White : this.os.highlightColor);
        this.y += 60;
      }
      Vector2 zero = Vector2.Zero;
      rectangle.X = this.x + 1;
      rectangle.Width = 420;
      rectangle.Height = 41;
      Vector2 position = new Vector2((float) (rectangle.X + rectangle.Width - 36), (float) (rectangle.Y + 7));
      this.x += 10;
      for (int index = 0; index < computer.ports.Count; ++index)
      {
        rectangle.Y = this.y + 4;
        position.Y = (float) (rectangle.Y + 4);
        this.spriteBatch.Draw(Utils.white, rectangle, (int) computer.portsOpen[index] > 0 ? this.os.unlockedColor : this.os.lockedColor);
        this.spriteBatch.Draw((int) computer.portsOpen[index] > 0 ? this.openLockSprite : this.lockSprite, position, Color.White);
        string text1 = "Port#: " + (object) computer.GetDisplayPortNumberFromCodePort(computer.ports[index]);
        Vector2 vector2_1 = GuiData.font.MeasureString(text1);
        this.spriteBatch.DrawString(GuiData.font, text1, new Vector2((float) this.x, (float) (this.y + 3)), Color.White);
        string text2 = " - " + PortExploits.services[computer.ports[index]];
        Vector2 vector2_2 = GuiData.smallfont.MeasureString(text2);
        float scale = Math.Min(1f, (float) ((double) rectangle.Width - (double) vector2_1.X - 50.0) / vector2_2.X);
        this.spriteBatch.DrawString(GuiData.smallfont, text2, new Vector2((float) this.x + vector2_1.X, (float) (this.y + 4)), Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.8f);
        this.y += 45;
      }
    }

    private void DrawInvioabilityEffect(Rectangle dest)
    {
      Color color = Color.Lerp(this.os.lockedColor, this.os.brightLockedColor, Utils.randm(0.5f) + 0.5f);
      color.A = (byte) 0;
      Rectangle destinationRectangle1 = new Rectangle(dest.X + this.fancyCornerSprite.Width, dest.Y, dest.Width - this.fancyCornerSprite.Width * 2, this.fancyPanelSprite.Height);
      this.spriteBatch.Draw(this.fancyPanelSprite, destinationRectangle1, color);
      this.spriteBatch.Draw(this.fancyCornerSprite, new Vector2((float) dest.X, (float) dest.Y), new Rectangle?(), color, 0.0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0.4f);
      this.spriteBatch.Draw(this.fancyCornerSprite, new Vector2((float) (destinationRectangle1.X + destinationRectangle1.Width), (float) dest.Y), color);
      int num1 = this.fancyCornerSprite.Width - 38;
      Rectangle dest1 = new Rectangle(dest.X + num1, dest.Y + this.fancyCornerSprite.Height / 2, dest.Width - num1 * 2, dest.Height - this.fancyCornerSprite.Height);
      Color.Lerp(Utils.AddativeWhite, this.os.brightLockedColor, Utils.randm(0.5f) + 0.5f);
      FlickeringTextEffect.DrawLinedFlickeringText(dest1, "INVIOLABILITY ERROR", 4f, 0.26f, GuiData.titlefont, (object) this.os, Utils.AddativeWhite, 2);
      Rectangle destinationRectangle2 = destinationRectangle1;
      destinationRectangle2.Y = dest.Y + dest.Height - this.fancyPanelSprite.Height;
      this.spriteBatch.Draw(this.fancyPanelSprite, destinationRectangle2, new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.FlipVertically, 0.5f);
      float num2 = (float) (this.fancyCornerSprite.Height - this.fancyPanelSprite.Height);
      this.spriteBatch.Draw(this.fancyCornerSprite, new Vector2((float) dest.X, (float) destinationRectangle2.Y - num2), new Rectangle?(), color, 0.0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0.4f);
      this.spriteBatch.Draw(this.fancyCornerSprite, new Vector2((float) (destinationRectangle1.X + destinationRectangle1.Width), (float) destinationRectangle2.Y - num2), new Rectangle?(), color, 0.0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0.5f);
    }

    private void doLsDisplay()
    {
      if (this.os.hasConnectionPermission(false))
      {
        this.x = 5;
        this.y = 5;
        int num = this.bounds.Width - 25;
        TextItem.doFontLabel(new Vector2((float) (this.bounds.X + this.x), (float) (this.bounds.Y + this.y)), this.os.connectedComp == null ? this.os.thisComputer.name : this.os.connectedComp.name + " " + LocaleTerms.Loc("File System"), GuiData.font, new Color?(Color.White), (float) this.bounds.Width - 46f, 60f, false);
        if (Button.doButton(299999, this.bounds.X + (this.bounds.Width - 41), this.bounds.Y + 12, 27, 29, "<-", new Color?()))
        {
          if (this.os.navigationPath.Count > 0)
            this.os.runCommand("cd ..");
          else
            this.os.display.command = "connect";
        }
        this.y += 50;
        Rectangle tmpRect = GuiData.tmpRect;
        tmpRect.Width = this.bounds.Width;
        tmpRect.X = this.bounds.X;
        tmpRect.Y = this.bounds.Y + 55;
        tmpRect.Height = this.bounds.Height - 57;
        this.lsModuleHelper.DrawUI(tmpRect, this.os);
      }
      else
        this.command = "connect";
    }

    private void doFolderGui(int width, int height, int indexOffset, Folder f, int recItteration)
    {
      for (int i = 0; i < f.folders.Count; ++i)
      {
        if (Button.doButton(300000 + i + indexOffset, this.x, this.y, width, height, "/" + f.folders[i].name, new Color?()))
        {
          int num = 0;
          for (int index = 0; index < this.os.navigationPath.Count - recItteration; ++index)
          {
            Action action = (Action) (() => this.os.runCommand("cd .."));
            if (num > 0)
              this.os.delayer.Post(ActionDelayer.Wait((double) num * 1.0), action);
            else
              action();
            ++num;
          }
          Action action1 = (Action) (() => this.os.runCommand("cd " + f.folders[i].name));
          if (num > 0)
            this.os.delayer.Post(ActionDelayer.Wait((double) num * 1.0), action1);
          else
            action1();
        }
        this.y += height + 2;
        this.x += 30;
        if (this.os.navigationPath.Count - 1 >= recItteration && this.os.navigationPath[recItteration] == i)
          this.doFolderGui(width - 30, height, indexOffset + 10000 * (i + 1), f.folders[i], recItteration + 1);
        this.x -= 30;
      }
      for (int index1 = 0; index1 < f.files.Count; ++index1)
      {
        if (Button.doButton(400000 + index1 + indexOffset / 2 + (index1 + 1) * indexOffset, this.x, this.y, width, height, f.files[index1].name, new Color?()))
        {
          for (int index2 = 0; index2 < this.os.navigationPath.Count - recItteration; ++index2)
            this.os.runCommand("cd ..");
          this.os.runCommand("cat " + f.files[index1].name);
        }
        this.y += height + 2;
      }
      if (f.folders.Count != 0 || f.files.Count != 0)
        return;
      TextItem.doFontLabel(new Vector2((float) this.x, (float) this.y), "-" + LocaleTerms.Loc("Empty") + "-", GuiData.tinyfont, new Color?(), (float) width, (float) height, false);
      this.y += height + 2;
    }

    public static string splitForWidth(string s, int width)
    {
      string str = "";
      int num = 0;
      foreach (char ch in s)
      {
        str += (string) (object) ch;
        if ((int) ch != 10)
          ++num;
        else
          num = 0;
        if (num > width / 6 && ((int) ch == 32 || (double) num > (double) width / 5.19999980926514))
        {
          str += (string) (object) '\n';
          num = 0;
        }
      }
      return str;
    }

    public static string splitForWidth(string s, int width, bool correct)
    {
      string str = "";
      int num = 0;
      width /= 8;
      foreach (char ch in s)
      {
        str += (string) (object) ch;
        if ((int) ch != 10)
          ++num;
        else
          num = 0;
        if (num >= width && ((int) ch == 32 || (double) num > (double) width * 0.899999976158142))
        {
          str += (string) (object) '\n';
          num = 0;
        }
      }
      return str;
    }

    public static string cleanSplitForWidth(string s, int width)
    {
      int num1 = 10;
      width /= num1;
      string str = "";
      char[] chArray = new char[1]{ ' ' };
      string[] strArray = s.Split(chArray);
      int index1 = 0;
      if (strArray.Length == 1)
        return DisplayModule.splitForWidth(strArray[0], width * 8, true);
      while (index1 < strArray.Length)
      {
        for (int index2 = 0; index2 < width && index1 < strArray.Length; ++index1)
        {
          str = str + strArray[index1] + " ";
          index2 += strArray[index1].Length;
          int num2 = strArray[index1].IndexOf('\n');
          if (num2 >= 0)
            index2 = strArray[index1].Length - (num2 + 1);
        }
        str += (string) (object) '\n';
      }
      return str;
    }
  }
}
