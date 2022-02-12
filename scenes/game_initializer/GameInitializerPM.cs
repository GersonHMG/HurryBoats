using FlatBuffers;
using Godot;
using GameInitializerPackets;

public class GameInitializerPM : PacketManager{

    //initializer;
    GameInitializer game_initializer;

    enum METHODS_ID : ushort{
        PLAYER_GAME_LOADED,
        PLAYER_GAME_READY,
        START_GAME
    }


    public override void _Ready(){
        base._Ready();
        game_initializer = GetParent<GameInitializer>();
    }


    public override void ReceivePacket(byte[] raw_packet){
        ByteBuffer buffer = new ByteBuffer(GetPacketBody(raw_packet));
        METHODS_ID method_id = (METHODS_ID) GetMethodID(raw_packet);
        switch(method_id){
            case METHODS_ID.PLAYER_GAME_LOADED:
                ReceivePlayerGameLoaded(buffer);
                break;
            case METHODS_ID.PLAYER_GAME_READY:
                ReceivePlayerGameIsReady(buffer);
                break;
            case METHODS_ID.START_GAME:
                ReceiveStartGame();
                break;
        }
    }


    public void SendPlayerGameLoaded(ulong player_id){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        PlayerID.StartPlayerID(builder);
        PlayerID.AddId(builder, player_id);
        var stop_building = PlayerID.EndPlayerID(builder);
        builder.Finish(stop_building.Value);
        byte[] raw_packet = builder.SizedByteArray();
        byte[] packet = AssemblyPacket((byte) METHODS_ID.PLAYER_GAME_LOADED, raw_packet);
        network.SendReliableToID( packet, lobby_manager.GetHostSteamID());
    }


    void ReceivePlayerGameLoaded(ByteBuffer buffer){
        var table = PlayerID.GetRootAsPlayerID(buffer);
        game_initializer.OnPlayerGameLoaded(table.Id);
    }


    public void SendPlayerGameIsReady(ulong player_id){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        PlayerID.StartPlayerID(builder);
        PlayerID.AddId(builder, player_id);
        var stop_building = PlayerID.EndPlayerID(builder);
        builder.Finish(stop_building.Value);
        byte[] raw_packet = builder.SizedByteArray();
        byte[] packet = AssemblyPacket((byte) METHODS_ID.PLAYER_GAME_READY, raw_packet);
        network.SendReliableToID( packet, lobby_manager.GetHostSteamID());
    }


    void ReceivePlayerGameIsReady(ByteBuffer buffer){
        var table = PlayerID.GetRootAsPlayerID(buffer);
        game_initializer.OnPlayerGameIsReady(table.Id);
    }


    public void SendStartGame(){
        byte[] packet = AssemblyPacket((byte) METHODS_ID.START_GAME, new byte[1]);
        network.SendReliableToAll(packet);
    }


    void ReceiveStartGame(){
        game_initializer.StartGame();
    }


}