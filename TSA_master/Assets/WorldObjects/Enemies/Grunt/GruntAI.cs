using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TSA;

//written by Connor Poulton

[System.Serializable] //serialize to allow compatibility in Unity editor
public class Node
{
    public Transform target;
    public float degreeTurn;
    public int waitTime;
}

[RequireComponent(typeof(NavMeshAgent))]
public class GruntAI : MonoBehaviour
{

    //navigation system values
    public List<Node> nodes; //set value in the Unity editor
    NavMeshAgent agent;
    int CurrentNode = 0; 
    int MaxNode; //stores last element in list, used to loop back to start of patrol
    float timeRotating; //used to store the exact time passed while exectuing the rotateGrunt coroutine, keeps timing consistent

    //state machine variables
    enum States { PathToNextNode, WaitAtNode, InvestigateSound, ReturnToPatrol, PlayerSeen };
    States currentstate;
    States laststate = 0;

    //refrence variables
    private Vector3 lastPatrolPoint;
    public RectTransform UICanvas; 

    //----------------initialization---------------------------------------


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 5;
        
        GameObject levelGeometry = GameObject.FindWithTag("LevelGeometry"); //make sure you only have one object in your scene with this tag!

        if (levelGeometry == null)
        { Debug.Log("please apply the LevelGeometry tag to your scenes level prefab"); }


        try
        { this.transform.position = nodes[0].target.position; }
        catch
        { Debug.Log(this.name + " has no nodes attached"); }

        MaxNode = nodes.Count - 1;
        agent.stoppingDistance = 1;

        NewState(States.WaitAtNode);
        NextState();

    }



    //----------------------AI states------------------------------------------------------



    //----------WaitAtNode

    private IEnumerator WaitAtNode()
    {
        
        if (nodes[CurrentNode].degreeTurn != 1)
        {
            StartCoroutine(RotateGrunt(nodes[CurrentNode].degreeTurn));
        }

        yield return new WaitForSeconds(nodes[CurrentNode].waitTime + timeRotating); //wait for specified time plus time it takes to rotate
        StopCoroutine(RotateGrunt(nodes[CurrentNode].degreeTurn));

        StartCoroutine(RotateGrunt(GetYAngleToward(nodes[IterateToNextNode()].target.position))); //rotate to next target
        yield return new WaitForSeconds(timeRotating + 0.5f); //wait for rotation to finish
        StopCoroutine(RotateGrunt(GetYAngleToward(nodes[IterateToNextNode()].target.position)));

        CurrentNode = IterateToNextNode();
        NewState(States.PathToNextNode);
        NextState();

        yield return null;
    }



    //------------PathToNextNode

    private IEnumerator PathToNextNode()
    {

        agent.destination = nodes[CurrentNode].target.position;
        

        while (currentstate == States.PathToNextNode)
        {
            if (ArrivedAtNode())
            {

                States nextstate = States.WaitAtNode; NewState(nextstate);
            }
            yield return null;
        }
       
        NextState();
    }



    //------------ReturnToPatrol

    private IEnumerator ReturnToPatrol()
    {
        agent.destination = this.transform.position;
        
        StartCoroutine(RotateGrunt(GetYAngleToward(lastPatrolPoint)));
        yield return new WaitForSeconds(timeRotating + 1);
        StopCoroutine(RotateGrunt(GetYAngleToward(lastPatrolPoint)));

        agent.destination = lastPatrolPoint;

        while (currentstate == States.ReturnToPatrol)
        {
            if (ArrivedAtNode())
            {
                agent.destination = this.transform.position;

                StartCoroutine(RotateGrunt(GetYAngleToward(nodes[CurrentNode].target.position)));
                yield return new WaitForSeconds(timeRotating + 1);
                StopCoroutine(RotateGrunt(GetYAngleToward(nodes[CurrentNode].target.position)));

                States nextstate = States.PathToNextNode; NewState(nextstate);
            }

            yield return null;
        }
                
        NextState();
    }

    //------------InvestigateSound

    private IEnumerator InvestigateSound()
    {      
        lastPatrolPoint = this.transform.position;
        Vector3 target = agent.destination;
        agent.destination = this.transform.position;

        StartCoroutine(RotateGrunt(GetYAngleToward(target))); //rotate to next target        
        yield return new WaitForSeconds(timeRotating + 3);
        StopCoroutine(RotateGrunt(GetYAngleToward(target)));

        agent.destination = target;

        while (currentstate == States.InvestigateSound)
        {
            if (ArrivedAtNode())
            {
                
                yield return new WaitForSeconds(2);
                StartCoroutine(RotateGrunt(GetYAngleToward(nodes[CurrentNode].target.position))); //rotate to next target
                yield return new WaitForSeconds(timeRotating);
                StopCoroutine(RotateGrunt(GetYAngleToward(nodes[CurrentNode].target.position)));                
                States nextstate = States.ReturnToPatrol; NewState(nextstate);
            }
            yield return null;
        }
        
        NextState();
    }

    //--------------PlayerSeen

    private IEnumerator PlayerSeen()
    {        
        Vector3 playerPosition = agent.destination;
        agent.destination = this.transform.position;
        

        StartCoroutine(RotateGrunt(GetYAngleToward(playerPosition)));         
        yield return new WaitForSeconds(timeRotating);
        StopCoroutine(RotateGrunt(GetYAngleToward(playerPosition)));
        
        yield break;
    }



    //---------------state machine triggers-----------------------------------------

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "SoundField") //trigger investigate sound
        {
            agent.destination = col.gameObject.transform.parent.position;
            States nextstate = States.InvestigateSound; NewState(nextstate); //start InvestigateSound Coroutine
            NextState();
        }

        if (col.gameObject.tag == "Player") //raycast to see if found player
        {
            Vector3 origin = this.transform.position;
            Vector3 destination = col.gameObject.transform.position;
            Debug.Log("testing");
            Debug.DrawLine(origin, destination, Color.red, 10, false);
            RaycastHit hit;
            if (Physics.Linecast(origin, destination, out hit))
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    Debug.Log(hit.transform.gameObject.name);
                    ResourceManager.CallplayerFound(); //trigger event
                    agent.destination = col.gameObject.transform.position;
                    Instantiate(UICanvas, this.transform.position, UICanvas.rotation);
                    States nextState = States.PlayerSeen; NewState(nextState); //start player found coroutine
                    NextState();
                }
            }                
        }
    }


    //---------------coroutines-----------------------------------------------------

    public IEnumerator RotateGrunt(float targetRotation)
    {
        float ElapsedTime = 0f;
        float angle;
        float time = .5f;
        timeRotating = 0f;
        while (ElapsedTime <= time)
        {
            ElapsedTime += Time.deltaTime;
            float perc = (ElapsedTime / time);
            angle = Mathf.LerpAngle(this.transform.eulerAngles.y, targetRotation, perc);
            transform.eulerAngles = new Vector3(0, angle, 0);
            yield return new WaitForEndOfFrame();
        }
        timeRotating = ElapsedTime;
        yield return null;
    }





    //---------------state machine functions-----------------------------------------


    //call in the termination statement of an IEnumerator coroutine
    void NextState()
    {
        string stop = laststate.ToString();
        string start = currentstate.ToString();
        StopCoroutine(stop);
        StartCoroutine(start);
    }

    //call to trigger IEnumerator coroutine termination
    void NewState(States nextstate)
    {
        laststate = currentstate;
        currentstate = nextstate;
    }



    //-------------------functions---------------------------

    public float GetYAngleToward(Vector3 target)
    {
        var trans = this.transform.position;
        Vector3 direction = (target - trans).normalized;
        Quaternion q = Quaternion.LookRotation(direction);
        Vector3 angle = q.eulerAngles;
        return angle.y;
    }

    public int IterateToNextNode() //returns next Node in nodes
    {
        int returnVale;
        returnVale = (CurrentNode == MaxNode) ? 0 : returnVale = CurrentNode + 1;
        return returnVale;
    }

    public bool ArrivedAtNode() //check to see if agent has arrived at destination and stopped calculating path
    {
        
        if ( Vector3.Distance(agent.destination, agent.transform.position) <= agent.stoppingDistance)
         {
             if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
             {
                
                return true;
             }
         }
 
         return false;
    }
}

