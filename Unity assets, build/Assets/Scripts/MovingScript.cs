using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem.EnhancedTouch;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using System.Linq;
using Unity.Collections;
using System.Drawing;

public class MovingScript : MonoBehaviour
{
    [SerializeField] GameObject placedPrefab;
    [SerializeField] GameObject collectiblePrefab;
    [SerializeField] ARRaycastManager raycaster;
    [SerializeField] ARPlaneManager planeManager;
    public Button startButton;
    public TextMeshProUGUI planeText;
    GameObject spawnedObject;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    float duration;
    float multiplier;
    public int counter;
    Vector2 range1;
    Vector2 range2;
    float planeHeight;
    public bool pauseGame;
    UnityEngine.Gyroscope gyro;
    float initialPitch;
    Vector3 startPosition;
    Vector2 planeExtents;
    Vector3 planeCenter;
    bool right;


    void Start()
    {
        duration = 0.9f;    //animation duration control
        multiplier = 1 / duration;
        counter = 0;
        pauseGame = true;
        right = true;

        if (!SystemInfo.supportsGyroscope)
            planeText.text = "Gyroscope not supported on this device!";
        else
        {
            gyro = Input.gyro;
            gyro.enabled = true;
        }

    }

    void Awake()
    {
        raycaster = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    void SetRange(ARPlane plane)
    {
        planeExtents = plane.extents * 0.8f;
        planeCenter = plane.center;
    }

    void SpawnCube()
    {
        float x = Random.Range(planeCenter.x - planeExtents.x * 0.85f, planeCenter.x + planeExtents.x * 0.85f);
        float z = Random.Range(planeCenter.z - planeExtents.y * 0.85f, planeCenter.z + planeExtents.y * 0.85f);
        float y = planeCenter.y;

        // Set the random Vector3 point on the plane
        Vector3 point = new Vector3(x, y, z);
        Instantiate(collectiblePrefab, point, Quaternion.identity);
    }

    public void DestroyCollectibleObjects()
    {
        Object[] allObjects = FindObjectsOfType(typeof(CollectiblePrefab));

        if (allObjects.Length == 0)
            return;

        foreach (CollectiblePrefab obj in allObjects)
        {
            Destroy(obj);
        }
    }

    void CheckBounds()
    {
        if (spawnedObject.transform.position.x < planeCenter.x - planeExtents.x * 0.85f)
            spawnedObject.transform.position = new Vector3(planeCenter.x + planeExtents.x * 0.85f, startPosition.y, spawnedObject.transform.position.z);
        if (spawnedObject.transform.position.x > planeCenter.x + planeExtents.x * 0.85f)
            spawnedObject.transform.position = new Vector3(planeCenter.x - planeExtents.x * 0.85f, startPosition.y, spawnedObject.transform.position.z);
        if (spawnedObject.transform.position.z < planeCenter.z - planeExtents.y * 0.85f)
            spawnedObject.transform.position = new Vector3(spawnedObject.transform.position.x, startPosition.y, planeCenter.z + planeExtents.y * 0.85f);
        if (spawnedObject.transform.position.z > planeCenter.z + planeExtents.y * 0.85f)
            spawnedObject.transform.position = new Vector3(spawnedObject.transform.position.x, startPosition.y, planeCenter.z - planeExtents.y * 0.85f);
    }

    void ChangeRot()
    {
        CheckBounds();

        float rotationRate = gyro.rotationRate.y;
        if (rotationRate > 0.2f || rotationRate < -0.2f)
        {
            if (rotationRate < 0)
                right = true;
            else if (rotationRate > 0)
                right = false;
        }

        if (right)
            spawnedObject.transform.Rotate(Vector3.up * 80.0f * Time.deltaTime);
        else
            spawnedObject.transform.Rotate(-Vector3.up * 80.0f * Time.deltaTime);
    }

    public void Retry()
    {
        spawnedObject.transform.position = startPosition;
        initialPitch = gyro.attitude.eulerAngles.y;
    }



    // Update is called once per frame
    void Update()
    {
        Debug.Log(gyro.rotationRate);
        //Random spawning objects
        if (counter == 60 && !pauseGame)
        {
            SpawnCube();
            counter = 0;
        }
        else
        {
            counter++;
        }

        if (!pauseGame && spawnedObject != null)
        {
            ChangeRot();
            spawnedObject.transform.position += spawnedObject.transform.forward * Time.deltaTime * 0.3f;
        }

        if (Input.touchCount == 0)
            return;

        //get the screen touch position
        var touch = Input.GetTouch(0);
        if (touch.phase != UnityEngine.TouchPhase.Began)
            return;

        if (raycaster.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            //get the hit point (pose) on the plane
            Pose hitPose = hits[0].pose;

            if (hits[0].trackable is ARPlane plane && planeManager.currentDetectionMode == PlaneDetectionMode.Horizontal)
            {
                SetRange(plane);
                planeManager.requestedDetectionMode = PlaneDetectionMode.None;

                foreach (var pln in planeManager.trackables)
                {
                    pln.gameObject.SetActive(false);
                }
                plane.gameObject.SetActive(true);
                startButton.gameObject.SetActive(true);
                planeText.gameObject.SetActive(false);
                initialPitch = gyro.attitude.eulerAngles.y;
            }

            //if this is the first time placing Heli
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
                startPosition = hitPose.position;
            }
        }
    }
}