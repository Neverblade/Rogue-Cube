using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Constants")]
    [Range(1f, 10f)]
    public float speed; // Speed of the player in units per second.

    [Header("Logic")]
    public bool moving = true; // Is the player currently moving?

    GameObject cam;
    Vector3 relCamPos; // Camera position relative to player.
    GameObject playerCube;

    ObstacleDetector upDetector, downDetector, leftDetector, rightDetector;

    void Awake() {
        playerCube = transform.Find("PlayerCube").gameObject;
        upDetector = transform.Find("Colliders").Find("Up").GetComponent<ObstacleDetector>();
        downDetector = transform.Find("Colliders").Find("Down").GetComponent<ObstacleDetector>();
        leftDetector = transform.Find("Colliders").Find("Left").GetComponent<ObstacleDetector>();
        rightDetector = transform.Find("Colliders").Find("Right").GetComponent<ObstacleDetector>();
    }

    // Update is called once per frame
    void Update () {
        ManageInput();
        ManageCamera();
	}

    public void LinkCamera(GameObject cam) {
        this.cam = cam;
        relCamPos = cam.transform.position - playerCube.transform.position;
    }

    void ManageInput()
    {
        if (moving) return;

        if (Input.GetKey("up") && !upDetector.colliding) {
            moving = true;
            StartCoroutine(Move("up"));
        } else if (Input.GetKey("down") && !downDetector.colliding) {
            moving = true;
            StartCoroutine(Move("down"));
        } else if (Input.GetKey("left") && !leftDetector.colliding) {
            moving = true;
            StartCoroutine(Move("left"));
        } else if (Input.GetKey("right") && !rightDetector.colliding) {
            moving = true;
            StartCoroutine(Move("right"));
        }
    }

    IEnumerator Move(string dir)
    {
        // Determine point and axis of rotation.
        float halfScale = 0.5f;
        Vector3 point = Vector3.zero, axis = Vector3.zero;
        switch (dir)
        {
            case "up":
                point = transform.position + Vector3.forward * halfScale;
                axis = Vector3.right;
                break;
            case "down":
                point = transform.position + Vector3.back * halfScale;
                axis = Vector3.left;
                break;
            case "right":
                point = transform.position + Vector3.right * halfScale;
                axis = Vector3.back;
                break;
            case "left":
                point = transform.position + Vector3.left * halfScale;
                axis = Vector3.forward;
                break;
        }
        point += Vector3.down * halfScale;

        float timeAnchor = Time.time, duration = 1 / speed, angleRemaining = 90;
        while (Time.time - timeAnchor <= duration) {
            // Rotate the playerCube.
            float angle = 90 * Time.deltaTime * speed;
            angleRemaining -= angle;
            playerCube.transform.RotateAround(point, axis, angle);

            // Move the parent obj accordingly.
            Vector3 vec = playerCube.transform.position;
            transform.position = new Vector3(vec.x, transform.position.y, vec.z);
            playerCube.transform.position = vec;

            // Wait a frame.
            yield return 0;
        }
        playerCube.transform.RotateAround(point, axis, angleRemaining);
        transform.position = playerCube.transform.position;
        playerCube.transform.localPosition = Vector3.zero;
        moving = false;
    }

    void ManageCamera()
    {
        Vector3 vec = playerCube.transform.position + relCamPos;
        cam.transform.position = new Vector3(vec.x, cam.transform.position.y, vec.z);
    }
}