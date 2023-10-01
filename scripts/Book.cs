using Godot;
using System;

public partial class Book : RigidBody3D {
    private const float scoreSizeMul = 20f;
    private const float stillnessThreshold = 0.001f;
    private const float bookCarryMass = 0.5f;
    public bool IsOnShelf;
    public bool WasMoving;
    public bool IsCarried;
    private int givenScore;
    private int scoreValue;
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
        var scoreSize = shape.Size * scoreSizeMul;
        scoreValue = Mathf.RoundToInt(scoreSize.X * scoreSize.Y * scoreSize.Z);
        //GD.Print("Size ", shape.Size, " AABB: ", mesh.GetAabb().Size);
        colShape.Shape = shape;
    }

    public override void _PhysicsProcess(double delta) {
        if (LinearVelocity.LengthSquared() > stillnessThreshold) {
            WasMoving = true;
        }
        if (AngularVelocity.LengthSquared() < stillnessThreshold) {
            WasMoving = true;
        }
        if (IsCarried) { return; }
        if (!WasMoving) { return; }
        // settled!
        if (IsOnShelf) {
            if (givenScore == 0) {
                givenScore = scoreValue;
                Runtime.Score.Add(givenScore);
            }
        } else {
            if (givenScore > 0) {
                Runtime.Score.Add(-givenScore);
                givenScore = 0;
            }
        }
        WasMoving = false;
    }

    public void Carry() {
        Set("mass", bookCarryMass);
        Set("linear_damp", 5f);
        Set("angular_damp", 5f);
        if (givenScore > 0) {
            Runtime.Score.Add(-givenScore);
        }
        WasMoving = true;
        IsCarried = true;
    }

    public void Drop() {
        Set("mass", 1f);
        Set("linear_damp", 0f);
        Set("angular_damp", 0f);
        WasMoving = true;
        IsCarried = false;
    }

    public void SetOnShelf(bool isOnShelf) {
        if (isOnShelf == IsOnShelf) {
            GD.PrintErr("Trying to change Book.IsOnShelf to the same value! ",
                isOnShelf);
        }
        IsOnShelf = isOnShelf;
    }
}
