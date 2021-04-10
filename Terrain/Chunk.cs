using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public class Chunk : MeshInstance
{
    float[,,] vals;

    public override void _Ready()
    {
        OpenSimplexNoise osn = new OpenSimplexNoise();
        osn.Period = 16;
        Vector3[] vList;
        int[] tList;
        Vector3[] nList;
        vals = new float[30,30,30];
        
        for (int x = 1; x < 28; x++)
        {
            for (int y = 1; y < 28; y++)
            {
                for (int z = 1; z < 28; z++)
                {
                    vals[x,y,z] = osn.GetNoise3d(x,y,z);      
                }
            }
            
        }
        ConstructMesh(out vList,out tList,out nList);
        ConstructArrayMesh(vList,tList,nList);

    }


    public void ConstructArrayMesh(Vector3[] vertices, int[] triangles, Vector3[] normals)
    {
        var mesht = new ArrayMesh();
        object[] arr = new object[(int)Mesh.ArrayType.Max];
        arr[(int)Mesh.ArrayType.Vertex] = vertices;
        arr[(int)Mesh.ArrayType.Index] = triangles;
        arr[(int)Mesh.ArrayType.Normal] = normals;

        mesht.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles,new Godot.Collections.Array(arr));
        this.Mesh = mesht;
        this.Mesh.SurfaceSetMaterial(0,ResourceLoader.Load("terrain.tres") as Material);
    }

    public void ConstructMesh(out Vector3[] vertices, out int[] triangles,out Vector3[] vertexNormals)
    {
        List<Vector3> vList = new List<Vector3>();
        List<int> tList = new List<int>();
        

        for (int x = 0; x < 29; x++)
        {
            for (int y = 0; y < 29; y++)
            {
                for (int z = 0; z < 29; z++)
                {
                    var t = GetVals(x,y,z);
                    MarchingHelp.March(t,new Vector3(x,y,z),0.0f,ref tList,ref vList);
                }
            }
            
        }



        vertices = vList.ToArray();
        triangles = tList.ToArray();
        GenerateNormals(vertices,triangles,out vertexNormals);

    }

    public void GenerateNormals(Vector3[] vertices,int[] triangles, out Vector3[] vertexNormals)
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


}