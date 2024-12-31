using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
    //Pellet variables
    [SerializeField] private Transform target;
    public int pelletCount;
    public GameObject food;
    public List<GameObject> spawnedPelletsList = new List<GameObject>();

    //Agent variables
    [SerializeField] private float moveSpeed = 4f;
    private Rigidbody rb;

    //Environment variables
    [SerializeField] private Transform environmentLocation;
    Material envMaterial;
    public GameObject env;

    //Variables for time tracking
    [SerializeField] private int timeForEpisode;
    private float timeLeft;

    //Enemy Agent
    public HunterController classObject;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        envMaterial = env.GetComponent<Renderer>().material;
    }


    public override void OnEpisodeBegin()
    {
        //Agent
        transform.localPosition = new Vector3(Random.Range(-4f,4f), 0.3f, Random.Range(-4f, 4f));

        //Pellet
        CreatePellet();
        //target.localPosition = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));

        //Timer to determine if Agent is taking too long and needs to be punished
        EpisodeTimerNew();
    }

    private void Update()
    {
        CheckRemainingTime();
    }

    private void CreatePellet()
    {
        //distanceList.Clear();
        //badDistanceList.Clear();


        if (spawnedPelletsList.Count != 0)
        {
            RemovePellet(spawnedPelletsList);
        }

        for(int i = 0; i < pelletCount; i++)
        {
            int counter = 0;
            bool distanceGood;
            bool alreadyDecremented = false;

            //Spawning pellet
            GameObject newPellet = Instantiate(food);
            //Make pellet child of the environment
            newPellet.transform.parent = environmentLocation;
            //Give random spawn location
            Vector3 pelletLocation = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));

            if(spawnedPelletsList.Count != 0)
            {
                for (int k = 0; k < spawnedPelletsList.Count; k++)
                {
                    if (counter < 10)
                    {
                        distanceGood = CheckOverlap(pelletLocation, spawnedPelletsList[k].transform.localPosition, 5f);
                        if(distanceGood == false)
                        {
                            pelletLocation = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));
                            k--;
                            alreadyDecremented = true;
                            Debug.Log("Too close to other pellet");
                        }

                        distanceGood = CheckOverlap(pelletLocation, transform.localPosition, 5f);
                        if (distanceGood == false)
                        {
                            Debug.Log("Too close to agent");
                            pelletLocation = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));
                            if(alreadyDecremented == false)
                            {
                                k--;
                            }
                        }

                        counter++;
                    }
                    else
                    {
                        k = spawnedPelletsList.Count;
                    }
                }
            }

            //"Spawn" in new location
            newPellet.transform.localPosition = pelletLocation;
            //Add to list
            spawnedPelletsList.Add(newPellet);
        }
    }

    //public List<float> distanceList = new List<float>();
    //public List<float> badDistanceList = new List<float>();

    public bool CheckOverlap(Vector3 objectWeWantToAvoidOverlapping, Vector3 alreadyExistingObject, float minDistanceWanted)
    {
        float DistanceBetweenObjects = Vector3.Distance(objectWeWantToAvoidOverlapping, alreadyExistingObject);
        if(minDistanceWanted <= DistanceBetweenObjects)
        {
            //distanceList.Add(DistanceBetweenObjects);
            return true;
        }
        //badDistanceList.Add(DistanceBetweenObjects);
        return false;
    }

    private void RemovePellet(List<GameObject> toBeDeletedGameObjectList)
    {
        foreach(GameObject i in toBeDeletedGameObjectList)
        {
            Destroy(i.gameObject);
        }
        toBeDeletedGameObjectList.Clear();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Pellet")
        {
            //Remove from list
            spawnedPelletsList.Remove(other.gameObject);
            Destroy(other.gameObject);
            AddReward(10f);
            if(spawnedPelletsList.Count == 0)
            {
                envMaterial.color = Color.green;
                RemovePellet(spawnedPelletsList);
                AddReward(5f);
                classObject.AddReward(-5f);
                classObject.EndEpisode();
                EndEpisode();
            }
        }
        if (other.gameObject.tag == "Wall")
        {
            envMaterial.color = Color.red;
            RemovePellet(spawnedPelletsList);
            AddReward(-15f);
            classObject.EndEpisode();
            EndEpisode();
        }
    }

    private void EpisodeTimerNew()
    {
        timeLeft = Time.time + timeForEpisode;
    }

    private void CheckRemainingTime()
    {
        if(Time.time >= timeLeft)
        {
            envMaterial.color = Color.blue;
            AddReward(-15);
            classObject.AddReward(-15f);
            RemovePellet(spawnedPelletsList);
            classObject.EndEpisode();
            EndEpisode();
        }
    }
}
