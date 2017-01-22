using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Mat : IEquatable<Mat>, IComparable<Mat>
{
    public string m_name;
    public float m_sg; // specific gravity
    public float m_d; // density

    public Mat()
    {
        m_name = "New";
        m_sg = 0.0f;
        m_d = 0.0f;
    }

    public Mat(string name, float sg = 0.0f, float d = 0.0f)
    {
        m_name = name;
        m_sg = sg;
        m_d = d;
    }

    public override string ToString()
    {
        return "Name: " + m_name + "   Specific Gravity: " + m_sg.ToString() + "   Density: " + m_d.ToString();
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Mat objAsMat = obj as Mat;
        if (objAsMat == null) return false;
        else return Equals(objAsMat);
    }
    public bool Equals(Mat other)
    {
        if (other == null) return false;
        return (this.m_name.Equals(other.m_name));
    }
    public override int GetHashCode()
    {
        return m_name.GetHashCode();
    }

    public int CompareTo(Mat compareMat)
    {
        // A null value means that this object is greater.
        if (compareMat == null)
            return 1;

        else
            return this.m_name.CompareTo(compareMat.m_name);
    }
}

public class MaterialManager : MonoBehaviour
{
    [HideInInspector]
    public List<Mat> m_materials;

    public InputField m_matName, m_matSG, m_matD;

    public Dropdown m_calcMatDropDown, m_matMatDropDown;

	// Use this for initialization
	void Start ()
    {
        if (m_matName == null)
        {
            Debug.Log("m_matName not assigned!");
        }

        if (m_matSG == null)
        {
            Debug.Log("m_matSG not assigned!");
        }

        if (m_matD == null)
        {
            Debug.Log("m_matD not assigned!");
        }

        if (m_calcMatDropDown == null)
        {
            Debug.Log("m_calcMatDropDown not assigned!");
        }

        if (m_matMatDropDown == null)
        {
            Debug.Log("m_matMatDropDown not assigned!");
        }

        LoadList();
        RefreshDropDowns();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void UpdateMaterialName (string name) //called when input field value changes on mat panel
    {
        //Ensure name is not a duplicate (add _duplicate#)

        UpdateMaterialPanel();
    }

    public void UpdateMaterialSG(float sg) //called when input field value changes on mat panel
    {


        UpdateMaterialPanel();
    }

    public void UpdateMaterialD(float d) //called when input field value changes on mat panel
    {


        UpdateMaterialPanel();
    }

    public void UpdateMaterialPanel () //called when m_matMatDropDon.value changes
    {
        RefreshDropDowns();

        // update fields on materials panel
    }

    public void AddMaterial () //called by gui
    {
        RefreshDropDowns();
    }

    private void AlphabatizeList() //called by refreshdropdowns and loadlist
    {

    }

    private void RefreshDropDowns ()
    {
        //Ensure drop down selection does not change when/after alphabatizing

        Dropdown.OptionData calcMatDropDownOption = m_calcMatDropDown.options[m_calcMatDropDown.value];
        Dropdown.OptionData matMatDropDownOption = m_matMatDropDown.options[m_matMatDropDown.value];

        Debug.Log("selected m_calcMatDropDown material == " + calcMatDropDownOption.text);
        Debug.Log("selected m_matMatDropDown material == " + matMatDropDownOption.text);

        //AlphabatizeList();
        m_materials.Sort();

        m_calcMatDropDown.options.Clear();
        m_matMatDropDown.options.Clear();

        foreach (Mat mat in m_materials)
        {
            m_calcMatDropDown.options.Add(new Dropdown.OptionData(mat.m_name));
            m_matMatDropDown.options.Add(new Dropdown.OptionData(mat.m_name));
        }

        int calcOptionNum = m_materials.IndexOf(new Mat(calcMatDropDownOption.text));
        int matOptionNum = m_materials.IndexOf(new Mat(matMatDropDownOption.text));

        m_calcMatDropDown.value = calcOptionNum;
        m_matMatDropDown.value = matOptionNum;
    }

    private void LoadList ()
    {
        FileStream file;
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/MaterialList.dat"))
        {
            file = File.Open(Application.persistentDataPath + "/MaterialList.dat", FileMode.Open);            
            
            m_materials = (List<Mat>)bf.Deserialize(file);
        }
        else
        {
            file = File.Create(Application.persistentDataPath + "/MaterialList.dat");

            // GENERATE BASIC MATERIALS!!!

            m_materials.Add(new Mat("ABS", 0.0f, 0.0f));
            m_materials.Add(new Mat("ASA", 0.0f, 0.0f));
            m_materials.Add(new Mat("HIPS", 0.0f, 0.0f));
            m_materials.Add(new Mat("PETG", 0.0f, 0.0f));
            m_materials.Add(new Mat("TPU", 0.0f, 0.0f));
            m_materials.Add(new Mat("PLA", 0.0f, 0.0f));
            m_materials.Add(new Mat("Nylon", 0.0f, 0.0f));
            m_materials.Add(new Mat("SSU01", 0.0f, 0.0f)); // soluble
            m_materials.Add(new Mat("SSU02", 0.0f, 0.0f)); // softening
            m_materials.Add(new Mat("SSU03", 0.0f, 0.0f)); // water soluble

            //AlphabatizeList();
            m_materials.Sort();
            file.Close();
            SaveList();
            return;
        }

        file.Close();
        AlphabatizeList();
        return;
    }

    private bool SaveList ()
    {
        FileStream file;
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/MaterialList.dat"))
        {
            file = File.Open(Application.persistentDataPath + "/MaterialList.dat", FileMode.Open);
        }
        else
        {
            Debug.Log("error opening file!!!");
            return false;            
        }

        foreach (Mat mat in m_materials)
        {
            Debug.Log("mat.name == " + mat.m_name);
        }

        bf.Serialize(file, m_materials);
        file.Close();

        return true;
    }

    private float SGtoD (float SG)
    {
        return 0.0f;
    }

    private float DtoSG(float D)
    {
        return 0.0f;
    }
}
