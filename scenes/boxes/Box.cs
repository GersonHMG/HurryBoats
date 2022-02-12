using Godot;
using System;

public class Box : Area2D{

    bool is_full = true;
    protected string content = "DoubleCannon";
    protected string content_type = "weapon";
    BoxPM boxPM;

    public override void _Ready(){
        base._Ready();
        boxPM = GetNode<BoxPM>("BoxPM");
    }
    

    public string GetContent() => content;


    public string GetContentType() => content_type;


    public bool IsFull() => is_full;


    public void DestroyCrate(){
        //boxPM.SendDestroyCrate();
        is_full = false;
        QueueFree();
    }


    public void SyncDestroyCrate(){
        is_full = false;
        QueueFree();
    }

}