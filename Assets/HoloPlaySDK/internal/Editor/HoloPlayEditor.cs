//Copyright 2017 Looking Glass Factory Inc.
//All rights reserved.
//Unauthorized copying or distribution of this file, and the source code contained herein, is strictly prohibited.

//parts taken from Game Window Mover script, original notes for that here:

//Source from http://answers.unity3d.com/questions/179775/game-window-size-from-editor-window-in-editor-mode.html
//Modified by seieibob for use at the Virtual Environment and Multimodal Interaction Lab at the University of Maine.
//Use however you'd like!

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HoloPlaySDK;

using UnityEditor;

using UnityEngine;

namespace HoloPlaySDK_UI
{
    [InitializeOnLoad]
    [CustomEditor(typeof(HoloPlay))]
    public class HoloPlayEditor : Editor
    {
        SerializedProperty size;
        SerializedProperty fov;
        SerializedProperty nearClip;
        SerializedProperty farClip;
        SerializedProperty renderInEditor;
        static int delayedMove = -1;
        static int delayedStart = 1;
        bool openedWindow;
        struct Prefs
        {
            public Vector2 position;
            public enum Alignment { Right, Left, Center, Custom }
            public Alignment alignment;
            public bool windowOpened;
        }
        static Prefs prefs;

        static HoloPlayEditor()
        {
            EditorApplication.update += DelayedStart;
        }

        static void SavePrefs()
        {
            EditorPrefs.SetString(PlayerSettings.productName + "HoloWindowPrefs", JsonUtility.ToJson(prefs));
        }

        static void LoadPrefs()
        {
            var prefStr = EditorPrefs.GetString(PlayerSettings.productName + "HoloWindowPrefs", "{}");
            prefs = JsonUtility.FromJson<Prefs>(prefStr);
        }

        static void DelayedStart()
        {
            if (delayedStart == 0)
            {
                LoadPrefs();
                EditorApplication.update -= DelayedStart;
            }
            delayedStart--;
        }

        static void Update()
        {
            if (delayedMove > 0)
            {
                delayedMove--;
            }
            else if (delayedMove == 0)
            {
                MoveGameWindow();
                //set the proper aspect/resolution
                int gameSize = GameViewUtils.FindSize(GameViewSizeGroupType.Standalone, "Standalone");
                GameViewUtils.SetSize(gameSize);
                delayedMove--;
                EditorApplication.update -= Update;
            }
        }

        void OnEnable()
        {
            size = serializedObject.FindProperty("size");
            fov = serializedObject.FindProperty("fov");
            nearClip = serializedObject.FindProperty("nearClip");
            farClip = serializedObject.FindProperty("farClip");
            renderInEditor = serializedObject.FindProperty("renderInEditor");
        }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            Color gizc = Color.HSVToRGB(0.35f, 1, EditorGUIUtility.isProSkin ? 1 : 0.5f);
            HoloPlay holoPlay = ((HoloPlay)target);

            EditorGUILayout.Space();

            GUI.color = Color.HSVToRGB(0.35f, 0.3f, 1);
            EditorGUILayout.LabelField("HoloPlay " + HoloPlay.version.ToString("0.###"), EditorStyles.centeredGreyMiniLabel);
            GUI.color = Color.white;

            GUI.color = gizc;
            EditorGUILayout.LabelField("- Camera -", EditorStyles.whiteMiniLabel);
            GUI.color = Color.white;

            EditorGUILayout.PropertyField(size);
            EditorGUILayout.PropertyField(nearClip);
            EditorGUILayout.PropertyField(farClip);

            //fix for if the user is inspecting the prefab
            if (holoPlay.Cam == null)
                return;

            EditorGUILayout.BeginHorizontal();
            bool ortho = holoPlay.Cam.orthographic;
            EditorGUILayout.LabelField(new GUIContent("Projection"));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(
                    ortho ? "orthographic" : "perspective",
                    EditorStyles.miniButton,
                    GUILayout.MaxWidth(EditorGUIUtility.fieldWidth * 2)))
            {
                ortho = !ortho;
                holoPlay.Cam.orthographic = ortho;
            }
            EditorGUILayout.EndHorizontal();

            GUI.enabled = !ortho;
            EditorGUILayout.PropertyField(fov);
            GUI.enabled = true;

            EditorGUILayout.Space();

            GUI.color = gizc;
            EditorGUILayout.LabelField("- Preview -", EditorStyles.whiteMiniLabel);
            GUI.color = Color.white;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                new GUIContent(
                    "Render in editor",
                    "Renders with the lenticular postprocess shader while in edit mode"
                )
            );
            GUILayout.FlexibleSpace();
            if (
                GUILayout.Button(
                    renderInEditor.boolValue ? "on" : "off",
                    EditorStyles.miniButton,
                    GUILayout.MaxWidth(EditorGUIUtility.fieldWidth)
                )
            )
            {
                renderInEditor.boolValue = !renderInEditor.boolValue;
            }
            EditorGUILayout.EndHorizontal();

            var currentAlignment = prefs.alignment;
            prefs.alignment = (Prefs.Alignment)EditorGUILayout.EnumPopup("Alignment", prefs.alignment);
            if (currentAlignment != prefs.alignment)
                SavePrefs();

            switch (prefs.alignment)
            {
                case Prefs.Alignment.Right:
                    prefs.position = new Vector2(Screen.currentResolution.width, 0);
                    break;
                case Prefs.Alignment.Center:
                    prefs.position = Vector2.zero;
                    break;
                case Prefs.Alignment.Left:
                    prefs.position = new Vector2(-HoloPlay.Config.screenW, 0);
                    break;
                case Prefs.Alignment.Custom:
                    prefs.position = EditorGUILayout.Vector2Field("Position", prefs.position);
                    break;
            }

            string previewerShortcutKey = "Ctrl + E";
#if UNITY_EDITOR_OSX
            previewerShortcutKey = "⌘E";
#endif

            if (GUILayout.Button(new GUIContent(
                    "Toggle HoloPlay Previewer " + previewerShortcutKey,
                    "If your HoloPlayer One is set up as a second display, " +
                    "this will generate a game window on it to use as a " +
                    "realtime preview")))
            {
                ToggleWindow();
            }

            EditorGUILayout.Space();

            GUI.color = gizc;
            EditorGUILayout.LabelField("- Project Settings -", EditorStyles.whiteMiniLabel);
            GUI.color = Color.white;

            EditorGUILayout.Space();

            if (GUILayout.Button(new GUIContent(
                    "Open HoloPlay Optimization Settings",
                    "Open a window that will let you select project settings " +
                    "to be optimized for best performance with HoloPlay")))
            {
                HoloPlaySettingsPrompt window = EditorWindow.GetWindow<HoloPlaySettingsPrompt>();
                window.Show();
            }

            if (GUILayout.Button(new GUIContent(
                "Reload Config",
                "Reload the config, only really necessary if " +
                "you edited externally and the new config settings won't load"
            )))
            {
                HoloPlay.LoadConfig();
            }

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("GameObject/HoloPlay/HoloPlay Capture", false, 10)]
        public static void CreateHoloPlay()
        {
            new GameObject("HoloPlay Capture", typeof(HoloPlay), typeof(AudioListener));
        }

        [MenuItem("GameObject/HoloPlay/Realsense Manager", false, 11)]
        public static void CreateRealsenseManager()
        {
            var realsenseChild = new GameObject("Realsense Manager",
                typeof(depthPlugin),
                typeof(RealsenseCalibrator)
            );
            var cursorGo = Instantiate((GameObject)Resources.Load("Realsense Cursor"));
            cursorGo.name = "Realsense Cursor";
            cursorGo.transform.SetParent(realsenseChild.transform);
            cursorGo.transform.position = Vector3.zero;
        }

        //game window mover
        public static EditorWindow GetMainGameView(bool dontCreate = false)
        {
            if (!dontCreate)
                EditorApplication.ExecuteMenuItem("Window/Game");

            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetMainGameView = T.GetMethod(
                "GetMainGameView",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            );
            System.Object Res = GetMainGameView.Invoke(null, null);

            return (EditorWindow)Res;
        }

        static void MoveGameWindow(bool initial = false)
        {
            //The size of the toolbar above the game view, excluding the OS border.
            int tabHeight = 22;
            // int tabHeight = 0;

            //Get the size of the window borders. Changes depending on the OS.
#if UNITY_STANDALONE_WIN
            //Windows settings
            int osBorderWidth = 5;
#elif UNITY_STANDALONE_OSX
            //Mac settings (untested)
            int osBorderWidth = 0; //OSX windows are borderless.
#else
            //Linux / other platform; sizes change depending on the variant you're running
            int osBorderWidth = 5;
#endif

            EditorWindow gameView = GetMainGameView();
            gameView.titleContent = new GUIContent("Game (Stereo)");
            //When minSize and maxSize are the same, no OS border is applied to the window.
            gameView.minSize = new Vector2(
                HoloPlay.Config.screenW,
                HoloPlay.Config.screenH + tabHeight - osBorderWidth
            );
            gameView.maxSize = gameView.minSize;
            Rect newPos = new Rect();
            var x = !initial ? prefs.position.x : 100;
            var y = !initial ? prefs.position.y - tabHeight + osBorderWidth : 100;
            y += Application.platform == RuntimePlatform.OSXEditor ? 2 : 0;
            newPos = new Rect(
                x,
                y,
                HoloPlay.Config.screenW,
                HoloPlay.Config.screenH + tabHeight - osBorderWidth
            );
            gameView.position = newPos;
            gameView.ShowPopup();
        }

        static void DelayedMoveGameWindow(EditorWindow gameView, Rect newPos)
        {
            gameView.position = newPos;
        }

        [MenuItem("Window/Toggle HoloPlay Previewer %e")]
        static void ToggleWindow()
        {
            HoloPlay.LoadConfig();
            EditorWindow gameWindow = GetMainGameView(true);
            EditorApplication.update += Update;
            if (gameWindow != null)
                gameWindow.Close();
            else
            {
                MoveGameWindow(true);
                delayedMove = 1;
            }
            SavePrefs();

        }
    }
}