using Godot;
using System;

public partial class Chest : Area3D {
    public override void _Ready() {
        BodyEntered += OnBodyEnter;
    }

    public override void _Process(double delta) {
    }

    public void OnBodyEnter(Node body) {
        GD.Print("wow {0}", body);
    }
}
