// Load the game -> Prepare the game -> Start the game


using Godot;
using System;
using System.Collections.Generic;
using Steamworks;

public class GameInitializer : Node{

    LobbyManager lobby_manager;
    PackedScene HURRY_BOATS;
    HurryBoats loading_game;
    GameInitializerPM initializerPM;
    List<ulong> players_in_queue = new List<ulong>();
    

    public override void _Ready(){
        lobby_manager = GetTree().Root.GetNode<LobbyManager>("LobbyManager");
        initializerPM = GetNode<GameInitializerPM>("GameInitializerPM");
        HURRY_BOATS = (PackedScene)ResourceLoader.Load("res://scenes/game/HurryBoats.tscn");
        if( initializerPM.ImHost() ){
            FillPlayersInQueue();
        }
        LoadGame();
    }


    void FillPlayersInQueue(){
        players_in_queue.Clear();
        foreach(CSteamID player_id in lobby_manager.GetPlayersSteamID()){
                players_in_queue.Add((ulong) player_id);
        }
    }

    // --------------- Stage 1

    async public void LoadGame(){
        GetTree().Paused = true;
        loading_game = HURRY_BOATS.Instance<HurryBoats>();
        loading_game.Connect("GameReady", this, nameof(OnGameReadySignal));
        GetTree().Root.CallDeferred("add_child", loading_game);
        await ToSignal(loading_game, "ready");
        if(lobby_manager.ImHost())
            OnPlayerGameLoaded( (ulong) lobby_manager.GetMySteamID() );
        else{
            initializerPM.SendPlayerGameLoaded( (ulong) lobby_manager.GetMySteamID() );
            GD.Print("Game ready, waiting for host...");
        }
    }


    public void OnPlayerGameLoaded(ulong player_id){
        if(lobby_manager.ImHost()){
            players_in_queue.Remove(player_id);
            if(players_in_queue.Count == 0){
                GD.Print("Game succesfully loaded, preparing game...");
                PrepareGame();
                players_in_queue.Clear();
            }
        }
    }

    // --------------- Stage 2
    

    // Game is prepared internally
    void PrepareGame(){
        if( initializerPM.ImHost() ){
            FillPlayersInQueue();
            loading_game.HostGamePrepare();
        }
    }


    void OnGameReadySignal(){
        GD.Print("Game succesfully prepared!");
        if( initializerPM.ImHost() ){
            OnPlayerGameIsReady( (ulong) lobby_manager.GetHostSteamID() );
        }
        else{
            initializerPM.SendPlayerGameIsReady( (ulong) lobby_manager.GetMySteamID() );
        }
    }


    // Trigger when a peer has finished his game preparation
    public void OnPlayerGameIsReady(ulong player_id){
        if( initializerPM.ImHost()){
            players_in_queue.Remove(player_id);
            if(players_in_queue.Count == 0){
                // Send StartGame
                initializerPM.SendStartGame();
                StartGame();
            }
        }
    }


    public void StartGame(){
        GD.Print("Starting the game.");
        GetTree().Paused = false;
        QueueFree();
    }


}
