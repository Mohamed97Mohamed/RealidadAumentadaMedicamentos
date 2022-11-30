using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//References
using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using UnityEngine.UI;

public class RegisterManager : MonoBehaviour
{
	private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
	private IDataReader reader;
    public InputField Mail2;
	public InputField Password2;
	public InputField Nombre;
	public InputField Apellidos;
	public InputField Telefono;
	public Text TextError2;
	
	string DatabaseName = "BaseDatos.db";
	
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
        dbconn.Open(); */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	//Create
    public void Create_button(string sceneName)
    { 
        Nuevo_Usuario(Mail2.text, Password2.text, Nombre.text, Apellidos.text, Telefono.text, sceneName);
		
    }
	
	//Reset
    public void Reset_button()
    { 
        Mail2.text="";
		Password2.text="";
		Nombre.text="";
		Apellidos.text="";
		Telefono.text="";
		
    }
	
	//Search on Email and Password
    private void Nuevo_Usuario(string Mail, string Password, string Nombre, string Apellidos, string Telefono, string sceneName)
    {
		int contador=0;
		using (dbconn = new SqliteConnection(conn))
        {
           
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * " + "FROM Usuarios WHERE email LIKE '%"+Mail+"%'";// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
				contador++;
            }
			
			reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;

			if(contador==0){

				Usuario.email=Mail;
				Usuario.password=Password;
				Usuario.nombre=Nombre;
				Usuario.apellidos=Apellidos;
				Usuario.telefono=Telefono;
				SceneManager.LoadScene(sceneName);
				
			}else{
				TextError2.text="Email ya existe!";
			}
        }

    }
	
	
}
