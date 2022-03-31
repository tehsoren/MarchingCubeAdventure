using Godot;
using System;
using System.Collections.Generic;
public class TerrainMaster : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    Submarine sub;
    Reference chunkScript;
    Dictionary<Vector3,Chunk> chunks;

    TerrainGenerator terrainGen;

    int chunkSize = 16;
    int radius = 2;
    int graceRadius = 5;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sub = GetNode("/root/Spatial/Sub") as Submarine;
        chunkScript = ResourceLoader.Load("res://Terrain/Chunk.cs");

        chunks = new Dictionary<Vector3, Chunk>();

        terrainGen = new TerrainGenerator();
        terrainGen.init();

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        
        SpawnChunks();
        CleanChunks();
    }

    public void SpawnChunks()
    {
        var k = false;
        var subChunkPos = (sub.Transform.origin / chunkSize).Floor();
        var sx = (int)subChunkPos.x;
        var sz = (int)subChunkPos.z;
        for (int x = sx-radius; x < sx+radius+1; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = sz-radius; z < sz+radius+1; z++)
                {
                    if(chunks.ContainsKey(new Vector3(x,y,z) ))
                    {
                       continue;
                    }
                    else
                    {
                        CreateChunk(x,y,z);
                        k = true;
                        break;
                    }      
                }
                if(k)
                    break;
            }
        }
    }

    public void CleanChunks()
    {
        var subChunkPos = (sub.Transform.origin / chunkSize);
        List<Vector3> toRelease = new List<Vector3>();
        foreach (Vector3 pos in chunks.Keys)
        {
            var t = (pos - subChunkPos);
            t.y = 0;
            if(t.Length() > graceRadius)
            {
                toRelease.Add(pos);
            }
        }

        foreach (var key in toRelease)
        {
            chunks[key].QueueFree();
            chunks.Remove(key);
        }

    }

    public void CreateChunk(int x, int y,int z)
    {
        MeshInstance newChunk = new MeshInstance();
        var id = newChunk.GetInstanceId();
        newChunk.SetScript(chunkScript);
        Chunk chunk = GD.InstanceFromId(id) as Chunk;
        chunk.SetGenerator(terrainGen, new FloraController());
        chunk.ConstructChunk(chunkSize,x,y,z);
        AddChild(chunk);
        chunks.Add(new Vector3(x,y,z),chunk);
    }
}
