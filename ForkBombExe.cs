// Decompiled with JetBrains decompiler
// Type: Hacknet.ForkBombExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;

namespace Hacknet
{
  internal class ForkBombExe : ExeModule
  {
    public static float RAM_CHANGE_PS = 150f;
    public static string binary = "";
    private int targetRamUse = 999999999;
    public string runnerIP = "";
    public int binaryScroll = 0;
    public int charsWide = 0;
    public bool frameSwitch = false;

    public ForkBombExe(Rectangle location, OS operatingSystem)
      : base(location, operatingSystem)
    {
      this.ramCost = 10;
      this.runnerIP = "UNKNOWN";
      this.IdentifierName = "ForkBomb";
    }

    public ForkBombExe(Rectangle location, OS operatingSystem, string ipFrom)
      : base(location, operatingSystem)
    {
      this.ramCost = 10;
      this.runnerIP = ipFrom;
      this.IdentifierName = "ForkBomb";
    }

    public override void LoadContent()
    {
      base.LoadContent();
      if (ForkBombExe.binary.Equals(""))
        ForkBombExe.binary = Computer.generateBinaryString(5064);
      this.charsWide = (int) ((double) this.bounds.Width / (double) (GuiData.detailfont.MeasureString("0").X - 0.15f) + 0.5);
    }

    public override void Killed()
    {
      TrackerCompleteSequence.NextCompleteForkbombShouldTrace = false;
      base.Killed();
    }

    public override void Update(float t)
    {
      base.Update(t);
      if (this.frameSwitch)
      {
        this.binaryScroll = this.binaryScroll + 1;
        if (this.binaryScroll >= ForkBombExe.binary.Length - (this.charsWide + 1))
          this.binaryScroll = 0;
      }
      this.frameSwitch = !this.frameSwitch;
      if (this.targetRamUse == this.ramCost)
        return;
      int num = (int) ((double) t * (double) ForkBombExe.RAM_CHANGE_PS);
      if (this.os.ramAvaliable < num)
      {
        this.Completed();
      }
      else
      {
        this.ramCost += num;
        if (this.ramCost > this.targetRamUse)
          this.ramCost = this.targetRamUse;
      }
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      float num = 8f;
      int startIndex = this.binaryScroll;
      if (startIndex >= ForkBombExe.binary.Length - (this.charsWide + 1))
        startIndex = 0;
      Vector2 position = new Vector2((float) this.bounds.X, (float) this.bounds.Y);
      while ((double) position.Y < (double) (this.bounds.Y + this.bounds.Height) - (double) num)
      {
        this.spriteBatch.DrawString(GuiData.detailfont, ForkBombExe.binary.Substring(startIndex, this.charsWide), position, Color.White);
        startIndex += this.charsWide;
        if (startIndex >= ForkBombExe.binary.Length - (this.charsWide + 1))
          startIndex = 0;
        position.Y += num;
      }
    }

    public override void Completed()
    {
      base.Completed();
      if (TrackerCompleteSequence.NextCompleteForkbombShouldTrace)
      {
        TrackerCompleteSequence.NextCompleteForkbombShouldTrace = false;
        TrackerCompleteSequence.TriggerETAS((object) this.os);
        this.os.exes.Remove((ExeModule) this);
      }
      else
        this.os.thisComputer.crash(this.runnerIP);
    }
  }
}
