using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.IO;
using bibliotecaProyecto;

namespace ProyectoTP
{
    public partial class InterfazBot : Form
    {
        string cadenaRecibida;
        Bot bot;
        public InterfazBot()
        {
            InitializeComponent();
            bot = new Bot(puertoArduino);
            bot.prender();
            System.Windows.Forms.DataVisualization.Charting.Chart.CheckForIllegalCrossThreadCalls = false;
            sidePanel.Top = btnConexion.Top;
            panelDatos.Visible = false;
            panelConexion.Visible = true;
            panelComandos.Visible = false;
            
        }

        private void btnConexion_Click(object sender, EventArgs e)
        {
            sidePanel.Height = btnConexion.Height;
            sidePanel.Top = btnConexion.Top;
            panelDatos.Visible = false;
            panelConexion.Visible = true;
            panelComandos.Visible = false;

        }

        private void btnDatos_Click(object sender, EventArgs e)
        {
            sidePanel.Height = btnDatos.Height;
            sidePanel.Top = btnDatos.Top;
            panelDatos.Visible = true;
            panelConexion.Visible = false;
            panelComandos.Visible = false;


        }

        private void btnComandos_Click(object sender, EventArgs e)
        {
            sidePanel.Height = btnComandos.Height;
            sidePanel.Top = btnComandos.Top;
            panelDatos.Visible = false;
            panelConexion.Visible = false;
            panelComandos.Visible = true;

        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            string[] NombrePuertos = SerialPort.GetPortNames();
            cbPuertos.Items.Clear();
            foreach (string Nombre in NombrePuertos)
                cbPuertos.Items.Add(Nombre);
        }

        private void cbPuertos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                puertoArduino.PortName = cbPuertos.SelectedItem.ToString();
            }
            catch { }
            
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            try
            {
                puertoArduino.Open();
                MessageBox.Show("Puerto abierto.");
                reloj.Start();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDesconectar_Click(object sender, EventArgs e)
        {
            if (puertoArduino.IsOpen)
            {
                puertoArduino.Close();
                MessageBox.Show("Puerto cerrado.");
                reloj.Stop();
            }
            else
                MessageBox.Show("El puerto está cerrado");
            
        }

        private void puertoArduino_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(50);
                cadenaRecibida = puertoArduino.ReadExisting();
            }
            catch{}         
        }

        private void reloj_Tick(object sender, EventArgs e)
        {
            string[] datos = cadenaRecibida.Split(',');
            Lecturas.pot[Lecturas.pot.Length - 1] = Convert.ToInt32(datos[1]);
            Lecturas.prox[Lecturas.prox.Length - 1] = Convert.ToInt32(datos[0]);
            Lecturas.ilum[Lecturas.ilum.Length - 1] = Convert.ToInt32(datos[2]);

            if(Lecturas.ilum[Lecturas.ilum.Length - 1] < 40 && Lecturas.estadoLuces)
            {
                bot.prenderTodos();
                Lecturas.estadoLuces = false;
            }
            if (Lecturas.ilum[Lecturas.ilum.Length - 1] > 40 && !Lecturas.estadoLuces)
            {
                bot.apagarTodo();
                Lecturas.estadoLuces = true;
            }

            if (Lecturas.prox[Lecturas.prox.Length - 1] == 1 && !Lecturas.estadoProxi)
            {
                bot.mandarMensaje("Estoy detectando movimiento!");

                Lecturas.estadoProxi = true;
            }
            if (Lecturas.prox[Lecturas.prox.Length - 1] == 0 && Lecturas.estadoProxi)
            {
                bot.mandarMensaje("Ya no detecto movimiento..");
                Lecturas.estadoProxi = false;
            }

            Array.Copy(Lecturas.pot, 1, Lecturas.pot, 0, Lecturas.pot.Length - 1);
            Array.Copy(Lecturas.prox, 1, Lecturas.prox, 0, Lecturas.prox.Length - 1);
            Array.Copy(Lecturas.ilum, 1, Lecturas.ilum, 0, Lecturas.ilum.Length - 1);
            
            if (chartPot.IsHandleCreated)
            {
                chartIluminacion.Series["Series1"].Points.Clear();
                chartProximidad.Series["Series1"].Points.Clear();
                chartPot.Series["Series1"].Points.Clear();
                for (int i = 0; i < Lecturas.pot.Length - 1; ++i)
                {
                    chartIluminacion.Series["Series1"].Points.AddY(Lecturas.ilum[i]);
                    chartPot.Series["Series1"].Points.AddY(Lecturas.pot[i]);
                    chartProximidad.Series["Series1"].Points.AddY(Lecturas.prox[i]); 
                }
            }
            else { }
  
        }
        private void btnConectarBot_Click(object sender, EventArgs e)
        {
            bot.agregarChat(txtBot.Text);
        }

        private void btnDesconectarBot_Click(object sender, EventArgs e)
        {
            bot.eliminarChat(txtBot.Text);
        }
    }
}
