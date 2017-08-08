// Decompiled with JetBrains decompiler
// Type: Hacknet.WhitelistConnectionDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet
{
  internal class WhitelistConnectionDaemon : Daemon
  {
    public string RemoteSourceIP = (string) null;
    public bool AuthenticatesItself = true;
    private bool HasAllowedSingleTimeAdminPassthroughThisSession = false;
    private const string SystemFilename = "authenticator.dll";
    private const string ListFilename = "list.txt";
    private const string SourceFilename = "source.txt";
    private Folder folder;

    public WhitelistConnectionDaemon(Computer c, OS os)
      : base(c, "Whitelist Authenticator", os)
    {
    }

    public override void initFiles()
    {
      this.folder = this.comp.files.root.searchForFolder("Whitelist");
      if (this.folder == null)
      {
        this.folder = new Folder("Whitelist");
        this.comp.files.root.folders.Add(this.folder);
      }
      if (!this.folder.containsFile("authenticator.dll"))
        this.folder.files.Add(new FileEntry(Computer.generateBinaryString(500), "authenticator.dll"));
      if (this.RemoteSourceIP == null)
      {
        if (this.folder.containsFile("list.txt"))
          return;
        this.folder.files.Add(new FileEntry(this.comp.adminIP, "list.txt"));
      }
      else
        ComputerLoader.postAllLoadedActions += (Action) (() =>
        {
          Computer computer = Programs.getComputer(this.os, this.RemoteSourceIP);
          if (computer != null)
            this.RemoteSourceIP = computer.ip;
          if (this.folder.containsFile("source.txt"))
            return;
          this.folder.files.Add(new FileEntry(this.RemoteSourceIP, "source.txt"));
        });
    }

    public override void loadInit()
    {
      base.loadInit();
      this.folder = this.comp.files.root.searchForFolder("Whitelist");
      if (!this.folder.containsFile("source.txt"))
        return;
      this.RemoteSourceIP = this.folder.searchForFile("source.txt").data.Trim();
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      if (this.IPCanPassWhitelist(this.os.thisComputer.ip, false))
        return;
      this.DisconnectTarget();
    }

    public void DisconnectTarget()
    {
      this.os.execute("disconnect");
      this.os.display.command = "connectiondenied";
      this.os.delayer.Post(ActionDelayer.NextTick(), (Action) (() => this.os.display.command = "connectiondenied"));
      this.os.write(" ");
      this.os.write(" ");
      this.os.write("------------------------------");
      this.os.write("------------------------------");
      this.os.write(" ");
      this.os.write("---  " + LocaleTerms.Loc("CONNECTION ERROR") + "  ---");
      this.os.write(" ");
      this.os.write(LocaleTerms.Loc("Message from Server:"));
      this.os.write(string.Format(LocaleTerms.Loc("Whitelist Authenticator denied connection from IP {0}"), (object) this.os.thisComputer.ip));
      this.os.write(" ");
      this.os.write("------------------------------");
      this.os.write("------------------------------");
      this.os.write(" ");
    }

    private bool RemoteCompCanBeAccessed()
    {
      if (this.RemoteSourceIP == null)
        return false;
      Computer computer = Programs.getComputer(this.os, this.RemoteSourceIP);
      if (computer == null || (double) computer.bootTimer > 0.0)
        return false;
      WhitelistConnectionDaemon daemon = (WhitelistConnectionDaemon) computer.getDaemon(typeof (WhitelistConnectionDaemon));
      return daemon == null || daemon.comp.files.root.searchForFolder("Whitelist").searchForFile("authenticator.dll") != null;
    }

    public bool IPCanPassWhitelist(string ip, bool isFromRemote)
    {
      if (!this.AuthenticatesItself && !isFromRemote)
        return true;
      if (this.RemoteSourceIP != null)
      {
        Computer computer = Programs.getComputer(this.os, this.RemoteSourceIP);
        if (!this.RemoteCompCanBeAccessed() || this.folder.searchForFile("authenticator.dll") == null)
          return true;
        WhitelistConnectionDaemon daemon = (WhitelistConnectionDaemon) computer.getDaemon(typeof (WhitelistConnectionDaemon));
        if (daemon == null)
          return true;
        return daemon.IPCanPassWhitelist(ip, true);
      }
      if (this.folder.searchForFile("authenticator.dll") == null)
        return true;
      FileEntry fileEntry = this.folder.searchForFile("list.txt");
      if (fileEntry == null)
        return true;
      foreach (string str in fileEntry.data.Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries))
      {
        if (this.os.thisComputer.ip == str.Trim())
          return true;
      }
      return false;
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      if (this.comp.adminIP == this.os.thisComputer.ip && !this.HasAllowedSingleTimeAdminPassthroughThisSession)
      {
        this.os.display.command = "connect";
        this.HasAllowedSingleTimeAdminPassthroughThisSession = true;
      }
      Rectangle dest1 = Utils.InsetRectangle(bounds, 2);
      bool flag1 = this.RemoteCompCanBeAccessed();
      bool flag2 = this.IPCanPassWhitelist(this.os.thisComputer.ip, false) && flag1;
      bool flag3 = this.folder.searchForFile("authenticator.dll") != null;
      bool flag4 = this.RemoteSourceIP != null || this.folder.searchForFile("list.txt") != null;
      bool flag5 = flag2 && flag4 && flag3;
      Color color = !this.AuthenticatesItself ? this.os.highlightColor : (flag5 ? this.os.unlockedColor : this.os.brightLockedColor);
      PatternDrawer.draw(dest1, 1f, Color.Black * 0.1f, color * 0.4f, sb, flag5 ? PatternDrawer.thinStripe : PatternDrawer.errorTile);
      Rectangle dest2 = new Rectangle(dest1.X + 10, dest1.Y + dest1.Height / 4, dest1.Width - 20, 40);
      if (Button.doButton(8711133, dest2.X, dest2.Y - 30, dest1.Width / 4, 24, LocaleTerms.Loc("Proceed"), new Color?(this.os.highlightColor)))
        this.os.display.command = "connect";
      string text1 = string.Format(LocaleTerms.Loc("Whitelist Authentication Successful : {0}"), (object) this.os.thisComputer.ip);
      if (!this.AuthenticatesItself)
        text1 = LocaleTerms.Loc("Whitelist Server Active for remote nodes...");
      else if (!flag1)
        text1 = string.Format(LocaleTerms.Loc("Whitelist Authenticator Critical Error:") + "\n" + LocaleTerms.Loc("Source Whitelist server at {0} cannot be accessed"), (object) this.RemoteSourceIP);
      else if (!flag3)
        text1 = string.Format(LocaleTerms.Loc("Whitelist Authenticator Critical Error:") + "\n" + LocaleTerms.Loc("System File {0} not found"), (object) "authenticator.dll");
      else if (!flag4)
        text1 = string.Format(LocaleTerms.Loc("Whitelist Authenticator Critical Error:") + "\n" + LocaleTerms.Loc("Whitelist File {0} not found"), (object) "list.txt");
      sb.Draw(Utils.white, new Rectangle(bounds.X + 1, dest2.Y - 3, bounds.Width - 2, dest2.Height + 6), Color.Black * 0.7f);
      TextItem.doFontLabelToSize(dest2, text1, GuiData.font, color, true, true);
      Rectangle dest3 = new Rectangle(dest2.X, dest2.Y + dest2.Height + 6, dest2.Width, dest1.Height / 2);
      string text2 = LocaleTerms.Loc("Connection established");
      if (!this.AuthenticatesItself)
        text2 = LocaleTerms.Loc("Processing connections as authentication server.");
      else if (!flag1)
        text2 = string.Format(LocaleTerms.Loc("Could not establish connection to whitelist server {0}.") + "\n" + LocaleTerms.Loc("Aborting Execution."), (object) this.RemoteSourceIP);
      else if (!flag3)
        text2 = LocaleTerms.Loc("Unhanded FileNotFoundException") + "\n" + string.Format(LocaleTerms.Loc("File /Whitelist/{0} Could not be read."), (object) "authenticator.dll") + "\n" + LocaleTerms.Loc("Aborting Execution");
      else if (!flag4)
        text2 = LocaleTerms.Loc("Unhanded FileNotFoundException") + "\n" + string.Format(LocaleTerms.Loc("File /Whitelist/{0} Could not be read."), (object) "list.txt") + "\n" + LocaleTerms.Loc("Aborting Execution");
      TextItem.doFontLabelToSize(dest3, text2, GuiData.smallfont, Color.White * 0.8f, true, true);
    }

    public override string getSaveString()
    {
      return "<WhitelistAuthenticatorDaemon SelfAuthenticating=\"" + (object) this.AuthenticatesItself + "\"/>";
    }
  }
}
