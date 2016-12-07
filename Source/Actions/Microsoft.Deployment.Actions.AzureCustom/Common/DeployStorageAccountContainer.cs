﻿using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class DeployStorageAccountContainer : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {

            var azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
            var subscription = request.DataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString();
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation")["Name"].ToString();
            var deploymentName = request.DataStore.GetValue("DeploymentName");

            var storageAccountName = request.DataStore.GetValue("StorageAccountName");
            var storageAccountKey= request.DataStore.GetValue("StorageAccountKey");
            var containerName = request.DataStore.GetValue("StorageAccountContainer");

            //var connectionString = $"DefaultEndpoinsProtocol=[https;AccountName={storageAccountName};AccountKey={storageAccountKey}";
            var accountCredentials = new StorageCredentials(storageAccountName, storageAccountKey);
            CloudStorageAccount account = new CloudStorageAccount(accountCredentials, false);
            CloudBlobClient blobClient = account.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            await container.CreateIfNotExistsAsync();

            // Change Container from private to public
            BlobContainerPermissions permissions = container.GetPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            container.SetPermissions(permissions);


            return new ActionResponse(ActionStatus.Success);
        }
    }
}
