using Godot;

public class Score {
    private readonly RichTextLabel label;
    public Score(RichTextLabel label) {
        this.label = label;
        Add(0);
    }

    private int score;
    public void Add(int amt) {
        score = Mathf.Clamp(score + amt, 0, int.MaxValue);
        label.Text = $"Score: [color=green]{score}[/color]";
    }
}