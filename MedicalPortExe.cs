// Decompiled with JetBrains decompiler
// Type: Hacknet.MedicalPortExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;

namespace Hacknet
{
  internal class MedicalPortExe : ExeModule
  {
    private float elapsedTime = 0.0f;
    private float sucsessTimer = 0.0f;
    private Color DarkBaseColor = new Color(5, 0, 36);
    private Color LightBaseColor = new Color(39, 32, 83);
    private Color DarkFinColor = new Color(179, 25, 94);
    private Color LightFinColor = new Color(225, 14, 79);
    private const float RUNTIME = 22f;
    private const float COMPLETE_TIME = 2f;
    private Color[] displayData;
    private bool[] CompletedIndexes;

    public MedicalPortExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.needsProxyAccess = true;
      this.name = "KBT_PortTest";
      this.ramCost = 400;
      this.IdentifierName = "KBT Port Tester";
    }

    public override void Update(float t)
    {
      base.Update(t);
      if ((double) this.sucsessTimer <= 0.0)
      {
        this.elapsedTime += t;
        if ((double) this.elapsedTime >= 22.0)
          this.Complete();
      }
      else
      {
        this.sucsessTimer -= t;
        if ((double) this.sucsessTimer <= 0.0)
          this.isExiting = true;
      }
      if (this.displayData == null)
        this.InitializeDisplay();
      this.UpdateDisplay();
    }

    private void InitializeDisplay()
    {
      this.displayData = new Color[this.bounds.Height - 4 - Module.PANEL_HEIGHT];
      this.CompletedIndexes = new bool[this.displayData.Length];
      for (int index = 0; index < this.displayData.Length; ++index)
        this.CompletedIndexes[index] = false;
    }

    private void UpdateDisplay()
    {
      if ((double) this.elapsedTime % (double) (22f / (float) this.displayData.Length) < 0.0199999995529652)
      {
        int num = 0;
        int index;
        do
        {
          index = Utils.random.Next(this.displayData.Length);
          ++num;
        }
        while (this.CompletedIndexes[index] && num < this.bounds.Height * 2);
        this.CompletedIndexes[index] = true;
      }
      int index1 = Utils.random.Next(this.displayData.Length);
      if (this.CompletedIndexes[index1])
        this.displayData[index1] = Color.Lerp(this.displayData[index1], this.LightBaseColor, Utils.rand(0.5f));
      for (int index2 = 0; index2 < this.displayData.Length; ++index2)
        this.displayData[index2] = !this.CompletedIndexes[index2] ? Color.Lerp(this.displayData[index2], Color.Lerp(this.DarkBaseColor, this.LightBaseColor, Utils.rand(1f)), 0.05f) : Color.Lerp(this.displayData[index2], Color.Lerp(this.DarkFinColor, this.LightFinColor, Utils.rand(1f)), 0.05f);
    }

    private void Complete()
    {
      Computer computer = Programs.getComputer(this.os, this.targetIP);
      if (computer != null)
        computer.openPort(104, this.os.thisComputer.ip);
      this.sucsessTimer = 2f;
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      if (this.displayData == null)
        return;
      Rectangle bounds = this.bounds;
      ++bounds.X;
      bounds.Width -= 2;
      bounds.Y += 2 + Module.PANEL_HEIGHT;
      bounds.Height = 1;
      int width = bounds.Width;
      for (int index = 0; index < this.displayData.Length; ++index)
      {
        float range = (float) (0.850000023841858 * (1.0 - (double) this.elapsedTime / 22.0));
        float num = 0.95f - range;
        bounds.Width = !this.CompletedIndexes[index] ? width : (int) ((double) width * ((double) Utils.rand(range) + (double) num));
        this.spriteBatch.Draw(Utils.white, bounds, this.displayData[index]);
        ++bounds.Y;
        if (bounds.Y > this.bounds.Y + this.bounds.Height - 2)
          break;
      }
    }
  }
}
