using Godot;
using System;
using Steamworks;
using System.Collections.Generic;

public class Lobby : Control{
    
    LobbyPM lobbyPM;
    LobbyManager lobby_manager;
    SceneChanger scene_changer;
    VBoxContainer players_container;
    PackedScene PLAYER_LABEL_INFO;
    Dictionary<CSteamID, HBoxContainer> playerid_and_label = new Dictionary<CSteamID, HBoxContainer>();
    bool player_ready = false;

    public override void _Ready(){
        scene_changer = GetNode<SceneChanger>("/root/SceneChanger");
        lobby_manager = GetNode<LobbyManager>("/root/LobbyManager");
        lobbyPM = lobby_manager.GetLobbyPM();
        players_container = GetNode<VBoxContainer>("PlayersContainer");
        PLAYER_LABEL_INFO = (PackedScene)ResourceLoader.Load("res://scenes/main_menu/lobby/PlayerLobbyInfo.tscn");
        lobby_manager.Connect("PlayersUpdate", this, nameof(OnPlayersUpdate) );
        GetNode<TextEdit>("LobbyID").Text = ( (ulong) lobby_manager.GetLobbyID() ).ToString();
        if(!lobby_manager.ImHost()){
            lobbyPM.SendRequestPlayers( SteamUser.GetSteamID() );
            GetNode<Button>("StartGame").Visible = false;
        }else{
            GeneratePlayers();
        }
    }

    
    void GeneratePlayers(){
        if(!player_ready){
            lobbyPM.SendPlayerReady(lobby_manager.GetMySteamID());
        }
        player_ready = true;
        // Remove each player from container
        foreach( Node player in players_container.GetChildren()){
            player.QueueFree(); 
        }
        playerid_and_label.Clear();

        //Generate new players in the container
        List<PlayerLobbyData> players = lobby_manager.GetPlayersData();
        foreach( PlayerLobbyData player in players ){
            PlayerLobbyInfo new_player_label = PLAYER_LABEL_INFO.Instance<PlayerLobbyInfo>();
            players_container.AddChild(new_player_label);
            new_player_label.SetPlayerName( player.GetPlayerName() );
            new_player_label.SetAvatar( player.GetPlayerAvatar() );
            playerid_and_label.Add( player.GetSteamID() ,new_player_label);
            if(player.GetPlayerConnectionStatus() == (byte) PlayerLobbyData.CONNECTION_STATE.READY){
                new_player_label.SetStatusReady();
            }
        }
    }


    public void OnPlayersUpdate(){
        GeneratePlayers();
    }


    void _OnExitPressed(){
        scene_changer.ChangeSceneTo(this, SceneChanger.SCENES.MENU);
        lobby_manager.LeaveLobby();
    }


    void _OnStartGamePressed(){
        // Send packet StartGame
        if(lobby_manager.ImHost()){
            lobbyPM.StartGameSync();
            StartGame();
        }
    }


    public void StartGame(){
        GD.Print("Starting game, calling to game initializer");
        PackedScene SCENE = (PackedScene) ResourceLoader.Load<PackedScene>("res://scenes/game_initializer/GameInitializer.tscn");
        GameInitializer game_initiallizer = SCENE.Instance<GameInitializer>();
        GetTree().Root.CallDeferred("add_child", game_initiallizer);
        QueueFree();
    }


}
