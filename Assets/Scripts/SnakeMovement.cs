using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnakeMovement : MonoBehaviour
{
    public List<Transform> bodyparts = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MouseRoatationSnake();
        InputRotation();
        SpawnOrbManager();
    }

    public float spawnOrbEveryXseconds = 10;
    public GameObject orbPrefab;
   void SpawnOrbManager()
    {
        StartCoroutine("callEveryFewSeconds", spawnOrbEveryXseconds);
    }
    IEnumerator callEveryFewSeconds(float x)
    {
        yield return new WaitForSeconds(x);
        float radiusSpawn = 5;
       
        Vector3 randomNewPosition = new Vector3(
        Random.Range(
            Random.Range(transform.position.x - 10, transform.position.x - 5),
            Random.Range(transform.position.x + 5, transform.position.x + 10)
            ),
            Random.Range(
            Random.Range(transform.position.y - 10, transform.position.y - 5),
            Random.Range(transform.position.y + 5, transform.position.y + 10)
            ),
            0

            );

        Vector3 direction = randomNewPosition - transform.position;
        Vector3 finalPosition = transform.position + (direction.normalized * radiusSpawn);

        GameObject newOrb = Instantiate(orbPrefab, finalPosition, Quaternion.identity) as GameObject;
        GameObject orbParent = GameObject.Find("Orbs");
        // newOrb.transform.parent = orbParent.transform;
        StopCoroutine("callEveryFewSeconds");

    }

    private Vector3 pointInWorld;
    private Vector3 mousePosition;
    private float radius = 3.0f;
    private Vector3 direction;

    void MouseRoatationSnake()
    {
        Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        Physics.Raycast(ray, out hit, 1000.0f);
        mousePosition = new Vector3(hit.point.x, hit.point.y, 0);
      //  direction = mousePosition - transform.position;
        direction = Vector3.Slerp(direction, mousePosition - transform.position, Time.deltaTime *1);
        direction.z = 0;

        direction.z = 0;
        pointInWorld = direction.normalized * radius + transform.position;
        transform.LookAt(pointInWorld);
    }

    void InputRotation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            currentRotation += rotationSensitivity * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            currentRotation -= rotationSensitivity * Time.deltaTime;
        }
    }
    public float speed = 3.5f;

    public float currentRotation;
    public float rotationSensitivity = 50.0f;
    void FixedUpdate()
    {
        MoveForward();
        //Rotation();
        CameraFollow();
        ApplyingStuffForBody();
        Rurning();

    }

    public void MoveForward()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

    }
    public void Rotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, currentRotation));
    }
    [Range(0.0f,1.0f)]
    public float smoothTime = 0.05f;
    public void CameraFollow()
    {
        Transform camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject.transform;
        Vector3 cameraVelocity = Vector3.zero;
        camera.position = Vector3.SmoothDamp(camera.position, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10), ref cameraVelocity, smoothTime);
    }
    private int OrbCounter;
    private int currentOrb;
    public int[] growonThisOrb;
    private Vector3 currenSize = Vector3.one;
    public float growthRate = 0.1f;
    public float bodyPartOverTimeFollow = 0.19f;

    bool SizeUp(int x)
    {
        try
        {
            if (x == growonThisOrb[currentOrb])
            {
                currentOrb++;
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(System.Exception e)
        {
            print("no more grow from this point(add  more row)");
        }

    return false;

    }



    public Transform bodyObject;
     void OnCollisionEnter(Collision other)
    {
        
        if(other.transform.tag == "Orb")
        {
            this.speed = 3.5f;
            this.rotationSensitivity = 300f;
            Destroy(other.gameObject);
          //  OrbCounter++;

            if(SizeUp(OrbCounter)== false)
            {
                OrbCounter++;
                if (bodyparts.Count == 0)
                {
                    this.speed = 3.5f;
                    this.rotationSensitivity = 300f;
                    Vector3 currenPositon = transform.position;
                    Transform newBodyPart = Instantiate(bodyObject, currenPositon, Quaternion.identity) as Transform;

                   // newBodyPart.localScale = currenSize;
                 //   newBodyPart.GetComponent<SnakeBody>().overTime = bodyPartOverTimeFollow;

                    bodyparts.Add(newBodyPart);
                }
                else
                {
                    this.speed = 3.5f;
                    this.rotationSensitivity = 300f;
                    Vector3 currenPositon = bodyparts[bodyparts.Count - 1].position;
                    Transform newBodyPart = Instantiate(bodyObject, currenPositon, Quaternion.identity) as Transform;

                 //   newBodyPart.localScale = currenSize;
                //   newBodyPart.GetComponent<SnakeBody>().overTime = bodyPartOverTimeFollow;

                    bodyparts.Add(newBodyPart);

                }

            }
            else
            {
                currenSize += Vector3.one * growthRate;
                bodyPartOverTimeFollow += 0.04f;
                transform.localScale = currenSize; //head size


            }


            
        }
        
    }
    private bool rurning;
    public float SpeedWhileRurning = 6.5f;
    public float SpeedWhileWalking = 3.5f;
    public float bodyPartsFollowTimeWalking = 0.19f;
    public float bodyPartsFollowTimeRurning = 0.1f;
    void Rurning()
    {
        if (bodyparts.Count > 2)
        {
            if (Input.GetMouseButtonDown(0))
            {
                speed = SpeedWhileRurning;
                rurning = true;
                bodyPartOverTimeFollow = bodyPartsFollowTimeRurning;

            }
            if (Input.GetMouseButtonUp(0))
            {
                speed = SpeedWhileWalking;
                rurning = false;
                bodyPartOverTimeFollow = bodyPartsFollowTimeWalking;
            }
        }
        else
        {
            speed = SpeedWhileWalking;
            rurning = false;
            bodyPartOverTimeFollow = bodyPartsFollowTimeWalking;
        }
        
        if(rurning==true)
        {
            StartCoroutine("LooseBodyParts");
        }
        else
        {
            bodyPartOverTimeFollow = bodyPartsFollowTimeWalking;
        }
        
    }
    IEnumerator LooseBodyParts()
    {
        yield return new WaitForSeconds(0.05f);

        int lastIndex = bodyparts.Count - 1;
        Transform lastBodyPart = bodyparts[lastIndex].transform;

        bodyparts.RemoveAt(lastIndex);
        Destroy(lastBodyPart.gameObject);

        OrbCounter--;

        StopCoroutine("LooseBodyParts");
    }
    void ApplyingStuffForBody()
    {
        foreach (Transform bodyPart_x in bodyparts)
        {
            bodyPart_x.localScale = currenSize;
            bodyPart_x.GetComponent<SnakeBody>().overTime = bodyPartOverTimeFollow;
        }

    }

}
