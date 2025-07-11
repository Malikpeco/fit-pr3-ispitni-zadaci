using DLWMS.Data;
using DLWMS.Data.IspitIB230306;
using DLWMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLWMS.WinApp.IspitIB230306
{
    public partial class frmStipendijeIB230306 : Form
    {
        DLWMSContext db = new DLWMSContext();
        List<StipendijeGodineIB230306> stipendijegodine = new List<StipendijeGodineIB230306>();
        public frmStipendijeIB230306()
        {
            InitializeComponent();
        }

        private void frmStipendijeIB230306_Load(object sender, EventArgs e)
        {
            db.Studenti.ToList();
            db.StipendijeIB230306.ToList();
            db.StipendijeGodineIB230306.ToList();
            db.StudentiStipendijeIB230306.ToList();
            dataGridView1.AutoGenerateColumns = false;

            comboBox1.SelectedIndex = 0;
            comboBox2.DataSource = db.StipendijeIB230306.ToList();


            stipendijegodine = db.StipendijeGodineIB230306.ToList();

            ucitajpodatke();

        }

        private void ucitajpodatke()
        {
            stipendijegodine = db.StipendijeGodineIB230306.ToList();



            var tabela = new DataTable();
            tabela.Columns.Add("Godina");
            tabela.Columns.Add("Stipendija");
            tabela.Columns.Add("MjesecniIznos");
            tabela.Columns.Add("UkupniIznos");
            tabela.Columns.Add("Aktivna");

            for (int i = 0; i < stipendijegodine.Count; i++)
            {
                var sg = stipendijegodine[i];
                var red = tabela.NewRow();
                red["Godina"] = sg.Godina.ToString();
                red["Stipendija"] = sg.Stipendija;
                red["MjesecniIznos"] = sg.Iznos.ToString();
                red["UkupniIznos"] = sg.Godina == DateTime.Now.Year ? (DateTime.Now.Month * (sg.Iznos)).ToString() : ((12 * (DateTime.Now.Year - sg.Godina)) * sg.Iznos).ToString();
                red["Aktivna"] = sg.Status.ToString();
                tabela.Rows.Add(red);
            }
            dataGridView1.DataSource = tabela;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int godina = int.Parse(comboBox1.SelectedItem as string);
            var stip = comboBox2.SelectedItem as StipendijeIB230306;
            try
            {
                int.Parse(textBox1.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Unesite validan iznos");
                return;
            }


            if (db.StipendijeGodineIB230306.Any(sg => sg.Godina == godina && sg.StipendijaId == stip.Id))
            {
                MessageBox.Show("Stipendija vec postoji");

                return;
            }

            var nova = new StipendijeGodineIB230306()
            {
                Godina = godina,
                StipendijaId = stip.Id,
                Iznos = int.Parse(textBox1.Text),
                Status = true
            };
            db.StipendijeGodineIB230306.Add(nova);
            db.SaveChanges();
            ucitajpodatke();

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                stipendijegodine[e.RowIndex].Status = !stipendijegodine[e.RowIndex].Status;
                db.StipendijeGodineIB230306.Update(stipendijegodine[e.RowIndex]);
                db.SaveChanges();
                ucitajpodatke();
            }
        }

        async private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                var red = dataGridView1.SelectedRows[0];
                var stipgod = db.StipendijeGodineIB230306.Where(sg => sg.Godina == int.Parse(red.Cells[0].Value as string)
                && sg.Stipendija.Naziv == (red.Cells[1].Value as string)).ToList().First() as StipendijeGodineIB230306;

                var studenti = db.Studenti.Where(s => !db.StudentiStipendijeIB230306.Any(ss => ss.StipendijaGodinaId == stipgod.Id && ss.StudentId == s.Id)).ToList();
                textBox2.Text = "";
                Thread t = new Thread(() =>
                {
                    dodajthread(stipgod,studenti);
                });
                t.Start();
            }
        }

        private void dodajthread(StipendijeGodineIB230306 stipgod, List<Student> studenti)
        {
            for (int i = 0; i < studenti.Count; i++)
            {
                var nova = new StudentiStipendijeIB230306()
                {
                    StudentId = studenti[i].Id,
                    StipendijaGodinaId = stipgod.Id,
                };
                db.StudentiStipendijeIB230306.Add(nova);
                db.SaveChanges();

                Action ac = () =>
                {
                    textBox2.Text += $"{i + 1}. {stipgod.Stipendija.Naziv} stipendija u oznosu od {stipgod.Iznos} dodata {studenti[i]}{Environment.NewLine}";
                };
                BeginInvoke(ac);
                Thread.Sleep(300);
            }
            BeginInvoke(ucitajpodatke);
            MessageBox.Show("Generisanje stipendija: USPJESNO.");
        }
    }
}
