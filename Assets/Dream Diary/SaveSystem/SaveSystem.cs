using System.IO;
using UnityEngine;

public class SaveSystem {
    const string CONFIG_FILE_NAME = "Config.json";
    string filePath = Path.Combine(Application.persistentDataPath, CONFIG_FILE_NAME);

    public bool LoadConfigFile(out ConfigDTO config) {
        if (File.Exists(filePath)) {
            string json = File.ReadAllText(filePath);
            config = JsonUtility.FromJson<ConfigDTO>(json);
            ApplyConfigValues(config);
            return true;
        } else {
            ConfigLoader configLoader = new ConfigLoader();
            var defaultConfig = configLoader.LoadConfigSO();
            config = new ConfigDTO {
                Movement = defaultConfig.Movement
            };
            SaveConfigFile(config);
            return false;
        }
    }

    public void SaveConfigFile(ConfigDTO configToSave) {
        ApplyConfigValues(configToSave);
        string json = JsonUtility.ToJson(configToSave);
        File.WriteAllText(filePath, json);
    }

    void ApplyConfigValues(ConfigDTO config) {
        Config.MovementSpeed = config.Movement;
    }
}

public class ConfigDTO {
    public float Movement;
}