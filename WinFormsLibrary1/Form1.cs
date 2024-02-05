using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomComboBox.Controls;
using System.Windows.Forms;

namespace WinFormsLibrary1
{
    public partial class Form1 : Form
    {
        private IList<FrutasGrupos> listaFrutas { get; set; }
        public Form1()
        {
            InitializeComponent();
            CarregarLista();
        }

        private void CarregarLista()
        {
            // Crie a lista de FrutasGrupos
            List<FrutasGrupos> listaFrutas = new List<FrutasGrupos>();

            // Adicione as frutas à lista
            listaFrutas.Add(new FrutasGrupos { ID = 1, Nome = "Maçã", Grupo = "Vermelho" });
            listaFrutas.Add(new FrutasGrupos { ID = 11, Nome = "Maçã2", Grupo = "Vermelho" });
            listaFrutas.Add(new FrutasGrupos { ID = 14, Nome = "Maçã3", Grupo = "Vermelho" });
            listaFrutas.Add(new FrutasGrupos { ID = 2, Nome = "Tomate", Grupo = "Vermelho" });
            listaFrutas.Add(new FrutasGrupos { ID = 35, Nome = "Pera", Grupo = "Verde" });
            listaFrutas.Add(new FrutasGrupos { ID = 36, Nome = "Pera2", Grupo = "Verde" });
            listaFrutas.Add(new FrutasGrupos { ID = 3, Nome = "Pera3", Grupo = "Verde" });
            listaFrutas.Add(new FrutasGrupos { ID = 4, Nome = "Limão", Grupo = "Verde" });

            //Standard
            this.comboBox1.DataSource = null;
            this.comboBox1.Items.Clear();

            this.comboBox1.ValueMember = "ID";
            this.comboBox1.DisplayMember = "Nome";
            this.comboBox1.GroupMember = "Grupo";
            this.comboBox1.DataSource = listaFrutas;

            //Test
            this.groupedComboBox21.DisplayMember = "Nome";
            this.groupedComboBox21.GroupMember = "Grupo";
            this.groupedComboBox21.DataSource = listaFrutas;

            //Custom
            this.modernuiGroupedComboBox1.DisplayMember = "Nome";
            this.modernuiGroupedComboBox1.GroupMember = "Grupo";
            this.modernuiGroupedComboBox1.DataSource = listaFrutas;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void groupedComboBox21_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
