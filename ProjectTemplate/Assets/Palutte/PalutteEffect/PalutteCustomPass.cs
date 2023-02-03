using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;

public class PalutteCustomPass : CustomPass
{
    static int s_TempColorBuffer1 = Shader.PropertyToID("PalutteTempColorBuffer1");
    static int s_TempColorBuffer2 = Shader.PropertyToID("PalutteTempColorBuffer2");
    static readonly int k_PWidth = Shader.PropertyToID("_PWidth");
    static readonly int k_PHeight = Shader.PropertyToID("_PHeight");
    static readonly int k_DitherRange = Shader.PropertyToID("_DitherRange");
    static readonly int k_LutBlueTilesX = Shader.PropertyToID("_LUTBlueTilesX");
    static readonly int k_LutBlueTilesY = Shader.PropertyToID("_LUTBlueTilesY");
    static readonly int k_GridFractionX = Shader.PropertyToID("_GridFractionX");
    static readonly int k_GridFractionY = Shader.PropertyToID("_GridFractionY");
    static readonly int k_ColorSpaceCompressionFactor = Shader.PropertyToID("_ColorSpaceCompressionFactor");
    static readonly int k_LutTex = Shader.PropertyToID("_LUTTex");
    static readonly int k_DitherTex = Shader.PropertyToID("_DitherTex");
    static Material s_Material;

    public Texture LUTTexture;
    public int gridWidth = 16;
    public int gridHeight = 16;

    public bool matchCamSize;
    public int pixelsWidth = 200;
    public int pixelsHeight = 150;

    public Texture ditherMatrix;
    [Range(0f, 0.5f)]
    public float ditherAmount = 0.1f;

    [Range(0, 8)]
    public float colorSpaceCompressionPower = 0;

    public bool jaggiesAreGood = true;

    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
        if (s_Material == null)
            s_Material = new Material(Resources.Load<Shader>("PalutteShader"));
    }

    protected override void Execute(CustomPassContext ctx)
    {
        if (LUTTexture == null)
            return;

        var cam = ctx.hdCamera.camera;
        var colorBuffer = ctx.cameraColorBuffer;
        var cmd = ctx.cmd;

        var activeWidth = pixelsWidth;
        var activeHeight = pixelsHeight;

        if (matchCamSize)
        {
            activeWidth = cam.pixelWidth;
            activeHeight = cam.pixelHeight;
        }

        var filterMode = jaggiesAreGood ? FilterMode.Point : FilterMode.Bilinear;

        s_Material.SetFloat(k_PWidth, activeWidth);
        s_Material.SetFloat(k_PHeight, activeHeight);
        s_Material.SetFloat(k_DitherRange, ditherAmount);
        s_Material.SetFloat(k_LutBlueTilesX, gridWidth);
        s_Material.SetFloat(k_LutBlueTilesY, gridHeight);
        s_Material.SetFloat(k_GridFractionX, 1f / gridWidth);
        s_Material.SetFloat(k_GridFractionY, 1f / gridHeight);
        s_Material.SetFloat(k_ColorSpaceCompressionFactor, Mathf.Pow(2, colorSpaceCompressionPower));
        s_Material.SetTexture(k_LutTex, LUTTexture);
        s_Material.SetTexture(k_DitherTex, ditherMatrix);

        cmd.GetTemporaryRT(s_TempColorBuffer1, activeWidth, activeHeight, 0, filterMode, colorBuffer.rt.graphicsFormat);
        cmd.GetTemporaryRT(s_TempColorBuffer2, activeWidth, activeHeight, 0, filterMode, colorBuffer.rt.graphicsFormat);

        var scale = RTHandles.rtHandleProperties.rtHandleScale;

        cmd.Blit(colorBuffer, s_TempColorBuffer1, new Vector2(scale.x, scale.y), Vector2.zero, 0, 0);
        cmd.Blit(s_TempColorBuffer1, s_TempColorBuffer2, s_Material);
        cmd.Blit(s_TempColorBuffer2, colorBuffer, new Vector2(1f / scale.x, 1f / scale.y), Vector2.zero, 0, 0);

        cmd.ReleaseTemporaryRT(s_TempColorBuffer1);
        cmd.ReleaseTemporaryRT(s_TempColorBuffer2);
    }
}
