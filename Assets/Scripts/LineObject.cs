using UnityEngine;

public class LineObject : GraphicsObject
{
    private Vector3 p1;
    private Vector3 p2;

    public void SetPoints(Vector3 p1, Vector3 p2)
    {
        this.p1 = p1;
        this.p2 = p2;
    }

    protected override void OnRenderObject()
    {
        base.OnRenderObject();

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        mat.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Vertex(p1);
        GL.Vertex(p2);
        GL.End();
        GL.PopMatrix();
    }
}