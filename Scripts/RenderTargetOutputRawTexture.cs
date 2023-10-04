using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUtils.FixedCamRender
{
    public class RenderTargetOutputRawTexture : MonoBehaviour
    {
        private RawImage rawImage;

        private void Start()
        {
            rawImage = GetComponent<RawImage>();
            IFixedRenderTarget.OnRenderTextureChanged += IFixedRenderTarget_OnRenderTextureChanged;
            SetRenderTextureToRawImage(IFixedRenderTarget.Instance.TargetTexture);
        }

        private void SetRenderTextureToRawImage(RenderTexture texture)
        {
            if (rawImage == null)
            {
                Debug.LogError("Can't find component RawImage on this gameObject");
                return;
            }

            rawImage.texture = texture;
        }

        private void OnDestroy()
        {
            IFixedRenderTarget.OnRenderTextureChanged -= IFixedRenderTarget_OnRenderTextureChanged;
        }

        private void IFixedRenderTarget_OnRenderTextureChanged(RenderTexture texture)
        {                       
            SetRenderTextureToRawImage(texture);
        }
    }
}
