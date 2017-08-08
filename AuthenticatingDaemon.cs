// Decompiled with JetBrains decompiler
// Type: Hacknet.AuthenticatingDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;

namespace Hacknet
{
  internal class AuthenticatingDaemon : Daemon
  {
    public UserDetail user;

    public AuthenticatingDaemon(Computer computer, string serviceName, OS opSystem)
      : base(computer, serviceName, opSystem)
    {
    }

    public virtual void loginGoBack()
    {
    }

    public virtual void userLoggedIn()
    {
    }

    public void startLogin()
    {
      this.os.displayCache = "";
      this.os.execute("login");
      do
        ;
      while (this.os.displayCache.Equals(""));
      this.os.display.command = this.name;
    }

    public void doLoginDisplay(Rectangle bounds, SpriteBatch sb)
    {
      int num1 = bounds.X + 20;
      int num2 = bounds.Y + 100;
      string[] strArray = this.os.displayCache.Split(new string[1]{ "#$#$#$$#$&$#$#$#$#" }, StringSplitOptions.None);
      string text1 = "";
      string text2 = "";
      int num3 = -1;
      int num4 = 0;
      if (strArray[0].Equals("loginData"))
      {
        text1 = !(strArray[1] != "") ? this.os.terminal.currentLine : strArray[1];
        if (strArray.Length > 2)
        {
          num4 = 1;
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
          num4 = 2;
          num3 = Convert.ToInt32(strArray[3]);
        }
      }
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = bounds.X + 2;
      tmpRect.Y = num2;
      tmpRect.Height = 200;
      tmpRect.Width = bounds.Width - 4;
      sb.Draw(Utils.white, tmpRect, num3 == 0 ? this.os.lockedColor : this.os.indentBackgroundColor);
      if (num3 != 0 && num3 != -1)
      {
        for (int index = 0; index < this.comp.users.Count; ++index)
        {
          if (this.comp.users[index].name.Equals(text1))
            this.user = this.comp.users[index];
        }
        this.userLoggedIn();
      }
      tmpRect.Height = 22;
      int num5 = num2 + 30;
      Vector2 vector2 = TextItem.doMeasuredLabel(new Vector2((float) num1, (float) num5), LocaleTerms.Loc("Login") + " ", new Color?(Color.White));
      if (num3 == 0)
      {
        int num6 = num1 + (int) vector2.X;
        TextItem.doLabel(new Vector2((float) num6, (float) num5), LocaleTerms.Loc("Failed"), new Color?(this.os.brightLockedColor));
        num1 = num6 - (int) vector2.X;
      }
      int num7 = num5 + 60;
      if (num4 == 0)
      {
        tmpRect.Y = num7;
        sb.Draw(Utils.white, tmpRect, this.os.subtleTextColor);
      }
      int num8 = (int) (6.0 + (double) Math.Max(GuiData.smallfont.MeasureString(LocaleTerms.Loc("username :")).X, GuiData.smallfont.MeasureString(LocaleTerms.Loc("password :")).X));
      sb.DrawString(GuiData.smallfont, LocaleTerms.Loc("username :"), new Vector2((float) num1, (float) num7), Color.White);
      int num9 = num1 + num8;
      sb.DrawString(GuiData.smallfont, text1, new Vector2((float) num9, (float) num7), Color.White);
      int num10 = num9 - num8;
      int num11 = num7 + 30;
      if (num4 == 1)
      {
        tmpRect.Y = num11;
        sb.Draw(Utils.white, tmpRect, this.os.subtleTextColor);
      }
      sb.DrawString(GuiData.smallfont, LocaleTerms.Loc("password :"), new Vector2((float) num10, (float) num11), Color.White);
      int num12 = num10 + num8;
      sb.DrawString(GuiData.smallfont, text2, new Vector2((float) num12, (float) num11), Color.White);
      int y1 = num11 + 30;
      int x = num12 - num8;
      if (num3 != -1)
      {
        if (Button.doButton(12345, x, y1, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.os.indentBackgroundColor)))
          this.loginGoBack();
        if (!Button.doButton(123456, x + 165, y1, 160, 30, LocaleTerms.Loc("Retry"), new Color?(this.os.indentBackgroundColor)))
          return;
        this.os.displayCache = "";
        this.os.execute("login");
        do
          ;
        while (this.os.displayCache.Equals(""));
        this.os.display.command = this.name;
      }
      else
      {
        int y2 = y1 + 65;
        for (int index = 0; index < this.comp.users.Count; ++index)
        {
          if (this.comp.users[index].known && AuthenticatingDaemon.validUser(this.comp.users[index].type))
          {
            if (Button.doButton(123457 + index, x, y2, 300, 25, LocaleTerms.Loc("User") + ": " + this.comp.users[index].name + " " + LocaleTerms.Loc("Pass") + ": " + this.comp.users[index].pass, new Color?(this.os.darkBackgroundColor)))
              this.forceLogin(this.comp.users[index].name, this.comp.users[index].pass);
            y2 += 27;
          }
        }
      }
    }

    public new static bool validUser(byte type)
    {
      return Daemon.validUser(type) || (int) type == 3;
    }

    public void forceLogin(string username, string pass)
    {
      string prompt = this.os.terminal.prompt;
      this.os.terminal.currentLine = username;
      this.os.terminal.NonThreadedInstantExecuteLine();
      while (this.os.terminal.prompt.Equals(prompt))
        Thread.Sleep(0);
      this.os.terminal.currentLine = pass;
      this.os.terminal.NonThreadedInstantExecuteLine();
    }
  }
}
