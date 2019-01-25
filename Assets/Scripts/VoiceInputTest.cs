using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;


//https://docs.microsoft.com/en-us/windows/mixed-reality/voice-input-in-unity
public class VoiceInputTest : MonoBehaviour {

    Dictionary<string, Action> keywords = new Dictionary<string, Action>();
    KeywordRecognizer keywordRecognizer;

    TextMesh text;

    // Use this for initialization
    void Start () {
        text = gameObject.transform.Find("SpeachKeyword").GetComponent<TextMesh>();
        InitKeywords();
        InitKeywordRecognizer();
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    void OnDisable()
    {
        keywordRecognizer.Stop();
    }

    private void InitKeywordRecognizer()
    {
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Action keywordAction;
        // if the keyword recognized is in our dictionary, call that Action.
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    private void InitKeywords()
    {
        keywords.Add("open processes", ActionOpenProcesses);
        keywords.Add("scan device", ActionScanDevice);
        keywords.Add("close processes", ActionCloseProcesses);
    }


    private void ActionOpenProcesses()
    {
        Debug.Log("You said: 'open processes'");
        text.text = "You said: 'open processes'";
    }

    private void ActionScanDevice()
    {
        Debug.Log("You said: 'scan device'");
        text.text = "You said: 'scan device'";
    }

    private void ActionCloseProcesses()
    {
        Debug.Log("You said: 'close processes'");
        text.text = "You said: 'close processes'";
    }
}
