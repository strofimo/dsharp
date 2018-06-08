namespace DSharp.Compiler.TestFramework.Context
{
    public interface ITestContextFactory
    {
        ITestContext GetContext(string category, string testName);
    }
}