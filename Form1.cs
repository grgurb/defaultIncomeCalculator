using RacunanjeZatezneKamateConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZateznaKamataGUI
{
    public partial class Form1 : Form
    {
        ArrayList valuteStope = null;
        KamatneStopeValutePeriodi stope = null;
        string dugovanjaLolakacija;
        List<Dugovanje> listaDugovanja = new List<Dugovanje>();
        List<string[]> prikazListeDugovanja = new List<string[]>();
        decimal svaDugovanja = 0;
        public Form1()
        {
            InitializeComponent();

            try
            {
                valuteStope = KamatneStopeValutePeriodi.nizStopaZaSveValute();
                dataGridView1.ColumnCount = 6;
                dataGridView1.Columns[0].Name = "Šifra";
                dataGridView1.Columns[1].Name = "Datum";
                dataGridView1.Columns[2].Name = "Broj dana";
                dataGridView1.Columns[3].Name = "Glavnica";
                dataGridView1.Columns[4].Name = "Kamata";
                dataGridView1.Columns[5].Name = "Celokupno Dugovanje";
                /*Program treba da prvo u linkedlist ucita sve stope, zatim da naknadno ubaci prve dane u godini*/
                //Main
                //Test
                //decimal R = Dugovanje.DugovanjeNaDanUplateP(new Dugovanje[] { new Dugovanje("AX1", "170000", "12.05.2015.", "01.02.2021."), new Dugovanje("AX2", "170000", "12.05.2015.", "01.02.2021."), new Dugovanje("AX3", "21025", "13.05.2013.", "01.01.2017.") }, stope);

                foreach (KamatneStopeValutePeriodi valuta in valuteStope)
                {
                    comboBox1.Items.Add(valuta.Valuta);
                }
                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch(Exception greska)
            {
                MessageBox.Show(greska.Message);
                Environment.Exit(-1);
            }
        }
        private void odabirFajla_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    dugovanjaLolakacija = openFileDialog1.FileName;
                    label1.Text = $"Lokacija fajla: {dugovanjaLolakacija}";
                    proracunSvihDugovanja.Enabled = true;
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void proracunSvihDugovanja_Click(object sender, EventArgs e)
        {
            prikazListeDugovanja.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.ColumnCount = 6;
            dataGridView1.Columns[0].Name = "Šifra";
            dataGridView1.Columns[1].Name = "Rok";
            dataGridView1.Columns[2].Name = "Datum plaćanja";
            dataGridView1.Columns[3].Name = "Glavnica";
            dataGridView1.Columns[4].Name = "Kamata";
            dataGridView1.Columns[5].Name = "Celokupno Dugovanje";
            try
            {
                listaDugovanja = Dugovanje.ucitavanjeDugovanja(dugovanjaLolakacija);
                prikazPoSifri.Enabled = true;
                prikazSvihDugovanja.Enabled = true;
                foreach (Dugovanje dugovanje in listaDugovanja)
                {
                    if (dugovanje.Rok < stope.Stope.First.Value.DatumStupanjaNaSnagu)
                    {
                        throw new ArgumentException($"Rok za uplatu dugovanja sa šifrom {dugovanje.SifraDugovanja} je stariji od stupanja na snagu prve definisane stope za valutu {stope.Valuta}.");
                    }
                    decimal dug = dugovanje.DugovanjeNaDanUplate(stope, false);
                    svaDugovanja += dug;
                    prikazListeDugovanja.Add(new string[] { dugovanje.SifraDugovanja, PomocneFunkcije.Datum(dugovanje.Rok), PomocneFunkcije.Datum(dugovanje.DanUplate).ToString(), dugovanje.Glavnica.ToString(), (dug - dugovanje.Glavnica).ToString(), dug.ToString() });
                }
                label2.Text = $"Sva dugovanja: {svaDugovanja}";
                svaDugovanja = 0;
                foreach (string[] red in prikazListeDugovanja)
                {
                    dataGridView1.Rows.Add(red);
                }
                Dugovanje.detalji = new List<string[]>();
            }
            catch (Exception ex)
            {
                label2.Text = "Sva dugovanja: ";
                MessageBox.Show(ex.Message);
            }
        }
        private void prikazSvihDugovanja_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.ColumnCount = 6;
            dataGridView1.Columns[0].Name = "Šifra";
            dataGridView1.Columns[1].Name = "Datum";
            dataGridView1.Columns[2].Name = "Broj dana";
            dataGridView1.Columns[3].Name = "Glavnica";
            dataGridView1.Columns[4].Name = "Kamata";
            dataGridView1.Columns[5].Name = "Celokupno Dugovanje";
            foreach (string[] red in prikazListeDugovanja)
            {
                dataGridView1.Rows.Add(red);
            }
            Dugovanje.detalji = new List<string[]>();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (KamatneStopeValutePeriodi x in valuteStope)
                {
                    Debug.WriteLine(x);
                    if (x.Valuta == comboBox1.Text)
                    {
                        stope = x;
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                prikazPoSifri_click(this, new EventArgs());
            }
        }

        private void prikazPoSifri_click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "Datum";
            dataGridView1.Columns[1].Name = "Broj dana";
            dataGridView1.Columns[2].Name = "Kamatna Stopa";
            dataGridView1.Columns[3].Name = "Glavnica";
            dataGridView1.Columns[4].Name = "Kamata";
            bool nadjeno = false;
            foreach (Dugovanje dugovanje in listaDugovanja)
            {
                if (dugovanje.SifraDugovanja.ToLower() == textBox1.Text.ToLower())
                {
                    dugovanje.DugovanjeNaDanUplate(stope, true);
                    nadjeno = true;
                    label6.Text = "";
                }
            }
            if (nadjeno)
            {
                foreach (string[] red in Dugovanje.detalji)
                {
                    dataGridView1.Rows.Add(red);
                }
                Dugovanje.detalji = new List<string[]>();
            }
            else
            {
                label6.Text = "Dugovanje sa zadatom šifrom nije pronađeno.";
            }
        }
    }
}
