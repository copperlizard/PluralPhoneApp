using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class ReelWeightManager : MonoBehaviour
{
    private List<float> m_reelWeights, m_dispWeights = new List<float>();

    [SerializeField]
    private InputField m_reelWeightInput;

    [SerializeField]
    private Dropdown m_reelWeightDropDown, m_reelWeightUnitDropDown;

    private Text m_reelWeightPlaceHolderText;

    private float m_selectedReelWeight; //in kG

    private int m_selectedReelWeightDropDownInd = 0;

    private bool m_weightsLoaded = false;

	// Use this for initialization
	void Awake ()
    {
        if (!LoadReelWeights())
        {
            Debug.Log("Failed to load reel weights");
        }        
        
        if (m_reelWeightInput == null)
        {
            Debug.Log("m_reelWeightInput not assigned!");
        }
        else
        {
            m_reelWeightPlaceHolderText = m_reelWeightInput.placeholder.GetComponent<Text>();
        }

        if (m_reelWeightDropDown == null)
        {
            Debug.Log("m_reelWeightDropDown not assigned!");
        }

        if (m_reelWeightUnitDropDown == null)
        {
            Debug.Log("m_reelWeightUnitDropDown not assigned!");
        }

        m_selectedReelWeightDropDownInd = Mathf.Min(PlayerPrefs.GetInt("lastReelWeightValue", 0), m_reelWeightDropDown.options.Count - 1);
        m_reelWeightDropDown.value = m_selectedReelWeightDropDownInd;
        
        UpdateReelWeightDisplayList();

        m_weightsLoaded = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private bool LoadReelWeights ()
    {
        FileStream file;
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/ReelWeightList.dat"))
        {
            file = File.Open(Application.persistentDataPath + "/ReelWeightList.dat", FileMode.Open);

            m_reelWeights = (List<float>)bf.Deserialize(file);
        }
        else
        {
            file = File.Create(Application.persistentDataPath + "/ReelWeightList.dat");

            m_reelWeights = new List<float>();
            m_reelWeights.Add(0.1f);
            m_reelWeights.Add(0.15f);
            m_reelWeights.Add(0.2f);

            file.Close();
            return SaveReelWeights();
        }

        file.Close();
        return true;
    }

    private bool SaveReelWeights ()
    {
        m_reelWeights.Sort();

        FileStream file;
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/ReelWeightList.dat"))
        {
            file = File.Open(Application.persistentDataPath + "/ReelWeightList.dat", FileMode.Open);
        }
        else
        {
            Debug.Log("error opening file!!!");
            return false;
        }
        
        bf.Serialize(file, m_reelWeights);
        file.Close();

        return true;
    }

    public void UpdateReelWeightDisplayList () // Called after new weight entered or weight unit change
    {
        m_selectedReelWeightDropDownInd = m_reelWeightDropDown.value;
        //m_selectedReelWeight = m_reelWeights[m_selectedReelWeightDropDownInd];

        m_dispWeights.Clear();

        foreach (float w in m_reelWeights)
        {
            switch (m_reelWeightUnitDropDown.value)
            {
                case 0: //kg
                    m_dispWeights.Add(w);
                    break;
                case 1: //g
                    m_dispWeights.Add(w * 1000.0f);
                    break;
                case 2: //lb
                    m_dispWeights.Add(w / 0.453592f);
                    break;
                case 3: //oz
                    m_dispWeights.Add(w / 0.0283495f);
                    break;
                default:
                    Debug.Log("error selecting reel weight unit");
                    break;
            }
        }

        m_reelWeightDropDown.options.Clear();

        foreach (float w in m_dispWeights)
        {
            m_reelWeightDropDown.options.Add(new Dropdown.OptionData(w.ToString()));
        }
    }

    public void CheckNewReelWeight () // Called when new weight entered
    {
        // Check if weight already in list, if not add to list
        float w = float.Parse(m_reelWeightPlaceHolderText.text);

        //Debug.Log("w pre conversion == " + w.ToString());

        //Convert w to KG
        switch (m_reelWeightUnitDropDown.value)
        {
            case 0: //kg                
                break;
            case 1: //g
                w *= 0.001f;
                break;
            case 2: //lb
                w *= 0.453592f;
                break;
            case 3: //oz
                w *= 0.0283495f;
                break;
            default:
                Debug.Log("error selecting reel weight unit");
                break;
        }

        //Debug.Log("w == " + w.ToString());

        m_selectedReelWeight = w;

        if (m_reelWeights.Contains(w))
        {
            //Debug.Log("w already in list");

            m_selectedReelWeightDropDownInd = m_reelWeights.IndexOf(w);
            m_reelWeightDropDown.value = m_selectedReelWeightDropDownInd;
            return;
        }
        else
        {
            m_reelWeights.Add(w);

            m_reelWeights.Sort();

            m_selectedReelWeightDropDownInd = m_reelWeights.IndexOf(w);
            m_reelWeightDropDown.value = m_selectedReelWeightDropDownInd;

            SaveReelWeights();
        }

        UpdateReelWeightDisplayList();
    }

    public void SelectReelWeight () // Called when drop down selection made
    {
        return;

        if (m_weightsLoaded)
        {
            Debug.Log("hello");

            PlayerPrefs.SetInt("lastReelWeightValue", m_reelWeightDropDown.value);
            m_reelWeightInput.text = m_dispWeights[m_reelWeightDropDown.value].ToString("r");
            m_reelWeightPlaceHolderText.text = m_dispWeights[m_reelWeightDropDown.value].ToString("r");
        }
    }
}
