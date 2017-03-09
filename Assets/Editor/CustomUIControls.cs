using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rock)), CanEditMultipleObjects]
public class CustomUIControls : Editor //the example had this as "editor window"
{

    public static bool AddRock = true;
    public static string RockPrefabPath = "Assets/Prefabs/GrippingStone.prefab";
    public static GameObject RockPrefab;

    private static Vector3 pointSnap = Vector3.one * 0.1f;


    [MenuItem("Window/Scene GUI/Enable")]
    public static void Enable()
    {
        SceneView.onSceneGUIDelegate += ClickToAddRocks;
        RockPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(RockPrefabPath, typeof(GameObject));
    }

    [MenuItem("Window/Scene GUI/Disable")]
    public static void Disable()
    {
        SceneView.onSceneGUIDelegate -= ClickToAddRocks;
        Debug.Log("Scene GUI : Disabled");
    }


    private static void ClickToAddRocks(SceneView sceneview) //used to be onscenegui
    {
        Handles.BeginGUI();

        AddRock = GUI.Toggle(new Rect(25, 25, 100, 30), AddRock, "Add Rock");

        Handles.EndGUI();

        if (Event.current.type == EventType.MouseDown && Event.current.control && AddRock)
        {
            Debug.Log("Mouse down");
            Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(worldRay, out hitInfo))
            {
                Debug.Log("Attempting to add rock");
                GameObject rock = GameObject.Instantiate(RockPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                rock.transform.parent = hitInfo.transform;
                rock.name = "Rock";
            }

        }
    }

    public void OnSceneGUI()
    {
        Rock rock = target as Rock;
        Transform rockTransform = rock.transform;


        Handles.color = Color.magenta;

        
        EditorGUI.BeginChangeCheck();
        Vector3 oldPoint = rockTransform.position;
        Vector3 newPoint = Handles.PositionHandle(oldPoint, rockTransform.rotation);
 

        if (EditorGUI.EndChangeCheck())
        {
            Vector3 rayOrigin = newPoint + rockTransform.forward * 0.05f;
            Undo.RecordObject(target, "Slid Rock"); //todo figure out how undo works
            Debug.Log("attempting to stick rock to wall");

            LayerMask mask = ~(1 << 10); //grip layer, so it should see everything but grip
            RaycastHit hitInfo;

            if (Physics.Raycast(rayOrigin, -rockTransform.forward, out hitInfo, 1.0f, mask))
            {
                Debug.Log("Got ray cast from sliding rock: " + hitInfo.transform.name);
                rock.transform.parent = null;
                rock.transform.position = hitInfo.point;
                rock.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
                rock.transform.parent = hitInfo.transform;
            }


        }



    }
}