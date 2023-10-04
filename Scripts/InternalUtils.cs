using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameUtils.FixedCamRender
{
    internal class InternalUtils
    {
        public static void CalculateTargetResolution(float size, out float width, out float height)
        {
            float curWidth = Screen.width;
            float curHeight = Screen.height;
            float aspect = curWidth / curHeight;
            if (curWidth > curHeight)
            {
                height = size;
                width = height * aspect;
            }
            else
            {
                width = size;
                height = width / aspect;
            }
        }
    }
}
