using UnityEngine;

public class VectorObject : GraphicsObject
{
    private Vector3 origin;
    private Vector3 scaledDir;
    private Vector3 arrowP1;
    private Vector3 arrowP2;

    public void SetData(Vector3 origin, Vector3 scaledDir)
    {
        this.origin = origin;
        this.scaledDir = scaledDir;

        const float backwardsOffset = 0.1f;
        const float arrowSeparation = 0.05f;

        Vector3 perp = Vector3.Cross(scaledDir, Vector3.forward).normalized;
        Vector3 tip = origin + scaledDir;
        Vector3 dirNrm = scaledDir.normalized;
        arrowP1 = tip - dirNrm * backwardsOffset + perp * arrowSeparation;
        arrowP2 = tip - dirNrm * backwardsOffset - perp * arrowSeparation;
    }

    protected override void OnRenderObject()
    {
        base.OnRenderObject();

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        mat.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Vertex(origin);
        GL.Vertex(origin + scaledDir);
        GL.End();
        GL.Begin(GL.TRIANGLES);
        GL.Vertex(arrowP1);
        GL.Vertex(arrowP2);
        GL.Vertex(origin + scaledDir);
        GL.End();
        GL.PopMatrix();
    }
}