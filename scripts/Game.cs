using Godot;
using System;

public partial class Game : Node {
    public enum State {
        MainMenu,
        Game,
        GameOver,
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        var room = Utils.AddSceneTo<Room>(this, Scenes.Room);
        Runtime.Initialize(this, room);
        var ui = Utils.AddSceneTo<UI>(this, Scenes.UI);
        Utils.AddSceneTo<Node3D>(this, Scenes.Bookshelf, new Vector3(0, 0, -2.25f));
        Utils.AddSceneTo<Node3D>(this, Scenes.Player);
        Runtime.UI = ui;
        Runtime.Timer.OnDone += OnEndGame;
        var soundtrack = GetNode<AudioStreamPlayer>(Nodes.Soundtrack);
        soundtrack.Finished += () => soundtrack.Play();
    }

    public void StartGame() {
        Runtime.BookPool.DespawnAll();
        Runtime.Timer.Start();
        Runtime.Score.StartGame();
        Runtime.Room.SpawnBooks(Consts.SpawnBooks);
    }

    private void OnEndGame() {
    }

    public override void _Process(double delta) {
        Runtime.Timer.Tick(delta);
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
