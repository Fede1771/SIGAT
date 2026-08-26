using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.SERVICIOS;
using System;
using System.Drawing;
using System.Windows.Forms;
using QRCoder;

namespace SIGAT.UI
{
    public partial class FrmValidar2FA : Form
    {
        private Usuario _usuarioPendiente;
        private DosFactoresServicio _servicio2FA = new DosFactoresServicio();
        private BitacoraBLL _bitacoraBLL = new BitacoraBLL();

        private TextBox txtCodigo;
        private Button btnValidar;
        private PictureBox picQR;

        public FrmValidar2FA(Usuario usuario)
        {
            _usuarioPendiente = usuario;

            this.Text = "2FA - Código de Seguridad";
            this.Size = new Size(350, 450); // Agrandamos la ventana para que entre el QR
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ControlBox = false;

            this.Controls.Add(new Label { Text = "Escaneá el QR con Google Authenticator o Authy:", Location = new Point(30, 20), AutoSize = true });

            // Cuadro de imagen para el QR
            picQR = new PictureBox { Location = new Point(65, 50), Size = new Size(200, 200), SizeMode = PictureBoxSizeMode.StretchImage };
            this.Controls.Add(picQR);

            // Dibujamos el QR
            GenerarQR();

            this.Controls.Add(new Label { Text = "Ingrese el código de 6 dígitos:", Location = new Point(85, 270), AutoSize = true });
            txtCodigo = new TextBox { Location = new Point(90, 290), Width = 150, TextAlign = HorizontalAlignment.Center };
            this.Controls.Add(txtCodigo);

            btnValidar = new Button { Text = "Validar", Location = new Point(105, 330), Size = new Size(120, 35) };
            btnValidar.Click += BtnValidar_Click;
            this.AcceptButton = btnValidar;
            this.Controls.Add(btnValidar);
        }

        private void GenerarQR()
        {
            string uri = _servicio2FA.GenerarUriAutenticador(_usuarioPendiente.NombreUsuario);

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    picQR.Image = qrCode.GetGraphic(5);
                }
            }
        }

        private void BtnValidar_Click(object sender, EventArgs e)
        {
            // Pasamos el usuario y el código escrito a la capa de servicios
            if (_servicio2FA.ValidarCodigo(_usuarioPendiente.NombreUsuario, txtCodigo.Text))
            {
                SesionServicio.ObtenerInstancia().IniciarSesion(_usuarioPendiente);
                _bitacoraBLL.Registrar(_usuarioPendiente.NombreUsuario, "Login Exitoso 2FA", "Inicio de sesión validado con App Authenticator.");

                this.Hide();
                new FrmPrincipal().ShowDialog();
                this.Close();
            }
            else
            {
                _bitacoraBLL.Registrar(_usuarioPendiente.NombreUsuario, "2FA Fallido", "Intento fallido de Token TOTP.");
                MessageBox.Show("Código incorrecto o vencido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}