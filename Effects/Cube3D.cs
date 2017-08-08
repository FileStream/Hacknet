// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.Cube3D
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hacknet.Effects
{
  internal class Cube3D
  {
    private const int NUM_VERTICES = 36;
    private const int NUM_INDICIES = 14;
    private static VertexPositionNormalTexture[] verts;
    private static VertexBuffer vBuffer;
    private static IndexBuffer ib;
    private static BasicEffect wireframeEfect;
    private static RasterizerState wireframeRaster;

    public static void Initilize(GraphicsDevice gd)
    {
      Cube3D.ConstructCube();
      Cube3D.vBuffer = new VertexBuffer(gd, VertexPositionNormalTexture.VertexDeclaration, 36, BufferUsage.WriteOnly);
      Cube3D.vBuffer.SetData<VertexPositionNormalTexture>(Cube3D.verts);
      gd.SetVertexBuffer(Cube3D.vBuffer);
      Cube3D.ib = new IndexBuffer(gd, IndexElementSize.SixteenBits, 14, BufferUsage.WriteOnly);
      Cube3D.ib.SetData<short>(new short[14]
      {
        (short) 0,
        (short) 1,
        (short) 2,
        (short) 3,
        (short) 4,
        (short) 5,
        (short) 6,
        (short) 7,
        (short) 8,
        (short) 9,
        (short) 10,
        (short) 11,
        (short) 12,
        (short) 13
      });
      gd.Indices = Cube3D.ib;
      Cube3D.wireframeRaster = new RasterizerState();
      Cube3D.wireframeRaster.FillMode = FillMode.WireFrame;
      Cube3D.wireframeRaster.CullMode = CullMode.None;
      Cube3D.wireframeEfect = new BasicEffect(gd);
      Cube3D.wireframeEfect.Projection = Matrix.CreatePerspectiveFieldOfView(0.7853982f, Cube3D.wireframeEfect.GraphicsDevice.Viewport.AspectRatio, 0.01f, 3000f);
    }

    private static void ResetBuffers()
    {
      GraphicsDevice graphicsDevice = Cube3D.wireframeEfect.GraphicsDevice;
      graphicsDevice.SetVertexBuffer(Cube3D.vBuffer);
      graphicsDevice.Indices = Cube3D.ib;
    }

    public static void RenderWireframe(Vector3 position, float scale, Vector3 rotation, Color color)
    {
      Cube3D.RenderWireframe(position, scale, rotation, color, new Vector3(0.0f, 0.0f, 20f));
    }

    public static void RenderWireframe(Vector3 position, float scale, Vector3 rotation, Color color, Vector3 cameraOffset)
    {
      scale = Math.Max(1f / 1000f, scale);
      Cube3D.wireframeEfect.DiffuseColor = Utils.ColorToVec3(color);
      Cube3D.wireframeEfect.GraphicsDevice.BlendState = BlendState.Opaque;
      Cube3D.wireframeEfect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      Cube3D.wireframeEfect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
      Cube3D.ResetBuffers();
      RasterizerState rasterizerState = Cube3D.wireframeEfect.GraphicsDevice.RasterizerState;
      Cube3D.wireframeEfect.GraphicsDevice.RasterizerState = Cube3D.wireframeRaster;
      Matrix matrix = Matrix.CreateTranslation(-new Vector3(0.0f, 0.0f, 0.0f)) * Matrix.CreateScale(scale) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationZ(rotation.Z) * Matrix.CreateTranslation(position);
      Cube3D.wireframeEfect.World = matrix;
      Cube3D.wireframeEfect.View = Matrix.CreateLookAt(cameraOffset, position, Vector3.Up);
      try
      {
        foreach (EffectPass pass in Cube3D.wireframeEfect.CurrentTechnique.Passes)
        {
          pass.Apply();
          Cube3D.wireframeEfect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 36);
        }
      }
      catch (NotSupportedException ex)
      {
        Console.WriteLine("Not supported happened");
      }
      Cube3D.wireframeEfect.GraphicsDevice.RasterizerState = rasterizerState;
    }

    private static void ConstructCube()
    {
      Cube3D.verts = new VertexPositionNormalTexture[36];
      Vector3 position1 = new Vector3(-1f, 1f, -1f);
      Vector3 position2 = new Vector3(-1f, 1f, 1f);
      Vector3 position3 = new Vector3(1f, 1f, -1f);
      Vector3 position4 = new Vector3(1f, 1f, 1f);
      Vector3 position5 = new Vector3(-1f, -1f, -1f);
      Vector3 position6 = new Vector3(-1f, -1f, 1f);
      Vector3 position7 = new Vector3(1f, -1f, -1f);
      Vector3 position8 = new Vector3(1f, -1f, 1f);
      Vector3 normal1 = new Vector3(0.0f, 0.0f, 1f);
      Vector3 normal2 = new Vector3(0.0f, 0.0f, -1f);
      Vector3 normal3 = new Vector3(0.0f, 1f, 0.0f);
      Vector3 normal4 = new Vector3(0.0f, -1f, 0.0f);
      Vector3 normal5 = new Vector3(-1f, 0.0f, 0.0f);
      Vector3 normal6 = new Vector3(1f, 0.0f, 0.0f);
      Vector2 textureCoordinate1 = new Vector2(1f, 0.0f);
      Vector2 textureCoordinate2 = new Vector2(0.0f, 0.0f);
      Vector2 textureCoordinate3 = new Vector2(1f, 1f);
      Vector2 textureCoordinate4 = new Vector2(0.0f, 1f);
      Cube3D.verts[0] = new VertexPositionNormalTexture(position1, normal1, textureCoordinate1);
      Cube3D.verts[1] = new VertexPositionNormalTexture(position5, normal1, textureCoordinate3);
      Cube3D.verts[2] = new VertexPositionNormalTexture(position3, normal1, textureCoordinate2);
      Cube3D.verts[3] = new VertexPositionNormalTexture(position5, normal1, textureCoordinate3);
      Cube3D.verts[4] = new VertexPositionNormalTexture(position7, normal1, textureCoordinate4);
      Cube3D.verts[5] = new VertexPositionNormalTexture(position3, normal1, textureCoordinate2);
      Cube3D.verts[6] = new VertexPositionNormalTexture(position2, normal2, textureCoordinate2);
      Cube3D.verts[7] = new VertexPositionNormalTexture(position4, normal2, textureCoordinate1);
      Cube3D.verts[8] = new VertexPositionNormalTexture(position6, normal2, textureCoordinate4);
      Cube3D.verts[9] = new VertexPositionNormalTexture(position6, normal2, textureCoordinate4);
      Cube3D.verts[10] = new VertexPositionNormalTexture(position4, normal2, textureCoordinate1);
      Cube3D.verts[11] = new VertexPositionNormalTexture(position8, normal2, textureCoordinate3);
      Cube3D.verts[12] = new VertexPositionNormalTexture(position1, normal3, textureCoordinate3);
      Cube3D.verts[13] = new VertexPositionNormalTexture(position4, normal3, textureCoordinate2);
      Cube3D.verts[14] = new VertexPositionNormalTexture(position2, normal3, textureCoordinate1);
      Cube3D.verts[15] = new VertexPositionNormalTexture(position1, normal3, textureCoordinate3);
      Cube3D.verts[16] = new VertexPositionNormalTexture(position3, normal3, textureCoordinate4);
      Cube3D.verts[17] = new VertexPositionNormalTexture(position4, normal3, textureCoordinate2);
      Cube3D.verts[18] = new VertexPositionNormalTexture(position5, normal4, textureCoordinate1);
      Cube3D.verts[19] = new VertexPositionNormalTexture(position6, normal4, textureCoordinate3);
      Cube3D.verts[20] = new VertexPositionNormalTexture(position8, normal4, textureCoordinate4);
      Cube3D.verts[21] = new VertexPositionNormalTexture(position5, normal4, textureCoordinate1);
      Cube3D.verts[22] = new VertexPositionNormalTexture(position8, normal4, textureCoordinate4);
      Cube3D.verts[23] = new VertexPositionNormalTexture(position7, normal4, textureCoordinate2);
      Cube3D.verts[24] = new VertexPositionNormalTexture(position1, normal5, textureCoordinate2);
      Cube3D.verts[25] = new VertexPositionNormalTexture(position6, normal5, textureCoordinate3);
      Cube3D.verts[26] = new VertexPositionNormalTexture(position5, normal5, textureCoordinate4);
      Cube3D.verts[27] = new VertexPositionNormalTexture(position2, normal5, textureCoordinate1);
      Cube3D.verts[28] = new VertexPositionNormalTexture(position6, normal5, textureCoordinate3);
      Cube3D.verts[29] = new VertexPositionNormalTexture(position1, normal5, textureCoordinate2);
      Cube3D.verts[30] = new VertexPositionNormalTexture(position3, normal6, textureCoordinate1);
      Cube3D.verts[31] = new VertexPositionNormalTexture(position7, normal6, textureCoordinate3);
      Cube3D.verts[32] = new VertexPositionNormalTexture(position8, normal6, textureCoordinate4);
      Cube3D.verts[33] = new VertexPositionNormalTexture(position4, normal6, textureCoordinate2);
      Cube3D.verts[34] = new VertexPositionNormalTexture(position3, normal6, textureCoordinate1);
      Cube3D.verts[35] = new VertexPositionNormalTexture(position8, normal6, textureCoordinate4);
    }
  }
}
