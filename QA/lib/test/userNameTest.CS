namespace QA
{
    public class UserNameTest
    {
        List<String> userNameTestCode = new List<String>
        {
            "장현용",
            "왕",
            "장현용기사완전강함ㅋㅋ",
            "@장현용@",
            "#$!@#@#$@"
        };

        public void RunTests()
        {
            foreach (var testName in userNameTestCode)
            {
                bool isValid = UserNameSet.IsValidName(testName);
                Console.WriteLine($"Testing '{testName}': {(isValid ? "Valid" : "Invalid")}");
            }
        }
        
    }
}