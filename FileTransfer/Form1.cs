using SimpleTCP;
using System.Text;
using System.Windows.Forms;

namespace FileTransfer {
    public partial class Form1 : Form {
        bool odabrano = false;
        string putanja = "";
        List<byte[]> datoteke = new List<byte[]>();
        List<string> nazivi = new List<string>();

        public void server_start() {
            var server = new SimpleTcpServer();

            server.DataReceived += (sender, e) => {
                var ep = e.TcpClient.Client.RemoteEndPoint;
                var msg = Encoding.UTF8.GetString(e.Data);
                //MessageBox.Show($"Received message from {ep}: \"{msg}\"");

                string naziv = "";
                byte[] content = new byte[20000];
                bool predjeno = false;
                int broj = 0;
                for(int i = 0; i < msg.Length; i++) {
                    if (msg[i] == '|') {
                        predjeno = true;
                        broj = i;
                    } else if (predjeno == false)
                        naziv += msg[i];
                    else
                        content[i-broj] = e.Data[i];
                }

                MessageBox.Show($"{msg}");

                listBox1.Items.Add(ep + " | " + naziv);
                nazivi.Add(naziv);
                datoteke.Add(content);
            };

            server.Start(5000);
        }

        public Form1() {
            InitializeComponent();
            server_start();

            this.Text = "FileTransfer";

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e) {

        }

        private void button1_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog();

            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.Title = "Odaberite datoteku koju želite poslati";

            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            label_odabrano_putanja.Text = Path.GetFileName(openFileDialog1.FileName);
            putanja = openFileDialog1.FileName;
            odabrano = true;

            label_izbrisi.Visible = true;
            button2.Enabled = true;
        }

        private void label_izbrisi_Click(object sender, EventArgs e) {
            label_odabrano_putanja.Text = "";
            putanja = "";
            odabrano = false;
            button2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e) {
            try {
                var client = new SimpleTcpClient();
                client.Connect(textBox1.Text, int.Parse(textBox2.Text));
                client.Write(Encoding.UTF8.GetBytes(Path.GetFileName(putanja) + "|" + File.ReadAllText(putanja, Encoding.Default)));
            } catch {
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            if (listBox1.SelectedIndex >= 0) {
                SaveFileDialog SaveFileDialog1 = new SaveFileDialog();

                SaveFileDialog1.Filter = "All files (*.*) | *.*";
                SaveFileDialog1.FileName = nazivi[listBox1.SelectedIndex];
                SaveFileDialog1.InitialDirectory = @"C:\";
                SaveFileDialog1.RestoreDirectory = true;

                if(SaveFileDialog1.ShowDialog() == DialogResult.OK) {
                    Stream s = File.Open(SaveFileDialog1.FileName, FileMode.CreateNew);
                    BinaryWriter sw = new BinaryWriter(s);
                    sw.Write(datoteke[listBox1.SelectedIndex]);
                }

                //MessageBox.Show(Encoding.UTF8.GetString(datoteke[listBox1.SelectedIndex]));
            }
        }
    }
}