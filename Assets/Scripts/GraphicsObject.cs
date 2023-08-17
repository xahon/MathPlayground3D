using UnityEngine;

public class GraphicsObject : MonoBehaviour
{
    protected Material mat;
    protected Sandbox sandbox;

    internal virtual void Init(Sandbox sandbox)
    {
        this.sandbox = sandbox;

        Shader shader = Shader.Find("Unlit/DefaultShader");
        mat = new Material(shader);
        mat.hideFlags = HideFlags.HideAndDontSave;

        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
        mat.SetInt("_ZWrite", 1);
    }

    internal virtual void SetColor(Color color)
    {
        mat.SetColor("_Color", color);
    }

    protected virtual void OnRenderObject()
    {
    }
}