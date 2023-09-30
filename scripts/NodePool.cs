using System.Collections.Generic;
using Godot;

public class NodePool<T> where T : Node3D {
    private readonly string scene;
    private readonly Stack<T> pool = new();
    public NodePool(string scene) {
        this.scene = scene;
    }

    public T Spawn() {
        if (pool.TryPop(out var res)) {
            res.ProcessMode = Node.ProcessModeEnum.Inherit;
            res.Visible = true;
            return res;
        }
        return Utils.Instantiate<T>(scene);
    }

    public void Despawn(T item) {
        item.Visible = false;
        item.ProcessMode = Node.ProcessModeEnum.Disabled;
        pool.Push(item);
    }
}