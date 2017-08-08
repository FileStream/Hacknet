// Decompiled with JetBrains decompiler
// Type: Hacknet.Daemons.Helpers.BasicMedicalMonitor
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hacknet.Daemons.Helpers
{
  public class BasicMedicalMonitor : IMedicalMonitor
  {
    private CLinkBuffer<BasicMedicalMonitor.MonitorRecordKeypoint> data = new CLinkBuffer<BasicMedicalMonitor.MonitorRecordKeypoint>(1024);
    private Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>> UpdateAction;
    private Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>> HeartBeatAction;

    public BasicMedicalMonitor(Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>> updateAction, Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>> heartbeatAction)
    {
      this.UpdateAction = updateAction;
      this.HeartBeatAction = heartbeatAction;
    }

    public float GetCurrentValue(float timeRollback)
    {
      int offset = 0;
      Vector2 vector2 = new Vector2(0.0f, 0.0f);
      float num1 = 0.0f;
      while ((double) num1 < (double) timeRollback)
      {
        BasicMedicalMonitor.MonitorRecordKeypoint monitorRecordKeypoint1 = this.data.Get(offset);
        if ((double) monitorRecordKeypoint1.timeOffset != 0.0)
        {
          if ((double) num1 + (double) monitorRecordKeypoint1.timeOffset >= (double) timeRollback)
          {
            BasicMedicalMonitor.MonitorRecordKeypoint monitorRecordKeypoint2 = this.data.Get(offset - 1);
            float num2 = (timeRollback - num1) / monitorRecordKeypoint1.timeOffset;
            vector2 = Vector2.Lerp(new Vector2(0.0f, monitorRecordKeypoint2.value * 1f), new Vector2(0.0f, monitorRecordKeypoint1.value * 1f), 1f - num2);
            int num3 = offset - 1;
            break;
          }
          --offset;
          num1 += monitorRecordKeypoint1.timeOffset;
        }
        else
          break;
      }
      return vector2.Y;
    }

    public void Update(float dt)
    {
      List<BasicMedicalMonitor.MonitorRecordKeypoint> monitorRecordKeypointList = this.UpdateAction(this.data.Get(0).value, dt);
      for (int index = 0; index < monitorRecordKeypointList.Count; ++index)
        this.data.Add(monitorRecordKeypointList[index]);
    }

    public void HeartBeat(float beatTime)
    {
      List<BasicMedicalMonitor.MonitorRecordKeypoint> monitorRecordKeypointList = this.HeartBeatAction(this.data.Get(0).value, beatTime);
      for (int index = 0; index < monitorRecordKeypointList.Count; ++index)
        this.data.Add(monitorRecordKeypointList[index]);
    }

    public void Draw(Rectangle bounds, SpriteBatch sb, Color c, float timeRollback)
    {
      int offset = 0;
      float num1 = 4f;
      float num2 = 100f;
      float num3 = (float) bounds.Height / 3f;
      float x = (float) bounds.Width - num1;
      Vector2 vector2_1 = new Vector2(x, 0.0f);
      bool flag = false;
      Vector2 vector2_2 = new Vector2((float) bounds.X, (float) bounds.Y + (float) bounds.Height / 2f);
      BasicMedicalMonitor.MonitorRecordKeypoint monitorRecordKeypoint1;
      Vector2 vector2_3;
      if ((double) timeRollback > 0.0)
      {
        float num4 = 0.0f;
        while ((double) num4 < (double) timeRollback)
        {
          monitorRecordKeypoint1 = this.data.Get(offset);
          if ((double) monitorRecordKeypoint1.timeOffset != 0.0)
          {
            if ((double) num4 + (double) monitorRecordKeypoint1.timeOffset >= (double) timeRollback)
            {
              BasicMedicalMonitor.MonitorRecordKeypoint monitorRecordKeypoint2 = this.data.Get(offset - 1);
              float num5 = (timeRollback - num4) / monitorRecordKeypoint1.timeOffset;
              Vector2 vector2_4 = new Vector2(x, monitorRecordKeypoint2.value * num3);
              vector2_3 = new Vector2(x, monitorRecordKeypoint1.value * num3);
              vector2_1 = Vector2.Lerp(vector2_4, vector2_3, 1f - num5);
              flag = true;
              x -= (float) ((double) monitorRecordKeypoint1.timeOffset * (double) num2 * (1.0 - (double) num5));
              --offset;
              break;
            }
            --offset;
            num4 += monitorRecordKeypoint1.timeOffset;
          }
          else
            break;
        }
      }
      while ((double) x >= (double) num1)
      {
        monitorRecordKeypoint1 = this.data.Get(offset);
        if ((double) monitorRecordKeypoint1.timeOffset == 0.0)
          break;
        vector2_3 = new Vector2(x, monitorRecordKeypoint1.value * num3);
        if (flag)
          Utils.drawLine(sb, vector2_2 + vector2_1, vector2_2 + vector2_3, Vector2.Zero, c, 0.56f);
        x -= monitorRecordKeypoint1.timeOffset * num2;
        vector2_1 = vector2_3;
        --offset;
        flag = true;
      }
    }

    public struct MonitorRecordKeypoint
    {
      public float timeOffset;
      public float value;
    }
  }
}
