using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtils.FixedCamRender.Samples
{
    public class RotateObject : MonoBehaviour
    {
        public Vector3 rotateVel = new Vector3(0, 0, 45f);

        private void Update()
        {
            transform.Rotate(rotateVel * Time.deltaTime);
        }
    }
}
