using Godot;

public static class Runtime {
    public static void Initialize(Node root) {
        Input.MouseMode = Input.MouseModeEnum.Confined;
        IsFocused = true;
        BulletPool = new NodePool<RigidBody3D>(Scenes.Bullet);
        Root = root;
    }
    public static void Cleanup() {
        IsFocused = false;
        BulletPool = null;
        Root = null;
    }
    public static bool IsFocused;
    public static NodePool<RigidBody3D> BulletPool;
    public static Node Root;
}
