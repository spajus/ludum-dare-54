using Godot;

public static class Utils {
    public static T Instantiate<T>(string scene) where T : Node {
        var ps = (PackedScene) GD.Load(scene);
        return (T) ps.Instantiate();
    }

    public static T AddSceneTo<T>(Node node, string scene) where T : Node {
        var child = Instantiate<T>(scene);
        node.AddChild(child);
        return child;
    }
    public static T AddSceneTo<T>(Node node, string scene, Vector3 offset) where T : Node3D {
        var child = (Node3D) AddSceneTo<T>(node, scene);
        child.Position += offset;
        return (T) child;
    }

    public static T AddSceneTo<T>(Node node, string scene, Vector3 offset, Vector3 rot) where T : Node3D {
        var child = AddSceneTo<T>(node, scene, offset);
        child.RotationDegrees += rot;
        return child;
    }
}