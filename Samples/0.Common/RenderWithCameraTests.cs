using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtils.FixedCamRender.Samples
{
    public class RenderWithCameraTests : MonoBehaviour
    {
        [SerializeField] private Camera camera;

        private void Start()
        {
            IFixedRenderTarget.Instance.AddCamera(camera);
        }

        public void SetResolution(float res)
        {
            IFixedRenderTarget.Instance.SetOutputRenderSize(res);
        }
    }
}
