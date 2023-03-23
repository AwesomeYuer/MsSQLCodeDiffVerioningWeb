namespace UnitTests;

using Microshaoft;
using Newtonsoft.Json.Linq;
using System;
using System.IO.Compression;
using System.Reflection;
using UnitTests.Utilities;

[TestClass]
public class LinqHelperUnitTests
{
    [Fact]
    [TestCase]
    [TestMethod]
    public async Task TestMethod1Async()
    {
        var a = new int[]
                {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10
                };

        //await
        var results = a
                        .ForEachAsIEnumerable
                            (
                                (index, item) =>
                                {
                                    var needYield = item % 2 == 0;
                                    var needBreak = false;
                                    return
                                        (needYield, needBreak, (index, item));
                                }
                            );
        foreach (var item in results) 
        {
            MAssert.IsTrue(item.Result.item % 2 == 0);
            NAssert.IsTrue(item.Result.item % 2 == 0);
            xAssert.True(item.Result.item % 2 == 0);
            Console.WriteLine($"{item}");
        }


        var results2 = a
                        .ForEachAsIAsyncEnumerableSyncAsync
                            (
                                (index, item) =>
                                {
                                    var needYield = item > 5;
                                    var needBreak = false;
                                    return
                                        (needYield, needBreak, (index, item));
                                        
                                }
                            );

        var results3 = a
                        .ForEachAsIEnumerable
                            (
                                async (index, item) =>
                                {
                                    var needYield = item > 5;
                                    var needBreak = false;
                                    return
                                        await
                                            Task
                                                .FromResult
                                                    (
                                                        (needYield, needBreak, (index, item))
                                                    );

                                }
                            );

        var results4 = a
                    .ForEachAsIAsyncEnumerableAsyncAsync
                        (
                            async (index, item) =>
                            {
                                var needYield = item > 5;
                                var needBreak = false;
                                return
                                    await
                                        Task
                                            .FromResult
                                                (
                                                    (needYield, needBreak, (index, item))
                                                );

                            }
                        );

        foreach (var item in results)
        {
            MAssert.IsTrue(item.Result.item % 2 == 0);
            NAssert.IsTrue(item.Result.item % 2 == 0);
            xAssert.True(item.Result.item % 2 == 0);
            Console.WriteLine($"{item}");
        }

        await foreach (var item in results2)
        {
            
            MAssert.IsTrue(item.Result.item > 5);
            MAssert.IsTrue(item.Result.item > 5);
            MAssert.IsTrue(item.Result.item > 5);
            Console.WriteLine($"{item}");
        };

        foreach (var item in results3)
        {
            MAssert.IsTrue(item.Result.item > 5);
            MAssert.IsTrue(item.Result.item > 5);
            MAssert.IsTrue(item.Result.item > 5);
            Console.WriteLine($"{item}");
        };

        await foreach (var item in results4)
        {
            // ForEachAsIAsyncEnumerableAsyncAsync 取不到 字段名称 Source , Result
            MAssert.IsTrue(item.Item2.item > 5);
            MAssert.IsTrue(item.Item2.item > 5);
            MAssert.IsTrue(item.Item2.item > 5);
            Console.WriteLine($"{item}");
        };

    }
        


}