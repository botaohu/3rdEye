//Copyright 2017 Looking Glass Factory Inc.
//All rights reserved.
//Unauthorized copying or distribution of this file, and the source code contained herein, is strictly prohibited.

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace HoloPlaySDK_UI
{
    public class RealsenseTargetCircle : MonoBehaviour
    {
        public int segments;
        public float radius;
        LineRenderer line;
        public float circleWidth = 1f;
        public bool updateEveryFrame = true;

        public List<Vector3> points = new List<Vector3>();

        void Start()
        {
            if (line == null)
                line = gameObject.GetComponent<LineRenderer>();

            DoRing();
        }

        void Update()
        {
            if (updateEveryFrame)
            {
                DoRing();
            }
        }

        public void SetWidth(float w)
        {
            if (line == null)
                line = gameObject.GetComponent<LineRenderer>();

            circleWidth = w;
            DoRing();
        }

        void DoRing()
        {
            line.positionCount = segments + 2;
            line.useWorldSpace = false;
            line.startWidth = line.endWidth = circleWidth;
            CreatePoints();
        }

        public void Kill()
        {
            Destroy(gameObject);
        }
        void CreatePoints()
        {
            float x;
            float y;
            float z = 0f;

            float angle = 20f;

            for (int i = 0; i < (segments + 2); i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                Vector3 v = new Vector3(x, z, y);

                line.SetPosition(i, v);
                points.Add(v);

                angle += (360f / segments);
            }
        }
    }
}