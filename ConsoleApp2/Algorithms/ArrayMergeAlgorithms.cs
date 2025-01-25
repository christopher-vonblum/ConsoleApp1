namespace ConsoleApp2.Algorithms;

public static class ArrayMergeAlgorithms
{
    public static byte[] LinqConcat(params byte[][] arrays)
    {
        IEnumerable<byte> arr = arrays[0];

        foreach (var array in arrays.Skip(1))
        {
            arr = arr.Concat(array);
        }
        
        return arr.ToArray();
    }

    public static byte[] WriteByteByByteFromIterator(params byte[][] arrays)
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
    
    public static byte[] ConcatArrayUsingIteratorAndLinqToArray(params byte[][] arrays)
    {
        return ConcatArrysIterator(arrays).ToArray();
    }
    
    public static IEnumerable<byte> ConcatArrysIterator(params byte[][] arrays)
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

    public static byte[] ConcatArraysUsingListAndRanges(params byte[][] arrays)
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
    
    public static byte[] ConcatArraysUsingCopyTo(params byte[][] arrays)
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
    
    public static byte[] ConcatArraysByteByByte(params byte[][] arrays)
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
    
    public static unsafe byte[] ConcatArraysByteByByteUsingPointers(params byte[][] arrays)
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