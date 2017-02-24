using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

    public enum EnemyStates { Attacking, Stunned };

    public EnemyStates EnemyState;
    public GameController gc;
    public float StunTime;

    public Material AttackingMaterial;
    public Material StunnedMaterial;

    // Use this for initialization
    void Start() {
        EnemyState = EnemyStates.Attacking;
    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Controller"))
        {
            if (gc.PlayerState == GameController.PlayerStates.Normal && EnemyState == EnemyStates.Attacking)
            {
                gc.PlayerInjured();
            }
        }

        if (collider.gameObject.CompareTag("Bullet"))
        {
            StartCoroutine("EnemyStunned");

        }
    }

    IEnumerator EnemyStunned()
    {
        EnemyState = EnemyStates.Stunned;
        gameObject.layer = LayerMask.NameToLayer("Grip");
        gameObject.GetComponent<MeshRenderer>().material = StunnedMaterial;
        ScriptedMovement sm = gameObject.GetComponent<ScriptedMovement>();
        sm.PauseMovement();
        yield return new WaitForSecondsRealtime(StunTime);

        EnemyState = EnemyStates.Attacking;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        gameObject.GetComponent<MeshRenderer>().material = AttackingMaterial;
        sm.StartMovement();
        yield return null;
    }

}
