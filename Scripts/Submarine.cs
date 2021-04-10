using Godot;
using System;

public class Submarine : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        
        var aim = GlobalTransform.basis;
        if(Input.IsActionPressed("ui_up"))
            Translate(aim.z*5*delta);
        if(Input.IsActionPressed("spc"))
            Rotate(-aim.x,0.5f*delta);
        if(Input.IsActionPressed("ctrl"))
            Rotate(aim.x,0.5f*delta);
        if(Input.IsActionPressed("ui_left"))
            Rotate(new Vector3(0,1,0),0.5f*delta);
        if(Input.IsActionPressed("ui_right"))
            Rotate(new Vector3(0,-1,0),0.5f*delta);
        
    }
}
