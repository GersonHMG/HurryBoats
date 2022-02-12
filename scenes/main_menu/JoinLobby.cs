using Godot;
using System;

public class JoinLobby : Control
{
    // Singletons
    LobbyManager lobby_manager;
    SceneChanger scene_changer;

    public override void _Ready(){
        lobby_manager = GetNode<LobbyManager>("/root/LobbyManager");
        scene_changer = GetNode<SceneChanger>("/root/SceneChanger");
        lobby_manager.Connect("ConnectionEstablished", this, nameof(_ConnectionEstablished) );
        lobby_manager.Connect("ConnectionFailed", this, nameof(_ConnectionFailed) );
    }


    public void _OnToMenuPressed(){
        scene_changer.ChangeSceneTo(this, SceneChanger.SCENES.MENU);
    }


    public void _OnJoinPressed(){
        TextEdit text_label = GetNode<TextEdit>("VBoxContainer/HBoxContainer/TextEdit");
        Steamworks.CSteamID join_lobby_id = (Steamworks.CSteamID)ulong.Parse(  text_label.Text, System.Globalization.NumberStyles.None  );
        lobby_manager.JoinToLobby( join_lobby_id  );
        GD.Print("Tried to join to lobby: " + join_lobby_id.ToString());
        GetNode<VBoxContainer>("VBoxContainer").Visible = false;
        GetNode<ColorRect>("Waiting").Visible = true;
    }   


    public void _ConnectionEstablished(){
        scene_changer.ChangeSceneTo(this, SceneChanger.SCENES.LOBBY); 
    }


    void _ConnectionFailed(){
        GetNode<VBoxContainer>("VBoxContainer").Visible = true;
        GetNode<ColorRect>("Waiting").Visible = false;
        GetNode<WindowDialog>("ConnectionFailedPopUp").Popup_();
    }
    

}
