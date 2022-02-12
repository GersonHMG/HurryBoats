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
        REGISTER_PLAYER
    }

        // ------------ Receivers (Deserialize)
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

        }
    }


    // Trigger OnP2PSessionRequest (Host) when is succesfully received
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


    // NEW FEATURE
    public void SendPlayers(Dictionary<CSteamID,ushort> players, CSteamID to_id){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        int dict_size = players.Keys.Count;
        Offset<PlayersData>[] list = new Offset<PlayersData>[dict_size];
        int i = 0; 
        foreach(CSteamID id in players.Keys){
            ulong key_offset = ((ulong)id);
            ushort value_offset = players[id];
            PlayersData.StartPlayersData(builder);
            PlayersData.AddSteamId(builder, key_offset);
            PlayersData.AddPlayerId(builder, value_offset);
            list[i] = PlayersData.EndPlayersData(builder);
            i += 1; 
        }
        VectorOffset dict_data = PlayersData.CreateSortedVectorOfPlayersData(builder, list);
        PlayersDataDictionary.StartPlayersDataDictionary(builder);
        PlayersDataDictionary.AddItems(builder, dict_data);
        var stop_building = PlayersDataDictionary.EndPlayersDataDictionary(builder);
        builder.Finish(stop_building.Value);
        byte[] raw_packet = builder.SizedByteArray();
        byte[] packet = AssemblyPacket((byte) METHODS_ID.RECEIVE_PLAYERS, raw_packet);
        network.SendReliableToID(packet, to_id);
    }


    void ReceivePlayers(ByteBuffer buffer){
        Dictionary<CSteamID,ushort> r_dict = new Dictionary<CSteamID, ushort>();
        var table = PlayersDataDictionary.GetRootAsPlayersDataDictionary(buffer);
        for(int i = 0; i < table.ItemsLength; i++){
            PlayersData? entry = table.Items(i);
            r_dict.Add((CSteamID) entry.Value.SteamId, entry.Value.PlayerId);
        }
        lobby_manager.SetPlayersData(r_dict);
    }


    public void SendRequestPlayers(CSteamID my_steam_id){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        LobbyPacket.data_requester.Startdata_requester(builder);
        LobbyPacket.data_requester.AddPlayerId(builder, (ulong) my_steam_id);
        var stop_building = LobbyPacket.data_requester.Enddata_requester(builder);
        builder.Finish(stop_building.Value);
        byte[] raw_packet = builder.SizedByteArray();
        byte[] packet = AssemblyPacket((byte) METHODS_ID.PLAYERS_REQUESTER, raw_packet);
        network.SendReliableToID(packet, lobby_manager.GetHostSteamID() );
    }


    void ReceiveRequestPlayers(ByteBuffer buffer){
        var table = LobbyPacket.data_requester.GetRootAsdata_requester(buffer);
        Steamworks.CSteamID to_player_id = new CSteamID(table.PlayerId);
        Dictionary<CSteamID, ushort> playerid_by_steamid = new Dictionary<CSteamID, ushort>();
        foreach(PlayerLobbyData player in lobby_manager.GetPlayersData()){
            playerid_by_steamid.Add(player.GetSteamID(), player.GetPlayerID() );
        }
        SendPlayers(playerid_by_steamid, to_player_id);
    }


    public void SendRegisterPlayer(CSteamID steam_id, ushort player_id){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        PlayersData.StartPlayersData(builder);
        PlayersData.AddSteamId(builder,(ulong) steam_id);
        PlayersData.AddPlayerId(builder, player_id);
        var stop_building = PlayersData.EndPlayersData(builder);
        builder.Finish(stop_building.Value);
        byte[] raw_packet = builder.SizedByteArray();
        byte[] packet = AssemblyPacket((byte) METHODS_ID.REGISTER_PLAYER, raw_packet);
        network.SendReliableToAll(packet);
    }


    void ReceiveRegisterPlayer(ByteBuffer buffer){
        var table = PlayersData.GetRootAsPlayersData(buffer);
        lobby_manager.RegisterPlayer((CSteamID) table.SteamId, table.PlayerId);
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