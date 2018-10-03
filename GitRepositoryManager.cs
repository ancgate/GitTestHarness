using System;
using System.Linq;
using System.IO;
using LibGit2Sharp;
using System.Collections.Generic;
using LibGit2Sharp.Handlers;

namespace GitTestHarness
{
   public class GitRepositoryManager
    {
        private readonly string _repoSource;
        private readonly UsernamePasswordCredentials _credentials;
        public readonly DirectoryInfo _localFolder;
        private readonly LibGit2Sharp.Repository repo;
        public readonly string _folderPath;
        

        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepositoryManager" /> class.
        /// </summary>
        /// <param name="username">The Git credentials username.</param>
        /// <param name="password">The Git credentials password.</param>
        /// <param name="gitRepoUrl">The Git repo URL.</param>
        /// <param name="localFolder">The full path to local folder.</param>
        public GitRepositoryManager(string username, string password, string gitRepoUrl, string localFolder)
        {
            _folderPath = localFolder; 
            var folder = new DirectoryInfo(localFolder);
            _localFolder = folder;

            _credentials = new UsernamePasswordCredentials
            {
                Username = username,
                Password = password
            };

            _repoSource = gitRepoUrl;
        }
        public bool isValid()
        {
            return Repository.IsValid(_folderPath);

        }
        public void Clone()
        {
            var co = new CloneOptions();
            co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = "MaV", Password = "test" };
            Repository.Clone(_repoSource, _folderPath, co);
        }
        public void Pull()
        {
            Signature committer = new Signature("MaV", "ma@tempesttech.com", DateTime.Now);

            using (var repo = new Repository(_folderPath))
            {
                var head = repo.Branches.Single(branch => branch.FriendlyName == "master");
                var checkoutOptions = new CheckoutOptions();
                checkoutOptions.CheckoutModifiers = CheckoutModifiers.Force;
                repo.Checkout(head, checkoutOptions);

                PullOptions options = new PullOptions();
                options.FetchOptions = new FetchOptions();
                options.FetchOptions.CredentialsProvider = new CredentialsHandler(
                (url, usernameFromUrl, types) =>
                  new UsernamePasswordCredentials()
                  {
                      Username = "MaV",
                      Password = "test"
                  });
                Commands.Pull(repo, new LibGit2Sharp.Signature("translator", "example@demo.com",
                               new DateTimeOffset(DateTime.Now)), options);

            }
        }

        /// <summary>
        /// Commits all changes.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.Exception"></exception>
        public void CommitAllChanges(string message)
        {

            using (var repo = new Repository(_localFolder.FullName))
            {
                var files = _localFolder.GetFiles("*", SearchOption.AllDirectories).Select(f => f.FullName);

                LibGit2Sharp.Commands.Stage(repo,files);

                Signature committer = new Signature("MaV", "ma@tempesttech.com", DateTime.Now);
                Signature author = committer;
                try
                {
                    repo.Commit(message, committer, author);
                }
                catch
                {
                    Console.WriteLine("There is nothing to be committed.");
                }
            }
        }

        /// <summary>
        /// Pushes all commits.
        /// </summary>
        /// <param name="remoteName">Name of the remote server.</param>
        /// <param name="branchName">Name of the remote branch.</param>
        /// <exception cref="System.Exception"></exception>
        public void PushCommits(string remoteName, string branchName)
        {
            using (var repo = new Repository(_localFolder.FullName))
            {
                var remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == remoteName);
                if (remote == null)
                {
                    repo.Network.Remotes.Add(remoteName, _repoSource);
                    remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == remoteName);
                }

                var options = new PushOptions
                {
                    CredentialsProvider = (url, usernameFromUrl, types) => _credentials
                };
                repo.Network.Push(repo.Branches["master"], options);
                
            }
        }
    }
}
