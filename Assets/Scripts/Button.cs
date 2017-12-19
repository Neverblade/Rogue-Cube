using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    public Material activeMat;

    bool activated = false;

    void OnTriggerEnter(Collider other) {
        if (!activated && other.CompareTag("Player")) {
            ActivateButton();
        }
    }

    void ActivateButton() {
        activated = true;
        GetComponent<Renderer>().material = activeMat;
        GameManager.instance.ButtonActivated();
    }
}