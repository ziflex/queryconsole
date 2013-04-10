// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Threading;

    using QueryConsole.API.Models;

    /// <summary>
    /// App handler
    /// </summary>
    public partial class App
    {
        #region Constants

        /// <summary>
        /// The config path.
        /// </summary>
        private const string ConfigPath = "settings.xml";

        #endregion

        #region Public Properties

        /// <summary>
        /// Applications configuration
        /// </summary>
        public IConfiguration AppConfiguration { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// catch unhandled exceptions and send them to app handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Продотвращаем обработку не перехваченного исключения
            e.Handled = true;

            this.ExceptionsHandler(e.Exception);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.AppConfiguration = new AppConfiguration();
            this.AppConfiguration.Load(new FileInfo(ConfigPath));
        }

        /// <summary>
        /// handle all exceptions
        /// </summary>
        /// <param name="exception"></param>
        private void ExceptionsHandler(Exception exception)
        {
            MessageBox.Show(exception.InnerException != null ? exception.InnerException.Message : exception.Message);

            // TODO: create more flexible exceptions handling
            if (!(exception is ArgumentNullException))
            {
                Current.Shutdown();
            }
        }

        #endregion
    }
}