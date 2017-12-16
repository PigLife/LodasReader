using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Sql;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Lodas_Reader
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class ViewFestbezuegeMitarbeiter
        {
            public u_lod_psd_festbezuege inst_u_load_psd_festbezuege { get; set; }
            public int bnr { get { return this.inst_u_load_psd_festbezuege.bnr; } set { this.inst_u_load_psd_festbezuege.bnr = value; } } 
            public int mnr { get; set; }
            public int pnr_psd { get; set; }
            public Nullable<short> lohnart_nr_psd { get; set; }
            public int auto_lfd_nr { get; set; }
            public Nullable<decimal> betrag { get { return this.inst_u_load_psd_festbezuege.betrag_psd; } set { this.inst_u_load_psd_festbezuege.betrag_psd = value; } }
            public Nullable<short> kz_monatslohn_psd { get; set; }
            public Nullable<short> kuerzung_psd { get; set; }
            public string duevo_familienname_psd { get; set; }
            public string duevo_vorname_psd { get; set; }
        }

        String sqlPasswd = "";
        Entities con ;
        //List<u_lod_psd_festbezuege> TableData;
        //BindingList<u_lod_psd_festbezuege> myList;

        public MainWindow()
        {
            InitializeComponent();
            searchSQLInstance();
        }
       

        private String searchSQLInstance()
        {
            string SQLServerName = "";
             SqlDataSourceEnumerator instance = SqlDataSourceEnumerator.Instance;
             System.Data.DataTable table = instance.GetDataSources();
             
           // Retrieve the enumerator instance and then the data.
        
            foreach (System.Data.DataRow row in table.Rows)
            {
               if (row["InstanceName"].ToString()=="DATEV_DBENGINE") {
                 SQLServerName=row["ServerName"].ToString();
               }
            }
            return (SQLServerName);
        }

        private String CreateConnectionString(String SQLServerName)
        {
            String connectionString="metadata=res://*/Lodas.csdl|res://*/Lodas.ssdl|res://*/Lodas.msl;provider=System.Data.SqlClient;provider connection string=\";data source=";
            connectionString = connectionString+SQLServerName+"\\DATEV_DBENGINE;initial catalog=C:\\DATEV\\DATEN\\LODAS\\LODASDB;persist security info=True;user id=D_A_LODASDB;password=";
            connectionString = connectionString + sqlPasswd;
            connectionString = connectionString +";MultipleActiveResultSets=True;App=EntityFramework\";";
            //string providerName = "System.Data.SqlClient";
            //string databaseName = "C:\\DATEV\\DATEN\\LODAS\\LODASDB";

            // Initialize the connection string builder for the
            // underlying provider.
            //SqlConnectionStringBuilder sqlBuilder =
            //new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            //sqlBuilder.DataSource = SQLServerName;
            //sqlBuilder.InitialCatalog = databaseName;
            //sqlBuilder.IntegratedSecurity = true;
            //sqlBuilder.UserID = "D_A_LODASDB";
            //sqlBuilder.Password = "


            // Build the SqlConnection connection string.
            //string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            //EntityConnectionStringBuilder entityBuilder =
            //new EntityConnectionStringBuilder();

            //Set the provider name.
            //entityBuilder.Provider = providerName;

            // Set the provider-specific connection string.
            //entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            // entityBuilder.Metadata = @"res://*/AdventureWorksModel.csdl|
            //                        res://*/AdventureWorksModel.ssdl|
            //                        res://*/AdventureWorksModel.msl";
            //Console.WriteLine(entityBuilder.ToString()); 

            //MessageBox.Show(connectionString);

            return (connectionString);

        }
        private void Fill_Loehne()
        {
            try
            {
                con = new Entities(CreateConnectionString(tbxServer.Text));
                var query = (from festbezuege in con.u_lod_psd_festbezuege 
                    join mitarbeiter in con.u_lod_psd_mitarbeiter on festbezuege.mnr equals mitarbeiter.mnr
                 select new ViewFestbezuegeMitarbeiter
                {
                    inst_u_load_psd_festbezuege = festbezuege,
                    bnr = festbezuege.bnr,
                    mnr = festbezuege.mnr,
                    pnr_psd = festbezuege.pnr_psd,
                    lohnart_nr_psd = festbezuege.lohnart_nr_psd,
                    auto_lfd_nr = festbezuege.auto_lfd_nr,
                    betrag = festbezuege.betrag_psd,
                    kz_monatslohn_psd = festbezuege.kz_monatslohn_psd,
                    kuerzung_psd = festbezuege.kuerzung_psd,
                    duevo_familienname_psd = mitarbeiter.duevo_familienname_psd,
                    duevo_vorname_psd = mitarbeiter.duevo_vorname_psd
                });
                
                //TableData = con.u_lod_psd_festbezuege.ToList();
                //Loehne.ItemsSource = TableData;
                //myList = new BindingList<ViewFestbezuegeMitarbeiter>(query.ToList());
                Loehne.ItemsSource = query.ToList();
                Loehne.Columns[0].Visibility = Visibility.Hidden;
              }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void bntShow_onClick(object sender, RoutedEventArgs e)
        {
            Fill_Loehne();
        }

        private void tbxPassword_OnChange(object sender, TextChangedEventArgs e)
        {
            sqlPasswd = tbxPasswort.Text;
        }

        private void tbxServer_loaded(object sender, RoutedEventArgs e)
        {
            tbxServer.Text = searchSQLInstance();
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            if (con != null)
            {
                try
                {
                    con.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            Application.Current.Shutdown();
        }
    }
}
