using Godot;
public class Persist {
    private ConfigFile config;
    private readonly string savePath;
    public Persist(string savePath) {
        this.savePath = savePath;
        config = new ConfigFile();
        config.Load(savePath);
    }

    public void Save(string var, int value) {
        config.SetValue("game", var, value);
        config.Save(savePath);
    }

    public int Load(string var, Variant def) {
        return (int) config.GetValue("game", var, def);
    }
}