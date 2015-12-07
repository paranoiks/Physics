using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicsHelpers : MonoBehaviour {

    public static float GravityConst = 100f;

    private Platform[] Platforms;

    void Start()
    {
        Platforms = (Platform[])GameObject.FindObjectsOfType(typeof(Platform));
    }

    public bool CheckVisibility(Vector3 point1, Vector3 point2)
    {
        bool flag = true;
        foreach (var platform in Platforms)
        {
            if (PhysicsHelpers.RayHit(point1, point2, platform.gameObject.GetComponent<Shape>()))
            {
                flag = false;
            }
        }
        return flag;
    }

    //Check if two rectangles are overlapping
    //TODO: add body
	public static bool RectanglesOverlaping(Rect rect1, Rect rect2)
    {
        return false;
    }

    public static Vector2 GetVector2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 GetVector3(Vector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    public static float DistanceBetweenObjects(GameObject g1, GameObject g2)
    {
        return Vector3.Distance(g1.transform.position, g2.transform.position);
    }

    public static bool RayHit(Vector2 a1, Vector2 a2, Shape shape)
    {
        List<Vector2> shapeVertices = shape.Vertices;
        for (int i = 0; i < shapeVertices.Count - 1; i++)
        {
            if (Intersection(a1, a2, shapeVertices[i], shapeVertices[i + 1]))
            {
                return true;
            }
        }
        return false;
    }

    public static bool Intersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        if(Mathf.Sign(Determinant3(a1, b1, b2)) != Mathf.Sign(Determinant3(a2, b1, b2))
            && Mathf.Sign(Determinant3(b1, a1, a2)) != Mathf.Sign(Determinant3(b2, a1, a2)))
            {
            return true;
        }
        return false;
    }

    public static float Determinant3(Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float d = 0;
        d += v1.x * v2.y + v1.y * v3.x + v2.x * v3.y;
        d -= v3.x * v2.y + v3.y * v1.x + v2.x * v1.y;
        return d;
    }

    
}
