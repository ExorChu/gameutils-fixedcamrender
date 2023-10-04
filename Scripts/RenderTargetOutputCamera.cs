using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameUtils.FixedCamRender
{
    public class RenderTargetOutputCamera : MonoBehaviour
    {
        private Camera outputCamera;
        private CommandBuffer buffer;
        private Mesh mesh;
        [SerializeField] private Material material;        

        private void Start()
        {
            IFixedRenderTarget.OnRenderTextureChanged += IFixedRenderTarget_OnRenderTextureChanged;
            buffer = new CommandBuffer();
            //we need to manually add shader because otherwise Unity might strip it
            //material = new Material(Shader.Find("Unlit/Texture"));            
            outputCamera = gameObject.AddComponent<Camera>();
            SetupCamera(outputCamera);
        }

        private void OnDestroy()
        {
            if(buffer != null)
                buffer.Dispose();
            IFixedRenderTarget.OnRenderTextureChanged -= IFixedRenderTarget_OnRenderTextureChanged;
        }

        private void IFixedRenderTarget_OnRenderTextureChanged(RenderTexture obj)
        {
            material.mainTexture = obj;
        }

        private void SetupCamera(Camera camera)
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.green;
            camera.orthographic = true;
            camera.orthographicSize = 1f;
            camera.depth = 99;
            camera.useOcclusionCulling = false;
            camera.allowMSAA = false;
            camera.allowHDR = false;
            camera.allowDynamicResolution = false;
            camera.nearClipPlane = 0.01f;
            camera.farClipPlane = 0.3f;
            camera.cullingMask = 0;            

            if(IFixedRenderTarget.Instance != null)
            {
                //var renderTexture = IFixedRenderTarget.Instance.TargetTexture;
                Debug.Log("Render");
                float halfHeight = camera.orthographicSize;

                float halfWidth = Screen.width * halfHeight / Screen.height;

                mesh = CreateQuad(halfWidth, halfHeight);

                material.mainTexture = IFixedRenderTarget.Instance.TargetTexture;

                buffer.DrawMesh(mesh, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one), material);

                outputCamera.AddCommandBuffer(CameraEvent.AfterEverything, buffer);


                buffer.name = "Draw RenderTexture";
            }
        }

        private void OnPostRender()
        {
            return;
            GL.PushMatrix();
            material.SetPass(0);
            GL.LoadOrtho();
            GL.Clear(true, true, Color.green);
            GL.Begin(GL.QUADS);
            GL.TexCoord2(0, 0);
            GL.Vertex3(0, 0, 0);
            GL.TexCoord2(0, 1);
            GL.Vertex3(0, 1, 0);
            GL.TexCoord2(1, 1);
            GL.Vertex3(1, 1, 0);
            GL.TexCoord2(1, 0);
            GL.Vertex3(1, 0, 0);
            GL.End();
            GL.PopMatrix();
        }

        private Mesh CreateQuad(float hwidth, float hheight)
        {
            var quadMesh = new Mesh();

            Vector3[] vertices = new Vector3[]
            {
            new Vector3(-hwidth, -hheight ,0.1f),
            new Vector3(hwidth,-hheight,0.1f),
            new Vector3(-hwidth,hheight,0.1f),
            new Vector3(hwidth,hheight,0.1f)
            };

            quadMesh.vertices = vertices;
            int[] tris = new int[]
            {
            0,2,1,
            2,3,1
            };
            quadMesh.triangles = tris;
            Vector3[] normals = new[]
            {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
        };
            quadMesh.normals = normals;

            Vector2[] uvs = new[]
            {
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(0,1),
            new Vector2(1,1),
        };
            quadMesh.uv = uvs;

            quadMesh.UploadMeshData(true);
            return quadMesh;
        }
    }
}
