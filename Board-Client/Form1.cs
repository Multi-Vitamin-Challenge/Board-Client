using System.Threading;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;

namespace Board_Client
{
    public partial class Form1 : Form
    {
        BindingList<Team> teams;
        List<Team> temp;
        private static string url;

        public Form1()
        {

            InitializeComponent();
            temp = new List<Team>();
            teams = new BindingList<Team>(temp);
            dataGridView1.DataSource = teams;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            url = "http://" + textBox1.Text + "/board/score";
            updateAsync();

        }


        private async Task updateAsync()
        {
            var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            while (await periodicTimer.WaitForNextTickAsync())
            {
                var wb = new WebClient();
                var resp = wb.DownloadString(url);
                var resp2 = JsonConvert.DeserializeObject<Dictionary<string, int>>(resp);
                teams.Clear();
                foreach (var x in resp2.Keys)
                {
                    teams.Add(new Team(x, resp2[x]));
                }
                temp.Sort((Team X, Team Y) => (-X.Score).CompareTo(-Y.Score));
                teams.ResetBindings();
                int i = 1;
                teams[0].Rate = i;
                for (int j = 1; j < teams.Count; j++)
                {
                    if (teams[j].Score < teams[j - 1].Score) i++;
                    teams[j].Rate = i;
                }
            }

        }
    }
    class Team
    {
        public Team(string Name, int Score)
        {
            this.Name = Name;
            this.Score = Score;
        }
        public int Rate { set; get; }
        public string Name { get; set; }
        public int Score { get; set; }
    }
}