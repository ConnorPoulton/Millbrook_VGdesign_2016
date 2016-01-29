using UnityEngine;
using System.Collections;

public class SoundTest : MonoBehaviour
{
    Transform ChildTransform;
    bool MakeSound = false;

    void Start()
    {
        ChildTransform = this.transform.GetChild(0);
    }

	void Update ()
    {
        if (Input.GetButtonDown("Jump"))
        {
            MakeSound = !MakeSound;
        }

        if (MakeSound == true)
        {
            ChildTransform.position = this.transform.position;
        }
        else
        {
            Vector3 hidden = new Vector3(0, 0, -100);
            ChildTransform.position = hidden;
        }
        MakeSound = false;
	}
}
