using Godot;
using System;

public class BoatEffectsManager : Node2D
{
    Node2D effect_folder;
    PackedScene EXPLOSION_SCENE;

    public override void _Ready(){
        effect_folder = GetParent().GetParent<Node2D>();
        EXPLOSION_SCENE = ResourceLoader.Load<PackedScene>("res://scenes/effects/BoatExplosion.tscn");
    }

    public void Explosion(){
        Sprite new_explosion = EXPLOSION_SCENE.Instance<Sprite>();
        effect_folder.AddChild(new_explosion);
        new_explosion.GlobalPosition = GlobalPosition;
    }



}
