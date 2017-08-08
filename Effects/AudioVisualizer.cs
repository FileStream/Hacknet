// Decompiled with JetBrains decompiler
// Type: Hacknet.Effects.AudioVisualizer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hacknet.Effects
{
  public class AudioVisualizer
  {
    private VisualizationData visData = new VisualizationData();
    private ReadOnlyCollection<float> previousSamples = (ReadOnlyCollection<float>) null;
    private double SecondsSinceLastDataUpdate = 0.0;
    private List<ReadOnlyCollection<float>> samplesHistory;

    public void Draw(Rectangle bounds, SpriteBatch sb)
    {
      if (OS.currentInstance == null || !SettingsLoader.ShouldDrawMusicVis)
        return;
      if (this.samplesHistory == null)
      {
        this.samplesHistory = new List<ReadOnlyCollection<float>>();
        this.samplesHistory.Add(new ReadOnlyCollection<float>((IList<float>) new float[0]));
        this.samplesHistory.Add(new ReadOnlyCollection<float>((IList<float>) new float[0]));
        this.samplesHistory.Add(new ReadOnlyCollection<float>((IList<float>) new float[0]));
        this.samplesHistory.Add(new ReadOnlyCollection<float>((IList<float>) new float[0]));
        this.samplesHistory.Add(new ReadOnlyCollection<float>((IList<float>) new float[0]));
        this.samplesHistory.Add(new ReadOnlyCollection<float>((IList<float>) new float[0]));
        this.samplesHistory.Add(new ReadOnlyCollection<float>((IList<float>) new float[0]));
        this.samplesHistory.Add(new ReadOnlyCollection<float>((IList<float>) new float[0]));
        this.samplesHistory.Add(new ReadOnlyCollection<float>((IList<float>) new float[0]));
        this.samplesHistory.Add(new ReadOnlyCollection<float>((IList<float>) new float[0]));
        this.samplesHistory.Add(new ReadOnlyCollection<float>((IList<float>) new float[0]));
        this.samplesHistory.Add(new ReadOnlyCollection<float>((IList<float>) new float[0]));
      }
      if (MediaPlayer.State == MediaState.Playing && (OS.currentInstance != null && OS.currentInstance.lastGameTime != null))
      {
        this.SecondsSinceLastDataUpdate += OS.currentInstance.lastGameTime.ElapsedGameTime.TotalSeconds;
        if (this.SecondsSinceLastDataUpdate >= 1.0 / 24.0)
        {
          MediaPlayer.IsVisualizationEnabled = true;
          try
          {
            MediaPlayer.GetVisualizationData(this.visData);
            this.SecondsSinceLastDataUpdate = 0.0;
          }
          catch (OverflowException ex)
          {
          }
          catch (NullReferenceException ex)
          {
          }
          catch (IndexOutOfRangeException ex)
          {
          }
          catch (Exception ex)
          {
          }
        }
      }
      if (this.visData == null)
        return;
      List<float> floatList = new List<float>(this.visData.Samples.Count);
      for (int index = 0; index < this.visData.Samples.Count; ++index)
      {
        float num = Math.Max(0.0f, Math.Abs(this.visData.Samples[index]));
        if ((double) num > 1.0)
          num = 0.0f;
        floatList.Add(num);
      }
      ReadOnlyCollection<float> readOnlyCollection = new ReadOnlyCollection<float>((IList<float>) floatList);
      if (this.previousSamples == null)
        this.previousSamples = this.visData.Samples;
      float num1 = (float) bounds.Height / (float) readOnlyCollection.Count;
      Vector2 vector2 = new Vector2((float) bounds.X, (float) bounds.Y);
      float num2 = 0.2f;
      for (int index1 = 0; index1 < readOnlyCollection.Count; ++index1)
      {
        int index2 = index1;
        float num3 = Math.Abs(readOnlyCollection[index2]);
        sb.Draw(Utils.white, new Rectangle((int) vector2.X, (int) vector2.Y, (int) ((double) bounds.Width * (double) num3), Math.Max(1, (int) num1)), OS.currentInstance.highlightColor * (float) (0.200000002980232 + (double) num3 / 2.0) * num2);
        vector2.Y += num1;
      }
      this.samplesHistory.Add(new ReadOnlyCollection<float>((IList<float>) this.visData.Samples.ToArray<float>()));
      this.samplesHistory.RemoveAt(0);
      for (int index1 = 0; index1 < this.samplesHistory.Count; ++index1)
      {
        float num3 = (float) index1 / (float) (this.samplesHistory.Count - 1);
        vector2.Y = (float) bounds.Y;
        for (int index2 = 0; index2 < this.samplesHistory[index1].Count; ++index2)
        {
          Color color = index1 >= this.samplesHistory.Count - 1 ? Utils.AddativeWhite * 0.7f : OS.currentInstance.highlightColor;
          sb.Draw(Utils.white, new Vector2((float) bounds.X + (float) ((double) this.samplesHistory[index1][index2] * ((double) bounds.Width / 4.0) + (double) bounds.Width * 0.75) + (float) (index1 * 2) - (float) this.samplesHistory.Count, vector2.Y), new Rectangle?(), color * 0.6f * (0.01f + num3) * 0.4f);
          vector2.Y += num1;
        }
      }
    }
  }
}
