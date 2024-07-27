namespace ResponsibilityHub.Extensions;
public static class ChainExtension
{
    public static Tout Then<Tin, Tout>(this Tin input, Func<Tin, Tout> next)
    {
        return next(input);
    }

    public static void Then<Tin>(this Tin input, Action<Tin> next)
    {
        next(input);
    }
}
