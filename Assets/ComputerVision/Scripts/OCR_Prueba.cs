using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using System.Net;

//References
using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using UnityEngine.UI;

public class OCR_Prueba : MonoBehaviour
{
	// Computer Vision subscription key
    public string subKey;

    // Computer Vision API url
    public string url;

    // on-screen text which shows the text we've analyzed
    public TextMeshProUGUI uiText;
	
	 // instance
    public static OCR_Prueba instance;
	
	void Awake ()
    {
        // set the instance
        instance = this;
    }
	
	
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	// sends the image to the Computer Vision API and returns a JSON file
    public IEnumerator GetImageData (byte[] imageData)
    {
        uiText.text = "<i>[Calculating...]</i>";


        // create a new web request and set the method to POST
        UnityWebRequest webReq = new UnityWebRequest(url);
        webReq.method = UnityWebRequest.kHttpVerbPOST;

        // create a download handler to receive the JSON file
        webReq.downloadHandler = new DownloadHandlerBuffer();

        // upload the image data
        webReq.uploadHandler = new UploadHandlerRaw(imageData);
        webReq.uploadHandler.contentType = "application/octet-stream";

        // set the header
        webReq.SetRequestHeader("Ocp-Apim-Subscription-Key", subKey);     

        // send the content to the API and wait for a response
        yield return webReq.SendWebRequest();
		
		// convert the content string to a JSON file
        JSONNode jsonData = JSON.Parse(webReq.downloadHandler.text);

        //get just the text from the JSON file and display on-screen
        string imageText = GetTextFromJSON(jsonData);
		uiText.text = imageText;
	}
	
	// returns the text from the JSON data
    string GetTextFromJSON (JSONNode jsonData)
    {
        string text = "";
		string texto2;
        JSONNode lines = jsonData["regions"][0]["lines"];

        // loop through each line
        foreach(JSONNode line in lines.Children)
        {
            // loop through each word in the line
            foreach(JSONNode word in line["words"].Children)
            {
                // add the text
                text += word["text"] + " ";
				texto2 = word["text"];
				//arlist.Add(texto2);
            }
        }
		
		//uiText.text = "<i>[FINNNN...]</i>";

        return text;
    }
}
