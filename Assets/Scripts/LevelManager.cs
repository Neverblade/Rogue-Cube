using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public GameObject levelPrefab;
    public int difficulty;
    public int width, height;
    public int spawnX, spawnY;
    public int numButtons;
}

public class LevelManager : MonoBehaviour {

    [HideInInspector]
    public static LevelManager instance = null;
    public List<Level> levels = new List<Level>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
