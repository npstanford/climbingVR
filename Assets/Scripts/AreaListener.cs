using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaListener : MonoBehaviour {
    public bool Activated = false;


    private void OnTriggerEnter(Collider other)
    {
        InteractionAttributes ia = other.gameObject.GetComponent<InteractionAttributes>();
        if (ia!=null)
        {
            if (ia.IsPartOfBody)
            {
                Activated = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractionAttributes ia = other.gameObject.GetComponent<InteractionAttributes>();
        if (ia != null)
        {
            if (ia.IsPartOfBody)
            {
                Activated = false;
            }
        }
    }
}
