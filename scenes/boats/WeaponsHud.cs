using Godot;
using System;
using System.Collections.Generic;


public class WeaponsHud : HBoxContainer{
    PackedScene WEAPON_ICONS;
    Dictionary<ushort, WeaponIcon> weapon_icon_by_id = new Dictionary<ushort, WeaponIcon>();
    WeaponIcon last_icon = null;

    public override void _Ready(){
        WEAPON_ICONS = ResourceLoader.Load<PackedScene>("res://scenes/boats/hud/icons/WeaponIcon.tscn");
        weapon_icon_by_id.Add(0, GetNode<WeaponIcon>("Empty"));
    }


    public override void _PhysicsProcess(float delta){
         if(Input.IsActionPressed("q")){
            GetNode<TextureRect>("../QKey").Modulate = Colors.White;
        }
        else if(Input.IsActionPressed("e")){
            GetNode<TextureRect>("../EKey").Modulate = Colors.White;
        }
        else{
            GetNode<TextureRect>("../QKey").Modulate = new Color("5affffff");
            GetNode<TextureRect>("../EKey").Modulate = new Color("5affffff");
        }
        
    }


    public WeaponIcon AddNewWeapon(string weapon_name){
        WeaponIcon new_icon = WEAPON_ICONS.Instance<WeaponIcon>();
        new_icon.SetIcon(weapon_name);
        AddChild(new_icon);
        return new_icon;
    }

    void _OnNewWeapon(ushort weapon_id, string weapon_name){
        weapon_icon_by_id.Add(weapon_id, AddNewWeapon(weapon_name) );
    }


    void _OnDeleteWeapon(ushort weapon_id){
        weapon_icon_by_id[weapon_id].QueueFree();
        weapon_icon_by_id.Remove(weapon_id);
    }

    
    void _OnSelectWeapon(ushort weapon_id){
        if(Input.IsActionJustPressed("q"))
                GetNode<AudioStreamPlayer>("QKey").Play();
        if(Input.IsActionJustPressed("e"))
                GetNode<AudioStreamPlayer>("EKey").Play();
        if(IsInstanceValid(last_icon))
            last_icon.Deselect();
        last_icon = weapon_icon_by_id[weapon_id];
        last_icon.Select();
    }




}
