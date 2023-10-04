using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameUtils.FixedCamRender.IFixedRenderTarget;

namespace GameUtils.FixedCamRender
{
    public interface IFixedRenderTarget
    {
        public static event Action<RenderTexture> OnRenderTextureChanged;
        public static void NotifyRenderTexureChanged(RenderTexture tex) => OnRenderTextureChanged?.Invoke(tex);
        public static IFixedRenderTarget Instance { get; internal set; }
        public float CurrentHeight { get; }
        public float CurrentWidth { get; }
        public void SetOutputRenderDesc(RenderDescriptor descriptor);
        public void SetOutputRenderSize(float size);
        public void AddCamera(Camera camera);
        public void RemoveCamera(Camera camera);
        public RenderTexture TargetTexture { get; }

        public struct RenderDescriptor : IEquatable<RenderDescriptor>
        {
            public float size;
            public int aaLevel;

            public bool Equals(RenderDescriptor other)
            {
                return Mathf.Approximately(other.size,size) && other.aaLevel == aaLevel;
            }

            public static bool operator==(RenderDescriptor left, RenderDescriptor right)
                => left.Equals(right);

            public static bool operator!=(RenderDescriptor left, RenderDescriptor right)
                => !left.Equals(right);

            public override bool Equals(object obj)
            {
                return (obj is RenderDescriptor r) && r.Equals(this);
            }            
        }
    }

    public class FixedRenderTargetSource : MonoBehaviour, IFixedRenderTarget
    {
        [SerializeField] private RenderDescriptor descriptor = new RenderDescriptor
        {
            size = 720,
            aaLevel = 2
        };

        private RenderTexture targetRenderTexture;
        private bool dirtyFlags;
        private HashSet<Camera> cameras;

        public float CurrentHeight => targetRenderTexture ? targetRenderTexture.height : 0;
        public float CurrentWidth => targetRenderTexture ? targetRenderTexture.width : 0;

        private void ClearLastRenderTexture()
        {
            if(targetRenderTexture != null)
            {
                Destroy(targetRenderTexture);
            }
        }

        private void SetupRenderTexture()
        {
            InternalUtils.CalculateTargetResolution(descriptor.size, out float width, out float height);

            targetRenderTexture = new RenderTexture((int)width, (int)height, 24, RenderTextureFormat.Default);
            targetRenderTexture.autoGenerateMips = false;
            targetRenderTexture.useDynamicScale = false;
            targetRenderTexture.antiAliasing = descriptor.aaLevel;
        }

        public void SetOutputRenderDesc(RenderDescriptor descriptor)
        {
            if(this.descriptor != descriptor)
            {
                this.descriptor = descriptor;
                dirtyFlags = true;
            }
        }
        public void SetOutputRenderSize(float size)
        {
            if(!Mathf.Approximately(size, descriptor.size))
            {
                descriptor.size = size;
                dirtyFlags = true;
            }
        }

        public RenderTexture TargetTexture => targetRenderTexture;

        public void AddCamera(Camera camera)
        {
            if (cameras.Add(camera))
            {
                camera.targetTexture = targetRenderTexture;
            }
        }

        public void RemoveCamera(Camera camera)
        {
            if (cameras.Remove(camera))
            {
                camera.targetTexture = null;
            }
        }

        private void Awake()
        {
            //should delete this object?
            if (Instance != null)
                return;

            Instance = this;
            cameras = new HashSet<Camera>(16);
            SetupRenderTexture();
        }

        private void LateUpdate()
        {
            if (!dirtyFlags)
                return;

            dirtyFlags = false;

            ClearLastRenderTexture();
            SetupRenderTexture();
            //retarget all camera to newly created textures
            if(cameras.Count > 0)
            {
                foreach(var c in cameras)
                {
                    c.targetTexture = targetRenderTexture;
                }
            }
            //call an event update to notify all listeners
            NotifyRenderTexureChanged(targetRenderTexture);
        }

        private void OnDestroy()
        {
            if (ReferenceEquals(Instance, this))
            {
                Instance = null;
            }

            if(cameras.Count > 0)
            {
                foreach(var c in cameras)
                {
                    if (c == null)
                        continue;
                    c.targetTexture = null;
                }
                cameras.Clear();
            }

            if (targetRenderTexture != null)
            {
                Destroy(targetRenderTexture);
            }
        }
    }
}
