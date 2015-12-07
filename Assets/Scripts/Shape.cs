using UnityEngine;
using System.Collections.Generic;

public class Shape : MonoBehaviour
{
    //Enumerable to hold shape types
    public enum ShapeTypeEnum
    {
        Triangle,
        Square,
        Pentagon,
        Hexagon
    }

    //Shape type
    [SerializeField]
    private ShapeTypeEnum ShapeType;

    //Shape size
    [SerializeField]
    private Vector2 ShapeSize;

    //Dictionaries holding the vertices and normals data for all shapes
    //consider moving this to a global/static class
    private Dictionary<ShapeTypeEnum, List<Vector2>> ShapesVerticesData;
    private Dictionary<ShapeTypeEnum, List<Vector2>> ShapesNormalsData;

    //property to get the vertices of a shape, based on its type
    public List<Vector2> Vertices
    {
        get
        {
            return GetShapeVertices();
        }
    }

    //property to get the normals of a shape, based on its type
    public List<Vector2> Normals
    {
        get
        {
            return ShapesNormalsData[ShapeType];
        }
    }

    //Get the bounding box
    public Rect BoundingBox
    {
        get
        {
            MBoundingBox.position = new Vector2(transform.position.x, transform.position.y);
            MBoundingBox.position -= new Vector2(ShapeSize.x / 2, ShapeSize.y / 2);
            return MBoundingBox;
        }
    }

    private Rect MBoundingBox;

    private bool MSet = false;

    public bool Set
    {
        get
        {
            return MSet;
        }
    }

	// Use this for initialization
	void Start ()
    {
        MSet = false;
        CreateShapesVertices();
        CreateShapesNormals();
        CalculateBoundingBox();
        MSet = true;
	}

    //Initialize the vertices for all the shapes
    //hardcoded for now
    private void CreateShapesVertices()
    {
        ShapesVerticesData = new Dictionary<ShapeTypeEnum, List<Vector2>>();
        //triangle
        ShapesVerticesData.Add(ShapeTypeEnum.Triangle, new List<Vector2>());
        //square
        List<Vector2> squareVertices = new List<Vector2>(new Vector2[]
        {
            new Vector2(-0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, -0.5f),
            new Vector2(-0.5f, -0.5f)
        });
        ShapesVerticesData.Add(ShapeTypeEnum.Square, squareVertices);
        //pentagon
        ShapesVerticesData.Add(ShapeTypeEnum.Pentagon, new List<Vector2>());
        //hexagon
        ShapesVerticesData.Add(ShapeTypeEnum.Hexagon, new List<Vector2>());
    }

    //Create the normals for all the shapes
    private void CreateShapesNormals()
    {
        ShapesNormalsData = new Dictionary<ShapeTypeEnum, List<Vector2>>();   
        //go through all the vertices lists and generate normals for each shape
        foreach(var keyValuePair in ShapesVerticesData)
        { 
            if(keyValuePair.Value.Count != 0)
            {
                List<Vector2> currentVertices = keyValuePair.Value;
                List<Vector2> currentNormals = new List<Vector2>();
                for (int i = 0; i < currentVertices.Count; i++)
                {
                    Vector2 currentDiff = currentVertices[i] - (i == currentVertices.Count - 1 ? currentVertices[0] : currentVertices[i + 1]);
                    Vector2 currentNormal = new Vector2(-currentDiff.y, currentDiff.x).normalized;
                    currentNormals.Add(currentNormal);
                }
                ShapesNormalsData.Add(keyValuePair.Key, currentNormals);
            }
        }
    }

    //Get the list of vertices and add position and size to them 
    //to return the real vertices
    private List<Vector2> GetShapeVertices()
    {
        Vector3 realPosition = transform.position;
        Vector2 position = new Vector2(realPosition.x, realPosition.y);

        List<Vector2> vertices = new List<Vector2>();
        
        for (int i = 0; i < ShapesVerticesData[ShapeType].Count; i++)
        {
            Vector2 currentVertex = new Vector2(ShapesVerticesData[ShapeType][i].x, ShapesVerticesData[ShapeType][i].y);
            currentVertex.x *= ShapeSize.x;
            currentVertex.y *= ShapeSize.y;
            currentVertex += position;
            vertices.Add(currentVertex);            
        }
                
        return vertices;
    }

    //return the projection of the shape on the given axis
    //as seen http://www.dyn4j.org/2010/01/sat/#sat-projshape
    public Vector2 GetProjection(Vector2 axis)
    {
        List<Vector2> vertices = GetShapeVertices();
        float min = Vector2.Dot(axis, vertices[0]);
        float max = min;

        for (int i = 1; i < vertices.Count; i++)
        {
            float currentDot = Vector2.Dot(axis, vertices[i]);
            if (currentDot < min)
            {
                min = currentDot;
            }
            else if (currentDot > max)
            {
                max = currentDot;
            }
        }

        Vector2 projection = new Vector2(min, max);

        return projection;
    }

    //Calculate the bounding box of the shape
    private void CalculateBoundingBox()
    {        
        float minx = float.MaxValue;
        float maxx = -float.MaxValue;
        float miny = float.MaxValue;
        float maxy = -float.MaxValue;

        for (int i = 0; i < ShapesVerticesData[ShapeType].Count; i++)
        {
            Vector2 currentVertex = new Vector2(ShapesVerticesData[ShapeType][i].x, ShapesVerticesData[ShapeType][i].y);
            currentVertex.x *= ShapeSize.x;
            currentVertex.y *= ShapeSize.y;
            minx = Mathf.Max(currentVertex.x, minx);
            maxx = Mathf.Min(currentVertex.x, maxx);
            miny = Mathf.Max(currentVertex.y, miny);
            maxy = Mathf.Min(currentVertex.y, maxy);
        }

        MBoundingBox = new Rect(minx, miny, maxx - minx, maxy - miny);
    }
    
    /// <summary>
    /// Draw the shape
    /// </summary>
    public void DrawShapeGL()
    {
        List<Vector2> currentVertices = GetShapeVertices();       
        GL.Begin(GL.LINES);
        GL.Vertex(currentVertices[0]);        
        for (int i = 0; i < currentVertices.Count; i++)
        {
            Vector2 currentVertex = currentVertices[i];
            //Vector2 nextVertex = currentVertices[i == currentVertices.Count - 1 ? 0 : i + 1];
            GL.Vertex(new Vector3(currentVertex.x, currentVertex.y, 0));
            GL.Vertex(new Vector3(currentVertex.x, currentVertex.y, 0));
        }
        GL.Vertex(currentVertices[0]);
        GL.End();
    }  

    /// <summary>
    /// A function to draw the shape, for debugging purposes
    /// </summary>
    private void DrawShape()
    {
        for (int i = 0; i < Vertices.Count; i++)
        {
            Vector2 currentVertex = Vertices[0];
            Vector2 nextVertex = Vertices[i == Vertices.Count - 1 ? 0 : i + 1];
            Debug.DrawLine(new Vector3(currentVertex.x, currentVertex.y, 0), new Vector3(nextVertex.x, nextVertex.y, 0));
        }
    }

    void Update()
    {   
    }
}
