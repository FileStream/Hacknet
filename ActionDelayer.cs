// Decompiled with JetBrains decompiler
// Type: Hacknet.ActionDelayer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

using System;
using System.Collections.Generic;

namespace Hacknet
{
  internal class ActionDelayer
  {
    private List<ActionDelayer.Pair> pairs = new List<ActionDelayer.Pair>();
    private List<ActionDelayer.Pair> nextPairs = new List<ActionDelayer.Pair>();

    public DateTime Time { get; private set; }

    public void Pump()
    {
      this.Time = DateTime.Now;
      this.pairs.AddRange((IEnumerable<ActionDelayer.Pair>) this.nextPairs);
      this.nextPairs.Clear();
      for (int index = 0; index < this.pairs.Count; ++index)
      {
        ActionDelayer.Pair pair = this.pairs[index];
        if (pair.Condition(this))
        {
          pair.Action();
          this.pairs.RemoveAt(index--);
        }
      }
    }

    public void RunAllDelayedActions()
    {
      this.pairs.AddRange((IEnumerable<ActionDelayer.Pair>) this.nextPairs);
      this.nextPairs.Clear();
      this.Time = DateTime.MaxValue;
      for (int index = 0; index < this.pairs.Count; ++index)
      {
        ActionDelayer.Pair pair = this.pairs[index];
        if (pair.Condition(this))
        {
          pair.Action();
          this.pairs.RemoveAt(index--);
        }
      }
    }

    public void Post(ActionDelayer.Condition condition, Action action)
    {
      this.nextPairs.Add(new ActionDelayer.Pair()
      {
        Condition = condition,
        Action = action
      });
    }

    public void PostAnimation(IEnumerator<ActionDelayer.Condition> animation)
    {
      Action tick = (Action) null;
      tick = (Action) (() =>
      {
        if (!animation.MoveNext())
          return;
        this.Post(animation.Current, tick);
      });
      tick();
    }

    public static ActionDelayer.Condition WaitUntil(DateTime time)
    {
      return (ActionDelayer.Condition) (x => x.Time >= time);
    }

    public static ActionDelayer.Condition Wait(double time)
    {
      return ActionDelayer.WaitUntil(DateTime.Now + TimeSpan.FromSeconds(time));
    }

    public static ActionDelayer.Condition NextTick()
    {
      return (ActionDelayer.Condition) (x => true);
    }

    public static ActionDelayer.Condition FileDeleted(Folder f, string filename)
    {
      return (ActionDelayer.Condition) (x => !f.containsFile(filename));
    }

    public delegate bool Condition(ActionDelayer messagePump);

    private struct Pair
    {
      public ActionDelayer.Condition Condition;
      public Action Action;
    }
  }
}
