using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public static GameManager instance = null;

    [Header("Constants")]
    public Vector3 cameraOffsetPos;
    public Vector3 cameraRotation;
    [Range(0, 5)]
    public float minTweenDelay, maxTweenDelay;
    [Range(0, 5)]
    public float tweenDuration;
    [Range(0, 30)]
    public float tweenDistance;
    public int startingHealth;

    [Header("Variables")]
    public GameObject cam;
    public GameObject playerPrefab;
    public GameObject canvas;

    LevelManager levelManager;
    GameObject player;
    int currentDepth;
    Level currentLevel;
    GameObject currentLevelObj;
    int buttonsRemaining;
    int currentHealth;
    Text healthText;
    Text depthText;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    /** Starts the game. Loads up an initial level,
     *  and callbacks continue progress from there.
     */
    void Start() {
        // Initialize.
        currentDepth = 0;
        currentHealth = startingHealth;
        levelManager = LevelManager.instance;
        healthText = canvas.transform.Find("HealthText").GetComponent<Text>();
        depthText = canvas.transform.Find("DepthText").GetComponent<Text>();

        // Update canvas.
        UpdateCanvas();

        // Kick-start game.
        currentLevel = ChooseLevel();
        SetupLevel(currentLevel);
    }

    /** Updates the canvas with proper values of health and depth.
     */
    void UpdateCanvas() {
        healthText.text = "Health: " + currentHealth;
        depthText.text = "Depth: " + currentDepth;
    }

    #region Level Setup/Cleanup

    /** Selects the next level to load and returns it.
     */
    private Level ChooseLevel() {
        return levelManager.levels[1];
    }

    /** Given a level, loads it and spawns in the player.
     * 
     */
    private void SetupLevel(Level level) {
        StartCoroutine(SetupLevelCoroutine(level));
    }

    private IEnumerator SetupLevelCoroutine(Level level) {
        // Initialize.
        buttonsRemaining = level.numButtons;

        // Set up camera position.
        Vector3 playerPosition = new Vector3(.5f, .5f, -.5f) + new Vector3(level.spawnX, 0, -level.spawnY);
        cam.transform.position = playerPosition + cameraOffsetPos;
        cam.transform.eulerAngles = cameraRotation;

        // Load in the level.
        Vector3 diffVec = new Vector3(0, tweenDistance, 0);
        currentLevelObj = Instantiate(level.levelPrefab, diffVec, Quaternion.identity);

        // Tween the environment down.
        foreach (Transform child in currentLevelObj.transform.Find("Floor")) {
            float delay = Random.Range(minTweenDelay, maxTweenDelay);
            iTween.FadeFrom(child.gameObject, 0, delay + tweenDuration);
            iTween.MoveAdd(child.gameObject, iTween.Hash("amount", -diffVec,
                                                         "time", tweenDuration,
                                                         "easetype", "easeOutQuart",
                                                         "delay", delay));
        }
        foreach (Transform child in currentLevelObj.transform.Find("Walls")) {
            float delay = Random.Range(minTweenDelay, maxTweenDelay);
            iTween.FadeFrom(child.gameObject, 0, delay + tweenDuration);
            iTween.MoveAdd(child.gameObject, iTween.Hash("amount", -diffVec,
                                                         "time", tweenDuration,
                                                         "easetype", "easeOutQuart",
                                                         "delay", delay));
        }
        yield return new WaitForSeconds(tweenDuration + maxTweenDelay);

        // Tween the interactables down.
        foreach (Transform child in currentLevelObj.transform.Find("Buttons")) {
            float delay = Random.Range(minTweenDelay, maxTweenDelay);
            iTween.FadeFrom(child.gameObject, 0, delay + tweenDuration);
            iTween.MoveAdd(child.gameObject, iTween.Hash("amount", -diffVec,
                                                         "time", tweenDuration,
                                                         "easetype", "easeOutQuart",
                                                         "delay", delay));
        }
        yield return new WaitForSeconds(tweenDuration + maxTweenDelay);

        // Spawn in the player, tween it down, enable movement.
        player = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
        player.GetComponent<PlayerController>().LinkCamera(cam);
        iTween.FadeFrom(player, 0, tweenDuration);
        iTween.MoveFrom(player, iTween.Hash("position", player.transform.position + diffVec,
                                            "time", tweenDuration,
                                            "easetype", "easeOutQuart"));
        yield return new WaitForSeconds(tweenDuration + .5f);
        player.GetComponent<PlayerController>().canMove = true;
    }

    #endregion

    #region Callbacks

    /** Callback for when a level is completed.
     *  Cleans up the current level and loads a new one.
     */
    public void LevelFinished() {
        StartCoroutine(LevelFinishedCoroutine());
    }

    IEnumerator LevelFinishedCoroutine() {
        // Logic
        currentDepth += 1;
        UpdateCanvas();

        // Tween player up, remove player.
        Vector3 diffVec = new Vector3(0, tweenDistance, 0);
        //iTween.FadeTo(player, 0, tweenDuration);
        iTween.MoveBy(player, iTween.Hash("amount", diffVec,
                                          "time", tweenDuration,
                                          "easetype", "easeInQuart"));
        yield return new WaitForSeconds(tweenDuration);
        Destroy(player);

        // Tween environment up, remove environment.
        foreach (Transform child in currentLevelObj.transform.Find("Buttons")) {
            float delay = Random.Range(minTweenDelay, maxTweenDelay);
            iTween.FadeTo(child.gameObject, 0, delay + tweenDuration);
            iTween.MoveAdd(child.gameObject, iTween.Hash("amount", diffVec,
                                                         "time", tweenDuration,
                                                         "easetype", "easeOutQuart",
                                                         "delay", delay));
        }
        yield return new WaitForSeconds(tweenDuration + maxTweenDelay);

        foreach (Transform child in currentLevelObj.transform.Find("Floor")) {
            float delay = Random.Range(minTweenDelay, maxTweenDelay);
            iTween.FadeTo(child.gameObject, 0, delay + tweenDuration);
            iTween.MoveAdd(child.gameObject, iTween.Hash("amount", diffVec,
                                                         "time", tweenDuration,
                                                         "easetype", "easeOutQuart",
                                                         "delay", delay));
        }
        foreach (Transform child in currentLevelObj.transform.Find("Walls")) {
            float delay = Random.Range(minTweenDelay, maxTweenDelay);
            iTween.FadeTo(child.gameObject, 0, delay + tweenDuration);
            iTween.MoveAdd(child.gameObject, iTween.Hash("amount", diffVec,
                                                         "time", tweenDuration,
                                                         "easetype", "easeOutQuart",
                                                         "delay", delay));
        }
        yield return new WaitForSeconds(tweenDuration + maxTweenDelay);
        Destroy(currentLevelObj);

        // Prepare next level.
        currentLevel = ChooseLevel();
        SetupLevel(currentLevel);
    }

    /** A button was activated.
     */
    public void ButtonActivated() {
        buttonsRemaining -= 1;
        if (buttonsRemaining == 0) {
            player.GetComponent<PlayerController>().canMove = false;
            LevelFinished();
        }
    }

    #endregion
}