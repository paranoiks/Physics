using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//A class to make printing of Dictionaries and Lists easier
public class DebugHelpers : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    //accepts dicionaries with generic types
    //only works for vectors
    //this function is to be expanded when necessary
    public static void PrintDictionary<Type1, Type2>(Dictionary<Type1, Type2> dict)
    {
        foreach (var keyValuePair in dict)
        {
            string s = "";
            s += keyValuePair.Key.ToString() + " ";
            if (keyValuePair.Value is List<Vector2>)
            {
                foreach (Vector2 v in (keyValuePair.Value as List<Vector2>))
                {
                    s += "(" + v.x + "," + v.y + ") ";
                }
            }
            Debug.Log(s);
        }
    }

    //accepts lists with generic tpyes
    //only works for vectors
    //this function is to be expanded when necessary
    public static void PrintList<Type1>(List<Type1> list)
    {
        if(list is List<Vector2>)
        {
            string s = "";
            foreach(Vector2 v in (list as List<Vector2>))
            {
                s += v.ToString() + " ";
            }
            Debug.Log(s);
        }
        if(list is List<Node>)
        {
            string s = "";
            foreach(Node n in (list as List<Node>))
            {
                s += n.gameObject.transform.position.ToString() + " ";
            }
            Debug.Log(s);
        }
    }
}
