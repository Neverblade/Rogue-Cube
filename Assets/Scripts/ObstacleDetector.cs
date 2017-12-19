using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour {

    public bool colliding = false;
    public int numCollides = 0;

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player") && !other.CompareTag("Detector")) {
            numCollides += 1;
            colliding = numCollides > 0;
        }
        
    }

    void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player") && !other.CompareTag("Detector")) {
            numCollides -= 1;
            colliding = numCollides > 0;
        }
    }

}
