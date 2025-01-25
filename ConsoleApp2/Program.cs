// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

class Program
{
    private const int arraysToMerge = 20;
    private static int innerArraySize = 300;
    
    public static void Main(string[] args)
    {
        byte[][] arrays = new[] {new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0],new byte[0]};
        
        BenchmarkAll(arrays);

        innerArraySize = 3000;
        
        BenchmarkAll(arrays);

        innerArraySize = 30000;
        
        BenchmarkAll(arrays);

        innerArraySize = 300000;
        
        BenchmarkAll(arrays);

        innerArraySize = 3000000;
        
        BenchmarkAll(arrays);
        
        innerArraySize = 30000000;
        
        BenchmarkAll(arrays);
    }

    private static void CreateDummyData(byte[][] arrays, Random rnd)
    {
        for (int i = 0; i < arraysToMerge; i++)
        {
            arrays[i] = new byte[innerArraySize];
            rnd.NextBytes(arrays[i]);
        }
    }

    private static void BenchmarkAll(byte[][] arrays)
    {
        // one set of dummy data for all
        CreateDummyData(arrays, Random.Shared);
        
        Dictionary<string, long> resultDictionary = new Dictionary<string, long>();
        
        // GOOO
        Console.WriteLine("inner array size:" + innerArraySize);
        
        Stopwatch sw = Stopwatch.StartNew();

        byte[] merged = MergeItLinq(arrays);
        
        sw.Stop();

        Console.WriteLine(merged.Length == (arraysToMerge * innerArraySize));
        
        Console.WriteLine("linq:" + sw.Elapsed.Ticks.ToString("N0"));
        
        resultDictionary.Add("linq", sw.Elapsed.Ticks);
        
        //___________________________________________________________________
        sw = Stopwatch.StartNew();

        merged = CallConcatArraysIterator(arrays);
        
        sw.Stop();
        
        Console.WriteLine(merged.Length == (arraysToMerge * innerArraySize));
        
        Console.WriteLine("iterator+linq:" + sw.Elapsed.Ticks.ToString("N0"));
        
        resultDictionary.Add("iterator+linq", sw.Elapsed.Ticks);
        
        //___________________________________________________________________
        sw = Stopwatch.StartNew();

        merged = FromIterator(arrays);
        
        sw.Stop();
        
        Console.WriteLine(merged.Length == (arraysToMerge * innerArraySize));
        
        Console.WriteLine("iterator:" + sw.Elapsed.Ticks.ToString("N0"));
        
        resultDictionary.Add("iterator", sw.Elapsed.Ticks);
        
        //___________________________________________________________________
        sw = Stopwatch.StartNew();

        merged = ConcatArraysUsingCopyTo(arrays);
        
        sw.Stop();
        
        Console.WriteLine(merged.Length == (arraysToMerge * innerArraySize));
        
        Console.WriteLine("copyto:" + sw.Elapsed.Ticks.ToString("N0"));
        
        resultDictionary.Add("copyto", sw.Elapsed.Ticks);
        
        //___________________________________________________________________
        sw = Stopwatch.StartNew();

        merged = ConcatArraysByteByByte(arrays);
        
        sw.Stop();
        
        Console.WriteLine(merged.Length == (arraysToMerge * innerArraySize));
        
        Console.WriteLine("bytebybyte:" + sw.Elapsed.Ticks.ToString("N0"));
        
        resultDictionary.Add("bytebybyte", sw.Elapsed.Ticks);
        
        //___________________________________________________________________
        sw = Stopwatch.StartNew();

        merged = ConcatArraysUsingListAndRanges(arrays);
        
        sw.Stop();
        
        Console.WriteLine(merged.Length == (arraysToMerge * innerArraySize));
        
        Console.WriteLine("listrangeadd:" + sw.Elapsed.Ticks.ToString("N0"));
        
        resultDictionary.Add("listrangeadd", sw.Elapsed.Ticks);
        
        //___________________________________________________________________
        sw = Stopwatch.StartNew();

        merged = ConcatArraysUnsafe(arrays);
        
        sw.Stop();
        
        Console.WriteLine(merged.Length == (arraysToMerge * innerArraySize));
        
        Console.WriteLine("unsafebytebybyte:" + sw.Elapsed.Ticks.ToString("N0"));
        
        resultDictionary.Add("unsafebytebybyte", sw.Elapsed.Ticks);
        
        int rank = 1;
        string ranks = string.Join('\n', resultDictionary.OrderBy(kv => kv.Value).Select(kv => rank + ": " + kv.Key));
        Console.WriteLine(ranks);
        
        Console.WriteLine("--------------------------------------------------------");
    }

    private static byte[] MergeItLinq(params byte[][] arrays)
    {
        IEnumerable<byte> arr = arrays[0];

        foreach (var array in arrays.Skip(1))
        {
            arr = arr.Concat(array);
        }
        
        return arr.ToArray();
    }

    private static byte[] FromIterator(params byte[][] arrays)
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
    
    private static byte[] CallConcatArraysIterator(params byte[][] arrays)
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
    
    private static unsafe byte[] ConcatArraysUnsafe(params byte[][] arrays)
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
