// See https://aka.ms/new-console-template for more information
public class Hirrr : IDisposable
{
    static Hirrr()
    {
        new Hirrr();
    }
    public void TestBruh()
    {
        
    }
    public void Dispose()
    {
        
    }
    public void Test()
    {
        // looks fine in il
        using (var hirr1 = new Hirrr())
        using (var hirr2 = new Hirrr())
        using (var hirr3 = new Hirrr())
        foreach (var s in new string[1] { "1!" })
        {
            hirr3.TestBruh();
        }

        //hirr3; geht nicht

        Console.WriteLine();
    }
}

