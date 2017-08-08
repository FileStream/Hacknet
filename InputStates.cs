// Decompiled with JetBrains decompiler
// Type: Hacknet.InputStates
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 44D58447-4185-43DF-BEF1-8BBDED416CAA
// Assembly location: E:\SteamLibrary\steamapps\common\Hacknet\Hacknet.exe

namespace Hacknet
{
  public struct InputStates
  {
    public float movement;
    public bool jumping;
    public bool useItem;
    public float wmovement;
    public bool wjumping;
    public bool wuseItem;

    public static bool operator ==(InputStates self, InputStates other)
    {
      return (double) self.movement == (double) other.movement && self.jumping == other.jumping && (self.useItem == other.useItem && (double) self.wmovement == (double) other.wmovement) && self.wjumping == other.wjumping && self.wuseItem == other.wuseItem;
    }

    public static bool operator !=(InputStates self, InputStates other)
    {
      return (double) self.movement != (double) other.movement || self.jumping != other.jumping || (self.useItem != other.useItem || (double) self.wmovement != (double) other.wmovement) || self.wjumping != other.wjumping || self.wuseItem != other.wuseItem;
    }

    public override bool Equals(object obj)
    {
      return obj is InputStates && (InputStates) obj == this;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
