using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MTV
{
    public Vector2 Axis { get; set; }
    public float Magnitude { get; set; }

    public MTV(Vector2 axis, float magnitude)
    {
        Axis = axis;
        Magnitude = magnitude;
    }

    public Vector3 TranslateVector3D()
    {
        return new Vector3(Axis.x, Axis.y, 0) * Magnitude;
    }

    override public string ToString()
    {
        return Axis.ToString() + " " + Magnitude;
    }
}

public class CollisionChecker : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Check if two projections overlap each other
    //based on their min/max values (x/y values in the vector)
    private bool ProjectionsOverlap(Vector2 projection1, Vector2 projection2)
    {
        return ((projection1.x > projection2.x && projection1.x < projection2.y)
            || (projection1.y > projection2.x && projection1.y < projection2.y));
    }

    private float OverlapSize(Vector2 projection1, Vector2 projection2)
    {
        if (!ProjectionsOverlap(projection1, projection2))
        {
            return 0;
        }

        if (projection1.x > projection2.x)
        {
            return -Mathf.Abs(Mathf.Min(projection1.y, projection2.y) - Mathf.Max(projection1.x, projection2.x));
        }
        else
        {
            return Mathf.Abs(Mathf.Min(projection1.y, projection2.y) - Mathf.Max(projection1.x, projection2.x));
        }
    }

    //checking for collision using SAT
    //as seen here http://www.dyn4j.org/2010/01/sat/
    private MTV CheckForCollision(GameObject object1, GameObject object2)
    {
        Shape shape1 = object1.GetComponent<Shape>();
        Shape shape2 = object2.GetComponent<Shape>();

        if(shape1 == null || shape2 == null || !shape1.Set || !shape2.Set)
        {
            return null;
        }

        float overlap = float.MaxValue;
        Vector2 overlapAxis = Vector2.zero;

        List<Vector2> axes1 = shape1.Normals;
        List<Vector2> axes2 = shape2.Normals;

        //check for overlaps on all the normals of the first shape
        for (int i = 0; i < axes1.Count; i++)
        {
            Vector2 currentAxis = axes1[i];
            Vector2 projection1 = shape1.GetProjection(currentAxis);
            Vector2 projection2 = shape2.GetProjection(currentAxis);

            if (!ProjectionsOverlap(projection1, projection2))
            {
                return null;
            }
            else
            {
                //Check if the current overlap is the smallest so far
                float currentOverlap = OverlapSize(projection1, projection2);

                if (Mathf.Abs(currentOverlap) < Mathf.Abs(overlap))
                {
                    overlap = currentOverlap;
                    overlapAxis = axes1[i];
                }
                else if (Mathf.Abs(currentOverlap) == Mathf.Abs(overlap) && currentOverlap < 0)
                {
                    overlap = currentOverlap;
                    overlapAxis = -axes1[i];
                }
            }
        }

        //check for overlaps on all the normals of the second shape
        for (int i = 0; i < axes2.Count; i++)
        {
            Vector2 currentAxis = axes2[i];
            Vector2 projection1 = shape1.GetProjection(currentAxis);
            Vector2 projection2 = shape2.GetProjection(currentAxis);

            if (!ProjectionsOverlap(projection1, projection2))
            {
                return null;
            }
            else
            {
                //Check if the current overlap is the smallest so far
                float currentOverlap = OverlapSize(projection1, projection2);

                if (Mathf.Abs(currentOverlap) < Mathf.Abs(overlap))
                {
                    overlap = currentOverlap;
                    overlapAxis = axes2[i];
                }
                else if (Mathf.Abs(currentOverlap) == Mathf.Abs(overlap) && currentOverlap < 0)
                {
                    overlap = currentOverlap;
                    overlapAxis = -axes2[i];
                }
            }
        }

        return new MTV(overlapAxis, Mathf.Abs(overlap));
    }

    //Check for collision against every other SOLID object in the scene
    //TODO: only check for objects that are close (maybe use bounding boxes first? and then SAT? or get distance?)
    private void HandleCollisionChecks()
    {
        GameObject[] solidObjects = GameObject.FindGameObjectsWithTag("Solid");
        for (int i = 0; i < solidObjects.Length; i++)
        {
            if (solidObjects[i] == gameObject)
            {
                continue;
            }
            if(Vector3.Distance(gameObject.transform.position, solidObjects[i].transform.position) > 5
                && solidObjects[i].name != "Floor")
            {
                continue;
            }            
            MTV mtv = CheckForCollision(gameObject, solidObjects[i]);
            if (mtv != null)
            {
                //push back if a collision was found
                //also stop the drop of the object
                //TODO: Add a check to see if the object has actually collided with something underneath it
                transform.position -= mtv.TranslateVector3D();
                gameObject.GetComponent<PhysicsHandler>().Hit(solidObjects[i], mtv.Axis);
            }
        }
    }

    /// <summary>
    /// This function is to be invoked externally
    /// </summary>
    /// <returns></returns>
    public Vector2 CheckForCollisions()
    {
        GameObject[] solidObjects = GameObject.FindGameObjectsWithTag("Solid");
        Vector2 totalMTV = Vector2.zero;

        for (int i = 0; i < solidObjects.Length; i++)
        {
            if (solidObjects[i] == gameObject)
            {
                continue;
            }
            MTV mtv = CheckForCollision(gameObject, solidObjects[i]);
            if (mtv != null)
            {
                totalMTV += mtv.Axis;   
            }
        }

        return totalMTV;
    }

    void FixedUpdate()
    {
        HandleCollisionChecks();
    }
}
