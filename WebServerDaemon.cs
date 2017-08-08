// Decompiled with JetBrains decompiler
// Type: Hacknet.WebServerDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Extensions;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Hacknet
{
  internal class WebServerDaemon : Daemon
  {
    private bool shouldShow404 = false;
    private string saveURL = "";
    public const string ROOT_FOLDERNAME = "web";
    public const string DEFAULT_PAGE_FILE = "index.html";
    public const string TEMP_WEBPAGE_CACHE_FILENAME = "/Content/Web/Cache/HN_OS_WebCache.html";
    private const string DEFAULT_PAGE_DATA_LOCATION = "Content/Web/BaseImageWebPage.html";
    private const string COMPANY_NAME_SENTINAL = "#$#COMPANYNAME#$#";
    private const string COMPANY_NAME_COMPACT_SENTINAL = "#$#LC_COMPANYNAME#$#";
    public const int BASE_BAR_HEIGHT = 16;
    private static string BaseWebpageData;
    private static string BaseComnayPageData;
    private string webPageFileLocation;
    public Folder root;
    public FileEntry lastLoadedFile;

    public WebServerDaemon(Computer computer, string serviceName, OS opSystem, string pageFileLocation = "Content/Web/BaseImageWebPage.html")
      : base(computer, serviceName, opSystem)
    {
      if (pageFileLocation == null)
        return;
      this.webPageFileLocation = pageFileLocation;
      if (this.webPageFileLocation != "Content/Web/BaseImageWebPage.html")
        WebServerDaemon.BaseWebpageData = (string) null;
    }

    public override void initFiles()
    {
      this.root = this.comp.files.root.searchForFolder("web");
      if (this.root == null)
      {
        this.root = new Folder("web");
        this.comp.files.root.folders.Add(this.root);
      }
      if (WebServerDaemon.BaseWebpageData == null)
      {
        if (Settings.IsInExtensionMode)
          this.webPageFileLocation = Utils.GetFileLoadPrefix() + this.webPageFileLocation;
        string data = Utils.readEntireFile(this.webPageFileLocation);
        WebServerDaemon.BaseWebpageData = Settings.ActiveLocale == "en-us" ? FileSanitiser.purifyStringForDisplay(data) : data;
      }
      FileEntry fileEntry = new FileEntry(WebServerDaemon.BaseWebpageData, "index.html");
      this.root.files.Add(fileEntry);
      this.lastLoadedFile = fileEntry;
    }

    public override void loadInit()
    {
      this.root = this.comp.files.root.searchForFolder("web");
    }

    public void generateBaseCorporateSite(string companyName, string targetBaseFile = "Content/Web/BaseCorporatePage.html")
    {
      if (WebServerDaemon.BaseComnayPageData == null || targetBaseFile != "Content/Web/BaseCorporatePage.html")
        WebServerDaemon.BaseComnayPageData = FileSanitiser.purifyStringForDisplay(Utils.readEntireFile(targetBaseFile));
      string dataEntry = WebServerDaemon.BaseComnayPageData.Replace("#$#COMPANYNAME#$#", companyName).Replace("#$#LC_COMPANYNAME#$#", companyName.Replace(' ', '_'));
      FileEntry fileEntry = this.root.searchForFile("index.html");
      if (fileEntry == null)
        fileEntry = new FileEntry(dataEntry, "index.html");
      else
        fileEntry.data = dataEntry;
      this.comp.files.root.searchForFolder("home").files.Add(new FileEntry(fileEntry.data, "index_BACKUP.html"));
    }

    public virtual void LoadWebPage(string url = "index.html")
    {
      this.saveURL = url;
      FileEntry fileEntry = this.root.searchForFile(url);
      if (fileEntry != null)
      {
        this.shouldShow404 = false;
        this.ShowPage(fileEntry.data);
      }
      else
        this.shouldShow404 = true;
      this.lastLoadedFile = fileEntry;
    }

    public virtual void ShowPage(string pageData)
    {
      string str = Directory.GetCurrentDirectory() + "/Content/Web/Cache/HN_OS_WebCache.html";
      if (Settings.IsInExtensionMode)
      {
        string path = Path.Combine(Directory.GetCurrentDirectory(), ExtensionLoader.ActiveExtensionInfo.FolderPath, "Web", "Cache");
        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);
        str = path + "/HN_OS_WebCache.html";
      }
      Utils.writeToFile(pageData, str);
      WebRenderer.navigateTo(str);
    }

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
      bounds.Height -= 16;
      ++bounds.X;
      bounds.Width -= 2;
      if (this.shouldShow404)
      {
        Vector2 vector2 = new Vector2((float) (bounds.X + 10), (float) (bounds.Y + bounds.Height / 4));
        sb.Draw(Utils.white, new Rectangle(bounds.X, (int) vector2.Y, (int) ((double) bounds.Width * 0.7), 80), this.os.highlightColor * 0.3f);
        TextItem.doFontLabel(vector2 + new Vector2(0.0f, 10f), LocaleTerms.Loc("Error 404"), GuiData.font, new Color?(Color.White), float.MaxValue, float.MaxValue, false);
        TextItem.doFontLabel(vector2 + new Vector2(0.0f, 42f), LocaleTerms.Loc("Page not found"), GuiData.smallfont, new Color?(Color.White), float.MaxValue, float.MaxValue, false);
      }
      else
        this.os.postFXDrawActions += (Action) (() => WebRenderer.drawTo(bounds, sb));
      Rectangle rectangle = bounds;
      rectangle.Y += bounds.Height;
      rectangle.Height = 16;
      bool smallButtonDraw = Button.smallButtonDraw;
      Button.smallButtonDraw = true;
      int width = 200;
      if (Button.doButton(83801, rectangle.X + 1, rectangle.Y + 1, width, rectangle.Height - 2, "HN: " + LocaleTerms.Loc("Exit Web View"), new Color?()))
        this.os.display.command = "connect";
      if (this.os.hasConnectionPermission(false) && Button.doButton(83805, rectangle.X + rectangle.Width - (width + 1), rectangle.Y + 1, width, rectangle.Height - 2, LocaleTerms.Loc("View Source"), new Color?()))
        this.showSourcePressed();
      Button.smallButtonDraw = smallButtonDraw;
    }

    public virtual void showSourcePressed()
    {
      string str = "";
      int count = Programs.getNavigationPathAtPath("", this.os, this.root).Count;
      for (int index = 0; index < count; ++index)
        str += "../";
      this.os.runCommand("cd " + (str + "web"));
      if (this.lastLoadedFile == null)
        return;
      this.os.delayer.Post(ActionDelayer.Wait(0.1), (Action) (() => this.os.runCommand("cat " + this.lastLoadedFile.name)));
    }

    public override void navigatedTo()
    {
      this.LoadWebPage("index.html");
    }

    public override string getSaveString()
    {
      return "<WebServer name=\"" + this.name + "\" url=\"" + this.saveURL + "\" />";
    }
  }
}
