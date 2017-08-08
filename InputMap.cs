// Decompiled with JetBrains decompiler
// Type: Hacknet.InputMap
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public struct InputMap
  {
    private static InputMap e = new InputMap();
    public InputStates now;
    public InputStates last;

    public static InputMap Empty
    {
      get
      {
        InputMap e = InputMap.e;
        return InputMap.e;
      }
    }

    public InputMap(InputStates last, InputStates now)
    {
      this.last = last;
      this.now = now;
    }

    public static bool operator ==(InputMap self, InputMap other)
    {
      return self.now == other.now && self.last == other.last;
    }

    public static bool operator !=(InputMap self, InputMap other)
    {
      return !(self.now == other.now) || !(self.last == other.last);
    }

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
