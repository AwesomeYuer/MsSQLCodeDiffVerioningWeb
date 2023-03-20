namespace Microshaoft
{
    using System;
    using System.Security;



    public static class ConsoleHelper
    {
        public enum CaptureOption
        { 
            CaptureStandardOnly
            , CaptureAll
            , CaptureEvery
        }


        public static (bool? HasErrors, string Message, string ErrorMessage)
                            CaptureOutput
                                (
                                    this TextWriter @this
                                    , Action onCaptureProcessAction// = null!
                                    , CaptureOption captureOption = CaptureOption.CaptureEvery
                                )
        {
            Stream allStream = null!;
            Stream errorStream = null!;
            StreamWriter errorStreamWriter = null!;// = new StreamWriter(stream);
            StreamWriter allStreamWriter = null!;// = new StreamWriter(stream);
            bool? hasErrors = null;
            var message = string.Empty;
            var errorMessage = string.Empty;
            // store original console output
            TextWriter originalConsoleOut = null!;
            TextWriter originalConsoleError = null!;
            
            try
            {
                if
                    (@this == Console.Error)
                {
                    errorStream ??= new MemoryStream();
                    errorStreamWriter ??= new StreamWriter(errorStream);
                    errorStreamWriter.AutoFlush = true;
                    
                    originalConsoleError = Console.Error;
                    Console.SetError(errorStreamWriter);
                }
                else if
                    (@this == Console.Out)
                {
                    allStream ??= new MemoryStream();
                    
                    allStreamWriter ??= new StreamWriter(allStream);
                    allStreamWriter.AutoFlush = true;

                    originalConsoleOut = Console.Out;
                    Console.SetOut(allStreamWriter);

                    if 
                        (captureOption == CaptureOption.CaptureAll)
                    {
                        originalConsoleError = Console.Error;
                        Console.SetError(allStreamWriter);
                    }
                    else if
                        (captureOption == CaptureOption.CaptureEvery)
                    {
                        errorStream ??= new MemoryStream();
                        errorStreamWriter ??= new StreamWriter(errorStream);
                        errorStreamWriter.AutoFlush = true;
                        
                        originalConsoleError = Console.Error;
                        Console.SetError(errorStreamWriter);
                    }
                }
                else
                { 
                    throw new InvalidOperationException($"Can't capture non Console output!");
                }
                onCaptureProcessAction();
                if (errorStreamWriter is not null)
                {
                    errorStreamWriter.Flush();
                    using var streamReader = new StreamReader(errorStream);
                    errorStream.Position = 0;
                    errorMessage = streamReader.ReadToEnd();
                    hasErrors = !errorMessage.IsNullOrEmptyOrWhiteSpaceOrZeroLength();
                }
                if (allStreamWriter is not null)
                {
                    allStreamWriter.Flush();
                    using var streamReader = new StreamReader(allStream);
                    allStream.Position = 0;
                    message = streamReader.ReadToEnd();
                }
            }
            finally
            {
                if (errorStream is not null)
                {
                    errorStream.Close();
                    errorStream.Dispose();
                    errorStream = null!;
                }
                if (allStream is not null)
                {
                    allStream.Close();
                    allStream.Dispose();
                    allStream = null!;
                }
                if (errorStreamWriter is not null)
                {
                    errorStreamWriter.Close();
                    errorStreamWriter.Dispose();
                    errorStreamWriter = null!;
                }
                if (allStreamWriter is not null)
                {
                    allStreamWriter.Close();
                    allStreamWriter.Dispose();
                    allStreamWriter = null!;
                }

                // restore original Console.Error
                if (originalConsoleError is not null)
                {
                    Console.SetError(originalConsoleError);
                }
                
                if (originalConsoleOut is not null)
                {
                    Console.SetOut(originalConsoleOut);
                }
            }
            return
                (hasErrors, message, errorMessage);
        }


        public static SecureString ReadMaskedSecureStringLine(string mask = "*")
        {
            var r = new SecureString();
            while (true)
            {
                var i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (i.Key == ConsoleKey.Backspace)
                {
                    if (r.Length > 0)
                    {
                        r.RemoveAt(r.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    r.AppendChar(i.KeyChar);
                    Console.Write(mask);
                }
            }
            r.MakeReadOnly();
            return r;
        }
        public static void HighlightWriteLine
                                (
                                    string message
                                    , ConsoleColor foregroundColor = ConsoleColor.Red
                                    , params object?[]? arg
                                )
        {
            var orininalForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(message, arg);
            Console.ForegroundColor = orininalForegroundColor;
        }
    }
}
