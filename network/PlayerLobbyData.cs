// Save player lobby data
using Godot;
using System;
using Steamworks;

public class PlayerLobbyData{

    enum States : ushort{
        READY,
        JOINING
    }

    string player_name;
    CSteamID steam_id;
    ushort player_id;
    ushort player_state = (ushort) States.JOINING;
    ImageTexture player_avatar;



    public PlayerLobbyData(CSteamID _steam_id, ushort _player_id){
        steam_id = _steam_id;
        player_id = _player_id;
        player_name = SteamFriends.GetFriendPersonaName(_steam_id);
        SetPlayerImage();
    }

    public ImageTexture GetPlayerAvatar() => player_avatar;


    void SetPlayerImage(){
        int image = SteamFriends.GetMediumFriendAvatar(steam_id);
        uint width, height;
        bool bIsValid = SteamUtils.GetImageSize(image, out width, out height);
        Image avatar = new Image();
        ImageTexture avatar_texture = new ImageTexture();
        if(bIsValid){
            byte[] image_buffer = new byte[width * height * 4];
            bIsValid = SteamUtils.GetImageRGBA(image, image_buffer, (int)(width * height * 4));
            if (bIsValid){
                avatar.Create((int) width,(int) height, false, Image.Format.Rgbf);
                avatar.Lock();
                for(int x = 0; x < width; x++){
                    for(int y = 0; y < height; y++){
                        var pixel = 4*(x+ y*width);
                        float r = image_buffer[pixel]/255.0f;
                        float g = image_buffer[pixel + 1]/255.0f;
                        float b = image_buffer[pixel + 2]/255.0f;
                        float a = image_buffer[pixel + 3]/255.0f;
                        avatar.SetPixel(x, y, new Color(r,g,b,a));
                    }
                }
                avatar.Unlock();
            }
        }
        avatar_texture.CreateFromImage(avatar);
        player_avatar = avatar_texture;
    }

    public string GetPlayerName() => player_name;

    public CSteamID GetSteamID() => steam_id;

    public ushort GetPlayerID() => player_id;


}
