.NET Query Console
============

Simple .NET SQL console for query execution using different data providers.

##How to configure

.NET Query Console supports multiple connection strings for each data provider.
All settings are located in 'settings.xml' file. 

Here is an example of possible file structure:

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
  
##Features

###Multitabs
.NET Query Console allows you to write your queries in different tabs, save for later opening and, of course, open saved earlier. 

###Autocompletion
.NET Query Console provides default autocompetion.
But you can create your own content by creating a new file with ".autocomplete' extension and define 
it in configuration file:
  
  ```
  <provider name="SqlProvider" value="System.Data.SqlClient" autocomplete="Sql.autocomplete">
   ...
   </provider>
  ```
  
###Export to excel
You can easily export query result into .xls file - just click 'Query->Export to excel'.

##Dependencies
============
EPPlus is used for exporting query results to Excel.
https://github.com/pruiz/EPPlus.git
