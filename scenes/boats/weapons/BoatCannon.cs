using Godot;
using System;

public class BoatCannon : Sprite
{

    PackedScene PROYECTILE;
    Boat boat;

    public override void _Ready()
    {
        PROYECTILE = ResourceLoader.Load<PackedScene>("res://scenes/projectiles/CannonBall.tscn");
    }

    public void Init(Boat _boat){
        boat = _boat;
    }

    public void Shoot(){
        CannonBall new_proyectile = PROYECTILE.Instance<CannonBall>();
        new_proyectile.SetDirection( GlobalPosition.DirectionTo(GetNode<Position2D>("ShootPosition").GlobalPosition));
        new_proyectile.SetLinealVelocity(boat.GetLinealVelocity());
        GetParent().GetParent().GetParent().GetParent().AddChild(new_proyectile);
        new_proyectile.GlobalPosition = GetNode<Position2D>("ShootPosition").GlobalPosition;
    }




}
