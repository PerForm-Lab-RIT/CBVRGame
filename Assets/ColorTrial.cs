using System.Collections;
using UnityEngine;
using Core;
using UXF;
using System.Collections.Generic;
using System;

public class ColorTrial : MonoBehaviour, ITrial
{
    [SerializeField] private GameObject sphere;
    [SerializeField] private float presentationDelay;
    [SerializeField] private int numRepetitions;

    /*
    public IDictionary GetTemplateSettings()
    {
        var settings = new Dictionary<string, object>
        {
            { "PresentationDelay", presentationDelay },
            { "NumRepetitions", numRepetitions }
        };
        return settings;
    }

    public void LoadSettingsFromJson()
    {
        var settingsDictionary = Session.instance.settings.GetDict(GetTrialName());
        presentationDelay = Convert.ToSingle(settingsDictionary["PresentationDelay"]);
        numRepetitions = Convert.ToInt32(settingsDictionary["NumRepetitions"]);
    }
    */

    public IDictionary GetTemplateSettings()
    {
        var settings = new Dictionary<string, object>
        {
            { "PresentationDelay", presentationDelay },
            { "NumRepetitions", numRepetitions }
        };
        return settings;
    }

    public void LoadSettingsFromJson()
    {
        var settingsDictionary = Session.instance.settings.GetDict(GetTrialName());
        presentationDelay = Convert.ToSingle(settingsDictionary["PresentationDelay"]);
        numRepetitions = Convert.ToInt32(settingsDictionary["NumRepetitions"]);
    }

    public string GetTrialName()
    {
        return gameObject.name;
    }

    public string GetPromptText()
    {
        return "Color Change!";
    }

    public int GetNumRepetitions()
    {
        return numRepetitions;
    }

    public IEnumerator Perform()
    {
        // Updating the properties of the sphere will eventually be done here
        sphere.SetActive(true);
        yield return new WaitForSeconds(presentationDelay);
        sphere.SetActive(false);
    }

    
    
    public string[] GetColumnNames()
    {
        throw new System.NotImplementedException();
    }

    

    public UXFDataRow RetrieveTrialData()
    {
        throw new System.NotImplementedException();
    }
}
