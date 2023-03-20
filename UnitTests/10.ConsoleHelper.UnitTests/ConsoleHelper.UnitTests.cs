namespace UnitTests;

using Microshaoft;

using static Microshaoft.ConsoleHelper;
using Microshaoft.UnitTests.MsTest;

[TestClass]
public class ConsoleHelperUnitTests
{
    [TestCase(CaptureOption.CaptureAll            , "[standard{0}]", "(error{0})", "[standard1](error1)[standard2](error2)"    , ""                  , null)]
    [TestCase(CaptureOption.CaptureStandardOnly   , "[standard{0}]", "(error{0})", "[standard1][standard2]"                    , ""                  , null)]
    [TestCase(CaptureOption.CaptureEvery          , "[standard{0}]", "(error{0})", "[standard1][standard2]"                    , "(error1)(error2)"  , true)]

    [DataRow(CaptureOption.CaptureAll            , "[standard{0}]", "(error{0})", "[standard1](error1)[standard2](error2)"    , ""                  , null)]
    [DataRow(CaptureOption.CaptureStandardOnly   , "[standard{0}]", "(error{0})", "[standard1][standard2]"                    , ""                  , null)]
    [DataRow(CaptureOption.CaptureEvery          , "[standard{0}]", "(error{0})", "[standard1][standard2]"                    , "(error1)(error2)"  , true)]
    [TestMethod]

    [InlineData(CaptureOption.CaptureAll            , "[standard{0}]", "(error{0})", "[standard1](error1)[standard2](error2)"    , ""                  , null)]
    [InlineData(CaptureOption.CaptureStandardOnly   , "[standard{0}]", "(error{0})", "[standard1][standard2]"                    , ""                  , null)]
    [InlineData(CaptureOption.CaptureEvery          , "[standard{0}]", "(error{0})", "[standard1][standard2]"                    , "(error1)(error2)"  , true)]
    [xTheory]
    public void Test_Console_Out
                            (
                                CaptureOption captureOption
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

    [TestCase(CaptureOption.CaptureAll            , "[standard{0}]", ""              , ""    , ""                    , false)]
    [TestCase(CaptureOption.CaptureStandardOnly   , "[standard{0}]", ""              , ""    , ""                    , false)]
    [TestCase(CaptureOption.CaptureEvery          , "[standard{0}]", ""              , ""    , ""                    , false)]
    [TestCase(CaptureOption.CaptureAll            , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [TestCase(CaptureOption.CaptureStandardOnly   , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [TestCase(CaptureOption.CaptureEvery          , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]

    [DataRow(CaptureOption.CaptureAll            , "[standard{0}]", ""              , ""    , ""                    , false)]
    [DataRow(CaptureOption.CaptureStandardOnly   , "[standard{0}]", ""              , ""    , ""                    , false)]
    [DataRow(CaptureOption.CaptureEvery          , "[standard{0}]", ""              , ""    , ""                    , false)]
    [DataRow(CaptureOption.CaptureAll            , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [DataRow(CaptureOption.CaptureStandardOnly   , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [DataRow(CaptureOption.CaptureEvery          , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [TestMethod]

    [InlineData(CaptureOption.CaptureAll            , "[standard{0}]", ""              , ""    , ""                    , false)]
    [InlineData(CaptureOption.CaptureStandardOnly   , "[standard{0}]", ""              , ""    , ""                    , false)]
    [InlineData(CaptureOption.CaptureEvery          , "[standard{0}]", ""              , ""    , ""                    , false)]
    [InlineData(CaptureOption.CaptureAll            , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [InlineData(CaptureOption.CaptureStandardOnly   , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [InlineData(CaptureOption.CaptureEvery          , "[standard{0}]", "(error{0})"    , ""    , "(error1)(error2)"    , true)]
    [xTheory]
    public void Test_Console_Error
                            (
                                CaptureOption captureOption
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

        MAssert
                .That
                .CaughtUnhandleException
                        <InvalidOperationException>
                            (
                                () =>
                                { 
                                    process();
                                }
                                , $"Can't capture non Console output!"
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
                <InvalidOperationException>
                    (
                        () =>
                        {
                            process();
                        }
                    );
    }
}