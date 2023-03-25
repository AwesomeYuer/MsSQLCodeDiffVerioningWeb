using System.Runtime.CompilerServices;

namespace Microshaoft;

public static class CallerHelper
{
    public static (string CallerMemberName, string CallerFilePath, int CallerLineNumber) 
                    GetCallerInfo
                                (
                                    [CallerMemberName]
                                    string callerMemberName = null!
                                    , [CallerFilePath]
                                    string callerFilePath = null!
                                    , [CallerLineNumber]
                                    int callerLineNumber = -1
                                )
    {

        return (callerMemberName, callerFilePath, callerLineNumber);
    }
}
