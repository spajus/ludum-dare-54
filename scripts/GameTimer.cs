using System;
using System.Threading.Tasks.Dataflow;
using Godot;

public class GameTimer {
    private readonly RichTextLabel label;
    public bool IsRunning;
    private double time;
    private readonly double timeout;
    private int extraTime;
    private int lastTimeLeft;
    public event Action OnDone;
    public GameTimer(RichTextLabel label, double timeout) {
        this.timeout = timeout;
        this.label = label;
        UpdateTimerLabel((int) timeout);
    }

    private void UpdateTimerLabel(int timeLeft) {
        if (lastTimeLeft == timeLeft) { return; }
        lastTimeLeft = timeLeft;
        if (timeLeft < 6) {
            Runtime.Audio.PlayBeepAt(timeLeft > 0, 1f + (5 - timeLeft) * 0.05f);
        }
        var color = "green";
        if (timeLeft <= 10) {
            color = "red";
        } else if (timeLeft < 30) {
            color = "yellow";
        }
        label.Text = $"Time Left: [color={color}]{timeLeft}[/color]";
    }

    public void Start() {
        time = 0;
        extraTime = 0;
        IsRunning = true;
    }



    public void Tick(double delta) {
        if (!IsRunning) { return; }
        if (Runtime.GameState != Game.State.Game) { return; }
        time += delta;
        var timeLeft = Mathf.RoundToInt(timeout - time) + extraTime;
        UpdateTimerLabel(timeLeft);
        if (time < timeout + extraTime) { return; }
        OnDone?.Invoke();
        IsRunning = false;
    }

    public void AddTime(int amt) {
        extraTime += amt;
    }
}