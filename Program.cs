
namespace GitTestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            GitRepositoryManager git = new GitRepositoryManager("UserName","Name", "Repo", @"localRepo");
            git.CommitAllChanges("Name");
            git.PushCommits("origin", "master");
        }
    }
}
