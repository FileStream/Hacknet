﻿// Decompiled with JetBrains decompiler
// Type: Hacknet.MSRandom
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet
{
  public class MSRandom
  {
    private int[] SeedArray = new int[56];
    private const int MBIG = 2147483647;
    private const int MSEED = 161803398;
    private const int MZ = 0;
    private int inext;
    private int inextp;

    public MSRandom()
      : this(Environment.TickCount)
    {
    }

    public MSRandom(int Seed)
    {
      int num1 = 161803398 - (Seed == int.MinValue ? int.MaxValue : Math.Abs(Seed));
      this.SeedArray[55] = num1;
      int num2 = 1;
      for (int index1 = 1; index1 < 55; ++index1)
      {
        int index2 = 21 * index1 % 55;
        this.SeedArray[index2] = num2;
        num2 = num1 - num2;
        if (num2 < 0)
          num2 += int.MaxValue;
        num1 = this.SeedArray[index2];
      }
      for (int index1 = 1; index1 < 5; ++index1)
      {
        for (int index2 = 1; index2 < 56; ++index2)
        {
          this.SeedArray[index2] -= this.SeedArray[1 + (index2 + 30) % 55];
          if (this.SeedArray[index2] < 0)
            this.SeedArray[index2] += int.MaxValue;
        }
      }
      this.inext = 0;
      this.inextp = 21;
      Seed = 1;
    }

    protected virtual double Sample()
    {
      return (double) this.InternalSample() * 4.6566128752458E-10;
    }

    private int InternalSample()
    {
      int inext = this.inext;
      int inextp = this.inextp;
      int index1;
      if ((index1 = inext + 1) >= 56)
        index1 = 1;
      int index2;
      if ((index2 = inextp + 1) >= 56)
        index2 = 1;
      int num = this.SeedArray[index1] - this.SeedArray[index2];
      if (num == int.MaxValue)
        --num;
      if (num < 0)
        num += int.MaxValue;
      this.SeedArray[index1] = num;
      this.inext = index1;
      this.inextp = index2;
      return num;
    }

    public virtual int Next()
    {
      return this.InternalSample();
    }

    private double GetSampleForLargeRange()
    {
      int num = this.InternalSample();
      if (this.InternalSample() % 2 == 0)
        num = -num;
      return ((double) num + 2147483646.0) / 4294967293.0;
    }

    public virtual int Next(int minValue, int maxValue)
    {
      if (minValue > maxValue)
        throw new ArgumentOutOfRangeException("minValue", "minValue out of range: " + (object) minValue);
      long num = (long) maxValue - (long) minValue;
      if (num <= (long) int.MaxValue)
        return (int) (this.Sample() * (double) num) + minValue;
      return (int) ((long) (this.GetSampleForLargeRange() * (double) num) + (long) minValue);
    }

    public virtual int Next(int maxValue)
    {
      if (maxValue < 0)
        throw new ArgumentOutOfRangeException("maxValue", "maxValue out of range: " + (object) maxValue);
      return (int) (this.Sample() * (double) maxValue);
    }

    public virtual double NextDouble()
    {
      return this.Sample();
    }

    public virtual void NextBytes(byte[] buffer)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      for (int index = 0; index < buffer.Length; ++index)
        buffer[index] = (byte) (this.InternalSample() % 256);
    }
  }
}
