using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Icaros {
[InitializeOnLoad]
    public class ScriptExecutionOrder {

        static List<string> relevantScripts = new List<string>(new string[]{
        "UISystem",
        "DeviceManager"
    });
        static List<string> priorityScripts = new List<string>(new string[]{
        "PlayerManager"
    });

        static ScriptExecutionOrder() {
            MonoScript[] scripts = MonoImporter.GetAllRuntimeMonoScripts();
            foreach (MonoScript script in scripts) {
                if (relevantScripts.Contains(script.name)) {
                    if (MonoImporter.GetExecutionOrder(script) >= 0)
                        MonoImporter.SetExecutionOrder(script, -100);
                }
            }
            foreach (MonoScript script in scripts) {
                if (priorityScripts.Contains(script.name)) {
                    if (MonoImporter.GetExecutionOrder(script) >= 0)
                        MonoImporter.SetExecutionOrder(script, -200);
                }
            }
        }
    }
}