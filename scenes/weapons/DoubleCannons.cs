using Godot;
using System;
using System.Collections.Generic;

public class DoubleCannons : Node2D{

    bool can_shoot = false;
    int ammo = 1;
    AnimationPlayer animationPlayer;
    List<BoatCannon> cannons = new List<BoatCannon>();
    Boat boat;
    WeaponPM weaponPM;

    [Signal]
    public delegate void OutOfAmmo();


    public override void _Ready(){
        boat = GetParent().GetParent<Boat>();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        weaponPM = GetNode<WeaponPM>("WeaponPM");
        InitCannons();
    }


    void InitCannons(){
        cannons.Add( GetNode<BoatCannon>("BoatCannon") );
        cannons.Add( GetNode<BoatCannon>("BoatCannon2") );
        foreach(BoatCannon cannon in cannons){
            cannon.Init(boat);
        }
    }


    public void DeployWeapon(){
        animationPlayer.Play("DeployCannons");
    }


    public void HideWeapon(){
        animationPlayer.PlayBackwards("HideCannons");
        can_shoot = false;
    }


    public void TryToShoot(){
        if(ammo > 0 && can_shoot){
            weaponPM.SendShoot();
            Shoot();
        }
        else{
            GetNode<AudioStreamPlayer2D>("CantShootSound").Play();
        }
    }


    public void Shoot(){
        GetNode<AudioStreamPlayer2D>("ShootSound").Play();
        foreach( BoatCannon cannon in cannons){
            cannon.Shoot();
        }
        HideWeapon();
        ammo -= 1;
        if(ammo <= 0){
            EmitSignal(nameof(OutOfAmmo));
        }
    }


    public void Delete(){
        Visible = false;
        GetNode<Timer>("DeleteTime").Start();
        
    }


    void _OnDeleteTimeOut(){
        QueueFree();
    }


    void SetCanShootTrue(){
        can_shoot = true;
    }


}
