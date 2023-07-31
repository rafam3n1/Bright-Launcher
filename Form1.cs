using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using DiscordRPC;

namespace Bright_Launcher_3
{
    public partial class Form1 : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem menuItem1;
        private ToolStripMenuItem menuItem2;
        private ToolStripMenuItem menuItem3;
        private ToolStripMenuItem menuItem4;
        private bool navigationCompleted = false;

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            // Obtenha a resolução do monitor
            Rectangle screen = Screen.PrimaryScreen.Bounds;

            // Defina a largura e a altura para 50% da resolução
            int width = (int)(screen.Width * 0.7);
            int height = (int)(screen.Height * 0.7);

            // Defina o tamanho do formulário
            this.Size = new Size(width, height);

            // Defina a posição do formulário para que ele esteja centrado na tela
            this.StartPosition = FormStartPosition.CenterScreen;


            InitializeDiscord();
            this.FormClosing += Form1_FormClosing;
            pictureBox2.Click += BrightClient_Click;
            pictureBox3.Click += MicClient_Click;
            pictureBox1.Click += PictureBox1_Click;
            pictureBox4.Click += PictureBox4_Click;
            panel1.MouseDown += PanelTitulo_MouseDown;
            panel1.MouseMove += PanelTitulo_MouseMove;
            panel1.MouseUp += PanelTitulo_MouseUp;
            this.Load += Form1_Load;
            this.Resize += Form1_Resize;

            contextMenuStrip1 = new ContextMenuStrip();
            menuItem1 = new ToolStripMenuItem("Launcher");
            menuItem2 = new ToolStripMenuItem("Microfone");
            menuItem3 = new ToolStripMenuItem("Configurações");
            menuItem4 = new ToolStripMenuItem("Fechar");

            menuItem1.Click += (sender, e) => { this.Show(); this.WindowState = FormWindowState.Normal; };
            menuItem2.Click += (sender, e) => MicClient_Click(sender, e);
            menuItem3.Click += (sender, e) => BrightClient_Click(sender, e);
            menuItem4.Click += (sender, e) => { Application.Exit(); };

            menuItem1.Image = Properties.Resources.icon;
            menuItem2.Image = Properties.Resources.microfone2_16x16;
            menuItem3.Image = Properties.Resources.configuracoes2_16x16;
            menuItem4.Image = Properties.Resources.X_preto;


            contextMenuStrip1.Items.AddRange(new ToolStripMenuItem[] { menuItem1, menuItem2, menuItem3, menuItem4 });

            contextMenuStrip1.Renderer = new MyRenderer();

            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.MouseUp += notifyIcon1_MouseUp;
            notifyIcon1.DoubleClick += (sender, e) => { this.Show(); this.WindowState = FormWindowState.Normal; };

            webView21.CoreWebView2InitializationCompleted += WebView21_CoreWebView2InitializationCompleted;
            webView21.NavigationCompleted += CoreWebView2_NavigationCompleted;
            webView21.NavigationStarting += CoreWebView2_NavigationStarting;
            webView21.Click += WebView21_Click;

            Debug.WriteLine("Form1 constructor completed");
        }

        private class MyRenderer : ToolStripProfessionalRenderer
        {
            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                base.OnRenderToolStripBackground(e);
                e.ToolStrip.BackColor = ColorTranslator.FromHtml("#fff");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int titleBarHeight = SystemInformation.CaptionHeight;
            webView21.Location = new Point(0, titleBarHeight);
            webView21.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - titleBarHeight);

            Timer timer = new Timer
            {
                Interval = 5000, // 5 segundos
                Enabled = true
            };
            timer.Tick += (s, ea) =>
            {
                if (navigationCompleted)
                {
                    webView21.CoreWebView2.Navigate("https://grupobright.com/dashboard/");
                    timer.Stop();
                    timer.Dispose();
                }
            };
            timer.Start();

            Debug.WriteLine("Form1_Load completed");

            ConnectToZeroTier();
        }

        private void WebView21_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webView21.CoreWebView2.Navigate("https://grupobright.com/loading.html");
            }
            else
            {
                MessageBox.Show($"WebView2 initialization failed: {e.InitializationException}");
                Debug.WriteLine($"WebView21_CoreWebView2InitializationCompleted: Failed - {e.InitializationException}");
            }
        }

        private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Cursor.Current = Cursors.Default;
            navigationCompleted = true;
        }

        private void CoreWebView2_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
        }

        private async void WebView21_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            await Task.Delay(2000);
            this.Cursor = Cursors.Default;
            Debug.WriteLine("webView21_Click: WebView clicked");
        }

        private void PanelTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
            Debug.WriteLine("PanelTitulo_MouseDown: Mouse button pressed");
        }

        private void PanelTitulo_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
                Debug.WriteLine("PanelTitulo_MouseMove: Mouse moved");
            }
        }

        private void PanelTitulo_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
            Debug.WriteLine("PanelTitulo_MouseUp: Mouse button released");
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            notifyIcon1.Visible = true;
            notifyIcon1.BalloonTipText = "Bright Launcher minimizado!";
            notifyIcon1.ShowBalloonTip(1000);
            Debug.WriteLine("PictureBox1_Click: Application minimized to tray");
        }

        private void PictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.Xbranco;
        }

        private void PictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.X; 
        }


        private void PictureBox4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            Debug.WriteLine("PictureBox4_Click: Application minimized");
        }

        private void PictureBox5_Click(object sender, EventArgs e)
        {
            webView21.CoreWebView2.Navigate("http://grupobright.com/minha-conta");
        }

        private void PictureBox6_Click(object sender, EventArgs e)
        {
            sobre sobreForm = new sobre(); // Cria uma nova instância de Sobre
            sobreForm.ShowDialog(); // Mostra o Sobre como um dialog modal.
        }


        //zerotier
        public void ConnectToZeroTier()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/c zerotier-cli join 8bd5124fd6092278"; // Substitua "yourNetworkId" pela ID da sua rede
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Debug.WriteLine("ZeroTier joined successfully.");
                }
                else
                {
                    Debug.WriteLine($"ZeroTier join failed with exit code: {process.ExitCode}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An error occurred while joining ZeroTier: {e.Message}");
            }
        }


        private DiscordRpcClient client;

        // Adicione isto ao método OnLoad ou ao método do construtor após InitializeComponent()
        private void InitializeDiscord()
        {
            client = new DiscordRpcClient("1134959085679292526");
            client.Initialize();
            client.SetPresence(new RichPresence()
            {
                Details = "Jogando na Bright",
                State = "www.grupobright.com",
                Timestamps = Timestamps.Now,
                Assets = new Assets()
                {
                    LargeImageKey = "capa", // Substitua por uma imagem do seu aplicativo
                    LargeImageText = "BrightApp", // Substitua pelo nome do seu jogo
                }
            });
        }

        // Adicione isso ao método de evento FormClosing para desligar corretamente o cliente.
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Deinitialize();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            int titleBarHeight = SystemInformation.CaptionHeight;
            webView21.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - titleBarHeight);

            if (this.WindowState == FormWindowState.Normal)
            {
                notifyIcon1.Visible = true;
                this.ShowInTaskbar = true;
            }

            Debug.WriteLine("Form1_Resize: Form resized");
        }

        private void BrightClient_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"C:\Program Files\Bright App\streaming\Bright Stream\settings.exe");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"BrightClient_Click: Error launching Bright Client - {ex}");
            }
        }

        private void MicClient_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"C:\Program Files\Bright App\streaming\Bright Stream\mic-client.exe");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MicClient_Click: Error launching Mic Client - {ex}");
            }
        }

        private void notifyIcon1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon1, null);
            }
            Debug.WriteLine("notifyIcon1_MouseUp: NotifyIcon mouse button pressed");
        }
    }


}

