// Decompiled with JetBrains decompiler
// Type: Hacknet.Clock2Exe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet
{
  internal class Clock2Exe : ExeModule
  {
    private bool isLargeMode = false;
    private Texture2D triangle;
    private Texture2D arc;
    private Texture2D arcThin;
    private Rectangle arcClip;
    private Rectangle arcClipSmaller;

    public Clock2Exe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.name = "Clock v2";
      this.ramCost = 60;
      for (int index = 1; index < p.Length; ++index)
      {
        if (p[index].ToLower() == "-c")
          this.ramCost = 40;
        else if (p[index].ToLower() == "-l")
        {
          this.ramCost = 160;
          this.isLargeMode = true;
        }
      }
      this.IdentifierName = "Clock v2";
      this.targetIP = this.os.thisComputer.ip;
      this.triangle = operatingSystem.content.Load<Texture2D>("DLC/Sprites/Triangle");
      this.arc = operatingSystem.content.Load<Texture2D>("DLC/Sprites/CircleOutlineThick");
      this.arcThin = operatingSystem.content.Load<Texture2D>("CircleOutlineLarge");
      this.arcClip = new Rectangle(this.arc.Width / 4, 0, this.arc.Width / 2, this.arc.Height / 2);
      this.arcClipSmaller = new Rectangle((int) ((double) this.arc.Width * 0.45), 0, (int) ((double) this.arc.Width * 0.1), this.arc.Height / 2);
      this.os.write("Executing ClockV2.exe");
      this.os.write("Additional Arguments: -c / -l");
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      DateTime now = DateTime.Now;
      Rectangle bounds = this.bounds;
      Rectangle fullArea = Utils.InsetRectangle(new Rectangle(bounds.X, bounds.Y, Math.Min(bounds.Width, bounds.Height), Math.Min(bounds.Width, bounds.Height)), 1);
      Rectangle rectangle1 = new Rectangle(bounds.X + fullArea.Width, bounds.Y, bounds.Width - fullArea.Width, bounds.Height);
      this.DrawRadialPointer((float) now.Hour / 24f, fullArea, (float) fullArea.Width / 5f, Color.Gray, false);
      this.DrawRadialPointer((float) now.Minute / 60f, fullArea, (float) fullArea.Width / 3f, this.os.moduleColorStrong, false);
      this.DrawRadialPointer((float) now.Second / 60f, fullArea, (float) fullArea.Width / 2.25f, this.os.highlightColor * 0.5f, true);
      this.DrawRadialPointer((float) now.Millisecond / 1000f, fullArea, (float) fullArea.Width / 2f, this.os.topBarColor * 0.2f, true);
      Rectangle rectangle2 = new Rectangle(rectangle1.X, rectangle1.Y + 1, rectangle1.Width - 1, 9);
      this.spriteBatch.Draw(Utils.gradientLeftRight, rectangle2, this.os.exeModuleTopBar);
      rectangle2.Y += 5;
      TextItem.doRightAlignedBackingLabel(rectangle2, "ClockV2.exe", GuiData.detailfont, Color.Transparent, this.os.exeModuleTitleText);
      if (this.isLargeMode)
      {
        rectangle1 = new Rectangle(this.bounds.X + 80, this.bounds.Y + this.bounds.Height - 41, this.bounds.Width - 81, 40);
        rectangle2 = new Rectangle(rectangle1.X, rectangle1.Y + 1, rectangle1.Width - 1, 9);
      }
      string text = " " + now.ToShortTimeString();
      Rectangle rectangle3 = new Rectangle(rectangle2.X, rectangle2.Y + rectangle2.Height - 5, rectangle2.Width, rectangle1.Height - rectangle2.Height);
      Vector2 vector2 = GuiData.font.MeasureString(text);
      float scale = vector2.X / (float) rectangle3.Width;
      float num = (float) (rectangle3.Height / 2) - (float) ((double) vector2.Y * (double) scale / 2.0);
      Vector2 position = new Vector2((float) rectangle3.X, (float) rectangle3.Y + num);
      for (int index = 0; index < text.Length; ++index)
      {
        this.spriteBatch.DrawString(GuiData.font, string.Concat((object) text[index]), position, Utils.AddativeWhite * 0.8f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
        position.X += (float) rectangle3.Width / (float) text.Length;
      }
    }

    private void DrawRadialPointer(float rotationPercent, Rectangle fullArea, float radius, Color c, bool small = false)
    {
      float rotation = rotationPercent * 6.283185f;
      Vector2 position = new Vector2((float) fullArea.X + (float) fullArea.Width / 2f, (float) fullArea.Y + (float) fullArea.Height / 2f);
      float num = radius / ((float) this.arc.Width / 2f);
      this.spriteBatch.Draw(small ? this.arcThin : this.arc, position, new Rectangle?(), c * 0.2f, rotation, this.arc.GetCentreOrigin(), new Vector2(num), SpriteEffects.None, 0.4f);
      this.spriteBatch.Draw(small ? this.arcThin : this.arc, position, new Rectangle?(this.arcClip), c, rotation, new Vector2((float) (this.arcClip.Width / 2), (float) this.arcClip.Height), new Vector2(num), SpriteEffects.None, 0.4f);
      this.spriteBatch.Draw(this.arcThin, position, new Rectangle?(this.arcClipSmaller), Utils.AddativeWhite * 0.2f, rotation, new Vector2((float) (this.arcClipSmaller.Width / 2), (float) this.arcClipSmaller.Height), new Vector2(num), SpriteEffects.None, 0.4f);
    }
  }
}
