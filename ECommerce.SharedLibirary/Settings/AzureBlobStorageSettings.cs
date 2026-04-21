namespace ECommerce.SharedLibirary.Settings
{
    /// <summary>
    /// Strongly-typed config for Azure Blob Storage.
    /// Values are read from the "AzureBlobStorage" section of appsettings.json
    /// (or environment variables: AzureBlobStorage__ConnectionString etc.).
    /// </summary>
    public class AzureBlobStorageSettings
    {
        /// <summary>Full Azure Storage connection string from the portal (Access keys > key1).</summary>
        public string ConnectionString { get; set; } = null!;

        /// <summary>Name of the blob container that holds product images (e.g. "product-images").</summary>
        public string ContainerName { get; set; } = "product-images";
    }
}
