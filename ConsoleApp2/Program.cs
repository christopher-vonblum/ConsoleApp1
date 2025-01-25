using System.Diagnostics;
using ConsoleApp2.Algorithms;

class Program
{
    private const int arraysToMerge = 20;
    
    public static void Main(string[] args)
    {
        Dictionary<string, List<long>> overallResultDictionary = new Dictionary<string, List<long>>();
        
        BenchmarkAll(30, overallResultDictionary);
        BenchmarkAll(300, overallResultDictionary);
        BenchmarkAll(3000, overallResultDictionary);
        BenchmarkAll(30000, overallResultDictionary);
        BenchmarkAll(300000, overallResultDictionary);
        BenchmarkAll(3000000, overallResultDictionary);
        BenchmarkAll(30000000, overallResultDictionary);
        
        int rank = 0;
        string ranks = string.Join('\n', 
            overallResultDictionary
                .OrderBy(kv => kv.Value.Average())
                .Select(kv => ++rank + ": " + kv.Key + " ~ " + kv.Value.Average()));
        
        Console.WriteLine(ranks);
    }
    
    private static void BenchmarkAll(int innerArraySize, Dictionary<string, List<long>> overallResults)
    {
        // one set of dummy data for all
        byte[][] arrays = CreateDummyData(innerArraySize, Random.Shared);
        
        Dictionary<string, long> resultDictionary = new Dictionary<string, long>();

        BenchmarkUnit(resultDictionary, arrays, innerArraySize, ArrayMergeAlgorithms.LinqConcat);
        BenchmarkUnit(resultDictionary, arrays, innerArraySize, ArrayMergeAlgorithms.ConcatArrayUsingIteratorAndLinqToArray);
        BenchmarkUnit(resultDictionary, arrays, innerArraySize, ArrayMergeAlgorithms.WriteByteByByteFromIterator);
        BenchmarkUnit(resultDictionary, arrays, innerArraySize, ArrayMergeAlgorithms.ConcatArraysUsingCopyTo);
        BenchmarkUnit(resultDictionary, arrays, innerArraySize, ArrayMergeAlgorithms.ConcatArraysByteByByte);
        BenchmarkUnit(resultDictionary, arrays, innerArraySize, ArrayMergeAlgorithms.ConcatArraysUsingListAndRanges);
        BenchmarkUnit(resultDictionary, arrays, innerArraySize, ArrayMergeAlgorithms.ConcatArraysByteByByteUsingPointers);
        
        Console.WriteLine("--------------------------------------------------------");
        
        resultDictionary.ToList().ForEach(res =>
        {
            if (overallResults.ContainsKey(res.Key))
                overallResults[res.Key].Add(res.Value);
            else
                (overallResults[res.Key] = new List<long>()).Add(res.Value);
        });
    }
    
    private static byte[][] CreateDummyData(int innerArraySize, Random rnd)
    {
        byte[][] arrays = new[] {new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0]};

        for (int i = 0; i < arraysToMerge; i++)
        {
            arrays[i] = new byte[innerArraySize];
            rnd.NextBytes(arrays[i]);
        }

        return arrays;
    }
    
    private static void BenchmarkUnit(Dictionary<string, long> resultDictionary, byte[][] dummyData, int innerArraySize, Func<byte[][], byte[]> testCase)
    {
        // GOOO
        Console.WriteLine("inner array size:" + innerArraySize);
        
        Stopwatch sw = Stopwatch.StartNew();

        byte[] merged = testCase(dummyData);
        
        sw.Stop();

        Console.WriteLine(merged.Length == (arraysToMerge * innerArraySize));
        
        Console.WriteLine($"{testCase.Method.Name}:" + sw.Elapsed.Ticks.ToString("N0"));
        
        resultDictionary.Add($"{testCase.Method.Name}", sw.Elapsed.Ticks);
    }
}