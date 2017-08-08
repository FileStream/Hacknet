// Decompiled with JetBrains decompiler
// Type: Hacknet.UploadServerDaemon
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
  internal class UploadServerDaemon : AuthenticatingDaemon
  {
    private static string MESSAGE_FILE_DATA = (string) null;
    private static Vector2 ARROW_VELOCITY = new Vector2(0.0f, 400f);
    private UploadServerDaemon.UploadServerState state = UploadServerDaemon.UploadServerState.Menu;
    public bool hasReturnViewButton = false;
    private float uploadDetectedEffectTimer = 0.0f;
    private const string DEFAULT_FOLDERNAME = "Drop";
    private const string STORAGE_FOLDER_FOLDERNAME = "Uploads";
    private const string MESSAGE_FILENAME = "Server_Message.txt";
    private const int NUMBER_OF_ARROWS = 90;
    private const float MAX_FADE_TIME = 2f;
    private Texture2D arrow;
    private Color themeColor;
    private Color darkThemeColor;
    private Color lightThemeColor;
    private Folder root;
    private Folder storageFolder;
    public string Foldername;
    public bool needsAuthentication;
    private int uploadFileCountLastFrame;
    private List<Vector2> arrowPositions;
    private List<float> arrowFades;
    private List<float> arrowDepths;

    public UploadServerDaemon(Computer computer, string serviceName, Color themeColor, OS opSystem, string foldername = null, bool needsAuthentication = false)
      : base(computer, serviceName, opSystem)
    {
      this.arrow = this.os.content.Load<Texture2D>("Arrow");
      this.themeColor = themeColor;
      this.lightThemeColor = Color.Lerp(themeColor, Color.White, 0.4f);
      this.darkThemeColor = Color.Lerp(themeColor, Color.Black, 0.8f);
      this.Foldername = foldername;
      if (this.Foldername == null)
        this.Foldername = "Drop";
      this.needsAuthentication = needsAuthentication;
      UploadServerDaemon.MESSAGE_FILE_DATA = Utils.readEntireFile("Content/LocPost/UploadServerText.txt");
    }

    public override string getSaveString()
    {
      return "<UploadServerDaemon name=\"" + this.name + "\" foldername=\"" + this.Foldername + "\" color=\"" + Utils.convertColorToParseableString(this.themeColor) + "\" needsAuth=\"" + (object) this.needsAuthentication + "\" hasReturnViewButton=\"" + (object) this.hasReturnViewButton + "\" />";
    }

    public override void initFiles()
    {
      base.initFiles();
      this.root = this.comp.files.root.searchForFolder(this.Foldername);
      if (this.root == null)
      {
        this.root = new Folder(this.Foldername);
        this.comp.files.root.folders.Add(this.root);
      }
      this.storageFolder = this.root.searchForFolder("Uploads");
      if (this.storageFolder == null)
      {
        this.storageFolder = new Folder("Uploads");
        this.root.folders.Add(this.storageFolder);
      }
      this.root.files.Add(new FileEntry(UploadServerDaemon.MESSAGE_FILE_DATA, "Server_Message.txt"));
      this.uploadFileCountLastFrame = this.storageFolder.files.Count;
    }

    public override void loadInit()
    {
      base.loadInit();
      this.root = this.comp.files.root.searchForFolder(this.Foldername);
      this.storageFolder = this.root.searchForFolder("Uploads");
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      if (!this.needsAuthentication)
        this.moveToActiveState();
      else
        this.state = UploadServerDaemon.UploadServerState.Menu;
    }

    private void moveToActiveState()
    {
      this.state = UploadServerDaemon.UploadServerState.Active;
      Programs.sudo(this.os, (Action) (() =>
      {
        Programs.cd(new string[2]
        {
          "cd",
          this.Foldername + "/Uploads"
        }, this.os);
        this.os.display.command = this.name;
      }));
      if (this.comp.userLoggedIn)
        return;
      this.comp.userLoggedIn = true;
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      this.drawBackground(bounds, sb);
      int num1 = bounds.X + 10;
      int num2 = bounds.Y + 10;
      TextItem.doFontLabel(new Vector2((float) num1, (float) num2), this.name, GuiData.font, new Color?(), (float) (bounds.Width - 20), (float) bounds.Height, false);
      int num3 = num2 + ((int) GuiData.font.MeasureString(this.name).Y + 2);
      string text = "Error: System.IO.FileNotFoundException went unhandled\n" + "File Server_Message.txt not found\n" + "at UploadDaemonCore.RenderModule.HelptextDisplay.cs\nline 107 position 95";
      FileEntry fileEntry = this.root.searchForFile("Server_Message.txt");
      if (fileEntry != null)
        text = fileEntry.data;
      Vector2 vector2 = TextItem.doMeasuredFontLabel(new Vector2((float) num1, (float) num3), text, GuiData.tinyfont, new Color?(), float.MaxValue, float.MaxValue);
      if (!this.hasReturnViewButton || !Button.doButton(50821549, num1 - 2, (int) ((double) num3 + (double) vector2.Y + 8.0), bounds.Width / 3, 26, "Exit Upload View", new Color?(this.themeColor)))
        return;
      this.os.display.command = "connect";
    }

    public void drawBackground(Rectangle bounds, SpriteBatch sb)
    {
      bounds = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
      sb.Draw(Utils.gradient, bounds, this.themeColor);
      sb.Draw(this.arrow, new Rectangle(bounds.X, bounds.Y + bounds.Height / 3, bounds.Width / 2, (int) ((double) bounds.Height * 0.666660010814667) + 1), new Rectangle?(new Rectangle(this.arrow.Width / 2, 0, this.arrow.Width / 2, this.arrow.Height)), this.darkThemeColor);
      Vector2 vector2_1;
      if (this.arrowPositions == null)
      {
        this.arrowPositions = new List<Vector2>();
        this.arrowFades = new List<float>();
        this.arrowDepths = new List<float>();
        for (int index = 0; index < 90; ++index)
        {
          float num = 70f;
          vector2_1 = new Vector2((float) (-(double) num + Utils.random.NextDouble() * ((double) num + (double) bounds.Width)), (float) bounds.Height / (float) (Utils.random.NextDouble() * 2.0));
          this.arrowPositions.Add(vector2_1);
          this.arrowFades.Add((float) (0.100000001490116 + 0.899999998509884 * Utils.random.NextDouble()));
          this.arrowDepths.Add((float) (0.200000002980232 + (Utils.random.NextDouble() - 0.2)));
        }
      }
      float totalSeconds = (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
      for (int index1 = 0; index1 < this.arrowPositions.Count; ++index1)
      {
        this.arrowPositions[index1] -= totalSeconds * UploadServerDaemon.ARROW_VELOCITY * this.arrowDepths[index1];
        List<float> arrowFades;
        int index2;
        (arrowFades = this.arrowFades)[index2 = index1] = arrowFades[index2] - totalSeconds / 2f;
        if ((double) this.arrowFades[index1] < 0.0)
        {
          float num = 70f;
          vector2_1 = new Vector2((float) (-(double) num + Utils.random.NextDouble() * ((double) num + (double) bounds.Width)), (float) bounds.Height / (float) (Utils.random.NextDouble() * 2.0));
          this.arrowPositions[index1] = vector2_1;
          this.arrowFades[index1] = (float) (0.5 + 1.5 * Utils.random.NextDouble());
          this.arrowDepths[index1] = (float) (0.200000002980232 + Utils.random.NextDouble() * 0.4);
        }
        float scale = 0.3f * this.arrowDepths[index1];
        Vector2 pos = new Vector2((float) bounds.X + this.arrowPositions[index1].X, (float) (bounds.Y + bounds.Height) + this.arrowPositions[index1].Y);
        Rectangle rectForSpritePos = Utils.getClipRectForSpritePos(bounds, this.arrow, pos, scale);
        Vector2 vector2_2 = new Vector2((float) rectForSpritePos.X, (float) rectForSpritePos.Y);
        pos += vector2_2;
        Rectangle destinationRectangle = new Rectangle((int) pos.X, (int) pos.Y, (int) ((double) scale * (double) (rectForSpritePos.Width - rectForSpritePos.X)), (int) ((double) scale * (double) (rectForSpritePos.Height - rectForSpritePos.Y)));
        sb.Draw(this.arrow, destinationRectangle, new Rectangle?(rectForSpritePos), this.lightThemeColor * (this.arrowDepths[index1] * 1.5f) * this.arrowFades[index1], 0.0f, Vector2.Zero, SpriteEffects.None, this.arrowDepths[index1]);
      }
      this.drawUploadDetectedEffect(bounds, sb);
    }

    public void drawUploadDetectedEffect(Rectangle bounds, SpriteBatch sb)
    {
      this.uploadDetectedEffectTimer -= (float) this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
      if (this.uploadFileCountLastFrame < this.storageFolder.files.Count)
      {
        this.uploadDetectedEffectTimer = 4f;
        this.uploadFileCountLastFrame = this.storageFolder.files.Count;
      }
      if ((double) this.uploadDetectedEffectTimer <= 0.0)
        return;
      Color white = Color.White;
      white.A = (byte) 0;
      white *= this.uploadDetectedEffectTimer / 4f;
      bool drawShadow = TextItem.DrawShadow;
      TextItem.DrawShadow = false;
      TextItem.doFontLabel(new Vector2((float) (bounds.X + bounds.Width / 3), (float) (bounds.Y + bounds.Height - 60)), "UPLOAD COMPLETE", GuiData.titlefont, new Color?(white), (float) (bounds.Width / 3 * 2), 58f, false);
      TextItem.DrawShadow = drawShadow;
    }

    private enum UploadServerState
    {
      Menu,
      Login,
      Active,
    }
  }
}
