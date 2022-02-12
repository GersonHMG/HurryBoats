using Godot;
using System;

public class HurryBoatsCamera : Camera2D{
    float DISTANCE_PER_CHUNK = 1000.0f;

    float speed = 20.0f;
    Vector2 velocity = Vector2.Zero;
    float traveled_distance = 0.0f;
    

    [Signal]
    delegate void MaxDistanceTraveled();



    public override void _PhysicsProcess(float delta){
        CameraMovement();
        //DebugMovement();
        ApplyMovement(delta);
        if(traveled_distance >= DISTANCE_PER_CHUNK){
            traveled_distance = 0.0f;
            EmitSignal(nameof(MaxDistanceTraveled));
        }
    }


    void ApplyMovement(float delta){
        GlobalPosition += velocity*delta;
        traveled_distance += velocity.Length()*delta;
    }


    public void _OnBodyLimitsEntered(Node2D body){
        if(body is Boat){
            Boat boat = (body as Boat);
            boat.Destroy();
        }
    }


    void CameraMovement(){
        velocity = speed*Vector2.Up;
    }


    void DebugMovement(){
        Vector2 dir = Vector2.Zero;
        if(Input.IsActionPressed("ui_up"))
            dir += Vector2.Up;
        if(Input.IsActionPressed("ui_down"))
            dir += Vector2.Down;
        if(Input.IsActionPressed("ui_left"))
            dir += Vector2.Left;
        if(Input.IsActionPressed("ui_right"))
            dir += Vector2.Right;
        velocity = 800*dir;
    }


    

}
