using Godot;
using System;

public class TraceEffect : Line2D
{
    const int MAX_POINTS = 5;
    Node2D parent;

    const float MAX_TIME_PASSED = 0.02f;
    float time_passed;

    public override void _Ready(){
        parent = GetParent<Node2D>();
    }

    public override void _Process(float delta){
        GlobalPosition = Vector2.Zero;
        GlobalRotation = 0;
        time_passed += delta;
        if(time_passed > MAX_TIME_PASSED){
            time_passed = 0.0f;
            AddPoint(parent.GlobalPosition);
            while(GetPointCount() > MAX_POINTS){
                RemovePoint(0);
        }
        }
        
        
    }
}
