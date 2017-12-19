using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Constants")]
    [Range(1f, 10f)]
    public float speed; // Speed of the player in units per second.

    [Header("Logic")]
    public bool canMove = false;
    public bool moving = false; // Is the player currently moving?

    GameObject cam;
    Vector3 relCamPos; // Camera position relative to player.
    GameObject playerCube;

    void Awake() {
        playerCube = transform.Find("PlayerCube").gameObject;
    }

    void Update () {
        ManageGravity();
        ManageInput();
        ManageCamera();
	}

    /** Link the camera to the player's movement.
     */
    public void LinkCamera(GameObject cam) {
        this.cam = cam;
        relCamPos = cam.transform.position - playerCube.transform.position;
    }

    /** Input manager for the player.
     */
    void ManageInput()
    {
        if (moving || !canMove) return;

        if ((Input.GetKey("up") || Input.GetKey(KeyCode.W)) && IsDirectionClear("up")) {
            moving = true;
            StartCoroutine(Move("up"));
        }
        else if ((Input.GetKey("down") || Input.GetKey(KeyCode.S)) && IsDirectionClear("down")) {
            moving = true;
            StartCoroutine(Move("down"));
        }
        else if ((Input.GetKey("left") || Input.GetKey(KeyCode.A)) && IsDirectionClear("left")) {
            moving = true;
            StartCoroutine(Move("left"));
        }
        else if ((Input.GetKey("right") || Input.GetKey(KeyCode.D)) && IsDirectionClear("right")) {
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

    /** Check if a given direction is clear (true) or not (false).
     */
    bool IsDirectionClear(string dir) {
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.zero;
        switch (dir) {
            case "up":
                direction = new Vector3(0, 0, 1);
                break;
            case "down":
                direction = new Vector3(0, 0, -1);
                break;
            case "right":
                direction = new Vector3(1, 0, 0);
                break;
            case "left":
                direction = new Vector3(-1, 0, 0);
                break;
            case "below":
                direction = new Vector3(0, -1, 0);
                break;
        }
        return !Physics.Raycast(origin, direction, 1);
    }

    void ManageCamera()
    {
        Vector3 vec = playerCube.transform.position + relCamPos;
        cam.transform.position = new Vector3(vec.x, cam.transform.position.y, vec.z);
    }

    void ManageGravity() {
        if (canMove && !moving && IsDirectionClear("below")) {
            canMove = false;
            playerCube.GetComponent<Rigidbody>().useGravity = true;
            playerCube.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}