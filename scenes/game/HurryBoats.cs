using Godot;
using System;
using System.Collections.Generic;

public class HurryBoats : Node2D{

    LobbyManager lobby_manager;
    PackedScene BOAT_SCENE;
    MapStructures game_map;
    HurryBoatsPM hurryBoatsPM;
    GameReset game_reset;
    bool reset_round = false;

    [Signal]
    public delegate void GameReady();
    SceneChanger scene_changer;

    int boats_left = 0;

    public override void _Ready(){
        scene_changer = GetNode<SceneChanger>("/root/SceneChanger");
        BOAT_SCENE = ResourceLoader.Load<PackedScene>("res://scenes/boats/Boat.tscn");
        game_map = GetNode<MapStructures>("MapStructures");
        hurryBoatsPM = GetNode<HurryBoatsPM>("HurryBoatsPM");
        lobby_manager = GetTree().Root.GetNode<LobbyManager>("LobbyManager");
        game_reset = GetNode<GameReset>("GameReset");
    }


    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
    }


    public void HostGamePrepare(){
        GD.Print("Host is preparing the game");
        // Prepare Init Data
        // Send PrepareGame
        int seed = new Random().Next();
        hurryBoatsPM.SendPrepareGame(seed);
        PrepareGame(seed);
    }


    public void PrepareGame(int seed){
        SpawnBoats();
        game_map.InitMap( seed );
        EmitSignal(nameof(GameReady));
    }


    void SpawnBoats(){
        List<PlayerLobbyData> players = lobby_manager.GetPlayersData();
        List<ushort> players_id = new List<ushort>();
        foreach(PlayerLobbyData player in players){
            players_id.Add( player.GetPlayerID() );
        }
        players_id.Sort();
        Node2D spawn_positions =  GetNode<Node2D>("SpawnPositions");
        foreach(PlayerLobbyData player in players){
            Boat new_boat = BOAT_SCENE.Instance<Boat>();
            int player_index = players_id.IndexOf(player.GetPlayerID());
            new_boat.GlobalPosition = spawn_positions.GetChild<Position2D>(player_index).GlobalPosition;
            new_boat.Name = player.GetPlayerID().ToString() + "boat";
            boats_left += 1;
            new_boat.SetBoatID(player.GetPlayerID());
            new_boat.Connect("OnDestroy", this, nameof(OnBoatDestroy));
            GetNode<Node2D>("Entities").AddChild(new_boat);
            new_boat.SetSkin(player_index);
        }
    }


    public void ResetWorld(){
        boats_left = 0;
        GetNode<HurryBoatsCamera>("HurryBoatsCamera").GlobalPosition = new Vector2(959, 280);
        int name_r = 0;
        foreach(Node children in GetNode<Node2D>("Entities").GetChildren() ){
            children.Name = "del" + name_r.ToString();
            name_r += 1;
            children.QueueFree();
        }
        game_map.Reset();
        reset_round = false;
    }


    void ResetRound(){
        HostGamePrepare();
    }


    async void OnBoatDestroy(){
        boats_left -= 1;
        if(boats_left <= 1 && !reset_round){
            reset_round = true;
            GetNode<AudioStreamPlayer>("WinSound").Play();
            if(hurryBoatsPM.ImHost()){
                await ToSignal(GetTree().CreateTimer(2), "timeout");
                game_reset.StartResetRound();
            }
        }
    }


    async void ToLobby(){
        GetNode<AudioStreamPlayer>("WinSound").Play();
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        scene_changer.ChangeSceneTo(this, SceneChanger.SCENES.LOBBY);
    }




}
