using UnityEngine;
using UnityEditor;
using System.Collections;

public class AutoTile : MonoBehaviour
{
    /*
    public float scaleToTiles;

    public Dir dir;

    void OnDrawGizmos()
    {
        float scaleX = 1;
        float scaleY = 1;

        switch (dir)
        {
            case Dir.XY:
                scaleX = transform.lossyScale.x;
                scaleY = transform.lossyScale.y;
                break;
            case Dir.ZY:
                scaleX = transform.lossyScale.z;
                scaleY = transform.lossyScale.y;
                break;
            case Dir.XZ:
                scaleX = transform.lossyScale.x;
                scaleY = transform.lossyScale.z;
                break;
        }
        //if (!Application.isEditor || Application.isPlaying)
        //{
            GetComponent<Renderer>().material.SetTextureScale("_MainTex",
                new Vector2(scaleX * scaleToTiles, scaleY * scaleToTiles));
        //}
    }
    */
}

public enum Dir { XY, ZY, XZ }