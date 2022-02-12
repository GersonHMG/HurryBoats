using Godot;
using System;
using FlatBuffers;
using GameResetPackets;


public class GameResetPM : PacketManager{
    
    enum METHODS_ID: byte{
        RESET_ROUND,
        PLAYER_READY,
        START_ROUND
    }

    GameReset parent;

    public override void _Ready(){
        base._Ready();
        parent = GetParent<GameReset>();
    }

    public override void ReceivePacket(byte[] packet){
        ByteBuffer buffer = new ByteBuffer(GetPacketBody(packet));
        METHODS_ID method_id = (METHODS_ID) GetMethodID(packet);
        switch(method_id){
            case METHODS_ID.RESET_ROUND:
                ReceiveResetRound(buffer);
                break;
            case METHODS_ID.PLAYER_READY:
                ReceivePlayerReady(buffer);
                break;
            case METHODS_ID.START_ROUND:
                ReceiveStartRound();
                break;
        }
    }


    public void SendResetRound(int seed){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        MapSeed.StartMapSeed(builder);
        MapSeed.AddSeed(builder, seed);
        var stop_builder = MapSeed.EndMapSeed(builder);
        builder.Finish(stop_builder.Value);
        byte[] packet = AssemblyPacket((byte) METHODS_ID.RESET_ROUND, builder.SizedByteArray() );
        network.SendReliableToAll(packet);
    }


    void ReceiveResetRound(ByteBuffer buffer){
        var table = MapSeed.GetRootAsMapSeed(buffer);
        parent.ResetRound(table.Seed);
    }


    public void SendPlayerReady(ushort player_id){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        PlayerGameID.StartPlayerGameID(builder);
        PlayerGameID.AddId(builder, player_id);
        var stop_builder = PlayerGameID.EndPlayerGameID(builder);
        builder.Finish(stop_builder.Value);
        byte[] packet = AssemblyPacket((byte) METHODS_ID.PLAYER_READY, builder.SizedByteArray() );
        network.SendReliableToID(packet, lobby_manager.GetHostSteamID());

    }


    void ReceivePlayerReady(ByteBuffer buffer){
        var table = PlayerGameID.GetRootAsPlayerGameID(buffer);
        parent.OnPlayerRoundReady(table.Id);
    }


    public void SendStartRound(){
        byte[] packet = AssemblyPacket((byte) METHODS_ID.START_ROUND,  new byte[0]);
        network.SendReliableToAll(packet);
    }


    void ReceiveStartRound(){
        parent.StartRound();
    }


}
