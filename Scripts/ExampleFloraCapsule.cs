using System.Collections;
using System.Collections.Generic;
using Godot;

public class ExampleFloraCapsule : IFlora {

    private float maxAngleSin;
    private float minAngleSin;
    public ExampleFloraCapsule(float maxAngle,float minAngle)
    {
        this.maxAngleSin = Mathf.Sin(Mathf.Deg2Rad(maxAngle));
        this.minAngleSin = Mathf.Sin(Mathf.Deg2Rad(minAngle));
    }
    public bool IsValidSlope(float slope)
    {
        return slope >= minAngleSin && slope <= maxAngleSin;
    }
    public Mesh GetMesh()
    {
        return new CapsuleMesh();
    }

}