using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialCalculator : MonoBehaviour
{
    static float m_PI = 3.14159265359f;

    public MaterialManager m_matManager;

    public InputField m_matDia, m_matWeight;

    public Dropdown m_matDiaUnitDropDown, m_matWeightUnitDropDown;

    public Text m_answerText;

    private float m_selMatD = 0.0f, m_filLength = 0.0f, m_weight = -1.0f, m_dia = 0.0f;

    private Text m_matDiaPlaceHolderText, m_matWeightPlaceHolderText;
    
    // Use this for initialization
    void Start ()
    {
		if (m_matManager == null)
        {
            Debug.Log("m_matManager not assigned!");
        }

        if (m_matDia == null)
        {
            Debug.Log("m_matDia not assigned!");
        }

        if (m_matWeight == null)
        {
            Debug.Log("m_matWeight not assigned!");
        }

        if (m_matDiaUnitDropDown == null)
        {
            Debug.Log("m_matDiaUnitDropDown not assigned!");
        }

        if (m_matWeightUnitDropDown == null)
        {
            Debug.Log("m_matWeightUnitDropDown not assigned!");
        }

        if (m_answerText == null)
        {
            Debug.Log("m_answerText not assigned!");
        }
        m_answerText.text = "Enter Weight";
        
        m_matDiaPlaceHolderText = m_matDia.placeholder.GetComponent<Text>();
        m_matWeightPlaceHolderText = m_matWeight.placeholder.GetComponent<Text>();

        int lastMat = PlayerPrefs.GetInt("lastMat", 0);
        if (lastMat > m_matManager.m_materials.Count - 1)
        {
            lastMat = 0;
            Debug.Log("error loading lastMat (exceeds mat list ind range)!");
        }
        m_matManager.m_calcMatDropDown.value = lastMat;

        int lastDiaUnit = PlayerPrefs.GetInt("lastDia", 0);
        int lastweightUnit = PlayerPrefs.GetInt("lastWeight", 0);
        m_matDiaUnitDropDown.value = lastDiaUnit;
        m_matWeightUnitDropDown.value = lastweightUnit;

        SelectMaterial();

        m_dia = PlayerPrefs.GetFloat("savedMatDia", 2.85f);

        float dispDia = m_dia;
        switch (m_matDiaUnitDropDown.value)
        {
            case 0: //mm
                break;
            case 1: //cm
                dispDia /= 10.0f;
                break;
            case 2: //in
                dispDia /= 25.4f;
                break;
            default:
                Debug.Log("error selecting diameter unit");
                break;
        }
        m_matDiaPlaceHolderText.text = string.Format("{0:N2}", dispDia);        
    }
    
    public void SaveMatDia() // called by change to mat diameter input field
    {
        m_dia = float.Parse(m_matDia.text);

        switch (m_matDiaUnitDropDown.value)
        {
            case 0: //mm
                break;
            case 1: //cm
                m_dia *= 10.0f;
                break;
            case 2: //in
                m_dia *= 25.4f;
                break;
            default:
                Debug.Log("error selecting diameter unit");
                break;
        }

        PlayerPrefs.SetFloat("savedMatDia", m_dia);
        m_matDiaPlaceHolderText.text = m_matDia.text;
        m_matDia.text = "";

        CalculateFilLength();
    }

    public void UpdateMatDiaDisp () //called by madDia unit dropdown
    {
        PlayerPrefs.SetInt("lastDia", m_matDiaUnitDropDown.value);

        float dispDia = m_dia;
        switch (m_matDiaUnitDropDown.value)
        {
            case 0: //mm
                break;
            case 1: //cm
                dispDia /= 10.0f;
                break;
            case 2: //in
                dispDia /= 25.4f;
                break;
            default:
                Debug.Log("error selecting diameter unit");
                break;
        }

        m_matDiaPlaceHolderText.text = string.Format("{0:N3}", dispDia);
    }

    public void UpdateMatWeightDisp () //called by weight unit dropdown
    {
        PlayerPrefs.SetInt("lastWeight", m_matWeightUnitDropDown.value);

        if (m_weight <= 0.0f)
        {
            return;
        }

        float dispWeight = m_weight;
        switch (m_matWeightUnitDropDown.value)
        {
            case 0: //kg
                break;
            case 1: //g
                dispWeight /= 0.001f;
                break;
            case 2: //lb
                dispWeight /= 0.453592f;
                break;
            case 3: //oz
                dispWeight /= 0.0283495f;
                break;
            default:
                Debug.Log("error selecting weight unit");
                break;
        }

        m_matWeightPlaceHolderText.text = string.Format("{0:N2}", dispWeight);
    }

    public void SelectMaterial () //called when m_matManager.m_calcMatDropDown.value changes
    {
        m_selMatD = m_matManager.m_materials[m_matManager.m_calcMatDropDown.value].m_d;
        PlayerPrefs.SetInt("lastMat", m_matManager.m_calcMatDropDown.value);

        CalculateFilLength();
    }

    public void SetWeight ()
    {
        m_matWeightPlaceHolderText.text = m_matWeight.text;
        m_matWeight.text = "";

        m_weight = float.Parse(m_matWeightPlaceHolderText.text);

        switch (m_matWeightUnitDropDown.value)
        {
            case 0: //kg
                break;
            case 1: //g
                m_weight *= 0.001f;
                break;
            case 2: //lb
                m_weight *= 0.453592f;
                break;
            case 3: //oz
                m_weight *= 0.0283495f;
                break;
            default:
                Debug.Log("error selecting weight unit");
                break;
        }

        CalculateFilLength();
    }

    public void CalculateFilLength ()  
    {   
        if (m_weight < 0)
        {
            return;
        }

        //Debug.Log("calculating length...");
        
        float v = WeightToVolume(m_weight);
        //float v = 1.0f;

        //v = pi*r^2*h
        //h = v/(pi*r^2)

        float r2 = (m_dia * 0.5f) * 0.001f; //mm to m
        r2 *= r2;

        m_filLength = v / (m_PI * r2);

        //Debug.Log("m_filLenght == " + m_filLength.ToString() + System.Environment.NewLine);

        m_answerText.text = string.Format("{0:N2}", m_filLength) + "m";
        m_answerText.text += System.Environment.NewLine + string.Format("{0:N2}", m_filLength * 3.28084f) + "ft";
        m_answerText.text += System.Environment.NewLine + string.Format("{0:N2}", m_filLength * 39.3701f) + "in";        
    }

    private float WeightToVolume (float w)
    {
        //V[m^3]=m[kg]/ρ[kg/m^3]
        return w / m_selMatD;        
    }
}
