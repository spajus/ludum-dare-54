using System;
using System.Diagnostics;
using Godot;

public partial class UI : Node {
    private CanvasModulate crosshair;
    private CanvasLayer layoutMainMenu;
    private CanvasLayer layoutGame;
    private CanvasLayer layoutCrosshair;
    private Game.State lastState;
    private Label highScoreLabel;
    private Label lastScoreLabel;
    private Button startButton;
    public override void _Ready() {
        crosshair = GetNode<CanvasModulate>(Nodes.CrosshairModulate);
        var scoreLabel = GetNode<RichTextLabel>(Nodes.ScoreLabel);
        var timerLabel = GetNode<RichTextLabel>(Nodes.TimerLabel);
        layoutMainMenu = GetNode<CanvasLayer>(Nodes.LayoutMainMenu);
        layoutGame = GetNode<CanvasLayer>(Nodes.LayoutGame);
        layoutCrosshair = GetNode<CanvasLayer>(Nodes.LayoutCrosshair);
        startButton = GetNode<Button>(Nodes.StartGameButton);
        startButton.Pressed += OnStartGamePressed;
        highScoreLabel = GetNode<Label>(Nodes.HighScoreLabel);
        lastScoreLabel = GetNode<Label>(Nodes.LastScoreLabel);
        GetNode<Button>(Nodes.ExitButton).Pressed += () => GetTree().Quit();

        Runtime.Timer = new GameTimer(timerLabel, Consts.SessionSeconds);
        Runtime.Score = new Score(scoreLabel);
        Runtime.Timer.OnDone += EndGame;

        ChangeState(Game.State.MainMenu);
    }

    private void OnStartGamePressed() {
        Runtime.GameState = Game.State.Game;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        if (!Runtime.Timer.IsRunning) {
            Runtime.Game.StartGame();
        }
    }

    private void EndGame() {
        Runtime.GameState = Game.State.GameOver;
    }

    public override void _Input(InputEvent evt) {
        bool isEscPressed = false;
        // Release mouse when pressing the 'Esc' key
        if (evt is InputEventKey eventKey && eventKey.Pressed) {
            if (eventKey.PhysicalKeycode == Key.Escape) {
                isEscPressed = true;
            }
        }
        if (Runtime.GameState == Game.State.Game) {
            if (isEscPressed) {
                Runtime.GameState = Game.State.MainMenu;
                return;
                //Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            // Capture mouse when clicking on the game window
            if (evt is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed) {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }

        } else {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            if (Runtime.GameState == Game.State.MainMenu && Runtime.Timer.IsRunning) {
                if (isEscPressed) {
                    OnStartGamePressed();
                }
            }
        }
    }

    private void ChangeState(Game.State state) {
        lastState = state;
        if (!Runtime.Timer.IsRunning) {
            layoutGame.Hide();
        }
        layoutCrosshair.Hide();
        layoutMainMenu.Hide();
        var hs = Runtime.Score.High;
        if (hs > 0) {
            highScoreLabel.Text = $"High Score: {hs}";
            highScoreLabel.Show();
        } else {
            highScoreLabel.Hide();
        }
        var ls = Runtime.Score.Last;
        if (ls > 0) {
            lastScoreLabel.Text = $"Last Score: {ls}";
            lastScoreLabel.Show();
        } else {
            lastScoreLabel.Hide();
        }
        switch (state) {
            case Game.State.MainMenu: {
                startButton.Text = Runtime.Timer.IsRunning ? "Resume" : "Play";
                layoutMainMenu.Show();
                break;
            }
            case Game.State.Game: {
                layoutGame.Show();
                layoutCrosshair.Show();
                break;
            }
            case Game.State.GameOver: {
                startButton.Text = "Restart";
                layoutMainMenu.Show();
                break;
            }
            default: {
                break;
            }
        }
    }
    public override void _Process(double delta) {
        if (Runtime.GameState != lastState) {
            ChangeState(Runtime.GameState);
        }
    }

    public void SetCrosshairTint(Color color) {
        crosshair.Color = color;
    }
}