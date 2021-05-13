# CBVR Game
# Extending the main scene with more trial types
To add a new trial type to the game, a GameObject with a script that implements the ITrial C# interface should be added as a child
to the *Trials* GameObject in the top level of the scene Hierarchy.

![trials_hierarchy](Doc/Images/trials_hierarchy.png)


To see an example of how to implement a simple trial, check the DummyTrial script in the Scripts/TrialTypes folder.
Additionally, the ITrial script in the Scripts/Core folder explains what each function in a class
that implements ITrial should do. It is important to have some understanding of what an Interface in C# is and what a Coroutine in Unity
does. The links to official Microsoft and Unity documentation do a good job of explaining both and are good places to get started.

Microsoft documentation on interfaces in C#: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/ \
Unity documentation on Coroutines: https://docs.unity3d.com/Manual/Coroutines.html

### Lifetime of a Trial
The number of trials that will be run that will run from each type can be controlled *(more information when implemented)*.
The trials are shuffled randomly after being placed in a list. Each trial runs through its Perform routine. This is where
all trial logic, interactivity, and visuals should be controlled. 

Consider each trial type its own 'Scene' that
can be instanced out and played independently of one another. In the case of this game, no two trials' Perform coroutines ever run
at the same time. The advantage of using Coroutines is for their ability to sequence functionality with delays easily and for reducing
the number of total calls to Update() significantly.

Before a trial's Perform() routine is called, the SessionManager will display a prompt that's provided by the ITrial
through its GetPromptText() method, then hide it just before the trial's Perform() routine is started.

After a trial's Perform() routine finishes, the SessionManager pulls the gathered data contained on the ITrial
through its GetColumnNames() and RetrieveTrialData() methods. The SessionManager maintains a table of each ITrial's
data. A new row is added to an ITrial's data table every time that trial ends. At the end of the game/session, all of the 
data is written out to separated files for each trial type that exists. The name of each file is determined by the implementation
of the GetTrialName() method on each trial type.

# Trial Creation Tutorial
***NOTE:*** To test out creating trials without VR, open the TutorialScene in the Assets/Scenes folder

## Basics
### 1. Create an empty GameObject in the Trials list
First, create an empty GameObject and give it a unique name. This will be the container for our Trial type.

![CreateGO](Doc/Gifs/CreateGO.gif)

### 2. Create a new script on the empty GameObject
Head over to the Inspector for your GameObject and create a new script that (optionally) has the same name as the GameObject you created. To open the script
in your IDE, right click the script and choose 'Edit Script' (This tutorial will use Visual Studio Community 2019, but any IDE or text editor will work).

![CreateScript](Doc/Gifs/CreateScript.gif)

### 3. Implement the ITrial interface in the new script

In your IDE, import the 'Core' module by typing 'using Core;' at the top of the file. To implement the ITrial interface, insert it next to the 'MonoBehavior' entry in the file. At this point if you're using an IDE,
it should complain by saying that members from ITrial must be implemented. Most IDE's will automatically do this for you (In Visual Studio, this is done through Intellisense).

![ImplementITrial](Doc/Gifs/ImplementITrial.gif)

Your file should look similar to the code below at this point:

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using UXF;

public class ColorTrial : MonoBehaviour, ITrial
{
    public IEnumerator Perform()
    {
        throw new System.NotImplementedException();
    }
    
    public int GetNumRepetitions()
    {
        throw new System.NotImplementedException();
    }
    
    public string[] GetColumnNames()
    {
        throw new System.NotImplementedException();
    }

    public string GetPromptText()
    {
        throw new System.NotImplementedException();
    }

    public IDictionary GetTemplateSettings()
    {
        throw new System.NotImplementedException();
    }

    public string GetTrialName()
    {
        throw new System.NotImplementedException();
    }

    public void LoadSettingsFromJson()
    {
        throw new System.NotImplementedException();
    }

    public UXFDataRow RetrieveTrialData()
    {
        throw new System.NotImplementedException();
    }
}
```

Next, we'll go through how each of these methods should be implemented after doing a bit of setup.

### 4. Setup trial assets
For this example trial, we want to create a sphere that changes its size and color at the start of the trial.
To achieve this, we'll want to create a sphere object and change its properties on the fly. Go back to the Unity editor and select
your trial object again. Right click the trial object and, using the context menu, create a new sphere.


Then, in the inspector for the sphere, remove the collider as we'll have no use for it in this tutorial, and deactivate the sphere GameObject so that we only
have it visible when we need it in the script (we'll go over how that's done in a bit).

![SphereEditor](Doc/Gifs/SphereInspector.gif)

Back in the ColorTrial script, add a SerializedField where the Sphere GameObject will eventually go.

![SerializedSphere](Doc/Gifs/SerializedSphere.gif)

### 5. Program the main trial logic
To keep things simple, the main flow of the trial (which is implemented in the Perform method) will simply be to update the current size/color of the sphere, present it
for a certain amount of time, then hide it. Delays can easily be added in the trial coroutine by yielding a "WaitForSeconds" object as demonstrated below.
The delay in seconds is implemented as a serialized field that can later be controlled through a JSON setting, as covered in a later part of the tutorial.

![WaitForSeconds](Doc/Gifs/WaitForSeconds.gif)

### 6. Define the number of trial repetitions
The number of repetitions a trial type will have can be determined in various simple or dynamic ways. This value is used by
the SessionManager to determine the total number of trial repetitions needed across all trial types. The most simple
way to implement this method would be to define an integer field of repetitions and simply return that value. For this tutorial, that's what
will be done.

![NumRepetitions](Doc/Gifs/NumRepetitions.gif)

### 7. Define the trial name and prompt text
The trial name is used when creating a new JSON settings template and for data output. It is important to make sure the value
returned from this function is always consistent. It is highly recommended to return the name of the GameObject the trial script is
attached to so that the trial name can easily be changed and is visible from the editor.

![TrialName](Doc/Gifs/TrialName.gif)

Next is to define the text that appears every time before this trial type is activated so that the participant has an idea of what action
they should perform. This can change based on certain parameters between trials or it can be the same every time. In this case, we'll hardcode
the text directly into the method.

![PromptText](Doc/Gifs/PromptText.gif)

### 8. Define methods related to reading/writing JSON settings
The GetTemplateSettings method is used when generating a JSON template file and the LoadSettingsFromJson method is meant to load from settings
that are based on the template. It is important to ensure that the settings that get written into the template can be loaded
properly by the LoadSettingsFromJson method. If you edit one of the methods, you like will need to modify the other as well.

For now, we'll parameterize the number of repetitions and the presentation delay. The default values that will be pulled into the
JSON template file will be the values assigned in the editor. In this Unity application, the JSON settings can be loaded through the UXF
JSON file chosen at the start of a session. When using the UXF JSON loader, the LoadSettingsFromJson method should almost always start with this
line:

```var settingsDictionary = Session.instance.settings.GetDict(GetTrialName());```

Note that it will also be necessary to cast or convert each setting to their correct data type.

![JsonMethods](Doc/Gifs/JsonMethods.gif)

### 9. Define methods related to data export
In the SessionManager, when a trial type finishes its Perform Coroutine, the SessionManager will then call
the trial types RetrieveTrialData method 