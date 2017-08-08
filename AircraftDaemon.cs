// Decompiled with JetBrains decompiler
// Type: Hacknet.AircraftDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Daemons.Helpers;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Globalization;

namespace Hacknet
{
  internal class AircraftDaemon : Daemon
  {
    public double CurrentAltitude = 37900.0;
    private float currentAirspeed = 460f;
    private float rateOfClimb = 0.073f;
    private Color ThemeColor = Color.CornflowerBlue;
    private bool PilotAlerted = false;
    private bool IsReloadingFirmware = false;
    private float firmwareReloadProgress = 0.0f;
    private float timeFallingFor = 0.0f;
    private float timeSinceLastDataUpdate = 0.0f;
    private bool IsSubscribedForUpdates = false;
    public bool IsInCriticalFirmwareFailure = false;
    public bool AircraftFallStartsImmediatley = true;
    private const float FlightHoursPerLengthUnit = 0.06855416f;
    private const float ReloadFirmwareTime = 6f;
    internal const string CriticalFilename = "747FlightOps.dll";
    private const float RoughTotalFallTimeSeconds = 135f;
    private const float StartingAltitude = 38000f;
    private Texture2D WorldMap;
    private Texture2D Circle;
    private Texture2D Plane;
    private Texture2D CautionIcon;
    private Texture2D StatusOKIcon;
    private Texture2D CircleOutline;
    private Vector2 mapOrigin;
    private Vector2 mapDest;
    private float FlightProgress;
    private Folder MainFolder;
    public Action CrashAction;

    public AircraftDaemon(Computer c, OS os, string name, Vector2 mapOrigin, Vector2 mapDest, float progress)
      : base(c, name, os)
    {
      this.WorldMap = os.content.Load<Texture2D>("DLC/Sprites/SmallWorldMap");
      this.Circle = os.content.Load<Texture2D>("Circle");
      this.StatusOKIcon = os.content.Load<Texture2D>("CircleOutlineLarge");
      this.CircleOutline = os.content.Load<Texture2D>("CircleOutlineLarge");
      this.CautionIcon = os.content.Load<Texture2D>("Sprites/Icons/CautionIcon");
      this.Plane = os.content.Load<Texture2D>("DLC/Sprites/Airplane");
      this.mapOrigin = mapOrigin;
      this.mapDest = mapDest;
      this.FlightProgress = progress;
      this.ThemeColor = os.highlightColor;
    }

    public override void initFiles()
    {
      base.initFiles();
      this.MainFolder = this.comp.files.root.searchForFolder("FlightSystems");
      if (this.MainFolder == null)
      {
        this.MainFolder = new Folder("FlightSystems");
        this.comp.files.root.folders.Add(this.MainFolder);
      }
      this.MainFolder.files.Add(new FileEntry(PortExploits.ValidAircraftOperatingDLL, "747FlightOps.dll"));
      this.MainFolder.files.Add(new FileEntry(Computer.generateBinaryString(200), "InFlightWifiRouter.dll"));
      this.MainFolder.files.Add(new FileEntry(Computer.generateBinaryString(200), "Scheduler.dll"));
      this.MainFolder.files.Add(new FileEntry(Computer.generateBinaryString(200), "EntertainmentServices.dll"));
      this.MainFolder.files.Add(new FileEntry(Computer.generateBinaryString(200), "AnnouncementsSys.dll"));
    }

    public override void loadInit()
    {
      base.loadInit();
      this.MainFolder = this.comp.files.root.searchForFolder("FlightSystems");
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
    }

    public void StartUpdating()
    {
      if (!this.IsSubscribedForUpdates)
        this.os.UpdateSubscriptions += new Action<float>(this.Update);
      this.IsSubscribedForUpdates = true;
    }

    public void UnsubscribeFromUpdates()
    {
      if (!this.IsSubscribedForUpdates)
        return;
      this.os.UpdateSubscriptions -= new Action<float>(this.Update);
    }

    private void Update(float t)
    {
      if (this.IsReloadingFirmware)
      {
        this.firmwareReloadProgress += t;
        if ((double) this.firmwareReloadProgress > 6.0)
          this.FinishReloadingFirmware();
      }
      this.timeSinceLastDataUpdate += t;
      if ((double) this.timeSinceLastDataUpdate <= 0.100000001490116)
        return;
      this.timeSinceLastDataUpdate -= 0.1f;
      t = 0.1f;
      if (this.IsInCriticalFirmwareFailure)
      {
        this.timeFallingFor += t;
        float num = -876.9231f;
        if (this.AircraftFallStartsImmediatley)
        {
          this.rateOfClimb = num;
          this.CurrentAltitude = 38000.0 * (1.0 - (double) (this.timeFallingFor / 135f));
        }
        else
        {
          float val1 = 15f;
          this.rateOfClimb = Math.Max(num, (float) (0.0 - (double) Utils.QuadraticOutCurve(Math.Min(val1, this.timeFallingFor) / val1) * (-1.0 * (double) num)));
          this.CurrentAltitude += (double) this.rateOfClimb * (double) t;
        }
        if (Utils.FloatEquals(this.rateOfClimb, num))
          this.rateOfClimb += (1f - Utils.rand(2f)) * t;
        if ((double) this.currentAirspeed < -1600.0)
          this.currentAirspeed -= (1f - Utils.rand(2f)) * this.rateOfClimb * t;
        else
          this.currentAirspeed -= this.rateOfClimb * t;
      }
      else
      {
        double num1 = 38000.0;
        double num2 = 500.0;
        if ((double) this.currentAirspeed > num2 * 1.25)
          this.currentAirspeed -= (float) ((double) t * (double) this.rateOfClimb * 3.0);
        else
          this.currentAirspeed += (float) ((double) (5 - Utils.random.Next(11)) * (double) t * ((double) this.currentAirspeed > num2 ? -2.0 : 1.0));
        if (this.CurrentAltitude > num1 + 500.0)
          this.CurrentAltitude -= (double) this.rateOfClimb * (double) t + (double) (5 - Utils.random.Next(11)) * (double) t * 2.0 * (this.CurrentAltitude > num1 ? -1.0 : 1.0);
        else
          this.CurrentAltitude += (double) this.rateOfClimb * (double) t + (double) (5 - Utils.random.Next(11)) * (double) t * 2.0 * (this.CurrentAltitude > num1 ? -1.0 : 1.0);
        if ((double) this.rateOfClimb < -0.100000001490116 || this.CurrentAltitude < 37500.0)
        {
          if ((double) this.rateOfClimb < -1.0)
            this.rateOfClimb += (float) (-1.0 * (double) this.rateOfClimb / 2.0) * t;
          else
            this.rateOfClimb += 5f * t;
        }
        else if ((double) this.rateOfClimb > 0.100000001490116)
          this.rateOfClimb -= 1.666667f * t;
        else
          this.rateOfClimb += (Utils.rand(0.1f) - Utils.rand(0.06f)) * t;
      }
      if (this.CurrentAltitude <= 0.0)
        this.CrashAircraft();
    }

    private void CrashAircraft()
    {
      if (this.os.connectedComp == this.comp)
      {
        this.os.execute("disconnect");
        this.os.display.command = "dc";
      }
      if (this.CrashAction != null)
        this.CrashAction();
      this.os.netMap.visibleNodes.Remove(this.os.netMap.nodes.IndexOf(this.comp));
      this.comp.ip = "DCLOC:" + this.comp.ip;
      this.UnsubscribeFromUpdates();
    }

    public void StartReloadFirmware()
    {
      this.StartUpdating();
      this.IsReloadingFirmware = true;
      this.firmwareReloadProgress = 0.0f;
    }

    private void FinishReloadingFirmware()
    {
      this.IsReloadingFirmware = false;
      FileEntry fileEntry = this.MainFolder.searchForFile("747FlightOps.dll");
      if (fileEntry == null || fileEntry.data != PortExploits.ValidAircraftOperatingDLL)
        this.IsInCriticalFirmwareFailure = true;
      else
        this.IsInCriticalFirmwareFailure = false;
    }

    public bool IsInCriticalDescent()
    {
      return (double) this.rateOfClimb < -0.800000011920929 || this.CurrentAltitude < 800.0;
    }

    public override string getSaveString()
    {
      return "<AircraftDaemon " + this.VSString("Name", this.name) + this.VSString("OriginX", this.mapOrigin.X) + this.VSString("OriginY", this.mapOrigin.Y) + this.VSString("DestX", this.mapDest.X) + this.VSString("DestY", this.mapDest.Y) + this.VSString("Progress", this.FlightProgress) + "/>";
    }

    private string VSString(string name, string result)
    {
      return name + "=\"" + result + "\" ";
    }

    private string VSString(string name, float result)
    {
      return name + "=\"" + result.ToString("0.0000", (IFormatProvider) CultureInfo.InvariantCulture) + "\" ";
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      if (!this.IsSubscribedForUpdates)
        this.Update((float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
      Rectangle dest = Utils.InsetRectangle(bounds, 1);
      this.DrawMap(dest, sb);
      this.DrawHeadings(new Rectangle(bounds.X, bounds.Y, (int) ((double) bounds.Width * 0.666), (int) ((double) bounds.Height * 0.666)), sb);
      AircraftAltitudeIndicator.RenderAltitudeIndicator(dest, sb, (int) (this.CurrentAltitude + 0.5), this.IsInCriticalDescent(), AircraftAltitudeIndicator.GetFlashRateFromTimer(OS.currentInstance.timer), 50000, 40000, 30000, 14000, 3000);
    }

    private void DrawHeadings(Rectangle bounds, SpriteBatch sb)
    {
      Rectangle rectangle1 = new Rectangle(bounds.X, bounds.Y + 4, bounds.Width, 40);
      Rectangle dest1 = rectangle1;
      dest1.X += 8;
      dest1.Width -= 8;
      TextItem.doFontLabelToSize(dest1, this.name, GuiData.font, Color.White, true, true);
      rectangle1.Y += rectangle1.Height - 1;
      rectangle1.Height = 1;
      sb.Draw(Utils.white, rectangle1, Color.White);
      Color themeColor = this.ThemeColor;
      rectangle1.Y += 2;
      rectangle1.Height = 20;
      Color patternColor = this.IsInCriticalFirmwareFailure ? Color.DarkRed : themeColor * 0.28f;
      if (!this.IsInCriticalFirmwareFailure && this.PilotAlerted)
        patternColor = this.os.warningColor * 0.5f;
      PatternDrawer.draw(rectangle1, 1f, themeColor * 0.1f, patternColor, sb, PatternDrawer.warningStripe);
      if (this.IsReloadingFirmware)
      {
        Rectangle destinationRectangle = rectangle1;
        destinationRectangle.Width = (int) ((double) destinationRectangle.Width * (double) Utils.QuadraticOutCurve(this.firmwareReloadProgress / 6f));
        sb.Draw(Utils.white, destinationRectangle, Utils.AddativeWhite * 0.4f);
      }
      Rectangle dest2 = Utils.InsetRectangle(rectangle1, 1);
      string text = this.IsReloadingFirmware ? LocaleTerms.Loc("RELOADING FIRMWARE") : (this.IsInCriticalFirmwareFailure ? LocaleTerms.Loc("CRITICAL FIRMWARE FAILURE") : (this.PilotAlerted ? LocaleTerms.Loc("PILOT ALERTED") : LocaleTerms.Loc("FLIGHT IN PROGRESS")));
      TextItem.doCenteredFontLabel(dest2, text, GuiData.font, Color.White, false);
      Rectangle rectangle2 = new Rectangle(dest2.X, dest2.Y + dest2.Height + 8, dest2.Width, 24);
      int num1 = 4;
      int num2 = (rectangle2.Width - num1 * 3) / 3;
      if (Button.doButton(632877701, rectangle2.X, rectangle2.Y, num2 - 20, rectangle2.Height, LocaleTerms.Loc("Disconnect"), new Color?(this.os.lockedColor)))
        this.os.runCommand("disconnect");
      if (Button.doButton(632877703, rectangle2.X + num1 + num2 - 20, rectangle2.Y, num2 + 10 + num1, rectangle2.Height, LocaleTerms.Loc("Pilot Alert"), new Color?(this.ThemeColor)))
        this.PilotAlerted = true;
      if (Button.doButton(632877706, rectangle2.X + num1 * 3 + num2 * 2 - 10, rectangle2.Y, num2 + 10 + num1, rectangle2.Height, LocaleTerms.Loc("Reload Firmware"), new Color?(this.os.lockedColor)))
        this.StartReloadFirmware();
      Rectangle dest3 = new Rectangle(rectangle2.X + 6, rectangle2.Y + rectangle2.Height + 20, rectangle2.Width - 75, 70);
      byte status1 = (double) this.currentAirspeed <= 500.0 ? (byte) 0 : ((double) this.currentAirspeed < 600.0 ? (byte) 1 : (byte) 2);
      this.DrawFieldDisplay(dest3, sb, LocaleTerms.Loc("Air Speed (kn)"), this.currentAirspeed.ToString("0.0"), status1);
      dest3.Y += dest3.Height + 6;
      byte status2 = (double) this.rateOfClimb > -0.200000002980232 ? (byte) 0 : ((double) this.rateOfClimb > -1.0 ? (byte) 1 : (byte) 2);
      this.DrawFieldDisplay(dest3, sb, LocaleTerms.Loc("Rate of Climb (f/s)"), this.rateOfClimb.ToString("0.000"), status2);
      dest3.Y += dest3.Height + 6;
      this.DrawFieldDisplay(dest3, sb, LocaleTerms.Loc("Heading (deg)"), string.Concat((object) 67.228f), (byte) 0);
      dest3.Y += dest3.Height + 6;
    }

    private void DrawFieldDisplay(Rectangle dest, SpriteBatch sb, string title, string value, byte status)
    {
      Rectangle rectangle = new Rectangle(dest.X, dest.Y, dest.Height, dest.Height);
      Texture2D texture = (int) status == 0 ? this.StatusOKIcon : this.CautionIcon;
      Color color = (int) status == 0 ? this.ThemeColor : ((int) status == 1 ? Color.Orange : Color.Red);
      sb.Draw(this.CircleOutline, rectangle, color);
      if ((int) status < 2 || (double) this.os.timer % 0.400000005960464 >= 0.200000002980232)
      {
        bool flag = (int) status != 0;
        Rectangle destinationRectangle = Utils.InsetRectangle(rectangle, rectangle.Width / 5);
        if (flag)
          destinationRectangle.Y -= 2;
        sb.Draw(texture, destinationRectangle, color);
      }
      Rectangle dest1 = new Rectangle(rectangle.X + rectangle.Width + 6, dest.Y, dest.Width - rectangle.Width, dest.Height / 3 - 1);
      TextItem.doFontLabelToSize(dest1, title, GuiData.font, color, true, true);
      Rectangle destinationRectangle1 = new Rectangle(dest1.X - 8, dest1.Y + dest1.Height, dest1.Width + 8, 1);
      dest1.Y += dest1.Height + 1;
      sb.Draw(Utils.white, destinationRectangle1, color);
      dest1.Height = (int) ((double) dest.Height / 3.0 * 2.0) - 1;
      TextItem.doFontLabelToSize(dest1, value, GuiData.font, (int) status == 0 ? Color.White * 0.9f : color, true, true);
    }

    private void DrawMap(Rectangle dest, SpriteBatch sb)
    {
      Rectangle rectangle = Utils.DrawSpriteAspectCorrect(dest, sb, this.WorldMap, Color.Gray, true);
      float num = 10f;
      Vector2 vector2_1 = new Vector2((float) rectangle.X + (float) rectangle.Width * this.mapOrigin.X, (float) rectangle.Y + (float) rectangle.Height * this.mapOrigin.Y);
      Vector2 vector2_2 = new Vector2((float) rectangle.X + (float) rectangle.Width * this.mapDest.X, (float) rectangle.Y + (float) rectangle.Height * this.mapDest.Y);
      sb.Draw(this.Circle, vector2_1, new Rectangle?(), Color.Black, 0.0f, this.Circle.GetCentreOrigin(), new Vector2((num + 3f) / (float) this.Circle.Width), SpriteEffects.None, 0.4f);
      sb.Draw(this.Circle, vector2_1, new Rectangle?(), Utils.AddativeWhite, 0.0f, this.Circle.GetCentreOrigin(), new Vector2(num / (float) this.Circle.Width), SpriteEffects.None, 0.4f);
      sb.Draw(this.Circle, vector2_2, new Rectangle?(), Color.Black, 0.0f, this.Circle.GetCentreOrigin(), new Vector2((num + 3f) / (float) this.Circle.Width), SpriteEffects.None, 0.4f);
      sb.Draw(this.Circle, vector2_2, new Rectangle?(), this.ThemeColor, 0.0f, this.Circle.GetCentreOrigin(), new Vector2(num / (float) this.Circle.Width), SpriteEffects.None, 0.4f);
      Utils.drawLine(sb, vector2_1, vector2_2, Vector2.Zero, this.ThemeColor * 0.5f, 0.3f);
      Vector2 position = Vector2.Lerp(vector2_1, vector2_2, this.FlightProgress);
      Vector2 scale = new Vector2(55f / (float) this.Plane.Width);
      Vector2 vector2_3 = vector2_2 - position;
      float rotation = (float) (Math.Atan2((double) vector2_3.Y, (double) vector2_3.X) + Math.PI / 2.0);
      sb.Draw(this.Plane, position, new Rectangle?(), Color.Black, rotation, this.Plane.GetCentreOrigin(), scale, SpriteEffects.None, 0.4f);
      scale = new Vector2(53f / (float) this.Plane.Width);
      sb.Draw(this.Plane, position, new Rectangle?(), this.IsInCriticalFirmwareFailure ? Color.Red : this.ThemeColor, rotation, this.Plane.GetCentreOrigin(), scale, SpriteEffects.None, 0.4f);
    }
  }
}
