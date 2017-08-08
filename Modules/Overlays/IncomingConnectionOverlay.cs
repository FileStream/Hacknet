// Decompiled with JetBrains decompiler
// Type: Hacknet.Modules.Overlays.IncomingConnectionOverlay
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Modules.Overlays
{
  public class IncomingConnectionOverlay
  {
    private static Color DrawColor = new Color(290, 0, 0, 0);
    public bool IsActive = false;
    private float timeElapsed = 0.0f;
    private const float DURATION = 6f;
    private Texture2D CautionSign;
    private Texture2D CautionSignBG;
    private SoundEffect sound1;
    private SoundEffect sound2;

    public IncomingConnectionOverlay(object OSobj)
    {
      this.CautionSign = ((OS) OSobj).content.Load<Texture2D>("Sprites/Icons/CautionIcon");
      this.CautionSignBG = ((OS) OSobj).content.Load<Texture2D>("Sprites/Icons/CautionIconBG");
      this.sound1 = ((OS) OSobj).content.Load<SoundEffect>("SFX/DoomShock");
      this.sound2 = ((OS) OSobj).content.Load<SoundEffect>("SFX/BrightFlash");
    }

    public void Activate()
    {
      this.IsActive = true;
      this.timeElapsed = 0.0f;
      this.sound1.Play();
      this.sound2.Play();
    }

    public void Update(float dt)
    {
      this.timeElapsed += dt;
      if ((double) this.timeElapsed <= 6.0)
        return;
      this.IsActive = false;
    }

    public void Draw(Rectangle dest, SpriteBatch sb)
    {
      if (!this.IsActive)
        return;
      float timeElapsed = this.timeElapsed;
      if ((double) this.timeElapsed > 5.5)
        timeElapsed -= 5.5f;
      if ((double) timeElapsed <= 0.5 && (double) timeElapsed % 0.100000001490116 < 0.0500000007450581)
        return;
      int height1 = 120;
      float num1 = this.timeElapsed / 6f;
      float num2 = 1f;
      float num3 = 0.2f;
      if ((double) this.timeElapsed < (double) num3)
      {
        num2 = this.timeElapsed / num3;
        height1 = (int) ((double) height1 * ((double) this.timeElapsed / (double) num3));
      }
      else if ((double) this.timeElapsed > 6.0 - (double) num3)
      {
        float num4 = (float) (1.0 - ((double) this.timeElapsed - (6.0 - (double) num3)));
        num2 = num4;
        height1 = (int) ((double) height1 * (double) num4);
      }
      Rectangle destinationRectangle = new Rectangle(dest.X, dest.Y + dest.Height / 2 - height1 / 2, dest.Width, height1);
      sb.Draw(Utils.white, destinationRectangle, Color.Black * 0.9f * num2);
      string text1 = "INCOMING CONNECTION";
      string text2 = "External unsyndicated UDP traffic on port 22\nLogging all activity to ~/log";
      int num5 = dest.Width / 3;
      int height2 = (int) (24.0 * (double) num2);
      Rectangle dest1 = new Rectangle(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, height2);
      PatternDrawer.draw(dest1, 1f, Color.Transparent, IncomingConnectionOverlay.DrawColor, sb, PatternDrawer.warningStripe);
      dest1.Y += height1 - height2;
      PatternDrawer.draw(dest1, 1f, Color.Transparent, IncomingConnectionOverlay.DrawColor, sb, PatternDrawer.warningStripe);
      int width1 = 700;
      Rectangle rectangle1 = new Rectangle(destinationRectangle.X + destinationRectangle.Width / 2 - width1 / 2, destinationRectangle.Y, width1, destinationRectangle.Height);
      int width2 = (int) ((double) this.CautionSign.Width / (double) this.CautionSign.Height * (double) height1);
      Rectangle rectangle2 = new Rectangle(rectangle1.X, rectangle1.Y, width2, height1);
      rectangle2 = Utils.InsetRectangle(rectangle2, -30);
      sb.Draw(this.CautionSignBG, rectangle2, Color.Black * num2);
      int num6 = 4;
      rectangle2 = new Rectangle(rectangle2.X + num6, rectangle2.Y + num6, rectangle2.Width - num6 * 2, rectangle2.Height - num6 * 2);
      sb.Draw(this.CautionSign, rectangle2, Color.Lerp(Color.Red, IncomingConnectionOverlay.DrawColor, (float) (0.949999988079071 + 0.0500000007450581 * (double) Utils.rand())));
      Rectangle dest2 = new Rectangle(rectangle1.X + rectangle2.Width + 2 * num6 - 18, rectangle1.Y + 4, rectangle1.Width - (rectangle2.Width + num6 * 2) + 20, (int) ((double) rectangle1.Height * 0.8));
      TextItem.doFontLabelToSize(dest2, text1, GuiData.titlefont, IncomingConnectionOverlay.DrawColor, false, false);
      dest2.Y += dest2.Height - 27;
      dest2.Height = (int) ((double) rectangle1.Height * 0.2);
      TextItem.doFontLabelToSize(dest2, text2, GuiData.detailfont, IncomingConnectionOverlay.DrawColor * num2, false, false);
    }
  }
}
