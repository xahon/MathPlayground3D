using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting;


public class Sandbox : MonoBehaviour
{
    private List<GraphicsObject> objects = new List<GraphicsObject>();
    private Color nextColor = Color.red;

    private bool isInitStep = true;
    private int internalCounter = 0;

    public void ResetState()
    {
        foreach (GraphicsObject obj in objects)
        {
            Destroy(obj.gameObject);
        }
        objects.Clear();
        internalCounter = 0;
        isInitStep = true;
    }

    [Preserve]
    [UsedImplicitly]
    public void SetColor(float r, float g, float b, float a)
    {
        nextColor = new Color(r, g, b, a);
    }

    [Preserve]
    [UsedImplicitly]
    public void DrawLine(Vector3 p1, Vector3 p2)
    {
        LineObject obj = null;
        string name = "Line " + p1 + ", " + p2;

        if (isInitStep)
        {
            obj = new GameObject(name).AddComponent<LineObject>();
            obj.transform.parent = transform;
            obj.Init(this);
            objects.Add(obj);
        }
        else
        {
            obj = (LineObject)objects[internalCounter];
            obj.gameObject.name = name;
            internalCounter = (internalCounter + 1) % objects.Count;
        }

        obj.SetColor(nextColor);
        obj.SetPoints(p1, p2);
    }

    [Preserve]
    [UsedImplicitly]
    public void DrawVector(Vector3 origin, Vector3 scaledDir)
    {
        VectorObject obj = null;
        string name = "Vector {" + origin + "} " + scaledDir;

        if (isInitStep)
        {
            obj = new GameObject(name).AddComponent<VectorObject>();
            obj.transform.parent = transform;
            obj.Init(this);
            objects.Add(obj);
        }
        else
        {
            obj = (VectorObject)objects[internalCounter];
            obj.gameObject.name = name;
            internalCounter = (internalCounter + 1) % objects.Count;
        }

        obj.SetColor(nextColor);
        obj.SetData(origin, scaledDir);
    }

    [Preserve]
    [UsedImplicitly]
    public void DrawTriangle(Vector3 p1, Vector3 p2, Vector3 p3, bool filled)
    {
        TriangleObject obj = null;
        string name = "Triangle " + p1 + ", " + p2 + ", " + p3 + ", " + filled;

        if (isInitStep)
        {
            obj = new GameObject(name).AddComponent<TriangleObject>();
            obj.transform.parent = transform;
            obj.Init(this);
            objects.Add(obj);
        }
        else
        {
            obj = (TriangleObject)objects[internalCounter];
            obj.gameObject.name = name;
            internalCounter = (internalCounter + 1) % objects.Count;
        }

        obj.SetColor(nextColor);
        obj.SetPoints(p1, p2, p3, filled);
    }

    public void BeginFrame()
    {
    }

    public void EndFrame()
    {
        isInitStep = false;
    }
}
