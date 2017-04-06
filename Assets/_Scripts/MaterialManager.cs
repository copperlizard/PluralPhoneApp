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
    public double m_sg; // specific gravity
    public double m_d; // density

    public Mat()
    {
        m_name = "New";
        m_sg = 0.0d;
        m_d = 0.0d;
    }

    public Mat(string name, double sg = 0.0d, double d = 0.0d)
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

    public Dropdown m_calcMatDropDown, m_matMatDropDown, m_matDensityUnitDropDown;

    private Text m_matNamePlaceHolderText, m_matSGPlaceHolderText, m_matDPlaceHolderText;

    // Use this for initialization
    void Awake ()
    {
        LoadList();

        m_matNamePlaceHolderText = m_matName.placeholder.GetComponent<Text>();
        m_matSGPlaceHolderText = m_matSG.placeholder.GetComponent<Text>();
        m_matDPlaceHolderText = m_matD.placeholder.GetComponent<Text>();

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

        if (m_matDensityUnitDropDown == null)
        {
            Debug.Log("m_matDensityUnitDropDown not assigned!");
        }
        int lastDensitytUnit = PlayerPrefs.GetInt("lastDensityUnit", 0);
        m_matDensityUnitDropDown.value = lastDensitytUnit;
        
        UpdateMaterialPanel();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void UpdateMaterialName () //called when input field value changes on mat panel
    {
        //Ensure name is not a duplicate
        bool dupe = false;
        string newName = m_matName.text;
        int dupNum = 0;

        do
        {
            dupe = false;
            foreach (Mat mat in m_materials)
            {
                if (newName == mat.m_name)
                {
                    dupe = true;
                    dupNum++;

                    newName = name + dupNum.ToString();
                }
            }

        } while (dupe);

        m_materials[m_matMatDropDown.value].m_name = newName;

        SaveList(); //sorts list
        UpdateMaterialPanel();

        int matOptionNum = m_materials.IndexOf(new Mat(newName));
        m_matMatDropDown.value = matOptionNum;        
    }

    public void UpdateMaterialSG() //called when input field value changes on mat panel
    {
        //SG * ρH2O = ρsubstance

        //for D[kg/m^3] => ρH2O = 1000
        //for D[g/cm^3] => ρH2O = 1

        m_materials[m_matMatDropDown.value].m_sg = double.Parse(m_matSG.text);
        m_materials[m_matMatDropDown.value].m_d = m_materials[m_matMatDropDown.value].m_sg * 1000.0d;

        SaveList();
        UpdateMaterialPanel();
    }

    public void UpdateMaterialDensity() //called when input field value changes on mat panel
    {
        //SG = ρsubstance / ρH2O

        //for D[kg/m^3] => ρH2O = 1000
        //for D[g/cm^3] => ρH2O = 1

        double num = double.Parse(m_matD.text);
        //convert to kg/m^3
        switch (m_matDensityUnitDropDown.value)
        {
            case 0: //no conversion
                break;
            case 1: //g/cm^3
                num /= (1000.0d / 100.0d);
                break;
            case 2: //g/mm^3
                num /= (1000.0d / 1000.0d);
                break;
            case 3: //oz/in^3
                num /= (35.274d / 39.3701d);
                break;
            case 4: //lb/ft^3
                num /= (2.20462d / 3.28084d);
                break;
            default:
                break;
        }

        m_materials[m_matMatDropDown.value].m_d = num;
        m_materials[m_matMatDropDown.value].m_sg = m_materials[m_matMatDropDown.value].m_d / 1000.0d;

        SaveList();
        UpdateMaterialPanel();
    }

    public void UpdateMaterialDensityDisp ()
    {
        //convert to desired density unit from kg/m^3

        //Debug.Log("m_matDensityUnitDropDown.value == " + m_matDensityUnitDropDown.value.ToString());
        PlayerPrefs.SetInt("lastDensityUnit", m_matDensityUnitDropDown.value);

        double dNum = m_materials[m_matMatDropDown.value].m_d;
        switch (m_matDensityUnitDropDown.value)
        {
            case 0: //no conversion
                break;
            case 1: //g/cm^3
                dNum *= (1000.0d / 100.0d);
                break;
            case 2: //g/mm^3
                dNum *= (1000.0d / 1000.0d);
                break;
            case 3: //oz/in^3
                dNum *= (35.274d / 39.3701d);
                break;
            case 4: //lb/ft^3
                dNum *= (2.20462d / 3.28084d);
                break;
            default:
                break;
        }

        m_matDPlaceHolderText.text = string.Format("{0:N2}", dNum);
    }

    public void UpdateMaterialPanel () //called when m_matMatDropDon.value changes
    {        
        RefreshDropDowns();

        // update fields on materials panel  
        m_matName.text = "";
        m_matSG.text = "";
        m_matD.text = "";
        m_matNamePlaceHolderText.text = m_materials[m_matMatDropDown.value].m_name;
        m_matSGPlaceHolderText.text = string.Format("{0:N2}", m_materials[m_matMatDropDown.value].m_sg);

        //convert to desired density unit from kg/m^3
        double dNum = m_materials[m_matMatDropDown.value].m_d;
        switch (m_matDensityUnitDropDown.value)
        {
            case 0: //no conversion
                break;
            case 1: //g/cm^3
                dNum *= (1000.0d / 100.0d);
                break;
            case 2: //g/mm^3
                dNum *= (1000.0d / 1000.0d);
                break;
            case 3: //oz/in^3
                dNum *= (35.274d / 39.3701d);
                break;
            case 4: //lb/ft^3
                dNum *= (2.20462d / 3.28084d);
                break;
            default:
                break;
        }

        m_matDPlaceHolderText.text = string.Format("{0:N2}", dNum);
    }

    public void AddMaterial () //called by gui
    {
        bool dupe = false;
        string newName = "new";
        int dupNum = 0;

        do
        {
            dupe = false;
            foreach (Mat mat in m_materials)
            {
                if (newName == mat.m_name)
                {
                    dupe = true;
                    dupNum++;

                    newName = "new" + dupNum.ToString();
                }
            }

        } while (dupe);

        m_materials.Add(new Mat(newName));

        SaveList();
        RefreshDropDowns();

        int matOptionNum = m_materials.IndexOf(new Mat(newName));
        m_matMatDropDown.value = matOptionNum;
    }
    
    public void RemoveMaterial () //called by gui
    {
        m_materials.RemoveAt(m_matMatDropDown.value);
        if (m_materials.Count < 1)
        {
            AddMaterial();
        }
        
        m_matMatDropDown.value = 0;
        
        SaveList();
        RefreshDropDowns();
    }

    private void RefreshDropDowns ()
    {
        Dropdown.OptionData calcMatDropDownOption = m_calcMatDropDown.options[m_calcMatDropDown.value];
        Dropdown.OptionData matMatDropDownOption = m_matMatDropDown.options[m_matMatDropDown.value];
        
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

            m_materials.Add(new Mat("ABS", 1.04d, 1040.0d));
            m_materials.Add(new Mat("ASA", 1.08d, 1080.0d));
            m_materials.Add(new Mat("HIPS", 1.04d, 1040.0d));
            m_materials.Add(new Mat("PETG", 1.27d, 1270.0d));
            m_materials.Add(new Mat("TPU", 1.12d, 1120.0d));
            m_materials.Add(new Mat("PLA", 1.24d, 1240.0d));
            m_materials.Add(new Mat("PCABS", 1.2d, 1200.0d));
            m_materials.Add(new Mat("nPower", 1.26d, 1260.0d));
            m_materials.Add(new Mat("nPower Support", 1.26d, 1260.0d));
            m_materials.Add(new Mat("Nylon 680", 1.1d, 1100.0d));
            m_materials.Add(new Mat("SSU01", 1.24d, 1240.0d)); // soluble
            m_materials.Add(new Mat("SSU02", 1.24d, 1240.0d)); // softening            

            //AlphabatizeList();
            m_materials.Sort(); //because I'm lazy
            file.Close();
            SaveList();
            return;
        }

        file.Close();        
        return;
    }

    private bool SaveList ()
    {
        m_materials.Sort();

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

        /*
        Debug.Log("saving!");
        foreach (Mat mat in m_materials)
        {
            Debug.Log(mat.ToString());
        }
        */

        bf.Serialize(file, m_materials);
        file.Close();

        return true;
    }
}
