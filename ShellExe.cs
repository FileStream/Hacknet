// Decompiled with JetBrains decompiler
// Type: Hacknet.ShellExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;

namespace Hacknet
{
  internal class ShellExe : ExeModule
  {
    public static int INFOBAR_HEIGHT = 16;
    public static int BASE_RAM_COST = 40;
    public static float RAM_CHANGE_PS = 200f;
    public static int TRAP_RAM_USE = 100;
    public string destinationIP = "";
    private Computer destComp = (Computer) null;
    private Computer compThisShellIsRunningOn = (Computer) null;
    private int destCompIndex = -1;
    private int state = 0;
    private int targetRamUse = ShellExe.BASE_RAM_COST;
    private const int IDLE_STATE = 0;
    private const int CLOSING_STATE = -1;
    private const int PROXY_OVERLOAD_STATE = 1;
    private const int FORKBOMB_TRAP_STATE = 2;
    private Rectangle infoBar;

    public ShellExe(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.ramCost = ShellExe.BASE_RAM_COST;
      this.IdentifierName = "Shell@" + this.targetIP;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.infoBar = new Rectangle(this.bounds.X, this.bounds.Y, this.bounds.Width, ShellExe.INFOBAR_HEIGHT);
      this.os.shells.Add(this);
      this.os.shellIPs.Add(this.targetIP);
      this.compThisShellIsRunningOn = Programs.getComputer(this.os, this.targetIP);
      this.compThisShellIsRunningOn.log(this.os.thisComputer.ip + "_Opened_#SHELL");
    }

    public override void Update(float t)
    {
      base.Update(t);
      if (!Programs.getComputer(this.os, this.targetIP).adminIP.Equals(this.os.thisComputer.ip) || Programs.getComputer(this.os, this.targetIP).disabled)
      {
        if (this.state != -1)
        {
          this.os.write(">>");
          this.os.write(">> " + string.Format(LocaleTerms.Loc("SHELL ERROR: Administrator account lost on {0}"), (object) this.compThisShellIsRunningOn.ip));
          this.os.write(">>");
        }
        this.Completed();
      }
      if (this.targetRamUse != this.ramCost)
      {
        if (this.targetRamUse < this.ramCost)
        {
          this.ramCost -= (int) ((double) t * (double) ShellExe.RAM_CHANGE_PS);
          if (this.ramCost < this.targetRamUse)
            this.ramCost = this.targetRamUse;
        }
        else
        {
          int num = (int) ((double) t * (double) ShellExe.RAM_CHANGE_PS);
          if (this.os.ramAvaliable >= num)
          {
            this.ramCost += num;
            if (this.ramCost > this.targetRamUse)
              this.ramCost = this.targetRamUse;
          }
        }
      }
      switch (this.state)
      {
        case 1:
          if (!this.destComp.hasProxy)
            break;
          this.destComp.proxyOverloadTicks -= t;
          if ((double) this.destComp.proxyOverloadTicks <= 0.0)
          {
            this.destComp.proxyOverloadTicks = 0.0f;
            this.destComp.proxyActive = false;
            this.completedAction(1);
          }
          else
            this.destComp.hostileActionTaken();
          break;
      }
    }

    public override void Draw(float t)
    {
      string identifierName = this.IdentifierName;
      this.IdentifierName = "@" + this.targetIP;
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("S");
      this.IdentifierName = identifierName;
      this.doGui();
    }

    public void doGui()
    {
      if (this.state == 2 && this.ramCost == this.targetRamUse)
      {
        Color color = this.os.highlightColor;
        if (this.os.opponentLocation.Equals(this.targetIP))
          color = this.os.lockedColor;
        if (Button.doButton(95000 + this.os.exes.IndexOf((ExeModule) this), this.bounds.X + 10, this.bounds.Y + 20, this.bounds.Width - 20, 50, LocaleTerms.Loc("Trigger"), new Color?(color)))
        {
          this.destComp.forkBombClients(this.targetIP);
          this.completedAction(2);
          this.compThisShellIsRunningOn.log("#SHELL_TrapActivate_:_ConnectionsFlooded");
        }
      }
      this.doControlButtons();
    }

    public void StartOverload()
    {
      this.state = 1;
      this.targetRamUse = ShellExe.BASE_RAM_COST;
      this.destinationIP = this.os.connectedComp == null ? this.os.thisComputer.ip : this.os.connectedComp.ip;
      if (this.destComp == null || this.destComp.ip != this.destinationIP)
        this.compThisShellIsRunningOn.log("#SHELL_Overload_@_" + this.destinationIP);
      this.destComp = Programs.getComputer(this.os, this.destinationIP);
      this.destCompIndex = this.os.netMap.nodes.IndexOf(this.destComp);
    }

    public void doControlButtons()
    {
      int num1 = 2;
      int width = 76;
      int num2 = this.bounds.Width - (num1 + 3 * num1 + width - 10);
      int num3 = (int) ((double) num2 * 0.33333);
      int x = this.bounds.X + num1 + 1;
      int height = this.bounds.Height - Module.PANEL_HEIGHT - 8;
      int y = this.bounds.Y + this.bounds.Height - height - 6;
      if (this.bounds.Height - Module.PANEL_HEIGHT <= 5)
        return;
      Button.ForceNoColorTag = true;
      if (this.state == 2)
      {
        int num4 = 50;
        int num5 = 17;
        y += num4 + 9;
        height = num5;
      }
      if (Button.doButton(89200 + this.os.exes.IndexOf((ExeModule) this), x, y, (int) ((double) num2 * 0.5), height, LocaleTerms.Loc("Overload"), new Color?(this.os.shellButtonColor * this.fade)))
        this.StartOverload();
      int num6;
      if (Button.doButton(89300 + this.os.exes.IndexOf((ExeModule) this), num6 = x + ((int) ((double) num2 * 0.5) + num1), y, (int) ((double) num2 * 0.36), height, LocaleTerms.Loc("Trap"), new Color?(this.os.shellButtonColor * this.fade)))
      {
        this.state = 2;
        this.targetRamUse = ShellExe.TRAP_RAM_USE;
        this.destinationIP = this.os.connectedComp == null ? this.os.thisComputer.ip : this.os.connectedComp.ip;
        if (this.destComp == null || this.destComp.ip != this.destinationIP)
          this.compThisShellIsRunningOn.log("#SHELL_TrapAcive");
        this.destComp = Programs.getComputer(this.os, this.destinationIP);
        this.destCompIndex = this.os.netMap.nodes.IndexOf(this.destComp);
      }
      if (Button.doButton(89101 + this.os.exes.IndexOf((ExeModule) this), this.bounds.X + this.bounds.Width - width - (num1 + 1), y, width, height, LocaleTerms.Loc("Close"), new Color?(this.os.lockedColor * this.fade)))
        this.Completed();
      Button.ForceNoColorTag = false;
    }

    public void completedAction(int action)
    {
      this.cancelTarget();
    }

    public void cancelTarget()
    {
      this.state = 0;
      this.destinationIP = "";
      this.destComp = (Computer) null;
      this.destCompIndex = -1;
      this.targetRamUse = ShellExe.BASE_RAM_COST;
    }

    public override void Completed()
    {
      base.Completed();
      this.cancelTarget();
      this.state = -1;
      this.os.shells.Remove(this);
      this.os.shellIPs.Remove(this.targetIP);
      if (!this.isExiting)
        this.compThisShellIsRunningOn.log("#SHELL_Closed");
      this.isExiting = true;
    }

    public void reportedTo(string data)
    {
    }
  }
}
