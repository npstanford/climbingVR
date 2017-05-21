using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemyFacePivot : MonoBehaviour {

    public ChargingEnemy ce;
    public float faceRotateSpeed;

    public void Update()
    {
        if (ce.attack && !ce.IsStunned)
        {

            float step = ce.rotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, ce.rot, step);
            transform.eulerAngles = new Vector3(Mathf.Clamp(transform.rotation.eulerAngles.x, -20f, 20f),
                transform.rotation.eulerAngles.y, Mathf.Clamp(transform.rotation.eulerAngles.z, -20f, 20f));
        }
    }

}
