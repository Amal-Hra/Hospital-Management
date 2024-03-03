using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.PortableExecutable;
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

namespace Medecins
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection conDB;
        public MainWindow()
        {
        
            conDB = new SqlConnection(@"Data Source=DESKTOP-96BNCAA\SQLEXPRESS;Initial Catalog=Hopital;Integrated Security=True");
            InitializeComponent();
            charger_Liste_Medecins();
        }


        public void charger_Liste_Medecins()
        {
            try
            {
                conDB.Open();

                // Création de la commande SQL pour sélectionner les données de la table medecin
                SqlCommand cmd = new SqlCommand("SELECT * FROM medecin", conDB);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Charger les données dans la DataGrid dgMedecin
                dgMedecin.ItemsSource = dataTable.DefaultView;

                conDB.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des médecins : " + ex.Message);
            }
        }



        private void Ligne_Selectionnee(object sender, SelectionChangedEventArgs e)
        {
            if (dgMedecin.SelectedItem != null)
            {
                DataRowView row = (DataRowView)dgMedecin.SelectedItem;

                tbIDMedecin.Text = row["MedecinID"].ToString();
                tbPrenom.Text = row["Prenom"].ToString();
                tbnom.Text = row["Nom"].ToString();
                tbSpecialite.Text = row["Specialite"].ToString();
                tbHopital.Text = row["Hopital"].ToString();
                tbNumeroContact.Text = row["phone"].ToString();
                tbSalaire.Text = row["Salaire"].ToString();
                tbCourriel.Text = row["email"].ToString();
            }
        }


        public bool Verifier_champ()
        {

            return (!string.IsNullOrEmpty(tbPrenom.Text) && !string.IsNullOrEmpty(tbnom.Text) && !string.IsNullOrEmpty(tbNumeroContact.Text) && !string.IsNullOrEmpty(tbCourriel.Text)
                && !string.IsNullOrEmpty(tbSalaire.Text) && !string.IsNullOrEmpty(tbSpecialite.Text) && !string.IsNullOrEmpty(tbHopital.Text));
        }

        public bool Medecin_Existant(int MedID)
        {
            bool medecin_existe = false;

            try
            {
                conDB.Open();

                
                string query = "SELECT COUNT(*) FROM Medecin WHERE MedecinID = @MedID";
                SqlCommand command = new SqlCommand(query, conDB);
                command.Parameters.AddWithValue("@MedID", MedID);

                int count = (int)command.ExecuteScalar(); 

                if (count > 0)
                {
                    medecin_existe = true; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la vérification du médecin : " + ex.Message);
            }
            finally
            {
                conDB.Close();
            }

            return medecin_existe;
        }
        private void btnAjouteur_Med(object sender, RoutedEventArgs e)
        {
            try
            {
              
                if (Verifier_champ())
                {
                 
                    int medID = int.Parse(tbIDMedecin.Text); 
                    bool medecinExiste = Medecin_Existant(medID);

                    if (!medecinExiste)
                    {
                       
                        conDB.Open();

                        string query = "INSERT INTO Medecin (Prenom, Nom, phone, email, Salaire, Specialite, Hopital) VALUES (@Prenom, @Nom, @phone, @email, @Salaire, @Specialite, @Hopital)";
                        SqlCommand command = new SqlCommand(query, conDB);
                        command.Parameters.AddWithValue("@Prenom", tbPrenom.Text);
                        command.Parameters.AddWithValue("@Nom", tbnom.Text);
                        command.Parameters.AddWithValue("@phone", tbNumeroContact.Text);
                        command.Parameters.AddWithValue("@email", tbCourriel.Text);
                        command.Parameters.AddWithValue("@Salaire", int.Parse(tbSalaire.Text)); // Supposant que Salaire est de type int dans la base de données
                        command.Parameters.AddWithValue("@Specialite", tbSpecialite.Text);
                        command.Parameters.AddWithValue("@Hopital", tbHopital.Text);

                        int rowsAffected = command.ExecuteNonQuery(); // Exécute la commande d'insertion

                        if (rowsAffected > 0)
                        {
                            // L'insertion a réussi
                            lbMessage.Content = "Médecin ajouté avec succès.";
                        }
                        else
                        {
                            // Aucune ligne affectée lors de l'insertion
                            lbMessage.Content = "Échec de l'ajout du médecin.";
                        }
                    }
                    else
                    {
                        lbMessage.Content = "Ce médecin existe déjà.";
                    }
                }
                else
                {
                    lbMessage.Content = "Veuillez remplir tous les champs.";
                }
            }
            catch (Exception ex)
            {
                lbMessage.Content = "Erreur lors de l'ajout du médecin : " + ex.Message;
            }
            finally
            {
                conDB.Close();
            }
        }



        private void btnsuprimer_Med(object sender, RoutedEventArgs e)
        {
            try
            {
                int medID = int.Parse(tbIDMedecin.Text);
                bool medecinExiste = Medecin_Existant(medID);

                if (medecinExiste)
                {
                    conDB.Open();

                    string query = "DELETE FROM Medecin WHERE IDMedecin = @IDMedecin";
                    SqlCommand command = new SqlCommand(query, conDB);
                    command.Parameters.AddWithValue("@IDMedecin", medID);

                    int rowsAffected = command.ExecuteNonQuery(); 

                    if (rowsAffected > 0)
                    {
                        // Suppression réussie
                        lbMessage.Content = "Médecin supprimé avec succès.";
                        charger_Liste_Medecins();
                    }
                    else
                    {
                        // Aucun médecin n'a ete  supprimé
                        lbMessage.Content = "Échec de la suppression du médecin.";
                    }
                }
                else
                {
                    lbMessage.Content = "Ce médecin n'existe pas.";
                }
            }
            catch (Exception ex)
            {
                lbMessage.Content = "Erreur lors de la suppression du médecin : " + ex.Message;
            }
            finally
            {
                conDB.Close();
            }
        }


        private void Superieur(object sender, RoutedEventArgs e)
        {
            if (Salaire_Superieur_A.IsChecked == true) Salaire_Inferieur_A.IsChecked = false;
        }

        private void inferieur(object sender, RoutedEventArgs e)
        {
            if (Salaire_Inferieur_A.IsChecked == true) Salaire_Superieur_A.IsChecked = false;
        }


        private void Salaire_Consulter(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Salaire_Superieur_A.IsChecked == true || Salaire_Inferieur_A.IsChecked == true)
                {
                    string operatorSign = Salaire_Superieur_A.IsChecked == true ? ">" : "<";

                    
                    string query = $"SELECT * FROM Medecin WHERE Salaire {operatorSign} @Salaire";

                    using (SqlCommand cmd = new SqlCommand(query, conDB))
                    {
                        cmd.Parameters.AddWithValue("@Salaire", int.Parse(ctbSalaire.Text));

                        conDB.Open();

                     
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            grille_consulter.ItemsSource = dt.DefaultView;
                        }
                    }
                }
                else
                {
                    lbMessage.Content = " selectionner une option de salaire.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur avec consultation par salaire : " + ex.Message);
            }
            finally
            {
                conDB.Close();
            }
        }

        private void consulter_Nom_Prenom(object sender, RoutedEventArgs e)
            {
                try
                {
                    string query = "SELECT * FROM Medecin WHERE";

                    
                    if (!string.IsNullOrWhiteSpace(ctbNom.Text))
                    {
                        query += $" Nom LIKE @Nom AND";
                    }

                   
                    if (!string.IsNullOrWhiteSpace(ctbPrenom.Text))
                    {
                        query += $" Prenom LIKE @Prenom AND";
                    }

                    
                    if (query.EndsWith(" AND"))
                    {
                        query = query.Substring(0, query.Length - 4);
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conDB))
                    {
                      
                        if (!string.IsNullOrWhiteSpace(ctbNom.Text))
                        {
                            cmd.Parameters.AddWithValue("@Nom", $"%{ctbNom.Text}%");
                        }

                        if (!string.IsNullOrWhiteSpace(ctbPrenom.Text))
                        {
                            cmd.Parameters.AddWithValue("@Prenom", $"%{ctbPrenom.Text}%");
                        }

                        conDB.Open();

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            grille_consulter.ItemsSource = dt.DefaultView;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur loen consultant  par nom et prénom : " + ex.Message);
                }
                finally
                {
                    conDB.Close();
                }
            }
        private void Effacertextboxes()
        {
            tbIDMedecin.Clear();
            tbPrenom.Clear();
            tbnom.Clear();
            tbNumeroContact.Clear();
            tbCourriel.Clear();
            tbSalaire.Clear();
            tbSpecialite.Clear();
            tbHopital.Clear();
        }



        private void tbnModifier_Med(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Verifier_champ() && !string.IsNullOrEmpty(tbIDMedecin.Text))
                {
                    conDB.Open();

                  
                    using (SqlCommand cmd = new SqlCommand("UPDATE Medecin SET Prenom = @Prenom, Nom = @Nom, " +
                                                           "phone = @Phone, email = @Email, Salaire = @Salaire, " +
                                                           "Specialite = @Specialite, Hopital = @Hopital " +
                                                           "WHERE MedecinID = @MedecinID", conDB))
                    {
                        cmd.Parameters.AddWithValue("@Prenom", tbPrenom.Text);
                        cmd.Parameters.AddWithValue("@Nom", tbnom.Text);
                        cmd.Parameters.AddWithValue("@Phone", tbNumeroContact.Text);
                        cmd.Parameters.AddWithValue("@Email", tbCourriel.Text);
                        cmd.Parameters.AddWithValue("@Salaire", int.Parse(tbSalaire.Text));
                        cmd.Parameters.AddWithValue("@Specialite", tbSpecialite.Text);
                        cmd.Parameters.AddWithValue("@Hopital", tbHopital.Text);
                        cmd.Parameters.AddWithValue("@MedecinID", int.Parse(tbIDMedecin.Text));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            lbMessage.Content = "Le médecin  modifiee par succès.";
                        }
                        else
                        {
                            lbMessage.Content = "pas de medecin avec ce id";
                        }
                    }

                    conDB.Close();

                    charger_Liste_Medecins();
                    Effacertextboxes();
                }
                else
                {
                    lbMessage.Content = "choisis un medecin a modifier .";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("il a une erreur pour modification " + ex.Message);
            }
        }

      
       



    }
}