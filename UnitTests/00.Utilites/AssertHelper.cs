namespace Microshaoft.UnitTests;

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

    static void processExpectedExceptionMessage
                                (
                                    Exception exception
                                    , Action<bool, string>
                                            onCheckIsTrueAssertProcessAction
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
            var failedMessage = $@"Expected exception of type {exception.GetType()} with a message of ""{expectedExceptionMessage}"" but expected exception with actual message of ""{exception.Message}"" was thrown instead.
The caught actual ""{exception.GetType()}"" as below:
{_beginSpliterLineOfMessageBlock}
{exception}
{_endSpliterLineOfMessageBlock}
";
            onCheckIsTrueAssertProcessAction
                (
                    string.Compare(expectedExceptionMessage, exception.Message, true) == 0
                    , failedMessage
                );

            
        }
    }

    public static void CaughtUnhandleException
                            <TAssert, TExpectedException>
                                    (
                                        Action
                                                action
                                        , string
                                                expectedExceptionMessage = null!
                                        , Action<TExpectedException>
                                                onCaughtExpectedExceptionWithMessageAssertProcessAction = null!
                                        , bool
                                                needDrillDownInnerExceptions = true
                                    )
                                where TExpectedException : Exception
    {
        Action<Exception> onCaughtExceptionWithMessageAssertProcessAction = null!;
        if (onCaughtExpectedExceptionWithMessageAssertProcessAction != null)
        {
            onCaughtExceptionWithMessageAssertProcessAction = (e) =>
            {
                onCaughtExpectedExceptionWithMessageAssertProcessAction((TExpectedException) e);
            };
        }

        CaughtUnhandleException
                        (
                              action
                            , typeof(TExpectedException)
                            , (failedMessage) =>
                            {
                                if
                                    (typeof(TAssert) == typeof(MAssert))
                                {
                                    MAssert.Fail(failedMessage);
                                }
                                else if
                                    (typeof(TAssert) == typeof(NAssert))
                                {
                                    NAssert.Fail(failedMessage);
                                }
                                else if
                                    (typeof(TAssert) == typeof(NAssert))
                                {
                                    xAssert.Fail(failedMessage);
                                }
                            }
                            , (found, failedMessage) =>
                            {
                                if
                                    (typeof(TAssert) == typeof(MAssert))
                                {
                                    MAssert.IsTrue(found, failedMessage);
                                }
                                else if
                                    (typeof(TAssert) == typeof(NAssert))
                                {
                                    NAssert.IsTrue(found, failedMessage);
                                }
                                else if
                                    (typeof(TAssert) == typeof(NAssert))
                                {
                                    xAssert.True(found, failedMessage);
                                }
                            }
                            , expectedExceptionMessage
                            , onCaughtExceptionWithMessageAssertProcessAction
                            , needDrillDownInnerExceptions
                        );
    }

    public static void CaughtUnhandleException
                                <TExpectedException>
                                        (
                                             this MAssert @this
                                            , Action action
                                            , string expectedExceptionMessage = null!
                                            , Action<TExpectedException>
                                                    onCaughtExpectedExceptionWithMessageAssertProcessAction = null!
                                            , bool needDrillDownInnerExceptions = true
                                        )
                                    where TExpectedException : Exception
    {
            CaughtUnhandleException
                        <MAssert, TExpectedException>
                            (
                                  action
                                , expectedExceptionMessage
                                , onCaughtExpectedExceptionWithMessageAssertProcessAction
                                , needDrillDownInnerExceptions
                            );
    }


    private static void CaughtUnhandleException
                                (
                                    Action action
                                    , Type expectedExceptionType
                                    , Action<string>
                                            onCatchNothingAssertProcessAction
                                    , Action<bool, string>
                                            onCheckIsTrueAssertProcessAction
                                    , string
                                            expectedExceptionMessage = null!
                                    , Action<Exception>
                                            onCaughtExpectedExceptionWithMessageAssertProcessAction = null!
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
            var failedMessage = $@"Expected exception of type ""{expectedExceptionType}"" but no exception was thrown.";
            onCatchNothingAssertProcessAction(failedMessage);
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
                processExpectedExceptionMessage(caughtExpectedException, onCheckIsTrueAssertProcessAction,expectedExceptionMessage);
            }

            var failedMessage = $@"Expected exception of type ""{expectedExceptionType}"" but actual type of ""{caughtException.GetType()}"" was thrown instead.
The caught actual ""{caughtException.GetType()}"" as below:
{_beginSpliterLineOfMessageBlock}
{caughtException}
{_endSpliterLineOfMessageBlock}
";
            onCheckIsTrueAssertProcessAction(foundCaughtExpectedExceptionWithMessage, failedMessage);
            onCaughtExpectedExceptionWithMessageAssertProcessAction?.Invoke(caughtExpectedExceptionWithMessage);
        }
    }
}
