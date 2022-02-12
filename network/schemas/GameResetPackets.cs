// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace GameResetPackets
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct PlayerGameID : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static PlayerGameID GetRootAsPlayerGameID(ByteBuffer _bb) { return GetRootAsPlayerGameID(_bb, new PlayerGameID()); }
  public static PlayerGameID GetRootAsPlayerGameID(ByteBuffer _bb, PlayerGameID obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public PlayerGameID __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public ushort Id { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUshort(o + __p.bb_pos) : (ushort)0; } }

  public static Offset<GameResetPackets.PlayerGameID> CreatePlayerGameID(FlatBufferBuilder builder,
      ushort id = 0) {
    builder.StartTable(1);
    PlayerGameID.AddId(builder, id);
    return PlayerGameID.EndPlayerGameID(builder);
  }

  public static void StartPlayerGameID(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddId(FlatBufferBuilder builder, ushort id) { builder.AddUshort(0, id, 0); }
  public static Offset<GameResetPackets.PlayerGameID> EndPlayerGameID(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<GameResetPackets.PlayerGameID>(o);
  }
};

public struct MapSeed : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static MapSeed GetRootAsMapSeed(ByteBuffer _bb) { return GetRootAsMapSeed(_bb, new MapSeed()); }
  public static MapSeed GetRootAsMapSeed(ByteBuffer _bb, MapSeed obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public MapSeed __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int Seed { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }

  public static Offset<GameResetPackets.MapSeed> CreateMapSeed(FlatBufferBuilder builder,
      int seed = 0) {
    builder.StartTable(1);
    MapSeed.AddSeed(builder, seed);
    return MapSeed.EndMapSeed(builder);
  }

  public static void StartMapSeed(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddSeed(FlatBufferBuilder builder, int seed) { builder.AddInt(0, seed, 0); }
  public static Offset<GameResetPackets.MapSeed> EndMapSeed(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<GameResetPackets.MapSeed>(o);
  }
};


}