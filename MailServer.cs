// Decompiled with JetBrains decompiler
// Type: Hacknet.MailServer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Daemons.Helpers;
using Hacknet.Gui;
using Hacknet.Localization;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class MailServer : Daemon
  {
    public static bool shouldGenerateJunk = true;
    private static string emailSplitDelimiter = "@*&^#%@)_!_)*#^@!&*)(#^&\n";
    private static string[] emailSplitDelims = new string[1]{ MailServer.emailSplitDelimiter };
    private static string[] spaceDelim = new string[1]{ "#%#" };
    private int inboxPage = 0;
    private int totalPagesDetected = -1;
    private Color themeColor = new Color(125, 5, 6);
    public bool shouldGenerateJunkEmails = true;
    private List<string> emailReplyStrings = new List<string>();
    private bool addingNewReplyString = false;
    public const int TYPE = 0;
    public const int SENDER = 1;
    public const int SUBJECT = 2;
    public const int BODY = 3;
    public const int ATTACHMENT = 4;
    public const int UNREAD = 0;
    public const int READ = 1;
    public const float MISSION_COMPLETE_FLASH_TIME = 3f;
    private Folder root;
    private Folder accounts;
    private Folder userFolder;
    private List<int> rootPath;
    private List<int> accountsPath;
    private UserDetail user;
    private FileEntry selectedEmail;
    private int state;
    public Color textColor;
    private string[] emailData;
    public Color evenLine;
    public Color oddLine;
    public Color senderDarkeningColor;
    public Color seperatorLineColor;
    private List<MailResponder> responders;
    public Texture2D panel;
    public Texture2D corner;
    public Texture2D unopenedIcon;
    private Rectangle panelRect;
    private bool missionIncompleteReply;
    private static SoundEffect buttonSound;
    private ScrollableSectionedPanel sectionedPanel;
    public Action setupComplete;

    public MailServer(Computer c, string name, OS os)
      : base(c, name, os)
    {
      this.state = 0;
      this.panel = os.content.Load<Texture2D>("Panel");
      this.corner = os.content.Load<Texture2D>("Corner");
      this.unopenedIcon = os.content.Load<Texture2D>("UnopenedMail");
      MailServer.buttonSound = os.content.Load<SoundEffect>("SFX/Bip");
      this.panelRect = new Rectangle();
      this.evenLine = new Color(80, 81, 83);
      this.oddLine = new Color(58, 58, 58);
      this.senderDarkeningColor = new Color(0, 0, 0, 100);
      this.seperatorLineColor = Color.Transparent;
      this.textColor = Color.White;
      this.responders = new List<MailResponder>();
    }

    public override void initFiles()
    {
      base.initFiles();
      this.initFilesystem();
      for (int index = 0; index < 10; ++index)
        this.comp.users.Add(new UserDetail(UsernameGenerator.getName()));
      for (int index = 0; index < this.comp.users.Count; ++index)
      {
        UserDetail user = this.comp.users[index];
        if ((int) user.type == 1 || (int) user.type == 0 || (int) user.type == 2)
        {
          Folder folder = new Folder(user.name);
          folder.files.Add(new FileEntry("Username: " + user.name + "\nPassword: " + user.pass, "AccountInfo"));
          Folder f = new Folder("inbox");
          if (this.shouldGenerateJunkEmails && user.name != this.os.defaultUser.name)
            this.addJunkEmails(f);
          folder.folders.Add(f);
          folder.folders.Add(new Folder("sent"));
          this.accounts.folders.Add(folder);
        }
      }
      if (this.setupComplete == null)
        return;
      this.setupComplete();
    }

    public override void loadInit()
    {
      base.loadInit();
      this.root = this.comp.files.root.searchForFolder("mail");
      this.accounts = this.root.searchForFolder("accounts");
    }

    public void addJunkEmails(Folder f)
    {
      if (!MailServer.shouldGenerateJunk)
        return;
      int num = Utils.random.Next(10);
      for (int index = 0; index < num; ++index)
        f.files.Add(new FileEntry(MailServer.generateEmail("Re: Junk", BoatMail.JunkEmail, "admin@" + this.comp.name), "Re:_Junk#" + (object) OS.currentElapsedTime));
    }

    public void initFilesystem()
    {
      this.rootPath = new List<int>();
      this.accountsPath = new List<int>();
      this.root = this.comp.files.root.searchForFolder("mail");
      if (this.root == null)
      {
        this.root = new Folder("mail");
        this.comp.files.root.folders.Add(this.root);
      }
      this.rootPath.Add(this.comp.files.root.folders.IndexOf(this.root));
      this.accountsPath.Add(this.comp.files.root.folders.IndexOf(this.root));
      this.accounts = new Folder("accounts");
      this.accountsPath.Add(0);
      this.root.folders.Add(this.accounts);
    }

    public void setThemeColor(Color newThemeColor)
    {
      this.themeColor = newThemeColor;
    }

    public void addResponder(MailResponder resp)
    {
      this.responders.Add(resp);
    }

    public void removeResponder(MailResponder resp)
    {
      this.responders.Remove(resp);
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      this.state = 0;
      this.inboxPage = 0;
      this.totalPagesDetected = -1;
    }

    public void viewInbox(UserDetail newUser)
    {
      this.userFolder = (Folder) null;
      this.state = 3;
      for (int index1 = 0; index1 < this.comp.users.Count; ++index1)
      {
        if (this.comp.users[index1].name.Equals(newUser.name))
        {
          this.user = this.comp.users[index1];
          for (int index2 = 0; index2 < this.accounts.folders.Count; ++index2)
          {
            if (this.accounts.folders[index2].name.Equals(this.user.name))
            {
              this.userFolder = this.accounts.folders[index2];
              break;
            }
          }
          break;
        }
      }
      this.comp.currentUser = this.user;
      if (this.userFolder != null)
        ;
    }

    public void addMail(string mail, string userTo)
    {
      for (int index = 0; index < this.responders.Count; ++index)
        this.responders[index].mailReceived(mail, userTo);
      Folder folder = (Folder) null;
      for (int index = 0; index < this.accounts.folders.Count; ++index)
      {
        if (this.accounts.folders[index].name.Equals(userTo))
        {
          folder = this.accounts.folders[index].folders[0];
          break;
        }
      }
      if (folder != null)
        folder.files.Insert(0, new FileEntry(mail, MailServer.getSubject(mail)));
      else
        throw new NullReferenceException("User " + userTo + " has no valid mail account on this mail server :" + this.comp.idName + ". Check account type and name matching!");
    }

    public bool MailWithSubjectExists(string userName, string mailSubject)
    {
      for (int index1 = 0; index1 < this.accounts.folders.Count; ++index1)
      {
        if (this.accounts.folders[index1].name.Equals(userName))
        {
          Folder folder = this.accounts.folders[index1];
          for (int index2 = 0; index2 < folder.files.Count; ++index2)
          {
            string[] strArray = folder.files[index2].data.Split(MailServer.emailSplitDelims, StringSplitOptions.None);
            if (strArray.Length >= 4 && strArray[2].ToLower() == mailSubject.ToLower())
              return true;
          }
          break;
        }
      }
      return false;
    }

    public override void userAdded(string name, string pass, byte type)
    {
      base.userAdded(name, pass, type);
      if ((int) type != 0 && (int) type != 1 && (int) type != 2)
        return;
      Folder folder = new Folder(name);
      folder.files.Add(new FileEntry("Username: " + name + "\nPassword: " + pass, "AccountInfo"));
      Folder f = new Folder("inbox");
      this.addJunkEmails(f);
      folder.folders.Add(f);
      folder.folders.Add(new Folder("sent"));
      this.accounts.folders.Add(folder);
    }

    public override string getSaveString()
    {
      return "<MailServer name=\"" + this.name + "\" color=\"" + Utils.convertColorToParseableString(this.themeColor) + "\"/>";
    }

    public virtual void drawTopBar(Rectangle bounds, SpriteBatch sb)
    {
      this.panelRect = new Rectangle(bounds.X + 1, bounds.Y, bounds.Width - (this.corner.Width + 2), this.panel.Height);
      sb.Draw(this.panel, this.panelRect, this.themeColor);
      sb.Draw(this.corner, new Vector2((float) (bounds.X + bounds.Width - (this.corner.Width + 1)), (float) bounds.Y), this.themeColor);
    }

    public virtual void drawBackingGradient(Rectangle boundsTo, SpriteBatch sb)
    {
      Rectangle destinationRectangle = boundsTo;
      ++destinationRectangle.X;
      destinationRectangle.Width -= 2;
      destinationRectangle.Height -= 2;
      sb.Draw(Utils.gradient, destinationRectangle, Color.Black);
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      Vector2 position = new Vector2((float) (bounds.X + 8), (float) (bounds.Y + 120));
      this.drawTopBar(bounds, sb);
      switch (this.state)
      {
        case 0:
          sb.DrawString(GuiData.font, this.name + " " + LocaleTerms.Loc("Mail Server"), position, this.textColor);
          position.Y += 80f;
          position.Y += 35f;
          if (Button.doButton(800002, (int) position.X, (int) position.Y, 300, 40, LocaleTerms.Loc("Login"), new Color?(this.themeColor)))
          {
            this.state = 2;
            this.os.displayCache = "";
            this.os.execute("login");
            do
              ;
            while (this.os.displayCache.Equals(""));
            this.os.display.command = this.name;
          }
          position.Y += 45f;
          if (!Button.doButton(800003, (int) position.X, (int) position.Y, 300, 30, LocaleTerms.Loc("Exit"), new Color?(this.themeColor)))
            break;
          this.os.display.command = "connect";
          break;
        case 1:
          sb.DrawString(GuiData.font, LocaleTerms.Loc("Test"), position, this.textColor);
          position.Y += 80f;
          if (!Button.doButton(800001, (int) position.X, (int) position.Y, 300, 60, "testing", new Color?()))
            break;
          this.state = 0;
          break;
        case 2:
          this.drawBackingGradient(bounds, sb);
          sb.DrawString(GuiData.font, LocaleTerms.Loc("login"), position, this.textColor);
          position.Y += 80f;
          this.doLoginDisplay(bounds, sb);
          break;
        case 3:
          this.doInboxDisplay(bounds, sb);
          break;
        case 4:
          this.doEmailViewerDisplay(bounds, sb);
          break;
        case 5:
          this.doRespondDisplay(bounds, sb);
          break;
      }
    }

    private int GetRenderTextHeight()
    {
      return (int) ((double) GuiData.ActiveFontConfig.tinyFontCharHeight + 2.0);
    }

    private int DrawMailMessageText(Rectangle textBounds, SpriteBatch sb, string[] text)
    {
      if (this.sectionedPanel == null || this.sectionedPanel.PanelHeight != this.GetRenderTextHeight())
        this.sectionedPanel = new ScrollableSectionedPanel(this.GetRenderTextHeight(), sb.GraphicsDevice);
      this.sectionedPanel.NumberOfPanels = text.Length;
      int itemsDrawn = 0;
      this.sectionedPanel.Draw((Action<int, Rectangle, SpriteBatch>) ((index, dest, spBatch) =>
      {
        spBatch.DrawString(GuiData.tinyfont, LocalizedFileLoader.SafeFilterString(text[index]), new Vector2((float) dest.X, (float) dest.Y), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.8f);
        ++itemsDrawn;
      }), sb, textBounds);
      if (this.sectionedPanel.NumberOfPanels * this.sectionedPanel.PanelHeight < textBounds.Height)
        return this.sectionedPanel.NumberOfPanels * this.sectionedPanel.PanelHeight;
      return textBounds.Height;
    }

    public void doEmailViewerDisplay(Rectangle bounds, SpriteBatch sb)
    {
      Vector2 vector2 = new Vector2((float) (bounds.X + 2), (float) (bounds.Y + 20));
      if (Button.doButton(800007, (int) vector2.X, (int) vector2.Y, bounds.Width - 20 - this.corner.Width, 30, LocaleTerms.Loc("Return to Inbox"), new Color?(this.os.darkBackgroundColor)))
        this.state = 3;
      vector2.Y += 35f;
      Rectangle tmpRect = GuiData.tmpRect;
      tmpRect.X = bounds.X + 1;
      tmpRect.Y = (int) vector2.Y;
      tmpRect.Width = bounds.Width - 2;
      tmpRect.Height = 38;
      sb.Draw(Utils.white, tmpRect, this.textColor);
      vector2.Y += 3f;
      sb.DrawString(GuiData.UITinyfont, "<" + this.emailData[1] + ">", vector2, Color.Black);
      vector2.Y += 18f;
      sb.DrawString(GuiData.UITinyfont, LocaleTerms.Loc("Subject") + ": " + LocalizedFileLoader.SafeFilterString(this.emailData[2]), vector2, Color.Black);
      vector2.Y += 25f;
      vector2.X += 20f;
      int num1 = 25;
      int num2 = (this.emailData.Length - 4 + 1) * num1;
      Rectangle textBounds = new Rectangle((int) vector2.X, (int) vector2.Y, bounds.Width - 22, (int) ((double) bounds.Height - ((double) vector2.Y - (double) bounds.Y) - (double) num2));
      string str1;
      if (LocaleActivator.ActiveLocaleIsCJK())
      {
        string str2 = "\t";
        str1 = Utils.SuperSmartTwimForWidth(LocalizedFileLoader.SafeFilterString(this.emailData[3].Replace("。\n", str2).Replace("\n", "").Replace(str2, "。\n").Replace(str2, "！\n").Replace(str2, "!\n")), bounds.Width - 50, GuiData.tinyfont).Replace("\r\n\r\n\r\n", "\r\n\r\n");
      }
      else
        str1 = Utils.SmartTwimForWidth(LocalizedFileLoader.SafeFilterString(this.emailData[3]), bounds.Width - 50, GuiData.tinyfont);
      vector2.Y += (float) this.DrawMailMessageText(textBounds, sb, str1.Split(Utils.newlineDelim));
      vector2.Y += (float) (num1 - 5);
      int startingButtonIndex = 0;
      for (int index = 4; index < this.emailData.Length; ++index)
      {
        if (AttachmentRenderer.RenderAttachment(this.emailData[index], (object) this.os, vector2, startingButtonIndex, MailServer.buttonSound))
        {
          ++startingButtonIndex;
          vector2.Y += (float) num1;
        }
      }
      vector2.Y = (float) (bounds.Y + bounds.Height - 35);
      if (!Button.doButton(90200, (int) vector2.X, (int) vector2.Y, 300, 30, LocaleTerms.Loc("Reply"), new Color?()))
        return;
      this.emailReplyStrings.Clear();
      this.addingNewReplyString = false;
      this.missionIncompleteReply = false;
      this.state = 5;
    }

    private void DrawButtonGlow(Vector2 dpos, Vector2 labelSize)
    {
      Rectangle rectangle = new Rectangle((int) ((double) dpos.X + (double) labelSize.X + 5.0), (int) dpos.Y, 20, 17);
      float num1 = Utils.QuadraticOutCurve((float) (1.0 - (double) this.os.timer % 1.0));
      float num2 = 8.5f;
      Rectangle destinationRectangle = Utils.InsetRectangle(rectangle, (int) (-1.0 * ((double) num2 * (1.0 - (double) num1))));
      GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, Utils.AddativeWhite * num1 * 0.32f);
      GuiData.spriteBatch.Draw(Utils.white, rectangle, Color.Black * 0.7f);
    }

    public virtual void doInboxHeader(Rectangle bounds, SpriteBatch sb)
    {
      Vector2 vector2_1 = new Vector2((float) (bounds.X + 2), (float) (bounds.Y + 20));
      string text = string.Format(LocaleTerms.Loc("Logged in as {0}"), (object) this.user.name);
      sb.DrawString(GuiData.font, text, vector2_1, this.textColor);
      vector2_1.Y += 26f;
      if (Button.doButton(800007, bounds.X + bounds.Width - 95, (int) vector2_1.Y, 90, 30, LocaleTerms.Loc("Logout"), new Color?(this.themeColor)))
        this.state = 0;
      if (this.totalPagesDetected <= 0)
        return;
      int width = 100;
      int height = 20;
      vector2_1.Y = (float) (bounds.Y + 91 - (height + 4));
      if (this.inboxPage < this.totalPagesDetected && Button.doButton(801008, (int) vector2_1.X, (int) vector2_1.Y, width, height, "<", new Color?()))
        ++this.inboxPage;
      vector2_1.X += (float) (width + 2);
      Vector2 vector2_2 = TextItem.doMeasuredFontLabel(vector2_1, (this.inboxPage + 1).ToString() + " / " + (object) (this.totalPagesDetected + 1), GuiData.tinyfont, new Color?(Utils.AddativeWhite), float.MaxValue, float.MaxValue);
      vector2_1.X += vector2_2.X + 4f;
      if (this.inboxPage > 0 && Button.doButton(801009, (int) vector2_1.X, (int) vector2_1.Y, width, height, ">", new Color?()))
        --this.inboxPage;
    }

    public void doInboxDisplay(Rectangle bounds, SpriteBatch sb)
    {
      int height1 = 24;
      this.doInboxHeader(bounds, sb);
      Vector2 vector2 = new Vector2((float) (bounds.X + 2), (float) (bounds.Y + 91));
      Folder folder = this.userFolder.folders[0];
      Rectangle destinationRectangle1 = new Rectangle(bounds.X + 2, (int) vector2.Y, bounds.Width - 4, height1);
      Button.outlineOnly = true;
      int num1 = 0;
      while (destinationRectangle1.Y + destinationRectangle1.Height < bounds.Y + bounds.Height - 2)
      {
        sb.Draw(Utils.white, destinationRectangle1, num1 % 2 == 0 ? this.evenLine : this.oddLine);
        ++num1;
        int height2 = destinationRectangle1.Height;
        destinationRectangle1.Height = 1;
        if (num1 > 1)
          sb.Draw(Utils.white, destinationRectangle1, this.seperatorLineColor);
        destinationRectangle1.Height = height2;
        destinationRectangle1.Y += height1;
      }
      Rectangle tmpRect = GuiData.tmpRect;
      this.drawBackingGradient(bounds, sb);
      tmpRect.X = bounds.X + 1;
      tmpRect.Y = (int) vector2.Y - 3;
      tmpRect.Width = bounds.Width - 4;
      tmpRect.Height = 3;
      sb.Draw(Utils.white, tmpRect, this.themeColor);
      tmpRect.X += tmpRect.Width;
      tmpRect.Width = 3;
      sb.Draw(Utils.white, tmpRect, this.themeColor);
      tmpRect.X = destinationRectangle1.X;
      tmpRect.Y = (int) vector2.Y;
      tmpRect.Width = 160;
      tmpRect.Height = bounds.Height - (tmpRect.Y - bounds.Y) - 1;
      sb.Draw(Utils.white, tmpRect, this.senderDarkeningColor);
      tmpRect.X += tmpRect.Width;
      tmpRect.Width = 3;
      sb.Draw(Utils.white, tmpRect, this.themeColor);
      tmpRect.X = destinationRectangle1.X;
      tmpRect.Y = (int) vector2.Y;
      tmpRect.Width = 160;
      tmpRect.Height = bounds.Height - (tmpRect.Y - bounds.Y) - 1;
      destinationRectangle1.Y = (int) vector2.Y;
      TextItem.DrawShadow = false;
      float num2 = (float) (bounds.Height - 2);
      int val1 = num1;
      int num3 = Math.Max(0, val1 * this.inboxPage - 1);
      this.totalPagesDetected = (int) ((double) folder.files.Count / (double) val1);
      int num4 = Math.Min(val1, folder.files.Count - num3);
      for (int index = num3; index < num3 + num4; ++index)
      {
        try
        {
          string[] strArray = folder.files[index].data.Split(MailServer.emailSplitDelims, StringSplitOptions.None);
          byte num5 = Convert.ToByte(strArray[0]);
          int myID = 8100 + index;
          if (GuiData.hot == myID)
          {
            Rectangle destinationRectangle2 = new Rectangle(bounds.X + 2, destinationRectangle1.Y + 1, bounds.Width - 4, height1 - 2);
            sb.Draw(Utils.white, destinationRectangle2, index % 2 == 0 ? Color.White * 0.07f : Color.Black * 0.2f);
          }
          if (Button.doButton(myID, bounds.X + 2, destinationRectangle1.Y + 1, bounds.Width - 4, height1 - 2, "", new Color?(Color.Transparent)))
          {
            this.state = 4;
            this.selectedEmail = folder.files[index];
            this.emailData = this.selectedEmail.data.Split(MailServer.emailSplitDelims, StringSplitOptions.None);
            this.selectedEmail.data = "1" + this.selectedEmail.data.Substring(1);
            if (this.sectionedPanel != null)
              this.sectionedPanel.ScrollDown = 0.0f;
          }
          if ((int) num5 == 0)
            sb.Draw(this.unopenedIcon, vector2 + Vector2.One, this.themeColor);
          TextItem.doFontLabel(vector2 + new Vector2((float) (this.unopenedIcon.Width + 1), 2f), strArray[1], GuiData.tinyfont, new Color?(this.textColor), (float) (tmpRect.Width - this.unopenedIcon.Width - 3), (float) height1, false);
          TextItem.doFontLabel(vector2 + new Vector2((float) (tmpRect.Width + 10), 2f), strArray[2], GuiData.tinyfont, new Color?(this.textColor), (float) (bounds.Width - tmpRect.Width - 20), (float) height1, false);
          vector2.Y += (float) height1;
          destinationRectangle1.Y = (int) vector2.Y;
        }
        catch (FormatException ex)
        {
        }
      }
      Button.outlineOnly = false;
    }

    public void doRespondDisplay(Rectangle bounds, SpriteBatch sb)
    {
      Vector2 pos = new Vector2((float) (bounds.X + 2), (float) (bounds.Y + 20));
      string str = (string) null;
      int width = bounds.Width - 20 - this.corner.Width;
      if (Button.doButton(800007, (int) pos.X, (int) pos.Y, width, 30, LocaleTerms.Loc("Return to Inbox"), new Color?(this.os.darkBackgroundColor)))
        this.state = 3;
      pos.Y += 50f;
      int num1 = 24;
      TextItem.doFontLabel(pos, LocaleTerms.Loc("Additional Details") + " :", GuiData.smallfont, new Color?(), (float) bounds.Width - (float) (((double) pos.X - (double) bounds.Width) * 1.20000004768372), float.MaxValue, false);
      pos.Y += (float) num1;
      for (int index = 0; index < this.emailReplyStrings.Count; ++index)
      {
        TextItem.doFontLabel(pos + new Vector2(25f, 0.0f), this.emailReplyStrings[index], GuiData.tinyfont, new Color?(), (float) ((double) bounds.Width - ((double) pos.X - (double) bounds.X) * 2.0 - 20.0), float.MaxValue, false);
        float num2 = Math.Min(GuiData.tinyfont.MeasureString(this.emailReplyStrings[index]).X, (float) ((double) bounds.Width - ((double) pos.X - (double) bounds.X) * 2.0 - 20.0));
        if (Button.doButton(80000 + index * 100, (int) ((double) pos.X + (double) num2 + 30.0), (int) pos.Y, 20, 20, "-", new Color?()))
          this.emailReplyStrings.RemoveAt(index);
        pos.Y += (float) num1;
      }
      if (this.addingNewReplyString)
      {
        string data = (string) null;
        bool getStringCommand = Programs.parseStringFromGetStringCommand(this.os, out data);
        if (data == null)
          data = "";
        pos.Y += 5f;
        GuiData.spriteBatch.Draw(Utils.white, new Rectangle(bounds.X + 1, (int) pos.Y, bounds.Width - 2 - bounds.Width / 9, 40), this.os.indentBackgroundColor);
        pos.Y += 10f;
        TextItem.doFontLabel(pos + new Vector2(25f, 0.0f), data, GuiData.tinyfont, new Color?(), float.MaxValue, float.MaxValue, false);
        Vector2 vector2 = GuiData.tinyfont.MeasureString(data);
        vector2.Y = 0.0f;
        if ((double) this.os.timer % 1.0 <= 0.5)
          GuiData.spriteBatch.Draw(Utils.white, new Rectangle((int) ((double) pos.X + (double) vector2.X + 2.0) + 25, (int) pos.Y, 4, 20), Color.White);
        int num2 = bounds.Width - 1 - bounds.Width / 10;
        if (getStringCommand || Button.doButton(8000094, bounds.X + num2 - 4, (int) pos.Y - 10, bounds.Width / 9 - 3, 40, LocaleTerms.Loc("Add"), new Color?(this.os.highlightColor)))
        {
          if (!getStringCommand)
            this.os.terminal.executeLine();
          this.addingNewReplyString = false;
          this.emailReplyStrings.Add(data);
          str = (string) null;
        }
        else
          str = data;
      }
      else if (Button.doButton(8000098, (int) ((double) pos.X + 25.0), (int) pos.Y, 20, 20, "+", new Color?()))
      {
        this.addingNewReplyString = true;
        this.os.execute("getString Detail");
        this.os.terminal.executionPreventionIsInteruptable = true;
      }
      pos.Y += 50f;
      if (Button.doButton(800008, (int) pos.X, (int) pos.Y, width, 30, LocaleTerms.Loc("Send"), new Color?()) && this.os.currentMission != null)
      {
        if (str != null)
        {
          this.os.terminal.executeLine();
          this.addingNewReplyString = false;
          if (!string.IsNullOrEmpty(str))
            this.emailReplyStrings.Add(str);
        }
        ActiveMission currentMission = this.os.currentMission;
        bool flag = this.attemptCompleteMission(this.os.currentMission);
        if (!flag)
        {
          for (int index = 0; index < this.os.branchMissions.Count && !flag; ++index)
          {
            flag = this.attemptCompleteMission(this.os.branchMissions[index]);
            if (flag)
              this.os.branchMissions.Clear();
          }
        }
        if (!flag)
          this.missionIncompleteReply = true;
        else
          this.AddSentEmailRecordFileForMissionCompletion(currentMission, this.emailReplyStrings);
      }
      pos.Y += 45f;
      if (Settings.forceCompleteEnabled && Button.doButton(800009, (int) pos.X, (int) pos.Y, width, 30, LocaleTerms.Loc("Force Complete"), new Color?()))
      {
        if (this.os.currentMission != null)
        {
          this.os.currentMission.finish();
          this.os.MissionCompleteFlashTime = 3f;
        }
        this.state = 3;
      }
      pos.Y += 70f;
      if (!this.missionIncompleteReply || !(this.comp.idName == "jmail"))
        return;
      PatternDrawer.draw(new Rectangle(bounds.X + 2, (int) pos.Y, bounds.Width - 4, 128), 1f, this.os.lockedColor * 0.1f, this.os.brightLockedColor, sb, PatternDrawer.errorTile);
      string text = LocaleTerms.Loc("Mission Incomplete");
      Vector2 vector2_1 = GuiData.font.MeasureString(text);
      TextItem.doLabel(new Vector2((float) (bounds.X + bounds.Width / 2) - vector2_1.X / 2f, pos.Y + 40f), text, new Color?());
    }

    private void AddSentEmailRecordFileForMissionCompletion(ActiveMission mission, List<string> additionalDetails)
    {
      try
      {
        Folder folder = this.userFolder.searchForFolder("sent");
        string body = mission.email.subject + " completed.";
        if (additionalDetails != null && additionalDetails.Count > 0)
        {
          body += "\nRequested Details:\n";
          for (int index = 0; index < additionalDetails.Count; ++index)
            body = body + additionalDetails[index] + "\n";
        }
        string email = MailServer.generateEmail(mission.email.subject + " : Complete", body, this.user.name + "@" + this.comp.name);
        string subject = MailServer.getSubject(email);
        FileEntry fileEntry = new FileEntry(email, subject);
        folder.files.Add(fileEntry);
      }
      catch (Exception ex)
      {
      }
    }

    private bool attemptCompleteMission(ActiveMission mission)
    {
      if (!mission.isComplete(this.emailReplyStrings) || !mission.ShouldIgnoreSenderVerification && !(mission.email.sender == this.emailData[1]))
        return false;
      mission.finish();
      this.state = 3;
      this.os.MissionCompleteFlashTime = 3f;
      return true;
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
        for (int index1 = 0; index1 < this.comp.users.Count; ++index1)
        {
          if (this.comp.users[index1].name.Equals(text1))
          {
            this.user = this.comp.users[index1];
            for (int index2 = 0; index2 < this.accounts.folders.Count; ++index2)
            {
              if (this.accounts.folders[index2].name.Equals(this.user.name))
              {
                this.userFolder = this.accounts.folders[index2];
                break;
              }
            }
            break;
          }
        }
        this.state = 3;
      }
      tmpRect.Height = 22;
      int num5 = num2 + 30;
      Vector2 vector2 = TextItem.doMeasuredLabel(new Vector2((float) num1, (float) num5), LocaleTerms.Loc("Login") + " ", new Color?(this.textColor));
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
      sb.DrawString(GuiData.smallfont, LocaleTerms.Loc("username") + " :", new Vector2((float) num1, (float) num7), this.textColor);
      int num8 = num1 + 100;
      sb.DrawString(GuiData.smallfont, text1, new Vector2((float) num8, (float) num7), this.textColor);
      int num9 = num8 - 100;
      int num10 = num7 + 30;
      if (num4 == 1)
      {
        tmpRect.Y = num10;
        sb.Draw(Utils.white, tmpRect, this.os.subtleTextColor);
      }
      sb.DrawString(GuiData.smallfont, LocaleTerms.Loc("password") + " :", new Vector2((float) num9, (float) num10), this.textColor);
      int num11 = num9 + 100;
      sb.DrawString(GuiData.smallfont, text2, new Vector2((float) num11, (float) num10), this.textColor);
      int y1 = num10 + 30;
      int x = num11 - 100;
      if (num3 != -1)
      {
        if (Button.doButton(12345, x, y1, 70, 30, LocaleTerms.Loc("Back"), new Color?(this.os.indentBackgroundColor)))
          this.state = 0;
        if (!Button.doButton(123456, x + 75, y1, 70, 30, LocaleTerms.Loc("Retry"), new Color?(this.os.indentBackgroundColor)))
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
          if (this.comp.users[index].known && MailServer.validUser(this.comp.users[index].type))
          {
            if (Button.doButton(123457 + index, x, y2, 300, 25, "User: " + this.comp.users[index].name + " Pass: " + this.comp.users[index].pass, new Color?(this.os.darkBackgroundColor)))
              this.forceLogin(this.comp.users[index].name, this.comp.users[index].pass);
            y2 += 27;
          }
        }
      }
    }

    public void forceLogin(string username, string pass)
    {
      string prompt = this.os.terminal.prompt;
      this.os.terminal.currentLine = username;
      this.os.terminal.executeLine();
      do
        ;
      while (this.os.terminal.prompt.Equals(prompt));
      this.os.terminal.currentLine = pass;
      this.os.terminal.executeLine();
    }

    public new static bool validUser(byte type)
    {
      return Daemon.validUser(type) || (int) type == 2;
    }

    public static string generateEmail(string subject, string body, string sender)
    {
      return "0" + MailServer.emailSplitDelimiter + MailServer.cleanString(sender) + MailServer.emailSplitDelimiter + MailServer.cleanString(subject) + MailServer.emailSplitDelimiter + MailServer.minimalCleanString(body);
    }

    public static string generateEmail(string subject, string body, string sender, List<string> attachments)
    {
      string str = MailServer.generateEmail(subject, body, sender) + MailServer.emailSplitDelimiter;
      for (int index = 0; index < attachments.Count; ++index)
        str = str + attachments[index] + MailServer.emailSplitDelimiter;
      return str;
    }

    public static string getSubject(string mail)
    {
      return mail.Split(MailServer.emailSplitDelims, StringSplitOptions.None)[2].Replace(' ', '_');
    }

    public static string cleanString(string s)
    {
      return s.Replace('\n', '_').Replace('\r', '_').Replace(MailServer.emailSplitDelimiter, "_");
    }

    public static string minimalCleanString(string s)
    {
      return s.Replace('\t', ' ');
    }

    public struct EMailData
    {
      public string sender;
      public string body;
      public string subject;
      public List<string> attachments;

      public EMailData(string sendr, string bod, string subj, List<string> _attachments)
      {
        this.sender = sendr;
        this.body = bod;
        this.subject = subj;
        this.attachments = _attachments;
      }
    }
  }
}
