using Godot;
using System;
using System.Collections.Generic;

public class WeaponIcon : TextureRect{
    
    Dictionary<string,string> icons_by_names = new Dictionary<string, string>(){
        {"DoubleCannon","res://assets/icons/boat_hud/double_cannon_icon.png"}
    };


    public void SetIcon(string icon_name){
        Texture = ResourceLoader.Load<Texture>(icons_by_names[icon_name]);
    }

    public void Select(){
        Modulate = new Color("ffffff");
    }

    public void Deselect(){
        Modulate = new Color("3bffffff");

    }


}
