namespace ResponsibilityHub.Patterns;
public sealed class Singleton
{
    private static Singleton instance;
    private Singleton() { }

    public static Singleton Get()
    {
        if(instance == null)
            instance = new Singleton();
        return instance;
    }
}