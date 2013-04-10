.NET Query Console
============

Query Console

Simple .NET SQL console can use different dataproviders.

##How to configure

.NET Query Console supports multiple connection strings for each data provider.
All settings are located in 'settings.xml'. 

Here is a file structure:

  ```
<settings>
  <providers>
    <provider name="SqlProvider" value="System.Data.SqlClient">
      <connectionString name="connection1" value="Data Source=MySqlDB1; Initial Catalog=catalog1; User Id=userId1;Password=password1;" />
      <connectionString name="connection2" value="Data Source=MySqlDB2; Initial Catalog=catalog2; User Id=userId2;Password=password2;" />
    </provider>
    <provider name="OracleProvider" value="System.Data.OracleClient">
      <connectionString name="connection1" value="Data Source=MyOracleDB1;User Id=myUsername1;Password=myPassword1;Integrated Security=no;" />
      <connectionString name="connection2" value="Data Source=MyOracleDB2;User Id=myUsername2;Password=myPassword2;Integrated Security=no;" />
    </provider>
  </providers>
</settings>
  ```

##Dependencies
============
EPPlus
https://github.com/pruiz/EPPlus.git
