using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BYSResults;

namespace ExampleMudSystem
{
    internal class HelperMethods
    {
        // This method will accept a list of Error instances, likely
        //		retrieved from a Result instance returned from a DB
        //		System operation.  The List of Error will be turned
        //		into a List of String so that they can be read into the
        //		Error block on the Blazor page.
        public static List<string> GetErrorMessages(List<Error> errorList)
        {
            List<string> errorMessages = new List<string>();

            foreach (Error error in errorList)
            {
                errorMessages.Add(error.ToString());
            }

            return errorMessages;
        }

        // Though we have eliminated most of the error handling (try/catch)
        // 		from our CodeBehind and SystemLibrary methods, it is 
        //		impossible to eliminate them 100% as there will always
        //		be some exceptions that cannot be predicted.  This method
        //		will help us dig down to the most core Exception message,
        //		instead of having to try to figure out what is happening
        // 		from the higher level, more generic message.
        public static Exception GetInnerMostException(Exception ex)
        {
            // While the current exception has an InnerException
            while (ex.InnerException != null)
            {
                // Make the InnerException the current Exception
                ex = ex.InnerException;
            }

            // When we reach this point we have the InnerMostException.  Return
            //		it to the calling scope.
            return ex;
        }
    }
}
