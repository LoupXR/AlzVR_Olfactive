using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour
{
    private static DebugConsole _instance;
    public static DebugConsole Instance
    {
        get
        {
            if (_instance == null)
            {
                CreateConsole();
            }
            return _instance;
        }
    }
    public Canvas canvas;
    public Text consoleText;
    public bool followCamera = true;
    List<int> lineEndIndices = new List<int>();

    public int maxLines = 20;
    private static bool supressWarnings = true;

    public static void SupressWarnings(bool value)
    {
        supressWarnings = value;
    }

    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        if(_instance != this)
        {
            Destroy(this);
        }

        if(canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = followCamera ? RenderMode.ScreenSpaceOverlay : RenderMode.WorldSpace;
            //canvas.worldCamera = Camera.main;
        }
        if(consoleText == null)
        {
            GameObject textObject = new GameObject("Console Text");
            textObject.transform.SetParent(canvas.transform);
            consoleText = textObject.AddComponent<Text>();
            consoleText.rectTransform.anchorMin = new Vector2(0.12f, 0.3f);
            consoleText.rectTransform.anchorMax = new Vector2(0.4f, 0.7f);
            consoleText.rectTransform.offsetMax = Vector2.zero;
            consoleText.rectTransform.offsetMin = Vector2.zero;
            consoleText.fontSize = 15;
            consoleText.alignment = TextAnchor.LowerLeft;
            consoleText.verticalOverflow = VerticalWrapMode.Truncate;
            consoleText.horizontalOverflow = HorizontalWrapMode.Wrap;
            consoleText.font = Font.CreateDynamicFontFromOSFont("consoleFont", 20);
        }
        /* if(followCamera)
        {
            Transform mainCamera = Camera.main.transform;
            transform.parent = mainCamera;
            transform.position = mainCamera.position + mainCamera.forward + Vector3.down * 0.3f;
            transform.LookAt(2 * transform.position - mainCamera.position);
            transform.localScale = Vector3.one * 0.022f;
        } */
    }


    void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }

    //Called when there is an exception
    void LogCallback(string condition, string stackTrace, LogType type)
    {
        if(!(type == LogType.Warning) || !supressWarnings)
        {
            switch(type)
            {
                case LogType.Assert :
                case LogType.Exception :
                case LogType.Error :
                    consoleText.color = Color.red;
                    break;
                case LogType.Warning :
                    consoleText.color = Color.yellow;
                    break;
                case LogType.Log :
                    consoleText.color = Color.white;
                    break;
            }
            Log(condition);
            if(type == LogType.Error || type == LogType.Exception) Log(stackTrace);
        }
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }


    public static void Log(string message)
    {
        Instance.TruncateOverflow(message);
        Instance.consoleText.text += "\n" + message;
    }

    public static void CreateConsole()
    {
        GameObject gameObject = new GameObject("DebugConsole");
        gameObject.AddComponent<DebugConsole>();
    }


    private void TruncateOverflow(string message)
    {
        if(consoleText.text != null)
        {
            lineEndIndices.Add(consoleText.text.Length);
        }
        
        for (int i = 0; i < message.Length; i++)
        {
            if(message[i] == '\n')
            {
                lineEndIndices.Add(Instance.consoleText.text.Length + i);
            }
        }
        
        while(lineEndIndices.Count > maxLines)
        {
            consoleText.text = consoleText.text.Remove(0, lineEndIndices[0]);
            for(int i = 1; i < lineEndIndices.Count; i ++)
            {
                lineEndIndices[i] -= lineEndIndices[0];
            }
            lineEndIndices.RemoveAt(0);
        }

        
    }

}
