using Godot;
using System;

public partial class Book : RigidBody3D, IPoolable {
    public bool IsOnShelf;
    public bool WasMoving;
    public bool IsCarried;
    private int givenScore;
    private int scoreValue;
    private bool IsJustSpawned;
    private Vector3 lastVelocity;
    public void OnSpawn() {
        IsJustSpawned = true;
    }

    public override void _Ready() {
        var rand = new Random();
        BodyEntered += OnCollision;
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
        var scoreSize = shape.Size * Consts.BookScoreSizeMul;
        scoreValue = Mathf.RoundToInt(scoreSize.X * scoreSize.Y * scoreSize.Z);
        //GD.Print("Size ", shape.Size, " AABB: ", mesh.GetAabb().Size);
        colShape.Shape = shape;
    }

    private void OnCollision(Node body) {
        if (IsJustSpawned) {
            IsJustSpawned = false;
            return;
        }
        if (IsCarried) { return; }
        if (lastVelocity.LengthSquared() > Consts.BookHitSoundMinVelocity) {
            Runtime.Audio.PlayBookDropAt(GlobalPosition);
        }
    }


    public override void _PhysicsProcess(double delta) {
        lastVelocity = LinearVelocity;
        var velMag = lastVelocity.LengthSquared();
        var isMoving = false;
        if (velMag > Consts.BookStillnessThreshold) {
            WasMoving = true;
            isMoving = true;
        }
        if (AngularVelocity.LengthSquared() > Consts.BookStillnessThreshold) {
            WasMoving = true;
            isMoving = true;
        }
        if (IsCarried) { return; }
        if (isMoving || !WasMoving) { return; }
        // settled!
        if (IsOnShelf) {
            AddScore();
        } else {
            RemoveScore();
        }

        WasMoving = false;
    }

    private void AddScore() {
        if (givenScore > 0) { return; }
        givenScore = scoreValue;
        Runtime.Score.Add(givenScore);
        Runtime.Audio.PlayAddBookAt();
    }

    private void RemoveScore() {
        if (givenScore == 0) { return; }
        Runtime.Score.Add(-givenScore);
        Runtime.Audio.PlayLoseBookAt();
        givenScore = 0;
    }


    public void Carry() {
        Set("mass", Consts.BookCarryMass);
        Set("linear_damp", 5f);
        Set("angular_damp", 5f);
        IsCarried = true;
        RemoveScore();
        WasMoving = true;
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
        GD.Print("Book now on shelf: ", isOnShelf);
        IsOnShelf = isOnShelf;
    }
}
