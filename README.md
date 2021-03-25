# CBVR Game
## Extending the main scene with more trial types
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



