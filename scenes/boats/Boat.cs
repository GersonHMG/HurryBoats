using Godot;
using System;

public class Boat : KinematicBody2D{
    
    const float MAX_LINEAL_VELOCITY = 100.0f;
    const float MAX_ANGULAR_VELOCITY = 2.0f;
    Vector2 velocity = Vector2.Zero;
    Vector2 head = Vector2.Up;
    Vector2 acceleration = Vector2.Zero;
    float angular_acceleration = 0.0f;
    float angular_velocity = 0.0f;
    ushort throttle_force = 1;
    float drag_foce = 0.05f;
    float rudder_direction = 0.0f; 

    WeaponManager cannons;
    BoatPM boatPM;
    BoatEffectsManager effects;
    public bool is_owner = false;
    ushort boat_id = 0;
    
    [Signal]
    delegate void OnDestroy();

    public void SetBoatID(ushort new_id){
        boat_id = new_id;
    }


    public override void _Ready(){
        cannons = GetNode<WeaponManager>("WeaponManager");
        boatPM = GetNode<BoatPM>("BoatPM"); 
        effects = GetNode<BoatEffectsManager>("EffectsManager");
        if(boat_id == boatPM.GetClientData().GetPlayerID()){
            is_owner = true;
        } 
        if(!is_owner){
            GetNode<Control>("Hud/Control").Visible = false;
        } 
    }


    public override void _PhysicsProcess(float delta){
        base._PhysicsProcess(delta);
        if(is_owner){
            PlayerInput();
            ApplyControlls();
        }
        ApplyDragForces();
        ApplyMovement(delta);
        if(is_owner){
            boatPM.SendMovement(GlobalPosition, GlobalRotation, velocity, angular_velocity);
        }
    }


    void ApplyControlls(){
        angular_acceleration = rudder_direction*0.05f;
        rudder_direction = Godot.Mathf.Clamp(rudder_direction, -1.0f, 1.0f);
        rudder_direction = 0.0f;
        angular_velocity += angular_acceleration;
        if(velocity.Length() <= MAX_LINEAL_VELOCITY)
            velocity += acceleration;
        angular_velocity = Godot.Mathf.Clamp(angular_velocity, -MAX_ANGULAR_VELOCITY, MAX_ANGULAR_VELOCITY);
    }


    void RotateSail(float magnitude){
        float m = (0.39f)/(MAX_ANGULAR_VELOCITY);
        float new_rotation = m*(magnitude);
        new_rotation = Godot.Mathf.Clamp(new_rotation, -0.39f, 0.39f);
        GetNode<Sprite>("Sail").Rotation = new_rotation;
    }


    public void SetSkin(int skin_number){
        Sprite sail = GetNode<Sprite>("Sail");
        Sprite boat_skin = GetNode<Sprite>("BoatBase");
        switch(skin_number){
            case 0:
                sail.Texture = ResourceLoader.Load<Texture>("res://assets/boats/default_sail.png");
                boat_skin.Texture = ResourceLoader.Load<Texture>("res://assets/boats/default_boat.png");
                break;
            case 1:
                sail.Texture = ResourceLoader.Load<Texture>("res://assets/boats/patriot_sail.png");
                boat_skin.Texture = ResourceLoader.Load<Texture>("res://assets/boats/patriot_boat.png");
                break;
            case 2:
                sail.Texture = ResourceLoader.Load<Texture>("res://assets/boats/orange_sail.png");
                boat_skin.Texture = ResourceLoader.Load<Texture>("res://assets/boats/orange_boat.png");
                break;
            case 3:
                sail.Texture = ResourceLoader.Load<Texture>("res://assets/boats/green_sail.png");
                boat_skin.Texture = ResourceLoader.Load<Texture>("res://assets/boats/green_boat.png");
                break;
        }
    }




    void ApplyDragForces(){
        if( Math.Abs( angular_acceleration ) <= 0){
            angular_velocity -= angular_velocity*drag_foce;
        }
        if(acceleration.LengthSquared() <= 0){
            velocity -= velocity*drag_foce*drag_foce;
        }
        Vector2 side_velocity = velocity.Dot( head.Rotated( Mathf.Pi/2.0f) )*head.Rotated(Mathf.Pi/2.0f);
        velocity -= side_velocity*drag_foce;
    }


    void ApplyMovement(float delta){
        //velocity = velocity.Clamped(MAX_LINEAL_VELOCITY);
        GlobalRotation += angular_velocity*delta;
        velocity = MoveAndSlide(velocity);
        acceleration = Vector2.Zero;
        angular_acceleration = 0.0f;
        RotateSail(angular_velocity);
        head = Vector2.Up.Rotated(GlobalRotation);
    }


    void PlayerInput(){
        if(Input.IsActionPressed("w")){
            acceleration = head*throttle_force;
        }
        if(Input.IsActionPressed("a")){
            rudder_direction = -1.0f;
        }
        if(Input.IsActionPressed("d")){
            rudder_direction = 1.0f;
        }
    }


    public void Destroy(){
        if(boatPM.ImHost()){
            SyncDestroy();
            boatPM.SendDestroy();
        }
    }
    

    public void SyncDestroy(){
        effects.Explosion();
        EmitSignal(nameof(OnDestroy));
        QueueFree();
    }





    void Shoot(){
        velocity += head.Rotated(Mathf.Pi/2.0f)*800.0f;
    }


    public Vector2 GetLinealVelocity() => velocity;


    public void SyncMovement(Vector2 new_position, float new_rotation, Vector2 new_velocity, float new_angular_velocity){
        GlobalPosition = new_position;
        GlobalRotation = new_rotation;
        velocity = new_velocity;
        angular_velocity = new_angular_velocity;
    }

}
