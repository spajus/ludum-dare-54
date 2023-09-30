using Godot;

public static class Utils {
    public static T Instantiate<T>(string scene) where T : Node {
        var ps = (PackedScene) GD.Load(scene);
        return (T) ps.Instantiate();
    }

    public static void AddSceneTo(Node node, string scene) {
        node.AddChild(Instantiate<Node>(scene));
    }
}