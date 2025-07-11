using DLWMS.Data;
using DLWMS.Data.IspitIB230306;
using DLWMS.Infrastructure;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Internal;
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
    public partial class frmStipendijaAddEditIB230306 : Form
    {
        private StudentiStipendijeIB230306 studentstipendija;
        DLWMSContext db = new DLWMSContext();

        public frmStipendijaAddEditIB230306(StudentiStipendijeIB230306 studentstipendija)
        {
            InitializeComponent();
            this.studentstipendija = studentstipendija;
        }

        private void frmStipendijaAddEditIB230306_Load(object sender, EventArgs e)
        {
            db.Studenti.ToList();
            db.StipendijeIB230306.ToList();
            db.StipendijeGodineIB230306.ToList();
            db.StudentiStipendijeIB230306.ToList();

            if (studentstipendija != null)
            {
                comboBox1.DataSource = db.Studenti.Where(s => s.Id == studentstipendija.StudentId).ToList();
                comboBox2.SelectedIndex = 0;
                int god = int.Parse(comboBox2.SelectedItem as string);
                comboBox2.SelectedItem = studentstipendija.StipendijaGodina.Godina.ToString();
                comboBox3.DataSource = db.StipendijeIB230306.Where(s => db.StipendijeGodineIB230306.Any(sg => sg.Godina == god && sg.StipendijaId == s.Id)).ToList();
                comboBox3.SelectedItem = studentstipendija.StipendijaGodina.Stipendija;
            }
            else
            {
                comboBox1.DataSource = db.Studenti.ToList();
                comboBox2.SelectedIndex = 0;
                int god = int.Parse(comboBox2.SelectedItem as string);
                comboBox3.DataSource = db.StipendijeIB230306.Where(s => db.StipendijeGodineIB230306.Any(sg => sg.Godina == god && sg.StipendijaId == s.Id)).ToList();

            }
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            comboBox3.DataSource = null;
            int god = int.Parse(comboBox2.SelectedItem as string);
            comboBox3.DataSource = db.StipendijeIB230306.Where(s => db.StipendijeGodineIB230306.Any(sg => sg.Godina == god && sg.StipendijaId == s.Id)).ToList();
        }

        private void button1_Click(object sender, EventArgs e)
        {   
            if (studentstipendija != null)
            {

                int god = int.Parse(comboBox2.SelectedItem as string);
                var stip = comboBox3.SelectedItem as StipendijeIB230306;

                var stipgo = db.StipendijeGodineIB230306.Where(sg => sg.Godina == god && sg.StipendijaId == stip.Id).ToList().First();
                if (db.StudentiStipendijeIB230306.Any(s => s.StipendijaGodinaId == stipgo.Id && s.StudentId == studentstipendija.StudentId))
                {
                    MessageBox.Show("Student vec ima ovu stipendiju.");
                    return;
                }

                var ss = db.StudentiStipendijeIB230306.Where(s => s.Id == studentstipendija.Id).ToList().First();
                ss.StipendijaGodinaId = stipgo.Id;
                db.StudentiStipendijeIB230306.Update(ss);
                db.SaveChanges();
                Close();
            }

            if (studentstipendija == null)
            {
                if (comboBox3.SelectedItem != null)
                {

                    var stu = comboBox1.SelectedItem as Student;
                    int god = int.Parse(comboBox2.SelectedItem as string);
                    var stip = comboBox3.SelectedItem as StipendijeIB230306;
                    var stipgo = db.StipendijeGodineIB230306.Where(sg => sg.Godina == god && sg.StipendijaId == stip.Id).ToList().First();
                    if (db.StudentiStipendijeIB230306.Any(s=>s.StipendijaGodinaId==stipgo.Id && s.StudentId==stu.Id))
                    {
                        MessageBox.Show("Student vec ima ovu stipendiju.");
                        return;
                    }    

                    var nova = new StudentiStipendijeIB230306()
                    {
                        StudentId = stu.Id,
                        StipendijaGodinaId = stipgo.Id
                    };
                    db.StudentiStipendijeIB230306.Add(nova);
                    db.SaveChanges();
                    Close();
                }
            }
        }
    }
}
