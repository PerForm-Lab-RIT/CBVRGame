﻿using System.Collections;
using Core;
using UnityEngine;
using UXF;

namespace TrialTypes
{
    public class DummyTrial : MonoBehaviour, ITrial
    {
        [SerializeField] private float waitTimeSeconds;
        [SerializeField] private string trialName;

        private int clickCount;
        private static readonly string[] ColumnNames = {"ClickCount"};

        public IEnumerator Perform()
        {
            clickCount = 0;
            gameObject.SetActive(true);
            yield return new WaitForSeconds(waitTimeSeconds);
            gameObject.SetActive(false);
        }

        public string GetTrialName()
        {
            return trialName;
        }

        public string[] GetColumnNames()
        {
            return ColumnNames;
        }

        public UXFDataRow RetrieveTrialData()
        {
            var row = new UXFDataRow {(ColumnNames[0], clickCount)};
            return row;
        }

        public void ButtonClicked()
        {
            clickCount += 1;
        }
    }
}