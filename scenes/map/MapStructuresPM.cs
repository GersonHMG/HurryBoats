using Godot;
using System;

public class MapStructuresPM : Node{

    MapStructures parent;

    public override void _Ready(){
        base._Ready();
        parent = GetParent<MapStructures>();
    }


    


}
