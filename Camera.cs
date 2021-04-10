using Godot;
using System;
using System.Collections.Generic;

public class Camera : Godot.Camera
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
        if(Input.IsActionPressed("ui_left"))this.Translate(Vector3.Left*0.1f);
        if(Input.IsActionPressed("ui_right"))this.Translate(Vector3.Right*0.1f);
        if(Input.IsActionPressed("ui_up"))this.Translate(Vector3.Up*0.1f);
        if(Input.IsActionPressed("ui_down"))this.Translate(Vector3.Down*0.1f);                

    }
}