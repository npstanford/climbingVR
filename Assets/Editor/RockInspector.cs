using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Rock)), CanEditMultipleObjects]
public class RockInspector : Editor
{

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    /*
    public override void OnInspectorGUI()
    {

    }
    */
    public void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            Debug.Log("Mouse down 377");
          
        }
    }
}
