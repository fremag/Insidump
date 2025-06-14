namespace DumpDummy;

class Zog
{
    public DateTime datetime = new DateTime(2023, 1, 1, 12, 34, 56);
    public DateOnly dateOnly = new DateOnly(2023, 1, 1);
    public TimeOnly timeOnly = new TimeOnly(23, 34, 45);
}

class Program
{
    public enum Status {None, Ok, Error, Waiting, Running}
    static void Main(string[] args)
    {
        var zog = new Zog();
        TestEnum(Status.Error, Status.Ok);
        Toto(zog.dateOnly, zog.datetime, zog.timeOnly);
    }

    private static void Toto(params object[] values)
    {
        Console.WriteLine(string.Join(',', values));
    }

    private static void TestEnum(Status status1, Status status2)
    {
        if(status1 == status2)
        {
            throw new Exception("Error");
        }

        var nom1 = status1.ToString();
        Console.WriteLine(nom1);
        Console.ReadLine();
    }
}