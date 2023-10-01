using Godot;

public class Score {
    private readonly Label label;
    public Score(Label label) {
        this.label = label;
        Add(0);
    }

    private int score;
    public void Add(int amt) {
        score = Mathf.Clamp(score + amt, 0, int.MaxValue);
        label.Text = $"SCORE: {score}";
    }
}