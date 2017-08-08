// Decompiled with JetBrains decompiler
// Type: Hacknet.MissionListingServer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Extensions;
using Hacknet.Gui;
using Hacknet.Mission;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hacknet
{
  internal class MissionListingServer : AuthenticatingDaemon
  {
    public bool missionAssigner = false;
    private bool NeedsCustomFolderLoad = false;
    private string CustomFolderLoadPath = (string) null;
    private string IconReloadPath = (string) null;
    private string ArticleFolderPath = (string) null;
    private bool HasCustomColor = false;
    private List<List<ActiveMission>> branchMissions = new List<List<ActiveMission>>();
    private const int NEED_LOGIN = 0;
    private const int BOARD = 1;
    private const int MESSAGE = 2;
    private const int LOGIN = 3;
    private string groupName;
    public string listingTitle;
    private Color themeColor;
    private Texture2D topBar;
    private Texture2D corner;
    private Texture2D logo;
    private Rectangle panelRect;
    private Rectangle logoRect;
    private Folder root;
    private Folder missionFolder;
    private Folder closedMissionsFolder;
    private List<int> rootPath;
    private List<int> missionFolderPath;
    private FileEntry sysFile;
    private int targetIndex;
    private ScrollableTextRegion TextRegion;
    public bool isPublic;
    private int state;
    private List<ActiveMission> missions;

    public MissionListingServer(Computer c, string serviceName, string group, OS _os, bool _isPublic = false, bool _isAssigner = false)
      : base(c, serviceName, _os)
    {
      this.groupName = group;
      this.topBar = this.os.content.Load<Texture2D>("Panel");
      this.corner = this.os.content.Load<Texture2D>("Corner");
      if (group.Equals("Entropy"))
      {
        this.themeColor = new Color(3, 102, 49);
        this.logo = this.os.content.Load<Texture2D>("EntropyLogo");
      }
      else if (group.Equals("NetEdu"))
      {
        this.themeColor = new Color(119, 104, 160);
        this.logo = this.os.content.Load<Texture2D>("Sprites/Academic_Logo");
      }
      else if (group.Equals("Kellis Biotech"))
      {
        this.themeColor = new Color(106, 176, (int) byte.MaxValue);
        this.logo = this.os.content.Load<Texture2D>("Sprites/KellisLogo");
      }
      else
      {
        this.themeColor = new Color(204, 163, 27);
        this.logo = this.os.content.Load<Texture2D>("SlashbotLogo");
      }
      this.logoRect = new Rectangle(0, 0, 64, 64);
      this.isPublic = _isPublic;
      this.missionAssigner = _isAssigner;
      this.state = !this.isPublic ? 0 : 1;
      this.missions = new List<ActiveMission>();
      this.branchMissions = new List<List<ActiveMission>>();
      if (this.isPublic)
        this.listingTitle = string.Format(LocaleTerms.Loc("{0} News"), (object) group);
      else
        this.listingTitle = LocaleTerms.Loc("Available Contracts");
    }

    public MissionListingServer(Computer c, string serviceName, string iconPath, string articleFolderPath, Color themeColor, OS _os, bool _isPublic = false, bool _isAssigner = false)
      : base(c, serviceName, _os)
    {
      this.groupName = serviceName;
      this.topBar = this.os.content.Load<Texture2D>("Panel");
      this.corner = this.os.content.Load<Texture2D>("Corner");
      this.themeColor = themeColor;
      string str = "Content/";
      if (Settings.IsInExtensionMode)
        str = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
      try
      {
        using (FileStream fileStream = File.OpenRead(str + iconPath))
          this.logo = Texture2D.FromStream(GuiData.spriteBatch.GraphicsDevice, (Stream) fileStream);
      }
      catch (Exception ex)
      {
        this.logo = this.os.content.Load<Texture2D>("Sprites/Academic_Logo");
      }
      this.NeedsCustomFolderLoad = true;
      this.CustomFolderLoadPath = str + articleFolderPath;
      this.CustomFolderLoadPath = this.CustomFolderLoadPath.Replace('\\', '/');
      if (!this.CustomFolderLoadPath.EndsWith("/"))
        this.CustomFolderLoadPath += "/";
      this.logoRect = new Rectangle(0, 0, 64, 64);
      this.isPublic = _isPublic;
      this.missionAssigner = _isAssigner;
      this.state = !this.isPublic ? 0 : 1;
      this.missions = new List<ActiveMission>();
      this.branchMissions = new List<List<ActiveMission>>();
      this.listingTitle = !this.isPublic ? LocaleTerms.Loc("Available Contracts") : serviceName;
      this.HasCustomColor = true;
      this.IconReloadPath = iconPath;
      this.ArticleFolderPath = articleFolderPath;
    }

    public override void initFiles()
    {
      base.initFiles();
      this.initFilesystem();
      this.addListingsForGroup();
    }

    public override void loadInit()
    {
      base.loadInit();
      this.root = this.comp.files.root.searchForFolder("MsgBoard");
      this.missionFolder = this.root.searchForFolder("listings");
      this.closedMissionsFolder = this.root.searchForFolder("closed");
      if (this.closedMissionsFolder == null)
      {
        this.closedMissionsFolder = new Folder("closed");
        this.root.folders.Add(this.closedMissionsFolder);
      }
      int num;
      if (!this.missionAssigner)
      {
        for (int index = 0; index < this.missionFolder.files.Count; ++index)
        {
          try
          {
            string str = this.missionFolder.files[index].name.Replace("_", " ");
            string data = this.missionFolder.files[index].data;
            this.missions.Add(new ActiveMission((List<MisisonGoal>) null, (string) null, new MailServer.EMailData())
            {
              postingTitle = str,
              postingBody = data
            });
          }
          catch (Exception ex)
          {
            num = 0 + 1;
          }
        }
      }
      else
      {
        List<ActiveMission> branchMissions = this.os.branchMissions;
        for (int index = 0; index < this.missionFolder.files.Count; ++index)
        {
          try
          {
            string data = this.missionFolder.files[index].data;
            int contractRegistryNumber = 0;
            this.os.branchMissions.Clear();
            ActiveMission activeMission = (ActiveMission) MissionSerializer.restoreMissionFromFile(data, out contractRegistryNumber);
            List<ActiveMission> activeMissionList = new List<ActiveMission>();
            activeMissionList.AddRange((IEnumerable<ActiveMission>) this.os.branchMissions.ToArray());
            this.branchMissions.Add(activeMissionList);
            this.missions.Add(activeMission);
          }
          catch (Exception ex)
          {
            num = 0 + 1;
          }
        }
        this.os.branchMissions = branchMissions;
      }
    }

    public override string getSaveString()
    {
      string str = "";
      if (this.HasCustomColor)
        str = string.Format("icon=\"{0}\" color=\"{1}\" articles=\"{2}\" ", (object) this.IconReloadPath, (object) Utils.convertColorToParseableString(this.themeColor), (object) this.ArticleFolderPath);
      return "<MissionListingServer name=\"" + this.name + "\" group=\"" + this.groupName + "\" public=\"" + (object) this.isPublic + "\" assign=\"" + (object) this.missionAssigner + "\" title=\"" + this.listingTitle + "\" " + str + "/>";
    }

    public void addMisison(ActiveMission m, bool injectToTop = false)
    {
      string dataEntry = m.postingBody;
      if (this.missionAssigner)
        dataEntry = MissionSerializer.generateMissionFile((object) m, 0, this.groupName, (string) null);
      FileEntry fileEntry = new FileEntry(dataEntry, m.postingTitle);
      if (injectToTop)
      {
        this.missionFolder.files.Insert(0, fileEntry);
        this.missions.Insert(0, m);
        this.branchMissions.Insert(0, this.os.branchMissions);
      }
      else
      {
        this.missionFolder.files.Add(fileEntry);
        this.missions.Add(m);
        this.branchMissions.Add(this.os.branchMissions);
      }
    }

    public void removeMission(string missionPath)
    {
      for (int index = 0; index < this.missions.Count; ++index)
      {
        if (this.missions[index].reloadGoalsSourceFile == LocalizedFileLoader.GetLocalizedFilepath(missionPath))
        {
          this.removeMission(index);
          --index;
        }
      }
    }

    public void removeMission(int index)
    {
      bool flag = false;
      for (int index1 = 0; index1 < this.missionFolder.files.Count; ++index1)
      {
        if (this.missionFolder.files[index1].name.Equals(this.missions[index].postingTitle.Replace(" ", "_")))
        {
          FileEntry file = this.missionFolder.files[index1];
          this.missionFolder.files.RemoveAt(index1);
          this.closedMissionsFolder.files.Add(file);
          flag = true;
          break;
        }
      }
      if (flag)
        ;
      this.missions.RemoveAt(index);
    }

    public void initFilesystem()
    {
      this.rootPath = new List<int>();
      this.missionFolderPath = new List<int>();
      this.root = new Folder("MsgBoard");
      this.rootPath.Add(this.comp.files.root.folders.IndexOf(this.root));
      this.missionFolderPath.Add(this.comp.files.root.folders.IndexOf(this.root));
      this.missionFolder = new Folder("listings");
      this.missionFolderPath.Add(0);
      this.root.folders.Add(this.missionFolder);
      this.closedMissionsFolder = new Folder("closed");
      this.root.folders.Add(this.closedMissionsFolder);
      this.sysFile = new FileEntry(Computer.generateBinaryString(1024), "config.sys");
      this.root.files.Add(this.sysFile);
      this.root.files.Add(new FileEntry(Utils.readEntireFile("Content/LocPost/ListingServerCautionFile.txt"), "Config_CAUTION.txt"));
      this.comp.files.root.folders.Add(this.root);
    }

    public void addListingsForGroup()
    {
      List<ActiveMission> branchMissions = this.os.branchMissions;
      this.os.branchMissions = (List<ActiveMission>) null;
      bool flag = false;
      if (!this.NeedsCustomFolderLoad && this.groupName.ToLower().Equals("entropy"))
      {
        this.NeedsCustomFolderLoad = true;
        this.CustomFolderLoadPath = "Content/Missions/Entropy/StartingSet/";
        flag = true;
      }
      if (this.NeedsCustomFolderLoad)
      {
        foreach (FileSystemInfo file in new DirectoryInfo(this.CustomFolderLoadPath).GetFiles("*.xml"))
        {
          string filename = this.CustomFolderLoadPath + file.Name;
          this.os.branchMissions = new List<ActiveMission>();
          this.addMisison((ActiveMission) ComputerLoader.readMission(filename), false);
        }
        if (flag)
        {
          for (int index = 0; index < 2; ++index)
          {
            this.os.branchMissions = new List<ActiveMission>();
            this.addMisison((ActiveMission) MissionGenerator.generate(2), false);
          }
        }
      }
      else if (this.groupName.ToLower().Equals("netedu"))
      {
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education4.0.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education2.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education3.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education1.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education4.1.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education5.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education6.xml"), false);
      }
      else if (this.groupName.ToLower().Equals("slashbot"))
      {
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/Entropy/NewsArticles/EntropyNews1.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/Entropy/NewsArticles/EntropyNews3.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/Entropy/NewsArticles/EntropyNews2.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/Entropy/NewsArticles/EntropyNews4.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/Entropy/NewsArticles/EntropyNews5.xml"), false);
      }
      else if (this.groupName.ToLower().Equals("kellis biotech"))
      {
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/MainHub/PacemakerSet/HardwareArticles/Hardware1.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/MainHub/PacemakerSet/HardwareArticles/Hardware2.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/MainHub/PacemakerSet/HardwareArticles/Hardware3.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/MainHub/PacemakerSet/HardwareArticles/Hardware4.xml"), false);
        this.addMisison((ActiveMission) ComputerLoader.readMission("Content/Missions/MainHub/PacemakerSet/HardwareArticles/Hardware5.xml"), false);
      }
      this.os.branchMissions = branchMissions;
    }

    public override void navigatedTo()
    {
      base.navigatedTo();
      this.state = !this.isPublic ? 0 : 1;
      this.ProgressionSaveFixHacks();
    }

    private void ProgressionSaveFixHacks()
    {
      if (!this.groupName.Equals("Entropy") || (this.missions.Count != 0 || !(this.os.currentFaction.idName == "entropy") || this.os.currentMission != null || this.os.Flags.HasFlag("ThemeHackTransitionAssetsAdded")))
        return;
      ComputerLoader.loadMission("Content/Missions/Entropy/ThemeHackTransitionMission.xml", false);
      this.os.saveGame();
    }

    public override void loginGoBack()
    {
      base.loginGoBack();
      this.state = 0;
    }

    public override void userLoggedIn()
    {
      base.userLoggedIn();
      if (!this.user.name.Equals(""))
        this.state = 1;
      else
        this.state = 0;
    }

    public bool hasSysfile()
    {
      for (int index = 0; index < this.root.files.Count; ++index)
      {
        if (this.root.files[index].name.Equals("config.sys"))
          return true;
      }
      return false;
    }

    public bool hasListingFile(string name)
    {
      name = name.Replace(" ", "_");
      for (int index = 0; index < this.missionFolder.files.Count; ++index)
      {
        if (this.missionFolder.files[index].name == name)
          return true;
      }
      return false;
    }

    public void drawTopBar(Rectangle bounds, SpriteBatch sb)
    {
      this.panelRect = new Rectangle(bounds.X, bounds.Y, bounds.Width - this.corner.Width, this.topBar.Height);
      sb.Draw(this.topBar, this.panelRect, this.themeColor);
      sb.Draw(this.corner, new Vector2((float) (bounds.X + bounds.Width - this.corner.Width), (float) bounds.Y), this.themeColor);
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      base.draw(bounds, sb);
      PatternDrawer.draw(new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2), 0.28f, Color.Transparent, this.themeColor * 0.1f, sb, PatternDrawer.thinStripe);
      this.drawTopBar(bounds, sb);
      if (!this.hasSysfile())
      {
        if (Button.doButton(800003, bounds.X + 10, bounds.Y + this.topBar.Height + 10, 300, 30, LocaleTerms.Loc("Exit"), new Color?(this.themeColor)))
          this.os.display.command = "connect";
        PatternDrawer.draw(new Rectangle(bounds.X + 1, bounds.Y + 1 + 64, bounds.Width - 2, bounds.Height - 2 - 64), 1f, Color.Transparent, this.os.lockedColor, sb, PatternDrawer.errorTile);
        int num1 = bounds.X + 20;
        int num2 = bounds.Y + bounds.Height / 2 - 20;
        TextItem.doLabel(new Vector2((float) num1, (float) num2), LocaleTerms.Loc("CRITICAL ERROR"), new Color?());
        int num3 = num2 + 40;
        TextItem.doSmallLabel(new Vector2((float) num1, (float) num3), "ERROR #4040408 - NULL_SYSFILE\nUnhandled Exception - IOException@L 2217 :R 28\nSystem Files Corrupted and/or Destroyed\nContact the System Administrator", new Color?());
      }
      else
      {
        switch (this.state)
        {
          case 0:
            if (Button.doButton(800003, bounds.X + 10, bounds.Y + this.topBar.Height + 10, 300, 30, LocaleTerms.Loc("Exit"), new Color?(this.themeColor)))
              this.os.display.command = "connect";
            sb.Draw(this.logo, new Rectangle(bounds.X + 30, bounds.Y + 115, 128, 128), Color.White);
            string text1 = string.IsNullOrWhiteSpace(this.listingTitle) ? string.Format(LocaleTerms.Loc("{0} Group"), (object) this.groupName) + "\n" + LocaleTerms.Loc("Message Board") : this.listingTitle;
            TextItem.doFontLabel(new Vector2((float) (bounds.X + 40 + 128), (float) (bounds.Y + 115)), text1, GuiData.font, new Color?(), (float) (bounds.Width - 40), 60f, false);
            if (!Button.doButton(800004, bounds.X + 30, bounds.Y + bounds.Height / 2, 300, 40, LocaleTerms.Loc("Login"), new Color?(this.themeColor)))
              break;
            this.startLogin();
            this.state = 3;
            break;
          case 1:
            if (Button.doButton(800003, bounds.X + 10, bounds.Y + this.topBar.Height + 10, 300, 30, LocaleTerms.Loc("Exit"), new Color?(this.themeColor)))
              this.os.display.command = "connect";
            int num4 = bounds.X + 10;
            int num5 = bounds.Y + this.topBar.Height + 50;
            this.logoRect.X = num4;
            this.logoRect.Y = num5;
            sb.Draw(this.logo, this.logoRect, Color.White);
            int x = num4 + (this.logoRect.Width + 5);
            TextItem.doLabel(new Vector2((float) x, (float) num5), this.listingTitle, new Color?());
            int y = num5 + 40;
            for (int index = 0; index < this.missions.Count; ++index)
            {
              if (this.hasListingFile(this.missions[index].postingTitle))
              {
                Rectangle rectangle = new Rectangle(x, y, (int) ((double) bounds.Width * 0.800000011920929), 30);
                rectangle = Utils.InsetRectangle(rectangle, 1);
                rectangle.X += 12;
                rectangle.Width -= 12;
                if (this.missions[index].postingTitle.StartsWith("#"))
                  PatternDrawer.draw(rectangle, 1f, Color.Black * 1f, Color.DarkRed * 0.3f, sb, PatternDrawer.warningStripe);
                if (Button.doButton(87654 + index, x, y, (int) ((double) bounds.Width * 0.800000011920929), 30, this.missions[index].postingTitle, new Color?()))
                {
                  this.state = 2;
                  this.targetIndex = index;
                }
                y += 35;
              }
            }
            break;
          case 2:
            if (Button.doButton(800003, bounds.X + 10, bounds.Y + this.topBar.Height + 10, 300, 30, LocaleTerms.Loc("Back"), new Color?(this.themeColor)))
              this.state = 1;
            int num6 = 60;
            int num7 = 84;
            Rectangle destinationRectangle = new Rectangle(bounds.X + 30, bounds.Y + this.topBar.Height + num6, num7, num7);
            sb.Draw(this.logo, destinationRectangle, this.themeColor);
            int num8 = num6 + 30;
            TextItem.doFontLabel(new Vector2((float) (bounds.X + 34 + num7), (float) (bounds.Y + this.topBar.Height + num8)), this.missions[this.targetIndex].postingTitle, GuiData.font, new Color?(), (float) (bounds.Width - (36 + num7 + 6)), 40f, false);
            int num9 = num8 + 40;
            PatternDrawer.draw(new Rectangle(destinationRectangle.X + destinationRectangle.Width + 2, bounds.Y + this.topBar.Height + num9 - 8, bounds.Width - (destinationRectangle.X - bounds.X + destinationRectangle.Width + 10), PatternDrawer.warningStripe.Height / 2), 1f, Color.Transparent, this.themeColor, sb, PatternDrawer.warningStripe);
            int num10 = num9 + 36;
            string text2 = Utils.SuperSmartTwimForWidth(this.missions[this.targetIndex].postingBody, bounds.Width - 60, GuiData.tinyfont);
            if (this.TextRegion == null)
              this.TextRegion = new ScrollableTextRegion(sb.GraphicsDevice);
            this.TextRegion.Draw(new Rectangle(bounds.X + 30, bounds.Y + this.topBar.Height + num10, bounds.Width - 50, bounds.Height - num10 - this.topBar.Height - 10), text2, sb);
            bool flag = this.os.currentFaction != null && this.os.currentFaction.idName.ToLower() == this.groupName.ToLower();
            if (this.missionAssigner && this.os.currentMission == null && flag && Button.doButton(800005, bounds.X + bounds.Width / 2 - 10, bounds.Y + bounds.Height - 35, bounds.Width / 2, 30, LocaleTerms.Loc("Accept"), new Color?(this.os.highlightColor)))
            {
              this.os.currentMission = this.missions[this.targetIndex];
              ActiveMission activeMission = (ActiveMission) ComputerLoader.readMission(this.missions[this.targetIndex].reloadGoalsSourceFile);
              this.missions[this.targetIndex].sendEmail(this.os);
              this.missions[this.targetIndex].ActivateSuppressedStartFunctionIfPresent();
              this.removeMission(this.targetIndex);
              this.state = 1;
              break;
            }
            if (this.missionAssigner && this.os.currentMission != null)
            {
              if (this.os.currentMission.wasAutoGenerated && Button.doButton(8000105, bounds.X + 6, bounds.Y + bounds.Height - 29, 210, 25, LocaleTerms.Loc("Abandon Current Contract"), new Color?(this.os.lockedColor)))
              {
                this.os.currentMission = (ActiveMission) null;
                this.os.currentFaction.contractAbbandoned((object) this.os);
              }
              TextItem.doFontLabel(new Vector2((float) (bounds.X + 10), (float) (bounds.Y + bounds.Height - 52)), LocaleTerms.Loc("Mission Unavailable") + " : " + (flag ? LocaleTerms.Loc("Complete Existing Contracts") : LocaleTerms.Loc("User ID Assigned to Different Faction") + " "), GuiData.smallfont, new Color?(), (float) (bounds.Width - 20), 30f, false);
              break;
            }
            if (!this.missionAssigner || flag)
              break;
            TextItem.doFontLabel(new Vector2((float) (bounds.X + 10), (float) (bounds.Y + bounds.Height - 52)), LocaleTerms.Loc("Mission Unavailable") + " : " + LocaleTerms.Loc("User ID Assigned to Different Faction") + " ", GuiData.smallfont, new Color?(), (float) (bounds.Width - 20), 30f, false);
            break;
          case 3:
            this.doLoginDisplay(bounds, sb);
            break;
        }
      }
    }
  }
}
