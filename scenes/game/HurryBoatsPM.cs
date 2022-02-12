using Godot;
using System;
using FlatBuffers;
using HurryBoatsPackets;

public class HurryBoatsPM : PacketManager{
 
    HurryBoats main_game;

    enum METHODS_ID : byte{
        PREPARE_GAME
    }

    public override void _Ready(){
        base._Ready();
        main_game = GetParent<HurryBoats>(); 
    }



    public override void ReceivePacket(byte[] raw_packet){
        ByteBuffer buffer = new ByteBuffer(GetPacketBody(raw_packet));
        METHODS_ID method_id = (METHODS_ID) GetMethodID(raw_packet);
        switch(method_id){
            case METHODS_ID.PREPARE_GAME:
                ReceivePrepareGame(buffer);
                break;
        }
    }

    
    public void SendPrepareGame(int seed){
        // Send prepare game
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        PrepareGame.StartPrepareGame(builder);
        PrepareGame.AddSeed(builder, seed);
        var stop_building = PrepareGame.EndPrepareGame(builder);
        builder.Finish(stop_building.Value);
        byte[] raw_packet = builder.SizedByteArray();
        byte[] packet = AssemblyPacket((byte) METHODS_ID.PREPARE_GAME, raw_packet);
        network.SendReliableToAll(packet);
    }


    void ReceivePrepareGame(ByteBuffer buffer){
        var table = PrepareGame.GetRootAsPrepareGame(buffer);
        GD.Print(" PREPARING GAME");
        main_game.PrepareGame(table.Seed);
    }



}
