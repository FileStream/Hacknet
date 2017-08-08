// Decompiled with JetBrains decompiler
// Type: Hacknet.OnlineWebServerDaemon
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Net;

namespace Hacknet
{
  internal class OnlineWebServerDaemon : WebServerDaemon
  {
    private static string DEFAULT_PAGE_URL = "http://www.google.com";
    public string webURL = OnlineWebServerDaemon.DEFAULT_PAGE_URL;
    private string lastRequestedURL = (string) null;

    public OnlineWebServerDaemon(Computer computer, string serviceName, OS opSystem)
      : base(computer, serviceName, opSystem, "Content/Web/BaseImageWebPage.html")
    {
    }

    public void setURL(string url)
    {
      this.webURL = url;
    }

    public override void LoadWebPage(string url = null)
    {
      if (url == null || url == "index.html")
        url = this.webURL;
      WebClient webClient = new WebClient();
      webClient.DownloadStringAsync(new Uri(url));
      webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.web_DownloadStringCompleted);
      this.lastRequestedURL = url;
    }

    private void web_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
    {
      if (!e.Cancelled && e.Error == null)
        this.instantiateWebPage(e.Result);
      else
        this.instantiateWebPage(FileSanitiser.purifyStringForDisplay(Utils.readEntireFile("Content/Web/404Page.html")));
    }

    private void instantiateWebPage(string body)
    {
      string str1 = FileSanitiser.purifyStringForDisplay(body);
      string str2 = this.lastRequestedURL;
      FileEntry fileEntry = this.root.searchForFile(str2);
      if (fileEntry == null)
      {
        fileEntry = this.root.searchForFile("index.html");
        str2 = "index.html";
      }
      if (fileEntry != null)
        fileEntry.data = str1;
      base.LoadWebPage(str2);
    }

    public override string getSaveString()
    {
      return "<OnlineWebServer name=\"" + this.name + "\" url=\"" + this.webURL + "\" />";
    }
  }
}
