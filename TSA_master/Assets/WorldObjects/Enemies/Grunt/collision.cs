using UnityEngine;
using System.Collections;

public class collision : MonoBehaviour {

    void OnCollisionEnter(Collision col) //check to see if noise has been heard
    {
        Debug.Log("heard it");
    }
}
