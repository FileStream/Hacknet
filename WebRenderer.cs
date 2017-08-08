// Decompiled with JetBrains decompiler
// Type: Hacknet.WebRenderer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices;

namespace Hacknet
{
  internal class WebRenderer
  {
    public static bool Enabled = true;
    public static XNAWebRenderer.TextureUpdatedDelegate textureUpdated = new XNAWebRenderer.TextureUpdatedDelegate(WebRenderer.TextureUpdated);
    private static int width = 500;
    private static int height = 500;
    private static string url = "http://www.google.com";
    private static WebRenderer instance;
    public static Texture2D texture;
    private static byte[] texBuffer;
    private static GraphicsDevice graphics;
    private static bool loadingPage;

    public static void setSize(int frameWidth, int frameHeight)
    {
      WebRenderer.width = frameWidth;
      WebRenderer.height = frameHeight;
      if (!WebRenderer.Enabled)
        return;
      if (WebRenderer.graphics != null)
      {
        if (WebRenderer.texture != null)
          WebRenderer.texture.Dispose();
        WebRenderer.texture = new Texture2D(WebRenderer.graphics, WebRenderer.width, WebRenderer.height, false, SurfaceFormat.Color);
        WebRenderer.texBuffer = new byte[WebRenderer.width * WebRenderer.height * 4];
        XNAWebRenderer.XNAWR_SetViewport(WebRenderer.width, WebRenderer.height);
      }
      WebRenderer.loadingPage = true;
    }

    public static void init(GraphicsDevice gd)
    {
      if (!WebRenderer.Enabled)
      {
        WebRenderer.loadingPage = true;
      }
      else
      {
        WebRenderer.instance = new WebRenderer();
        WebRenderer.graphics = gd;
        WebRenderer.loadingPage = true;
      }
    }

    public static void navigateTo(string urlTo)
    {
      WebRenderer.loadingPage = true;
      Console.WriteLine("Launching Web Thread");
      WebRenderer.url = urlTo;
      if (!WebRenderer.Enabled)
        return;
      XNAWebRenderer.XNAWR_LoadURL(new Uri(WebRenderer.url).ToString());
    }

    private static void TextureUpdated(IntPtr buffer)
    {
      try
      {
        Marshal.Copy(buffer, WebRenderer.texBuffer, 0, WebRenderer.texBuffer.Length);
        WebRenderer.texture.SetData<byte>(WebRenderer.texBuffer);
        WebRenderer.loadingPage = false;
      }
      catch (AccessViolationException ex)
      {
        Console.WriteLine((object) ex);
        WebRenderer.navigateTo(WebRenderer.url);
      }
    }

    public static void drawTo(Rectangle bounds, SpriteBatch sb)
    {
      if (!WebRenderer.loadingPage)
        sb.Draw(WebRenderer.texture, bounds, Color.White);
      else
        WebpageLoadingEffect.DrawLoadingEffect(bounds, sb, (object) OS.currentInstance, true);
    }

    public static string getURL()
    {
      return WebRenderer.url;
    }
  }
}
