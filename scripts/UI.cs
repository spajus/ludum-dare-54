using System;
using Godot;

public partial class UI : Node {
    private CanvasModulate crosshair;
    public override void _Ready() {
        crosshair = GetNode<CanvasModulate>(Nodes.CrosshairModulate);
        var scoreLabel = GetNode<RichTextLabel>(Nodes.ScoreLabel);
        var timerLabel = GetNode<RichTextLabel>(Nodes.TimerLabel);
        Runtime.Score = new Score(scoreLabel);
        Runtime.Timer = new GameTimer(timerLabel, 300);
        Runtime.Timer.Start();
    }

    public void SetCrosshairTint(Color color) {
        crosshair.Color = color;
    }
}