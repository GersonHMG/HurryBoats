// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace BoatPackets
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct Vec2 : IFlatbufferObject
{
  private Struct __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public void __init(int _i, ByteBuffer _bb) { __p = new Struct(_i, _bb); }
  public Vec2 __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public float X { get { return __p.bb.GetFloat(__p.bb_pos + 0); } }
  public float Y { get { return __p.bb.GetFloat(__p.bb_pos + 4); } }

  public static Offset<BoatPackets.Vec2> CreateVec2(FlatBufferBuilder builder, float X, float Y) {
    builder.Prep(4, 8);
    builder.PutFloat(Y);
    builder.PutFloat(X);
    return new Offset<BoatPackets.Vec2>(builder.Offset);
  }
};

public struct Movement : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static Movement GetRootAsMovement(ByteBuffer _bb) { return GetRootAsMovement(_bb, new Movement()); }
  public static Movement GetRootAsMovement(ByteBuffer _bb, Movement obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public Movement __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public BoatPackets.Vec2? Position { get { int o = __p.__offset(4); return o != 0 ? (BoatPackets.Vec2?)(new BoatPackets.Vec2()).__assign(o + __p.bb_pos, __p.bb) : null; } }
  public float Rotation { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetFloat(o + __p.bb_pos) : (float)0.0f; } }
  public BoatPackets.Vec2? Velocity { get { int o = __p.__offset(8); return o != 0 ? (BoatPackets.Vec2?)(new BoatPackets.Vec2()).__assign(o + __p.bb_pos, __p.bb) : null; } }
  public float AngularVelocity { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetFloat(o + __p.bb_pos) : (float)0.0f; } }

  public static void StartMovement(FlatBufferBuilder builder) { builder.StartTable(4); }
  public static void AddPosition(FlatBufferBuilder builder, Offset<BoatPackets.Vec2> positionOffset) { builder.AddStruct(0, positionOffset.Value, 0); }
  public static void AddRotation(FlatBufferBuilder builder, float rotation) { builder.AddFloat(1, rotation, 0.0f); }
  public static void AddVelocity(FlatBufferBuilder builder, Offset<BoatPackets.Vec2> velocityOffset) { builder.AddStruct(2, velocityOffset.Value, 0); }
  public static void AddAngularVelocity(FlatBufferBuilder builder, float angularVelocity) { builder.AddFloat(3, angularVelocity, 0.0f); }
  public static Offset<BoatPackets.Movement> EndMovement(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<BoatPackets.Movement>(o);
  }
};


}
