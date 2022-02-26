// Manager lobby packets for Lobby Manager

using System.Collections.Generic; 
using FlatBuffers;
using Steamworks;
using Godot;
using LobbyPackets;


public class LobbyPM : PacketManager{
    
    Lobby lobby;
    
    enum METHODS_ID : byte{
        INIT_CONNECTION,
        VALID_CONNECTION,
        RECEIVE_PLAYERS,
        PLAYERS_REQUESTER,
        START_GAME,
        REGISTER_PLAYER,
        PLAYER_LOBBY_READY
    }


    public override void ReceivePacket(byte[] incoming_packet){
        ByteBuffer buffer = new ByteBuffer(GetPacketBody(incoming_packet));
        METHODS_ID method_id = (METHODS_ID) GetMethodID(incoming_packet);
        switch(method_id){
            case METHODS_ID.RECEIVE_PLAYERS:
                ReceivePlayers(buffer);
                break;
            case METHODS_ID.VALID_CONNECTION:
                ReceiveTellIsConnectionValid();
                break;
            case METHODS_ID.PLAYERS_REQUESTER:
                ReceiveRequestPlayers(buffer);
                break;
            case METHODS_ID.START_GAME:
                OnStartGame();
                break;
            case METHODS_ID.REGISTER_PLAYER:
                ReceiveRegisterPlayer(buffer);
                break;
            case METHODS_ID.PLAYER_LOBBY_READY:
                ReceivePlayerReady(buffer);
                break;
        }
    }


    // Trigger OnP2PSessionRequest in lobbymanager (Host) when is succesfully received
    public void SendStartP2PConnection(){
        byte[] packet = AssemblyPacket( (byte) METHODS_ID.INIT_CONNECTION, new byte[0]);
        network.SendReliableToID(packet, lobby_manager.GetHostSteamID());
    }


    public void SendTellIsConnectionValid(CSteamID player_id){
        byte[] packet = AssemblyPacket((byte) METHODS_ID.VALID_CONNECTION, new byte[0] );
        network.SendReliableToID( packet, player_id );
    }


    void ReceiveTellIsConnectionValid(){
        lobby_manager.OnConnectionAccepted();
    }


    public void SendPlayers(List<PlayerLobbyData> players, CSteamID to_id){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        int data_size = players.Count;
        Offset<PlayerData>[] list = new Offset<PlayerData>[data_size];
        int i = 0; 
        foreach(PlayerLobbyData player in players){
            PlayerData.StartPlayerData(builder);
            PlayerData.AddSteamId(builder,(ulong) player.GetSteamID() );
            PlayerData.AddPlayerId(builder, player.GetPlayerID() );
            PlayerData.AddStatus(builder, (byte) player.GetPlayerConnectionStatus() );
            list[i] = PlayerData.EndPlayerData(builder);
            i += 1; 
        }
        VectorOffset dict_data = PlayersData.CreateDataVector(builder, list);
        PlayersData.StartPlayersData(builder);
        PlayersData.AddData(builder, dict_data);
        var stop_building = PlayersData.EndPlayersData(builder);
        builder.Finish(stop_building.Value);
        byte[] raw_packet = builder.SizedByteArray();
        byte[] packet = AssemblyPacket((byte) METHODS_ID.RECEIVE_PLAYERS, raw_packet);
        network.SendReliableToID(packet, to_id);
    }


    void ReceivePlayers(ByteBuffer buffer){
        var table = PlayersData.GetRootAsPlayersData(buffer);
        List<PlayerLobbyData> new_players = new List<PlayerLobbyData>();
        for(int i = 0; i < table.DataLength; i++){
            PlayerData? entry = table.Data(i);
            PlayerLobbyData new_player = new PlayerLobbyData((CSteamID) entry.Value.SteamId, entry.Value.PlayerId);
            new_player.SetPlayerConnectionStatus( (PlayerLobbyData.CONNECTION_STATE) entry.Value.Status);
            new_players.Add(new_player);
        }
        lobby_manager.SetPlayersData(new_players);
    }


    public void SendRequestPlayers(CSteamID my_steam_id){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        PlayerSteamID.StartPlayerSteamID(builder);
        PlayerSteamID.AddSteamId(builder, (ulong) my_steam_id);
        var stop_building = PlayerSteamID.EndPlayerSteamID(builder);
        builder.Finish(stop_building.Value);
        byte[] raw_packet = builder.SizedByteArray();
        byte[] packet = AssemblyPacket((byte) METHODS_ID.PLAYERS_REQUESTER, raw_packet);
        network.SendReliableToID(packet, lobby_manager.GetHostSteamID() );
    }


    void ReceiveRequestPlayers(ByteBuffer buffer){
        var table = PlayerSteamID.GetRootAsPlayerSteamID(buffer);
        CSteamID to_player_id = new CSteamID(table.SteamId);
        SendPlayers(lobby_manager.GetPlayersData(), to_player_id);
    }


    public void SendRegisterPlayer(CSteamID steam_id, ushort player_id){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        PlayerData.StartPlayerData(builder);
        PlayerData.AddSteamId(builder,(ulong) steam_id);
        PlayerData.AddPlayerId(builder, player_id);
        var stop_building = PlayersData.EndPlayersData(builder);
        builder.Finish(stop_building.Value);
        byte[] raw_packet = builder.SizedByteArray();
        byte[] packet = AssemblyPacket((byte) METHODS_ID.REGISTER_PLAYER, raw_packet);
        network.SendReliableToAll(packet);
    }


    void ReceiveRegisterPlayer(ByteBuffer buffer){
        var table = PlayerData.GetRootAsPlayerData(buffer);
        lobby_manager.RegisterPlayer((CSteamID) table.SteamId, table.PlayerId);
    }


    public void SendPlayerReady(CSteamID my_steam_id){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        PlayerSteamID.StartPlayerSteamID(builder);
        PlayerSteamID.AddSteamId(builder, (ulong) my_steam_id);
        var stop_building = PlayerSteamID.EndPlayerSteamID(builder);
        builder.Finish(stop_building.Value);
        byte[] raw_packet = builder.SizedByteArray();
        byte[] packet = AssemblyPacket((byte) METHODS_ID.PLAYER_LOBBY_READY, raw_packet);
        network.SendReliableToAll(packet);
    }


    void ReceivePlayerReady(ByteBuffer buffer){
        var table = PlayerData.GetRootAsPlayerData(buffer);
        lobby_manager.SetPlayerConnectionReady((CSteamID) table.SteamId);
    }



    public void StartGameSync(){
        byte[] packet = AssemblyPacket((byte) METHODS_ID.START_GAME, new byte[0] );
        network.SendReliableToAll(packet);
    }


    // CHANGE THISS!!
    void OnStartGame(){
        if( !IsInstanceValid(lobby)){
            lobby = GetTree().Root.GetNode<Lobby>("Lobby");
        }
        lobby.StartGame();
    }



    public void SetLobby(Lobby _lobby){
        lobby = _lobby;
    }

}