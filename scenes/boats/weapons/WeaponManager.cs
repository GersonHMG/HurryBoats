using Godot;
using System;
using System.Collections.Generic;

public class WeaponManager : Node2D{

    Boat boat;
    WeaponManagerPM weaponManagerPM;
    ushort weapon_count = 1;
    Dictionary<ushort, DoubleCannons> weapons_by_id = new Dictionary<ushort, DoubleCannons>(){{0,null}};
    List<ushort> weapons_order = new List<ushort>(){0};
    ushort current_weapon;

    [Signal]
    delegate void NewWeapon(ushort ID, string wpn_name);
    [Signal]
    delegate void DeleteWeapon(ushort ID);
    [Signal]
    delegate void OnSelectWeapon(ushort ID);


    Dictionary<string, string> scene_by_name = new Dictionary<string, string>(){
        {"DoubleCannon","res://scenes/weapons/DoubleCannons.tscn"}
    };

    public override void _Ready(){
        boat = GetParent<Boat>();
        weaponManagerPM = GetNode<WeaponManagerPM>("WeaponManagerPM");
    }


    public override void _PhysicsProcess(float delta){
        if(boat.is_owner){
            PlayerInput();
        }
    }


    void PlayerInput(){
        if (Input.IsActionJustPressed("ui_accept")){
            if(IsInstanceValid(weapons_by_id[current_weapon])){
                weapons_by_id[current_weapon].TryToShoot();
            }
        }
        if(Input.IsActionJustPressed("q")){
            SelectBackWeapon();
        }
        else if(Input.IsActionJustPressed("e")){
            SelectNextWeapon();
        }
    }


    void SelectNextWeapon(){
        ushort last_weapon_id = current_weapon;
        int current_index = weapons_order.IndexOf(current_weapon);
        int next_weapon_index = current_index + 1;
        if(next_weapon_index < weapons_order.Count){
            current_weapon = weapons_order[next_weapon_index];
        }
        else{
            current_weapon = 0;
        }
        if (current_index <= 0)
            current_index = 0;
        if(boat.is_owner){
            weaponManagerPM.SendSelectWeapon(last_weapon_id, current_weapon); // SendUnreliableToAll()
            SelectWeapon(last_weapon_id, current_weapon);
        }
    }


    void SelectBackWeapon(){
        ushort last_weapon_id = current_weapon;
        int current_index = weapons_order.IndexOf(current_weapon);
        int next_weapon_index = current_index - 1;
        if(next_weapon_index >= 0){
            current_weapon = weapons_order[next_weapon_index];
        }
        else{
            current_weapon = weapons_order[weapons_order.Count - 1];
        }
        if(boat.is_owner){
            weaponManagerPM.SendSelectWeapon(last_weapon_id, current_weapon); // SendUnreliableToAll()
            SelectWeapon(last_weapon_id, current_weapon);
        }
    }


    public void SelectWeapon(ushort last_id, ushort weapon_id){
        DoubleCannons weapon = null;
        if( weapons_by_id.TryGetValue(weapon_id, out weapon) ){
            if( IsInstanceValid(weapon) ){
                weapons_by_id[weapon_id].DeployWeapon();
            }
        }
        DoubleCannons value = null;
        if(weapons_by_id.TryGetValue(last_id,out value)){
            if( IsInstanceValid(value) )
                value.HideWeapon();
        }
        if(boat.is_owner){
            EmitSignal(nameof(OnSelectWeapon), weapon_id);
        }
    }


    public bool TryToPickWeapon(string weapon){
        
        if(true){
            if(weaponManagerPM.ImHost()){
                weaponManagerPM.SendRegisterWeapon(weapon, weapon_count);
                RegisterWeapon(weapon, weapon_count);
                weapon_count += 1;
                }
            return true;
        }
        return false;
    }


    public void RegisterWeapon(string weapon_name, ushort weapon_id){
        PackedScene scene = ResourceLoader.Load<PackedScene>(scene_by_name[weapon_name]);
        DoubleCannons new_weapon = scene.Instance<DoubleCannons>();
        new_weapon.Name = weapon_id.ToString() + weapon_name;
        AddChild(new_weapon);
        weapons_by_id.Add(weapon_id, new_weapon);
        weapons_order.Add(weapon_id);
        new_weapon.Connect("OutOfAmmo", this, nameof(_OnWeaponUseless), new Godot.Collections.Array(weapon_id));
        EmitSignal(nameof(NewWeapon), weapon_id, weapon_name);
    }


    public void UnregisterWeapon(ushort id){
        DoubleCannons weapon = null;
        if(weapons_by_id.TryGetValue(id, out weapon)){
            weapon.Delete();
            weapons_by_id.Remove(id);
            weapons_order.Remove(id);
            EmitSignal(nameof(DeleteWeapon), id);
        }
        SelectBackWeapon();
    }


    void _OnWeaponUseless(ushort weapon_id){
        if(weaponManagerPM.ImHost()){
            weaponManagerPM.SendUnregisterWeapon(weapon_id);
            UnregisterWeapon(weapon_id);
        }
    }
    
}
