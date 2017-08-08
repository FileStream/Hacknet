// Decompiled with JetBrains decompiler
// Type: Hacknet.PostProcessor
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
  public static class PostProcessor
  {
    public static bool bloomEnabled = true;
    public static bool scanlinesEnabled = true;
    public static bool dangerModeEnabled = false;
    public static float dangerModePercentComplete = 0.0f;
    public static bool EndingSequenceFlashOutActive = false;
    public static float EndingSequenceFlashOutPercentageComplete = 0.0f;
    private static RenderTarget2D target;
    private static RenderTarget2D backTarget;
    private static RenderTarget2D dangerBufferTarget;
    private static GraphicsDevice device;
    private static SpriteBatch sb;
    private static Effect bloom;
    private static Effect blur;
    private static Effect danger;
    private static Color bloomColor;
    private static Color dangerLineColor;
    private static Color dangerLineColorAlt;
    private static Color bloomAbsenceHighlighterColor;

    public static void init(GraphicsDevice gDevice, SpriteBatch spriteBatch, ContentManager content)
    {
      PostProcessor.device = gDevice;
      PostProcessor.GenerateMainTarget(gDevice);
      GraphicsDevice graphicsDevice1 = gDevice;
      Viewport viewport1 = gDevice.Viewport;
      int width1 = viewport1.Width;
      viewport1 = gDevice.Viewport;
      int height1 = viewport1.Height;
      PostProcessor.backTarget = new RenderTarget2D(graphicsDevice1, width1, height1);
      GraphicsDevice graphicsDevice2 = gDevice;
      Viewport viewport2 = gDevice.Viewport;
      int width2 = viewport2.Width;
      viewport2 = gDevice.Viewport;
      int height2 = viewport2.Height;
      PostProcessor.dangerBufferTarget = new RenderTarget2D(graphicsDevice2, width2, height2);
      PostProcessor.sb = spriteBatch;
      PostProcessor.bloom = content.Load<Effect>("Shaders/Bloom");
      PostProcessor.blur = content.Load<Effect>("Shaders/DOFBlur");
      PostProcessor.danger = content.Load<Effect>("Shaders/DangerEffect");
      PostProcessor.blur.CurrentTechnique = PostProcessor.blur.Techniques["SmoothGaussBlur"];
      PostProcessor.danger.CurrentTechnique = PostProcessor.danger.Techniques["PostProcess"];
      PostProcessor.bloomColor = new Color(90, 90, 90, 0);
      PostProcessor.bloomAbsenceHighlighterColor = new Color(70, 70, 70, 0);
      PostProcessor.dangerLineColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, 0);
      PostProcessor.dangerLineColorAlt = new Color(240, 0, 0, 0);
    }

    public static void GenerateMainTarget(GraphicsDevice gDevice)
    {
      GraphicsDevice graphicsDevice = gDevice;
      Viewport viewport = gDevice.Viewport;
      int width = viewport.Width;
      viewport = gDevice.Viewport;
      int height = viewport.Height;
      int num1 = 0;
      int num2 = 11;
      int num3 = 0;
      int preferredMultiSampleCount = SettingsLoader.ShouldMultisample ? 4 : 0;
      int num4 = 2;
      PostProcessor.target = new RenderTarget2D(graphicsDevice, width, height, num1 != 0, (SurfaceFormat) num2, (DepthFormat) num3, preferredMultiSampleCount, (RenderTargetUsage) num4);
    }

    public static void begin()
    {
      PostProcessor.device.SetRenderTarget(PostProcessor.target);
    }

    public static void end()
    {
      PostProcessor.device.SetRenderTarget(PostProcessor.backTarget);
      PostProcessor.sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, PostProcessor.blur);
      PostProcessor.sb.Draw((Texture2D) PostProcessor.target, Vector2.Zero, Color.White);
      PostProcessor.sb.End();
      RenderTarget2D renderTarget = PostProcessor.dangerModeEnabled ? PostProcessor.dangerBufferTarget : (RenderTarget2D) null;
      PostProcessor.device.SetRenderTarget(renderTarget);
      if (PostProcessor.EndingSequenceFlashOutActive)
        PostProcessor.device.Clear(Color.Black);
      PostProcessor.sb.Begin();
      Rectangle fullscreenRect = PostProcessor.GetFullscreenRect();
      if (PostProcessor.EndingSequenceFlashOutActive)
        FlickeringTextEffect.DrawFlickeringSprite(PostProcessor.sb, fullscreenRect, (Texture2D) PostProcessor.target, 12f, 0.0f, (object) null, Color.White);
      else
        PostProcessor.sb.Draw((Texture2D) PostProcessor.target, fullscreenRect, Color.White);
      if (PostProcessor.bloomEnabled)
        PostProcessor.sb.Draw((Texture2D) PostProcessor.backTarget, fullscreenRect, PostProcessor.bloomColor);
      else
        PostProcessor.sb.Draw((Texture2D) PostProcessor.target, fullscreenRect, PostProcessor.bloomAbsenceHighlighterColor);
      PostProcessor.sb.End();
      if (!PostProcessor.dangerModeEnabled)
        return;
      PostProcessor.DrawDangerModeFliters();
    }

    internal static Texture2D GetLastRenderedCompleteFrame()
    {
      return (Texture2D) PostProcessor.backTarget;
    }

    public static string GetStatusReportString()
    {
      return "Post Processor" + "\r\n Target : " + (object) PostProcessor.target + "\r\n TargetDisposed : " + (object) PostProcessor.target.IsDisposed + "\r\n BTarget : " + (object) PostProcessor.backTarget + "\r\n BTargetDisposed : " + (object) PostProcessor.backTarget.IsDisposed + "\r\n DBTarget : " + (object) PostProcessor.dangerBufferTarget + "\r\n DBTargetDisposed : " + (object) PostProcessor.dangerBufferTarget.IsDisposed;
    }

    private static Rectangle GetFullscreenRect()
    {
      return new Rectangle(0, 0, PostProcessor.target.Width, PostProcessor.target.Height);
    }

    private static void DrawDangerModeFliters()
    {
      PostProcessor.danger.Parameters["FlickerMultiplier"].SetValue(Utils.randm(0.4f));
      int y = (int) ((double) PostProcessor.dangerModePercentComplete * (double) PostProcessor.dangerBufferTarget.Height);
      PostProcessor.danger.Parameters["PercentDown"].SetValue(PostProcessor.dangerModePercentComplete);
      PostProcessor.device.SetRenderTarget((RenderTarget2D) null);
      PostProcessor.sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, PostProcessor.danger);
      PostProcessor.sb.Draw((Texture2D) PostProcessor.dangerBufferTarget, Vector2.Zero, Color.White);
      PostProcessor.sb.End();
      PostProcessor.sb.Begin();
      Rectangle destinationRectangle = new Rectangle(0, y, PostProcessor.dangerBufferTarget.Width, 1);
      PostProcessor.sb.Draw(Utils.white, destinationRectangle, PostProcessor.dangerLineColor * (Utils.randm(0.7f) + 0.3f));
      destinationRectangle.Y -= 1 + (Utils.flipCoin() ? 1 : 0);
      PostProcessor.sb.Draw(Utils.white, destinationRectangle, Color.Lerp(PostProcessor.dangerLineColor * 0.4f, PostProcessor.dangerLineColorAlt, Utils.randm(0.5f)));
      destinationRectangle.Y += 1 + (Utils.flipCoin() ? -2 : 0);
      PostProcessor.sb.Draw(Utils.white, destinationRectangle, Color.Lerp(PostProcessor.dangerLineColor * 0.4f, PostProcessor.dangerLineColorAlt, Utils.randm(0.5f)));
      PostProcessor.sb.End();
    }
  }
}
