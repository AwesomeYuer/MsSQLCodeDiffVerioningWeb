#define NonMsTestUnitTests
#if MsTestUnitTests
namespace ChinaCPPMigTransLayer.MSTest.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    public static class AssertHelper
    {
        private static void processExpectedExceptionMessage(Exception exception, string expectedExceptionMessage = null!)
        {
            if (!string.IsNullOrEmpty(expectedExceptionMessage))
            {
                Assert
                    .AreEqual
                        (
                            expectedExceptionMessage
                            , exception.Message
                            , $@"Expected exception with a message of ""{expectedExceptionMessage}"" but exception with message of ""{exception.Message}"" was thrown instead."
                        );
            }
        }
        public static void Throws<TExpectedException>
                                        (
                                            Action action
                                            , string expectedExceptionMessage = null!
                                        )
                                                where TExpectedException : Exception
        {

            try
            {
                action();
            }
            catch (TExpectedException expectedException)
            {
                Assert
                    .IsTrue
                        (
                            expectedException.GetType() == typeof(TExpectedException)
                        );
                processExpectedExceptionMessage(expectedException, expectedExceptionMessage);
                return;
            }
            catch (Exception exception)
            {
                Assert
                    .Fail
                        (
                            $@"Expected exception of type ""{typeof(TExpectedException)}"" but type of ""{exception.GetType()}"" was thrown instead."
                        );
                processExpectedExceptionMessage(exception, expectedExceptionMessage);
                return;
            }
            Assert
                .Fail
                    (
                        $@"Expected exception of type ""{typeof(TExpectedException)}"" but no exception was thrown."
                    );
        }

        public static void Throws
                                (
                                    Type expectedExceptionType
                                    , Action action
                                    , string expectedExceptionMessage = null!
                                )

        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                Assert
                    .IsTrue
                        (
                            exception.GetType() == expectedExceptionType
                            , $@"Expected exception of type ""{expectedExceptionType}"" but type of ""{exception.GetType()}"" was thrown instead."
                        );
                processExpectedExceptionMessage(exception, expectedExceptionMessage);
                return;
            }
            Assert
                .Fail
                    (
                        $@"Expected exception of type ""{expectedExceptionType}"" but no exception was thrown."
                    );
        }
    }
}
#endif