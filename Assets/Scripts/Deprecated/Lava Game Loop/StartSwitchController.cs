using UnityEngine;
using System.Collections;

public class StartSwitchController : MonoBehaviour {
    public enum SwitchType { Start, End};

    public SwitchType st;
    public GameController gc;

    public Material PracticeMaterial;
    public Material GameMaterial;

    private MeshRenderer mr;

	// Use this for initialization
	void Start () {
        mr = transform.GetComponent<MeshRenderer>();
        if (st == SwitchType.Start)
            mr.material = PracticeMaterial;
        else if (st == SwitchType.End)
            mr.material = GameMaterial;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (st == SwitchType.Start)
        {
            gc.StartGame();
            mr.material = GameMaterial;
        } else if (st == SwitchType.End)
        {
            gc.WinGame();
            mr.material = PracticeMaterial;
        }
    }
}
