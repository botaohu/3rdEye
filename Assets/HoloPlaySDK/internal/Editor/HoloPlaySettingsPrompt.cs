using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.Rendering;

[InitializeOnLoad]
public class HoloPlaySettingsPrompt : EditorWindow
{
    // public static HoloPlaySettingsPrompt Instance { get; private set; }
    // public static bool IsOpen { get { return Instance != null; } }

    class setting
    {
        public string label;
        public bool on;
        public setting(string label, bool on)
        {
            this.label = label;
            this.on = on;
        }
    }
    List<setting> settings;
    static string editorPrefName = "HoloPlay Proj Settings";
    BuildTarget[] buildPlatforms = new []
    {
        BuildTarget.StandaloneOSXIntel,
        BuildTarget.StandaloneOSXIntel64,
        BuildTarget.StandaloneOSXUniversal
    };
    GraphicsDeviceType[] graphicsAPIs = new []
    {
        GraphicsDeviceType.OpenGLCore,
        GraphicsDeviceType.Metal
    };

    static HoloPlaySettingsPrompt()
    {
        EditorApplication.update += CheckIfPromptedYet;
    }

    static void CheckIfPromptedYet()
    {
        if (!EditorPrefs.GetBool(editorPrefName + PlayerSettings.productName, false))
        {
            Init();
        }
        EditorApplication.update -= CheckIfPromptedYet;
    }

    static void Init()
    {
        HoloPlaySettingsPrompt window = EditorWindow.GetWindow<HoloPlaySettingsPrompt>();
        window.Show();
    }

    void OnEnable()
    {
        titleContent = new GUIContent("HoloPlay Settings");

        settings = new List<setting>
        {
            new setting("Shadows: Hard Only", true),
            new setting("Shadow Projection: Close Fit", true),
            new setting("Shadow Distance: 1000", true),
            new setting("Shadow Cascades: 0", true),
            new setting("vSync: off", true),
            new setting("macOS Graphics API: OpenGLCore", true)
        };

        Vector2 size = new Vector2(360, 260);
        maxSize = size;
        minSize = size;
    }

    void OnGUI()
    {
        EditorGUILayout.HelpBox(
            "It is recommended you change the following project settings " +
            "to ensure the best performace for your HoloPlay application",
            MessageType.Warning
        );
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Select which options to change:", EditorStyles.miniLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        foreach(var s in settings)
        {
            EditorGUILayout.BeginHorizontal();
            s.on = EditorGUILayout.ToggleLeft(s.label, s.on);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        // GUILayout.FlexibleSpace();
        GUI.backgroundColor = EditorGUIUtility.isProSkin ? Color.green : Color.Lerp(Color.green, Color.white, 0.5f);
        if (GUILayout.Button("Apply Changes"))
        {
            var qs = QualitySettings.names;
            int currentQuality = QualitySettings.GetQualityLevel();
            for (int i = 0; i < qs.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);

                if (settings[0].on)
                    QualitySettings.shadows = ShadowQuality.HardOnly;

                if (settings[1].on)
                    QualitySettings.shadowProjection = ShadowProjection.CloseFit;

                if (settings[2].on)
                    QualitySettings.shadowDistance = 1000;

                if (settings[3].on)
                    QualitySettings.shadowCascades = 0;

                if (settings[4].on)
                    QualitySettings.vSyncCount = 0;
            }

            //graphics api settings
            if (settings[5].on)
            {
                foreach(var b in buildPlatforms)
                {
                    PlayerSettings.SetUseDefaultGraphicsAPIs(b, false);
                    PlayerSettings.SetGraphicsAPIs(b, graphicsAPIs);
                    if (EditorUserBuildSettings.activeBuildTarget == b)
                    {
                        Debug.LogWarning("For graphics API switch to take effect, a project re-open is required");
                    }
                }
            }
            QualitySettings.SetQualityLevel(currentQuality, true);
            EditorPrefs.SetBool(editorPrefName + PlayerSettings.productName, true);
            Debug.Log(HoloPlaySDK.HoloPlay.warningText + "Optimization settings applied!");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = EditorGUIUtility.isProSkin ? Color.yellow : Color.Lerp(Color.yellow, Color.white, 0.5f);
        if (GUILayout.Button("Never display this popup again"))
        {
            EditorPrefs.SetBool(editorPrefName + PlayerSettings.productName, true);
            Debug.Log(HoloPlaySDK.HoloPlay.warningText + "Optimization popup hidden--" +
                "to show again, open in inspector window on HoloPlay Capture");
            Close();
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }
}