namespace QueryConsole
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    using QueryConsole.API.Models;

    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constants

        private const string ConfigPath = "settings.xml";

        #endregion

        #region Members

        private AppConfiguration _appConfiguration;

        #endregion

        #region Properties

        public AppConfiguration AppConfiguration
        {
            get
            {
                return this._appConfiguration;
            }
        }

        #endregion

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Продотвращаем обработку не перехваченного исключения
            e.Handled = true;

            this.ExceptionsHandler(e.Exception);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this._appConfiguration = new AppConfiguration(ConfigPath);
        }

        private void ExceptionsHandler(Exception exception)
        {
            MessageBox.Show(exception.InnerException != null ? exception.InnerException.Message : exception.Message);

            if (!(exception is ArgumentNullException))
            {
                Current.Shutdown();   
            }
        }
    }
}
