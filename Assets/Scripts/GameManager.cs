using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public static GameManager instance = null;

    [Header("Constants")]
    public Vector3 cameraOffsetPos;
    public Vector3 cameraRotation;
    [Range(0, 5)]
    public float minTweenDuration, maxTweenDuration;
    [Range(0, 5)]
    public float playerTweenDuration;
    [Range(0, 10)]
    public float tweenDistance;

    [Header("Variables")]
    public GameObject cam;
    public GameObject playerPrefab;

    LevelManager levelManager;
    GameObject player;
    int currentDepth;
    Level currentLevel;
    int buttonsRemaining;

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
        levelManager = LevelManager.instance;

        // Kick-start game.
        currentLevel = ChooseLevel();
        SetupLevel(currentLevel);
    }

    #region Level Setup/Cleanup

    /** Selects the next level to load and returns it.
     */
    private Level ChooseLevel() {
        return levelManager.levels[0];
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
        GameObject levelObj = Instantiate(level.levelPrefab, Vector3.zero, Quaternion.identity);

        // Tween the environment down.
        Vector3 diffVec = new Vector3(0, tweenDistance, 0);
        foreach (Transform child in levelObj.transform.Find("Floor")) {
            float duration = Random.Range(minTweenDuration, maxTweenDuration);
            iTween.FadeFrom(child.gameObject, 0, duration);
            iTween.MoveFrom(child.gameObject, iTween.Hash("position", child.position + diffVec,
                                                          "time", duration,
                                                          "easetype", "easeOutQuart"));
        }
        foreach (Transform child in levelObj.transform.Find("Walls")) {
            float duration = Random.Range(minTweenDuration, maxTweenDuration);
            iTween.FadeFrom(child.gameObject, 0, duration);
            iTween.MoveFrom(child.gameObject, iTween.Hash("position", child.position + diffVec,
                                                          "time", duration,
                                                          "easetype", "easeOutQuart"));
        }
        foreach (Transform child in levelObj.transform.Find("Buttons")) {
            float duration = Random.Range(minTweenDuration, maxTweenDuration);
            iTween.FadeFrom(child.gameObject, 0, duration);
            iTween.MoveFrom(child.gameObject, iTween.Hash("position", child.position + diffVec,
                                                          "time", duration,
                                                          "easetype", "easeOutQuart"));
        }
        yield return new WaitForSeconds(maxTweenDuration + .5f);

        // Spawn in the player, tween it down, enable movement.
        player = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
        player.GetComponent<PlayerController>().LinkCamera(cam);
        iTween.FadeFrom(player, 0, playerTweenDuration);
        iTween.MoveFrom(player, iTween.Hash("position", player.transform.position + diffVec,
                                                          "time", playerTweenDuration,
                                                          "easetype", "easeOutQuart"));
        yield return new WaitForSeconds(playerTweenDuration + .5f);
        player.GetComponent<PlayerController>().moving = false;
    }

    #endregion

    #region Callbacks

    /** Callback for when a level is completed.
     *  Cleans up the current level and loads a new one.
     */
    public void LevelFinished() {
        print("You win!");
    }

    /** A button was activated.
     */
    public void ButtonActivated() {
        buttonsRemaining -= 1;
        if (buttonsRemaining == 0) {
            LevelFinished();
        }
    }

    #endregion
}