using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SittingScript2 : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component

    // Start is called before the first frame update
    void Start()
    {
        // Check if animator is not null
        if (animator != null)
        {
            // Change the animator state to "rig|BlockyPerson_Pose_12"
            animator.Play("rig|BlockyPerson_Pose_11", -1, 0f);
        }
        else
        {
            Debug.LogError("Animator component is not assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
