using Godot;

public static class Utils {
    public static T Instantiate<T>(string scene) where T : Node {
        var ps = (PackedScene) GD.Load(scene);
        return (T) ps.Instantiate();
    }

    public static void AddSceneTo(Node node, string scene) {
        node.AddChild(Instantiate<Node>(scene));
    }
    public static void AddSceneTo(Node node, string scene, Vector3 offset) {
        var child = Instantiate<Node3D>(scene);
        child.Position += offset;
        node.AddChild(child);
    }

    public static void AddSceneTo(Node node, string scene, Vector3 offset, Vector3 rot) {
        var child = Instantiate<Node3D>(scene);
        child.Position += offset;
        child.RotationDegrees += rot;
        node.AddChild(child);
    }
}