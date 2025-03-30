using UnityEngine;
using Object = UnityEngine.Object;

public class ConfigLoader {
    const string CONFIG_PATH = "Config/ConfigSO";
    public void ApplyConfigFromSO() {
        var configSO = LoadConfigSO(CONFIG_PATH);
        Config.DEFAULT_MOVEMENT = configSO.Movement;
        UnloadConfigSO(configSO);
    }

    public ConfigSO LoadConfigSO(string path)
        => Resources.Load<ConfigSO>(path);

    public void UnloadConfigSO(Object unloadObject) {
        Resources.UnloadAsset(unloadObject);
    }
}
