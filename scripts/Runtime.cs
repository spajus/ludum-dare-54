using Godot;

public static class Runtime {
    public static void Initialize(Game root, Room room) {
        Input.MouseMode = Input.MouseModeEnum.Confined;
        IsFocused = true;
        BookPool = new NodePool<RigidBody3D>(Scenes.Book);
        Root = root;
        Room = room;
        Game = root;
        Audio = new Audio(root);
        GameState = Game.State.MainMenu;
        Persist = new Persist(Consts.SavePath);
    }
    public static void Cleanup() {
        GameState = Game.State.MainMenu;
        IsFocused = false;
        BookPool = null;
        Game = null;
        Root = null;
        Room = null;
        Score = null;
        Timer = null;
        Persist = null;
        Camera = null;
        UI = null;
        Audio = null;
    }
    public static bool IsFocused;
    public static NodePool<RigidBody3D> BookPool;
    public static Persist Persist;
    public static Audio Audio;
    public static Game Game;
    public static Node Root;
    public static Room Room;
    public static Score Score;
    public static Camera3D Camera;
    public static GameTimer Timer;
    public static Game.State GameState;
    public static UI UI;
}
