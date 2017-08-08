// Decompiled with JetBrains decompiler
// Type: Hacknet.NetmapSortingAlgorithms
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;

namespace Hacknet
{
  public static class NetmapSortingAlgorithms
  {
    internal static Vector2 GetNodePosition(NetmapSortingAlgorithm algorithm, float mapWidth, float mapHeight, Computer node, int nodeIndex, int totalNodes, int totalRevealedNodes, OS os)
    {
      switch (algorithm)
      {
        case NetmapSortingAlgorithm.Grid:
          int num1 = 5;
          int num2 = (int) ((double) totalNodes / (double) num1) + 1;
          if (totalRevealedNodes > 60)
          {
            num1 = 7;
            num2 = (int) ((double) totalNodes / (double) num1) + 1;
          }
          if (totalRevealedNodes > 150)
          {
            num1 = 9;
            num2 = (int) ((double) totalNodes / (double) num1) + 1;
          }
          int num3 = (int) ((double) nodeIndex / (double) num1 + 0.0);
          int num4 = num1 * num2;
          if (totalNodes > num4 - 10)
          {
            num3 = (int) ((double) nodeIndex / (double) num1 + 0.5);
            ++num2;
          }
          int num5 = nodeIndex % num1;
          float xout1 = (float) num3 / ((float) num2 - 2f);
          float yout1 = (float) num5 / ((float) num1 - 1f);
          NetmapSortingAlgorithms.ClipToMargins(xout1, yout1, totalRevealedNodes, out xout1, out yout1);
          float x1 = xout1 * mapWidth;
          yout1 *= mapHeight;
          return new Vector2(x1, yout1);
        case NetmapSortingAlgorithm.Chaos:
          return new Vector2(Utils.randm(1f) * mapWidth, Utils.randm(1f) * mapHeight);
        case NetmapSortingAlgorithm.LockGrid:
          int num6 = 5;
          int count = os.netMap.visibleNodes.Count;
          int num7 = (int) ((double) (count + (int) ((double) (totalNodes - count) / 8.0)) / (double) num6) + 1;
          if (count > 50)
          {
            num6 = 6;
            num7 = (int) ((double) count / (double) num6) + 1;
          }
          if (count > 80)
          {
            num6 = 7;
            num7 = (int) ((double) count / (double) num6) + 1;
          }
          if (count > 110)
          {
            num6 = 8;
            num7 = (int) ((double) count / (double) num6) + 1;
          }
          if (count > 150)
          {
            num6 = 9;
            num7 = (int) ((double) count / (double) num6) + 1;
          }
          int num8 = Math.Max(num7 + 1, 3);
          int num9 = os.netMap.visibleNodes.IndexOf(os.netMap.nodes.IndexOf(node));
          if (num9 == -1)
            num9 = 1;
          int num10 = num9 / num6;
          if (count > 150)
          {
            num10 = (int) ((double) num9 / (double) num6 + 0.5);
            ++num8;
          }
          int num11 = num9 % num6;
          float xout2 = (float) num10 / ((float) num8 - 2f);
          float yout2 = (float) num11 / ((float) num6 - 1f);
          NetmapSortingAlgorithms.ClipToMargins(xout2, yout2, totalRevealedNodes, out xout2, out yout2);
          float x2 = xout2 * mapWidth;
          yout2 *= mapHeight;
          return new Vector2(x2, yout2);
        default:
          Vector2 vector2 = Utils.Clamp(node.location, 0.0f, 1f);
          return new Vector2(vector2.X * mapWidth, vector2.Y * mapHeight);
      }
    }

    private static void ClipToMargins(float x, float y, int revealed, out float xout, out float yout)
    {
      bool flag = revealed > 60;
      xout = Math.Min(flag ? 0.999f : 0.98f, Math.Max(flag ? 1f / 1000f : 0.02f, x));
      yout = Math.Min(flag ? 0.98f : 0.93f, Math.Max(flag ? 1f / 500f : 0.02f, y));
    }
  }
}
