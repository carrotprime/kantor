using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace CurrencyApp
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			LoadRates();
		}

		public class Rate
		{
			public string code { get; set; }
			public double mid { get; set; }
		}

		public class ExchangeRatesTable
		{
			public List<Rate> rates { get; set; }
		}

		private List<Rate> rates;

		private async void LoadRates()
		{
			try
			{
				using HttpClient client = new HttpClient();
				string url = "https://api.nbp.pl/api/exchangerates/tables/A?format=json";

				var response = await client.GetStringAsync(url);
				var data = JsonSerializer.Deserialize<List<ExchangeRatesTable>>(response);

				rates = data[0].rates;

				cmbFrom.Items.Add("PLN");
				cmbTo.Items.Add("PLN");

				foreach (var r in rates)
				{
					cmbFrom.Items.Add(r.code);
					cmbTo.Items.Add(r.code);
				}

				cmbFrom.SelectedItem = "PLN";
				cmbTo.SelectedItem = "EUR";
			}
			catch
			{
				MessageBox.Show("Błąd pobierania kursów z NBP");
			}
		}

		private void Convert_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				double amount = double.Parse(txtAmount.Text);

				double fromRate = GetRate(cmbFrom.Text);
				double toRate = GetRate(cmbTo.Text);

				double result = amount * (fromRate / toRate);

				lblResult.Text = $"{cmbTo.Text}: {result:F2}";
			}
			catch
			{
				MessageBox.Show("Wpisz poprawną kwotę!");
			}
		}

		private double GetRate(string code)
		{
			if (code == "PLN") return 1;

			return rates.FirstOrDefault(r => r.code == code)?.mid ?? 1;
		}
	}
}