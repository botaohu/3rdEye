//Copyright 2017 Looking Glass Factory Inc.
//All rights reserved.
//Unauthorized copying or distribution of this file, and the source code contained herein, is strictly prohibited.

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using HoloPlaySDK;

using UnityEngine;

namespace HoloPlaySDK_UI
{
    public class CalibratedPointer : MonoBehaviour
    {
        public bool autoScale = true;
        public float pointerScale = 1;
        public int fingerIndex = 0;
        [Header("Physics")]
        [Tooltip("If enabled, cursor movement will be updated by moving this gameobject's rigidbody." +
            "\n\n Must have a rigidbody attached to use.")]
        public bool physicsBasedMovement;
        Rigidbody rb;

        void OnEnable() { SceneManager.activeSceneChanged += SceneChange; }
        void OnDisable() { SceneManager.activeSceneChanged -= SceneChange; }

        void SceneChange(Scene from, Scene to)
        {
            if (autoScale)
                transform.localScale = HoloPlay.Main.size * Vector3.one * .1f * pointerScale;
        }

        void Start()
        {
            if (autoScale)
                transform.localScale = HoloPlay.Main.size * Vector3.one * .1f * pointerScale;
        }

        void Update()
        {
            if (!physicsBasedMovement)
                transform.position = RealsenseCalibrator.Instance.GetWorldPos(fingerIndex);
        }

        void FixedUpdate()
        {
            if (physicsBasedMovement)
            {
                if (rb == null)
                {
                    rb = GetComponent<Rigidbody>();
                    if (rb == null)
                    {
                        Debug.LogWarning("Cannot use physics based movement on cursor " +
                            "because there is no rigidbody attached to the gameobject!");
                        physicsBasedMovement = false;
                        return;
                    }
                }
                rb.MovePosition(RealsenseCalibrator.Instance.GetWorldPos(0));
            }
        }

    }
}