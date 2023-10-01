using System;
using System.Threading.Tasks.Dataflow;
using Godot;

public class GameTimer {
    private readonly RichTextLabel label;
    public bool IsRunning;
    private double time;
    private readonly double timeout;
    public event Action OnDone;
    public GameTimer(RichTextLabel label, double timeout) {
        this.timeout = timeout;
        this.label = label;
        UpdateTimerLabel((int) timeout);
    }

    private void UpdateTimerLabel(int timeLeft) {
        var color = "green";
        if (timeLeft < 10) {
            color = "red";
        } else if (timeLeft < 60) {
            color = "yellow";
        }
        label.Text = $"Time Left: [color={color}]{timeLeft}[/color]";
    }

    public void Start() {
        time = 0;
        IsRunning = true;
    }

    public void Tick(double delta) {
        if (!IsRunning) { return; }
        if (Runtime.GameState != Game.State.Game) { return; }
        time += delta;
        var timeLeft = Mathf.RoundToInt(timeout - time);
        UpdateTimerLabel(timeLeft);
        if (time < timeout) { return; }
        OnDone?.Invoke();
        IsRunning = false;
    }
}