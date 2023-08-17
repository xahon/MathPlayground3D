using UnityEngine;

public class SphereObject : GraphicsObject
{
    // private Vector3 origin;
    // private float radius;

    private GameObject primitive;

    private void Awake()
    {
        primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        primitive.transform.parent = transform;
    }

    internal override void Init(Sandbox sandbox)
    {
        base.Init(sandbox);
        primitive.GetComponent<MeshRenderer>().material = mat;
    }

    public void SetData(Vector3 origin, float radius)
    {
        // this.origin = origin;
        // this.radius = radius;

        primitive.transform.localPosition = origin;

        if (Mathf.Approximately(radius, 0))
        {
            primitive.SetActive(false);
        }
        else
        {
            primitive.transform.localScale = new Vector3(radius, radius, radius);
            primitive.SetActive(true);
        }
    }
}