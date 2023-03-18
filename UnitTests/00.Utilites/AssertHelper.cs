#define MsTestUnitTests
#if MsTestUnitTests
namespace Microshaoft.UnitTests.MsTest;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

public static class AssertHelper
{
    private const string _beginSpliterLineOfMessageBlock = "<<<<<<<<<<<<<<<<<<<<<<<<<<";
    private const string _endSpliterLineOfMessageBlock = ">>>>>>>>>>>>>>>>>>>>>>>>>>";

    private static Exception drillDownInnerException
                                        (
                                            this Exception @this
                                            , Func<Exception, bool> onProcessFunc = null!
                                            , bool needDrillDownInnerException = true
                                        )
    {
        var result = @this;
        while (result != null)
        {
            if (onProcessFunc != null)
            {
                bool @return = onProcessFunc(result);
                if (@return)
                {
                    break;
                }
            }
            if (!needDrillDownInnerException)
            {
                break;
            }
            result = result.InnerException;
        }
        return result!;

    }

    private static void processExpectedExceptionMessage
                                (
                                    Exception exception
                                    , string expectedExceptionMessage = null!
                                )
    {
        if
            (
                !string
                    .IsNullOrEmpty
                            (
                                expectedExceptionMessage
                            )
            )
        {
            Assert
                .IsTrue
                    (
                        string.Compare(expectedExceptionMessage, exception.Message, true) == 0
                        , $@"Expected exception of type {exception.GetType()} with a message of ""{expectedExceptionMessage}"" but expected exception with actual message of ""{exception.Message}"" was thrown instead.
The caught actual ""{exception.GetType()}"" as below:
{_beginSpliterLineOfMessageBlock}
{exception}
{_endSpliterLineOfMessageBlock}
"
                    );
        }
    }

    public static void CaughtUnhandleException
                            <TExpectedException>
                                (
                                    this Assert @this
                                    , Action action
                                    , string expectedExceptionMessage = null!
                                    , Action<TExpectedException> onProcessAction = null!
                                    , bool needDrillDownInnerExceptions = true
                                )
                                    where TExpectedException : Exception
    {
        Action<Exception> actionExceptionProcess = null!;
        if (onProcessAction != null)
        {
            actionExceptionProcess = (e) =>
            {
                onProcessAction((TExpectedException)e);
            };
        }
        Throws
            (
                @this
                , action
                , typeof(TExpectedException)
                , expectedExceptionMessage
                , actionExceptionProcess
                , needDrillDownInnerExceptions
            );
    }

    public static void Throws
                            (
                                this Assert @this
                                , Action action
                                , Type expectedExceptionType
                                , string expectedExceptionMessage = null!
                                , Action<Exception> onProcessAction = null!
                                , bool needDrillDownInnerExceptions = true
                            )
    {
        Exception caughtException = null!;
        Exception caughtExpectedException = null!;
        Exception caughtExpectedExceptionWithMessage = null!;
        var foundCaughtExpectedExceptionWithMessage = false;

        void drillDownInnerExceptionProcess(Exception e)
        {
            caughtExpectedExceptionWithMessage = e
                                                    .drillDownInnerException
                                                        (
                                                            (ee) =>
                                                            {
                                                                if
                                                                    (
                                                                        expectedExceptionType
                                                                                        .IsAssignableFrom
                                                                                                (ee.GetType())
                                                                    )
                                                                {
                                                                    caughtExpectedException = ee;
                                                                    foundCaughtExpectedExceptionWithMessage =
                                                                        (
                                                                            string
                                                                                .Compare
                                                                                    (
                                                                                        expectedExceptionMessage
                                                                                        , ee.Message
                                                                                        , true
                                                                                    ) == 0
                                                                            ||
                                                                            string.IsNullOrEmpty(expectedExceptionMessage)
                                                                        );
                                                                }
                                                                return foundCaughtExpectedExceptionWithMessage;
                                                            }
                                                            , needDrillDownInnerExceptions
                                                        );
        }

        try
        {
            action();
        }
        catch (AggregateException aggregateException)
        {
            caughtException = aggregateException;
            if
                (
                    expectedExceptionType
                                    .IsAssignableFrom
                                            (caughtException.GetType())
                )
            {
                if
                    (
                        string
                            .Compare
                                (
                                    expectedExceptionMessage
                                    , caughtException.Message
                                    , true
                                ) == 0
                        ||
                        string.IsNullOrEmpty(expectedExceptionMessage)
                    )
                {
                    caughtExpectedExceptionWithMessage = caughtException;
                    caughtExpectedException = caughtException;
                    foundCaughtExpectedExceptionWithMessage = true;
                }
            }
            if (!foundCaughtExpectedExceptionWithMessage)
            {
                if (needDrillDownInnerExceptions)
                {
                    if (caughtException.InnerException != null)
                    {
                        drillDownInnerExceptionProcess(caughtException.InnerException);
                        if (!foundCaughtExpectedExceptionWithMessage)
                        {
                            caughtExpectedExceptionWithMessage = null!;
                        }
                    }
                }
            }
            if (!foundCaughtExpectedExceptionWithMessage)
            {
                var innerExceptions = aggregateException.Flatten().InnerExceptions;
                if (needDrillDownInnerExceptions)
                {
                    foreach (var e in innerExceptions)
                    {
                        drillDownInnerExceptionProcess(e);
                        if (foundCaughtExpectedExceptionWithMessage)
                        {
                            break;
                        }
                    }
                }
                if (!foundCaughtExpectedExceptionWithMessage)
                {
                    caughtExpectedExceptionWithMessage = null!;
                }
            }
        }
        //catch (TExpectedException expectedException)
        //{
        //    caughtException = expectedException;
        //    caughtExpectedException = expectedException;
        //    drillDownInnerExceptionProcess(caughtExpectedException);
        //    if (!foundCaughtExpectedException)
        //    {
        //        caughtExpectedException = null!;
        //    }
        //}
        catch (Exception exception)
        {
            caughtException = exception;
            caughtExpectedException = exception;
            drillDownInnerExceptionProcess(caughtExpectedException);
            if (!foundCaughtExpectedExceptionWithMessage)
            {
                caughtExpectedExceptionWithMessage = null!;
            }
        }

        if (caughtException == null)
        {
            Assert
                .Fail
                    (
                        $@"Expected exception of type ""{expectedExceptionType}"" but no exception was thrown."
                    );
        }
        else
        {
            if
                (
                    !foundCaughtExpectedExceptionWithMessage
                    &&
                    caughtExpectedException != null
                )
            {
                processExpectedExceptionMessage(caughtExpectedException, expectedExceptionMessage);
            }

            Assert
                .IsTrue
                    (
                        foundCaughtExpectedExceptionWithMessage
                        , $@"Expected exception of type ""{expectedExceptionType}"" but actual type of ""{caughtException.GetType()}"" was thrown instead.
The caught actual ""{caughtException.GetType()}"" as below:
{_beginSpliterLineOfMessageBlock}
{caughtException}
{_endSpliterLineOfMessageBlock}
"
                    );
            onProcessAction?.Invoke(caughtExpectedExceptionWithMessage);
        }
    }
}
#endif