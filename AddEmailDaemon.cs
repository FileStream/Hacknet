// Decompiled with JetBrains decompiler
// Type: Hacknet.AddEmailDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace Hacknet
{
  internal class AddEmailDaemon : Daemon
  {
    private const string SEND_SCRIPT_URL = "http://www.tijital-games.com/hacknet/SendVictoryEmail.php?mail=[EMAIL]";
    private const int WAITING = 0;
    private const int ENTERING = 1;
    private const int CONFIRM = 2;
    private const int ERROR = 3;
    private static string lastSentEmail;
    private int state;
    private string email;

    public AddEmailDaemon(Computer computer, string serviceName, OS opSystem)
      : base(computer, serviceName, opSystem)
    {
      this.state = 0;
      this.email = "";
    }

    private void saveEmail()
    {
      try
      {
        Utils.appendToFile(this.email + "\r\n", "Emails.txt");
        AddEmailDaemon.lastSentEmail = this.email;
        SFX.addCircle(this.os.netMap.GetNodeDrawPos(this.comp) + new Vector2((float) this.os.netMap.bounds.X, (float) this.os.netMap.bounds.Y) + new Vector2((float) (NetworkMap.NODE_SIZE / 2)), this.os.thisComputerNode, 100f);
        this.state = 0;
        this.os.display.command = "connect";
      }
      catch (Exception ex)
      {
        this.state = 3;
      }
    }

    public void sendEmail()
    {
      if (AddEmailDaemon.lastSentEmail != null && AddEmailDaemon.lastSentEmail.Equals(this.email))
        return;
      if (this.email != null && this.email.Contains<char>('@'))
      {
        new Thread(new ThreadStart(this.makeWebRequest))
        {
          IsBackground = true
        }.Start();
        AddEmailDaemon.lastSentEmail = this.email;
        SFX.addCircle(this.comp.location + new Vector2((float) this.os.netMap.bounds.X, (float) this.os.netMap.bounds.Y) + new Vector2((float) (NetworkMap.NODE_SIZE / 2)), this.os.thisComputerNode, 100f);
        this.state = 0;
        this.os.display.command = "connect";
      }
      else
        this.state = 3;
    }

    public override string getSaveString()
    {
      return "<AddEmailServer name=\"" + this.name + "\"/>";
    }

    public void makeWebRequest()
    {
      try
      {
        Console.WriteLine(new WebClient().DownloadString("http://www.tijital-games.com/hacknet/SendVictoryEmail.php?mail=[EMAIL]".Replace("[EMAIL]", this.email)));
      }
      catch (Exception ex)
      {
      }
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      int x = bounds.X + 10;
      int num1 = bounds.Y + 10;
      TextItem.doFontLabel(new Vector2((float) x, (float) num1), "Email Verification", GuiData.font, new Color?(), (float) (bounds.Width - 20), (float) bounds.Height, false);
      int num2 = num1 + 50;
      switch (this.state)
      {
        case 1:
          Vector2 vector2_1 = TextItem.doMeasuredSmallLabel(new Vector2((float) x, (float) num2), "Enter a secure email address :\nEnsure that you are the only one with access to it", new Color?());
          int y1 = num2 + ((int) vector2_1.Y + 10);
          string[] strArray = this.os.getStringCache.Split(new string[1]{ "#$#$#$$#$&$#$#$#$#" }, StringSplitOptions.None);
          if (strArray.Length > 1)
          {
            this.email = strArray[1];
            if (this.email.Equals(""))
              this.email = this.os.terminal.currentLine;
          }
          Rectangle destinationRectangle = new Rectangle(x, y1, bounds.Width - 20, 200);
          sb.Draw(Utils.white, destinationRectangle, this.os.darkBackgroundColor);
          int num3 = y1 + 80;
          Vector2 vector2_2 = TextItem.doMeasuredSmallLabel(new Vector2((float) x, (float) num3), "Email: " + this.email, new Color?());
          destinationRectangle.X = x + (int) vector2_2.X + 2;
          destinationRectangle.Y = num3;
          destinationRectangle.Width = 7;
          destinationRectangle.Height = 20;
          if ((double) this.os.timer % 1.0 < 0.300000011920929)
            sb.Draw(Utils.white, destinationRectangle, this.os.outlineColor);
          int y2 = num3 + 122;
          if (strArray.Length <= 2 && !Button.doButton(30, x, y2, 300, 22, "Confirm", new Color?(this.os.highlightColor)))
            break;
          if (strArray.Length <= 2)
            this.os.terminal.executeLine();
          this.state = 2;
          break;
        case 2:
          int num4 = num2 + 20;
          TextItem.doSmallLabel(new Vector2((float) x, (float) num4), "Confirm this Email Address :\n" + this.email, new Color?());
          int y3 = num4 + 60;
          if (Button.doButton(21, x, y3, 200, 50, "Confirm Email", new Color?(this.os.highlightColor)))
          {
            if (!Settings.isDemoMode)
              this.sendEmail();
            else
              this.saveEmail();
          }
          if (!Button.doButton(20, x + 220, y3, 200, 50, "Re-Enter Email", new Color?()))
            break;
          this.state = 1;
          this.email = "";
          this.os.execute("getString Email");
          break;
        default:
          int y4 = num2 + (bounds.Height / 2 - 60);
          if (this.state == 3)
            TextItem.doSmallLabel(new Vector2((float) x, (float) (y4 - 60)), "Error - Invalid Email Address", new Color?());
          if (Button.doButton(10, x, y4, 300, 50, "Add Email", new Color?(this.os.highlightColor)))
          {
            this.state = 1;
            this.os.execute("getString Email");
          }
          if (!Button.doButton(12, x, y4 + 55, 300, 20, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
            break;
          this.os.display.command = "connect";
          break;
      }
    }
  }
}
