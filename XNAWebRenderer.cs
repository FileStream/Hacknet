// Decompiled with JetBrains decompiler
// Type: XNAWebRenderer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Runtime.InteropServices;

public static class XNAWebRenderer
{
  private const string nativeLibName = "XNAWebRenderer.dll";

  [DllImport("XNAWebRenderer.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void XNAWR_Initialize([MarshalAs(UnmanagedType.LPStr)] string initialURL, XNAWebRenderer.TextureUpdatedDelegate callback, int width, int height);

  [DllImport("XNAWebRenderer.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void XNAWR_Shutdown();

  [DllImport("XNAWebRenderer.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void XNAWR_Update();

  [DllImport("XNAWebRenderer.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void XNAWR_LoadURL([MarshalAs(UnmanagedType.LPStr)] string URL);

  [DllImport("XNAWebRenderer.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void XNAWR_SetViewport(int width, int height);

  public delegate void TextureUpdatedDelegate(IntPtr buffer);
}
