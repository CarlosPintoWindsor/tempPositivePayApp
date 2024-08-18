
namespace positive_pay_app.app.Sftp
{
    using Microsoft.Extensions.Logging;
    using Renci.SshNet;
    using Renci.SshNet.Sftp;

    /// <summary>
    /// Class containing sftp file operations
    /// </summary>
    public class SftpService : ISftpService
    {
        private readonly ILogger _logger;
        private readonly SftpConfig _config;
        private readonly SftpClient _client;

        public SftpService(ILogger logger, SftpConfig sftpConfig)
        {
            _logger = logger;
            _config = sftpConfig;
            _client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port, _config.UserName, _config.Password);
        }

        public SftpConfig Configs() => this._config;


        public void Connect()
        {

            _logger.LogDebug("Connecting to client {0} - {1} - {2}", _config.Host, _config.Port, _config.Directory);
            _client.Connect();
        }

        public void Disconnect()
        {
            _logger.LogDebug("Disconnecting client {0} - {1} - {2}", _config.Host, _config.Port, _config.Directory);
            this._client?.Disconnect();
        }

        /// <summary>
        /// Get list of all the files in remote directory
        /// </summary>
        /// <param name="remoteDirectory">is the directory path.</param>
        /// <returns>IEnumerable<SftpFile> , containing collection of files in the given directory</returns>

        public IEnumerable<ISftpFile> ListAllFiles(string relativePath = "")
        {
            try
            {
                return this._client.ListDirectory(_config.Directory + relativePath);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to list files under [{_config.Directory + relativePath}]");
                return Array.Empty<SftpFile>();
            }
        }

        /// <summary>
        /// Upload the files from a local directory to the remote directory
        /// </summary>
        /// <param name="localFilePath">is the source path of the file .</param>
        /// <param name="remoteFilePath">is the destination path of the file .</param>
        /// <returns>void</returns>
        public void UploadFile(FileStream sourceFileStream, string relativePath = "")
        {
            try
            {
                this._client.UploadFile(sourceFileStream, _config.Directory + relativePath);
                _logger.LogDebug($"Finished uploading file [{sourceFileStream.Name}] to [{_config.Directory + relativePath}]");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to upload file [{sourceFileStream.Name}] to [{_config.Directory + relativePath}]");
            }
        }

        /// <summary>
        /// download the files from remote location to local path
        /// </summary>
        /// <param name="localFilePath">is the source path of the file .</param>
        /// <param name="remoteFilePath">is the destination path of the file .</param>
        /// <returns>void</returns>
        public void DownloadFile(string localFilePath, string relativePath = "")
        {
            try
            {
                using var s = File.Create(localFilePath);
                this._client.DownloadFile(_config.Directory + relativePath, s);
                _logger.LogDebug($"Finished downloading file [{localFilePath}] from [{_config.Directory + relativePath}]");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to download file [{localFilePath}] from [{_config.Directory + relativePath}]");
            }
        }

        /// <summary>
        /// download the files from remote location to local path
        /// </summary>
        /// <param name="localFilePath">is the source path of the file .</param>
        /// <param name="remoteFilePath">is the destination path of the file .</param>
        /// <returns>void</returns>
        public SftpFileStream DownloadFileStream(string relativePath = "")
        {
            try
            {
                SftpFileStream file = this._client.OpenRead(_config.Directory + relativePath);
                _logger.LogDebug($"Finished downloading file from [{_config.Directory + relativePath}]");
                return file;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to download file from [{_config.Directory + relativePath}]");
            }
            return null;
        }

        /// <summary>
        /// delete the remote files 
        /// </summary>     
        /// <param name="remoteFilePath">is the path of the file to be deleted .</param>
        /// <returns>void</returns>
        public void DeleteFile(string relativePath = "")
        {
            try
            {
                this._client.DeleteFile(_config.Directory + relativePath);
                _logger.LogDebug($"File [{_config.Directory + relativePath}] deleted.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to delete file [{_config.Directory + relativePath}]");
            }
        }
    }
}
