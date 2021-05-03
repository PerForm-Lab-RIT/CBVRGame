using System.Collections;
using UXF;

namespace Core
{
        /*
         * Interface: ITrial
         * A generic interface for creating modularized trials
         */
        public interface ITrial
        {
                /*
                 * Function: Perform
                 * The actual trial to perform. This is a coroutine. It should be implemented such that when this
                 * coroutine returns, the trial is over.
                 */
                IEnumerator Perform();
                
                /*
                 * Function: RetrieveTrialData
                 * Retrieve the collected data from a finished trial, as a UXFDataRow.
                 * This method is meant to be called directly after the Perform coroutine finishes. Calling it before
                 * then will likely lead to undefined behavior.
                 */
                UXFDataRow RetrieveTrialData();
                
                /*
                 * Function: GetTrialName
                 * Retrieve the name of this trial, most importantly used for data saving purposes.
                 */
                string GetTrialName();
                
                /*
                 * Function: GetColumnNames
                 * Retrieve the names of the data columns for this trial.
                 */
                string[] GetColumnNames();

                /*
                 * Function: GetPromptText
                 * Get the desired prompt text for display by a SessionManager.
                 */
                string GetPromptText();

                /*
                 * Function: GetTemplateSettings
                 * Get settings for writing out to the TEMPLATE json file
                 */
                IDictionary GetTemplateSettings();

                /*
                 * Function: LoadSettingsFromJson
                 * Loads relevant settings from the JSON file loaded into UXF. When implementing, the dictionary
                 * to pull from should always be from the string provided in GetTrialName.
                 *
                 * Example (Almost all implementations should start with this line):
                 * var settingsDictionary = Session.instance.settings.GetDict(GetTrialName());
                 */
                void LoadSettingsFromJson();
                
                /*
                 * Function: GetNumRepetitions
                 * Retrieves the number of total times this trial type should repeat
                 */
                int GetNumRepetitions();
        }
}