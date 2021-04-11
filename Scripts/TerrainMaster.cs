using Godot;
using System;

public class TerrainMaster : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    Submarine sub;
    Reference chunkScript;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sub = GetNode("/root/Spatial/Sub") as Submarine;
        chunkScript = ResourceLoader.Load("res://Terrain/Chunk.cs");
        //CreateChunk(0,0);
        //CreateChunk(0,1);
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                CreateChunk(i,0,j);
            }
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public void CreateChunk(int x, int y,int z)
    {
        MeshInstance newChunk = new MeshInstance();
        var id = newChunk.GetInstanceId();
        newChunk.SetScript(chunkScript);
        Chunk chunk = GD.InstanceFromId(id) as Chunk;
        chunk.ConstructChunk(16,x,y,z);
        AddChild(chunk);
    }
}
