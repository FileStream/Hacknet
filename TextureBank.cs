// Decompiled with JetBrains decompiler
// Type: Hacknet.TextureBank
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet
{
  public static class TextureBank
  {
    public static List<LoadedTexture> textures = new List<LoadedTexture>();

    public static Texture2D load(string filename, ContentManager content)
    {
      for (int index = 0; index < TextureBank.textures.Count; ++index)
      {
        if (TextureBank.textures[index].path.Equals(filename))
        {
          if (TextureBank.textures[index].tex.IsDisposed)
          {
            TextureBank.textures.Remove(TextureBank.textures[index]);
          }
          else
          {
            LoadedTexture texture = TextureBank.textures[index];
            ++texture.retainCount;
            TextureBank.textures[index] = texture;
            return texture.tex;
          }
        }
      }
      try
      {
        Texture2D texture2D = content.Load<Texture2D>(filename);
        TextureBank.textures.Add(new LoadedTexture()
        {
          tex = texture2D,
          path = filename,
          retainCount = 1
        });
        return texture2D;
      }
      catch (Exception ex)
      {
        Console.WriteLine("File \"" + filename + "\" Experienced an Error in Loading\n" + (object) ex);
        return (Texture2D) null;
      }
    }

    public static Texture2D getIfLoaded(string filename)
    {
      foreach (LoadedTexture texture in TextureBank.textures)
      {
        if (texture.path.Equals(filename))
        {
          if (!texture.tex.IsDisposed)
            return texture.tex;
          TextureBank.textures.Remove(texture);
        }
      }
      return (Texture2D) null;
    }

    public static void unload(Texture2D tex)
    {
      TextureBank.unloadWithoutRemoval(tex);
    }

    public static void unloadWithoutRemoval(Texture2D tex)
    {
      for (int index = 0; index < TextureBank.textures.Count; ++index)
      {
        if (TextureBank.textures[index].tex.Equals((object) tex))
        {
          if (TextureBank.textures[index].tex.IsDisposed)
          {
            TextureBank.textures.Remove(TextureBank.textures[index]);
          }
          else
          {
            LoadedTexture texture = TextureBank.textures[index];
            --texture.retainCount;
            TextureBank.textures[index] = texture;
            break;
          }
        }
      }
    }
  }
}
