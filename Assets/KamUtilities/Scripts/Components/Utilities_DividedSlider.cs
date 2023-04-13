using System;
using System.Collections;
using System.Collections.Generic;
using Kam.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class Utilities_DividedSlider : MonoBehaviour
{
    public string propertyName;
    public int divisionCurrent;
    public float minValue;
    public float maxValue;
    public int divisionAmount;
    public List<GameObject> divisionsInOrder = new List<GameObject>();
    
    public UnityEvent<float> onValueChange;

    public int DivisionCurrent
    {
        get => divisionCurrent;
        set
        {
            divisionCurrent = value;
            currentValue = minValue + valuePerDivision * divisionCurrent;
            PlayerPrefs.SetFloat(propertyName, currentValue);
            //Debug.Log($"Saving: {propertyName} = {currentValue}");
            foreach (var division in divisionsInOrder)
            {
                division.SetActive(false);
            }

            for (int i = 0; i < divisionCurrent; i++)
            {
                divisionsInOrder[i].SetActive(true);
            }
            
            onValueChange.Invoke(currentValue);
        }
    }
    
    float currentValue;
    public float CurrentValue
    {
        get => currentValue;
        set
        {
            currentValue = value;
            PlayerPrefs.SetFloat(propertyName, currentValue);
            //Debug.Log($"Saving: {propertyName} = {currentValue}");
            
            int newDivisionCurrent = (int) (KamUtilities.Map(currentValue, minValue, maxValue, 0, maxValue-minValue) / valuePerDivision);
            if (newDivisionCurrent > divisionAmount)
            {
                newDivisionCurrent = divisionAmount;
            }
            else if (newDivisionCurrent < 0)
            {
                newDivisionCurrent = 0;
            }
            
            DivisionCurrent = newDivisionCurrent;
        }
    }
    private float valuePerDivision => (maxValue - minValue) / divisionAmount;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(propertyName))
        {
            CurrentValue = PlayerPrefs.GetFloat(propertyName);
        }
        else
        {
            DivisionCurrent = divisionCurrent;
        }
    }

    private void Start()
    {
        
    }

    public void DeletePlayerPref()
    {
        PlayerPrefs.DeleteKey(propertyName);
    }

    public void Increase()
    {
        if (DivisionCurrent >= divisionAmount)
        {
            DivisionCurrent = divisionAmount;
        }
        else
        {
            DivisionCurrent++;
        }
    }

    public void Decrease()
    {
        if (DivisionCurrent <= 0)
        {
            DivisionCurrent = 0;
        }
        else
        {
            DivisionCurrent--;
        }
    }
}
