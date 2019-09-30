using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Icaros.Mobile.Input {
    public class UnityInputSetup {

        [MenuItem("ICAROS/Input/Add Default Keyboard Controls")]
        private static void addKeyboard() {
            try {
                BindAxis(new Axis() { name = DeviceManager.INPUT_FIRST_BUTTON, positiveButton = "k", altPositiveButton = "space", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_SECOND_BUTTON, positiveButton = "l", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_THIRD_BUTTON, positiveButton = "i", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_FOURTH_BUTTON, positiveButton = "o", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_X_AXIS, positiveButton = "w", altPositiveButton = "up", negativeButton = "s", altNegativeButton = "down", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_Y_AXIS, positiveButton = "e", altPositiveButton = "right ctrl", negativeButton = "q", altNegativeButton = "right shift", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_Z_AXIS, positiveButton ="a", altPositiveButton = "left", negativeButton = "d", altNegativeButton = "right", type = 0 });
            }
            catch {
                Debug.LogError("Failed to apply Keyboard input manager bindings.");
            }
        }
        
        //IMPORTANT! The GearVR Gamepad registers on unity android as keyboard instead of a joystick for some reason. 
        [MenuItem("ICAROS/Input/Add Default GearVR Gamepad Controls")]
        private static void addGearVR() {
            try {
                BindAxis(new Axis() { name = DeviceManager.INPUT_FIRST_BUTTON, positiveButton = "joystick button 0", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_SECOND_BUTTON, positiveButton = "joystick button 1", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_THIRD_BUTTON, positiveButton = "joystick button 2", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_FOURTH_BUTTON, positiveButton = "joystick button 3", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_X_AXIS, axis = 1, invert = true });
                BindAxis(new Axis() { name = DeviceManager.INPUT_Y_AXIS, axis = 2 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_Z_AXIS, axis = 0 });
            }
            catch {
                Debug.LogError("Failed to apply GearVR input manager bindings.");
            }
        }

        private class Axis {
            public string name = String.Empty;
            public string descriptiveName = String.Empty;
            public string descriptiveNegativeName = String.Empty;
            public string negativeButton = String.Empty;
            public string positiveButton = String.Empty;
            public string altNegativeButton = String.Empty;
            public string altPositiveButton = String.Empty;
            public float gravity = 0.0f;
            public float dead = 0.001f;
            public float sensitivity = 1.0f;
            public bool snap = false;
            public bool invert = false;
            public int type = 2;
            public int axis = 0;
            public int joyNum = 0;
        }

        private static void BindAxis(Axis axis) {
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");
            
            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();
            
            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);
            axisProperty.FindPropertyRelative("m_Name").stringValue = axis.name;
            axisProperty.FindPropertyRelative("descriptiveName").stringValue = axis.descriptiveName;
            axisProperty.FindPropertyRelative("descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
            axisProperty.FindPropertyRelative("negativeButton").stringValue = axis.negativeButton;
            axisProperty.FindPropertyRelative("positiveButton").stringValue = axis.positiveButton;
            axisProperty.FindPropertyRelative("altNegativeButton").stringValue = axis.altNegativeButton;
            axisProperty.FindPropertyRelative("altPositiveButton").stringValue = axis.altPositiveButton;
            axisProperty.FindPropertyRelative("gravity").floatValue = axis.gravity;
            axisProperty.FindPropertyRelative("dead").floatValue = axis.dead;
            axisProperty.FindPropertyRelative("sensitivity").floatValue = axis.sensitivity;
            axisProperty.FindPropertyRelative("snap").boolValue = axis.snap;
            axisProperty.FindPropertyRelative("invert").boolValue = axis.invert;
            axisProperty.FindPropertyRelative("type").intValue = axis.type;
            axisProperty.FindPropertyRelative("axis").intValue = axis.axis;
            axisProperty.FindPropertyRelative("joyNum").intValue = axis.joyNum;
            serializedObject.ApplyModifiedProperties();
        }
    }
}