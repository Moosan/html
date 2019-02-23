using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Xml;
using System.IO;
public class wwwSample : MonoBehaviour {
    [SerializeField]
    private string url;
	void Start () {
        var result = ObservableWWW.Get("http://localhost:3000/?url="+url);
        
        result.Subscribe(log=> Debug.Log(Result(log)));
	}
    private string Result(string log) {
        Debug.Log(log);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(log));
        return xmlDoc.ToString();
    }

}
