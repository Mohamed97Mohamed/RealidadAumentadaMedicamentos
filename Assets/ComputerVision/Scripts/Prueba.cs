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


public class Prueba : MonoBehaviour
{
	public Toggle check;
	public Text textoCheck;
	public Text texto;
	public Text texto2;
	ArrayList arlist = new ArrayList(); 
	List<string> items;
	public string variable="";
	
	
	private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
	
	string DatabaseName = "BaseDatos.db";
    // Start is called before the first frame update
	
	// instance
    public static Prueba instance;
	void Awake ()
    {
        // set the instance
        instance = this;
    }
	
    void Start()
    {
		
		
        string filepath = Application.dataPath + "/Plugins/" + DatabaseName;

        //open db connection
        conn = "URI=file:" + filepath;

        Debug.Log("Stablishing connection to: " + conn);
        dbconn = new SqliteConnection(conn);
        dbconn.Open();
		
		//DROPDOWN
		var dropdown = transform.GetComponent<Dropdown>();
		dropdown.options.Clear();
		items = new List<string>();
		Search_function();
		
		/*items.Add("Item 1");
		items.Add("Item 2");
		items.Add("Item 3");
		items.Add("Item 4");*/
		
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
		texto.text = dropdown.options[index].text;
		variable = dropdown.options[index].text;
		
		textoCheck.text=variable;
		
		 //Controlador.instance.StartCoroutine("SetData", variable);
	}
	
	public void Pulsar(string Global){
		/*arlist.Clear();
		Search_function("VALSARTAN");
		string secondElement = (string) arlist[0];
		string secondElement2 = (string) arlist[1];
		string textoPrint = secondElement+" "+  secondElement2;
		textoCheck.text=textoPrint;*/
		
		
		//texto2.text = variable;
		//Debug.Log(" VAR =" + Global.name);
		Debug.Log(" VAR =" + Global);
	}
	public void Atras(string sceneName){
		SceneManager.LoadScene(sceneName);
	}
	
	
	private void Search_function()
    {
        using (dbconn = new SqliteConnection(conn))
        {
            string Name_readers_Search;
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT principio " + "FROM Principios";// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                //  string id = reader.GetString(0);
                Name_readers_Search = reader.GetString(0);
				items.Add(Name_readers_Search);
				Debug.Log(" DENTRO");
				//arlist.Add(Name_readers_Search);
                //Address_readers_Search = reader.GetString(1);
                //data_staff.text += Name_readers_Search + " - " + Address_readers_Search + "\n";

               // Debug.Log(" name =" + Name_readers_Search + "Address=" + Address_readers_Search);
			   //Debug.Log(" name =" + Name_readers_Search);

            }
			Debug.Log(" AQUIII");
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();


        }
   
    }
	
	public void valueChanged(Toggle t)
    {
        if (t.isOn) {
            t.GetComponentInChildren<Text> ().text = "Toggle is on";
        } else {
            t.GetComponentInChildren<Text> ().text = "Toggle is off";
        }
    }
	
	public void BOTON(){
		if (check.isOn) {
            //check.GetComponentInChildren<Text> ().text = "ON";
			textoCheck.text=check.GetComponentInChildren<Text> ().text;
			//textoCheck.text="ON";
        } else {
            //check.GetComponentInChildren<Text> ().text = "OFF";
			textoCheck.text=check.GetComponentInChildren<Text> ().text;
			//textoCheck.text="OFF";
        }
	}
	
}
