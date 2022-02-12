using Godot;
using System;
using System.Collections.Generic;

public class MapStructures : Node2D{

    int CHUNK_SIZE;
    PackedScene MAP_CHUNK;
    ushort current_chunk = 0;
    Camera2D camera;
    Random map_seed;
    MapStructuresPM mapStructuresPM;

    List<MapChunk> chunks = new List<MapChunk>();


    public override void _Ready(){
        MAP_CHUNK = ResourceLoader.Load<PackedScene>("res://scenes/map/MapChunk.tscn");
        camera = GetNode<Camera2D>("../HurryBoatsCamera");
        mapStructuresPM = GetNode<MapStructuresPM>("MapStructuresPM");
    }


    public override void _PhysicsProcess(float delta){
        CheckChunkPassed();
    }


    public void InitMap(int seed){
        CreateFirstChunk(current_chunk);
        current_chunk += 1;
        map_seed = new Random(seed);
        CreateNewRandomChunk(map_seed.Next(), current_chunk);
        current_chunk += 1;
        CreateNewRandomChunk(map_seed.Next(), current_chunk);
        current_chunk += 1;
    }


    void CheckChunkPassed(){
        if(chunks.Count > 0){
            if(camera.GlobalPosition.DistanceTo( chunks[0].GlobalPosition) > 1500 ){
                int seed = map_seed.Next();
                CreateNewRandomChunk(seed, current_chunk);
                current_chunk += 1;
                DeleteFirstChunk();
            }
        }
    }

    public void DeleteFirstChunk(){
        chunks[0].QueueFree();
        chunks.RemoveAt(0);
    }


    public void Reset(){
        current_chunk = 0;
        foreach(MapChunk chunk in chunks){
            chunk.QueueFree();
        }
        chunks.Clear();
    }
    

    async public void CreateNewRandomChunk(int chunk_seed, int chunk_number){
        MapChunk new_chunk = MAP_CHUNK.Instance<MapChunk>();
        CallDeferred("add_child", new_chunk);
        await ToSignal(new_chunk, "ready");
        new_chunk.GlobalPosition = chunk_number*new_chunk.GetChunkSize()*Vector2.Up;
        new_chunk.GenerateRandomChunk(chunk_seed);
        chunks.Add(new_chunk); 
    }


    async void CreateFirstChunk(int chunk_number){
        MapChunk new_chunk = MAP_CHUNK.Instance<MapChunk>();
        CallDeferred("add_child", new_chunk);
        await ToSignal(new_chunk, "ready");
        new_chunk.GlobalPosition = chunk_number*new_chunk.GetChunkSize()*Vector2.Up;
        new_chunk.GenerateBottomWall();
        chunks.Add(new_chunk); 
    }
    
}
