using UnityEngine;
using System.Collections;
using TSA;

public class Goal : MonoBehaviour {

    public string NextLevel;
    public RectTransform LevelClearScreen;
    Vector3 rotation = new Vector3(0,0,50);
	
	void Update ()
    {
        this.transform.Rotate((rotation * Time.deltaTime), Space.Self); 
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == ("Player"))
        {
            StartCoroutine(LevelOutro());
            Destroy(col.gameObject);
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            Instantiate(LevelClearScreen, LevelClearScreen.position, LevelClearScreen.rotation);
            //TODO replace destroyed player with an animation dummy
        }           
    }

    IEnumerator LevelOutro()
    {
        float ElapsedTime = 0f;
        float TimeToLerp = .5f;
        Vector3 pos = this.transform.position;
        Vector3 CameraStartPosition = Camera.main.transform.position;
        Quaternion CameraStartRotation = Camera.main.transform.rotation;

        Vector3 CameraTargetPosition = new Vector3(pos.x, pos.y + 5, pos.z + 5);
        Quaternion CameraTargetRotation = Quaternion.Euler(45,-180,0);

        while (ElapsedTime <= TimeToLerp)
        {
            ElapsedTime += Time.deltaTime;
            float perc = (ElapsedTime / TimeToLerp);
            Camera.main.transform.position = Vector3.Lerp(CameraStartPosition, CameraTargetPosition, perc);
            Camera.main.transform.rotation = Quaternion.Lerp(CameraStartRotation, CameraTargetRotation, perc);
            yield return new WaitForEndOfFrame();
        }

        while (true)
        {
            if (Input.GetButtonDown("pause"))
                Application.LoadLevel(NextLevel);
            yield return null;
        }
    }
}
