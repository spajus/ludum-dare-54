using System;
using Godot;
public class Audio {
    private readonly AudioStreamPlayer3D bookDrop;
    private readonly AudioStreamPlayer addBook;
    private readonly AudioStreamPlayer loseBook;
    private readonly AudioStreamPlayer beepShort;
    private readonly AudioStreamPlayer beepLong;
    private float lastDropPlayStart;

    public Audio(Node container) {
        bookDrop = new AudioStreamPlayer3D();
        bookDrop.Stream = (AudioStream) GD.Load(Consts.AudioBookDrop);
        container.AddChild(bookDrop);

        addBook = new AudioStreamPlayer();
        addBook.Stream = (AudioStream) GD.Load(Consts.AudioAddBook);
        container.AddChild(addBook);

        loseBook = new AudioStreamPlayer();
        loseBook.Stream = (AudioStream) GD.Load(Consts.AudioLoseBook);
        container.AddChild(loseBook);

        beepShort = new AudioStreamPlayer();
        beepShort.Stream = (AudioStream) GD.Load(Consts.AudioBeepShort);
        container.AddChild(beepShort);

        beepLong = new AudioStreamPlayer();
        beepLong.Stream = (AudioStream) GD.Load(Consts.AudioBeepLong);
        container.AddChild(beepLong);
    }
    public void PlayBeepAt(bool isShort, float pitch) {
        Play(isShort ? beepShort : beepLong, pitch);
    }

    public void PlayAddBookAt() {
        Play(addBook);
    }
    public void PlayLoseBookAt() {
        Play(loseBook);
    }

    private void Play(AudioStreamPlayer player) {
        Play(player, Rng.NextFloat(0.95f, 1.05f));
        player.Play();
    }
    private void Play(AudioStreamPlayer player, float pitch) {
        player.PitchScale = pitch;
        player.Play();
    }
    private void Play(AudioStreamPlayer3D player, Vector3 at, float pitch) {
        player.GlobalPosition = at;
        player.PitchScale = pitch;
        player.Play();
    }

    private void Play(AudioStreamPlayer3D player, Vector3 at) {
        Play(player, at, Rng.NextFloat(0.95f, 1.05f));
    }

    public void PlayBookDropAt(Vector3 pos) {
        var t = Time.GetTicksMsec();
        if (lastDropPlayStart + Consts.AudioRepeatDelayMsec > t) {
            return;
        }
        lastDropPlayStart = t;
        Play(bookDrop, pos);
    }
}