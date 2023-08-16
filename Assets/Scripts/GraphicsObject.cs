using UnityEngine;

public class GraphicsObject : MonoBehaviour
{
    protected Material mat;
    protected Sandbox sandbox;

    internal virtual void Init(Sandbox sandbox)
    {
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        mat = new Material(shader);
        mat.hideFlags = HideFlags.HideAndDontSave;
        //Turn on alpha blending
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // Turn backface culling off
        mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        // Turn off depth writes
        mat.SetInt("_ZWrite", 0);
    }

    internal void SetColor(Color color)
    {
        mat.SetColor("_Color", color);
    }

    protected virtual void OnRenderObject()
    {
    }
}