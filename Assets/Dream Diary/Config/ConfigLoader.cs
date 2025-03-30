using UnityEngine;
using Object = UnityEngine.Object;

public class ConfigLoader {
    public const string CONFIG_PATH = "Config/ConfigSO";

    public ConfigSO LoadConfigSO()
        => Resources.Load<ConfigSO>(CONFIG_PATH);

    public void UnloadConfigSO(Object unloadObject) {
        Resources.UnloadAsset(unloadObject);
    }
}
