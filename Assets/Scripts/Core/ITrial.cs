using System.Collections;
using UXF;

namespace Core
{
        public interface ITrial
        {
                IEnumerator Perform();
                string GetTrialName();
                string[] GetColumnNames();
                UXFDataRow RetrieveTrialData();
        }
}