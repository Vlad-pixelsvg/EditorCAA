using Microsoft.Win32; // Necessario per SaveFileDialog
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EditorCAA_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InputParola_TextChanged(object sender, TextChangedEventArgs e)
        {
            AggiornaFoglioCAA();
        }

        private void Parametro_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded) AggiornaFoglioCAA();
        }

        private void Align_Click(object sender, RoutedEventArgs e)
        {
            var tag = ((Button)sender).Tag.ToString();
            ContenitoreImmagini.HorizontalAlignment = (tag == "Left") ? HorizontalAlignment.Left : HorizontalAlignment.Center;
            AggiornaFoglioCAA();
        }

        private void AggiornaFoglioCAA()
        {
            if (ContenitoreImmagini == null) return;
            ContenitoreImmagini.Children.Clear();

            string[] parole = InputParola.Text.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string cartellaCAA = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ImmaginiCAA");

            foreach (string p in parole)
            {
                string ricercaFile = p.ToLower().Trim('.', ',', '!', '?', ';', ':');
                string pathImmagine = TrovaImmagine(cartellaCAA, ricercaFile);

                UIElement card = (pathImmagine != null)
                    ? CreaCardImmagine(pathImmagine, p)
                    : CreaCardSoloTesto(p);

                ContenitoreImmagini.Children.Add(card);
            }
        }

        private string TrovaImmagine(string cartella, string parola)
        {
            if (!Directory.Exists(cartella)) return null;
            string png = Path.Combine(cartella, parola + ".png");
            if (File.Exists(png)) return png;
            string jpg = Path.Combine(cartella, parola + ".jpg");
            if (File.Exists(jpg)) return jpg;
            return null;
        }

        private UIElement CreaCardImmagine(string path, string testo)
        {
            string fontNome = (ComboFont.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Arial";
            double fontDim = double.Parse((ComboSize.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "12");

            StackPanel sp = new StackPanel { Margin = new Thickness(5), Width = 110 };
            Border b = new Border { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Height = 110 };

            try
            {
                Image img = new Image { Source = new BitmapImage(new Uri(path)), Stretch = Stretch.Uniform };
                b.Child = img;
            }
            catch { }

            sp.Children.Add(b);

            TextBlock tb = new TextBlock
            {
                Text = testo,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily(fontNome),
                FontSize = fontDim,
                Margin = new Thickness(0, 2, 0, 0)
            };

            // PROTEZIONE MAIUSCOLO E CORSIVO
            tb.SetValue(Typography.CapitalsProperty, FontCapitals.Normal);
            tb.SetValue(TextBlock.FontStyleProperty, FontStyles.Normal);

            sp.Children.Add(tb);
            return sp;
        }

        private UIElement CreaCardSoloTesto(string testo)
        {
            string fontNome = (ComboFont.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Arial";
            double fontDim = double.Parse((ComboSize.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "12");

            StackPanel sp = new StackPanel { Margin = new Thickness(5), Width = 110 };
            Border b = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(1), Height = 110, Background = Brushes.WhiteSmoke };

            sp.Children.Add(b);

            TextBlock tb = new TextBlock
            {
                Text = testo,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontFamily = new FontFamily(fontNome),
                FontSize = fontDim,
                Margin = new Thickness(0, 2, 0, 0)
            };

            tb.SetValue(Typography.CapitalsProperty, FontCapitals.Normal);
            tb.SetValue(TextBlock.FontStyleProperty, FontStyles.Normal);

            sp.Children.Add(tb);
            return sp;
        }

        private void Pulisci_Click(object sender, RoutedEventArgs e)
        {
            InputParola.Clear();
            ContenitoreImmagini.Children.Clear();
        }

        private void Stampa_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true) { EseguiStampa(pd); }
        }

        private void EsportaPDF_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Documento PDF (*.pdf)|*.pdf",
                FileName = "Foglio_CAA.pdf"
            };

            if (sfd.ShowDialog() == true)
            {
                PrintDialog pd = new PrintDialog();
                // Avviso per l'utente su come completare il salvataggio
                MessageBox.Show("Seleziona 'Microsoft Print to PDF' per completare il salvataggio.", "Info PDF");
                if (pd.ShowDialog() == true) { EseguiStampa(pd); }
            }
        }

        private void EseguiStampa(PrintDialog pd)
        {
            InputParola.Visibility = Visibility.Collapsed;
            try
            {
                pd.PrintVisual(AreaFoglio, "Documento CAA");
            }
            finally
            {
                InputParola.Visibility = Visibility.Visible;
            }
        }
    }
}