using UnityEngine;
using System.Collections;
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerCollider : MonoBehaviour {

    private CapsuleCollider cap;

	void Start ()
    {
        cap = this.GetComponent<CapsuleCollider>();
        Vector3 pos = this.transform.position;
        if (Physics.CheckCapsule(pos, pos, cap.radius))  
        {
            Vector3 down = transform.TransformDirection(Vector3.down);
            if (Physics.Raycast(pos, down, cap.radius))
            {

            }
            this.transform.position = new Vector3(pos.x, pos.y, pos.z + cap.radius + .1f);
        }
	}
	
	
}
