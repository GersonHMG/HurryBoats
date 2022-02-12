using Godot;
using System;
using System.Collections.Generic;


public class SceneChanger : CanvasLayer
{

    public enum SCENES : ushort{
        MENU,
        LOBBY,
        JOIN_LOBBY
    };

    private Dictionary<SCENES, string> SCENES_PATH = new Dictionary<SCENES, string>
    {
        {SCENES.MENU , "res://scenes/main_menu/MainMenu.tscn"},
        {SCENES.JOIN_LOBBY , "res://scenes/main_menu/JoinLobby.tscn"},
        {SCENES.LOBBY, "res://scenes/main_menu/lobby/Lobby.tscn"}
    };


    async public void ChangeSceneTo( Node from, SCENES to_scene){
        var new_scene = (Node) ( (PackedScene) GD.Load( SCENES_PATH[ to_scene ] ) ).Instance();
        GetParent().CallDeferred("add_child", new_scene);
        await ToSignal(new_scene, "ready");
        from.QueueFree();

    }


}
