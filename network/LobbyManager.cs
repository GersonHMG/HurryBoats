// Manage lobby logic with Steam API, Abstract layer from steam lobby to normal lobby


using Godot;
using System;
using System.Collections.Generic;
using Steamworks;
using FlatBuffers;

public class LobbyManager : Node{
    LobbyPM lobbyPM;
    // Lobby settings
    int MAX_PLAYERS = 8;
    bool is_host = false;
    // Lobby info

    CSteamID lobby_id;
    PlayerLobbyData host_data;
    PlayerLobbyData my_data;
    Dictionary<CSteamID, PlayerLobbyData> playerdata_by_steamid = new Dictionary<CSteamID, PlayerLobbyData>();

    // CallBacks
    Callback<LobbyCreated_t> Callback_lobbyCreated;
    Callback<LobbyEnter_t> Callback_lobbyEntered;
    Callback<LobbyChatUpdate_t> Callback_chatUpdate;
    Callback<P2PSessionRequest_t> Callback_P2PSessionRequest;
    Callback<P2PSessionConnectFail_t> Callback_P2PSessionConnectFailed;

    // Signals
    [Signal]
    public delegate void LobbyJoinedResult(bool result);
    [Signal]
    public delegate void LobbyCreatedResult(bool result);
    [Signal]
    public delegate void PlayersUpdate();
    [Signal]
    public delegate void ConnectionEstablished();
    [Signal]
    public delegate void ConnectionFailed();


    public override void _Ready(){
        Callback_P2PSessionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
        Callback_P2PSessionConnectFailed = Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectionFailed);
        Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        Callback_lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        Callback_chatUpdate = Callback<LobbyChatUpdate_t>.Create(OnChatUpdate);
        lobbyPM = GetNode<LobbyPM>("LobbyPM");
    }


    public override void _Process(float delta){
        SteamAPI.RunCallbacks();
    }


    //--------------------------------------------- Callbacks

    void OnLobbyCreated(LobbyCreated_t lobby){
        if (lobby.m_eResult == EResult.k_EResultOK){
            lobby_id = (CSteamID) lobby.m_ulSteamIDLobby;
            host_data = RegisterPlayer( SteamUser.GetSteamID(), 0 );
            host_data.SetPlayerConnectionStatus( PlayerLobbyData.CONNECTION_STATE.READY);
            my_data = host_data;
            is_host = true;
            GD.Print("Lobby created succesfully.");
            EmitSignal(nameof(LobbyCreatedResult), true);
        }
        else{
            GD.Print("Lobby failed to create. ERROR CODE: " + lobby.m_eResult.ToString());
            EmitSignal(nameof(LobbyCreatedResult), false);
        }
    }

  
    void OnLobbyEntered(LobbyEnter_t entrance){
        CSteamID temp_host_id = SteamMatchmaking.GetLobbyOwner( (CSteamID) entrance.m_ulSteamIDLobby );
        if(temp_host_id != new CSteamID()){
            GD.Print("Lobby Entered.");
            lobby_id = (CSteamID) entrance.m_ulSteamIDLobby;
            if(!ImHost()){
                host_data = RegisterPlayer(temp_host_id, 0);
                lobbyPM.SendStartP2PConnection();
            }
            PrintLobbyInfo();
            EmitSignal(nameof(LobbyJoinedResult), true);
        }
        else{
            GD.Print("Can't join to lobby");
            EmitSignal(nameof(LobbyJoinedResult), false);
        }
    }


    public void OnP2PSessionRequest(P2PSessionRequest_t request){
        SteamNetworking.AcceptP2PSessionWithUser(request.m_steamIDRemote);
        if(ImHost()){
            lobbyPM.SendTellIsConnectionValid(request.m_steamIDRemote);
            PlayerLobbyData player = RegisterPlayer(request.m_steamIDRemote, (ushort) playerdata_by_steamid.Count);
            lobbyPM.SendRegisterPlayer(player.GetSteamID(), player.GetPlayerID() );
        }
        GD.Print("You have accepted incoming connection from " + SteamFriends.GetFriendPersonaName(request.m_steamIDRemote));
    }
        

    public void OnP2PSessionConnectionFailed(P2PSessionConnectFail_t failure){
        GD.Print("P2P session failed. Error code: " + failure.m_eP2PSessionError);
        EmitSignal(nameof(ConnectionFailed));
        LeaveLobby();
    }


    void OnChatUpdate(LobbyChatUpdate_t update){
        CSteamID member_id = (CSteamID) update.m_ulSteamIDUserChanged;
        // Member joined
        if(update.m_rgfChatMemberStateChange == (int) EChatMemberStateChange.k_EChatMemberStateChangeEntered ){
            GD.Print( SteamFriends.GetFriendPersonaName(member_id) + " has entered to lobby."  );
        }
        // Member leave
        else if(update.m_rgfChatMemberStateChange == (int) EChatMemberStateChange.k_EChatMemberStateChangeLeft){
            GD.Print( SteamFriends.GetFriendPersonaName(member_id) + " has left lobby."  );
            UnregisterPlayer(member_id);
            EmitSignal( nameof(PlayersUpdate) );
        }
    }
    
    // -----------------------------------------------------


    public void CreateLobby(){
        SteamAPICall_t new_lobby = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, MAX_PLAYERS);
        GD.Print("Attempt to create lobby: " + new_lobby.ToString());
    }


    public void LeaveLobby(){
        GD.Print("Leaving Lobby: " + lobby_id.ToString());
        SteamMatchmaking.LeaveLobby(lobby_id);
        ResetLobbyParameters();
    }

    
    public void JoinToLobby(CSteamID join_id){
        SteamMatchmaking.JoinLobby(join_id);
    }

    
    public void SetPlayerConnectionReady(CSteamID steam_id){
        playerdata_by_steamid[steam_id].SetPlayerConnectionStatus(PlayerLobbyData.CONNECTION_STATE.READY);
        EmitSignal( nameof(PlayersUpdate) );
    }


    void ResetLobbyParameters(){
        is_host = false;
        my_data = null;
        host_data = null;
        lobby_id = new CSteamID();
        playerdata_by_steamid.Clear();
    }

    
    public PlayerLobbyData RegisterPlayer(CSteamID steam_id, ushort player_id){
        PlayerLobbyData new_player = new PlayerLobbyData(steam_id,player_id);
        playerdata_by_steamid[steam_id] = new_player;
        GD.Print("register player: " + new_player.GetSteamID().ToString() + " with id: " + new_player.GetPlayerID() + " and name: " + new_player.GetPlayerName());
        EmitSignal( nameof(PlayersUpdate) );
        return new_player;
    }


    void UnregisterPlayer(CSteamID steam_id){
        playerdata_by_steamid.Remove(steam_id);
    }


    public List<PlayerLobbyData> GetPlayersData(){
        List<PlayerLobbyData> data = new List<PlayerLobbyData>(playerdata_by_steamid.Values);
         return data;
    }

    
    public List<CSteamID> GetPlayersSteamID(){
        List<CSteamID> player_list = new List<CSteamID>(playerdata_by_steamid.Keys);
        return player_list;
    }
    

    public CSteamID GetHostSteamID() => host_data.GetSteamID();


    public CSteamID GetMySteamID() => my_data.GetSteamID();


    public PlayerLobbyData GetMyData() => my_data;


    public bool ImHost() => is_host; 


    public CSteamID GetLobbyID() => lobby_id;
    

    public LobbyPM GetLobbyPM() => lobbyPM;


    public bool IsLobbyActive(){
        if(my_data != null){
            return true;
        }
        return false;
    }


    public void SetPlayersData(List<PlayerLobbyData> new_players){
        playerdata_by_steamid.Clear();
        foreach(PlayerLobbyData player in new_players){
            playerdata_by_steamid.Add(player.GetSteamID(), player);
        }
        my_data = playerdata_by_steamid[ SteamUser.GetSteamID() ];
        EmitSignal( nameof(PlayersUpdate) );
    }


    public void OnConnectionAccepted(){
        GD.Print("Connection established succesful");
        
        EmitSignal(nameof(ConnectionEstablished));
    }


    public void PrintLobbyInfo(){
        GD.Print("Lobby ID: " + lobby_id.ToString());
        GD.Print("Host name: "  + host_data.GetPlayerName());
        GD.Print("Host ID: " + host_data.GetSteamID().ToString());
    }

} 