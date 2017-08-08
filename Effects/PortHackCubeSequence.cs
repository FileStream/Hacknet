// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.PortHackCubeSequence
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using System;

namespace Hacknet.Effects
{
  public class PortHackCubeSequence
  {
    private float rotTime = 0.0f;
    private float elapsedTime = 0.0f;
    private float startup = 0.3f;
    private float spinup = 0.0f;
    private float runtime = 0.0f;
    private float spindown = 0.0f;
    private float idle = 0.0f;
    public bool HeartFadeSequenceComplete = false;
    public bool ShouldCentralSpinInfinitley = false;

    public void Reset()
    {
      this.spinup = this.startup = this.runtime = this.idle = this.spindown = this.rotTime = 0.0f;
      this.HeartFadeSequenceComplete = false;
    }

    public void DrawSequence(Rectangle dest, float t, float totalTime)
    {
      float num1 = 0.6f;
      float val1_1 = 1f;
      float val1_2 = this.ShouldCentralSpinInfinitley ? float.MaxValue : totalTime - 1.7f;
      float maxValue1 = float.MaxValue;
      float maxValue2 = float.MaxValue;
      this.elapsedTime += t;
      Math.Max(0.0f, this.elapsedTime);
      float num2 = 1f;
      float num3 = 1f;
      float num4 = 1f;
      if ((double) this.startup < (double) num1)
      {
        this.startup += t;
        num2 = 0.0f;
        num3 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(this.startup / num1));
      }
      else if ((double) this.spinup < (double) val1_1)
      {
        this.spinup = Math.Min(val1_1, this.spinup + t);
        num2 = this.spinup / val1_1;
        this.rotTime += t * num2;
      }
      else if ((double) this.runtime < (double) val1_2)
      {
        this.runtime = Math.Min(val1_2, this.runtime + t);
        this.rotTime += t;
      }
      else if ((double) this.spindown < (double) maxValue1)
      {
        this.spindown += t;
        float num5 = (float) ((double) Math.Min(1f, this.spindown) / 2.29999995231628 / 1.0);
        num2 = (float) (0.100000001490116 + 0.899999976158142 * (1.0 - (double) num5));
        if ((double) num5 >= 0.400000005960464)
          num2 = 0.2f;
        this.rotTime += t * num2;
        num4 = 1f - num5;
        num3 = (float) (0.300000011920929 + 0.699999988079071 * (double) Utils.QuadraticOutCurve(1f - num5));
      }
      else if ((double) this.idle < (double) maxValue2)
      {
        this.idle += t;
        num2 = 0.1f;
        this.rotTime += t * (num2 * 2f);
        num4 = 0.0f;
        num3 = 0.3f;
      }
      else
        this.spinup = this.startup = this.runtime = this.idle = this.spindown = this.rotTime = 0.0f;
      int num6 = 20;
      for (int index1 = 0; index1 < num6; ++index1)
      {
        float num5 = (float) index1 / (float) num6;
        float num7 = (float) (1.0 + 0.0500000007450581 * (double) num2 * ((double) (num6 - index1) / (double) num6 * 10.0));
        float num8 = num4;
        float num9 = Math.Max(0.0f, (float) ((double) this.rotTime * (double) num7 - (double) index1 * 0.100000001490116 * (double) num8)) * 1.5f;
        float num10 = num3;
        for (int index2 = 0; index2 < index1; ++index2)
          num10 *= num3;
        float num11 = num3 * ((float) (num6 - index1) / (float) num6);
        bool flag = (double) num5 <= (double) num2;
        Cube3D.RenderWireframe(new Vector3((float) (dest.X / 2), 0.0f, 0.0f), (float) (2.59999990463257 + (double) index1 / 4.0 * (double) num10), new Vector3(MathHelper.ToRadians(35.4f), MathHelper.ToRadians(45f) + num9, MathHelper.ToRadians(0.0f)), Color.White);
      }
    }

    public void DrawHeartSequence(Rectangle dest, float t, float totalTime)
    {
      float num1 = 3f;
      float num2 = 10f;
      float num3 = 2f;
      float num4 = 9f;
      float maxValue = float.MaxValue;
      float num5 = 2f;
      float num6 = 21f;
      this.elapsedTime += t;
      Math.Max(0.0f, this.elapsedTime);
      float num7 = 1f;
      float num8 = 1f;
      float num9 = 1f;
      float num10 = 0.0f;
      if ((double) this.startup < (double) num1)
      {
        this.startup += t;
        num8 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(this.startup / 3f));
      }
      else if ((double) this.spinup < (double) num2)
      {
        this.spinup = Math.Min(10f, this.spinup + t);
        num7 = this.spinup / 10f;
        this.rotTime += t * num7;
      }
      else if ((double) this.runtime < (double) num3)
      {
        this.runtime = Math.Min(2f, this.runtime + t);
        this.rotTime += t;
      }
      else if ((double) this.spindown < (double) num4)
      {
        this.spindown += t;
        float num11 = this.spindown / 9f;
        num7 = (float) (0.100000001490116 + 0.899999976158142 * (1.0 - (double) num11));
        this.rotTime += t * num7;
        num9 = 1f - num11;
        num8 = (float) (0.300000011920929 + 0.699999988079071 * (double) Utils.QuadraticOutCurve(1f - num11));
      }
      else if ((double) this.idle < (double) maxValue)
      {
        this.idle += t;
        num7 = 0.1f;
        this.rotTime += t * num7;
        num9 = 0.0f;
        num8 = 0.3f;
        if ((double) this.idle > (double) num5)
        {
          num10 = Utils.QuadraticOutCurve(Math.Min(1f, (this.idle - num5) / num6));
          this.HeartFadeSequenceComplete = (double) num10 >= 1.0;
        }
      }
      else
        this.spinup = this.startup = this.runtime = this.idle = this.spindown = this.rotTime = 0.0f;
      int num12 = 20;
      for (int index1 = 0; index1 < num12; ++index1)
      {
        float num11 = (float) index1 / (float) num12;
        float num13 = (float) (1.0 + 0.0500000007450581 * (double) num7 * ((double) (num12 - index1) / (double) num12 * 10.0));
        float num14 = num9;
        float num15 = Math.Max(0.0f, (float) ((double) this.rotTime * (double) num13 - (double) index1 * 0.100000001490116 * (double) num14)) * 1.5f;
        float num16 = num8;
        for (int index2 = 0; index2 < index1; ++index2)
          num16 *= num8;
        float num17 = num8 * ((float) (num12 - index1) / (float) num12);
        bool flag = (double) num11 <= (double) num7;
        float num18 = 1f;
        Color color = Color.White;
        if ((double) num10 > 0.0)
        {
          float num19 = 1f / (float) num12;
          if ((double) index1 * (double) num19 < (double) num10)
          {
            float num20 = (float) (index1 + 1) * num19;
            if ((double) num10 > (double) num20)
            {
              num18 = 0.0f;
              color = Color.Transparent;
            }
            else
            {
              float amount = (float) (1.0 - (double) num10 % (double) num19 / (double) num19);
              color = Color.Lerp(Utils.AddativeRed, Color.Red, amount) * amount;
            }
          }
        }
        Cube3D.RenderWireframe(new Vector3((float) (dest.X / 2), 0.0f, 0.0f), (float) (2.59999990463257 + (double) index1 / 4.0 * (double) num16), new Vector3(MathHelper.ToRadians(35.4f), MathHelper.ToRadians(45f) + num15, MathHelper.ToRadians(0.0f)), color);
      }
    }
  }
}
