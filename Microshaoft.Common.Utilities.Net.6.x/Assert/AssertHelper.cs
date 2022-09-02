//namespace Microshaoft
//{
//    public static class AssertHelper
//    {
//        public static void Throws<TExpectedException>
//                                        (Action action)
//                                                where TExpectedException : Exception
//        {
//            try
//            {
//                action();
//            }
//            catch (Exception ex)
//            {
//                Assert
//                    .IsTrue
//                        (
//                            ex.GetType() == typeof(TExpectedException)
//                            , $"Expected exception of type {typeof(TExpectedException)} but type of {ex.GetType()} was thrown instead.");
//                return;
//            }
//            Assert.Fail($"Expected exception of type {typeof(TExpectedException)} but no exception was thrown.");
//        }
//        public static void Throws<TExpectedException>
//                                        (Action action, string expectedMessage)
//                                                where TExpectedException : Exception
//        {
//            try
//            {
//                action();
//            }
//            catch (Exception ex)
//            {
//                Assert
//                    .IsTrue
//                        (
//                            ex.GetType() == typeof(TExpectedException)
//                            , $"Expected exception of type {typeof(TExpectedException)} but type of {ex.GetType()} was thrown instead."
//                        );
//                Assert
//                    .AreEqual
//                        (
//                            expectedMessage
//                            , ex.Message
//                            , $"Expected exception with a message of '{expectedMessage}' but exception with message of '{ex.Message}' was thrown instead."
//                        );
//                return;
//            }
//            Assert.Fail($"Expected exception of type{typeof(TExpectedException)} but no exception was thrown.");
//        }
//    }
//}
