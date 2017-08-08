// Decompiled with JetBrains decompiler
// Type: Hacknet.UIUtils.AnimatedSpriteExporter
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Hacknet.UIUtils
{
  public static class AnimatedSpriteExporter
  {
    public static void ExportAnimation(string folderPath, string nameStarter, int width, int height, float framesPerSecond, float totalTime, GraphicsDevice gd, Action<float> update, Action<SpriteBatch, Rectangle> draw, int antialiasingMultiplier = 1)
    {
      int width1 = width * antialiasingMultiplier;
      int height1 = height * antialiasingMultiplier;
      RenderTarget2D renderTarget = new RenderTarget2D(gd, width1, height1, false, SurfaceFormat.Rgba64, DepthFormat.Depth16, 8, RenderTargetUsage.PlatformContents);
      SpriteBatch spriteBatch1 = new SpriteBatch(gd);
      gd.PresentationParameters.MultiSampleCount = 8;
      RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
      gd.SetRenderTarget(renderTarget);
      float num1 = 1f / framesPerSecond;
      float num2 = 0.0f;
      int num3 = 0;
      if (!Directory.Exists(folderPath))
        Directory.CreateDirectory(folderPath);
      SpriteBatch spriteBatch2 = GuiData.spriteBatch;
      GuiData.spriteBatch = spriteBatch1;
      Rectangle rectangle = new Rectangle(0, 0, width1, height1);
      while ((double) num2 < (double) totalTime)
      {
        gd.Clear(Color.Transparent);
        spriteBatch1.Begin();
        draw(spriteBatch1, rectangle);
        spriteBatch1.End();
        update(num1);
        gd.SetRenderTarget((RenderTarget2D) null);
        string str = nameStarter + "_" + (object) num3 + ".png";
        using (FileStream fileStream = File.Create(folderPath + "/" + str))
          renderTarget.SaveAsPng((Stream) fileStream, width, height);
        gd.SetRenderTarget(renderTarget);
        ++num3;
        num2 += num1;
      }
      GuiData.spriteBatch = spriteBatch2;
      gd.SetRenderTarget(currentRenderTarget);
    }
  }
}
