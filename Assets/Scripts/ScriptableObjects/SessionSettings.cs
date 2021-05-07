using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UXF;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Settings/Session Settings")]
    public class SessionSettings : ScriptableObject
    {
        public float fixationDotRadius;
        public float focusDepth;

        public float headFixationDistanceErrorTolerance;
        public float headFixationAngleErrorTolerance;
        public string eyeTracker;
        
        public void LoadFromUxfJson()
        {
            var sessionSettingsDict = Session.instance.settings.GetDict("SessionSettings");
            
            fixationDotRadius = Convert.ToSingle(sessionSettingsDict["FixationDotRadiusDeg"]);
            focusDepth = Convert.ToSingle(sessionSettingsDict["FocusDepthMeters"]);

            headFixationDistanceErrorTolerance = Convert.ToSingle(sessionSettingsDict["HeadFixationDistanceErrorToleranceMeters"]);
            headFixationAngleErrorTolerance =
                Convert.ToSingle(sessionSettingsDict["HeadFixationAngleErrorToleranceDegrees"]);
            eyeTracker = Convert.ToString(sessionSettingsDict["EyeTracker"]);
        }

        public IDictionary<string, object> GetSettingsDict()
        {
            return new Dictionary<string, object>
            {
                {"FixationDotRadiusDeg", fixationDotRadius},
                {"FocusDepthMeters", focusDepth},
                {"HeadFixationDistanceErrorToleranceMeters", headFixationDistanceErrorTolerance},
                {"HeadFixationAngleErrorToleranceDegrees", headFixationAngleErrorTolerance},
                {"EyeTracker", eyeTracker}
            };
        }
    }
}
