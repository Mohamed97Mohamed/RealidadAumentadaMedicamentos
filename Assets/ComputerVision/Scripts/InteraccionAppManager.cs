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

public static class EmailInterac
{
	public static string name;
}

public class InteraccionAppManager : MonoBehaviour
{
    // Computer Vision subscription key
    public string subKey;

    // Computer Vision API url
    public string url;

    // on-screen text which shows the text we've analyzed
    public TextMeshProUGUI uiText;
	

    // instance
    public static InteraccionAppManager instance;
	
	//Arraylist
	ArrayList listaPrincipios_1 = new ArrayList(); 
	ArrayList listaPrincipios_2 = new ArrayList(); 
	ArrayList listaDescripcion = new ArrayList();
	

	
	//Sqlite
	private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    private IDataReader reader;
	string DatabaseName = "BaseDatos.db";
	
	private bool encontrado=false;
	private bool encontrado_2_medicamentos=false;
	private bool medicamento_registrado=false;
	private string codNacional;
	private string numRegistro;
	private string Nombre_1;
	private string Nombre_2;

	private bool hayInteracciones=false;
	
	//Objetos
	public GameObject UIPrincipal;
	public GameObject UIPopUp;
	public GameObject UIPopUp2;
	public GameObject UIPopUp3;
	
	[SerializeField] Image Interac_Image;

	
	//Text
	public Text Email_Text;
	public Text Nombre_Text_2;
	public Text Interac_Text;

	private int contadorMedicamento=0;


	
    void Awake ()
    {
        // set the instance
        instance = this;
    }
	
	 void Start()
    {
		//ANDRIOD
		
		 //Application database Path android
		string filepath = Application.persistentDataPath + "/" + DatabaseName;
        if (!File.Exists(filepath))
        {
            WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/BaseDatos.db");
            while (!loadDB.isDone) { }
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDB.bytes);
        }
        conn = "URI=file:" + filepath;
		
		
		//PC
		/*string filepath = Application.dataPath + "/Plugins/" + DatabaseName;

        //open db connection
        conn = "URI=file:" + filepath;

        Debug.Log("Stablishing connection to: " + conn);
        dbconn = new SqliteConnection(conn);
        dbconn.Open();*/
    
    }
	


    // sends the image to the Computer Vision API and returns a JSON file
    public IEnumerator GetImageData (byte[] imageData)
    {
        uiText.text = "<i>[Detectando...]</i>";

		
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

        // get just the text from the JSON file and display on-screen
        string imageText = GetTextFromJSON(jsonData);
		
		string[] cadenaToken = imageText.Split(new[]{'.', ' '});
		for (int i = 0; i < cadenaToken.Length; i++) 
		{
			
			if(int.TryParse(cadenaToken[i], out int num)&& cadenaToken[i].Length==6){
				codNacional = cadenaToken[i];
				//LLAMAMOs A LA API REST
				string URL="https://cima.aemps.es/cima/rest/medicamentos?=&multiple="+codNacional;
				
				UnityWebRequest request = UnityWebRequest.Get(URL);
				// create a download handler to receive the JSON file
				request.downloadHandler = new DownloadHandlerBuffer();
				// send the content to the API and wait for a response
				yield return request.SendWebRequest();
				// convert the content string to a JSON file
				JSONNode jsonData2 = JSON.Parse(request.downloadHandler.text);
				// get just the text from the JSON file and display on-screen
				string imageText2 = GetTextFromJSON2(jsonData2);	
			}
			
			if(encontrado==true){
				break;
			}
		}
		
		
		if(encontrado==true){
			string URL2="https://cima.aemps.es/cima/rest/medicamento?nregistro="+numRegistro+"&carganotas=1&cargamateriales=1";
		
			UnityWebRequest request2 = UnityWebRequest.Get(URL2);
			
			// create a download handler to receive the JSON file
			request2.downloadHandler = new DownloadHandlerBuffer();
			
			// send the content to the API and wait for a response
			yield return request2.SendWebRequest();
			
			// convert the content string to a JSON file
			JSONNode jsonData3 = JSON.Parse(request2.downloadHandler.text);
			
			// get just the text from the JSON file and display on-screen
			GetTextFromJSON3(jsonData3);
			
			if(contadorMedicamento==1){
				encontrado_2_medicamentos=true;
			}
		}
		
		if(encontrado==false){
			uiText.text = "<i>[No se ha detectado el medicamento!!]</i>";
		}else{
			uiText.text = "<i>[Se ha detectado el medicamento...]</i>";
		}
		encontrado=false;
		
		
		if(contadorMedicamento==0){
			contadorMedicamento++;
		}
		else if(contadorMedicamento==1){
			contadorMedicamento=0;
		}
		
		if(encontrado_2_medicamentos==true){
			encontrado_2_medicamentos=false;
			
			uiText.text = "";
			UIPopUp2.SetActive(true);
			Nombre_Text_2.text = "Interaccion entre el medicamento (1) "+Nombre_1+" y medicamento (2) "+Nombre_2;
			
			for(int i = 0 ; i < listaPrincipios_1.Count; i++){
				string principio_1 = (string) listaPrincipios_1[i];
				for(int j = 0 ; j < listaPrincipios_2.Count; j++){
					string principio_2 = (string) listaPrincipios_2[j];
					BuscarInteracciones(principio_1,principio_2);
				}
			}
			
			string textoInteracciones="";
			if(hayInteracciones==true){
				for (int i = 0; i < listaDescripcion.Count; i++){	
					textoInteracciones+=(string) listaDescripcion[i]+"\n";
				}
				Interac_Image.color = new Color32(243, 13, 15, 255);
				
			}else{
				textoInteracciones+="No hay Interacciones de alimentos con este medicamento";
				Interac_Image.color = new Color32(16, 226, 79, 255);
			}
			hayInteracciones=false;
			Interac_Text.text=textoInteracciones;
			
			listaPrincipios_1.Clear();
			listaPrincipios_2.Clear();
			listaDescripcion.Clear();		
		}
		 
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
            }
        }
        return text;
    }
	

	// returns the text from the JSON data
    string GetTextFromJSON2 (JSONNode jsonData)
    {
        string text = "";

        //JSONNode resultados = jsonData["resultados"];
		JSONNode lines = jsonData["resultados"];
		 foreach(JSONNode line in lines.Children)
        {
			text += line["nregistro"];
			numRegistro = line["nregistro"];
			encontrado=true;
        }
        return text;
    }
	
	// returns the text from the JSON data
    void GetTextFromJSON3 (JSONNode jsonData)
    {
        
		string texto2;
		if(contadorMedicamento==0){
			Nombre_1 = jsonData["nombre"];
		}
		else if(contadorMedicamento==1){
			Nombre_2 = jsonData["nombre"];
		}


		JSONNode lines = jsonData["principiosActivos"];
		foreach(JSONNode line in lines.Children)
        {
			texto2 = line["nombre"];
			
			if(contadorMedicamento==0){
				listaPrincipios_1.Add(texto2);
			}
			else if(contadorMedicamento==1){
				listaPrincipios_2.Add(texto2);
			}
        }
    }
	
	public void Boton_Usuario()
	{
		Email_Text.text = EmailInfo.name;
		UIPopUp3.SetActive(true);
	}
	
	public void Boton_VolverAtrasUsuario()
	{
		UIPopUp3.SetActive(false);
	}
	
	public void Boton_Cerrar()
	{
		UIPopUp2.SetActive(false);
		numRegistro="";
	}
	
	
	private void BuscarInteracciones(string principio1, string principio2)
    {
        using (dbconn = new SqliteConnection(conn))
        {
            string Alim_Search, Desc_Search;
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT descripcion " + "FROM Interacciones_Medicamentos WHERE principio_1 LIKE '%"+principio1+"%' AND principio_2 LIKE '%"+principio2+"%' OR principio_1 LIKE '%"+principio2+"%' AND principio_2 LIKE '%"+principio1+"%'";// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
				Desc_Search = reader.GetString(0);
				listaDescripcion.Add(Desc_Search);
				hayInteracciones=true;

            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();


        }
   
    }
	

	
}