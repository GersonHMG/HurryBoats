using Godot;
using System;
using System.Collections.Generic;


public class GameReset : Node{

    HurryBoats parent;
    GameResetPM gameResetPM;
    LobbyManager lobby_manager;
    List<ushort> players_queue = new List<ushort>();
    CurtainTransition transition;


    public override void _Ready(){
        base._Ready();
        parent = GetParent<HurryBoats>();
        gameResetPM = GetNode<GameResetPM>("GameResetPM");
        lobby_manager = GetTree().Root.GetNode<LobbyManager>("LobbyManager");
        transition = GetNode<CurtainTransition>("CanvasLayer/CurtainTransition");
    }


    public void StartResetRound(){
        if(gameResetPM.ImHost()){
            players_queue.Clear();
            foreach(PlayerLobbyData player in lobby_manager.GetPlayersData()){
                players_queue.Add( player.GetPlayerID() );
            }
            int seed = new Random().Next();
            gameResetPM.SendResetRound(seed);
            ResetRound(seed);
        }
    }


    async public void ResetRound(int seed){
        transition.StartTransition();
        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
        GetTree().Paused = true;
        parent.ResetWorld();
        // Wait 1 time to wait queuefree of nodes
        await ToSignal(GetTree().CreateTimer(1), "timeout"); // <--- BAD SOLUTION
        parent.PrepareGame(seed);
        if(gameResetPM.ImHost()){
            OnPlayerRoundReady(gameResetPM.GetClientData().GetPlayerID());
        }
        else{
            gameResetPM.SendPlayerReady(gameResetPM.GetClientData().GetPlayerID());
        }
        
    }


    public void OnPlayerRoundReady(ushort player_id){
        if(gameResetPM.ImHost()){
            players_queue.Remove(player_id);
            if(players_queue.Count <= 0){
                gameResetPM.SendStartRound();
                StartRound();
            }
        }
    }


    public void StartRound(){
        transition.EndTransition();

        GetTree().Paused = false;
    }



}
