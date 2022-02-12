using Godot;
using System;

public class CurtainTransition : Control{


    public void StartTransition(){
        Visible = true;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Close");
    }


    public void EndTransition(){
        GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("Close");
    }   

}
