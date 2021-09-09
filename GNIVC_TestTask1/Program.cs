using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace GNIVC_TestTask1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("Введите ссылку на организацию в Azure DevOps");
            Console.WriteLine("Пример: https://dev.azure.com/MyOrg");
            string uriString = Console.ReadLine();
            Uri orgUrl = new Uri(uriString);

            Console.WriteLine("");
            Console.WriteLine("Введите PAT (ключ)");
            string personalAccessToken = Console.ReadLine();

            Console.WriteLine("");
            Console.WriteLine("Введите название проекта");
            string projectName = Console.ReadLine();

            // Create a connection
            VssConnection connection = new VssConnection(orgUrl, new VssBasicCredential(string.Empty, personalAccessToken));

            Console.WriteLine("");
            Console.WriteLine("Содержимое TFS-репозитория");
            GetItemsList(connection, projectName);
            Console.WriteLine("ChangeSet'ы репозитория");
            GetChangesetsList(connection, projectName);
        }

        static public IEnumerable<TfvcItem> GetItemsList(VssConnection connection, string projectName)
        {
            TfvcHttpClient tfvcClient = connection.GetClient<TfvcHttpClient>();
            List<TfvcItem> items = tfvcClient.GetItemsAsync(scopePath: $"$/{projectName}/", recursionLevel: VersionControlRecursionType.OneLevel).Result;

            foreach (TfvcItem item in items)
            {
                Console.WriteLine("{0}    {1}    {2}", item.ChangeDate, item.IsFolder ? " <FILE> " : "<FOLDER>", item.Path);
            }
            Console.WriteLine("");

            return items;
        }

        static public IEnumerable<TfvcChangesetRef> GetChangesetsList(VssConnection connection, string projectName)
        {
            TfvcHttpClient tfvcClient = connection.GetClient<TfvcHttpClient>();
            IEnumerable<TfvcChangesetRef> changesets = tfvcClient.GetChangesetsAsync(project: projectName).Result;

            foreach (TfvcChangesetRef changeset in changesets)
            {
                IEnumerable<TfvcChange> changes = tfvcClient.GetChangesetChangesAsync(id: changeset.ChangesetId).Result;
                Console.WriteLine("Номер изменения: {0}", changeset.ChangesetId);
                Console.WriteLine("Автор: {0}", changeset.Author.DisplayName);
                Console.WriteLine("Email автора: {0}", changeset.Author.UniqueName);
                Console.WriteLine("Время изменения: {0}", changeset.CreatedDate);
                if(changeset.Comment != null)
                {
                    Console.WriteLine("Комментарий: {0}", changeset.Comment);
                }
                foreach (TfvcChange change in changes)
                {
                    Console.WriteLine("Изменения в файле: {0}", change.Item.Path);
                    Console.WriteLine("Хэш: {0}", change.Item.HashValue);
                }
                Console.WriteLine("");
            }

            return changesets;
        }
    }
}
