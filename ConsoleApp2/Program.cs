// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

class Program
{
    private const int arraysToMerge = 20;
    private static int innerArraySize = 300;
    
    public static void Main(string[] args)
    {
        byte[][] arrays = new[] {new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0]};
        
        Dictionary<string, List<long>> overallResultDictionary = new Dictionary<string, List<long>>();
        
        BenchmarkAll(arrays, overallResultDictionary);

        innerArraySize = 3000;
        
        BenchmarkAll(arrays, overallResultDictionary);

        innerArraySize = 30000;
        
        BenchmarkAll(arrays, overallResultDictionary);

        innerArraySize = 300000;
        
        BenchmarkAll(arrays, overallResultDictionary);

        innerArraySize = 3000000;
        
        BenchmarkAll(arrays, overallResultDictionary);
        
        innerArraySize = 30000000;
        
        BenchmarkAll(arrays, overallResultDictionary);
        
        int rank = 0;
        string ranks = string.Join('\n', 
            overallResultDictionary
                .OrderBy(kv => kv.Value.Average())
                .Select(kv => ++rank + ": " + kv.Key + " => " + kv.Value.Average()));
        
        Console.WriteLine(ranks);
    }

    private static void CreateDummyData(byte[][] arrays, Random rnd)
    {
        for (int i = 0; i < arraysToMerge; i++)
        {
            arrays[i] = new byte[innerArraySize];
            rnd.NextBytes(arrays[i]);
        }
    }

    private static void BenchmarkUnit(Dictionary<string, long> resultDictionary, byte[][] dummyData, Func<byte[][], byte[]> testCase)
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

    private static void BenchmarkAll(byte[][] arrays, Dictionary<string, List<long>> overallResults)
    {
        // one set of dummy data for all
        CreateDummyData(arrays, Random.Shared);
        
        Dictionary<string, long> resultDictionary = new Dictionary<string, long>();
        
        // GOOO

        BenchmarkUnit(resultDictionary, arrays, LinqConcat);
        BenchmarkUnit(resultDictionary, arrays, ConcatArrayUsingIteratorAndLinqToArray);
        BenchmarkUnit(resultDictionary, arrays, WriteByteByByteFromIterator);
        BenchmarkUnit(resultDictionary, arrays, ConcatArraysUsingCopyTo);
        BenchmarkUnit(resultDictionary, arrays, ConcatArraysByteByByte);
        BenchmarkUnit(resultDictionary, arrays, ConcatArraysUsingListAndRanges);
        BenchmarkUnit(resultDictionary, arrays, ConcatArraysByteByByteUsingPointers);
        
        Console.WriteLine("--------------------------------------------------------");
        
        resultDictionary.ToList().ForEach(res =>
        {
            if (overallResults.ContainsKey(res.Key))
                overallResults[res.Key].Add(res.Value);
            else
                (overallResults[res.Key] = new List<long>()).Add(res.Value);
        });
    }

    private static byte[] LinqConcat(params byte[][] arrays)
    {
        IEnumerable<byte> arr = arrays[0];

        foreach (var array in arrays.Skip(1))
        {
            arr = arr.Concat(array);
        }
        
        return arr.ToArray();
    }

    private static byte[] WriteByteByByteFromIterator(params byte[][] arrays)
    {
        var it = ConcatArrysIterator(arrays);
        
        int overallBytes = arrays.Sum(a => a.Length);

        byte[] merged = new byte[overallBytes];

        int byteCounter = 0;
        foreach (byte b in it)
        {
            merged[byteCounter] = b;
            byteCounter++;
        }
        
        return merged;
    }
    
    private static byte[] ConcatArrayUsingIteratorAndLinqToArray(params byte[][] arrays)
    {
        return ConcatArrysIterator(arrays).ToArray();
    }
    
    private static IEnumerable<byte> ConcatArrysIterator(params byte[][] arrays)
    {
        for (int i = 0; i < arrays.Length; i++)
        {
            byte[] current = arrays[i];
            for (int j = 0; j < current.Length; j++)
            {
                yield return current[j];
            }
        }
    }

    private static byte[] ConcatArraysUsingListAndRanges(params byte[][] arrays)
    {
        int overallBytes = arrays.Sum(a => a.Length);

        List<byte> merged = new List<byte>(overallBytes);

        long byteCounter = 0;
        
        for (int i = 0; i < arrays.Length; i++)
        {
            byte[] current = arrays[i];
            
            merged.AddRange(current);
        }

        return merged.ToArray();
    }
    
    private static byte[] ConcatArraysUsingCopyTo(params byte[][] arrays)
    {
        int overallBytes = arrays.Sum(a => a.Length);

        byte[] merged = new byte[overallBytes];

        long byteCounter = 0;
        
        for (int i = 0; i < arrays.Length; i++)
        {
            byte[] current = arrays[i];
            
            current.CopyTo(merged, byteCounter);

            byteCounter += current.Length;
        }

        return merged;
    }
    
    private static byte[] ConcatArraysByteByByte(params byte[][] arrays)
    {
        int overallBytes = arrays.Sum(a => a.Length);

        byte[] merged = new byte[overallBytes];

        long byteCounter = 0;
        
        for (int i = 0; i < arrays.Length; i++)
        {
            byte[] current = arrays[i];
            for (int j = 0; j < current.Length; j++)
            {
                merged[byteCounter] = current[j];
                byteCounter++;
            }
        }

        return merged;
    }
    
    private static unsafe byte[] ConcatArraysByteByByteUsingPointers(params byte[][] arrays)
    {
        int overallBytes = arrays.Sum(a => a.Length);

        byte[] merged = new byte[overallBytes];
        
        fixed (byte* pointerToTargetByte = &merged[0])
        {        
            for (int i = 0; i < arrays.Length; i++)
            {
                int currentLength = arrays[i].Length;
                fixed (byte* pointerToCurrentSourceByte = &(arrays[i][0]))
                {
                    for (int j = 0; j < currentLength; j++)
                    {
                        *pointerToTargetByte = *pointerToCurrentSourceByte;
                        (*pointerToTargetByte)++;
                        (*pointerToCurrentSourceByte)++;
                    }                    
                }
            }   
        }    
        
        return merged;
    }
}
