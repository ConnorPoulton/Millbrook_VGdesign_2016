using UnityEngine;
using System.Collections;
using TSA;

public class LevelSetUp : MonoBehaviour {

	
	void Start ()
    {
        
        ResourceManager.SceneURBounds = new Vector3(0, 0, 0);

        if (this.transform.FindChild("LLBounds") != null)
        { ResourceManager.SceneLLBounds = this.transform.FindChild("LLBounds").transform.position; }
        else
        {
            ResourceManager.SceneLLBounds = new Vector3(0, 0, 0);
            Debug.Log("Your Scene is lacking a lower left bound. Make sure you made your level with LevelSetUpV2 and LLBounds has not been tampered with"); }


        if (this.transform.FindChild("URBounds") != null)
        { ResourceManager.SceneURBounds = this.transform.FindChild("URBounds").transform.position; }
        else
        {
            ResourceManager.SceneURBounds = new Vector3(0, 0, 0);
            Debug.Log("Your Scene is lacking a upper right bound. Make sure you made your level with LevelSetUpV2 and LLBounds has not been tampered with");
        }

    }
	
}
