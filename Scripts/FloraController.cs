using Godot;
using System;
using System.Collections.Generic;

public class FloraController
{
    List<Flora> floras;
    List<Mesh> meshes;


    public FloraController()
    {
        floras = new List<Flora>();
        meshes = new List<Mesh>();

        AddNewFlora("hej",90,-90);
        AddNewFlora("b",90,-90);
    }


    private void AddNewFlora(string floraName, float maxAngle,float minAngle)
    {
        Flora newFlora = new Flora(floraName,maxAngle,minAngle);
        floras.Add(newFlora);
        meshes.Add(null);
    }

    public List<int> GetPossibleFlora(float normalHeight)
    {
        List<int> possibleFlora = new List<int>();

        for (int i = 0; i < floras.Count; i++)
        {
            if(normalHeight >= floras[i].minAngleSin && normalHeight <= floras[i].maxAngleSin)
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


    private struct Flora
    {
        public string modelName;
        public float maxAngleSin;
        public float minAngleSin;

        public Flora(string modelName, float maxAngle, float minAngle)
        {
            this.modelName = modelName;
            this.maxAngleSin = Mathf.Sin(Mathf.Deg2Rad(maxAngle));
            this.minAngleSin = Mathf.Sin(Mathf.Deg2Rad(minAngle));
        }
    }


}

