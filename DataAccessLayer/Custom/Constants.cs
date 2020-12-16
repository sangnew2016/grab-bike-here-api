namespace DataAccessLayer.Custom
{
    public static class CONSTANTS
    {
        public const string CONNECTION_STRING_SQL = @"Data Source=SANG-PC\SQLEXPRESS;Initial Catalog=Test_Catalog;User Id=sa;Password=123456;";
        public const ProviderType PROVIDER_TYPE_SQL = ProviderType.SqlClient;

        public const string CONNECTION_STRING_MYSQL = @"server=127.0.0.1;uid=root;pwd=;database=grabbooking";
        public const ProviderType PROVIDER_TYPE_MYSQL = ProviderType.MySql;


        public const string KEY_ENCRYPT_DECRYPT_STRING = "b14ca5898a4e4133bbce2ea2315a1916";        // 32 byte
    }
}
