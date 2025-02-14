using Newtonsoft.Json.Linq;

namespace CatImages
{
    public partial class CatsPopup : Form
    {
        private readonly PictureBox pictureBox;
        private readonly System.Windows.Forms.Timer timer;
        private readonly NotifyIcon notifyIcon;
        public CatsPopup()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(300, 200);
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.BackColor = Color.White;
            this.Opacity = 0.9;
            this.Icon = new Icon("paw.ico");

            // Obtener la resolución de pantalla y colocar la ventana en la esquina inferior derecha
            var screen = Screen.PrimaryScreen!.WorkingArea;
            this.Location = new Point(screen.Width - this.Width - 10, screen.Height - this.Height - 10);

            pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill
            };

            this.Controls.Add(pictureBox);

            // Configurar el Timer
            timer = new System.Windows.Forms.Timer
            {
                Interval = 5000 // 5000 ms = 5 segundos
            };
            timer.Tick += (sender, e) => CargarImagen(pictureBox);
            timer.Start();

            // Cargar la imagen al iniciar el formulario
            CargarImagen(pictureBox);

            // Configurar el NotifyIcon para la bandeja del sistema
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon("paw.ico"),
                Visible = true,
                Text = "Imagen de Gato"
            };


            // Restaurar el Form al hacer doble clic en el icono de la bandeja
            notifyIcon.DoubleClick += (sender, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            };

            // Menú contextual para el icono de la bandeja
            ContextMenuStrip contextMenu = new();
            ToolStripMenuItem exitMenuItem = new("Salir");
            exitMenuItem.Click += (sender, e) => Application.Exit();
            contextMenu.Items.Add(exitMenuItem);
            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private static async void CargarImagen(PictureBox pictureBox)
        {
            string apiUrl = "https://api.thecatapi.com/v1/images/search";

            using HttpClient client = new();
            try
            {
                string response = await client.GetStringAsync(apiUrl);

                // Parsear el JSON
                JArray jsonArray = JArray.Parse(response);
                string imageUrl = jsonArray[0]["url"]!.ToString();

                // Asignar la URL de la imagen al PictureBox
                pictureBox.Load(imageUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener la imagen: {ex.Message}");
            }
        }

        // Evento para minimizar a la bandeja en lugar de cerrar
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                notifyIcon.Visible = false;
            }
        }


    }
}
