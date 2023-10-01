using System;
using Godot;
public class Audio {
    private readonly AudioStreamPlayer3D bookDrop;
    private readonly AudioStreamPlayer3D addBook;
    private readonly AudioStreamPlayer3D loseBook;
    private readonly AudioStreamPlayer3D beepShort;
    private readonly AudioStreamPlayer3D beepLong;
    private float lastDropPlayStart;

    public Audio(Node container) {
        bookDrop = new AudioStreamPlayer3D();
        bookDrop.Stream = (AudioStream) GD.Load(Consts.AudioBookDrop);
        container.AddChild(bookDrop);

        addBook = new AudioStreamPlayer3D();
        addBook.Stream = (AudioStream) GD.Load(Consts.AudioAddBook);
        container.AddChild(addBook);

        loseBook = new AudioStreamPlayer3D();
        loseBook.Stream = (AudioStream) GD.Load(Consts.AudioLoseBook);
        container.AddChild(loseBook);

        beepShort = new AudioStreamPlayer3D();
        beepShort.Stream = (AudioStream) GD.Load(Consts.AudioBeepShort);
        container.AddChild(beepShort);

        beepLong = new AudioStreamPlayer3D();
        beepLong.Stream = (AudioStream) GD.Load(Consts.AudioBeepLong);
        container.AddChild(beepLong);
    }
    public void PlayBeepAt(Vector3 pos, bool isShort, float pitch) {
        Play(isShort ? beepShort : beepLong, pos, pitch);
    }

    public void PlayAddBookAt(Vector3 pos) {
        Play(addBook, pos);
    }
    public void PlayLoseBookAt(Vector3 pos) {
        GD.Print("Playing lose book");
        Play(loseBook, pos);
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