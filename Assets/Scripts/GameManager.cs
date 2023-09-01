using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using OlfyDemo;
using UnityEngine.Serialization;

//using JetBrains.Annotations;

//using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    internal static GameManager Instance { get; private set; }

    public delegate void GameEvent();
    
    [Header("Tutorial setup")]
    [SerializeField] private GameObject tutorialGameObject;
    [SerializeField] private List<SimpleTask> tutorialTasks;
    
    public static bool HasCompletedTutorial { get; [UsedImplicitly] set; }
    [Space(10)]
    
    [Header("Guided scene setup")]
    [SerializeField] private GameObject guidedSceneGameObject;
    [SerializeField] private List<SimpleTask> guidedTasks;
    [Space(10)]
    
    [Header("Distraction scene setup")]
    [SerializeField] private GameObject distractionSceneGameObject;
    [SerializeField] private List<SimpleTask> distractionTasks;
    [Space(10)]
    
    [Header("Unguided scene setup")]
    [SerializeField] private GameObject unguidedSceneGameObject;
    [SerializeField] private List<SimpleTask> unguidedTasks;
    
    [Space(20)]
    
    // [Range(2,9)]
    
    [Header("Control bools")]
    [SerializeField] private bool showTutorial = false;
    [SerializeField] private bool showGuidedScene = false;
    [SerializeField] private bool showDistractionScene = false;
    [SerializeField] private bool showUnguidedScene = false;
    [SerializeField] private bool hasEnded;
    [Space(10)]
    
    [Header("Debug utilities")]
    [SerializeField] private bool isDebugging = true;

    [SerializeField] private TextMeshProUGUI textbox;
    [Space(50)]

    [Header("Old Kitchen Stuff")]
    [SerializeField] private Fire fire;
    [SerializeField] private AudioSource smokeDetector;
    [SerializeField] private List<TidyingTask> tidyingTasks;

    // public delegate void GameEvent();
    [SerializeField] public GameEvent StopFire;
    private bool _victory;
    internal bool conditionsForFireMet{get; set;}

    [SerializeField] private float timeBeginning;
    [SerializeField] private float timeFireBeginning;
    [SerializeField] private float timeEnd;
    [SerializeField] private GameObject endCanvas;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI timeToStopFireText;

    // Is called before Start
    private void Awake() {
        if (!Instance) Instance = this;
        else Destroy(this);
        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called before the first frame update
    private void Start() {
        // if (isDebugging) DebugConsole.CreateConsole();

        // If player presses Start
        StartCoroutine(Scenario());
        
        // timeBeginning = Time.time;
    }
    
    private IEnumerator Scenario() {
        Debug.Log("Scenario started");
        
        // "Scene" 1: tutorial -> Player is presented (by a video) how to grab, move his hand(s);
        // he is then asked to put a red mug on a red plate
        /* "Scene" 2:
         1) First task: put the bowl and the 2 mugs (including the upside-down one) on the plate right side up.
         2) Second task: add the 2 (say coffee and orange juice) of the jugs on the plate 
            (note: the only way to distinguish the pitchers is by smelling them)
         3) Third task: add sugar and chocolate on the plate
         4) Fourth task: player puts sugar inside his coffee mug, adds coffee and their coffee spoon inside
         */
        // Distraction "scene" 3: put a green jug on a red plate and a red jug on a green plate
        // "Scene" 4: your turn -> fill the plate like we did before (maybe move things around, change colors of some things)

        /* Notes:
         * ALWAYS end scenes with a congratulations!
         * 
         */
        
        /* "Scene" 1: tutorial
          1) Player is presented (by a video) how to grab, move his hand(s)
          he is then asked to put a red mug on a red plate
        */
        
        if (!HasCompletedTutorial && showTutorial) StartCoroutine(ScenarioStarter(tutorialTasks, tutorialGameObject, isDebugging));

        // Congratulations();
        
        /* "Scene" 2:
        1) First task: put the bowl and the 2 mugs (including the upside-down one) on the plate right side up.
        2) Second task: add the 2 (say coffee and orange juice) of the jugs on the plate 
        (note: the only way to distinguish the pitchers is by smelling them)
        3) Third task: add sugar and chocolate on the plate
        4) Fourth task: player puts sugar inside his coffee mug, adds coffee and their coffee spoon inside
        */

        if (showGuidedScene) {
            StartCoroutine(ScenarioStarter(guidedTasks, guidedSceneGameObject, isDebugging));
        }
        
        // First group of tasks
        
        // Second group of tasks
        
        // Third group of tasks
        
        // Fourth (and last) group of tasks
        
        // if (task done)
        // Congratulations();
        
        // Distraction "scene" 3: put a green jug on a red plate and a red jug on a green plate
        if (showDistractionScene) {
            StartCoroutine(ScenarioStarter(distractionTasks, distractionSceneGameObject, isDebugging));
            
        }
        
        // if (task done)
        // Congratulations();
        
        /* "Scene" 4: your turn
         fill the plate as we did before (maybe move things around, change colors of some things AND REMOVE INDICATORS & GUIDELINES)
        */
        if (showUnguidedScene) StartCoroutine(ScenarioStarter(unguidedTasks, unguidedSceneGameObject, isDebugging));
        
        // if (Finish button pressed)
        // Congratulations();

        if (hasEnded) {
            unguidedSceneGameObject.SetActive(false);
            End();
            Debug.Log("Scenario ended");
            yield return null;
        }

        // SetActive(false) Unguided Scene
    }

    private IEnumerator ScenarioStarter(List<SimpleTask> tasks, GameObject scenarioGameObject,
        bool debug = false) {
        if (debug) Debug.Log(scenarioGameObject.name + " starting");
        
        scenarioGameObject.SetActive(true);

        foreach (SimpleTask task in tasks) task.StartTask();

        var tasksComplete = false;
        while (!tasksComplete) {
            tasksComplete = true;
            foreach (SimpleTask task in tasks)
                if (!task.Complete) {
                    tasksComplete = false;
                    break;
                }

            yield return 0;
        }

        foreach (SimpleTask task in tasks) task.loopTask = false;

        if (debug) Debug.Log(scenarioGameObject.name + " ending");

        scenarioGameObject.SetActive(false);
    }

    internal void End() {
        // _victory = true;
        // _timeEnd = Time.time;
        // timeText.text = (_timeEnd - _timeBeginning).ToString("0.00") + " sec";
        // endCanvas.SetActive(true);
        throw new NotImplementedException(); 
    }

    private void Congratulations() {
        throw new NotImplementedException();
        // show Congratulations message, confetti to show that the player is doing well
    }

    IEnumerator ManageFire()
    {
        yield return new WaitUntil(() => conditionsForFireMet);
        bool startFire = false;
        float timeConditionsMet = Time.time;
        while(!startFire)
        {
            if(!conditionsForFireMet)
            {
                yield return new WaitUntil(() => conditionsForFireMet);
                timeConditionsMet = Time.time;
            }
            else if(Time.time - timeConditionsMet >= 20)
            {
                startFire = true;
            }
            yield return null;
        }
        fire.StartFire();
        timeFireBeginning = Time.time;
        yield return new WaitForSeconds(5);
        smokeDetector.Play();
        yield return new WaitUntil(()=>_victory);
        StopFire.Invoke();
    }
}