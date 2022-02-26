using Godot;
using System;
using System.Collections.Generic;

public class MapChunk : Node2D{
    
    const int GRID_SIZE_X = 18;
    public const int GRID_SIZE_Y= 10;
    public const int HEXAGON_SIZE = 67;
    // The sum of max density must not be more than 100%
    const float MAX_BOX_DENSITY = 0.3f;
    const float MAX_LAND_DENSITY = 0.35f;
    const float MIN_BOX_DENSITY = 0.1f;
    const float MIN_LAND_DENSITY = 0.10f;

    float hexagon_horizontal_separation;
    float hexagon_vertical_separation;
    float CHUNK_SIZE = 0;

    PackedScene LAND_PIECE;
    PackedScene CANNON_BOX;
    PackedScene SEA_ZERO;
    PackedScene SEA_ONE;

    List<Vector2> boxes_positions = new List<Vector2>();
    List<Vector2> lands_positions = new List<Vector2>();

    public override void _Ready(){
        LAND_PIECE = ResourceLoader.Load<PackedScene>("res://scenes/map/LandPiece.tscn");
        SEA_ZERO = ResourceLoader.Load<PackedScene>("res://scenes/map/SeaZero.tscn");
        SEA_ONE = ResourceLoader.Load<PackedScene>("res://scenes/map/SeaOne.tscn");
        CANNON_BOX = ResourceLoader.Load<PackedScene>("res://scenes/boxes/Box.tscn");
        InitMeasurements();
    }


    void InitMeasurements(){
        hexagon_horizontal_separation = Mathf.Sqrt(3)*HEXAGON_SIZE; // sin(60) * size
        float height = HEXAGON_SIZE*2.0f;
        hexagon_vertical_separation = (height*3.0f)/4.0f;  //h*3/4
        CHUNK_SIZE = GRID_SIZE_Y*hexagon_vertical_separation;
    }


    public void GenerateRandomChunk(int seed){
        GD.Print("random chunk seed is: " + seed.ToString() );
        Random rnd = new Random(seed);
        List<Vector2> grid = GetGrid();
        int grid_size = GRID_SIZE_X*GRID_SIZE_Y;
        int min_lands = Mathf.FloorToInt( MIN_LAND_DENSITY*grid_size);
        int max_lands = Mathf.FloorToInt(MAX_LAND_DENSITY*grid_size);
        for(int i = 0; i < rnd.Next(min_lands, max_lands); i++){
            int index = rnd.Next( 0, grid.Count );
            lands_positions.Add( ConvertGridPosToLocal( grid[index]) );
            grid.RemoveAt(index);
        }
        GenerateLandPieces(lands_positions);
        int min_boxes = Mathf.FloorToInt(MIN_BOX_DENSITY*grid_size);
        int max_boxes = Mathf.FloorToInt(MAX_BOX_DENSITY*grid_size);
        for(int i = 0; i < rnd.Next(min_boxes, max_boxes); i++){
            int index = rnd.Next( 0, grid.Count );
            boxes_positions.Add( ConvertGridPosToLocal( grid[index]) );
            grid.RemoveAt(index);
        }
        GenerateBoxes(boxes_positions);
        GenerateSeaDecorations(grid);
        
    }


    public void GenerateBottomWall(){
        List<Vector2> grid = GetGrid();
        foreach(Vector2 grid_pos in GetGrid()){
            if(grid_pos.y > GRID_SIZE_Y - 4){
                SpawnLandPiece( ConvertGridPosToLocal(grid_pos ) ); 
                grid.Remove(grid_pos);
            }
        }
        GenerateSeaDecorations(grid);
    }


    void GenerateSeaDecorations(List<Vector2> grid){
        Random rnd = new Random();
        foreach(Vector2 grid_pos in grid){
            int number = rnd.Next(0,10);
            if(number > 8 ){
                Sprite new_sea = SEA_ONE.Instance<Sprite>();
                AddChild(new_sea);
                new_sea.Position = ConvertGridPosToLocal(grid_pos);
            }
            else if(number < 4){
                Sprite new_sea = SEA_ZERO.Instance<Sprite>();
                AddChild(new_sea);
                new_sea.Position = ConvertGridPosToLocal(grid_pos);
            }
        }
    }


    List<Vector2> GetGrid(){
        List<Vector2> grid = new List<Vector2>();
        for(int x = 0; x < GRID_SIZE_X; x++){
            for(int y = 0; y < GRID_SIZE_Y; y++){
                grid.Add(new Vector2(x,y));
            }
        }
        return grid;
    }


    public List<Box> GenerateBoxes(List<Vector2> positions){
        List<Box> boxes = new List<Box>();
        foreach(Vector2 place in positions){
            boxes.Add( SpawnBox(place) );
        }
        return boxes;
    }


    Box SpawnBox(Vector2 global_coordinates){
        Box new_box = CANNON_BOX.Instance<Box>();
        CallDeferred("add_child", new_box);
        new_box.GlobalPosition = global_coordinates;
        return new_box;
    }


    public List<LandPiece> GenerateLandPieces(List<Vector2> positions){
        List<LandPiece> lands = new List<LandPiece>();
        foreach(Vector2 place in positions){
            lands.Add( SpawnLandPiece(place ) );
        }
        return lands;
    }
    

    LandPiece SpawnLandPiece(Vector2 global_coordinates){
        LandPiece new_land_piece = LAND_PIECE.Instance<LandPiece>();
        CallDeferred("add_child", new_land_piece);
        new_land_piece.GlobalPosition = global_coordinates;
        return new_land_piece;
    }


    Vector2 ConvertGridPosToLocal(Vector2 local_coordinates){
        float x_coord = 0.0f;
        if(( ( (int) local_coordinates.y )% 2) == 0){
            x_coord = local_coordinates.x*hexagon_horizontal_separation;
        }
        else{
            x_coord = local_coordinates.x*(hexagon_horizontal_separation) + (hexagon_horizontal_separation/2);
        }
        float y_coord = local_coordinates.y*(hexagon_vertical_separation);
        return new Vector2(x_coord, y_coord);
    }


    // For debug purposes
    void FillHexagons(){
        List<Vector2> grid = new List<Vector2>();
        for(int x = 0; x < GRID_SIZE_X; x++){
            for(int y = 0; y < GRID_SIZE_Y; y++){
                grid.Add(new Vector2(x,y));
            }
        }
        foreach(Vector2 place in grid){
            SpawnLandPiece(ConvertGridPosToLocal(place));
        }
    }

    public List<Vector2> GetBoxesPositions() => boxes_positions;

    public List<Vector2> GetLandsPositions() => lands_positions;

    public float GetChunkSize() => CHUNK_SIZE;

}
