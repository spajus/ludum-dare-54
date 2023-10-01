using Godot;
using System;

public partial class Game : Node {
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        var room = Utils.AddSceneTo<Room>(this, Scenes.Room);
        Runtime.Initialize(this, room);
        room.SpawnBooks(10);
        Utils.AddSceneTo<Node3D>(this, Scenes.Player);
        Utils.AddSceneTo<Node3D>(this, Scenes.Bookshelf, new Vector3(0, 0, -2.25f));
        //Utils.AddSceneTo(this, Scenes.Chest, new Vector3(2f, 0, -1.7f), new Vector3(0, 145f, 0));
    }

    public override void _Input(InputEvent evt) {
        // Capture mouse when clicking on the game window
        if (evt is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed) {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }

        // Release mouse when pressing the 'Esc' key
        if (evt is InputEventKey eventKey && eventKey.Pressed) {
            if (eventKey.PhysicalKeycode == Key.Escape) {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
        }
    }

    public override void _ExitTree() {
        Runtime.Cleanup();
        base._ExitTree();
    }

    public override void _Notification(int what) {
        if (what == MainLoop.NotificationApplicationFocusIn) {
            Runtime.IsFocused = true;
        } else if (what == MainLoop.NotificationApplicationFocusOut) {
            Runtime.IsFocused = false;
        }
    }
}
