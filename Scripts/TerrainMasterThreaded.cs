using Godot;
using System;
using System.Threading;  
using System.Collections.Generic;
using System.Collections.Concurrent;
public class TerrainMasterThreaded : Node
{
    Submarine sub;
    Reference chunkScript;
    ConcurrentDictionary<Vector3,Chunk> chunks;

    System.Threading.Thread thread;
    ITerrainGenerator terrainGen;
    FloraController floraController;
    int chunkSize = 16;
    int radius = 2;
    int graceRadius = 5;

    float destroyRadius = 8f;
    float hideRadius    = 6f;
    float createRadius  = 5f;
    float hideFloraRadius = 5f;
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sub = GetNode("/root/Spatial/Sub") as Submarine;
        chunkScript = ResourceLoader.Load("res://Terrain/Chunk.cs");

        chunks = new ConcurrentDictionary<Vector3, Chunk>();
    
        TerrainGenerator newTerrainGenerator = new TerrainGenerator();
        newTerrainGenerator.init();
        terrainGen = newTerrainGenerator;

        floraController = new FloraController();


        StartChunkGenThread();

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

        CleanChunks();   
        
    }

    private void StartChunkGenThread()
    {
        thread = new System.Threading.Thread(ChunkGenThreadFunc);
        thread.Start(this);
    }


    public void ChunkGenThreadFunc(object data)
    {
        var tm = (TerrainMasterThreaded)data;
        while (true)
        {
            var newChunk = CreateNewChunk();
        }

    }

    public Chunk CreateNewChunk()
    {
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
                        return CreateChunk(x,y,z);
                    }      
                }
            }
        }
        return null;
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
            else
            {
                var chunkDir = (pos+new Vector3(0.5f,0.5f,0.5f))-subChunkPos;
            }
        }

        foreach (var key in toRelease)
        {
            Chunk toRemove;
            if(chunks.TryRemove(key, out toRemove))
            {
              toRemove.QueueFree();  
            }
        }

    }

    public Chunk CreateChunk(int x, int y,int z)
    {
        MeshInstance newChunk = new MeshInstance();
        var id = newChunk.GetInstanceId();
        newChunk.SetScript(chunkScript);
        Chunk chunk = GD.InstanceFromId(id) as Chunk;

        chunk.SetGenerator(terrainGen,floraController);
        chunk.ConstructChunk(chunkSize,x,y,z);
        chunk.x = x;
        chunk.y = y;
        chunk.z = z;
        chunk.Name = x +", "+y+", "+z;
        AddChild(chunk);
        chunks.TryAdd(new Vector3(x,y,z),chunk);
        return chunk;
    }



}