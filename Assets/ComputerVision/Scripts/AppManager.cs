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

public static class EmailInfo
{
	public static string name;
}

public class AppManager : MonoBehaviour
{
    // Computer Vision subscription key
    public string subKey;

    // Computer Vision API url
    public string url;

    // on-screen text which shows the text we've analyzed
    public TextMeshProUGUI uiText;
	

    // instance
    public static AppManager instance;
	
	//Arraylist
	ArrayList listaPrincipios = new ArrayList(); 
	ArrayList listaAlergias = new ArrayList(); 
	ArrayList listaAlimentos = new ArrayList(); 
	ArrayList listaDescripcion = new ArrayList(); 
	
	//Sqlite
	private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    private IDataReader reader;
	string DatabaseName = "BaseDatos.db";
	

	private bool encontrado=false;
	private string codNacional;
	private string numRegistro;
	private string Nombre;
	private bool comerc;
	private bool receta;
	private string Laboratorio;
	private bool hayAlergias=false;
	private bool hayInteracciones=false;
	private int ID_principio;
	private int ID_usuario;
	
	//Objetos
	public GameObject UIPrincipal;
	public GameObject UIPopUp;
	public GameObject UIPopUp2;
	public GameObject UIPopUp3;
	[SerializeField] Image Comerc_Image;
	[SerializeField] Image Alergias_Image;
	[SerializeField] Image Interac_Image;
	
	//Text
	public Text Email_Text;
	public Text Nombre_Text;
	public Text Nombre_Text_2;
	public Text Comerc_Text;
	public Text Receta_Text;
	public Text Registro_Text;
	public Text CN_Text;
	public Text Laboratorio_Text;
	public Text Principios_Text;
	public Text Alergias_Text;
	public Text Interac_Text;
	
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
        dbconn.Open(); */
    }
	


    // sends the image to the Computer Vision API and returns a JSON file
    public IEnumerator GetImageData (byte[] imageData)
    {
        uiText.text = "<i>[Detectando...]</i>";
		listaPrincipios.Clear();
		listaAlergias.Clear();
		listaAlimentos.Clear();
		listaDescripcion.Clear();

		
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
		  
		}
				
		if(encontrado==false){
			uiText.text = "<i>[No se ha detectado el medicamento!!]</i>";
		}

		string URL2="https://cima.aemps.es/cima/rest/medicamento?nregistro="+numRegistro+"&carganotas=1&cargamateriales=1";
	
		UnityWebRequest request2 = UnityWebRequest.Get(URL2);
		
		// create a download handler to receive the JSON file
		request2.downloadHandler = new DownloadHandlerBuffer();
		
		// send the content to the API and wait for a response
		yield return request2.SendWebRequest();
		
		// convert the content string to a JSON file
		JSONNode jsonData3 = JSON.Parse(request2.downloadHandler.text);
		
		// get just the text from the JSON file and display on-screen
		string imageText3 = GetTextFromJSON3(jsonData3);
		
		if(encontrado==true){
			uiText.text = "";
			UIPopUp2.SetActive(true);
			Nombre_Text_2.text = Nombre;
			
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
    string GetTextFromJSON3 (JSONNode jsonData)
    {
        string text = "";
		string texto2;
        Nombre = jsonData["nombre"];
		comerc = jsonData["comerc"];
		receta = jsonData["receta"];
		Laboratorio = jsonData["labtitular"];

		JSONNode lines = jsonData["principiosActivos"];
		 foreach(JSONNode line in lines.Children)
        {
			texto2 = line["nombre"];
			listaPrincipios.Add(texto2);
        }
        return text;
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
	
	public void Boton_Cerrar2()
	{
		UIPopUp2.SetActive(false);
	}
	
	public void Boton_MasInfo()
	{
		UIPopUp2.SetActive(false);
		UIPrincipal.SetActive(false);
		UIPopUp.SetActive(true);
		
		if (!UIPrincipal.activeSelf){
			Nombre_Text.text = Nombre;
			//Texto de si esta comercializado
			if(comerc==true){
				Comerc_Text.text = "COMERCIALIZADO";
				Comerc_Image.color = new Color32(16, 226, 79, 255);
				
			}else{
				Comerc_Text.text = "NO COMERCIALIZADO";
				Comerc_Image.color = new Color32(243, 13, 15, 255);
			}
			
			//Texto de si es con receta o no
			if(receta==true){
				Receta_Text.text = "CON RECETA MEDICA";
			}else{
				Receta_Text.text = "SIN RECETA MEDICA";
			}
			
			Registro_Text.text=numRegistro;
			CN_Text.text=codNacional;
			Laboratorio_Text.text=Laboratorio;
			
			string textoPrincipios="";
			string principioActual="";
			for (int i = 0; i < listaPrincipios.Count; i++){
				principioActual = (string) listaPrincipios[i];
				
				Buscar_ID_Principio(principioActual);
				Buscar_ID_Usuario();
				BuscarAlergias(principioActual);
				
				BuscarInteracciones(principioActual);
				textoPrincipios+=principioActual;
				textoPrincipios+="\n";
				Debug.Log(principioActual);
			}
			Principios_Text.text=textoPrincipios;
			
			string textoAlergias="";
			if(hayAlergias==true){
				for (int i = 0; i < listaAlergias.Count; i++){	
					textoAlergias+=(string) listaAlergias[i];
					textoAlergias+="\n";
				}
				Alergias_Image.color = new Color32(243, 13, 15, 255);
			}else{
				textoAlergias+="No tienes alergia de este medicamento";
				Alergias_Image.color = new Color32(16, 226, 79, 255);
			}
			hayAlergias=false;
			Alergias_Text.text =textoAlergias;
			
			string textoInteracciones="";
			if(hayInteracciones==true){
				for (int i = 0; i < listaAlimentos.Count; i++){	
					textoInteracciones+="Alimento: "+(string) listaAlimentos[i]+"\n";
					textoInteracciones+="Descripcion: "+(string) listaDescripcion[i]+"\n";
				}
				Interac_Image.color = new Color32(16, 226, 79, 255);
			}else{
				textoInteracciones+="No hay Interacciones de alimentos con este medicamento";
				Interac_Image.color = new Color32(243, 13, 15, 255);
			}
			hayInteracciones=false;
			Interac_Text.text=textoInteracciones;
			
			ID_principio=0;
			ID_usuario=0;
			
			
		}
	}
	
	public void Boton_Ficha()
	{
		Application.OpenURL("https://cima.aemps.es/cima/dochtml/ft/"+numRegistro+"/FT_"+numRegistro+".html");
	}
	
	public void Boton_Prospecto()
	{
		Application.OpenURL("https://cima.aemps.es/cima/dochtml/p/"+numRegistro+"/P_"+numRegistro+".html");
	}
	
	public void Boton_Cerrar()
	{
		UIPrincipal.SetActive(true);
		UIPopUp.SetActive(false);
		numRegistro="";
	}
	

	
	private void Buscar_ID_Principio(string principio)
    {
		using (dbconn = new SqliteConnection(conn))
        {
            string Name_readers_Search;
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT ID " + "FROM Principio WHERE principio LIKE '%"+principio+"%'";// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                ID_principio = reader.GetInt32(0);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
        }
    }
	
	private void Buscar_ID_Usuario()
    {
		using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT ID " + "FROM Usuarios WHERE email LIKE '%"+EmailInfo.name+"%'";// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                ID_usuario = reader.GetInt32(0);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
        }
   
    }
	
	private void BuscarAlergias(string principio)
    {
		using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
			string sqlQuery = "SELECT * " + "FROM Lista_Alergias WHERE ID_usuario LIKE "+ID_usuario+" AND ID_principio LIKE "+ID_principio;
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                listaAlergias.Add(principio);
				hayAlergias=true;
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
        }
   
    }
	
	private void BuscarInteracciones(string principio)
    {
        using (dbconn = new SqliteConnection(conn))
        {
            string Alim_Search, Desc_Search;
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT alimento, descripcion " + "FROM Interacciones_Alimentos WHERE principio LIKE '%"+principio+"%'";// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                Alim_Search = reader.GetString(0);
				Desc_Search = reader.GetString(1);
				listaAlimentos.Add(Alim_Search);
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