// Decompiled with JetBrains decompiler
// Type: Hacknet.IRCDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Daemons.Helpers;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class IRCDaemon : Daemon, IMonitorableDaemon
  {
    public List<KeyValuePair<string, string>> StartingMessages = new List<KeyValuePair<string, string>>();
    public Dictionary<string, Color> UserColors = new Dictionary<string, Color>();
    public bool RequiresLogin = false;
    public IRCSystem System;
    public Color ThemeColor;
    public DelayableActionSystem DelayedActions;

    public IRCDaemon(Computer c, OS os, string name)
      : base(c, name, os)
    {
      this.isListed = true;
    }

    public override void initFiles()
    {
      base.initFiles();
      Folder storageFolder = this.comp.files.root.searchForFolder("IRC");
      if (storageFolder == null)
      {
        storageFolder = new Folder("IRC");
        this.comp.files.root.folders.Add(storageFolder);
      }
      this.System = new IRCSystem(storageFolder);
      for (int index = 0; index < this.StartingMessages.Count; ++index)
        this.System.AddLog(this.StartingMessages[index].Key, this.StartingMessages[index].Value, (string) null);
      string str = this.name + "\nRequireAuth: " + (this.RequiresLogin ? "true" : "false") + "\n" + Utils.convertColorToParseableString(this.ThemeColor) + "\n";
      foreach (KeyValuePair<string, Color> userColor in this.UserColors)
        str = str + userColor.Key + "#%#" + Utils.convertColorToParseableString(userColor.Value) + "\n";
      storageFolder.files.Add(new FileEntry(str.Trim(), "users.cfg"));
      Folder sourceFolder = storageFolder.searchForFolder("runtime");
      if (sourceFolder == null)
      {
        sourceFolder = new Folder("runtime");
        storageFolder.folders.Add(sourceFolder);
      }
      this.DelayedActions = new DelayableActionSystem(sourceFolder, (object) this.os);
    }

    public override void loadInit()
    {
      base.loadInit();
      Folder storageFolder = this.comp.files.root.searchForFolder("IRC");
      this.System = new IRCSystem(storageFolder);
      this.DelayedActions = new DelayableActionSystem(storageFolder.searchForFolder("runtime"), (object) this.os);
      this.ReloadUserColors();
    }

    public void SubscribeToAlertActionFroNewMessage(Action<string, string> act)
    {
      this.System.LogAdded += act;
    }

    public void UnSubscribeToAlertActionFroNewMessage(Action<string, string> act)
    {
      this.System.LogAdded -= act;
    }

    public bool ShouldDisplayNotifications()
    {
      return true;
    }

    public string GetName()
    {
      return this.name;
    }

    private void ReloadUserColors()
    {
      Folder folder = this.comp.files.root.searchForFolder("IRC");
      this.UserColors.Clear();
      FileEntry fileEntry = folder.searchForFile("users.cfg");
      if (fileEntry == null)
        return;
      string[] strArray1 = fileEntry.data.Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries);
      try
      {
        this.name = strArray1[0];
        this.RequiresLogin = strArray1[1].Substring("RequireAuth: ".Length).Trim().ToLower() == "true";
        this.ThemeColor = Utils.convertStringToColor(strArray1[2]);
      }
      catch (Exception ex)
      {
      }
      for (int index = 3; index < strArray1.Length; ++index)
      {
        try
        {
          string[] strArray2 = strArray1[index].Split(new string[1]
          {
            "#%#"
          }, StringSplitOptions.RemoveEmptyEntries);
          this.UserColors.Add(strArray2[0], Utils.convertStringToColor(strArray2[1]));
        }
        catch (Exception ex)
        {
        }
      }
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      this.ReloadUserColors();
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      bool flag = !this.RequiresLogin || this.comp.adminIP == this.os.thisComputer.ip || this.comp.userLoggedIn;
      Rectangle dest = Utils.InsetRectangle(bounds, 2);
      Rectangle destinationRectangle = new Rectangle(dest.X, dest.Y - 1, 18, dest.Height + 2);
      sb.Draw(Utils.white, destinationRectangle, this.ThemeColor);
      destinationRectangle.X += destinationRectangle.Width / 2;
      destinationRectangle.Width /= 2;
      sb.Draw(Utils.white, destinationRectangle, Color.Black * 0.2f);
      dest.X += 20;
      dest.Width -= 25;
      Rectangle rectangle1 = new Rectangle(dest.X + 4, dest.Y, dest.Width, 35);
      TextItem.doFontLabelToSize(rectangle1, this.name, GuiData.font, Color.White, true, true);
      int width = dest.Width / 4;
      int height1 = 22;
      if (Button.doButton(37849102, rectangle1.X + rectangle1.Width - 6 - width, rectangle1.Y + rectangle1.Height - rectangle1.Height / 2 - height1 / 2, width, height1, LocaleTerms.Loc("Exit IRC View"), new Color?(this.ThemeColor)))
        this.os.display.command = "connect";
      rectangle1.Y += rectangle1.Height;
      rectangle1.X -= 6;
      dest.Y += rectangle1.Height;
      dest.Height -= rectangle1.Height;
      rectangle1.Height = 2;
      sb.Draw(Utils.white, rectangle1, this.ThemeColor);
      dest.Y += rectangle1.Height + 2;
      dest.Height -= rectangle1.Height + 2;
      dest.Height -= 6;
      PatternDrawer.draw(dest, 0.22f, Color.Black * 0.5f, flag ? this.ThemeColor * 0.12f : Utils.AddativeRed * 0.2f, sb, flag ? PatternDrawer.thinStripe : PatternDrawer.warningStripe);
      dest.X += 2;
      dest.Width -= 4;
      dest.Height -= 4;
      if (flag)
      {
        this.System.Draw(dest, sb, false, LocaleTerms.Loc("UNKNOWN"), this.UserColors);
      }
      else
      {
        int height2 = dest.Height / 4;
        Rectangle rectangle2 = new Rectangle(dest.X - 4, dest.Y + dest.Height / 2 - height2 / 2, dest.Width + 6, height2);
        sb.Draw(Utils.white, rectangle2, this.os.lockedColor);
        rectangle2.Height -= 35;
        TextItem.doCenteredFontLabel(rectangle2, LocaleTerms.Loc("Login To Server"), GuiData.font, Color.White, false);
        if (Button.doButton(84109551, rectangle2.X + rectangle2.Width / 2 - rectangle2.Width / 4, rectangle2.Y + rectangle2.Height - 32, rectangle2.Width / 2, 28, "Login", new Color?()))
          this.os.runCommand("login");
      }
    }

    public override string getSaveString()
    {
      return "<IRCDaemon />";
    }
  }
}
