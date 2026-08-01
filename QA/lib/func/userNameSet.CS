namespace QA
{
    public class UserNameSet
    {
        public static string PromptForName()
        {
            while (true)
            {
                Console.WriteLine("이름은 특수문자를 포함할 수 없고 2 ~ 7자로 설정해야 됩니다.");
                Console.Write("이름을 입력하세요: ");

                string name = Console.ReadLine() ?? string.Empty;

                if (IsValidName(name))
                {
                    return name;
                }

                Console.WriteLine("입력한 이름이 조건에 맞지 않습니다. 다시 입력해주세요.");
            }
        }

        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            if (name.Length < 2 || name.Length > 7)
            {
                return false;
            }

            foreach (char c in name)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

