namespace ResponsibilityHub.Utility;
public static class PeselValidator
{
    public static bool IsValidPesel(string pesel)
    {
        if (pesel.Length != 11 || !pesel.All(char.IsDigit))
        {
            return false; // PESEL musi mieć 11 cyfr i składać się wyłącznie z cyfr
        }

        int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int sum = 0;

        for (int i = 0; i < 10; i++)
        {
            sum += weights[i] * (pesel[i] - '0');
        }

        int controlDigit = (10 - (sum % 10)) % 10;

        if (controlDigit != (pesel[10] - '0'))
        {
            return false; // Cyfra kontrolna nie pasuje
        }

        return true; // PESEL jest poprawny
    }
}
