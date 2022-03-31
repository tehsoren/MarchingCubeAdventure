using Godot;
using System;
using System.Collections.Generic;
//using Godot.Collections;
using System.Linq;

public class Chunk : MeshInstance
{
	float[,,] vals;

	ITerrainGenerator terrainGen;
    FloraController floraController;
	int faunaCount = 1;
	public int x,y,z;
	public override void _Ready()
	{
        

	}

	public void SetGenerator(ITerrainGenerator terrainGen,FloraController fc)
	{
		this.terrainGen = terrainGen;
        this.floraController = fc;
	}

	private void GenerateChunkValues(int size,float xOffset,float yOffset,float zOffset)
	{
		vals = new float[size+1,size+1,size+1];
		for (int x = 0; x < size+1; x++)
		{
			for (int y = 0; y < size+1; y++)
			{
				for (int z = 0; z < size+1; z++)
				{    
					vals[x,y,z] = terrainGen.GetValue(x+xOffset,y+yOffset,z+zOffset);
				}
			}    
		}
	}

	public void ConstructChunk(int size,int chunkX,int chunkY,int chunkZ)
	{
		
		var xOffset = size*chunkX;
		var yOffset = size*chunkY;
		var zOffset = size*chunkZ;
		GenerateChunkValues(size,xOffset,yOffset,zOffset);
		Vector3[] vList;
		int[] tList;
		Vector3[] nList;
		ConstructMesh(size,xOffset,yOffset,zOffset,out vList,out tList,out nList);
		ConstructArrayMesh(vList,tList,nList);

		CreateFlora(vList,nList);
	}

	private void ConstructArrayMesh(Vector3[] vertices, int[] triangles, Vector3[] normals)
	{
		var mesht = new ArrayMesh();
		object[] arr = new object[(int)Mesh.ArrayType.Max];
		arr[(int)Mesh.ArrayType.Vertex] = vertices;
		arr[(int)Mesh.ArrayType.Index] = triangles;
		arr[(int)Mesh.ArrayType.Normal] = normals;

		mesht.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles,new Godot.Collections.Array(arr));
		this.Mesh = mesht;
		this.Mesh.SurfaceSetMaterial(0,ResourceLoader.Load("Terrain/terrain.tres") as Material);
	}

	private void ConstructMesh(int size,int xOffset,int yOffset,int zOffset,out Vector3[] vertices, out int[] triangles,out Vector3[] vertexNormals)
	{
		List<Vector3> vList = new List<Vector3>();
		List<int> tList = new List<int>();
		

		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				for (int z = 0; z < size; z++)
				{
					var t = GetVals(x,y,z);
					MarchingHelp.March(t,new Vector3(x+xOffset,y+yOffset,z+zOffset),0.0f,ref tList,ref vList);
				}
			}
		}

		

		vertices = vList.ToArray();
		//vertices = vertices.Select(x => x*terrainGen.scale).ToArray(); // scale mesh size
		triangles = tList.ToArray();
		GenerateNormals(vertices,triangles,out vertexNormals);

	}

	private void GenerateNormals(Vector3[] vertices,int[] triangles, out Vector3[] vertexNormals)
	{
		List<Vector3> faceNormals = new List<Vector3>();
		vertexNormals = new Vector3[vertices.Length];
		for (int i = 0; i < triangles.Length; i+=3)
		{
			var v0 = vertices[triangles[i]];
			var v1 = vertices[triangles[i+1]];
			var v2 = vertices[triangles[i+2]];
			var u = v1-v0;
			var v = v2-v0;
			Vector3 fn = new Vector3();
			fn.x = u.y*v.z - u.z*v.y;
			fn.y = u.x*v.z - u.z*v.x;
			fn.z = u.x*v.y - u.y*v.x;
			//faceNormals.Add(fn);
			if(vertexNormals[triangles[i]] == null) vertexNormals[triangles[i]] = new Vector3();
			if(vertexNormals[triangles[i+1]] == null) vertexNormals[triangles[i+1]] = new Vector3();
			if(vertexNormals[triangles[i+2]] == null) vertexNormals[triangles[i+2]] = new Vector3();
			vertexNormals[triangles[i]] += fn;
			vertexNormals[triangles[i+1]] += fn;
			vertexNormals[triangles[i+2]] += fn;
			
		}
		for (int i = 0; i < vertices.Length; i++)
		{
			vertexNormals[i] = vertexNormals[i].Normalized();
		}

	}

	private float[] GetVals(int x,int y,int z)
	{
		List<float> t = new List<float>();
		t.Add(vals[x  ,y  ,z]);
		t.Add(vals[x+1,y+0,z+0]);
		t.Add(vals[x+1,y+1,z+0]);
		t.Add(vals[x+0,y+1,z+0]);
		t.Add(vals[x+0,y+0,z+1]);
		t.Add(vals[x+1,y+0,z+1]);
		t.Add(vals[x+1,y+1,z+1]);
		t.Add(vals[x+0,y+1,z+1]);
		return t.ToArray();
	}

	private void CreateFlora(Vector3[] vertices,Vector3[] normals)
	{
		//find points to create fauna at - vertices is simple, use normals to only select those not on a side
		List<int> floraPoints = new List<int>();
		var r = new RandomNumberGenerator();
		
		for (int i = 0; i < faunaCount; i++)
		{
		   var index = r.RandiRange(0,vertices.Length -1);
		   if(!floraPoints.Contains(index))
		   {
				floraPoints.Add(index);
		   }
		}

        var floraLists = GetNewFloraMultiMeshPositions(floraPoints,vertices,normals);
        //generate multimeshes
        foreach (var i in floraLists.Keys)
        {
            
            MultiMeshInstance newMMI = CreateNewFloraMultiMesh(i,floraLists[i],vertices,normals);
            AddChild(newMMI);
        }
	}

    
    private Dictionary<int,List<int>> GetNewFloraMultiMeshPositions(List<int> floraPoints,Vector3[] vertices,Vector3[] normals)
    {
        Dictionary<int,List<int>> floraLists = new Dictionary<int, List<int>>(); // flora id, points id<
        var rng = new RandomNumberGenerator();
        rng.Randomize();
        //populate list of flora with their locations
		foreach (int i in floraPoints)
		{
            var possibleFlora = floraController.GetPossibleFlora(normals[i].y);
            if(possibleFlora.Count >0)
            {
                var index = rng.RandiRange(0,possibleFlora.Count-1);
                if(!floraLists.ContainsKey(index))
                {
                    floraLists[index] = new List<int>();
                }
                    floraLists[index].Add(i);
            }
        }

        return floraLists;
    }
    
	private MultiMeshInstance CreateNewFloraMultiMesh(int floraIndex,List<int> indexes,Vector3[] vertices,Vector3[] normals)
    {

        var mm = new MultiMesh();
        mm.Mesh = floraController.GetFloraMesh(floraIndex);
        mm.TransformFormat = MultiMesh.TransformFormatEnum.Transform3d;
        mm.InstanceCount = indexes.Count;
        int ii = 0;
        foreach (int i in indexes)
        {

			var t = Transform.Identity;
			t = t.Translated(vertices[i]);
			var tar = vertices[i]+normals[i];
			var rbasis = t.LookingAt(tar,Vector3.Up).basis;
        	t.basis= rbasis;
			
            mm.SetInstanceTransform(ii,t);
            ii++;
        }

        MultiMeshInstance mmi = new MultiMeshInstance();
        mmi.Multimesh = mm;
        return mmi;

    }

}
