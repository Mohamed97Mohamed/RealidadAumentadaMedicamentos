using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//References
using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
public static class Principio
{
	public static string name;
}

public static class Usuario
{
	public static string nombre;
	public static string apellidos;
	public static string telefono;
	public static string email;
	public static string password;
	
}
public class AlergiasManager : MonoBehaviour
{
	[SerializeField] private Transform m_ContentContainer;
    [SerializeField] private GameObject m_ItemPrefab;
	
	private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
	
	string DatabaseName = "BaseDatos.db";
	
	public Text texto2;
	public Text texto2_rojo;
	public Text texto2_verde;
	public Text Title_Text_1;
	public Dropdown Dropdown;
	
	private List<string> items;
	private string principio="";
	private int contador2=0;
	private bool encontrado=false;
	private int ID_principio; 
	private int ID_usuario;

	
	public GameObject UIMainAlergias;
	public GameObject UIPopUp;
	
	ArrayList listaPrincipios = new ArrayList(); 
	
	// instance
    public static AlergiasManager instance;
	void Awake ()
    {
        // set the instance
        instance = this;
    }
	
    // Start is called before the first frame update
    void Start()
    {
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
		
		
		
		//DROPDOWN
		var dropdown = transform.GetComponent<Dropdown>();
		dropdown.options.Clear();
		items = new List<string>();
		BuscarPrincipiosActivos();

		
		foreach(var item in items)
		{
			dropdown.options.Add(new Dropdown.OptionData() {text = item});
		}
		
		DropdownItemSelected(dropdown);
		dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); }) ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void DropdownItemSelected(Dropdown dropdown)
	{
		int index = dropdown.value;
		principio = dropdown.options[index].text;
		Principio.name=dropdown.options[index].text;

	}
	
	//Añadimos el pirncipio al Scroll View
	public void Pulsar2()
	{
		var item_go = Instantiate(m_ItemPrefab);

		item_go.GetComponentInChildren<Text>().text = Principio.name;

		//parent the item to the content container
		item_go.transform.SetParent(m_ContentContainer);
		//reset the item's scale -- this can get munged with UI prefabs
		item_go.transform.localScale = Vector2.one;

	}
	
	public void Boton_Añadir(){
		
		Debug.Log("Email: "+Usuario.email);
		Debug.Log("Password: "+Usuario.password);
		Debug.Log("Nombre: "+Usuario.nombre);
		Debug.Log("Telefono: "+Usuario.telefono);
		
		if(listaPrincipios.Contains(Principio.name))
		{         
		  encontrado = true;
		}
		
		if(encontrado==false)
		{
			Title_Text_1.text="¿Quieres añadir el "+Principio.name+" a la lista?";
			UIMainAlergias.SetActive(false);
			UIPopUp.SetActive(true);
		}
		else{
			texto2_verde.text="";
			texto2_rojo.text="El principio activo "+Principio.name+" ya esta en la lista!";
		}
		
		encontrado=false;
		
	}
	
	public void Boton_Cancelar(){
		
		UIMainAlergias.SetActive(true);
		UIPopUp.SetActive(false);
		
		
	}
	
	public void Boton_Aceptar(){

		listaPrincipios.Add(Principio.name);
		UIMainAlergias.SetActive(true);
		UIPopUp.SetActive(false);
		texto2_rojo.text="";
		texto2_verde.text="El principio activo "+Principio.name+" se ha introducido correctamente!";
		Pulsar2();
		
		
	}
	
	public void Boton_Finalizar(string sceneName){
		Crear_Usuario();
		Buscar_ID_Usuario();
		
		for (int i = 0; i < listaPrincipios.Count; i++){
			 string principio = (string) listaPrincipios[i];
			 BuscarUnPrincipio(principio);
			 InsertarListaAlergias(ID_usuario, ID_principio);
			 
		}
		EmailInfo.name=Usuario.email;
		EmailInterac.name=Usuario.email;

		SceneManager.LoadScene(sceneName);
	}
	
	
	private void InsertarListaAlergias(int ID_Usuario, int ID_Principio)
    {
		using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            dbcmd = dbconn.CreateCommand();
            sqlQuery = string.Format("insert into Lista_Alergias (ID_usuario, ID_principio) values (\"{0}\",\"{1}\")", ID_Usuario, ID_Principio);// table name
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbconn.Close();
        }
		
   
    }
	
	private void BuscarUnPrincipio(string principio)
    {
		using (dbconn = new SqliteConnection(conn))
        {
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
            string sqlQuery = "SELECT ID " + "FROM Usuarios WHERE email LIKE '%"+Usuario.email+"%'";// table name
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
	
	private void BuscarPrincipiosActivos()
    {
        using (dbconn = new SqliteConnection(conn))
        {
            string Name_readers_Search;
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT principio " + "FROM Principio";// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                //  string id = reader.GetString(0);
                Name_readers_Search = reader.GetString(0);
				items.Add(Name_readers_Search);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
        }
   
    }
	
	private void Crear_Usuario()
    {
		using (dbconn = new SqliteConnection(conn))
        {
			dbconn.Open(); //Open connection to the database.
			dbcmd = dbconn.CreateCommand();
			string sqlQuery2 = string.Format("insert into Usuarios (email, password, nombre, apellidos, telefono) values (\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\")", Usuario.email, Usuario.password, Usuario.nombre, Usuario.apellidos, Usuario.telefono);// table name
			dbcmd.CommandText = sqlQuery2;
			dbcmd.ExecuteScalar();
			dbconn.Close();
			
        }
    }
	
}
