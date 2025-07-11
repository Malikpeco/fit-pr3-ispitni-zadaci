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
    public partial class frmPretragaIB230306 : Form
    {
        DLWMSContext db = new DLWMSContext();
        List<StudentiStipendijeIB230306> studentistipendije = new List<StudentiStipendijeIB230306>();
        public frmPretragaIB230306()
        {
            InitializeComponent();
        }

        private void frmPretragaIB230306_Load(object sender, EventArgs e)
        {
            db.StipendijeIB230306.ToList();
            db.StipendijeGodineIB230306.ToList();
            db.Studenti.ToList();
            db.StudentiStipendijeIB230306.ToList();


            dataGridView1.AutoGenerateColumns = false;
            comboBox1.SelectedIndex = 0;
            var god = comboBox1.SelectedItem as string;
            int godina = int.Parse(god);
            comboBox2.DataSource = db.StipendijeIB230306.Where(s => db.StipendijeGodineIB230306.Any(sg => sg.Godina == godina && sg.StipendijaId == s.Id)).ToList();
            ucitajpodatke();
        }

        private void ucitajpodatke()
        {
            var god = comboBox1.SelectedItem as string;
            int godina = int.Parse(god);
            var stip = comboBox2.SelectedItem as StipendijeIB230306;

            if (comboBox2.SelectedItem != null)
            {

                var stipgod = db.StipendijeGodineIB230306.Where(sg => sg.Godina == godina && sg.StipendijaId == stip.Id).ToList().First() as StipendijeGodineIB230306;
                studentistipendije = db.StudentiStipendijeIB230306.Where(ss => ss.StipendijaGodinaId == stipgod.Id).ToList();
                var tabela = new DataTable();
                tabela.Columns.Add("IndeksImeIPrezime");
                tabela.Columns.Add("Godina");
                tabela.Columns.Add("Stipendija");
                tabela.Columns.Add("MjesecniIznos");
                tabela.Columns.Add("Ukupno");
                for (int i = 0; i < studentistipendije.Count; i++)
                {
                    var red = tabela.NewRow();
                    var ss = studentistipendije[i];
                    red["IndeksImeIPrezime"] = ss.Student.ToString();
                    red["Godina"] = ss.StipendijaGodina.Godina.ToString();
                    red["Stipendija"] = ss.StipendijaGodina.Stipendija.ToString();
                    red["MjesecniIznos"] = ss.StipendijaGodina.Iznos.ToString();
                    red["Ukupno"] = godina == DateTime.Now.Year ? (DateTime.Now.Month * (ss.StipendijaGodina.Iznos)).ToString() : ((12 * (DateTime.Now.Year - godina)) * ss.StipendijaGodina.Iznos).ToString();
                    tabela.Rows.Add(red);
                }
                dataGridView1.DataSource = tabela;
                Text = $"Broj prikazanih studenata: {studentistipendije.Count}";
            }

            if (comboBox2.SelectedItem == null || studentistipendije.Count == 0)
            {
                dataGridView1.DataSource = null;
                MessageBox.Show($"U bazi nisu evidentirani studenti kojima je u {god} godini dodijeljena {stip} stipendija");
                Text = "Broj prikazanih studenata: 0";
            }
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var god = comboBox1.SelectedItem as string;
            int godina = int.Parse(god);
            comboBox2.SelectedItem = null;
            comboBox2.DataSource = db.StipendijeIB230306.Where(s => db.StipendijeGodineIB230306.Any(sg => sg.Godina == godina && sg.StipendijaId == s.Id)).ToList();
            ucitajpodatke();
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ucitajpodatke();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5 && e.RowIndex != -1)
            {
                if (MessageBox.Show("Da li zelite obrisati dodijeljenu stipendiju?", " ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    db.StudentiStipendijeIB230306.Remove(studentistipendije[e.RowIndex]);
                    db.SaveChanges();
                    ucitajpodatke();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new frmStipendijaAddEditIB230306(null).ShowDialog();
            ucitajpodatke();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                new frmStipendijaAddEditIB230306(studentistipendije[e.RowIndex]).ShowDialog();
                db.Entry(studentistipendije[e.RowIndex]).Reload();
                ucitajpodatke();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new frmStipendijeIB230306().ShowDialog();
            ucitajpodatke();
        }
    }
}
