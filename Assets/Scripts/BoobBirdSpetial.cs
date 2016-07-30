using UnityEngine;
using System.Collections;

public class BoobBirdSpetial : MonoBehaviour {
    public bool bSpetial = true;
    private Animator animator;
	// Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }
	// Update is called once per frame
	void Update () {
        if(bSpetial)
        {
            animator.SetBool("Spectial", true);
        }

    }
}
