using System;
using Godot;

public partial class Bookshelf : Node3D {
    private Area3D bookArea;
    public override void _Ready() {
        bookArea = GetNode<Area3D>("BookArea");
        bookArea.BodyEntered += OnBookEnter;
        bookArea.BodyExited += OnBookExit;
    }

    private void OnBookExit(Node3D node) {
        if (node is Book book) {
            book.SetOnShelf(false);
        }
    }

    private void OnBookEnter(Node3D node) {
        if (node is Book book) {
            book.SetOnShelf(true);
        }
    }
}