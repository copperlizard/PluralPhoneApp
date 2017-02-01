using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialCalculator : MonoBehaviour
{
    static double m_PI = 3.14159265359f;

    public MaterialManager m_matManager;

    public InputField m_matDia, m_matWeight, m_reelWeight;

    public Dropdown m_matDiaUnitDropDown, m_matWeightUnitDropDown, m_reelWeightUnitDropDown;

    public Text m_answerText;

    private double m_selMatD = 0.0f, m_filLength = 0.0f, m_weight = -1.0f, m_dia = 0.0f, m_emptyReelWeight = 0.0f;

    private Text m_matDiaPlaceHolderText, m_matWeightPlaceHolderText, m_reelWeightPlaceHolderText;
    
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

        if (m_reelWeightUnitDropDown == null)
        {
            Debug.Log("m_reelWeightUnitDropDown not assigned!");
        }

        if (m_reelWeight == null)
        {
            Debug.Log("m_reelWeight not assigned!");
        }

        if (m_answerText == null)
        {
            Debug.Log("m_answerText not assigned!");
        }
        m_answerText.text = "Enter Weight";
        
        m_matDiaPlaceHolderText = m_matDia.placeholder.GetComponent<Text>();
        m_matWeightPlaceHolderText = m_matWeight.placeholder.GetComponent<Text>();
        m_reelWeightPlaceHolderText = m_reelWeight.placeholder.GetComponent<Text>();

        int lastMat = PlayerPrefs.GetInt("lastMat", 0);
        if (lastMat > m_matManager.m_materials.Count - 1)
        {
            lastMat = 0;
            Debug.Log("error loading lastMat (exceeds mat list ind range)!");
        }
        m_matManager.m_calcMatDropDown.value = lastMat;

        int lastDiaUnit = PlayerPrefs.GetInt("lastDia", 0);
        int lastweightUnit = PlayerPrefs.GetInt("lastWeight", 0);
        int lastReelweightUnit = PlayerPrefs.GetInt("lastReelWeight", 0);
        m_matDiaUnitDropDown.value = lastDiaUnit;
        m_matWeightUnitDropDown.value = lastweightUnit;
        m_reelWeightUnitDropDown.value = lastReelweightUnit;

        SelectMaterial();

        //m_dia = PlayerPrefs.GetFloat("savedMatDia", 2.85f);
        //m_emptyReelWeight = PlayerPrefs.GetFloat("savedReelWeight", 0.1f);
        m_dia = double.Parse(PlayerPrefs.GetString("savedMatDia", 2.85f.ToString("r")));
        m_emptyReelWeight = double.Parse(PlayerPrefs.GetString("savedReelWeight", 0.1f.ToString("r")));

        UpdateMatDiaDisp();
        UpdateReelWeightDisp();       
    }
    
    public void SaveMatDia () // called by change to mat diameter input field
    {
        m_dia = double.Parse(m_matDia.text);

        switch (m_matDiaUnitDropDown.value)
        {
            case 0: //mm
                break;
            case 1: //cm
                m_dia *= 10.0d;
                break;
            case 2: //in
                m_dia *= 25.4d;
                break;
            default:
                Debug.Log("error selecting diameter unit");
                break;
        }

        //PlayerPrefs.SetFloat("savedMatDia", (float)m_dia);
        PlayerPrefs.SetString("savedMatDia", m_dia.ToString("r"));
        m_matDiaPlaceHolderText.text = m_matDia.text;
        m_matDia.text = "";

        CalculateFilLength();
    }

    public void SaveReelWeight () // called by change to reel weight input field
    {
        m_emptyReelWeight = double.Parse(m_reelWeight.text);

        switch (m_reelWeightUnitDropDown.value)
        {
            case 0: //kg
                break;
            case 1: //g
                m_emptyReelWeight *= 0.001d;                
                break;
            case 2: //lb
                m_emptyReelWeight *= 0.453592d;                
                break;
            case 3: //oz
                m_emptyReelWeight *= 0.0283495d;                
                break;
            default:
                Debug.Log("error selecting reel weight unit");
                break;
        }

        //PlayerPrefs.SetFloat("savedReelWeight", (float)m_emptyReelWeight);
        PlayerPrefs.SetString("savedReelWeight", m_emptyReelWeight.ToString("r"));
        m_reelWeightPlaceHolderText.text = m_reelWeight.text;
        m_reelWeight.text = "";

        CalculateFilLength();
    }

    public void UpdateMatDiaDisp () //called by madDia unit dropdown
    {
        PlayerPrefs.SetInt("lastDia", m_matDiaUnitDropDown.value);

        double dispDia = m_dia;
        switch (m_matDiaUnitDropDown.value)
        {
            case 0: //mm
                break;
            case 1: //cm
                dispDia /= 10.0d;
                break;
            case 2: //in
                dispDia /= 25.4d;
                break;
            default:
                Debug.Log("error selecting diameter unit");
                break;
        }

        m_matDiaPlaceHolderText.text = string.Format("{0:N2}", dispDia);
    }

    public void UpdateReelWeightDisp ()
    {
        PlayerPrefs.SetInt("lastReelWeight", m_reelWeightUnitDropDown.value);

        if (m_emptyReelWeight <= 0.0f)
        {
            return;
        }

        double dispWeight = m_emptyReelWeight;
        switch (m_reelWeightUnitDropDown.value)
        {
            case 0: //kg
                break;
            case 1: //g
                dispWeight /= 0.001d;
                break;
            case 2: //lb
                dispWeight /= 0.453592d;
                break;
            case 3: //oz
                dispWeight /= 0.0283495d;
                break;
            default:
                Debug.Log("error selecting weight unit");
                break;
        }

        m_reelWeightPlaceHolderText.text = string.Format("{0:N2}", dispWeight);
    }

    public void UpdateMatWeightDisp () //called by weight unit dropdown
    {
        PlayerPrefs.SetInt("lastWeight", m_matWeightUnitDropDown.value);

        if (m_weight <= 0.0f)
        {
            return;
        }

        double dispWeight = m_weight;
        switch (m_matWeightUnitDropDown.value)
        {
            case 0: //kg
                break;
            case 1: //g
                dispWeight /= 0.001d;
                break;
            case 2: //lb
                dispWeight /= 0.453592d;
                break;
            case 3: //oz
                dispWeight /= 0.0283495d;
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

        m_weight = double.Parse(m_matWeightPlaceHolderText.text);

        switch (m_matWeightUnitDropDown.value)
        {
            case 0: //kg
                break;
            case 1: //g
                m_weight *= 0.001d;
                break;
            case 2: //lb
                m_weight *= 0.453592d;
                break;
            case 3: //oz
                m_weight *= 0.0283495d;
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
        
        double v = WeightToVolume(m_weight - m_emptyReelWeight);
              
        //v = pi*r^2*h
        //h = v/(pi*r^2)

        double r2 = (m_dia * 0.5d) * 0.001d; //mm to m
        r2 *= r2;

        m_filLength = v / (m_PI * r2);

        m_answerText.text = "<b>Length <size=52>(Volume)</size></b>";
        m_answerText.text += System.Environment.NewLine + string.Format("{0:N2}", m_filLength) + "m <size=52>(" + string.Format("{0:N2}", v * 1000000f) + "cm\xB3)</size>";
        m_answerText.text += System.Environment.NewLine + string.Format("{0:N2}", m_filLength * 3.28084f) + "ft <size=52>(" + string.Format("{0:N2}", v * 35.3147f) + "ft\xB3)</size>";
        m_answerText.text += System.Environment.NewLine + string.Format("{0:N2}", m_filLength * 39.3701f) + "in <size=52>(" + string.Format("{0:N2}", v * 61023.7f) + "in\xB3)</size>";        
    }

    private double WeightToVolume (double w)
    {
        //V[m^3]=m[kg]/ρ[kg/m^3]
        return w / m_selMatD;        
    }
}
