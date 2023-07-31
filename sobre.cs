using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bright_Launcher_3
{
    public partial class sobre : Form
    {
        private Label label;

        public sobre()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            // Cria um novo Label
            label = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Bright Launcher\nv1.0.0\nhttps://grupobright.com/"
            };

            // Adiciona o Label ao Form
            this.Controls.Add(label);

            // Adiciona o evento de clique ao fechar2
            fechar2.Click += Fechar2_Click;
        }

        private void Fechar2_Click(object sender, EventArgs e)
        {
            this.Close(); // Fecha o formulário
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
