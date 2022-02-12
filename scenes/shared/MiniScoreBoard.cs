using Godot;
using System;
using System.Collections.Generic;

public class MiniScoreBoard : Control{
    PackedScene player_rectangle;
    VBoxContainer rectangle_container;
    Dictionary<ushort, ColorRect> rectangle_by_playerid = new Dictionary<ushort, ColorRect>();
    Dictionary<ulong, int> score_by_playerid = new Dictionary<ulong, int>();


    public override void _Ready(){
        player_rectangle = (PackedScene)ResourceLoader.Load("res://minigames/shared/PlayerBoardInfo.tscn");
        rectangle_container = GetNode<VBoxContainer>("VBoxContainer");
    }


    public void AddPlayer(PlayerLobbyData player){
        ColorRect new_player_rect = AddPlayerToBoard(player.GetPlayerName());
        rectangle_by_playerid.Add(player.GetPlayerID(), new_player_rect);
        score_by_playerid.Add(player.GetPlayerID(), 0);
    }


    ColorRect AddPlayerToBoard(string player_name){
        ColorRect new_rectangle = player_rectangle.Instance<ColorRect>();
        rectangle_container.AddChild(new_rectangle);
        new_rectangle.GetNode<Label>("HBoxContainer/Place").Text = (rectangle_by_playerid.Count  + 1).ToString();
        //string new_player_name = player_name.Substring(0, 20);
        new_rectangle.GetNode<Label>("HBoxContainer/Name").Text = player_name;
        new_rectangle.GetNode<Label>("HBoxContainer/Score").Text = "0";
        return new_rectangle;
    }


    public void UpdatePlayerScore(ushort player_id, int new_score){
        rectangle_by_playerid[player_id].GetNode<Label>("HBoxContainer/Score").Text = new_score.ToString();
    }


    public void AddPointsToPlayer(ushort player_id, int score){
        SetPlayerScore(player_id, score_by_playerid[player_id] + score);
    }


    public void SetPlayerScore(ushort player_id, int new_score){
        score_by_playerid[player_id] = new_score;
        UpdatePlayerScore(player_id, new_score);
    }



}
