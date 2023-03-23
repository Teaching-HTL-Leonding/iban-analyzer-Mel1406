#region var
const string FALSE_NUMBER_ARGUMENTS = "To {0} arguments.";
const string FALSE_CODE = "{0} must not contain letters.";
const string WRONG_LENGTH = "{0} has wrong length, must contain {1} digits.";
const string WRONG_COMMAND = """Invalid command, must be "build" or "analyze""";
const string INVALID = "Invalid {0}";
#endregion

#region program
if (args.Length == 0)
{
    Console.WriteLine("Please enter arguments!");
}
else
{
    switch (args[0])
    {
        case "build":
            if (CheckIfEnoughArguments(args.Length, 3))
            {
                Console.WriteLine(BuildIBAN(args[1], args[2], "NO", "7"));
            }
            break;
        case "analyze":
            if (CheckIfEnoughArguments(args.Length, 2))
            {
                Console.WriteLine(AnalyzeIBAN(args[1]));
            }
            break;
        default:
            Console.WriteLine(WRONG_COMMAND);
            break;
    }
}
#endregion

#region methods
bool CheckIfEnoughArguments(int numberArguments, int argumentsRequired)
{
    if (numberArguments < argumentsRequired)
    {
        Console.WriteLine(FALSE_NUMBER_ARGUMENTS, "few");
        return false;
    }
    else if (numberArguments > argumentsRequired)
    {
        Console.WriteLine(FALSE_NUMBER_ARGUMENTS, "many");
        return false;
    }
    return true;
}

bool GetErrorMessage(string bankCode,
                     string accountNumber,
                     string countryCode,
                     string checksum,
                     string nationDigit,
                     out string errorMessage)
{
    bool result = true;
    if (!int.TryParse(bankCode, out int bankCodeInteger))
    {
        errorMessage = string.Format(FALSE_CODE, "Bank code");
    }
    else if (!int.TryParse(accountNumber, out int accountNumberInteger))
    {
        errorMessage = string.Format(FALSE_CODE, "Account number");
    }
    else if (bankCode.Length != 4)
    {
        errorMessage = string.Format(WRONG_LENGTH, "Bank code", 4);
    }
    else if (accountNumber.Length != 6)
    {
        errorMessage = string.Format(WRONG_LENGTH, "Account number", 6);
    }
    else if (countryCode != "NO" && countryCode != "")
    {
        errorMessage = string.Format(INVALID, "country code");
    }
    else if (nationDigit != "7" && nationDigit != "")
    {
        errorMessage = string.Format(INVALID, "nation digit");
    }
    else if (checksum != CalculateChecksum(bankCode, accountNumber, nationDigit, countryCode, out long rest) && checksum != "" && rest != 1)
    {
        errorMessage = string.Format(INVALID, "checksum");
    }
    else
    { result = false; }
    errorMessage = "";
    return result;
}

string BuildIBAN(string bankCode, string accountNumber, string countryCode, string nationDigit)
{
    string checksum = CalculateChecksum(bankCode, accountNumber, nationDigit, countryCode, out long rest);
    return GetErrorMessage(bankCode,
                           accountNumber,
                           countryCode,
                           checksum,
                           nationDigit,
                           out string errorMessage) ? errorMessage : $"{countryCode}{checksum}{bankCode}{accountNumber}{nationDigit}";
}
string AnalyzeIBAN(string IBAN)
{
    if (IBAN.Length != 15)
    {
        return string.Format(INVALID, "length of IBAN");
    }
    string countryCode = IBAN.Substring(0, 2);
    string checksum = IBAN.Substring(2, 2);
    string bankCode = IBAN.Substring(4, 4);
    string accountNumber = IBAN.Substring(8, 6);
    string nationDigit = IBAN.Substring(14);

    return GetErrorMessage(bankCode,
                           accountNumber,
                           countryCode,
                           checksum,
                           nationDigit,
                           out string errorMessage) ? errorMessage : $"Bank code: {bankCode}\n\rAccount number: {accountNumber}";
}
string CalculateChecksum(string bankCode, string accountNumber, string nationDigit, string countryCode, out long check)
{
    int firstLetterDigit = countryCode[0] - 'A' + 10;
    int secondLetterDigit = countryCode[1] - 'A' + 10;
    string countryCodeDigit = firstLetterDigit.ToString() + secondLetterDigit.ToString();
    long checksum = long.Parse(bankCode) % 97;
    checksum = (checksum * GetMultiplier(6) + long.Parse(accountNumber)) % 97;
    checksum = (checksum * GetMultiplier(1) + long.Parse(nationDigit)) % 97;
    checksum = (checksum * GetMultiplier(4) + long.Parse(countryCodeDigit)) % 97;
    check = checksum;
    checksum = checksum * GetMultiplier(2) % 97;
    long returnValue = 98 - checksum;
    check = (checksum * GetMultiplier(2) + returnValue) % 97;
    return returnValue.ToString();
}
long GetMultiplier(int exponent)
{
    long result = 1;
    for (int i = 0; i < exponent; i++)
    {
        result *= 10;
    }
    return result;
}
#endregion