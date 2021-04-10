using Godot;
using System;

public class Submarine : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    float speed = 20f;
    
    Vector3 velocity;
    
    float curSpeed = 1;

    private float rotX = 0f;
    private float rotY = 0f;
    private float rotSpeed = 0.1f;
    Spatial guide;
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        velocity = new Vector3();
        guide = GetNode("guide") as Spatial;
        

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var aim = guide.Transform.basis;
        float dir = 0;
        if(Input.IsActionPressed("ui_up"))
            dir = 1;
        if(Input.IsActionPressed("ui_down"))
            dir = -1;
        Translate(-aim.z*dir*speed*delta);
        var vec = -aim.z;//*dir;
        //Transform.Translated(vec*5);
        Transform = Transform.Orthonormalized();

        if(Input.IsActionJustPressed("ui_cancel"))
        {
            if(Input.GetMouseMode() == Input.MouseMode.Visible)
            {
                Input.SetMouseMode(Input.MouseMode.Captured);
            }
            else
            {
                Input.SetMouseMode(Input.MouseMode.Visible);
            }
            
        }

        
    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseMotion mouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
        {
            rotX += (mouseMotion.Relative.x)*0.01f*-1f*rotSpeed;
            rotY += (mouseMotion.Relative.y)*0.01f*rotSpeed;

            Transform transform = Transform;
            transform.basis = Basis.Identity;
            Transform = transform;
            RotateObjectLocal(Vector3.Up,rotX);
            RotateObjectLocal(Vector3.Right,rotY);

        }
    }
}
