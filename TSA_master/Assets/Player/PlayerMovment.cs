using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TSA;

public class PlayerMovment : MonoBehaviour {

    
    public float p_CamZoomYFromPlayer;
    float CamZoomY; //actual camera zoom based on player position
    public float p_CamDefaultYFromPlayer;
    float CamDefaultY;
    public float p_CameraDemoSpeed;
    public float p_CameraZoomSpeed;
    float CamYTarget;
    Vector3 CamTarget;
    bool CamBoundToPlayer = true;

    public float p_SprintSpeed;
    public float p_BaseSpeed; 
    float SpeedFromBase; //difference in player SpeedFromBase from base
    float CurrentSpeed; // SpeedFromBase + baseSpeed
    float MoveDown = 0f; //velocity caused by p_Gravity
    public float p_Gravity = 9.8f;
    bool IsSprinting = false;
    

    public RectTransform UICanvas;
    Transform SoundBounds; 
    CharacterController cc;
    


    // Use this for initialization
    void Start ()
    {
        Vector3 trans = this.transform.position;
        SoundBounds = this.transform.GetChild(0);

        CamDefaultY = this.transform.position.y + p_CamDefaultYFromPlayer;

        Camera.main.transform.position = new Vector3(trans.x, CamDefaultY, trans.z);
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));

        cc = this.GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        ResourceManager.playerFound += OnPlayerFound;
    }

    void OnDisable()
    {
        ResourceManager.playerFound -= OnPlayerFound;
    }
	
	
 
    void Update()
    {
        Vector3 camPos = Camera.main.transform.position;
        Vector3 trans = this.transform.position;
        CurrentSpeed = (SpeedFromBase + p_BaseSpeed);  
              
        IsSprinting = Input.GetButton("sprint") ? true : false;

        if (IsSprinting == true)
        { SprintState(trans);}
        else { WalkState(trans); }
                   
        ApplyMovement();
        AdjustCamera(camPos, trans);

        if (Input.GetButtonDown("Jump"))
        {SoundBounds.position = trans;}
        else { SoundBounds.position = ResourceManager.OUTOFBOUNDS; }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


    }


    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "CameraTrigger")
        {
            CamBoundToPlayer = false;
            Vector3 target = col.transform.parent.GetChild(1).transform.position;
            CamTarget = target;
        }
        else {

            CamBoundToPlayer = true;
        }
    }

    //---event functions-------------

    void OnPlayerFound()
    {
        this.GetComponent<PlayerMovment>().enabled = false;
    }


    //-----functions-----------------
    void ApplyMovement()
    {
        float moveHorizontal = 0f;
        float moveVertical = 0f;
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        if (cc.isGrounded)
        {          
            MoveDown = 0;
        }
        else
        {
            MoveDown -= p_Gravity * Time.deltaTime;
        }
        
        Vector3 movement = new Vector3(moveHorizontal, MoveDown, moveVertical);
        cc.Move((movement * (SpeedFromBase * Time.deltaTime)));
    }


    void AdjustCamera(Vector3 camPos, Vector3 trans)
    {
        float camlag = (5 * Time.deltaTime);
        float zoomSpeed = 1;        
        if (CamBoundToPlayer == true)          
            zoomSpeed = p_CameraZoomSpeed;
        

        Vector3 velocity = ((CamTarget - camPos));     
        Vector3 newcampos = camPos += new Vector3((velocity.x * camlag), ((velocity.y) * (zoomSpeed)), (velocity.z * Time.deltaTime));
        if (CamBoundToPlayer == true)
        {
            float CamYOut = Mathf.Abs(camPos.y - trans.y);
            newcampos = ResourceManager.MakeCameraVecInBounds(newcampos, CamYOut);
        }
            
        Camera.main.transform.position = newcampos;        
        return;        
    }


    void WalkState(Vector3 trans)
    {
        if (SpeedFromBase > p_BaseSpeed)
        { SpeedFromBase -= .1f; }
        else if (SpeedFromBase < p_BaseSpeed)
        { SpeedFromBase = p_BaseSpeed; }

        if (CamBoundToPlayer == true)
            SetCameraDefaultZoom(trans);
    }

    void SprintState(Vector3 trans)
    {
        if (SpeedFromBase < p_SprintSpeed)        
            SpeedFromBase += .1f;
        
        if (CamBoundToPlayer == true)
            SetCameraZoomIn(trans);
    }

    void SetCameraDefaultZoom(Vector3 trans)
    {
        CamYTarget = p_CamDefaultYFromPlayer;
        CamDefaultY = CamYTarget + trans.y;
        Debug.Log(p_CamDefaultYFromPlayer);       
        CamTarget = new Vector3(
            trans.x,
            CamDefaultY,
            trans.z
            );
    }

    void SetCameraZoomIn(Vector3 trans)
    {
        CamYTarget = p_CamZoomYFromPlayer;
        CamZoomY = CamYTarget + trans.y;        
        CamTarget = new Vector3(
            trans.x,
            CamZoomY,
            trans.z
            );
    }

    

}
