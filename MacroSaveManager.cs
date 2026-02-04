using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;



public static class MacroFileManager
{
    public static void Save(string path, List<MacroAction> actions)
    {
        string json = JsonConvert.SerializeObject(actions, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public static List<MacroAction> Load(string path)
    {
        if (!File.Exists(path))
            return new List<MacroAction>();

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<List<MacroAction>>(json);
    }
}
