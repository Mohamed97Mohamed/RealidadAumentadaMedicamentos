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

public class LoginManager : MonoBehaviour
{
	
	//Sqlite
	private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
	private IDataReader reader;
	
	//Inputs
    public InputField Mail;
	public InputField Password;
	public Text TextError;
	
	private bool encontrado=false;
	
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
        dbconn.Open();*/
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	//Login
    public void Login_button(string sceneName)
    { 
        Login_function(Mail.text, Password.text,sceneName);
		
    }
	
	
	 //Search on Email and Password
    private void Login_function(string Mail, string Password,string sceneName)
    {
		
		using (dbconn = new SqliteConnection(conn))
        {
           
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * " + "FROM Usuarios WHERE email LIKE '%"+Mail+"%' AND password LIKE '%"+Password+"%'";// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
				encontrado = true;
            }
			if(encontrado==true){
				EmailInfo.name=Mail;
				EmailInterac.name=Mail;
				SceneManager.LoadScene(sceneName);
			}else{
				TextError.text="El Email o Contraseña son incorrectos!!";
			}
			
			encontrado = false;
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();


        }
    }
}
