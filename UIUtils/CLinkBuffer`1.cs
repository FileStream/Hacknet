// Decompiled with JetBrains decompiler
// Type: Hacknet.UIUtils.CLinkBuffer`1
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet.UIUtils
{
  public class CLinkBuffer<T>
  {
    private int currentIndex = 0;
    private T[] data;

    public CLinkBuffer(int BufferSize = 128)
    {
      this.data = new T[BufferSize];
    }

    public T Get(int offset)
    {
      int index = this.currentIndex + offset;
      while (index < 0)
        index += this.data.Length;
      while (index >= this.data.Length)
        index -= this.data.Length;
      return this.data[index];
    }

    public void Add(T added)
    {
      this.currentIndex = this.NextIndex();
      this.data[this.currentIndex] = added;
    }

    public void AddOneAhead(T added)
    {
      this.data[this.NextIndex()] = added;
    }

    private int NextIndex()
    {
      int num = this.currentIndex + 1;
      if (num >= this.data.Length)
        num = 0;
      return num;
    }
  }
}
