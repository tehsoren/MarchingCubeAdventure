using System.Collections;
using System.Collections.Generic;
using Godot;

public interface IFlora {


    bool IsValidSlope(float slope);
    Mesh GetMesh();

}