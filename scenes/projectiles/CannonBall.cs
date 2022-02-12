using Godot;
using System;

public class CannonBall : Area2D{
    
    Vector2 lineal_velocity;
    float speed = 200.0f;
    float air_drag = 0.02f;
    const float MAX_DISTANCE = 200.0f;
    float traveled_distance = 0.0f;
    Vector2 direction = Vector2.Zero;


    public override void _PhysicsProcess(float delta){
        if(lineal_velocity.Length() <= 40){
            SubMerge();
        }
        ApplyMovement(delta);
    }

    void SubMerge(){
        QueueFree();
    }


    void ApplyMovement(float delta){
        GlobalPosition += lineal_velocity*delta;
        traveled_distance += lineal_velocity.Length()*delta;
    }


    public void SetLinealVelocity(Vector2 new_velocity){
        lineal_velocity += new_velocity;
    }


    public void SetDirection(Vector2 _direction){
        direction = _direction.Normalized();
        lineal_velocity += direction*speed;
    }

    void _OnCannonBallBodyEntered(Node2D body){
        if(body.IsInGroup("land")){
            (body as LandPiece).Destroy();
            QueueFree();
        }
        else if(body.IsInGroup("boat")){
            (body as Boat).Destroy();
            QueueFree();
        }
    }

    public void _OnDeleteTimeOut(){
        QueueFree();
    }


}
