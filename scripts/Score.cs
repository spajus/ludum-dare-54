using Godot;

public class Score {
    private readonly RichTextLabel label;
    private int score;
    public int High;
    public int Last;
    public int NumGames;
    public Score(RichTextLabel label) {
        Runtime.Timer.OnDone += EndGame;
        this.label = label;
        Add(0);
        Last = Runtime.Persist.Load(Consts.VarScoreLast, 0);
        High = Runtime.Persist.Load(Consts.VarScoreHigh, 0);
        NumGames = Runtime.Persist.Load(Consts.VarNumGames, 0);
    }

    public void StartGame() {
        NumGames++;
        score = 0;
        Runtime.Persist.Save(Consts.VarNumGames, NumGames);
    }

    private void EndGame() {
        Last = score;
        Runtime.Persist.Save(Consts.VarScoreLast, Last);
        if (High < Last) {
            High = Last;
            Runtime.Persist.Save(Consts.VarScoreHigh, High);
        }
    }

    public void Add(int amt) {
        score = Mathf.Clamp(score + amt, 0, int.MaxValue);
        label.Text = $"Score: [color=green]{score}[/color]";
    }
}