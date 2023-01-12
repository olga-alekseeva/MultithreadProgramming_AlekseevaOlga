using UnityEngine;
using UnityEngine.Rendering;

namespace CustomRenderPipeline
{
    partial class SpaceRunPipelineRender : RenderPipeline
    {
        public CameraRenderer _cameraRenderer;
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {

            CamerasRender(context, cameras);
        }

        private void CamerasRender(ScriptableRenderContext context, Camera[] cameras)
        {
            foreach (var camera in cameras)
            {
                _cameraRenderer.Render(context, camera);
            }
        }
    }
}

