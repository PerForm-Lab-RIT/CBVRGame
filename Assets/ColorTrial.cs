using System.Collections;
using UnityEngine;
using Core;
using UXF;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class ColorTrial : MonoBehaviour, ITrial
{
    [SerializeField] private GameObject sphere;
    [SerializeField] private Color[] colors;
    [SerializeField] private float presentationDelay;
    [SerializeField] private int numRepetitions;
    private static readonly string[] ColumnNames = { "PresentationTime" };

    public IEnumerator Perform()
    {
        sphere.GetComponent<MeshRenderer>().material.color = colors[Random.Range(0, colors.Length)];
        sphere.SetActive(true);
        yield return new WaitForSeconds(presentationDelay);
        sphere.SetActive(false);
    }







    public string[] GetColumnNames()
    {
        return ColumnNames;
    }

    public UXFDataRow RetrieveTrialData()
    {
        return new UXFDataRow { (ColumnNames[0], presentationDelay) };
    }

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
}
