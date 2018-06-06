namespace DSharp.Compiler.TestFramework
{
    public interface ITestContextFactory
    {
        ITestContext GetContext(string category, string testName);
    }
}