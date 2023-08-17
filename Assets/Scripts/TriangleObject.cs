using UnityEngine;

public class TriangleObject : GraphicsObject
{
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;
    private bool filled;

    public void SetData(Vector3 p1, Vector3 p2, Vector3 p3, bool filled)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
        this.filled = filled;
    }

    protected override void OnRenderObject()
    {
        base.OnRenderObject();

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        mat.SetPass(0);

        if (filled)
        {
            GL.Begin(GL.TRIANGLES);
            GL.Vertex(p1);
            GL.Vertex(p2);
            GL.Vertex(p3);
            GL.Vertex(p1);
            GL.Vertex(p3);
            GL.Vertex(p2);
            GL.End();
        }
        else
        {
            GL.Begin(GL.LINES);
            GL.Vertex(p1);
            GL.Vertex(p2);
            GL.Vertex(p2);
            GL.Vertex(p3);
            GL.Vertex(p3);
            GL.Vertex(p1);
            GL.End();
        }
        GL.PopMatrix();
    }
}