namespace ChatRoom.Infrastructure.Database.AppSettings;

public class CosmoDbSettings
{
    public string AccountEndpointUrl { get; set; }
    /// <summary>
    ///     Key - The primary key for the Azure DocumentDB account.
    /// </summary>
    public string PrimaryKey { get; set; }
    /// <summary>
    ///     Database name
    /// </summary>
    public string DatabaseName { get; set; }
    /// <summary>
    /// Connection string
    /// </summary>
    public string AccountConnectionString { get; set; }
    /// <summary>
    ///  Stored Procedure UpdateMessage
    /// </summary>
    public string ProcUpdateMessage { get; set; }

    /// <summary>
    ///     Stored Procedure UpdateUserAvatar
    /// </summary>
    public string ProcUpdateUserAvatar { get; set; }
    /// <summary>
    ///  User defined Function ConvertDate
    /// </summary>
    public string UdfConvertDate { get; set; }

}