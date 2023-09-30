using Godot;
using System;

public partial class Book : RigidBody3D {
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        var rand = new Random();
        var bookMeshes = GetNode<Node3D>("book");
        var childCount = bookMeshes.GetChildCount();
        var choice = Mathf.FloorToInt(childCount * rand.NextSingle());
        MeshInstance3D chosen = null;
        for (int i = 0; i < childCount; i++) {
            var child = bookMeshes.GetChild<MeshInstance3D>(i);
            if (i != choice) {
                child.Hide();
            } else {
                chosen = child;
            }
        }

        GD.Print("Children: ", childCount);
        GD.Print("Choice: ", choice);
        var mesh = chosen;
        var mat = mesh.GetActiveMaterial(0);
        var dupeMat = (Material) mat.Duplicate();
        dupeMat.Set("albedo_color", new Color(
            rand.NextSingle(),
            rand.NextSingle(),
            rand.NextSingle()));
        mesh.SetSurfaceOverrideMaterial(0, dupeMat);

        var colShape = GetNode<CollisionShape3D>("CollisionShape3D");
        var shape = (BoxShape3D) colShape.Shape.Duplicate();
        shape.Size = mesh.GetAabb().Size * chosen.Scale;
        //GD.Print("Size ", shape.Size, " AABB: ", mesh.GetAabb().Size);
        colShape.Shape = shape;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }
}
