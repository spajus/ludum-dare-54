using Godot;

public static class Runtime {
    public static void Initialize(Node root, Room room) {
        Input.MouseMode = Input.MouseModeEnum.Confined;
        IsFocused = true;
        BookPool = new NodePool<RigidBody3D>(Scenes.Book);
        Root = root;
        Room = room;
        GameState = Game.State.MainMenu;
    }
    public static void Cleanup() {
        IsFocused = false;
        BookPool = null;
        Root = null;
        Room = null;
        Score = null;
        Timer = null;
        GameState = Game.State.MainMenu;
        UI = null;
    }
    public static bool IsFocused;
    public static NodePool<RigidBody3D> BookPool;
    public static Node Root;
    public static Room Room;
    public static Score Score;
    public static GameTimer Timer;
    public static Game.State GameState;
    public static UI UI;
}
