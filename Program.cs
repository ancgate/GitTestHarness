
namespace GitTestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            GitRepositoryManager git = new GitRepositoryManager("MaV","test", "http://@git.tempesttech.com:7990/scm/tmp/gittestharness.git", @"C:\test");
            git.CommitAllChanges("test");
            git.PushCommits("origin", "master");
        }
    }
}
