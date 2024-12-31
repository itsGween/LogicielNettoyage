using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FirstProjectUdemy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string version = "1.0.0";

        public DirectoryInfo winTemp;                         //Dossier contenant les fichiers temporaire windows
        public DirectoryInfo appTemp;                         //Fichier temporaire de certaine application

        public MainWindow()
        {
            InitializeComponent();

            winTemp = new DirectoryInfo(@"c:\Windows\Temp");
            appTemp = new DirectoryInfo(System.IO.Path.GetTempPath());
            CheckActu();
            GetDate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir">Dossier a traiter</param>
        /// <returns>Retourne la taille total a nettoyer</returns>
        public long DirSize(DirectoryInfo dir) //long est comme int mais permet de stocke des chiffre plus grand
        {
            return dir.GetFiles().Sum(fi => fi.Length) + dir.GetDirectories().Sum(di => DirSize(di));
        }

        //Vider un dossier
        public void ClearTempData(DirectoryInfo di)
        {
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                    Console.WriteLine(file.FullName);
                }
                catch (Exception ex) 
                {
                    continue;
                }
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                    Console.WriteLine(dir.FullName);
                }

                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        

        /// <summary>
        /// Verifier les actus
        /// </summary>
        public void CheckActu()
        {
            string url = "http://localhost/siteweb/actuel.txt";
            //Pour recuperer quelque chose via l'url
            using (WebClient Client = new WebClient())
            {
                //recuperer des donnees via une url en ligne (on utilise get)
                string actu = Client.DownloadString(url);
                if (actu != String.Empty)
                {
                    actuTxt.Content = actu;
                    actuTxt.Visibility = Visibility.Visible;
                    bandeau.Visibility = Visibility.Visible;

                }
            }
        }

        /// <summary>
        /// Verifie s'il ya des mise a jour
        /// </summary>
        public void CheckVersion()
        {
            string url = "http://localhost/siteweb/version.txt";
            
            using (WebClient Client = new WebClient())
            {
                
                string v = Client.DownloadString(url);
                if (version != v)
                {
                    MessageBox.Show("Une mise a jour est dispo !", "Mise a jour", MessageBoxButton.OK, MessageBoxImage.Information);
                    

                }
                else
                {
                    MessageBox.Show("Votre logiciel est a jour !", "Mise a jour", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
        }

        private void Button_MAJ_Click(object sender, RoutedEventArgs e)
        {
            CheckVersion();        }

        private void Button_Histo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TODO: Creer page Historique", "Historique", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Ouverture site web
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Button_Web_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://www.collegelacite.ca")
                {
                    UseShellExecute = true
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
           
        }

        private void Button_Analyser_Click(object sender, RoutedEventArgs e)
        {
            AnalyseFolders();
        }

        /// <summary>
        /// Analyser les dossiers
        /// </summary>

        public void AnalyseFolders()
        {
            Console.WriteLine("Debut de l'analyse... ");
            long totalSize = 0;

            totalSize += DirSize(winTemp) / 1000000;
            totalSize += DirSize(appTemp) / 1000000;

            espace.Content = totalSize + "Mb";
            titre.Content = "Analyse effectue !";
            date.Content = DateTime.Today;

        }

        /// <summary>
        /// Nettoyage du PC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Button_Nettoyer_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Nettoyage en cours...");
            btnClean.Content = "NETTOYAGE EN COURS";

            Clipboard.Clear();

            try
            {
                ClearTempData(winTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur: " + ex.Message);
            }

            try
            {
                ClearTempData(appTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur: " + ex.Message);
            }

            btnClean.Content = "NETTOYAGE TERMINE";
            titre.Content = "Nettoyage effectue !";
            espace.Content = "0 Mb";

        }

        public void SaveDate()
        {
            string date = DateTime.Today.ToString();
            File.WriteAllText("date.txt", date);
        }

        public void GetDate()
        {
            string dateFichier = File.ReadAllText("date.txt");
            //S'il ya deja une date 
            if (dateFichier != String.Empty)
            {
                date.Content = dateFichier;
            }
        }
    }
}