using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class CameraRenderer 
{
    private ScriptableRenderContext _context;
    private Camera _camera;
    private readonly CommandBuffer _commandBuffer = new CommandBuffer
    { name = bufferName };
    private const string bufferName = "Camera Render";
    public void Render(ScriptableRenderContext context, Camera camera)
    {
        _camera = camera;
        _context = context;

        Settings();
        DrawVisible();
        Submit();

    }
    private void Settings()
    {
        _commandBuffer.BeginSample(bufferName);
        ExecuteCommandBuffer();
        _context.SetupCameraProperties(_camera);
    }
    private void Submit()
    {
        _commandBuffer.EndSample(bufferName);
        ExecuteCommandBuffer();
        _context.Submit();
    }
    private void ExecuteCommandBuffer()
    {
        _context.ExecuteCommandBuffer(_commandBuffer);
        _commandBuffer.Clear();
    }
    private void DrawVisible()
    {
        _context.DrawSkybox(_camera);
    }

}
