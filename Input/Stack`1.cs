// Decompiled with JetBrains decompiler
// Type: Hacknet.Input.Stack`1
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;

namespace Hacknet.Input
{
  public class Stack<T>
  {
    private T[] stack;

    public int Capacity
    {
      get
      {
        return this.stack.Length;
      }
    }

    public int Count { get; private set; }

    public Stack()
      : this(32)
    {
    }

    public Stack(int capacity)
    {
      if (capacity < 0)
        capacity = 0;
      this.stack = new T[capacity];
    }

    public void Push(ref T item)
    {
      if (this.Count == this.stack.Length)
      {
        T[] objArray = new T[this.stack.Length << 1];
        Array.Copy((Array) this.stack, 0, (Array) objArray, 0, this.stack.Length);
        this.stack = objArray;
      }
      this.stack[this.Count] = item;
      ++this.Count;
    }

    public void Pop(out T item)
    {
      if (this.Count <= 0)
        throw new InvalidOperationException();
      item = this.stack[this.Count];
      this.stack[this.Count] = default (T);
      --this.Count;
    }

    public void PopSegment(out ArraySegment<T> segment)
    {
      segment = new ArraySegment<T>(this.stack, 0, this.Count);
      this.Count = 0;
    }
  }
}
