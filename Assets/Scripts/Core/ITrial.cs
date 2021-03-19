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
                 * Get the desired prompt text for display.
                 */
                string GetPromptText();
        }
}