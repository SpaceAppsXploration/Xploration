using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using xploration.Resources;

namespace xploration
{
    public partial class App : Application
    {


        public static PhoneApplicationFrame RootFrame { get; private set; }

        public App()
        {

            UnhandledException += Application_UnhandledException;

            InitializeComponent();

            InitializePhoneApplication();

            // Using thememanager 
            ThemeManager.ToDarkTheme();
            ThemeManager.SetAccentColor(AccentColor.Blue);
            ThemeManager.OverrideOptions = ThemeManagerOverrideOptions.SystemTrayColors;

            InitializeLanguage();

            if (Debugger.IsAttached)
            {
                Application.Current.Host.Settings.EnableFrameRateCounter = true;
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        {

        }

        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        #region Inizializzazione dell'applicazione Windows Phone

        // Evitare la doppia inizializzazione
        private bool phoneApplicationInitialized = false;

        // Non aggiungere altro codice a questo metodo
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Creare il fotogramma ma non impostarlo ancora come RootVisual; in questo modo
            // la schermata iniziale rimane attiva finché non viene completata la preparazione al rendering dell'applicazione.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Gestisce gli errori di navigazione
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Gestisce le richieste di reset per cancellare il backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Accertarsi che l'inizializzazione non venga ripetuta
            phoneApplicationInitialized = true;
        }

        // Non aggiungere altro codice a questo metodo
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Impostare l'elemento visivo radice per consentire il rendering dell'applicazione
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Rimuovere il gestore in quanto non più necessario
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // Se l'applicazione ha ricevuto una navigazione 'reset', occorre controllare
            // la navigazione successiva per verificare se lo stack della pagina può essere ripristinato
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Annullare la registrazione dell'evento in modo che non venga chiamato nuovamente
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Cancellare solo lo stack per le navigazioni "nuove" (in avanti) e "aggiorna"
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // Per coerenza della IU, cancellare l'intero stack della pagina
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // non eseguire alcuna azione
            }
        }

        #endregion

        private void InitializeLanguage()
        {
            try
            {
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}