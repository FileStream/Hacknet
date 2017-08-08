// Decompiled with JetBrains decompiler
// Type: Hacknet.EOSDeviceScannerExe
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class EOSDeviceScannerExe : ExeModule
  {
    private List<Vector2> locations = new List<Vector2>();
    private float timeToNextBounce = 0.0f;
    private List<string> ResultTitles = new List<string>();
    private List<string> ResultBodies = new List<string>();
    private float timer = 0.0f;
    private bool IsComplete = false;
    private int devicesFound = 0;
    private bool isError = false;
    private string errorMessage = (string) null;
    private const float TOTAL_TIME = 8f;
    private const float SHORTCUT_TIME = 3.5f;
    private const float timeBetweenBounces = 0.07f;
    private Computer targetComp;

    public EOSDeviceScannerExe(Rectangle location, OS operatingSystem, string[] p)
      : base(location, operatingSystem)
    {
      this.name = "eOS_DeviceScanner";
      this.ramCost = 300;
      this.IdentifierName = "eOS Device Scanner";
      this.targetComp = this.os.connectedComp;
      if (this.targetComp == null)
        this.targetComp = this.os.thisComputer;
      this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
      this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
      if (this.os.hasConnectionPermission(true))
        return;
      this.isError = true;
      this.errorMessage = LocaleTerms.Loc("ADMIN ACCESS\nREQUIRED FOR SCAN");
      this.IsComplete = true;
      for (int index = 0; index < 30; ++index)
        this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
    }

    public override void Update(float t)
    {
      base.Update(t);
      this.timer += t;
      if (((double) this.timer > 8.0 || this.targetComp.attatchedDeviceIDs == null && (double) this.timer > 3.5) && !this.IsComplete)
        this.Completed();
      if ((double) this.timer >= 8.0 || this.errorMessage != null)
        return;
      this.timeToNextBounce -= t;
      if ((double) this.timeToNextBounce <= 0.0)
      {
        this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
        this.timeToNextBounce = 0.07f;
      }
    }

    public override void Completed()
    {
      base.Completed();
      this.IsComplete = true;
      if (this.targetComp.attatchedDeviceIDs != null)
      {
        string[] strArray = this.targetComp.attatchedDeviceIDs.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
        float num = 0.0f;
        for (int index = 0; index < strArray.Length; ++index)
        {
          Computer device = Programs.getComputer(this.os, strArray[index]);
          if (device != null)
          {
            Action action = (Action) (() =>
            {
              this.os.netMap.discoverNode(device);
              Vector2 loc = this.os.netMap.GetNodeDrawPos(device) + new Vector2((float) this.os.netMap.bounds.X, (float) this.os.netMap.bounds.Y) + new Vector2((float) (NetworkMap.NODE_SIZE / 2));
              SFX.addCircle(loc, this.os.highlightColor, 120f);
              this.os.delayer.Post(ActionDelayer.Wait(0.2), (Action) (() => SFX.addCircle(loc, this.os.highlightColor, 80f)));
              this.os.delayer.Post(ActionDelayer.Wait(0.4), (Action) (() => SFX.addCircle(loc, this.os.highlightColor, 65f)));
              this.os.write(string.Format(LocaleTerms.Loc("eOS Device \"{0}\" opened for connection at {1}"), (object) device.name, (object) device.ip));
              this.ResultTitles.Add(device.name);
              this.ResultBodies.Add(device.ip + " " + device.location.ToString() + "\n" + Guid.NewGuid().ToString());
            });
            this.os.delayer.Post(ActionDelayer.Wait((double) num), action);
            ++num;
            ++this.devicesFound;
          }
        }
      }
      if (this.devicesFound != 0)
        return;
      this.isError = true;
    }

    public override void Draw(float t)
    {
      base.Draw(t);
      this.drawOutline();
      this.drawTarget("app:");
      bool drawShadow = TextItem.DrawShadow;
      TextItem.DrawShadow = false;
      Vector2 location = this.locations[0];
      Vector2 vector2_1 = new Vector2((float) this.bounds.X + 2f, (float) this.bounds.Y + 26f);
      Vector2 vector2_2 = new Vector2((float) this.bounds.Width - 4f, (float) this.bounds.Height - 30f);
      if ((double) vector2_2.X > 0.0 && (double) vector2_2.Y > 0.0)
      {
        for (int index = 1; index < this.locations.Count; ++index)
        {
          Vector2 vector2_3 = this.locations[index];
          if (index == this.locations.Count - 1)
            vector2_3 = Vector2.Lerp(location, vector2_3, (float) (1.0 - (double) this.timeToNextBounce / 0.0700000002980232));
          Utils.drawLine(this.spriteBatch, vector2_1 + location * vector2_2, vector2_1 + vector2_3 * vector2_2, Vector2.Zero, (this.isError ? Utils.AddativeRed : Utils.AddativeWhite) * 0.5f * ((float) index / (float) this.locations.Count), 0.4f);
          location = this.locations[index];
        }
        for (int index = 1; index < this.locations.Count; ++index)
          this.spriteBatch.Draw(Utils.white, this.locations[index] * vector2_2 + vector2_1, Utils.AddativeWhite);
      }
      SpriteFont font = Settings.ActiveLocale == "en-us" ? GuiData.titlefont : GuiData.font;
      if (this.IsComplete)
      {
        bool flag = this.errorMessage != null;
        Rectangle destinationRectangle1 = new Rectangle(this.bounds.X + 1, this.bounds.Y + 26, this.bounds.Width - 2, this.bounds.Height - 28);
        this.spriteBatch.Draw(Utils.white, destinationRectangle1, Color.Black * 0.7f);
        Rectangle rectangle = destinationRectangle1;
        rectangle.Height = Math.Min(35, this.bounds.Height / 5);
        string str = LocaleTerms.Loc(flag ? "ERROR" : "SCAN COMPLETE");
        TextItem.doFontLabel(new Vector2((float) rectangle.X, (float) rectangle.Y), str, font, new Color?(this.os.highlightColor), (float) rectangle.Width, (float) rectangle.Height, false);
        TextItem.doFontLabel(new Vector2((float) rectangle.X, (float) rectangle.Y), Utils.FlipRandomChars(str, 0.1), font, new Color?(Utils.AddativeWhite * (0.1f * Utils.rand())), (float) rectangle.Width, (float) rectangle.Height, false);
        Rectangle destinationRectangle2 = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width, 1);
        this.spriteBatch.Draw(Utils.white, destinationRectangle2, Utils.AddativeWhite * 0.5f);
        if (!this.isExiting)
        {
          string text = string.Format(LocaleTerms.Loc("DEVICES FOUND : {0}"), (object) this.devicesFound);
          if (flag)
            text = this.errorMessage;
          Vector2 pos = new Vector2((float) (rectangle.X + 2), (float) (rectangle.Y + rectangle.Height + 2));
          TextItem.doFontLabel(pos, text, font, new Color?((this.devicesFound > 0 ? Utils.AddativeWhite : Color.Red) * 0.8f), (float) (this.bounds.Width - 10), flag ? (float) this.bounds.Height * 0.8f : 23f, false);
          pos.Y += 25f;
          for (int index = 0; index < this.ResultTitles.Count && (double) pos.Y - (double) this.bounds.Y + 60.0 <= (double) this.bounds.Height; ++index)
          {
            TextItem.doFontLabel(pos, Utils.FlipRandomChars(this.ResultTitles[index], 0.01), GuiData.font, new Color?(Color.Lerp(this.os.highlightColor, Utils.AddativeWhite, (float) (0.200000002980232 + 0.100000001490116 * (double) Utils.rand()))), (float) (this.bounds.Width - 10), 24f, false);
            pos.Y += 22f;
            TextItem.doFontLabel(pos, this.ResultBodies[index], GuiData.detailfont, new Color?(Utils.AddativeWhite * 0.85f), (float) (this.bounds.Width - 10), 30f, false);
            pos.Y += 30f;
            pos.Y += 4f;
          }
        }
        if (!this.isExiting && Button.doButton(646464029 + this.PID, this.bounds.X + 2, this.bounds.Y + this.bounds.Height - 2 - 20, this.bounds.Width - 50, 20, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
          this.isExiting = true;
      }
      else
      {
        int height = Math.Min(38, this.bounds.Height / 3);
        Rectangle rectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + this.bounds.Height / 2 - height / 2, this.bounds.Width - 2, height);
        this.spriteBatch.Draw(Utils.white, rectangle, Color.Black * 0.7f);
        TextItem.doFontLabelToSize(rectangle, Utils.FlipRandomChars(LocaleTerms.Loc("SCANNING"), 0.009), font, this.IsComplete ? this.os.highlightColor : Utils.AddativeWhite * 0.8f, false, false);
        TextItem.doFontLabelToSize(rectangle, Utils.FlipRandomChars(LocaleTerms.Loc("SCANNING"), 0.15), font, this.IsComplete ? this.os.highlightColor : Utils.AddativeWhite * (0.18f * Utils.rand()), false, false);
      }
      TextItem.DrawShadow = drawShadow;
    }
  }
}
