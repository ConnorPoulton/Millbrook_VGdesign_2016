using UnityEngine;
using System.Collections;
using TSA;
public class test : MonoBehaviour {

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(ResourceManager.SceneLLBounds, 1);
        Gizmos.DrawSphere(ResourceManager.SceneURBounds, 1);

    }

    
}
