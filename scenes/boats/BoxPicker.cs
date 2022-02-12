using Godot;
using System;

public class BoxPicker : Area2D{


    WeaponManager weapon_manager;


    public override void _Ready(){
        weapon_manager = GetParent<WeaponManager>();
    }


    void _onBoxPickerAreaEntered(Area2D area){
        if(area is Box){
            CheckBox((area as Box));
        }
    }


    void CheckBox(Box box){
        if(box.IsFull()){
            if(box.GetContentType() == "weapon"){
                OpenWeaponBox(box);
            }
        }
    }


    void OpenWeaponBox(Box box){
        if( weapon_manager.TryToPickWeapon(box.GetContent())){
            box.DestroyCrate();
        }
    }

}
