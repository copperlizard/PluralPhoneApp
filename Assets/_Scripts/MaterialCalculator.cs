using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialCalculator : MonoBehaviour
{
    static float m_PI = 3.14159265359f;

    public MaterialManager m_matManager;

    public InputField m_matDia, m_matWeight;

    public Text m_answerText;

    private float m_selMatD = 0.0f, m_filLength = 0.0f, m_weight = -1.0f;

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

        SelectMaterial();

        m_matDiaPlaceHolderText.text = string.Format("{0:N3}", PlayerPrefs.GetFloat("savedMatDia", 2.85f));        
    }
    
    public void SaveMatDia() // called by change to mat diameter input field
    {
        PlayerPrefs.SetFloat("savedMatDia", float.Parse(m_matDia.text));
        m_matDiaPlaceHolderText.text = m_matDia.text;
        m_matDia.text = "";

        CalculateFilLength();
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

        float r2 = (float.Parse(m_matDiaPlaceHolderText.text) * 0.5f) * 0.001f; //mm to m
        r2 *= r2;

        m_filLength = v / (m_PI * r2);

        //Debug.Log("m_filLenght == " + m_filLength.ToString() + System.Environment.NewLine);

        m_answerText.text = string.Format("{0:N4}", m_filLength) + "m";
    }

    private float WeightToVolume (float w)
    {
        //V[m^3]=m[kg]/ρ[kg/m^3]
        return w / m_selMatD;        
    }
}
