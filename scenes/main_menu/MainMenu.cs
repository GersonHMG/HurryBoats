using Godot;
using System;

public class MainMenu : Control
{

	SceneChanger scene_changer;
	LobbyManager lobby_manager;

	public override void _Ready(){
		lobby_manager = GetNode<LobbyManager>("/root/LobbyManager");
		scene_changer = GetNode<SceneChanger>("/root/SceneChanger");
		lobby_manager.Connect("LobbyCreatedResult", this, nameof(_OnLobbyCreatedResult) );
		
	}

	void HostGame(){
		lobby_manager.CreateLobby(); 
	}


	public void _OnLobbyCreatedResult(bool result){
		if(result){
			scene_changer.ChangeSceneTo(this, SceneChanger.SCENES.LOBBY);
		}
	}


	public void _OnStartGamePressed(){
		HostGame();
	}


	public void _OnJoinGamePressed(){
		scene_changer.ChangeSceneTo(this, SceneChanger.SCENES.JOIN_LOBBY);
	}
}
