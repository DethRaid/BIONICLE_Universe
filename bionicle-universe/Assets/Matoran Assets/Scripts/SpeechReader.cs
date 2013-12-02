using System.Collections.Generic;
using UnityEngine;
using System.IO;

class SpeechReader : MonoBehaviour
{
    public string data;
    public StreamReader reader;
    public SpeechData lastData, root;

    public void Awake()
    {
        print(Application.dataPath);
    }

    public void getSpeech(string name)
    {
        reader = new StreamReader(new FileStream(Application.dataPath + "/Data/" + name + ".busf", FileMode.Open));
        root = new SpeechData();
        data = reader.ReadToEnd();
        root.output = getOutput("root");
        root.responces = getResponces("root");
        followConvo(root);
    }

    public void followConvo(SpeechData speech)
    {
        foreach (string s in speech.responces)
        {
            SpeechData next = new SpeechData();
            next.output = getOutput(s);
            next.responces = getResponces(s);
            print("Adding new speech data: " + next.ToString());
            speech.otherOuts.Add(next);
        }
        if (speech.responces.Count != 0)
            foreach (SpeechData s in speech.otherOuts)
                followConvo(s);
    }

    public string getOutput(string tag)
    {
        int pos = data.IndexOf("<" + tag + "<") + ("<" + tag + "<").Length;
        return data.Substring(pos, data.IndexOf('\n', pos) - pos);
    }

    public List<string> getResponces(string tag)
    {
        List<string> outs = new List<string>();
        int pos = data.IndexOf("<" + tag + "<") + ("<" + tag + "<").Length;
        int startPos = data.IndexOf('{', pos);
        startPos = data.IndexOf('>', startPos);
        for (int i = 0; i < 4; i++)
        {
            if (data.IndexOf('>', startPos) != -1)
            {
                outs.Add(data.Substring(startPos, data.IndexOf('>', startPos)));
                startPos = data.IndexOf('>', startPos);
            }
        }
        return outs;
    }
}

[System.Serializable]
class SpeechData
{
    public string output;
    public List<string> responces;
    public List<SpeechData> otherOuts;

    public SpeechData() {}

    public SpeechData(string main, List<string> res)
    {
        output = main;
        responces = res;
    }

    public string ToString()
    {
        string value = output;
        foreach (string s in responces)
            value += ">" + s;
        return value;
    }
}