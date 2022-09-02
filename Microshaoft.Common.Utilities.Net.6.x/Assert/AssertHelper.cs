//namespace ChinaCPPMigTransLayer.MSTest.UnitTests
//{
//    public static class AssertHelper
//    {
//        public static void Throws<TExpectedException>
//                                        (
//                                            Action action
//                                            , string expectedMessage = null!
//                                        )
//                                                where TExpectedException : Exception
//        {
//            void process(Exception exception)
//            {
//                if (!string.IsNullOrEmpty(expectedMessage))
//                {
//                    Assert
//                        .AreEqual
//                            (
//                                expectedMessage
//                                , exception.Message
//                                , $"Expected exception with a message of '{expectedMessage}' but exception with message of '{exception.Message}' was thrown instead."
//                            );
//                }
//            }
//            try
//            {
//                action();
//            }
//            catch (TExpectedException expectedException)
//            {
//                Assert
//                    .IsTrue
//                        (
//                            expectedException.GetType() == typeof(TExpectedException)
//                        );
//                process(expectedException);
//                return;
//            }
//            catch (Exception exception)
//            {
//                Assert
//                    .Fail
//                        (
//                            $"Expected exception of type {typeof(TExpectedException)} but type of {exception.GetType()} was thrown instead."
//                        );
//                process(exception);
//                return;
//            }
//            Assert
//                .Fail
//                    (
//                        $"Expected exception of type {typeof(TExpectedException)} but no exception was thrown."
//                    );
//        }
//    }
//}
