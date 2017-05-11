using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour
{

    public TeleportLocation LastTeleportLocation;
    public GameObject Player;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Teleport()
    {
        if (LastTeleportLocation != null)
        {
            Player.transform.position = LastTeleportLocation.transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Teleport();
        }
    }


}
