namespace UnitTests;

using Microshaoft;

using static Microshaoft.ConsoleHelper;
using Microshaoft.UnitTests;

[TestClass]
public class ConsoleHelperUnitTests
{
    [TestCase(CaptureOutputOption.All            , "[standard{0}]", "(error{0})", "[standard1](error1)[standard2](error2)"    , ""                  , null)]
    [TestCase(CaptureOutputOption.StandardOnly   , "[standard{0}]", "(error{0})", "[standard1][standard2]"                    , ""                  , null)]
    [TestCase(CaptureOutputOption.Every          , "[standard{0}]", "(error{0})", "[standard1][standard2]"                    , "(error1)(error2)"  , true)]

    [DataRow(CaptureOutputOption.All            , "[standard{0}]", "(error{0})", "[standard1](error1)[standard2](error2)"    , ""                  , null)]
    [DataRow(CaptureOutputOption.StandardOnly   , "[standard{0}]", "(error{0})", "[standard1][standard2]"                    , ""                  , null)]
    [DataRow(CaptureOutputOption.Every          , "[standard{0}]", "(error{0})", "[standard1][standard2]"                    , "(error1)(error2)"  , true)]
    [TestMethod]

    [InlineData(CaptureOutputOption.All            , "[standard{0}]", "(error{0})", "[standard1](error1)[standard2](error2)"    , ""                  , null)]
    [InlineData(CaptureOutputOption.StandardOnly   , "[standard{0}]", "(error{0})", "[standard1][standard2]"                    , ""                  , null)]
    [InlineData(CaptureOutputOption.Every          , "[standard{0}]", "(error{0})", "[standard1][standard2]"                    , "(error1)(error2)"  , true)]
    [xTheory]
    public void Test_Console_Out
                            (
                                CaptureOutputOption captureOption
                                , string message
                                , string errorMessage
                                , string expectMessage
                                , string expectErrorMessage
                                , bool? expectHasErrors
                            )
    {
         (var hasErrors, message, errorMessage) =
                            Console
                                .Out
                                .CaptureOutput
                                    (
                                        () =>
                                        {
                                            Console.Write(message, 1);
                                            Console.Error.Write(errorMessage, 1);
                                            Console.Write(message, 2);
                                            Console.Error.Write(errorMessage, 2);
                                        }
                                        , captureOption
                                    );

        Console.WriteLine($"==========================");
        Console.WriteLine($"Capture: {nameof(hasErrors)}: {hasErrors}\r\n\t{nameof(message)}:\r\n\t[{message}],\r\n{nameof(errorMessage)}:\r\n\t[{errorMessage}]");
        Console.WriteLine($"==========================");

        MAssert.IsTrue(hasErrors == expectHasErrors);
        MAssert.IsTrue(message == expectMessage);
        MAssert.IsTrue(errorMessage == expectErrorMessage);

        NAssert.IsTrue(hasErrors == expectHasErrors);
        NAssert.IsTrue(message == expectMessage);
        NAssert.IsTrue(errorMessage == expectErrorMessage);

        xAssert.True(hasErrors == expectHasErrors);
        xAssert.True(message == expectMessage);
        xAssert.True(errorMessage == expectErrorMessage);

    }

    [TestCase(CaptureOutputOption.All            , "[standard{0}]", ""              , ""    , ""                    , false)]
    [TestCase(CaptureOutputOption.StandardOnly   , "[standard{0}]", ""              , ""    , ""                    , false)]
    [TestCase(CaptureOutputOption.Every          , "[standard{0}]", ""              , ""    , ""                    , false)]
    [TestCase(CaptureOutputOption.All            , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [TestCase(CaptureOutputOption.StandardOnly   , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [TestCase(CaptureOutputOption.Every          , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]

    [DataRow(CaptureOutputOption.All            , "[standard{0}]", ""              , ""    , ""                    , false)]
    [DataRow(CaptureOutputOption.StandardOnly   , "[standard{0}]", ""              , ""    , ""                    , false)]
    [DataRow(CaptureOutputOption.Every          , "[standard{0}]", ""              , ""    , ""                    , false)]
    [DataRow(CaptureOutputOption.All            , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [DataRow(CaptureOutputOption.StandardOnly   , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [DataRow(CaptureOutputOption.Every          , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [TestMethod]

    [InlineData(CaptureOutputOption.All            , "[standard{0}]", ""              , ""    , ""                    , false)]
    [InlineData(CaptureOutputOption.StandardOnly   , "[standard{0}]", ""              , ""    , ""                    , false)]
    [InlineData(CaptureOutputOption.Every          , "[standard{0}]", ""              , ""    , ""                    , false)]
    [InlineData(CaptureOutputOption.All            , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [InlineData(CaptureOutputOption.StandardOnly   , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [InlineData(CaptureOutputOption.Every          , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [xTheory]
    public void Test_Console_Error
                            (
                                CaptureOutputOption captureOption
                                , string message
                                , string errorMessage
                                , string expectMessage
                                , string expectErrorMessage
                                , bool? expectHasErrors
                            )
    {
        (var hasErrors, message, errorMessage) =
                            Console
                                .Error
                                .CaptureOutput
                                    (
                                        () =>
                                        {
                                            Console.Write(message, 1);
                                            Console.Error.Write(errorMessage, 1);
                                            Console.Write(message, 2);
                                            Console.Error.Write(errorMessage, 2);
                                        }
                                        , captureOption
                                    );

        Console.WriteLine($"==========================");
        Console.WriteLine($"Capture: {nameof(hasErrors)}: {hasErrors}\r\n\t{nameof(message)}:\r\n\t[{message}],\r\n{nameof(errorMessage)}:\r\n\t[{errorMessage}]");
        Console.WriteLine($"==========================");

        MAssert.IsTrue(hasErrors == expectHasErrors);
        MAssert.IsTrue(message == expectMessage);
        MAssert.IsTrue(errorMessage == expectErrorMessage);
    }

    [Fact]
    [TestCase]
    [TestMethod]
    public void Test_Non_Console_Caught_InvalidOperationException()
    {
        static void process()
        {
            using Stream stream = new MemoryStream();
            using TextWriter textWriter = new StreamWriter(stream);
            var (hasErrors, message, errorMessage) =
                        textWriter
                            .CaptureOutput
                                (
                                    () =>
                                    {
                                        textWriter.WriteLine("aaaaa");
                                    }
                                );
        }

        var expectedExceptionMessage = $"Can't capture non Console output!";

        MAssert
            .That
            .CaughtUnhandleException
                    <InvalidOperationException>
                        (
                            () =>
                            {
                                Task
                                    .Run
                                        (
                                            () =>
                                            {
                                                process();
                                            }
                                        )
                                    .Wait();
                            }
                            , expectedExceptionMessage
                            , (x) =>
                            { 
                                MAssert.IsInstanceOfType<InvalidOperationException>(x);
                            }
                        );

        AssertHelper
            .CaughtUnhandleException
                    <NAssert, InvalidOperationException>
                        (
                            () =>
                            {
                                process();
                            }
                            , expectedExceptionMessage
                            , (x) =>
                            {
                                NAssert.IsInstanceOf<InvalidOperationException>(x);
                            }
                        );

        AssertHelper
            .CaughtUnhandleException
                    <xAssert, InvalidOperationException>
                        (
                            () =>
                            {
                                process();
                            }
                            //, expectedExceptionMessage
                            //, (x) =>
                            //{
                            //    xAssert.IsType<InvalidOperationException>(x);
                            //}
                        );

        MAssert
            .ThrowsException
                <InvalidOperationException>
                    (
                        () =>
                        {
                            process();
                        }
                    );
        NAssert
            .Throws
                <InvalidOperationException>
                    (
                        () =>
                        {
                            process();
                        }
                    );

        xAssert
            .Throws
                <AggregateException>
                    (
                        () =>
                        {
                            Task
                                .Run
                                    (
                                        () =>
                                        {
                                            process();
                                        }
                                    )
                                .Wait();
                        }
                    );
    }
}