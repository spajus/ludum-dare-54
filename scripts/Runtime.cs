using Godot;

public static class Runtime {
    public static void Initialize(Node root) {
        Input.MouseMode = Input.MouseModeEnum.Confined;
        IsFocused = true;
        BookPool = new NodePool<RigidBody3D>(Scenes.Book);
        Root = root;
    }
    public static void Cleanup() {
        IsFocused = false;
        BookPool = null;
        Root = null;
    }
    public static bool IsFocused;
    public static NodePool<RigidBody3D> BookPool;
    public static Node Root;
}
