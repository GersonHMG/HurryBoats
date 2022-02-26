using Godot;
using System;

public class PlayerLobbyInfo : HBoxContainer
{
    const int MAX_NAME_LENGTH = 20;


    public void SetPlayerName(string new_name){
        if (new_name.Length > MAX_NAME_LENGTH)
            new_name = new_name.Substring(0, MAX_NAME_LENGTH);
        GetNode<Button>("PlayerName").Text = new_name;
    }

    public void SetAvatar(ImageTexture img){
        GetNode<TextureRect>("Avatar").Texture = img;
    }


    public void SetStatusReady(){
        GetNode<Label>("Status").Text = "Ready";
        GetNode<Label>("Status").AddColorOverride("font_color",Colors.Green);
    }

}
