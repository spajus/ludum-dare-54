using System;
using Godot;

public partial class Room : Node3D {
    public override void _Ready() {
    }

    public void SpawnBooks(int count) {
        var spawnShape = GetNode<CollisionShape3D>("BookSpawnArea/Shape");
        for (int i = 0; i < count; i++) {
            SpawnBook(spawnShape);
        }
    }

    private void SpawnBook(CollisionShape3D spawnShape) {
        var box = (BoxShape3D) spawnShape.Shape;
        var extents = box.Size * 0.5f;
        var bookPos = new Vector3(
                Rng.NextFloat(-extents.X, extents.X),
                Rng.NextFloat(-extents.Y, extents.Y),
                Rng.NextFloat(-extents.Z, extents.Z)
            ) * spawnShape.GlobalTransform.Basis
                + spawnShape.GlobalTransform.Origin;
        var book = Runtime.BookPool.Spawn(Runtime.Root);
        book.GlobalPosition = bookPos;
        book.GlobalRotation = new Vector3(
                Rng.NextFloat() * 180f,
                Rng.NextFloat() * 180f,
                Rng.NextFloat() * 180f);
    }
}