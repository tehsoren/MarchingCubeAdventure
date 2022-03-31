using Godot;
using System;
using System.Collections.Generic;

public class FloraController
{
    List<IFlora> floras;
    List<Mesh> meshes;

    public FloraController()
    {
        floras = new List<IFlora>();
        meshes = new List<Mesh>();
        TempAddNewFloras();
    }

    private void TempAddNewFloras()
    {
        IFlora newFlora = new ExampleFlora(90,-90);
        AddNewFlora(newFlora);
        newFlora = new ExampleFloraCapsule(90,-90);
        AddNewFlora(newFlora);
    }
    
    private void AddNewFlora(IFlora flora)
    {
        floras.Add(flora);
        meshes.Add(flora.GetMesh());
    }

    public List<int> GetPossibleFlora(float normalHeight)
    {
        List<int> possibleFlora = new List<int>();

        for (int i = 0; i < floras.Count; i++)
        {
            if(floras[i].IsValidSlope(normalHeight))
            {
                possibleFlora.Add(i);
            }
        }

        return possibleFlora;
    }

    public Mesh GetFloraMesh(int id)
    {
        var m = meshes[id];
        if(m == null)
        {
            return new CubeMesh();
        }
        else
        {
            return m;
        }

    }

}

