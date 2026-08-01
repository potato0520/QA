namespace QA
{
    class TestMainLocation
    {
        // 올바른 형태
        static void Main(string[] args)
        {
            Console.WriteLine("테스트를 시작합니다.");
            UserNameTest.RunTests();
            Console.WriteLine("테스트가 완료되었습니다.");
        }
    }
}